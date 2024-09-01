// Utilities
import { defineStore } from 'pinia'
import { WpPage, WpPost, WpTag, WpCategory } from './types'
import apiClient from '@/api/elysianClient'

type State = {
  pages: WpPage[]
}

export const useAppStore = defineStore('app', {
  state: (): State => ({
    pages: []
  }),
  getters: {
    // pages
    homePage: (state: State) => state.pages.find((page) => page.slug === 'geeks-closet-home-page'),
    aboutPage: (state: State) => state.pages.find((page) => page.slug === 'geeks-closet-about-page'),
    findCardPage: (state: State) => state.pages.find((page) => page.slug === 'geeks-closet-find-card')
  },
  actions: {
    async fetchContent(): Promise<void> {
      const response = await apiClient.getData('/api/WordPressContent')
      if (!response.success){
        console.error("Failed to get page content.")
        return
      }
      
      this.pages = response.data.pages
    }
  }
})