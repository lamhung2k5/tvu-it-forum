<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Dashboard của tôi</h1>
        <p class="page-subtitle">Theo dõi hoạt động cá nhân trong diễn đàn.</p>
      </div>

      <el-button
        type="primary"
        class="btn-accent"
        @click="$router.push('/questions/create')"
      >
        Đặt câu hỏi
      </el-button>
    </div>

    <el-skeleton v-if="loading" :rows="8" animated />

    <template v-else>
      <el-card shadow="never" class="profile-summary" v-if="profile">
        <UserAvatar
          :name="profile.hoTen"
          :src="profile.anhDaiDien"
          :size="64"
        />

        <div>
          <h2>Xin chào, {{ profile.hoTen }}</h2>
          <p>{{ profile.email }} · {{ profile.vaiTro }}</p>
        </div>

        <el-button @click="$router.push('/profile')">
          Xem hồ sơ
        </el-button>
      </el-card>

      <div class="stat-grid">
        <MetricCard label="Câu hỏi" :value="profile?.soCauHoi || 0" />
        <MetricCard label="Câu trả lời" :value="profile?.soCauTraLoi || 0" />
        <MetricCard label="Bình luận" :value="profile?.soBinhLuan || 0" />
        <MetricCard label="Tổng điểm" :value="profile?.tongDiemBinhChon || 0" />
      </div>

      <div class="dashboard-grid">
        <el-card shadow="never" class="section-card">
          <template #header>
            <div class="section-head">
              <span>Câu hỏi gần đây</span>
              <el-button link @click="$router.push('/my-questions')">
                Xem tất cả
              </el-button>
            </div>
          </template>

          <EmptyState
            v-if="questions.length === 0"
            description="Bạn chưa đăng câu hỏi nào."
          />

          <QuestionCard
            v-for="q in questions"
            :key="q.id"
            :question="q"
          />
        </el-card>

        <el-card shadow="never" class="section-card">
          <template #header>
            <div class="section-head">
              <span>Câu trả lời gần đây</span>
              <el-button link @click="$router.push('/my-answers')">
                Xem tất cả
              </el-button>
            </div>
          </template>

          <EmptyState
            v-if="answers.length === 0"
            description="Bạn chưa có câu trả lời nào."
          />

          <AnswerCard
            v-for="a in answers"
            :key="a.id"
            :answer="a"
          />
        </el-card>
      </div>
    </template>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getMyDashboard } from '../api/userApi'
import { normalizeProfile, normalizeQuestion, normalizeAnswer, pick } from '../utils/format'
import UserAvatar from '../components/UserAvatar.vue'
import MetricCard from '../components/MetricCard.vue'
import QuestionCard from '../components/QuestionCard.vue'
import AnswerCard from '../components/AnswerCard.vue'
import EmptyState from '../components/EmptyState.vue'

const loading = ref(false)
const profile = ref(null)
const questions = ref([])
const answers = ref([])

onMounted(loadDashboard)

async function loadDashboard() {
  loading.value = true

  try {
    const data = await getMyDashboard()

    profile.value = normalizeProfile(pick(data, ['profile', 'Profile'], {}))

    questions.value = (pick(data, ['cauHoiGanDay', 'CauHoiGanDay'], []) || [])
      .map(normalizeQuestion)
      .filter(question => Number(question.isDeleted || question.IsDeleted || 0) === 0)

    answers.value = (pick(data, ['cauTraLoiGanDay', 'CauTraLoiGanDay'], []) || [])
      .map(normalizeAnswer)
      .filter(answer => Number(answer.isDeleted || answer.IsDeleted || 0) === 0)
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được dashboard.')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.profile-summary {
  border-radius: 14px;
  margin-bottom: 14px;
}

.profile-summary :deep(.el-card__body) {
  display: flex;
  align-items: center;
  gap: 16px;
}

.profile-summary h2 {
  margin: 0 0 4px;
}

.profile-summary p {
  margin: 0;
  color: var(--forum-muted);
}

.profile-summary .el-button {
  margin-left: auto;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: 1.2fr 0.8fr;
  gap: 16px;
  margin-top: 16px;
}

.section-card {
  border-radius: 14px;
}

.section-head {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

@media (max-width: 980px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
  }

  .profile-summary :deep(.el-card__body) {
    flex-wrap: wrap;
  }

  .profile-summary .el-button {
    margin-left: 0;
  }
}
</style>