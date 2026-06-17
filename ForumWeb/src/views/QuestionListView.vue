<template>
  <div class="page-container">
    <section class="home-hero">
      <div>
        <el-tag type="warning" effect="light">Diễn đàn hỏi đáp IT</el-tag>
        <h1>Hỏi đáp lập trình, cơ sở dữ liệu và công nghệ</h1>
        <p>Chia sẻ vấn đề bạn đang gặp, nhận câu trả lời từ cộng đồng và lưu lại tri thức cho sinh viên CNTT.</p>
      </div>
      <el-button v-if="isLoggedIn" type="primary" size="large" @click="$router.push('/questions/create')">Đặt câu hỏi</el-button>
      <el-button v-else size="large" @click="$router.push('/login')">Đăng nhập để hỏi</el-button>
    </section>

    <el-card shadow="never" class="filter-card">
      <el-input v-model="filters.keyword" placeholder="Tìm theo tiêu đề hoặc nội dung..." clearable @keyup.enter="loadQuestions" />
      <el-input v-model="filters.tag" placeholder="Lọc theo tag, ví dụ: sqlite" clearable @keyup.enter="loadQuestions" />
      <el-select v-model="filters.idChuyenMuc" placeholder="Chuyên mục" clearable>
        <el-option v-for="item in chuyenMuc" :key="item.id" :label="item.name" :value="item.id" />
      </el-select>
      <el-button type="primary" @click="loadQuestions">Tìm kiếm</el-button>
      <el-button @click="resetFilters">Xóa lọc</el-button>
    </el-card>

    <section v-loading="loading">
      <EmptyState v-if="!loading && questions.length === 0" description="Chưa có câu hỏi phù hợp.">
        <el-button type="primary" @click="$router.push('/questions/create')">Đặt câu hỏi đầu tiên</el-button>
      </EmptyState>

      <QuestionCard v-for="question in questions" :key="question.id" :question="question" />
    </section>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { getQuestions } from '../api/questionApi'
import { isAuthenticated } from '../utils/auth'
import { normalizeQuestion } from '../utils/format'
import QuestionCard from '../components/QuestionCard.vue'
import EmptyState from '../components/EmptyState.vue'

const route = useRoute()
const loading = ref(false)
const questions = ref([])
const filters = reactive({ keyword: '', tag: '', idChuyenMuc: '' })
const chuyenMuc = [
  { id: 1, name: 'Lập trình Web' },
  { id: 2, name: 'Cơ sở dữ liệu' },
  { id: 3, name: 'Chia sẻ kinh nghiệm' }
]
const isLoggedIn = computed(() => isAuthenticated())

onMounted(syncFromRoute)
watch(() => route.query, syncFromRoute)

function syncFromRoute() {
  filters.keyword = route.query.keyword || ''
  filters.tag = route.query.tag || ''
  filters.idChuyenMuc = route.query.idChuyenMuc ? Number(route.query.idChuyenMuc) : ''
  loadQuestions()
}

async function loadQuestions() {
  loading.value = true
  try {
    const data = await getQuestions({
      keyword: String(filters.keyword || '').trim(),
      tag: String(filters.tag || '').trim(),
      idChuyenMuc: filters.idChuyenMuc
    })
    questions.value = (data || []).map(normalizeQuestion)
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được danh sách câu hỏi.')
  } finally {
    loading.value = false
  }
}

function resetFilters() {
  filters.keyword = ''
  filters.tag = ''
  filters.idChuyenMuc = ''
  loadQuestions()
}
</script>

<style scoped>
.home-hero {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 18px;
  margin-bottom: 18px;
  padding: 24px;
  background: linear-gradient(135deg, #fff4ec, #ffffff);
  border: 1px solid var(--forum-border);
  border-radius: 16px;
}
.home-hero h1 { margin: 12px 0 8px; font-size: 30px; }
.home-hero p { margin: 0; color: var(--forum-muted); max-width: 720px; line-height: 1.65; }
.filter-card { margin-bottom: 18px; border-radius: 14px; }
.filter-card :deep(.el-card__body) {
  display: grid;
  grid-template-columns: minmax(220px, 1.5fr) minmax(160px, 1fr) minmax(160px, 0.8fr) auto auto;
  gap: 12px;
}
@media (max-width: 980px) {
  .home-hero { flex-direction: column; }
  .filter-card :deep(.el-card__body) { grid-template-columns: 1fr; }
}
</style>
