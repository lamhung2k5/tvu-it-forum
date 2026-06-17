<template>
  <header class="forum-header">
    <div class="brand" @click="$router.push('/')">
      <div class="brand-mark">?</div>
      <div>
        <h1>TVU IT Forum</h1>
        <p>Hỏi đáp công nghệ thông tin</p>
      </div>
    </div>

    <div class="header-actions">
      <el-button type="primary" plain @click="$router.push('/')">Câu hỏi</el-button>
      <el-button v-if="isLoggedIn" type="primary" @click="$router.push('/questions/create')">Đặt câu hỏi</el-button>
      <template v-if="isLoggedIn">
        <UserDropdown :user="user" />
      </template>
      <template v-else>
        <el-button plain @click="$router.push('/login')">Đăng nhập</el-button>
        <el-button type="primary" @click="$router.push('/register')">Đăng ký</el-button>
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
  justify-content: space-between;
  gap: 16px;
  padding: 0 24px;
  position: sticky;
  top: 0;
  z-index: 20;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  min-width: 220px;
}

.brand-mark {
  width: 42px;
  height: 42px;
  display: grid;
  place-items: center;
  border-radius: 12px;
  background: var(--forum-primary);
  color: white;
  font-size: 24px;
  font-weight: 800;
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

.header-actions {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
  justify-content: flex-end;
}

@media (max-width: 900px) {
  .forum-header {
    height: auto;
    align-items: flex-start;
    flex-direction: column;
    padding: 12px 16px;
  }

  .header-actions {
    width: 100%;
    justify-content: flex-start;
  }
}
</style>
