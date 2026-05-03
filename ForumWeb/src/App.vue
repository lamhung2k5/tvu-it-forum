<template>
  <el-container>
    <!-- Thanh điều hướng phía trên -->
    <el-header style="background-color: #409EFF; color: white; display: flex; align-items: center;">
      <h2 style="margin: 0;">Hệ thống Hỏi Đáp IT</h2>
    </el-header>
    
    <!-- Phần thân nội dung -->
    <el-main>
      <!-- router-view là nơi các trang (Đăng nhập, Trang chủ...) sẽ được nhúng vào -->
      <router-view></router-view>
    </el-main>
  </el-container>
</template>

<script setup>
import { RouterView } from 'vue-router'
</script>

<style>
/* Xóa viền trắng mặc định của trình duyệt */
body {
  margin: 0;
  padding: 0;
  font-family: 'Helvetica Neue', Helvetica, 'PingFang SC', 'Hiragino Sans GB', 'Microsoft YaHei', Arial, sans-serif;
  background-color: #f3f4f6;
}
</style>