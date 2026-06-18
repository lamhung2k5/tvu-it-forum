<template>
  <div
    class="answer-card"
    :class="{ 'detail-mode': detailMode }"
    @click="handleCardClick"
  >
    <div class="answer-layout">
      <div v-if="detailMode" class="answer-vote-col">
        <button
          class="vote-btn"
          :disabled="!canInteract"
          @click.stop="$emit('vote', 1)"
        >
          ▲
        </button>

        <div class="answer-score-main">
          {{ answer.diemBinhChon || 0 }}
        </div>

        <button
          class="vote-btn"
          :disabled="!canInteract"
          @click.stop="$emit('vote', -1)"
        >
          ▼
        </button>
      </div>

      <div class="answer-main">
        <div class="answer-top">
          <div class="answer-meta-main">
            <span v-if="!detailMode" class="answer-score">
              {{ answer.diemBinhChon || 0 }} điểm
            </span>

            <span class="answer-author-time">
              <strong>{{ answerAuthor }}</strong>
              · đã trả lời {{ formatRelativeTime(answer.ngayTao) }}
            </span>
          </div>

          <div class="answer-status-actions">
            <el-tag
              v-if="Number(answer.daChapNhan) === 1"
              type="success"
              size="small"
              class="accepted-tag"
            >
              Được chấp nhận
            </el-tag>

            <el-button
              v-if="canAccept"
              size="small"
              type="success"
              plain
              @click.stop="$emit('accept', answer)"
            >
              Chấp nhận
            </el-button>

            <el-button
              v-if="canUnaccept"
              size="small"
              type="warning"
              plain
              @click.stop="$emit('unaccept', answer)"
            >
              Bỏ chấp nhận
            </el-button>
          </div>
        </div>

        <h3
          v-if="answer.tieuDeCauHoi && !detailMode"
          class="answer-question-title"
        >
          {{ answer.tieuDeCauHoi }}
        </h3>

        <p class="answer-excerpt">
          {{ answer.noiDung }}
        </p>

        <div class="answer-footer">
          <div class="answer-actions">
            <el-button
              v-if="isOwner"
              link
              size="small"
              @click.stop="$emit('edit', answer)"
            >
              Sửa
            </el-button>

            <el-button
              v-if="isOwner"
              link
              type="danger"
              size="small"
              @click.stop="$emit('delete', answer)"
            >
              Xóa
            </el-button>

            <span
              v-if="!detailMode"
              class="answer-link"
            >
              Xem câu hỏi
            </span>
          </div>
        </div>

        <slot />
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { formatRelativeTime } from '../utils/format'

const props = defineProps({
  answer: {
    type: Object,
    required: true
  },
  detailMode: {
    type: Boolean,
    default: false
  },
  isOwner: {
    type: Boolean,
    default: false
  },
  canAccept: {
    type: Boolean,
    default: false
  },
  canUnaccept: {
    type: Boolean,
    default: false
  },
  canInteract: {
    type: Boolean,
    default: false
  }
})

defineEmits(['vote', 'accept', 'unaccept', 'edit', 'delete'])

const router = useRouter()

const answerAuthor = computed(() => {
  return props.answer.hoTen
    || props.answer.HoTen
    || props.answer.tenNguoiDung
    || props.answer.TenNguoiDung
    || 'Người dùng'
})

function handleCardClick() {
  if (props.detailMode) return

  if (props.answer.cauHoiId) {
    router.push(`/questions/${props.answer.cauHoiId}`)
  }
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

.answer-card.detail-mode {
  cursor: default;
}

.answer-card.detail-mode:hover {
  transform: none;
  border-color: var(--forum-border);
  box-shadow: none;
}

.answer-card:last-child {
  margin-bottom: 0;
}

.answer-layout {
  display: flex;
  gap: 14px;
}

.answer-vote-col {
  width: 34px;
  display: flex;
  flex-direction: column;
  align-items: center;
  flex-shrink: 0;
}

.vote-btn {
  width: 28px;
  height: 26px;
  border: none;
  background: transparent;
  color: var(--forum-primary);
  font-weight: 700;
  cursor: pointer;
}

.vote-btn:disabled {
  cursor: not-allowed;
  color: #cbd5e1;
}

.answer-score-main {
  font-weight: 700;
  color: var(--forum-text);
  margin: 2px 0;
}

.answer-main {
  flex: 1;
  min-width: 0;
}

.answer-top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 10px;
  margin-bottom: 10px;
}

.answer-meta-main {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
  min-width: 0;
}

.answer-status-actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
  flex-shrink: 0;
}

.answer-score {
  color: var(--forum-muted);
  font-size: 13px;
  font-weight: 500;
}

.answer-author-time {
  color: var(--forum-muted);
  font-size: 13px;
}

.answer-author-time strong {
  color: var(--forum-text);
  font-weight: 600;
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

.answer-card:not(.detail-mode):hover .answer-question-title {
  color: var(--forum-primary-dark);
  text-decoration: underline;
}

.answer-excerpt {
  margin: 0 0 12px;
  color: #4b5563;
  line-height: 1.6;
  white-space: pre-wrap;
}

.answer-card:not(.detail-mode) .answer-excerpt {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.answer-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  color: var(--forum-muted);
  font-size: 13px;
}

.answer-actions {
  display: flex;
  align-items: center;
  gap: 8px;
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

  .answer-layout {
    gap: 10px;
  }

  .answer-top,
  .answer-footer {
    align-items: flex-start;
    flex-direction: column;
    gap: 6px;
  }

  .answer-status-actions {
    width: 100%;
  }

  .answer-question-title {
    font-size: 16px;
  }

  .answer-excerpt {
    font-size: 14px;
  }
}
</style>