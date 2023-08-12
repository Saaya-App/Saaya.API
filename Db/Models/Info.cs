#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Saaya.API.Db.Models
{
    public class Info : SaayaEntity
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? Platform { get; set; }
    }
}