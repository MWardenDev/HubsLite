using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapPost("ingress/orders", async (HttpContext ctx, IHttpClientFactory httpClientFactory) => {
    using var reader = new StreamReader(ctx.Request.Body);
    var json = await reader.ReadToEndAsync();

    var client = httpClientFactory.CreateClient();
    var resp = await client.PostAsync(
        "http://localhost:5126/transform",
        new StringContent(json, System.Text.Encoding.UTF8, "application/json")
    );

    var respBody = await resp.Content.ReadAsStringAsync();

    return Results.Content(respBody, "application/json", System.Text.Encoding.UTF8, (int)resp.StatusCode);
});

app.Run();

