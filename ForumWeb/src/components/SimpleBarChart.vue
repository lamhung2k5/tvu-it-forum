<template>
  <el-card shadow="never" class="chart-card">
    <template #header>{{ title }}</template>
    <div v-if="normalized.length" class="bars">
      <div v-for="item in normalized" :key="item.ten" class="bar-row">
        <div class="bar-label">
          <span>{{ item.ten }}</span>
          <strong>{{ item.giaTri }}</strong>
        </div>
        <el-progress :percentage="item.percent" :stroke-width="12" :show-text="false" />
      </div>
    </div>
    <el-empty v-else description="Chưa có dữ liệu" />
  </el-card>
</template>

<script setup>
import { computed } from 'vue'
import { pick } from '../utils/format'

const props = defineProps({
  title: { type: String, required: true },
  items: { type: Array, default: () => [] }
})

const normalized = computed(() => {
  const rows = props.items.map(x => ({
    ten: pick(x, ['ten', 'Ten'], 'Không xác định'),
    giaTri: Number(pick(x, ['giaTri', 'GiaTri'], 0))
  }))
  const max = Math.max(...rows.map(x => x.giaTri), 1)
  return rows.map(x => ({ ...x, percent: Math.round((x.giaTri / max) * 100) }))
})
</script>

<style scoped>
.chart-card { border-radius: 14px; }
.bars { display: grid; gap: 13px; }
.bar-label { display: flex; justify-content: space-between; gap: 10px; margin-bottom: 6px; font-size: 13px; }
.bar-label strong { color: var(--forum-primary); }
</style>
