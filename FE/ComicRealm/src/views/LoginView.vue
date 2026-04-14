<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { roleLabels, setCurrentUserRole } from '../auth/roleSession'
import { UserRole } from '../models/enums/UserRole'

const router = useRouter()
const selectedRole = ref<UserRole>(UserRole.Friend)
const feedback = ref('')

const roleOptions = computed(() => [UserRole.SuperAdmin, UserRole.Admin, UserRole.Friend])

function submitLogin(event: Event): void {
  event.preventDefault()
  feedback.value = ''

  setCurrentUserRole(selectedRole.value)

  feedback.value = `Logged in as ${roleLabels[selectedRole.value]}.`
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
          <input type="email" placeholder="you@example.com" required />
        </label>

        <label>
          Password
          <input type="password" placeholder="Enter your password" required />
        </label>

        <label>
          Role
          <select v-model="selectedRole">
            <option v-for="role in roleOptions" :key="role" :value="role">
              {{ roleLabels[role] }}
            </option>
          </select>
        </label>

        <button type="submit">Login</button>
      </form>

      <p v-if="feedback" class="creation-feedback">{{ feedback }}</p>
    </section>
  </main>
</template>
