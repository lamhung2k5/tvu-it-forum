<template>
  <el-empty :description="description">
    <slot />
  </el-empty>
</template>

<script setup>
defineProps({ description: { type: String, default: 'Chưa có dữ liệu.' } })
</script>
