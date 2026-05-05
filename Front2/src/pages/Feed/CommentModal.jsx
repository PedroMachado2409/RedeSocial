import { useState, useEffect, useRef } from 'react';
import Avatar from '../../components/shared/Avatar';
import { formatDateTime } from '../../utils/helpers';
import styles from './CommentModal.module.css';

export default function CommentModal({ post, currentUser, onComment, onClose }) {
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(false);
  const [comments, setComments] = useState(post.comentarios || []);
  const inputRef = useRef(null);
  const listRef = useRef(null);

  useEffect(() => {
    inputRef.current?.focus();
    // Scroll to bottom
    if (listRef.current) {
      listRef.current.scrollTop = listRef.current.scrollHeight;
    }
  }, []);

  useEffect(() => {
    if (listRef.current) {
      listRef.current.scrollTop = listRef.current.scrollHeight;
    }
  }, [comments]);

  async function handleSend() {
    const trimmed = text.trim();
    if (!trimmed || loading) return;
    setLoading(true);
    const ok = await onComment(post.id, trimmed);
    if (ok) {
      // Adiciona otimisticamente
      setComments((prev) => [
        ...prev,
        {
          id: Date.now(),
          usuarioNome: currentUser?.nome || 'Você',
          descricao: trimmed,
          dtComentario: new Date().toISOString(),
        },
      ]);
      setText('');
    }
    setLoading(false);
  }

  function handleKeyDown(e) {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  }

  // Fecha ao clicar no overlay
  function handleOverlayClick(e) {
    if (e.target === e.currentTarget) onClose();
  }

  return (
    <div className={styles.overlay} onClick={handleOverlayClick}>
      <div className={styles.modal}>
        {/* Header */}
        <div className={styles.header}>
          <h3>Comentários</h3>
          <button className={styles.closeBtn} onClick={onClose}>
            <i className="fa-solid fa-xmark" />
          </button>
        </div>

        {/* Post preview */}
        <div className={styles.postPreview}>
          <Avatar name={post.usuarioNome} src={post.usuarioFotoPerfilBase64 ?? null} size={36} />
          <div className={styles.postPreviewContent}>
            <strong>{post.usuarioNome}</strong>
            <p>{post.titulo}</p>
          </div>
        </div>

        <hr className={styles.divider} />

        {/* Comments list */}
        <div className={styles.commentsList} ref={listRef}>
          {comments.length === 0 ? (
            <p className={styles.empty}>
              Nenhum comentário ainda. Seja o primeiro!
            </p>
          ) : (
            comments.map((c) => (
              <div key={c.id} className={styles.commentItem}>
                <Avatar name={c.usuarioNome} src={c.usuarioFotoPerfilBase64 ?? null} size={32} />
                <div className={styles.bubble}>
                  <strong>{c.usuarioNome}</strong>
                  <p>{c.descricao}</p>
                  <small>{formatDateTime(c.dtComentario)}</small>
                </div>
              </div>
            ))
          )}
        </div>

        {/* Input */}
        <div className={styles.inputRow}>
          <Avatar name={currentUser?.nome} src={currentUser?.fotoPerfilBase64 ?? null} size={32} />
          <div className={styles.inputWrap}>
            <input
              ref={inputRef}
              type="text"
              placeholder="Escreva um comentário..."
              value={text}
              onChange={(e) => setText(e.target.value)}
              onKeyDown={handleKeyDown}
              className={styles.input}
              disabled={loading}
            />
            <button
              className={styles.sendBtn}
              onClick={handleSend}
              disabled={!text.trim() || loading}
            >
              {loading
                ? <i className="fa-solid fa-spinner fa-spin" />
                : <i className="fa-solid fa-paper-plane" />
              }
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
