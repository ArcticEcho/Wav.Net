using System;
using WavDotNet.Core;



namespace WavDotNet.Tools
{
    public class PhaseShifter
    {
        private readonly uint sampleRate;



        public PhaseShifter(uint sampleRate)
        {
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

            this.sampleRate = sampleRate;
        }



        public Samples<double> Shift(Samples<double> samples, double depth = 4, double delay = 100, double rate = 0.1)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var newSamples = new double[samples.Count];

            double minWp, coef, x1;
            double lx1 = 0.0, ly1 = 0.0, lx2 = 0.0, ly2 = 0.0, lx3 = 0.0, ly3 = 0.0, lx4 = 0.0, ly4 = 0.0;

            // Calc params for sweeping filters.
            var wp = minWp = (Math.PI * delay) / sampleRate;
            var range = Math.Pow(2.0, depth);
            var maxWp = (Math.PI * delay * range) / sampleRate;
            rate = Math.Pow(range, rate / (sampleRate / 2.0));
            var sweepfac = rate;

            for (var i = 0; i < samples.Count; i++)
            {
                coef = (1.0 - wp) / (1.0 + wp);     // Calc coef for current freq.

                x1 = samples[i];
                ly1 = coef * (ly1 + x1) - lx1;      // Do 1st filter.
                lx1 = x1;
                ly2 = coef * (ly2 + ly1) - lx2;     // Do 2nd filter.
                lx2 = ly1;
                ly3 = coef * (ly3 + ly2) - lx3;     // Do 3rd filter. 
                lx3 = ly2;
                ly4 = coef * (ly4 + ly3) - lx4;     // Do 4th filter.
                lx4 = ly3;

                // Final output.

                newSamples[i] = ly4;

                wp *= sweepfac;            // Adjust freq of filters.

                if (wp > maxWp)            // Max?
                {
                    sweepfac = 1.0 / rate; // Sweep back down.
                }
                else
                {
                    if (wp < minWp)        // Min?
                    {
                        sweepfac = rate;   // Sweep back up.
                    }
                }
            }

            return new Samples<double>(newSamples);
        }

        public Samples<float> Shift(Samples<float> samples, float depth = 4, float delay = 100, float rate = 0.1f)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var newSamples = new float[samples.Count];

            float minWp, coef, x1;
            float lx1 = 0f, ly1 = 0f, lx2 = 0f, ly2 = 0f, lx3 = 0f, ly3 = 0f, lx4 = 0f, ly4 = 0f;

            // Calc params for sweeping filters.
            var wp = minWp = (float)((Math.PI * delay) / sampleRate);
            var range = (float)Math.Pow(2.0, depth);
            var maxWp = (float)((Math.PI * delay * range) / sampleRate);
            rate = (float)Math.Pow(range, rate / (sampleRate / 2f));
            var sweepfac = rate;

            for (var i = 0; i < samples.Count; i++)
            {
                coef = (1f - wp) / (1f + wp);     // Calc coef for current freq.

                x1 = samples[i];
                ly1 = coef * (ly1 + x1) - lx1;      // Do 1st filter.
                lx1 = x1;
                ly2 = coef * (ly2 + ly1) - lx2;     // Do 2nd filter.
                lx2 = ly1;
                ly3 = coef * (ly3 + ly2) - lx3;     // Do 3rd filter. 
                lx3 = ly2;
                ly4 = coef * (ly4 + ly3) - lx4;     // Do 4th filter.
                lx4 = ly3;

                // Final output.

                newSamples[i] = ly4;

                wp *= sweepfac;            // Adjust freq of filters.

                if (wp > maxWp)            // Max?
                {
                    sweepfac = 1f / rate; // Sweep back down.
                }
                else
                {
                    if (wp < minWp)        // Min?
                    {
                        sweepfac = rate;   // Sweep back up.
                    }
                }
            }

            return new Samples<float>(newSamples);
        }
    }
}
