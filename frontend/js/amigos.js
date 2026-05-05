/**
 * amigos.js — Gerenciamento de amizades
 */

function initAmigos() {
  loadAmigos();
  loadRecebidos();
  loadEnviados();
}

// ─── Amigos confirmados ───────────────────────────────────────────────────────
async function loadAmigos() {
  const el = document.getElementById('amigos-list');
  el.innerHTML = '<div class="loading-spinner"><i class="fa-solid fa-spinner fa-spin"></i></div>';

  try {
    const amizades = await ApiAmizade.listar();
    if (!amizades || !amizades.length) {
      el.innerHTML = '<p class="empty-msg">Você ainda não tem amigos adicionados.</p>';
      return;
    }

    el.innerHTML = amizades.map(a => `
      <div class="friend-card" id="amizade-${a.id}">
        <div class="friend-card-avatar">${(a.amigoNome || 'A')[0].toUpperCase()}</div>
        <div class="friend-card-name">${escapeHtml(a.amigoNome || 'Amigo')}</div>
        <button class="btn-danger" onclick="removerAmizade(${a.id})">
          <i class="fa-solid fa-user-minus"></i> Remover
        </button>
      </div>
    `).join('');
  } catch (err) {
    el.innerHTML = `<p class="empty-msg">Erro: ${err.message}</p>`;
  }
}

async function removerAmizade(id) {
  if (!confirm('Remover esta amizade?')) return;
  try {
    await ApiAmizade.remover(id);
    document.getElementById(`amizade-${id}`)?.remove();
    showToast('Amizade removida.', 'success');
    loadAmigosSidebar();
  } catch (err) {
    showToast(err.message, 'error');
  }
}

// ─── Solicitações recebidas ───────────────────────────────────────────────────
async function loadRecebidos() {
  const el = document.getElementById('recebidos-list');
  el.innerHTML = '<div class="loading-spinner"><i class="fa-solid fa-spinner fa-spin"></i></div>';

  try {
    const pedidos = await ApiAmizadePendente.recebidos();
    renderRecebidos(pedidos || []);
    updateNotifBadge(pedidos ? pedidos.length : 0);
  } catch (err) {
    el.innerHTML = `<p class="empty-msg">Erro: ${err.message}</p>`;
  }
}

function renderRecebidos(pedidos) {
  const el = document.getElementById('recebidos-list');

  if (!pedidos.length) {
    el.innerHTML = '<p class="empty-msg">Nenhuma solicitação recebida.</p>';
    return;
  }

  el.innerHTML = pedidos.map(p => `
    <div class="request-item" id="recebido-${p.id}">
      <div class="request-avatar">${(p.solicitanteNome || 'U')[0].toUpperCase()}</div>
      <div class="request-info">
        <strong>${escapeHtml(p.solicitanteNome || 'Usuário')}</strong>
        <small>${formatDate(p.dataSolicitacao)}</small>
      </div>
      <div class="request-actions">
        <button class="btn-primary" style="padding:6px 14px;font-size:13px;" onclick="aceitarAmizade(${p.id})">
          <i class="fa-solid fa-check"></i> Aceitar
        </button>
        <button class="btn-danger" onclick="rejeitarPedido(${p.id})">
          <i class="fa-solid fa-xmark"></i> Recusar
        </button>
      </div>
    </div>
  `).join('');
}

async function aceitarAmizade(pedidoId) {
  try {
    await ApiAmizade.aceitar(pedidoId);
    document.getElementById(`recebido-${pedidoId}`)?.remove();
    showToast('Amizade aceita!', 'success');
    loadAmigos();
    loadAmigosSidebar();
    // Atualiza badge
    const remaining = document.querySelectorAll('[id^="recebido-"]').length;
    updateNotifBadge(remaining);
    // Atualiza painel de notificações
    loadNotifPanel();
  } catch (err) {
    showToast(err.message, 'error');
  }
}

async function rejeitarPedido(id) {
  try {
    await ApiAmizadePendente.rejeitar(id);
    document.getElementById(`recebido-${id}`)?.remove();
    showToast('Solicitação recusada.', 'success');
    const remaining = document.querySelectorAll('[id^="recebido-"]').length;
    updateNotifBadge(remaining);
    loadNotifPanel();
  } catch (err) {
    showToast(err.message, 'error');
  }
}

