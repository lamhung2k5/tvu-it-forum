<template>
  <div class="page-container">
    <section class="home-hero">
      <div class="hero-overlay"></div>

      <div class="hero-content">
        <el-tag class="hero-badge">
          Diễn đàn hỏi đáp IT
        </el-tag>

        <h1>Hỏi đáp lập trình, cơ sở dữ liệu và công nghệ</h1>

        <p>
          Chia sẻ vấn đề bạn đang gặp, nhận câu trả lời từ cộng đồng
          và lưu lại tri thức cho sinh viên CNTT.
        </p>
      </div>

      <div class="hero-actions">
        <el-button
          v-if="isLoggedIn"
          type="primary"
          class="btn-accent"
          size="large"
          @click="$router.push('/questions/create')"
        >
          Đặt câu hỏi
        </el-button>

        <el-button
          v-else
          size="large"
          @click="$router.push('/login')"
        >
          Đăng nhập để hỏi
        </el-button>
      </div>
    </section>

    <el-card shadow="never" class="filter-card">
      <el-input
        v-model="filters.keyword"
        placeholder="Tìm theo tiêu đề hoặc nội dung..."
        clearable
        @keyup.enter="searchQuestions"
      />

      <el-input
        v-model="filters.tag"
        placeholder="Lọc theo tag, ví dụ: sqlite"
        clearable
        @keyup.enter="searchQuestions"
      />

      <el-select
        v-model="filters.idChuyenMuc"
        placeholder="Chuyên mục"
        clearable
      >
        <el-option
          v-for="item in chuyenMuc"
          :key="item.id"
          :label="item.name"
          :value="item.id"
        />
      </el-select>

      <el-button type="primary" @click="searchQuestions">
        Tìm kiếm
      </el-button>

      <el-button @click="resetFilters">
        Xóa lọc
      </el-button>
    </el-card>

    <section v-loading="loading">
      <EmptyState
        v-if="!loading && questions.length === 0"
        description="Chưa có câu hỏi phù hợp."
      >
        <el-button
          type="primary"
          class="btn-accent"
          @click="$router.push('/questions/create')"
        >
          Đặt câu hỏi đầu tiên
        </el-button>
      </EmptyState>

      <QuestionCard
        v-for="question in pagedQuestions"
        :key="question.id"
        :question="question"
      />

      <div
        v-if="!loading && questions.length > 0"
        class="pagination-wrap"
      >
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :total="questions.length"
          :page-sizes="[5, 10, 20]"
          layout="sizes, prev, pager, next, jumper"
          background
          @current-change="handlePageChange"
          @size-change="handlePageSizeChange"
        />
      </div>
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

const filters = reactive({
  keyword: '',
  tag: '',
  idChuyenMuc: ''
})

const pagination = reactive({
  page: 1,
  pageSize: 5
})

const chuyenMuc = [
  { id: 1, name: 'Lập trình Web' },
  { id: 2, name: 'Cơ sở dữ liệu' },
  { id: 3, name: 'Chia sẻ kinh nghiệm' }
]

const isLoggedIn = computed(() => isAuthenticated())

const pagedQuestions = computed(() => {
  const start = (pagination.page - 1) * pagination.pageSize
  const end = start + pagination.pageSize

  return questions.value.slice(start, end)
})

onMounted(syncFromRoute)
watch(() => route.query, syncFromRoute)

function syncFromRoute() {
  filters.keyword = route.query.keyword || ''
  filters.tag = route.query.tag || ''
  filters.idChuyenMuc = route.query.idChuyenMuc
    ? Number(route.query.idChuyenMuc)
    : ''

  pagination.page = 1
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

    if ((pagination.page - 1) * pagination.pageSize >= questions.value.length) {
      pagination.page = 1
    }
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được danh sách câu hỏi.')
  } finally {
    loading.value = false
  }
}

function searchQuestions() {
  pagination.page = 1
  loadQuestions()
}

function handlePageChange(page) {
  pagination.page = page
}

function handlePageSizeChange(pageSize) {
  pagination.pageSize = pageSize
  pagination.page = 1
}

function resetFilters() {
  filters.keyword = ''
  filters.tag = ''
  filters.idChuyenMuc = ''
  pagination.page = 1
  loadQuestions()
}
</script>

<style scoped>
.home-hero {
  position: relative;
  overflow: hidden;

  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 24px;

  min-height: 190px;
  margin-bottom: 18px;
  padding: 30px 32px;

  border: 1px solid var(--forum-border);
  border-radius: 18px;

  background-image: url('/images/tvu-hero.png');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.hero-overlay {
  position: absolute;
  inset: 0;

  background: linear-gradient(
    90deg,
    rgba(15, 23, 42, 0.22) 0%,
    rgba(30, 58, 138, 0.22) 55%,
    rgba(30, 58, 138, 0.12) 100%
  );
}

.hero-content,
.hero-actions {
  position: relative;
  z-index: 1;
}

.hero-content {
  max-width: 780px;
}

.home-hero h1 {
  margin: 14px 0 10px;
  font-size: 32px;
  line-height: 1.25;
  color: #ffffff;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.28);
}

.home-hero p {
  margin: 0;
  max-width: 760px;
  color: rgba(255, 255, 255, 0.9);
  font-size: 16px;
  line-height: 1.65;
  text-shadow: 0 1px 6px rgba(0, 0, 0, 0.22);
}

.hero-badge {
  background: rgba(255, 255, 255, 0.92) !important;
  color: var(--forum-primary) !important;
  border-color: transparent !important;
  font-weight: 600;
}

.hero-actions {
  flex-shrink: 0;
  padding-top: 2px;
}

.filter-card {
  margin-bottom: 18px;
  border-radius: 14px;
}

.filter-card :deep(.el-card__body) {
  display: grid;
  grid-template-columns: minmax(220px, 1.5fr) minmax(160px, 1fr) minmax(160px, 0.8fr) auto auto;
  gap: 12px;
}

.pagination-wrap {
  display: flex;
  justify-content: center;
  margin: 24px 0 6px;
}

@media (max-width: 980px) {
  .home-hero {
    flex-direction: column;
    min-height: 220px;
    padding: 26px 24px;
  }

  .hero-actions {
    padding-top: 4px;
  }

  .filter-card :deep(.el-card__body) {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .home-hero {
    min-height: 230px;
    padding: 22px 18px;
    border-radius: 16px;
    background-position: center;
  }

  .home-hero h1 {
    font-size: 24px;
  }

  .home-hero p {
    font-size: 14px;
  }
}
</style>