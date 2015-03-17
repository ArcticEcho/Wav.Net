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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace WavDotNet.Core
{
    public class SampleReader<T> : IDisposable, IEnumerable<T>
    {
        private bool disposed;
        private ushort bitDepth;
        private ushort channels;
        private uint headerSize;
        private uint speakerMask;
        private uint audioLengthBytes;
        private WavFormat audioFormat;
        private readonly Stream stream;
        private readonly Dictionary<uint, T> buffer;
        private readonly uint bufferCapacity;

        public ChannelPositions Channel { get; private set; }

        public uint SampleCount 
        {
            get
            {
                return (uint)(audioLengthBytes / channels / (bitDepth / 8));
            }
        }

        /// <summary>
        /// Returns a sample at the specified index.
        /// </summary>
        public T this[uint index]
        {
            get
            {
                if (index == uint.MaxValue || index + 1 > SampleCount) { throw new IndexOutOfRangeException(); }

                if (buffer == null)
                {
                    return LoadSamples(index, index + 1)[0];
                }

                if (!buffer.ContainsKey(index))
                {
                    UpdateBuffer(index);
                }

                return buffer[index];
            }
        }

        /// <summary>
        /// Returns a range of samples from the specified indexes.
        /// </summary>
        /// <param name="startIndex">The index at which to start reading samples.</param>
        /// <param name="endIndex">The index at which to finish reading samples.</param>
        public Samples<T> this[uint startIndex, uint endIndex]
        {
            get
            {
                if (startIndex > endIndex) { throw new IndexOutOfRangeException("'startIndex' must not be more than 'endIndex'."); }
                if (endIndex > SampleCount) { throw new IndexOutOfRangeException("'endIndex' must not be more than 'SampleCount'."); }

                var count = endIndex - startIndex;

                if (buffer == null || count > buffer.Count)
                {
                    return LoadSamples(startIndex, endIndex);
                }

                if (!buffer.ContainsKey(startIndex) || !buffer.ContainsKey(endIndex))
                {
                    UpdateBuffer(startIndex);
                }

                var realSamples = new T[count];

                for (uint i = 0; i < count; i++)
                {
                    realSamples[i] = buffer[startIndex + i];
                }

                return new Samples<T>(realSamples);
            }
        }



        # region Constructors/destructor.

        public SampleReader(string filePath, ChannelPositions channel, uint? bufferCapacity)
        {
            if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException(); }
            if (new FileInfo(filePath).Length > int.MaxValue) { throw new ArgumentException("File is too large. Must be less than 2 GiB.", "filePath"); }
            if (bufferCapacity < 1024) { throw new ArgumentOutOfRangeException("bufferCapacity", "Must be more than 1024."); }

            stream = File.OpenRead(filePath);
            Channel = channel;

            GetMeta();
            CheckMeta();

            if (bufferCapacity != null)
            {
                this.bufferCapacity = (uint)bufferCapacity;
                buffer = new Dictionary<uint, T>();
                UpdateBuffer(0);
            }
        }

        public SampleReader(string filePath, ChannelPositions channel)
        {
            if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException(); }
            if (new FileInfo(filePath).Length > int.MaxValue) { throw new ArgumentException("File is too large. Must be less than 2 GiB.", "filePath"); }

            stream = File.OpenRead(filePath);
            Channel = channel;

            GetMeta();
            CheckMeta();

            bufferCapacity = 1048576;
            buffer = new Dictionary<uint, T>();
            UpdateBuffer(0);
        }

        public SampleReader(Stream stream, ChannelPositions channel, uint? bufferCapacity)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }
            if (stream.Length > int.MaxValue) { throw new ArgumentException("Stream is too large. Must be less than 2GiB.", "stream"); }
            if (bufferCapacity < 1024) { throw new ArgumentOutOfRangeException("bufferCapacity", "Must be more than 1024."); }

            this.stream = stream;
            Channel = channel;

            GetMeta();
            CheckMeta();

            if (bufferCapacity != null)
            {
                this.bufferCapacity = (uint)bufferCapacity;
                buffer = new Dictionary<uint, T>();
                UpdateBuffer(0);
            }
        }

        public SampleReader(Stream stream, ChannelPositions channel)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }
            if (stream.Length > int.MaxValue) { throw new ArgumentException("Stream is too large. Must be less than 2GiB.", "stream"); }

            this.stream = stream;
            Channel = channel;

            GetMeta();
            CheckMeta();

            bufferCapacity = 1048576;
            buffer = new Dictionary<uint, T>();
            UpdateBuffer(0);
            
        }

        ~SampleReader()
        {
            if (!disposed)
            {
                Dispose();
            }
        }

        # endregion


        /// <summary>
        /// Reads all samples from the stream/file.
        /// </summary>
        public Samples<T> LoadAllSamples()
        {
            return ReadAudioData(0, SampleCount);
        }

        public Samples<T> LoadSamples(uint startIndex, uint endIndex)
        {
            if (endIndex == 0) { throw new ArgumentOutOfRangeException("endIndex", "endIndex must be more than 0."); }
            if (startIndex > endIndex) { throw new ArgumentOutOfRangeException("startIndex", "startIndex must be less than endIndex."); }

            return ReadAudioData(startIndex, endIndex);
        }

        public void Dispose()
        {
            if (disposed) { return; }

            stream.Dispose();
            GC.SuppressFinalize(this);
            disposed = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (uint i = 0; i < SampleCount; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        private void UpdateBuffer(uint sampleIndex)
        {
            buffer.Clear();

            var trueByteDepth = (audioFormat == WavFormat.FloatingPoint ? Math.Max((ushort)32, bitDepth) : bitDepth) / 8;
            var endIndex = Math.Min((uint)(bufferCapacity / trueByteDepth) + sampleIndex, SampleCount);
            var samples = LoadSamples(sampleIndex, endIndex);

            for (uint i = 0; i < samples.Count; i++)
            {
                buffer.Add(sampleIndex + i, samples[(int)i]);
            }
        }

        # region Private meta reading methods.

        private void GetMeta()
        {
            var bytes = new byte[1024];

            stream.Position = 0;
            stream.Read(bytes, 0, 1024);

            var streamLength = (int)stream.Length;
            var header = Encoding.ASCII.GetString(bytes);

            if (!header.StartsWith("RIFF", StringComparison.Ordinal) || header.IndexOf("data", 0, StringComparison.Ordinal) == -1)
            {
                throw new UnrecognisedWavFileException("Stream is not in a recognised wav format."); 
            }

            var fmtStartIndex = header.IndexOf("fmt ", 0, StringComparison.Ordinal) + 4;
            headerSize = (uint)header.IndexOf("data", 0, StringComparison.Ordinal) + 8;
            audioLengthBytes = (uint)streamLength - headerSize;
            audioFormat = (WavFormat)BitConverter.ToUInt16(bytes, fmtStartIndex + 4);
            channels = BitConverter.ToUInt16(bytes, fmtStartIndex + 6);
            bitDepth = BitConverter.ToUInt16(bytes, fmtStartIndex + 18);

            if ((int)audioFormat == 65534)
            {
                speakerMask = BitConverter.ToUInt32(bytes, 40);
                audioFormat = (WavFormat)bytes[fmtStartIndex + 28 + 3];
            }
            else
            {
                // If the speaker mask is not present, then that assume 2 channels = FL + FR, otherwise call GetSpakerMask.
                speakerMask = channels == 2 ? 3 : GetSpeakerMask(channels);
            }
        }

        private void CheckMeta()
        {
            if (bitDepth == 0) { throw new UnrecognisedWavFileException("File is displaying an invalid bit depth."); }
            if (channels == 0) { throw new UnrecognisedWavFileException("File is displaying an invalid number of channels."); }
            if (audioFormat == WavFormat.Unknown) { throw new UnrecognisedWavFileException("Can only read audio in either PCM or IEEE format."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64 && bitDepth != 128)
            {
                throw new UnrecognisedWavFileException("File is of an unsupported bit depth of:" + bitDepth + ".\nSupported bit depths: 8, 16, 24, 32, 64 and 128.");
            }
        }

        private static uint GetSpeakerMask(int channelCount)
        {
            if (channelCount == 8)
            {
                return 0x33F;
            }

            uint mask = 0;
            var positions = new List<ChannelPositions>();

            foreach (var pos in Enum.GetValues(typeof(ChannelPositions)))
            {
                var ch = (ChannelPositions)pos;

                if (ch == ChannelPositions.Mono) { continue; }

                positions.Add(ch);
            }

            for (var i = 0; i < channelCount; i++)
            {
                mask += (uint)positions[i];
            }

            return mask;
        }

        # endregion

        # region Private sample reading methods.

        private Samples<T> ReadAudioData(uint startIndex, uint endIndex)
        {
            switch (bitDepth)
            {
                case 8:
                {
                    return Read8BitSamples(startIndex, endIndex);
                }

                case 16:
                {
                    return Read16BitSamples(startIndex, endIndex);
                }

                case 24:
                {
                    return Read24BitSamples(startIndex, endIndex);
                }

                case 32:
                {
                    return Read32BitSamples(startIndex, endIndex);
                }

                case 64:
                {
                    return Read64BitSamples(startIndex, endIndex);
                }

                default:
                {
                    throw new NotSupportedException("Can not read audio at bit depth of: " + bitDepth);
                }
            }
        }

        private unsafe Samples<T> Read8BitSamples(uint sampleStartIndex, uint sampleEndIndex)
        {
            int length;
            var bytes = ReadBytes(sampleStartIndex, sampleEndIndex, out length);
            var samples = new byte[length];

            for (var i = 0; i < length; i++)
            {
                samples[i] = bytes[i];
            }

            Marshal.FreeHGlobal((IntPtr)bytes);

            var realSamples = new SampleConverter<byte, T>().Convert(samples);

            return new Samples<T>(realSamples);
        }

        private unsafe Samples<T> Read16BitSamples(uint sampleStartIndex, uint sampleEndIndex)
        {
            int length;
            var bytes = ReadBytes(sampleStartIndex, sampleEndIndex, out length);
            var tempSamples = new short[length / 2];

            for (var i = 0; i < length; i += 2)
            {
                tempSamples[i / 2] = (short)(bytes[i] | bytes[i + 1] << 8);
            }

            Marshal.FreeHGlobal((IntPtr)bytes);

            var realSamples = new SampleConverter<short, T>().Convert(tempSamples);

            return new Samples<T>(realSamples);
        }

        private unsafe Samples<T> Read24BitSamples(uint sampleStartIndex, uint sampleEndIndex)
        {
            int length;
            var bytes = ReadBytes(sampleStartIndex, sampleEndIndex, out length);
            var tempSamples = new int[length / 3];

            for (var i = 0; i < length; i += 3)
            {
                tempSamples[i / 3] = bytes[i] << 8 | bytes[i + 1] << 16 | bytes[i + 2] << 24;
            }

            Marshal.FreeHGlobal((IntPtr)bytes);

            var realSamples = new SampleConverter<int, T>().Convert(tempSamples);

            return new Samples<T>(realSamples);
        }

        private unsafe Samples<T> Read32BitSamples(uint sampleStartIndex, uint sampleEndIndex)
        {
            int length;
            var bytes = ReadBytes(sampleStartIndex, sampleEndIndex, out length);
            var intSamples = new int[length / 4];

            for (var i = 0; i < length; i += 4)
            {
                intSamples[i / 4] = bytes[i] | bytes[i + 1] << 8 | bytes[i + 2] << 16 | bytes[i + 3] << 24;
            }

            Marshal.FreeHGlobal((IntPtr)bytes);

            if (audioFormat == WavFormat.FloatingPoint)
            {
                var floatSamples = new float[intSamples.Length];

                for (var i = 0; i < intSamples.Length; i++)
                {
                    var sample = intSamples[i];

                    floatSamples[i] = *(float*)&sample;
                }

                var realSamples = new SampleConverter<float, T>().Convert(floatSamples);

                return new Samples<T>(realSamples);
            }
            else
            {
                var realSamples = new SampleConverter<int, T>().Convert(intSamples);

                return new Samples<T>(realSamples);
            }
        }

        private unsafe Samples<T> Read64BitSamples(uint sampleStartIndex, uint sampleEndIndex)
        {
            int length;
            var bytes = ReadBytes(sampleStartIndex, sampleEndIndex, out length);
            var longSamples = new long[length / 8];

            for (var i = 0; i < length; i += 8)
            {
                longSamples[i / 8] = bytes[i] | bytes[i + 1] << 8 | bytes[i + 2] << 16 | bytes[i + 3] << 24 | bytes[i + 4] << 32 | bytes[i + 5] << 40 | bytes[i + 6] << 48 | bytes[i + 7] << 56;
            }

            Marshal.FreeHGlobal((IntPtr)bytes);

            if (audioFormat == WavFormat.FloatingPoint)
            {
                var doubleSamples = new double[longSamples.Length];

                for (var i = 0; i < longSamples.Length; i++)
                {
                    var sample = longSamples[i];

                    doubleSamples[i] = *(double*)&sample;
                }

                var realSamples = new SampleConverter<double, T>().Convert(doubleSamples);

                return new Samples<T>(realSamples);
            }
            else
            {
                var realSamples = new SampleConverter<long, T>().Convert(longSamples);

                return new Samples<T>(realSamples);
            }
        }

        # endregion

        # region Private byte reading methods.

        private unsafe byte* ReadBytes(uint sampleStartIndex, uint sampleEndIndex, out int byteCount)
        {
            var chNo = GetChannelNumber(Channel, FindExistingChannels(speakerMask));
            var byteDepth = bitDepth / 8;
            var chDelta = chNo * byteDepth;
            var frameSize = byteDepth * channels;
            var startDelta = (int)(sampleStartIndex * byteDepth * channels);
            var endDelta = (int)(sampleEndIndex * byteDepth * channels);
            var outputBytesCount = endDelta - startDelta;
            var temp = new byte[outputBytesCount];
            var ptr = (byte*)Marshal.AllocHGlobal(outputBytesCount);
            var i = 0;

            stream.Position = headerSize + startDelta;
            stream.Read(temp, 0, outputBytesCount);

            startDelta += chDelta;

            for (var j = startDelta; j < endDelta; j += frameSize)
            {
                for (var k = j; k < j + byteDepth; k++)
                {
                    ptr[i] = temp[(k - startDelta) + chDelta];
                    i++;
                }
            }

            byteCount = outputBytesCount;

            return ptr;
        }

        private static int GetChannelNumber(ChannelPositions channel, IList<ChannelPositions> existingChannels)
        {
            var sorted = Sort(existingChannels);

            if (existingChannels.Count == 1)
            {
                return 0;
            }

            for (var i = 0; i < existingChannels.Count; i++)
            {
                if (sorted[i] == channel)
                {
                    return i;
                }
            }

            return -1;
        }

        private static List<ChannelPositions> Sort(IList<ChannelPositions> input)
        {
            var sorted = new List<ChannelPositions>();

            for (var i = 0; i < input.Count; i++)
            {
                if (sorted.Count == 0)
                {
                    sorted.Add(input[0]);
                }
                else
                {
                    if (input[i] > sorted[0])
                    {
                        sorted.Add(input[i]);
                    }
                    else
                    {
                        sorted.Insert(0, input[i]);
                    }
                }
            }

            return sorted;
        }

        private static List<ChannelPositions> FindExistingChannels(uint mask)
        {
            if (mask == 0)
            {
                return new List<ChannelPositions>
                {
                    ChannelPositions.Mono
                };
            }

            var output = new List<ChannelPositions>();

            foreach (var channel in Enum.GetValues(typeof(ChannelPositions)))
            {
                var ch = (ChannelPositions)channel;

                if ((ch & (ChannelPositions)mask) == ch & ch != ChannelPositions.Mono)
                {
                    output.Add(ch);
                }
            }

            return output;
        }

        # endregion
    }
}
