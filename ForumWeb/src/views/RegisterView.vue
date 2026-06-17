<template>
  <div class="auth-page">
    <div class="auth-card">
      <section class="auth-form-panel">
        <el-button text class="back-link" @click="$router.push('/')">← Về trang chủ</el-button>
        <h2>Tạo tài khoản</h2>
        <p class="muted">Tham gia cộng đồng hỏi đáp công nghệ thông tin.</p>

        <el-form :model="form" label-position="top" class="auth-form" @keyup.enter="submit">
          <el-form-item label="Họ tên">
            <el-input v-model="form.hoTen" placeholder="Ví dụ: Lâm Hưng" size="large" clearable />
          </el-form-item>

          <el-form-item label="Email">
            <el-input v-model="form.email" placeholder="you@example.com" size="large" clearable />
          </el-form-item>

          <el-form-item label="Mật khẩu">
            <el-input v-model="form.password" placeholder="Tối thiểu 6 ký tự" size="large" show-password />
          </el-form-item>

          <el-form-item label="Xác nhận mật khẩu">
            <el-input v-model="form.confirmPassword" placeholder="Nhập lại mật khẩu" size="large" show-password />
          </el-form-item>

          <el-button type="primary" size="large" :loading="loading" class="full-button" @click="submit">
            Đăng ký
          </el-button>
        </el-form>

        <p class="auth-switch">
          Đã có tài khoản?
          <router-link to="/login" class="orange-link">Đăng nhập</router-link>
        </p>
      </section>

      <section class="auth-hero">
        <h2>Hỏi đúng chỗ, học nhanh hơn</h2>
        <p>Đặt câu hỏi, gắn tag, nhận câu trả lời, bình chọn giải pháp tốt và lưu lại kiến thức cho cộng đồng.</p>
      </section>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { register } from '../api/authApi'

const router = useRouter()
const loading = ref(false)
const form = reactive({ hoTen: '', email: '', password: '', confirmPassword: '' })

async function submit() {
  if (!form.hoTen.trim() || !form.email.trim() || !form.password.trim()) {
    ElMessage.warning('Vui lòng nhập đầy đủ thông tin.')
    return
  }

  if (form.password.length < 6) {
    ElMessage.warning('Mật khẩu nên có tối thiểu 6 ký tự.')
    return
  }

  if (form.password !== form.confirmPassword) {
    ElMessage.warning('Mật khẩu xác nhận không khớp.')
    return
  }

  loading.value = true
  try {
    await register({ hoTen: form.hoTen.trim(), email: form.email.trim(), password: form.password })
    ElMessage.success('Đăng ký thành công. Vui lòng đăng nhập.')
    router.push('/login')
  } catch (error) {
    ElMessage.error(error.message || 'Đăng ký thất bại.')
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
