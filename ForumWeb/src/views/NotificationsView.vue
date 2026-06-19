<template>
  <div class="notifications-page">
    <div class="notifications-header">
      <div>
        <h1>Thông báo của tôi</h1>
        <p>Quản lý thông báo tương tác và thông báo từ hệ thống.</p>
      </div>

      <div class="header-actions">
        <el-button
          :disabled="total === 0"
          @click="markAllRead"
        >
          Đánh dấu tất cả đã đọc
        </el-button>

        <el-button
          type="danger"
          plain
          :disabled="total === 0"
          @click="clearRead"
        >
          Dọn thông báo đã đọc
        </el-button>
      </div>
    </div>

    <div class="filter-panel">
      <div class="filter-left">
        <el-radio-group
          v-model="filters.status"
          @change="reloadFromFirstPage"
        >
          <el-radio-button label="all">Tất cả</el-radio-button>
          <el-radio-button label="unread">Chưa đọc</el-radio-button>
          <el-radio-button label="read">Đã đọc</el-radio-button>
        </el-radio-group>
      </div>

      <div class="filter-right">
        <el-select
          v-model="filters.category"
          class="filter-select"
          placeholder="Loại thông báo"
          @change="reloadFromFirstPage"
        >
          <el-option label="Tất cả loại thông báo" value="all" />
          <el-option label="Tương tác" value="interaction" />
          <el-option label="Quản trị" value="admin" />
        </el-select>

        <el-select
          v-model="filters.timeRange"
          class="filter-select time-select"
          placeholder="Thời gian"
          @change="reloadFromFirstPage"
        >
          <el-option label="Tất cả thời gian" value="all" />
          <el-option label="Hôm nay" value="today" />
          <el-option label="Tuần này" value="week" />
          <el-option label="Tháng này" value="month" />
          <el-option label="Năm nay" value="year" />
        </el-select>

        <el-button @click="resetFilters">
          Xóa lọc
        </el-button>
      </div>
    </div>

    <div class="management-card">
      <div class="management-card-header">
        <div>
          <h2>Danh sách thông báo</h2>
          <p>{{ total }} thông báo</p>
        </div>

        <el-button
          link
          type="danger"
          :disabled="total === 0"
          @click="clearAll"
        >
          Xóa tất cả thông báo
        </el-button>
      </div>

      <el-empty
        v-if="!loading && notifications.length === 0"
        description="Không có thông báo phù hợp"
      />

      <div
        v-else
        v-loading="loading"
        class="notification-table"
      >
        <div
          v-for="item in notifications"
          :key="notificationId(item)"
          class="notification-row"
          :class="{ unread: !isRead(item) }"
        >
          <div class="read-indicator">
            <span
              v-if="!isRead(item)"
              class="status-dot"
            />
          </div>

          <div class="notification-main">
            <div class="notification-title-line">
              <h3>{{ pick(item, ['tieuDe', 'TieuDe'], 'Thông báo mới') }}</h3>

              <el-tag
                size="small"
                :type="tagType(item)"
                effect="light"
              >
                {{ notificationTypeLabel(item) }}
              </el-tag>
            </div>

            <p class="notification-content">
              {{ pick(item, ['noiDung', 'NoiDung'], '') }}
            </p>

            <div class="notification-meta">
              <span>{{ formatDate(pick(item, ['ngayTao', 'NgayTao'], '')) }}</span>
              <span>{{ isRead(item) ? 'Đã đọc' : 'Chưa đọc' }}</span>
            </div>
          </div>

          <div class="row-actions">
            <el-button
              size="small"
              @click="openNotification(item)"
            >
              Xem
            </el-button>

            <el-button
              v-if="!isRead(item)"
              size="small"
              plain
              @click="markOneRead(item)"
            >
              Đã đọc
            </el-button>

            <el-button
              size="small"
              type="danger"
              plain
              @click="deleteOne(item)"
            >
              Xóa
            </el-button>
          </div>
        </div>
      </div>

      <ForumPagination
        :total="total"
        :page="pagination.page"
        :page-size="pagination.pageSize"
        :page-sizes="[5, 10, 20]"
        @page-change="handlePageChange"
        @page-size-change="handlePageSizeChange"
      />
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  clearAllNotifications,
  clearReadNotifications,
  deleteNotification,
  getNotifications,
  markAllNotificationsAsRead,
  markNotificationAsRead
} from '../api/thongBaoApi'
import { pick, formatDate } from '../utils/format'
import ForumPagination from '../components/ForumPagination.vue'

