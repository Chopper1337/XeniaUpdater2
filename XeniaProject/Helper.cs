using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        string _currentFullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, "");

        //Creates the expected folder structure for a build of Xenia which backs up the zip of the previously downloaded build
        public void CreateFolderStructure(XeniaBuild build)
        {
            Directory.CreateDirectory(build.FolderName); // Xenia build extracts here
        }
        public void UninstallBuild(XeniaBuild build, bool skipMsg = false)
        {
            string[] filesToDelete = { $"{build.FolderName}\\{build.ExecutableName}.exe", $"{build.FolderName}\\LICENSE", $"{build.FolderName}\\xenia.pdb" };
            List<string> existingFiles = filesToDelete.Where(File.Exists).ToList();

            if (existingFiles.Count == 0) return;

            var validFiles = string.Join("\n", existingFiles);

            if (!skipMsg && MessageBox.Show($"Are you sure you want to delete\n{validFiles}", "Warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            if (!Directory.Exists(build.FolderName)) return;

            // Kill Xenia if it's running
            try
            {
                StopProcess(build);
                foreach (var file in existingFiles)
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
            }
            catch
            {
                UninstallBuild(build, true);
            }

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
                // ignored
            }
        }

        // Starts a build of Xenia
        public void OpenLocation(XeniaBuild build)
        {
            try
            {
                Process.Start($"{build.FolderName}");
            }
            catch
            {
                MessageBox.Show($"\"{build.ExecutableName}.exe\" could not be found.", "Error");
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
                var proc = Process.GetProcessesByName(build.ExecutableName);
                proc[0].Kill();
            }
            catch
            {
                // ignored
            }
        }

        public void ExtractPatches()
        {
            try
            {
                ZipFile.ExtractToDirectory($"XeniaCanary/patches.zip", "XeniaCanary");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool InternetAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://github.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
