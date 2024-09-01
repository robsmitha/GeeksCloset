<template>
    <v-sheet color="grey-darken-4" class="pb-3">
        <v-container>
            <v-card color="transparent"  title="Services" subtitle="Try them out!" flat>
                <v-card-text>
                    <v-row>
                        <template v-for="f in features">
                            <v-col v-if="f.enabled" :key="f.title" md="4" sm="6" cols="12">
                                <v-card
                                    class="fill-height"
                                    :title="f.title" 
                                    :subtitle="f.subtitle" 
                                    :text="f.text" 
                                    :to="f.to"
                                    :href="f.href"
                                    :target="f.href ? '_blank': ''"
                                    :prepend-icon="f.icon"
                                    rounded="0"
                                    hover>
                                </v-card>
                            </v-col>
                        </template>
                    </v-row>
                </v-card-text>
            </v-card>
        </v-container>
    </v-sheet>
</template>

<script setup lang="ts">

const features = [
    {
        title: 'Card Search',
        subtitle: 'Find a card by serial number',
        text: 'Look up a card by serial number to view images, grade, and notes.',
        to: '/search',
        icon: 'mdi-certificate-outline',
        enabled: true
    },
    {
        title: 'Buy Online',
        subtitle: 'Merchandise at the best price',
        text: 'Comic Books, Anime Manga, Video Games and Funko Pops.',
        href: 'https://www.ebay.com/str/bemysalesman',
        icon: 'mdi-shopping-outline',
        enabled: true
    },
    {
        title: 'Visit In-Store',
        subtitle: 'New Location',
        text: 'Get directions to visit our Tallahassee store.',
        href: 'https://maps.app.goo.gl/mPUTjdGnsoH4k5L88',
        icon: 'mdi-store',
        enabled: true
    }
]
</script>