using System;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.Localization.Infrastructure
{
    /// <summary>
    /// Class Logger
    /// </summary>
    internal static class Logger
    {
        private const string DefaultLoggingPrefix = "[Localization]: {0}";

        public static void Info(string message, object owner)
        {
            Log.Info(string.Format(DefaultLoggingPrefix, message), owner);
        }

        public static void Warn(string message, object owner)
        {
            Log.Warn(string.Format(DefaultLoggingPrefix, message), owner);
        }

        public static void Warn(string message, Exception ex, object owner)
        {
            Log.Warn(string.Format(DefaultLoggingPrefix, message), ex, owner);
        }

        public static void Error(string message, Type owner)
        {
            Log.Error(string.Format(DefaultLoggingPrefix, message), owner);
        }

        public static void Error(string message, Exception ex, Type owner)
        {
            Log.Error(string.Format(DefaultLoggingPrefix, message), ex, owner);
        }

        public static void Error(string message, Exception ex, object owner)
        {
            Log.Error(string.Format(DefaultLoggingPrefix, message), ex, owner);
        }

        public static void Error(string message, object owner)
        {
            Log.Error(string.Format(DefaultLoggingPrefix, message), owner);
        }

        public static void ExtraInfo(string message, object owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Info(string.Format(DefaultLoggingPrefix, message), owner);
            }
        }

        public static void ExtraWarn(string message, object owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Warn(string.Format(DefaultLoggingPrefix, message), owner);
            }
        }

        public static void ExtraWarn(string message, Exception ex, object owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Warn(string.Format(DefaultLoggingPrefix, message), ex, owner);
            }
        }

        public static void ExtraError(string message, Type owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Error(string.Format(DefaultLoggingPrefix, message), owner);
            }
        }

        public static void ExtraError(string message, Exception ex, Type owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Error(string.Format(DefaultLoggingPrefix, message), ex, owner);
            }
        }

        public static void ExtraError(string message, Exception ex, object owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Error(string.Format(DefaultLoggingPrefix, message), ex, owner);
            }
        }

        public static void ExtraError(string message, object owner)
        {
            if (ModuleSettings.LocalizationEnableExtraLogging)
            {
                Log.Error(string.Format(DefaultLoggingPrefix, message), owner);
            }
        }
    }
}
