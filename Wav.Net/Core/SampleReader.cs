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

        public Samples<T> this[uint startIndex, uint endIndex]
        {
            get
            {
                if (startIndex > endIndex) { throw new IndexOutOfRangeException("'startIndex' can not be more than 'endIndex'."); }
                if (endIndex > SampleCount) { throw new IndexOutOfRangeException(); }

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

            var endIndex = Math.Max((uint)(bufferCapacity / trueByteDepth) + sampleIndex, SampleCount);

            var samples = LoadSamples(sampleIndex, endIndex);

            for (uint i = 0; i < samples.Count; i++)
            {
                buffer.Add(sampleIndex + i, samples[(int)i]);
            }
        }

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

                //case 128:
                //{
                //	return Read128BitSamples(startIndex, endIndex);
                //}

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
                tempSamples[i / 2] = (short)(bytes[i] << 16 | bytes[i + 1] << 24);
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

        //TODO: Look-up how to read 128 bit samples.
        //private Samples<T> Read128BitSamples(uint sampleStartIndex, uint sampleEndIndex)
        //{
        //	var bytesLength = (sampleEndIndex * 16) - (sampleStartIndex * 16);
        //	var bytes = new byte[bytesLength];
        //	var tempSamples = new decimal[bytesLength / 16];

        //	ReadBytes(sampleStartIndex, sampleEndIndex, bytes);

        //	unsafe
        //	{
        //		for (var i = 0; i < bytesLength; i += 16)
        //		{
        //			var bin = new[]
        //			{
        //				bytes[i     ] | bytes[i +  1] << 8 | bytes[i +  2] << 16 | bytes[i +  3] << 32,
        //				bytes[i +  4] | bytes[i +  5] << 8 | bytes[i +  6] << 16 | bytes[i +  7] << 32,
        //				bytes[i +  8] | bytes[i +  9] << 8 | bytes[i + 10] << 16 | bytes[i + 11] << 32,
        //				bytes[i + 12] | bytes[i + 13] << 8 | bytes[i + 14] << 16 | bytes[i + 15] << 32
        //			};

        //			//var temp = bin1 | bin2 << 32 | bin3 << 64 | bin4 << 128; // Or +?

        //			var sample = 0m;

        //			for (var k = 0; k < 128; k += 32)
        //			{
        //				var temp = bin[k / 32];

        //				for (var j = 0; j < k; j++)
        //				{
        //					temp *= temp;
        //				}

        //				sample += temp;
        //			}

        //			tempSamples[i / 16] = sample;
        //		}
        //	}
            
        //	var realSamples = SampleConverter<decimal, T>.Convert(tempSamples).ToArray();

        //	return new Samples<T>(() => realSamples.Length, i => realSamples[i]);
        //}

        private unsafe byte* ReadBytes(uint sampleStartIndex, uint sampleEndIndex, out int byteCount)
        {
            var ch = GetChannelNumber(Channel, FindExistingChannels(speakerMask));
            var byteDepth = bitDepth / 8;
            var chDelta = ch * byteDepth;
            var frameSize = byteDepth * channels;
            var a = (int)(sampleStartIndex * byteDepth * channels);
            var b = (int)(sampleEndIndex * byteDepth * channels);
            var outputBytesCount = b - a;
            var temp = new byte[outputBytesCount];
            //var outputBytes = new byte[outputBytesCount];
            var ptr = (byte*)Marshal.AllocHGlobal(outputBytesCount);
            var i = 0;

            stream.Position = headerSize + a;
            stream.Read(temp, 0, outputBytesCount);

            a += chDelta;

            for (var j = a; j < b; j += frameSize)
            {
                for (var k = j; k < j + byteDepth; k++)
                {
                    ptr[i] = temp[(k - a) + chDelta];
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
    }
}
