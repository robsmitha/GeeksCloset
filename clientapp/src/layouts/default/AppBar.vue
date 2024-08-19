<template>
    <v-app-bar 
      :color="!drawer && transparency ? 'transparent' : 'black'" 
      :class="{
        'text-white': !drawer && transparency
      }" 
      flat
      fixed
    >
      <template #prepend>
        <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
      </template>
      <v-app-bar-title class="text-uppercase font-weight-medium">
        <span v-if="!$vuetify.display.mobile">
          Geeks Closet Grading
        </span>
        <span v-else>
          GCG
        </span>
      </v-app-bar-title>
      <v-spacer />
      <v-btn href="tel:850-371-5472" icon>
        <v-icon>mdi-phone</v-icon>
      </v-btn>
    </v-app-bar>

    <v-navigation-drawer
        v-model="drawer"
        temporary
    >
      <v-list density="compact" nav>
        <v-list-item prepend-icon="mdi-home-roof" title="Home" value="home" to="/"></v-list-item>
        <v-list-item prepend-icon="mdi-magnify" title="Find Card" value="search" to="/search"></v-list-item>

        <v-list-item subtitle="External Links"></v-list-item>
        <v-list-item prepend-icon="mdi-link-variant" title="Socials & Podcast" href="https://linktr.ee/geekscloset" target="_blank"></v-list-item>
        <v-list-item prepend-icon="mdi-shopping-outline" title="Shop Here" href="https://www.ebay.com/str/bemysalesman" target="_blank"></v-list-item>

        <v-list-item :subtitle="auth.signedIn ? auth.userDetails : 'Administration'"></v-list-item>
        <template v-if="auth.signedIn">
          <v-list-item prepend-icon="mdi-cards-outline" title="GCG Cards" href="/admin"></v-list-item>
          <v-list-item prepend-icon="mdi-logout" title="Sign out" href="/.auth/logout"></v-list-item>
        </template>
        <v-list-item v-else prepend-icon="mdi-login" title="Sign in" href="/.auth/login/aad"></v-list-item>
      </v-list>
    </v-navigation-drawer>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useAppStore } from "@/store/app"
import { useAuthStore } from "@/store/auth"
import { useRoute } from 'vue-router'
const route = useRoute()

const store = useAppStore()
store.fetchContent()

const auth = useAuthStore()
auth.fetchAuth()

const drawer = ref(false)
const transparency = ref(false)

watch(route, (newVal) => {
    transparency.value = newVal.path === '/'
}, { immediate: true })

onMounted(() =>{
  window.addEventListener('scroll', () => {
    transparency.value = route.path === '/' && window.scrollY <= 75
  })
})
</script>
