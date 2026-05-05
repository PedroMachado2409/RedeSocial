/**
 * app.js — Ponto de entrada principal, navegação e utilitários globais
 */

// ─── Inicialização ────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  if (Auth.isLoggedIn()) {
    startApp();
  } else {
    showAuthPage();
    initAuth();
  }
});

function showAuthPage() {
  document.getElementById('page-auth').classList.remove('hidden');
  document.getElementById('page-app').classList.add('hidden');
}

function startApp() {
  document.getElementById('page-auth').classList.add('hidden');
  document.getElementById('page-app').classList.remove('hidden');

  const user = Auth.getUser();
  if (user) {
    populateUserUI(user);
  }

  initNavigation();
  initFeed();
  initBuscar();
  loadNotifPanel();

  // Carrega amigos na sidebar direita
  loadAmigosSidebar();
}

// ─── Preenche UI com dados do usuário ────────────────────────────────────────
function populateUserUI(user) {
  const initial = (user.nome || 'U')[0].toUpperCase();

  document.getElementById('nav-user-initial').textContent  = initial;
  document.getElementById('sidebar-initial').textContent   = initial;
  document.getElementById('create-initial').textContent    = initial;
  document.getElementById('dropdown-nome').textContent     = user.nome || '';
  document.getElementById('dropdown-email').textContent    = user.email || '';
  document.getElementById('sidebar-nome').textContent      = user.nome || '';
  document.getElementById('sidebar-email-text').textContent = user.email || '';
}

// ─── Navegação por abas ───────────────────────────────────────────────────────
function initNavigation() {
  const tabs = document.querySelectorAll('.nav-tab');

  tabs.forEach(tab => {
    tab.addEventListener('click', () => {
      const target = tab.dataset.tab;
      switchTab(target);
    });
  });

  // Dropdown do avatar
  const avatarBtn  = document.getElementById('nav-avatar-btn');
  const dropdown   = document.getElementById('nav-dropdown');

  avatarBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    dropdown.classList.toggle('hidden');
  });

  document.addEventListener('click', () => dropdown.classList.add('hidden'));

  // Logout
  document.getElementById('btn-logout').addEventListener('click', () => {
    Auth.clear();
    location.reload();
  });

  // Notificações
  const notifBtn   = document.getElementById('btn-notif');
  const notifPanel = document.getElementById('notif-panel');

  notifBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    notifPanel.classList.toggle('hidden');
    if (!notifPanel.classList.contains('hidden')) {
      loadNotifPanel();
    }
  });

  document.getElementById('close-notif').addEventListener('click', () => {
    notifPanel.classList.add('hidden');
  });

  document.addEventListener('click', (e) => {
    if (!notifPanel.contains(e.target) && e.target !== notifBtn) {
      notifPanel.classList.add('hidden');
    }
  });
}

function switchTab(tabName) {
  // Atualiza botões da navbar
  document.querySelectorAll('.nav-tab').forEach(t => {
    t.classList.toggle('active', t.dataset.tab === tabName);
  });

  // Mostra/esconde conteúdo
  document.querySelectorAll('.tab-content').forEach(c => c.classList.add('hidden'));
  document.getElementById(`tab-${tabName}`)?.classList.remove('hidden');

  // Carrega dados da aba
  if (tabName === 'amigos') {
    initAmigos();
  } else if (tabName === 'feed') {
    loadFeed();
    loadAmigosSidebar();
  }
}

// ─── Utilitários globais ──────────────────────────────────────────────────────

/**
 * Exibe um toast de notificação
 * @param {string} msg
 * @param {'success'|'error'|''} type
 */
function showToast(msg, type = '') {
  const toast = document.getElementById('toast');
  toast.textContent = msg;
  toast.className = `toast ${type}`;
  toast.classList.remove('hidden');

  clearTimeout(toast._timer);
  toast._timer = setTimeout(() => toast.classList.add('hidden'), 3500);
}

/**
 * Escapa HTML para evitar XSS
 */
function escapeHtml(str) {
  if (!str) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
}

/**
 * Formata data ISO para pt-BR
 */
function formatDate(iso) {
  if (!iso) return '';
  try {
    return new Date(iso).toLocaleDateString('pt-BR', {
      day: '2-digit', month: '2-digit', year: 'numeric'
    });
  } catch {
    return iso;
  }
}
