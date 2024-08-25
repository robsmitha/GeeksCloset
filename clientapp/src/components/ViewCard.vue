<template>
    <v-container>
        <template v-if="!cardDetails">
            <v-row>
                <v-col>
                    <span class="d-block">
                        Use this page to look up a card by serial number.
                    </span>
                    
                </v-col>
            </v-row>
        </template>
        <template v-else>
            <v-row>
                <v-col>
                    <span class="d-block">
                        {{ cardDetails?.name }}
                    </span>
                    <span class="d-block text-grey-darken-1">#{{ cardDetails?.serialNumber }}</span>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    
                    <v-chip><span>Grade: </span>{{ cardDetails?.grade }}</v-chip>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <span class="d-block mb-3">
                        Notes:
                    </span>
                    <span class="d-block">
                        {{ cardDetails?.description }}
                    </span>
                </v-col>
            </v-row>
            <v-row>
                <v-col 
                    v-for="url in cardImages" :key="url" 
                    class="d-flex child-flex"
                    md="4"
                    cols="12"
                >
                    <v-card class="w-100" :href="url" target="_blank">
                        <v-img 
                            :src="url" 
                            height="300px" 
                            cover
                            :aspect-ratio="1"
                            class="bg-grey-lighten-2"
                        >
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
            <v-row>
                <v-col>
                    <span class="text-grey-darken-1">Click an image to download</span>
                </v-col>
            </v-row>
        </template>
    </v-container>
</template>
  
<script setup lang="ts">
import { ref, computed } from 'vue'
  
const props = defineProps(['card', 'images', 'loading'])

const cardDetails = computed(() => props.card)
const cardImages = computed(() => props.images)
</script>
  