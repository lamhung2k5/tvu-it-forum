import { request, buildQuery } from './http'

export function getRecentNotifications(limit = 5) {
  return request(`/api/thongbao/recent${buildQuery({ limit })}`)
}

export function getNotifications(params = {}) {
  return request(`/api/thongbao/${buildQuery(params)}`)
}

export function getUnreadNotificationCount() {
  return request('/api/thongbao/unread-count')
}

export function markNotificationAsRead(id) {
  return request(`/api/thongbao/${id}/read`, { method: 'PATCH' })
}

export function markAllNotificationsAsRead() {
  return request('/api/thongbao/read-all', { method: 'PATCH' })
}

export function deleteNotification(id) {
  return request(`/api/thongbao/${id}`, { method: 'DELETE' })
}

export function clearReadNotifications() {
  return request('/api/thongbao/clear-read', { method: 'DELETE' })
}

export function clearAllNotifications() {
  return request('/api/thongbao/clear-all', { method: 'DELETE' })
}
