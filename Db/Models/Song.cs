#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Saaya.API.Db.Models
{
    public class Song : SaayaEntity
    {
        public User User { get; set; }
        public int UserId { get; set; }

        public Playlist? Playlist { get; set; }
        public int? PlaylistId { get; set; }

        [Required]
        public string Thumbnail { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public TimeSpan Length { get; set; }

        [Required]
        public string Url { get; set; }
    }
}