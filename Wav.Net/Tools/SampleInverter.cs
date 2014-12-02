using WavDotNet.Core;



namespace WavDotNet.Tools
{
	public static class SampleInverter
	{
		public static Samples<float> Invert(Samples<float> samples)
		{
			var inverted = new float[samples.Count];

			for (var i = 0; i < samples.Count; i++)
			{
				inverted[i] = -samples[i];
			}

			return new Samples<float>(inverted);
		}

		public static float[] Invert(float[] samples)
		{
			var inverted = new float[samples.Length];

			for (var i = 0; i < samples.Length; i++)
			{
				inverted[i] = -samples[i];
			}

			return inverted;
		}

		public static Samples<double> Invert(Samples<double> samples)
		{
			var inverted = new double[samples.Count];

			for (var i = 0; i < samples.Count; i++)
			{
				inverted[i] = -samples[i];
			}

			return new Samples<double>(inverted);
		}

		public static double[] Invert(double[] samples)
		{
			var inverted = new double[samples.Length];

			for (var i = 0; i < samples.Length; i++)
			{
				inverted[i] = -samples[i];
			}

			return inverted;
		}
	}
}
