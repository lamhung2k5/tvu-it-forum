<template>
  <div class="page-container narrow">
    <h1 class="page-title">Chỉnh sửa câu hỏi</h1>
    <p class="page-subtitle">Chỉ chủ câu hỏi mới có quyền chỉnh sửa nội dung này.</p>

    <el-card shadow="never" class="question-form-card" v-loading="loadingPage">
      <el-form :model="form" label-position="top">
        <el-form-item label="Chuyên mục">
          <el-select v-model="form.idChuyenMuc" class="full">
            <el-option v-for="item in chuyenMuc" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>

        <el-form-item label="Tiêu đề">
          <el-input v-model="form.tieuDe" maxlength="250" show-word-limit />
        </el-form-item>

        <el-form-item label="Nội dung">
          <el-input v-model="form.noiDung" type="textarea" :rows="8" />
        </el-form-item>

        <el-form-item label="Thẻ">
          <el-input v-model="form.the" placeholder="dotnet,dapper,sqlite" />
        </el-form-item>

        <div class="form-actions">
          <el-button @click="$router.back()">Hủy</el-button>
          <el-button type="primary" :loading="saving" @click="submit">Lưu thay đổi</el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { getQuestionById, updateQuestion } from '../api/questionApi'
import { getCurrentUser } from '../utils/auth'

const route = useRoute()
const router = useRouter()
const loadingPage = ref(false)
const saving = ref(false)
const questionId = route.params.id
const chuyenMuc = [
  { id: 1, name: 'Lập trình Web' },
  { id: 2, name: 'Cơ sở dữ liệu' },
  { id: 3, name: 'Chia sẻ kinh nghiệm' }
]

const form = reactive({
  idChuyenMuc: 1,
  tieuDe: '',
  noiDung: '',
  the: ''
})

function pick(obj, keys, fallback = '') {
  for (const key of keys) if (obj?.[key] !== undefined && obj?.[key] !== null) return obj[key]
  return fallback
}

onMounted(loadQuestion)

async function loadQuestion() {
  loadingPage.value = true
  try {
    const q = await getQuestionById(questionId)
    const user = getCurrentUser()
    const ownerId = Number(pick(q, ['iD_NguoiDung', 'ID_NguoiDung', 'id_NguoiDung', 'idNguoiDung'], 0))
    if (user && ownerId !== user.id_NguoiDung) {
      ElMessage.warning('Bạn không phải chủ câu hỏi này.')
    }
    form.idChuyenMuc = Number(pick(q, ['iD_ChuyenMuc', 'ID_ChuyenMuc', 'id_ChuyenMuc', 'idChuyenMuc'], 1))
    form.tieuDe = pick(q, ['tieuDe', 'TieuDe'], '')
    form.noiDung = pick(q, ['noiDung', 'NoiDung'], '')
    form.the = pick(q, ['tags', 'Tags'], '') || ''
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được câu hỏi.')
    router.push('/')
  } finally {
    loadingPage.value = false
  }
}

async function submit() {
  if (!form.tieuDe.trim() || !form.noiDung.trim()) {
    ElMessage.warning('Vui lòng nhập tiêu đề và nội dung.')
    return
  }

  saving.value = true
  try {
    await updateQuestion(questionId, {
      idChuyenMuc: form.idChuyenMuc,
      tieuDe: form.tieuDe.trim(),
      noiDung: form.noiDung.trim(),
      the: form.the.trim()
    })
    ElMessage.success('Cập nhật câu hỏi thành công!')
    router.push(`/questions/${questionId}`)
  } catch (error) {
    ElMessage.error(error.message || 'Cập nhật thất bại.')
  } finally {
    saving.value = false
  }
}
</script>

<style scoped>
.narrow {
  max-width: 840px;
}

.full {
  width: 100%;
}

.question-form-card {
  border-radius: 14px;
}
</style>
