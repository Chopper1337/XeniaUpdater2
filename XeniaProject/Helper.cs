using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows;
using System.Net;

namespace XeniaProject
{
    internal class Helper
    {
        string currentFullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, "");
        //Creates the expected folder structure for a build of Xenia which backs up the zip of the previously downloaded build
        public void CreateFolderStructure(XeniaBuild build)
        {
            //Directory.CreateDirectory($"{currentFullPath}/{folderName}"); 
            Directory.CreateDirectory(build.FolderName); // Xenia build extracts here
            Directory.CreateDirectory($"{build.FolderName}/LastUpdate"); // Old Zip gets moved here
        }
        public void UninstallBuild(XeniaBuild build)
        {
            //Directory.CreateDirectory($"{currentFullPath}/{folderName}"); 
            if (Directory.Exists(build.FolderName))
            {
                Directory.Delete(build.FolderName, true); // Deletes the folder in which the build is contained
            }
        }

        public void ExtractBuild(XeniaBuild build)
        {
            //Deletes LICENSE file because it isn't needed and also causes issues for some reason
            try
            {
                File.Delete($"{build.FolderName}/LICENSE");
                ZipFile.ExtractToDirectory($"{build.FolderName}/{build.ZipName}.zip", build.FolderName);
                File.Delete($"{build.FolderName}/LICENSE");

            }
            catch (Exception e)
            {
            }
        }

        public void StartProcess(XeniaBuild build)
        {
            try
            {
                Process.Start($"{build.FolderName}\\{build.ExecutableName}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"\"{build.ExecutableName}.exe\" could not be started.\nThe file must be present and executable.", "Error");
            }
        }

        public bool InternetAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://appveyor.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
