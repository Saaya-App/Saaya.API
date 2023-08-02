#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Saaya.API.Db.Models
{
    public class Playlist : SaayaEntity
    {
        public User User { get; set; }
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Song> Songs { get; set; } = new List<Song>();
    }
}