using System;



namespace WavDotNet.Tools
{
	static public class DecibelCalculator
	{
		public static double ToDecibels(double amplitude)
		{
			return Math.Log10(amplitude) * 20;
		}

		public static float ToDecibels(float amplitude)
		{
			return (float)Math.Log10(amplitude) * 20;
		}

		public static double ToAmplitude(double decibels)
		{
			return Math.Pow(10, decibels / 20.0);
		}

		public static float ToAmplitude(float decibels)
		{
			return (float)Math.Pow(10, decibels / 20.0);
		}
	}
}
