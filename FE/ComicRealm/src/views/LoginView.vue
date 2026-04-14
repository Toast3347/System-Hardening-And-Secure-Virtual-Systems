<template>
  <main class="login-page">
    <section class="login-card">
      <p class="eyebrow">ComicRealm</p>
      <h1>Welcome back.</h1>
      <p class="lede">
        Sign in to continue your reading list, favorites, and comic shelves.
      </p>

      <form class="login-form">
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

<style scoped>
.login-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 2rem;
}

.login-card {
  width: min(100%, 28rem);
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 28px;
  background: rgba(12, 17, 31, 0.84);
  box-shadow: 0 30px 80px rgba(5, 10, 22, 0.35);
}

.eyebrow {
  margin-bottom: 0.75rem;
  color: #f4c66b;
  text-transform: uppercase;
  letter-spacing: 0.18em;
  font-size: 0.75rem;
  font-weight: 700;
}

h1 {
  margin-bottom: 0.75rem;
  color: #f8fbff;
  font-family: 'Georgia', 'Times New Roman', serif;
  font-size: clamp(2.4rem, 4vw, 3.4rem);
  line-height: 1.05;
}

.lede {
  margin-bottom: 1.5rem;
  color: rgba(238, 243, 255, 0.78);
}

.login-form {
  display: grid;
  gap: 1rem;
}

label {
  display: grid;
  gap: 0.45rem;
  color: #eef2ff;
  font-weight: 700;
}

input {
  min-height: 3rem;
  padding: 0.85rem 1rem;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.05);
  color: #f8fbff;
}

input::placeholder {
  color: rgba(232, 239, 255, 0.42);
}

button {
  min-height: 3rem;
  margin-top: 0.25rem;
  border: 0;
  border-radius: 999px;
  background: linear-gradient(135deg, #ffe08f, #f7b95e);
  color: #11151f;
  font-weight: 800;
  cursor: pointer;
}
</style>
