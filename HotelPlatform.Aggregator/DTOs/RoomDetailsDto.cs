namespace HotelPlatform.Aggregator.DTOs;

public class RoomDetailsDto
{
    public int RoomId { get; set; }
    public RoomCategoryDto? Category { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
    public List<ReservationDto> Reservations { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}