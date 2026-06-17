<template>
  <aside class="forum-sidebar">
    <el-menu :default-active="activePath" router class="sidebar-menu">
      <el-menu-item index="/">Trang chủ</el-menu-item>
      <el-menu-item v-if="isLoggedIn" index="/dashboard">Dashboard</el-menu-item>
      <el-menu-item v-if="isLoggedIn" index="/my-questions">Câu hỏi của tôi</el-menu-item>
      <el-menu-item v-if="isLoggedIn" index="/my-answers">Câu trả lời của tôi</el-menu-item>
      <el-menu-item v-if="isAdmin" index="/admin">Quản trị</el-menu-item>
    </el-menu>

    <el-card class="sidebar-card" shadow="never">
      <template #header>Chuyên mục</template>
      <div class="category-list">
        <el-tag v-for="item in categories" :key="item.id" type="warning" effect="light" @click="$emit('select-category', item.id)">
          {{ item.name }}
        </el-tag>
      </div>
    </el-card>

    <el-card class="sidebar-card" shadow="never">
      <template #header>Tag gợi ý</template>
      <div class="tag-list">
        <el-tag v-for="tag in tags" :key="tag" effect="plain" @click="$emit('select-tag', tag)">{{ tag }}</el-tag>
      </div>
    </el-card>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const props = defineProps({
  isLoggedIn: { type: Boolean, default: false },
  isAdmin: { type: Boolean, default: false }
})

defineEmits(['select-category', 'select-tag'])

const route = useRoute()
const activePath = computed(() => route.path)

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
  padding: 18px 12px;
  border-right: 1px solid var(--forum-border);
  background: var(--forum-sidebar);
  min-height: calc(100vh - 72px);
  position: sticky;
  top: 72px;
  align-self: flex-start;
}

.sidebar-menu {
  border-right: none;
  background: transparent;
  margin-bottom: 16px;
}

.sidebar-menu :deep(.el-menu-item) {
  border-radius: 10px;
  height: 42px;
}

.sidebar-menu :deep(.el-menu-item.is-active) {
  color: var(--forum-primary);
  background: var(--forum-primary-soft);
}

.sidebar-card {
  margin-bottom: 14px;
  border-radius: 12px;
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

@media (max-width: 980px) {
  .forum-sidebar {
    width: 100%;
    flex: none;
    min-height: auto;
    position: static;
    border-right: none;
    border-bottom: 1px solid var(--forum-border);
  }
}
</style>
