/**
 * buscar.js — Busca de usuários e envio de solicitações
 */

function initBuscar() {
  const btnSearch   = document.getElementById('btn-search');
  const searchInput = document.getElementById('search-input');

  btnSearch.addEventListener('click', handleSearch);
  searchInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') handleSearch();
  });
}

async function handleSearch() {
  const input = document.getElementById('search-input');
  const nome  = input.value.trim();
  const results = document.getElementById('search-results');

  if (!nome) return;

  results.innerHTML = '<div class="loading-spinner"><i class="fa-solid fa-spinner fa-spin"></i></div>';

  try {
    const usuarios = await ApiAuth.buscarPorNome(nome);

    if (!usuarios || !usuarios.length) {
      results.innerHTML = '<p class="empty-msg">Nenhum usuário encontrado.</p>';
      return;
    }

    const user = Auth.getUser();

    results.innerHTML = usuarios.map(u => `
      <div class="search-result-item" id="search-user-${u.id}">
        <div class="search-result-avatar">${(u.nome || 'U')[0].toUpperCase()}</div>
        <div class="search-result-info">
          <strong>${escapeHtml(u.nome)}</strong>
          <small>${escapeHtml(u.email)}</small>
        </div>
        <button class="btn-outline" id="btn-add-${u.id}" onclick="enviarSolicitacao(${u.id})">
          <i class="fa-solid fa-user-plus"></i> Adicionar
        </button>
      </div>
    `).join('');
  } catch (err) {
    results.innerHTML = `<p class="empty-msg">Erro: ${err.message}</p>`;
  }
}

async function enviarSolicitacao(destinatarioId) {
  const btn = document.getElementById(`btn-add-${destinatarioId}`);
  if (!btn) return;

  btn.disabled = true;
  btn.innerHTML = '<i class="fa-solid fa-clock"></i> Enviando...';

  try {
    await ApiAmizadePendente.enviar(destinatarioId);
    btn.innerHTML = '<i class="fa-solid fa-check"></i> Solicitação enviada';
    btn.classList.remove('btn-outline');
    btn.classList.add('btn-ghost');
    btn.disabled = true;
    showToast('Solicitação de amizade enviada!', 'success');
    // Atualiza lista de enviados se estiver na aba amigos
    loadEnviados();
  } catch (err) {
    btn.disabled = false;
    btn.innerHTML = '<i class="fa-solid fa-user-plus"></i> Adicionar';
    showToast(err.message, 'error');
  }
}
