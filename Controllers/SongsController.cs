using Microsoft.AspNetCore.Mvc;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;
using Saaya.API.Db.Models;
using Saaya.API.Services;

namespace Saaya.API.Controllers
{
    [Route("[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ILogger<SongsController> _logger;

        private readonly ApiContext _db;
        private readonly LibraryService _library;
        
        public SongsController(ILogger<SongsController> logger, ApiContext db, LibraryService library)
        {
            _logger = logger;
            _db = db;
            _library = library;
        }

        [HttpGet]
        public IActionResult GetUserSongs()
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest();

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest();

            return Ok(_db.Users.GetSongs(AuthToken));
        }

        [HttpGet("{playlist}")]
        public IActionResult GetPlaylistSongs(int playlist)
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest(new List<Song>());

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest(new List<Song>());

            return Ok(_db.Users.GetPlaylistSongs(AuthToken, playlist) ?? new List<Song>());
        }

        [HttpPost("{song}")]
        public async Task<IActionResult> AddSongForUser(string song)
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest();

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest();

            var user = _db.Users.GetUser(AuthToken);
            
            await _library.DownloadSong(song, user);

            return Ok();
        }
    }
}