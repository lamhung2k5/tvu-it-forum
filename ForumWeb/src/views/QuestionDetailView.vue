<template>
  <div class="page-container detail-page">
    <el-skeleton v-if="loading" :rows="8" animated />

    <template v-else-if="question">
      <el-card shadow="never" class="question-detail-card">
        <div class="question-detail-layout">
          <VoteBox
            :score="question.diemBinhChon"
            :disabled="!isLoggedIn"
            @vote="voteQuestionHandler"
          />

          <div class="question-body">
            <div class="detail-head">
              <div>
                <h1 class="page-title">{{ question.tieuDe }}</h1>
                <p class="page-subtitle">
                  {{ question.tenChuyenMuc }} · {{ question.hoTen }} · {{ formatDate(question.ngayTao) }}
                </p>
              </div>

              <div v-if="isQuestionOwner" class="owner-actions">
                <el-button
                  size="small"
                  @click="$router.push(`/questions/${questionId}/edit`)"
                >
                  Sửa
                </el-button>

                <el-button
                  size="small"
                  type="danger"
                  plain
                  @click="deleteQuestionHandler"
                >
                  Xóa
                </el-button>
              </div>
            </div>

            <p class="content-text">{{ question.noiDung }}</p>

            <div class="tag-list" v-if="question.tags.length">
              <el-tag
                v-for="tag in question.tags"
                :key="tag"
                class="forum-tag"
              >
                {{ tag }}
              </el-tag>
            </div>
          </div>
        </div>
      </el-card>

      <el-card shadow="never" class="section-card question-comment-card">
        <template #header>Bình luận câu hỏi</template>

        <EmptyState
          v-if="questionComments.length === 0"
          description="Chưa có bình luận cho câu hỏi."
        />

        <div
          v-for="comment in questionComments"
          :key="comment.id"
          class="comment-item"
        >
          <p>{{ comment.noiDung }}</p>
          <small>{{ comment.hoTen }} · {{ formatDate(comment.ngayTao) }}</small>

          <span v-if="isOwner(comment)" class="inline-actions">
            <el-button link size="small" @click="editComment(comment)">
              Sửa
            </el-button>

            <el-button
              link
              type="danger"
              size="small"
              @click="deleteCommentHandler(comment)"
            >
              Xóa
            </el-button>
          </span>
        </div>

        <div v-if="isLoggedIn" class="comment-form">
          <el-input
            v-model="newQuestionComment"
            type="textarea"
            :rows="2"
            placeholder="Viết bình luận cho câu hỏi..."
          />

          <el-button
            type="primary"
            :loading="commentingQuestion"
            @click="submitQuestionComment"
          >
            Gửi bình luận
          </el-button>
        </div>

        <el-alert
          v-else
          title="Đăng nhập để bình luận câu hỏi."
          type="info"
          show-icon
          :closable="false"
        />
      </el-card>

      <section class="answers-section">
        <div class="section-title">
          <h2>{{ answers.length }} câu trả lời</h2>
        </div>

        <EmptyState
          v-if="answers.length === 0"
          description="Chưa có câu trả lời nào. Hãy chia sẻ giải pháp của bạn."
        />

        <AnswerCard
          v-for="answer in answers"
          :key="answer.id"
          :answer="answer"
          detail-mode
          :is-owner="isOwner(answer)"
          :can-accept="isQuestionOwner && Number(answer.daChapNhan) !== 1"
          :can-unaccept="isQuestionOwner && Number(answer.daChapNhan) === 1"
          :can-interact="isLoggedIn"
          @vote="voteAnswerHandler(answer, $event)"
          @accept="acceptAnswerHandler(answer)"
          @unaccept="unacceptAnswerHandler(answer)"
          @edit="editAnswer(answer)"
          @delete="deleteAnswerHandler(answer)"
        >
          <div class="answer-comments">
            <h4>Bình luận</h4>

            <div
              v-for="comment in answer.comments"
              :key="comment.id"
              class="comment-item mini"
            >
              <p>{{ comment.noiDung }}</p>
              <small>{{ comment.hoTen }} · {{ formatDate(comment.ngayTao) }}</small>

              <span v-if="isOwner(comment)" class="inline-actions">
                <el-button link size="small" @click="editComment(comment)">
                  Sửa
                </el-button>

                <el-button
                  link
                  type="danger"
                  size="small"
                  @click="deleteCommentHandler(comment)"
                >
                  Xóa
                </el-button>
              </span>
            </div>

            <p v-if="!answer.comments.length" class="muted">
              Chưa có bình luận.
            </p>

            <div v-if="isLoggedIn" class="comment-form compact">
              <el-input
                v-model="answer.newComment"
                size="small"
                placeholder="Bình luận câu trả lời..."
              />

              <el-button
                size="small"
                type="primary"
                @click="submitAnswerComment(answer)"
              >
                Gửi
              </el-button>
            </div>
          </div>
        </AnswerCard>
      </section>

      <el-card shadow="never" class="section-card answer-form-card">
        <template #header>Trả lời câu hỏi</template>

        <template v-if="isLoggedIn">
          <el-input
            v-model="newAnswer"
            type="textarea"
            :rows="5"
            placeholder="Nhập câu trả lời của bạn..."
          />

          <div class="form-actions top-gap">
            <el-button
              type="primary"
              :loading="answering"
              @click="submitAnswer"
            >
              Gửi câu trả lời
            </el-button>
          </div>
        </template>

        <el-alert
          v-else
          title="Bạn cần đăng nhập để trả lời câu hỏi."
          type="info"
          show-icon
          :closable="false"
        />
      </el-card>
    </template>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getQuestionById, deleteQuestion } from '../api/questionApi'
