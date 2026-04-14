import { createRouter, createWebHistory } from 'vue-router'
import { UserRole } from '../models/enums/UserRole'
import { getCurrentUserRole } from '../auth/roleSession'
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
        allowedRoles: [UserRole.SuperAdmin, UserRole.Admin],
      },
    },
  ],
})

router.beforeEach((to) => {
  const allowedRoles = to.meta.allowedRoles as UserRole[] | undefined

  if (!allowedRoles || allowedRoles.length === 0) {
    return true
  }

  const currentRole = getCurrentUserRole()

  if (currentRole === null) {
    return { name: 'login' }
  }

  if (!allowedRoles.includes(currentRole)) {
    return { name: 'home' }
  }

  return true
})

export default router
