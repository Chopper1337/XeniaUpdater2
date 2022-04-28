using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;

/// <summary>
/// This class contains a collection of methods which do not need to be within the same class as the UI elements
/// </summary>
namespace XeniaProject
{
    internal class Helper
    {
        // The location the XeniaUpdater application is stored
        string currentFullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, "");

        //Creates the expected folder structure for a build of Xenia which backs up the zip of the previously downloaded build
        public void CreateFolderStructure(XeniaBuild build)
        {
            Directory.CreateDirectory(build.FolderName); // Xenia build extracts here
        }
        public void UninstallBuild(XeniaBuild build)
        {
            if (Directory.Exists(build.FolderName))
                Directory.Delete(build.FolderName, true); // Deletes the folder in which the build is contained
        }

        // Extracts build of Xenia to expected folder
        public void ExtractBuild(XeniaBuild build)
        {
            //Deletes LICENSE file because it isn't needed and also causes issues for some reason
            try
            {
                File.Delete($"{build.FolderName}/LICENSE");
                ZipFile.ExtractToDirectory($"{build.FolderName}/{build.ZipName}.zip", build.FolderName);
                File.Delete($"{build.FolderName}/LICENSE");

            }
            catch
            {
            }
        }

        // Starts a build of Xenia
        public void StartProcess(XeniaBuild build)
        {
            try
            {
                Process.Start($"{build.FolderName}\\{build.ExecutableName}");
            }
            catch
            {
                MessageBox.Show($"\"{build.ExecutableName}.exe\" could not be started.\nThe file must be present and executable.", "Error");
            }
        }

        // Kills the Xenia process
        public void StopProcess(XeniaBuild build)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(build.ExecutableName);
                proc[0].Kill();
            }
            catch
            {

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
