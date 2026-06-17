  <template>
    <aside class="forum-sidebar">
      <el-menu :default-active="activePath" router class="sidebar-menu">
      <el-menu-item index="/">
        <el-icon><House /></el-icon>
        <span>Trang chủ</span>
      </el-menu-item>

      <el-menu-item v-if="isLoggedIn" index="/dashboard">
        <el-icon><DataBoard /></el-icon>
        <span>Dashboard</span>
      </el-menu-item>

      <el-menu-item v-if="isLoggedIn" index="/my-questions">
        <el-icon><QuestionFilled /></el-icon>
        <span>Câu hỏi của tôi</span>
      </el-menu-item>

      <el-menu-item v-if="isLoggedIn" index="/my-answers">
        <el-icon><ChatDotRound /></el-icon>
        <span>Câu trả lời của tôi</span>
      </el-menu-item>

      <el-menu-item v-if="isAdmin" index="/admin">
        <el-icon><Setting /></el-icon>
        <span>Quản trị</span>
      </el-menu-item>
    </el-menu>

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
    Setting
  } from '@element-plus/icons-vue'
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
    border-left: 3px solid transparent;
    display: flex;
    align-items: center;
  }
  .sidebar-menu :deep(.el-menu-item) {
    border-radius: 10px;
    height: 42px;
    border-left: 3px solid transparent;
    display: flex;
    align-items: center;
  }

  .sidebar-menu :deep(.el-menu-item.is-active) {
    color: var(--forum-primary);
    background: var(--forum-primary-soft);
    border-left-color: var(--forum-primary);
    font-weight: 650;
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
