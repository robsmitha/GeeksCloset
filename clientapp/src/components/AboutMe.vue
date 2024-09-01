<template>
  <v-sheet class="pt-3">
    <v-container>
      <v-row>
        <v-col md="8" cols="12">
          <template v-if="!homePage">
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
              <v-card-subtitle>
                <span v-html="homePage.title.rendered"></span>
              </v-card-subtitle>
              <v-card-text>
                <span v-html="homePage.content.rendered"></span>
              </v-card-text>
              <v-card-actions>
                <v-btn to="/search" variant="tonal" :block="$vuetify.display.mobile">Search by Serial Number</v-btn>
              </v-card-actions>
            </v-card>
          </template>
        </v-col>
        <v-col md="4" cols="12">
          <template v-if="!aboutPage">
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
              <v-card-text>
                <span v-html="aboutPage.content.rendered"></span>
              </v-card-text>
            </v-card>
          </template>
        </v-col>
      </v-row>
    </v-container>
  </v-sheet>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAppStore } from "@/store/app";
import { storeToRefs } from 'pinia'

const emit = defineEmits(['search', 'clear'])
const appStore = useAppStore();
const { homePage, aboutPage } = storeToRefs(appStore)

</script>
