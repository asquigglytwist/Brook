using Brook.MainWin.Model;
using Brook.MainWin.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Brook.MainWin.ViewModel
{
    public class DetailsViewModel : ViewModelBase
    {
        string _rootDirPath;
        readonly int totalMp3Files;
        int processedMp3Files, corruptedMp3Files;

        public ObservableCollection<SoundFile> AllSoundFiles
        { get; private set; }

        public DetailsViewModel(string rootDirPath)
        {
            Common.PerfTracker.StartTracking("InitDVM");
            _rootDirPath = rootDirPath;
            totalMp3Files = _Init();
            var elapsedTime = Common.PerfTracker.StopTracking("InitDVM");
            Logger.Instance.Debug($"Total time elapsed in reading {AllSoundFiles.Count} files:  {elapsedTime}");
            Logger.Instance.Info($"\nTotal files:  {totalMp3Files}\nProcessed:  {processedMp3Files}\nCorrupted:  {corruptedMp3Files}");
        }

        private int _Init()
        {
            AllSoundFiles = new ObservableCollection<SoundFile>();
            var mp3Files = Helpers.GetFilesWithExtension(_rootDirPath, "mp3");
            processedMp3Files = corruptedMp3Files = 0;
            for (int i = 0; i < mp3Files.Length; i++)
            {
                try
                {
                    var sFile = new SoundFile(mp3Files[i]);
                    sFile.Init();
                    AllSoundFiles.Add(sFile);
                    processedMp3Files++;
                }
                catch (TagLib.CorruptFileException cfe)
                {
                    Logger.Instance.Error($"CorruptFileException occured with file # {i:00000}:{Environment.NewLine}{mp3Files[i]}");
                    Logger.Instance.Debug(cfe.AsLoggableString());
                    corruptedMp3Files++;
                }
            }
            return mp3Files.Length;
        }

        #region LSVSelectedItemChanged
        // [BIB]:  https://stackoverflow.com/a/12297537
        SoundFile _selectedItem;

        public SoundFile SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (value == _selectedItem)
                {
                    return;
                }
                _selectedItem = value;
                HandleLSVSelectionChanged(SelectedItem);
            }
        }

        public byte[] SelectedItemAlbumArtData
        {
            get; private set;
        }

        public void HandleLSVSelectionChanged(object p)
        {
            //SelectedItemAlbumArtData = SelectedItem.AlbumArtData;
        }
        #endregion
    }
}
