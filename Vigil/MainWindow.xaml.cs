﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vigil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Strings to hold input from TextBoxes
        string serverAddress;
        string familyName;
        string deviceName;

        // Dictionary holding FIND3 API calls
        // A function would likely work better, but for proof of concept, this should work.
        readonly Dictionary<string, string> find3ApiCalls = new Dictionary<string, string>()
        {
            { "simpleLocationOfSingleDevice", "/api/v1/location_basic/" }, // after appending /FAMILY/DEVICE gets location of device (loc), probability (p), and last seen in seconds (seen). Also gets GPS (lat) (lon)
        };
        
        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        // Fires when Button_GetDeviceLocation is clicked
        private void Button_GetDeviceLocation_Click(object sender, RoutedEventArgs e)
        {
            SimpleDevice simpleDevice;
            serverAddress = TextBox_ServerAddress.Text;
            familyName = TextBox_FamilyName.Text;
            deviceName = TextBox_DeviceName.Text;
            string response;
            

            string uri = serverAddress + find3ApiCalls["simpleLocationOfSingleDevice"] + familyName + "/" + deviceName;

            MessageBox.Show("URI built: " + uri);

            response = @Get(uri);
            MessageBox.Show( "GET response: " + response);
            MessageBox.Show("deserializing");

            simpleDevice = JsonSerializer.Deserialize<SimpleDevice>(response);
            MessageBox.Show("message: " + simpleDevice.message);
            MessageBox.Show("location: " + simpleDevice.data.loc);




        }

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
