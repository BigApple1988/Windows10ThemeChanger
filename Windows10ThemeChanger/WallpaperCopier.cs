using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Windows10ThemeChanger
{
    public class WallpaperCopier
    {
        public WallpaperCopier(string pathDestination)
        {
            if(!Directory.Exists(pathDestination))
                Directory.CreateDirectory(pathDestination);
            _pathDestination = pathDestination + "\\";
            var packagesFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages";
            string[] directories = Directory.GetDirectories(packagesFolder, "Microsoft.Windows.ContentDeliveryManager*");
            if(directories.Length > 0)
            {
                _pathAssets = directories[0] + "\\LocalState\\Assets";
            }
            else
            {
                MessageBox.Show("More than one Wallpaper Directory");
                Application.Exit();
            }
            UpdateFiles();
            Timer.Interval = 60 * 30 * 1000;
        }

        public void StartWork()
        {
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateFiles();
        }

        private bool CheckFileWidth(string assetFileName)
        {
            Stream imageStreamSource = new FileStream(assetFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource,
                                                                  BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                return bitmapSource.Width == 1920 || bitmapSource.PixelWidth == 1920;
            }
            catch(FileFormatException)
            {
                return false;
            }
            catch(NotSupportedException)
            {
                return false;
            }
        }

        private void UpdateFiles()
        {
            var filesAssets = Directory.GetFiles(_pathAssets);
            if(filesAssets.Length == _filesCount) return;
            _filesCount = filesAssets.Length;
            var filesDestination = Directory.GetFiles(_pathDestination);
            var filesToCopy = new List<string>();
            foreach(string filesAsset in filesAssets)
            {
                bool isContains = false;
                var assetFileName = Path.GetFileName(filesAsset);
                foreach(var fileDestination in filesDestination)
                {
                    var destFileName = Path.GetFileName(fileDestination);
                    if(destFileName.Length < assetFileName.Length) break;
                    var formattedDestFilename = Path.GetFileName(fileDestination).Substring(0, assetFileName.Length);
                    if(assetFileName.Equals(formattedDestFilename))
                    {
                        isContains = true;
                        break;
                    }
                }
                if(isContains) 
                    continue;
                {
                    if(!CheckFileWidth(filesAsset)) 
                        continue;
                    var destFileName = _pathDestination + assetFileName + ".jpg";
                    if(File.Exists(destFileName)) 
                        continue;
                    var copyEngine = new CopyEngine(filesAsset,destFileName);
                    copyEngine.CpEvHandler+=new CopyEventHandler(CopyInProgress);
                    copyEngine.CopyFiles();
//                    if((File.GetAttributes(filesAsset) & FileAttributes.Encrypted) == FileAttributes.Encrypted)
//                    {
//                        var decryptedAsset = $"{filesAsset}_decrypted";
//                        File.Copy(filesAsset, decryptedAsset, true);
//                        FileSystem.
//                        File.SetAttributes(decryptedAsset,FileAttributes.Normal);
//                        File.Decrypt(decryptedAsset);
//                        File.SetAttributes(decryptedAsset,FileAttributes.Normal);
//                        File.Copy(decryptedAsset, destFileName,true);
//                        File.SetAttributes(decryptedAsset,FileAttributes.Normal);
//                        File.Delete(decryptedAsset);
//                    }
//                    else
//                    {
//                        File.Copy(filesAsset, destFileName, false);
//                    }
                }
            }
            
        }

        static void CopyInProgress(CopyEngine Sender,CopyEngine.CopyEventArgs e)
        {
            Console.WriteLine("{0}%\t{1}Ko/s",e.CurrentPercent.ToString(".##"),e.CurrentTauxTransfert.ToString(".##"));
        }

        internal Timer Timer = new Timer();
        private readonly string _pathAssets;
        private readonly string _pathDestination;
        private int _filesCount;
    }
}