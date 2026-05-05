import { useState } from 'react';
import { authApi, pendenteApi } from '../../api/api';
import { getErrorMsg } from '../../utils/helpers';
import Avatar from '../../components/shared/Avatar';
import Spinner from '../../components/shared/Spinner';
import Toast from '../../components/shared/Toast';
import { useToast } from '../../hooks/useToast';
import styles from './Buscar.module.css';

export default function Buscar() {
  const { toast, showToast } = useToast();

  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searched, setSearched] = useState(false);
  const [sentTo, setSentTo] = useState(new Set()); // IDs que já receberam solicitação
  const [actionLoading, setActionLoading] = useState(null);

  async function handleSearch(e) {
    e?.preventDefault();
    const trimmed = query.trim();
    if (!trimmed) return;

    setLoading(true);
    setSearched(true);
    try {
      const { data } = await authApi.buscarPorNome(trimmed);
      setResults(data || []);
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
      setResults([]);
    } finally {
      setLoading(false);
    }
  }

  async function handleEnviar(destinatarioId) {
    setActionLoading(destinatarioId);
    try {
      await pendenteApi.enviar(destinatarioId);
      setSentTo((prev) => new Set([...prev, destinatarioId]));
      showToast('Solicitação enviada!', 'success');
    } catch (err) {
      showToast(getErrorMsg(err), 'error');
    } finally {
      setActionLoading(null);
    }
  }

  return (
    <div className={styles.page}>
      <div className={styles.hero}>
        <h2>Encontre pessoas</h2>
        <p>Busque pelo nome para adicionar novos amigos</p>
      </div>

      <form className={styles.searchBar} onSubmit={handleSearch}>
        <i className="fa-solid fa-magnifying-glass" />
        <input
          type="text"
          placeholder="Digite um nome..."
          className={styles.input}
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          autoFocus
        />
        <button type="submit" className={styles.btnSearch} disabled={loading || !query.trim()}>
          {loading ? <i className="fa-solid fa-spinner fa-spin" /> : 'Buscar'}
        </button>
      </form>

      <div className={styles.results}>
        {loading ? (
          <Spinner />
        ) : searched && results.length === 0 ? (
          <div className={styles.noResults}>
            <i className="fa-solid fa-user-slash" />
            <p>Nenhum usuário encontrado para "<strong>{query}</strong>"</p>
          </div>
        ) : (
          results.map((u) => {
            const sent = sentTo.has(u.id);
            return (
              <div key={u.id} className={styles.resultCard}>
                <Avatar name={u.nome} size={52} />
                <div className={styles.resultInfo}>
                  <strong>{u.nome}</strong>
                  <small>{u.email}</small>
                </div>
                <button
                  className={sent ? styles.btnSent : styles.btnAdd}
                  onClick={() => !sent && handleEnviar(u.id)}
                  disabled={sent || actionLoading === u.id}
                >
                  {actionLoading === u.id ? (
                    <i className="fa-solid fa-spinner fa-spin" />
                  ) : sent ? (
                    <><i className="fa-solid fa-check" /> Enviado</>
                  ) : (
                    <><i className="fa-solid fa-user-plus" /> Adicionar</>
                  )}
                </button>
              </div>
            );
          })
        )}
      </div>

      <Toast toast={toast} />
    </div>
  );
}
