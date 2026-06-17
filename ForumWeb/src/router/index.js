import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated, isAdmin } from '../utils/auth'

const QuestionListView = () => import('../views/QuestionListView.vue')
const QuestionDetailView = () => import('../views/QuestionDetailView.vue')
const CreateQuestionView = () => import('../views/CreateQuestionView.vue')
const EditQuestionView = () => import('../views/EditQuestionView.vue')
const LoginView = () => import('../views/LoginView.vue')
const RegisterView = () => import('../views/RegisterView.vue')
const AdminView = () => import('../views/AdminView.vue')
const DashboardView = () => import('../views/DashboardView.vue')
const ProfileView = () => import('../views/ProfileView.vue')
const MyQuestionsView = () => import('../views/MyQuestionsView.vue')
const MyAnswersView = () => import('../views/MyAnswersView.vue')
const PublicProfileView = () => import('../views/PublicProfileView.vue')
const NotFoundView = () => import('../views/NotFoundView.vue')

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', name: 'questions', component: QuestionListView },
    { path: '/questions/create', name: 'question-create', component: CreateQuestionView, meta: { requiresAuth: true } },
    { path: '/questions/:id', name: 'question-detail', component: QuestionDetailView, props: true },
    { path: '/questions/:id/edit', name: 'question-edit', component: EditQuestionView, props: true, meta: { requiresAuth: true } },
    { path: '/dashboard', name: 'dashboard', component: DashboardView, meta: { requiresAuth: true } },
    { path: '/profile', name: 'profile', component: ProfileView, meta: { requiresAuth: true } },
    { path: '/my-questions', name: 'my-questions', component: MyQuestionsView, meta: { requiresAuth: true } },
    { path: '/my-answers', name: 'my-answers', component: MyAnswersView, meta: { requiresAuth: true } },
    { path: '/users/:id', name: 'public-profile', component: PublicProfileView, props: true },
    { path: '/login', name: 'login', component: LoginView, meta: { authLayout: true } },
    { path: '/register', name: 'register', component: RegisterView, meta: { authLayout: true } },
    { path: '/admin', name: 'admin', component: AdminView, meta: { requiresAuth: true, requiresAdmin: true } },
    { path: '/:pathMatch(.*)*', name: 'not-found', component: NotFoundView }
  ],
})

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !isAuthenticated()) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.meta.requiresAdmin && !isAdmin()) {
    return { name: 'questions' }
  }
})

export default router
