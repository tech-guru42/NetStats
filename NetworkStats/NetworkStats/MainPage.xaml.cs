using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NetworkStats
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    #region Chart Structures
    //Structures for Chart in WinRTXaml data
    public class Downloads
    {
        public string Name {get; set;}
        public int download {get; set;}
    }
    
    public class Uploads
    {
        public string Name { get; set; }
        public int upload { get; set; }
    }
    #endregion
    public sealed partial class MainPage : Page
    {
        List<Downloads> downloads = new List<Downloads>();
        List<Uploads> uploads = new List<Uploads>();
        List<ulong> data = new List<ulong>();
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public MainPage()
        {
            this.InitializeComponent();
            //Dissable all contant in Download and Upload pivots
            downPivot.Visibility = Visibility.Collapsed;
            UpPivot.Visibility = Visibility.Collapsed;
            #region LocalSetting
            //Create if not exist container in localsetting
            localSettings.CreateContainer("NetStats", Windows.Storage.ApplicationDataCreateDisposition.Always);
            //Check if localsetting container exist, basically if app is running for the fisrt time
            if (localSettings.Containers["NetStats"].Values.Any())
            {
                //Gets last checked usage and start app in that same usage mode
                String option = localSettings.Containers["NetStats"].Values["LastUsage"].ToString();
                switch (option)
                {
                    case "day":
                        dayUsage();
                        break;
                    case "week":
                        weekUsage();
                        break;
                    case "month":
                        monthUsage();
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }

        public List<ulong> GetUsage(DateTime start, DateTime end)
        {
            List<ulong> usageBytes = new List<ulong>();
            //Get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            // Async call to get usage of current internet connection Wifi or Mobile data
            var localUsage = connectionProfile.GetNetworkUsageAsync(start, end, DataUsageGranularity.Total, new NetworkUsageStates());
            //Waits for the async call to finish
            var temp = localUsage.GetAwaiter();
            //Return Task<List<String>>. Get result extracts List<String> from task.
            var usage = temp.GetResult().ElementAt(0);
            //Populate Ulong list with usage data and returns it
            usageBytes.Add(usage.BytesReceived);
            usageBytes.Add(usage.BytesSent);
            return usageBytes;
        }
        #region Buttons
        private void day_Click(object sender, RoutedEventArgs e)
        {
            dayUsage();
            localSettings.Containers["NetStats"].Values["LastUsage"] = "day";
        }

        private void week_Click(object sender, RoutedEventArgs e)
        {
            weekUsage();
            localSettings.Containers["NetStats"].Values["LastUsage"] = "week";
        }

        private void month_Click(object sender, RoutedEventArgs e)
        {
            monthUsage();
            localSettings.Containers["NetStats"].Values["LastUsage"] = "month";
        }
        #endregion
        private void dayUsage()
        {
            loadingImage.Visibility = Visibility.Visible;
            //Set end Time to now
            var currTime = DateTime.Now;
            //Set start Time to 24 hours before current time
            var startTime = currTime - TimeSpan.FromHours(24);
            List<DateTime> period = new List<DateTime>();
            List<String> dataList = new List<string>();
            usageTable.ItemsSource = null;
            //Gets usage data for period
            List<ulong> usageData = GetUsage(startTime, currTime);
            //Using ByteSize to convert usage data to easy to read data. Converts to MB, GB, TB ect...
            var download = ByteSize.FromBytes(usageData[0]);
            var upload = ByteSize.FromBytes(usageData[1]);
            //Display total usage data
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
            //Clears Data lists for Charts to avoid data from previous period mixing in
            downloads.Clear();
            uploads.Clear();
            //Do 24 hour loop for getting usage data
            for (int hour = 0; hour < 24; hour++)
            {
                period.Clear();
                //Beggining of usage period
                period.Add(DateTime.Now.AddDays(-1).AddHours(hour));
                //End of usage period
                period.Add(DateTime.Now.AddDays(-1).AddHours(hour + 1));
                //Getting usage for beggining to the end of period set
                List<ulong> data = GetUsage(period[0], period[1]);
                //
                dataList.Add(period[1].ToString() + " Download: " + ByteSize.FromBytes(data[0]) + " Upload: " + ByteSize.FromBytes(data[1]));
                data[0] = data[0] / 1024;
                int downTemp = unchecked((int)data[0]);
                downloads.Add(new Downloads()
                {
                    Name = period[1].Hour.ToString(),
                    download = downTemp,
                });
                data[1] = data[1] / 1024;
                int upTemp = unchecked((int)data[1]);
                uploads.Add(new Uploads()
                {
                    Name = period[1].Hour.ToString(),
                    upload = unchecked(upTemp)
                });
            }
            #region Change GUI
            //Makes content of download and upload chart pivot visible
            downPivot.Visibility = Visibility.Visible;
            UpPivot.Visibility = Visibility.Visible;
            loadingImage.Visibility = Visibility.Collapsed;
            //Populate Listbox with usage data. Close texbox with "About information" and makes Listbox with usage data visible 
            usageTable.ItemsSource = dataList;
            AboutBox.Visibility = Visibility.Collapsed;
            usageTable.Visibility = Visibility.Visible;
            //Clears Charts from possible previous data and assaign new data to it.
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = null;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = downloads;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = null;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = uploads;
            #endregion
        }

        private void weekUsage()
        {
            loadingImage.Visibility = Visibility.Visible;
            var currTime = DateTime.Now;
            List<String> dataList = new List<string>();
            usageTable.ItemsSource = null;
            //Set start Time to 7 Days before current time
            var startTime = currTime - TimeSpan.FromDays(7);
            GetUsage(startTime, currTime);
            List<ulong> usageData = GetUsage(startTime, currTime);
            var download = ByteSize.FromBytes(usageData[0]);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usageData[1]);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
            downloads.Clear();
            uploads.Clear();
            List<DateTime> period = new List<DateTime>();
            for (int day = 0; day < 7; day++)
            {
                period.Clear();
                period.Add(DateTime.Now.AddDays(-7 + day));
                period.Add(DateTime.Now.AddDays(-6 + day));
                List<ulong> data = GetUsage(period[0], period[1]);
                dataList.Add(period[1].ToString() + " Download: " + ByteSize.FromBytes(data[0]) + " Upload: " + ByteSize.FromBytes(data[1]));
                data[0] = data[0] / 1024;
                int downTemp = unchecked((int)data[0]);
                downloads.Add(new Downloads()
                {
                    Name = period[1].DayOfWeek.ToString().Substring(0, 2),
                    download = downTemp,
                });
                data[1] = data[1] / 1024;
                int upTemp = unchecked((int)data[1]);
                uploads.Add(new Uploads()
                {
                    Name = period[1].DayOfWeek.ToString().Substring(0, 2),
                    upload = unchecked(upTemp)
                });
            }
            #region GUI change
            downPivot.Visibility = Visibility.Visible;
            UpPivot.Visibility = Visibility.Visible;
            loadingImage.Visibility = Visibility.Collapsed;
            usageTable.ItemsSource = dataList;
            AboutBox.Visibility = Visibility.Collapsed;
            usageTable.Visibility = Visibility.Visible;
            DownloadChart.Visibility = Visibility.Visible;
            UploadChart.Visibility = Visibility.Visible;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = null;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = downloads;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = null;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = uploads;
            #endregion
        }

        private void monthUsage()
        {
            loadingImage.Visibility = Visibility.Visible;
            List<String> dataList = new List<string>();
            usageTable.ItemsSource = null;
            downloads.Clear();
            uploads.Clear();
            var currTime = DateTime.Now;
            //Set start Time to 1st of actual month
            var startTime = new DateTime(currTime.Year, currTime.Month, 1);
            GetUsage(startTime, currTime);
            List<ulong> usageData = GetUsage(startTime, currTime);
            var download = ByteSize.FromBytes(usageData[0]);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usageData[1]);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
            List<DateTime> period = new List<DateTime>();
            for (int day = 0; day < 30; day++)
            {
                period.Clear();
                period.Add(DateTime.Now.AddDays(-30 + day));
                period.Add(DateTime.Now.AddDays(-29 + day));
                List<ulong> data = GetUsage(period[0], period[1]);
                dataList.Add(period[1].ToString() + " Download: " + ByteSize.FromBytes(data[0]) + " Upload: " + ByteSize.FromBytes(data[1]));
                data[0] = data[0] / 1024;
                int downTemp = unchecked((int)data[0]);
                downloads.Add(new Downloads()
                {
                    Name = period[1].Day.ToString(),
                    download = downTemp,
                });
                data[1] = data[1] / 1024;
                int upTemp = unchecked((int)data[1]);
                uploads.Add(new Uploads()
                {
                    Name = period[1].Day.ToString(),
                    upload = unchecked(upTemp)
                });
            }
            #region GUI change
            downPivot.Visibility = Visibility.Visible;
            UpPivot.Visibility = Visibility.Visible;
            usageTable.ItemsSource = dataList;
            AboutBox.Visibility = Visibility.Collapsed;
            usageTable.Visibility = Visibility.Visible;
            loadingImage.Visibility = Visibility.Collapsed;
            DownloadChart.Visibility = Visibility.Visible;
            UploadChart.Visibility = Visibility.Visible;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = null;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = downloads;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = null;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = uploads;
            #endregion
        }
    }
}