// ─── Solicitações enviadas ────────────────────────────────────────────────────
async function loadEnviados() {
  const el = document.getElementById('enviados-list');
  el.innerHTML = '<div class="loading-spinner"><i class="fa-solid fa-spinner fa-spin"></i></div>';

  try {
    const pedidos = await ApiAmizadePendente.enviados();
    if (!pedidos || !pedidos.length) {
      el.innerHTML = '<p class="empty-msg">Nenhuma solicitação enviada.</p>';
      return;
    }

    el.innerHTML = pedidos.map(p => `
      <div class="request-item" id="enviado-${p.id}">
        <div class="request-avatar">${(p.destinatarioNome || 'U')[0].toUpperCase()}</div>
        <div class="request-info">
          <strong>${escapeHtml(p.destinatarioNome || 'Usuário')}</strong>
          <small>Enviado em ${formatDate(p.dataSolicitacao)}</small>
        </div>
        <div class="request-actions">
          <button class="btn-danger" onclick="cancelarEnviado(${p.id})">
            <i class="fa-solid fa-xmark"></i> Cancelar
          </button>
        </div>
      </div>
    `).join('');
  } catch (err) {
    el.innerHTML = `<p class="empty-msg">Erro: ${err.message}</p>`;
  }
}

async function cancelarEnviado(id) {
  try {
    await ApiAmizadePendente.rejeitar(id);
    document.getElementById(`enviado-${id}`)?.remove();
    showToast('Solicitação cancelada.', 'success');
  } catch (err) {
    showToast(err.message, 'error');
  }
}

// ─── Notification panel ───────────────────────────────────────────────────────
async function loadNotifPanel() {
  const list = document.getElementById('notif-list');

  try {
    const pedidos = await ApiAmizadePendente.recebidos();
    updateNotifBadge(pedidos ? pedidos.length : 0);

    if (!pedidos || !pedidos.length) {
      list.innerHTML = '<p class="empty-msg">Nenhuma solicitação pendente.</p>';
      return;
    }

    list.innerHTML = pedidos.map(p => `
      <div class="notif-item" id="notif-${p.id}">
        <div class="notif-avatar">${(p.solicitanteNome || 'U')[0].toUpperCase()}</div>
        <div class="notif-info">
          <strong>${escapeHtml(p.solicitanteNome || 'Usuário')}</strong>
          <small>quer ser seu amigo</small>
        </div>
        <div class="notif-actions">
          <button class="btn-primary" style="padding:6px 12px;font-size:13px;" onclick="aceitarNotif(${p.id})">
            Aceitar
          </button>
          <button class="btn-danger" onclick="rejeitarNotif(${p.id})">
            Recusar
          </button>
        </div>
      </div>
    `).join('');
  } catch {
    list.innerHTML = '<p class="empty-msg">Erro ao carregar.</p>';
  }
}

async function aceitarNotif(pedidoId) {
  try {
    await ApiAmizade.aceitar(pedidoId);
    document.getElementById(`notif-${pedidoId}`)?.remove();
    showToast('Amizade aceita!', 'success');
    loadAmigos();
    loadAmigosSidebar();
    const remaining = document.querySelectorAll('[id^="notif-"]').length;
    updateNotifBadge(remaining);
  } catch (err) {
    showToast(err.message, 'error');
  }
}

async function rejeitarNotif(id) {
  try {
    await ApiAmizadePendente.rejeitar(id);
    document.getElementById(`notif-${id}`)?.remove();
    showToast('Solicitação recusada.', 'success');
    const remaining = document.querySelectorAll('[id^="notif-"]').length;
    updateNotifBadge(remaining);
  } catch (err) {
    showToast(err.message, 'error');
  }
}

function updateNotifBadge(count) {
  const badge = document.getElementById('notif-badge');
  if (count > 0) {
    badge.textContent = count;
    badge.classList.remove('hidden');
  } else {
    badge.classList.add('hidden');
  }
}
