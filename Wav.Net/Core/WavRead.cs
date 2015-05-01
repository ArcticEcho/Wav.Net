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
using System.Text;

namespace WavDotNet.Core
{
    /// <summary>
    /// Represents a class for gathering data (meta-data and actual audio data) from a .wav file/Stream.
    /// </summary>
    /// <typeparam name="T">The type of which all samples should be (converted if necessary, and) returned as.</typeparam>
    public class WavRead<T> : WavMeta, IDisposable, IEnumerable<SampleReader<T>>
    {
        private Stream stream;
        private string filePath;
        private bool disposed;
        private uint headerSize;
        private uint speakerMask;

        /// <summary>
        /// The size of the internal buffer(s), used when reading samples.
        /// </summary>
        public uint BufferCapacity { get; private set; }

        /// <summary>
        /// A Dictionary of SampleReader<T>s paired with a channel (I.E, the key, of type ChannelPositions) that represent the actual audio data.
        /// </summary>
        public Dictionary<ChannelPositions, SampleReader<T>> AudioData { get; private set; }

        /// <summary>
        /// The total number of channels.
        /// </summary>
        public ushort ChannelCount { get; private set; }

        /// <summary>
        /// Returns a SampleReader<T> for a given ChannelPositions key.
        /// </summary>
        public SampleReader<T> this[ChannelPositions channel]
        {
            get
            {
                if (!AudioData.ContainsKey(channel)) { throw new KeyNotFoundException(); }

                return AudioData[channel];
            }
        }



        # region Constructors/destructor.

        public WavRead(string filePath)
        {
            var ex = InitialiseFromFile(filePath, SampleReader<int>.DefaultBufferSize);
            if (ex != null) { throw ex; }
        }

        public WavRead(string filePath, uint internalBufferCapacity)
        {
            var ex = InitialiseFromFile(filePath, internalBufferCapacity);
            if (ex != null) { throw ex; }
        }

        public WavRead(Stream stream)
        {
            var ex = InitialiseFromStream(stream, SampleReader<int>.DefaultBufferSize);
            if (ex != null) { throw ex; }
        }

        public WavRead(Stream stream, uint internalBufferCapacity)
        {
            var ex = InitialiseFromStream(stream, internalBufferCapacity);
            if (ex != null) { throw ex; }
        }

        ~WavRead()
        {
            if (!disposed)
            {
                Dispose();
            }
        }

        # endregion



        # region Public methods/enumulator.

        /// <summary>
        /// Returns a SampleReader<T> for the given channel.
        /// </summary>
        public SampleReader<T> GetChannel(ChannelPositions channel)
        {
            foreach (var ch in AudioData)
            {
                if (ch.Key == channel)
                {
                    return ch.Value;
                }
            }

            throw new KeyNotFoundException("The file does not contain channel: " + channel);
        }

        /// <summary>
        /// Checks whether the specified channel exists in the Stream/file.
        /// </summary>
        /// <param name="channel">The channel to search for.</param>
        /// <returns>True if the channel is present, otherwise, false.</returns>
        public bool ChannelExists(ChannelPositions channel)
        {
            return AudioData.ContainsKey(channel);
        }

        public void Dispose()
        {
            if (disposed) { return; }

            foreach (var reader in AudioData.Values)
            {
                reader.Dispose();
            }

            GC.SuppressFinalize(this);
            disposed = true;
        }

