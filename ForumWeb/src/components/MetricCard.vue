<template>
  <el-card shadow="never" class="metric-card">
    <div class="metric-label">{{ label }}</div>
    <div class="metric-value">{{ value }}</div>
    <div v-if="hint" class="metric-hint">{{ hint }}</div>
  </el-card>
</template>

<script setup>
defineProps({
  label: { type: String, required: true },
  value: { type: [Number, String], default: 0 },
  hint: { type: String, default: '' }
})
</script>

<style scoped>
.metric-card {
  border-radius: 14px;
}
.metric-label { color: var(--forum-muted); font-size: 13px; }
.metric-value { font-size: 30px; font-weight: 780; color: #232629; margin-top: 5px; }
.metric-hint { color: var(--forum-primary); font-size: 12px; margin-top: 5px; }
</style>
