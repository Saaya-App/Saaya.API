﻿#nullable disable
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

        /// <summary>
        /// Downloads a song from a provided YouTube URL and adds it to the user's library.
        /// </summary>
        /// <param name="url">The URL of the YouTube video to download.</param>
        /// <param name="user">The user to add the song to their library.</param>
        /// <returns>True if the song was downloaded and added to the user's library, false otherwise.</returns>
        public async Task<bool> DownloadSong(string url, User user)
        {
            if (!Songs.IsMatch(Uri.UnescapeDataString(url)) && !ShortSongs.IsMatch(Uri.UnescapeDataString(url)))
                return false;

            string result = null;
            if (Songs.Match(Uri.UnescapeDataString(url)).Success)
                result = Songs.Match(Uri.UnescapeDataString(url)).Groups[0].Value;
            else if (ShortSongs.Match(Uri.UnescapeDataString(url)).Success)
                result = ShortSongs.Match(Uri.UnescapeDataString(url)).Groups[0].Value;

            var song = await _yt.Videos.GetAsync(result);

            if (_db.Songs.Where(x => x.Url == song.Url && x.UserId == user.Id).Any())
                return false;

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
            return true;
        }

        /// <summary>
        /// Downloads a song from a provided YouTube URL and adds it to the user's library.
        /// </summary>
        /// <param name="playlist">The URL of the YouTube playlist from which to download videos.</param>
        /// <param name="user">The user to add the song to their library.</param>
        /// <returns>True if the playlist was downloaded and added to the user's library, false otherwise.</returns>
        public async Task<bool> DownloadPlaylist(string playlist, User user)
        {
            var result = Playlists.Match(Uri.UnescapeDataString(playlist));
            if (!result.Success)
                return false;

            var YTPlaylist = await _yt.Playlists.GetAsync(result.Groups[0].Value);

            List<Song> songs = new List<Song>();

            await foreach (var video in _yt.Playlists.GetVideosAsync(Uri.UnescapeDataString(playlist)))
            {
                if (_db.Songs.Where(x => x.Url == video.Url && x.UserId == user.Id).Any())
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
            return true;
        }
    }
}