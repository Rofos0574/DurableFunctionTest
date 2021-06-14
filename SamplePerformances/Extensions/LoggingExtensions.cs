using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamplePerformances.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogDebugJson(this ILogger logger, string message, params object[] args)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                string[] values = args.Select(ToJson).ToArray();
                logger.LogDebug(message, values);
            }
        }

        public static void LogInformationJson(this ILogger logger, string message, params object[] args)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                string[] values = args.Select(ToJson).ToArray();
                logger.LogInformation(message, values);
            }
        }

        public static void LogWarningJson(this ILogger logger, string message, params object[] args)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                string[] values = args.Select(ToJson).ToArray();
                logger.LogWarning(message, values);
            }
        }

        public static void LogErrorJson(this ILogger logger, string message, params object[] args)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                string[] values = args.Select(ToJson).ToArray();
                logger.LogError(message, values);
            }
        }

        public static void LogErrorJson(this ILogger logger, Exception ex, string message, params object[] args)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                string[] values = args.Select(ToJson).ToArray();
                logger.LogError(ex, message, values);
            }
        }

        private static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private static string ToJson(object obj)
        {
            if (obj == null)
                return null;
            if (obj.GetType().IsValueType)
                return obj.ToString();
            if (obj is EntityId)
                return obj.ToString();
            if (obj is FileStreamResult)
                return JsonConvert.SerializeObject(new { ContentType = (obj as FileStreamResult).ContentType, Stream = "STREAM" }, JsonSerializerSettings);
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }
    }
}
