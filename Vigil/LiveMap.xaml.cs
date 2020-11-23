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
            var top = Canvas.GetTop(Image_Pin);
            var left = Canvas.GetLeft(Image_Pin);
            TranslateTransform trans = new TranslateTransform();
            Image_Pin.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(top, newCoordinate.Item1 - top, TimeSpan.FromSeconds(0.25));
            DoubleAnimation anim2 = new DoubleAnimation(left, newCoordinate.Item2 - left, TimeSpan.FromSeconds(0.25));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }
    }
}
