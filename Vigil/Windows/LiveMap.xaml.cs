using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.ComponentModel;

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
        Timer updateLiveMapTimer; // Timer for handling periodic updates of the LiveMap.

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
            // Stops timer from running after the window is closed.
            Closing += OnWindowClosing;

            this.serverUri = serverUri;
            StartUpdating();
        }
        // Runs when the LiveMap window is closed.
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Stops our update timer.
            updateLiveMapTimer.Stop();
        }
        // Starts a timer to run Update.
        private void StartUpdating()
        {
            // Make timer responsible for periodic updates.
            updateLiveMapTimer = new Timer(updateInterval);
            // Add Update() to methods ran when timer fires.
            updateLiveMapTimer.Elapsed += Update;
            // Set timer to reset itself.
            updateLiveMapTimer.AutoReset = true;
            // Start timer.
            updateLiveMapTimer.Start();
        }
        // Updates LiveMap.
        public async void Update(object source, ElapsedEventArgs e)
        {
            await UpdateDeviceLocationAsync();
            // Update our label with new location.
            Dispatcher.Invoke(() => Label_Location.Content = deviceLocation);
            // Move our pin to the new location.
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
            // Make new HttpClient, have it talk to server, read the output.
            // TODO study documentation on this a bit more.
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);
            return (new StreamReader(stream).ReadToEnd());
        }
        // Handles movement of the pin.
        public void MovePin((int, int) newCoordinate)
        {
            var currentLeft = Canvas.GetLeft(Image_Pin); // Get current "from left" of Image_Pin.
            var currentTop = Canvas.GetTop(Image_Pin); // Get current "from top" of Image_Pin.
            var newLeft = newCoordinate.Item1; // Get new "from left" value.
            var newTop = newCoordinate.Item2; // Get new "from top" value.
            
            // If we aren't moving to where we already are, proceed
            if (currentLeft != newLeft && currentTop != newTop)
            {
                // If we are animating pin movements (i.e., if user has enabled animatePins in settings) proceed.
                if (animatePins)
                {
                    // New storyboard to hold our animations and such.
                    Storyboard storyboard = new Storyboard();

                    // Anim1 is responsible for left-right movement (using "distance from left").
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

                    storyboard.Begin(); // Start our storyboard & animations.
                } else
                {
                    // Move pin -- effectively "jumps" pin to the new location instead of animating.
                    Canvas.SetLeft(Image_Pin, newLeft); // Set Image_Pin "from left" to new "from left"
                    Canvas.SetTop(Image_Pin, newTop); // Set Image_Pin "from top" to new "from top"
                }

            }

        }
    }
}
