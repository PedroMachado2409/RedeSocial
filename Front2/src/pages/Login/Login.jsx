import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { getErrorMsg } from '../../utils/helpers';
import styles from './Login.module.css';

export default function Login() {
  const { login, registrar } = useAuth();
  const navigate = useNavigate();

  const [mode, setMode] = useState('login'); // 'login' | 'register'

  // Login state
  const [loginEmail, setLoginEmail] = useState('');
  const [loginSenha, setLoginSenha] = useState('');
  const [loginError, setLoginError] = useState('');
  const [loginLoading, setLoginLoading] = useState(false);

  // Register state
  const [regNome, setRegNome] = useState('');
  const [regEmail, setRegEmail] = useState('');
  const [regSenha, setRegSenha] = useState('');
  const [regError, setRegError] = useState('');
  const [regLoading, setRegLoading] = useState(false);
  const [regSuccess, setRegSuccess] = useState('');

  async function handleLogin(e) {
    e.preventDefault();
    if (!loginEmail || !loginSenha) {
      setLoginError('Preencha e-mail e senha.');
      return;
    }
    setLoginLoading(true);
    setLoginError('');
    try {
      await login(loginEmail, loginSenha);
      navigate('/');
    } catch (err) {
      setLoginError(getErrorMsg(err));
    } finally {
      setLoginLoading(false);
    }
  }

  async function handleRegister(e) {
    e.preventDefault();
    if (!regNome || !regEmail || !regSenha) {
      setRegError('Preencha todos os campos.');
      return;
    }
    setRegLoading(true);
    setRegError('');
    setRegSuccess('');
    try {
      await registrar(regNome, regEmail, regSenha);
      setRegSuccess('Conta criada com sucesso! Faça login.');
      setMode('login');
      setLoginEmail(regEmail);
    } catch (err) {
      setRegError(getErrorMsg(err));
    } finally {
      setRegLoading(false);
    }
  }

  return (
    <div className={styles.wrapper}>
      {/* LEFT */}
      <div className={styles.left}>
        <div className={styles.logoIcon}>
          <i className="fa-solid fa-people-group" />
        </div>
        <h1 className={styles.brandName}>RedeSocial</h1>
        <p className={styles.tagline}>
          Conecte-se com amigos e o mundo ao seu redor.
        </p>
      </div>

      {/* RIGHT */}
      <div className={styles.right}>
        {mode === 'login' ? (
          <form className={styles.card} onSubmit={handleLogin} noValidate>
            {regSuccess && (
              <div className={styles.successMsg}>{regSuccess}</div>
            )}
            <input
              type="email"
              placeholder="E-mail"
              className={styles.input}
              value={loginEmail}
              onChange={(e) => setLoginEmail(e.target.value)}
              autoComplete="email"
              autoFocus
            />
            <input
              type="password"
              placeholder="Senha"
              className={styles.input}
              value={loginSenha}
              onChange={(e) => setLoginSenha(e.target.value)}
              autoComplete="current-password"
            />
            {loginError && <div className={styles.error}>{loginError}</div>}
            <button
              type="submit"
              className={styles.btnPrimary}
              disabled={loginLoading}
            >
              {loginLoading ? 'Entrando...' : 'Entrar'}
            </button>

            <div className={styles.divider}><span>ou</span></div>

            <button
              type="button"
              className={styles.btnGreen}
              onClick={() => { setMode('register'); setLoginError(''); }}
            >
              Criar nova conta
            </button>
          </form>
        ) : (
          <form className={styles.card} onSubmit={handleRegister} noValidate>
            <div className={styles.registerHeader}>
              <h3>Criar uma conta</h3>
              <p>É rápido e fácil.</p>
            </div>
            <hr className={styles.hr} />
            <input
              type="text"
              placeholder="Nome completo"
              className={styles.input}
              value={regNome}
              onChange={(e) => setRegNome(e.target.value)}
              autoComplete="name"
              autoFocus
            />
            <input
              type="email"
              placeholder="E-mail"
              className={styles.input}
              value={regEmail}
              onChange={(e) => setRegEmail(e.target.value)}
              autoComplete="email"
            />
            <input
              type="password"
              placeholder="Senha"
              className={styles.input}
              value={regSenha}
              onChange={(e) => setRegSenha(e.target.value)}
              autoComplete="new-password"
            />
            {regError && <div className={styles.error}>{regError}</div>}
            <button
              type="submit"
              className={styles.btnGreen}
              disabled={regLoading}
            >
              {regLoading ? 'Registrando...' : 'Registrar'}
            </button>
            <button
              type="button"
              className={styles.btnGhost}
              onClick={() => { setMode('login'); setRegError(''); }}
            >
              Já tenho uma conta
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
