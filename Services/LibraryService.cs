#nullable disable
using AngleSharp.Dom;
using Microsoft.OpenApi.Validations;
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
        private readonly Regex ShortSongs = new Regex(@"(?<=be/)[^&]+");

        private readonly Regex Playlists = new Regex(@"(?<=list=)[^&]+");

        public LibraryService(ApiContext db, YoutubeClient yt)
        {
            _db = db;
            _yt = yt;
        }

        public async Task DownloadSong(string url, User user)
        {
            if (!Songs.IsMatch(Uri.UnescapeDataString(url)) && !ShortSongs.IsMatch(Uri.UnescapeDataString(url)))
                return;

            string result = null;
            if (Songs.Match(Uri.UnescapeDataString(url)).Success)
                result = Songs.Match(Uri.UnescapeDataString(url)).Groups[0].Value;
            else if (ShortSongs.Match(Uri.UnescapeDataString(url)).Success)
                result = ShortSongs.Match(Uri.UnescapeDataString(url)).Groups[0].Value;

            var song = await _yt.Videos.GetAsync(result);

            if (_db.Songs.Where(x => x.Url == song.Url && x.User == user).Any())
                return;

            await _db.Songs.AddAsync(new Song
            {
                UserId = user.Id,
                Thumbnail = song.Thumbnails.First().Url,
                Title = song.Title,
                Author = song.Author.ChannelTitle,
                Length = song.Duration.Value,
                Url = song.Url,
            });

            await _db.SaveChangesAsync();
        }

        public async Task DownloadPlaylist(string playlist, User user)
        {
            var result = Playlists.Match(Uri.UnescapeDataString(playlist));
            if (!result.Success)
                return;

            var YTPlaylist = await _yt.Playlists.GetAsync(result.Groups[0].Value);

            List<Song> songs = new List<Song>();

            await foreach (var video in _yt.Playlists.GetVideosAsync(Uri.UnescapeDataString(playlist)))
            {
                if (_db.Songs.Where(x => x.Url == video.Url && x.User == user).Any())
                    continue;

                songs.Add(new Song
                {
                    UserId = user.Id,
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
                UserId = user.Id,
                Name = YTPlaylist.Title,
                Songs = songs
            });

            await _db.SaveChangesAsync();

        }
    }
}