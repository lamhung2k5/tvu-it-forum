<template>
  <div class="auth-page">
    <div class="auth-card">
      <section class="auth-form-panel">
        <el-button text class="back-link" @click="$router.push('/')">← Về trang chủ</el-button>
        <h2>Đăng nhập</h2>
        <p class="muted">Truy cập diễn đàn để đặt câu hỏi, trả lời và bình chọn.</p>

        <el-form :model="form" label-position="top" class="auth-form" @keyup.enter="submit">
          <el-form-item label="Email">
            <el-input v-model="form.email" placeholder="you@example.com" size="large" clearable />
          </el-form-item>

          <el-form-item label="Mật khẩu">
            <el-input v-model="form.password" placeholder="Nhập mật khẩu" size="large" show-password />
          </el-form-item>

          <el-button type="primary" size="large" :loading="loading" class="full-button" @click="submit">
            Đăng nhập
          </el-button>
        </el-form>

        <p class="auth-switch">
          Chưa có tài khoản?
          <router-link to="/register" class="orange-link">Đăng ký ngay</router-link>
        </p>
      </section>

      <section class="auth-hero">
        <h2>TVU IT Forum</h2>
        <p>Nơi sinh viên CNTT trao đổi kiến thức về .NET, Vue, cơ sở dữ liệu, API và kinh nghiệm học tập.</p>
      </section>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { login } from '../api/authApi'
import { setAuth } from '../utils/auth'

const router = useRouter()
const route = useRoute()
const loading = ref(false)
const form = reactive({ email: '', password: '' })

async function submit() {
  if (!form.email.trim() || !form.password.trim()) {
    ElMessage.warning('Vui lòng nhập email và mật khẩu.')
    return
  }

  loading.value = true
  try {
    const result = await login({ email: form.email.trim(), password: form.password })
    const token = result.accessToken || result.AccessToken
    const user = result.user || result.User

    if (!token) throw new Error('API đăng nhập chưa trả về AccessToken.')

    setAuth(token, user)
    ElMessage.success('Đăng nhập thành công!')
    router.push(route.query.redirect || '/')
  } catch (error) {
    ElMessage.error(error.message || 'Đăng nhập thất bại.')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-form { margin-top: 26px; }
.auth-form-panel h2 { margin: 12px 0 8px; font-size: 30px; }
.full-button { width: 100%; }
.auth-switch { text-align: center; margin-top: 18px; color: var(--forum-muted); }
.back-link { padding-left: 0; }
</style>
