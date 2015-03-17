﻿/*
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
using WavDotNet.Core;

namespace WavDotNet.Tools
{
    public static class Math
    {
        # region ToDecibels methods.

        public static double ToDecibels(double amplitude)
        {
            return System.Math.Log10(amplitude) * 20;
        }

        public static double ToDecibels(float amplitude)
        {
            return System.Math.Log10(amplitude) * 20;
        }

        public static double ToDecibels(short amplitude)
        {
            return System.Math.Log10(amplitude / short.MaxValue) * 20;
        }

        # endregion

        # region ToAmplitude methods.

        public static double ToAmplitude(double decibels)
        {
            return System.Math.Pow(10, decibels / 20.0);
        }

        public static double ToAmplitude(float decibels)
        {
            return System.Math.Pow(10, decibels / 20.0);
        }

        # endregion

        # region EffectiveBitsOfPrecision methods.

        public static double EffectiveBitsOfPrecision(List<double> samples)
        {
            var sorted = samples;
            sorted.Sort();
            var data = sorted.ToArray();
            var smallestDiff = double.MaxValue;

            for (var i = 0; i < data.Length - 1; i++)
            {
                var diff = data[i + 1] - data[i];
                if (diff < smallestDiff && diff > 0)
                {
                    smallestDiff = diff;
                }
            }

            return System.Math.Log(1 / smallestDiff, 2);
        }

        public static double EffectiveBitsOfPrecision(List<float> samples)
        {
            var sorted = samples;
            sorted.Sort();
            var data = sorted.ToArray();
            var smallestDiff = float.MaxValue;

            for (var i = 0; i < data.Length - 1; i++)
            {
                var diff = data[i + 1] - data[i];
                if (diff < smallestDiff && diff > 0)
                {
                    smallestDiff = diff;
                }
            }

            return System.Math.Log(1 / smallestDiff, 2);
        }

        public static double EffectiveBitsOfPrecision(List<short> samples)
        {
            var sorted = samples;
            sorted.Sort();
            var data = sorted.ToArray();
            var smallestDiff = (int)short.MaxValue;

            for (var i = 0; i < data.Length - 1; i++)
            {
                var diff = data[i + 1] - data[i];
                if (diff < smallestDiff && diff > 0)
                {
                    smallestDiff = diff;
                }
            }

            return System.Math.Log(1 / smallestDiff, 2);
        }

        # endregion
    }
}
