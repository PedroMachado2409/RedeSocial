/**
 * api.js — Camada de comunicação com a API RedeSocial
 * Base URL: http://localhost:5000
 */

const API_BASE = 'http://localhost:5289';

// ─── Token helpers ────────────────────────────────────────────────────────────
const Auth = {
  getToken:   ()      => localStorage.getItem('rs_token'),
  setToken:   (t)     => localStorage.setItem('rs_token', t),
  removeToken:()      => localStorage.removeItem('rs_token'),

  getUser:    ()      => { try { return JSON.parse(localStorage.getItem('rs_user')); } catch { return null; } },
  setUser:    (u)     => localStorage.setItem('rs_user', JSON.stringify(u)),
  removeUser: ()      => localStorage.removeItem('rs_user'),

  isLoggedIn: ()      => !!localStorage.getItem('rs_token'),

  clear: () => {
    localStorage.removeItem('rs_token');
    localStorage.removeItem('rs_user');
  }
};

// ─── Core fetch wrapper ───────────────────────────────────────────────────────
async function apiFetch(path, options = {}) {
  const token = Auth.getToken();
  const headers = { 'Content-Type': 'application/json', ...(options.headers || {}) };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (res.status === 401) {
    Auth.clear();
    window.location.reload();
    return;
  }

  // No-content responses
  if (res.status === 204 || res.headers.get('content-length') === '0') return null;

  let data;
  try { data = await res.json(); } catch { data = null; }

  if (!res.ok) {
    const msg = data?.title || data?.message || data?.detail || 'Erro desconhecido';
    throw new Error(msg);
  }

  return data;
}

// ─── Auth endpoints ───────────────────────────────────────────────────────────
const ApiAuth = {
  login: (email, senha) =>
    apiFetch('/api/usuario/autenticar', {
      method: 'POST',
      body: JSON.stringify({ email, senha })
    }),

  registrar: (nome, email, senha) =>
    apiFetch('/api/usuario/Registrar', {
      method: 'POST',
      body: JSON.stringify({ nome, email, senha })
    }),

  me: () => apiFetch('/api/usuario/Autenticado'),

  buscarPorNome: (nome) => apiFetch(`/api/usuario/nome?nome=${encodeURIComponent(nome)}`)
};

// ─── Post endpoints ───────────────────────────────────────────────────────────
const ApiPost = {
  listar:       ()        => apiFetch('/api/post'),
  listarAmigos: ()        => apiFetch('/api/post/PostsAmigos'),
  criar:        (titulo)  => apiFetch('/api/post', { method: 'POST', body: JSON.stringify({ titulo, comentarios: [] }) }),
  comentar:     (postId, descricao) =>
    apiFetch('/api/post/comentar', {
      method: 'POST',
      body: JSON.stringify({ postId, descricao })
    }),
  curtir:        (postId) => apiFetch(`/api/post/${postId}/curtir`, { method: 'POST' }),
  removerCurtida:(postId) => apiFetch(`/api/post/${postId}/removerCurtida`, { method: 'DELETE' })
};

// ─── Amizade endpoints ────────────────────────────────────────────────────────
const ApiAmizade = {
  listar:  ()   => apiFetch('/api/amizade'),
  remover: (id) => apiFetch(`/api/amizade/Remover/${id}`, { method: 'DELETE' }),
  aceitar: (pedidoId) =>
    apiFetch('/api/amizade/aceitar', {
      method: 'POST',
      body: JSON.stringify({ pedidoId })
    })
};

// ─── Amizade Pendente endpoints ───────────────────────────────────────────────
const ApiAmizadePendente = {
  enviar:    (destinatarioId) =>
    apiFetch('/api/amizadependente', {
      method: 'POST',
      body: JSON.stringify({ destinatarioId })
    }),
  recebidos: ()   => apiFetch('/api/amizadependente/Recebidos'),
  enviados:  ()   => apiFetch('/api/amizadependente/enviados'),
  rejeitar:  (id) => apiFetch(`/api/amizadependente/Enviados/${id}`, { method: 'DELETE' })
};
