using HotelReviews.Domain.Entities;

namespace HotelReviews.Domain.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Request> CreateAsync(Request request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Request request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Request>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetByRoomIdAsync(int roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetPendingRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetCriticalRequestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Request>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);

    Task<int> GetRequestCountByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetRequestsByStatusGroupedAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetRequestsByCategoryGroupedAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveRequestsCountAsync(CancellationToken cancellationToken = default);

    Task<(IEnumerable<Request> Items, long TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? roomId = null,
        int? clientId = null,
        string? status = null,
        string? category = null,
        int? minPriority = null,
        bool? hasResponse = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Request>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetOpenRequestsCountByClientAsync(int clientId, CancellationToken cancellationToken = default);
}