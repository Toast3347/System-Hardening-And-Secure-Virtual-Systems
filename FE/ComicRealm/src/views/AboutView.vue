<script setup lang="ts">
import { RouterLink } from 'vue-router'

const pillars = [
  {
    title: 'Curated by mood',
    description: 'Discover stories based on tone, pace, art style, or the kind of escape you want.',
  },
  {
    title: 'Built for collectors',
    description: 'Track favorite runs, special editions, and long-running arcs in one place.',
  },
  {
    title: 'Easy to return to',
    description: 'Jump back into the next page, issue, or shelf without hunting through tabs.',
  },
]

const milestones = [
  {
    year: '01',
    title: 'Discover',
    description: 'Explore shelves that feel hand-picked instead of algorithmically flattened.',
  },
  {
    year: '02',
    title: 'Collect',
    description: 'Keep the comics that matter to you organized with clear ownership and context.',
  },
  {
    year: '03',
    title: 'Return',
    description: 'Pick up your reading session exactly where it left off, across every device.',
  },
]
</script>

<template>
  <main class="about-shell">
    <section class="about-hero">
      <div class="about-kicker">About ComicRealm</div>
      <div class="about-hero-grid">
        <div class="about-copy">
          <p class="eyebrow">Our story</p>
          <h1>We make comic discovery feel like opening a great back issue bin.</h1>
          <p class="about-intro">
            ComicRealm is designed for readers who want more than a plain catalog. It is a place
            to browse with intent, follow the books you love, and keep your collection feeling
            alive instead of buried.
          </p>

          <div class="about-actions">
            <RouterLink class="button button-primary" to="/">Back to home</RouterLink>
            <RouterLink class="button button-secondary" to="/login">Login</RouterLink>
          </div>
        </div>

        <aside class="about-card about-quote">
          <p class="quote-mark">“</p>
          <p>
            We believe comic readers deserve a digital home with the same energy as a great local
            shop: curated, welcoming, and easy to get lost in.
          </p>
          <p class="quote-source">The ComicRealm team</p>
        </aside>
      </div>
    </section>

    <section class="about-section">
      <div class="section-heading">
        <p class="eyebrow">What we value</p>
        <h2>A sharper reading experience starts with a clearer point of view.</h2>
      </div>

      <div class="about-grid">
        <article v-for="pillar in pillars" :key="pillar.title" class="about-card">
          <h3>{{ pillar.title }}</h3>
          <p>{{ pillar.description }}</p>
        </article>
      </div>
    </section>

    <section class="about-section about-split">
      <div class="section-heading">
        <p class="eyebrow">How it works</p>
        <h2>From discovery to reading, the flow stays simple.</h2>
      </div>

      <div class="timeline">
        <article v-for="step in milestones" :key="step.title" class="timeline-item">
          <div class="timeline-year">{{ step.year }}</div>
          <div>
            <h3>{{ step.title }}</h3>
            <p>{{ step.description }}</p>
          </div>
        </article>
      </div>
    </section>
  </main>
</template>
