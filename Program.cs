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

posts.MapGet("/", GetPosts);
posts.MapGet("/{id}", GetPost);
posts.MapPost("/", CreatePost);
posts.MapPut("/{id}", UpdatePost);
posts.MapDelete("/{id}", DeletePost);


static async Task<IResult> GetPosts(string? searchTerm, SombraDb db)
{
    if (string.IsNullOrEmpty(searchTerm))
    {
        var posts = await db.Posts.ToListAsync();
        return TypedResults.Ok(posts);
    }

    var results = await db.Posts.Where(p => p.Title.Contains(searchTerm) ||
                        p.Content.Contains(searchTerm) ||
                        p.Category.Contains(searchTerm)
                        ).ToListAsync();

    return TypedResults.Ok(results);
}

static async Task<IResult> GetPost(int id, SombraDb db)
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return TypedResults.NotFound();

    return TypedResults.Ok(post);
}

static async Task<IResult> CreatePost(Post post, SombraDb db)
{
    db.Posts.Add(post);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/posts/{post.Id}", post);
}

static async Task<IResult> UpdatePost(int id, Post input, SombraDb db)
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return TypedResults.NotFound();

    post.Title = input.Title;
    post.Content = input.Content;
    post.Category = input.Category;
    post.Tags = input.Tags;

    await db.SaveChangesAsync();
    return TypedResults.Ok(post);
}

static async Task<IResult> DeletePost(int id, SombraDb db)
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return TypedResults.NotFound();

    db.Posts.Remove(post);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}


app.Run();
