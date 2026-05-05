import { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../../context/AuthContext';
import { postApi, amizadeApi } from '../../api/api';
import { getErrorMsg } from '../../utils/helpers';
import { useLikedPosts } from '../../hooks/useLikedPosts';
import CreatePost from './CreatePost';
import PostCard from './PostCard';
import SidebarLeft from './SidebarLeft';
import SidebarRight from './SidebarRight';
import Spinner from '../../components/shared/Spinner';
import EmptyState from '../../components/shared/EmptyState';
import Toast from '../../components/shared/Toast';
import { useToast } from '../../hooks/useToast';
import styles from './Feed.module.css';

export default function Feed() {
  const { user } = useAuth();
  const { toast, showToast } = useToast();

  const [feedMode, setFeedMode] = useState('todos');
  const [posts, setPosts] = useState([]);
  const [loadingPosts, setLoadingPosts] = useState(true);
  const [amigos, setAmigos] = useState([]);
  const { likedPosts, addLike, removeLike, isLiked, setLikedPosts } = useLikedPosts();

  const loadPosts = useCallback(async () => {
    setLoadingPosts(true);
    try {
      let data;
      if (feedMode === 'todos') {
        // Todos os posts: próprios + amigos (novo endpoint)
        ({ data } = await postApi.listar());
      } else {
        // Só meus posts
        ({ data } = await postApi.listarMeus());
      }
      setPosts(data || []);
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
      setPosts([]);
    } finally {
      setLoadingPosts(false);
    }
  }, [feedMode]);

  const loadAmigos = useCallback(async () => {
    try {
      const { data } = await amizadeApi.listar();
      setAmigos(data || []);
    } catch {
      setAmigos([]);
    }
  }, []);

  useEffect(() => { loadPosts(); }, [loadPosts]);
  useEffect(() => { loadAmigos(); }, [loadAmigos]);

  function handleFeedMode(mode) {
    if (mode === feedMode) return;
    setFeedMode(mode);
  }

  async function handleNewPost(titulo, imagemBase64 = null) {
    try {
      await postApi.criar(titulo, imagemBase64);
      showToast('Post publicado!', 'success');
      loadPosts();
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    }
  }

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

      // API disse "já curtiu" mas estado local dizia não curtido → corrige
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

  async function handleComment(postId, descricao) {
    try {
      await postApi.comentar(postId, descricao);
      showToast('Comentário enviado!', 'success');
      let data;
      if (feedMode === 'todos') {
        ({ data } = await postApi.listar());
      } else {
        ({ data } = await postApi.listarMeus());
      }
      setPosts(data || []);
      return true;
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
      return false;
    }
  }

  return (
    <div className={styles.feedLayout}>
      <SidebarLeft
        user={user}
        feedMode={feedMode}
        onFeedMode={handleFeedMode}
      />

      <section className={styles.feedCenter}>
        <CreatePost user={user} onPost={handleNewPost} />

        <div className={styles.feedLabel}>
          {feedMode === 'todos' ? (
            <><i className="fa-solid fa-globe" /> Todos os Posts</>
          ) : (
            <><i className="fa-solid fa-user" /> Meus Posts</>
          )}
        </div>

        {loadingPosts ? (
          <Spinner />
        ) : posts.length === 0 ? (
          <EmptyState
            icon="📝"
            text={
              feedMode === 'amigos'
                ? 'Seus amigos ainda não publicaram nada.'
                : 'Nenhum post ainda. Seja o primeiro!'
            }
          />
        ) : (
          posts.map((post) => (
            <PostCard
              key={post.id}
              post={post}
              liked={isLiked(post.id)}
              onToggleLike={handleToggleLike}
              onComment={handleComment}
              currentUser={user}
            />
          ))
        )}
      </section>

      <SidebarRight amigos={amigos} />

      <Toast toast={toast} />
    </div>
  );
}
