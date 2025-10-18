using System.ComponentModel.DataAnnotations;

namespace HotelCatalog.Bll.DTOs
{
    public class RoomCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ServiceDto> Services { get; set; } = new();
        public int ServiceCount { get; set; }
    }

    public class CreateRoomCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }

    public class UpdateRoomCategoryDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }

    public class AssignServiceToCategoryDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ServiceId { get; set; }
    }
}