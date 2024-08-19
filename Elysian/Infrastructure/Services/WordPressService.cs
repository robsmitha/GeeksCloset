using Elysian.Application.Features.ContentManagement.Models;
using Elysian.Application.Interfaces;
using Elysian.Domain.Responses.WordPress;
using Newtonsoft.Json;

namespace Elysian.Infrastructure.Services
{
    public class WordPressService(HttpClient httpClient) : IWordPressService
    {
        public async Task<WordPressContent> GetWordPressContentAsync()
        {
            var pages = await httpClient.GetStringAsync("/wp-json/wp/v2/pages");
            return new WordPressContent
            {
                Pages = JsonConvert.DeserializeObject<List<WpPageResponse>>(pages)
            };
        }

    }
}
