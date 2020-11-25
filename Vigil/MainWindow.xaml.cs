using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System;
using System.Threading;

namespace Vigil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Strings to hold input from TextBoxes
        // TODO replace textboxes with dropdowns, populated upon clicking a "connect to server button"
        string serverAddress;
        string familyName;
        string deviceName;
        string deviceLocation;
        LiveMap liveMap;
        SimpleLocationOfDevice simpleDevice;

        string uri = "";

        // UpdateLiveMap every X milliseconds
        int updateInterval = Settings.Default.updateInterval;

        // Dictionary holding FIND3 API calls
        // A function would likely work better, but for proof of concept, this should work.
        readonly Dictionary<string, string> find3ApiCalls = new Dictionary<string, string>()
        {
            { "simpleLocationOfSingleDevice", "api/v1/location_basic/" }, // after appending /FAMILY/DEVICE gets location of device (loc), probability (p), and last seen in seconds (seen). Also gets GPS (lat) (lon)
        };
        
        public MainWindow()
        {
            InitializeComponent();
        }

        // Fires when Button_GetDeviceLocation is clicked
        private async void Button_GetDeviceLocation_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_MainDisplay.Text = "Working...";
            

            serverAddress = TextBox_ServerAddress.Text;

            // If the first character of serverAddress is a number, it's safe to assume it's an IP address
            // And has been entered without http:// or https:// -- thus, we can add http:// to the front.
            // TODO verify if a GET request (below in Get()) will properly redirect from Http to Https
            if (char.IsNumber(serverAddress[0]))
            {
                serverAddress = serverAddress.Insert(0, "http://");
            } else if (!serverAddress.StartsWith("http://") || !serverAddress.StartsWith("https://")) // else, it does not begin with a number, so it must be a domain name. If it does not begin w/ http or https...
            {
                // Complain to user
                // TODO replace TextBlock with Label, and complain via MainDisplay. (Or, maybe, put TextBlock inside of label to acheive full content centering, while maintaining textwrapping?)
                MessageBox.Show("URLs must begin with http:// or https:// \nFor example: https://google.com");
                // Return, we don't want a broken URL
                return;
                // TODO maybe better if we correct the URL anyway, then get a confirmation from the user that the address is correct? This could be a "settings" option for user to check/uncheck.
            }

            familyName = TextBox_FamilyName.Text;
            deviceName = TextBox_DeviceName.Text;

            // Start building uri by adding serverAddress
            uri += serverAddress;
            // If serverAddress does NOT end with a /, add it to uri
            if (!serverAddress.EndsWith('/'))
            {
                uri += "/";
            }
            // Add the simpleLocationOfSingleDevice API call location, family name, slash, and deviceName.
            uri += find3ApiCalls["simpleLocationOfSingleDevice"] + familyName + "/" + deviceName;

            // GET from server, Deserialize JSON data (that is, convert it into a class simpleDevice)
            simpleDevice = JsonSerializer.Deserialize<SimpleLocationOfDevice>(await @Get(uri));
            // Save device location to variable
            deviceLocation = simpleDevice.data.loc;

            // Update MainDisplay with given device location.
            TextBlock_MainDisplay.Text = $"Location Acquired: {deviceLocation}";

            // Launch our LiveMap window, and give it our current location.
            liveMap = new LiveMap();
            liveMap.Show();
            liveMap.Update(simpleDevice.data.loc);


            System.Timers.Timer updateLiveMapTimer = new System.Timers.Timer(updateInterval);
            updateLiveMapTimer.Elapsed += UpdateLiveMap;
            updateLiveMapTimer.AutoReset = true;
            updateLiveMapTimer.Start();
            TextBlock_MainDisplay.Text = "updateLiveMapTimer is running";
        }

        // Simple method that handles GET'ing from server.
        private async Task<string> Get(string uri)
        {
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);
            return (new StreamReader(stream).ReadToEnd());
        }

        // Simple method to update deviceLocation
        private async Task UpdateDeviceLocation()
        {
            // GET from server, Deserialize JSON data (that is, convert it into a class simpleDevice)
            simpleDevice = JsonSerializer.Deserialize<SimpleLocationOfDevice>(await @Get(uri));
            // Save device location to variable
            deviceLocation = simpleDevice.data.loc;
        }

        // Method meant to be used for updating the "live map"
        private async void UpdateLiveMap(object source, ElapsedEventArgs e)
        {
            await UpdateDeviceLocation();

            if (deviceName != "" || deviceName != null)
            {
                Dispatcher.Invoke(() => liveMap.Update($"{simpleDevice.data.loc}"));
            } else
            {
                liveMap.Update("Err: deviceName empty");
            }
        }

        private void Toolbar_Settings_Clicked(object sender, RoutedEventArgs e)
        {
            new UserSettings().Show();
        }
    }
}
