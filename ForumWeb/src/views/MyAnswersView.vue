<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Câu trả lời của tôi</h1>
        <p class="page-subtitle">Theo dõi các câu trả lời bạn đã đóng góp cho cộng đồng.</p>
      </div>
    </div>

    <el-card shadow="never" class="answer-list-card" v-loading="loading">
      <EmptyState v-if="!loading && answers.length === 0" description="Bạn chưa có câu trả lời nào." />
      <div v-for="answer in answers" :key="answer.id" class="answer-row">
        <div>
          <router-link :to="`/questions/${answer.cauHoiId}`" class="answer-title">{{ answer.tieuDeCauHoi }}</router-link>
          <p>{{ answer.noiDung }}</p>
          <div class="answer-meta">
            <el-tag v-if="answer.daChapNhan === 1" type="success">Được chấp nhận</el-tag>
            <el-tag v-if="answer.isDeleted === 1" type="danger">Đã xóa mềm</el-tag>
            <span>{{ answer.diemBinhChon }} điểm</span>
            <span>{{ answer.soBinhLuan }} bình luận</span>
            <span>{{ formatDate(answer.ngayTao) }}</span>
          </div>
        </div>
        <el-button @click="$router.push(`/questions/${answer.cauHoiId}`)">Xem câu hỏi</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getMyAnswers } from '../api/userApi'
import { normalizeAnswer, formatDate } from '../utils/format'
import EmptyState from '../components/EmptyState.vue'

const loading = ref(false)
const answers = ref([])

onMounted(load)
async function load() {
  loading.value = true
  try {
    answers.value = (await getMyAnswers() || []).map(normalizeAnswer)
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được câu trả lời của tôi.')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.answer-list-card { border-radius: 14px; }
.answer-row { display: flex; justify-content: space-between; gap: 18px; border-bottom: 1px solid var(--forum-border); padding: 16px 0; }
.answer-row:last-child { border-bottom: none; }
.answer-title { color: #0c65a5; font-weight: 700; font-size: 17px; }
.answer-row p { color: var(--forum-muted); line-height: 1.6; }
.answer-meta { display: flex; gap: 8px; flex-wrap: wrap; color: var(--forum-muted); font-size: 13px; }
@media (max-width: 760px) { .answer-row { flex-direction: column; } }
</style>
