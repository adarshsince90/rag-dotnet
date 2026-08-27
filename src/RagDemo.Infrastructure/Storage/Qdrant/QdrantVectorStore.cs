using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using RagDemo.Domain.Models;
using RagDemo.Infrastructure.Configuration;

public sealed class QdrantVectorStore : IVectorStore
{
    private readonly QdrantClient _client;
    private readonly QdrantOptions _options;
    private readonly RetrievalOptions _retrievalOptions;

    public QdrantVectorStore(
        IOptions<QdrantOptions> options,
        IOptions<RetrievalOptions> retrievalOptions)
    {
        _options = options.Value;
        _retrievalOptions = retrievalOptions.Value;

        var uri = new Uri(_options.BaseUrl);

        _client = new QdrantClient(
            uri.Host,
            uri.Port);
    }

    public async Task InitializeAsync(
    CancellationToken cancellationToken = default)
    {
        var collections =
            await _client.ListCollectionsAsync(
                cancellationToken);

        var exists =
            collections.Contains(
                _options.CollectionName);

        if (exists)
            return;

        await _client.CreateCollectionAsync(
            _options.CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = (ulong)_options.VectorSize,
                Distance = Distance.Cosine
            },
            cancellationToken: cancellationToken);
    }

    public async Task UpsertAsync(
    IReadOnlyCollection<DocumentChunk> chunks,
    CancellationToken cancellationToken = default)
    {
        var points = chunks
            .Where(c => c.Embedding is not null)
            .Select(chunk =>
                {
                    var point = new PointStruct
                    {
                        Id = Guid.Parse(chunk.Id),
                        Vectors = new Vectors
                        {
                            Vector = chunk.Embedding!
                        },
                        Payload = 
                        {
                            ["source"] = chunk.Source,
                            ["chunkIndex"] = chunk.ChunkIndex,
                            ["content"] = chunk.Content
                        }
                    };
                    return point;
                }
            )
            .ToList();

        await _client.UpsertAsync(
            _options.CollectionName,
            points,
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<RetrievalResult>> SearchAsync(
    float[] embedding,
    CancellationToken cancellationToken = default)
    {
        var response =
            await _client.QueryAsync(
                collectionName: _options.CollectionName,
                query: embedding,
                limit: (ulong)_retrievalOptions.SearchLimit,
                cancellationToken: cancellationToken);

        return response
            .Select((point, index) => new RetrievalResult
            {
               Chunk = new DocumentChunk
               {
                    Id = point.Id.ToString(),
                    ChunkIndex = (int)point.Payload["chunkIndex"].IntegerValue,
                    Content = point.Payload["content"].StringValue,
                    Source =  point.Payload["source"].StringValue
               },
                Score = point.Score,
                Rank = index + 1
            })
            .ToList();
    }
}