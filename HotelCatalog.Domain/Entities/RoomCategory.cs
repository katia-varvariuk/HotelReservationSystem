using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCatalog.Domain.Entities
{
    public class RoomCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
    }
}