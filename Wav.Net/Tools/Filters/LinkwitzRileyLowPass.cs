/*
 * Wav.Net. A .Net 2.0 based library for transcoding ".wav" (wave) files.
 * Copyright © 2014, ArcticEcho.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */



using System;
using WavDotNet.Core;

namespace WavDotNet.Tools.Filters
{
    public class LinkwitzRileyLowPass
    {
        private readonly uint sampleRate;



        public LinkwitzRileyLowPass(uint sampleRate)
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
            var wc = 2 * Math.PI * cutoff;
            var wc2 = wc * wc;
            var wc3 = wc2 * wc;
            var wc4 = wc2 * wc2;
            var k = wc / Math.Tan(Math.PI * cutoff / sampleRate);
            var k2 = k * k;
            var k3 = k2 * k;
            var k4 = k2 * k2;
            var sqrt2 = Math.Sqrt(2);
            var sqTmp1 = sqrt2 * wc3 * k;
            var sqTmp2 = sqrt2 * wc * k3;
            var aTmp = 4 * wc2 * k2 + 2 * sqTmp1 + k4 + 2 * sqTmp2 + wc4;

            var b1 = (4 * (wc4 + sqTmp1 - k4 - sqTmp2)) / aTmp;
            var b2 = (6 * wc4 - 8 * wc2 * k2 + 6 * k4) / aTmp;
            var b3 = (4 * (wc4 - sqTmp1 + sqTmp2 - k4)) / aTmp;
            var b4 = (k4 - 2 * sqTmp1 + wc4 - 2 * sqTmp2 + 4 * wc2 * k2) / aTmp;

            var a0 = wc4 / aTmp;
            var a1 = 4 * wc4 / aTmp;
            var a2 = 6 * wc4 / aTmp;
            var a3 = a1;
            var a4 = a0;

            double ym1 = 0.0, ym2 = 0.0, ym3 = 0.0, ym4 = 0.0, xm1 = 0.0, xm2 = 0.0, xm3 = 0.0, xm4 = 0.0, tempy;

            for (var i = 0; i < samples.Count; i++)
            {
                var tempx = samples[i];

                tempy = a0 * tempx + a1 * xm1 + a2 * xm2 + a3 * xm3 + a4 * xm4 - b1 * ym1 - b2 * ym2 - b3 * ym3 - b4 * ym4;
                xm4 = xm3;
                xm3 = xm2;
                xm2 = xm1;
                xm1 = tempx;
                ym4 = ym3;
                ym3 = ym2;
                ym2 = ym1;
                ym1 = tempy;

                newSamples[i] = tempy;
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
            var wc = (float)(2 * Math.PI * cutoff);
            var wc2 = wc * wc;
            var wc3 = wc2 * wc;
            var wc4 = wc2 * wc2;
            var k = (float)(wc / Math.Tan(Math.PI * cutoff / sampleRate));
            var k2 = k * k;
            var k3 = k2 * k;
            var k4 = k2 * k2;
            var sqrt2 = (float)Math.Sqrt(2);
            var sqTmp1 = sqrt2 * wc3 * k;
            var sqTmp2 = sqrt2 * wc * k3;
            var aTmp = 4 * wc2 * k2 + 2 * sqTmp1 + k4 + 2 * sqTmp2 + wc4;

            var b1 = (4 * (wc4 + sqTmp1 - k4 - sqTmp2)) / aTmp;
            var b2 = (6 * wc4 - 8 * wc2 * k2 + 6 * k4) / aTmp;
            var b3 = (4 * (wc4 - sqTmp1 + sqTmp2 - k4)) / aTmp;
            var b4 = (k4 - 2 * sqTmp1 + wc4 - 2 * sqTmp2 + 4 * wc2 * k2) / aTmp;

            var a0 = wc4 / aTmp;
            var a1 = 4 * wc4 / aTmp;
            var a2 = 6 * wc4 / aTmp;
            var a3 = a1;
            var a4 = a0;

            float ym1 = 0f, ym2 = 0f, ym3 = 0f, ym4 = 0f, xm1 = 0f, xm2 = 0f, xm3 = 0f, xm4 = 0f, tempy;

            for (var i = 0; i < samples.Count; i++)
            {
                var tempx = samples[i];

                tempy = a0 * tempx + a1 * xm1 + a2 * xm2 + a3 * xm3 + a4 * xm4 - b1 * ym1 - b2 * ym2 - b3 * ym3 - b4 * ym4;
                xm4 = xm3;
                xm3 = xm2;
                xm2 = xm1;
                xm1 = tempx;
                ym4 = ym3;
                ym3 = ym2;
                ym2 = ym1;
                ym1 = tempy;

                newSamples[i] = tempy;
            }

            return new Samples<float>(newSamples);
        }
    }
}
