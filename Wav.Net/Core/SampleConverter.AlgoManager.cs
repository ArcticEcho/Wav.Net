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
            private readonly invCulture = CultureInfo.InvariantCulture;

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

            private readonly GetSByte getSByte = sample => (sbyte?)Convert.ChangeType(sample, typeof(sbyte), invCulture) ?? 0;
            private readonly GetByte getByte = sample => (byte?)Convert.ChangeType(sample, typeof(byte), invCulture) ?? 0;
            private readonly GetInt16 getInt16 = sample => (short?)Convert.ChangeType(sample, typeof(short), invCulture) ?? 0;
            private readonly GetUInt16 getUInt16 = sample => (ushort?)Convert.ChangeType(sample, typeof(ushort), invCulture) ?? 0;
            private readonly GetInt32 getInt32 = sample => (int?)Convert.ChangeType(sample, typeof(int), invCulture) ?? 0;
            private readonly GetUInt32 getUInt32 = sample => (uint?)Convert.ChangeType(sample, typeof(uint), invCulture) ?? 0;
            private readonly GetInt64 getInt64 = sample => (long?)Convert.ChangeType(sample, typeof(long), invCulture) ?? 0;
            private readonly GetUInt64 getUInt64 = sample => (ulong?)Convert.ChangeType(sample, typeof(ulong), invCulture) ?? 0;
            private readonly GetSingle getSingle = sample => (float?)Convert.ChangeType(sample, typeof(float), invCulture) ?? 0;
            private readonly GetDouble getDouble = sample => (double?)Convert.ChangeType(sample, typeof(double), invCulture) ?? 0;
            private readonly GetDecimal getDecimal = sample => (decimal?)Convert.ChangeType(sample, typeof(decimal), invCulture) ?? 0;

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
                    { "System.SByte", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(getSByte(sample) + 128, outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(getSByte(sample) * 256, outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType((getSByte(sample) + 128) * 256, outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(getSByte(sample) * 16777216, outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType((getSByte(sample) + 128) * 16777216, outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(getSByte(sample) * 72057594037927936, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType((getSByte(sample) + 128) * 72057594037927936, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(getSByte(sample) / 128F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(getSByte(sample) / 128D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(getSByte(sample) / 128M, outType, invCulture) },
                };
            }

            private void PopulateFromByte()
            {
                fromByte = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(getByte(sample) - 128, outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType((getByte(sample) - 128) * 256, outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(getByte(sample) * 256, outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType((getByte(sample) - 128) * 16777216, outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(getByte(sample) * 16777216, outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType((getByte(sample) - 128) * 72057594037927936, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(getByte(sample) * 72057594037927936, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType((getByte(sample) - 128) / 128F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType((getByte(sample) - 128) / 128D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType((getByte(sample) - 128) / 128M, outType, invCulture) },
                };
            }

            private void PopulateFromInt16()
            {
                fromInt16 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getInt16(sample) / 256F), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round(getInt16(sample) / 256F) + 128, outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(getInt16(sample) + 32768, outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(getInt16(sample) * 65536, outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType((getInt16(sample) + 32768) * 65536, outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(getInt16(sample) * 281474976710656, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType((getInt16(sample) + 32768) * 281474976710656, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(getInt16(sample) / 32768F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(getInt16(sample) / 32768D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(getInt16(sample) / 32768M, outType, invCulture) },
                };
            }

            private void PopulateFromUInt16()
            {
                fromUInt16 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getUInt16(sample) / 256F) - 128, outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round(getUInt16(sample) / 256F), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(getUInt16(sample) - 32768, outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType((getUInt16(sample) - 32768) * 65536, outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(getUInt16(sample) * 65536, outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType((getUInt16(sample) - 32768) * 281474976710656, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(getUInt16(sample) * 281474976710656, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType((getUInt16(sample) - 32768) / 32768F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType((getUInt16(sample) - 32768) / 32768D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType((getUInt16(sample) - 32768) / 32768M, outType, invCulture) },
                };
            }

            private void PopulateFromInt32()
            {
                fromInt32 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getInt32(sample) / 16777216F), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round((getInt32(sample) / 16777216F) + 128), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round(getInt32(sample) / 65536F), outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(Math.Round((getInt32(sample) / 65536F) + 32768), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(getInt32(sample) + 2147483648, outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(getInt32(sample) * 4294967296, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType((getInt32(sample) + 2147483648) * 4294967296, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(getInt32(sample) / 2147483648F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(getInt32(sample) / 2147483648D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(getInt32(sample) / 2147483648M, outType, invCulture) },
                };
            }

            private void PopulateFromUInt32()
            {
                fromUInt32 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round((getUInt32(sample) / 16777216F) - 128), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round(getUInt32(sample) / 16777216F), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round((getUInt32(sample) / 65536F) - 32768), outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(Math.Round(getUInt32(sample) / 65536F), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(getUInt32(sample) - 2147483648, outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType((getUInt32(sample) - 2147483648) * 4294967296, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(getUInt32(sample) * 4294967296, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType((getUInt32(sample) - 2147483648) / 2147483648F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType((getUInt32(sample) - 2147483648) / 2147483648F, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType((getUInt32(sample) - 2147483648) / 2147483648F, outType, invCulture) },
                };
            }

            private void PopulateFromInt64()
            {
                fromInt64 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getInt64(sample) / 72057594037927936F), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round((getInt64(sample) / 72057594037927936F) + 128), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round(getInt64(sample) / 281474976710656F), outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(Math.Round((getInt64(sample) / 281474976710656F) + 32768), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(Math.Round(getInt64(sample) / 4294967296F), outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(Math.Round((getInt64(sample) / 4294967296F) + 2147483648), outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType((ulong)getInt64(sample) + 9223372036854775808, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(getInt64(sample) / 9223372036854775808F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(getInt64(sample) / 9223372036854775808D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(getInt64(sample) / 9223372036854775808M, outType, invCulture) },
                };
            }

            private void PopulateFromUInt64()
            {
                fromUInt64 = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round((getUInt64(sample) / 72057594037927936F) - 128), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round(getUInt64(sample) / 72057594037927936F), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round((getUInt64(sample) / 281474976710656F) - 32768), outType, invCulture) },
                    { "System.UInt16", sample => (TTo)Convert.ChangeType(Math.Round(getUInt64(sample) / 281474976710656F), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(Math.Round((getUInt64(sample) / 4294967296F) - 2147483648), outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(Math.Round(getUInt64(sample) / 4294967296F), outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(getUInt64(sample) - 9223372036854775808, outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType((getUInt64(sample) - 9223372036854775808) / 9223372036854775808F, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType((getUInt64(sample) - 9223372036854775808) / 9223372036854775808D, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType((getUInt64(sample) - 9223372036854775808) / 9223372036854775808M, outType, invCulture) },
                };
            }

            private void PopulateFromSingle()
            {
                fromSingle = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getSingle(sample) * 128), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round((getSingle(sample) * 128) + 128), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round(getSingle(sample) * 32768), outType, invCulture) },
                    { "System.UInt16",sample => (TTo)Convert.ChangeType(Math.Round((getSingle(sample) * 32768) + 32768), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(Math.Round(getSingle(sample) * 2147483648), outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(Math.Round((getSingle(sample) * 2147483648) + 2147483648), outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(Math.Round(getSingle(sample) * 9223372036854775808), outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(Math.Round((getSingle(sample) * 9223372036854775808) + 9223372036854775808), outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                };
            }

            private void PopulateFromDouble()
            {
                fromDouble = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getDouble(sample) * 128), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round((getDouble(sample) * 128) + 128), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round(getDouble(sample) * 32768), outType, invCulture) },
                    { "System.UInt16",sample => (TTo)Convert.ChangeType(Math.Round((getDouble(sample) * 32768) + 32768), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(Math.Round(getDouble(sample) * 2147483648), outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(Math.Round((getDouble(sample) * 2147483648) + 2147483648), outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(Math.Round(getDouble(sample) * 9223372036854775808), outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(Math.Round((getDouble(sample) * 9223372036854775808) + 9223372036854775808), outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(Math.Round(getDouble(sample), 7), outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                };
            }

            private void PopulateFromDecimal()
            {
                fromDecimal = new Dictionary<string, Algo>
                {
                    { "System.SByte", sample => (TTo)Convert.ChangeType(Math.Round(getDecimal(sample) * 128), outType, invCulture) },
                    { "System.Byte", sample => (TTo)Convert.ChangeType(Math.Round((getDecimal(sample) * 128) + 128), outType, invCulture) },
                    { "System.Int16", sample => (TTo)Convert.ChangeType(Math.Round(getDecimal(sample) * 32768), outType, invCulture) },
                    { "System.UInt16",sample => (TTo)Convert.ChangeType(Math.Round((getDecimal(sample) * 32768) + 32768), outType, invCulture) },
                    { "System.Int32", sample => (TTo)Convert.ChangeType(Math.Round(getDecimal(sample) * 2147483648), outType, invCulture) },
                    { "System.UInt32", sample => (TTo)Convert.ChangeType(Math.Round((getDecimal(sample) * 2147483648) + 2147483648), outType, invCulture) },
                    { "System.Int64", sample => (TTo)Convert.ChangeType(Math.Round(getDecimal(sample) * 9223372036854775808), outType, invCulture) },
                    { "System.UInt64", sample => (TTo)Convert.ChangeType(Math.Round((getDecimal(sample) * 9223372036854775808) + 9223372036854775808), outType, invCulture) },
                    { "System.Single", sample => (TTo)Convert.ChangeType(Math.Round(getDecimal(sample), 7), outType, invCulture) },
                    { "System.Double", sample => (TTo)Convert.ChangeType(Math.Round(getDecimal(sample), 15), outType, invCulture) },
                    { "System.Decimal", sample => (TTo)Convert.ChangeType(sample, outType, invCulture) },
                };
            }
        }
    }
}
