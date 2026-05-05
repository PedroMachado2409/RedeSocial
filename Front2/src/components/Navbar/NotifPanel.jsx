import { useEffect, useState } from 'react';
import { pendenteApi, amizadeApi } from '../../api/api';
import Avatar from '../shared/Avatar';
import Spinner from '../shared/Spinner';
import { getErrorMsg, formatDate } from '../../utils/helpers';
import styles from './NotifPanel.module.css';

export default function NotifPanel({ onClose, onCountChange }) {
  const [pedidos, setPedidos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(null);

  useEffect(() => {
    load();
  }, []);

  async function load() {
    setLoading(true);
    try {
      const { data } = await pendenteApi.recebidos();
      setPedidos(data || []);
      onCountChange?.(data?.length || 0);
    } catch {
      setPedidos([]);
    } finally {
      setLoading(false);
    }
  }

  async function aceitar(pedidoId) {
    setActionLoading(pedidoId);
    try {
      await amizadeApi.aceitar(pedidoId);
      const updated = pedidos.filter((p) => p.id !== pedidoId);
      setPedidos(updated);
      onCountChange?.(updated.length);
    } catch (err) {
      alert(getErrorMsg(err));
    } finally {
      setActionLoading(null);
    }
  }

  async function rejeitar(id) {
    setActionLoading(id);
    try {
      await pendenteApi.rejeitar(id);
      const updated = pedidos.filter((p) => p.id !== id);
      setPedidos(updated);
      onCountChange?.(updated.length);
    } catch (err) {
      alert(getErrorMsg(err));
    } finally {
      setActionLoading(null);
    }
  }

  return (
    <div className={styles.panel}>
      <div className={styles.header}>
        <h3>Solicitações de Amizade</h3>
        <button className={styles.closeBtn} onClick={onClose}>
          <i className="fa-solid fa-xmark" />
        </button>
      </div>

      <div className={styles.list}>
        {loading ? (
          <Spinner size={24} />
        ) : pedidos.length === 0 ? (
          <p className={styles.empty}>Nenhuma solicitação pendente.</p>
        ) : (
          pedidos.map((p) => (
            <div key={p.id} className={styles.item}>
              <Avatar
                name={p.solicitanteNome}
                src={p.solicitanteFotoPerfilBase64 ?? null}
                size={48}
              />
              <div className={styles.info}>
                <strong>{p.solicitanteNome}</strong>
                <small>{formatDate(p.dataSolicitacao)}</small>
              </div>
              <div className={styles.actions}>
                <button
                  className={styles.btnAccept}
                  onClick={() => aceitar(p.id)}
                  disabled={actionLoading === p.id}
                >
                  {actionLoading === p.id ? '...' : 'Aceitar'}
                </button>
                <button
                  className={styles.btnReject}
                  onClick={() => rejeitar(p.id)}
                  disabled={actionLoading === p.id}
                >
                  Recusar
                </button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
