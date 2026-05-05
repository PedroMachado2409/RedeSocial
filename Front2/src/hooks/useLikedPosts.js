import { useState, useCallback } from 'react';

const SESSION_KEY = 'rs_liked_posts';

/** Lê o Set do sessionStorage */
function readFromSession() {
  try {
    const raw = sessionStorage.getItem(SESSION_KEY);
    return raw ? new Set(JSON.parse(raw)) : new Set();
  } catch {
    return new Set();
  }
}

/** Persiste o Set no sessionStorage */
function writeToSession(set) {
  try {
    sessionStorage.setItem(SESSION_KEY, JSON.stringify([...set]));
  } catch {
    // ignora erros de storage
  }
}

/**
 * Hook que mantém o Set de posts curtidos persistido no sessionStorage.
 * Sobrevive à navegação entre páginas dentro da mesma aba.
 */
export function useLikedPosts() {
  const [likedPosts, setLikedPostsState] = useState(() => readFromSession());

  const setLikedPosts = useCallback((updater) => {
    setLikedPostsState(prev => {
      const next = typeof updater === 'function' ? updater(prev) : updater;
      writeToSession(next);
      return next;
    });
  }, []);

  const addLike    = useCallback((id) => setLikedPosts(prev => { const n = new Set(prev); n.add(id);    writeToSession(n); return n; }), [setLikedPosts]);
  const removeLike = useCallback((id) => setLikedPosts(prev => { const n = new Set(prev); n.delete(id); writeToSession(n); return n; }), [setLikedPosts]);
  const isLiked    = useCallback((id) => likedPosts.has(id), [likedPosts]);

  return { likedPosts, setLikedPosts, addLike, removeLike, isLiked };
}
