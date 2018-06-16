using System;
using System.Collections.Generic;
using Xunit;

namespace Scalider.Security.Test
{
    
    public class LuhnUtilityTests
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("123a")]
        [InlineData("0000-0000-0000-000")]
        public void CalculateCheckDigitShouldFailForInvalidInput(string value)
        {
            var exception = Assert.ThrowsAny<Exception>(() => LuhnUtility.CalculateCheckDigit(value));
            Assert.Contains(
                exception.GetType(),
                new List<Type>
                {
                    typeof(ArgumentNullException),
                    typeof(ArgumentException)
                }
            );
        }

        [Theory]
        [InlineData("000000000000000", 0)]
        [InlineData("1234567890", 3)]
        [InlineData("0004120", 2)]
        [InlineData("12345678938190", 8)]
        public void CalculateCheckDigitShouldGenerateValidCheckDigits(string inputValue, int expectedCheckDigit)
        {
            Assert.True(LuhnUtility.TryCalculateCheckDigit(inputValue, out var actualCheckDigit));
            Assert.Equal(expectedCheckDigit, actualCheckDigit);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("123a")]
        public void ValidationShouldReturnFalseButNotThrowForInvalidInput(string input)
        {
            Assert.False(LuhnUtility.Validate(input));
        }

        [Theory]
        [InlineData("0000-0000-0000-0000")]
        [InlineData("12345678903")]
        [InlineData("00041202")]
        [InlineData("123456789381908")]
        public void ValidationShouldReturnTrueForValidInput(string input)
        {
            Assert.True(LuhnUtility.Validate(input));
        }
    }
    
}