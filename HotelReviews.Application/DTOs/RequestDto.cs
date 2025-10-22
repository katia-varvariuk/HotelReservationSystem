namespace HotelReviews.Application.DTOs;
public class RequestDto
{
    public string Id { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int RoomId { get; set; }
    public string RequestText { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Priority { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Response { get; set; }
    public DateTime? ResponseDate { get; set; }
    public int? HandledBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class CreateRequestDto
{
    public int ClientId { get; set; }
    public int RoomId { get; set; }
    public string RequestText { get; set; } = string.Empty;
    public string Category { get; set; } = "general";
    public int Priority { get; set; } = 3;
}
public class UpdateRequestStatusDto
{
    public string Status { get; set; } = string.Empty;
}
public class AddRequestResponseDto
{
    public string ResponseText { get; set; } = string.Empty;
    public int? HandledByEmployeeId { get; set; }
}
public class RequestStatisticsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int CompletedRequests { get; set; }
    public Dictionary<string, int> RequestsByStatus { get; set; } = new();
    public Dictionary<string, int> RequestsByCategory { get; set; } = new();
    public int CriticalRequests { get; set; }
}