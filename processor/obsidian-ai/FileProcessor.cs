using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Text;
using obsidian_ai.Ollama;
using OllamaSharp;
using StackExchange.Redis;

namespace obsidian_ai;

#pragma warning disable SKEXP0001
public class FileProcessor
#pragma warning restore SKEXP0001
{
    private readonly IVectorStoreRecordCollection<string, VectorizedChunk> _collection;
    private readonly OllamaConfig _ollamaConfig;

    public FileProcessor(IVectorStore memoryStore, IOptionsMonitor<OllamaConfig> ollamaConfig, IConnectionMultiplexer connectionMultiplexer)
    {
        _ollamaConfig = ollamaConfig.CurrentValue;
        _collection = memoryStore.GetCollection<string, VectorizedChunk>("notes");
    }

    public async Task Process(string filePath, string baseDir)
    {
        if (filePath.EndsWith(".md"))
        {
            await ProcessText(filePath, baseDir);
        }
        
        if (filePath.EndsWith(".jpg"))
        {
            await ProcessImage(filePath, baseDir);
        }
    }

    private async Task ProcessImage(string filePath, string baseDir)
    {
        var fileBytes = await File.ReadAllBytesAsync(Path.Join(baseDir, filePath));
        var imageData= Convert.ToBase64String(fileBytes);

        var client = new OllamaApiClient(new Uri(_ollamaConfig.Url), "gemma3:12b");
        var chat = new Chat(client);
        var description = await chat.SendAsync(
           """
                   Elaborately describe what is on this picture. It was taken for informational purposes.
                   Mostly likely the information on it  
                   Answer without any reasoning. 
                   If there is any text, be sure to put that in the description as well.
                   """,
            [imageData]).StreamToEndAsync();
        await ChunkAndVectorize(filePath, description);
    }

    private async Task ProcessText(string filePath, string baseDir)
    {
        var fileContent = await File.ReadAllTextAsync(Path.Join(baseDir, filePath));
        await ChunkAndVectorize(filePath, fileContent);
    }

    private async Task ChunkAndVectorize(string filename, string fileContent)
    {
#pragma warning disable SKEXP0050
        var lines = TextChunker.SplitPlainTextLines(fileContent, 40);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 150);
#pragma warning restore SKEXP0050
        
        var generator = new OllamaEmbeddingGenerator(new Uri(_ollamaConfig.Url), "mxbai-embed-large");

        await _collection.CreateCollectionIfNotExistsAsync();
        
        var chunkNumber = 0;
        foreach (var paragraph in paragraphs)
        {
            chunkNumber++;
            await GenerateAndStoreVectorizedChunk(filename, chunkNumber, paragraph, generator);
        }
        
        await PurgeOldChunks(filename, chunkNumber);
    }

    private async Task GenerateAndStoreVectorizedChunk(string filename, int chunkNumber, string paragraph,
        IEmbeddingGenerator<string, Embedding<float>> generator)
    {
        var vectorizedChunk = new VectorizedChunk()
        {
            Key = $"{filename}:chunk:{chunkNumber}",
            ChunkNumber = chunkNumber,
            FilePath = filename,
            Text = paragraph,
        };
        vectorizedChunk.Vector = await generator.GenerateEmbeddingVectorAsync(vectorizedChunk.Text,
            new EmbeddingGenerationOptions()
            {
                Dimensions = 1536
            });
            
        await _collection.UpsertAsync(vectorizedChunk);
    }

    private async Task PurgeOldChunks(string filename, int chunkNumber)
    {
        chunkNumber++;
        var nextChunk = await _collection.GetAsync($"{filename}:chunk:{chunkNumber}");
        while (nextChunk != null)
        {
            await _collection.DeleteAsync($"{filename}:chunk:{chunkNumber}");
            chunkNumber++;
            nextChunk = await _collection.GetAsync($"{filename}:chunk:{chunkNumber}");
        }
    }

    public async Task ProcessRecusrively(string filePath, string baseDir)
    {
        await _collection.DeleteCollectionAsync();
        var directory = Path.Join(baseDir, filePath);
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            await Process(file[baseDir.Length..], baseDir);
        }
    }
}

