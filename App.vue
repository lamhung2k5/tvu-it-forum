<template>
  <div class="auth-wrapper">
    <el-card class="auth-card" shadow="always">
      
      <div v-if="currentPage === 'register'">
        <div class="auth-header">
          <h2 class="title">Tạo Tài Khoản Mới</h2>
          <p class="subtitle">Tham gia cộng đồng diễn đàn chuyên ngành</p>
        </div>

        <el-form ref="registerFormRef" :model="registerData" :rules="registerRules" label-position="top" status-icon>
          <el-form-item label="Tên tài khoản (Username)" prop="username">
            <el-input v-model="registerData.username" placeholder="Ví dụ: nguyenvan_it" prefix-icon="User" clearable />
          </el-form-item>

          <el-form-item label="Địa chỉ Email" prop="email">
            <el-input v-model="registerData.email" placeholder="Ví dụ: example@domain.com" prefix-icon="Message" clearable />
          </el-form-item>

          <el-form-item label="Mật khẩu" prop="password">
            <el-input v-model="registerData.password" type="password" placeholder="Tối thiểu 8 ký tự" prefix-icon="Lock" show-password />
          </el-form-item>

          <el-form-item label="Xác nhận mật khẩu" prop="confirmPassword">
            <el-input v-model="registerData.confirmPassword" type="password" placeholder="Nhập lại mật khẩu" prefix-icon="CircleCheck" show-password />
          </el-form-item>

          <div class="form-actions">
            <el-button type="primary" :loading="isSubmitting" class="submit-btn" @click="handleRegister">
              Đăng Ký Ngay
            </el-button>
          </div>
        </el-form>

        <div class="auth-footer">
          <span>Bạn đã có tài khoản? </span>
          <el-link type="primary" :underlined="false" @click="currentPage = 'login'">Đăng nhập ngay</el-link>
        </div>
      </div>

      <div v-else-if="currentPage === 'login'">
        <div class="auth-header">
          <h2 class="title">Chào Mừng Quay Trở Lại</h2>
          <p class="subtitle">Đăng nhập để tiếp tục chia sẻ tri thức</p>
        </div>

        <el-form ref="loginFormRef" :model="loginData" :rules="loginRules" label-position="top" status-icon>
          <el-form-item label="Tên tài khoản hoặc Email" prop="account">
            <el-input v-model="loginData.account" placeholder="Nhập username hoặc email..." prefix-icon="User" clearable />
          </el-form-item>

          <el-form-item label="Mật khẩu" prop="password">
            <el-input v-model="loginData.password" type="password" placeholder="Nhập mật khẩu..." prefix-icon="Lock" show-password />
          </el-form-item>

          <div class="login-options">
            <el-checkbox v-model="loginData.remember">Ghi nhớ đăng nhập</el-checkbox>
            <el-link type="danger" :underlined="false" @click="currentPage = 'forgot'">Quên mật khẩu?</el-link>
          </div>

          <div class="form-actions">
            <el-button type="success" :loading="isSubmitting" class="submit-btn" @click="handleLogin">
              Đăng Nhập
            </el-button>
          </div>
        </el-form>

        <div class="auth-footer">
          <span>Chưa có tài khoản diễn đàn? </span>
          <el-link type="primary" :underlined="false" @click="currentPage = 'register'">Đăng ký tài khoản</el-link>
        </div>
      </div>

      <div v-else-if="currentPage === 'forgot'">
        <div class="auth-header">
          <h2 class="title">Khôi Phục Mật Khẩu</h2>
          <p class="subtitle">Nhập email đăng ký tài khoản để nhận mã xác minh OTP</p>
        </div>

        <el-form ref="forgotFormRef" :model="forgotData" :rules="forgotRules" label-position="top">
          <el-form-item label="Email khôi phục" prop="email">
            <el-input v-model="forgotData.email" placeholder="Nhập email của bạn..." prefix-icon="Message" clearable />
          </el-form-item>

          <div class="form-actions">
            <el-button type="warning" :loading="isSubmitting" class="submit-btn" @click="handleSendOtp">
              Gửi Mã Xác Nhận Qua Gmail
            </el-button>
          </div>
        </el-form>

        <div class="auth-footer">
          <el-link type="info" :underlined="false" icon="ArrowLeft" @click="currentPage = 'login'">Quay lại Đăng nhập</el-link>
        </div>
      </div>

      <div v-else-if="currentPage === 'reset'">
        <div class="auth-header">
          <h2 class="title">Thiết Lập Mật Khẩu Mới</h2>
          <el-alert 
            :title="`Hệ thống đã gửi mã OTP gồm 6 chữ số đến email: ${forgotData.email}`" 
            type="info" 
            :closable="false"
            style="margin-top: 10px;"
          />
        </div>

        <el-form ref="resetFormRef" :model="resetData" :rules="resetRules" label-position="top" status-icon>
          <el-form-item label="Mã xác thực OTP (Giả lập: 123456)" prop="otp">
            <el-input v-model="resetData.otp" placeholder="Nhập 6 chữ số..." maxlength="6" prefix-icon="Key" clearable />
          </el-form-item>

          <el-form-item label="Mật khẩu mới" prop="newPassword">
            <el-input v-model="resetData.newPassword" type="password" placeholder="Tối thiểu 8 ký tự" prefix-icon="Lock" show-password />
          </el-form-item>

          <el-form-item label="Xác nhận mật khẩu mới" prop="confirmNewPassword">
            <el-input v-model="resetData.confirmNewPassword" type="password" placeholder="Nhập lại mật khẩu mới" prefix-icon="CircleCheck" show-password />
          </el-form-item>

          <div class="form-actions">
            <el-button type="primary" :loading="isSubmitting" class="submit-btn" @click="handleResetPassword">
              Xác Nhận Đổi Mật Khẩu
            </el-button>
          </div>
        </el-form>
      </div>

    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'

