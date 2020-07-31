using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinParc_Monolith.Models.DTO
{
    public class CreateUserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
