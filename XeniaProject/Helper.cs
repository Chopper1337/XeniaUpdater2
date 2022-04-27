using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows;

namespace XeniaProject
{
    internal class Helper
    {
        //Creates the expected folder structure for a build of Xenia which backs up the zip of the previously downloaded build
        public void CreateFolderStructure(string folderName)
        {
            Directory.CreateDirectory(folderName); // Xenia build extracts here
            Directory.CreateDirectory($"{folderName}/LastUpdate"); // Old Zip gets moved here
        }

        public void ExtractBuild(string folderName, string zipName)
        {
            //Deletes LICENSE file because it isn't needed and also causes issues for some reason
            try
            {
                File.Delete($"{folderName}/LICENSE");
                ZipFile.ExtractToDirectory($"{folderName}/{zipName}", folderName);
                File.Delete($"{folderName}/LICENSE");

            }
            catch (Exception e)
            {
            }
        }

        public void StartProcess(string ExecutableName, string ExeLocation)
        {
            try
            {
                Process.Start($"{ExeLocation}\\{ExecutableName}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"\"{ExecutableName}.exe\" could not be started.\nThe file must be present and executable.", "Error");
            }
        }
    }
}
