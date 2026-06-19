<template>
  <el-dropdown trigger="click" @command="handleCommand">
    <button class="user-trigger">
      <UserAvatar :name="user?.hoTen" :src="user?.anhDaiDien" :size="34" />
      <span class="user-name">{{ user?.hoTen || 'Người dùng' }}</span>
      <span class="user-role">{{ user?.vaiTro || 'User' }}</span>
      <span class="caret">▾</span>
    </button>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item command="dashboard">Dashboard của tôi</el-dropdown-item>
        <el-dropdown-item command="profile">Hồ sơ cá nhân</el-dropdown-item>
        <el-dropdown-item command="myQuestions">Câu hỏi của tôi</el-dropdown-item>
        <el-dropdown-item command="myAnswers">Câu trả lời của tôi</el-dropdown-item>
        <el-dropdown-item v-if="isAdmin" command="admin" divided>Quản trị</el-dropdown-item>
        <el-dropdown-item command="logout" divided>Đăng xuất</el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { clearAuth } from '../utils/auth'
import UserAvatar from './UserAvatar.vue'

const props = defineProps({
  user: { type: Object, default: null }
})

const router = useRouter()
const isAdmin = computed(() => String(props.user?.vaiTro || '').toLowerCase() === 'admin')

function handleCommand(command) {
  const routes = {
    dashboard: '/dashboard',
    profile: '/profile',
    myQuestions: '/my-questions',
    myAnswers: '/my-answers',
    admin: '/admin'
  }

  if (command === 'logout') {
    clearAuth()
    router.push('/login')
    return
  }

  router.push(routes[command] || '/')
}
</script>

<style scoped>
.user-trigger {
  border: 1px solid var(--forum-border);
  background: white;
  border-radius: 999px;
  padding: 4px 10px 4px 4px;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  color: var(--forum-text);
}

.user-trigger:hover {
  border-color: var(--forum-primary);
  box-shadow: 0 4px 12px rgba(244, 128, 36, 0.12);
}

.user-name {
  font-weight: 650;
  max-width: 140px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.user-role {
  font-size: 12px;
  color: var(--forum-muted);
  background: var(--forum-primary-soft);
  padding: 2px 7px;
  border-radius: 999px;
}

.caret { color: var(--forum-muted); }

@media (max-width: 640px) {
  .user-name, .user-role { display: none; }
}
</style>