const router = useRouter()

const loading = ref(false)
const notifications = ref([])
const total = ref(0)

const filters = reactive({
  status: 'all',
  category: 'all',
  timeRange: 'all'
})

const pagination = reactive({
  page: 1,
  pageSize: 10
})

onMounted(loadNotifications)

async function loadNotifications() {
  loading.value = true

  try {
    const result = await getNotifications({
      status: filters.status,
      category: filters.category,
      timeRange: filters.timeRange,
      page: pagination.page,
      pageSize: pagination.pageSize
    })

    notifications.value = pick(result, ['items', 'Items'], []) || []
    total.value = Number(pick(result, ['total', 'Total'], 0)) || 0
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được thông báo.')
  } finally {
    loading.value = false
  }
}

function reloadFromFirstPage() {
  pagination.page = 1
  loadNotifications()
}

function resetFilters() {
  filters.status = 'all'
  filters.category = 'all'
  filters.timeRange = 'all'
  reloadFromFirstPage()
}

function handlePageChange(page) {
  pagination.page = page
  loadNotifications()
}

function handlePageSizeChange(pageSize) {
  pagination.pageSize = pageSize
  pagination.page = 1
  loadNotifications()
}

async function openNotification(item) {
  const id = notificationId(item)
  const link = pick(item, ['link', 'Link'], '')

  try {
    if (id && !isRead(item)) {
      await markNotificationAsRead(id)
    }

    if (link) {
      router.push(link)
      return
    }

    await loadNotifications()
  } catch (error) {
    ElMessage.error(error.message || 'Không thể mở thông báo.')
  }
}

async function markOneRead(item) {
  const id = notificationId(item)

  try {
    await markNotificationAsRead(id)
    ElMessage.success('Đã đánh dấu thông báo là đã đọc.')
    await loadNotifications()
  } catch (error) {
    ElMessage.error(error.message || 'Không thể đánh dấu thông báo.')
  }
}

async function markAllRead() {
  try {
    await markAllNotificationsAsRead()
    ElMessage.success('Đã đánh dấu tất cả thông báo là đã đọc.')
    await loadNotifications()
  } catch (error) {
    ElMessage.error(error.message || 'Không thể đánh dấu thông báo.')
  }
}

async function deleteOne(item) {
  const id = notificationId(item)

  try {
    await ElMessageBox.confirm(
      'Bạn có chắc muốn xóa thông báo này không?',
      'Xác nhận xóa',
      {
        type: 'warning',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
      }
    )

    await deleteNotification(id)
    ElMessage.success('Đã xóa thông báo.')
    await loadNotifications()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể xóa thông báo.')
    }
  }
}

async function clearRead() {
  try {
    await ElMessageBox.confirm(
      'Các thông báo đã đọc sẽ được dọn khỏi danh sách của bạn. Bạn có muốn tiếp tục?',
      'Dọn thông báo đã đọc',
      {
        type: 'warning',
        confirmButtonText: 'Dọn',
        cancelButtonText: 'Hủy'
      }
    )

    await clearReadNotifications()
    ElMessage.success('Đã dọn thông báo đã đọc.')
    await loadNotifications()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể dọn thông báo.')
    }
  }
}

async function clearAll() {
  try {
    await ElMessageBox.confirm(
      'Tất cả thông báo của bạn sẽ bị xóa khỏi danh sách. Bạn có chắc muốn tiếp tục?',
      'Xóa tất cả thông báo',
      {
        type: 'warning',
        confirmButtonText: 'Xóa tất cả',
        cancelButtonText: 'Hủy'
      }
    )

    await clearAllNotifications()
    ElMessage.success('Đã xóa tất cả thông báo.')
    pagination.page = 1
    await loadNotifications()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể xóa tất cả thông báo.')
    }
  }
}

function notificationId(item) {
  return Number(pick(item, ['iD_ThongBao', 'ID_ThongBao', 'idThongBao', 'IdThongBao'], 0)) || 0
}

function isRead(item) {
  return Number(pick(item, ['daDoc', 'DaDoc'], 0)) === 1
}

