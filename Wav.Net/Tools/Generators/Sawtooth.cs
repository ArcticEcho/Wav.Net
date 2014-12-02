using System;
using WavDotNet.Core;



namespace WavDotNet.Tools.Generators
{
	public class Sawtooth
	{
		private readonly uint sampleRate;



		public Sawtooth(uint sampleRate)
		{
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

			this.sampleRate = sampleRate;
		}



		public Samples<float> Generate32Bit(TimeSpan duration, uint frequency, float amplitude)
		{
			if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
			if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

			var samples = new float[(int)duration.TotalSeconds * sampleRate];
			var freq = sampleRate / frequency;
			var ii = -1f;
			var k = 2f / freq;

			for (var i = 0; i < samples.Length; i++)
			{
				samples[i] = ii * amplitude;
				ii += k;

				if (ii > 1)
				{
					ii = -1;
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
			var ii = -1.0;
			var k = 2.0 / freq;

			for (var i = 0; i < samples.Length; i++)
			{
				samples[i] = ii * amplitude;
				ii += k;

				if (ii > 1)
				{
					ii = -1;
				}
			}

			return new Samples<double>(samples);
		}
	}
}