import {
  getAnswersByQuestion,
  createAnswer,
  updateAnswer,
  deleteAnswer,
  acceptAnswer,
  unacceptAnswer
} from '../api/answerApi'
import { voteQuestion, voteAnswer } from '../api/voteApi'
import {
  createQuestionComment,
  getQuestionComments,
  getAnswerComments,
  createAnswerComment,
  updateComment,
  deleteComment
} from '../api/commentApi'
import { getCurrentUser, isAuthenticated } from '../utils/auth'
import {
  normalizeQuestion,
  normalizeAnswer,
  normalizeComment,
  formatDate,
  isMine
} from '../utils/format'
import VoteBox from '../components/VoteBox.vue'
import AnswerCard from '../components/AnswerCard.vue'
import EmptyState from '../components/EmptyState.vue'

const route = useRoute()
const router = useRouter()

const questionId = route.params.id
const loading = ref(false)

const question = ref(null)
const answers = ref([])
const questionComments = ref([])

const newAnswer = ref('')
const newQuestionComment = ref('')

const answering = ref(false)
const commentingQuestion = ref(false)

const currentUser = computed(() => getCurrentUser())
const isLoggedIn = computed(() => isAuthenticated())
const isQuestionOwner = computed(() => {
  return question.value && isMine(question.value.userId, currentUser.value)
})

onMounted(loadAll)

async function loadAll() {
  loading.value = true

  try {
    await Promise.all([
      loadQuestion(),
      loadAnswers(),
      loadQuestionComments()
    ])
  } finally {
    loading.value = false
  }
}

async function loadQuestion() {
  try {
    const tangLuotXem = shouldIncreaseQuestionView(questionId)

    question.value = normalizeQuestion(await getQuestionById(questionId, {
      tangLuotXem
    }))

    if (tangLuotXem) {
      saveQuestionViewTime(questionId)
    }
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được câu hỏi.')
    router.push('/')
  }
}

function getQuestionViewKey(id) {
  return `forum_question_view_${id}`
}

function shouldIncreaseQuestionView(id) {
  const key = getQuestionViewKey(id)
  const lastViewedAt = Number(localStorage.getItem(key) || 0)
  const now = Date.now()

  // 30 phút mới tính thêm 1 lượt xem
  const cooldown = 30 * 60 * 1000

  return !lastViewedAt || now - lastViewedAt > cooldown
}

function saveQuestionViewTime(id) {
  localStorage.setItem(getQuestionViewKey(id), String(Date.now()))
}

