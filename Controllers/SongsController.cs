using Microsoft.AspNetCore.Mvc;
using Saaya.API.Db;
using Saaya.API.Db.Extensions;
using Saaya.API.Db.Models;

namespace Saaya.API.Controllers
{
    [Route("[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ILogger<SongsController> _logger;
        private readonly ApiContext _db;
        
        public SongsController(ILogger<SongsController> logger, ApiContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult GetUserSongs()
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return BadRequest(new List<Song>());

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest(new List<Song>());

            return Ok(_db.Users.GetSongs(AuthToken) ?? new List<Song>());
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
    }
}