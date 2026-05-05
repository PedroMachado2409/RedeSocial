import { useState, useRef } from 'react';
import Avatar from '../../components/shared/Avatar';
import styles from './CreatePost.module.css';

const MAX_SIZE_MB = 5;

function fileToBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload  = () => resolve(reader.result.split(',')[1]);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

export default function CreatePost({ user, onPost }) {
  const [expanded,  setExpanded]  = useState(false);
  const [text,      setText]      = useState('');
  const [imagem,    setImagem]     = useState(null);   // base64 sem prefixo
  const [imgPreview,setImgPreview] = useState(null);   // data-url para preview
  const [imgError,  setImgError]   = useState('');
  const [loading,   setLoading]    = useState(false);

  const textareaRef = useRef(null);
  const fileInputRef = useRef(null);

  function handleExpand() {
    setExpanded(true);
    setTimeout(() => textareaRef.current?.focus(), 50);
  }

  function handleCancel() {
    setExpanded(false);
    setText('');
    setImagem(null);
    setImgPreview(null);
    setImgError('');
  }

  async function handleImageChange(e) {
    const file = e.target.files?.[0];
    if (!file) return;

    if (file.size > MAX_SIZE_MB * 1024 * 1024) {
      setImgError(`A imagem deve ter no máximo ${MAX_SIZE_MB} MB.`);
      e.target.value = '';
      return;
    }

    setImgError('');
    const b64 = await fileToBase64(file);
    setImagem(b64);
    setImgPreview(`data:${file.type};base64,${b64}`);
  }

  function handleRemoveImage() {
    setImagem(null);
    setImgPreview(null);
    setImgError('');
    if (fileInputRef.current) fileInputRef.current.value = '';
  }

  async function handleSubmit() {
    const trimmed = text.trim();
    if (!trimmed) return;
    setLoading(true);
    try {
      await onPost(trimmed, imagem ?? null);
      setText('');
      setImagem(null);
      setImgPreview(null);
      setImgError('');
      setExpanded(false);
    } finally {
      setLoading(false);
    }
  }

  function handleKeyDown(e) {
    if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) handleSubmit();
    if (e.key === 'Escape') handleCancel();
  }

  return (
    <div className={styles.card}>
      {/* Linha do avatar + trigger */}
      <div className={styles.topRow}>
        <Avatar name={user?.nome} src={user?.fotoPerfilBase64 ?? null} size={40} />
        <button className={styles.trigger} onClick={handleExpand}>
          No que você está pensando, {user?.nome?.split(' ')[0]}?
        </button>
      </div>

      {expanded && (
        <>
          <hr className={styles.divider} />

          {/* Textarea */}
          <textarea
            ref={textareaRef}
            className={styles.textarea}
            placeholder="No que você está pensando?"
            value={text}
            onChange={(e) => setText(e.target.value)}
            onKeyDown={handleKeyDown}
            maxLength={500}
            rows={3}
          />

          {/* Preview da imagem */}
          {imgPreview && (
            <div className={styles.imgPreviewWrapper}>
              <img src={imgPreview} alt="Preview" className={styles.imgPreview} />
              <button
                className={styles.imgRemoveBtn}
                onClick={handleRemoveImage}
                title="Remover imagem"
              >
                <i className="fa-solid fa-xmark" />
              </button>
            </div>
          )}

          {imgError && <p className={styles.imgError}>{imgError}</p>}

          {/* Barra de ações */}
          <div className={styles.footer}>
            <div className={styles.footerLeft}>
              {/* Botão adicionar foto */}
              <button
                className={styles.btnAddPhoto}
                onClick={() => fileInputRef.current?.click()}
                title="Adicionar foto"
                type="button"
              >
                <i className="fa-regular fa-image" />
                <span>Foto</span>
              </button>
              <input
                ref={fileInputRef}
                type="file"
                accept="image/*"
                style={{ display: 'none' }}
                onChange={handleImageChange}
              />
              <span className={`${styles.charCount} ${text.length > 450 ? styles.charWarn : ''}`}>
                {text.length}/500
              </span>
            </div>

            <div className={styles.footerBtns}>
              <button className={styles.btnCancel} onClick={handleCancel} type="button">
                Cancelar
              </button>
              <button
                className={styles.btnPost}
                onClick={handleSubmit}
                disabled={!text.trim() || loading}
                type="button"
              >
                {loading
                  ? <><i className="fa-solid fa-spinner fa-spin" /> Publicando...</>
                  : <><i className="fa-solid fa-paper-plane" /> Publicar</>
                }
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
