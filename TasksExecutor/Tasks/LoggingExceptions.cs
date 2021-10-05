using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TasksExecutor.Tasks
{
    static class LoggingExceptions
    {
        public static void WriteLogToFile(Exception ex)
        {
            string currentDir = Directory.GetCurrentDirectory();
            Directory.CreateDirectory($"{currentDir}\\Log");
            string logFile = $"{currentDir}\\Log\\Bookings.txt";

            string text = CreateLogText(ex);

            using (StreamWriter writer = new(logFile, true))
            {
                writer.WriteLine(text);
            }
        }
        public static void WriteLogToFile(Exception ex, string taskLink)
        {
            string currentDir = Directory.GetCurrentDirectory();
            Directory.CreateDirectory($"{currentDir}\\Log");
            string logFile = $"{currentDir}\\Log\\Bookings.txt";

            string text = CreateLogText(ex, taskLink);

            using (StreamWriter writer = new(logFile, true))
            {
                writer.WriteLine(text);
            }
        }

        static string CreateLogText(Exception ex)
        {
            StringBuilder text = new();

            text.AppendLine("------------------------");
            text.AppendLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            text.AppendLine($"Message: {ex.Message}");
            text.AppendLine($"Source: {ex.Source}");
            text.AppendLine($"StackTrace: {ex.StackTrace}");
            text.AppendLine($"Method caused exception: {ex.TargetSite}");

            return text.ToString();
        }
        static string CreateLogText(Exception ex, string taskLink)
        {
            StringBuilder text = new();

            text.AppendLine("------------------------");
            text.AppendLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            text.AppendLine($"\nBookingUrl: {taskLink}");
            text.AppendLine($"Message: {ex.Message}");

            return text.ToString();
        }
    }
}
