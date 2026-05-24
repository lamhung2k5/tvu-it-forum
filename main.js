import { createApp } from 'vue'
import App from './App.vue'

// BẮT BUỘC PHẢI CÓ 2 DÒNG NÀY ĐỂ HIỂN THỊ NÚT BẤM VÀ Ô NHẬP LIỆU:
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css' // <-- Dòng này quyết định giao diện đẹp/xấu

import * as ElementPlusIconsVue from '@element-plus/icons-vue'

const app = createApp(App)

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(ElementPlus)
app.mount('#app')