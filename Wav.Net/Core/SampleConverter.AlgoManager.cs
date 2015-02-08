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
using System.Globalization;

namespace WavDotNet.Core
{
    public partial class SampleConverter<TIn, TOut>
    {
        private class AlgoManager<TFrom, TTo>
        {
            private readonly Type outType = typeof(TTo);

            #region Private delegates.

            private delegate sbyte GetSByte(TFrom sample);
            private delegate byte GetByte(TFrom sample);
            private delegate short GetInt16(TFrom sample);
            private delegate ushort GetUInt16(TFrom sample);
            private delegate int GetInt32(TFrom sample);
            private delegate uint GetUInt32(TFrom sample);
            private delegate long GetInt64(TFrom sample);
            private delegate ulong GetUInt64(TFrom sample);
            private delegate float GetSingle(TFrom sample);
            private delegate double GetDouble(TFrom sample);
            private delegate decimal GetDecimal(TFrom sample);

            private readonly GetSByte getSByte = sample => (sbyte?)System.Convert.ChangeType(sample, typeof(sbyte), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetByte getByte = sample => (byte?)System.Convert.ChangeType(sample, typeof(byte), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetInt16 getInt16 = sample => (short?)System.Convert.ChangeType(sample, typeof(short), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetUInt16 getUInt16 = sample => (ushort?)System.Convert.ChangeType(sample, typeof(ushort), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetInt32 getInt32 = sample => (int?)System.Convert.ChangeType(sample, typeof(int), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetUInt32 getUInt32 = sample => (uint?)System.Convert.ChangeType(sample, typeof(uint), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetInt64 getInt64 = sample => (long?)System.Convert.ChangeType(sample, typeof(long), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetUInt64 getUInt64 = sample => (ulong?)System.Convert.ChangeType(sample, typeof(ulong), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetSingle getSingle = sample => (float?)System.Convert.ChangeType(sample, typeof(float), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetDouble getDouble = sample => (double?)System.Convert.ChangeType(sample, typeof(double), CultureInfo.InvariantCulture) ?? 0;
            private readonly GetDecimal getDecimal = sample => (decimal?)System.Convert.ChangeType(sample, typeof(decimal), CultureInfo.InvariantCulture) ?? 0;

            # endregion

            # region Private algos.

            private Dictionary<string, Algo> fromSByte;
            private Dictionary<string, Algo> fromByte;
            private Dictionary<string, Algo> fromInt16;
            private Dictionary<string, Algo> fromUInt16;
            private Dictionary<string, Algo> fromInt32;
            private Dictionary<string, Algo> fromUInt32;
            private Dictionary<string, Algo> fromInt64;
            private Dictionary<string, Algo> fromUInt64;
            private Dictionary<string, Algo> fromSingle;
            private Dictionary<string, Algo> fromDouble;
            private Dictionary<string, Algo> fromDecimal;

            # endregion

            public delegate TTo Algo(TFrom sample);



            public AlgoManager()
            {
                PopulateFromSByte();
                PopulateFromByte();
                PopulateFromInt16();
                PopulateFromUInt16();
                PopulateFromInt32();
                PopulateFromUInt32();
                PopulateFromInt64();
                PopulateFromUInt64();
                PopulateFromSingle();
                PopulateFromDouble();
                PopulateFromDecimal();
            }



            public Algo GetAlgo()
            {
                switch (typeof(TFrom).FullName)
                {
                    case "System.SByte":
                    {
                        return fromSByte[outType.FullName];
                    }

                    case "System.Byte":
                    {
                        return fromByte[outType.FullName];
                    }

                    case "System.Int16":
                    {
                        return fromInt16[outType.FullName];
                    }

                    case "System.UInt16":
                    {
                        return fromUInt16[outType.FullName];
                    }

                    case "System.Int32":
                    {
                        return fromInt32[outType.FullName];
                    }

                    case "System.UInt32":
                    {
                        return fromUInt32[outType.FullName];
                    }

                    case "System.Int64":
                    {
                        return fromInt64[outType.FullName];
                    }

                    case "System.UInt64":
                    {
                        return fromUInt64[outType.FullName];
                    }

                    case "System.Single":
                    {
                        return fromSingle[outType.FullName];
                    }

                    case "System.Double":
                    {
                        return fromDouble[outType.FullName];
                    }

                    case "System.Decimal":
                    {
                        return fromDecimal[outType.FullName];
                    }

                    default:
                    {
                        throw new NotSupportedException();
                    }
                }
            }



            private void PopulateFromSByte()
            {
                fromSByte = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(sample, outType, CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(getSByte(sample) + 128, outType, CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(getSByte(sample) * 256, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType((getSByte(sample) + 128) * 256, outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(getSByte(sample) * 16777216, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType((getSByte(sample) + 128) * 16777216, outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(getSByte(sample) * 72057594037927936, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType((getSByte(sample) + 128) * 72057594037927936, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(getSByte(sample) / 128f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(getSByte(sample) / 128d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(getSByte(sample) / 128m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromByte()
            {
                fromByte = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(getByte(sample) - 128, outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType((getByte(sample) - 128) * 256, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(getByte(sample) * 256, outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType((getByte(sample) - 128) * 16777216, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(getByte(sample) * 16777216, outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType((getByte(sample) - 128) * 72057594037927936, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(getByte(sample) * 72057594037927936, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType((getByte(sample) - 128) / 128f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType((getByte(sample) - 128) / 128d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType((getByte(sample) - 128) / 128m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromInt16()
            {
                fromInt16 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt16(sample) / 256f), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt16(sample) / 256f) + 128, outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(getInt16(sample) + 32768, outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(getInt16(sample) * 65536, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType((getInt16(sample) + 32768) * 65536, outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(getInt16(sample) * 281474976710656, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType((getInt16(sample) + 32768) * 281474976710656, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(getInt16(sample) / 32768f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(getInt16(sample) / 32768d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(getInt16(sample) / 32768m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromUInt16()
            {
                fromUInt16 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt16(sample) / 256f) - 128, outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt16(sample) / 256f), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(getUInt16(sample) - 32768, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType((getUInt16(sample) - 32768) * 65536, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(getUInt16(sample) * 65536, outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType((getUInt16(sample) - 32768) * 281474976710656, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(getUInt16(sample) * 281474976710656, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType((getUInt16(sample) - 32768) / 32768f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType((getUInt16(sample) - 32768) / 32768d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType((getUInt16(sample) - 32768) / 32768m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromInt32()
            {
                fromInt32 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt32(sample) / 16777216f), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round((getInt32(sample) / 16777216f) + 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt32(sample) / 65536f), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(Math.Round((getInt32(sample) / 65536f) + 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(getInt32(sample) + 2147483648, outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(getInt32(sample) * 4294967296, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType((getInt32(sample) + 2147483648) * 4294967296, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(getInt32(sample) / 2147483648f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(getInt32(sample) / 2147483648d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(getInt32(sample) / 2147483648m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromUInt32()
            {
                fromUInt32 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round((getUInt32(sample) / 16777216f) - 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt32(sample) / 16777216f), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round((getUInt32(sample) / 65536f) - 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt32(sample) / 65536f), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(getUInt32(sample) - 2147483648, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType((getUInt32(sample) - 2147483648) * 4294967296, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(getUInt32(sample) * 4294967296, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType((getUInt32(sample) - 2147483648) / 2147483648f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType((getUInt32(sample) - 2147483648) / 2147483648f, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType((getUInt32(sample) - 2147483648) / 2147483648f, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromInt64()
            {
                fromInt64 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt64(sample) / 72057594037927936f), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round((getInt64(sample) / 72057594037927936f) + 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt64(sample) / 281474976710656f), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(Math.Round((getInt64(sample) / 281474976710656f) + 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(Math.Round(getInt64(sample) / 4294967296f), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(Math.Round((getInt64(sample) / 4294967296f) + 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType((ulong)getInt64(sample) + 9223372036854775808, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(getInt64(sample) / 9223372036854775808f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(getInt64(sample) / 9223372036854775808d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(getInt64(sample) / 9223372036854775808m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromUInt64()
            {
                fromUInt64 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round((getUInt64(sample) / 72057594037927936f) - 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt64(sample) / 72057594037927936f), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round((getUInt64(sample) / 281474976710656f) - 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt64(sample) / 281474976710656f), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(Math.Round((getUInt64(sample) / 4294967296f) - 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(Math.Round(getUInt64(sample) / 4294967296f), outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(getUInt64(sample) - 9223372036854775808, outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType((getUInt64(sample) - 9223372036854775808) / 9223372036854775808f, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType((getUInt64(sample) - 9223372036854775808) / 9223372036854775808d, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType((getUInt64(sample) - 9223372036854775808) / 9223372036854775808m, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromSingle()
            {
                fromSingle = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getSingle(sample) * 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round((getSingle(sample) * 128) + 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round(getSingle(sample) * 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16",sample => (TTo)System.Convert.ChangeType(Math.Round((getSingle(sample) * 32768) + 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(Math.Round(getSingle(sample) * 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(Math.Round((getSingle(sample) * 2147483648) + 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(Math.Round(getSingle(sample) * 9223372036854775808), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(Math.Round((getSingle(sample) * 9223372036854775808) + 9223372036854775808), outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromDouble()
            {
                fromDouble = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getDouble(sample) * 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round((getDouble(sample) * 128) + 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round(getDouble(sample) * 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16",sample => (TTo)System.Convert.ChangeType(Math.Round((getDouble(sample) * 32768) + 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(Math.Round(getDouble(sample) * 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(Math.Round((getDouble(sample) * 2147483648) + 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(Math.Round(getDouble(sample) * 9223372036854775808), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(Math.Round((getDouble(sample) * 9223372036854775808) + 9223372036854775808), outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(Math.Round(getDouble(sample), 7), outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                };
            }

            private void PopulateFromDecimal()
            {
                fromDecimal = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)System.Convert.ChangeType(Math.Round(getDecimal(sample) * 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Byte", sample => (TTo)System.Convert.ChangeType(Math.Round((getDecimal(sample) * 128) + 128), outType , CultureInfo.InvariantCulture) },
                    { "System.Int16", sample => (TTo)System.Convert.ChangeType(Math.Round(getDecimal(sample) * 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt16",sample => (TTo)System.Convert.ChangeType(Math.Round((getDecimal(sample) * 32768) + 32768), outType , CultureInfo.InvariantCulture) },
                    { "System.Int32", sample => (TTo)System.Convert.ChangeType(Math.Round(getDecimal(sample) * 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt32", sample => (TTo)System.Convert.ChangeType(Math.Round((getDecimal(sample) * 2147483648) + 2147483648), outType , CultureInfo.InvariantCulture) },
                    { "System.Int64", sample => (TTo)System.Convert.ChangeType(Math.Round(getDecimal(sample) * 9223372036854775808), outType , CultureInfo.InvariantCulture) },
                    { "System.UInt64", sample => (TTo)System.Convert.ChangeType(Math.Round((getDecimal(sample) * 9223372036854775808) + 9223372036854775808), outType , CultureInfo.InvariantCulture) },
                    { "System.Single", sample => (TTo)System.Convert.ChangeType(Math.Round(getDecimal(sample), 7), outType , CultureInfo.InvariantCulture) },
                    { "System.Double", sample => (TTo)System.Convert.ChangeType(Math.Round(getDecimal(sample), 15), outType , CultureInfo.InvariantCulture) },
                    { "System.Decimal", sample => (TTo)System.Convert.ChangeType(sample, outType , CultureInfo.InvariantCulture) },
                };
            }
        }
    }
}
