using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XeniaProject
{
    public partial class MainWindow : Window
    {
        // Create instance of the games database
        DatabaseContainer db = new DatabaseContainer();

        //Create List of XeniaBuilds to hold the Xenia 
        List<XeniaBuild> builds = new List<XeniaBuild>();

        // Int to hold the selected item in Xenia list
        int selected;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* -------- Xenia Builds -------- */
            // Creation of instances of XeniaBuild to fill the list with the different builds of Xenia
            XeniaBuild XeniaMaster = new XeniaBuild("Master", "https://ci.appveyor.com/api/projects/benvanik/xenia/artifacts/xenia_master.zip?branch=master&job=Configuration%3A%20Release&pr=false", "Xenia Master is the release intended for use by most people", "XeniaMaster", "\\images\\master.png", XeniaBuild.Stability.Stable, "xenia", "xenia_master");
            XeniaBuild XeniaCanary = new XeniaBuild("Canary", "https://github.com/xenia-canary/xenia-canary/releases/latest/download/xenia_canary.zip", "Xenia Canary is a build intended for testing features with a small subset of users.", "XeniaCanary", "\\images\\canary.png", XeniaBuild.Stability.Unstable, "xenia_canary", "xenia_canary");
            XeniaBuild XeniaPR = new XeniaBuild("Canary Ex", "https://ci.appveyor.com/api/projects/chris-hawley/xenia-canary/artifacts/xenia_canary.zip?branch=canary_experimental&job=Configuration:%20Release&pr=false", "Xenia Canary PR is the experimental release of Xenia build on each pull request", "XeniaPR", "\\images\\pr.png", XeniaBuild.Stability.Unsafe, "xenia_canary", "xenia_canary");

            // Add these to the list
            builds.Add(XeniaMaster);
            builds.Add(XeniaCanary);
            builds.Add(XeniaPR);

            // XeniaBuildsMainList shows the contents of the created list
            XeniaBuildsMainList.ItemsSource = builds.ToList();

            /* -------- Games -------- */
            // Set Games list to show all the games in the DB
            ResetGamesDG();

        }

        /* -------- Xenia Builds -------- */
        private void XeniaBuildsMainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = XeniaBuildsMainList.SelectedIndex; // Selected item in list
            XeniaBuildsMainImage.Source = new BitmapImage(new Uri(builds[selected].ImagePath, UriKind.Relative)); // Set image source to image path
            XeniaBuildsNameLabel.Content = builds[selected].Name; // Set displayed name to the builds name
            XeniaBuildsDescTxblk.Text = builds[selected].Description; // Update the description

            string stability = builds[selected].StabilityRating.ToString(); // Convert the stability rating of the game to a string

            // Depending on the stability, the text is set to a different colour
            // Stability can be only one of three possible values as it is an enum
            switch (stability)
            {
                case "Stable":
                    XeniaBuildsStabilityTxblk.Foreground = Brushes.LimeGreen;
                    XeniaBuildsStabilityTxblk.Text = $"Stability rating: {stability}";
                    break;
                case "Unstable":
                    XeniaBuildsStabilityTxblk.Foreground = Brushes.Yellow;
                    XeniaBuildsStabilityTxblk.Text = $"Stability rating: {stability}";
                    break;
                case "Unsafe":
                    XeniaBuildsStabilityTxblk.Foreground = Brushes.IndianRed;
                    XeniaBuildsStabilityTxblk.Text = $"Stability rating: {stability}";
                    break;
                default:
                    break;
            }

        }

        // Clicking the Start button 
        private void StartBTNClick(object sender, RoutedEventArgs e)
        {
            if (selected > -1) // If the user has selected an entry
            {
                Helper helper = new Helper(); // Create an instance of the helper class
                helper.StartProcess(builds[selected]); // Use the start process method to start the build's executable
            }
        }

        // Clicking stop button
        private void StopBTNClick(object sender, RoutedEventArgs e)
        {
            if (selected > -1)
            {
                Helper helper = new Helper();
                helper.StopProcess(builds[selected]); // Use method in helper to kill the process 
            }
        }

        // Update button clicked 
        private void UpdateBTNClick(object sender, RoutedEventArgs e)
        {
            if (selected > -1)
                UpdateXenia(builds[selected]); // Use method to update xenia
        }

        // Downloads the latest copy of a selected Xenia build
        private void UpdateXenia(XeniaBuild build)
        {
            if (XeniaBuildsMainList.SelectedIndex > -1)
            {
                Helper helper = new Helper();
                if (helper.InternetAvailable()) //Checks if there is a working internet connection
                {
                    helper.CreateFolderStructure(build); // Create the folder which the build will be downloaded to
                    DownloadFile(build); // Download the build
                }
                else
                    MessageBox.Show("Could not connected to server.\nPlease check you internet connection.", "Error");
            }
        }

        //Downloads a Xenia build
        public void DownloadFile(XeniaBuild build)
        {
            Helper helper = new Helper();
            ToggleButtons(false); // Disables buttons

            using (WebClient wc = new WebClient())
            {
                //Download from URL to location
                wc.DownloadFileAsync(new Uri(build.URL), $"{build.FolderName}/{build.ZipName}.zip");

                //For each change in progress, output progress to the wc_DownloadProgressChanged method
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);

                // For each update in the downloads progress, do this
                void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                {
                    DownloadProgressBar.Value = e.ProgressPercentage;

                    if (DownloadProgressBar.Value == 100)
                    {
                        wc.Dispose(); // Dispose of the web client
                        ToggleButtons(true); // Enable buttons
                        helper.ExtractBuild(build); // Go to the next step, extracting the downloaded build
                    }
                }

                // If the web client could not download the zip, this code executes
                wc.Dispose(); // Dispose of the web client
            }
        }
        //Disables all the buttons such that the user cannot just spam click them during the update process
        public void ToggleButtons(bool status)
        {
            StartBTN.IsEnabled = status;
            StopBTN.IsEnabled = status;
            UpdateBTN.IsEnabled = status;
            DeleteBTN.IsEnabled = status;
        }

        // Call the Uninstall build function for a selected build
        private void DeleteBTNClick(object sender, RoutedEventArgs e)
        {
            if (selected > -1)
            {
                Helper helper = new Helper();
                helper.UninstallBuild(builds[selected]);
            }
        }

        /* -------- Games -------- */

        //Each time a new character is entered into the search box, do this
        private void GamesSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = GamesSearchBox.Text;
            // If the string entered is not blank
            if (searchQuery.Length > 0)
            {
                // Query the database to search for the game by title
                var q = from a in db.Games
                        where a.Title.Contains(searchQuery)
                        select new
                        {
                            Title = a.Title,
                            Year = a.Year,
                            AgeRating = a.AgeRating,
                            Compatability = a.Compatability,
                            RunsBestOn = a.XeniaBuild
                        };
                GamesDG.ItemsSource = q.ToList();
            }
            else // Else, if text box is empty so reset the list box
                ResetGamesDG();
        }

        //Returns the Games list to its default state containing all games in the DB
        private void ResetGamesDG()
        {
            var q = from a in db.Games
                    select new
                    {
                        Title = a.Title,
                        Year = a.Year,
                        AgeRating = a.AgeRating,
                        Compatability = a.Compatability,
                        RunsBestOn = a.XeniaBuild
                    };
            GamesDG.ItemsSource = q.ToList();
        }
    }
}
