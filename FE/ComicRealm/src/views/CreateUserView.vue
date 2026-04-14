<script setup lang="ts">
import { computed, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { UserRole } from '../models/enums/UserRole'
import {
  canAccessCreateUserPage,
  canCreateRole,
  getCreatableRolesByActor,
  getCurrentUserRole,
} from '../auth/roleSession'

const actorRole = ref<UserRole | null>(getCurrentUserRole())

const creatableRoles = computed(() => getCreatableRolesByActor(actorRole.value))
const hasAccess = computed(() => canAccessCreateUserPage(actorRole.value))

const email = ref('')
const password = ref('')
const targetRole = ref<UserRole | null>(creatableRoles.value[0] ?? null)
const feedback = ref('')

const roleLabels: Record<UserRole, string> = {
  [UserRole.SuperAdmin]: 'SuperAdmin',
  [UserRole.Admin]: 'Admin',
  [UserRole.Friend]: 'Friend',
}

const actorRuleText = computed(() => {
  if (actorRole.value === UserRole.SuperAdmin) {
    return 'As SuperAdmin, you can only create Admin users.'
  }

  if (actorRole.value === UserRole.Admin) {
    return 'As Admin, you can create Admin and Friend users.'
  }

  return 'Only Admin and SuperAdmin users can create users.'
})

function submitCreateUser(event: Event): void {
  event.preventDefault()
  feedback.value = ''

  if (!hasAccess.value || targetRole.value === null) {
    return
  }

  if (!canCreateRole(actorRole.value, targetRole.value)) {
    feedback.value = 'You are not allowed to create that role.'
    return
  }

  const trimmedEmail = email.value.trim()
  if (!trimmedEmail || !password.value) {
    feedback.value = 'Email and password are required.'
    return
  }

  feedback.value = `User ${trimmedEmail} prepared as ${roleLabels[targetRole.value]}. Connect this form to your backend endpoint to persist.`
  email.value = ''
  password.value = ''
  targetRole.value = creatableRoles.value[0] ?? null
}
</script>

<template>
  <main class="create-user-page">
    <section class="create-user-shell">
      <header class="create-user-header">
        <p class="eyebrow">Restricted Area</p>
        <h1>Create a user account</h1>
        <p>Admins and superadmins can provision new users based on role policy.</p>
      </header>

      <div class="role-rules">
        <p>{{ actorRuleText }}</p>
      </div>

      <div v-if="!hasAccess" class="access-denied">
        Access denied. Sign in as an Admin or SuperAdmin to use this page.
      </div>

      <form v-else class="create-user-form" @submit="submitCreateUser">
        <label>
          Email
          <input v-model="email" type="email" placeholder="new.user@example.com" />
        </label>

        <label>
          Password
          <input v-model="password" type="password" placeholder="Set temporary password" />
        </label>

        <label>
          Role
          <select v-model="targetRole">
            <option v-for="role in creatableRoles" :key="role" :value="role">
              {{ roleLabels[role] }}
            </option>
          </select>
        </label>

        <div class="create-user-actions">
          <button class="button button-primary" type="submit">Create user</button>
          <RouterLink class="button button-secondary" to="/">Back home</RouterLink>
        </div>
      </form>

      <p v-if="feedback" class="creation-feedback">
        {{ feedback }}
      </p>
    </section>
  </main>
</template>
