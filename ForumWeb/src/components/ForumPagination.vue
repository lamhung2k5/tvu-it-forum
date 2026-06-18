<template>
  <div
    v-if="total > 0"
    class="pagination-wrap"
  >
    <el-pagination
      :current-page="page"
      :page-size="pageSize"
      :total="total"
      :page-sizes="pageSizes"
      layout="sizes, prev, pager, next, jumper"
      background
      @current-change="handlePageChange"
      @size-change="handlePageSizeChange"
    />
  </div>
</template>

<script setup>
defineProps({
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
  }
})

const emit = defineEmits(['page-change', 'page-size-change'])

function handlePageChange(page) {
  emit('page-change', page)
}

function handlePageSizeChange(pageSize) {
  emit('page-size-change', pageSize)
}
</script>

<style scoped>
.pagination-wrap {
  display: flex;
  justify-content: center;
  margin: 24px 0 6px;
}
</style>