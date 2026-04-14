<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { ApiError, apiRequest } from '../api/client'
import {
  canAccessCreateUserPage,
  canManageComics,
  canViewComics,
  clearCurrentUserRole,
  getCurrentUserId,
  getCurrentUserRole,
  isSessionExpired,
  roleLabels,
} from '../auth/roleSession'
import { UserRole } from '../models/enums/UserRole'

interface ComicItem {
  comicId: number
  serie: string
  number: string
  title: string
}

interface ComicResponse {
  comicId: number
  serie: string
  number: string
  title: string
}

const router = useRouter()
const currentRole = ref<UserRole | null>(getCurrentUserRole())

const canCreateUsers = computed(() => canAccessCreateUserPage(currentRole.value))
const canSeeComics = computed(() => canViewComics(currentRole.value))
const canEditComics = computed(() => canManageComics(currentRole.value))
const isLoggedIn = computed(() => currentRole.value !== null)
const roleSummary = computed(() => {
  if (currentRole.value === null) {
    return 'No active role'
  }

  return roleLabels[currentRole.value]
})

const comics = ref<ComicItem[]>([
])
const loadingComics = ref(false)

function refreshCurrentRole(): void {
  if (isSessionExpired()) {
    clearCurrentUserRole()
    currentRole.value = null
    return
  }

  currentRole.value = getCurrentUserRole()
}

function loadComics(): void {
  if (!canSeeComics.value) {
    comics.value = []
    return
  }

  loadingComics.value = true
  feedback.value = ''

  void apiRequest<ComicResponse[]>('/api/Comics')
    .then((response) => {
      comics.value = response.map((item) => ({
        comicId: item.comicId,
        serie: item.serie,
        number: item.number,
        title: item.title,
      }))
    })
    .catch((error: unknown) => {
      if (error instanceof ApiError) {
        feedback.value = error.message
      } else {
        feedback.value = 'Unable to load comics right now.'
      }
    })
    .finally(() => {
      loadingComics.value = false
    })
}

onMounted(() => {
  refreshCurrentRole()
  loadComics()
})

const newSerie = ref('')
const newNumber = ref('')
const newTitle = ref('')
const editingComicId = ref<number | null>(null)
const editSerie = ref('')
const editNumber = ref('')
const editTitle = ref('')
const feedback = ref('')

function resetCreateForm(): void {
  newSerie.value = ''
  newNumber.value = ''
  newTitle.value = ''
}

function createComic(event: Event): void {
  event.preventDefault()
  feedback.value = ''

  if (!canEditComics.value) {
    feedback.value = 'Only Admin users can create comics.'
    return
  }

  const serie = newSerie.value.trim()
  const number = newNumber.value.trim()
  const title = newTitle.value.trim()

  if (!serie || !number || !title) {
    feedback.value = 'Serie, number, and title are required.'
    return
  }

  const actorUserId = getCurrentUserId()
  if (actorUserId === null) {
    feedback.value = 'Session expired. Please login again.'
    return
  }

  void apiRequest('/api/Comics', {
    method: 'POST',
    body: JSON.stringify({
      serie,
      number,
      title,
      createdBy: actorUserId,
    }),
  })
    .then(() => {
      feedback.value = `Comic ${serie} #${number} was added.`
      resetCreateForm()
      loadComics()
    })
    .catch((error: unknown) => {
      if (error instanceof ApiError) {
        feedback.value = error.message
        return
      }

      feedback.value = 'Unable to add comic right now.'
    })
}

function beginEdit(comic: ComicItem): void {
  if (!canEditComics.value) {
    return
  }

  editingComicId.value = comic.comicId
  editSerie.value = comic.serie
  editNumber.value = comic.number
  editTitle.value = comic.title
  feedback.value = ''
}

function saveEdit(event: Event): void {
  event.preventDefault()
  feedback.value = ''

  if (!canEditComics.value || editingComicId.value === null) {
    feedback.value = 'Only Admin users can update comics.'
    return
  }

  const serie = editSerie.value.trim()
  const number = editNumber.value.trim()
  const title = editTitle.value.trim()

  if (!serie || !number || !title) {
    feedback.value = 'Serie, number, and title are required.'
    return
  }

  const comicId = editingComicId.value
  void apiRequest(`/api/Comics/${comicId}`, {
    method: 'PUT',
    body: JSON.stringify({
      serie,
      number,
      title,
    }),
  })
    .then(() => {
      feedback.value = 'Comic updated.'
      editingComicId.value = null
      loadComics()
    })
    .catch((error: unknown) => {
      if (error instanceof ApiError) {
        feedback.value = error.message
        return
      }

      feedback.value = 'Unable to update comic right now.'
    })
}

function cancelEdit(): void {
  editingComicId.value = null
  feedback.value = ''
}

