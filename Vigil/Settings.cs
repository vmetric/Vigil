using System;
using System.Collections.Generic;
using System.Text;

namespace Vigil
{
    // This class represents various configuration options for the user to set.
    // It will be saved as a JSON file, then read back when adjusting settings.
    class Settings
    {
        // Should Pin movements be animated? True = yes, False = no
        public bool animatePins = true;
        // How long, in seconds, should Pin movement animations take?
        public double animationDurationSeconds = 0.5;
        // LiveMap updates every updateInterval, in milliseconds.
        public int updateInterval = 1000;
    }
}
