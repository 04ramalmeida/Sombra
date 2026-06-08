using Microsoft.EntityFrameworkCore;
using Sombra.Endpoints;
using Sombra.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DockerConnection");
builder.Services.AddDbContext<SombraDb>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<PostService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Sombra";
    config.Title = "Sombra v1";
    config.Version = "v1";
});

builder.Services.AddValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Sombra";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}


// Register API endpoints
app.MapGroup("/api")
    .MapPostsApi();

app.Run();
