using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Scalider.Security
{

    /// <summary>
    /// Computes the 33-bits Cyclic Redundancy Check (CRC) hash for the input data.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CRC32 : HashAlgorithm
    {

        /// <summary>
        /// Gets a value indicating the default polynomial value.
        /// </summary>
        [UsedImplicitly] public const uint DefaultPolynomial = 0xedb88320u;

        /// <summary>
        /// Gets a value indicating the default seed value.
        /// </summary>
        [UsedImplicitly] public const uint DefaultSeed = 0xffffffffu;

        private static readonly ConcurrentDictionary<uint, uint[]> TableCache =
            new ConcurrentDictionary<uint, uint[]>();

        private readonly uint _seed;
        private readonly uint[] _table;

        private uint _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="CRC32"/> class with custom polynomial and seed.
        /// </summary>
        public CRC32()
            : this(DefaultPolynomial)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CRC32"/> class with custom polynomial and seed.
        /// </summary>
        /// <param name="polynomial"></param>
        /// <param name="seed"></param>
        [UsedImplicitly]
        public CRC32(uint polynomial, uint seed = DefaultSeed)
        {
            _table = GetOrCreateTableForPolynomial(polynomial);
            _seed = _hash = seed;
        }

        /// <inheritdoc />
        public override void Initialize() => _hash = _seed;

        /// <inheritdoc />
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var hash = _seed;
            for (var i = ibStart; i < ibStart + cbSize; i++)
                hash = (hash >> 8) ^ _table[array[i] ^ (hash & 0xff)];

            _hash = hash;
        }

        /// <inheritdoc />
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(~_hash);
#if NETSTANDARD2_0
            HashValue = hashBuffer;
#endif

            return hashBuffer;
        }

        /// <summary>
        /// Creates an instance of the <see cref="CRC32"/> hash algorithm with the default values.
        /// </summary>
        /// <returns>
        /// </returns>
        public new static CRC32 Create() => new CRC32();

        /// <summary>
        /// Creates an instance of the <see cref="CRC32"/> hash algorithm with the default values.
        /// </summary>
        /// <param name="algorithmName">This parameter is ignored.</param>
        /// <returns>
        /// </returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public new static CRC32 Create(string algorithmName) => new CRC32();

        private static byte[] UInt32ToBigEndianBytes(uint i)
        {
            var result = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        private static uint[] GetOrCreateTableForPolynomial(uint polynomial)
        {
            return TableCache.GetOrAdd(
                polynomial,
                t =>
                {
                    var table = new uint[256];
                    for (var i = 0; i < table.Length; i++)
                    {
                        var entry = (uint)i;
                        for (var j = 0; j < 8; j++)
                            if ((entry & 1) == 1)
                                entry = (entry >> 1) ^ polynomial;
                            else
                                entry = entry >> 1;

                        table[i] = entry;
                    }

                    // Done
                    return table;
                }
            );
        }

    }

}