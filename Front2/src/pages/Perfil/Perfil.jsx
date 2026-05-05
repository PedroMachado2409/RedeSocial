import { useState, useEffect, useRef, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { perfilApi, postApi, amizadeApi, authApi } from '../../api/api';
import { getErrorMsg, formatDateTime, avatarColor } from '../../utils/helpers';
import { useLikedPosts } from '../../hooks/useLikedPosts';
import Avatar from '../../components/shared/Avatar';
import Toast from '../../components/shared/Toast';
import Spinner from '../../components/shared/Spinner';
import { useToast } from '../../hooks/useToast';
import styles from './Perfil.module.css';

function toDataUrl(base64) {
  if (!base64) return null;
  if (base64.startsWith('data:')) return base64;
  return `data:image/jpeg;base64,${base64}`;
}

function fileToBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload  = () => resolve(reader.result.split(',')[1]);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

export default function Perfil() {
  const { id } = useParams();           // presente em /perfil/:id, ausente em /perfil
  const { user: authUser, atualizarFotoPerfil } = useAuth();
  const { toast, showToast } = useToast();
  const navigate = useNavigate();

  // ── Resolve o ID do usuário alvo ──────────────────────────────
  // authUser pode não ter .id se o login foi feito antes dessa versão.
  // Nesse caso buscamos via /api/usuario/Autenticado.
  const [resolvedId,   setResolvedId]   = useState(id ? Number(id) : authUser?.id ?? null);
  const [resolving,    setResolving]    = useState(!resolvedId); // true enquanto busca o id

  useEffect(() => {
    // Se já temos o id (via URL ou authUser.id), não precisa resolver
    if (id) {
      setResolvedId(Number(id));
      setResolving(false);
      return;
    }
    if (authUser?.id) {
      setResolvedId(authUser.id);
      setResolving(false);
      return;
    }
    // authUser.id ausente → busca via API
    setResolving(true);
    authApi.me()
      .then(({ data }) => {
        setResolvedId(data.id);
      })
      .catch(() => {
        setResolvedId(null);
      })
      .finally(() => setResolving(false));
  }, [id, authUser?.id]);

  const isOwner = !id || (resolvedId && authUser?.id && String(authUser.id) === String(id));

  // ── Estado principal ──────────────────────────────────────────
  const [perfil,        setPerfil]        = useState(null);
  const [posts,         setPosts]         = useState([]);
  const [amigos,        setAmigos]        = useState([]);
  const [loadingPerfil, setLoadingPerfil] = useState(true);
  const [loadingPosts,  setLoadingPosts]  = useState(true);

  // Curtidas: persistidas no sessionStorage via hook
  const { isLiked, addLike, removeLike } = useLikedPosts();

  // ── Edição de perfil ──────────────────────────────────────────
  const [editMode,      setEditMode]      = useState(false);
  const [descricao,     setDescricao]     = useState('');
  const [fotoPerfilB64, setFotoPerfilB64] = useState(null);
  const [fotoBannerB64, setFotoBannerB64] = useState(null);
  const [saving,        setSaving]        = useState(false);

  // ── Troca de senha ────────────────────────────────────────────
  const [senhaModal,   setSenhaModal]   = useState(false);
  const [senhaAtual,   setSenhaAtual]   = useState('');
  const [novaSenha,    setNovaSenha]    = useState('');
  const [senhaLoading, setSenhaLoading] = useState(false);

  const inputFotoRef   = useRef(null);
  const inputBannerRef = useRef(null);

  // ── Carrega dados quando resolvedId estiver pronto ────────────
  const loadPerfil = useCallback(async () => {
    if (!resolvedId) return;
    setLoadingPerfil(true);
    try {
      const { data } = await perfilApi.obter(resolvedId);
      setPerfil(data);
      setDescricao(data.descricaoPerfil || '');
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
      setPerfil(null);
    } finally {
      setLoadingPerfil(false);
    }
  }, [resolvedId]);

  const loadPosts = useCallback(async () => {
    if (!resolvedId) return;
    setLoadingPosts(true);
    try {
      const { data } = await postApi.listarDoUsuario(resolvedId);
      setPosts(data || []);
    } catch {
      setPosts([]);
    } finally {
      setLoadingPosts(false);
    }
  }, [resolvedId]);

  const loadAmigos = useCallback(async () => {
    try {
      const { data } = await amizadeApi.listar();
      setAmigos(data || []);
    } catch {
      setAmigos([]);
    }
  }, []);

  useEffect(() => {
    if (resolving || !resolvedId) return;
    loadPerfil();
    loadPosts();
    if (isOwner) loadAmigos();
  }, [resolving, resolvedId, loadPerfil, loadPosts, loadAmigos, isOwner]);

  // ── Curtidas ──────────────────────────────────────────────────
  async function handleToggleLike(postId) {
    const liked = isLiked(postId);

    // Optimistic update
    liked ? removeLike(postId) : addLike(postId);
    setPosts(prev =>
      prev.map(p => p.id === postId
        ? { ...p, curtidas: p.curtidas + (liked ? -1 : 1) }
        : p
      )
    );

    try {
      if (liked) {
        await postApi.removerCurtida(postId);
      } else {
        await postApi.curtir(postId);
      }
    } catch (err) {
      const msg = getErrorMsg(err);
      if (!liked && msg.toLowerCase().includes('já curtiu')) {
        addLike(postId);
        return;
      }
      // Reverte
      liked ? addLike(postId) : removeLike(postId);
      setPosts(prev =>
        prev.map(p => p.id === postId
          ? { ...p, curtidas: p.curtidas + (liked ? 1 : -1) }
          : p
        )
      );
      showToast(msg, 'error');
    }
  }

  // ── Upload de imagens ─────────────────────────────────────────
  async function handleFotoPerfilChange(e) {
    const file = e.target.files?.[0];
    if (!file) return;
    if (file.size > 2 * 1024 * 1024) { showToast('Máximo 2 MB.', 'error'); return; }
    setFotoPerfilB64(await fileToBase64(file));
  }

  async function handleBannerChange(e) {
    const file = e.target.files?.[0];
    if (!file) return;
    if (file.size > 2 * 1024 * 1024) { showToast('Máximo 2 MB.', 'error'); return; }
    setFotoBannerB64(await fileToBase64(file));
  }

  // ── Salvar perfil ─────────────────────────────────────────────
  async function handleSalvarPerfil() {
    setSaving(true);
    try {
      const { data } = await perfilApi.atualizar(descricao, fotoPerfilB64, fotoBannerB64);
      setPerfil(data);
      setFotoPerfilB64(null);
      setFotoBannerB64(null);
      setEditMode(false);
      showToast('Perfil atualizado!', 'success');
      // Sincroniza foto no contexto global (Navbar, Sidebar, etc.)
      if (data.fotoPerfilBase64) {
        atualizarFotoPerfil(data.fotoPerfilBase64);
      }
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setSaving(false);
    }
  }

  function handleCancelarEdicao() {
    setEditMode(false);
    setFotoPerfilB64(null);
    setFotoBannerB64(null);
    setDescricao(perfil?.descricaoPerfil || '');
  }

  // ── Trocar senha ──────────────────────────────────────────────
  async function handleTrocarSenha(e) {
    e.preventDefault();
    if (!senhaAtual || !novaSenha) return;
    setSenhaLoading(true);
    try {
      await perfilApi.trocarSenha(senhaAtual, novaSenha);
      showToast('Senha alterada com sucesso!', 'success');
      setSenhaModal(false);
      setSenhaAtual('');
      setNovaSenha('');
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setSenhaLoading(false);
    }
  }

  // ── Render ────────────────────────────────────────────────────
  if (resolving || loadingPerfil) {
    return <div style={{ paddingTop: 80 }}><Spinner /></div>;
  }

  if (!perfil) {
    return (
      <div style={{ textAlign: 'center', paddingTop: 80, color: '#65676b' }}>
        <i className="fa-solid fa-user-slash" style={{ fontSize: 40, display: 'block', marginBottom: 12 }} />
        <p>Perfil não encontrado.</p>
      </div>
    );
  }

  const bannerSrc   = fotoBannerB64
    ? `data:image/jpeg;base64,${fotoBannerB64}`
    : toDataUrl(perfil.fotoBannerBase64);

  const fotoPerfSrc = fotoPerfilB64
    ? `data:image/jpeg;base64,${fotoPerfilB64}`
    : toDataUrl(perfil.fotoPerfilBase64);

  const avatarBg = avatarColor(perfil.nome);
  const initial  = (perfil.nome || 'U')[0].toUpperCase();

  return (
    <div className={styles.page}>

      {/* ── BANNER ── */}
      <div className={styles.bannerWrapper}>
        {bannerSrc
          ? <img src={bannerSrc} alt="Banner" className={styles.bannerImg} />
          : <div className={styles.bannerPlaceholder} />
        }
        {isOwner && editMode && (
          <>
            <button className={styles.bannerEditBtn} onClick={() => inputBannerRef.current?.click()}>
              <i className="fa-solid fa-camera" /> Trocar banner
            </button>
            <input ref={inputBannerRef} type="file" accept="image/*"
              style={{ display: 'none' }} onChange={handleBannerChange} />
          </>
        )}
      </div>

      {/* ── HEADER ── */}
      {/* O avatarArea sobe sobre o banner via margin-top negativo.
          O restante (nome, info, botões) fica ABAIXO do banner, sem sobreposição. */}
      <div className={styles.profileHeaderOuter}>
        {/* Linha do avatar — sobrepõe o banner */}
        <div className={styles.profileAvatarRow}>
          <div className={styles.avatarArea}>
            <div className={styles.avatarRing}>
              {fotoPerfSrc
                ? <img src={fotoPerfSrc} alt="Foto de perfil" className={styles.avatarImg} />
                : <div className={styles.avatarFallback} style={{ background: avatarBg }}>{initial}</div>
              }
            </div>
            {isOwner && editMode && (
              <>
                <button className={styles.avatarEditBtn} onClick={() => inputFotoRef.current?.click()}>
                  <i className="fa-solid fa-camera" />
                </button>
                <input ref={inputFotoRef} type="file" accept="image/*"
                  style={{ display: 'none' }} onChange={handleFotoPerfilChange} />
              </>
            )}
          </div>
        </div>

        {/* Linha de info — sempre abaixo do banner */}
        <div className={styles.profileInfoRow}>
          <div className={styles.profileInfo}>
            <h1 className={styles.profileName}>{perfil.nome}</h1>
            <p className={styles.profileEmail}>{perfil.email}</p>

            {editMode ? (
              <textarea
                className={styles.descricaoInput}
                value={descricao}
                onChange={(e) => setDescricao(e.target.value)}
                placeholder="Adicione uma descrição..."
                maxLength={300}
                rows={2}
              />
            ) : (
              <p className={styles.descricao}>
                {perfil.descricaoPerfil || (isOwner ? 'Clique em "Editar perfil" para adicionar uma descrição.' : '')}
              </p>
            )}

            <div className={styles.stats}>
              <div className={styles.statItem}>
                <strong>{perfil.totalAmigos}</strong>
                <span>amigos</span>
              </div>
              <div className={styles.statDivider} />
              <div className={styles.statItem}>
                <strong>{perfil.totalPosts}</strong>
                <span>posts</span>
              </div>
            </div>
          </div>

          {isOwner && (
            <div className={styles.profileActions}>
              {editMode ? (
                <>
                  <button className={styles.btnSave} onClick={handleSalvarPerfil} disabled={saving}>
                    {saving
                      ? <><i className="fa-solid fa-spinner fa-spin" /> Salvando...</>
                      : <><i className="fa-solid fa-check" /> Salvar alterações</>
                    }
                  </button>
                  <button className={styles.btnCancel} onClick={handleCancelarEdicao}>
                    Cancelar
                  </button>
                </>
              ) : (
                <>
                  <button className={styles.btnEdit} onClick={() => setEditMode(true)}>
                    <i className="fa-solid fa-pen" /> Editar perfil
                  </button>
                  <button className={styles.btnSenha} onClick={() => setSenhaModal(true)}>
                    <i className="fa-solid fa-lock" /> Trocar senha
                  </button>
                </>
              )}
            </div>
          )}
        </div>
      </div>

      <hr className={styles.divider} />

      {/* ── CONTEÚDO ── */}
      <div className={styles.content}>

        {/* Sidebar amigos */}
        <aside className={styles.sideCol}>
          <div className={styles.widget}>
            <h3 className={styles.widgetTitle}>
              <i className="fa-solid fa-user-group" /> Amigos
              <span className={styles.widgetCount}>
                {isOwner ? amigos.length : perfil.totalAmigos}
              </span>
            </h3>
            {isOwner ? (
              amigos.length === 0 ? (
                <p className={styles.emptyMsg}>Nenhum amigo ainda.</p>
              ) : (
                <div className={styles.amigosGrid}>
                  {amigos.slice(0, 9).map((a) => (
                    <div
                      key={a.id}
                      className={styles.amigoCard}
                      onClick={() => navigate(`/perfil/${a.amigoId}`)}
                      title={`Ver perfil de ${a.amigoNome}`}
                    >
                      <Avatar
                        name={a.amigoNome}
                        src={a.amigoFotoPerfilBase64 ?? null}
                        size={80}
                        className={styles.amigoAvatarImg}
                      />
                      <span className={styles.amigoNome}>{a.amigoNome}</span>
                    </div>
                  ))}
                </div>
              )
            ) : (
              <p className={styles.emptyMsg}>
                {perfil.totalAmigos} amigo{perfil.totalAmigos !== 1 ? 's' : ''}
              </p>
            )}
          </div>
        </aside>

        {/* Posts */}
        <section className={styles.postsCol}>
          <h3 className={styles.postsTitle}>
            <i className="fa-solid fa-newspaper" /> Posts
          </h3>
          {loadingPosts ? (
            <Spinner />
          ) : posts.length === 0 ? (
            <div className={styles.emptyPosts}>
              <i className="fa-regular fa-newspaper" />
              <p>Nenhum post ainda.</p>
            </div>
          ) : (
            posts.map((post) => (
              <PostCard
                key={post.id}
                post={post}
                liked={isLiked(post.id)}
                onToggleLike={handleToggleLike}
              />
            ))
          )}
        </section>
      </div>

      {/* ── MODAL TROCAR SENHA ── */}
      {senhaModal && (
        <div className={styles.modalOverlay}
          onClick={(e) => e.target === e.currentTarget && setSenhaModal(false)}>
          <div className={styles.modalBox}>
            <div className={styles.modalHeader}>
              <h3><i className="fa-solid fa-lock" /> Trocar Senha</h3>
              <button className={styles.modalClose} onClick={() => setSenhaModal(false)}>
                <i className="fa-solid fa-xmark" />
              </button>
            </div>
            <form onSubmit={handleTrocarSenha} className={styles.senhaForm}>
              <label className={styles.label}>Senha atual</label>
              <input type="password" className={styles.senhaInput}
                value={senhaAtual} onChange={(e) => setSenhaAtual(e.target.value)}
                placeholder="Digite sua senha atual" autoFocus />
              <label className={styles.label}>Nova senha</label>
              <input type="password" className={styles.senhaInput}
                value={novaSenha} onChange={(e) => setNovaSenha(e.target.value)}
                placeholder="Mínimo 6 caracteres" />
              <button type="submit" className={styles.btnSave}
                disabled={senhaLoading || !senhaAtual || !novaSenha}
                style={{ marginTop: 8 }}>
                {senhaLoading
                  ? <><i className="fa-solid fa-spinner fa-spin" /> Salvando...</>
                  : 'Confirmar troca'
                }
              </button>
            </form>
          </div>
        </div>
      )}

      <Toast toast={toast} />
    </div>
  );
}

// ── PostCard do perfil (com curtida funcional) ────────────────────────────────
function PostCard({ post, liked, onToggleLike }) {
  const [loading, setLoading] = useState(false);
  const imgSrc       = post.imagemBase64 ? toDataUrl(post.imagemBase64) : null;
  const commentCount = post.comentarios?.length ?? 0;

  async function handleLike() {
    if (loading) return;
    setLoading(true);
    await onToggleLike(post.id);
    setLoading(false);
  }

  return (
    <div className={styles.postCard}>
      <div className={styles.postHeader}>
        <Avatar
          name={post.usuarioNome}
          src={post.usuarioFotoPerfilBase64 ?? null}
          size={40}
        />
        <strong className={styles.postAuthor}>{post.usuarioNome}</strong>
      </div>

      <p className={styles.postBody}>{post.titulo}</p>

      {imgSrc && (
        <img src={imgSrc} alt="Imagem do post" className={styles.postImg} />
      )}

      <div className={styles.postFooter}>
        {/* Botão curtir — vermelho quando já curtiu */}
        <button
          className={`${styles.postLikeBtn} ${liked ? styles.postLikeBtnActive : ''}`}
          onClick={handleLike}
          disabled={loading}
          title={liked ? 'Remover curtida' : 'Curtir'}
        >
          <i className={`fa-${liked ? 'solid' : 'regular'} fa-heart`} />
          <span>{post.curtidas}</span>
        </button>

        <span className={styles.postStat}>
          <i className="fa-regular fa-comment" />
          {commentCount} comentário{commentCount !== 1 ? 's' : ''}
        </span>
      </div>
    </div>
  );
}
