using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinParc_Monolith.Models.Domain
{
    public partial class Order
    {
        public int ID { get; set; }
        public string DestFrom { get; set; }
        public string DestTo { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int ParcelTypeID { get; set; }
        public int Weight { get; set; }
        public int DeliveryTypeID { get; set; }
        public int PaymentMethodID { get; set; }
        public int OrderStatus { get; set; }
        public Guid? UserID { get; set; }
        public Guid? CourierID { get; set; }

        public int? Distance { get; set; }
        public decimal? Price { get; set; }

        public virtual ParcelTypes ParcelType { get; set; }
    }
}
