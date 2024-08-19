<template>
  <v-parallax 
      :src="props.src" 
      :height="props.height"
      class="mt-n16"
  >
      <v-overlay v-model="display" contained class="align-center justify-center text-white">
          <div>
            
              <v-avatar size="350" class="d-flex">
                <v-img src="https://smitha-cdn.s3.us-east-2.amazonaws.com/GeeksCloset/e067fd3cbadf49d3ab0eda8ca4f230a3.png" />
              </v-avatar>
              <div class="text-center">
                <v-btn v-for="b in actions" :key="b.text" variant="text" :to="b.to" :href="b.href" :target="b.href ? '_blank': ''" :prepend-icon="b.icon">{{ b.text }}</v-btn>
              </div>
          </div>
      </v-overlay>
  </v-parallax>
</template>

<script setup lang="ts">
type HeroAction = {
  text: string,
  to: string | undefined,
  href: string | undefined,
  icon: string | undefined
}

let display = true
const props = defineProps({
  src: { type: String, default: 'https://smitha-cdn.s3.us-east-2.amazonaws.com/GeeksCloset/erik-mclean-0_wXoeCDry0-unsplash.jpg' },
  height: { type: Number, default: 400 },
  text: { type: String, required: true },
  actions: { type: Array<HeroAction>, default: [] }
})
</script>
