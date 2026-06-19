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
      <el-button type="primary" plain @click="$router.push('/')">
        Câu hỏi
      </el-button>

      <el-button
        v-if="isLoggedIn"
        type="primary"
        class="btn-accent"
        @click="$router.push('/questions/create')"
      >
        Đặt câu hỏi
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
import { computed } from 'vue'
import UserDropdown from './UserDropdown.vue'

const props = defineProps({
  user: { type: Object, default: null }
})

const isLoggedIn = computed(() => Boolean(props.user))
</script>

<style scoped>
.forum-header {
  height: 72px;
  background: white;
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
  gap: 42px;
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
}

.brand p {
  margin: 3px 0 0;
  font-size: 12px;
  color: var(--forum-muted);
}

.main-nav {
  display: flex;
  align-items: center;
  gap: 18 px;
  flex-shrink: 0;
}

.main-nav :deep(.el-button) {
  font-size: 15px;
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
  gap: 12px;
  flex-shrink: 0;
}

.header-actions :deep(.el-button) {
}

.header-actions :deep(.el-button.is-text) {
  color: var(--forum-text);
}

.header-actions :deep(.el-button.is-text:hover) {
  color: var(--forum-primary);
  background: var(--forum-primary-soft);
}

/* Màn hình vừa: giảm khoảng cách để không bị chật */
@media (max-width: 1100px) {
  .forum-header {
    gap: 16px;
  }

  .header-left {
    gap: 24px;
  }

  .main-nav {
    gap: 10px;
  }

  .header-actions {
    gap: 8px;
  }
}

/* Tablet nhỏ: ẩn bớt menu phụ */
@media (max-width: 900px) {
  .main-nav {
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

/* Điện thoại: giữ gọn header */
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

/* Điện thoại rất nhỏ: ẩn nút Câu hỏi, chỉ giữ Đặt câu hỏi + User */
@media (max-width: 480px) {
  .header-actions :deep(.el-button:first-child) {
    display: none;
  }
}
</style>