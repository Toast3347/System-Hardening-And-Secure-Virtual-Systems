<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { setAuthSession } from '../auth/roleSession'
import { ApiError, apiRequest } from '../api/client'

const router = useRouter()
const username = ref('')
const password = ref('')
const feedback = ref('')

interface LoginResponse {
  token: string
  role: string
  username: string
}

function submitLogin(event: Event): void {
  event.preventDefault()
  feedback.value = ''

  const trimmedUsername = username.value.trim()
  if (!trimmedUsername || !password.value) {
    feedback.value = 'Username and password are required.'
    return
  }

  void apiRequest<LoginResponse>('/api/Auth/login', {
    method: 'POST',
    body: JSON.stringify({
      username: trimmedUsername,
      password: password.value,
    }),
  })
    .then((response) => {
      setAuthSession(response.token)
      feedback.value = `Logged in as ${response.role}.`
      void router.push({ name: 'home' })
    })
    .catch((error: unknown) => {
      if (error instanceof ApiError) {
        feedback.value = error.message
        return
      }

      feedback.value = 'Login failed. Please try again.'
    })
}
</script>

<template>
  <main class="login-page">
    <section class="login-card">
      <p class="eyebrow">ComicRealm</p>
      <h1>Welcome back.</h1>
      <p class="lede">
        Sign in to continue your reading list, favorites, and comic shelves.
      </p>

      <form class="login-form" @submit="submitLogin">
        <label>
          Username
          <input v-model="username" type="text" placeholder="admin" required />
        </label>

        <label>
          Password
          <input v-model="password" type="password" placeholder="Enter your password" required />
        </label>

        <button type="submit">Login</button>
      </form>

      <p v-if="feedback" class="creation-feedback">{{ feedback }}</p>
    </section>
  </main>
</template>
