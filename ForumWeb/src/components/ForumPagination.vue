<template>
  <div
    v-if="props.total > 0"
    class="pagination-wrap"
  >
    <div
      class="pagination-inner"
      :class="{ compact: props.compact }"
    >
      <el-select
        v-if="showPageSizeSelect"
        :model-value="props.pageSize"
        class="page-size-select"
        @change="handlePageSizeChange"
      >
        <el-option
          v-for="size in props.pageSizes"
          :key="size"
          :label="`${size}/page`"
          :value="size"
        />
      </el-select>

      <el-button
        class="page-btn"
        :disabled="props.page <= 1"
        @click="handlePageChange(props.page - 1)"
      >
        ‹
      </el-button>

      <button
        v-for="item in visiblePages"
        :key="item.key"
        class="pager-item"
        :class="{
          active: item.value === props.page,
          dots: item.type === 'dots'
        }"
        :disabled="item.type === 'dots'"
        @click="item.type === 'page' && handlePageChange(item.value)"
      >
        {{ item.label }}
      </button>

      <el-button
        class="page-btn"
        :disabled="props.page >= totalPages"
        @click="handlePageChange(props.page + 1)"
      >
        ›
      </el-button>

      <div
        v-if="showJumperBox"
        class="jumper"
      >
        <span>Go to</span>

        <el-input-number
          :model-value="props.page"
          :min="1"
          :max="totalPages"
          :controls="false"
          class="jumper-input"
          @change="handleJump"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  total: {
    type: Number,
    default: 0
  },
  page: {
    type: Number,
    default: 1
  },
  pageSize: {
    type: Number,
    default: 5
  },
  pageSizes: {
    type: Array,
    default: () => [5, 10, 20]
  },
  compact: {
    type: Boolean,
    default: false
  },
  showPageSize: {
    type: Boolean,
    default: true
  },
  showJumper: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['page-change', 'page-size-change'])

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(props.total / props.pageSize))
})

const showPageSizeSelect = computed(() => {
  return props.showPageSize && !props.compact
})

const showJumperBox = computed(() => {
  return props.showJumper && !props.compact
})

const visiblePages = computed(() => {
  const current = props.page
  const total = totalPages.value

  if (props.compact) {
    return buildCompactPages(current, total)
  }

  return buildNormalPages(current, total)
})

function buildNormalPages(current, total) {
  if (total <= 6) {
    return createPageRange(1, total)
  }

  const pages = []

  if (current <= 3) {
    addPage(pages, 1)
    addPage(pages, 2)
    addPage(pages, 3)
    addDots(pages, 'dots-right')
    addPage(pages, total - 1)
    addPage(pages, total)

    return pages
  }

  if (current >= total - 2) {
    addPage(pages, 1)
    addPage(pages, 2)
    addDots(pages, 'dots-left')
    addPage(pages, total - 2)
    addPage(pages, total - 1)
    addPage(pages, total)

    return pages
  }

  addPage(pages, 1)
  addDots(pages, 'dots-left')
  addPage(pages, current - 1)
  addPage(pages, current)
  addPage(pages, current + 1)
  addDots(pages, 'dots-right')
  addPage(pages, total)

  return pages
}

function buildCompactPages(current, total) {
  if (total <= 5) {
    return createPageRange(1, total)
  }

  const pages = []

  if (current <= 2) {
    addPage(pages, 1)
    addPage(pages, 2)
    addPage(pages, 3)
    addDots(pages, 'dots-right')
    addPage(pages, total)

    return pages
  }

  if (current >= total - 1) {
    addPage(pages, 1)
    addDots(pages, 'dots-left')
    addPage(pages, total - 2)
    addPage(pages, total - 1)
    addPage(pages, total)

    return pages
  }

  addPage(pages, 1)
  addDots(pages, 'dots-left')
  addPage(pages, current)
  addDots(pages, 'dots-right')
  addPage(pages, total)

  return pages
}

function createPageRange(start, end) {
  const pages = []

  for (let page = start; page <= end; page++) {
    addPage(pages, page)
  }

  return pages
}

function addPage(pages, value) {
  if (value < 1 || value > totalPages.value) return

  if (pages.some(item => item.type === 'page' && item.value === value)) {
    return
  }

  pages.push({
    key: `page-${value}`,
    type: 'page',
    value,
    label: value
  })
}

function addDots(pages, key) {
  pages.push({
    key,
    type: 'dots',
    label: '...'
  })
}

function handlePageChange(page) {
  if (page < 1 || page > totalPages.value) return

  emit('page-change', page)
}

function handlePageSizeChange(pageSize) {
  emit('page-size-change', pageSize)
}

function handleJump(value) {
  const page = Number(value)

  if (!page || page < 1 || page > totalPages.value) return

  emit('page-change', page)
}
</script>

<style scoped>
.pagination-wrap {
  display: flex;
  justify-content: center;
  margin: 24px 0 6px;
}

.pagination-inner {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  flex-wrap: wrap;
}

.page-size-select {
  width: 130px;
}

.page-btn {
  width: 40px;
  height: 40px;
  padding: 0;
}

.pager-item {
  min-width: 40px;
  height: 40px;
  padding: 0 12px;
  border: none;
  border-radius: 4px;
  background: #f4f6f9;
  color: #374151;
  cursor: pointer;
  font-size: 15px;
  transition: 0.2s;
}

.pager-item:hover:not(:disabled) {
  background: #e8eefc;
  color: #1f3f95;
}

.pager-item.active {
  background: #1f3f95;
  color: #ffffff;
  font-weight: 700;
}

.pager-item.dots {
  min-width: 28px;
  padding: 0 6px;
  background: transparent;
  color: #8b95a5;
  cursor: default;
}

.jumper {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #606f85;
  font-size: 14px;
}

.jumper-input {
  width: 74px;
}

.jumper-input :deep(.el-input__inner) {
  text-align: center;
}

.pagination-inner.compact {
  gap: 6px;
}

.pagination-inner.compact .page-btn {
  width: 34px;
  height: 34px;
}

.pagination-inner.compact .pager-item {
  min-width: 34px;
  height: 34px;
  padding: 0 10px;
  font-size: 13px;
}

.pagination-inner.compact .pager-item.dots {
  min-width: 22px;
  padding: 0 4px;
}

@media (max-width: 760px) {
  .pagination-inner {
    gap: 6px;
  }

  .page-size-select {
    width: 110px;
  }

  .pager-item,
  .page-btn {
    min-width: 34px;
    width: 34px;
    height: 34px;
    font-size: 13px;
  }

  .jumper {
    width: 100%;
    justify-content: center;
    margin-top: 4px;
  }
}
</style>
