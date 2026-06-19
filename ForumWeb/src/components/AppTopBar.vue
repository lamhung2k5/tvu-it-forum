<template>
  <div class="app-topbar">
    <div class="topbar-inner">
      <div class="topbar-left">
        TRƯỜNG KỸ THUẬT VÀ CÔNG NGHỆ - KHOA CÔNG NGHỆ THÔNG TIN
      </div>

      <div class="topbar-socials">
        <a
          href="https://www.facebook.com/tvuCET"
          aria-label="Facebook"
          title="Facebook"
        >
          <svg viewBox="0 0 24 24">
            <path
              d="M22 12.06C22 6.48 17.52 2 11.94 2S2 6.48 2 12.06c0 5.02 3.66 9.18 8.44 9.94v-7.03H7.9v-2.91h2.54V9.84c0-2.5 1.49-3.88 3.77-3.88 1.09 0 2.23.19 2.23.19v2.45h-1.25c-1.24 0-1.63.77-1.63 1.56v1.9h2.78l-.44 2.91h-2.34V22C18.34 21.24 22 17.08 22 12.06Z"
            />
          </svg>
        </a>

        <a href="https://www.tiktok.com/@lamrobert2005?is_from_webapp=1&sender_device=pc" aria-label="TikTok" title="TikTok">
          <svg viewBox="0 0 24 24">
            <path
              d="M16.6 5.82c1.13.82 2.43 1.3 3.82 1.35v3.1a7.7 7.7 0 0 1-3.82-1.02v5.78c0 3.12-2.53 5.65-5.65 5.65S5.3 18.15 5.3 15.03s2.53-5.65 5.65-5.65c.36 0 .72.03 1.06.1v3.22a2.5 2.5 0 1 0 1.77 2.39V3.32h2.82v2.5Z"
            />
          </svg>
        </a>

        <a href="mailto:admin.cetforit@tvu.edu.vn?subject=Liên hệ CET FORIT" aria-label="Email" title="Email">
          <svg viewBox="0 0 24 24">
            <path
              d="M4 5h16c1.1 0 2 .9 2 2v10c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V7c0-1.1.9-2 2-2Zm8 7.4L4 8.1V17h16V8.1l-8 4.3Zm0-2.2L20 6H4l8 4.2Z"
            />
          </svg>
        </a>
      </div>
    </div>
  </div>
</template>

<style scoped>
.app-topbar {
  height: 34px;
  background: var(--forum-topbar);
  color: white;
  font-size: 13px;
}

.topbar-inner {
  width: 100%;
  height: 100%;
  box-sizing: border-box;

  /* Quan trọng: dùng padding giống AppHeader */
  padding: 0 24px;

  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
}

.topbar-left {
  font-weight: 700;
  letter-spacing: 0.2px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.topbar-socials {
  display: flex;
  align-items: center;
  gap: 14px;
  flex-shrink: 0;
}

.topbar-socials a {
  width: 24px;
  height: 24px;
  display: grid;
  place-items: center;
  border-radius: 50%;
  color: white;
  transition:
    background 0.2s ease,
    transform 0.2s ease;
}

.topbar-socials a:hover {
  background: rgba(255, 255, 255, 0.18);
  transform: translateY(-1px);
}

.topbar-socials svg {
  width: 18px;
  height: 18px;
  fill: currentColor;
}

/* Màn hình điện thoại thì ẩn luôn topbar */
@media (max-width: 768px) {
  .app-topbar {
    display: none;
  }
}
</style>
