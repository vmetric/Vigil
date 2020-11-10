using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;


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
            
            SimpleLocationOfDevice simpleDevice;
            
            string uri = "";
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

            // Update MainDisplay with given device location.
            TextBlock_MainDisplay.Text = $"Location Acquired: {simpleDevice.data.loc}";

        }

        public async Task<string> Get(string uri)
        {
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);
            return (new StreamReader(stream).ReadToEnd());
        }
    }
}
