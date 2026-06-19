import { request, buildQuery } from './http'

export function getAdminDashboard() {
  return request('/api/admin/dashboard')
}

export function getUsers(params = {}) {
  return request(`/api/admin/users${buildQuery(params)}`)
}

export function getAdminQuestions(params = {}) {
  return request(`/api/admin/cauhoi${buildQuery(params)}`)
}

export function deleteAdminQuestion(id) {
  return request(`/api/admin/cauhoi/${id}`, { method: 'DELETE' })
}

export function restoreAdminQuestion(id) {
  return request(`/api/admin/cauhoi/${id}/khoiphuc`, { method: 'PATCH' })
}

export function getAdminAnswers(params = {}) {
  return request(`/api/admin/cautraloi${buildQuery(params)}`)
}

export function deleteAdminAnswer(id) {
  return request(`/api/admin/cautraloi/${id}`, { method: 'DELETE' })
}

export function restoreAdminAnswer(id) {
  return request(`/api/admin/cautraloi/${id}/khoiphuc`, { method: 'PATCH' })
}

export function getAdminComments(params = {}) {
  return request(`/api/admin/binhluan${buildQuery(params)}`)
}

export function deleteAdminComment(id) {
  return request(`/api/admin/binhluan/${id}`, { method: 'DELETE' })
}

export function restoreAdminComment(id) {
  return request(`/api/admin/binhluan/${id}/khoiphuc`, { method: 'PATCH' })
}

export function lockAdminUser(id) {
  return request(`/api/admin/users/${id}/lock`, {
    method: 'PATCH'
  })
}

export function unlockAdminUser(id) {
  return request(`/api/admin/users/${id}/unlock`, {
    method: 'PATCH'
  })
}

export function updateAdminUserRole(id, vaiTro) {
  return request(`/api/admin/users/${id}/role`, {
    method: 'PATCH',
    body: JSON.stringify({ vaiTro })
  })
}
export function getAdminReports(params = {}) {
  return request(`/api/admin/tocao${buildQuery(params)}`)
}

export function getPendingReportCount() {
  return request('/api/admin/tocao/pending-count')
}

export function rejectAdminReport(id, ghiChuXuLy = '') {
  return request(`/api/admin/tocao/${id}/reject`, {
    method: 'PATCH',
    body: JSON.stringify({ ghiChuXuLy })
  })
}

export function remindAdminReport(id, ghiChuXuLy = '') {
  return request(`/api/admin/tocao/${id}/remind`, {
    method: 'PATCH',
    body: JSON.stringify({ ghiChuXuLy })
  })
}

export function resolveAdminReport(id, ghiChuXuLy = '') {
  return request(`/api/admin/tocao/${id}/resolve`, {
    method: 'PATCH',
    body: JSON.stringify({ ghiChuXuLy })
  })
}
