using System;

namespace Scalider.DotLiquid.Filters
{
    
    internal static class MathFilters
    {

        public static object Abs(object input)
        {
            switch (input)
            {
                case int intValue: return Math.Abs(intValue);
                case decimal decimalValue: return Math.Abs(decimalValue);
                case double doubleValue: return Math.Abs(doubleValue);
                case float floatValue: return Math.Abs(floatValue);
                case string strValue:
                    if (decimal.TryParse(strValue, out var value))
                    {
                        // We are only going to get the absolute value of something that could be
                        // parsed to decimal
                        return Math.Abs(value);
                    }

                    break;
            }

            //
            return input;
        }

        public static object Floor(object input)
        {
            switch (input)
            {
                case int intValue: return (int)Math.Floor((decimal)intValue);
                case decimal decimalValue: return Math.Floor(decimalValue);
                case double doubleValue: return Math.Floor(doubleValue);
                case float floatValue: return (float)Math.Floor(floatValue);
                case string strValue:
                    if (decimal.TryParse(strValue, out var value))
                    {
                        // We are only going to get the floor value of something that could be
                        // parsed to decimal
                        return Math.Floor(value);
                    }

                    break;
            }

            //
            return input;
        }

        public static object Ceil(object input)
        {
            switch (input)
            {
                case int intValue: return (int)Math.Ceiling((decimal)intValue);
                case decimal decimalValue: return Math.Floor(decimalValue);
                case double doubleValue: return Math.Floor(doubleValue);
                case float floatValue: return (float)Math.Ceiling(floatValue);
                case string strValue:
                    if (decimal.TryParse(strValue, out var value))
                    {
                        // We are only going to get the ceiling value of something that could be
                        // parsed to decimal
                        return Math.Ceiling(value);
                    }

                    break;
            }

            //
            return input;
        }

    }
    
}