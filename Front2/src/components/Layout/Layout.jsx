import { useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import Navbar from '../Navbar/Navbar';
import { pendenteApi } from '../../api/api';
import styles from './Layout.module.css';

export default function Layout() {
  const [notifCount, setNotifCount] = useState(0);

  useEffect(() => {
    loadNotifCount();
    // Polling a cada 30s
    const interval = setInterval(loadNotifCount, 30000);
    return () => clearInterval(interval);
  }, []);

  async function loadNotifCount() {
    try {
      const { data } = await pendenteApi.recebidos();
      setNotifCount(data?.length || 0);
    } catch {
      // silencioso
    }
  }

  return (
    <div className={styles.appShell}>
      <Navbar
        notifCount={notifCount}
        onNotifCountChange={setNotifCount}
      />
      <div className={styles.pageContent}>
        <Outlet />
      </div>
    </div>
  );
}
