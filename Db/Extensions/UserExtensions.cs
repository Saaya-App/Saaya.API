using Microsoft.EntityFrameworkCore;
using Saaya.API.Db.Models;

namespace Saaya.API.Db.Extensions
{
    public static class UserExtensions
    {
        public static bool UserExists(this DbSet<User> users, string token)
            => users.Any(x => x.Token == token);

        public static User? GetUser(this DbSet<User> users, string token)
            => Include(users).Where(x => x.Token == token).FirstOrDefault();

        public static IQueryable<User> Include(this DbSet<User> users)
            => users.Include(x => x.Songs)
                    .Include(x => x.Playlists)
                    .AsNoTracking()
                    .AsQueryable();

        /// <summary>
        /// Gets all songs associated with the user token.
        /// </summary>
        /// <param name="user">The database set of users.</param>
        /// <param name="token">The unique user token.</param>
        /// <returns>List of songs associated with the user.</returns>
        public static List<Song>? GetSongs(this DbSet<User> user, string token)
            => Include(user)
                .Where(x => x.Token == token)?
                .FirstOrDefault()?
                .Songs
                .ToList();

        /// <summary>
        /// Gets all songs associated with the user token.
        /// </summary>
        /// <param name="user">The database set of users.</param>
        /// <param name="token">The unique user token.</param>
        /// <returns>List of playlists associated with the user.</returns>
        public static List<Playlist>? GetPlaylists(this DbSet<User> user, string token)
            => Include(user)
                .Where(x => x.Token == token)?
                .FirstOrDefault()?
                .Playlists
                .ToList();

        /// <summary>
        /// Gets all songs associated with the user token.
        /// </summary>
        /// <param name="user">The database set of users.</param>
        /// <param name="token">The unique user token.</param>
        /// <param name="playlist">The playlist ID.</param>
        /// <returns>List of songs, in a specific playlist, associated with the user.</returns>
        public static List<Song>? GetPlaylistSongs(this DbSet<User> user, string token, int playlist)
            => Include(user)
                .Where(x => x.Token == token)?
                .FirstOrDefault()?
                .Songs
                .Where(x => x.PlaylistId == playlist)
                .ToList();
    }
}
