<template>
  <div class="answer-card" @click="goToQuestion">
    <div class="answer-top">
      <span class="answer-score">{{ answer.diemBinhChon || 0 }} điểm</span>

      <el-tag
        v-if="Number(answer.daChapNhan) === 1"
        type="success"
        size="small"
        class="accepted-tag"
      >
        Được chấp nhận
      </el-tag>
    </div>

    <h3 class="answer-question-title">
      {{ answer.tieuDeCauHoi }}
    </h3>

    <p class="answer-excerpt">
      {{ answer.noiDung }}
    </p>

    <div class="answer-footer">
      <span>đã trả lời {{ formatRelativeTime(answer.ngayTao) }}</span>
      <span class="answer-link">Xem câu hỏi</span>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { formatRelativeTime } from '../utils/format'

const props = defineProps({
  answer: { type: Object, required: true }
})

const router = useRouter()

function goToQuestion() {
  router.push(`/questions/${props.answer.cauHoiId}`)
}
</script>

<style scoped>
.answer-card {
  padding: 16px 18px;
  margin-bottom: 14px;
  border: 1px solid var(--forum-border);
  border-radius: 14px;
  background: var(--forum-surface);
  cursor: pointer;
  transition: box-shadow 0.2s ease, transform 0.2s ease, border-color 0.2s ease;
}

.answer-card:hover {
  transform: translateY(-1px);
  border-color: var(--forum-primary-soft);
  box-shadow: 0 8px 20px rgba(15, 23, 42, 0.08);
}

.answer-card:last-child {
  margin-bottom: 0;
}

.answer-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  margin-bottom: 10px;
}

.answer-score {
  color: var(--forum-muted);
  font-size: 13px;
  font-weight: 500;
}

.accepted-tag {
  font-weight: 600;
}

.answer-question-title {
  margin: 0 0 8px;
  color: var(--forum-primary);
  font-size: 18px;
  font-weight: 700;
  line-height: 1.3;
}

.answer-card:hover .answer-question-title {
  color: var(--forum-primary-dark);
  text-decoration: underline;
}

.answer-excerpt {
  margin: 0 0 12px;
  color: #4b5563;
  line-height: 1.55;

  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.answer-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  color: var(--forum-muted);
  font-size: 13px;
}

.answer-link {
  color: var(--forum-primary);
  font-weight: 600;
  white-space: nowrap;
}

@media (max-width: 640px) {
  .answer-card {
    padding: 14px;
  }

  .answer-top,
  .answer-footer {
    align-items: flex-start;
    flex-direction: column;
    gap: 6px;
  }

  .answer-question-title {
    font-size: 16px;
  }

  .answer-excerpt {
    font-size: 14px;
    -webkit-line-clamp: 2;
  }
}
</style>