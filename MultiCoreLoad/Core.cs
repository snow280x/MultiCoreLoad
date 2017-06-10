using System;
using System.Diagnostics;

namespace MultiCoreLoad
{
	class Core : IDisposable
	{
		private PerformanceCounter Parking;
		private PerformanceCounter Usage;
		private PerformanceCounter Freqency;

		public Core(int index)
		{
			Parking = new PerformanceCounter("Processor Information", "Parking Status", $"0,{index}");
			Debug.WriteLine($"Parking:{Parking.NextValue()}");
			Usage = new PerformanceCounter("Processor Information", "% Processor Time", $"0,{index}");
			Debug.WriteLine($"Usage:{Usage.NextValue()}");
			Freqency = new PerformanceCounter("Processor Information", "% Processor Performance", $"0,{index}");
			Debug.WriteLine($"Frequency:{Freqency.NextValue()}");
		}

		public bool Parked()
		{
			return (Parking.NextValue() == 1) ? true : false;
		}

		public double Load()
		{
			return Usage.NextValue();
		}

		public double Freq()
		{
			return Freqency.NextValue();
		}

		public void Dispose()
		{
			Parking.Dispose();
			Usage.Dispose();
			Freqency.Dispose();
		}
	}
}
