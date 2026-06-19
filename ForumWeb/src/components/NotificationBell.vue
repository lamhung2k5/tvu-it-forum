<template>
  <div class="notification-wrapper">
    <el-popover
      v-model:visible="visible"
      placement="bottom-end"
      width="380"
      trigger="click"
      popper-class="notification-popover"
      @show="loadNotifications"
    >
      <template #reference>
        <el-badge
          :value="unreadCount"
          :hidden="unreadCount <= 0"
          :max="99"
          class="notification-badge"
        >
          <button
            type="button"
            class="notification-trigger"
            aria-label="Thông báo"
            title="Thông báo"
          >
            <el-icon class="notification-icon">
              <Bell />
            </el-icon>
          </button>
        </el-badge>
      </template>

      <div class="notification-panel">
        <div class="notification-header">
          <div>
            <strong>Thông báo</strong>
            <p>{{ unreadCount > 0 ? `${unreadCount} thông báo chưa đọc` : 'Không có thông báo mới' }}</p>
          </div>

          <el-button
            v-if="unreadCount > 0"
            link
            size="small"
            class="read-all-button"
            @click="markAllRead"
          >
            Đánh dấu đã đọc
          </el-button>
        </div>

        <el-empty
          v-if="notifications.length === 0"
          description="Không có thông báo chưa đọc"
          :image-size="72"
        />

        <div v-else class="notification-list">
          <button
            v-for="item in notifications"
            :key="notificationId(item)"
            type="button"
            class="notification-item unread"
            @click="openNotification(item)"
          >
            <span class="unread-dot" />

            <div class="notification-body">
              <div class="notification-title">
                {{ pick(item, ['tieuDe', 'TieuDe'], 'Thông báo mới') }}
              </div>

              <div class="notification-content">
                {{ pick(item, ['noiDung', 'NoiDung'], '') }}
              </div>

              <div class="notification-time">
                {{ formatDate(pick(item, ['ngayTao', 'NgayTao'], '')) }}
              </div>
            </div>
          </button>
        </div>

        <div class="notification-footer">
          <el-button
            link
            class="view-all-button"
            @click="goToAllNotifications"
          >
            Xem tất cả thông báo
          </el-button>
        </div>
      </div>
    </el-popover>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Bell } from '@element-plus/icons-vue'
import {
  getRecentNotifications,
  getUnreadNotificationCount,
  markNotificationAsRead,
  markAllNotificationsAsRead
} from '../api/thongBaoApi'
import { pick, formatDate } from '../utils/format'

const router = useRouter()

const visible = ref(false)
const unreadCount = ref(0)
const notifications = ref([])

let timer = null

onMounted(() => {
  refreshUnreadCount()
  timer = window.setInterval(refreshUnreadCount, 60000)
})

onUnmounted(() => {
  if (timer) {
    window.clearInterval(timer)
  }
})

async function refreshUnreadCount() {
  try {
    const result = await getUnreadNotificationCount()
    unreadCount.value = Number(pick(result, ['count', 'Count'], 0)) || 0
  } catch {
    unreadCount.value = 0
  }
}

async function loadNotifications() {
  try {
    notifications.value = await getRecentNotifications(5) || []
    await refreshUnreadCount()
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được thông báo.')
  }
}

async function openNotification(item) {
  const id = notificationId(item)
  const link = pick(item, ['link', 'Link'], '')

  try {
    if (id) {
      await markNotificationAsRead(id)
    }

    notifications.value = notifications.value.filter(notification => notificationId(notification) !== id)
    await refreshUnreadCount()
    visible.value = false

    if (link) {
      router.push(link)
    }
  } catch (error) {
    ElMessage.error(error.message || 'Không thể mở thông báo.')
  }
}

async function markAllRead() {
  try {
    await markAllNotificationsAsRead()
    notifications.value = []
    await refreshUnreadCount()
  } catch (error) {
    ElMessage.error(error.message || 'Không thể đánh dấu thông báo.')
  }
}

function goToAllNotifications() {
  visible.value = false
  router.push('/notifications')
}

function notificationId(item) {
  return Number(pick(item, ['iD_ThongBao', 'ID_ThongBao', 'idThongBao', 'IdThongBao'], 0)) || 0
}
</script>

<style scoped>
.notification-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 40px;
  flex-shrink: 0;
  overflow: visible;
}

.notification-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
  overflow: visible;
}

.notification-trigger {
  width: 38px;
  height: 38px;
  border: 1px solid var(--forum-border);
  outline: none;
  background: #ffffff;
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
  border-radius: 999px;
  padding: 0;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: var(--forum-primary);
  cursor: pointer;
  transition: all 0.2s ease;
}

.notification-trigger:hover {
  border-color: var(--forum-primary);
  background: #eef3ff;
  color: var(--forum-primary-dark, #16357a);
  box-shadow: 0 4px 12px rgba(31, 63, 149, 0.14);
}

.notification-trigger:active {
  transform: scale(0.96);
}

.notification-icon {
  font-size: 20px;
}

.notification-badge :deep(.el-badge__content) {
  top: 0;
  right: 0;
  transform: translate(38%, -35%);
  height: 18px;
  min-width: 18px;
  padding: 0 5px;
  border: 2px solid #ffffff;
  font-size: 11px;
  font-weight: 700;
  line-height: 14px;
  z-index: 2;
}

.notification-panel {
  max-height: 440px;
  overflow: hidden;
}

.notification-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  padding: 2px 2px 12px;
  border-bottom: 1px solid var(--forum-border);
}

.notification-header strong {
  display: block;
  color: var(--forum-text);
  font-size: 15px;
  line-height: 1.3;
}

.notification-header p {
  margin: 3px 0 0;
  color: var(--forum-muted);
  font-size: 12px;
}

.read-all-button {
  flex-shrink: 0;
  margin-top: -2px;
}

.notification-list {
  max-height: 300px;
  overflow-y: auto;
  padding-top: 8px;
  padding-right: 2px;
}

.notification-item {
  position: relative;
  width: 100%;
  border: none;
  border-radius: 12px;
  background: transparent;
  text-align: left;
  padding: 10px 10px 10px 26px;
  cursor: pointer;
  margin-bottom: 6px;
  display: flex;
  align-items: flex-start;
  transition: background 0.2s ease;
}

.notification-item:hover {
  background: #ffedd5;
}

.notification-item.unread {
  background: #fff7ed;
}

.unread-dot {
  position: absolute;
  top: 16px;
  left: 10px;
  width: 8px;
  height: 8px;
  border-radius: 999px;
  background: #f97316;
}

.notification-body {
  min-width: 0;
}

.notification-title {
  color: var(--forum-text);
  font-weight: 700;
  font-size: 14px;
  margin-bottom: 4px;
  line-height: 1.35;
}

.notification-content {
  color: #4b5563;
  font-size: 13px;
  line-height: 1.45;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.notification-time {
  color: var(--forum-muted);
  font-size: 12px;
  margin-top: 6px;
}

.notification-footer {
  padding-top: 8px;
  margin-top: 6px;
  border-top: 1px solid var(--forum-border);
  display: flex;
  justify-content: center;
}

.view-all-button {
  font-weight: 700;
}

@media (max-width: 640px) {
  .notification-trigger {
    width: 34px;
    height: 34px;
  }

  .notification-icon {
    font-size: 19px;
  }
}
</style>
