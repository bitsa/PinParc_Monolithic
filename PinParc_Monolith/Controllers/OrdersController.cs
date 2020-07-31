using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PinParc_Monolith.Models.Domain;
using PinParc_Monolith.Services.Orders;

namespace PinParc_Monolith.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _service;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(UserManager<IdentityUser> userManager,IOrdersService service)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders (CancellationToken token)
        {
            
            var userID = Guid.Parse("60780fff-b469-46b6-a347-0b94b37750bb");
            var res = await _service.GetOrders(userID, token);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> PostOrders([FromBody]Order model,CancellationToken token)
        {

            var userID = Guid.Parse("60780fff-b469-46b6-a347-0b94b37750bb");
            model.UserID = userID;
            model.OrderStatus = 0;
            await _service.SetOrders(model, token);
            return Ok();
        }
    }
}
