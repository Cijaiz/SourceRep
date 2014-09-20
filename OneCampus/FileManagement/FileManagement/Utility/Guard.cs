using System;
using System.Globalization;

namespace FileManagement.Utility
{
    public static class Guard
    {
        /// <summary>
        /// Checks Null on Objects
        /// </summary>
        /// <param name="target">Parameter to Check</param>
        /// <param name="parameterName">Parameter Name to check</param>
        public static void IsNotNull(object target, string parameterName)
        {
            if (target == null)
            {
                string errorMessage = string.Format(CultureInfo.CurrentUICulture, "\"{0}\" cannot be null.", parameterName);

                throw new ArgumentNullException(errorMessage);
            }
        }

        /// <summary>
        /// Checks for Null/Whitespace on String type
        /// </summary>
        /// <param name="target">Parameter to Check</param>
        /// <param name="parameterName">Parameter Name to check</param>
        public static void IsNotBlank(string target, string parameterName)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrWhiteSpace(target))
            {
                string errorMessage = string.Format(CultureInfo.CurrentUICulture, "\"{0}\" cannot be blank.", parameterName);

                throw new ArgumentException(errorMessage);
            }
        }
    }
}
