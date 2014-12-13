/*
 * 
 * 
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
 * 
 * 
 */





using System;
using WavDotNet.Core;
using WavDotNet.Tools.Filters;



namespace WavDotNet.Tools.Generators
{
    /// <summary>
    /// Reproduces the sound of rain.
    /// </summary>
    public class Rain
    {
        private readonly Random r = new Random();
        private readonly double sampleRate;
        private double foregroundRainIntensity;
        private double backgroundRainIntensity;
        private int minDropFreq;
        private double lpfCuttoffFreq;
        private double hpfCutoffFreq;
        private double maxOscillationsPerDrop;
        private int maxDropFreq;
        private LinkwitzRileyLowPass lowPass;
        private LinkwitzRileyHighPass highPass;

        /// <summary>
        /// The foreground rain intensity percentage (1 = 100%, 0.5 = 50%, etc.). Default: 0.05.
        /// </summary>
        public double ForegroundRainIntensity
        {
            get
            {
                return foregroundRainIntensity;
            }

            set
            {
                if (value < 0 || value > 1) { throw new ArgumentOutOfRangeException("value", "Must be between 0 and 1."); }

                foregroundRainIntensity = value;
            }
        }

        /// <summary>
        /// The background rain intensity percentage (1 = 100%, 0.5 = 50%, etc.). Default: 0.3.
        /// </summary>
        public double BackgroundRainIntensity
        {
            get
            {
                return backgroundRainIntensity;
            }

            set
            {
                if (value < 0 || value > 1) { throw new ArgumentOutOfRangeException("value", "Must be between 0 and 1."); }

                backgroundRainIntensity = value;
            }
        }

        /// <summary>
        /// The minimum (inclusive) permitted frequency of a rain drop. Default: 3500.
        /// </summary>
        public int MinDropFreq
        {
            get
            {
                return minDropFreq;
            }

            set
            {
                if (value < 1) { throw new ArgumentOutOfRangeException("value", "Must be more than 0."); }

                minDropFreq = value;
            }
        }

        /// <summary>
        /// The maximum (exclusive) permitted frequency of a rain drop. Default: 120001.
        /// </summary>
        public int MaxDropFreq
        {
            get
            {
                return maxDropFreq;
            }

            set
            {
                if (value < 1) { throw new ArgumentOutOfRangeException("value", "Must be more than 0."); }

                maxDropFreq = value;
            }
        }

        /// <summary>
        /// The maximum (exclusive) number of samples per drop. Default: 5.
        /// </summary>
        public double MaxOscillationsPerDrop
        {
            get
            {
                return maxOscillationsPerDrop;
            }

            set
            {
                if (value < 2) { throw new ArgumentOutOfRangeException("value", "Must be more than 1."); }

                maxOscillationsPerDrop = value;
            }
        }

        /// <summary>
        /// The highpass filter (HPF) cutoff frequency. Default: 250.
        /// </summary>
        public double HpfCutoffFreq
        {
            get
            {
                return hpfCutoffFreq;
            }

            set
            {
                if (value < 1) { throw new ArgumentOutOfRangeException("value", "Must be more than 0."); }

                hpfCutoffFreq = value;
            }
        }

        /// <summary>
        /// The lowpass filter (LPF) cutoff frequency. Default: 13000.
        /// </summary>
        public double LpfCuttoffFreq
        {
            get
            {
                return lpfCuttoffFreq;
            }

            set
            {
                if (value < 1) { throw new ArgumentOutOfRangeException("value", "Must be more than 0."); }

                lpfCuttoffFreq = value;
            }
        }



        /// <param name="sampleRate">The sample rate of the generated rain.</param>
        public Rain(double sampleRate)
        {
            if (sampleRate < 1) { throw new ArgumentOutOfRangeException("sampleRate", "'sampleRate' must be more than 0."); }

            this.sampleRate = sampleRate;
            ForegroundRainIntensity = 0.05;
            BackgroundRainIntensity = 0.3;
            MinDropFreq = 3500;
            MaxDropFreq = 12001;
            MaxOscillationsPerDrop = 5;
            HpfCutoffFreq = 250;
            LpfCuttoffFreq = 13000;

            lowPass = new LinkwitzRileyLowPass((uint)sampleRate);
            highPass = new LinkwitzRileyHighPass((uint)sampleRate);
        }



