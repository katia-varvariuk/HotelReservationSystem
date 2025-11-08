namespace HotelPlatform.Aggregator.DTOs;

public class RoomCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; } = string.Empty;
}