using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Brook.MainWin.Utils
{
    public static class Helpers
    {
        private static string NewLine;

        static Helpers()
        {
            NewLine = Environment.NewLine;
            UserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            UserDesktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
            ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            StartUpTimeStampAsFileName = Helpers.CurrentTimeAsFileName;
        }

        #region Properties
        public static string ProgramData { get; private set; }

        public static string ApplicationDirectory { get; private set; }

        public static string UserAppData { get; private set; }

        public static string UserDesktop { get; internal set; }

        public static string StartUpTimeStampAsFileName { get; internal set; }

        public static string CurrentTimeAsFileName
        {
            get { return DateTime.UtcNow.ToString("yyyyMMddHHmmss"); }
        }

        public static string CurrentTimeForLogging
        {
            get { return DateTime.UtcNow.ToString("yyyy-MM-dd  HH:mm:ss"); }
        }

        // [BIB]:  https://stackoverflow.com/questions/3573863/create-an-empty-bitmapsource-in-c-sharp
        public static byte[] EmptyBitmap
        {
            get
            {
                var bmptmp = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgr24, null, new byte[3] { 0, 0, 0 }, 3);
                var imgcreated = new TransformedBitmap(bmptmp, new ScaleTransform(1, 1));
                byte[] data;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imgcreated));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
                return data;
            }
        }
        #endregion

        #region Public Methods
        public static void SleepForMS(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        public static void MoveDir(string from, string to)
        {
            Directory.Move(from, to);
            Logger.Instance.Debug("Successfully moved dir:  " + from + "  to  " + to);
        }

        public static bool DeleteDirRecursively(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    Logger.Instance.Debug($"Dir {dirPath} exists; Now deleting recursively.");
                    Directory.Delete(dirPath, true);
                }
                else
                {
                    Logger.Instance.Debug($"Dir {dirPath} doesn't exist.");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e, "Unable to delete directory");
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            Logger.Instance.Debug("Successfully deleted file:  " + filePath);
            return true;
        }

        public static bool FileExists(string localPath)
        {
            return File.Exists(localPath);
        }

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static void EnsureDirectoryIsAvailable(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Logger.Instance.Trace("Dir:  " + dirPath + " doesn't exist; Creating now...");
                Directory.CreateDirectory(dirPath);
                //Helpers.SleepForMS(3000);
            }
        }

        public static void EnsureFilePathIsAvailable(string filePath)
        {
            var dirName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirName))
            {
                Logger.Instance.Trace("Dir:  " + dirName + " doesn't exist; Creating now to ensure " + filePath + " can be created...");
                Directory.CreateDirectory(filePath);
                //Helpers.SleepForMS(3000);
            }
        }

        public static string[] GetFilesWithExtension(string dir, string fileExtension)
        {
            if (!Directory.Exists(dir))
            {
                throw new Exception($"Path {dir} does not exist.");
            }
            var pattern = fileExtension;
            if (!pattern.StartsWith("*."))
            {
                pattern = ($"*.{fileExtension}");
            }
            return Directory.GetFiles(dir, pattern, SearchOption.AllDirectories);
        }

        public static void DumpEnvDetails()
        {
            Logger.Instance.TraceEntry();
            OperatingSystem oSVersion = Environment.OSVersion;
            var text = string.Format("Environment Details:{0}Version:      {1}{0}Bitness:      {2}-Bit OS{0}MachineName:  {3}{0}UserName:     {4}", Environment.NewLine,
                oSVersion.VersionString,
                Environment.Is64BitOperatingSystem ? "64" : "32",
                Environment.MachineName,
                Environment.UserName);
            text = string.Format("{1}{0}AppDir:       {2}{0}AppDataDir:   {3}{0}Desktop:      {4}{0}ProgramData:  {5}{0}StartUpTime:  {6}", Environment.NewLine, text,
                ApplicationDirectory,
                UserAppData,
                UserDesktop,
                ProgramData,
                StartUpTimeStampAsFileName);
            Logger.Instance.Info(text);
            Logger.Instance.TraceExit();
            Logger.Instance.FlushIt();
        }
        #endregion

        #region Extensions
        public static string AsLoggableString(this Exception e)
        {
            var text = $"Exception occurred;  {e.Message}{NewLine}{e.StackTrace}";
            if (e.InnerException != null)
            {
                text = $"{text}{NewLine}{e.InnerException.AsLoggableString()}";
            }
            return text;
        }

        public static bool EqualsIgnoreCase(this string first, string second)
        {
            return first.Equals(second, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// A list of XElement descendent elements with the supplied local name (ignoring any namespace), or null if the element is not found.
        /// https://stackoverflow.com/a/43381447
        /// </summary>
        public static IEnumerable<XElement> FindDescendantsWithTagName(this XContainer xmlAnything,
            string tagName)
        {
            var result = xmlAnything.Descendants().Where(ele => ele.Name.LocalName == tagName);
            return result;
        }

        public static string GetFirstNodeValue(this IEnumerable<XElement> xmlElement)
        {
            return xmlElement?.First()?.Value;
        }

        public static int LaunchProcAndWaitForExit(this ProcessStartInfo psiProcInfo, string timeTrackingMessage)
        {
            Logger.Instance.TraceEntry();
            Logger.Instance.Debug($"Launching Process:  \"{psiProcInfo.FileName}\" with args:{Environment.NewLine}\t\"{psiProcInfo.Arguments}\"");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var proc = Process.Start(psiProcInfo);
            Logger.Instance.Debug($"Launched process {proc.Id} and waiting for it to exit.");
            proc.WaitForExit();
            int procExitCode = -999;
            if (!proc.HasExited)
            {
                Logger.Instance.Error("Process did not exit clean; Returning -1.");
            }
            procExitCode = proc.ExitCode;
            stopWatch.Stop();
            Logger.Instance.Debug($"Process ExitCode:  {procExitCode}");
            Logger.Instance.Perf($"Time elapsed in {timeTrackingMessage}:  " + stopWatch.Elapsed.ToString());
            Logger.Instance.FlushIt();
            Logger.Instance.TraceExit();
            return procExitCode;
        }

        // [BIB]:  https://stackoverflow.com/questions/35804375/how-do-i-save-a-bitmapimage-from-memory-into-a-file-in-wpf-c
        public static void Save(this BitmapImage image, string filePath)
        {
            if (image.Height > 60 && image.Width > 60)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        //public static string TryGetConfigValue(this IniFile configFile,
        //    string sectionName, string keyName, string originalValue)
        //{
        //    var temp = configFile.IniReadValue(sectionName, keyName);
        //    if (string.IsNullOrWhiteSpace(temp))
        //    {
        //        return originalValue;
        //    }
        //    return temp;
        //}
        #endregion
    }
}
