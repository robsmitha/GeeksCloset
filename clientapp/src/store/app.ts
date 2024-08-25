// Utilities
import { defineStore } from 'pinia'
import { WpPage, WpPost, WpTag, WpCategory } from './types'

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
    aboutPage: (state: State) => state.pages.find((page) => page.slug === 'geeks-closet-about-page')
  },
  actions: {
    async fetchContent(): Promise<void> {
      const response = await fetch('/api/WordPressContent', {
        method: 'get',
        headers: {
            '___tenant___': 'geekscloset'
        }
    });
      if (!response.ok){
        console.error("Failed to get page content.")
        return
      }
      const data = await response.json()
      this.pages = data.pages
    }
  }
})