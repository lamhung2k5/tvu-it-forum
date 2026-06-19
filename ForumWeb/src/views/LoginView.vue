<template>
  <div class="auth-page">
    <div class="auth-card">
      <section class="auth-form-panel">
        <el-button text class="back-link" @click="$router.push('/')">
          ← Về trang chủ
        </el-button>

        <h2>Đăng nhập</h2>
        <p class="muted">Truy cập diễn đàn để đặt câu hỏi, trả lời và bình chọn.</p>

        <el-form :model="form" label-position="top" class="auth-form" @keyup.enter="submit">
          <el-form-item label="Email">
            <el-input
              v-model="form.email"
              type="email"
              placeholder="you@example.com"
              size="large"
              clearable
            />
          </el-form-item>

          <el-form-item label="Mật khẩu">
            <el-input
              v-model="form.password"
              placeholder="Nhập mật khẩu"
              size="large"
              show-password
            />
          </el-form-item>

          <el-button
            type="primary"
            size="large"
            :loading="loading"
            class="full-button"
            @click="submit"
          >
            Đăng nhập
          </el-button>

          <el-button
            size="large"
            plain
            class="guest-button"
            @click="continueAsGuest"
          >
            Tiếp tục với tư cách khách
          </el-button>
        </el-form>

        <p class="auth-switch">
          Chưa có tài khoản?
          <router-link to="/register" class="orange-link">Đăng ký ngay</router-link>
        </p>
      </section>

      <div class="auth-visual">
        <img
          src="/images/auth-login.png"
          alt=""
          class="auth-visual-image"
        />

        <div class="auth-visual-content">
          <h2>TVU ITerFORUM</h2>
          <p>
            Nơi sinh viên CNTT trao đổi kiến thức về .NET, Vue,
            cơ sở dữ liệu, API và kinh nghiệm học tập.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { login } from '../api/authApi'
import { setAuth, isValidEmail, normalizeEmailValue } from '../utils/auth'

const router = useRouter()
const route = useRoute()
const loading = ref(false)

const form = reactive({
  email: '',
  password: ''
})

async function submit() {
  if (!form.email.trim() || !form.password.trim()) {
    ElMessage.warning('Vui lòng nhập email và mật khẩu.')
    return
  }

  const email = normalizeEmailValue(form.email)

  if (!isValidEmail(email)) {
    ElMessage.warning('Email không đúng định dạng.')
    return
  }

  loading.value = true

  try {
    const result = await login({
      email,
      password: form.password
    })

    const token = result.accessToken || result.AccessToken
    const user = result.user || result.User

    if (!token) {
      throw new Error('API đăng nhập chưa trả về AccessToken.')
    }

    setAuth(token, user)

    ElMessage.success('Đăng nhập thành công!')
    router.push(route.query.redirect || '/')
  } catch (error) {
    ElMessage.error(error.message || 'Đăng nhập thất bại.')
  } finally {
    loading.value = false
  }
}

function continueAsGuest() {
  ElMessage.info('Bạn đang xem diễn đàn với tư cách khách.')
  router.push('/')
}
</script>

<style scoped>
.auth-page {
  min-height: 620px;
  padding: 56px 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.auth-card {
  width: min(1120px, 100%);
  min-height: 520px;
  display: grid;
  grid-template-columns: 0.9fr 1.1fr;
  overflow: hidden;
  border-radius: 22px;
  background: #ffffff;
  border: 1px solid var(--forum-border);
  box-shadow: 0 22px 55px rgba(15, 23, 42, 0.12);
}

/* Khối form bên trái */
.auth-form-panel {
  padding: 56px 54px;
}

.auth-form-panel h2 {
  margin: 0 0 10px;
  color: var(--forum-text);
  font-size: 32px;
  line-height: 1.2;
  font-weight: 800;
  font-family: tahoma;
  letter-spacing: -0.2px;
}

.back-link {
  width: fit-content;
  padding-left: 0;
  margin-bottom: 18px;
  color: var(--forum-muted);
  font-weight: 500;
}

.back-link:hover {
  color: var(--forum-primary);
}

.muted {
  margin: 0;
  max-width: 360px;
  color: var(--forum-muted);
  font-size: 16px;
  line-height: 1.55;
}

.auth-form {
  margin-top: 28px;
}

.full-button {
  width: 100%;
  font-weight: 700;
}

.guest-button {
  width: 100%;
  margin-top: 12px;
  margin-left: 0;
  font-weight: 700;
  color: var(--forum-muted);
}

.guest-button:hover {
  color: var(--forum-primary);
  border-color: var(--forum-primary);
  background: #fff7ed;
}

.auth-switch {
  text-align: center;
  margin-top: 20px;
  color: var(--forum-muted);
}

/* Khối ảnh bên phải */
.auth-visual {
  position: relative;
  overflow: hidden;
  min-height: 520px;

  display: flex;
  align-items: center;
  justify-content: flex-start;

  padding: 56px 54px;
  color: #ffffff;
  background: var(--forum-primary);
}

/* Ảnh phủ kín vùng phải, không méo hình */
.auth-visual-image {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
  z-index: 0;
}

/* Lớp phủ để chữ trắng đọc rõ */
.auth-visual::after {
  content: '';
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

/* Chữ nằm trong hình */
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
    min-height: 260px;
    padding: 36px 32px;
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

