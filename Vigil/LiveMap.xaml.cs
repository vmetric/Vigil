using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vigil
{
    /// <summary>
    /// Interaction logic for LiveMap.xaml
    /// </summary>
    public partial class LiveMap : Window
    {
        public LiveMap()
        {
            InitializeComponent();
        }

        public void Update(string newLocation)
        {
            Label_Location.Content = newLocation;
        }
    }
}
