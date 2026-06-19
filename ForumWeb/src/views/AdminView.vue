```vue
<template>
  <div class="page-container admin-page">
    <div class="page-header">
      <div>
        <h1 class="page-title">Bảng quản trị</h1>
        <p class="page-subtitle">
          Theo dõi số liệu và quản lý nội dung trong hệ thống.
        </p>
      </div>

      <el-button type="primary" @click="loadAll">
        Làm mới
      </el-button>
    </div>

    <el-tabs v-model="activeTab" @tab-change="loadCurrentTab">
      <el-tab-pane label="Tổng quan" name="overview">
        <div class="overview-toolbar">
          <div>
            <h2 class="overview-title">Tổng quan hệ thống</h2>
            <p class="overview-subtitle">
              Theo dõi hoạt động chính của diễn đàn theo khoảng thời gian.
            </p>
          </div>

          <div class="overview-filter-group">
            <el-select
              v-model="overviewFilters.timeRange"
              placeholder="Khoảng thời gian"
            >
              <el-option
                v-for="option in overviewTimeOptions"
                :key="option.value"
                :label="option.label"
                :value="option.value"
              />
            </el-select>

            <el-button type="primary" @click="loadOverview">
              Tải tổng quan
            </el-button>
          </div>
        </div>

        <div
          class="overview-metric-grid"
          v-loading="loadingDashboard || loadingOverview"
        >
          <el-card
            v-for="card in overviewCards"
            :key="card.key"
            shadow="never"
            :class="['overview-metric-card', card.className]"
          >
            <div class="metric-content">
              <p>{{ card.label }}</p>
              <strong>{{ card.value }}</strong>
              <span>{{ card.hint }}</span>
            </div>

            <div class="metric-icon">
              {{ card.icon }}
            </div>
          </el-card>
        </div>

        <el-card
          shadow="never"
          class="activity-line-card"
          v-loading="loadingOverview"
        >
          <template #header>
            <div class="chart-header">
              <div>
                <strong>Hoạt động theo thời gian</strong>
                <p>
                  Thống kê người dùng mới, câu hỏi mới, câu trả lời mới và bình luận mới.
                </p>
              </div>
            </div>
          </template>

          <div v-if="activityChart.hasData" class="line-chart-wrap">
            <svg
              class="line-chart"
              viewBox="0 0 760 280"
              role="img"
              aria-label="Biểu đồ hoạt động theo thời gian"
            >
              <g class="y-grid">
                <g
                  v-for="tick in activityChart.yTicks"
                  :key="tick.value"
                >
                  <line
                    :x1="activityChart.padding.left"
                    :x2="activityChart.width - activityChart.padding.right"
                    :y1="tick.y"
                    :y2="tick.y"
                  />

                  <text
                    :x="activityChart.padding.left - 10"
                    :y="tick.y + 4"
                    text-anchor="end"
                  >
                    {{ tick.value }}
                  </text>
                </g>
              </g>

              <g class="x-labels">
                <text
                  v-for="label in activityChart.xLabels"
                  :key="label.index"
                  :x="label.x"
                  :y="activityChart.height - 14"
                  text-anchor="middle"
                >
                  {{ label.text }}
                </text>
              </g>

              <g
                v-for="line in activityChart.series"
                :key="line.key"
              >
                <polyline
                  :class="['chart-line', line.className]"
                  :points="line.pointsText"
                  fill="none"
                />

                <circle
                  v-for="point in line.points"
                  :key="line.key + '-' + point.label"
                  :class="['chart-point', line.className]"
                  :cx="point.x"
                  :cy="point.y"
                  r="3.5"
                >
                  <title>{{ point.tooltip }}</title>
                </circle>
              </g>
            </svg>

            <div class="line-legend">
              <span
                v-for="line in activityChart.series"
                :key="line.key"
                class="legend-item"
              >
                <span :class="['legend-dot', line.className]"></span>
                {{ line.label }}
              </span>
            </div>
          </div>

          <el-empty
            v-else
            description="Chưa có dữ liệu hoạt động trong khoảng thời gian này"
          />
        </el-card>

        <div class="overview-chart-grid">
          <SimpleBarChart
            title="Trạng thái nội dung"
            :items="overviewStatusItems"
          />

          <SimpleBarChart
            title="Câu hỏi theo chuyên mục"
            :items="overviewCategoryItems"
          />

          <SimpleBarChart
            title="Top tag được dùng"
            :items="overviewTopTagItems"
          />
        </div>

        <div class="overview-action-grid">
          <el-card shadow="never" class="attention-card">
            <template #header>
              <div class="section-header">
                <div>
                  <strong>Câu hỏi chưa có trả lời</strong>
                  <p>Các câu hỏi mới nhất cần được hỗ trợ.</p>
                </div>
              </div>
            </template>

            <el-table
              v-if="unansweredQuestions.length > 0"
              :data="pagedUnansweredQuestions"
              size="small"
              border
              stripe
            >
              <el-table-column label="Câu hỏi" min-width="260">
                <template #default="s">
                  <div class="attention-question">
                    <div class="attention-title">
                      {{ pick(s.row, ['tieuDe', 'TieuDe']) }}
                    </div>

                    <div class="attention-meta">
                      {{ pick(s.row, ['hoTen', 'HoTen'], 'Ẩn danh') }}
                      ·
                      {{ displayDate(s.row) }}
                    </div>
                  </div>
                </template>
              </el-table-column>

              <el-table-column label="Chuyên mục" width="150" show-overflow-tooltip>
                <template #default="s">
                  {{ pick(s.row, ['tenChuyenMuc', 'TenChuyenMuc'], 'Chưa phân loại') }}
                </template>
              </el-table-column>

              <el-table-column label="Thao tác" width="100" align="center">
                <template #default="s">
                  <router-link
                    class="detail-link"
                    :to="questionLink(s.row)"
                  >
                    Xem
                  </router-link>
                </template>
              </el-table-column>
            </el-table>

            <ForumPagination v-if="unansweredQuestions.length > pagination.overviewUnanswered.pageSize" compact class="overview-small-pagination" :total="unansweredQuestions.length" :page="pagination.overviewUnanswered.page" :page-size="pagination.overviewUnanswered.pageSize" @page-change="page => handlePageChange('overviewUnanswered', page)" @page-size-change="pageSize => handlePageSizeChange('overviewUnanswered', pageSize)" />

            <el-empty
              v-if="unansweredQuestions.length === 0"
              description="Không có câu hỏi nào đang bị bỏ trống"
            />
          </el-card>

          <el-card shadow="never" class="recent-card">
            <template #header>
              <div class="section-header">
                <div>
                  <strong>Hoạt động gần đây</strong>
                  <p>Các hoạt động mới nhất trong hệ thống.</p>
                </div>
              </div>
            </template>

            <el-timeline v-if="recentActivities.length > 0">
              <el-timeline-item
                v-for="activity in pagedRecentActivities"
                :key="activity.key"
                :timestamp="activity.time"
                :type="activity.type"
              >
                <div class="activity-item">
                  <strong>{{ activity.title }}</strong>
                  <p>{{ activity.description }}</p>
                </div>
              </el-timeline-item>
            </el-timeline>

            <ForumPagination v-if="recentActivities.length > pagination.overviewActivities.pageSize" compact class="overview-small-pagination" :total="recentActivities.length" :page="pagination.overviewActivities.page" :page-size="pagination.overviewActivities.pageSize" @page-change="page => handlePageChange('overviewActivities', page)" @page-size-change="pageSize => handlePageSizeChange('overviewActivities', pageSize)" />

            <el-empty
              v-if="recentActivities.length === 0"
              description="Chưa có hoạt động gần đây"
            />
          </el-card>
        </div>
      </el-tab-pane>

      <el-tab-pane label="Người dùng" name="users">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.users.keyword"
            class="admin-search"
            placeholder="Tìm tên hoặc email"
            clearable
            @keyup.enter="loadUsers"
          />

          <el-select
            v-model="filters.users.role"
            placeholder="Vai trò"
            clearable
          >
            <el-option label="User" value="user" />
            <el-option label="Admin" value="admin" />
          </el-select>

          <el-select
            v-model="filters.users.status"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Hoạt động" value="1" />
            <el-option label="Bị khóa" value="0" />
          </el-select>

          <el-select
            v-model="filters.users.timeRange"
            placeholder="Thời gian"
            clearable
          >
            <el-option
              v-for="option in timeOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>

          <el-button type="primary" @click="loadUsers">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedUsers"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ pick(s.row, ['iD_NguoiDung', 'ID_NguoiDung']) }}
            </template>
          </el-table-column>

          <el-table-column label="Họ tên" min-width="170" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column label="Email" min-width="220" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['email', 'Email']) }}
            </template>
          </el-table-column>

          <el-table-column label="Vai trò" width="110" align="center">
            <template #default="s">
              <el-tag :type="isAdminRole(s.row) ? 'warning' : 'info'">
                {{ pick(s.row, ['vaiTro', 'VaiTro']) }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="120" align="center">
            <template #default="s">
              <el-tag :type="isActiveUser(s.row) ? 'success' : 'danger'">
                {{ isActiveUser(s.row) ? 'Hoạt động' : 'Bị khóa' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column
            label="Ngày tạo"
            width="140"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              {{ displayDate(s.row) }}
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="230" align="center">
            <template #default="s">
              <el-button
                v-if="isActiveUser(s.row)"
                size="small"
                type="warning"
                plain
                @click="lockUser(s.row)"
              >
                Khóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="unlockUser(s.row)"
              >
                Mở khóa
              </el-button>

              <el-button
                v-if="isAdminRole(s.row)"
                size="small"
                type="info"
                plain
                @click="changeUserRole(s.row, 'User')"
              >
                Hạ User
              </el-button>

              <el-button
                v-else
                size="small"
                type="primary"
                plain
                @click="changeUserRole(s.row, 'Admin')"
              >
                Cấp Admin
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="filteredUsers.length"
          :page="pagination.users.page"
          :page-size="pagination.users.pageSize"
          @page-change="page => handlePageChange('users', page)"
          @page-size-change="pageSize => handlePageSizeChange('users', pageSize)"
        />
      </el-tab-pane>

      <el-tab-pane label="Câu hỏi" name="questions">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.questions.keyword"
            class="admin-search"
            placeholder="Tìm tiêu đề, tác giả"
            clearable
            @keyup.enter="loadQuestions"
          />

          <el-select
            v-model="filters.questions.chuyenMuc"
            placeholder="Chuyên mục"
            clearable
          >
            <el-option
              v-for="category in categoryOptions"
              :key="category"
              :label="category"
              :value="category"
            />
          </el-select>

          <el-select
            v-model="filters.questions.isDeleted"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>

          <el-select
            v-model="filters.questions.timeRange"
            placeholder="Thời gian"
            clearable
          >
            <el-option
              v-for="option in timeOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>

          <el-button type="primary" @click="loadQuestions">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedQuestions"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ idOf(s.row, 'question') }}
            </template>
          </el-table-column>

          <el-table-column label="Câu hỏi" min-width="360">
            <template #default="s">
              <div class="admin-title-block">
                <div class="admin-main-text">
                  {{ pick(s.row, ['tieuDe', 'TieuDe']) }}
                </div>

                <div class="admin-sub-text">
                  Chuyên mục:
                  {{ pick(s.row, ['tenChuyenMuc', 'TenChuyenMuc'], 'Chưa phân loại') }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Người đăng" min-width="140" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column label="Vote" width="80" align="center">
            <template #default="s">
              {{ num(s.row, ['diemBinhChon', 'DiemBinhChon']) }}
            </template>
          </el-table-column>

          <el-table-column
            label="Thời gian"
            width="155"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              <div class="time-cell">
                <div>
                  <span class="time-label">Tạo:</span>
                  {{ displayDate(s.row) }}
                </div>

                <div>
                  <span class="time-label">Sửa:</span>
                  {{ displayUpdatedDate(s.row) }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="110" align="center">
            <template #default="s">
              <el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">
                {{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="120" align="center">
            <template #default="s">
              <el-button
                v-if="!isDeleted(s.row)"
                size="small"
                type="danger"
                plain
                @click="removeQuestion(s.row)"
              >
                Xóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="restoreQuestion(s.row)"
              >
                Khôi phục
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="filteredQuestions.length"
          :page="pagination.questions.page"
          :page-size="pagination.questions.pageSize"
          @page-change="page => handlePageChange('questions', page)"
          @page-size-change="pageSize => handlePageSizeChange('questions', pageSize)"
        />
      </el-tab-pane>

      <el-tab-pane label="Câu trả lời" name="answers">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.answers.keyword"
            class="admin-search"
            placeholder="Tìm nội dung, người trả lời"
            clearable
            @keyup.enter="loadAnswers"
          />

          <el-select
            v-model="filters.answers.isDeleted"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>

          <el-select
            v-model="filters.answers.acceptedStatus"
            placeholder="Chấp nhận"
            clearable
          >
            <el-option label="Đã chấp nhận" value="1" />
            <el-option label="Chưa chấp nhận" value="0" />
          </el-select>

          <el-select
            v-model="filters.answers.timeRange"
            placeholder="Thời gian"
            clearable
          >
            <el-option
              v-for="option in timeOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>

          <el-button type="primary" @click="loadAnswers">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedAnswers"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ idOf(s.row, 'answer') }}
            </template>
          </el-table-column>

          <el-table-column label="Câu trả lời" min-width="430">
            <template #default="s">
              <div class="admin-title-block">
                <div class="admin-main-text">
                  {{ pick(s.row, ['noiDung', 'NoiDung']) }}
                </div>

                <div class="admin-sub-text">
                  Trong câu hỏi:
                  {{ pick(s.row, ['tieuDeCauHoi', 'TieuDeCauHoi'], '—') }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Người trả lời" min-width="140" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column
            label="Thời gian"
            width="155"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              <div class="time-cell">
                <div>
                  <span class="time-label">Tạo:</span>
                  {{ displayDate(s.row) }}
                </div>

                <div>
                  <span class="time-label">Sửa:</span>
                  {{ displayUpdatedDate(s.row) }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="110" align="center">
            <template #default="s">
              <el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">
                {{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="120" align="center">
            <template #default="s">
              <el-button
                v-if="!isDeleted(s.row)"
                size="small"
                type="danger"
                plain
                @click="removeAnswer(s.row)"
              >
                Xóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="restoreAnswer(s.row)"
              >
                Khôi phục
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="filteredAnswers.length"
          :page="pagination.answers.page"
          :page-size="pagination.answers.pageSize"
          @page-change="page => handlePageChange('answers', page)"
          @page-size-change="pageSize => handlePageSizeChange('answers', pageSize)"
        />
      </el-tab-pane>

      <el-tab-pane label="Bình luận" name="comments">
        <div class="admin-toolbar">
          <el-input
            v-model="filters.comments.keyword"
            class="admin-search"
            placeholder="Tìm nội dung, người bình luận"
            clearable
            @keyup.enter="loadComments"
          />

          <el-select
            v-model="filters.comments.loaiDoiTuong"
            placeholder="Loại nội dung"
            clearable
          >
            <el-option label="Câu hỏi" value="CAUHOI" />
            <el-option label="Câu trả lời" value="CAUTRALOI" />
          </el-select>

          <el-select
            v-model="filters.comments.isDeleted"
            placeholder="Trạng thái"
            clearable
          >
            <el-option label="Đang hiển thị" :value="0" />
            <el-option label="Đã xóa mềm" :value="1" />
          </el-select>

          <el-select
            v-model="filters.comments.timeRange"
            placeholder="Thời gian"
            clearable
          >
            <el-option
              v-for="option in timeOptions"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>

          <el-button type="primary" @click="loadComments">
            Tải dữ liệu
          </el-button>
        </div>

        <el-table
          :data="pagedComments"
          v-loading="loading"
          border
          stripe
          class="responsive-table"
        >
          <el-table-column label="ID" width="70">
            <template #default="s">
              {{ idOf(s.row, 'comment') }}
            </template>
          </el-table-column>

          <el-table-column label="Bình luận" min-width="450">
            <template #default="s">
              <div class="admin-title-block">
                <div class="admin-main-text">
                  {{ pick(s.row, ['noiDung', 'NoiDung']) }}
                </div>

                <div class="admin-sub-text">
                  {{ pick(s.row, ['loaiDoiTuong', 'LoaiDoiTuong'], '—') }}
                  · Thuộc:
                  {{ pick(s.row, ['tieuDeDoiTuong', 'TieuDeDoiTuong', 'tieuDeCauHoi', 'TieuDeCauHoi'], '—') }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Người bình luận" min-width="140" show-overflow-tooltip>
            <template #default="s">
              {{ pick(s.row, ['hoTen', 'HoTen']) }}
            </template>
          </el-table-column>

          <el-table-column
            label="Thời gian"
            width="155"
            sortable
            :sort-method="sortByNgayTao"
          >
            <template #default="s">
              <div class="time-cell">
                <div>
                  <span class="time-label">Tạo:</span>
                  {{ displayDate(s.row) }}
                </div>

                <div>
                  <span class="time-label">Sửa:</span>
                  {{ displayUpdatedDate(s.row) }}
                </div>
              </div>
            </template>
          </el-table-column>

          <el-table-column label="Trạng thái" width="110" align="center">
            <template #default="s">
              <el-tag :type="isDeleted(s.row) ? 'danger' : 'success'">
                {{ isDeleted(s.row) ? 'Đã xóa' : 'Hiện' }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column label="Thao tác" width="120" align="center">
            <template #default="s">
              <el-button
                v-if="!isDeleted(s.row)"
                size="small"
                type="danger"
                plain
                @click="removeComment(s.row)"
              >
                Xóa
              </el-button>

              <el-button
                v-else
                size="small"
                type="success"
                plain
                @click="restoreComment(s.row)"
              >
                Khôi phục
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <ForumPagination
          :total="filteredComments.length"
          :page="pagination.comments.page"
          :page-size="pagination.comments.pageSize"
          @page-change="page => handlePageChange('comments', page)"
          @page-size-change="pageSize => handlePageSizeChange('comments', pageSize)"
        />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getAdminDashboard,
  getUsers,
  getAdminQuestions,
  deleteAdminQuestion,
  restoreAdminQuestion,
  getAdminAnswers,
  deleteAdminAnswer,
  restoreAdminAnswer,
  getAdminComments,
  deleteAdminComment,
  restoreAdminComment,
  lockAdminUser,
  unlockAdminUser,
  updateAdminUserRole
} from '../api/adminApi'
import { pick, idOf, formatDate } from '../utils/format'
import SimpleBarChart from '../components/SimpleBarChart.vue'
import ForumPagination from '../components/ForumPagination.vue'

const activeTab = ref('overview')
const loading = ref(false)
const loadingDashboard = ref(false)
const loadingOverview = ref(false)

const dashboard = ref({})
const users = ref([])
const questions = ref([])
const answers = ref([])
const comments = ref([])

const overviewFilters = reactive({
  timeRange: 'last7days'
})

const pagination = reactive({
  overviewUnanswered: {
    page: 1,
    pageSize: 5
  },
  overviewActivities: {
    page: 1,
    pageSize: 5
  },
  users: {
    page: 1,
    pageSize: 5
  },
  questions: {
    page: 1,
    pageSize: 5
  },
  answers: {
    page: 1,
    pageSize: 5
  },
  comments: {
    page: 1,
    pageSize: 5
  }
})

const filters = reactive({
  users: {
    keyword: '',
    role: '',
    status: '',
    timeRange: ''
  },
  questions: {
    keyword: '',
    chuyenMuc: '',
    isDeleted: '',
    timeRange: ''
  },
  answers: {
    keyword: '',
    isDeleted: '',
    acceptedStatus: '',
    timeRange: ''
  },
  comments: {
    keyword: '',
    loaiDoiTuong: '',
    isDeleted: '',
    timeRange: ''
  }
})

const overviewTimeOptions = [
  { label: 'Hôm nay', value: 'today' },
  { label: '7 ngày gần đây', value: 'last7days' },
  { label: 'Tháng này', value: 'thisMonth' },
  { label: 'Năm nay', value: 'thisYear' }
]

const timeOptions = [
  { label: 'Tất cả thời gian', value: '' },
  ...overviewTimeOptions
]

const overviewPeriodLabel = computed(() => {
  return timeRangeLabel(overviewFilters.timeRange)
})

const overviewUsers = computed(() => {
  return users.value.filter(row => isInTimeRange(row, overviewFilters.timeRange))
})

const overviewQuestions = computed(() => {
  return questions.value.filter(row => isInTimeRange(row, overviewFilters.timeRange))
})

const overviewAnswers = computed(() => {
  return answers.value.filter(row => isInTimeRange(row, overviewFilters.timeRange))
})

const overviewComments = computed(() => {
  return comments.value.filter(row => isInTimeRange(row, overviewFilters.timeRange))
})

const overviewStats = computed(() => {
  const totalUsers = num(dashboard.value, ['tongNguoiDung', 'TongNguoiDung']) || users.value.length

  const deletedQuestions = overviewQuestions.value.filter(isDeleted).length
  const deletedAnswers = overviewAnswers.value.filter(isDeleted).length
  const deletedComments = overviewComments.value.filter(isDeleted).length

  return {
    totalUsers,
    newUsers: overviewUsers.value.length,
    newQuestions: overviewQuestions.value.length,
    newAnswers: overviewAnswers.value.length,
    newComments: overviewComments.value.length,
    deletedContents: deletedQuestions + deletedAnswers + deletedComments
  }
})

const overviewCards = computed(() => {
  return [
    {
      key: 'totalUsers',
      label: 'Tổng người dùng',
      value: overviewStats.value.totalUsers,
      hint: 'Toàn hệ thống',
      icon: '👥',
      className: 'card-users'
    },
    {
      key: 'newUsers',
      label: 'Người dùng mới',
      value: overviewStats.value.newUsers,
      hint: overviewPeriodLabel.value,
      icon: '🆕',
      className: 'card-new-users'
    },
    {
      key: 'newQuestions',
      label: 'Câu hỏi mới',
      value: overviewStats.value.newQuestions,
      hint: overviewPeriodLabel.value,
      icon: '❓',
      className: 'card-questions'
    },
    {
      key: 'newAnswers',
      label: 'Câu trả lời mới',
      value: overviewStats.value.newAnswers,
      hint: overviewPeriodLabel.value,
      icon: '💬',
      className: 'card-answers'
    },
    {
      key: 'newComments',
      label: 'Bình luận mới',
      value: overviewStats.value.newComments,
      hint: overviewPeriodLabel.value,
      icon: '🗨️',
      className: 'card-comments'
    },
    {
      key: 'deletedContents',
      label: 'Nội dung đã xóa',
      value: overviewStats.value.deletedContents,
      hint: overviewPeriodLabel.value,
      icon: '🗑️',
      className: 'card-deleted'
    }
  ]
})

const overviewStatusItems = computed(() => {
  const visibleQuestions = overviewQuestions.value.filter(row => !isDeleted(row)).length
  const deletedQuestions = overviewQuestions.value.filter(isDeleted).length

  const visibleAnswers = overviewAnswers.value.filter(row => !isDeleted(row)).length
  const deletedAnswers = overviewAnswers.value.filter(isDeleted).length

  const visibleComments = overviewComments.value.filter(row => !isDeleted(row)).length
  const deletedComments = overviewComments.value.filter(isDeleted).length

  return [
    chartItem('Câu hỏi hiện', visibleQuestions),
    chartItem('Câu hỏi xóa', deletedQuestions),
    chartItem('Trả lời hiện', visibleAnswers),
    chartItem('Trả lời xóa', deletedAnswers),
    chartItem('Bình luận hiện', visibleComments),
    chartItem('Bình luận xóa', deletedComments)
  ]
})

const overviewCategoryItems = computed(() => {
  const map = new Map()

  overviewQuestions.value.forEach(row => {
    const name = pick(row, ['tenChuyenMuc', 'TenChuyenMuc'], 'Chưa phân loại')
    map.set(name, (map.get(name) || 0) + 1)
  })

  return mapToTopItems(map, 8)
})

const overviewTopTagItems = computed(() => {
  const map = new Map()

  overviewQuestions.value.forEach(row => {
    getTagNames(row).forEach(tag => {
      map.set(tag, (map.get(tag) || 0) + 1)
    })
  })

  const items = mapToTopItems(map, 8)

  if (items.length > 0) {
    return items
  }

  return arr(dashboard.value, ['topTags', 'TopTags'])
})

const activityChartRows = computed(() => {
  const groups = createActivityGroups(overviewFilters.timeRange)

  return groups.map(group => ({
    label: group.label,
    users: countRowsInDateRange(users.value, group.start, group.end),
    questions: countRowsInDateRange(questions.value, group.start, group.end),
    answers: countRowsInDateRange(answers.value, group.start, group.end),
    comments: countRowsInDateRange(comments.value, group.start, group.end)
  }))
})

const activityChart = computed(() => {
  const width = 760
  const height = 280
  const padding = {
    left: 44,
    right: 16,
    top: 18,
    bottom: 44
  }

  const rows = activityChartRows.value
  const allValues = rows.flatMap(row => [
    row.users,
    row.questions,
    row.answers,
    row.comments
  ])

  const maxValue = Math.max(...allValues, 1)
  const plotWidth = width - padding.left - padding.right
  const plotHeight = height - padding.top - padding.bottom

  const xOf = index => {
    if (rows.length <= 1) {
      return padding.left + plotWidth / 2
    }

    return padding.left + (index / (rows.length - 1)) * plotWidth
  }

  const yOf = value => {
    return padding.top + plotHeight - (value / maxValue) * plotHeight
  }

  const buildSeries = (key, label, className) => {
    const points = rows.map((row, index) => {
      const value = Number(row[key]) || 0
      const x = xOf(index)
      const y = yOf(value)

      return {
        x,
        y,
        label: row.label,
        value,
        tooltip: `${label} - ${row.label}: ${value}`
      }
    })

    return {
      key,
      label,
      className,
      points,
      pointsText: points.map(point => `${point.x},${point.y}`).join(' ')
    }
  }

  const tickValues = [
    maxValue,
    Math.round(maxValue * 0.75),
    Math.round(maxValue * 0.5),
    Math.round(maxValue * 0.25),
    0
  ]

  const yTicks = [...new Set(tickValues)]
    .sort((a, b) => b - a)
    .map(value => ({
      value,
      y: yOf(value)
    }))

  const labelStep = Math.max(1, Math.ceil(rows.length / 7))

  const xLabels = rows
    .map((row, index) => ({
      index,
      text: row.label,
      x: xOf(index)
    }))
    .filter((label, index) => {
      return index === 0 || index === rows.length - 1 || index % labelStep === 0
    })

  const hasData = allValues.some(value => Number(value) > 0)

  return {
    width,
    height,
    padding,
    hasData,
    yTicks,
    xLabels,
    series: [
      buildSeries('users', 'Người dùng mới', 'users'),
      buildSeries('questions', 'Câu hỏi mới', 'questions'),
      buildSeries('answers', 'Câu trả lời mới', 'answers'),
      buildSeries('comments', 'Bình luận mới', 'comments')
    ]
  }
})

const unansweredQuestions = computed(() => {
  const answeredQuestionIds = new Set(
    answers.value
      .filter(row => !isDeleted(row))
      .map(row => answerQuestionIdOf(row))
      .filter(id => id > 0)
  )

  return overviewQuestions.value
    .filter(row => !isDeleted(row))
    .filter(row => !answeredQuestionIds.has(questionIdOf(row)))
    .sort((a, b) => createdDateValue(b) - createdDateValue(a))
})

const pagedUnansweredQuestions = computed(() => {
  return paginate(unansweredQuestions.value, pagination.overviewUnanswered)
})

const recentActivities = computed(() => {
  const activities = []

  overviewUsers.value.forEach(row => {
    activities.push({
      key: `user-${userId(row)}`,
      dateValue: createdDateValue(row),
      time: displayDate(row),
      type: 'success',
      title: 'Người dùng mới',
      description: `${userName(row)} vừa tạo tài khoản.`
    })
  })

  overviewQuestions.value.forEach(row => {
    activities.push({
      key: `question-${questionIdOf(row)}`,
      dateValue: createdDateValue(row),
      time: displayDate(row),
      type: 'primary',
      title: 'Câu hỏi mới',
      description: truncateText(pick(row, ['tieuDe', 'TieuDe'], 'Một câu hỏi mới'), 90)
    })
  })

  overviewAnswers.value.forEach(row => {
    activities.push({
      key: `answer-${idOf(row, 'answer')}`,
      dateValue: createdDateValue(row),
      time: displayDate(row),
      type: 'warning',
      title: 'Câu trả lời mới',
      description: `${pick(row, ['hoTen', 'HoTen'], 'Một người dùng')} trả lời: ${truncateText(pick(row, ['tieuDeCauHoi', 'TieuDeCauHoi'], 'một câu hỏi'), 70)}`
    })
  })

  overviewComments.value.forEach(row => {
    activities.push({
      key: `comment-${idOf(row, 'comment')}`,
      dateValue: createdDateValue(row),
      time: displayDate(row),
      type: 'info',
      title: 'Bình luận mới',
      description: truncateText(pick(row, ['noiDung', 'NoiDung'], 'Một bình luận mới'), 90)
    })
  })

  return activities
    .filter(item => item.dateValue > 0)
    .sort((a, b) => b.dateValue - a.dateValue)
})

const pagedRecentActivities = computed(() => {
  return paginate(recentActivities.value, pagination.overviewActivities)
})

const categoryOptions = computed(() => {
  const names = questions.value
    .map(row => pick(row, ['tenChuyenMuc', 'TenChuyenMuc'], ''))
    .filter(name => String(name).trim() !== '')

  return [...new Set(names)].sort((a, b) => String(a).localeCompare(String(b)))
})

const filteredUsers = computed(() => {
  return users.value.filter(row => {
    const role = String(pick(row, ['vaiTro', 'VaiTro'], '')).toLowerCase()
    const status = String(pick(row, ['trangThai', 'TrangThai'], '1'))

    const matchRole = !filters.users.role || role === filters.users.role
    const matchStatus = !filters.users.status || status === filters.users.status
    const matchTime = isInTimeRange(row, filters.users.timeRange)

    return matchRole && matchStatus && matchTime
  })
})

const filteredQuestions = computed(() => {
  return questions.value.filter(row => {
    const tenChuyenMuc = pick(row, ['tenChuyenMuc', 'TenChuyenMuc'], '')

    const matchCategory = !filters.questions.chuyenMuc
      || tenChuyenMuc === filters.questions.chuyenMuc

    const matchTime = isInTimeRange(row, filters.questions.timeRange)

    return matchCategory && matchTime
  })
})

const filteredAnswers = computed(() => {
  const keyword = normalizeKeyword(filters.answers.keyword)

  return answers.value.filter(row => {
    const matchKeyword = !keyword || includesKeyword(row, keyword, [
      ['tieuDeCauHoi', 'TieuDeCauHoi'],
      ['noiDung', 'NoiDung'],
      ['hoTen', 'HoTen']
    ])

    const daChapNhan = Number(pick(row, ['daChapNhan', 'DaChapNhan'], 0))

    const matchAccepted = !filters.answers.acceptedStatus
      || String(daChapNhan) === filters.answers.acceptedStatus

    const matchTime = isInTimeRange(row, filters.answers.timeRange)

    return matchKeyword && matchAccepted && matchTime
  })
})

const filteredComments = computed(() => {
  const keyword = normalizeKeyword(filters.comments.keyword)

  return comments.value.filter(row => {
    const matchKeyword = !keyword || includesKeyword(row, keyword, [
      ['noiDung', 'NoiDung'],
      ['hoTen', 'HoTen'],
      ['loaiDoiTuong', 'LoaiDoiTuong'],
      ['tieuDeDoiTuong', 'TieuDeDoiTuong'],
      ['tieuDeCauHoi', 'TieuDeCauHoi']
    ])

    const matchTime = isInTimeRange(row, filters.comments.timeRange)

    return matchKeyword && matchTime
  })
})

const pagedUsers = computed(() => {
  return paginate(filteredUsers.value, pagination.users)
})

const pagedQuestions = computed(() => {
  return paginate(filteredQuestions.value, pagination.questions)
})

const pagedAnswers = computed(() => {
  return paginate(filteredAnswers.value, pagination.answers)
})

const pagedComments = computed(() => {
  return paginate(filteredComments.value, pagination.comments)
})

onMounted(loadAll)

watch(
  () => overviewFilters.timeRange,
  () => {
    pagination.overviewUnanswered.page = 1
    pagination.overviewActivities.page = 1
  }
)

watch(
  () => [
    filters.users.keyword,
    filters.users.role,
    filters.users.status,
    filters.users.timeRange
  ],
  () => {
    pagination.users.page = 1
  }
)

watch(
  () => [
    filters.questions.keyword,
    filters.questions.chuyenMuc,
    filters.questions.isDeleted,
    filters.questions.timeRange
  ],
  () => {
    pagination.questions.page = 1
  }
)

watch(
  () => [
    filters.answers.keyword,
    filters.answers.isDeleted,
    filters.answers.acceptedStatus,
    filters.answers.timeRange
  ],
  () => {
    pagination.answers.page = 1
  }
)

watch(
  () => [
    filters.comments.keyword,
    filters.comments.loaiDoiTuong,
    filters.comments.isDeleted,
    filters.comments.timeRange
  ],
  () => {
    pagination.comments.page = 1
  }
)

function num(obj, keys) {
  return Number(pick(obj, keys, 0)) || 0
}

function arr(obj, keys) {
  return pick(obj, keys, []) || []
}

function isDeleted(row) {
  return Number(pick(row, ['isDeleted', 'IsDeleted'], 0)) === 1
}

function isActiveUser(row) {
  return Number(pick(row, ['trangThai', 'TrangThai'], 1)) === 1
}

function isAdminRole(row) {
  return String(pick(row, ['vaiTro', 'VaiTro'], '')).toLowerCase() === 'admin'
}

function normalizeKeyword(value) {
  return String(value || '').trim().toLowerCase()
}

function includesKeyword(row, keyword, keyGroups) {
  return keyGroups.some(keys => {
    return String(pick(row, keys, '') || '').toLowerCase().includes(keyword)
  })
}

function timeRangeLabel(value) {
  const option = timeOptions.find(item => item.value === value)
  return option?.label || 'Tất cả thời gian'
}

function isInTimeRange(row, timeRange) {
  if (!timeRange) return true

  const createdTime = createdDateValue(row)
  if (!createdTime) return false

  const createdDate = new Date(createdTime)
  const now = new Date()

  if (timeRange === 'today') {
    return createdDate.getFullYear() === now.getFullYear()
      && createdDate.getMonth() === now.getMonth()
      && createdDate.getDate() === now.getDate()
  }

  if (timeRange === 'last7days') {
    const sevenDaysAgo = new Date()
    sevenDaysAgo.setDate(now.getDate() - 6)
    sevenDaysAgo.setHours(0, 0, 0, 0)

    return createdDate >= sevenDaysAgo && createdDate <= now
  }

  if (timeRange === 'thisMonth') {
    return createdDate.getFullYear() === now.getFullYear()
      && createdDate.getMonth() === now.getMonth()
  }

  if (timeRange === 'thisYear') {
    return createdDate.getFullYear() === now.getFullYear()
  }

  return true
}

function createActivityGroups(timeRange) {
  const now = new Date()
  const groups = []

  if (timeRange === 'today') {
    for (let hour = 0; hour < 24; hour++) {
      const start = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hour, 0, 0)
      const end = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hour + 1, 0, 0)

      groups.push({
        label: `${hour}h`,
        start,
        end
      })
    }

    return groups
  }

  if (timeRange === 'thisMonth') {
    const currentDay = now.getDate()

    for (let day = 1; day <= currentDay; day++) {
      const start = new Date(now.getFullYear(), now.getMonth(), day, 0, 0, 0)
      const end = new Date(now.getFullYear(), now.getMonth(), day + 1, 0, 0, 0)

      groups.push({
        label: `${day}/${now.getMonth() + 1}`,
        start,
        end
      })
    }

    return groups
  }

  if (timeRange === 'thisYear') {
    for (let month = 0; month < 12; month++) {
      const start = new Date(now.getFullYear(), month, 1, 0, 0, 0)
      const end = new Date(now.getFullYear(), month + 1, 1, 0, 0, 0)

      groups.push({
        label: `T${month + 1}`,
        start,
        end
      })
    }

    return groups
  }

  for (let i = 6; i >= 0; i--) {
    const date = new Date()
    date.setDate(now.getDate() - i)

    const start = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0)
    const end = new Date(date.getFullYear(), date.getMonth(), date.getDate() + 1, 0, 0, 0)

    groups.push({
      label: `${date.getDate()}/${date.getMonth() + 1}`,
      start,
      end
    })
  }

  return groups
}

function countRowsInDateRange(rows, start, end) {
  return rows.filter(row => {
    const createdTime = createdDateValue(row)

    if (!createdTime) return false

    return createdTime >= start.getTime() && createdTime < end.getTime()
  }).length
}

function getTagNames(row) {
  const raw = pick(row, [
    'tags',
    'Tags',
    'the',
    'The',
    'tenThe',
    'TenThe',
    'danhSachThe',
    'DanhSachThe'
  ], '')

  if (Array.isArray(raw)) {
    return raw
      .map(item => {
        if (typeof item === 'string') return item

        return pick(item, ['ten', 'Ten', 'tenThe', 'TenThe', 'name', 'Name'], '')
      })
      .map(item => String(item).trim())
      .filter(Boolean)
  }

  return String(raw || '')
    .split(/[,;|]/)
    .map(item => item.trim())
    .filter(Boolean)
}

function chartItem(name, value) {
  return {
    ten: name,
    Ten: name,
    label: name,
    Label: name,
    giaTri: value,
    GiaTri: value,
    value,
    Value: value,
    soLuong: value,
    SoLuong: value
  }
}

function mapToTopItems(map, limit = 8) {
  return [...map.entries()]
    .map(([name, value]) => chartItem(name, value))
    .sort((a, b) => Number(b.giaTri) - Number(a.giaTri))
    .slice(0, limit)
}

function questionIdOf(row) {
  return Number(pick(row, [
    'iD_CauHoi',
    'ID_CauHoi',
    'idCauHoi',
    'IdCauHoi'
  ], idOf(row, 'question'))) || 0
}

function answerQuestionIdOf(row) {
  return Number(pick(row, [
    'iD_CauHoi',
    'ID_CauHoi',
    'idCauHoi',
    'IdCauHoi'
  ], 0)) || 0
}

function questionLink(row) {
  return `/questions/${questionIdOf(row)}`
}

function truncateText(value, maxLength = 80) {
  const text = String(value || '').trim()

  if (text.length <= maxLength) return text

  return `${text.slice(0, maxLength)}...`
}

function paginate(list, config) {
  const start = (config.page - 1) * config.pageSize
  const end = start + config.pageSize

  return list.slice(start, end)
}

function handlePageChange(type, page) {
  pagination[type].page = page
}

function handlePageSizeChange(type, pageSize) {
  pagination[type].pageSize = pageSize
  pagination[type].page = 1
}

function parseDateValue(value) {
  if (!value) return 0

  const date = new Date(String(value).replace(' ', 'T'))

  return Number.isNaN(date.getTime()) ? 0 : date.getTime()
}

function createdDateValue(row) {
  return parseDateValue(pick(row, ['ngayTao', 'NgayTao'], ''))
}

function sortByNgayTao(a, b) {
  return createdDateValue(a) - createdDateValue(b)
}

function displayDate(row) {
  const value = pick(row, ['ngayTao', 'NgayTao'], '')

  if (!value) return '—'

  return formatDate(value)
}

function displayUpdatedDate(row) {
  const updatedValue = pick(row, ['ngayCapNhat', 'NgayCapNhat'], '')

  if (!updatedValue) return '—'

  const createdTime = createdDateValue(row)
  const updatedTime = parseDateValue(updatedValue)

  if (!updatedTime) return '—'

  if (createdTime && createdTime === updatedTime) {
    return '—'
  }

  return formatDate(updatedValue)
}

function userId(row) {
  return Number(pick(row, ['iD_NguoiDung', 'ID_NguoiDung'], 0))
}

function userName(row) {
  return pick(row, ['hoTen', 'HoTen'], 'người dùng này')
}

function lockUser(row) {
  return confirmAction(
    `Khóa tài khoản "${userName(row)}"? Người dùng này sẽ không thể đăng nhập.`,
    () => lockAdminUser(userId(row)),
    loadUsers
  )
}

function unlockUser(row) {
  return confirmAction(
    `Mở khóa tài khoản "${userName(row)}"?`,
    () => unlockAdminUser(userId(row)),
    loadUsers
  )
}

function changeUserRole(row, role) {
  const actionText = role === 'Admin'
    ? 'cấp quyền Admin cho'
    : 'hạ quyền về User cho'

  return confirmAction(
    `Bạn có chắc muốn ${actionText} "${userName(row)}"?`,
    () => updateAdminUserRole(userId(row), role),
    loadUsers
  )
}

async function loadAll() {
  if (activeTab.value === 'overview') {
    await loadOverview()
    return
  }

  await loadDashboard()
  await loadCurrentTab()
}

async function loadOverview() {
  await Promise.all([
    loadDashboard(),
    loadOverviewData()
  ])
}

async function loadOverviewData() {
  loadingOverview.value = true

  try {
    const [
      userRows,
      questionRows,
      answerRows,
      commentRows
    ] = await Promise.all([
      getUsers({}),
      getAdminQuestions({}),
      getAdminAnswers({}),
      getAdminComments({})
    ])

    users.value = userRows
    questions.value = questionRows
    answers.value = answerRows
    comments.value = commentRows

    pagination.overviewUnanswered.page = 1
    pagination.overviewActivities.page = 1
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được dữ liệu tổng quan.')
  } finally {
    loadingOverview.value = false
  }
}

async function loadDashboard() {
  loadingDashboard.value = true

  try {
    dashboard.value = await getAdminDashboard()
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được dashboard quản trị.')
  } finally {
    loadingDashboard.value = false
  }
}

function loadCurrentTab() {
  if (activeTab.value === 'overview') return loadOverview()
  if (activeTab.value === 'users') return loadUsers()
  if (activeTab.value === 'questions') return loadQuestions()
  if (activeTab.value === 'answers') return loadAnswers()
  if (activeTab.value === 'comments') return loadComments()
}

async function runLoad(fn) {
  loading.value = true

  try {
    await fn()
  } catch (error) {
    ElMessage.error(error.message || 'Không tải được dữ liệu quản trị.')
  } finally {
    loading.value = false
  }
}

async function loadUsers() {
  await runLoad(async () => {
    users.value = await getUsers({
      keyword: filters.users.keyword
    })

    pagination.users.page = 1
  })
}

async function loadQuestions() {
  await runLoad(async () => {
    questions.value = await getAdminQuestions({
      keyword: filters.questions.keyword,
      isDeleted: filters.questions.isDeleted
    })

    pagination.questions.page = 1
  })
}

async function loadAnswers() {
  await runLoad(async () => {
    answers.value = await getAdminAnswers({
      isDeleted: filters.answers.isDeleted
    })

    pagination.answers.page = 1
  })
}

async function loadComments() {
  await runLoad(async () => {
    comments.value = await getAdminComments({
      loaiDoiTuong: filters.comments.loaiDoiTuong,
      isDeleted: filters.comments.isDeleted
    })

    pagination.comments.page = 1
  })
}

async function confirmAction(message, action, reload) {
  try {
    await ElMessageBox.confirm(message, 'Xác nhận', {
      type: 'warning',
      confirmButtonText: 'Đồng ý',
      cancelButtonText: 'Hủy'
    })

    const result = await action()

    ElMessage.success(
      result?.message ||
      result?.Message ||
      'Thao tác thành công.'
    )

    await reload()
    await loadDashboard()
  } catch (error) {
    if (error === 'cancel') return

    ElMessage.error(
      error?.data?.message ||
      error?.data?.Message ||
      error?.message ||
      'Thao tác thất bại.'
    )
  }
}

function removeQuestion(row) {
  return confirmAction(
    'Admin sẽ xóa mềm câu hỏi này?',
    () => deleteAdminQuestion(idOf(row, 'question')),
    loadQuestions
  )
}

function restoreQuestion(row) {
  return confirmAction(
    'Khôi phục câu hỏi này?',
    () => restoreAdminQuestion(idOf(row, 'question')),
    loadQuestions
  )
}

function removeAnswer(row) {
  return confirmAction(
    'Admin sẽ xóa mềm câu trả lời này?',
    () => deleteAdminAnswer(idOf(row, 'answer')),
    loadAnswers
  )
}

function restoreAnswer(row) {
  return confirmAction(
    'Khôi phục câu trả lời này?',
    () => restoreAdminAnswer(idOf(row, 'answer')),
    loadAnswers
  )
}

function removeComment(row) {
  return confirmAction(
    'Admin sẽ xóa mềm bình luận này?',
    () => deleteAdminComment(idOf(row, 'comment')),
    loadComments
  )
}

function restoreComment(row) {
  return confirmAction(
    'Khôi phục bình luận này?',
    () => restoreAdminComment(idOf(row, 'comment')),
    loadComments
  )
}
</script>

<style scoped>
.admin-page :deep(.el-tabs__content) {
  padding-top: 14px;
}

.overview-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
  flex-wrap: wrap;
  margin-bottom: 16px;
}

.overview-title {
  margin: 0;
  color: var(--forum-text);
  font-size: 20px;
  font-weight: 700;
}

.overview-subtitle {
  margin: 5px 0 0;
  color: var(--forum-muted);
  font-size: 13px;
}

.overview-filter-group {
  display: flex;
  gap: 10px;
  align-items: center;
  flex-wrap: wrap;
}

.overview-filter-group .el-select {
  width: 220px;
}

.overview-filter-group .el-button {
  min-width: 120px;
}

.overview-metric-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.overview-metric-card {
  position: relative;
  overflow: hidden;
  border: none;
  border-radius: 16px;
}

.metric-content {
  position: relative;
  z-index: 2;
}

.metric-content p {
  margin: 0;
  color: var(--forum-muted);
  font-size: 13px;
  font-weight: 500;
}

.metric-content strong {
  display: block;
  margin-top: 8px;
  color: var(--forum-text);
  font-size: 30px;
  line-height: 1;
}

.metric-content span {
  display: block;
  margin-top: 8px;
  color: var(--forum-muted);
  font-size: 12px;
}

.metric-icon {
  position: absolute;
  right: 18px;
  bottom: 10px;
  font-size: 44px;
  opacity: 0.18;
  z-index: 1;
}

.card-users {
  background: #eef6ff;
}

.card-new-users {
  background: #ecfdf5;
}

.card-questions {
  background: #fff7ed;
}

.card-answers {
  background: #f0fdf4;
}

.card-comments {
  background: #f5f3ff;
}

.card-deleted {
  background: #fef2f2;
}

.activity-line-card {
  border-radius: 14px;
  margin-top: 16px;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: flex-start;
}

.chart-header strong,
.section-header strong {
  color: var(--forum-text);
  font-size: 15px;
}

.chart-header p,
.section-header p {
  margin: 4px 0 0;
  color: var(--forum-muted);
  font-size: 12px;
}

.line-chart-wrap {
  width: 100%;
}

.line-chart {
  width: 100%;
  min-height: 260px;
  display: block;
}

.y-grid line {
  stroke: #e8edf5;
  stroke-width: 1;
}

.y-grid text,
.x-labels text {
  fill: var(--forum-muted);
  font-size: 11px;
}

.chart-line {
  stroke-width: 2.4;
  stroke-linecap: round;
  stroke-linejoin: round;
}

.chart-point {
  stroke: #ffffff;
  stroke-width: 2;
}

.chart-line.users,
.chart-point.users,
.legend-dot.users {
  stroke: #409eff;
  background: #409eff;
  fill: #409eff;
}

.chart-line.questions,
.chart-point.questions,
.legend-dot.questions {
  stroke: #67c23a;
  background: #67c23a;
  fill: #67c23a;
}

.chart-line.answers,
.chart-point.answers,
.legend-dot.answers {
  stroke: #e6a23c;
  background: #e6a23c;
  fill: #e6a23c;
}

.chart-line.comments,
.chart-point.comments,
.legend-dot.comments {
  stroke: #f56c6c;
  background: #f56c6c;
  fill: #f56c6c;
}

.line-legend {
  display: flex;
  gap: 14px;
  flex-wrap: wrap;
  justify-content: center;
  margin-top: 8px;
  color: var(--forum-muted);
  font-size: 12px;
}

.legend-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.legend-dot {
  width: 9px;
  height: 9px;
  border-radius: 999px;
  display: inline-block;
}

.overview-chart-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
  margin-top: 16px;
}

.overview-action-grid {
  display: grid;
  grid-template-columns: minmax(0, 7fr) minmax(320px, 5fr);
  gap: 16px;
  margin-top: 16px;
}

.attention-card,
.recent-card {
  border-radius: 14px;
}

.attention-question {
  min-width: 0;
}

.attention-title {
  color: var(--forum-text);
  font-weight: 500;
  line-height: 1.4;
}

.attention-meta {
  margin-top: 4px;
  color: var(--forum-muted);
  font-size: 12px;
}

.detail-link {
  color: #f97316;
  font-weight: 600;
  text-decoration: none;
}

.detail-link:hover {
  text-decoration: underline;
}

.activity-item strong {
  color: var(--forum-text);
  font-size: 13px;
}

.activity-item p {
  margin: 4px 0 0;
  color: var(--forum-muted);
  font-size: 12px;
  line-height: 1.45;
}

.overview-small-pagination {
  margin-top: 12px;
}

.overview-small-pagination :deep(.pagination-wrap) {
  margin: 12px 0 0;
  justify-content: center;
}

.overview-small-pagination :deep(.el-pagination) {
  flex-wrap: wrap;
  row-gap: 8px;
}

.admin-toolbar {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
  margin-bottom: 14px;
  align-items: center;
}

.admin-toolbar .admin-search {
  width: 340px;
}

.admin-toolbar .el-select {
  width: 220px;
}

.admin-toolbar .el-button {
  min-width: 110px;
}

.responsive-table {
  width: 100%;
}

.responsive-table :deep(.el-table__cell) {
  vertical-align: middle;
  padding: 8px 0;
}

.responsive-table :deep(.el-table__header th) {
  color: var(--forum-text);
  font-weight: 600;
  background: #fbfcff;
}

.responsive-table :deep(.el-table__fixed-right),
.responsive-table :deep(.el-table__fixed-right::before),
.responsive-table :deep(.el-table__fixed-right-patch) {
  box-shadow: none;
}

.admin-title-block {
  min-width: 0;
}

.admin-main-text {
  color: var(--forum-text);
  font-weight: 400;
  line-height: 1.45;
  white-space: normal;
  overflow: visible;
}

.admin-sub-text {
  margin-top: 4px;
  color: var(--forum-muted);
  font-size: 12px;
  line-height: 1.35;
  white-space: normal;
  overflow: visible;
}

.time-cell {
  color: var(--forum-muted);
  font-size: 12px;
  line-height: 1.6;
  white-space: nowrap;
}

.time-label {
  color: var(--forum-muted);
  font-weight: 400;
}

@media (max-width: 1180px) {
  .overview-metric-grid,
  .overview-chart-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .overview-action-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 760px) {
  .overview-metric-grid,
  .overview-chart-grid {
    grid-template-columns: 1fr;
  }

  .overview-filter-group,
  .overview-filter-group .el-select,
  .overview-filter-group .el-button,
  .admin-toolbar .el-input,
  .admin-toolbar .el-select,
  .admin-toolbar .el-button {
    width: 100%;
  }
}
</style>
