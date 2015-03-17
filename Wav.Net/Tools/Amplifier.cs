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
using System.Collections.Generic;
using System.Text;
using WavDotNet.Core;

namespace WavDotNet.Tools
{
    public static class Amplifier
    {
        public static double[] Amplify(double[] samples, double level)
        {
            var highestSample = 0d;

            for (var i = 0; i < samples.Length; i++)
            {
                var abs = System.Math.Abs(samples[i]);

                if (abs > highestSample)
                {
                    highestSample = abs;
                }
            }

            var delta = level / highestSample;
            var newSamples = new double[samples.Length];

            for (var i = 0; i < samples.Length; i++)
            {
                newSamples[i] = samples[i] * delta;
            }

            return newSamples;
        }

        public static float[] Amplify(float[] samples, double level)
        {
            var highestSample = 0f;

            for (var i = 0; i < samples.Length; i++)
            {
                var abs = System.Math.Abs(samples[i]);

                if (abs > highestSample)
                {
                    highestSample = abs;
                }
            }

            var delta = level / highestSample;
            var newSamples = new float[samples.Length];

            for (var i = 0; i < samples.Length; i++)
            {
                newSamples[i] = (float)System.Math.Round(samples[i] * delta, 7);
            }

            return newSamples;
        }

        public static short[] Amplify(short[] samples, double level)
        {
            var highestSample = 0;

            for (var i = 0; i < samples.Length; i++)
            {
                var abs = System.Math.Abs(samples[i]);

                if (abs > highestSample)
                {
                    highestSample = abs;
                }
            }

            var delta = (level * short.MaxValue) / highestSample;
            var newSamples = new short[samples.Length];

            for (var i = 0; i < samples.Length; i++)
            {
                newSamples[i] = (short)System.Math.Round(samples[i] * delta);
            }

            return newSamples;
        }
    }
}
