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
            TestProcAsync();
        }

        public async void TestProcAsync()
        {
            //Set end Time to now
            var currTime = DateTime.Now;

            //Set start Time to 1 hour before current time
            var startTime = currTime - TimeSpan.FromHours(24);
            var startOfMonth = new DateTime(currTime.Year, currTime.Month, 1);

            //Get the ConnectionProfile that is currently used to connect to the Internet
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            var localUsage = await connectionProfile.GetNetworkUsageAsync(startOfMonth, currTime, DataUsageGranularity.Total, new NetworkUsageStates());

            
                var usage = localUsage[0];
                var download = ByteSize.FromBytes(usage.BytesReceived);
                Download.Text = download.ToString("##,#", CultureInfo.InvariantCulture);
                var upload = ByteSize.FromBytes(usage.BytesSent);
                Upload.Text = upload.ToString("##,#", CultureInfo.InvariantCulture);
                /*Debug.WriteLine("Local Data Usage: \n\r"
                    + "Bytes Sent: " + usage.BytesSent + "\n\r"
                    + "Bytes Received: " + usage.BytesReceived + "\n\r");*/
            
        }

    }
}
