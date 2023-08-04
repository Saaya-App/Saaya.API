#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Saaya.API.Db.Models
{
    public class Song : SaayaEntity
    {
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public Playlist? Playlist { get; set; }
        [JsonIgnore]
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