using Saaya.API.Db;
using Saaya.API.Db.Models;
using System.Text.RegularExpressions;
using YoutubeExplode;

namespace Saaya.API.Services
{
    public class LibraryService
    {
        private readonly ApiContext _db;
        private readonly YoutubeClient _yt;

        private readonly Regex Songs = new Regex(@"(?<=v=)[^&]+");
        private readonly Regex Playlists = new Regex(@"(?<=list=)[^&]+");

        public LibraryService(ApiContext db, YoutubeClient yt)
        {
            _db = db;
            _yt = yt;
        }

        // Should use Uri.EscapeDataString() to escape the song URL, when making POST req
        public async Task DownloadSong(string url, User user)
        {
            var result = Songs.Match(url);
            if (!result.Success)
                return;

            var song = await _yt.Videos.GetAsync(result.Groups[0].Value);

            if (_db.Songs.Where(x => x.Url == song.Url && x.User == user).Any())
                return;

            await _db.Songs.AddAsync(new Song
            {
                User = user,
                Thumbnail = song.Thumbnails.First().Url,
                Title = song.Title,
                Author = song.Author.ChannelTitle,
                Length = song.Duration.Value,
                Url = song.Url,
            });

            await _db.SaveChangesAsync();
        }

        // Should use Uri.EscapeDataString() to escape the playlist URL, when making POST req
        public async Task DownloadPlaylist(string playlist, User user)
        {
            var result = Playlists.Match(playlist);
            if (!result.Success)
                return;

            var YTPlaylist = await _yt.Playlists.GetAsync(result.Groups[0].Value);

            List<Song> songs = new List<Song>();

            await foreach (var video in _yt.Playlists.GetVideosAsync(playlist))
            {
                if (_db.Songs.Where(x => x.Url == video.Url && x.User == user).Any())
                    continue;

                songs.Add(new Song
                {
                    User = user,
                    Thumbnail = video.Thumbnails.First().Url,
                    Title = video.Title,
                    Author = video.Author.ChannelTitle,
                    Length = video.Duration.Value,
                    Url = video.Url,
                });

                await _db.SaveChangesAsync();
            }

            await _db.Playlists.AddAsync(new Db.Models.Playlist
            {
                User = user,
                Name = YTPlaylist.Title,
                Songs = songs
            });

            await _db.SaveChangesAsync();

        }
    }
}