using HotelReviews.Domain.Entities;
using HotelReviews.Domain.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HotelReviews.Infrastructure.Persistence.Repositories;
public class RequestRepository : IRequestRepository
{
    private readonly IMongoCollection<Request> _collection;

    public RequestRepository(MongoDbContext context)
    {
        _collection = context.Requests;
    }

    public async Task<Request?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(_ => true)
            .ToListAsync(cancellationToken);
    }

    public async Task<Request> CreateAsync(Request request, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(request, cancellationToken: cancellationToken);
        return request;
    }

    public async Task UpdateAsync(Request request, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            r => r.Id == request.Id,
            request,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.ClientId == clientId)
            .SortByDescending(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByRoomIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.RoomId == roomId)
            .SortByDescending(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.Status.Value == status)
            .SortByDescending(r => r.Priority)
            .ThenBy(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.Category == category)
            .SortByDescending(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.Priority == priority)
            .SortByDescending(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetPendingRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.Status.Value == "pending")
            .SortByDescending(r => r.Priority)
            .ThenBy(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetCriticalRequestsAsync(CancellationToken cancellationToken = default)
    {
        var yesterday = DateTime.UtcNow.AddHours(-24);

        return await _collection
            .Find(r => r.Status.Value == "pending" && r.Date < yesterday)
            .SortBy(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(r => r.HandledBy == employeeId)
            .SortByDescending(r => r.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetRequestCountByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return (int)await _collection
            .CountDocumentsAsync(r => r.Status.Value == status, cancellationToken: cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetRequestsByStatusGroupedAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _collection
            .Find(_ => true)
            .ToListAsync(cancellationToken);

        return requests
            .GroupBy(r => r.Status.Value)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<string, int>> GetRequestsByCategoryGroupedAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _collection
            .Find(_ => true)
            .ToListAsync(cancellationToken);

        return requests
            .GroupBy(r => r.Category)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<int> GetActiveRequestsCountAsync(CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[] { "pending", "approved", "in_progress" };

        return (int)await _collection
            .CountDocumentsAsync(
                r => activeStatuses.Contains(r.Status.Value),
                cancellationToken: cancellationToken);
    }

    public async Task<(IEnumerable<Request> Items, long TotalCount)> GetPagedAsync(
    int page,
    int pageSize,
    int? roomId = null,
    int? clientId = null,
    string? status = null,
    string? category = null,
    int? minPriority = null,
    bool? hasResponse = null,
    CancellationToken cancellationToken = default)
    {
        var query = _collection.AsQueryable();

        if (roomId.HasValue)
        {
            query = query.Where(r => r.RoomId == roomId.Value);
        }

        if (clientId.HasValue)
        {
            query = query.Where(r => r.ClientId == clientId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status.ToString() == status);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(r => r.Category == category);
        }

        if (minPriority.HasValue)
        {
            query = query.Where(r => r.Priority >= minPriority.Value);
        }

        if (hasResponse.HasValue)
        {
            if (hasResponse.Value)
            {
                query = query.Where(r => r.Response != null);
            }
            else
            {
                query = query.Where(r => r.Response == null);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Request>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Request>.Filter.Text(searchTerm);

        return await _collection
            .Find(filter)
            .SortByDescending(r => r.Date)
            .Limit(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(r => r.Id == id, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<int> GetOpenRequestsCountByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[] { "pending", "approved", "in_progress" };

        return (int)await _collection
            .CountDocumentsAsync(
                r => r.ClientId == clientId && activeStatuses.Contains(r.Status.Value),
                cancellationToken: cancellationToken);
    }
}