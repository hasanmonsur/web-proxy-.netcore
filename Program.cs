using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add YARP reverse proxy services
builder.Services.AddReverseProxy()
    .LoadFromMemory(
        new[]
        {
            new RouteConfig
            {
                RouteId = "route1",
                ClusterId = "backend_cluster",
                Match = new RouteMatch
                {
                    Path = "{**catch-all}"
                }
            }
        },
        new[]
        {
            new ClusterConfig
            {
                ClusterId = "backend_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "destination1", new DestinationConfig { Address = "https://jsonplaceholder.typicode.com/" } }
                }
            }
        }
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.Use(async (context, next) =>
{
    Console.WriteLine($"Request URL: {context.Request.Path}");
    await next.Invoke();
});

// Use YARP reverse proxy
app.MapReverseProxy();

app.Run();
