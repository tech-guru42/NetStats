# NetworkStats
UWP to display WLAN and Cellular data usage

# Description
Application displays network data usage as table and charts as Download and Upload for current Internet connection.

# Usage
Choose usage period you want to see the statistics for by clicking on button for particular period. Statistics will be displayed as table and you can slide to see usage charts for upload and download. I used WinRTXaml toolkit for that. Total usage data are manipulated using ByteSize toolkit. That makes data more readable as they are displatyed in appropriate format of MB, GB, ect. First time app is launched short About message is displayed. Once you select period of usage you want to display, app will remember last choice selected and when relaunched it will display same period usage on a start up. I am using localsetting to store that choice. Charts data are displayed in MB as default for better value reading. 
