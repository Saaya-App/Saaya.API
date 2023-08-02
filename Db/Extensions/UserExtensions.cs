#nullable disable
using Microsoft.EntityFrameworkCore;
using Saaya.API.Db.Models;

namespace Saaya.API.Db.Extensions
{
    public static class UserExtensions
    {
        public static bool UserExists(this DbSet<User> users, string token)
            => users.Any(x => x.Token == token);

        public static IQueryable<User> Include(this DbSet<User> users)
            => users.Include(x => x.Songs)
                    .Include(x => x.Playlists)
                    .AsNoTracking()
                    .AsQueryable();

        public static List<Song>? GetSongs(this DbSet<User> user, string token)
            => Include(user)
                .Where(x => x.Token == token)
                .FirstOrDefault()
                .Songs
                .ToList();

        public static List<Playlist>? GetPlaylists(this DbSet<User> user, string token)
            => Include(user)
                .Where(x => x.Token == token)
                .FirstOrDefault()
                .Playlists
                .ToList();

        public static List<Song>? GetPlaylistSongs(this DbSet<User> user, string token, int playlist)
            => Include(user)
                .Where(x => x.Token == token)
                .FirstOrDefault()
                .Songs
                .Where(x => x.PlaylistId == playlist)
                .ToList();
    }
}
