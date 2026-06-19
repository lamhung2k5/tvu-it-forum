<template>
  <div class="page-container narrow">
    <div class="page-header">
      <div>
        <h1 class="page-title">Đặt câu hỏi mới</h1>
        <p class="page-subtitle">Mô tả rõ vấn đề, công nghệ liên quan và điều bạn đã thử.</p>
      </div>
    </div>

    <el-card shadow="never" class="question-form-card">
      <el-form :model="form" label-position="top">
        <el-form-item label="Chuyên mục">
          <el-select v-model="form.idChuyenMuc" placeholder="Chọn chuyên mục" class="full" size="large">
            <el-option v-for="item in chuyenMuc" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>

        <el-form-item label="Tiêu đề">
          <el-input v-model="form.tieuDe" maxlength="250" show-word-limit size="large" placeholder="Ví dụ: Lỗi khi dùng Dapper với SQLite trong .NET" />
        </el-form-item>

        <el-form-item label="Nội dung">
          <el-input v-model="form.noiDung" type="textarea" :rows="9" placeholder="Trình bày lỗi, đoạn code liên quan hoặc điều bạn đã thử..." />
        </el-form-item>

        <el-form-item label="Thẻ">
          <el-input v-model="form.the" placeholder="Ví dụ: dotnet,dapper,sqlite" />
          <small class="muted">Nhập các thẻ cách nhau bằng dấu phẩy.</small>
        </el-form-item>

        <div class="upload-placeholder">
          <el-button plain disabled>+ Chèn hình ảnh</el-button>
          <span>Chức năng chèn hình được chừa sẵn để phát triển sau.</span>
        </div>

        <div class="form-actions top-gap">
          <el-button @click="$router.back()">Hủy</el-button>
          <el-button type="primary" :loading="loading" @click="submit">Đăng câu hỏi</el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { createQuestion } from '../api/questionApi'

const router = useRouter()
const loading = ref(false)
const chuyenMuc = [
  { id: 1, name: 'Lập trình Web' },
  { id: 2, name: 'Cơ sở dữ liệu' },
  { id: 3, name: 'Chia sẻ kinh nghiệm' }
]
const form = reactive({ idChuyenMuc: 1, tieuDe: '', noiDung: '', the: '' })

async function submit() {
  if (!form.tieuDe.trim() || !form.noiDung.trim()) return ElMessage.warning('Vui lòng nhập tiêu đề và nội dung câu hỏi.')
  loading.value = true
  try {
    const result = await createQuestion({ idChuyenMuc: form.idChuyenMuc, tieuDe: form.tieuDe.trim(), noiDung: form.noiDung.trim(), the: form.the.trim() })
    const newId = result.cauHoiId || result.CauHoiId || result.id
    ElMessage.success('Đăng câu hỏi thành công!')
    router.push(newId ? `/questions/${newId}` : '/')
  } catch (error) {
    ElMessage.error(error.message || 'Đăng câu hỏi thất bại.')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.narrow { max-width: 860px; }
.full { width: 100%; }
.question-form-card { border-radius: 14px; }
.upload-placeholder {
  border: 1px dashed var(--forum-border);
  border-radius: 12px;
  padding: 14px;
  display: flex;
  gap: 12px;
  align-items: center;
  color: var(--forum-muted);
  margin-bottom: 18px;
}
.top-gap { margin-top: 8px; }
@media (max-width: 640px) { .upload-placeholder { flex-direction: column; align-items: flex-start; } }
</style>
