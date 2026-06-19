import { request } from './http'

export function voteQuestion(id, giaTri) {
  return request(`/api/cauhoi/${id}/binhchon`, {
    method: 'POST',
    body: JSON.stringify({ giaTri })
  })
}

export function voteAnswer(id, giaTri) {
  return request(`/api/cautraloi/${id}/binhchon`, {
    method: 'POST',
    body: JSON.stringify({ giaTri })
  })
}
