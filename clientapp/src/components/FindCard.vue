<template>
    <v-sheet class="pt-3">
      <v-container>
        <v-row>
            <v-col>
                <div class="text-h5 font-weight-medium">
                    Find Your Card
                </div>
                <span class="text-grey-darken-1">
                    Search for a card in the GCG database using the issued Serial Number
                </span>
            </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <v-text-field 
                v-model="search"
                append-inner-icon="mdi-magnify"
                label="Search"
                hint="Enter your card's serial number" 
                variant="outlined"
                persistent-hint
                :readonly="loading"
                :loading="loading"
                clearable
                @click:clear="emit('clear')"
                @click:append-inner="emit('search', search)"
                @keypress.enter="emit('search', search)"
                rounded="0"
            ></v-text-field>

            <v-btn
              class="mt-3"
              variant="tonal"
              :disabled="loading"
              :block="$vuetify.display.mobile"
              @click="emit('search', search)">
              Search
            </v-btn>
          </v-col>
        </v-row>
      </v-container>
    </v-sheet>
  </template>
  
  <script setup lang="ts">
  import { computed, watch } from 'vue'
  
  const props = defineProps(['loading', 'term'])
  const emit = defineEmits(['search', 'clear', 'input'])
  
  const search = computed({
    get() {
      return props.term
    },
    set(newValue) {
      emit('input', newValue)
    }
  })
  
  watch(search, async (newSearch: string) => {
      if(!newSearch || newSearch.length === 0){
          emit('clear')
      }
  })
  </script>
  