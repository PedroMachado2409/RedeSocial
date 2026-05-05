import { useState } from 'react';
import Avatar from '../../components/shared/Avatar';
import CommentModal from './CommentModal';
import styles from './PostCard.module.css';

export default function PostCard({ post, liked, onToggleLike, onComment, currentUser }) {
  const [modalOpen,   setModalOpen]   = useState(false);
  const [likeLoading, setLikeLoading] = useState(false);

  async function handleLike() {
    if (likeLoading) return;
    setLikeLoading(true);
    await onToggleLike(post.id);
    setLikeLoading(false);
  }

  const commentCount = post.comentarios?.length ?? 0;
  const imgSrc = post.imagemBase64
    ? (post.imagemBase64.startsWith('data:') ? post.imagemBase64 : `data:image/jpeg;base64,${post.imagemBase64}`)
    : null;

  return (
    <>
      <div className={styles.card}>
        {/* Header */}
        <div className={styles.header}>
          <Avatar
            name={post.usuarioNome}
            src={post.usuarioFotoPerfilBase64 ?? null}
            size={42}
          />
          <div className={styles.headerInfo}>
            <span className={styles.author}>{post.usuarioNome || 'Usuário'}</span>
          </div>
        </div>

        {/* Body */}
        <div className={styles.body}>
          <p>{post.titulo}</p>
        </div>

        {/* Imagem do post */}
        {imgSrc && (
          <img src={imgSrc} alt="Imagem do post" className={styles.postImg} />
        )}

        {/* Stats */}
        {(post.curtidas > 0 || commentCount > 0) && (
          <div className={styles.stats}>
            {post.curtidas > 0 && (
              <span className={styles.statsLikes}>
                <span className={styles.likeIcon}>❤️</span>
                {post.curtidas}
              </span>
            )}
            {commentCount > 0 && (
              <button
                className={styles.statsComments}
                onClick={() => setModalOpen(true)}
              >
                {commentCount} comentário{commentCount !== 1 ? 's' : ''}
              </button>
            )}
          </div>
        )}

        {/* Actions */}
        <div className={styles.actions}>
          <button
            className={`${styles.actionBtn} ${liked ? styles.actionLiked : ''}`}
            onClick={handleLike}
            disabled={likeLoading}
          >
            <i className={`fa-${liked ? 'solid' : 'regular'} fa-heart`} />
            <span>Curtir</span>
          </button>
          <button
            className={styles.actionBtn}
            onClick={() => setModalOpen(true)}
          >
            <i className="fa-regular fa-comment" />
            <span>Comentar</span>
          </button>
        </div>
      </div>

      {modalOpen && (
        <CommentModal
          post={post}
          currentUser={currentUser}
          onComment={onComment}
          onClose={() => setModalOpen(false)}
        />
      )}
    </>
  );
}
