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
    const response = await fetch('/api/GetProducts', {
        method: 'get',
        headers: {
            '___tenant___': 'geekscloset'
        }
    })
    
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

function viewCard(serialNumber: string) {
    router.push(`/search/${serialNumber}`)
}

async function editCard(productId: number) {
    dialogLoading.value = true
    dialog.value = true

    const response = await fetch(`/api/GetProduct?productId=${productId}`, {
        method: 'get',
        headers: {
            '___tenant___': 'geekscloset'
        }
    })
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
        productId: data.product.productId,
        name: data.product.name,
        description: data.product.description,
        serialNumber: data.product.serialNumber,
        grade: data.product.grade,
        images: data.images
    }
    dialogLoading.value = false
}

async function deleteCard(productId: number) {
    const response = await fetch('/api/DeleteProduct', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json',
            '___tenant___': 'geekscloset'
        },
        body: JSON.stringify({ productId })
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

async function deleteCardImage(productImageId: number){
    dialogLoading.value = true
    const response = await fetch('/api/DeleteProductImage', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json',
            '___tenant___': 'geekscloset'
        },
        body: JSON.stringify({ productImageId })
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
    const response = await fetch(`/api/GenerateSasToken?fileName=${file.name}`, {
        method: 'get',
        headers: {
            '___tenant___': 'geekscloset'
        }
    })
    
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
    const response = await fetch('/api/SaveProduct', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json',
            '___tenant___': 'geekscloset'
        },
        body: JSON.stringify({
            productId: form.productId,
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
    if (data.productId) {
        await getCards()
    }
}
</script>
  