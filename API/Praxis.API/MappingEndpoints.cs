using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Praxis.Features.Stories
{
    public static class MappingEndpoints
    {
        public static void MapStoryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/stories/{n:int}", async (
                int n,
                IStoryService service,
                HttpContext ctx) =>
            {
                var data = await service.GetBestStories(n);

                var json = JsonSerializer.Serialize(data);
                var etag = Convert.ToBase64String(
                    SHA256.HashData(Encoding.UTF8.GetBytes(json)));

                if (ctx.Request.Headers["If-None-Match"] == etag)
                    return Results.StatusCode(304);

                ctx.Response.Headers.ETag = etag;

                return Results.Content(json, "application/json");
            })
                .WithName("GetBestStories");
        }
    }
}
