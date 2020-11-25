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
    /// Interaction logic for UserSettings.xaml
    /// </summary>
    public partial class UserSettings : Window
    {
        public UserSettings()
        {
            InitializeComponent();

            // Populate UserSettings with current settings.
            CheckBox_AnimatePinMovements.IsChecked = Settings.Default.animatePins;
            TextBox_AnimationDuration.Text = Settings.Default.animationDurationSeconds.ToString();
            TextBox_UpdateInterval.Text = Settings.Default.updateInterval.ToString();

        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.animatePins = (bool)CheckBox_AnimatePinMovements.IsChecked;
            Settings.Default.animationDurationSeconds = double.Parse(TextBox_AnimationDuration.Text);
            Settings.Default.updateInterval = int.Parse(TextBox_UpdateInterval.Text);
        }
    }
}
