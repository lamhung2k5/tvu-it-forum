<template>
  <div class="page-container about-page">
    <section class="about-hero">
      <section class="about-hero">
        <img
            src="/images/cet-hero.jpg"
            alt="CET FORIT"
            class="about-hero-image"
        />
        </section>
    </section>

    <section class="about-section">
      <div class="section-heading">
        <h2>Mục tiêu của diễn đàn</h2>
        <p>
          Hệ thống được xây dựng nhằm tạo môi trường trao đổi học thuật
          thân thiện, tập trung vào các chủ đề học tập và kỹ thuật.
        </p>
      </div>

      <div class="about-card-grid">
        <el-card
          v-for="item in goals"
          :key="item.title"
          shadow="never"
          class="about-card"
        >
          <div class="about-card-icon">
            <el-icon>
              <component :is="item.icon" />
            </el-icon>
          </div>

          <h3>{{ item.title }}</h3>
          <p>{{ item.description }}</p>
        </el-card>
      </div>
    </section>

    <section class="about-section">
      <div class="section-heading">
        <h2>Chức năng chính</h2>
        <p>
          Diễn đàn cung cấp các chức năng cơ bản của một hệ thống hỏi đáp
          dành cho sinh viên ngành công nghệ thông tin.
        </p>
      </div>

      <div class="feature-list">
        <div
          v-for="feature in features"
          :key="feature.title"
          class="feature-item"
        >
          <div class="feature-icon">
            <el-icon>
              <component :is="feature.icon" />
            </el-icon>
          </div>

          <div>
            <h3>{{ feature.title }}</h3>
            <p>{{ feature.description }}</p>
          </div>
        </div>
      </div>
    </section>

    <section class="about-section">
      <div class="section-heading">
        <h2>Chủ đề thảo luận</h2>
        <p>
          Các nội dung được tổ chức theo chuyên mục và thẻ để người dùng
          dễ dàng tìm kiếm, theo dõi và trao đổi.
        </p>
      </div>

      <div class="tag-cloud">
        <el-tag
          v-for="tag in topics"
          :key="tag"
          effect="plain"
          round
        >
          {{ tag }}
        </el-tag>
      </div>
    </section>

    <section class="about-final">
      <h2>Cùng tham gia xây dựng cộng đồng học tập</h2>
      <p>
        Mỗi câu hỏi, câu trả lời và bình luận tích cực đều góp phần tạo nên
        nguồn tài liệu học tập hữu ích cho sinh viên.
      </p>

      <el-button size="large" class="final-button" @click="$router.push('/')">
        Bắt đầu khám phá
      </el-button>
    </section>
  </div>
</template>

<script setup>
import {
  Bell,
  ChatDotRound,
  ChatLineRound,
  DataBoard,
  EditPen,
  Reading,
  Setting,
  Star,
  User
} from '@element-plus/icons-vue'

const goals = [
  {
    icon: Reading,
    title: 'Hỗ trợ học tập',
    description: 'Giúp sinh viên đặt câu hỏi, nhận câu trả lời và trao đổi kiến thức trong quá trình học.'
  },
  {
    icon: ChatDotRound,
    title: 'Tăng tương tác',
    description: 'Tạo môi trường để sinh viên thảo luận, bình luận và chia sẻ kinh nghiệm học tập.'
  },
  {
    icon: Setting,
    title: 'Quản lý nội dung',
    description: 'Hỗ trợ quản trị viên theo dõi, kiểm duyệt và xử lý nội dung không phù hợp.'
  }
]

const features = [
  {
    icon: EditPen,
    title: 'Đăng câu hỏi theo chuyên mục',
    description: 'Người dùng có thể tạo câu hỏi, chọn chuyên mục và gắn thẻ để dễ tìm kiếm.'
  },
  {
    icon: ChatLineRound,
    title: 'Trả lời và bình luận',
    description: 'Sinh viên có thể trả lời câu hỏi, bình luận vào câu hỏi hoặc câu trả lời.'
  },
  {
    icon: Star,
    title: 'Bình chọn nội dung',
    description: 'Hệ thống cho phép bình chọn để làm nổi bật các nội dung hữu ích.'
  },
  {
    icon: Bell,
    title: 'Thông báo và tố cáo',
    description: 'Người dùng nhận thông báo khi có tương tác mới và có thể tố cáo nội dung chưa phù hợp.'
  },
  {
    icon: User,
    title: 'Trang cá nhân',
    description: 'Mỗi người dùng có thể theo dõi câu hỏi, câu trả lời và hoạt động của mình.'
  },
  {
    icon: DataBoard,
    title: 'Bảng quản trị',
    description: 'Quản trị viên có thể xem thống kê, quản lý người dùng và xử lý nội dung.'
  }
]

