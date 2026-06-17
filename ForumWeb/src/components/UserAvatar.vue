<template>
  <el-avatar :size="size" :src="src || undefined" class="user-avatar">
    {{ initials(name) }}
  </el-avatar>
</template>

<script setup>
import { initials } from '../utils/format'

defineProps({
  name: { type: String, default: 'Người dùng' },
  src: { type: String, default: '' },
  size: { type: Number, default: 36 }
})
</script>

<style scoped>
.user-avatar {
  background: var(--forum-primary);
  color: white;
  font-weight: 700;
}
</style>
