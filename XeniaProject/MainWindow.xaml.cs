using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace XeniaProject
{
    public partial class MainWindow : Window
    {
        //Create List of XeniaBuilds to hold the Xenia 
        List<XeniaBuild> _builds = new List<XeniaBuild>();

        // Int to hold the selected item in Xenia list
        private int _selected = 0;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* -------- Xenia Builds -------- */
            // Creation of instances of XeniaBuild to fill the list with the different builds of Xenia
            XeniaBuild xeniaMaster = new XeniaBuild("Master",
                "https://github.com/xenia-project/release-builds-windows/releases/latest/download/xenia_master.zip",
                "Xenia Master is the release intended for use by most people", "XeniaMaster", "\\images\\master.png",
                XeniaBuild.Stability.Stable, "xenia", "xenia_master");
            XeniaBuild xeniaCanary = new XeniaBuild("Canary",
                "https://github.com/xenia-canary/xenia-canary/releases/latest/download/xenia_canary.zip",
                "Xenia Canary is a build intended for testing features with a small subset of users.", "XeniaCanary",
                "\\images\\canary.png", XeniaBuild.Stability.Unstable, "xenia_canary", "xenia_canary");

            // Add these to the list
            _builds.Add(xeniaMaster);
            _builds.Add(xeniaCanary);

            // XeniaBuildsMainList shows the contents of the created list
            XeniaBuildsMainList.ItemsSource = _builds.ToList();
            XeniaBuildsMainList.SelectedIndex = 0; // Set the selected index to 0
            ReactiveButtonText();

        }

        /* -------- Xenia Builds -------- */
        private void XeniaBuildsMainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = XeniaBuildsMainList.SelectedIndex; // Selected item in list
            ReactiveButtonText();
            XeniaBuildsMainImage.Source = new BitmapImage(new Uri(_builds[_selected].ImagePath, UriKind.Relative)); // Set image source to image path
            XeniaBuildsNameLabel.Content = _builds[_selected].Name; // Set displayed name to the builds name
            XeniaBuildsDescTxtblk.Text = _builds[_selected].Description; // Update the description
        }

        private void ReactiveButtonText()
        {
            UpdateBtn.Content = !File.Exists($"{_builds[_selected].FolderName}/{_builds[_selected].ExecutableName}.exe") ? "Download" : "Update";
        }

        // Clicking the Start button 
        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
            if (_selected <= -1) return; // If the user has selected an entry
            Helper helper = new Helper(); // Create an instance of the helper class
            helper.StartProcess(_builds[_selected]); // Use the start process method to start the build's executable
        }

        // Clicking stop button
        private void StopBTNClick(object sender, RoutedEventArgs e)
        {
            if (_selected <= -1) return;
            var helper = new Helper();
            helper.StopProcess(_builds[_selected]); // Use method in helper to kill the process 
        }

        // Clicking patches button
        private void PatchesBtnClick(object sender, RoutedEventArgs e)
        {
            var helper = new Helper();

            using (var wc = new WebClient())
            {
                //Download from URL to location
                wc.DownloadFileAsync(new Uri(
                    "https://github.com/xenia-canary/game-patches/releases/latest/download/game-patches.zip"), "XeniaCanary\\patches.zip");

                //For each change in progress, output progress to the wc_DownloadProgressChanged method
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WcDownloadProgressChanged);

                // For each update in the downloads progress, do this
                void WcDownloadProgressChanged(object s, DownloadProgressChangedEventArgs ea)
                {
                    DownloadProgressBar.Value = ea.ProgressPercentage;

                    if (Math.Abs(DownloadProgressBar.Value - 100) > 1) return;
                    wc.Dispose(); // Dispose of the web client
                    helper.ExtractPatches();
                }

                // If the web client could not download the zip, this code executes
                wc.Dispose(); // Dispose of the web client
            }
        }

        // Update button clicked 
        private void UpdateBTNClick(object sender, RoutedEventArgs e)
        {
            if (_selected <= -1) return;
            UpdateXenia(_builds[_selected]); // Use method to update xenia
            ReactiveButtonText();
        }

        // Downloads the latest copy of a selected Xenia build
        private void UpdateXenia(XeniaBuild build)
        {
            if (XeniaBuildsMainList.SelectedIndex <= -1) return;
            var helper = new Helper();
            if (helper.InternetAvailable()) //Checks if there is a working internet connection
            {
                helper.CreateFolderStructure(build); // Create the folder which the build will be downloaded to
                DownloadXenia(build); // Download the build
            }
            else
                MessageBox.Show("Could not connected to server.\nPlease check you internet connection.", "Error");
        }

        //Downloads a Xenia build
        public void DownloadXenia(XeniaBuild build)
        {
            var helper = new Helper();
            ToggleButtons(false); // Disables buttons

            using (var wc = new WebClient())
            {
                //Download from URL to location
                wc.DownloadFileAsync(new Uri(build.URL), $"{build.FolderName}/{build.ZipName}.zip");

                //For each change in progress, output progress to the wc_DownloadProgressChanged method
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WcDownloadProgressChanged);

                // For each update in the downloads progress, do this
                void WcDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                {
                    DownloadProgressBar.Value = e.ProgressPercentage;

                    if (Math.Abs(DownloadProgressBar.Value - 100) > 1) return;
                    wc.Dispose(); // Dispose of the web client
                    ToggleButtons(true); // Enable buttons
                    helper.ExtractBuild(build); // Go to the next step, extracting the downloaded build
                    ReactiveButtonText();
                }

                // If the web client could not download the zip, this code executes
                wc.Dispose(); // Dispose of the web client
            }
        }

        //Disables all the buttons such that the user cannot just spam click them during the update process
        public void ToggleButtons(bool status)
        {
            foreach (var x in ControlBtnGrid.Children)
            {
                if (x is Button btn)
                {
                    btn.IsEnabled = status;
                }
            }
        }

        // Call the Uninstall build function for a selected build
        private void DeleteBtnClick(object sender, RoutedEventArgs e)
        {
            if (_selected <= -1) return;

            Helper helper = new Helper();
            helper.UninstallBuild(_builds[_selected]);
            ReactiveButtonText();
        }

        private void ExplorerBtnClick(object sender, RoutedEventArgs e)
        {
            if (_selected <= -1) return;
            Helper helper = new Helper();
            helper.OpenLocation(_builds[_selected]);
        }
    }
}
