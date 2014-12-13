using System;
using WavDotNet.Core;



namespace WavDotNet.Tools.Generators
{
    public class WhiteNoise
    {
        private readonly int sampleRate;



        public WhiteNoise(int sampleRate)
        {
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

            this.sampleRate = sampleRate;
        }



        public Samples<float> Generate32Bit(TimeSpan duration, float amplitude)
        {
            if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }

            var samples = new float[(int)duration.TotalSeconds * sampleRate];
            var r = new Random();

            for (var i = 0; i < samples.Length; i++)
            {
                samples[i] = (float)r.NextDouble() * amplitude;
            }

            return new Samples<float>(samples);
        }

        public Samples<double> Generate64Bit(TimeSpan duration, double amplitude)
        {
            if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }

            var samples = new double[(int)duration.TotalSeconds * sampleRate];
            var r = new Random();

            for (var i = 0; i < samples.Length; i++)
            {
                samples[i] = r.NextDouble() * amplitude;
            }

            return new Samples<double>(samples);
        }
    }
}
