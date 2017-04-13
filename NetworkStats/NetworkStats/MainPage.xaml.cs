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
    public sealed partial class MainPage : Page
    {
        List<Downloads> downloads = new List<Downloads>();
        List<Uploads> uploads = new List<Uploads>();
        List<ulong> data = new List<ulong>();
        public MainPage()
        {
            this.InitializeComponent();
            DownloadChart.Visibility = Visibility.Collapsed;
            UploadChart.Visibility = Visibility.Collapsed;
        }

        public List<ulong> GetUsage(DateTime start, DateTime end)
        {
            List<ulong> usageBytes = new List<ulong>();
            //Get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            var localUsage = connectionProfile.GetNetworkUsageAsync(start, end, DataUsageGranularity.Total, new NetworkUsageStates());
            var temp = localUsage.GetAwaiter();
            var usage = temp.GetResult().ElementAt(0);
            usageBytes.Add(usage.BytesReceived);
            usageBytes.Add(usage.BytesSent);
            return usageBytes;
        }

        private void day_Click(object sender, RoutedEventArgs e)
        {
            //Set end Time to now
            var currTime = DateTime.Now;
            //Set start Time to 24 hours before current time
            var startTime = currTime - TimeSpan.FromHours(24);
            List<ulong> usageData =  GetUsage(startTime, currTime);
            var download = ByteSize.FromBytes(usageData[0]);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usageData[1]);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
            downloads.Clear();
            uploads.Clear();
            List<DateTime> period = new List<DateTime>();
            for (int hour = 0; hour < 24; hour++)
            {
                period.Clear();
                period.Add(DateTime.Now.AddDays(-1).AddHours(hour));
                period.Add(DateTime.Now.AddDays(-1).AddHours(hour + 1));
                List<ulong> data = GetUsage(period[0], period[1]);
                data[0] = data[0] / 1024;
                int downTemp = unchecked((int)data[0]);
                downloads.Add(new Downloads()
                {
                    Name = period[0].Hour.ToString(),
                    download = downTemp,
                });
                data[1] = data[1] / 1024;
                int upTemp = unchecked((int)data[1]);
                uploads.Add(new Uploads()
                {
                    Name = period[0].Hour.ToString(),
                    upload = unchecked(upTemp)
                });
            }
            DownloadChart.Visibility = Visibility.Visible;
            UploadChart.Visibility = Visibility.Visible;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = null;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = downloads;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = null;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = uploads;
        }

        private void week_Click(object sender, RoutedEventArgs e)
        {
            var currTime = DateTime.Now;
            //Set start Time to 7 Days before current time
            var startTime = currTime - TimeSpan.FromDays(7);
            GetUsage(startTime, currTime);
            List<ulong> usageData = GetUsage(startTime, currTime);
            var download = ByteSize.FromBytes(usageData[0]);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usageData[1]);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
            #region
            downloads.Clear();
            uploads.Clear();
            List<DateTime> period = new List<DateTime>();
            for (int day = 0; day < 7; day++)
            {
                period.Clear();
                period.Add(DateTime.Now.AddDays(-7 + day));
                period.Add(DateTime.Now.AddDays(-6 + day));
                List<ulong> data = GetUsage(period[0], period[1]);
                data[0] = data[0] / 1024;
                int downTemp = unchecked((int)data[0]);
                downloads.Add(new Downloads()
                {
                    Name = period[0].DayOfWeek.ToString().Substring(0,2),
                    download = downTemp,
                });
                data[1] = data[1] / 1024;
                int upTemp = unchecked((int) data[1]);
                uploads.Add(new Uploads()
                {
                    Name = period[0].DayOfWeek.ToString().Substring(0,2),
                    upload = unchecked(upTemp)
                });
            }
            DownloadChart.Visibility = Visibility.Visible;
            UploadChart.Visibility = Visibility.Visible;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = null;
            (DownloadChart.Series[0] as AreaSeries).ItemsSource = downloads;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = null;
            (UploadChart.Series[0] as AreaSeries).ItemsSource = uploads;
            #endregion
        }

        private void month_Click(object sender, RoutedEventArgs e)
        {
            var currTime = DateTime.Now;
            //Set start Time to 1st of actual month
            var startTime = new DateTime(currTime.Year, currTime.Month, 1);
            GetUsage(startTime, currTime);
            List<ulong> usageData = GetUsage(startTime, currTime);
            var download = ByteSize.FromBytes(usageData[0]);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usageData[1]);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
            #region
            List<DateTime> period = new List<DateTime>();
            for (int day = 0; day < 7; day++)
            {
                period.Clear();
                period.Add(DateTime.Now.AddDays(-30 + day));
                period.Add(DateTime.Now.AddDays(-29 + day));
                List<ulong> data = GetUsage(period[0], period[1]);
                data[0] = data[0] / 1024;
                int downTemp = unchecked((int)data[0]);
                downloads.Add(new Downloads()
                {
                    Name = period[0].Day.ToString(),
                    download = downTemp,
                });
                data[1] = data[1] / 1024;
                int upTemp = unchecked((int)data[1]);
                uploads.Add(new Uploads()
                {
                    Name = period[0].Day.ToString(),
                    upload = unchecked(upTemp)
                });
            }
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
