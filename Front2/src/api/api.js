import axios from 'axios';

const API_BASE = 'http://localhost:5289';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('rs_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('rs_token');
      localStorage.removeItem('rs_user');
      window.location.href = '/login';
    }
    return Promise.reject(err);
  }
);

// ── Auth ──────────────────────────────────────────────────────────────────────
export const authApi = {
  login:        (email, senha)       => api.post('/api/usuario/autenticar', { email, senha }),
  registrar:    (nome, email, senha) => api.post('/api/usuario/Registrar', { nome, email, senha }),
  me:           ()                   => api.get('/api/usuario/Autenticado'),
  buscarPorNome:(nome)               => api.get('/api/usuario/nome', { params: { nome } }),
};

// ── Perfil ────────────────────────────────────────────────────────────────────
export const perfilApi = {
  obter: (usuarioId) =>
    api.get(`/api/usuario/perfil/${usuarioId}`),
  atualizar: (descricaoPerfil, fotoPerfilBase64, fotoBannerBase64) =>
    api.put('/api/usuario/perfil', { descricaoPerfil, fotoPerfilBase64, fotoBannerBase64 }),
  trocarSenha: (senhaAtual, novaSenha) =>
    api.put('/api/usuario/senha', { senhaAtual, novaSenha }),
};

// ── Posts ─────────────────────────────────────────────────────────────────────
export const postApi = {
  listar:          ()                            => api.get('/api/post'),           // todos + amigos
  listarMeus:      ()                            => api.get('/api/post/meus'),      // só meus
  listarAmigos:    ()                            => api.get('/api/post/PostsAmigos'),
  listarDoUsuario: (usuarioId)                   => api.get(`/api/post/usuario/${usuarioId}`),
  criar:           (titulo, imagemBase64 = null) => api.post('/api/post', { titulo, comentarios: [], imagemBase64 }),
  comentar:        (postId, descricao)           => api.post('/api/post/comentar', { postId, descricao }),
  curtir:          (postId)                      => api.post(`/api/post/${postId}/curtir`),
  removerCurtida:  (postId)                      => api.delete(`/api/post/${postId}/removerCurtida`),
};

// ── Amizade ───────────────────────────────────────────────────────────────────
export const amizadeApi = {
  listar:  ()         => api.get('/api/amizade'),
  aceitar: (pedidoId) => api.post('/api/amizade/aceitar', { pedidoId }),
  remover: (id)       => api.delete(`/api/amizade/Remover/${id}`),
};

// ── Amizade Pendente ──────────────────────────────────────────────────────────
export const pendenteApi = {
  enviar:    (destinatarioId) => api.post('/api/amizadependente', { destinatarioId }),
  recebidos: ()               => api.get('/api/amizadependente/Recebidos'),
  enviados:  ()               => api.get('/api/amizadependente/enviados'),
  rejeitar:  (id)             => api.delete(`/api/amizadependente/Enviados/${id}`),
};

export default api;
