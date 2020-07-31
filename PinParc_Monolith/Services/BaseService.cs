using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PinParc_Monolith.Infrastructure.EF;

namespace PinParc_Monolith.Services
{
    public class BaseService
    {
        protected readonly PinParcDbContext _db;
        public BaseService(PinParcDbContext db)
        {
            _db = db;
        }
    }
}
