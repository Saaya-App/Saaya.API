using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Saaya.API.Db.Models;

namespace Saaya.API.Db
{
    public class ApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Playlist>()
                .HasOne(x => x.User)
                .WithMany(x => x.Playlists)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Song>()
                .HasOne(x => x.User)
                .WithMany(x => x.Songs)
                .HasForeignKey(x => x.UserId);
        }
    }
}