using System.Collections.Concurrent;
using System.Text;

namespace Batter.Core.Utils;

// TODO: implement ILogger and IDisposable for better integration with .NET logging systems etc

public static class BatterLog {
    private static readonly String LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Mount and Blade II Bannerlord", "Logs", "SafeWarLog.txt");

    private static readonly ConcurrentQueue<String> _logQueue = new();
    private static readonly StreamWriter? _writer;
    private static readonly Thread? _flushThread;
    private static Boolean _running;
    private static String? _lastMessage;
    private static Int32 _repeatCount;
    private static readonly AutoResetEvent _logSignal = new(false);
    private static readonly Object _lock = new();

    static BatterLog() {
        try {
            var dir = Path.GetDirectoryName(BatterLog.LogPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Always start a brand-new file each session:
            if (File.Exists(BatterLog.LogPath))
                File.Delete(BatterLog.LogPath);

            BatterLog._writer = new(BatterLog.LogPath, false, Encoding.UTF8) {
                AutoFlush = false,
            };

            BatterLog._running = true;
            BatterLog._flushThread = new(BatterLog.FlushLoop) {
                IsBackground = true,
                Name = "SafeLogFlushThread",
            };
            BatterLog._flushThread.Start();

            BatterLog.Info($"=== Mod session started: {DateTime.Now} ===");
        }
        catch (Exception ex) {
            Console.WriteLine($"[SafeLog Init ERROR] {ex}");
        }
    }

    public static void Info(String message) {
        BatterLog.EnqueueMessage("INFO", message);
    }

    public static void Warning(String message) {
        BatterLog.EnqueueMessage("WARNING", message);
    }

    public static void Warn(String message) {
        BatterLog.Warning(message);
    }

    public static void Hr() {
        BatterLog.Info(new('=', 50)); // Default length of separator
    }

    public static void Hr(String message) {
        const Int32 totalWidth = 50;
        if (String.IsNullOrWhiteSpace(message)) {
            BatterLog.Hr(); // fallback to plain
            return;
        }

        message = message.Trim();
        var messageLength = message.Length + 2; // add space for surrounding spaces
        var padding = (totalWidth - messageLength) / 2;

        if (padding < 0) {
            BatterLog.Info($"= {message} ="); // fallback: no centering if message is too long
            return;
        }

        var line = new String('=', padding) + " " + message + " " +
                   new String('=', totalWidth - messageLength - padding);
        BatterLog.Info(line);
    }


    public static void Error(String message) {
        BatterLog.EnqueueMessage("ERROR", message);
    }

    private static void EnqueueMessage(String level, String message) {
        lock (BatterLog._lock) {
            var formattedMessage = $"[{level}] {message}";

            if (formattedMessage == BatterLog._lastMessage) {
                BatterLog._repeatCount++;
                return;
            }

            if (BatterLog._repeatCount > 0 && BatterLog._lastMessage != null) {
                BatterLog._logQueue.Enqueue($"[Repeated {BatterLog._repeatCount + 1} times] {BatterLog._lastMessage}");
                BatterLog._repeatCount = 0;
            }

            BatterLog._lastMessage = formattedMessage;
            BatterLog._logQueue.Enqueue($"[{DateTime.Now:HH:mm:ss.fff}] {formattedMessage}");
        }

        BatterLog._logSignal.Set();
    }

    private static void FlushLoop() {
        try {
            while (BatterLog._running || !BatterLog._logQueue.IsEmpty) {
                BatterLog._logSignal.WaitOne(100); // wait up to 100ms

                while (BatterLog._logQueue.TryDequeue(out var line))
                    try {
                        BatterLog._writer?.WriteLine(line);
                    }
                    catch (IOException ex) {
                        Console.WriteLine($"[SafeLog WRITE ERROR] {ex}");
                    }

                BatterLog._writer?.Flush();
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"[SafeLog FlushLoop ERROR] {ex}");
        }
    }

    public static void Shutdown() {
        lock (BatterLog._lock) {
            if (BatterLog._repeatCount > 0 && BatterLog._lastMessage != null)
                BatterLog._logQueue.Enqueue($"[Repeated {BatterLog._repeatCount + 1} times] {BatterLog._lastMessage}");
            BatterLog._repeatCount = 0;
            BatterLog._lastMessage = null;
        }

        BatterLog._running = false;
        BatterLog._logSignal.Set();

        try {
            BatterLog._flushThread?.Join(3000); // wait max 3 sec
        }
        catch (Exception ex) {
            Console.WriteLine($"[SafeLog Shutdown ERROR] {ex}");
        }

        try {
            while (BatterLog._logQueue.TryDequeue(out var line))
                BatterLog._writer?.WriteLine(line);

            BatterLog._writer?.Flush();

            BatterLog._writer?.Close();
            BatterLog._writer?.Dispose();
        }
        catch (Exception ex) {
            Console.WriteLine($"[SafeLog Final Flush ERROR] {ex}");
        }
    }
}