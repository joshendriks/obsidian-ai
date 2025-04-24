using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using obsidian_ai.Ollama;
using obsidian_ai.VectorStore.Redis;

namespace obsidian_ai;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddOllama(builder.Configuration);
        builder.Services.AddRedis(builder.Configuration);
        builder.Services.AddScoped<FileProcessor>();

        builder.Services.AddQueuing(builder.Configuration);

        builder.Services.Configure<FormOptions>(options =>
        {
            // Prevent upload of data greater than 5MB
            options.MultipartBodyLengthLimit = 5 * 1024 * 1024;
        });

        var baseDir = builder.Configuration["BaseDir"];
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapGet("/fullscan/{*filePath}", async (
                [FromRoute] string filePath,
                [FromServices] FileProcessor fileProcessor) =>
            {
                Console.WriteLine("start fullscan " + filePath);
                await fileProcessor.ProcessRecusrively(filePath, baseDir);
                Console.WriteLine("finish fullscan " + filePath);
            }).DisableAntiforgery()
            .WithName("Fullscan");

        app.MapPost("/{*filePath}", async (
                [FromRoute] string filePath,
                IFormFile file,
                [FromServices] FileProcessor fileProcessor,
                [FromServices] QueueThing queueThing) =>
            {
                Console.WriteLine("update " + filePath);
                queueThing.Enqueue(new DocumentChanged(filePath, ChangeType.Updated));
                await fileProcessor.Process(filePath, baseDir);

                return Results.Ok($"Uploaded file {file.FileName} successfully");
            }).DisableAntiforgery()
            .WithName("UploadFile");

        app.MapDelete("/{*filePath}", ([FromRoute] string filePath) =>
            {
                Console.WriteLine("delete " + filePath);
                return Results.Ok();
            }).DisableAntiforgery()
            .WithName("DeleteFile");
        
        app.MapGet("/", () => 
            Results.Ok("Hello World!"));

        await app.RunAsync();
    }
}