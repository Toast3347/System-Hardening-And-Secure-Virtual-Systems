import { createRouter, createWebHistory } from 'vue-router'
import { UserRole } from '../models/enums/UserRole'
import { clearCurrentUserRole, getCurrentUserRole, isSessionExpired } from '../auth/roleSession'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
      meta: {
        requiresAuth: true,
      },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
    },
    {
      path: '/admin/users/create',
      name: 'create-user',
      component: () => import('../views/CreateUserView.vue'),
      meta: {
        requiresAuth: true,
        allowedRoles: [UserRole.SuperAdmin, UserRole.Admin],
      },
    },
  ],
})

router.beforeEach((to) => {
  const requiresAuth = Boolean(to.meta.requiresAuth)
  const allowedRoles = to.meta.allowedRoles as UserRole[] | undefined
  let currentRole = getCurrentUserRole()

  if (currentRole !== null && isSessionExpired()) {
    clearCurrentUserRole()
    currentRole = null
  }

  if (to.name === 'login' && currentRole !== null) {
    return { name: 'home' }
  }

  if (requiresAuth && currentRole === null) {
    return { name: 'login' }
  }

  if (!requiresAuth || !allowedRoles || allowedRoles.length === 0) {
    return true
  }

  if (!allowedRoles.includes(currentRole)) {
    return { name: 'home' }
  }

  return true
})

export default router
