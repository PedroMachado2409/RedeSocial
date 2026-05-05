import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import PrivateRoute from './components/Layout/PrivateRoute';
import Layout from './components/Layout/Layout';
import Login from './pages/Login/Login';
import Feed from './pages/Feed/Feed';
import Amigos from './pages/Amigos/Amigos';
import Buscar from './pages/Buscar/Buscar';
import Perfil from './pages/Perfil/Perfil';

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Pública */}
          <Route path="/login" element={<Login />} />

          {/* Privadas — dentro do Layout com Navbar */}
          <Route
            element={
              <PrivateRoute>
                <Layout />
              </PrivateRoute>
            }
          >
            <Route path="/"              element={<Feed />} />
            <Route path="/amigos"        element={<Amigos />} />
            <Route path="/buscar"        element={<Buscar />} />
            <Route path="/perfil"        element={<Perfil />} />
            <Route path="/perfil/:id"    element={<Perfil />} />
          </Route>

          {/* Fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}
