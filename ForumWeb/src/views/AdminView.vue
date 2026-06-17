<template>
  <div class="page-container admin-page">
    <div class="page-header">
      <div>
        <h1 class="page-title">Bảng quản trị</h1>
        <p class="page-subtitle">Theo dõi số liệu và quản lý nội dung trong hệ thống.</p>
      </div>
      <el-button type="primary" @click="loadAll">Làm mới</el-button>
    </div>

    <el-tabs v-model="activeTab" @tab-change="loadCurrentTab">
      <el-tab-pane label="Tổng quan" name="overview">
        <div class="stat-grid" v-loading="loadingDashboard">
          <MetricCard label="Người dùng" :value="num(dashboard, ['tongNguoiDung', 'TongNguoiDung'])" />
          <MetricCard label="Câu hỏi" :value="num(dashboard, ['tongCauHoi', 'TongCauHoi'])" />
          <MetricCard label="Câu trả lời" :value="num(dashboard, ['tongCauTraLoi', 'TongCauTraLoi'])" />
          <MetricCard label="Bình luận" :value="num(dashboard, ['tongBinhLuan', 'TongBinhLuan'])" />
        </div>
        <div class="chart-grid">
          <SimpleBarChart title="Nội dung theo loại" :items="arr(dashboard, ['noiDungTheoLoai', 'NoiDungTheoLoai'])" />
          <SimpleBarChart title="Trạng thái nội dung" :items="arr(dashboard, ['trangThaiNoiDung', 'TrangThaiNoiDung'])" />
          <SimpleBarChart title="Top tag được dùng" :items="arr(dashboard, ['topTags', 'TopTags'])" />
          <SimpleBarChart title="Câu hỏi theo chuyên mục" :items="arr(dashboard, ['cauHoiTheoChuyenMuc', 'CauHoiTheoChuyenMuc'])" />
        </div>
      </el-tab-pane>

      <el-tab-pane label="Người dùng" name="users">
        <div class="admin-toolbar">
          <el-input v-model="filters.users.keyword" placeholder="Tìm người dùng" clearable @keyup.enter="loadUsers" />
          <el-button type="primary" @click="loadUsers">Tải dữ liệu</el-button>
        </div>
        <el-table :data="users" v-loading="loading" border stripe class="responsive-table">
          <el-table-column label="ID" width="80"><template #default="s">{{ pick(s.row, ['iD_NguoiDung', 'ID_NguoiDung']) }}</template></el-table-column>
          <el-table-column label="Họ tên" min-width="180"><template #default="s">{{ pick(s.row, ['hoTen', 'HoTen']) }}</template></el-table-column>
          <el-table-column label="Email" min-width="220"><template #default="s">{{ pick(s.row, ['email', 'Email']) }}</template></el-table-column>
          <el-table-column label="Vai trò" width="120"><template #default="s"><el-tag :type="String(pick(s.row, ['vaiTro', 'VaiTro'])).toLowerCase() === 'admin' ? 'warning' : 'info'">{{ pick(s.row, ['vaiTro', 'VaiTro']) }}</el-tag></template></el-table-column>
          <el-table-column label="Trạng thái" width="130"><template #default="s"><el-tag :type="Number(pick(s.row, ['trangThai', 'TrangThai'], 1)) === 1 ? 'success' : 'danger'">{{ Number(pick(s.row, ['trangThai', 'TrangThai'], 1)) === 1 ? 'Hoạt động' : 'Bị khóa' }}</el-tag></template></el-table-column>
        </el-table>
      </el-tab-pane>

      <el-tab-pane label="Câu hỏi" name="questions">
        <div class="admin-toolbar">
          <el-input v-model="filters.questions.keyword" placeholder="Tìm câu hỏi" clearable @keyup.enter="loadQuestions" />
          <el-select v-model="filters.questions.isDeleted" placeholder="Trạng thái" clearable>
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>
          <el-button type="primary" @click="loadQuestions">Tải dữ liệu</el-button>
        </div>
        <el-table :data="questions" v-loading="loading" border stripe class="responsive-table">
          <el-table-column label="ID" width="80"><template #default="s">{{ idOf(s.row, 'question') }}</template></el-table-column>
          <el-table-column label="Tiêu đề" min-width="260"><template #default="s">{{ pick(s.row, ['tieuDe', 'TieuDe']) }}</template></el-table-column>
          <el-table-column label="Người đăng" min-width="150"><template #default="s">{{ pick(s.row, ['hoTen', 'HoTen']) }}</template></el-table-column>
          <el-table-column label="Vote" width="90"><template #default="s">{{ num(s.row, ['diemBinhChon', 'DiemBinhChon']) }}</template></el-table-column>
          <el-table-column label="Trạng thái" width="130"><template #default="s"><el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">{{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}</el-tag></template></el-table-column>
          <el-table-column label="Thao tác" width="180" fixed="right">
            <template #default="s">
              <el-button v-if="!isDeleted(s.row)" size="small" type="danger" plain @click="removeQuestion(s.row)">Xóa</el-button>
              <el-button v-else size="small" type="success" plain @click="restoreQuestion(s.row)">Khôi phục</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <el-tab-pane label="Câu trả lời" name="answers">
        <div class="admin-toolbar">
          <el-input v-model="filters.answers.cauHoiId" placeholder="Lọc theo ID câu hỏi" clearable @keyup.enter="loadAnswers" />
          <el-select v-model="filters.answers.isDeleted" placeholder="Trạng thái" clearable>
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>
          <el-button type="primary" @click="loadAnswers">Tải dữ liệu</el-button>
        </div>
        <el-table :data="answers" v-loading="loading" border stripe class="responsive-table">
          <el-table-column label="ID" width="80"><template #default="s">{{ idOf(s.row, 'answer') }}</template></el-table-column>
          <el-table-column label="Câu hỏi" min-width="220"><template #default="s">{{ pick(s.row, ['tieuDeCauHoi', 'TieuDeCauHoi']) }}</template></el-table-column>
          <el-table-column label="Nội dung" min-width="280"><template #default="s">{{ shortText(pick(s.row, ['noiDung', 'NoiDung']), 100) }}</template></el-table-column>
          <el-table-column label="Người trả lời" min-width="150"><template #default="s">{{ pick(s.row, ['hoTen', 'HoTen']) }}</template></el-table-column>
          <el-table-column label="Trạng thái" width="130"><template #default="s"><el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">{{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}</el-tag></template></el-table-column>
          <el-table-column label="Thao tác" width="180" fixed="right">
            <template #default="s">
              <el-button v-if="!isDeleted(s.row)" size="small" type="danger" plain @click="removeAnswer(s.row)">Xóa</el-button>
              <el-button v-else size="small" type="success" plain @click="restoreAnswer(s.row)">Khôi phục</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <el-tab-pane label="Bình luận" name="comments">
        <div class="admin-toolbar">
          <el-select v-model="filters.comments.loaiDoiTuong" placeholder="Loại nội dung" clearable>
            <el-option label="Câu hỏi" value="CAUHOI" />
            <el-option label="Câu trả lời" value="CAUTRALOI" />
          </el-select>
          <el-select v-model="filters.comments.isDeleted" placeholder="Trạng thái" clearable>
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>
          <el-button type="primary" @click="loadComments">Tải dữ liệu</el-button>
        </div>
        <el-table :data="comments" v-loading="loading" border stripe class="responsive-table">
          <el-table-column label="ID" width="80"><template #default="s">{{ idOf(s.row, 'comment') }}</template></el-table-column>
          <el-table-column label="Loại" width="130"><template #default="s">{{ pick(s.row, ['loaiDoiTuong', 'LoaiDoiTuong']) }}</template></el-table-column>
          <el-table-column label="Nội dung" min-width="300"><template #default="s">{{ shortText(pick(s.row, ['noiDung', 'NoiDung']), 110) }}</template></el-table-column>
          <el-table-column label="Người bình luận" min-width="150"><template #default="s">{{ pick(s.row, ['hoTen', 'HoTen']) }}</template></el-table-column>
          <el-table-column label="Trạng thái" width="130"><template #default="s"><el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">{{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}</el-tag></template></el-table-column>
          <el-table-column label="Thao tác" width="180" fixed="right">
            <template #default="s">
              <el-button v-if="!isDeleted(s.row)" size="small" type="danger" plain @click="removeComment(s.row)">Xóa</el-button>
              <el-button v-else size="small" type="success" plain @click="restoreComment(s.row)">Khôi phục</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getAdminDashboard, getUsers, getAdminQuestions, deleteAdminQuestion, restoreAdminQuestion, getAdminAnswers, deleteAdminAnswer, restoreAdminAnswer, getAdminComments, deleteAdminComment, restoreAdminComment } from '../api/adminApi'
