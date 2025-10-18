using System.ComponentModel.DataAnnotations;

namespace HotelCatalog.Bll.DTOs
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<RoomCategoryDto> RoomCategories { get; set; } = new();
    }

    public class CreateServiceDto
    {
        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100000")]
        public decimal Price { get; set; }
    }

    public class UpdateServiceDto
    {
        [Required]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100000")]
        public decimal Price { get; set; }
    }
}