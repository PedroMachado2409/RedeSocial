import { useNavigate } from 'react-router-dom';
import Avatar from '../../components/shared/Avatar';
import styles from './SidebarRight.module.css';

export default function SidebarRight({ amigos }) {
  const navigate = useNavigate();

  return (
    <aside className={styles.sidebar}>
      <div className={styles.widget}>
        <h4 className={styles.title}>
          <i className="fa-solid fa-user-group" /> Seus Amigos
        </h4>
        {amigos.length === 0 ? (
          <p className={styles.empty}>Nenhum amigo ainda.</p>
        ) : (
          <ul className={styles.list}>
            {amigos.map((a) => (
              <li
                key={a.id}
                className={styles.item}
                onClick={() => navigate(`/perfil/${a.amigoId}`)}
                title={`Ver perfil de ${a.amigoNome}`}
                style={{ cursor: 'pointer' }}
              >
                <Avatar
                  name={a.amigoNome}
                  src={a.amigoFotoPerfilBase64 ?? null}
                  size={32}
                />
                <span className={styles.name}>{a.amigoNome}</span>
                <span className={styles.onlineDot} />
              </li>
            ))}
          </ul>
        )}
      </div>
    </aside>
  );
}
