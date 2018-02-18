using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Scalider.Security
{

    /// <summary>
    /// Computes the 17-bits Cyclic Redundancy Check (CRC) hash for the input
    /// data.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CRC16 : HashAlgorithm
    {

        /// <summary>
        /// Gets a value indicating the default polynomial value.
        /// </summary>
        [UsedImplicitly] public const ushort DefaultPolynomial = 0xc920;

        /// <summary>
        /// Gets a value indicating the default seed value.
        /// </summary>
        [UsedImplicitly] public const ushort DefaultSeed = 0xffff;

        private static readonly ConcurrentDictionary<ushort, ushort[]> TableCache =
            new ConcurrentDictionary<ushort, ushort[]>();

        private readonly ushort _seed;
        private readonly ushort[] _table;

        private ushort _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="CRC16"/> class with
        /// custom polynomial and seed.
        /// </summary>
        [UsedImplicitly]
        public CRC16()
            : this(DefaultPolynomial)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CRC16"/> class with
        /// custom polynomial and seed.
        /// </summary>
        /// <param name="polynomial"></param>
        /// <param name="seed"></param>
        public CRC16(ushort polynomial, ushort seed = DefaultSeed)
        {
            _table = GetOrCreateTableForPolynomial(polynomial);
            _seed = seed;
        }

        /// <inheritdoc />
        public override void Initialize() => _hash = _seed;

        /// <inheritdoc />
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var hash = _seed;
            for (var i = ibStart; i < ibStart + cbSize; i++)
            {
                var index = (byte)(hash ^ array[i]);
                hash = (ushort)((hash >> 8) ^ _table[index]);
            }

            _hash = hash;
        }

        /// <inheritdoc />
        protected override byte[] HashFinal()
        {
            Console.WriteLine(_hash);
            var hashBuffer = UInt16ToBigEndianBytes((ushort)~_hash);
#if NETSTANDARD2_0
            HashValue = hashBuffer;
#endif

            return hashBuffer;
        }

        /// <summary>
        /// Creates an instance of the <see cref="CRC16"/> hash algorithm with
        /// the default values.
        /// </summary>
        /// <returns>
        /// </returns>
        public new static CRC16 Create() => new CRC16();

        /// <summary>
        /// Creates an instance of the <see cref="CRC16"/> hash algorithm with
        /// the default values.
        /// </summary>
        /// <param name="algorithmName">This parameter is ignored.</param>
        /// <returns>
        /// </returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public new static CRC16 Create(string algorithmName) => new CRC16();

        private static byte[] UInt16ToBigEndianBytes(ushort i)
        {
            var result = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        private static ushort[] GetOrCreateTableForPolynomial(ushort polynomial)
        {
            return TableCache.GetOrAdd(
                polynomial,
                t =>
                {
                    var table = new ushort[256];
                    for (var i = 0; i < table.Length; ++i)
                    {
                        ushort value = 0;
                        var temp = (ushort)i;

                        for (var j = 0; j < 8; ++j)
                        {
                            if (((value ^ temp) & 0x0001) != 0)
                                value = (ushort)((value >> 1) ^ polynomial);
                            else
                                value >>= 1;

                            temp >>= 1;
                        }

                        table[i] = value;
                    }

                    // Done
                    return table;
                }
            );
        }

    }
}