function removeComic(comicId: number): void {
  feedback.value = ''

  if (!canEditComics.value) {
    feedback.value = 'Only Admin users can delete comics.'
    return
  }

  void apiRequest(`/api/Comics/${comicId}`, {
    method: 'DELETE',
  })
    .then(() => {
      if (editingComicId.value === comicId) {
        editingComicId.value = null
      }

      feedback.value = 'Comic deleted.'
      loadComics()
    })
    .catch((error: unknown) => {
      if (error instanceof ApiError) {
        feedback.value = error.message
        return
      }

      feedback.value = 'Unable to delete comic right now.'
    })
}

function logout(): void {
  clearCurrentUserRole()
  currentRole.value = null
  void router.push({ name: 'login' })
}
</script>

<template>
  <main class="home-shell">
    <section class="hero">
      <header class="topbar">
        <div>
          <p class="eyebrow">ComicRealm</p>
          <h1>Role-based comic portal.</h1>
          <p class="lede">Signed in as: {{ roleSummary }}</p>
        </div>

        <nav aria-label="Primary">
          <RouterLink to="/">Home</RouterLink>
          <RouterLink to="/about">About</RouterLink>
          <RouterLink v-if="canCreateUsers" to="/admin/users/create">Create User</RouterLink>
          <RouterLink v-if="!isLoggedIn" class="login-link" to="/login">Login</RouterLink>
          <button v-else type="button" class="topbar-logout" @click="logout">Logout</button>
        </nav>
      </header>

      <div v-if="canSeeComics" class="hero-grid">
        <div class="hero-copy">
          <p class="lede">
            Friends can view comics. Admins can add, edit, and delete comics. SuperAdmins can
            focus on user administration.
          </p>

          <div class="cta-row">
            <RouterLink class="button button-secondary" to="/about">About</RouterLink>
            <RouterLink v-if="canCreateUsers" class="button button-primary" to="/admin/users/create">
              Create users
            </RouterLink>
          </div>

          <div v-if="!canEditComics" class="role-rules">
            <p>Comic management is restricted to Admin role.</p>
          </div>

          <div v-if="loadingComics" class="role-rules">
            <p>Loading comics...</p>
          </div>
        </div>

        <aside class="hero-panel" aria-label="Comic permissions">
          <div class="spotlight-card">
            <p class="spotlight-label">RBAC</p>
            <h2>Comics</h2>
            <p>
              Read access: SuperAdmin, Admin, Friend. Write access: Admin only.
            </p>
          </div>
        </aside>
      </div>

      <div v-else class="access-denied">
        You do not have permission to view comics.
      </div>
    </section>

    <section v-if="canSeeComics" class="section" id="featured">
      <div class="section-heading">
        <p class="eyebrow">Comics</p>
        <h2>Current comic catalog</h2>
      </div>

      <div class="shelf-grid">
        <article v-for="comic in comics" :key="comic.comicId" class="shelf-card">
          <p class="shelf-meta">Issue {{ comic.number }}</p>
          <h3>{{ comic.serie }}</h3>
          <p>{{ comic.title }}</p>

          <div v-if="canEditComics" class="comic-actions">
            <button class="button button-secondary" type="button" @click="beginEdit(comic)">
              Edit
            </button>
            <button class="button button-secondary" type="button" @click="removeComic(comic.comicId)">
              Delete
            </button>
          </div>
        </article>
      </div>
    </section>

    <section v-if="canSeeComics && canEditComics" class="section layout-split">
      <div class="section-heading">
        <p class="eyebrow">Admin only</p>
        <h2>Manage comic catalog</h2>
      </div>

      <div class="highlight-list">
        <article class="highlight-card">
          <h3>Add comic</h3>
          <form class="create-user-form" @submit="createComic">
            <label>
              Serie
              <input v-model="newSerie" type="text" placeholder="Serie name" />
            </label>
            <label>
              Number
              <input v-model="newNumber" type="text" placeholder="Issue number" />
            </label>
            <label>
              Title
              <input v-model="newTitle" type="text" placeholder="Issue title" />
            </label>
            <button class="button button-primary" type="submit">Add comic</button>
          </form>
        </article>

        <article class="highlight-card">
          <h3>Edit comic</h3>
          <div v-if="editingComicId === null" class="role-rules">
            <p>Select a comic and click Edit to modify it.</p>
          </div>

          <form v-else class="create-user-form" @submit="saveEdit">
            <label>
              Serie
              <input v-model="editSerie" type="text" placeholder="Serie name" />
            </label>
            <label>
              Number
              <input v-model="editNumber" type="text" placeholder="Issue number" />
            </label>
            <label>
              Title
              <input v-model="editTitle" type="text" placeholder="Issue title" />
            </label>
            <div class="create-user-actions">
              <button class="button button-primary" type="submit">Save changes</button>
              <button class="button button-secondary" type="button" @click="cancelEdit">
                Cancel
              </button>
            </div>
          </form>
        </article>
      </div>
    </section>

    <p v-if="feedback" class="creation-feedback">{{ feedback }}</p>
  </main>
</template>
