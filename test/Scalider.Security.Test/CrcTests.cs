using System;
using NUnit.Framework;
using Scalider.Security;

namespace Scalider
{
    
    public class CrcTests
    {

        [TestCase("Hello", "F7D18982"),
         TestCase("Good bye", "86D0A38B"),
         TestCase("Random string", "270E2C62")]
        public void Crc32ShouldMatchValues(string value, string expectedValue)
        {
            using (var crc = Crc.Create())
            {
                var hashed = crc.ComputeHash(value.ToByteArray()).ToHexString();
                Assert.AreEqual(expectedValue, hashed);
            }
        }

        [TestCase("invalid"),
         TestCase("algorithm")]
        public void InvalidAlgorithmShouldFail(string algorithmName)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var _ = Crc.Create(algorithmName))
                {
                }
            });
        }
        
        #region CRC8 Algorithms

        [TestCase("Hello", "F6"),
         TestCase("Good bye", "B8"),
         TestCase("Random string", "25")]
        public void Crc8ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8", value, expectedValue);

        [TestCase("Hello", "06"),
         TestCase("Good bye", "81"),
         TestCase("Random string", "E3")]
        public void Crc8Cdma2000ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/cdma2000", value, expectedValue);

        [TestCase("Hello", "00"),
         TestCase("Good bye", "30"),
         TestCase("Random string", "86")]
        public void Crc8DarcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/darc", value, expectedValue);

        [TestCase("Hello", "8E"),
         TestCase("Good bye", "50"),
         TestCase("Random string", "E0")]
        public void Crc8DvbS2ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/dvb-s2", value, expectedValue);

        [TestCase("Hello", "D5"),
         TestCase("Good bye", "35"),
         TestCase("Random string", "89")]
        public void Crc8EbuShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/ebu", value, expectedValue);

        [TestCase("Hello", "43"),
         TestCase("Good bye", "14"),
         TestCase("Random string", "FD")]
        public void Crc8ICodeShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/i-code", value, expectedValue);

        [TestCase("Hello", "A3"),
         TestCase("Good bye", "ED"),
         TestCase("Random string", "70")]
        public void Crc8ItuShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/itu", value, expectedValue);

        [TestCase("Hello", "EB"),
         TestCase("Good bye", "DE"),
         TestCase("Random string", "0D")]
        public void Crc8MaximShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/maxim", value, expectedValue);

        [TestCase("Hello", "C9"),
         TestCase("Good bye", "11"),
         TestCase("Random string", "2A")]
        public void Crc8RohcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/rohc", value, expectedValue);

        [TestCase("Hello", "E1"),
         TestCase("Good bye", "D1"),
         TestCase("Random string", "2C")]
        public void Crc8WcdmaShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc8/wcdma", value, expectedValue);
        
        #endregion
        
        #region CRC16 Algorithms

        [TestCase("Hello", "9B0D"),
         TestCase("Good bye", "0BFE"),
         TestCase("Random string", "AD48")]
        public void CrcAShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc/a", value, expectedValue);

        [TestCase("Hello", "F353"),
         TestCase("Good bye", "E2EB"),
         TestCase("Random string", "B420")]
        public void Crc16ArcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/arc", value, expectedValue);

        [TestCase("Hello", "3A18"),
         TestCase("Good bye", "AF44"),
         TestCase("Random string", "3727")]
        public void Crc16AugCcittShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/aug-ccitt", value, expectedValue);

        [TestCase("Hello", "B7C6"),
         TestCase("Good bye", "300C"),
         TestCase("Random string", "0F3E")]
        public void Crc16BuypassShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/buypass", value, expectedValue);

        [TestCase("Hello", "DADA"),
         TestCase("Good bye", "7F43"),
         TestCase("Random string", "51E8")]
        public void Crc16CcittFalseShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/ccitt-false", value, expectedValue);

        [TestCase("Hello", "C4E8"),
         TestCase("Good bye", "F269"),
         TestCase("Random string", "3958")]
        public void Crc16Cdma2000ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/cdma2000", value, expectedValue);

        [TestCase("Hello", "185B"),
         TestCase("Good bye", "813A"),
         TestCase("Random string", "8F47")]
        public void Crc16T10DifShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/t10-dif", value, expectedValue);

        [TestCase("Hello", "6FC6"),
         TestCase("Good bye", "3EEC"),
         TestCase("Random string", "0DC3")]
        public void Crc16Dds110ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dds-110", value, expectedValue);

        [TestCase("Hello", "3BAA"),
         TestCase("Good bye", "83AA"),
         TestCase("Random string", "52F8")]
        public void Crc16DectRShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dect-r", value, expectedValue);

        [TestCase("Hello", "3BAB"),
         TestCase("Good bye", "83AB"),
         TestCase("Random string", "52F9")]
        public void Crc16DectXShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dect-x", value, expectedValue);

        [TestCase("Hello", "40EC"),
         TestCase("Good bye", "BB5C"),
         TestCase("Random string", "5054")]
        public void Crc16DnpShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/dnp", value, expectedValue);

        [TestCase("Hello", "640D"),
         TestCase("Good bye", "5F81"),
         TestCase("Random string", "E8C8")]
        public void Crc16En13757ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/en-13757", value, expectedValue);

        [TestCase("Hello", "9B5B"),
         TestCase("Good bye", "5EC4"),
         TestCase("Random string", "8E70")]
        public void Crc16KermitShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/kermit", value, expectedValue);

        [TestCase("Hello", "2525"),
         TestCase("Good bye", "80BC"),
         TestCase("Random string", "AE17")]
        public void Crc16GenibusShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/genibus", value, expectedValue);

        [TestCase("Hello", "0CAC"),
         TestCase("Good bye", "1D14"),
         TestCase("Random string", "4BDF")]
        public void Crc16MaximShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/maxim", value, expectedValue);

        [TestCase("Hello", "ABD3"),
         TestCase("Good bye", "2248"),
         TestCase("Random string", "BE64")]
        public void Crc16Mcrf4XxShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/mcrf4xx", value, expectedValue);

        [TestCase("Hello", "F377"),
         TestCase("Good bye", "E9AB"),
         TestCase("Random string", "5F23")]
        public void Crc16ModbusShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/modbus", value, expectedValue);

        [TestCase("Hello", "9D4C"),
         TestCase("Good bye", "0384"),
         TestCase("Random string", "11CB")]
        public void Crc16RielloShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/riello", value, expectedValue);

        [TestCase("Hello", "FE02"),
         TestCase("Good bye", "F659"),
         TestCase("Random string", "4270")]
        public void Crc16TelediskShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/teledisk", value, expectedValue);

        [TestCase("Hello", "B5D9"),
         TestCase("Good bye", "5EB3"),
         TestCase("Random string", "AB5F")]
        public void Crc16Tms37157ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/tms37157", value, expectedValue);

        [TestCase("Hello", "0C88"),
         TestCase("Good bye", "1654"),
         TestCase("Random string", "A0DC")]
        public void Crc16UsbShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/usb", value, expectedValue);

        [TestCase("Hello", "542C"),
         TestCase("Good bye", "DDB7"),
         TestCase("Random string", "419B")]
        public void Crc16X25ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/x-25", value, expectedValue);

        [TestCase("Hello", "CBD6"),
         TestCase("Good bye", "4E7D"),
         TestCase("Random string", "79E4")]
        public void Crc16XmodemShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc16/xmodem", value, expectedValue);
        
        #endregion
        
        #region CRC32 Algorithms

        [TestCase("Hello", "1A546492"),
         TestCase("Good bye", "5AA88CA0"),
         TestCase("Random string", "DBB21FAD")]
        public void Crc32Bzip2ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/bzip2", value, expectedValue);

        [TestCase("Hello", "81D90E1B"),
         TestCase("Good bye", "02E54841"),
         TestCase("Random string", "84D698BA")]
        public void Crc32CShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/c", value, expectedValue);

        [TestCase("Hello", "DAB57281"),
         TestCase("Good bye", "134EA988"),
         TestCase("Random string", "E33A26D9")]
        public void Crc32DShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/d", value, expectedValue);

        [TestCase("Hello", "082E767D"),
         TestCase("Good bye", "792F5C74"),
         TestCase("Random string", "D8F1D39D")]
        public void Crc32JamCrcShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/jamcrc", value, expectedValue);

        [TestCase("Hello", "E5AB9B6D"),
         TestCase("Good bye", "A557735F"),
         TestCase("Random string", "244DE052")]
        public void Crc32Mpeg2ShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/mpeg2", value, expectedValue);

        [TestCase("Hello", "5D44DF0E"),
         TestCase("Good bye", "33AC37F9"),
         TestCase("Random string", "652FCEA2")]
        public void Crc32PoxisShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/poxis", value, expectedValue);

        [TestCase("Hello", "105D1351"),
         TestCase("Good bye", "07926283"),
         TestCase("Random string", "F9CAC1D8")]
        public void Crc32QShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/q", value, expectedValue);

        [TestCase("Hello", "0ACEF329"),
         TestCase("Good bye", "4EDED98A"),
         TestCase("Random string", "0B9446E1")]
        public void Crc32XferShouldMatchValues(string value, string expectedValue) =>
            TestAlgorithmWithMatchingValue("crc32/xfer", value, expectedValue);
        
        #endregion

        private static void TestAlgorithmWithMatchingValue(string algorithm, string value, string expectedValue)
        {
            using (var crc = Crc.Create(algorithm))
            {
                var hashed = crc.ComputeHash(value.ToByteArray()).ToHexString();
                Assert.AreEqual(expectedValue, hashed);
            }
        }

    }
}