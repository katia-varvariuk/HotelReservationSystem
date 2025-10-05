using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCatalog.Domain.Entities
{
    public class RoomService
    {
        public int CategoryId { get; set; }
        public int ServiceId { get; set; }
        public RoomCategory RoomCategory { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}