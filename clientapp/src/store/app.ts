// Utilities
import { defineStore } from 'pinia'
import { WpPage, WpPost, WpTag, WpCategory } from './types'
import apiClient from '@/api/elysianClient'

type State = {
  pages: PageContent[]
}

type PageContent = {
  title: string,
  content: string,
  slug: string
}

export const useAppStore = defineStore('app', {
  state: (): State => ({
    pages: defaultState.pages
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
      
      this.pages = response.data.pages.map((p : WpPage) => {
        return {
          title: p.title.rendered,
          content: p.content.rendered,
          slug: p.slug
        }
      })
    }
  }
})


const defaultState: State = {
  pages: [
    {
      content: '\n<p>We pride ourselves on offering the most extensive selection of comic books, anime manga, video games, and Funko Pops. As a true family-run business, we bring a personal touch to everything we do, ensuring that our customers not only find what they’re looking for but also enjoy a warm, welcoming experience. With a passion for pop culture, we scour the nation to provide you with the best merchandise at unbeatable prices, carefully curating our inventory to cater to both casual fans and serious collectors alike.</p>\n\n\n<div style="height:22px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>Beyond our diverse product range, we specialize in grading comic books and trading cards, offering expert assessments that help preserve and enhance your collection\'s value. We also travel the country attending conventions, where we secure Verified Authentic Autographs to add an extra layer of authenticity and excitement to your favorite items. Whether you\'re hunting for a rare issue, seeking out the latest release, or looking to invest in a graded collectible, our dedication to quality and customer satisfaction sets us apart.</p>\n',
      title: 'We buy, sell & trade everything Geek.',
      slug: 'geeks-closet-home-page'
    },
    {
      content: '\n<h4 class="wp-block-heading"><strong>Address</strong></h4>\n\n\n<div style="height:25px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>1140 Capital Circle SE #3<br>Tallahassee, FL 32301, US</p>\n\n\n<div style="height:25px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p><a href="tel:850-371-5472">850-371-5472</a>&nbsp;</p>\n\n\n<div style="height:25px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p><a href="mailto:MajorGeek@Geekscloset.com">MajorGeek@Geekscloset.com</a></p>\n\n\n<div style="height:25px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p><a href="https://www.fsunews.com/story/entertainment/arts/2024/01/21/geeks-closet-is-tallahassees-new-pop-culture-paradise/72298356007/" data-type="link" data-id="https://www.fsunews.com/story/entertainment/arts/2024/01/21/geeks-closet-is-tallahassees-new-pop-culture-paradise/72298356007/" target="_blank" rel="noreferrer noopener">In the News</a></p>\n\n\n<div style="height:22px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p><a href="https://linktr.ee/geekscloset" data-type="link" data-id="https://www.fsunews.com/story/entertainment/arts/2024/01/21/geeks-closet-is-tallahassees-new-pop-culture-paradise/72298356007/" target="_blank" rel="noreferrer noopener">Socials & Podcast</a></p>\n\n\n<p></p>\n',
      title: 'Geeks Closet About',
      slug: 'geeks-closet-about-page'
    },
    {
      content: '\n<p>Quickly and easily locate your specific card using the serial number provided on your card graded by Geeks Closet Grading. Whether you\'re checking the status of a card you\'ve submitted, verifying its authenticity, or simply browsing through your collection, the serial number is your key to accessing detailed information about your card.</p>\n\n\n<div style="height:20px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p><strong>How to Use:</strong></p>\n\n\n<div style="height:20px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>1. <strong>Locate the Serial Number:</strong> On each card graded by Graded Geeks, you\'ll find a unique serial number. This number is typically found on the label at the top of the card\'s case.</p>\n\n\n<ol class="wp-block-list"></ol>\n\n\n<div style="height:20px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>2. <strong>Enter the Serial Number:</strong> Type the serial number into the search box provided on this page. Be sure to enter it exactly as it appears on your card to ensure accurate results.</p>\n\n\n<ol class="wp-block-list"></ol>\n\n\n<div style="height:20px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>3. <strong>View Your Card&#8217;s Details:</strong> Once you\'ve entered the serial number, click the search button. You&#8217;ll be presented with a detailed view of your card, including its grade, condition, and any other pertinent information.</p>\n\n\n<ol class="wp-block-list"></ol>\n\n\n<div style="height:20px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>This tool is here to help you keep track of your cards and ensure you&#8217;re always informed about the details of your collection. If you have any questions or need assistance, feel free to reach out to our support team.</p>\n\n\n<div style="height:20px" aria-hidden="true" class="wp-block-spacer"></div>\n\n\n<p>Thank you for choosing Geeks Closet Grading, and happy collecting!</p>\n\n\n<p></p>\n',
      title: 'Geeks Closet Find Card',
      slug: 'geeks-closet-find-card'
    }
  ]
};