const TOKEN_KEY = 'forum_access_token'
const USER_KEY = 'forum_user'

const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

// Dùng sessionStorage để không lưu đăng nhập quá lâu sau khi đóng trình duyệt
const storage = window.sessionStorage

export function getToken() {
  const token = storage.getItem(TOKEN_KEY)

  if (!token) return null

  if (isTokenExpired(token)) {
    clearAuth()
    return null
  }

  return token
}

export function saveToken(token) {
  if (token) storage.setItem(TOKEN_KEY, token)
}

export function getCurrentUser() {
  const token = getToken()
  if (!token) return null

  const raw = storage.getItem(USER_KEY)

  if (raw) {
    try {
      const user = JSON.parse(raw)
      return normalizeUser(user)
    } catch {
      storage.removeItem(USER_KEY)
    }
  }

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

  storage.setItem(USER_KEY, JSON.stringify(normalizeUser(user)))
  window.dispatchEvent(new Event('auth-changed'))
}

export function setAuth(token, user) {
  saveToken(token)
  saveUser(user)
  window.dispatchEvent(new Event('auth-changed'))
}

export function clearAuth() {
  storage.removeItem(TOKEN_KEY)
  storage.removeItem(USER_KEY)

  // Xóa luôn dữ liệu cũ trước đây nếu project từng dùng localStorage
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

export function isTokenExpired(token) {
  const payload = parseJwt(token)

  if (!payload?.exp) return false

  const currentTime = Math.floor(Date.now() / 1000)
  return payload.exp <= currentTime
}

export function normalizeEmailValue(email) {
  return String(email || '').trim().toLowerCase()
}

// Dùng cho đăng nhập: admin Gmail vẫn đăng nhập được
export function isValidEmail(email) {
  const value = normalizeEmailValue(email)

  return /^[a-z0-9]+(?:[._%+-][a-z0-9]+)*@(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z]{2,}$/.test(value)
}

// Dùng cho đăng ký: chỉ cho email sinh viên TVU
export function isValidTvuStudentEmail(email) {
  const value = normalizeEmailValue(email)

  return /^\d{10}@st\.tvu\.edu\.vn$/.test(value)
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