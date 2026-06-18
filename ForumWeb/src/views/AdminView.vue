<template>
  <div class="page-container admin-page">
    <div class="page-header">
      <div>
        <h1 class="page-title">Bảng quản trị</h1>
        <p class="page-subtitle">
          Theo dõi số liệu và quản lý nội dung trong hệ thống.
        </p>
      </div>

      <el-button type="primary" @click="loadAll">
        Làm mới
      </el-button>
    </div>

    <el-tabs v-model="activeTab" @tab-change="loadCurrentTab">
      <el-tab-pane label="Tổng quan" name="overview">
        <div class="stat-grid" v-loading="loadingDashboard">
          <MetricCard
            label="Người dùng"
            :value="num(dashboard, ['tongNguoiDung', 'TongNguoiDung'])"
          />

          <MetricCard
            label="Câu hỏi"
            :value="num(dashboard, ['tongCauHoi', 'TongCauHoi'])"
          />

          <MetricCard
            label="Câu trả lời"
            :value="num(dashboard, ['tongCauTraLoi', 'TongCauTraLoi'])"
          />

          <MetricCard
            label="Bình luận"
            :value="num(dashboard, ['tongBinhLuan', 'TongBinhLuan'])"
          />
        </div>

        <div class="chart-grid">
          <SimpleBarChart
            title="Nội dung theo loại"
            :items="arr(dashboard, ['noiDungTheoLoai', 'NoiDungTheoLoai'])"
          />

          <SimpleBarChart
            title="Trạng thái nội dung"
            :items="arr(dashboard, ['trangThaiNoiDung', 'TrangThaiNoiDung'])"
          />

          <SimpleBarChart
            title="Top tag được dùng"
            :items="arr(dashboard, ['topTags', 'TopTags'])"
          />

          <SimpleBarChart
            title="Câu hỏi theo chuyên mục"
            :items="arr(dashboard, ['cauHoiTheoChuyenMuc', 'CauHoiTheoChuyenMuc'])"
          />
        </div>
      </el-tab-pane>

      <el-tab-pane label="Người dùng" name="users">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.users.keyword"
            class="admin-search"
            placeholder="Tìm tên hoặc email"
            clearable
            @keyup.enter="loadUsers"
          />

          <el-button type="primary" @click="loadUsers">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedUsers"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ pick(s.row, ['iD_NguoiDung', 'ID_NguoiDung']) }}
            </template>
          </el-table-column>

          <el-table-column label="Họ tên" min-width="170" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column label="Email" min-width="220" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['email', 'Email']) }}
            </template>
          </el-table-column>

          <el-table-column label="Vai trò" width="110" align="center">
            <template #default="s">
              <el-tag :type="isAdminRole(s.row) ? 'warning' : 'info'">
                {{ pick(s.row, ['vaiTro', 'VaiTro']) }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="120" align="center">
            <template #default="s">
              <el-tag :type="isActiveUser(s.row) ? 'success' : 'danger'">
                {{ isActiveUser(s.row) ? 'Hoạt động' : 'Bị khóa' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column
            label="Ngày tạo"
            width="140"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              {{ displayDate(s.row) }}
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="230" align="center">
            <template #default="s">
              <el-button
                v-if="isActiveUser(s.row)"
                size="small"
                type="warning"
                plain
                @click="lockUser(s.row)"
              >
                Khóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="unlockUser(s.row)"
              >
                Mở khóa
              </el-button>

              <el-button
                v-if="isAdminRole(s.row)"
                size="small"
                type="info"
                plain
                @click="changeUserRole(s.row, 'User')"
              >
                Hạ User
              </el-button>

              <el-button
                v-else
                size="small"
                type="primary"
                plain
                @click="changeUserRole(s.row, 'Admin')"
              >
                Cấp Admin
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="users.length"
          :page="pagination.users.page"
          :page-size="pagination.users.pageSize"
          @page-change="page => handlePageChange('users', page)"
          @page-size-change="pageSize => handlePageSizeChange('users', pageSize)"
        />
      </el-tab-pane>

      <el-tab-pane label="Câu hỏi" name="questions">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.questions.keyword"
            class="admin-search"
            placeholder="Tìm tiêu đề, tác giả"
            clearable
            @keyup.enter="loadQuestions"
          />

          <el-select
            v-model="filters.questions.isDeleted"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>

          <el-button type="primary" @click="loadQuestions">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedQuestions"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ idOf(s.row, 'question') }}
            </template>
          </el-table-column>

          <el-table-column label="Câu hỏi" min-width="360">
            <template #default="s">
              <div class="admin-title-block">
                <div class="admin-main-text">
                  {{ pick(s.row, ['tieuDe', 'TieuDe']) }}
                </div>

                <div class="admin-sub-text">
                  Chuyên mục:
                  {{ pick(s.row, ['tenChuyenMuc', 'TenChuyenMuc'], 'Chưa phân loại') }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Người đăng" min-width="140" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column label="Vote" width="80" align="center">
            <template #default="s">
              {{ num(s.row, ['diemBinhChon', 'DiemBinhChon']) }}
            </template>
          </el-table-column>

          <el-table-column
            label="Thời gian"
            width="155"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              <div class="time-cell">
                <div>
                  <span class="time-label">Tạo:</span>
                  {{ displayDate(s.row) }}
                </div>

                <div>
                  <span class="time-label">Sửa:</span>
                  {{ displayUpdatedDate(s.row) }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="110" align="center">
            <template #default="s">
              <el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">
                {{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="120" align="center">
            <template #default="s">
              <el-button
                v-if="!isDeleted(s.row)"
                size="small"
                type="danger"
                plain
                @click="removeQuestion(s.row)"
              >
                Xóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="restoreQuestion(s.row)"
              >
                Khôi phục
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="questions.length"
          :page="pagination.questions.page"
          :page-size="pagination.questions.pageSize"
          @page-change="page => handlePageChange('questions', page)"
          @page-size-change="pageSize => handlePageSizeChange('questions', pageSize)"
        />
      </el-tab-pane>

      <el-tab-pane label="Câu trả lời" name="answers">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.answers.keyword"
            class="admin-search"
            placeholder="Tìm nội dung, người trả lời"
            clearable
            @keyup.enter="loadAnswers"
          />

          <el-select
            v-model="filters.answers.isDeleted"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>

          <el-button type="primary" @click="loadAnswers">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedAnswers"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ idOf(s.row, 'answer') }}
            </template>
          </el-table-column>

          <el-table-column label="Câu trả lời" min-width="430">
            <template #default="s">
              <div class="admin-title-block">
                <div class="admin-main-text">
                  {{ pick(s.row, ['noiDung', 'NoiDung']) }}
                </div>

                <div class="admin-sub-text">
                  Trong câu hỏi:
                  {{ pick(s.row, ['tieuDeCauHoi', 'TieuDeCauHoi'], '—') }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Người trả lời" min-width="140" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column
            label="Thời gian"
            width="155"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              <div class="time-cell">
                <div>
                  <span class="time-label">Tạo:</span>
                  {{ displayDate(s.row) }}
                </div>

                <div>
                  <span class="time-label">Sửa:</span>
                  {{ displayUpdatedDate(s.row) }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="110" align="center">
            <template #default="s">
              <el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">
                {{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="120" align="center">
            <template #default="s">
              <el-button
                v-if="!isDeleted(s.row)"
                size="small"
                type="danger"
                plain
                @click="removeAnswer(s.row)"
              >
                Xóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="restoreAnswer(s.row)"
              >
                Khôi phục
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="filteredAnswers.length"
          :page="pagination.answers.page"
          :page-size="pagination.answers.pageSize"
          @page-change="page => handlePageChange('answers', page)"
          @page-size-change="pageSize => handlePageSizeChange('answers', pageSize)"
        />
      </el-tab-pane>

      <el-tab-pane label="Bình luận" name="comments">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.comments.keyword"
            class="admin-search"
            placeholder="Tìm nội dung, người bình luận"
            clearable
            @keyup.enter="loadComments"
          />

          <el-select
            v-model="filters.comments.loaiDoiTuong"
            placeholder="Loại nội dung"
            clearable
          >
            <el-option label="Câu hỏi" value="CAUHOI" />
            <el-option label="Câu trả lời" value="CAUTRALOI" />
          </el-select>

          <el-select
            v-model="filters.comments.isDeleted"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>

          <el-button type="primary" @click="loadComments">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedComments"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ idOf(s.row, 'comment') }}
            </template>
          </el-table-column>

          <el-table-column label="Bình luận" min-width="450">
            <template #default="s">
              <div class="admin-title-block">
                <div class="admin-main-text">
                  {{ pick(s.row, ['noiDung', 'NoiDung']) }}
                </div>

                <div class="admin-sub-text">
                  {{ pick(s.row, ['loaiDoiTuong', 'LoaiDoiTuong'], '—') }}
                  · Thuộc:
                  {{ pick(s.row, ['tieuDeDoiTuong', 'TieuDeDoiTuong', 'tieuDeCauHoi', 'TieuDeCauHoi'], '—') }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Người bình luận" min-width="140" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column
            label="Thời gian"
            width="155"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              <div class="time-cell">
                <div>
                  <span class="time-label">Tạo:</span>
                  {{ displayDate(s.row) }}
                </div>

                <div>
                  <span class="time-label">Sửa:</span>
                  {{ displayUpdatedDate(s.row) }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="110" align="center">
            <template #default="s">
              <el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">
                {{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="120" align="center">
            <template #default="s">
              <el-button
                v-if="!isDeleted(s.row)"
                size="small"
                type="danger"
                plain
                @click="removeComment(s.row)"
              >
                Xóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="restoreComment(s.row)"
              >
                Khôi phục
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="filteredComments.length"
          :page="pagination.comments.page"
          :page-size="pagination.comments.pageSize"
          @page-change="page => handlePageChange('comments', page)"
          @page-size-change="pageSize => handlePageSizeChange('comments', pageSize)"
        />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getAdminDashboard,
  getUsers,
  getAdminQuestions,
  deleteAdminQuestion,
  restoreAdminQuestion,
  getAdminAnswers,
  deleteAdminAnswer,
  restoreAdminAnswer,
  getAdminComments,
  deleteAdminComment,
  restoreAdminComment,
  lockAdminUser,
  unlockAdminUser,
  updateAdminUserRole
} from '../api/adminApi'
import { pick, idOf, formatDate } from '../utils/format'
import MetricCard from '../components/MetricCard.vue'
import SimpleBarChart from '../components/SimpleBarChart.vue'
import ForumPagination from '../components/ForumPagination.vue'

const activeTab = ref('overview')
const loading = ref(false)
const loadingDashboard = ref(false)

const dashboard = ref({})
const users = ref([])
const questions = ref([])
const answers = ref([])
const comments = ref([])

const pagination = reactive({
  users: {
    page: 1,
    pageSize: 5
  },
  questions: {
    page: 1,
    pageSize: 5
  },
  answers: {
    page: 1,
    pageSize: 5
  },
  comments: {
    page: 1,
    pageSize: 5
  }
})

const filters = reactive({
  users: { keyword: '' },
  questions: { keyword: '', isDeleted: '' },
  answers: { keyword: '', isDeleted: '' },
  comments: { keyword: '', loaiDoiTuong: '', isDeleted: '' }
})

const filteredAnswers = computed(() => {
  const keyword = normalizeKeyword(filters.answers.keyword)

  if (!keyword) return answers.value

  return answers.value.filter(row => {
    return includesKeyword(row, keyword, [
      ['tieuDeCauHoi', 'TieuDeCauHoi'],
      ['noiDung', 'NoiDung'],
      ['hoTen', 'HoTen']
    ])
  })
})

const filteredComments = computed(() => {
  const keyword = normalizeKeyword(filters.comments.keyword)

  if (!keyword) return comments.value

  return comments.value.filter(row => {
    return includesKeyword(row, keyword, [
      ['noiDung', 'NoiDung'],
      ['hoTen', 'HoTen'],
      ['loaiDoiTuong', 'LoaiDoiTuong'],
      ['tieuDeDoiTuong', 'TieuDeDoiTuong'],
      ['tieuDeCauHoi', 'TieuDeCauHoi']
    ])
  })
})

const pagedUsers = computed(() => {
  return paginate(users.value, pagination.users)
})

const pagedQuestions = computed(() => {
  return paginate(questions.value, pagination.questions)
})

const pagedAnswers = computed(() => {
  return paginate(filteredAnswers.value, pagination.answers)
})

const pagedComments = computed(() => {
  return paginate(filteredComments.value, pagination.comments)
})

onMounted(loadAll)

watch(
  () => filters.users.keyword,
  () => {
    pagination.users.page = 1
  }
)

watch(
  () => [filters.questions.keyword, filters.questions.isDeleted],
  () => {
    pagination.questions.page = 1
  }
)

watch(
  () => [filters.answers.keyword, filters.answers.isDeleted],
  () => {
    pagination.answers.page = 1
  }
)

watch(
  () => [
    filters.comments.keyword,
    filters.comments.loaiDoiTuong,
    filters.comments.isDeleted
  ],
  () => {
    pagination.comments.page = 1
  }
)

function num(obj, keys) {
  return Number(pick(obj, keys, 0)) || 0
}

function arr(obj, keys) {
  return pick(obj, keys, []) || []
}

function isDeleted(row) {
  return Number(pick(row, ['isDeleted', 'IsDeleted'], 0)) === 1
}

function isActiveUser(row) {
  return Number(pick(row, ['trangThai', 'TrangThai'], 1)) === 1
}

function isAdminRole(row) {
  return String(pick(row, ['vaiTro', 'VaiTro'], '')).toLowerCase() === 'admin'
}

function normalizeKeyword(value) {
  return String(value || '').trim().toLowerCase()
}

function includesKeyword(row, keyword, keyGroups) {
  return keyGroups.some(keys => {
    return String(pick(row, keys, '') || '').toLowerCase().includes(keyword)
  })
}

function paginate(list, config) {
  const start = (config.page - 1) * config.pageSize
  const end = start + config.pageSize

  return list.slice(start, end)
}

function handlePageChange(type, page) {
  pagination[type].page = page
}

function handlePageSizeChange(type, pageSize) {
  pagination[type].pageSize = pageSize
  pagination[type].page = 1
}

function parseDateValue(value) {
  if (!value) return 0

  const date = new Date(String(value).replace(' ', 'T'))

  return Number.isNaN(date.getTime()) ? 0 : date.getTime()
}

function createdDateValue(row) {
  return parseDateValue(pick(row, ['ngayTao', 'NgayTao'], ''))
}

function updatedDateValue(row) {
  return parseDateValue(pick(row, ['ngayCapNhat', 'NgayCapNhat'], ''))
}

function sortByNgayTao(a, b) {
  return createdDateValue(a) - createdDateValue(b)
}

function displayDate(row) {
  const value = pick(row, ['ngayTao', 'NgayTao'], '')

  if (!value) return '—'

  return formatDate(value)
}

function displayUpdatedDate(row) {
  const updatedValue = pick(row, ['ngayCapNhat', 'NgayCapNhat'], '')

  if (!updatedValue) return '—'

  const createdTime = createdDateValue(row)
  const updatedTime = parseDateValue(updatedValue)

  if (!updatedTime) return '—'

  if (createdTime && createdTime === updatedTime) {
    return '—'
  }

  return formatDate(updatedValue)
}

function userId(row) {
  return Number(pick(row, ['iD_NguoiDung', 'ID_NguoiDung'], 0))
}

function userName(row) {
  return pick(row, ['hoTen', 'HoTen'], 'người dùng này')
}

function lockUser(row) {
  return confirmAction(
    `Khóa tài khoản "${userName(row)}"? Người dùng này sẽ không thể đăng nhập.`,
    () => lockAdminUser(userId(row)),
    loadUsers
  )
}

function unlockUser(row) {
  return confirmAction(
    `Mở khóa tài khoản "${userName(row)}"?`,
    () => unlockAdminUser(userId(row)),
    loadUsers
  )
}

function changeUserRole(row, role) {
  const actionText = role === 'Admin'
    ? 'cấp quyền Admin cho'
    : 'hạ quyền về User cho'

  return confirmAction(
    `Bạn có chắc muốn ${actionText} "${userName(row)}"?`,
    () => updateAdminUserRole(userId(row), role),
    loadUsers
  )
}

async function loadAll() {
  await loadDashboard()

  if (activeTab.value === 'overview') return

  await loadCurrentTab()
}

async function loadDashboard() {
  loadingDashboard.value = true

  try {
    dashboard.value = await getAdminDashboard()
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được dashboard quản trị.')
  } finally {
    loadingDashboard.value = false
  }
}

function loadCurrentTab() {
  if (activeTab.value === 'overview') return loadDashboard()
  if (activeTab.value === 'users') return loadUsers()
  if (activeTab.value === 'questions') return loadQuestions()
  if (activeTab.value === 'answers') return loadAnswers()
  if (activeTab.value === 'comments') return loadComments()
}

async function runLoad(fn) {
  loading.value = true

  try {
    await fn()
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được dữ liệu quản trị.')
  } finally {
    loading.value = false
  }
}

async function loadUsers() {
  await runLoad(async () => {
    users.value = await getUsers({
      keyword: filters.users.keyword
    })

    pagination.users.page = 1
  })
}

async function loadQuestions() {
  await runLoad(async () => {
    questions.value = await getAdminQuestions({
      keyword: filters.questions.keyword,
      isDeleted: filters.questions.isDeleted
    })

    pagination.questions.page = 1
  })
}

async function loadAnswers() {
  await runLoad(async () => {
    answers.value = await getAdminAnswers({
      isDeleted: filters.answers.isDeleted
    })

    pagination.answers.page = 1
  })
}

async function loadComments() {
  await runLoad(async () => {
    comments.value = await getAdminComments({
      loaiDoiTuong: filters.comments.loaiDoiTuong,
      isDeleted: filters.comments.isDeleted
    })

    pagination.comments.page = 1
  })
}

async function confirmAction(message, action, reload) {
  try {
    await ElMessageBox.confirm(message, 'Xác nhận', {
      type: 'warning',
      confirmButtonText: 'Đồng ý',
      cancelButtonText: 'Hủy'
    })

    const result = await action()

    ElMessage.success(
      result?.message ||
      result?.Message ||
      'Thao tác thành công.'
    )

    await reload()
    await loadDashboard()
  } catch (error) {
    if (error === 'cancel') return

    ElMessage.error(
      error?.data?.message ||
      error?.data?.Message ||
      error?.message ||
      'Thao tác thất bại.'
    )
  }
}

function removeQuestion(row) {
  return confirmAction(
    'Admin sẽ xóa mềm câu hỏi này?',
    () => deleteAdminQuestion(idOf(row, 'question')),
    loadQuestions
  )
}

function restoreQuestion(row) {
  return confirmAction(
    'Khôi phục câu hỏi này?',
    () => restoreAdminQuestion(idOf(row, 'question')),
    loadQuestions
  )
}

function removeAnswer(row) {
  return confirmAction(
    'Admin sẽ xóa mềm câu trả lời này?',
    () => deleteAdminAnswer(idOf(row, 'answer')),
    loadAnswers
  )
}

function restoreAnswer(row) {
  return confirmAction(
    'Khôi phục câu trả lời này?',
    () => restoreAdminAnswer(idOf(row, 'answer')),
    loadAnswers
  )
}

function removeComment(row) {
  return confirmAction(
    'Admin sẽ xóa mềm bình luận này?',
    () => deleteAdminComment(idOf(row, 'comment')),
    loadComments
  )
}

function restoreComment(row) {
  return confirmAction(
    'Khôi phục bình luận này?',
    () => restoreAdminComment(idOf(row, 'comment')),
    loadComments
  )
}
</script>

<style scoped>
.admin-page :deep(.el-tabs__content) {
  padding-top: 14px;
}

.chart-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
  margin-top: 16px;
}

.admin-toolbar {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
  margin-bottom: 14px;
  align-items: center;
}

.admin-toolbar .admin-search {
  width: 340px;
}

.admin-toolbar .el-select {
  width: 220px;
}

.admin-toolbar .el-button {
  min-width: 110px;
}

.responsive-table {
  width: 100%;
}

.responsive-table :deep(.el-table__cell) {
  vertical-align: middle;
  padding: 8px 0;
}

.responsive-table :deep(.el-table__header th) {
  color: var(--forum-text);
  font-weight: 600;
  background: #fbfcff;
}

.responsive-table :deep(.el-table__fixed-right),
.responsive-table :deep(.el-table__fixed-right::before),
.responsive-table :deep(.el-table__fixed-right-patch) {
  box-shadow: none;
}

.admin-title-block {
  min-width: 0;
}

.admin-main-text {
  color: var(--forum-text);
  font-weight: 400;
  line-height: 1.45;
  white-space: normal;
  overflow: visible;
}

.admin-sub-text {
  margin-top: 4px;
  color: var(--forum-muted);
  font-size: 12px;
  line-height: 1.35;
  white-space: normal;
  overflow: visible;
}

.time-cell {
  color: var(--forum-muted);
  font-size: 12px;
  line-height: 1.6;
  white-space: nowrap;
}

.time-label {
  color: var(--forum-muted);
  font-weight: 400;
}

@media (max-width: 980px) {
  .chart-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 760px) {
  .admin-toolbar .el-input,
  .admin-toolbar .el-select,
  .admin-toolbar .el-button {
    width: 100%;
  }
}
</style>