<template>
  <header class="forum-header">
    <div class="header-left">
      <div class="brand" @click="$router.push('/')">
        <div class="brand-logo-box">
          <img
            src="/images/logo-khoa.png"
            alt="TVU IT Forum"
            class="brand-logo"
          />
        </div>

        <div class="brand-text">
          <h1>CET FORIT</h1>
          <p>Hỏi đáp công nghệ thông tin</p>
        </div>
      </div>

      <nav class="main-nav">
        <el-button text @click="$router.push('/')">
          Giới thiệu
        </el-button>

        <el-button text @click="$router.push('/')">
          Khám phá CET
        </el-button>
      </nav>
    </div>

    <div class="header-actions">
      <div class="nav-search">
        <el-input
          v-model="navKeyword"
          placeholder="Tìm kiếm câu hỏi..."
          clearable
          @keyup.enter="searchFromHeader"
        >
          <template #prefix>
            <el-icon
              class="nav-search-icon"
              title="Tìm kiếm"
              @click.stop="searchFromHeader"
            >
              <Search />
            </el-icon>
          </template>
        </el-input>
      </div>

      <el-button
        type="primary"
        plain
        class="question-link-btn"
        @click="$router.push('/')"
      >
        Câu hỏi
      </el-button>

      <template v-if="isLoggedIn">
        <UserDropdown :user="user" />
      </template>

      <template v-else>
        <el-button plain @click="$router.push('/login')">
          Đăng nhập
        </el-button>

        <el-button type="primary" @click="$router.push('/register')">
          Đăng ký
        </el-button>
      </template>
    </div>
  </header>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Search } from '@element-plus/icons-vue'
import UserDropdown from './UserDropdown.vue'

const props = defineProps({
  user: { type: Object, default: null }
})

const router = useRouter()
const route = useRoute()

const navKeyword = ref('')

const isLoggedIn = computed(() => Boolean(props.user))

watch(
  () => route.query.keyword,
  value => {
    navKeyword.value = value || ''
  },
  { immediate: true }
)

function searchFromHeader() {
  const keyword = navKeyword.value.trim()

  if (!keyword) {
    router.push('/')
    return
  }

  router.push({
    path: '/',
    query: {
      keyword
    }
  })
}
</script>

<style scoped>
.forum-header {
  height: 72px;
  background: #ffffff;
  border-bottom: 1px solid var(--forum-border);

  display: flex;
  align-items: center;
  gap: 24px;

  padding: 0 24px;

  position: sticky;
  top: 0;
  z-index: 20;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 34px;
  min-width: 0;
  flex: 1;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  flex-shrink: 0;
}

.brand-logo-box {
  width: 72px;
  height: 48px;
  overflow: hidden;
  background: #ffffff;

  display: flex;
  align-items: center;
  justify-content: center;

  flex-shrink: 0;
}

.brand-logo {
  width: 100%;
  height: 100%;
  object-fit: contain;
  display: block;
}

.brand h1 {
  margin: 0;
  font-size: 20px;
  line-height: 1.05;
  color: var(--forum-text);
}

.brand p {
  margin: 3px 0 0;
  font-size: 12px;
  color: var(--forum-muted);
}

.main-nav {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-shrink: 0;
}

.main-nav :deep(.el-button) {
  margin-left: 0 !important;
  padding: 8px 10px;
  font-size: 15px;
}

.main-nav :deep(.el-button + .el-button) {
  margin-left: 0 !important;
}

.main-nav :deep(.el-button.is-text) {
  color: var(--forum-text);
}

.main-nav :deep(.el-button.is-text:hover) {
  color: var(--forum-primary);
  background: var(--forum-primary-soft);
}

.header-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 14px;
  flex-shrink: 0;
}

.nav-search {
  width: 360px;
  flex-shrink: 0;
}

.nav-search :deep(.el-input__wrapper) {
  height: 38px;
  border-radius: 999px;
  background: #f8fafc;
  box-shadow: 0 0 0 1px var(--forum-border) inset;
  transition: all 0.2s ease;
}

.nav-search :deep(.el-input__wrapper:hover) {
  background: #ffffff;
  box-shadow: 0 0 0 1px rgba(30, 64, 175, 0.35) inset;
}

.nav-search :deep(.el-input__wrapper.is-focus) {
  background: #ffffff;
  box-shadow: 0 0 0 1px var(--forum-primary) inset;
}

.nav-search :deep(.el-input__inner) {
  font-size: 14px;
}

.nav-search-icon {
  cursor: pointer;
  color: var(--forum-muted);
  font-size: 16px;
  transition: color 0.2s ease;
}

.nav-search-icon:hover {
  color: var(--forum-primary);
}

.question-link-btn {
  height: 38px;
  border-radius: 8px;
  font-weight: 600;
}

.header-actions :deep(.el-button.is-text) {
  color: var(--forum-text);
}

.header-actions :deep(.el-button.is-text:hover) {
  color: var(--forum-primary);
  background: var(--forum-primary-soft);
}

/* Màn hình vừa */
@media (max-width: 1220px) {
  .forum-header {
    gap: 16px;
  }

  .header-left {
    gap: 22px;
  }

  .main-nav {
    gap: 4px;
  }

  .main-nav :deep(.el-button) {
    padding: 8px 8px;
  }

  .nav-search {
    width: 280px;
  }

  .header-actions {
    gap: 10px;
  }
}

/* Tablet nhỏ */
@media (max-width: 980px) {
  .main-nav,
  .nav-search {
    display: none;
  }

  .forum-header {
    padding: 0 16px;
  }

  .brand-logo-box {
    width: 60px;
    height: 40px;
  }

  .brand h1 {
    font-size: 18px;
  }

  .brand p {
    font-size: 12px;
  }
}

/* Điện thoại */
@media (max-width: 640px) {
  .forum-header {
    height: auto;
    min-height: 64px;
    padding: 10px 12px;
    gap: 10px;
  }

  .header-left {
    flex: 1;
    min-width: 0;
  }

  .brand {
    gap: 8px;
    min-width: 0;
  }

  .brand-logo-box {
    width: 48px;
    height: 34px;
  }

  .brand-text {
    min-width: 0;
  }

  .brand h1 {
    font-size: 16px;
    white-space: nowrap;
  }

  .brand p {
    display: none;
  }

  .header-actions {
    gap: 6px;
  }

  .header-actions :deep(.el-button) {
    padding: 8px 10px;
  }
}

/* Điện thoại rất nhỏ: ẩn nút Câu hỏi */
@media (max-width: 480px) {
  .question-link-btn {
    display: none;
  }
}
</style>