<template>
  <div class="page-container profile-page">
    <el-skeleton v-if="loading" :rows="6" animated />
    <el-card v-else-if="profile" shadow="never" class="profile-card">
      <div class="profile-head">
        <UserAvatar :name="profile.hoTen" :src="profile.anhDaiDien" :size="88" />
        <div class="profile-info">
          <h1>{{ profile.hoTen }}</h1>
          <p>Thành viên diễn đàn · {{ profile.vaiTro }}</p>
          <el-tag type="warning">Profile công khai</el-tag>
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
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getPublicProfile } from '../api/userApi'
import { normalizeProfile } from '../utils/format'
import UserAvatar from '../components/UserAvatar.vue'
import MetricCard from '../components/MetricCard.vue'

const props = defineProps({ id: { type: [String, Number], required: true } })
const loading = ref(false)
const profile = ref(null)

onMounted(async () => {
  loading.value = true
  try {
    profile.value = normalizeProfile(await getPublicProfile(props.id))
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được hồ sơ người dùng.')
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.profile-page { max-width: 900px; }
.profile-card { border-radius: 14px; }
.profile-head { display: flex; align-items: center; gap: 18px; }
.profile-info h1 { margin: 0 0 6px; }
.profile-info p { margin: 0 0 8px; color: var(--forum-muted); }
@media (max-width: 640px) { .profile-head { flex-direction: column; align-items: flex-start; } }
</style>
