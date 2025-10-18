using System.ComponentModel.DataAnnotations;

namespace HotelCatalog.Bll.DTOs
{
    public class DiscountCategoryDto
    {
        public int DiscountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
    }

    public class CreateDiscountCategoryDto
    {
        [Required(ErrorMessage = "Discount name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100")]
        public decimal DiscountPercent { get; set; }
    }

    public class UpdateDiscountCategoryDto
    {
        [Required]
        public int DiscountId { get; set; }

        [Required(ErrorMessage = "Discount name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100")]
        public decimal DiscountPercent { get; set; }
    }
}