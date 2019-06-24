using Brook.MainWin.Model;
using Brook.MainWin.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brook.MainWin.ViewModel
{
    public class DetailsViewModel : ViewModelBase
    {
        string _rootDirPath;

        public ObservableCollection<SoundFile> AllSoundFiles
        { get; private set; }

        public DetailsViewModel(string rootDirPath)
        {
            _rootDirPath = rootDirPath;
            _Init();
        }

        private void _Init()
        {
            AllSoundFiles = new ObservableCollection<SoundFile>();
            var mp3Files = Helpers.GetFilesWithExtension(_rootDirPath, "mp3");
            for (int i = 0; i < mp3Files.Length; i++)
            {
                var sFile = new SoundFile(mp3Files[i]);
                sFile.Init();
                AllSoundFiles.Add(sFile);
            }
        }
    }
}