        /// <summary>
        /// Generates rain using the specified parameters (via properties).
        /// </summary>
        /// <param name="duration">The total duration of the rain.</param>
        /// <returns>64-bit samples at the sample rate specified at instantiation.</returns>
        public Samples<double> Generate(TimeSpan duration)
        {
            if (duration == default(TimeSpan) || duration.TotalSeconds < 1) { throw new ArgumentOutOfRangeException("duration"); }

            var sampleCount = (int)(duration.TotalSeconds * sampleRate);
            var combinedAmp = 1 + BackgroundRainIntensity;
            var samples = new double[sampleCount];
            var addDrop = false;
            var dropAdded = true;
            var dropFreq = 0;
            var samplesPerDrop = 0.0;
            var currentDropDuration = 0;
            var amplitude = 0.0;

            for (var i = 0; i < sampleCount; i++)
            {
                if (dropAdded)
                {
                    // Pick the drop's frequency.
                    dropFreq = r.Next(MinDropFreq, MaxDropFreq);

                    // Calc how many samples each oscillation of the drop will be for the specified frequency (i.e., sample count of a sinlge oscillation).
                    var samplesPerOscillation = sampleRate / dropFreq;

                    // Calc the drop's total sample count.
                    samplesPerDrop = r.Next(1, (int)MaxOscillationsPerDrop) * samplesPerOscillation;

                    // Then calc the max drop count per sec.
                    var maxDropsPerSec = sampleRate / samplesPerDrop;

                    // Finally we can now calc rain intensity.
                    addDrop = r.NextDouble() < (maxDropsPerSec * ForegroundRainIntensity) / maxDropsPerSec;

                    // Choose maximum drop amplitude ("loudness").
                    amplitude = r.NextDouble();
                    amplitude /= combinedAmp;
                }

                // Add background rain.
                samples[i] += GetBackgroundNoise(i, dropFreq) * (BackgroundRainIntensity / combinedAmp);

                if (addDrop)
                {
                    // Add drop.
                    samples[i] += amplitude * Math.Sin(((Math.PI * 2 * dropFreq) / sampleRate) * i) * 4;

                    // Soften drop.
                    samples[i] *= currentDropDuration / samplesPerDrop;
                    samples[i] *= 1 - currentDropDuration / samplesPerDrop;

                    dropAdded = false;
                    currentDropDuration++;

                    if (currentDropDuration > samplesPerDrop)
                    {
                        dropAdded = true;
                        addDrop = false;
                        currentDropDuration = 0;
                    }
                }
            }

            return new Samples<double>(lowPass.Apply(highPass.Apply(samples, HpfCutoffFreq), LpfCuttoffFreq));
        }



        private double GetBackgroundNoise(int i, int freq)
        {
            return (Math.Sin(((Math.PI * 2 * freq) / (sampleRate + r.Next(-(int)(sampleRate * 0.01), (int)(sampleRate * 0.01)))) * i) * 0.5) + (r.NextDouble() * 0.5);
        }

        private double[] LinkwitzRileyLowPass(double[] samples, double cutoff)
        {
            if (cutoff < 1 || cutoff >= sampleRate / 2) { throw new ArgumentOutOfRangeException("cutoff", "The cutoff frequency must be between 0 and 'sampleRate' / 2."); }
            if (samples == null || samples.Length == 0) { throw new ArgumentException("'samples' can not be null or empty.", "samples"); }

            var newSamples = new double[samples.Length];
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

            for (var i = 0; i < samples.Length; i++)
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

            return newSamples;
        }

        private double[] LinkwitzRileyHighPass(double[] samples, double cutoff)
        {
            if (cutoff < 1 || cutoff >= sampleRate / 2) { throw new ArgumentOutOfRangeException("cutoff", "The cutoff frequency must be between 0 and 'sampleRate' / 2."); }
            if (samples == null || samples.Length == 0) { throw new ArgumentException("'samples' can not be null or empty.", "samples"); }

            var newSamples = new double[samples.Length];
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

            var a0 = k4 / aTmp;
            var a1 = -4 * k4 / aTmp;
            var a2 = 6 * k4 / aTmp;
            var a3 = a1;
            var a4 = a0;

            double ym1 = 0.0, ym2 = 0.0, ym3 = 0.0, ym4 = 0.0, xm1 = 0.0, xm2 = 0.0, xm3 = 0.0, xm4 = 0.0, tempy;

            for (var i = 0; i < samples.Length; i++)
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

            return newSamples;
        }
    }
}
