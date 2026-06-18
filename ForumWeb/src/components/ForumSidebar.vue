<template>
  <aside class="forum-sidebar" :class="{ 'is-collapsed': isCollapsed }">
    <div class="sidebar-control">
      <button
        type="button"
        class="sidebar-toggle"
        :title="isCollapsed ? 'Mở sidebar' : 'Thu sidebar'"
        @click="toggleSidebar"
      >
        <el-icon>
          <Expand v-if="isCollapsed" />
          <Fold v-else />
        </el-icon>
      </button>
    </div>

    <el-menu :default-active="activePath" router class="sidebar-menu">
      <el-menu-item index="/">
        <el-icon><House /></el-icon>
        <span class="menu-label">Trang chủ</span>
      </el-menu-item>

      <el-menu-item v-if="isLoggedIn" index="/dashboard">
        <el-icon><DataBoard /></el-icon>
        <span class="menu-label">Dashboard</span>
      </el-menu-item>

      <el-menu-item v-if="isLoggedIn" index="/my-questions">
        <el-icon><QuestionFilled /></el-icon>
        <span class="menu-label">Câu hỏi của tôi</span>
      </el-menu-item>

      <el-menu-item v-if="isLoggedIn" index="/my-answers">
        <el-icon><ChatDotRound /></el-icon>
        <span class="menu-label">Câu trả lời của tôi</span>
      </el-menu-item>

      <el-menu-item v-if="isAdmin" index="/admin">
        <el-icon><Setting /></el-icon>
        <span class="menu-label">Quản trị</span>
      </el-menu-item>
    </el-menu>

    <div class="sidebar-extra">
      <el-card class="sidebar-card" shadow="never">
        <template #header>
          <div class="card-title">
            <el-icon><Collection /></el-icon>
            <span>Chuyên mục</span>
          </div>
        </template>

        <div class="category-list">
          <el-tag
            v-for="item in categories"
            :key="item.id"
            class="forum-tag"
            @click="$emit('select-category', item.id)"
          >
            {{ item.name }}
          </el-tag>
        </div>
      </el-card>

      <el-card class="sidebar-card" shadow="never">
        <template #header>
          <div class="card-title">
            <el-icon><PriceTag /></el-icon>
            <span>Tag gợi ý</span>
          </div>
        </template>

        <div class="tag-list">
          <el-tag
            v-for="tag in tags"
            :key="tag"
            class="forum-tag"
            @click="$emit('select-tag', tag)"
          >
            {{ tag }}
          </el-tag>
        </div>
      </el-card>
    </div>
  </aside>
</template>

<script setup>
import {
  House,
  DataBoard,
  QuestionFilled,
  ChatDotRound,
  Collection,
  PriceTag,
  Setting,
  Fold,
  Expand
} from '@element-plus/icons-vue'
import { computed, ref } from 'vue'
import { useRoute } from 'vue-router'

defineProps({
  isLoggedIn: { type: Boolean, default: false },
  isAdmin: { type: Boolean, default: false }
})

defineEmits(['select-category', 'select-tag'])

const route = useRoute()
const activePath = computed(() => route.path)

const isCollapsed = ref(false)

function toggleSidebar() {
  isCollapsed.value = !isCollapsed.value
}

const categories = [
  { id: 1, name: 'Lập trình Web' },
  { id: 2, name: 'Cơ sở dữ liệu' },
  { id: 3, name: 'Chia sẻ kinh nghiệm' }
]

const tags = ['dotnet', 'vue', 'sqlite', 'dapper', 'csharp', 'api']
</script>

<style scoped>
.forum-sidebar {
  width: 245px;
  flex: 0 0 245px;
  padding: 14px 12px 18px;
  border-right: 1px solid var(--forum-border);
  background: var(--forum-sidebar);
  min-height: calc(100vh - 72px);
  position: sticky;
  top: 72px;
  align-self: flex-start;
  transition:
    width 0.25s ease,
    flex-basis 0.25s ease,
    padding 0.25s ease;
}

.sidebar-control {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 10px;
}