// Điều hướng trang chính: 'register', 'login', 'forgot', 'reset'
const currentPage = ref('login') 
const isSubmitting = ref(false)

const registerFormRef = ref(null)
const loginFormRef = ref(null)
const forgotFormRef = ref(null)
const resetFormRef = ref(null)

// --- DATA & RULES CHO ĐĂNG KÝ ---
const registerData = reactive({ username: '', email: '', password: '', confirmPassword: '' })
const registerRules = {
  username: [{ required: true, message: 'Vui lòng nhập tên tài khoản', trigger: 'blur' }],
  email: [{ required: true, message: 'Vui lòng nhập email', trigger: 'blur' }, { type: 'email', message: 'Email không hợp lệ', trigger: 'blur' }],
  password: [{ required: true, message: 'Vui lòng nhập mật khẩu', trigger: 'blur' }, { min: 8, message: 'Mật khẩu tối thiểu 8 ký tự', trigger: 'blur' }],
  confirmPassword: [{ required: true, message: 'Vui lòng xác nhận mật khẩu', trigger: 'blur' }, {
    validator: (rule, value, callback) => {
      if (value !== registerData.password) callback(new Error('Mật khẩu xác nhận không khớp!'))
      else callback()
    }, trigger: 'change'
  }]
}

// --- DATA & RULES CHO ĐĂNG NHẬP ---
const loginData = reactive({ account: '', password: '', remember: false })
const loginRules = {
  account: [{ required: true, message: 'Vui lòng nhập tài khoản hoặc email', trigger: 'blur' }],
  password: [{ required: true, message: 'Vui lòng nhập mật khẩu', trigger: 'blur' }]
}

// --- DATA & RULES CHO QUÊN MẬT KHẨU ---
const forgotData = reactive({ email: '' })
const forgotRules = {
  email: [
    { required: true, message: 'Vui lòng nhập địa chỉ email', trigger: 'blur' },
    { type: 'email', message: 'Email không đúng định dạng', trigger: 'blur' }
  ]
}

