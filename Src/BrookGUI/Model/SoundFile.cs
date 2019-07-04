using Brook.MainWin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

        // [BIB]:  https://stackoverflow.com/questions/41998142/converting-system-drawing-image-to-system-windows-media-imagesource-with-no-resu
        //public BitmapImage AlbumArtThumbNail
        //{ get; private set; }

        public byte[] AlbumArtData
        { get; private set; }

        public string AlbumArtHash
        { get; private set; }

        public void Init()
        {
            _tlf = TagLib.File.Create(SourceAudioFile);
            Duration = _tlf.Properties.Duration;
            var fileTagData = _tlf.Tag;
            Album = fileTagData.Album;
            if (string.IsNullOrWhiteSpace(Album))
            {
                Album = "--NA--";
            }
            Title = fileTagData.Title;
            if (string.IsNullOrWhiteSpace(Title))
            {
                Title = "--NA--";
            }
            Year = fileTagData.Year;
            Track = fileTagData.Track;
            Performers = fileTagData.Performers;
            AlbumArtists = fileTagData.AlbumArtists;
            Composers = fileTagData.Composers;
            Genres = fileTagData.Genres;
            Lyrics = fileTagData.Lyrics;
            Comment = fileTagData.Comment;
            var temp = _tlf.Tag.Pictures.FirstOrDefault()?.Data?.Data;
            if (temp == null)
            {
                AlbumArtData = Helpers.EmptyBitmap;
                AlbumArtHash = "--NA--";
            }
            else
            {
                AlbumArtData = temp.ToBitmapImage().ToByteArray();
                AlbumArtHash = AlbumArtData.MD5Hash();
            }
        }
    }
}
