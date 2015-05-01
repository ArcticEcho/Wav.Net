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
        private readonly IEnumerator sampleEnumerator;
        private readonly SampleIndexer sampleIndexer;
        private readonly GetCount getCount;

        public delegate T SampleIndexer(int index);
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
                return sampleIndexer(index);
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
            sampleIndexer = i => samples[i];
            sampleEnumerator = samples.GetEnumerator();
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
            sampleIndexer = i => samples[i];
            sampleEnumerator = samples.GetEnumerator();
        }

        public Samples(IEnumerable<T> samples)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var inTypeName = typeof(T).FullName;

            if (!typeCheck.IsMatch(inTypeName))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float or double.");
            }

            var count = 0;
            using (var enumerator = samples.GetEnumerator()) { while (enumerator.MoveNext()) { count++; } }

            getCount = () => count;
            sampleIndexer = i =>
            {
                var curInd = 0;
                foreach (var sam in samples)
                {
                    if (i == curInd) { return sam; }
                    curInd++;
                }

                throw new IndexOutOfRangeException();
            };
            sampleEnumerator = samples.GetEnumerator();
        }

        public Samples(GetCount getCount, SampleIndexer sampleIndexer, IEnumerator enumerator)
        {
            if (getCount == null) { throw new ArgumentNullException("getCount"); }
            if (sampleIndexer == null) { throw new ArgumentNullException("sampleIndexer"); }
            if (enumerator == null) { throw new ArgumentNullException("enumerator"); }

            var inTypeName = typeof(T).FullName;

            if (!typeCheck.IsMatch(inTypeName))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float or double.");
            }

            this.getCount = getCount;
            this.sampleIndexer = sampleIndexer;
            sampleEnumerator = enumerator;
        }



        public IEnumerator<T> GetEnumerator()
        {
            if (!sampleEnumerator.MoveNext()) { throw new Exception("Unable to enumerate collection."); }

            yield return (T)sampleEnumerator.Current;

            while (sampleEnumerator.MoveNext())
            {
                yield return (T)sampleEnumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
