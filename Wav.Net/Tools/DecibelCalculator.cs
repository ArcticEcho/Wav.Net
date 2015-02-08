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
