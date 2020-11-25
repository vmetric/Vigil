using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.IO;
using System.Timers;

namespace Vigil
{
    /// <summary>
    /// Interaction logic for LiveMap.xaml
    /// </summary>
    public partial class LiveMap : Window
    {
        string deviceLocation; // Device's location.
        string serverUri; // URI/L of FIND3 server.
        SimpleLocationOfDevice deviceInfoSimple; // Simple version of device info from FIND3 server.

        bool animatePins = Settings.Default.animatePins; // If true, the Pin will animate to new locations. If false, Pin jumps to new locations.
        double animationDurationSeconds = Settings.Default.animationDurationSeconds; // Time, in seconds, it takes Pin animations to complete.
        int updateInterval = Settings.Default.updateInterval;
        Dictionary<string, (int, int)> locationCoordinates = new Dictionary<string, (int, int)>()
        {
            { "bedroom", (116, 446) },
            { "bathroom", (448, 198) },
            { "living room", (1022, 428) }
        }; // Holds the coordinates of each location. Currently hardcoded.

        public LiveMap(string serverUri)
        {
            InitializeComponent();

            this.serverUri = serverUri;
            StartUpdating();
        }
        // Starts a timer to run Update.
        private void StartUpdating()
        {
            Timer updateLiveMapTimer = new Timer(updateInterval);
            updateLiveMapTimer.Elapsed += Update;
            updateLiveMapTimer.AutoReset = true;
            updateLiveMapTimer.Start();
        }
        // Updates LiveMap.
        public async void Update(object source, ElapsedEventArgs e)
        {
            await UpdateDeviceLocationAsync();
            Dispatcher.Invoke(() => Label_Location.Content = deviceLocation);
            Dispatcher.Invoke(() => MovePin(locationCoordinates[deviceLocation]));
        }
        // Updates deviceLocation when called.
        private async Task UpdateDeviceLocationAsync()
        {
            if (serverUri != null && serverUri != "")
            {
                // GET from server, Deserialize JSON data (that is, convert it into a class simpleDevice)
                deviceInfoSimple = JsonSerializer.Deserialize<SimpleLocationOfDevice>(await @Get(serverUri));
                // Update deviceLocation
                deviceLocation = deviceInfoSimple.data.loc;
            }
        }
        // Handles GET'ing from server.
        private async Task<string> Get(string uri)
        {
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);
            return (new StreamReader(stream).ReadToEnd());
        }
        // Handles movement of the pin.
        public void MovePin((int, int) newCoordinate)
        {
            var currentLeft = Canvas.GetLeft(Image_Pin);
            var currentTop = Canvas.GetTop(Image_Pin);
            var newLeft = newCoordinate.Item1;
            var newTop = newCoordinate.Item2;
            
            if (currentLeft != newLeft && currentTop != newTop)
            {
                if (animatePins)
                {
                    Storyboard storyboard = new Storyboard();

                    // Anim1 is responsible for left-right movement (using "distance from left")
                    DoubleAnimation anim1 = new DoubleAnimation(currentLeft, newLeft, TimeSpan.FromSeconds(animationDurationSeconds));
                    anim1.FillBehavior = FillBehavior.Stop;
                    anim1.Completed += (s, e) => Canvas.SetLeft(Image_Pin, newLeft);
                    Storyboard.SetTarget(anim1, Image_Pin);
                    Storyboard.SetTargetProperty(anim1, new PropertyPath(Canvas.LeftProperty));
                    storyboard.Children.Add(anim1);
                    // Anim2 is responsible for up-down movement (using "distance from top")
                    DoubleAnimation anim2 = new DoubleAnimation(currentTop, newTop, TimeSpan.FromSeconds(animationDurationSeconds));
                    anim2.FillBehavior = FillBehavior.Stop;
                    anim2.Completed += (s, e) => Canvas.SetTop(Image_Pin, newTop);
                    Storyboard.SetTarget(anim2, Image_Pin);
                    Storyboard.SetTargetProperty(anim2, new PropertyPath(Canvas.TopProperty));
                    storyboard.Children.Add(anim2);

                    storyboard.Begin();
                } else
                {
                    // Move pin
                    Canvas.SetLeft(Image_Pin, newLeft);
                    Canvas.SetTop(Image_Pin, newTop);
                }

            }

        }
    }
}
