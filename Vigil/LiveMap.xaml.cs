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
        // Eventually user-settable, this variable will control if pins animate to their new location or simply jump.
        bool animatePins = true;
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
                    TranslateTransform trans = new TranslateTransform();
                    Image_Pin.RenderTransform = trans;
                    DoubleAnimation anim1 = new DoubleAnimation(currentLeft, newLeft, TimeSpan.FromSeconds(0.50));
                    anim1.FillBehavior = FillBehavior.Stop;
                    anim1.Completed += (s, e) => Canvas.SetLeft(Image_Pin, newLeft);
                    DoubleAnimation anim2 = new DoubleAnimation(currentTop, newTop, TimeSpan.FromSeconds(0.50));
                    anim2.FillBehavior = FillBehavior.Stop;
                    anim2.Completed += (s, e) => Canvas.SetTop(Image_Pin, newTop);

                    trans.BeginAnimation(TranslateTransform.XProperty, anim1);
                    trans.BeginAnimation(TranslateTransform.YProperty, anim2);
                } else
                {
                    //MessageBox.Show("Setting Left");

                    // Move pin
                    Canvas.SetLeft(Image_Pin, newLeft);
                    //MessageBox.Show("Setting Top");
                    Canvas.SetTop(Image_Pin, newTop);
                    //MessageBox.Show("done!");
                }

            }

        }
    }
}
