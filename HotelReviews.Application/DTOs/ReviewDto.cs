namespace HotelReviews.Application.DTOs;
public class ReviewDto
{
    public string Id { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int RoomId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsVerified { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class CreateReviewDto
{
    public int ClientId { get; set; }
    public int RoomId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
public class UpdateReviewDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
public class ReviewStatisticsDto
{
    public int RoomId { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
    public int VerifiedReviewsCount { get; set; }
    public int ApprovedReviewsCount { get; set; }
}