function notificationType(item) {
  return String(pick(item, ['loaiThongBao', 'LoaiThongBao'], 'SYSTEM')).toUpperCase()
}

function notificationTypeLabel(item) {
  const type = notificationType(item)

  const map = {
    ANSWER: 'Trả lời',
    COMMENT_QUESTION: 'Bình luận',
    COMMENT_ANSWER: 'Bình luận',
    REPORT_WARNING: 'Quản trị',
    REPORT_DELETE: 'Quản trị',
    ACCOUNT_LOCK: 'Quản trị',
    ACCOUNT_UNLOCK: 'Quản trị',
    SYSTEM: 'Hệ thống'
  }

  return map[type] || 'Thông báo'
}

function tagType(item) {
  const type = notificationType(item)

  if (['REPORT_WARNING', 'REPORT_DELETE', 'ACCOUNT_LOCK', 'ACCOUNT_UNLOCK'].includes(type)) {
    return 'warning'
  }

  if (['ANSWER', 'COMMENT_QUESTION', 'COMMENT_ANSWER'].includes(type)) {
    return 'success'
  }

  return 'info'
}
</script>

<style scoped>
.notifications-page {
  width: 100%;
  padding: 0 0 56px;
  color: #1f2937;
  font-family: inherit;
}

.notifications-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 18px;
  margin-bottom: 28px;
}

.notifications-header h1 {
  margin: 0 0 8px;
  color: #111827;
  font-size: 30px;
  line-height: 1.25;
  font-weight: 700;
  letter-spacing: 0;
  font-family: inherit;
}

.notifications-header p {
  margin: 0;
  color: #6b7280;
  font-size: 16px;
  line-height: 1.5;
  font-family: inherit;
}

.header-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  flex-wrap: wrap;
}

.filter-panel {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 18px;
  padding: 22px 24px;
  margin-bottom: 20px;
  border: 1px solid var(--forum-border);
  border-radius: 16px;
  background: #ffffff;
}

.filter-left,
.filter-right {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.filter-right {
  justify-content: flex-end;
}

.filter-select {
  width: 230px;
}

.time-select {
  width: 180px;
}

.management-card {
  overflow: hidden;
  border: 1px solid var(--forum-border);
  border-radius: 16px;
  background: #ffffff;
}

.management-card-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 16px;
  padding: 22px 24px;
  border-bottom: 1px solid var(--forum-border);
}

.management-card-header h2 {
  margin: 0;
  color: #111827;
  font-size: 18px;
  line-height: 1.3;
  font-weight: 700;
  font-family: inherit;
}

.management-card-header p {
  margin: 4px 0 0;
  color: #6b7280;
  font-size: 14px;
}

.notification-table {
  min-height: 140px;
  padding: 18px 24px 0;
}

.notification-row {
  display: grid;
  grid-template-columns: 18px minmax(0, 1fr) 240px;
  gap: 14px;
  align-items: center;
  padding: 18px 0;
  border-bottom: 1px solid #e5e7eb;
}

.notification-row:last-child {
  border-bottom: none;
}

.notification-row.unread .notification-title-line h3 {
  color: var(--forum-primary);
}

.read-indicator {
  display: flex;
  align-items: center;
  justify-content: center;
}

.status-dot {
  width: 9px;
  height: 9px;
  border-radius: 999px;
  background: #f97316;
}

.notification-main {
  min-width: 0;
}

.notification-title-line {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
  margin-bottom: 8px;
}

.notification-title-line h3 {
  margin: 0;
  color: #111827;
  font-size: 16px;
  line-height: 1.35;
  font-weight: 700;
  font-family: inherit;
}

.notification-content {
  margin: 0;
  color: #4b5563;
  line-height: 1.55;
  font-size: 14px;
  font-family: inherit;
}

.notification-meta {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 8px;
  color: #6b7280;
  font-size: 13px;
}

.row-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 8px;
  flex-wrap: wrap;
}

@media (max-width: 900px) {
  .notifications-header,
  .filter-panel,
  .management-card-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions,
  .filter-left,
  .filter-right {
    justify-content: flex-start;
  }

  .filter-select,
  .time-select {
    width: 100%;
  }

  .notification-row {
    grid-template-columns: 14px minmax(0, 1fr);
    align-items: flex-start;
  }

  .row-actions {
    grid-column: 2;
    justify-content: flex-start;
  }
}
</style>
