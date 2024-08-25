<template>
    <FindCard 
        :term="term"
        :loading="loading"
        @input="term = $event"
        @search="searchBySerialNumber" 
    />
    <ViewCard 
        :card="card" 
        :images="sasUris" 
        :loading="loading" 
    />
    <v-snackbar
        v-model="snackbar"
    >
      {{ errorMessage }}

      <template v-slot:actions>
        <v-btn
          color="white"
          variant="text"
          @click="snackbar = false"
        >
          Close
        </v-btn>
      </template>
    </v-snackbar>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const props = defineProps({
  serialNumber: { type: String }
})

const term = ref(props.serialNumber)
const sasUris = ref([])
const card = ref()
const loading = ref(false)
const snackbar = ref(false)
const errorMessage = ref('')

onMounted(async () => {
    if(props?.serialNumber != null && props.serialNumber.length > 0){
        await searchBySerialNumber()
    }
})

async function searchBySerialNumber(){
    if(props.serialNumber !== term.value){
        router.replace({ path: '/search' })
    }
    loading.value = true
    const response = await fetch(`/api/GetProductBySerialNumber?serialNumber=${term.value}`, {
        method: 'get',
        headers: {
            '___tenant___': 'geekscloset'
        }
    })
    if(!response.ok){
        switch (response.status) {
            case 400:
                errorMessage.value = 'Please provide a serial number to search.'
                break
            case 404:
                errorMessage.value = 'A GCG card was not found with the provided serial number.'
                break
            default:
                errorMessage.value = 'An error occurred searching. Please try again later.'
                break
        }
        snackbar.value = true
    }

    const data = await response.json()
    card.value = data.product
    sasUris.value = data.imageUris
    loading.value = false
}
</script>
  