import { getToken, clearAuth, firstValue } from '../utils/auth'

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5182'

export async function request(path, options = {}) {
  const token = getToken()
  const headers = {
    Accept: 'application/json',
    ...(options.body ? { 'Content-Type': 'application/json' } : {}),
    ...(options.headers || {})
  }

  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  })

  const contentType = response.headers.get('content-type') || ''
  let data = null

  if (contentType.includes('application/json')) {
    data = await response.json().catch(() => null)
  } else {
    const text = await response.text().catch(() => '')
    data = text ? { message: text } : null
  }

  if (response.status === 401) {
    clearAuth()
  }

  if (!response.ok) {
    const message = firstValue(data || {}, ['message', 'Message', 'error', 'Error']) || `Lỗi ${response.status}`
    const error = new Error(message)
    error.status = response.status
    error.data = data
    throw error
  }

  return data
}

export function buildQuery(params = {}) {
  const query = new URLSearchParams()
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      query.append(key, value)
    }
  })
  const qs = query.toString()
  return qs ? `?${qs}` : ''
}