const topics = [
  '.NET',
  'Vue',
  'SQLite',
  'Dapper',
  'Cơ sở dữ liệu',
  'API',
  'JavaScript',
  'Lập trình Web',
  'Bảo mật',
  'Kinh nghiệm học tập'
]
</script>

<style scoped>
.about-page {
  padding-top: 24px;
  padding-bottom: 40px;
  font-family: inherit;
}

.about-hero {
  min-height: 320px;
  padding: 46px;
  border-radius: 22px;
  background:
    radial-gradient(circle at top right, rgba(30, 64, 175, 0.16), transparent 34%),
    linear-gradient(135deg, #ffffff 0%, #eef3ff 100%);
  border: 1px solid var(--forum-border);
  box-shadow: 0 18px 45px rgba(15, 23, 42, 0.08);
  display: flex;
  align-items: center;
}

.about-hero {
  overflow: hidden;
  border-radius: 22px;
  border: 1px solid var(--forum-border);
  background: #ffffff;
  box-shadow: 0 18px 45px rgba(15, 23, 42, 0.08);
}

.about-hero-image {
  display: block;
  width: 100%;
  height: auto;
  object-fit: cover;
}

.about-section {
  margin-top: 34px;
}

.section-heading {
  margin-bottom: 18px;
}

.section-heading h2 {
  margin: 0;
  color: var(--forum-text);
  font-family: inherit;
  font-size: 24px;
  line-height: 1.35;
  font-weight: 700;
  letter-spacing: normal;
}

.section-heading p {
  margin: 8px 0 0;
  max-width: 780px;
  color: var(--forum-muted);
  font-size: 14px;
  line-height: 1.6;
  font-weight: 400;
}

.about-card-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
}

.about-card {
  border-radius: 16px;
}

.about-card-icon {
  width: 46px;
  height: 46px;
  display: grid;
  place-items: center;
  border-radius: 14px;
  background: var(--forum-primary-soft);
  color: var(--forum-primary);
  margin-bottom: 14px;
}

.about-card-icon .el-icon {
  font-size: 23px;
}

.about-card h3 {
  margin: 0;
  color: var(--forum-text);
  font-family: inherit;
  font-size: 17px;
  line-height: 1.4;
  font-weight: 650;
  letter-spacing: normal;
}

.about-card p {
  margin: 9px 0 0;
  color: var(--forum-muted);
  font-size: 14px;
  line-height: 1.6;
  font-weight: 400;
}

.feature-list {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.feature-item {
  display: flex;
  gap: 13px;
  padding: 18px;
  border-radius: 16px;
  background: #ffffff;
  border: 1px solid var(--forum-border);
}

.feature-icon {
  width: 34px;
  height: 34px;
  display: grid;
  place-items: center;
  border-radius: 10px;
  background: var(--forum-primary-soft);
  color: var(--forum-primary);
  flex-shrink: 0;
}

.feature-icon .el-icon {
  font-size: 18px;
}

.feature-item h3 {
  margin: 0;
  color: var(--forum-text);
  font-family: inherit;
  font-size: 16px;
  line-height: 1.4;
  font-weight: 650;
  letter-spacing: normal;
}

.feature-item p {
  margin: 7px 0 0;
  color: var(--forum-muted);
  font-size: 14px;
  line-height: 1.55;
  font-weight: 400;
}

.tag-cloud {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  padding: 20px;
  border-radius: 16px;
  background: #ffffff;
  border: 1px solid var(--forum-border);
}

.about-final {
  margin-top: 34px;
  padding: 32px;
  border-radius: 20px;
  text-align: center;
  background: var(--forum-primary);
  color: #ffffff;
}

.about-final h2 {
  margin: 0;
  font-family: inherit;
  font-size: 25px;
  line-height: 1.35;
  font-weight: 700;
  letter-spacing: normal;
}

.about-final p {
  margin: 12px auto 22px;
  max-width: 700px;
  color: rgba(255, 255, 255, 0.9);
  line-height: 1.65;
  font-size: 15px;
  font-weight: 400;
}

.final-button {
  color: #ffffff;
  border-color: rgba(255, 255, 255, 0.8);
  background: transparent;
}

.final-button:hover {
  color: var(--forum-primary);
  background: #ffffff;
  border-color: #ffffff;
}

@media (max-width: 980px) {
  .about-hero {
    padding: 32px;
  }

  .about-hero h1 {
    font-size: 34px;
  }

  .about-card-grid,
  .feature-list {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .about-page {
    padding-top: 16px;
  }

  .about-hero {
    padding: 24px 18px;
    border-radius: 18px;
    min-height: auto;
  }

  .about-hero h1 {
    font-size: 28px;
  }

  .about-hero p {
    font-size: 15px;
  }

  .about-final {
    padding: 26px 18px;
  }

  .about-final h2 {
    font-size: 22px;
  }
  @media (max-width: 640px) {
    .about-hero {
        border-radius: 18px;
    }

    .about-hero-image {
        height: 180px;
    }
  }
}
</style>

