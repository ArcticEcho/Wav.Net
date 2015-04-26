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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace WavDotNet.Core
{
    public class WavFileWrite<T> : WavFile, IDisposable
    {
        private string filePath;
        private Stream stream;
        private int headerSize;
        private bool flushed;
        private bool disposed;

        /// <summary>
        /// The IoPriority used while writing samples to the file/Stream.
        /// </summary>
        public IoPriority WritePriority { get; set; }

        /// <summary>
        /// The (mutable) collection holding the audio data to be flushed.
        /// (Edit this property as necessary to hold the data you wish to flush.)
        /// </summary>
        public Collection<Channel<T>> AudioData { get; private set; }

        /// <summary>
        /// The total number of channels this object currently has.
        /// </summary>
        public ushort ChannelCount
        {
            get
            {
                return (ushort)AudioData.Count;
            }
        }



        # region Constructors/destructor.

        public WavFileWrite(string filePath, uint sampleRate, WavFormat format, ushort bitDepth, ushort validBits)
        {
            var ex = InitialiseFromFile(filePath, sampleRate, format, bitDepth, validBits);
            if (ex != null) { throw ex; }
            
            
            
            /*if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64 && bitDepth != 128)
            {
                throw new ArgumentException("Can only be: 8, 16, 24, 32 64 or 128.", "bitDepth");
            }
            if (validBits == 0) { throw new ArgumentOutOfRangeException("validBits", "Can not be 0."); }

            this.filePath = filePath;
            Format = format;
            SampleRate = sampleRate;
            BitDepth = bitDepth;
            ValidBits = validBits;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(string filePath, uint sampleRate, WavFormat format, ushort bitDepth)
        {
            var ex = InitialiseFromFile(filePath, sampleRate, format, bitDepth);
            if (ex != null) { throw ex; }
            
            
            
            /*if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64 && bitDepth != 128)
            {
                throw new ArgumentException("Can only be: 8, 16, 24, 32 64 or 128.", "bitDepth");
            }

            this.filePath = filePath;
            Format = format;
            SampleRate = sampleRate;
            BitDepth = bitDepth;
            ValidBits = bitDepth;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(string filePath, uint sampleRate, WavFormat format)
        {
            var ex = InitialiseFromFile(filePath, sampleRate, format);
            if (ex != null) { throw ex; }
            
            
            
            /*if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

            this.filePath = filePath;
            Format = format;
            SampleRate = sampleRate;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(string filePath, uint sampleRate)
        {
            var ex = InitialiseFromFile(filePath, sampleRate);
            if (ex != null) { throw ex; }
            
            
            
            /*if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

            this.filePath = filePath;
            SampleRate = sampleRate;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(Stream stream, uint sampleRate, WavFormat format, ushort bitDepth, ushort validBits)
        {
            var ex = InitialiseFromStream(stream, sampleRate, format, bitDepth, validBits);
            if (ex != null) { throw ex; }
            
            
            
            /*if (stream == null) { throw new ArgumentNullException("stream"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64 && bitDepth != 128)
            {
                throw new ArgumentException("Can only be: 8, 16, 24, 32 64 or 128.", "bitDepth");
            }
            if (validBits == 0) { throw new ArgumentOutOfRangeException("validBits", "Can not be 0."); }

            this.stream = stream;
            Format = format;
            SampleRate = sampleRate;
            BitDepth = bitDepth;
            ValidBits = validBits;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(Stream stream, uint sampleRate, WavFormat format, ushort bitDepth)
        {
            var ex = InitialiseFromStream(stream, sampleRate, format, bitDepth);
            if (ex != null) { throw ex; }
            
            
            
            /*if (stream == null) { throw new ArgumentNullException("stream"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64 && bitDepth != 128)
            {
                throw new ArgumentException("Can only be: 8, 16, 24, 32 64 or 128.", "bitDepth");
            }

            this.stream = stream;
            Format = format;
            SampleRate = sampleRate;
            BitDepth = bitDepth;
            ValidBits = bitDepth;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(Stream stream, uint sampleRate, WavFormat format)
        {
            var ex = InitialiseFromStream(stream, sampleRate, format);
            if (ex != null) { throw ex; }
            
            
            
            /*if (stream == null) { throw new ArgumentNullException("stream"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

            this.stream = stream;
            Format = format;
            SampleRate = sampleRate;
            AudioData = new Collection<Channel<T>>();*/
        }

        public WavFileWrite(Stream stream, uint sampleRate)
        {
            var ex = InitialiseFromStream(stream, sampleRate);
            if (ex != null) { throw ex; }
            
            
            
            /*if (stream == null) { throw new ArgumentNullException("stream"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

            this.stream = stream;
            SampleRate = sampleRate;
            AudioData = new Collection<Channel<T>>();*/
        }

        ~WavFileWrite()
        {
            if (!disposed)
            {
                Dispose();
            }
        }

        # endregion



        public void Flush()
        {
            if (flushed) { throw new AlreadyFlushedException("Data has already been flushed."); }

            if (BitDepth == 0)
            {
                BitDepth = GuessBitDepth();
                ValidBits = BitDepth;
            }

            if (Format == WavFormat.Unknown)
            {
                Format = GuessFormat();
            }

            using (stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                AudioLengthBytes = CalcAudioSize();
                CheckAudioData();

                var samples = CombineSamples();
                WriteMetaData();
                WriteAudioData(samples);

                flushed = true;
            }
        }

        public void Dispose()
        {
            if (disposed) { return; }

            if (stream != null)
            {
                stream.Dispose();
            }

            GC.SuppressFinalize(this);

            disposed = true;
        }


        # region Initialisation methods.

        private Exception InitialiseFromFile(string filePath, uint sampleRate, WavFormat format = WavFormat.Unknown, ushort bitDepth = 0, ushort validBits = 0)
        {
            if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Must not be null or empty.", "filePath"); }
            if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Must not be 0."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64)
            {
                return new ArgumentException("Can only be: 8, 16, 24, 32 or 64-bits.", "bitDepth");
            }
            if (validBits == 0) { throw new ArgumentOutOfRangeException("validBits", "Must not be 0."); }

            this.filePath = filePath;
            SampleRate = sampleRate;
            AudioData = new Collection<Channel<T>>();
            Format = format;
            BitDepth = bitDepth;
            ValidBits = validBits;
        }
        
        private Exception InitialiseFromStream(Stream stream, uint sampleRate, WavFormat format = WavFormat.Unknown, ushort bitDepth = 0, ushort validBits = 0)
        {
            if (stream == null) { return new ArgumentNullException("stream"); }
            if (sampleRate == 0) { return new ArgumentOutOfRangeException("sampleRate", "Must not be 0."); }
            if (bitDepth != 8 && bitDepth != 16 && bitDepth != 24 && bitDepth != 32 && bitDepth != 64)
            {
                return new ArgumentException("Can only be: 8, 16, 24, 32 or 64-bits.", "bitDepth");
            }
            if (validBits == 0) { return new ArgumentOutOfRangeException("validBits", "Must not be 0."); }

            this.stream = stream;
            SampleRate = sampleRate;
            AudioData = new Collection<Channel<T>>();
            Format = format;
            BitDepth = bitDepth;
            ValidBits = validBits;
            
            return null;
        }
        
        # endregion

        private static WavFormat GuessFormat()
        {
            switch (typeof(T).FullName)
            {
                case "System.SByte":
                {
                    return WavFormat.Pcm;
                }

                case "System.Byte":
                {
                    return WavFormat.Pcm;
                }

                case "System.Int16":
                {
                    return WavFormat.Pcm;
                }

                case "System.UInt16":
                {
                    return WavFormat.Pcm;
                }

                case "System.Int32":
                {
                    return WavFormat.Pcm;
                }

                case "System.UInt32":
                {
                    return WavFormat.Pcm;
                }

                case "System.Int64":
                {
                    return WavFormat.Pcm;
                }

                case "System.UInt64":
                {
                    return WavFormat.Pcm;
                }

                case "System.Single":
                {
                    return WavFormat.FloatingPoint;
                }

                case "System.Double":
                {
                    return WavFormat.FloatingPoint;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static ushort GuessBitDepth()
        {
            switch (typeof(T).FullName)
            {
                case "System.SByte":
                {
                    return 8;
                }

                case "System.Byte":
                {
                    return 8;
                }

                case "System.Int16":
                {
                    return 16;
                }

                case "System.UInt16":
                {
                    return 16;
                }

                case "System.Int32":
                {
                    return 32;
                }

                case "System.UInt32":
                {
                    return 32;
                }

                case "System.Int64":
                {
                    return 32;
                }

                case "System.UInt64":
                {
                    return 32;
                }

                case "System.Single":
                {
                    return 32;
                }

                case "System.Double":
                {
                    return 64;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private uint CalcAudioSize()
        {
            var smallestChLength = int.MaxValue;

            foreach (var ch in AudioData)
            {
                if (ch.Samples.Count < smallestChLength)
                {
                    smallestChLength = ch.Samples.Count;
                }
            }

            // Smallest channel * channel count * byte depth.
            return (uint)(smallestChLength * AudioData.Count * (BitDepth == 24 ? 4 : BitDepth / 8));
        }

        private void CheckAudioData()
        {
            if (AudioData.Count == 0)
            {
                throw new InvalidWavDataException("'AudioData' can not be empty.");
            }

            foreach (var ch in AudioData)
            {
                if (ch.Samples.Count == 0)
                {
                    throw new InvalidWavDataException("All channels within 'AudioData' must not be empty.");
                }
            }

            if (AudioData.Count > 1)
            {
                var hasMono = false;

                foreach (var ch in AudioData)
                {
                    if (ch.Position == ChannelPositions.Mono)
                    {
                        hasMono = true;
                    }
                }

                if (hasMono) { throw new InvalidWavDataException("'AudioData' must not contain a 'Mono' channel if multiple channels are present."); }
            }

            foreach (var pos in Enum.GetValues(typeof(ChannelPositions)))
            {
                var count = 0;

                foreach (var ch in AudioData)
                {
                    if (ch.Position == (ChannelPositions)pos)
                    {
                        count++;
                    }
                }

                if (count > 1) { throw new InvalidWavDataException("'AudioData' must not contain multiple channels set with the same 'ChannelPosition'."); }
            }
        }

        private void WriteMetaData()
        {
            var guid = new List<byte>(BitConverter.GetBytes((ushort)Format));
            guid.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71});

            var isMono = AudioData[0].Position == ChannelPositions.Mono;

            stream.Position = 0;

            // RIFF header.
            // Chunk ID.
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);

            // Chunk size.
            stream.Write(BitConverter.GetBytes(AudioLengthBytes + 36 + (isMono ? 0 : 22)), 0, 4);

            // Format.
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);



            // Sub-chunk 1.
            // Sub-chunk 1 ID.
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);

            // Sub-chunk 1 size.
            stream.Write(isMono ? BitConverter.GetBytes(16) : BitConverter.GetBytes(40), 0, 4);

            // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((ushort)(isMono ? (Format == WavFormat.FloatingPoint ? 3 : 1) : 65534)), 0, 2);

            // Channels.
            stream.Write(BitConverter.GetBytes(ChannelCount), 0, 2);

            // Sample rate.
            stream.Write(BitConverter.GetBytes(SampleRate), 0, 4);

            // Bytes rate.
            stream.Write(BitConverter.GetBytes(SampleRate * ChannelCount * (BitDepth / 8)), 0, 4);

            // Block align.
            stream.Write(BitConverter.GetBytes((ushort)(ChannelCount * (BitDepth / 8))), 0, 2);

            // Bits per sample.
            stream.Write(BitConverter.GetBytes(Format == WavFormat.FloatingPoint ? Math.Max((ushort)32, BitDepth) : BitDepth), 0, 2);

            // If there's only one channel, and if that channel is set to Mono then don't specify speaker mask, otherwise specify.
            if (!isMono)
            {
                // Extra sub-chunk size.
                stream.Write(BitConverter.GetBytes((ushort)22), 0, 2);

                // Valid bits per sample. Example, bits per sample maybe 24 bits, but actual data maybe 20 bits.
                stream.Write(BitConverter.GetBytes(ValidBits), 0, 2);

                // Channel Mask (channel layout within file).
                stream.Write(BitConverter.GetBytes(GetSpeakerMask()), 0, 4);

                // GUID + Format. Format is either PCM or floating point. Anyother format indicates compression.
                stream.Write(guid.ToArray(), 0, 16);
            }



            // Sub-chunk 2.
            // Sub-chunk 2 ID.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);

            // Sub-chunk 2 size.
            stream.Write(BitConverter.GetBytes(AudioLengthBytes), 0, 4);

            headerSize = (int)stream.Position;
        }

        private void WriteAudioData(Samples<T> samples)
        {
            stream.Position = headerSize;

            switch (BitDepth)
            {
                case 8:
                {
                    WriteSamples8Bit(samples);
                    break;
                }

                case 16:
                {
                    WriteSamples16Bit(samples);
                    break;
                }

                case 24:
                {
                    WriteSamples24Bit(samples);
                    break;
                }

                case 32:
                {
                    WriteSamples32Bit(samples);
                    break;
                }

                case 64:
                {
                    WriteSamples64Bit(samples);
                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        # region Private 8-bit sample writing methods.

        private void WriteSamples8Bit(Samples<T> samples)
        {
            if (Format == WavFormat.Pcm)
            {
                if (typeof(T) == typeof(byte))
                {
                    WriteSamples8BitNoConvert(samples);
                }
                else
                {
                    WriteSamples8BitConvert(samples);
                }
            }
            else
            {
                WriteAsFloats(samples);
            }
        }

        private void WriteSamples8BitNoConvert(Samples<T> samples)
        {
            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    stream.WriteByte(sample as byte? ?? 0);
                }
            }
            else
            {
                var data = new byte[samples.Count];

                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = samples[i] as byte? ?? 0;
                }

                stream.Write(data, 0, data.Length);
            }
        }

        private void WriteSamples8BitConvert(Samples<T> samples)
        {
            var converter = new SampleConverter<T, byte>();

            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    stream.WriteByte(converter.Convert(sample));
                }
            }
            else
            {
                var data = new byte[samples.Count];

                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = converter.Convert(samples[i]);
                }

                stream.Write(data, 0, data.Length);
            }
        }

        # endregion

        # region Private 16-bit sample writing methods.

        private void WriteSamples16Bit(Samples<T> samples)
        {
            if (Format == WavFormat.Pcm)
            {
                if (typeof(T) == typeof(short))
                {
                    WriteSamples16BitNoConvert(samples);
                }
                else
                {
                    WriteSamples16BitConvert(samples);
                }
            }
            else
            {
                WriteAsFloats(samples);
            }
        }

        private void WriteSamples16BitNoConvert(Samples<T> samples)
        {
            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(sample as short? ?? 0);
                    stream.Write(bytes, 0, 2);
                }
            }
            else
            {
                var data = new byte[samples.Count * 2];

                for (var i = 0; i < data.Length; i += 2)
                {
                    var bytes = BitConverter.GetBytes(samples[i / 2] as short? ?? 0);
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        private void WriteSamples16BitConvert(Samples<T> samples)
        {
            var converter = new SampleConverter<T, short>();

            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(sample));
                    stream.Write(bytes, 0, 2);
                }
            }
            else
            {
                var data = new byte[samples.Count * 2];

                for (var i = 0; i < data.Length; i += 2)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(samples[i / 2]));
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        # endregion

        # region Private 24-bit sample writing methods.

        private void WriteSamples24Bit(Samples<T> samples)
        {
            if (Format == WavFormat.Pcm)
            {
                if (typeof(T) == typeof(int))
                {
                    WriteSamples24BitNoConvert(samples);
                }
                else
                {
                    WriteSamples24BitConvert(samples);
                }
            }
            else
            {
                WriteAsFloats(samples);
            }
        }

        private void WriteSamples24BitNoConvert(Samples<T> samples)
        {
            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var allBytes = BitConverter.GetBytes(sample as int? ?? 0);
                    var sampleBytes = new[] { allBytes[0], allBytes[1], allBytes[2] };

                    stream.Write(sampleBytes, 0, 3);
                }
            }
            else
            {
                var data = new byte[samples.Count * 3];

                for (var i = 0; i < data.Length; i += 3)
                {
                    var allBytes = BitConverter.GetBytes(samples[i / 3] as int? ?? 0);
                    var sampleBytes = new[] { allBytes[0], allBytes[1], allBytes[2] };
                    data[i] = sampleBytes[0];
                    data[i + 1] = sampleBytes[1];
                    data[i + 2] = sampleBytes[2];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        private void WriteSamples24BitConvert(Samples<T> samples)
        {
            var converter = new SampleConverter<T, short>();

            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var allBytes = BitConverter.GetBytes(converter.Convert(sample));
                    var sampleBytes = new[] { allBytes[0], allBytes[1], allBytes[2] };

                    stream.Write(sampleBytes, 0, 3);
                }
            }
            else
            {
                var data = new byte[samples.Count * 3];

                for (var i = 0; i < data.Length; i += 3)
                {
                    var allBytes = BitConverter.GetBytes(converter.Convert(samples[i / 3]));
                    var sampleBytes = new[] { allBytes[0], allBytes[1], allBytes[2] };
                    data[i] = sampleBytes[0];
                    data[i + 1] = sampleBytes[1];
                    data[i + 2] = sampleBytes[2];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        # endregion

        # region Private 32-bit sample writing methods.

        private void WriteSamples32Bit(Samples<T> samples)
        {
            if (Format == WavFormat.Pcm)
            {
                if (typeof(T) == typeof(int))
                {
                    WriteSamples32BitNoConvert(samples);
                }
                else
                {
                    WriteSamples32BitConvert(samples);
                }
            }
            else
            {
                WriteAsFloats(samples);
            }
        }

        private void WriteSamples32BitNoConvert(Samples<T> samples)
        {
            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(sample as int? ?? 0);
                    stream.Write(bytes, 0, 4);
                }
            }
            else
            {
                var data = new byte[samples.Count * 4];

                for (var i = 0; i < data.Length; i += 4)
                {
                    var bytes = BitConverter.GetBytes(samples[i / 4] as int? ?? 0);
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                    data[i + 2] = bytes[2];
                    data[i + 3] = bytes[3];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        private void WriteSamples32BitConvert(Samples<T> samples)
        {
            var converter = new SampleConverter<T, int>();

            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(sample));
                    stream.Write(bytes, 0, 4);
                }
            }
            else
            {
                var data = new byte[samples.Count * 4];

                for (var i = 0; i < data.Length; i += 4)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(samples[i / 4]));
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                    data[i + 2] = bytes[2];
                    data[i + 3] = bytes[3];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        # endregion

        # region Private 64-bit sample writing methods.

        // TODO: Finish off refactoring/optimising 64-bit sample writing methods.

        private void WriteSamples64Bit(Samples<T> samples)
        {
            if (Format == WavFormat.Pcm)
            {
                var converter = new SampleConverter<T, long>();

                foreach (var sample in samples)
                {
                    stream.Write(BitConverter.GetBytes(converter.Convert(sample)), 0, 8);
                }
            }
            else
            {
                var converter = new SampleConverter<T, double>();

                foreach (var sample in samples)
                {
                    stream.Write(BitConverter.GetBytes(converter.Convert(sample)), 0, 8);
                }
            }
        }

        private void WriteSamples64BitNoConvert(Samples<T> samples)
        {
            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(sample as long? ?? 0);
                    stream.Write(bytes, 0, 8);
                }
            }
            else
            {
                var data = new byte[samples.Count * 8];

                for (var i = 0; i < data.Length; i += 8)
                {
                    var bytes = BitConverter.GetBytes(samples[i / 8] as long? ?? 0);
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                    data[i + 2] = bytes[2];
                    data[i + 3] = bytes[3];
                    data[i + 1] = bytes[4];
                    data[i + 2] = bytes[5];
                    data[i + 3] = bytes[6];
                    data[i + 3] = bytes[7];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        private void WriteSamples64BitConvert(Samples<T> samples)
        {
            var converter = new SampleConverter<T, long>();

            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(sample));
                    stream.Write(bytes, 0, 8);
                }
            }
            else
            {
                var data = new byte[samples.Count * 8];

                for (var i = 0; i < data.Length; i += 8)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(samples[i / 8]));
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                    data[i + 2] = bytes[2];
                    data[i + 3] = bytes[3];
                    data[i + 1] = bytes[4];
                    data[i + 2] = bytes[5];
                    data[i + 3] = bytes[6];
                    data[i + 3] = bytes[7];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        # endregion

        # region Private float writing methods.

        private void WriteAsFloats(Samples<T> samples)
        {
            if (typeof(T) == typeof(float))
            {
                WriteAsFloatsNoConvert(samples);
            }
            else
            {
                WriteAsFloatsConvert(samples);
            }
        }

        private void WriteAsFloatsNoConvert(Samples<T> samples)
        {
            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(sample as float? ?? 0);
                    stream.Write(bytes, 0, 4);
                }
            }
            else
            {
                var data = new byte[samples.Count * 4];

                for (var i = 0; i < data.Length; i += 4)
                {
                    var bytes = BitConverter.GetBytes(samples[i / 4] as float? ?? 0);
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                    data[i + 2] = bytes[2];
                    data[i + 3] = bytes[3];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        private void WriteAsFloatsConvert(Samples<T> samples)
        {
            var converter = new SampleConverter<T, float>();

            if (WritePriority == IoPriority.Memory)
            {
                foreach (var sample in samples)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(sample));
                    stream.Write(bytes, 0, 4);
                }
            }
            else
            {
                var data = new byte[samples.Count * 4];

                for (var i = 0; i < data.Length; i += 4)
                {
                    var bytes = BitConverter.GetBytes(converter.Convert(samples[i / 4]));
                    data[i] = bytes[0];
                    data[i + 1] = bytes[1];
                    data[i + 2] = bytes[2];
                    data[i + 3] = bytes[3];
                }

                stream.Write(data, 0, data.Length);
            }
        }

        # endregion

        private uint GetSpeakerMask()
        {
            uint mask = 0;

            foreach (var ch in AudioData)
            {
                mask |= (uint)ch.Position;
            }

            return mask;
        }

        private Samples<T> CombineSamples()
        {
            if (AudioData.Count == 1)
            {
                return AudioData[0].Samples;
            }

            var combined = new List<T>();
            var sorted = SortChannels();
            var totalSampleCount = int.MaxValue;

            // Find the smallest channel.
            for (var i = 0; i < sorted.Count; i++)
            {
                var k = sorted[i].Count;

                if (k < totalSampleCount)
                {
                    totalSampleCount = k;
                }
            }
            
            totalSampleCount *= AudioData.Count;

            for (var i = 0; i < totalSampleCount; i += AudioData.Count) // Jump every frame.
            {
                for (var k = 0; k < AudioData.Count; k++) // Iterate over the frame.
                {
                    combined.Add(sorted[k][i / AudioData.Count]);
                }
            }

            return new Samples<T>(combined);
        }

        private List<Samples<T>> SortChannels()
        {
            var sorted = new List<Samples<T>>();

            foreach (var ch in AudioData)
            {
                if (sorted.Count == 0)
                {
                    sorted.Add(ch.Samples);
                }
                else
                {
                    if (ch.Position > AudioData[0].Position)
                    {
                        sorted.Add(ch.Samples);
                    }
                    else
                    {
                        sorted.Insert(0, ch.Samples);
                    }
                }
            }

            return sorted;
        }
    }
}
