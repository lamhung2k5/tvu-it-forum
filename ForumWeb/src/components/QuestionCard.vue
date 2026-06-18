<template>
  <el-card shadow="hover" class="question-card">
    <div class="question-layout">
      <div class="question-stats">
        <div class="stat-item">
          <span class="stat-number">{{ question.diemBinhChon }}</span>
          <span class="stat-label">vote</span>
        </div>

        <div
          class="stat-item answer-stat"
          :class="{ 'has-answer': question.soCauTraLoi > 0 }"
        >
          <span class="stat-number">{{ question.soCauTraLoi }}</span>
          <span class="stat-label">trả lời</span>
        </div>

        <div class="stat-item">
          <span class="stat-number">{{ question.luotXem || 0 }}</span>
          <span class="stat-label">lượt xem</span>
        </div>
      </div>

      <div class="question-main">
        <router-link
          :to="`/questions/${question.id}`"
          class="question-title"
        >
          {{ question.tieuDe }}
        </router-link>

        <p class="question-excerpt">
          {{ shortText(question.noiDung, 210) }}
        </p>

        <div class="question-bottom">
          <div class="tag-list">
            <el-tag
              v-for="tag in question.tags || []"
              :key="tag"
              class="forum-tag"
              size="small"
            >
              {{ tag }}
            </el-tag>
          </div>

          <div class="author">
            <UserAvatar :name="question.hoTen" :size="30" />

            <div class="author-info">
              <router-link
                v-if="question.userId"
                :to="`/users/${question.userId}`"
                class="author-name"
                @click.stop
              >
                {{ question.hoTen }}
              </router-link>

              <span
                v-else
                class="author-name"
              >
                {{ question.hoTen }}
              </span>

              <span class="author-time">
                đã hỏi {{ formatRelativeTime(question.ngayTao) }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </el-card>
</template>

<script setup>
import { shortText, formatRelativeTime } from '../utils/format'
import UserAvatar from './UserAvatar.vue'

defineProps({
  question: { type: Object, required: true }
})
</script>

<style scoped>
.question-card {
  border-radius: 16px;
  margin-bottom: 16px;
  border: 1px solid var(--forum-border);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.question-card:hover {
  transform: translateY(-1px);
}

.question-layout {
  display: grid;
  grid-template-columns: 105px 1fr;
  gap: 18px;
  align-items: flex-start;
}

.question-stats {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 9px;
  padding-top: 3px;
  color: var(--forum-muted);
  text-align: right;
  flex-shrink: 0;
}

.stat-item {
  display: flex;
  align-items: baseline;
  justify-content: flex-end;
  gap: 4px;

  min-height: 22px;
  padding: 0;
  white-space: nowrap;

  font-size: 14px;
  line-height: 1.4;
  color: var(--forum-muted);
}

.stat-number,
.stat-label {
  font-size: 14px;
  font-weight: 400;
  color: inherit;
}

.answer-stat.has-answer {
  color: #15803d;
  font-weight: 500;
}

.question-main {
  min-width: 0;
  display: flex;
  flex-direction: column;
  min-height: 130px;
}

.question-title {
  width: fit-content;
  font-size: 20px;
  font-weight: 700;
  color: var(--forum-primary);
  line-height: 1.35;
  text-decoration: none;
}

.question-title:hover {
  color: var(--forum-primary-dark);
  text-decoration: underline;
}

.question-excerpt {
  margin: 9px 0 14px;
  color: #4b5563;
  line-height: 1.55;

  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.question-bottom {
  margin-top: auto;
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 14px;
}

.tag-list {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
  min-width: 0;
}

.author {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
  color: var(--forum-muted);
}

.author-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  line-height: 1.2;
}

.author-name {
  width: fit-content;
  color: var(--forum-text);
  font-size: 13px;
  font-weight: 600;
  text-decoration: none;
}

.author-name:hover {
  color: var(--forum-primary);
  text-decoration: underline;
}

.author-time {
  color: var(--forum-muted);
  font-size: 12px;
}

/* Tablet / màn hình nhỏ */
@media (max-width: 760px) {
  .question-layout {
    grid-template-columns: 1fr;
    gap: 14px;
  }

  .question-stats {
    flex-direction: row;
    align-items: center;
    justify-content: flex-start;
    gap: 14px;
    text-align: left;
  }

  .stat-item {
    justify-content: flex-start;
  }

  .question-main {
    min-height: auto;
  }

  .question-bottom {
    align-items: flex-start;
    flex-direction: column;
  }

  .author {
    align-self: flex-end;
  }
}

/* Điện thoại nhỏ */
@media (max-width: 480px) {
  .question-title {
    font-size: 18px;
  }

  .question-excerpt {
    font-size: 14px;
  }

  .question-stats {
    gap: 10px;
    flex-wrap: wrap;
  }

  .stat-number,
  .stat-label {
    font-size: 13px;
  }
}
</style>