using Microsoft.Extensions.VectorData;

public class VectorizedChunk
{
    public VectorizedChunk()
    {
        
    }

    [VectorStoreRecordKey]
    public string Key { get; set; }

    [VectorStoreRecordData]
    public int ChunkNumber { get; set; }

    [VectorStoreRecordData]
    public string? FilePath { get; set; }

    [VectorStoreRecordData]
    public string? Text { get; set; }

    [VectorStoreRecordVector(1024, DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float>? Vector { get; set; }
}