export function pick(obj, keys, fallback = '') {
  for (const key of keys) {
    if (obj?.[key] !== undefined && obj?.[key] !== null) return obj[key]
  }
  return fallback
}

export function idOf(obj, type = 'question') {
  const map = {
    user: ['iD_NguoiDung', 'ID_NguoiDung', 'id_NguoiDung', 'idNguoiDung', 'id'],
    question: ['iD_CauHoi', 'ID_CauHoi', 'id_CauHoi', 'idCauHoi', 'cauHoiId'],
    answer: ['iD_CauTraLoi', 'ID_CauTraLoi', 'id_CauTraLoi', 'idCauTraLoi'],
    comment: ['iD_BinhLuan', 'ID_BinhLuan', 'id_BinhLuan', 'idBinhLuan']
  }
  return Number(pick(obj, map[type] || [], 0))
}

export function normalizeQuestion(item = {}) {
  return {
    id: idOf(item, 'question'),
    userId: idOf(item, 'user'),
    hoTen: pick(item, ['hoTen', 'HoTen'], 'Người dùng'),
    idChuyenMuc: Number(pick(item, ['iD_ChuyenMuc', 'ID_ChuyenMuc', 'id_ChuyenMuc', 'idChuyenMuc'], 0)),
    tenChuyenMuc: pick(item, ['tenChuyenMuc', 'TenChuyenMuc'], 'Chưa phân loại'),
    tieuDe: pick(item, ['tieuDe', 'TieuDe'], ''),
    noiDung: pick(item, ['noiDung', 'NoiDung'], ''),
    luotXem: Number(pick(item, ['luotXem', 'LuotXem'], 0)),
    diemBinhChon: Number(pick(item, ['diemBinhChon', 'DiemBinhChon'], 0)),
    soCauTraLoi: Number(pick(item, ['soCauTraLoi', 'SoCauTraLoi'], 0)),
    soBinhLuan: Number(pick(item, ['soBinhLuan', 'SoBinhLuan'], 0)),
    tags: splitTags(pick(item, ['tags', 'Tags'], '')),
    isDeleted: Number(pick(item, ['isDeleted', 'IsDeleted'], 0)),
    ngayTao: pick(item, ['ngayTao', 'NgayTao'], ''),
    ngayCapNhat: pick(item, ['ngayCapNhat', 'NgayCapNhat'], null)
  }
}

export function normalizeAnswer(item = {}) {
  return {
    id: idOf(item, 'answer'),
    cauHoiId: idOf(item, 'question'),
    tieuDeCauHoi: pick(item, ['tieuDeCauHoi', 'TieuDeCauHoi'], ''),
    userId: idOf(item, 'user'),
    hoTen: pick(item, ['hoTen', 'HoTen'], 'Người dùng'),
    anhDaiDien: pick(item, ['anhDaiDien', 'AnhDaiDien'], null),
    noiDung: pick(item, ['noiDung', 'NoiDung'], ''),
    daChapNhan: Number(pick(item, ['daChapNhan', 'DaChapNhan'], 0)),
    diemBinhChon: Number(pick(item, ['diemBinhChon', 'DiemBinhChon'], 0)),
    soBinhLuan: Number(pick(item, ['soBinhLuan', 'SoBinhLuan'], 0)),
    isDeleted: Number(pick(item, ['isDeleted', 'IsDeleted'], 0)),
    ngayTao: pick(item, ['ngayTao', 'NgayTao'], ''),
    ngayCapNhat: pick(item, ['ngayCapNhat', 'NgayCapNhat'], null)
  }
}

export function normalizeComment(item = {}) {
  return {
    id: idOf(item, 'comment'),
    userId: idOf(item, 'user'),
    hoTen: pick(item, ['hoTen', 'HoTen'], 'Người dùng'),
    loaiDoiTuong: pick(item, ['loaiDoiTuong', 'LoaiDoiTuong'], ''),
    idDoiTuong: Number(pick(item, ['iD_DoiTuong', 'ID_DoiTuong', 'id_DoiTuong', 'idDoiTuong'], 0)),
    tieuDeDoiTuong: pick(item, ['tieuDeDoiTuong', 'TieuDeDoiTuong'], ''),
    noiDung: pick(item, ['noiDung', 'NoiDung'], ''),
    isDeleted: Number(pick(item, ['isDeleted', 'IsDeleted'], 0)),
    ngayTao: pick(item, ['ngayTao', 'NgayTao'], ''),
    ngayCapNhat: pick(item, ['ngayCapNhat', 'NgayCapNhat'], null)
  }
}

export function normalizeProfile(item = {}) {
  return {
    id: idOf(item, 'user'),
    hoTen: pick(item, ['hoTen', 'HoTen'], 'Người dùng'),
    email: pick(item, ['email', 'Email'], ''),
    anhDaiDien: pick(item, ['anhDaiDien', 'AnhDaiDien'], null),
    vaiTro: pick(item, ['vaiTro', 'VaiTro'], 'User'),
    trangThai: Number(pick(item, ['trangThai', 'TrangThai'], 1)),
    ngayTao: pick(item, ['ngayTao', 'NgayTao'], ''),
    soCauHoi: Number(pick(item, ['soCauHoi', 'SoCauHoi'], 0)),
    soCauTraLoi: Number(pick(item, ['soCauTraLoi', 'SoCauTraLoi'], 0)),
    soBinhLuan: Number(pick(item, ['soBinhLuan', 'SoBinhLuan'], 0)),
    soCauTraLoiDuocChapNhan: Number(pick(item, ['soCauTraLoiDuocChapNhan', 'SoCauTraLoiDuocChapNhan'], 0)),
    tongDiemBinhChon: Number(pick(item, ['tongDiemBinhChon', 'TongDiemBinhChon'], 0))
  }
}

export function splitTags(tags) {
  if (Array.isArray(tags)) return tags.filter(Boolean)
  return String(tags || '').split(',').map(x => x.trim()).filter(Boolean)
}

export function shortText(text, length = 160) {
  const value = String(text || '').replace(/\s+/g, ' ').trim()
  return value.length > length ? `${value.slice(0, length)}...` : value
}

export function formatDate(value) {
  if (!value) return 'Chưa có'
  const date = new Date(String(value).replace(' ', 'T'))
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export function initials(name = '') {
  const parts = String(name).trim().split(/\s+/).filter(Boolean)
  if (!parts.length) return '?'
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase()
  return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase()
}

export function isMine(ownerId, currentUser) {
  return Number(ownerId) > 0 && Number(ownerId) === Number(currentUser?.id_NguoiDung || currentUser?.id || 0)
}
