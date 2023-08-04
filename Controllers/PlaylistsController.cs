using Microsoft.AspNetCore.Mvc;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;
using Saaya.API.Db.Models;
using Saaya.API.Services;

namespace Saaya.API.Controllers
{
    [Route("[controller]")]
    public class PlaylistsController : Controller
    {
        private readonly ILogger<PlaylistsController> _logger;

        private readonly ApiContext _db;
        private readonly LibraryService _library;

        public PlaylistsController(ILogger<PlaylistsController> logger, ApiContext db, LibraryService library)
        {
            _logger = logger;
            _db = db;
            _library = library;
        }

        [HttpGet]
        public IActionResult GetUserPlaylists()
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest(new List<Playlist>());

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest(new List<Playlist>());

            return Ok(_db.Users.GetPlaylists(AuthToken) ?? new List<Playlist>());
        }

        [HttpPost("{playlist}")]
        public async Task<IActionResult> AddPlaylist(string playlist)
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest();

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest();

            var result = await _library.DownloadPlaylist(playlist, _db.Users.Where(x => x.Token == AuthToken).FirstOrDefault());
            if (result)
                return Ok();
            else
                return BadRequest();
        }
    }
}
