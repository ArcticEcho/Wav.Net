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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;



namespace WavDotNet.Core
{
    public class Samples<T> : IEnumerable<T>
    {
        private readonly GetSample sampleGetter;
        private readonly SetSample sampleSetter;
        private readonly AddSample sampleAdder;
        private readonly RemoveSample sampleRemover;
        private readonly GetCount getCount;

        public delegate T GetSample(int index);
        public delegate void SetSample(int index, T value);
        public delegate void AddSample(T value);
        public delegate void RemoveSample(int index);
        public delegate int GetCount();

        public Type SampleType
        {
            get
            {
                return typeof(T);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return sampleSetter == null;
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

            set
            {
                if (sampleSetter != null)
                {
                    sampleSetter(index, value);
                }

                throw new ReadOnlyObjectException("This object is read-only.");
            }
        }



        public Samples(T[] samples)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var inTypeName = typeof(T).FullName;

            if (!Regex.IsMatch(inTypeName, @"^System\.(U?Int\d{1,2}|S?Byte|Single|Double|Decimal)$", RegexOptions.CultureInvariant))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
            }

            getCount = () => samples.Length;
            sampleGetter = i => samples[i];
            sampleSetter = (i, val) => samples[i] = val;
        }

        public Samples(IList<T> samples)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var inTypeName = typeof(T).FullName;

            if (!Regex.IsMatch(inTypeName, @"^System\.(U?Int\d{1,2}|S?Byte|Single|Double|Decimal)$", RegexOptions.CultureInvariant))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
            }

            getCount = () => samples.Count;
            sampleGetter = i => samples[i];
            sampleSetter = (i, val) => samples[i] = val;
            sampleAdder = samples.Add;
            sampleRemover = samples.RemoveAt;
        }

        public Samples(IList<T> samples, bool isReadOnly)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            var inTypeName = typeof(T).FullName;

            if (!Regex.IsMatch(inTypeName, @"^System\.(U?Int\d{1,2}|S?Byte|Single|Double|Decimal)$", RegexOptions.CultureInvariant))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
            }

            getCount = () => samples.Count;
            sampleGetter = i => samples[i];

            if (isReadOnly) { return; }

            sampleSetter = (i, val) => samples[i] = val;
            sampleAdder = samples.Add;
            sampleRemover = samples.RemoveAt;
        }

        public Samples(GetCount getCount, GetSample sampleGetter)
        {
            if (getCount == null) { throw new ArgumentNullException("getCount"); }
            if (sampleGetter == null) { throw new ArgumentNullException("sampleGetter"); }

            var inTypeName = typeof(T).FullName;

            if (!Regex.IsMatch(inTypeName, @"^System\.(U?Int\d{1,2}|S?Byte|Single|Double|Decimal)$", RegexOptions.CultureInvariant))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
            }

            this.getCount = getCount;
            this.sampleGetter = sampleGetter;
        }

        public Samples(GetCount getCount, GetSample sampleGetter, SetSample sampleSetter, AddSample sampleAdder, RemoveSample sampleRemover)
        {
            if (getCount == null) { throw new ArgumentNullException("getCount"); }
            if (sampleGetter == null) { throw new ArgumentNullException("sampleGetter"); }

            var inTypeName = typeof(T).FullName;

            if (!Regex.IsMatch(inTypeName, @"^System\.(U?Int\d{1,2}|S?Byte|Single|Double|Decimal)$", RegexOptions.CultureInvariant))
            {
                throw new Exception("T can only be of type: byte, sbyte, short, ushort, int, uint, long, ulong, float, double or decimal.");
            }

            this.getCount = getCount;
            this.sampleGetter = sampleGetter;
            this.sampleSetter = sampleSetter;
            this.sampleAdder = sampleAdder;
            this.sampleRemover = sampleRemover;
        }



        public void Add(T sample)
        {
            if (IsReadOnly) { throw new ReadOnlyObjectException("This object is read-only."); }
            if (sampleAdder == null) { throw new ReadOnlyObjectException("'sampleAdder' has not been specified."); }

            sampleAdder(sample);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly) { throw new ReadOnlyObjectException("This object is read-only."); }
            if (sampleRemover == null) { throw new ReadOnlyObjectException("'sampleRemover' has not been specified."); }

            sampleRemover(index);
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
