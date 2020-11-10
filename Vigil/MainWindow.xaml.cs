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

            serverAddress = TextBox_ServerAddress.Text;
            familyName = TextBox_FamilyName.Text;
            deviceName = TextBox_DeviceName.Text;
            
            //TODO check if trailing / exists in serverAddress before adding
            string uri = serverAddress + "/" + find3ApiCalls["simpleLocationOfSingleDevice"] + familyName + "/" + deviceName;

            simpleDevice = JsonSerializer.Deserialize<SimpleLocationOfDevice>(await @Get(uri));

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
