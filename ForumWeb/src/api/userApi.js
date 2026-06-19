import { request } from './http'

export function getMyProfile() {
  return request('/api/users/me')
}

export function updateMyProfile(payload) {
  return request('/api/users/me', {
    method: 'PUT',
    body: JSON.stringify(payload)
  })
}

export function getMyDashboard() {
  return request('/api/users/me/dashboard')
}

export function getMyQuestions() {
  return request('/api/users/me/questions')
}

export function getMyAnswers() {
  return request('/api/users/me/answers')
}

export function getMyComments() {
  return request('/api/users/me/comments')
}

export function getPublicProfile(id) {
  return request(`/api/users/${id}/profile`)
}
