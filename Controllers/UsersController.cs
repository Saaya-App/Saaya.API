#nullable disable
using Microsoft.AspNetCore.Mvc;
using Saaya.API.Common.Attributes;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;

namespace Saaya.API.Controllers
{
    [SaayaAuthorized]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ApiContext _db;

        public UsersController(ApiContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns the information about the user making the API request.
        /// </summary>
        /// <returns>The user's information.</returns>
        [HttpGet("me")]
        public IActionResult GetUserInfo()
        {
            string AuthToken = HttpContext.Items["AuthToken"] as string;

            return Ok(_db.Users.GetUser(AuthToken));
        }
    }
}