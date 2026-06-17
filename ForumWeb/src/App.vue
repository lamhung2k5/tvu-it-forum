<template>
  <RouterView v-if="isAuthLayout" />

  <div v-else class="forum-app">
    <AppHeader :user="currentUser" />
    <div class="forum-body">
      <ForumSidebar
        :is-logged-in="isLoggedIn"
        :is-admin="isUserAdmin"
        @select-category="handleCategory"
        @select-tag="handleTag"
      />
      <main class="forum-content">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getCurrentUser, isAuthenticated, isAdmin } from './utils/auth'
import AppHeader from './components/AppHeader.vue'
import ForumSidebar from './components/ForumSidebar.vue'

const route = useRoute()
const router = useRouter()
const authVersion = ref(0)

const refreshAuth = () => {
  authVersion.value += 1
}

const currentUser = computed(() => {
  authVersion.value
  return getCurrentUser()
})

const isLoggedIn = computed(() => {
  authVersion.value
  return isAuthenticated()
})

const isUserAdmin = computed(() => {
  authVersion.value
  return isAdmin()
})

const isAuthLayout = computed(() => Boolean(route.meta.authLayout))

function handleCategory(idChuyenMuc) {
  router.push({ name: 'questions', query: { idChuyenMuc } })
}

function handleTag(tag) {
  router.push({ name: 'questions', query: { tag } })
}

onMounted(() => window.addEventListener('auth-changed', refreshAuth))
onUnmounted(() => window.removeEventListener('auth-changed', refreshAuth))
</script>

<style scoped>
.forum-app {
  min-height: 100vh;
}

.forum-body {
  display: flex;
  align-items: stretch;
}

.forum-content {
  flex: 1;
  min-width: 0;
  padding: 24px;
}

@media (max-width: 980px) {
  .forum-body { flex-direction: column; }
  .forum-content { padding: 16px; }
}
</style>
