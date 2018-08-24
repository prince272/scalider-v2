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
        
        #region CrcAlgorithmParameters

        private static readonly IDictionary<string, CrcParameters> CrcAlgorithmParameters =
            new Dictionary<string, CrcParameters>(StringComparer.OrdinalIgnoreCase)
            {
                // CRC8
                {CrcAlgorithmNames.CRC8, new CrcParameters(8, 0x07, 0x00, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_CDMA2000, new CrcParameters(8, 0x9B, 0xFF, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_DARC, new CrcParameters(8, 0x39, 0x00, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_DVB_S2, new CrcParameters(8, 0xD5, 0x00, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_EBU, new CrcParameters(8, 0x1D, 0xFF, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_I_CODE, new CrcParameters(8, 0x1D, 0xFD, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_ITU, new CrcParameters(8, 0x07, 0x00, 0x55, false, false)},
                {CrcAlgorithmNames.CRC8_MAXIM, new CrcParameters(8, 0x31, 0x00, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_ROHC, new CrcParameters(8, 0x07, 0xFF, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_WCDMA, new CrcParameters(8, 0x9B, 0x00, 0x00, true, true)},

                // CRC16
                {CrcAlgorithmNames.CRC_A, new CrcParameters(16, 0x1021, 0xC6C6, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_ARC, new CrcParameters(16, 0x8005, 0x0000, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_AUG_CCITT, new CrcParameters(16, 0x1021, 0x1D0F, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_BUYPASS, new CrcParameters(16, 0x8005, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_CDMA2000, new CrcParameters(16, 0xC867, 0xFFFF, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_CCITT_FALSE, new CrcParameters(16, 0x1021, 0xFFFF, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_DDS_110, new CrcParameters(16, 0x8005, 0x800D, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_DECT_R, new CrcParameters(16, 0x0589, 0x0000, 0x0001, false, false)},
                {CrcAlgorithmNames.CRC16_DECT_X, new CrcParameters(16, 0x0589, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_DNP, new CrcParameters(16, 0x3D65, 0x0000, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_EN_13757, new CrcParameters(16, 0x3D65, 0x0000, 0xFFFF, false, false)},
                {CrcAlgorithmNames.CRC16_GENIBUS, new CrcParameters(16, 0x1021, 0xFFFF, 0xFFFF, false, false)},
                {CrcAlgorithmNames.CRC16_KERMIT, new CrcParameters(16, 0x1021, 0x0000, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_MAXIM, new CrcParameters(16, 0x8005, 0x0000, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_MCRF4XX, new CrcParameters(16, 0x1021, 0xFFFF, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_MODBUS, new CrcParameters(16, 0x8005, 0xFFFF, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_RIELLO, new CrcParameters(16, 0x1021, 0xB2AA, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_T10_DIF, new CrcParameters(16, 0x8BB7, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_TELEDISK, new CrcParameters(16, 0xA097, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_TMS37157, new CrcParameters(16, 0x1021, 0x89EC, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_USB, new CrcParameters(16, 0x8005, 0xFFFF, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_X_25, new CrcParameters(16, 0x1021, 0xFFFF, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_XMODEM, new CrcParameters(16, 0x1021, 0x0000, 0x0000, false, false)},

                // CRC32
                {CrcAlgorithmNames.CRC32, new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {
                    CrcAlgorithmNames.CRC32_BZIP2,
                    new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, false, false)
                },
                {CrcAlgorithmNames.CRC32_C, new CrcParameters(32, 0x1EDC6F41, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {CrcAlgorithmNames.CRC32_D, new CrcParameters(32, 0xA833982B, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {CrcAlgorithmNames.CRC32_JAMCRC, new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0x00000000, true, true)},
                {
                    CrcAlgorithmNames.CRC32_MPEG2,
                    new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0x00000000, false, false)
                },
                {
                    CrcAlgorithmNames.CRC32_POXIS,
                    new CrcParameters(32, 0x04C11DB7, 0x00000000, 0xFFFFFFFF, false, false)
                },
                {CrcAlgorithmNames.CRC32_Q, new CrcParameters(32, 0x814141AB, 0x00000000, 0x00000000, false, false)},
                {CrcAlgorithmNames.CRC32_XFER, new CrcParameters(32, 0x000000AF, 0x00000000, 0x00000000, false, false)}
            };
        
        #endregion

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
            ulong hash = _parameters.InitialValue;
            if (_parameters.ReflectIn)
                hash = ReflectBits(hash, _parameters.Width);

            // Calculate hash
            for (int i = ibStart; i < ibStart + cbSize; i++)
            {
                ulong b = array[i];
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

            // Done
            HashValue = hashBuffer;
            return hashBuffer;
        }
        
        /// <summary>
        /// Creates an instance of the default implementation of the <see cref="Crc"/> hash algorithm.
        /// </summary>
        /// <returns>
        /// A new instance of the <see cref="Crc"/>.
        /// </returns>
        public new static Crc Create()
            => new Crc(CrcAlgorithmParameters[CrcAlgorithmNames.CRC32]);

        /// <summary>
        /// Creates an instance of the specified implementation of the <see cref="Crc"/> hash algorithm.
        /// </summary>
        /// <param name="algorithmName">The name of the specific implementation of <see cref="Crc"/> to use.</param>
        /// <returns>
        /// A new instance of the specified implementation of <see cref="Crc"/>.
        /// </returns>
        public new static Crc Create(string algorithmName)
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
        
        #region GetOrCreateLookupTable

        private static ulong[] GetOrCreateLookupTable(CrcParameters p)
        {
            int refIn = p.ReflectIn ? 1 : 0;
            int refOut = p.ReflectOut ? 1 : 0;

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
        
        #endregion

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