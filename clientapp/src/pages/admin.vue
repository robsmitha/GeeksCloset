<template>
    <CardList
        :items="cards"
        @view="viewCard"
        @edit="editCard"
        @delete="deleteCard"
        @create="dialog = true"
    />
    <SaveCardDialog
        :open="dialog"
        :loading="dialogLoading"
        :item="selectedCard"
        @close="dialog = false"
        @save="saveCard"
        @delete-image="deleteCardImage"
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
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { BlobServiceClient } from '@azure/storage-blob'

const router = useRouter()
const dialog = ref(false)
const dialogLoading = ref(false)
const selectedCard = ref<any | undefined>()
const snackbar = ref(false)
const errorMessage = ref('')

const cards = ref([])

onMounted(() => {
    getCards()
})

watch(dialog, (newValue) => {
    if(!newValue){
        selectedCard.value = undefined
    }
})

async function getCards(){
    const response = await fetch('/api/GetCards')
    
    if(!response.ok){
        switch (response.status) {
            default:
                errorMessage.value = 'Error getting cards. Please try again later.'
                break
        }
        snackbar.value = true
    }

    const data = await response.json()
    cards.value = data
}

function viewCard(cardId: number) {
    router.push(`/search/${cardId}`)
}

async function editCard(cardId: number) {
    dialogLoading.value = true
    dialog.value = true

    const response = await fetch(`/api/GetCard?cardId=${cardId}`)
    if(!response.ok){
        switch (response.status) {
            default:
                errorMessage.value = 'Error getting card. Please try again later.'
                break
        }
        snackbar.value = true
    }

    const data = await response.json()
    selectedCard.value = {
        cardId: data.card.CardId,
        name: data.card.Name,
        description: data.card.Description,
        serialNumber: data.card.SerialNumber,
        grade: data.card.Grade,
        images: data.images
    }
    dialogLoading.value = false
}

async function deleteCard(cardId: number) {
    const response = await fetch('/api/DeleteCard', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ cardId })
    })
    
    if(!response.ok){
        switch (response.status) {
            default:
                errorMessage.value = 'Could not delete card'
                break
        }
        snackbar.value = true
    }

    const data = await response.json()
    if (data.success) {
        await getCards()
    }
}

async function deleteCardImage(cardImageId: number){
    dialogLoading.value = true
    const response = await fetch('/api/DeleteCardImage', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ cardImageId })
    })
    
    if(!response.ok){
        switch (response.status) {
            default:
                errorMessage.value = 'Could not delete card image'
                break
        }
        snackbar.value = true
    }
    dialogLoading.value = false

    const data = await response.json()
    if (!data.success) {
        errorMessage.value = 'Could not delete card'
        snackbar.value = true
    }
}

async function uploadFile(file: File) {
    const response = await fetch(`/api/GenerateSasToken?fileName=${file.name}`)
    
    if(!response.ok){
        switch (response.status) {
            default:
                errorMessage.value = 'Could not upload file'
                break
        }
        snackbar.value = true
        return null
    }

    const data = await response.json()

    const blobServiceClient = new BlobServiceClient(`https://${data.accountName}.blob.core.windows.net?${data.sasToken}`);
    
    const containerClient = blobServiceClient.getContainerClient(data.containerName);
    
    const blockBlobClient = containerClient.getBlockBlobClient(data.blobName);
    await blockBlobClient.uploadData(file);
    return {
        fileName: file.name,
        fileSize: file.size,
        storageId: data.folderId
    }
}

async function saveCard(form: any) {
    dialogLoading.value = true
    const images: any = []
    for(let i = 0; i < form.files.length; i++) {
        const image = await uploadFile(form.files[i])
        image && images.push(image)
    }
    const response = await fetch('/api/SaveCard', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            cardId: form.cardId,
            name: form.name,
            description: form.description,
            serialNumber: form.serialNumber,
            grade: form.grade,
            addImages: images
        })
    })
    
    if(!response.ok){
        switch (response.status) {
            default:
                errorMessage.value = 'Could not save card'
                break
        }
        snackbar.value = true
        return
    }
    selectedCard.value = undefined
    dialogLoading.value = false
    dialog.value = false

    const data = await response.json()
    if (data.CardId) {
        await getCards()
    }
}
</script>
  