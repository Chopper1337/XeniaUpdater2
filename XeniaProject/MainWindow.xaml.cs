using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Interface related code
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseContainer db = new DatabaseContainer();
        string currentFullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName, "");
        List<XeniaBuild> builds = new List<XeniaBuild>();

        
        // Initialize DB
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Creation of instances of XeniaBuild to fill the list with the different builds of Xenia
            XeniaBuild XeniaMaster = new XeniaBuild("Master", "URLHERE", "Xenia Master is the release intended for use by most people", "XeniaMaster", "\\images\\master.png", XeniaBuild.Stability.Stable, "xenia_master");
            XeniaBuild XeniaCanary = new XeniaBuild("Canary", "URLHERE", "Xenia Master is the release intended for use by most people", "XeniaMaster", "\\images\\canary.png", XeniaBuild.Stability.Unstable, "xenia_master");
            XeniaBuild XeniaPR = new XeniaBuild("Canary Ex", "URLHERE", "Xenia Canary PR is the experimental release of Xenia build on each pull request", "XeniaMaster", "\\images\\pr.png", XeniaBuild.Stability.Unsafe, "xenia_master");

            // Add these to the list
            builds.Add(XeniaMaster);
            builds.Add(XeniaCanary);
            builds.Add(XeniaPR);

            // XeniaBuildsMainList shows the contents of the created list
            XeniaBuildsMainList.ItemsSource = builds.ToList();
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
                    XeniaBuildsStabilityTxblk.Foreground = Brushes.Green;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void StartBTNClick(object sender, RoutedEventArgs e)
        {
            string selected = XeniaBuildsMainList.SelectedItem as string;
            Helper helper = new Helper();
            //helper.StartProcess($"{x[0].EXE}", $"{currentFullPath}/{x[0].folder}");
        }

        private void StopBTNClick(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBTNClick(object sender, RoutedEventArgs e)
        {

        }


        /* -------- Games -------- */
    }
}
