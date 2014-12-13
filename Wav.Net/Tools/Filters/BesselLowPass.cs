using System;
using WavDotNet.Core;



namespace WavDotNet.Tools.Filters
{
    public class BesselLowPass
    {
        
        private readonly uint sampleRate;


        public BesselLowPass(uint sampleRate)
        {
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

            this.sampleRate = sampleRate;
        }



        public Samples<double> Apply(Samples<double> samples, double cutoff)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }
            if (samples.Count == 0) { throw new ArgumentException("Samples must not be empty.", "samples"); }
            if (cutoff < 1 || cutoff > sampleRate / 2.0)
            {
                throw new ArgumentOutOfRangeException("cutoff", "Cutoff frequency must be between 0 and \"sampleRate\" / 2.");
            }

            var newSamples = new double[samples.Count];
            var sample = 0.0;
            var k = Math.Tan(Math.PI * cutoff / sampleRate);
            var k2 = k * k;

            var a0 = ((((105 * k + 105) * k + 45) * k + 10) * k + 1);
            var a1 = -(((420 * k + 210) * k2 - 20) * k - 4);
            var a2 = -((630 * k2 - 90) * k2 + 6);
            var a3 = -(((420 * k - 210) * k2 + 20) * k - 4);
            var a4 = -((((105 * k - 105) * k + 45) * k - 10) * k + 1);

            var b0 = 105 * k2 * k2;
            var b1 = 420 * k2 * k2;
            var b2 = 630 * k2 * k2;
            var b3 = 420 * k2 * k2;
            var b4 = 105 * k2 * k2;

            double output, state0 = 0.0, state1 = 0.0, state2 = 0.0, state3 = 0.0;

            for (var i = 0; i < samples.Count; i++)
            {
                sample = samples[i];
                output = b0 * sample + state0;
                state0 = b1 * sample + a1 / a0 * output + state1;
                state1 = b2 * sample + a2 / a0 * output + state2;
                state2 = b3 * sample + a3 / a0 * output + state3;
                state3 = b4 * sample + a4 / a0 * output;

                newSamples[i] = output;
            }

            return new Samples<double>(newSamples);
        }

        public Samples<float> Apply(Samples<float> samples, float cutoff)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }
            if (samples.Count == 0) { throw new ArgumentException("Samples must not be empty.", "samples"); }
            if (cutoff < 1 || cutoff > sampleRate / 2.0)
            {
                throw new ArgumentOutOfRangeException("cutoff", "Cutoff frequency must be between 0 and \"sampleRate\" / 2.");
            }

            var newSamples = new float[samples.Count];
            var sample = 0f;
            var k = (float)Math.Tan(Math.PI * cutoff / sampleRate);
            var k2 = k * k;

            var a0 = ((((105 * k + 105) * k + 45) * k + 10) * k + 1);
            var a1 = -(((420 * k + 210) * k2 - 20) * k - 4);
            var a2 = -((630 * k2 - 90) * k2 + 6);
            var a3 = -(((420 * k - 210) * k2 + 20) * k - 4);
            var a4 = -((((105 * k - 105) * k + 45) * k - 10) * k + 1);

            var b0 = 105 * k2 * k2;
            var b1 = 420 * k2 * k2;
            var b2 = 630 * k2 * k2;
            var b3 = 420 * k2 * k2;
            var b4 = 105 * k2 * k2;

            float output, state0 = 0f, state1 = 0f, state2 = 0f, state3 = 0f;

            for (var i = 0; i < samples.Count; i++)
            {
                sample = samples[i];
                output = b0 * sample + state0;
                state0 = b1 * sample + a1 / a0 * output + state1;
                state1 = b2 * sample + a2 / a0 * output + state2;
                state2 = b3 * sample + a3 / a0 * output + state3;
                state3 = b4 * sample + a4 / a0 * output;

                newSamples[i] = output;
            }

            return new Samples<float>(newSamples);
        }
    }
}
