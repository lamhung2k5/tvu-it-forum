import { request, buildQuery } from './http'

export function createReport(data) {
  return request('/api/tocao', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function getAdminReports(params = {}) {
  return request(`/api/admin/tocao${buildQuery(params)}`)
}

export function getPendingReportCount() {
  return request('/api/admin/tocao/pending-count')
}

export function rejectReport(id, data = {}) {
  return request(`/api/admin/tocao/${id}/reject`, {
    method: 'PATCH',
    body: JSON.stringify(data)
  })
}

export function remindReport(id, data = {}) {
  return request(`/api/admin/tocao/${id}/remind`, {
    method: 'PATCH',
    body: JSON.stringify(data)
  })
}

export function resolveReport(id, data = {}) {
  return request(`/api/admin/tocao/${id}/resolve`, {
    method: 'PATCH',
    body: JSON.stringify(data)
  })
}
