<template>
  <div class="vote-box">
    <el-button text circle :disabled="disabled" @click="$emit('vote', 1)">▲</el-button>
    <strong>{{ score }}</strong>
    <el-button text circle :disabled="disabled" @click="$emit('vote', -1)">▼</el-button>
  </div>
</template>

<script setup>
defineProps({
  score: { type: Number, default: 0 },
  disabled: { type: Boolean, default: false }
})
defineEmits(['vote'])
</script>

<style scoped>
.vote-box {
  width: 58px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 3px;
  color: var(--forum-muted);
}

.vote-box strong {
  color: var(--forum-text);
  font-size: 20px;
}

.vote-box :deep(.el-button) {
  font-size: 18px;
  color: var(--forum-primary);
}
</style>
