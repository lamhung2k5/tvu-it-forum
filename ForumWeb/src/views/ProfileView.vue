<template>
  <div class="page-container profile-page">
    <el-skeleton v-if="loading" :rows="6" animated />
    <template v-else-if="profile">
      <el-card shadow="never" class="profile-card">
        <div class="profile-head">
          <UserAvatar :name="profile.hoTen" :src="profile.anhDaiDien" :size="88" />
          <div class="profile-info">
            <h1>{{ profile.hoTen }}</h1>
            <p>{{ profile.email }}</p>
            <el-tag type="warning">{{ profile.vaiTro }}</el-tag>
          </div>
        </div>

        <el-divider />

        <div class="stat-grid">
          <MetricCard label="Câu hỏi" :value="profile.soCauHoi" />
          <MetricCard label="Câu trả lời" :value="profile.soCauTraLoi" />
          <MetricCard label="Bình luận" :value="profile.soBinhLuan" />
          <MetricCard label="Tổng điểm" :value="profile.tongDiemBinhChon" />
        </div>
      </el-card>

      <el-card shadow="never" class="edit-card">
        <template #header>Chỉnh sửa thông tin</template>
        <el-form :model="form" label-position="top">
          <el-form-item label="Họ tên">
            <el-input v-model="form.hoTen" placeholder="Nhập họ tên" />
          </el-form-item>
          <el-alert title="Ảnh đại diện hiện dùng chữ cái đầu. Chức năng upload ảnh thật có thể phát triển sau." type="info" show-icon :closable="false" />
          <div class="form-actions top-gap">
            <el-button type="primary" :loading="saving" @click="save">Lưu thay đổi</el-button>
          </div>
        </el-form>
      </el-card>
    </template>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getMyProfile, updateMyProfile } from '../api/userApi'
import { normalizeProfile } from '../utils/format'
import { saveUser, getCurrentUser } from '../utils/auth'
import UserAvatar from '../components/UserAvatar.vue'
import MetricCard from '../components/MetricCard.vue'

const loading = ref(false)
const saving = ref(false)
const profile = ref(null)
const form = reactive({ hoTen: '' })

onMounted(loadProfile)

async function loadProfile() {
  loading.value = true
  try {
    profile.value = normalizeProfile(await getMyProfile())
    form.hoTen = profile.value.hoTen
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được hồ sơ.')
  } finally {
    loading.value = false
  }
}

async function save() {
  if (!form.hoTen.trim()) return ElMessage.warning('Họ tên không được để trống.')
  saving.value = true
  try {
    await updateMyProfile({ hoTen: form.hoTen.trim() })
    const current = getCurrentUser()
    saveUser({ ...current, hoTen: form.hoTen.trim() })
    ElMessage.success('Đã cập nhật hồ sơ.')
    await loadProfile()
  } catch (error) {
    ElMessage.error(error.message || 'Cập nhật thất bại.')
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.profile-page { max-width: 900px; }
.profile-card, .edit-card { border-radius: 14px; margin-bottom: 16px; }
.profile-head { display: flex; align-items: center; gap: 18px; }
.profile-info h1 { margin: 0 0 6px; }
.profile-info p { margin: 0 0 8px; color: var(--forum-muted); }
.top-gap { margin-top: 14px; }
@media (max-width: 640px) { .profile-head { flex-direction: column; align-items: flex-start; } }
</style>
