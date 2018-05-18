using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Scalider.Security
{
    
    /// <summary>
    /// Computes the Cyclic Redundancy Check (CRC) hash for the input data.
    /// </summary>
    public sealed class Crc : HashAlgorithm
    {
        
        private static readonly ConcurrentDictionary<string, ulong[]> LookupTableCache =
            new ConcurrentDictionary<string, ulong[]>();

        private static readonly IDictionary<string, CrcParameters> CrcAlgorithmParameters =
            new Dictionary<string, CrcParameters>(StringComparer.OrdinalIgnoreCase)
            {
                // CRC8
                {"CRC8", new CrcParameters(8, 0x07, 0x00, 0x00, false, false)},
                {"CRC8/CDMA2000", new CrcParameters(8, 0x9B, 0xFF, 0x00, false, false)},
                {"CRC8/DARC", new CrcParameters(8, 0x39, 0x00, 0x00, true, true)},
                {"CRC8/DVB-S2", new CrcParameters(8, 0xD5, 0x00, 0x00, false, false)},
                {"CRC8/EBU", new CrcParameters(8, 0x1D, 0xFF, 0x00, true, true)},
                {"CRC8/I-CODE", new CrcParameters(8, 0x1D, 0xFD, 0x00, false, false)},
                {"CRC8/ITU", new CrcParameters(8, 0x07, 0x00, 0x55, false, false)},
                {"CRC8/MAXIM", new CrcParameters(8, 0x31, 0x00, 0x00, true, true)},
                {"CRC8/ROHC", new CrcParameters(8, 0x07, 0xFF, 0x00, true, true)},
                {"CRC8/WCDMA", new CrcParameters(8, 0x9B, 0x00, 0x00, true, true)},
                
                // CRC16
                {"CRC/A", new CrcParameters(16, 0x1021, 0xC6C6, 0x0000, true, true)},
                {"CRC16/ARC", new CrcParameters(16, 0x8005, 0x0000, 0x0000, true, true)},
                {"CRC16/AUG-CCITT", new CrcParameters(16, 0x1021, 0x1D0F, 0x0000, false, false)},
                {"CRC16/BUYPASS", new CrcParameters(16, 0x8005, 0x0000, 0x0000, false, false)},
                {"CRC16/CDMA2000", new CrcParameters(16, 0xC867, 0xFFFF, 0x0000, false, false)},
                {"CRC16/CCITT-FALSE", new CrcParameters(16, 0x1021, 0xFFFF, 0x0000, false, false)},
                {"CRC16/DDS-110", new CrcParameters(16, 0x8005, 0x800D, 0x0000, false, false)},
                {"CRC16/DECT-R", new CrcParameters(16, 0x0589, 0x0000, 0x0001, false, false)},
                {"CRC16/DECT-X", new CrcParameters(16, 0x0589, 0x0000, 0x0000, false, false)},
                {"CRC16/DNP", new CrcParameters(16, 0x3D65, 0x0000, 0xFFFF, true, true)},
                {"CRC16/EN-13757", new CrcParameters(16, 0x3D65, 0x0000, 0xFFFF, false, false)},
                {"CRC16/GENIBUS", new CrcParameters(16, 0x1021, 0xFFFF, 0xFFFF, false, false)},
                {"CRC16/KERMIT", new CrcParameters(16, 0x1021, 0x0000, 0x0000, true, true)},
                {"CRC16/MAXIM", new CrcParameters(16, 0x8005, 0x0000, 0xFFFF, true, true)},
                {"CRC16/MCRF4XX", new CrcParameters(16, 0x1021, 0xFFFF, 0x0000, true, true)},
                {"CRC16/MODBUS", new CrcParameters(16, 0x8005, 0xFFFF, 0x0000, true, true)},
                {"CRC16/RIELLO", new CrcParameters(16, 0x1021, 0xB2AA, 0x0000, true, true)},
                {"CRC16/T10-DIF", new CrcParameters(16, 0x8BB7, 0x0000, 0x0000, false, false)},
                {"CRC16/TELEDISK", new CrcParameters(16, 0xA097, 0x0000, 0x0000, false, false)},
                {"CRC16/TMS37157", new CrcParameters(16, 0x1021, 0x89EC, 0x0000, true, true)},
                {"CRC16/USB", new CrcParameters(16, 0x8005, 0xFFFF, 0xFFFF, true, true)},
                {"CRC16/X-25", new CrcParameters(16, 0x1021, 0xFFFF, 0xFFFF, true, true)},
                {"CRC16/XMODEM", new CrcParameters(16, 0x1021, 0x0000, 0x0000, false, false)},

                // CRC32
                {"CRC32", new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {"CRC32/BZIP2", new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, false, false)},
                {"CRC32/C", new CrcParameters(32, 0x1EDC6F41, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {"CRC32/D", new CrcParameters(32, 0xA833982B, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {"CRC32/JAMCRC", new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0x00000000, true, true)},
                {"CRC32/MPEG2", new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0x00000000, false, false)},
                {"CRC32/POXIS", new CrcParameters(32, 0x04C11DB7, 0x00000000, 0xFFFFFFFF, false, false)},
                {"CRC32/Q", new CrcParameters(32, 0x814141AB, 0x00000000, 0x00000000, false, false)},
                {"CRC32/XFER", new CrcParameters(32, 0x000000AF, 0x00000000, 0x00000000, false, false)}
            };

        private readonly CrcParameters _parameters;
        private ulong _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc"/> class.
        /// </summary>
        /// <param name="parameters"></param>
        private Crc([NotNull] CrcParameters parameters)
        {
            Check.NotNull(parameters, nameof(parameters));

            _parameters = parameters;
            LookupTable = GetOrCreateLookupTable(parameters);
        }

        /// <summary>
        /// Gets a value indicating the lookup table generated by the given parameters.
        /// </summary>
        [UsedImplicitly]
        public ulong[] LookupTable { get; }

        /// <inheritdoc />
        public override void Initialize() => _hash = _parameters.ReflectOut
            ? ReflectBits(_parameters.InitialValue, 32)
            : _parameters.InitialValue;

        /// <inheritdoc />
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var hash = _parameters.InitialValue;
            if (_parameters.ReflectIn)
                hash = ReflectBits(hash, _parameters.Width);

            // Calculate hash
            for (var i = ibStart; i < ibStart + cbSize; i++)
            {
                var b = array[i];
                hash = _parameters.ReflectIn
                    ? LookupTable[(hash ^ b) & 0xff] ^ (hash >> 8)
                    : LookupTable[((hash >> (_parameters.Width - 8)) ^ b) & 0xff] ^ (hash << 8);

                hash &= uint.MaxValue >> 32;
            }

            // Source: https://stackoverflow.com/a/28661073
            // Per Mark Adler - ...the reflect out different from the reflect in (CRC-12/3GPP). 
            // In that one case, you need to bit reverse the output since the input is not reflected,
            // but the output is.
            if (_parameters.ReflectIn ^ _parameters.ReflectOut)
                hash = ReflectBits(hash, _parameters.Width);

            // Done
            _hash = hash;
        }

        /// <inheritdoc />
        protected override byte[] HashFinal()
        {
            var hashBuffer = ToBigEndianBytes(_hash ^ _parameters.XorOutValue);

            var leftover = _parameters.Width / hashBuffer.Length;
            var newHashBuffer = new byte[leftover];
            Array.Copy(hashBuffer, hashBuffer.Length - leftover, newHashBuffer, 0, leftover);
            hashBuffer = newHashBuffer;

#if NETSTANDARD2_0
            HashValue = hashBuffer;
#endif

            return hashBuffer;
        }
        
#if NETSTANDARD2_0
        public new static Crc Create()
#else
        public static Crc Create()
#endif
            => new Crc(CrcAlgorithmParameters["CRC32"]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="algorithmName"></param>
        /// <returns>
        /// </returns>
#if NETSTANDARD2_0
        public new static Crc Create(string algorithmName)
#else
        public static Crc Create(string algorithmName)
#endif
        {
            Check.NotNullOrEmpty(algorithmName, nameof(algorithmName));
            if (!CrcAlgorithmParameters.TryGetValue(algorithmName, out var parameters) ||
                parameters == null)
            {
                // Unknown algorithm or the algorithm parameters haven't been defined
                throw new ArgumentException(
                    $"No algorithm with the name \"{algorithmName}\" could be found.",
                    nameof(algorithmName)
                );
            }
            
            // Done
            return new Crc(parameters);
        }

        private static byte[] ToBigEndianBytes(ulong i)
        {
            var result = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        private static ulong[] GetOrCreateLookupTable(CrcParameters p)
        {
            var refIn = p.ReflectIn ? 1 : 0;
            var refOut = p.ReflectOut ? 1 : 0;

            var cacheKey = $"{p.Width}:{p.Polynomial}{p.InitialValue}{p.XorOutValue}{refIn}{refOut}";
            return LookupTableCache.GetOrAdd(
                cacheKey,
                t =>
                {
                    var lookupTable = new ulong[256];
                    var topBit = (ulong)1 << (p.Width - 1);

                    // Calculate lookup table
                    for (var i = 0; i < lookupTable.Length; i++)
                    {
                        var r = (ulong)i;
                        if (p.ReflectIn)
                            r = ReflectBits(r, p.Width);
                        else if (p.Width > 8)
                            r <<= p.Width - 8;

                        //var r = (ulong)inByte << (width - 8);
                        for (var j = 0; j < 8; j++)
                            r = (r & topBit) != 0 ? (r << 1) ^ p.Polynomial : r << 1;

                        if (p.ReflectIn)
                            r = ReflectBits(r, p.Width);

                        lookupTable[i] = r & (ulong.MaxValue >> (64 - p.Width));
                    }

                    // Done
                    return lookupTable;
                }
            );
        }

        /// <summary>
        /// Reflects the bits of a provided numeric value.
        /// </summary>
        /// <param name="b">Value to reflect the bits of.</param>
        /// <param name="bitCount">Number of bits in the provided value.</param>
        /// <returns>
        /// Bit-reflected version of the provided numeric value.
        /// </returns>
        private static ulong ReflectBits(ulong b, int bitCount)
        {
            ulong reflection = 0x00;
            for (var bitNumber = 0; bitNumber < bitCount; ++bitNumber)
                if (((b >> bitNumber) & 0x01) == 0x01)
                    reflection |= (ulong)1 << (bitCount - 1 - bitNumber);

            return reflection;
        }

    }
    
}