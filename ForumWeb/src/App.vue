<template>
  <div class="forum-app">
    <AppTopBar />
    <AppHeader :user="currentUser" />

    <div
      class="forum-body"
      :class="{ 'auth-body': isAuthLayout }"
    >
      <ForumSidebar
        v-if="!isAuthLayout"
        :is-logged-in="isLoggedIn"
        :is-admin="isUserAdmin"
        @select-category="handleCategory"
        @select-tag="handleTag"
      />

      <main
        class="forum-content"
        :class="{ 'auth-content': isAuthLayout }"
      >
        <RouterView />
      </main>
    </div>

    <AppFooter />
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getCurrentUser, isAuthenticated, isAdmin } from './utils/auth'
import AppHeader from './components/AppHeader.vue'
import ForumSidebar from './components/ForumSidebar.vue'
import AppFooter from './components/AppFooter.vue'
import AppTopBar from './components/AppTopBar.vue'

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
  display: flex;
  flex-direction: column;
  background: var(--forum-bg);
}

.forum-body {
  flex: 1;
  display: flex;
  align-items: stretch;
}

.forum-content {
  flex: 1;
  min-width: 0;
  padding: 18px;
}

/* Layout riêng cho trang login/register */
.auth-body {
  display: block;
  background:
    radial-gradient(circle at top left, rgba(30, 58, 138, 0.08), transparent 34%),
    linear-gradient(135deg, #f8fafc 0%, #ffffff 55%, #f3f4f6 100%);
}

.auth-content {
  padding: 0;
  min-height: 620px;
}
</style>
