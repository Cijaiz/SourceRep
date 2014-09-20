using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace C2C.Core.Helper
{
    public static class Guard
    {
        /// <summary>
        /// Checks Null on Objects
        /// </summary>
        /// <param name="target">Parameter to Check</param>
        /// <param name="parameterName">Parameter Name to Check</param>
        public static void IsNotNull(object target, string parameterName)
        {
            if (target == null)
            {
                string errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0} cannot be blank.", parameterName);
                throw new ArgumentNullException(errorMessage);
            }
        }

        /// <summary>
        /// Checks for Null/Whitespace on String variable
        /// </summary>
        /// <param name="target">Parameter to Check</param>
        /// <param name="parameterName">Parameter Name to Check</param>
        public static void IsNotBlank(string target, string parameterName)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrWhiteSpace(target))
            {
                string errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0} cannot be blank.", parameterName);
                throw new ArgumentNullException(errorMessage);
            }
        }

        public static void IsNotZero(int target, string parameterName)
        {
            if (target <= 0)
            {
                string errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0} is not valid.", parameterName);
                throw new ArgumentOutOfRangeException(errorMessage);
            }
        }

        public static void IsCountNotZero(List<object> target, string parameterName)
        {
            if (target == null || target.Count() <= 0)
            {
                string errorMessage = string.Format(CultureInfo.CurrentUICulture, "{0} doesn't has value.", parameterName);
                throw new ArgumentNullException(errorMessage);
            }
        }
        

    }
}
