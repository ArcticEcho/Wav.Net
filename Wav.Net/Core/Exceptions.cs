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
