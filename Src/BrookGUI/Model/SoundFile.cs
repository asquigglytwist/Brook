using Brook.MainWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brook.MainWin.Model
{
    public class SoundFile
    {
        TagLib.File _tlf;

        public SoundFile(string soundFilePath)
        {
            if(Helpers.FileExists(soundFilePath))
            {
                SourceAudioFile = soundFilePath;
            }
            else
            {
                throw new Exception($"Audio file {soundFilePath} not found.");
            }
        }

        public string SourceAudioFile
        { get; private set; }

        public TimeSpan Duration
        { get; private set; }

        public string Album
        { get; private set; }

        public string Title
        { get; private set; }

        public string[] Performers
        { get; private set; }

        public string[] AlbumArtists
        { get; private set; }

        public string[] Composers
        { get; private set; }

        public uint Year
        { get; private set; }

        public uint Track
        { get; private set; }

        public string[] Genres
        { get; private set; }

        public string Lyrics
        { get; private set; }

        public string Comment
        { get; private set; }

        public void Init()
        {
            _tlf = TagLib.File.Create(SourceAudioFile);
            Duration = _tlf.Properties.Duration;
            var fileTagData = _tlf.Tag;
            Album = fileTagData.Album;
            Title = fileTagData.Title;
            Year = fileTagData.Year;
            Track = fileTagData.Track;
            Performers = fileTagData.Performers;
            AlbumArtists = fileTagData.AlbumArtists;
            Composers = fileTagData.Composers;
            Genres = fileTagData.Genres;
            Lyrics = fileTagData.Lyrics;
            Comment = fileTagData.Comment;
        }
    }
}
