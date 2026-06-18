<template>
  <div class="page-container profile-page">
    <el-skeleton v-if="loading" :rows="6" animated />

    <template v-else-if="profile">
      <el-card shadow="never" class="profile-card">
        <div class="profile-head">
          <UserAvatar
            :name="profile.hoTen"
            :src="profile.anhDaiDien"
            :size="88"
          />

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

      <el-card shadow="never" class="activity-card" v-loading="activityLoading">
        <el-tabs v-model="activeTab">
          <el-tab-pane label="Câu hỏi đã đăng" name="questions">
            <EmptyState
              v-if="questions.length === 0"
              description="Người dùng này chưa có câu hỏi công khai."
            />

            <QuestionCard
              v-for="question in questions"
              :key="question.id"
              :question="question"
            />
          </el-tab-pane>

          <el-tab-pane label="Câu trả lời đã đăng" name="answers">
            <EmptyState
              v-if="answers.length === 0"
              description="Người dùng này chưa có câu trả lời công khai."
            />

            <AnswerCard
              v-for="answer in answers"
              :key="answer.id"
              :answer="answer"
            />
          </el-tab-pane>
        </el-tabs>
      </el-card>
    </template>

    <EmptyState
      v-else
      description="Không tìm thấy hồ sơ người dùng."
    />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import {
  getPublicProfile,
  getPublicUserQuestions,
  getPublicUserAnswers
} from '../api/userApi'
import {
  normalizeProfile,
  normalizeQuestion,
  normalizeAnswer
} from '../utils/format'
import UserAvatar from '../components/UserAvatar.vue'
import MetricCard from '../components/MetricCard.vue'
import QuestionCard from '../components/QuestionCard.vue'
import AnswerCard from '../components/AnswerCard.vue'
import EmptyState from '../components/EmptyState.vue'

const props = defineProps({
  id: {
    type: [String, Number],
    required: true
  }
})

const loading = ref(false)
const activityLoading = ref(false)

const profile = ref(null)
const questions = ref([])
const answers = ref([])

const activeTab = ref('questions')

watch(
  () => props.id,
  () => {
    loadProfile()
  },
  { immediate: true }
)

async function loadProfile() {
  loading.value = true
  activityLoading.value = true

  try {
    const [profileData, questionData, answerData] = await Promise.all([
      getPublicProfile(props.id),
      getPublicUserQuestions(props.id),
      getPublicUserAnswers(props.id)
    ])

    profile.value = normalizeProfile(profileData)
    questions.value = (questionData || []).map(normalizeQuestion)
    answers.value = (answerData || []).map(normalizeAnswer)
  } catch (error) {
    profile.value = null
    questions.value = []
    answers.value = []

    ElMessage.error(error.message || 'Không tải được hồ sơ người dùng.')
  } finally {
    loading.value = false
    activityLoading.value = false
  }
}
</script>

<style scoped>
.profile-page {
  max-width: 980px;
}

.profile-card,
.activity-card {
  border-radius: 14px;
}

.profile-card {
  margin-bottom: 18px;
}

.profile-head {
  display: flex;
  align-items: center;
  gap: 18px;
}

.profile-info h1 {
  margin: 0 0 6px;
}

.profile-info p {
  margin: 0 0 8px;
  color: var(--forum-muted);
}

.activity-card :deep(.el-card__body) {
  padding-top: 10px;
}

.activity-card :deep(.question-card) {
  margin-bottom: 14px;
}

.activity-card :deep(.answer-card) {
  margin-bottom: 14px;
}

@media (max-width: 640px) {
  .profile-head {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>  