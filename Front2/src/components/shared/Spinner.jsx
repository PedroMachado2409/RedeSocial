export default function Spinner({ size = 32, color = '#1877f2' }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'center', padding: '32px 0' }}>
      <div
        style={{
          width: size,
          height: size,
          border: `3px solid #e4e6ea`,
          borderTop: `3px solid ${color}`,
          borderRadius: '50%',
          animation: 'spin .7s linear infinite',
        }}
      />
    </div>
  );
}
