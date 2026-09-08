using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Common.Shared
{
    public static class FileLogger
    {
        private static readonly object _lock = new();

        public static void Log(Exception ex)
        {
            try
            {
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var file = Path.Combine(
                    folder,
                    $"log_{DateTime.Now:yyyyMMdd}.txt");

                var content = $@"
                    ==========================================
                    Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

                    Message:
                    {ex.Message}

                    StackTrace:
                    {ex.StackTrace}

                    InnerException:
                    {ex.InnerException}

                    Full Exception:
                    {ex}
                    ";

                lock (_lock)
                {
                    File.AppendAllText(file, content);
                }
            }
            catch
            {
                // Không throw để tránh vòng lặp vô hạn
            }
        }

        public static void Log(string message)
        {
            try
            {
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var file = Path.Combine(folder, $"log_{DateTime.Now:yyyyMMdd}.txt");
                var content = $@"
                    ==========================================
                    Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

                    Message:
                    {message}
                    ";

                lock (_lock)
                {
                    File.AppendAllText(file, content);
                }
            }
            catch
            {
                // Không throw để tránh vòng lặp vô hạn
            }
        }
    }
}
