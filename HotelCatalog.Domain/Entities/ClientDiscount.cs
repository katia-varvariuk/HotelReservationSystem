using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCatalog.Domain.Entities
{
    public class ClientDiscount
    {
        public int ClientId { get; set; }
        public int DiscountId { get; set; }
        public DiscountCategory DiscountCategory { get; set; } = null!;
    }
}