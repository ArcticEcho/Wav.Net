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
