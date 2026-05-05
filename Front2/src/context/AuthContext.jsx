import { createContext, useContext, useState, useCallback } from 'react';
import { authApi, perfilApi } from '../api/api';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    try {
      const stored = localStorage.getItem('rs_user');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  });

  /** Salva user no estado e no localStorage */
  function saveUser(userData) {
    localStorage.setItem('rs_user', JSON.stringify(userData));
    setUser(userData);
  }

  const login = useCallback(async (email, senha) => {
    const { data } = await authApi.login(email, senha);
    localStorage.setItem('rs_token', data.token);

    // Busca dados completos do usuário (id + foto de perfil)
    let userId = null;
    let fotoPerfilBase64 = null;
    try {
      const meRes = await authApi.me();
      userId = meRes.data?.id ?? null;

      // Busca perfil para obter a foto
      if (userId) {
        const perfilRes = await perfilApi.obter(userId);
        fotoPerfilBase64 = perfilRes.data?.fotoPerfilBase64 ?? null;
      }
    } catch {
      // continua sem foto — não é crítico
    }

    const userData = { nome: data.nome, email: data.email, id: userId, fotoPerfilBase64 };
    saveUser(userData);
    return userData;
  }, []);

  const registrar = useCallback(async (nome, email, senha) => {
    await authApi.registrar(nome, email, senha);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('rs_token');
    localStorage.removeItem('rs_user');
    setUser(null);
  }, []);

  /** Atualiza a foto de perfil no contexto após edição */
  const atualizarFotoPerfil = useCallback((fotoPerfilBase64) => {
    setUser(prev => {
      if (!prev) return prev;
      const updated = { ...prev, fotoPerfilBase64 };
      localStorage.setItem('rs_user', JSON.stringify(updated));
      return updated;
    });
  }, []);

  const isLoggedIn = !!user;

  return (
    <AuthContext.Provider value={{ user, login, registrar, logout, isLoggedIn, atualizarFotoPerfil }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
