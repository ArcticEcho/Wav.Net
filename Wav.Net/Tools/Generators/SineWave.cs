using System;
using WavDotNet.Core;



namespace WavDotNet.Tools.Generators
{
	public class SineWave
	{
		private readonly int sampleRate;



		public SineWave(int sampleRate)
		{
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

			this.sampleRate = sampleRate;
		}



		public Samples<float> SineWave32Bit(TimeSpan duration, uint frequency, float amplitude)
		{
			if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
			if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

			var samples = new float[(int)duration.TotalSeconds * 10];
			var k = (float)(Math.PI * 2 * frequency) / sampleRate;

			for (var i = 0; i < samples.Length; i++)
			{
				samples[i] = (float)Math.Sin(k * i) * amplitude;
			}

			return new Samples<float>(samples);
		}

		public Samples<double> SineWave64Bit(TimeSpan duration, uint frequency, double amplitude)
		{
			if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
			if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

			var samples = new double[(int)duration.TotalSeconds * 10];
			var k = (Math.PI * 2 * frequency) / sampleRate;

			for (var i = 0; i < samples.Length; i++)
			{
				samples[i] = Math.Sin(k * i) * amplitude;
			}

			return new Samples<double>(samples);
		}
	}
}
