#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Saaya.API.Db.Models
{
    public class Playlist : SaayaEntity
    {
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}