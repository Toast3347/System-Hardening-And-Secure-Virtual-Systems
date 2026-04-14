<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function submitLogin(event: Event): void {
  event.preventDefault()
  void router.push({ name: 'home' })
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
          Email
          <input type="email" placeholder="you@example.com" />
        </label>

        <label>
          Password
          <input type="password" placeholder="Enter your password" />
        </label>

        <button type="submit">Login</button>
      </form>
    </section>
  </main>
</template>
