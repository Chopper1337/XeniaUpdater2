using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XeniaProject
{
    public partial class MainWindow : Window
    {
        // Create instance of the database
        DatabaseContainer db = new DatabaseContainer();

        //Create List of XeniaBuilds to hold the Xenia 
        List<XeniaBuild> builds = new List<XeniaBuild>();

        
        // Initialize DB
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
            // Add games to database
            // create query to list all the games and their data
            // fill data grid with that information

        }

        /* -------- Xenia Builds -------- */
        private void XeniaBuildsMainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected = XeniaBuildsMainList.SelectedIndex; // Selected item in list
            XeniaBuildsMainImage.Source = new BitmapImage(new Uri(builds[selected].ImagePath, UriKind.Relative)); // Set image source to image path
            XeniaBuildsNameLabel.Content = builds[selected].Name; // Set displayed name to the builds name
            XeniaBuildsDescTxblk.Text = builds[selected].Description; // Update the description

            string stability = builds[selected].StabilityRating.ToString();

            switch (stability){
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

        private void StartBTNClick(object sender, RoutedEventArgs e)
        {
            int selected = XeniaBuildsMainList.SelectedIndex;
            if(selected > -1)
            {
                Helper helper = new Helper();
                helper.StartProcess(builds[selected]);
            }
        }

        private void StopBTNClick(object sender, RoutedEventArgs e)
        {
            int selected = XeniaBuildsMainList.SelectedIndex;
            if(selected > -1)
            {
                try
                {
                    Process[] proc = Process.GetProcessesByName(builds[selected].ExecutableName);
                    proc[0].Kill();
                }
                catch
                {

                }
            }
        }

        private void UpdateBTNClick(object sender, RoutedEventArgs e)
        {
            int selected = XeniaBuildsMainList.SelectedIndex;
            if(selected > -1)
            {
                UpdateXenia(builds[selected]);
            }
        }

        private void UpdateXenia(XeniaBuild build)
        {
            Helper helper = new Helper();
            helper.CreateFolderStructure(build);
            DownloadFile(build);
        }

        //Downloads a file from a URL to a path with the file name you specify
        public void DownloadFile(XeniaBuild build)
        {
            Helper helper = new Helper();
            ToggleButtons(false);
            using (WebClient wc = new WebClient())
            {
                //Download from URL to location
                wc.DownloadFileAsync(new Uri(build.URL), $"{build.FolderName}/{build.ZipName}.zip");

                //For each change in progrress, output progress to the wc_DownloadProgressChanged method
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);

                // For each update in the downloads progress, do this
                void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                {
                    DownloadProgressBar.Value = e.ProgressPercentage;
                    //PercentageLBL.Text = $"{progressBar1.Value.ToString()}%";

                    if (DownloadProgressBar.Value == 100)
                    {
                        wc.Dispose();
                        ToggleButtons(true);
                        Helper h = new Helper();
                        h.ExtractBuild(build);
                    }
                }
                wc.Dispose();

            }
        }
        public void ToggleButtons(bool status)
        {
            StartBTN.IsEnabled = status;
            StopBTN.IsEnabled = status;
            UpdateBTN.IsEnabled = status;
        }

        private void DeleteBTNClick(object sender, RoutedEventArgs e)
        {
            int selected = XeniaBuildsMainList.SelectedIndex;
            if(selected > -1)
            {
                Helper helper = new Helper();
                helper.UninstallBuild(builds[selected]);
            }
        }

        /* -------- Games -------- */
    }
}
