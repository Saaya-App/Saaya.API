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

        /// <summary>
        /// Get user songs
        /// </summary>
        /// <returns>List of user songs</returns>
        [HttpGet]
        public IActionResult GetUserSongs()
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return Unauthorized("Token is invalid.");

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest();

            return Ok(_db.Users.GetSongs(AuthToken));
        }

        /// <summary>
        /// Get songs for a specific playlist
        /// </summary>
        /// <param name="playlist">The playlist ID</param>
        /// <returns>A list of songs from the playlist</returns>
        [HttpGet("{playlist}")]
        public IActionResult GetPlaylistSongs(int playlist)
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return Unauthorized("Token is invalid.");

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest(new List<Song>());

            return Ok(_db.Users.GetPlaylistSongs(AuthToken, playlist) ?? new List<Song>());
        }

        /// <summary>
        /// Adds a song for a user.
        /// </summary>
        /// <param name="song">The name of the song to be added.</param>
        /// <returns>HTTP response indicating the success or failure of the operation.</returns>
        [HttpPost("{song}")]
        public async Task<IActionResult> AddSongForUser(string song)
        {
            string AuthToken = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (string.IsNullOrEmpty(AuthToken))
                return Unauthorized("Token is invalid.");

            if (!_db.Users.UserExists(AuthToken))
                return BadRequest();

            var user = _db.Users.GetUser(AuthToken);
            
            var result = await _library.DownloadSong(song, user);
            if (result)
                return Ok();
            else
                return BadRequest();
        }
    }
}