// --- DATA & RULES CHO ĐẶT LẠI MẬT KHẨU ---
const resetData = reactive({ otp: '', newPassword: '', confirmNewPassword: '' })
const resetRules = {
  otp: [
    { required: true, message: 'Vui lòng nhập mã OTP', trigger: 'blur' },
    { len: 6, message: 'Mã OTP phải có đúng 6 chữ số', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: 'Vui lòng nhập mật khẩu mới', trigger: 'blur' },
    { min: 8, message: 'Mật khẩu mới tối thiểu phải 8 ký tự', trigger: 'blur' }
  ],
  confirmNewPassword: [
    { required: true, message: 'Vui lòng xác nhận mật khẩu mới', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== resetData.newPassword) callback(new Error('Mật khẩu mới nhập lại không khớp!'))
        else callback()
      }, trigger: 'change'
    }
  ]
}

// --- XỬ LÝ HÀM (LOGIC FUNCTIONS) ---

const handleRegister = () => {
  registerFormRef.value.validate((valid) => {
    if (valid) {
      isSubmitting.value = true
      setTimeout(() => {
        isSubmitting.value = false
        ElMessage.success('Đăng ký tài khoản thành công!')
        loginData.account = registerData.username
        currentPage.value = 'login'
        registerFormRef.value.resetFields()
      }, 1000)
    }
  })
}

const handleLogin = () => {
  loginFormRef.value.validate((valid) => {
    if (valid) {
      isSubmitting.value = true
      setTimeout(() => {
        isSubmitting.value = false
        ElMessage.success(`Chào mừng ${loginData.account} đăng nhập thành công!`)
      }, 1000)
    }
  })
}

// Gửi OTP giả lập
const handleSendOtp = () => {
  if (!forgotFormRef.value) return
  forgotFormRef.value.validate((valid) => {
    if (valid) {
      isSubmitting.value = true
      
      setTimeout(() => {
        isSubmitting.value = false
        
        // Hiện hộp thoại thông báo mã OTP mã hóa giả lập để người dùng tiện test thử
        ElMessageBox.alert(
          `Hệ thống giả lập: Một email chứa mã OTP khôi phục đã được gửi tới <b>${forgotData.email}</b>.<br><br>Mã OTP dùng thử của bạn là: <b>123456</b>`,
          'Gửi Mail Thành Công',
          {
            dangerouslyUseHTMLString: true,
            confirmButtonText: 'Nhập OTP ngay',
            callback: () => {
              // Chuyển sang trang đặt lại mật khẩu mới
              currentPage.value = 'reset'
            }
          }
        )
      }, 1500)
    }
  })
}

// Đổi mật khẩu mới
const handleResetPassword = () => {
  if (!resetFormRef.value) return
  resetFormRef.value.validate((valid) => {
    if (valid) {
      // Kiểm tra xem OTP nhập vào có đúng mã giả lập "123456" không
      if (resetData.otp !== '123456') {
        ElMessage.error('Mã OTP không chính xác hoặc đã hết hạn!')
        return
      }

      isSubmitting.value = true
      setTimeout(() => {
        isSubmitting.value = false
        ElMessage({
          message: 'Đặt lại mật khẩu thành công! Mời bạn đăng nhập bằng mật khẩu mới.',
          type: 'success',
          duration: 4000
        })
        
        // Đổi xong lật về trang Đăng nhập luôn
        currentPage.value = 'login'
        
        // Reset sạch form
        resetFormRef.value.resetFields()
        forgotFormRef.value.resetFields()
      }, 1500)
    }
  })
}
</script>

<style>
body {
  margin: 0;
  font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
}
.auth-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  padding: 20px;
  box-sizing: border-box;
}
.auth-card {
  width: 100%;
  max-width: 450px;
  border-radius: 12px;
  padding: 10px;
}
.auth-header {
  text-align: center;
  margin-bottom: 25px;
}
.auth-header .title {
  font-size: 23px;
  color: #303133;
  margin: 0 0 8px 0;
  font-weight: 600;
}
.auth-header .subtitle {
  font-size: 14px;
  color: #909399;
  margin: 0;
}
.login-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}
.form-actions {
  margin-top: 20px;
}
.submit-btn {
  width: 100%;
  height: 40px;
  font-size: 15px;
  font-weight: bold;
}
.auth-footer {
  text-align: center;
  margin-top: 25px;
  font-size: 14px;
  color: #606266;
}
</style>