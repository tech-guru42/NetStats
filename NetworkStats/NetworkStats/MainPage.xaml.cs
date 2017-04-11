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
    public class Records
    {
        public string Name {get; set;}
        public int Amount {get; set;}
    }
    public sealed partial class MainPage : Page
    {
        List<Records> records = new List<Records>();
        List<ulong> data = new List<ulong>();
        public MainPage()
        {
            this.InitializeComponent();
            //dayChart();
            LoadChartContents();
        }

        public List<ulong> GetUsage(List<DateTime> period)
        {
            DateTime startTime = period[0];
            DateTime endTime = period[1];
            List<ulong> usageBytes = new List<ulong>();
            //Get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            var localUsage = connectionProfile.GetNetworkUsageAsync(startTime, endTime, DataUsageGranularity.Total, new NetworkUsageStates());
            var temp = localUsage.GetAwaiter();
            var usage = temp.GetResult().ElementAt(0);
            //Converts and formats the output string according to size B,KB,MB,GB using ByteSize Library
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
            List<DateTime> period = new List<DateTime>();
            period.Add(startTime);
            period.Add(currTime);
            List<ulong> usageData =  GetUsage(period);
            var download = ByteSize.FromBytes(usageData[0]);
            Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
            var upload = ByteSize.FromBytes(usageData[0]);
            Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);

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

        private void dayChart()
        {
            var currTime = DateTime.Now;
            records.Clear();
            List<DateTime> period = new List<DateTime>();
            //Task<List<String>> res = await GetPeriodUsage(period);
            for (int hour = currTime.Hour - 24; hour < currTime.Hour; hour++)
            {
                period.Clear();
                period.Add(DateTime.Now - TimeSpan.FromHours(24 - hour));
                period.Add(DateTime.Now - TimeSpan.FromHours(24 - hour + 1));
                List<ulong> data = GetPeriodUsage(period);

                records.Add(new Records()
                {
                    Name = period[0].Hour.ToString(),

                    Amount = unchecked((int)data[0] / 1024)
                }
                    );
            }
        }

        public async Task<IReadOnlyList<NetworkUsage>> GetPerUsage(List<DateTime> period)
        {
            DateTime startTime = period[0];
            DateTime endTime = period[1];
            
            //Get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            var localUsage = await connectionProfile.GetNetworkUsageAsync(startTime, endTime, DataUsageGranularity.Total, new NetworkUsageStates());
            /* var temp = localUsage.Result.GetResults();
             NetworkUsage usage = temp.ElementAt(0);

             */
            int i = 0;
            i++;
            return localUsage;
        }

        public List<ulong> GetPeriodUsage (List<DateTime> period)
        {
            List<DateTime> thePeriod = period;
            List<ulong> usageData = new List<ulong>();
            //List<ulong> usage = new List<ulong>();
            var usage = Task.Run(() => GetPerUsage(thePeriod)).Result;
            /*usageData.Add(usage.ElementAt(0).BytesReceived);
            usageData.Add(usage.ElementAt(0).BytesSent);*/
            return usageData;
        }


        private void LoadChartContents()
        {
            records.Add(new Records()
            {
                Name = "1", Amount = 100
            });
            records.Add(new Records()
            {
                Name = "2", Amount = 200
            });
            records.Add(new Records()
            {
                Name = "3", Amount = 300
            });
            records.Add(new Records()
            {
                Name = "4", Amount = 25
            });
            records.Add(new Records()
            {
                Name = "4",
                Amount = 25
            });
            records.Add(new Records()
            {
                Name = "5",
                Amount = 25
            });
            records.Add(new Records()
            {
                Name = "6",
                Amount = 25
            });
            records.Add(new Records()
            {
                Name = "7",
                Amount = 25
            });
            (ColumnChart.Series[0] as AreaSeries).ItemsSource = records;
            
        }
    }
}
