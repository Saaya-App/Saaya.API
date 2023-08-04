using Microsoft.AspNetCore.Mvc;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;

namespace Saaya.API.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ApiContext _db;

        public UsersController(ApiContext db)
        {
            _db = db;
        }

        [HttpGet("me")]
        public IActionResult GetUserInfo()
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest();

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest();

            return Ok(_db.Users.GetUser(AuthToken));
        }
    }
}