using System;
using NUnit.Framework;

namespace Scalider
{
    
    [TestFixture]
    public class LuhnUtilityTests
    {

        [TestCase(null),
         TestCase(""),
         TestCase("abc"),
         TestCase("123a"),
         TestCase("0000-0000-0000-000")]
        public void CalculateCheckDigitShouldFailForInvalidInput(string value) =>
            Assert.That(
                () => LuhnUtility.CalculateCheckDigit(value),
                Throws.Exception
                      .TypeOf<ArgumentNullException>()
                      .Or
                      .TypeOf<ArgumentException>()
            );

        [TestCase("000000000000000", 0),
         TestCase("1234567890", 3),
         TestCase("0004120", 2),
         TestCase("12345678938190", 8)]
        public void CalculateCheckDigitShouldGenerateValiCheckDigits(string value, int expectedResult) =>
            Assert.AreEqual(expectedResult, LuhnUtility.CalculateCheckDigit(value));

        [TestCase(null),
         TestCase("")]
        public void ValidateShouldBeFalseForNullOrEmptyString(string value) =>
            Assert.IsFalse(LuhnUtility.Validate(value));


        [TestCase("0000-0000-0000-0000"),
         TestCase("12345678903"),
         TestCase("00041202"),
         TestCase("123456789381908")]
        public void ValidateShouldBeTrueForValues(string value) => Assert.IsTrue(LuhnUtility.Validate(value));
        
    }
    
}