import { getInitial, avatarColor } from '../../utils/helpers';

/**
 * Avatar component
 * @param {string}  name     - Nome do usuário (para inicial e cor)
 * @param {number}  size     - Tamanho em px (default 40)
 * @param {string}  src      - Base64 da foto de perfil (sem prefixo data:) — opcional
 * @param {string}  className
 */
export default function Avatar({ name, size = 40, src = null, className = '' }) {
  const initial = getInitial(name);
  const bg      = avatarColor(name);

  // Se tem foto, exibe a imagem
  if (src) {
    const imgSrc = src.startsWith('data:') ? src : `data:image/jpeg;base64,${src}`;
    return (
      <img
        src={imgSrc}
        alt={name || 'Avatar'}
        className={className}
        style={{
          width: size,
          height: size,
          minWidth: size,
          borderRadius: '50%',
          objectFit: 'cover',
          display: 'block',
          userSelect: 'none',
        }}
      />
    );
  }

  // Fallback: inicial colorida
  return (
    <div
      className={`avatar ${className}`}
      style={{
        width: size,
        height: size,
        minWidth: size,
        background: bg,
        borderRadius: '50%',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        color: '#fff',
        fontWeight: 700,
        fontSize: size * 0.4,
        userSelect: 'none',
        flexShrink: 0,
      }}
    >
      {initial}
    </div>
  );
}