import { pick, idOf, shortText } from '../utils/format'
import MetricCard from '../components/MetricCard.vue'
import SimpleBarChart from '../components/SimpleBarChart.vue'

const activeTab = ref('overview')
const loading = ref(false)
const loadingDashboard = ref(false)
const dashboard = ref({})
const users = ref([])
const questions = ref([])
const answers = ref([])
const comments = ref([])
const filters = reactive({
  users: { keyword: '' },
  questions: { keyword: '', isDeleted: '' },
  answers: { cauHoiId: '', isDeleted: '' },
  comments: { loaiDoiTuong: '', isDeleted: '' }
})

onMounted(loadAll)

function num(obj, keys) { return Number(pick(obj, keys, 0)) || 0 }
function arr(obj, keys) { return pick(obj, keys, []) || [] }
function isDeleted(row) { return Number(pick(row, ['isDeleted', 'IsDeleted'], 0)) === 1 }

async function loadAll() {
  await loadDashboard()
  await loadUsers()
}

async function loadDashboard() {
  loadingDashboard.value = true
  try { dashboard.value = await getAdminDashboard() }
  catch (error) { ElMessage.error(error.message || 'Không tải được dashboard quản trị.') }
  finally { loadingDashboard.value = false }
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
  try { await fn() }
  catch (error) { ElMessage.error(error.message || 'Không tải được dữ liệu quản trị.') }
  finally { loading.value = false }
}

