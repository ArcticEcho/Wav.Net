using System;
using WavDotNet.Core;



namespace WavDotNet.Tools.Generators
{
	public class SquareWave
	{
		private readonly int sampleRate;



		public SquareWave(int sampleRate)
		{
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

			this.sampleRate = sampleRate;
		}



		public Samples<float> Generatee32Bit(TimeSpan duration, uint frequency, float amplitude)
		{
			if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
			if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

			var samples = new float[(int)duration.TotalSeconds * sampleRate];
			var freq = sampleRate / frequency;

			for (var i = 0; i < samples.Length; i++)
			{
				if (i % freq < freq / 2)
				{
					samples[i] = amplitude;
				}
				else
				{
					samples[i] = -amplitude;
				}
			}

			return new Samples<float>(samples);
		}

		public Samples<double> Generate64Bit(TimeSpan duration, uint frequency, double amplitude)
		{
			if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
			if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

			var samples = new double[(int)duration.TotalSeconds * sampleRate];
			var freq = sampleRate / frequency;

			for (var i = 0; i < samples.Length; i++)
			{
				if (i % freq < freq / 2)
				{
					samples[i] = amplitude;
				}
				else
				{
					samples[i] = -amplitude;
				}
			}

			return new Samples<double>(samples);
		}
	}
}
