using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DigitalLearningPlatform.ApiGateway.Aggregator
{
    public class CourseSectionFileInitiateAndCreateAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var initiateResponse = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var sectionResponse = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

            var combined = new
            {
                InitiateResult = JsonSerializer.Deserialize<object>(initiateResponse),
                SectionResult = JsonSerializer.Deserialize<Guid>(sectionResponse)
            };

            var content = new StringContent(JsonSerializer.Serialize(combined), Encoding.UTF8, "application/json");

            return new DownstreamResponse(content, HttpStatusCode.OK,
                new List<KeyValuePair<string, IEnumerable<string>>>(), "application/json");
        }
    }
}
