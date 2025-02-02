using Scalar.AspNetCore;
using Spottarr.Services;
using Spottarr.Web.Helpers;
using Spottarr.Web.Logging;
using Spottarr.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(builder.Environment);
builder.Configuration.MapConfigurationSources(builder.Environment);

builder.Services.AddOpenApi();
builder.Services.AddAntiforgery();
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddSpottarrServices(builder.Configuration);

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

await app.MigrateDatabase();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.MapStaticAssets();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.MapFallbackToFile("/index.html");

// Middleware pipeline, order matters here
app.UseHttpsRedirection();
app.UseMiddleware<NewsznabQueryActionMiddleware>();
app.UseRouting();
app.UseCors();
app.UseAntiforgery();
app.UseAuthorization();

await app.RunAsync();