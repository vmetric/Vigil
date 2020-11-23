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
            var leftX = Canvas.GetLeft(Image_Pin);
            var topY = Canvas.GetTop(Image_Pin);
            
            if (leftX != newCoordinate.Item1 && topY != newCoordinate.Item2)
            {
                TranslateTransform trans = new TranslateTransform();
                Image_Pin.RenderTransform = trans;
                //DoubleAnimation anim1 = new DoubleAnimation(leftX, newCoordinate.Item1, TimeSpan.FromSeconds(0.25));
                //DoubleAnimation anim2 = new DoubleAnimation(topY, newCoordinate.Item2, TimeSpan.FromSeconds(0.25));
                //trans.BeginAnimation(TranslateTransform.YProperty, anim2);
                //trans.BeginAnimation(TranslateTransform.XProperty, anim1);
                
                // Move pin
                Canvas.SetLeft(Image_Pin, newCoordinate.Item1);
                Canvas.SetTop(Image_Pin, newCoordinate.Item2);
            }

        }
    }
}
