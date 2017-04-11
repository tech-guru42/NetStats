using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NetworkStats
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public static class BytesConvertor
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(this Int64 value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
        }
    }
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void GetUsage(List<DateTime> period)
        {
            DateTime startTime = period[0];
            DateTime endTime = period[1];
            //Get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            var localUsage = await connectionProfile.GetNetworkUsageAsync(startTime, endTime, DataUsageGranularity.Total, new NetworkUsageStates());
            var usage = localUsage[0];
            //Converts and formats the output string according to size B,KB,MB,GB using ByteSize Library
            var download = ByteSize.FromBytes(usage.BytesReceived);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usage.BytesSent);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
        }

        private void day_Click(object sender, RoutedEventArgs e)
        {
            //Set end Time to now
            var currTime = DateTime.Now;
            //Set start Time to 24 hours before current time
            var startTime = currTime - TimeSpan.FromHours(24);
            List<DateTime> period = new List<DateTime>();
            period.Add(startTime);
            period.Add(currTime);
            GetUsage(period);
        }

        private void week_Click(object sender, RoutedEventArgs e)
        {
            var currTime = DateTime.Now;
            //Set start Time to 7 Days before current time
            var startTime = currTime - TimeSpan.FromDays(7);
            List<DateTime> period = new List<DateTime>();
            period.Add(startTime);
            period.Add(currTime);
            GetUsage(period);
        }

        private void month_Click(object sender, RoutedEventArgs e)
        {
            var currTime = DateTime.Now;
            //Set start Time to 1st of actual month
            var startTime = new DateTime(currTime.Year, currTime.Month, 1);
            List<DateTime> period = new List<DateTime>();
            period.Add(startTime);
            period.Add(currTime);
            GetUsage(period);
        }
    }
}
