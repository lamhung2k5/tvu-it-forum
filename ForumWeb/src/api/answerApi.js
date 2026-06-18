import { request } from './http'

export function getAnswersByQuestion(questionId) {
  return request(`/api/cauhoi/${questionId}/cautraloi`)
}

export function getAnswerById(id) {
  return request(`/api/cautraloi/${id}`)
}

export function createAnswer(questionId, payload) {
  return request(`/api/cauhoi/${questionId}/cautraloi`, {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function updateAnswer(id, payload) {
  return request(`/api/cautraloi/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload)
  })
}

export function deleteAnswer(id) {
  return request(`/api/cautraloi/${id}`, {
    method: 'DELETE'
  })
}

export function acceptAnswer(id) {
  return request(`/api/cautraloi/${id}/chapnhan`, {
    method: 'PATCH'
  })
}

export function unacceptAnswer(id) {
  return request(`/api/cautraloi/${id}/bo-chap-nhan`, {
    method: 'PATCH'
  })
}
