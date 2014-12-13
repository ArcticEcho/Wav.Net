using System;
using System.Runtime.Serialization;



namespace WavDotNet.Core
{
    [Serializable]
    public class UnrecognisedWavFileException : Exception
    {
        public UnrecognisedWavFileException()
        {

        }

        public UnrecognisedWavFileException(string message) : base(message)
        {

        }

        public UnrecognisedWavFileException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected UnrecognisedWavFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    [Serializable]
    public class InvalidWavDataException : Exception
    {
        public InvalidWavDataException()
        {

        }

        public InvalidWavDataException(string message) : base(message)
        {

        }

        public InvalidWavDataException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected InvalidWavDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    [Serializable]
    public class ReadOnlyObjectException : Exception
    {
        public ReadOnlyObjectException()
        {

        }

        public ReadOnlyObjectException(string message) : base(message)
        {

        }

        public ReadOnlyObjectException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected ReadOnlyObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    [Serializable]
    public class AlreadyFlushedException : Exception
    {
        public AlreadyFlushedException()
        {

        }

        public AlreadyFlushedException(string message) : base(message)
        {

        }

        public AlreadyFlushedException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected AlreadyFlushedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