.sidebar-toggle {
  width: 32px;
  height: 32px;
  border: 1px solid var(--forum-border);
  border-radius: 10px;
  background: #ffffff;
  color: var(--forum-primary);
  display: grid;
  place-items: center;
  cursor: pointer;
  transition:
    background 0.2s ease,
    color 0.2s ease,
    border-color 0.2s ease,
    transform 0.2s ease;
}

.sidebar-toggle:hover {
  background: var(--forum-primary-soft);
  border-color: var(--forum-primary);
  transform: translateX(-1px);
}

.sidebar-menu {
  border-right: none;
  background: transparent;
  margin-bottom: 16px;
}

.sidebar-menu :deep(.el-menu-item) {
  border-radius: 10px;
  height: 42px;
  border-left: 3px solid transparent;
  display: flex;
  align-items: center;
  padding: 0 16px !important;
  color: var(--forum-text);
  transition:
    background 0.2s ease,
    color 0.2s ease,
    padding 0.25s ease;
}

.sidebar-menu :deep(.el-menu-item .el-icon) {
  margin-right: 10px;
  font-size: 18px;
  color: var(--forum-muted);
  transition:
    margin 0.25s ease,
    color 0.2s ease,
    font-size 0.25s ease;
}

.sidebar-menu :deep(.el-menu-item.is-active) {
  color: var(--forum-primary);
  background: var(--forum-primary-soft);
  border-left-color: var(--forum-primary);
  font-weight: 650;
}

.sidebar-menu :deep(.el-menu-item.is-active .el-icon) {
  color: var(--forum-primary);
}

.menu-label {
  white-space: nowrap;
  overflow: hidden;
  transition: opacity 0.2s ease;
}

.sidebar-card {
  margin-bottom: 14px;
  border-radius: 12px;
}

.card-title {
  display: flex;
  align-items: center;
  gap: 8px;
}

.card-title .el-icon {
  font-size: 18px;
  color: var(--forum-primary);
}

.category-list,
.tag-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.category-list .el-tag,
.tag-list .el-tag {
  cursor: pointer;
}

/* Trạng thái thu gọn */
.forum-sidebar.is-collapsed {
  width: 64px;
  flex-basis: 64px;
  padding: 14px 8px 18px;
}

.forum-sidebar.is-collapsed .sidebar-control {
  justify-content: center;
}

.forum-sidebar.is-collapsed .sidebar-toggle:hover {
  transform: translateX(1px);
}

.forum-sidebar.is-collapsed .menu-label {
  display: none;
}

.forum-sidebar.is-collapsed .sidebar-extra {
  display: none;
}

.forum-sidebar.is-collapsed .sidebar-menu {
  margin-bottom: 0;
}

.forum-sidebar.is-collapsed .sidebar-menu :deep(.el-menu-item) {
  justify-content: center;
  padding: 0 !important;
  border-left-width: 0;
}

.forum-sidebar.is-collapsed .sidebar-menu :deep(.el-menu-item .el-icon) {
  margin-right: 0;
  font-size: 19px;
}

.forum-sidebar.is-collapsed .sidebar-menu :deep(.el-menu-item.is-active) {
  border-left-color: transparent;
}

/* Tablet / mobile: không dùng chế độ thu gọn để tránh rối giao diện */
@media (max-width: 980px) {
  .forum-sidebar,
  .forum-sidebar.is-collapsed {
    width: 100%;
    flex: none;
    min-height: auto;
    position: static;
    border-right: none;
    border-bottom: 1px solid var(--forum-border);
    padding: 18px 12px;
  }

  .sidebar-control {
    display: none;
  }

  .forum-sidebar.is-collapsed .menu-label {
    display: inline;
  }

  .forum-sidebar.is-collapsed .sidebar-extra {
    display: block;
  }

  .forum-sidebar.is-collapsed .sidebar-menu :deep(.el-menu-item) {
    justify-content: flex-start;
    padding: 0 16px !important;
    border-left-width: 3px;
  }

  .forum-sidebar.is-collapsed .sidebar-menu :deep(.el-menu-item .el-icon) {
    margin-right: 10px;
    font-size: 18px;
  }
}
</style>