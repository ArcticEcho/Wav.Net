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
using System.Collections.Generic;



namespace WavDotNet.Core
{
    public partial class SampleConverter<TIn, TOut>
    {
        private readonly AlgoManager<TIn, TOut> manager = new AlgoManager<TIn, TOut>();



        public TOut[] Convert(IEnumerable<TIn> samples)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var algo = manager.GetAlgo();

            var converted = new List<TOut>();

            foreach (var sample in samples)
            {
                converted.Add(algo(sample));
            }

            return converted.ToArray();
        }

        public TOut Convert(TIn sample)
        {
            return manager.GetAlgo()(sample);
        }
    }
}
