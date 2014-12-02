using System;
using WavDotNet.Core;



namespace WavDotNet.Tools
{
	public class SilenceTrimmer
	{
		private readonly TimeSpan minDuration;
		private readonly double silenceThreshold;
		private readonly uint sampleRate;



		public SilenceTrimmer(uint sampleRate, TimeSpan minDuration, double silenceThreshold)
		{
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }
			if (silenceThreshold < 0 || silenceThreshold > 1) { throw new ArgumentOutOfRangeException("sampleRate", "Silence threshold must be between 0 and 1."); }

			this.sampleRate = sampleRate;
			this.minDuration = minDuration;
			this.silenceThreshold = silenceThreshold;
		}



		public Samples<float> Trim(Samples<float> samples)
		{
			if (samples == null) { throw new ArgumentNullException("samples"); }

			var silenceCount = 0;
			var silStart = 0;
			var silEnd = 0;

			for (var i = 0; i < samples.Count; i++)
			{
				if (Math.Abs(samples[i]) < silenceThreshold && silStart == -1)
				{
					silStart = i;
				}

				if (Math.Abs(samples[i]) > silenceThreshold && silStart != -1)
				{
					silEnd = i;

					if (silEnd - silStart > minDuration.TotalSeconds * sampleRate)
					{
						silenceCount += silEnd - silStart;
					}

					silStart = -1;
				}
			}

			var trimmed = new float[samples.Count - silenceCount];
			var k = 0;

			for (var i = 0; i < samples.Count; i++)
			{
				trimmed[i - k] = samples[i];

				if (Math.Abs(samples[i]) < silenceThreshold && silStart == -1)
				{
					silStart = i;
				}

				if (Math.Abs(samples[i]) > silenceThreshold && silStart != -1)
				{
					silEnd = i;

					if (silEnd - silStart > minDuration.TotalSeconds * sampleRate)
					{
						k  += silEnd - silStart;
					}

					silStart = -1;
				}
			}

			return new Samples<float>(trimmed);
		}
	}
}
