/**
 * feed.js — Feed de posts, curtidas e comentários
 */

let currentFeedMode = 'todos'; // 'todos' | 'amigos'
let currentModalPostId = null;

// IDs dos posts que o usuário já curtiu (rastreado localmente)
const likedPosts = new Set();

function initFeed() {
  const btnPublicar   = document.getElementById('btn-publicar');
  const postInput     = document.getElementById('post-input');
  const sidebarItems  = document.querySelectorAll('.sidebar-nav-item');

  // Publicar post
  btnPublicar.addEventListener('click', handlePublicar);
  postInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) handlePublicar();
  });

  // Sidebar feed toggle
  sidebarItems.forEach(btn => {
    btn.addEventListener('click', () => {
      sidebarItems.forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      currentFeedMode = btn.dataset.feed;
      updateFeedLabel();
      loadFeed();
    });
  });

  // Modal comentários
  document.getElementById('close-modal').addEventListener('click', closeCommentModal);
  document.getElementById('comment-modal').addEventListener('click', (e) => {
    if (e.target === document.getElementById('comment-modal')) closeCommentModal();
  });
  document.getElementById('btn-send-comment').addEventListener('click', handleSendComment);
  document.getElementById('modal-comment-text').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') handleSendComment();
  });

  loadFeed();
  loadAmigosSidebar();
}

function updateFeedLabel() {
  const label = document.getElementById('feed-label');
  if (currentFeedMode === 'todos') {
    label.innerHTML = '<i class="fa-solid fa-globe"></i> <span>Todos os Posts</span>';
  } else {
    label.innerHTML = '<i class="fa-solid fa-user-group"></i> <span>Posts dos Amigos</span>';
  }
}

async function loadFeed() {
  const list = document.getElementById('posts-list');
  list.innerHTML = '<div class="loading-spinner"><i class="fa-solid fa-spinner fa-spin"></i></div>';

  try {
    const posts = currentFeedMode === 'todos'
      ? await ApiPost.listar()
      : await ApiPost.listarAmigos();

    renderPosts(posts || []);
  } catch (err) {
    list.innerHTML = `<p class="empty-msg">Erro ao carregar posts: ${err.message}</p>`;
  }
}

function renderPosts(posts) {
  const list = document.getElementById('posts-list');
  const user = Auth.getUser();

  if (!posts.length) {
    list.innerHTML = '<p class="empty-msg">Nenhum post para exibir.</p>';
    return;
  }

  list.innerHTML = posts.map(post => {
    const isLiked = likedPosts.has(post.id);
    const initial = (post.usuarioNome || 'U')[0].toUpperCase();
    const commentCount = post.comentarios ? post.comentarios.length : 0;

    return `
      <div class="post-card" id="post-${post.id}">
        <div class="post-header">
          <div class="post-avatar">${initial}</div>
          <div>
            <div class="post-author">${escapeHtml(post.usuarioNome || 'Usuário')}</div>
          </div>
        </div>
        <div class="post-body">${escapeHtml(post.titulo)}</div>
        <div class="post-stats">
          <div class="post-stats-likes">
            <i class="fa-solid fa-thumbs-up"></i>
            <span id="likes-count-${post.id}">${post.curtidas}</span>
          </div>
          <span>${commentCount} comentário${commentCount !== 1 ? 's' : ''}</span>
        </div>
        <div class="post-actions">
          <button class="post-action-btn ${isLiked ? 'liked' : ''}" id="btn-like-${post.id}" onclick="toggleCurtida(${post.id})">
            <i class="fa-${isLiked ? 'solid' : 'regular'} fa-thumbs-up"></i>
            Curtir
          </button>
          <button class="post-action-btn" onclick="openCommentModal(${post.id}, '${escapeHtml(post.titulo).replace(/'/g, "\\'")}', ${JSON.stringify(post.comentarios || []).replace(/"/g, '&quot;')})">
            <i class="fa-regular fa-comment"></i>
            Comentar
          </button>
        </div>
      </div>
    `;
  }).join('');
}

async function handlePublicar() {
  const input = document.getElementById('post-input');
  const titulo = input.value.trim();
  if (!titulo) return;

  const btn = document.getElementById('btn-publicar');
  btn.disabled = true;

  try {
    await ApiPost.criar(titulo);
    input.value = '';
    showToast('Post publicado!', 'success');
    loadFeed();
  } catch (err) {
    showToast(err.message, 'error');
  } finally {
    btn.disabled = false;
  }
}

