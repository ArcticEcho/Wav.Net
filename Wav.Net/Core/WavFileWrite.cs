using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;



namespace WavDotNet.Core
{
	public class WavFileWrite<T> : WavFile, IDisposable
	{
		private readonly string filePath;
		private int headerSize;
		private bool flushed;
		private bool disposed;
		private Stream stream;

		public Collection<Channel<T>> AudioData { get; private set; }

		public ushort ChannelCount
		{
			get
			{
				return (ushort)AudioData.Count;
			}
		}



		public WavFileWrite(string filePath, uint sampleRate, WavFormat format, ushort bitDepth, ushort validBits)
		{
			if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
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
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(string filePath, uint sampleRate, WavFormat format, ushort bitDepth)
		{
			if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
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
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(string filePath, uint sampleRate, WavFormat format)
		{
			if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

			this.filePath = filePath;
			Format = format;
			SampleRate = sampleRate;
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(string filePath, uint sampleRate)
		{
			if (String.IsNullOrEmpty(filePath)) { throw new ArgumentException("Can not be null or empty.", "filePath"); }
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

			this.filePath = filePath;
			SampleRate = sampleRate;
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(Stream stream, uint sampleRate, WavFormat format, ushort bitDepth, ushort validBits)
		{
			if (stream == null) { throw new ArgumentNullException("stream"); }
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
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(Stream stream, uint sampleRate, WavFormat format, ushort bitDepth)
		{
			if (stream == null) { throw new ArgumentNullException("stream"); }
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
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(Stream stream, uint sampleRate, WavFormat format)
		{
			if (stream == null) { throw new ArgumentNullException("stream"); }
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

			this.stream = stream;
			Format = format;
			SampleRate = sampleRate;
			AudioData = new Collection<Channel<T>>();
		}

		public WavFileWrite(Stream stream, uint sampleRate)
		{
			if (stream == null) { throw new ArgumentNullException("stream"); }
			if (sampleRate == 0) { throw new ArgumentOutOfRangeException("sampleRate", "Can not be 0."); }

			this.stream = stream;
			SampleRate = sampleRate;
			AudioData = new Collection<Channel<T>>();
		}

		~WavFileWrite()
		{
			if (!disposed)
			{
				Dispose();
			}
		}



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

			stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

			AudioLengthBytes = CalcAudioSize();

			CheckAudioData();

			var samples = CombineSamples();

			WriteMetaData();

			WriteAudioData(samples);

			flushed = true;

			stream.Dispose();
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

				case "System.Decimal":
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

				case "System.Decimal":
				{
					return 128;
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

				if (hasMono) { throw new InvalidWavDataException("'AudioData' can not contain a 'Mono' channel if multiple channels are present."); }
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

				if (count > 1) { throw new InvalidWavDataException("'AudioData' can not contain multiple channels set with the same 'ChannelPosition'."); }
			}
		}

		private void WriteMetaData()
		{	// \x00\x00\x00\x00\x10\x00\x80\x00\x00\xAA\x00\x38\x9B\x71
			var guid = new List<byte>(BitConverter.GetBytes((ushort)Format));
			guid.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71});

			//var ieeeGuid = new byte[] { 0, 0, 0, 3, 0, 0, 0, 16, 128, 0, 0, 170, 0, 56, 155, 113 };
			//var pcmGuid = new byte[] { 0, 0, 0, 1, 0, 0, 0, 16, 128, 0, 0, 170, 0, 56, 155, 113 };
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

				//case 128:
				//{
				//	WriteSamples128Bit(samples);

				//	break;
				//}

				default:
				{
					throw new NotSupportedException();
				}
			}
		}

		private void WriteSamples8Bit(Samples<T> samples)
		{
			stream.Position = headerSize;

			if (Format == WavFormat.Pcm)
			{
				var converter = new SampleConverter<T, byte>();

				foreach (var sample in samples)
				{
					stream.WriteByte(converter.Convert(sample));
				}
			}
			else
			{
				WriteAsFloats(samples);
			}		
		}

		private void WriteSamples16Bit(Samples<T> samples)
		{
			stream.Position = headerSize;
			
			if (Format == WavFormat.Pcm)
			{
				var converter = new SampleConverter<T, short>();

				foreach (var sample in samples)
				{
					stream.Write(BitConverter.GetBytes(converter.Convert(sample)), 0, 2);
				}
			}
			else
			{
				WriteAsFloats(samples);
			}	
		}

		private void WriteSamples24Bit(Samples<T> samples)
		{
			stream.Position = headerSize;

			if (Format == WavFormat.Pcm)
			{
				var converter = new SampleConverter<T, int>();

				foreach (var sample in samples)
				{
					var allBytes = BitConverter.GetBytes(converter.Convert(sample));

					var sampleBytes = new[] { allBytes[0], allBytes[1], allBytes[2] };

					stream.Write(sampleBytes, 0, 3);
				}
			}
			else
			{
				WriteAsFloats(samples);
			}			
		}

		private void WriteSamples32Bit(Samples<T> samples)
		{
			stream.Position = headerSize;

			if (Format == WavFormat.Pcm)
			{
				var converter = new SampleConverter<T, int>();

				foreach (var sample in samples)
				{
					stream.Write(BitConverter.GetBytes(converter.Convert(sample)), 0, 4);
				}
			}
			else
			{
				WriteAsFloats(samples);
			}
		}

		private void WriteSamples64Bit(Samples<T> samples)
		{
			stream.Position = headerSize;

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

		/// <summary>
		/// WARNING! This method has not yet been fully tested.
		/// </summary>
		//private void WriteSamples128Bit(Samples<T> samples)
		//{
		//	stream.Position = headerSize;
		//	var converted = new SampleConverter<T, decimal>().Convert(samples);

		//	for (var i = 0; i < converted.Length; i++)
		//	{
		//		var bytes = new List<byte>();

		//		foreach (var bin in decimal.GetBits(converted[i]))
		//		{
		//			bytes.AddRange(BitConverter.GetBytes(bin));
		//		}

		//		stream.Write(bytes.ToArray(), 0, bytes.Count);
		//	}		
		//}

		private void WriteAsFloats(Samples<T> samples)
		{
			var converter = new SampleConverter<T, float>();

			foreach (var sample in samples)
			{
				stream.Write(BitConverter.GetBytes(converter.Convert(sample)), 0, 4);
			}
		}

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
