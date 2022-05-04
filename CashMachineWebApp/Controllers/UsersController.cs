using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CashMachineWebApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        { 
            var users = new[]
            {
                new {Name = "Oleg"},
                new {Name = "Andrey"}
            };

            return Ok(users);
        }
}

    
}