async function loadQuestionComments() {
  try {
    questionComments.value = (await getQuestionComments(questionId) || []).map(normalizeComment)
  } catch {
    questionComments.value = []
  }
}

async function loadAnswers() {
  try {
    const data = (await getAnswersByQuestion(questionId) || []).map(normalizeAnswer)

    const enriched = await Promise.all(data.map(async answer => {
      try {
        const comments = (await getAnswerComments(answer.id) || []).map(normalizeComment)
        return { ...answer, comments, newComment: '' }
      } catch {
        return { ...answer, comments: [], newComment: '' }
      }
    }))

    answers.value = enriched
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được câu trả lời.')
  }
}

function isOwner(item) {
  return isMine(item.userId, currentUser.value)
}

async function voteQuestionHandler(giaTri) {
  try {
    await voteQuestion(questionId, giaTri)
    await loadQuestion()
  } catch (error) {
    ElMessage.error(error.message || 'Bình chọn thất bại.')
  }
}

async function voteAnswerHandler(answer, giaTri) {
  try {
    await voteAnswer(answer.id, giaTri)
    await loadAnswers()
  } catch (error) {
    ElMessage.error(error.message || 'Bình chọn câu trả lời thất bại.')
  }
}

async function submitAnswer() {
  if (!newAnswer.value.trim()) {
    return ElMessage.warning('Vui lòng nhập nội dung câu trả lời.')
  }

  answering.value = true

  try {
    await createAnswer(questionId, {
      noiDung: newAnswer.value.trim()
    })

    newAnswer.value = ''

    ElMessage.success('Đã gửi câu trả lời.')
    await loadAnswers()
    await loadQuestion()
  } catch (error) {
    ElMessage.error(error.message || 'Gửi câu trả lời thất bại.')
  } finally {
    answering.value = false
  }
}

async function editAnswer(answer) {
  try {
    const { value } = await ElMessageBox.prompt(
      'Chỉnh sửa câu trả lời',
      'Sửa câu trả lời',
      {
        inputValue: answer.noiDung,
        inputType: 'textarea',
        confirmButtonText: 'Lưu',
        cancelButtonText: 'Hủy'
      }
    )

    if (!value.trim()) return

    await updateAnswer(answer.id, {
      noiDung: value.trim()
    })

    ElMessage.success('Đã cập nhật câu trả lời.')
    await loadAnswers()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể sửa câu trả lời.')
    }
  }
}

async function deleteAnswerHandler(answer) {
  try {
    await ElMessageBox.confirm(
      'Bạn có chắc muốn xóa câu trả lời này?',
      'Xác nhận',
      { type: 'warning' }
    )

    await deleteAnswer(answer.id)

    ElMessage.success('Đã xóa câu trả lời.')
    await loadAnswers()
    await loadQuestion()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể xóa câu trả lời.')
    }
  }
}

async function acceptAnswerHandler(answer) {
  try {
    await acceptAnswer(answer.id)

    ElMessage.success('Đã chọn câu trả lời được chấp nhận.')
    await loadAnswers()
  } catch (error) {
    ElMessage.error(error.message || 'Không thể chọn câu trả lời.')
  }
}

async function unacceptAnswerHandler(answer) {
  try {
    await ElMessageBox.confirm(
      'Bạn có chắc muốn thu hồi câu trả lời được chấp nhận không?',
      'Xác nhận thu hồi',
      {
        confirmButtonText: 'Thu hồi',
        cancelButtonText: 'Hủy',
        type: 'warning'
      }
    )

    await unacceptAnswer(answer.id)

    ElMessage.success('Đã thu hồi câu trả lời được chấp nhận.')
    await loadAnswers()
  } catch (error) {
    if (error === 'cancel' || error === 'close') return

    ElMessage.error(error.message || 'Không thể thu hồi câu trả lời được chấp nhận.')
  }
}

