/** Retorna a inicial maiúscula de um nome */
export function getInitial(name) {
  return (name || 'U')[0].toUpperCase();
}

/** Formata data ISO para pt-BR */
export function formatDate(iso) {
  if (!iso) return '';
  try {
    return new Date(iso).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  } catch {
    return iso;
  }
}

/** Formata data+hora ISO para pt-BR */
export function formatDateTime(iso) {
  if (!iso) return '';
  try {
    return new Date(iso).toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  } catch {
    return iso;
  }
}

/** Extrai mensagem de erro do axios */
export function getErrorMsg(err) {
  return (
    err?.response?.data?.detail ||
    err?.response?.data?.message ||
    err?.response?.data?.title ||
    err?.message ||
    'Erro desconhecido'
  );
}

/** Gera cor de avatar baseada no nome */
const COLORS = [
  '#1877f2', '#e91e63', '#9c27b0', '#673ab7',
  '#3f51b5', '#009688', '#4caf50', '#ff5722',
  '#795548', '#607d8b',
];
export function avatarColor(name) {
  if (!name) return COLORS[0];
  let hash = 0;
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
  return COLORS[Math.abs(hash) % COLORS.length];
}
