import { useState, useRef, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Avatar from '../shared/Avatar';
import NotifPanel from './NotifPanel';
import styles from './Navbar.module.css';

export default function Navbar({ notifCount, onNotifCountChange }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [notifOpen, setNotifOpen] = useState(false);
  const dropdownRef = useRef(null);
  const notifRef = useRef(null);

  // Fecha dropdowns ao clicar fora
  useEffect(() => {
    function handleClick(e) {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target)) {
        setDropdownOpen(false);
      }
      if (notifRef.current && !notifRef.current.contains(e.target)) {
        setNotifOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, []);

  const tabs = [
    { path: '/',        icon: 'fa-house',           label: 'Início'  },
    { path: '/amigos',  icon: 'fa-user-group',       label: 'Amigos'  },
    { path: '/buscar',  icon: 'fa-magnifying-glass', label: 'Buscar'  },
    { path: '/perfil',  icon: 'fa-user',             label: 'Perfil'  },
  ];

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <nav className={styles.navbar}>
      {/* ESQUERDA */}
      <div className={styles.navLeft}>
        <span className={styles.brand} onClick={() => navigate('/')}>
          <i className="fa-solid fa-people-group" />
          <span className={styles.brandText}>RedeSocial</span>
        </span>
      </div>

      {/* CENTRO */}
      <div className={styles.navCenter}>
        {tabs.map((tab) => {
          const active = location.pathname === tab.path;
          return (
            <button
              key={tab.path}
              className={`${styles.navTab} ${active ? styles.navTabActive : ''}`}
              onClick={() => navigate(tab.path)}
              title={tab.label}
            >
              <i className={`fa-solid ${tab.icon}`} />
              <span className={styles.navLabel}>{tab.label}</span>
            </button>
          );
        })}
      </div>

      {/* DIREITA */}
      <div className={styles.navRight}>
        {/* Notificações */}
        <div ref={notifRef} className={styles.notifWrapper}>
          <button
            className={styles.iconBtn}
            onClick={() => setNotifOpen((v) => !v)}
            title="Solicitações de amizade"
          >
            <i className="fa-solid fa-bell" />
            {notifCount > 0 && (
              <span className={styles.badge}>{notifCount}</span>
            )}
          </button>
          {notifOpen && (
            <NotifPanel
              onClose={() => setNotifOpen(false)}
              onCountChange={onNotifCountChange}
            />
          )}
        </div>

        {/* Avatar / Dropdown */}
        <div ref={dropdownRef} className={styles.avatarWrapper}>
          <button
            className={styles.avatarBtn}
            onClick={() => setDropdownOpen((v) => !v)}
            title="Meu perfil"
          >
            <Avatar name={user?.nome} src={user?.fotoPerfilBase64 ?? null} size={36} />
          </button>

          {dropdownOpen && (
            <div className={styles.dropdown}>
              <div className={styles.dropdownProfile}>
                <Avatar name={user?.nome} src={user?.fotoPerfilBase64 ?? null} size={48} />
                <div>
                  <strong>{user?.nome}</strong>
                  <small>{user?.email}</small>
                </div>
              </div>
              <hr className={styles.dropdownHr} />
              <button className={styles.dropdownItem} onClick={() => { navigate('/perfil'); setDropdownOpen(false); }}>
                <i className="fa-solid fa-user" />
                Meu Perfil
              </button>
              <button className={styles.dropdownItem} onClick={handleLogout}>
                <i className="fa-solid fa-right-from-bracket" />
                Sair da conta
              </button>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
