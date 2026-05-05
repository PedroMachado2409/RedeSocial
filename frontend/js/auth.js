/**
 * auth.js — Login, Registro e controle de sessão
 */

function initAuth() {
  // Elementos
  const formLogin    = document.getElementById('form-login');
  const formRegister = document.getElementById('form-register');

  const btnLogin        = document.getElementById('btn-login');
  const btnRegister     = document.getElementById('btn-register');
  const btnShowRegister = document.getElementById('btn-show-register');
  const btnShowLogin    = document.getElementById('btn-show-login');

  const loginEmail = document.getElementById('login-email');
  const loginSenha = document.getElementById('login-senha');
  const loginError = document.getElementById('login-error');

  const regNome  = document.getElementById('reg-nome');
  const regEmail = document.getElementById('reg-email');
  const regSenha = document.getElementById('reg-senha');
  const regError = document.getElementById('reg-error');

  // Alternar formulários
  btnShowRegister.addEventListener('click', () => {
    formLogin.classList.add('hidden');
    formRegister.classList.remove('hidden');
    clearErrors();
  });

  btnShowLogin.addEventListener('click', () => {
    formRegister.classList.add('hidden');
    formLogin.classList.remove('hidden');
    clearErrors();
  });

  // Login
  btnLogin.addEventListener('click', handleLogin);
  loginSenha.addEventListener('keydown', (e) => { if (e.key === 'Enter') handleLogin(); });

  async function handleLogin() {
    const email = loginEmail.value.trim();
    const senha = loginSenha.value;

    if (!email || !senha) {
      showError(loginError, 'Preencha e-mail e senha.');
      return;
    }

    btnLogin.disabled = true;
    btnLogin.textContent = 'Entrando...';
    hideError(loginError);

    try {
      const res = await ApiAuth.login(email, senha);
      Auth.setToken(res.token);
      Auth.setUser({ nome: res.nome, email: res.email });
      startApp();
    } catch (err) {
      showError(loginError, err.message);
    } finally {
      btnLogin.disabled = false;
      btnLogin.textContent = 'Entrar';
    }
  }

  // Registro
  btnRegister.addEventListener('click', handleRegister);
  regSenha.addEventListener('keydown', (e) => { if (e.key === 'Enter') handleRegister(); });

  async function handleRegister() {
    const nome  = regNome.value.trim();
    const email = regEmail.value.trim();
    const senha = regSenha.value;

    if (!nome || !email || !senha) {
      showError(regError, 'Preencha todos os campos.');
      return;
    }

    btnRegister.disabled = true;
    btnRegister.textContent = 'Registrando...';
    hideError(regError);

    try {
      await ApiAuth.registrar(nome, email, senha);
      showToast('Conta criada! Faça login.', 'success');
      // Volta para login e preenche email
      formRegister.classList.add('hidden');
      formLogin.classList.remove('hidden');
      loginEmail.value = email;
      loginSenha.focus();
    } catch (err) {
      showError(regError, err.message);
    } finally {
      btnRegister.disabled = false;
      btnRegister.textContent = 'Registrar';
    }
  }

  function clearErrors() {
    hideError(loginError);
    hideError(regError);
  }
}

function showError(el, msg) {
  el.textContent = msg;
  el.classList.remove('hidden');
}

function hideError(el) {
  el.textContent = '';
  el.classList.add('hidden');
}
