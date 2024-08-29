import axios, { AxiosResponse } from 'axios';

type ApiResponse = {
  data: any | undefined,
  errors: Map<string, string[]> | undefined,
  success: boolean,
  statusCode: number
}

class ApiClient {
  private tenant: string;

  constructor(tenant: string) {
    this.tenant = tenant;
  }

  private getHeaders(): { [key: string]: string } {
    return {
      '___tenant___': this.tenant,
      'Content-Type': 'application/json',
    };
  }

  private responseFromError(error: any): ApiResponse {
    return {
      errors: new Map<string, string[]>([
        ['ERROR', [error instanceof Error && error.message || 'An unhandled error occurred']]
      ]),
      data: undefined,
      success: false,
      statusCode: 500
    }
  }

  private responseFromStatus(response: AxiosResponse<any>) : ApiResponse {
    if(response.status >= 200 && response.status < 300) {
      return {
        errors: undefined,
        data: response.data,
        success: true,
        statusCode: response.statusCode
      }
    }

    const errors = new Map<string, string[]>();

    switch(response.status){
      case 400:
        for(const d in response.data) {
          errors.set(d, response.data[d])
        }
        break
      case 403:
        errors.set('FORBIDDEN', ['Access Denied'])
        break
      case 404:
        errors.set('NOTFOUND', ['Not found'])
        break
      case 429:
        errors.set('RATE_LIMIT', ['Not found'])
        break
    }

    return errors.size === 0 
      ? this.responseFromError(null)
      : {
        errors: errors,
        data: undefined,
        success: false,
        statusCode: response.statusCode
      }
  }

  async getData(endpoint: string): Promise<ApiResponse> {
    try {
      const response: AxiosResponse<any> = await axios.get(endpoint, {
        headers: this.getHeaders(),
      })

      return this.responseFromStatus(response)
    } catch (error) {
      console.error('Error fetching data:', error)
      return this.responseFromError(error)
    }
  }

  async postData(endpoint: string, data: any): Promise<ApiResponse> {
    try {
      const response: AxiosResponse<any> = await axios.post(endpoint, data, {
        headers: this.getHeaders(),
      })
      return this.responseFromStatus(response)
    } catch (error) {
      console.error('Error posting data:', error);
      return this.responseFromError(error)
    }
  }
}

export default ApiClient;
