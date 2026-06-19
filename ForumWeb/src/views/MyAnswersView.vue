<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Câu trả lời của tôi</h1>
        <p class="page-subtitle">Theo dõi các câu trả lời bạn đã đóng góp cho cộng đồng.</p>
      </div>
    </div>

    <el-card shadow="never" class="answer-list-card" v-loading="loading">
      <EmptyState
        v-if="!loading && answers.length === 0"
        description="Bạn chưa có câu trả lời nào."
      />

      <AnswerCard
        v-for="answer in pagedAnswers"
        :key="answer.id"
        :answer="answer"
      />

      <ForumPagination
        v-if="!loading && answers.length > 0"
        :total="answers.length"
        :page="pagination.page"
        :page-size="pagination.pageSize"
        @page-change="handlePageChange"
        @page-size-change="handlePageSizeChange"
      />
    </el-card>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getMyAnswers } from '../api/userApi'
import { normalizeAnswer } from '../utils/format'
import AnswerCard from '../components/AnswerCard.vue'
import EmptyState from '../components/EmptyState.vue'
import ForumPagination from '../components/ForumPagination.vue'

const loading = ref(false)
const answers = ref([])

const pagination = reactive({
  page: 1,
  pageSize: 5
})

const pagedAnswers = computed(() => {
  const start = (pagination.page - 1) * pagination.pageSize
  const end = start + pagination.pageSize

  return answers.value.slice(start, end)
})

onMounted(load)

async function load() {
  loading.value = true

  try {
    answers.value = (await getMyAnswers() || [])
      .map(normalizeAnswer)
      .filter(answer => Number(answer.isDeleted || answer.IsDeleted || 0) === 0)

    if ((pagination.page - 1) * pagination.pageSize >= answers.value.length) {
      pagination.page = 1
    }
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được câu trả lời của tôi.')
  } finally {
    loading.value = false
  }
}

function handlePageChange(page) {
  pagination.page = page
}

function handlePageSizeChange(pageSize) {
  pagination.pageSize = pageSize
  pagination.page = 1
}
</script>

<style scoped>
.answer-list-card {
  border-radius: 14px;
}

.answer-row {
  display: flex;
  justify-content: space-between;
  gap: 18px;
  border-bottom: 1px solid var(--forum-border);
  padding: 16px 0;
}

.answer-row:last-child {
  border-bottom: none;
}

.answer-title {
  color: var(--forum-link);
  font-weight: 700;
  font-size: 17px;
}

.answer-title:hover {
  color: var(--forum-primary-dark);
}

.answer-row p {
  color: var(--forum-muted);
  line-height: 1.6;
}

.answer-meta {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  color: var(--forum-muted);
  font-size: 13px;
}

@media (max-width: 760px) {
  .answer-row {
    flex-direction: column;
  }
}
</style>
