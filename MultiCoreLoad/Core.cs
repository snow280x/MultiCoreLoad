using System;
using System.Diagnostics;

namespace MultiCoreLoad
{
    internal class Core : IDisposable
    {
        private readonly PerformanceCounter Parking;
        private readonly PerformanceCounter Usage;
        private readonly PerformanceCounter Freqency;
        private readonly PerformanceCounter IdleTime;

        public Core(int index)
        {
            Parking = new PerformanceCounter("Processor Information", "Parking Status", $"0,{index}");
            Debug.WriteLine($"Core {index} Parking:{Parking.NextValue()}");

            Usage = new PerformanceCounter("Processor Information", "% Processor Time", $"0,{index}");
            Debug.WriteLine($"Core {index} Usage:{Usage.NextValue()}");

            Freqency = new PerformanceCounter("Processor Information", "% Processor Performance", $"0,{index}");
            Debug.WriteLine($"Core {index} Frequency:{Freqency.NextValue()}");

            IdleTime = new PerformanceCounter("Processor Information", "% Idle Time", $"0,{index}");
            Debug.WriteLine($"Core {index} Idle:{IdleTime.NextValue()}");
        }

        public bool Parked()
        {
            return (Parking.NextValue() == 1) ? true : false;
        }

        public double Load()
        {
            return ValueClip(0, Usage.NextValue(), 100);
        }

        public double Freq()
        {
            return ValueClip(0, Freqency.NextValue(), 1000);
        }

        public double Idle()
        {
            return ValueClip(0, IdleTime.NextValue(), 100);
        }

        public void Dispose()
        {
            Parking.Dispose();
            Usage.Dispose();
            Freqency.Dispose();
            IdleTime.Dispose();
        }

        private double ValueClip(double min, double value, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
