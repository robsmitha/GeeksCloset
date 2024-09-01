/**
 * main.ts
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import { registerPlugins } from '@/plugins'

// Components
import App from './App.vue'

// Composables
import { createApp } from 'vue'

// Filters
import { useDateFilter } from './filters/dateFilter'

const app = createApp(App)

registerPlugins(app)

const { dateFilter } = useDateFilter()

// Extend the global properties type
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
      $filters: {
        formatDate: (value: string | Date | null, format?: string) => string
      }
    }
  }
  
  app.config.globalProperties.$filters = {
    formatDate: dateFilter
  }

app.mount('#app')
