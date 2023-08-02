using Microsoft.AspNetCore.Mvc;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;
using Saaya.API.Db.Models;

namespace Saaya.API.Controllers
{
    [Route("[controller]")]
    public class PlaylistsController : Controller
    {
        private readonly ILogger<PlaylistsController> _logger;
        private readonly ApiContext _db;

        public PlaylistsController(ILogger<PlaylistsController> logger, ApiContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [Route("playlists/")]
        public IActionResult GetPlaylistsForDevice()
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest(new List<Playlist>());

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest(new List<Playlist>());

            return Ok(_db.Users.GetPlaylists(AuthToken) ?? new List<Playlist>());
        }
    }
}
