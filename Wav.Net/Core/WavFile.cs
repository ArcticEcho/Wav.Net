using System;



namespace WavDotNet.Core
{
	public abstract class WavFile
	{
		public uint SampleRate { get; protected set; }
		public uint AudioLengthBytes { get; protected set; }
		public ushort BitDepth { get; protected set; }
		public ushort ValidBits { get; protected set; }
		public WavFormat Format { get; protected set; }
	}
}
