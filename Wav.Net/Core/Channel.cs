using System;



namespace WavDotNet.Core
{
    public class Channel<T>
    {
        public Samples<T> Samples { get; private set; }
        public ChannelPositions Position { get; private set;}



        public Channel(Samples<T> samples, ChannelPositions position)
        {
            if (samples == null) { throw new ArgumentNullException("samples"); }

            Samples = samples;
            Position = position;
        }
    }
}
