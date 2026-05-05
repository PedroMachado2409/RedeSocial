export default function EmptyState({ icon = '📭', text = 'Nada por aqui.' }) {
  return (
    <div style={{
      textAlign: 'center',
      padding: '32px 16px',
      color: '#65676b',
    }}>
      <div style={{ fontSize: 36, marginBottom: 8 }}>{icon}</div>
      <p style={{ fontSize: 14 }}>{text}</p>
    </div>
  );
}
