using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PinParc_Monolith.Infrastructure.EF;
using PinParc_Monolith.Models.Domain;

namespace PinParc_Monolith.Services.Orders
{
    public class OrdersService : BaseService, IOrdersService
    {

        public OrdersService(PinParcDbContext db) : base(db)
        {

        }
        public async Task<IEnumerable<Order>> GetOrders(Guid? userID, CancellationToken token)
        {
            var res = await _db.Order.Include(i => i.ParcelType).Where(i => i.UserID == userID).ToListAsync(token);
            return res;
        }

        public async Task SetOrders(Order model, CancellationToken token)
        {

            _db.Order.Add(model);
            await _db.SaveChangesAsync(token);
        }
    }
}
