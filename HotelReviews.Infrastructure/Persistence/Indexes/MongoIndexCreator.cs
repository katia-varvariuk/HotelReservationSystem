using HotelReviews.Domain.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace HotelReviews.Infrastructure.Persistence.Indexes;
public class MongoIndexCreator
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoIndexCreator> _logger;

    public MongoIndexCreator(MongoDbContext context, ILogger<MongoIndexCreator> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task CreateIndexesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MongoDB indexes...");

        await CreateReviewIndexesAsync(cancellationToken);
        await CreateRequestIndexesAsync(cancellationToken);

        _logger.LogInformation("MongoDB indexes created successfully");
    }
    private async Task CreateReviewIndexesAsync(CancellationToken cancellationToken)
    {
        var collection = _context.Reviews;

        var roomIdIndex = Builders<Review>.IndexKeys.Ascending(r => r.RoomId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(roomIdIndex, new CreateIndexOptions { Name = "idx_roomId" }),
            cancellationToken: cancellationToken);

        var clientIdIndex = Builders<Review>.IndexKeys.Ascending(r => r.ClientId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(clientIdIndex, new CreateIndexOptions { Name = "idx_clientId" }),
            cancellationToken: cancellationToken);

        var approvedReviewsIndex = Builders<Review>.IndexKeys
            .Ascending(r => r.RoomId)
            .Ascending(r => r.IsApproved)
            .Descending(r => r.Date);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(approvedReviewsIndex, new CreateIndexOptions { Name = "idx_approved_reviews" }),
            cancellationToken: cancellationToken);

        var pendingIndex = Builders<Review>.IndexKeys
            .Ascending(r => r.IsApproved)
            .Ascending(r => r.Date);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(pendingIndex, new CreateIndexOptions { Name = "idx_pending_approval" }),
            cancellationToken: cancellationToken);

        var textIndex = Builders<Review>.IndexKeys.Text(r => r.Comment);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Review>(textIndex, new CreateIndexOptions { Name = "idx_text_search" }),
            cancellationToken: cancellationToken);

        _logger.LogInformation("Indexes for Reviews collection created");
    }

    private async Task CreateRequestIndexesAsync(CancellationToken cancellationToken)
    {
        var collection = _context.Requests;

        var roomIdIndex = Builders<Request>.IndexKeys.Ascending(r => r.RoomId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(roomIdIndex, new CreateIndexOptions { Name = "idx_roomId" }),
            cancellationToken: cancellationToken);

        var clientIdIndex = Builders<Request>.IndexKeys.Ascending(r => r.ClientId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(clientIdIndex, new CreateIndexOptions { Name = "idx_clientId" }),
            cancellationToken: cancellationToken);

        var statusPriorityIndex = Builders<Request>.IndexKeys
            .Ascending("status")
            .Descending(r => r.Priority)
            .Ascending(r => r.Date);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(statusPriorityIndex, new CreateIndexOptions { Name = "idx_status_priority" }),
            cancellationToken: cancellationToken);

        var categoryIndex = Builders<Request>.IndexKeys.Ascending(r => r.Category);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(categoryIndex, new CreateIndexOptions { Name = "idx_category" }),
            cancellationToken: cancellationToken);

        var handledByIndex = Builders<Request>.IndexKeys.Ascending(r => r.HandledBy);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(handledByIndex, new CreateIndexOptions { Name = "idx_handledBy" }),
            cancellationToken: cancellationToken);

        var textIndex = Builders<Request>.IndexKeys.Text(r => r.RequestText);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(textIndex, new CreateIndexOptions { Name = "idx_text_search" }),
            cancellationToken: cancellationToken);

        var criticalIndex = Builders<Request>.IndexKeys
            .Ascending("status")
            .Ascending(r => r.Date);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Request>(criticalIndex, new CreateIndexOptions { Name = "idx_critical_requests" }),
            cancellationToken: cancellationToken);

        _logger.LogInformation("Indexes for Requests collection created");
    }
}