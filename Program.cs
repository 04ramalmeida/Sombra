using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DockerConnection");
builder.Services.AddDbContext<SombraDb>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Sombra";
    config.Title = "Sombra v1";
    config.Version = "v1";
});

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

var posts = app.MapGroup("/posts");

posts.MapGet("/", async (string? searchTerm, SombraDb db) =>
{
    if (string.IsNullOrEmpty(searchTerm))
    {
        var posts = await db.Posts.ToListAsync();
        return Results.Ok(posts);
    }


    var results = await db.Posts.Where(p => p.Title.Contains(searchTerm) ||
                        p.Content.Contains(searchTerm) ||
                        p.Category.Contains(searchTerm)
                        ).ToListAsync();

    return Results.Ok(results);
});

posts.MapGet("/{id}", async (int id, SombraDb db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return Results.NotFound();

    return Results.Ok(post);

});

posts.MapPost("/", async (Post post, SombraDb db) =>
{
    db.Posts.Add(post);
    await db.SaveChangesAsync();

    return Results.Created($"/posts/{post.Id}", post);
});

posts.MapPut("/{id}", async (int id, Post input, SombraDb db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return Results.NotFound();

    post.Title = input.Title;
    post.Content = input.Content;
    post.Category = input.Category;
    post.Tags = input.Tags;

    await db.SaveChangesAsync();
    return Results.Ok(post);
});

posts.MapDelete("/{id}", async (int id, SombraDb db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return Results.NotFound();

    db.Posts.Remove(post);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
