using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PinParc_Monolith.Models.Domain;

namespace PinParc_Monolith.Services.Orders
{
    public interface IOrdersService
    {
        Task<IEnumerable<Order>> GetOrders(Guid? userID, CancellationToken token);
        Task SetOrders(Order model, CancellationToken token);
        

    }
}
