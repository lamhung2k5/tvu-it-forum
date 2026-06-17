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