async function loadUsers() { await runLoad(async () => { users.value = await getUsers({ keyword: filters.users.keyword }) }) }
async function loadQuestions() { await runLoad(async () => { questions.value = await getAdminQuestions(filters.questions) }) }
async function loadAnswers() { await runLoad(async () => { answers.value = await getAdminAnswers(filters.answers) }) }
async function loadComments() { await runLoad(async () => { comments.value = await getAdminComments(filters.comments) }) }

async function confirmAction(message, action, reload) {
  try {
    await ElMessageBox.confirm(message, 'Xác nhận', { type: 'warning' })
    await action()
    ElMessage.success('Thao tác thành công.')
    await reload()
    await loadDashboard()
  } catch (error) {
    if (error !== 'cancel') ElMessage.error(error.message || 'Thao tác thất bại.')
  }
}

function removeQuestion(row) { return confirmAction('Admin sẽ xóa mềm câu hỏi này?', () => deleteAdminQuestion(idOf(row, 'question')), loadQuestions) }
function restoreQuestion(row) { return confirmAction('Khôi phục câu hỏi này?', () => restoreAdminQuestion(idOf(row, 'question')), loadQuestions) }
function removeAnswer(row) { return confirmAction('Admin sẽ xóa mềm câu trả lời này?', () => deleteAdminAnswer(idOf(row, 'answer')), loadAnswers) }
function restoreAnswer(row) { return confirmAction('Khôi phục câu trả lời này?', () => restoreAdminAnswer(idOf(row, 'answer')), loadAnswers) }
function removeComment(row) { return confirmAction('Admin sẽ xóa mềm bình luận này?', () => deleteAdminComment(idOf(row, 'comment')), loadComments) }
function restoreComment(row) { return confirmAction('Khôi phục bình luận này?', () => restoreAdminComment(idOf(row, 'comment')), loadComments) }
</script>

<style scoped>
.admin-page :deep(.el-tabs__content) { padding-top: 14px; }
.chart-grid { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 16px; margin-top: 16px; }
.admin-toolbar { display: flex; gap: 10px; flex-wrap: wrap; margin-bottom: 14px; }
.admin-toolbar .el-input, .admin-toolbar .el-select { width: 240px; }
.responsive-table { width: 100%; }
@media (max-width: 980px) { .chart-grid { grid-template-columns: 1fr; } }
@media (max-width: 760px) { .admin-toolbar .el-input, .admin-toolbar .el-select, .admin-toolbar .el-button { width: 100%; } }
</style>
