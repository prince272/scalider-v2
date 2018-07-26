using System;
using Xunit;

namespace Scalider.Security.Test
{
    
    public class CrcTests
    {

        [Theory]
        [InlineData("Hello", "F7D18982")]
        [InlineData("Good bye", "86D0A38B")]
        [InlineData("Random string", "270E2C62")]
        public void Crc32ShouldMatchValues(string value, string expectedValue)
        {
            using (var crc = Crc.Create())
            {
                var hashed = crc.ComputeHash(value.ToByteArray()).ToHexString();
                Assert.Equal(expectedValue, hashed);
            }
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("algorithm")]
        public void InvalidAlgorithmShouldFail(string algorithmName)
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                using (var _ = Crc.Create(algorithmName))
                {
                }
            });
        }
        
        #region CRC8 Algorithms

        [Theory]
        [InlineData("Hello", "F6")]
        [InlineData("Good bye", "B8")]
        [InlineData("Random string", "25")]
        public void Crc8ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8", value, expectedValue);

        [Theory]
        [InlineData("Hello", "06")]
        [InlineData("Good bye", "81")]
        [InlineData("Random string", "E3")]
        public void Crc8Cdma2000ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/cdma2000", value, expectedValue);

        [Theory]
        [InlineData("Hello", "00")]
        [InlineData("Good bye", "30")]
        [InlineData("Random string", "86")]
        public void Crc8DarcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/darc", value, expectedValue);

        [Theory]
        [InlineData("Hello", "8E")]
        [InlineData("Good bye", "50")]
        [InlineData("Random string", "E0")]
        public void Crc8DvbS2ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/dvb-s2", value, expectedValue);

        [Theory]
        [InlineData("Hello", "D5")]
        [InlineData("Good bye", "35")]
        [InlineData("Random string", "89")]
        public void Crc8EbuShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/ebu", value, expectedValue);

        [Theory]
        [InlineData("Hello", "43")]
        [InlineData("Good bye", "14")]
        [InlineData("Random string", "FD")]
        public void Crc8ICodeShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/i-code", value, expectedValue);

        [Theory]
        [InlineData("Hello", "A3")]
        [InlineData("Good bye", "ED")]
        [InlineData("Random string", "70")]
        public void Crc8ItuShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/itu", value, expectedValue);

        [Theory]
        [InlineData("Hello", "EB")]
        [InlineData("Good bye", "DE")]
        [InlineData("Random string", "0D")]
        public void Crc8MaximShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/maxim", value, expectedValue);

        [Theory]
        [InlineData("Hello", "C9")]
        [InlineData("Good bye", "11")]
        [InlineData("Random string", "2A")]
        public void Crc8RohcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/rohc", value, expectedValue);

        [Theory]
        [InlineData("Hello", "E1")]
        [InlineData("Good bye", "D1")]
        [InlineData("Random string", "2C")]
        public void Crc8WcdmaShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/wcdma", value, expectedValue);
        
        #endregion
        
        #region CRC16 Algorithms

        [Theory]
        [InlineData("Hello", "9B0D")]
        [InlineData("Good bye", "0BFE")]
        [InlineData("Random string", "AD48")]
        public void CrcAShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc/a", value, expectedValue);

        [Theory]
        [InlineData("Hello", "F353")]
        [InlineData("Good bye", "E2EB")]
        [InlineData("Random string", "B420")]
        public void Crc16ArcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/arc", value, expectedValue);

        [Theory]
        [InlineData("Hello", "3A18")]
        [InlineData("Good bye", "AF44")]
        [InlineData("Random string", "3727")]
        public void Crc16AugCcittShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/aug-ccitt", value, expectedValue);

        [Theory]
        [InlineData("Hello", "B7C6")]
        [InlineData("Good bye", "300C")]
        [InlineData("Random string", "0F3E")]
        public void Crc16BuypassShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/buypass", value, expectedValue);

        [Theory]
        [InlineData("Hello", "DADA")]
        [InlineData("Good bye", "7F43")]
        [InlineData("Random string", "51E8")]
        public void Crc16CcittFalseShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/ccitt-false", value, expectedValue);

        [Theory]
        [InlineData("Hello", "C4E8")]
        [InlineData("Good bye", "F269")]
        [InlineData("Random string", "3958")]
        public void Crc16Cdma2000ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/cdma2000", value, expectedValue);

        [Theory]
        [InlineData("Hello", "185B")]
        [InlineData("Good bye", "813A")]
        [InlineData("Random string", "8F47")]
        public void Crc16T10DifShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/t10-dif", value, expectedValue);

        [Theory]
        [InlineData("Hello", "6FC6")]
        [InlineData("Good bye", "3EEC")]
        [InlineData("Random string", "0DC3")]
        public void Crc16Dds110ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dds-110", value, expectedValue);

        [Theory]
        [InlineData("Hello", "3BAA")]
        [InlineData("Good bye", "83AA")]
        [InlineData("Random string", "52F8")]
        public void Crc16DectRShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dect-r", value, expectedValue);

        [Theory]
        [InlineData("Hello", "3BAB")]
        [InlineData("Good bye", "83AB")]
        [InlineData("Random string", "52F9")]
        public void Crc16DectXShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dect-x", value, expectedValue);

        [Theory]
        [InlineData("Hello", "40EC")]
        [InlineData("Good bye", "BB5C")]
        [InlineData("Random string", "5054")]
        public void Crc16DnpShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dnp", value, expectedValue);

        [Theory]
        [InlineData("Hello", "640D")]
        [InlineData("Good bye", "5F81")]
        [InlineData("Random string", "E8C8")]
        public void Crc16En13757ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/en-13757", value, expectedValue);

        [Theory]
        [InlineData("Hello", "9B5B")]
        [InlineData("Good bye", "5EC4")]
        [InlineData("Random string", "8E70")]
        public void Crc16KermitShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/kermit", value, expectedValue);

        [Theory]
        [InlineData("Hello", "2525")]
        [InlineData("Good bye", "80BC")]
        [InlineData("Random string", "AE17")]
        public void Crc16GenibusShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/genibus", value, expectedValue);

        [Theory]
        [InlineData("Hello", "0CAC")]
        [InlineData("Good bye", "1D14")]
        [InlineData("Random string", "4BDF")]
        public void Crc16MaximShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/maxim", value, expectedValue);

        [Theory]
        [InlineData("Hello", "ABD3")]
        [InlineData("Good bye", "2248")]
        [InlineData("Random string", "BE64")]
        public void Crc16Mcrf4XxShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/mcrf4xx", value, expectedValue);

        [Theory]
        [InlineData("Hello", "F377")]
        [InlineData("Good bye", "E9AB")]
        [InlineData("Random string", "5F23")]
        public void Crc16ModbusShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/modbus", value, expectedValue);

        [Theory]
        [InlineData("Hello", "9D4C")]
        [InlineData("Good bye", "0384")]
        [InlineData("Random string", "11CB")]
        public void Crc16RielloShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/riello", value, expectedValue);

        [Theory]
        [InlineData("Hello", "FE02")]
        [InlineData("Good bye", "F659")]
        [InlineData("Random string", "4270")]
        public void Crc16TelediskShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/teledisk", value, expectedValue);

        [Theory]
        [InlineData("Hello", "B5D9")]
        [InlineData("Good bye", "5EB3")]
        [InlineData("Random string", "AB5F")]
        public void Crc16Tms37157ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/tms37157", value, expectedValue);

        [Theory]
        [InlineData("Hello", "0C88")]
        [InlineData("Good bye", "1654")]
        [InlineData("Random string", "A0DC")]
        public void Crc16UsbShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/usb", value, expectedValue);

        [Theory]
        [InlineData("Hello", "542C")]
        [InlineData("Good bye", "DDB7")]
        [InlineData("Random string", "419B")]
        public void Crc16X25ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/x-25", value, expectedValue);

        [Theory]
        [InlineData("Hello", "CBD6")]
        [InlineData("Good bye", "4E7D")]
        [InlineData("Random string", "79E4")]
        public void Crc16XmodemShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/xmodem", value, expectedValue);
        
        #endregion
        
        #region CRC32 Algorithms

        [Theory]
        [InlineData("Hello", "1A546492")]
        [InlineData("Good bye", "5AA88CA0")]
        [InlineData("Random string", "DBB21FAD")]
        public void Crc32Bzip2ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/bzip2", value, expectedValue);

        [Theory]
        [InlineData("Hello", "81D90E1B")]
        [InlineData("Good bye", "02E54841")]
        [InlineData("Random string", "84D698BA")]
        public void Crc32CShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/c", value, expectedValue);

        [Theory]
        [InlineData("Hello", "DAB57281")]
        [InlineData("Good bye", "134EA988")]
        [InlineData("Random string", "E33A26D9")]
        public void Crc32DShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/d", value, expectedValue);

        [Theory]
        [InlineData("Hello", "082E767D")]
        [InlineData("Good bye", "792F5C74")]
        [InlineData("Random string", "D8F1D39D")]
        public void Crc32JamCrcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/jamcrc", value, expectedValue);

        [Theory]
        [InlineData("Hello", "E5AB9B6D")]
        [InlineData("Good bye", "A557735F")]
        [InlineData("Random string", "244DE052")]
        public void Crc32Mpeg2ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/mpeg2", value, expectedValue);

        [Theory]
        [InlineData("Hello", "5D44DF0E")]
        [InlineData("Good bye", "33AC37F9")]
        [InlineData("Random string", "652FCEA2")]
        public void Crc32PoxisShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/poxis", value, expectedValue);

        [Theory]
        [InlineData("Hello", "105D1351")]
        [InlineData("Good bye", "07926283")]
        [InlineData("Random string", "F9CAC1D8")]
        public void Crc32QShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/q", value, expectedValue);

        [Theory]
        [InlineData("Hello", "0ACEF329")]
        [InlineData("Good bye", "4EDED98A")]
        [InlineData("Random string", "0B9446E1")]
        public void Crc32XferShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/xfer", value, expectedValue);
        
        #endregion

        private static void TestAlgorithmWithMatchingValue(string algorithm, string value, string expectedValue)
        {
            using (var crc = Crc.Create(algorithm))
            {
                var hashed = crc.ComputeHash(value.ToByteArray()).ToHexString();
                Assert.Equal(expectedValue, hashed);
            }
        }
    }
    
}