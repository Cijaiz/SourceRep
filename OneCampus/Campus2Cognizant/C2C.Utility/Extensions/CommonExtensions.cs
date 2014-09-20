using System;
using System.Text;

namespace C2C.Core.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Extension Method For Exception , that returns a formatted string containing inner exceptions caused it.
        /// </summary>
        /// <param name="ex">Exception Object</param>
        /// <returns>String Containing inner exception details</returns>
        public static string ToFormatedString(this Exception ex)
        {
            StringBuilder message = new StringBuilder();
            //assign the current exception as first object and then loop through its
            //inner exceptions till they are null
            for (Exception eCurrent = ex; eCurrent != null; eCurrent = eCurrent.InnerException)
            {
                message.Append(string.Format("Exception :{0},StackTrace:{1}\n", eCurrent.Message, eCurrent.StackTrace));
            }

            return message.ToString();
        }

    }
}
