using System.ComponentModel.DataAnnotations;

namespace Saaya.API.Db
{
    public class SaayaEntity
    {
        [Key]
        public int Id { get; set; }

        public long DateCreated { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}