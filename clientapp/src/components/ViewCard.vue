<template>
    <v-sheet class="pb-3">
        <v-container v-if="loading" class="mt-3">
            <v-row align="center" justify="center">
                <v-col cols="auto" class="text-center">
                <v-progress-circular
                    indeterminate
                    :size="70"
                    :width="5"    
                ></v-progress-circular>
                <div class="mt-5 text-h5">
                    Searching, please wait..
                </div>
                </v-col>
            </v-row>
        </v-container>
        <v-container v-else>
            <template v-if="!cardDetails">
                <v-row>
                    <v-col>
                        <span v-html="findCardPage?.content.rendered">
                        </span>
                    </v-col>
                </v-row>
            </template>
            <template v-else>
                <v-row>
                    <v-col cols="12">
                        <v-card variant="text" rounded="0">
                            <v-card-title>
                                <h2>{{ cardDetails.name}}</h2>
                            </v-card-title>
                            <v-card-subtitle class="text-subtitle-1">
                                Serial Number: {{ cardDetails.serialNumber }}
                            </v-card-subtitle>
                            <v-card-subtitle class="text-subtitle-2">
                                Grade: {{ cardDetails.grade }}
                            </v-card-subtitle>
                            <v-card-text>
                                <p>{{ cardDetails.description }}</p>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col v-for="(url, index) in cardImages" :key="index" cols="6" md="3">    
                        <v-card :href="url" target="_blank">
                            <v-img 
                                :src="url" 
                                :height="!$vuetify.display.mobile ? '300px' : '150px'" 
                                cover
                                :aspect-ratio="1"
                                class="bg-grey-lighten-2">
                                <template v-slot:placeholder>
                                    <v-row
                                        align="center"
                                        class="fill-height ma-0"
                                        justify="center"
                                    >
                                        <v-progress-circular
                                        color="grey-lighten-5"
                                        indeterminate
                                        ></v-progress-circular>
                                    </v-row>
                                </template>
                            </v-img>
                        </v-card>
                    </v-col>
                </v-row>
            </template>
        </v-container>
    </v-sheet>
</template>
  
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAppStore } from "@/store/app";
import { storeToRefs } from 'pinia'

const appStore = useAppStore();
const { findCardPage } = storeToRefs(appStore)
  
const props = defineProps(['card', 'images', 'loading'])

const cardDetails = computed(() => props.card)
const cardImages = computed(() => props.images)
</script>
  