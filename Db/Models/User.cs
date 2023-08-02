#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Saaya.API.Db.Models
{
    public class User : SaayaEntity
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        public List<Song> Songs { get; set; } = new List<Song>();
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}