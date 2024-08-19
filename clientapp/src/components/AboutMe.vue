<template>
  <v-sheet color="grey-lighten-4" class="pt-3">
    <v-container>
      <v-row>
        <v-col md="8" cols="12">
          <template v-if="!home">
            <v-skeleton-loader
              type="article"
              color="transparent"
            ></v-skeleton-loader>
            <v-skeleton-loader
              v-for="i in 2"
              :key="i"
              type="paragraph"
              color="transparent"
            ></v-skeleton-loader>
          </template>
          <template v-else>
            <v-card color="transparent" flat>
              <v-card-title>Geeks Closet</v-card-title>
              <v-card-subtitle v-html="home.title.rendered"></v-card-subtitle>
              <v-card-text v-html="home.content.rendered"></v-card-text>
              <v-card-actions>
                <v-btn to="/search">Find Card by Serial Number</v-btn>
              </v-card-actions>
            </v-card>
          </template>
        </v-col>
        <v-col md="4" cols="12">
          <template v-if="!about">
            <v-skeleton-loader
              type="article"
              color="transparent"
            ></v-skeleton-loader>
            <v-skeleton-loader
              v-for="i in 2"
              :key="i"
              type="paragraph"
              color="transparent"
            ></v-skeleton-loader>
          </template>
          <template v-else>
            <v-card color="transparent" flat>
              <v-card-text v-html="about.content.rendered"></v-card-text>
            </v-card>
          </template>
        </v-col>
      </v-row>
    </v-container>
  </v-sheet>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAppStore } from "@/store/app";
import { storeToRefs } from 'pinia'

const emit = defineEmits(['search', 'clear'])
const appStore = useAppStore();
const { homePage, aboutPage } = storeToRefs(appStore)

const home = ref(homePage)
const about = ref(aboutPage)
</script>
