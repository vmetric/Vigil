using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vigil
{
    /// <summary>
    /// Interaction logic for LiveMap.xaml
    /// </summary>
    public partial class LiveMap : Window
    {
        // If true, the Pin will animate to new locations. If false, Pin jumps to new locations.
        bool animatePins = Settings.Default.animatePins;

        // Time, in seconds, it takes Pin animations to complete.
        double animationDurationSeconds = Settings.Default.animationDurationSeconds;

        // Dictionary holding the coordinates of each location. Current hardcoded.
        readonly Dictionary<string, (int, int)> locationCoordinates = new Dictionary<string, (int, int)>()
        {
            { "bedroom", (116, 446) },
            { "bathroom", (448, 198) },
            { "living room", (1022, 428) }
        };

        public LiveMap()
        {
            InitializeComponent();
        }

        public void Update(string newLocation)
        {
            Label_Location.Content = newLocation;
            MovePin(locationCoordinates[newLocation]);
        }

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
