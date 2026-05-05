import { useEffect, useState } from 'react';

export default function Toast({ toast }) {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    if (toast) {
      setVisible(true);
    } else {
      setVisible(false);
    }
  }, [toast]);

  if (!toast || !visible) return null;

  const colors = {
    success: '#2e7d32',
    error: '#c62828',
    info: '#1565c0',
  };

  return (
    <div
      style={{
        position: 'fixed',
        bottom: 28,
        left: '50%',
        transform: 'translateX(-50%)',
        background: colors[toast.type] || '#333',
        color: '#fff',
        padding: '12px 28px',
        borderRadius: 24,
        fontSize: 14,
        fontWeight: 500,
        zIndex: 9999,
        boxShadow: '0 4px 16px rgba(0,0,0,.3)',
        animation: 'fadeUp .25s ease',
        whiteSpace: 'nowrap',
      }}
    >
      {toast.msg}
    </div>
  );
}
