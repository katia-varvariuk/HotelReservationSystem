namespace HotelPlatform.Aggregator.DTOs;

public class ReviewDto
{
    public string Id { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int RoomId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}