async function submitQuestionComment() {
  if (!newQuestionComment.value.trim()) {
    return ElMessage.warning('Vui lòng nhập nội dung bình luận.')
  }

  commentingQuestion.value = true

  try {
    await createQuestionComment(questionId, {
      noiDung: newQuestionComment.value.trim()
    })

    newQuestionComment.value = ''

    ElMessage.success('Đã gửi bình luận.')
    await loadQuestionComments()
    await loadQuestion()
  } catch (error) {
    ElMessage.error(error.message || 'Gửi bình luận thất bại.')
  } finally {
    commentingQuestion.value = false
  }
}

async function submitAnswerComment(answer) {
  if (!answer.newComment?.trim()) return

  try {
    await createAnswerComment(answer.id, {
      noiDung: answer.newComment.trim()
    })

    answer.newComment = ''
    await loadAnswers()
  } catch (error) {
    ElMessage.error(error.message || 'Gửi bình luận thất bại.')
  }
}

async function editComment(comment) {
  try {
    const { value } = await ElMessageBox.prompt(
      'Chỉnh sửa bình luận',
      'Sửa bình luận',
      {
        inputValue: comment.noiDung,
        inputType: 'textarea',
        confirmButtonText: 'Lưu',
        cancelButtonText: 'Hủy'
      }
    )

    if (!value.trim()) return

    await updateComment(comment.id, {
      noiDung: value.trim()
    })

    ElMessage.success('Đã cập nhật bình luận.')
    await loadQuestionComments()
    await loadAnswers()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể sửa bình luận.')
    }
  }
}

async function deleteCommentHandler(comment) {
  try {
    await ElMessageBox.confirm(
      'Bạn có chắc muốn xóa bình luận này?',
      'Xác nhận',
      { type: 'warning' }
    )

    await deleteComment(comment.id)

    ElMessage.success('Đã xóa bình luận.')
    await loadQuestionComments()
    await loadAnswers()
    await loadQuestion()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể xóa bình luận.')
    }
  }
}

async function deleteQuestionHandler() {
  try {
    await ElMessageBox.confirm(
      'Bạn có chắc muốn xóa câu hỏi này?',
      'Xác nhận',
      { type: 'warning' }
    )

    await deleteQuestion(questionId)

    ElMessage.success('Đã xóa câu hỏi.')
    router.push('/')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || 'Không thể xóa câu hỏi.')
    }
  }
}
</script>

<style scoped>
.detail-page {
  max-width: 980px;
}

.question-detail-card,
.section-card {
  border-radius: 14px;
  margin-bottom: 16px;
}

.question-detail-layout {
  display: flex;
  gap: 16px;
}

.question-body {
  flex: 1;
  min-width: 0;
}

.detail-head {
  display: flex;
  justify-content: space-between;
  gap: 14px;
}

.owner-actions {
  display: flex;
  gap: 8px;
}

.content-text {
  white-space: pre-wrap;
  line-height: 1.75;
  color: #3b4045;
}

.question-comment-card {
  margin-bottom: 20px;
}

.answers-section {
  margin-top: 20px;
  margin-bottom: 22px;
}

.section-title h2 {
  margin: 0 0 14px;
}

.answers-section :deep(.answer-card) {
  margin-bottom: 16px;
}

.answer-form-card {
  margin-top: 20px;
}

.comment-item {
  border-top: 1px solid var(--forum-border);
  padding: 10px 0;
}

.comment-item:first-child {
  border-top: none;
}

.comment-item p {
  margin: 0 0 4px;
  line-height: 1.55;
}

.comment-item small {
  color: var(--forum-muted);
}

.inline-actions {
  margin-left: 8px;
}

.comment-form {
  margin-top: 12px;
  display: flex;
  gap: 10px;
  align-items: flex-start;
}

.comment-form.compact {
  margin-top: 10px;
  align-items: center;
}

.answer-comments {
  margin-top: 14px;
  background: #fbfbfc;
  border: 1px solid var(--forum-border);
  border-radius: 12px;
  padding: 12px;
}

.answer-comments h4 {
  margin: 0 0 8px;
}

.top-gap {
  margin-top: 12px;
}

@media (max-width: 640px) {
  .question-detail-layout,
  .detail-head,
  .comment-form {
    flex-direction: column;
  }
}
</style>