using System;
using System.Text.RegularExpressions;
using log4net;

namespace Core.Common.Extensions
{
    public static class Log4NetExtension
    {
        public static void LogErr(this ILog logger, string errorText, Guid correlationId)
        {
            string result = Regex.Replace(errorText, @"\r\n?|\n", " |||");
            logger.Error($"{correlationId.ToString().Replace("-","")} {result}");
        }

        public static void LogDeb(this ILog logger, string errorText, Guid correlationId)
        {
            string result = Regex.Replace(errorText, @"\r\n?|\n", " |||");
            logger.Debug($"{correlationId.ToString().Replace("-", "")} {result}");
        }

        public static void LogInf(this ILog logger, string errorText, Guid correlationId)
        {
            string result = Regex.Replace(errorText, @"\r\n?|\n", " |||");
            logger.Info($"{correlationId.ToString().Replace("-", "")} {result}");
        }
    }
}
