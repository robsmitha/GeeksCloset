using Elysian.Application.Features.ContentManagement.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Elysian
{
    public class WordPressFunctions(ILogger<WordPressFunctions> logger, IMediator mediator)
    {
        [Function("WordPressContent")]
        public async Task<HttpResponseData> WordPressContent([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var content = await mediator.Send(new GetContentQuery());
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/json; charset=utf-8");
            await response.WriteStringAsync(JsonConvert.SerializeObject(content));
            return response;
        }
    }
}