        public IEnumerator<SampleReader<T>> GetEnumerator()
        {
            foreach (var ch in AudioData)
            {
                yield return ch.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        # endregion



        # region Private initialisation methods.

        private Exception InitialiseFromFile(string filePath, uint internalBufferCapacity)
        {
            if (String.IsNullOrEmpty(filePath)) { return new ArgumentException("Must not be null or empty.", "filePath"); }
            if (!File.Exists(filePath)) { return new FileNotFoundException(); }
            if (new FileInfo(filePath).Length > int.MaxValue) { return new ArgumentException("File is too large. Must be less than 2 GiB.", "filePath"); }
            if (internalBufferCapacity < 1024) { return new ArgumentOutOfRangeException("bufferCapacity", "Must be more than 1024 bytes."); }

            this.filePath = filePath;
            using (stream = File.OpenRead(filePath))
            {
                BufferCapacity = internalBufferCapacity;
                AudioData = new Dictionary<ChannelPositions, SampleReader<T>>();

                GetMeta();
                var ex = CheckMeta();
                if (ex != null) { return ex; }
            }

            AddChannels();

            return null;
        }

        private Exception InitialiseFromStream(Stream stream, uint internalBufferCapacity)
        {
            if (stream == null) { return new ArgumentNullException("stream"); }
            if (!stream.CanRead) { return new NotSupportedException("'stream' must be readable."); }
            if (stream.Length > int.MaxValue) { return new ArgumentException("Stream is too large. Must be less than 2GiB.", "stream"); }
            if (internalBufferCapacity < 1024) { return new ArgumentOutOfRangeException("bufferCapacity", "Must be more than 1024 bytes."); }

            this.stream = stream;
            BufferCapacity = internalBufferCapacity;
            AudioData = new Dictionary<ChannelPositions, SampleReader<T>>();

            GetMeta();
            var ex = CheckMeta();
            if (ex != null) { return ex; }

            AddChannels();

            return ex;
        }

        # endregion

        # region Private meta fetching/checking methods.

        private void GetMeta()
        {
            var cid = new byte[4];         // Chunk ID data.
            var sc1SData = new byte[4];    // Sub-chunk 1 size data.
            var afData = new byte[2];      // Audio format data.
            var guidAfData = new byte[16]; // GUID + Audio Format data.
            var chData = new byte[2];      // Channel count data.
            var srData = new byte[4];      // Sample rate data.
            var bpsData = new byte[2];     // Bits per sample data.
            var rbdData = new byte[2];     // Real bit depth.
            var cmData = new byte[4];      // Channel mask data.
            var bytes = new byte[1024];    // Header bytes.

            // Read header bytes.
            stream.Position = 0;
            stream.Read(bytes, 0, 1024);
            var header = Encoding.ASCII.GetString(bytes);

            // Read chunk ID.
            Buffer.BlockCopy(bytes, 0, cid, 0, 4);

            if (!header.StartsWith("RIFF", StringComparison.Ordinal)) { throw new UnrecognisedWavFileException("Stream is not in a recognised wav format."); }

            // Find where the "fmt " sub-chunk starts.
            var fmtStartIndex = header.IndexOf("fmt ", StringComparison.Ordinal) + 4;

            // Read sub-chunk 1 size.
            Buffer.BlockCopy(bytes, fmtStartIndex, sc1SData, 0, 4);

            // Read audio format.
            Buffer.BlockCopy(bytes, fmtStartIndex + 4, afData, 0, 2);

            // Read channel count.
            Buffer.BlockCopy(bytes, fmtStartIndex + 6, chData, 0, 2);

            // Read sample rate.
            Buffer.BlockCopy(bytes, fmtStartIndex + 8, srData, 0, 2);

            // Read bit depth.
            Buffer.BlockCopy(bytes, fmtStartIndex + 18, bpsData, 0, 2);

            // Check if sub-chunk extension exists.
            if (BitConverter.ToUInt16(afData, 0) == 65534)
            {
                var extraSize = new byte[2];

                // Read size of extra data.
                Buffer.BlockCopy(bytes, fmtStartIndex + 20, extraSize, 0, 2);

                // Read guid/format data.
                Buffer.BlockCopy(bytes, fmtStartIndex + 28, guidAfData, 0, 16);

                // Check if sub-chunk extension is the correct size and contains valid info. If not, it probably contains some other type of custom extension.
                if (BitConverter.ToUInt16(extraSize, 0) == 22 && (guidAfData[3] == 3 || guidAfData[3] == 1))
                {
                    // Read real bits per sample.
                    Buffer.BlockCopy(bytes, fmtStartIndex + 22, rbdData, 0, 2);

                    // Read speaker mask.
                    Buffer.BlockCopy(bytes, fmtStartIndex + 24, cmData, 0, 4);
                }
            }

            if (rbdData[0] == 0 && rbdData[1] == 0)
            {
                // Real bit depth not specified, assume real bit depth is same as bit depth.
                rbdData[0] = bpsData[0];
                rbdData[1] = bpsData[1];
            }

            headerSize = (uint)(header.IndexOf("data", StringComparison.Ordinal) + 8);
            Format = (WavFormat)(BitConverter.ToUInt16(afData, 0) == 65534 ? guidAfData[3] : BitConverter.ToUInt16(afData, 0));
            ChannelCount = BitConverter.ToUInt16(chData, 0);
            SampleRate = BitConverter.ToUInt32(srData, 0);
            BitDepth = BitConverter.ToUInt16(bpsData, 0);
            ValidBits = BitConverter.ToUInt16(rbdData, 0);
            speakerMask = BitConverter.ToUInt32(cmData, 0);

            AudioLengthBytes = (uint)(stream.Length - headerSize);
        }

        private Exception CheckMeta()
        {
            if (BitDepth == 0) { return new UnrecognisedWavFileException("File is displaying an invalid bit depth."); }
            if (ValidBits == 0) { return new UnrecognisedWavFileException("File is displaying an invalid real bit depth."); }
            if (SampleRate == 0) { return new UnrecognisedWavFileException("File is displaying an invalid sample rate."); }
            if (ChannelCount == 0) { return new UnrecognisedWavFileException("File is displaying an invalid number of channels."); }
            if (Format == WavFormat.Unknown) { return new UnrecognisedWavFileException("Can only read audio in either PCM or IEEE format."); }
            if (BitDepth != 8 && BitDepth != 16 && BitDepth != 24 && BitDepth != 32 && BitDepth != 64)
            {
                return new UnrecognisedWavFileException("File is of an unsupported bit depth of:" + BitDepth + ".\nSupported bit depths: 8, 16, 24, 32 and 64.");
            }
            if (BitDepth < ValidBits)
            {
                return new UnrecognisedWavFileException("File is displaying an invalid bit depth and/or invalid valid bits per sample. (The file is displaying a bit depth less than its valid bits per sample field.)");
            }
            
            return null;
        }

        private void AddChannels()
        {
            var mask = speakerMask;

            if (speakerMask == 0)
            {
                if (ChannelCount == 1)
                {
                    SampleReader<T> reader;

                    if (filePath != null)
                    {
                        reader = new SampleReader<T>(filePath, ChannelPositions.Mono, BufferCapacity);
                    }
                    else
                    {
                        reader = new SampleReader<T>(stream, ChannelPositions.Mono, BufferCapacity);
                    }

                    AudioData.Add(ChannelPositions.Mono, reader);

                    return;
                }

                mask = GetSpeakerMask(ChannelCount);
            }

            foreach (var pos in Enum.GetValues(typeof(ChannelPositions)))
            {
                var ch = (ChannelPositions)pos;

                if ((ch & (ChannelPositions)mask) != ch || ch == ChannelPositions.Mono) { continue; }

                SampleReader<T> reader;

                if (filePath != null)
                {
                    reader = new SampleReader<T>(filePath, ch, BufferCapacity);
                }
                else
                {
                    reader = new SampleReader<T>(stream, ch, BufferCapacity);
                }

                AudioData.Add(ch, reader);
            }
        }

        private static uint GetSpeakerMask(int channelCount)
        {
            if (channelCount == 8)
            {
                return 0x33F;
            }

            uint mask = 0;
            long[] posValues = (long[])Enum.GetValues(typeof(ChannelPositions));
            var positions = new uint[posValues.Length];

            for (int i = 0, j = 0; i < posValues.Length; i++)
            {
                var pos = posValues[i];

                if (pos != 0)
                {
                    positions[j] = (uint)pos;
                    j++;
                }
            }

            for (var i = 0; i < channelCount; i++)
            {
                mask += positions[i];
            }

            return mask;
        }

        # endregion
    }
}
