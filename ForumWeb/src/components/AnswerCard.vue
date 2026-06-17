<template>
  <el-card shadow="never" class="answer-card" :class="{ accepted: answer.daChapNhan === 1 }">
    <div class="answer-layout">
      <VoteBox :score="answer.diemBinhChon" :disabled="!canInteract" @vote="$emit('vote', $event)" />
      <div class="answer-main">
        <div class="answer-top">
          <div class="author">
            <UserAvatar :name="answer.hoTen" :src="answer.anhDaiDien" :size="32" />
            <div>
              <strong>{{ answer.hoTen }}</strong>
              <p>{{ formatDate(answer.ngayTao) }}</p>
            </div>
          </div>
          <el-tag v-if="answer.daChapNhan === 1" type="success">✓ Đã chấp nhận</el-tag>
        </div>

        <p class="answer-content">{{ answer.noiDung }}</p>

        <div class="answer-actions">
          <el-button v-if="canAccept" type="success" size="small" plain @click="$emit('accept')">Chấp nhận</el-button>
          <el-button v-if="isOwner" size="small" plain @click="$emit('edit')">Sửa</el-button>
          <el-button v-if="isOwner" size="small" type="danger" plain @click="$emit('delete')">Xóa</el-button>
          <el-button size="small" text @click="$emit('toggle-comments')">{{ answer.soBinhLuan }} bình luận</el-button>
        </div>

        <slot />
      </div>
    </div>
  </el-card>
</template>

<script setup>
import { formatDate } from '../utils/format'
import UserAvatar from './UserAvatar.vue'
import VoteBox from './VoteBox.vue'

defineProps({
  answer: { type: Object, required: true },
  isOwner: { type: Boolean, default: false },
  canAccept: { type: Boolean, default: false },
  canInteract: { type: Boolean, default: false }
})

defineEmits(['vote', 'accept', 'edit', 'delete', 'toggle-comments'])
</script>

<style scoped>
.answer-card {
  border-radius: 14px;
  margin-bottom: 14px;
}
.answer-card.accepted { border-color: #67c23a; background: #f6ffef; }
.answer-layout { display: flex; gap: 14px; }
.answer-main { flex: 1; min-width: 0; }
.answer-top { display: flex; justify-content: space-between; align-items: flex-start; gap: 12px; }
.author { display: flex; align-items: center; gap: 9px; }
.author p { margin: 3px 0 0; color: var(--forum-muted); font-size: 12px; }
.answer-content { line-height: 1.7; white-space: pre-wrap; }
.answer-actions { display: flex; gap: 8px; flex-wrap: wrap; align-items: center; }
@media (max-width: 640px) { .answer-layout { flex-direction: column; } }
</style>
