<template>
  <el-card shadow="hover" class="question-card">
    <div class="question-layout">
      <div class="question-stats">
        <div><strong>{{ question.diemBinhChon }}</strong><span>vote</span></div>
        <div><strong>{{ question.soCauTraLoi }}</strong><span>trả lời</span></div>
        <div><strong>{{ question.soBinhLuan }}</strong><span>bình luận</span></div>
      </div>

      <div class="question-main">
        <router-link :to="`/questions/${question.id}`" class="question-title">{{ question.tieuDe }}</router-link>
        <p class="question-excerpt">{{ shortText(question.noiDung, 190) }}</p>
        <div class="question-meta">
          <div class="tag-list">
            <el-tag v-for="tag in question.tags" :key="tag" type="warning" effect="light" size="small">{{ tag }}</el-tag>
          </div>
          <div class="author">
            <UserAvatar :name="question.hoTen" :size="28" />
            <span>{{ question.hoTen }}</span>
            <span>·</span>
            <span>{{ formatDate(question.ngayTao) }}</span>
          </div>
        </div>
      </div>
    </div>
  </el-card>
</template>

<script setup>
import { shortText, formatDate } from '../utils/format'
import UserAvatar from './UserAvatar.vue'

defineProps({ question: { type: Object, required: true } })
</script>

<style scoped>
.question-card {
  border-radius: 14px;
  margin-bottom: 14px;
}

.question-layout {
  display: flex;
  gap: 18px;
}

.question-stats {
  width: 86px;
  display: grid;
  gap: 8px;
  flex-shrink: 0;
  color: var(--forum-muted);
  text-align: center;
}

.question-stats div {
  border: 1px solid var(--forum-border);
  border-radius: 10px;
  padding: 8px 6px;
  background: #fbfbfc;
}

.question-stats strong {
  display: block;
  color: var(--forum-text);
  font-size: 18px;
}

.question-stats span { font-size: 12px; }

.question-main { min-width: 0; flex: 1; }

.question-title {
  font-size: 20px;
  font-weight: 700;
  color: #0c65a5;
  line-height: 1.35;
}

.question-title:hover { color: var(--forum-primary); }

.question-excerpt {
  margin: 9px 0 14px;
  color: var(--forum-muted);
  line-height: 1.55;
}

.question-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
}

.author {
  display: flex;
  align-items: center;
  gap: 7px;
  color: var(--forum-muted);
  font-size: 13px;
}

@media (max-width: 640px) {
  .question-layout { flex-direction: column; gap: 12px; }
  .question-stats { width: 100%; grid-template-columns: repeat(3, 1fr); }
}
</style>
