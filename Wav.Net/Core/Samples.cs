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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WavDotNet.Core
{
    /// <summary>
    /// Represents an immutable class for accessing a generic collection of samples.
    /// (Samples are not actually "stored" in this object, but are merely made accessible.
    /// It may help to think of this class as a "bridge" to get to the samples passed via the constructor.) 
    /// </summary>
    /// <typeparam name="T">The type of the samples.</typeparam>
    public class Samples<T> : IEnumerable<T>
    {
        private static readonly Regex typeCheck = new Regex(@"^System\.(U?Int\d{1,2}|S?Byte|Single|Double)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly GetSample sampleGetter;
        private readonly GetCount getCount;

        public delegate T GetSample(int index);
        public delegate int GetCount();

        public Type SampleType
        {
            get
            {
                return typeof(T);
            }
        }

        public int Count
        {
            get
            {
                return getCount();
            }
        }

        public T this[int index]
        {
            get
            {
                return sampleGetter(index);
            }
        }



        public Samples(T[] samples)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var inTypeName = typeof(T).FullName;

            if (!typeCheck.IsMatch(inTypeName))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float or double.");
            }

            getCount = () => samples.Length;
            sampleGetter = i => samples[i];
        }

        public Samples(IList<T> samples)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var inTypeName = typeof(T).FullName;

            if (!typeCheck.IsMatch(inTypeName))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float or double.");
            }

            getCount = () => samples.Count;
            sampleGetter = i => samples[i];
        }

        public Samples(GetCount getCount, GetSample sampleGetter)
        {
            if (getCount == null) { throw new ArgumentNullException("getCount"); }
            if (sampleGetter == null) { throw new ArgumentNullException("sampleGetter"); }

            var inTypeName = typeof(T).FullName;

            if (!typeCheck.IsMatch(inTypeName))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float or double.");
            }

            this.getCount = getCount;
            this.sampleGetter = sampleGetter;
        }



        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
