using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCoreLoad
{
    class Core
    {
        private PerformanceCounter Parking;
        private PerformanceCounter Usage;

        public Core(int index)
        {
            try
            {
                Parking = new PerformanceCounter("Processor Information", "Parking Status", $"0,{index}");
                Console.WriteLine($"Parking:{Parking.NextValue()}");
                Usage = new PerformanceCounter("Processor Information", "% Processor Time", $"0,{index}");
                Console.WriteLine($"Usage:{Usage.NextValue()}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Parked()
        {
            return (Parking.NextValue() == 1) ? true : false;
        }

        public double Load()
        {
            return Usage.NextValue();
        }
    }
}
