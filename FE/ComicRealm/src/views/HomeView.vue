<script setup lang="ts">
import { RouterLink } from 'vue-router'

const featuredShelves = [
  {
    title: 'Cosmic Frontiers',
    description: 'Space opera, first contact, and impossible tech with big-screen energy.',
    meta: '18 series',
  },
  {
    title: 'Shadow Alley',
    description: 'Noir mysteries, vigilantes, and one-page cliffhangers after midnight.',
    meta: '12 series',
  },
  {
    title: 'Myth & Machine',
    description: 'Fantasy worlds where ancient magic collides with sharp modern design.',
    meta: '24 series',
  },
]

const highlights = [
  {
    title: 'Curated discovery',
    description: 'Find comics by mood, genre, art style, or the kind of story you want tonight.',
  },
  {
    title: 'Always in sync',
    description: 'Pick up exactly where you left off across your library, queue, and reading list.',
  },
  {
    title: 'Built for collectors',
    description: 'Track series, favorites, and special editions in one place without the clutter.',
  },
]
</script>

<template>
  <main class="home-shell">
    <section class="hero">
      <header class="topbar">
        <div>
          <p class="eyebrow">ComicRealm</p>
          <h1>One home for discovering, collecting, and reading comics.</h1>
        </div>

        <nav aria-label="Primary">
          <RouterLink to="/">Home</RouterLink>
          <RouterLink to="/about">About</RouterLink>
          <RouterLink class="login-link" to="/login">Login</RouterLink>
        </nav>
      </header>

      <div class="hero-grid">
        <div class="hero-copy">
          <p class="lede">
            Explore bold worlds, keep your shelves organized, and jump back into your next issue
            without digging through bookmarks.
          </p>

          <div class="cta-row">
            <RouterLink class="button button-primary" to="/about">Start exploring</RouterLink>
            <a class="button button-secondary" href="#featured">See featured shelves</a>
          </div>

          <dl class="stats">
            <div>
              <dt>Curated shelves</dt>
              <dd>120+</dd>
            </div>
            <div>
              <dt>Reading moods</dt>
              <dd>24</dd>
            </div>
            <div>
              <dt>Fresh drops</dt>
              <dd>Weekly</dd>
            </div>
          </dl>
        </div>

        <aside class="hero-panel" aria-label="Featured comic spotlight">
          <div class="spotlight-card">
            <p class="spotlight-label">Spotlight</p>
            <h2>Nova Rift</h2>
            <p>
              A high-contrast sci-fi saga about a lost archive, an unstable crew, and a city built
              on broken stars.
            </p>

            <div class="spotlight-meta">
              <span>Issue 07</span>
              <span>4.9 rating</span>
              <span>New arrival</span>
            </div>
          </div>
        </aside>
      </div>
    </section>

    <section class="section" id="featured">
      <div class="section-heading">
        <p class="eyebrow">Featured shelves</p>
        <h2>Collections with a distinct tone, not a generic grid.</h2>
      </div>

      <div class="shelf-grid">
        <article v-for="shelf in featuredShelves" :key="shelf.title" class="shelf-card">
          <p class="shelf-meta">{{ shelf.meta }}</p>
          <h3>{{ shelf.title }}</h3>
          <p>{{ shelf.description }}</p>
        </article>
      </div>
    </section>

    <section class="section layout-split">
      <div class="section-heading">
        <p class="eyebrow">Why ComicRealm</p>
        <h2>A cleaner way to follow stories you actually care about.</h2>
      </div>

      <div class="highlight-list">
        <article v-for="item in highlights" :key="item.title" class="highlight-card">
          <h3>{{ item.title }}</h3>
          <p>{{ item.description }}</p>
        </article>
      </div>
    </section>
  </main>
</template>
