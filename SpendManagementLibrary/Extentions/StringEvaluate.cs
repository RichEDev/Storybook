namespace SpendManagementLibrary.Extentions
{
    using System;

    /// <summary>
    /// Evaluate a numeric function contained in a string..
    /// </summary>
    public static class StringEvaluate
    {
        /// <summary>
        /// JavaScript Eval Calculations for simple calculations
        /// </summary>
        /// <param name="numericValue">The string to evaluate for numerics</param>
        /// <returns>The Double value of the string or zero.</returns>
        public static double Calculate(this string numericValue)
        {
            numericValue = numericValue.Replace(".", ",");
            if (HasBrackets(numericValue, out var calculate))
            {
                return calculate;
            }

            double result = 0;
            string[] plus;
            if (HasPlus(numericValue, out plus, out var additionResult))
            {
                return additionResult;
            }

            string[] minus;
           if (HasMinus(plus, out minus, out var subtractionResult))
            {
                return subtractionResult;
            }

            string[] mult;
            if (HasMultiply(minus, out mult, out var multiplicationResult)) 
            {
                return multiplicationResult;
            }

            double divisionResult;
            if (HasDivide(mult, out divisionResult))
            {
                return divisionResult;
            }

            double modulusResult;
            if (HasModulus(mult, out modulusResult))
            {
                return modulusResult;
            }

            return double.Parse(numericValue);
        }

        /// <summary>
        /// Has the given calcuation string got a modulus sign.
        /// </summary>
        /// <param name="calculation">
        /// The calculation to be evaluated.
        /// </param>
        /// <param name="modulusResult">
        /// The modulus result.
        /// </param>
        /// <returns>
        /// True if the <para>calculation</para> contains a modulus <see cref="bool"/>.
        /// </returns>
        private static bool HasModulus(string[] calculation, out double modulusResult)
        {
            double result;
            string[] mod = calculation[0].Split('%');
            if (mod.Length > 1)
            {
                // there were some %
                result = Calculate(mod[0]);
                for (int i = 1; i < mod.Length; i++)
                {
                    result %= Calculate(mod[i]);
                }

                {
                    modulusResult = result;
                    return true;
                }
            }

            modulusResult = 0;
            return false;
        }

        /// <summary>
        /// Has the given calcuation string got a division sign.
        /// </summary>
        /// <param name="calculation">
        /// The calculation to be evaluated.
        /// </param>
        /// <param name="divisionResult">
        /// The division result.
        /// </param>
        /// <returns>
        /// True if the <para>calculation</para> contains a division <see cref="bool"/>.
        /// </returns>
        private static bool HasDivide(string[] calculation, out double divisionResult)
        {
            double result;
            string[] div = calculation[0].Split('/');
            if (div.Length > 1)
            {
                // there were some /
                result = Calculate(div[0]);
                for (int i = 1; i < div.Length; i++)
                {
                    result /= Calculate(div[i]);
                }

                {
                    divisionResult = result;
                    return true;
                }
            }

            divisionResult = 0;
            return false;
        }

        /// <summary>
        /// Has the given calcuation string got a multiplication sign.
        /// </summary>
        /// <param name="calculation">
        /// The calculation to be evaluated.
        /// </param>
        /// <param name="mult">
        /// The mult.
        /// </param>
        /// <param name="multiplicationResult">
        /// The division result.
        /// </param>
        /// <returns>
        /// True if the 
        /// <para>
        /// calculation
        /// </para>
        /// contains a multiplication <see cref="bool"/>.
        /// </returns>
        private static bool HasMultiply(string[] calculation, out string[] mult, out double multiplicationResult)
        {
            double result;
            mult = calculation[0].Split('*');
            if (mult.Length > 1)
            {
                // there were some *
                result = Calculate(mult[0]);
                for (int i = 1; i < mult.Length; i++)
                {
                    result *= Calculate(mult[i]);
                }

                {
                    multiplicationResult = result;
                    return true;
                }
            }

            multiplicationResult = 0;
            return false;
        }

        /// <summary>
        /// Has the given calcuation string got a minus sign.
        /// </summary>
        /// <param name="calculation">
        /// The calculation to be evaluated.
        /// </param>
        /// <param name="minus">
        /// a string array split by a "-".
        /// </param>
        /// <param name="subtractionResult">
        /// The division result.
        /// </param>
        /// <returns>
        /// True if the 
        /// <para>
        /// calculation
        /// </para>
        /// contains a minus <see cref="bool"/>.
        /// </returns>
        private static bool HasMinus(string[] calculation, out string[] minus, out double subtractionResult)
        {
            double result;
            minus = calculation[0].Split('-');
            if (minus.Length > 1)
            {
                // there were some -
                result = Calculate(minus[0]);
                for (int i = 1; i < minus.Length; i++)
                {
                    result -= Calculate(minus[i]);
                }

                {
                    subtractionResult = result;
                    return true;
                }
            }

            subtractionResult = 0;
            return false;
        }

        /// <summary>
        /// Has the given calcuation string got a plus sign.
        /// </summary>
        /// <param name="calculation">
        /// The calculation to be evaluated.
        /// </param>
        /// <param name="plus">
        /// a string array split by a "+".
        /// </param>
        /// <param name="additionResult">
        /// The division result.
        /// </param>
        /// <returns>
        /// True if the 
        /// <para>
        /// calculation
        /// </para>
        /// contains a plus <see cref="bool"/>.
        /// </returns>
        private static bool HasPlus(string calculation, out string[] plus, out double additionResult)
        {
            double result;
            plus = calculation.Split('+');
            if (plus.Length > 1)
            {
                // there were some +
                result = Calculate(plus[0]);
                for (int i = 1; i < plus.Length; i++)
                {
                    result += Calculate(plus[i]);
                }

                {
                    additionResult = result;
                    return true;
                }
            }

            additionResult = 0;
            return false;
        }

        /// <summary>
        /// Has the given calcuation string got a plus sign.
        /// </summary>
        /// <param name="calculation">
        /// The calculation to be evaluated.
        /// </param>
        /// <param name="bracketedCalculationResult">
        /// The division result.
        /// </param>
        /// <returns>
        /// True if the 
        /// <para>
        /// calculation
        /// </para>
        /// contains a bracket <see cref="bool"/>.
        /// </returns>
        private static bool HasBrackets(string calculation, out double bracketedCalculationResult)
        {
            if (calculation.IndexOf("(", StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                int startIndex = calculation.LastIndexOf("(", StringComparison.CurrentCultureIgnoreCase);
                int endIndex = calculation.IndexOf(")", startIndex, StringComparison.CurrentCultureIgnoreCase);
                double middle = Calculate(calculation.Substring(startIndex + 1, endIndex - startIndex - 1));
                {
                    bracketedCalculationResult = Calculate(
                        $"{calculation.Substring(0, startIndex)}{middle}{calculation.Substring(endIndex + 1)}");
                    return true;
                }
            }

            bracketedCalculationResult = 0;
            return false;
        }
    }
}
