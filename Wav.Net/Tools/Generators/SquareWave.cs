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
using WavDotNet.Core;

namespace WavDotNet.Tools.Generators
{
    public class SquareWave
    {
        private readonly int sampleRate;



        public SquareWave(int sampleRate)
        {
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be more than 0."); }

            this.sampleRate = sampleRate;
        }



        public Samples<float> Generatee32Bit(TimeSpan duration, uint frequency, float amplitude)
        {
            if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
            if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

            var samples = new float[(int)(duration.TotalSeconds * sampleRate)];
            var freq = sampleRate / frequency;

            for (var i = 0; i < samples.Length; i++)
            {
                if (i % freq < freq / 2)
                {
                    samples[i] = amplitude;
                }
                else
                {
                    samples[i] = -amplitude;
                }
            }

            return new Samples<float>(samples);
        }

        public Samples<double> Generate64Bit(TimeSpan duration, uint frequency, double amplitude)
        {
            if (amplitude < 0 || amplitude > 1) { throw new ArgumentOutOfRangeException("amplitude", "Amplitude must be between 0 and 1."); }
            if (frequency == 0) { throw new ArgumentOutOfRangeException("frequency", "Frequency must be more than 0."); }

            var samples = new double[(int)(duration.TotalSeconds * sampleRate)];
            var freq = sampleRate / frequency;

            for (var i = 0; i < samples.Length; i++)
            {
                if (i % freq < freq / 2)
                {
                    samples[i] = amplitude;
                }
                else
                {
                    samples[i] = -amplitude;
                }
            }

            return new Samples<double>(samples);
        }
    }
}
