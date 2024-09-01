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
import apiClient from '@/api/elysianClient'

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
    const response = await apiClient.getData('/api/GetProducts')
    
    if(!response.success){
        if(response.errorMessage){
            errorMessage.value = response.errorMessage
        } else {
            errorMessage.value = 'An error occurred. Please try again later.'
        }
        snackbar.value = true
    }

    cards.value = response.data
}

function viewCard(serialNumber: string) {
    router.push(`/search/${serialNumber}`)
}

async function editCard(productId: number) {
    dialogLoading.value = true
    dialog.value = true

    const response = await apiClient.getData(`/api/GetProduct?productId=${productId}`)
    if(!response.success){
        if(response.errorMessage){
            errorMessage.value = response.errorMessage
        } else {
            errorMessage.value = 'An error occurred. Please try again later.'
        }
        snackbar.value = true
    }

    const data = response.data
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
    const response = await apiClient.postData('/api/DeleteProduct', { productId })
    
    if(!response.success){
        if(response.errorMessage){
            errorMessage.value = response.errorMessage
        } else {
            errorMessage.value = 'An error occurred. Please try again later.'
        }
        snackbar.value = true
    }

    if (response.data.success) {
        await getCards()
    }
}

async function deleteCardImage(productImageId: number){
    dialogLoading.value = true
    const response = await apiClient.postData('/api/DeleteProductImage', { productImageId })
    
    if(!response.success){
        if(response.errorMessage){
            errorMessage.value = response.errorMessage
        } else {
            errorMessage.value = 'An error occurred. Please try again later.'
        }
        snackbar.value = true
    }
    dialogLoading.value = false

    if (!response.data.success) {
        errorMessage.value = 'Could not delete card'
        snackbar.value = true
    }
}

async function uploadFile(file: File) {
    const response = await apiClient.getData(`/api/GenerateSasToken?fileName=${file.name}`)
    
    if(!response.success){
        if(response.errorMessage){
            errorMessage.value = response.errorMessage
        } else {
            errorMessage.value = 'An error occurred. Please try again later.'
        }
        snackbar.value = true
        return null
    }

    const data = response.data

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
    const response = await apiClient.postData('/api/SaveProduct', {
        productId: form.productId,
        name: form.name,
        description: form.description,
        serialNumber: form.serialNumber,
        grade: form.grade,
        addImages: images
    })
    
    if(!response.success){
        if(response.errorMessage){
            errorMessage.value = response.errorMessage
        } else {
            errorMessage.value = 'An error occurred. Please try again later.'
        }
        snackbar.value = true
        dialogLoading.value = false
        return
    }
    selectedCard.value = undefined
    dialogLoading.value = false
    dialog.value = false

    if (response.data.productId) {
        await getCards()
    }
}
</script>
  