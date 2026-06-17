import { request } from './http'

export function getQuestionComments(questionId) {
  return request(`/api/cauhoi/${questionId}/binhluan`)
}

export function createQuestionComment(questionId, payload) {
  return request(`/api/cauhoi/${questionId}/binhluan`, {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function getAnswerComments(answerId) {
  return request(`/api/cautraloi/${answerId}/binhluan`)
}

export function createAnswerComment(answerId, payload) {
  return request(`/api/cautraloi/${answerId}/binhluan`, {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function updateComment(id, payload) {
  return request(`/api/binhluan/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload)
  })
}

export function deleteComment(id) {
  return request(`/api/binhluan/${id}`, {
    method: 'DELETE'
  })
}
