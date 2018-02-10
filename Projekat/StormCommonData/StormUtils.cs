using System;
using System.Collections.Generic;
using System.Text;

namespace StormCommonData
{
    public static class StormUtils
    {
        public static string FlattenException(Exception ex)
        {
            var stringBuilder = new StringBuilder();

            while (ex != null)
            {
                stringBuilder.AppendLine(ex.Message);
                stringBuilder.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}
