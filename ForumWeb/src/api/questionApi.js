import { request, buildQuery } from './http'

export function getQuestions(params = {}) {
  return request(`/api/cauhoi${buildQuery(params)}`)
}

export function getQuestionById(id) {
  return request(`/api/cauhoi/${id}`)
}

export function createQuestion(payload) {
  return request('/api/cauhoi', {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function updateQuestion(id, payload) {
  return request(`/api/cauhoi/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload)
  })
}

export function deleteQuestion(id) {
  return request(`/api/cauhoi/${id}`, {
    method: 'DELETE'
  })
}
