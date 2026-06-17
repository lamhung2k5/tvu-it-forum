const TOKEN_KEY = 'forum_access_token'
const USER_KEY = 'forum_user'

const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function saveToken(token) {
  if (token) localStorage.setItem(TOKEN_KEY, token)
}

export function getCurrentUser() {
  const raw = localStorage.getItem(USER_KEY)
  if (raw) {
    try {
      const user = JSON.parse(raw)
      return normalizeUser(user)
    } catch {
      localStorage.removeItem(USER_KEY)
    }
  }

  const token = getToken()
  if (!token) return null

  const payload = parseJwt(token)
  if (!payload) return null

  return normalizeUser({
    id_NguoiDung: payload.sub || payload.nameid || payload.id,
    hoTen: payload.HoTen || payload.hoTen || payload.email || 'Người dùng',
    email: payload.email,
    vaiTro: payload[roleClaim] || payload.role || payload.VaiTro || 'User'
  })
}

export function saveUser(user) {
  if (!user) return
  localStorage.setItem(USER_KEY, JSON.stringify(normalizeUser(user)))
  window.dispatchEvent(new Event('auth-changed'))
}

export function setAuth(token, user) {
  saveToken(token)
  saveUser(user)
  window.dispatchEvent(new Event('auth-changed'))
}

export function clearAuth() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(USER_KEY)
  window.dispatchEvent(new Event('auth-changed'))
}

export function isAuthenticated() {
  return Boolean(getToken())
}

export function isAdmin() {
  const user = getCurrentUser()
  return String(user?.vaiTro || '').toLowerCase() === 'admin'
}

export function parseJwt(token) {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch {
    return null
  }
}

export function normalizeUser(user = {}) {
  return {
    id_NguoiDung: Number(firstValue(user, ['id_NguoiDung', 'iD_NguoiDung', 'ID_NguoiDung', 'idNguoiDung', 'id']) || 0),
    hoTen: firstValue(user, ['hoTen', 'HoTen', 'name', 'email']) || 'Người dùng',
    email: firstValue(user, ['email', 'Email']) || '',
    anhDaiDien: firstValue(user, ['anhDaiDien', 'AnhDaiDien']) || null,
    vaiTro: firstValue(user, ['vaiTro', 'VaiTro', 'role']) || 'User',
    trangThai: firstValue(user, ['trangThai', 'TrangThai']) ?? 1
  }
}

export function firstValue(obj, keys) {
  for (const key of keys) {
    if (obj && obj[key] !== undefined && obj[key] !== null) return obj[key]
  }
  return undefined
}
