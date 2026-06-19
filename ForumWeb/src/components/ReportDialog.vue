<template>
  <el-dialog
    v-model="visible"
    title="Tố cáo nội dung"
    width="480px"
  >
    <el-form label-position="top">
      <el-form-item label="Lý do tố cáo">
        <el-select
          v-model="form.lyDo"
          placeholder="Chọn lý do"
          style="width: 100%"
        >
          <el-option label="Spam / quảng cáo" value="Spam / quảng cáo" />
          <el-option label="Ngôn từ không phù hợp" value="Ngôn từ không phù hợp" />
          <el-option label="Nội dung sai lệch" value="Nội dung sai lệch" />
          <el-option label="Nội dung trùng lặp" value="Nội dung trùng lặp" />
          <el-option label="Khác" value="Khác" />
        </el-select>
      </el-form-item>

      <el-form-item label="Mô tả thêm">
        <el-input
          v-model="form.moTa"
          type="textarea"
          :rows="4"
          placeholder="Mô tả ngắn lý do tố cáo nếu cần..."
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="visible = false">Hủy</el-button>
      <el-button type="danger" :loading="submitting" @click="submit">
        Gửi tố cáo
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { createReport } from '../api/toCaoApi'

const visible = ref(false)
const submitting = ref(false)
const target = ref(null)
const form = reactive({
  lyDo: '',
  moTa: ''
})

function open(payload) {
  target.value = payload
  form.lyDo = ''
  form.moTa = ''
  visible.value = true
}

async function submit() {
  if (!target.value) return

  if (!form.lyDo) {
    ElMessage.warning('Vui lòng chọn lý do tố cáo.')
    return
  }

  submitting.value = true

  try {
    await createReport({
      loaiDoiTuong: target.value.loaiDoiTuong,
      iD_DoiTuong: target.value.idDoiTuong,
      id_DoiTuong: target.value.idDoiTuong,
      ID_DoiTuong: target.value.idDoiTuong,
      lyDo: form.lyDo,
      moTa: form.moTa
    })

    ElMessage.success('Đã gửi tố cáo đến quản trị viên.')
    visible.value = false
  } catch (error) {
    ElMessage.error(error.message || 'Gửi tố cáo thất bại.')
  } finally {
    submitting.value = false
  }
}

defineExpose({ open })
</script>
