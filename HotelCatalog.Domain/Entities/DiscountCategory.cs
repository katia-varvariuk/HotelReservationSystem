using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCatalog.Domain.Entities
{
    public class DiscountCategory
    {
        public int DiscountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public ICollection<ClientDiscount> ClientDiscounts { get; set; } = new List<ClientDiscount>();
    }
}