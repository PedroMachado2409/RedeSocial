import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { amizadeApi, pendenteApi } from '../../api/api';
import { getErrorMsg, formatDate } from '../../utils/helpers';
import Avatar from '../../components/shared/Avatar';
import Spinner from '../../components/shared/Spinner';
import EmptyState from '../../components/shared/EmptyState';
import Toast from '../../components/shared/Toast';
import { useToast } from '../../hooks/useToast';
import styles from './Amigos.module.css';

export default function Amigos() {
  const navigate = useNavigate();
  const { toast, showToast } = useToast();

  const [amigos,    setAmigos]    = useState([]);
  const [recebidos, setRecebidos] = useState([]);
  const [enviados,  setEnviados]  = useState([]);
  const [loading,   setLoading]   = useState({ amigos: true, recebidos: true, enviados: true });
  const [actionLoading, setActionLoading] = useState(null);

  const loadAll = useCallback(async () => {
    setLoading({ amigos: true, recebidos: true, enviados: true });
    const [a, r, e] = await Promise.allSettled([
      amizadeApi.listar(),
      pendenteApi.recebidos(),
      pendenteApi.enviados(),
    ]);
    setAmigos   (a.status === 'fulfilled' ? a.value.data || [] : []);
    setRecebidos(r.status === 'fulfilled' ? r.value.data || [] : []);
    setEnviados (e.status === 'fulfilled' ? e.value.data || [] : []);
    setLoading({ amigos: false, recebidos: false, enviados: false });
  }, []);

  useEffect(() => { loadAll(); }, [loadAll]);

  async function handleAceitar(pedidoId) {
    setActionLoading(pedidoId);
    try {
      await amizadeApi.aceitar(pedidoId);
      setRecebidos(prev => prev.filter(p => p.id !== pedidoId));
      showToast('Amizade aceita!', 'success');
      const { data } = await amizadeApi.listar();
      setAmigos(data || []);
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setActionLoading(null);
    }
  }

  async function handleRejeitar(id) {
    setActionLoading(id);
    try {
      await pendenteApi.rejeitar(id);
      setRecebidos(prev => prev.filter(p => p.id !== id));
      showToast('Solicitação recusada.', 'success');
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setActionLoading(null);
    }
  }

  async function handleCancelarEnviado(id) {
    setActionLoading(id);
    try {
      await pendenteApi.rejeitar(id);
      setEnviados(prev => prev.filter(p => p.id !== id));
      showToast('Solicitação cancelada.', 'success');
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setActionLoading(null);
    }
  }

  async function handleRemoverAmizade(id) {
    if (!window.confirm('Remover esta amizade?')) return;
    setActionLoading(id);
    try {
      await amizadeApi.remover(id);
      setAmigos(prev => prev.filter(a => a.id !== id));
      showToast('Amizade removida.', 'success');
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setActionLoading(null);
    }
  }

  return (
    <div className={styles.page}>

      {/* ── Amigos confirmados ── */}
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <h2><i className="fa-solid fa-user-group" /> Seus Amigos</h2>
          <span className={styles.count}>{amigos.length}</span>
        </div>
        {loading.amigos ? (
          <Spinner />
        ) : amigos.length === 0 ? (
          <EmptyState icon="👥" text="Você ainda não tem amigos adicionados." />
        ) : (
          <div className={styles.friendsGrid}>
            {amigos.map((a) => (
              <div
                key={a.id}
                className={styles.friendCard}
                onClick={() => navigate(`/perfil/${a.amigoId}`)}
                title={`Ver perfil de ${a.amigoNome}`}
              >
                <Avatar
                  name={a.amigoNome}
                  src={a.amigoFotoPerfilBase64 ?? null}
                  size={72}
                />
                <p className={styles.friendName}>{a.amigoNome}</p>
                <button
                  className={styles.btnRemove}
                  onClick={(e) => { e.stopPropagation(); handleRemoverAmizade(a.id); }}
                  disabled={actionLoading === a.id}
                >
                  {actionLoading === a.id
                    ? '...'
                    : <><i className="fa-solid fa-user-minus" /> Remover</>
                  }
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* ── Solicitações recebidas ── */}
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <h2><i className="fa-solid fa-inbox" /> Solicitações Recebidas</h2>
          {recebidos.length > 0 && (
            <span className={styles.badgeRed}>{recebidos.length}</span>
          )}
        </div>
        {loading.recebidos ? (
          <Spinner />
        ) : recebidos.length === 0 ? (
          <EmptyState icon="📭" text="Nenhuma solicitação recebida." />
        ) : (
          <div className={styles.requestsList}>
            {recebidos.map((p) => (
              <div key={p.id} className={styles.requestItem}>
                <Avatar
                  name={p.solicitanteNome}
                  src={p.solicitanteFotoPerfilBase64 ?? null}
                  size={50}
                />
                <div className={styles.requestInfo}>
                  <strong>{p.solicitanteNome}</strong>
                  <small>{formatDate(p.dataSolicitacao)}</small>
                </div>
                <div className={styles.requestActions}>
                  <button
                    className={styles.btnAccept}
                    onClick={() => handleAceitar(p.id)}
                    disabled={actionLoading === p.id}
                  >
                    {actionLoading === p.id
                      ? '...'
                      : <><i className="fa-solid fa-check" /> Aceitar</>
                    }
                  </button>
                  <button
                    className={styles.btnDecline}
                    onClick={() => handleRejeitar(p.id)}
                    disabled={actionLoading === p.id}
                  >
                    <i className="fa-solid fa-xmark" /> Recusar
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* ── Solicitações enviadas ── */}
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <h2><i className="fa-solid fa-paper-plane" /> Solicitações Enviadas</h2>
          {enviados.length > 0 && (
            <span className={styles.count}>{enviados.length}</span>
          )}
        </div>
        {loading.enviados ? (
          <Spinner />
        ) : enviados.length === 0 ? (
          <EmptyState icon="✉️" text="Nenhuma solicitação enviada." />
        ) : (
          <div className={styles.requestsList}>
            {enviados.map((p) => (
              <div key={p.id} className={styles.requestItem}>
                <Avatar
                  name={p.destinatarioNome}
                  src={p.destinatarioFotoPerfilBase64 ?? null}
                  size={50}
                />
                <div className={styles.requestInfo}>
                  <strong>{p.destinatarioNome}</strong>
                  <small>Enviado em {formatDate(p.dataSolicitacao)}</small>
                </div>
                <div className={styles.requestActions}>
                  <button
                    className={styles.btnDecline}
                    onClick={() => handleCancelarEnviado(p.id)}
                    disabled={actionLoading === p.id}
                  >
                    {actionLoading === p.id
                      ? '...'
                      : <><i className="fa-solid fa-xmark" /> Cancelar</>
                    }
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <Toast toast={toast} />
    </div>
  );
}
