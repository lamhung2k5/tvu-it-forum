<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Câu hỏi của tôi</h1>
        <p class="page-subtitle">Quản lý các câu hỏi bạn đã đăng.</p>
      </div>

      <el-button type="primary" @click="$router.push('/questions/create')">
        Đặt câu hỏi
      </el-button>
    </div>

    <section v-loading="loading">
      <EmptyState
        v-if="!loading && questions.length === 0"
        description="Bạn chưa đăng câu hỏi nào."
      />

      <div
        v-for="question in pagedQuestions"
        :key="question.id"
        class="my-item"
      >
        <QuestionCard :question="question" />

        <div class="my-actions">
          <el-tag v-if="question.isDeleted === 1" type="danger">
            Đã xóa mềm
          </el-tag>

          <el-tag v-else type="success">
            Đang hiển thị
          </el-tag>

          <el-button
            size="small"
            @click="$router.push(`/questions/${question.id}`)"
          >
            Xem
          </el-button>

          <el-button
            size="small"
            @click="$router.push(`/questions/${question.id}/edit`)"
          >
            Sửa
          </el-button>
        </div>
      </div>

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
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { getMyQuestions } from '../api/userApi'
import { normalizeQuestion } from '../utils/format'
import QuestionCard from '../components/QuestionCard.vue'
import EmptyState from '../components/EmptyState.vue'

const loading = ref(false)
const questions = ref([])

const pagination = reactive({
  page: 1,
  pageSize: 5
})

const pagedQuestions = computed(() => {
  const start = (pagination.page - 1) * pagination.pageSize
  const end = start + pagination.pageSize

  return questions.value.slice(start, end)
})

onMounted(load)

async function load() {
  loading.value = true

  try {
    questions.value = (await getMyQuestions() || []).map(normalizeQuestion)

    if ((pagination.page - 1) * pagination.pageSize >= questions.value.length) {
      pagination.page = 1
    }
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được câu hỏi của tôi.')
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
.my-item {
  position: relative;
}

.my-actions {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 8px;
  margin: -6px 0 14px;
}

.pagination-wrap {
  display: flex;
  justify-content: center;
  margin: 24px 0 6px;
}
</style>