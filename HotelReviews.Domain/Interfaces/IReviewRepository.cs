using HotelReviews.Domain.Entities;

namespace HotelReviews.Domain.Interfaces;
public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Review> CreateAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Review>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByRoomIdAsync(int roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByRatingAsync(int rating, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetVerifiedReviewsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetApprovedReviewsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetPendingApprovalAsync(CancellationToken cancellationToken = default);

    Task<double> GetAverageRatingByRoomAsync(int roomId, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountByRoomAsync(int roomId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, int>> GetRatingDistributionByRoomAsync(int roomId, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Review> Items, long TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? roomId = null,
        int? clientId = null,
        int? minRating = null,
        bool? isVerified = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Review>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> HasClientReviewedRoomAsync(int clientId, int roomId, CancellationToken cancellationToken = default);
}