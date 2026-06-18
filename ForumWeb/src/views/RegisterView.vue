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
            <el-input
              v-model="form.email"
              type="email"
              placeholder="1101230015@st.tvu.edu.vn"
              size="large"
              clearable
            />
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

      <div class="auth-visual">
        <img
          src="/images/auth-register.png"
          alt=""
          class="auth-visual-image"
        />

        <div class="auth-visual-content">
          <h2>Hỏi đúng chỗ, học nhanh hơn</h2>
          <p>
            Đặt câu hỏi, gắn tag, nhận câu trả lời, bình chọn giải pháp tốt
            và lưu lại kiến thức cho cộng đồng.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { register } from '../api/authApi'
import { isValidTvuStudentEmail, normalizeEmailValue } from '../utils/auth'
import { isValidEmail } from '../utils/auth'

const router = useRouter()
const loading = ref(false)
const form = reactive({ hoTen: '', email: '', password: '', confirmPassword: '' })

async function submit() {
  if (!form.hoTen.trim() || !form.email.trim() || !form.password.trim()) {
    ElMessage.warning('Vui lòng nhập đầy đủ thông tin.')
    return
  }

  const email = normalizeEmailValue(form.email)

  if (!isValidTvuStudentEmail(email)) {
    ElMessage.warning('Email sinh viên không hợp lệ. Định dạng đúng: 1101230015@st.tvu.edu.vn')
    return
  }

  if (!isValidEmail(form.email)) {
    ElMessage.warning('Email không đúng định dạng. Ví dụ đúng: sinhvien@st.tvu.edu.vn')
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
    await register({
      hoTen: form.hoTen.trim(),
      email,
      password: form.password
    })
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
.auth-page {
  min-height: 660px;
  padding: 56px 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.auth-card {
  width: min(1120px, 100%);
  min-height: 580px;
  display: grid;
  grid-template-columns: 0.9fr 1.1fr;
  gap: 0;
  overflow: hidden;
  border-radius: 22px;
  background: #ffffff;
  border: 1px solid var(--forum-border);
  box-shadow: 0 22px 55px rgba(15, 23, 42, 0.12);
}

/* Khối form bên trái */
.auth-form-panel {
  padding: 46px 44px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  background: #ffffff;
}

.back-link {
  width: fit-content;
  padding-left: 0;
  margin-bottom: 16px;
  color: var(--forum-muted);
  font-weight: 500;
}

.back-link:hover {
  color: var(--forum-primary);
}

.auth-form-panel h2 {
  margin: 0 0 10px;
  color: var(--forum-text);
  font-size: 32px;
  line-height: 1.25;
  font-weight: 700;
  font-family: inherit;
  letter-spacing: -0.2px;
}

.muted {
  margin: 0;
  max-width: 380px;
  color: var(--forum-muted);
  font-size: 16px;
  line-height: 1.55;
}

.auth-form {
  margin-top: 24px;
}

.full-button {
  width: 100%;
  font-weight: 700;
}

.auth-switch {
  text-align: center;
  margin-top: 18px;
  color: var(--forum-muted);
}

/* Khối ảnh bên phải */
.auth-visual {
  position: relative;
  overflow: hidden;
  min-height: 580px;

  display: flex;
  align-items: center;
  justify-content: flex-start;

  padding: 56px 54px;
  color: #ffffff;
  background: var(--forum-primary);
}

/* Ảnh phủ kín vùng phải */
.auth-visual-image {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
  z-index: 0;
}

.auth-visual::after {
  position: absolute;
  inset: 0;
  z-index: 1;
  background: linear-gradient(
    180deg,
    rgba(15, 23, 42, 0.18) 0%,
    rgba(15, 23, 42, 0.42) 45%,
    rgba(15, 23, 42, 0.78) 100%
  );
}


/* Chữ nằm giữa chiều dọc của ảnh */
.auth-visual-content {
  position: relative;
  z-index: 2;
  max-width: 470px;
}

.auth-visual h2 {
  margin: 0 0 14px;
  color: #ffffff;
  font-size: 32px;
  line-height: 1.25;
  font-weight: 600;
  font-family: inherit;
  letter-spacing: -0.2px;
  text-shadow: none;
}

.auth-visual p {
  margin: 0;
  color: rgba(255, 255, 255, 0.92);
  font-size: 16px;
  line-height: 1.65;
  font-weight: 400;
  text-shadow: none;
}

/* Tablet */
@media (max-width: 900px) {
  .auth-page {
    padding: 36px 16px;
  }

  .auth-card {
    grid-template-columns: 1fr;
    max-width: 620px;
  }

  .auth-form-panel {
    padding: 42px 32px;
  }

  .auth-visual {
    min-height: 280px;
    padding: 38px 32px;
  }

  .auth-visual h2 {
    font-size: 28px;
  }

  .auth-visual p {
    font-size: 15px;
  }
}

/* Điện thoại */
@media (max-width: 640px) {
  .auth-page {
    padding: 24px 12px;
  }

  .auth-card {
    border-radius: 18px;
  }

  .auth-form-panel {
    padding: 34px 22px;
  }

  .auth-form-panel h2 {
    font-size: 28px;
  }

  .auth-visual {
    display: none;
  }
}
</style>