<template>
  <footer class="app-footer">
    <div class="footer-inner">
      <div class="footer-brand">
        <div class="footer-logo-box">
          <img
            src="/images/logo-khoa.png"
            alt="CET FORIT"
            class="footer-logo"
          />
        </div>

        <div class="footer-brand-text">
          <h3>CET FORIT</h3>
          <p>Diễn đàn hỏi đáp công nghệ thông tin dành cho sinh viên.</p>
        </div>
      </div>

      <div class="footer-section footer-links">
        <h4>LIÊN KẾT</h4>
        <RouterLink to="/">Trang chủ</RouterLink>
        <RouterLink to="/">Câu hỏi</RouterLink>
        <RouterLink to="/questions/create">Đặt câu hỏi</RouterLink>
        <RouterLink to="/dashboard">Dashboard</RouterLink>
      </div>

      <div class="footer-section footer-info">
        <h4>THÔNG TIN</h4>
        <p>Trường Kỹ thuật và Công nghệ</p>
        <p>Khoa Công nghệ thông tin</p>
        <p>© {{ currentYear }} CET FORIT</p>

        <div class="footer-socials">
          <a href="#" aria-label="Facebook" title="Facebook">
            <svg viewBox="0 0 24 24">
              <path
                d="M22 12.06C22 6.48 17.52 2 11.94 2S2 6.48 2 12.06c0 5.02 3.66 9.18 8.44 9.94v-7.03H7.9v-2.91h2.54V9.84c0-2.5 1.49-3.88 3.77-3.88 1.09 0 2.23.19 2.23.19v2.45h-1.25c-1.24 0-1.63.77-1.63 1.56v1.9h2.78l-.44 2.91h-2.34V22C18.34 21.24 22 17.08 22 12.06Z"
              />
            </svg>
          </a>

          <a href="#" aria-label="TikTok" title="TikTok">
            <svg viewBox="0 0 24 24">
              <path
                d="M16.6 5.82c1.13.82 2.43 1.3 3.82 1.35v3.1a7.7 7.7 0 0 1-3.82-1.02v5.78c0 3.12-2.53 5.65-5.65 5.65S5.3 18.15 5.3 15.03s2.53-5.65 5.65-5.65c.36 0 .72.03 1.06.1v3.22a2.5 2.5 0 1 0 1.77 2.39V3.32h2.82v2.5Z"
              />
            </svg>
          </a>

          <a href="#" aria-label="Email" title="Email">
            <svg viewBox="0 0 24 24">
              <path
                d="M4 5h16c1.1 0 2 .9 2 2v10c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V7c0-1.1.9-2 2-2Zm8 7.4L4 8.1V17h16V8.1l-8 4.3Zm0-2.2L20 6H4l8 4.2Z"
              />
            </svg>
          </a>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
const currentYear = new Date().getFullYear()
</script>

<style scoped>
.app-footer {
  background: var(--forum-topbar);
  color: #ffffff;
  padding: 30px 0;
  border-top: 1px solid rgba(255, 255, 255, 0.12);
}

.footer-inner {
  width: 100%;
  box-sizing: border-box;
  padding: 0 32px;

  display: grid;
  grid-template-columns: 1.5fr 1fr 1.2fr;
  gap: 56px;
  align-items: flex-start;
}

.footer-brand {
  display: flex;
  align-items: center;
  gap: 16px;
  min-width: 0;
}

.footer-logo-box {
  width: 78px;
  height: 54px;
  background: #ffffff;
  border-radius: 10px;
  padding: 5px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.footer-logo {
  width: 100%;
  height: 100%;
  object-fit: contain;
  display: block;
}

.footer-brand-text {
  min-width: 0;
}

.footer-brand h3,
.footer-section h4 {
  margin: 0 0 10px;
  color: #ffffff;
  font-weight: 700;
}

.footer-brand h3 {
  font-size: 20px;
}

.footer-brand p,
.footer-info p {
  margin: 5px 0;
  color: rgba(255, 255, 255, 0.82);
  line-height: 1.5;
}

.footer-section {
  min-width: 0;
}

.footer-links {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.footer-links a {
  width: fit-content;
  color: rgba(255, 255, 255, 0.82);
  font-weight: 500;
  text-decoration: none;
}

.footer-links a:hover {
  color: #ffffff;
  text-decoration: underline;
}

.footer-info {
  justify-self: end;
  text-align: left;
  min-width: 260px;
}

.footer-socials {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 14px;
}

.footer-socials a {
  width: 32px;
  height: 32px;
  display: grid;
  place-items: center;
  border-radius: 50%;
  color: #ffffff;
  background: rgba(255, 255, 255, 0.12);
  transition: background 0.2s ease, transform 0.2s ease;
}

.footer-socials a:hover {
  background: rgba(255, 255, 255, 0.22);
  transform: translateY(-2px);
}

.footer-socials svg {
  width: 18px;
  height: 18px;
  fill: currentColor;
}

@media (max-width: 980px) {
  .footer-inner {
    grid-template-columns: 1fr 1fr;
    gap: 32px;
  }

  .footer-info {
    justify-self: start;
    min-width: 0;
  }
}

@media (max-width: 640px) {
  .app-footer {
    padding: 26px 0;
  }

  .footer-inner {
    grid-template-columns: 1fr;
    gap: 24px;
    padding: 0 18px;
  }

  .footer-brand {
    align-items: flex-start;
  }

  .footer-logo-box {
    width: 66px;
    height: 46px;
  }

  .footer-info {
    justify-self: start;
  }
}
</style>