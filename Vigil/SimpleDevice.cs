using System;
using System.Collections.Generic;
using System.Text;

namespace Vigil
{
    // this file contains classes necessary to handle the response to a Get simple location of a single device from a FIND3 server.

    // Overall response from server after GETing simpleDeviceLocation
    public class SimpleDevice
    {
        // "Data" section of response -- see below for class
        public data data { get; set; }
        // Message - e.g., "ok"
        public string message { get; set; }
        // Success - e.g., true
        public bool success { get; set; }
    }

    // "Data" section of response
    public class data
    {
        // Location - e.g., "kitchen" -- the server's current guess as to the device's location
        public string loc { get; set; }
        // GPS - e.g., gps[lat] -> 47.5675768678. Both = -1 if no GPS coordinates available.
        Dictionary<string, double> gps { get; set; }
        // Probability - e.g., 0.97 (=97%) -- how confident the server is that the device is at current location
        public float prob { get; set; }
        // [last] Seen - e.g., 1387 -- how many seconds since the device was last seen by the server
        public int seen { get; set; }

    }
}
