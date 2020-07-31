using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinParc_Monolith.Models.Domain
{
    public partial class ParcelTypes
    {
        public ParcelTypes()
        {
            Order = new HashSet<Order>();
        }

        public int ID { get; set; }
        public decimal? Coefficient { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
