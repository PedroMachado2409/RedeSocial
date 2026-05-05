import Avatar from '../../components/shared/Avatar';
import styles from './SidebarLeft.module.css';

export default function SidebarLeft({ user, feedMode, onFeedMode }) {
  return (
    <aside className={styles.sidebar}>
      <div className={styles.profileCard}>
        <Avatar name={user?.nome} src={user?.fotoPerfilBase64 ?? null} size={36} />
        <div className={styles.profileInfo}>
          <p className={styles.profileName}>{user?.nome}</p>
          <p className={styles.profileEmail}>{user?.email}</p>
        </div>
      </div>

      <div className={styles.divider} />

      <nav className={styles.nav}>
        <button
          className={`${styles.navItem} ${feedMode === 'todos' ? styles.navItemActive : ''}`}
          onClick={() => onFeedMode('todos')}
        >
          <span className={styles.navIcon}><i className="fa-solid fa-globe" /></span>
          Todos os Posts
        </button>
        <button
          className={`${styles.navItem} ${feedMode === 'meus' ? styles.navItemActive : ''}`}
          onClick={() => onFeedMode('meus')}
        >
          <span className={styles.navIcon}><i className="fa-solid fa-user" /></span>
          Meus Posts
        </button>
      </nav>
    </aside>
  );
}
