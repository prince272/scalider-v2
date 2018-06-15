/*
Modified from: https://derekwill.com/2017/03/29/crc-algorithm-implementation-in-c/
Simplified BSD License

Copyright 2017 Derek Will

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer 
in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
OF SUCH DAMAGE.
*/

using System;
using JetBrains.Annotations;

namespace Scalider.Security
{

    internal class CrcParameters
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CrcParameters"/> class.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="polynomial"></param>
        /// <param name="initialValue"></param>
        /// <param name="xorOutValue"></param>
        /// <param name="reflectIn"></param>
        /// <param name="reflectOut"></param>
        public CrcParameters(int width, ulong polynomial, ulong initialValue, ulong xorOutValue, bool reflectIn,
            bool reflectOut)
        {
            ThrowIfParametersAreInvalid(width, polynomial, initialValue, xorOutValue);

            Width = width;
            Polynomial = polynomial;
            InitialValue = initialValue;
            XorOutValue = xorOutValue;
            ReflectIn = reflectIn;
            ReflectOut = reflectOut;
        }

        [UsedImplicitly]
        public int Width { get; }

        /// <summary>
        /// Gets the polynomial of the CRC algorithm.
        /// </summary>
        [UsedImplicitly]
        public ulong Polynomial { get; }

        /// <summary>
        /// Gets the initial value used in the computation of the CRC check value.
        /// </summary>
        [UsedImplicitly]
        public ulong InitialValue { get; }

        /// <summary>
        /// Gets the value which is XORed to the final computed value before returning the check value.
        /// </summary>
        [UsedImplicitly]
        public ulong XorOutValue { get; }

        /// <summary>
        /// Gets a value indicating whether bytes are reflected before being processed.
        /// </summary>
        [UsedImplicitly]
        public bool ReflectIn { get; }

        /// <summary>
        /// Gets a value indicating whether the final computed value is reflected before the XOR stage.
        /// </summary>
        [UsedImplicitly]
        public bool ReflectOut { get; }

        private void ThrowIfParametersAreInvalid(int width, ulong polynomial, ulong initialValue, ulong xorOutValue)
        {
            var maxValue = ulong.MaxValue >> (64 - width);
            if (polynomial > maxValue)
                throw new ArgumentOutOfRangeException(nameof(polynomial), $"Polynomial exceeds {width} bits.");

            if (initialValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(initialValue), $"Initial Value exceeds {width} bits.");

            if (xorOutValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(xorOutValue), $"XOR Out Value exceeds {width} bits.");
        }

    }

}