async function toggleCurtida(postId) {
  const btn = document.getElementById(`btn-like-${postId}`);
  const countEl = document.getElementById(`likes-count-${postId}`);
  if (!btn || !countEl) return;

  btn.disabled = true;

  try {
    if (likedPosts.has(postId)) {
      await ApiPost.removerCurtida(postId);
      likedPosts.delete(postId);
      btn.classList.remove('liked');
      btn.innerHTML = '<i class="fa-regular fa-thumbs-up"></i> Curtir';
      countEl.textContent = Math.max(0, parseInt(countEl.textContent) - 1);
    } else {
      await ApiPost.curtir(postId);
      likedPosts.add(postId);
      btn.classList.add('liked');
      btn.innerHTML = '<i class="fa-solid fa-thumbs-up"></i> Curtir';
      countEl.textContent = parseInt(countEl.textContent) + 1;
    }
  } catch (err) {
    showToast(err.message, 'error');
  } finally {
    btn.disabled = false;
  }
}

// ─── Comment Modal ────────────────────────────────────────────────────────────
function openCommentModal(postId, titulo, comentarios) {
  currentModalPostId = postId;
  document.getElementById('modal-post-title').textContent = titulo;
  renderModalComments(comentarios);
  document.getElementById('comment-modal').classList.remove('hidden');
  document.getElementById('modal-comment-text').focus();
}

function closeCommentModal() {
  document.getElementById('comment-modal').classList.add('hidden');
  document.getElementById('modal-comment-text').value = '';
  currentModalPostId = null;
}

function renderModalComments(comentarios) {
  const list = document.getElementById('modal-comments-list');

  if (!comentarios || !comentarios.length) {
    list.innerHTML = '<p class="empty-msg">Nenhum comentário ainda. Seja o primeiro!</p>';
    return;
  }

  list.innerHTML = comentarios.map(c => {
    const initial = (c.usuarioNome || 'U')[0].toUpperCase();
    const date = c.dtComentario ? new Date(c.dtComentario).toLocaleString('pt-BR') : '';
    return `
      <div class="comment-item">
        <div class="comment-avatar">${initial}</div>
        <div class="comment-bubble">
          <strong>${escapeHtml(c.usuarioNome || 'Usuário')}</strong>
          <p>${escapeHtml(c.descricao)}</p>
          <small>${date}</small>
        </div>
      </div>
    `;
  }).join('');
}

async function handleSendComment() {
  const input = document.getElementById('modal-comment-text');
  const descricao = input.value.trim();
  if (!descricao || !currentModalPostId) return;

  const btn = document.getElementById('btn-send-comment');
  btn.disabled = true;

  try {
    await ApiPost.comentar(currentModalPostId, descricao);
    input.value = '';
    showToast('Comentário enviado!', 'success');

    // Recarrega o post para atualizar comentários
    const posts = currentFeedMode === 'todos'
      ? await ApiPost.listar()
      : await ApiPost.listarAmigos();

    const post = (posts || []).find(p => p.id === currentModalPostId);
    if (post) {
      renderModalComments(post.comentarios || []);
      // Atualiza contagem no card
      const card = document.getElementById(`post-${currentModalPostId}`);
      if (card) {
        const statsEl = card.querySelector('.post-stats span:last-child');
        if (statsEl) {
          const count = post.comentarios.length;
          statsEl.textContent = `${count} comentário${count !== 1 ? 's' : ''}`;
        }
      }
    }
  } catch (err) {
    showToast(err.message, 'error');
  } finally {
    btn.disabled = false;
  }
}

// ─── Sidebar amigos ───────────────────────────────────────────────────────────
async function loadAmigosSidebar() {
  const el = document.getElementById('amigos-sidebar-list');
  try {
    const amizades = await ApiAmizade.listar();
    if (!amizades || !amizades.length) {
      el.innerHTML = '<p class="empty-msg">Nenhum amigo ainda.</p>';
      return;
    }
    el.innerHTML = amizades.map(a => `
      <div class="sidebar-friend-item">
        <div class="friend-avatar-sm">${(a.amigoNome || 'A')[0].toUpperCase()}</div>
        <span>${escapeHtml(a.amigoNome || 'Amigo')}</span>
      </div>
    `).join('');
  } catch {
    el.innerHTML = '<p class="empty-msg">Erro ao carregar.</p>';
  }
}
