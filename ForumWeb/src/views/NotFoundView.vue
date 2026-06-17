<template>
  <div class="page-container">
    <div class="card-panel center">
      <h1>404</h1>
      <p>Không tìm thấy trang bạn yêu cầu.</p>
      <el-button type="primary" @click="$router.push('/')">Quay về trang chủ</el-button>
    </div>
  </div>
</template>

<style scoped>
.center {
  text-align: center;
}

h1 {
  font-size: 64px;
  margin: 0;
  color: #2563eb;
}
</style>
