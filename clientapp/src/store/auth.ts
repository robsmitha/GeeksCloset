// Utilities
import { defineStore } from 'pinia'
import { ClaimsIdentity } from './types'
import apiClient from '@/api/elysianClient'

type State = {
  userDetails: string | undefined,
  hasGitHubAccessToken: boolean | undefined,
  identity: ClaimsIdentity | undefined
}

export const useAuthStore = defineStore('auth', {
  state: (): State => ({
    userDetails: undefined,
    hasGitHubAccessToken: undefined,
    identity: undefined
  }),
  getters: {
    // auth
    signedIn: (state: State) => state.userDetails !== undefined,
    hasValidAccessToken: (state: State) => Boolean(state.hasGitHubAccessToken)
  },
  actions: {
    async fetchAuth(): Promise<void> {
      const response = await apiClient.getData('/.auth/me');
      if (!response.success){
        console.error("Failed to get auth me.")
        return
      }

      const identity: ClaimsIdentity = response.data
      this.identity = identity
      if (identity?.clientPrincipal) {
        this.userDetails = identity.clientPrincipal.userDetails
      }
    }
  }
})