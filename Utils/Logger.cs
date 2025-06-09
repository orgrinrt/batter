using System.Collections.Concurrent;
using System.Text;

namespace SafeWarLogPatch.Utils;

// TODO: implement ILogger and IDisposable for better integration with .NET logging systems etc

public static class BatterLog
{
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Mount and Blade II Bannerlord", "Logs", "SafeWarLog.txt");

    private static readonly ConcurrentQueue<string> _logQueue = new();
    private static readonly StreamWriter? _writer;
    private static readonly Thread? _flushThread;
    private static bool _running;
    private static string? _lastMessage;
    private static int _repeatCount;
    private static readonly AutoResetEvent _logSignal = new(false);
    private static readonly object _lock = new();

    static BatterLog()
    {
        try
        {
            var dir = Path.GetDirectoryName(LogPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Always start a brand-new file each session:
            if (File.Exists(LogPath))
                File.Delete(LogPath);

            _writer = new StreamWriter(LogPath, false, Encoding.UTF8)
            {
                AutoFlush = false
            };

            _running = true;
            _flushThread = new Thread(FlushLoop)
            {
                IsBackground = true,
                Name = "SafeLogFlushThread"
            };
            _flushThread.Start();

            Info($"=== Mod session started: {DateTime.Now} ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SafeLog Init ERROR] {ex}");
        }
    }

    public static void Info(string message)
    {
        EnqueueMessage("INFO", message);
    }

    public static void Warning(string message)
    {
        EnqueueMessage("WARNING", message);
    }

    public static void Warn(string message)
    {
        Warning(message);
    }

    public static void Hr()
    {
        Info(new string('=', 50)); // Default length of separator
    }

    public static void Hr(string message)
    {
        const int totalWidth = 50;
        if (string.IsNullOrWhiteSpace(message))
        {
            Hr(); // fallback to plain
            return;
        }

        message = message.Trim();
        var messageLength = message.Length + 2; // add space for surrounding spaces
        var padding = (totalWidth - messageLength) / 2;

        if (padding < 0)
        {
            Info($"= {message} ="); // fallback: no centering if message is too long
            return;
        }

        var line = new string('=', padding) + " " + message + " " +
                   new string('=', totalWidth - messageLength - padding);
        Info(line);
    }


    public static void Error(string message)
    {
        EnqueueMessage("ERROR", message);
    }

    private static void EnqueueMessage(string level, string message)
    {
        lock (_lock)
        {
            var formattedMessage = $"[{level}] {message}";

            if (formattedMessage == _lastMessage)
            {
                _repeatCount++;
                return;
            }

            if (_repeatCount > 0 && _lastMessage != null)
            {
                _logQueue.Enqueue($"[Repeated {_repeatCount + 1} times] {_lastMessage}");
                _repeatCount = 0;
            }

            _lastMessage = formattedMessage;
            _logQueue.Enqueue($"[{DateTime.Now:HH:mm:ss.fff}] {formattedMessage}");
        }

        _logSignal.Set();
    }

    private static void FlushLoop()
    {
        try
        {
            while (_running || !_logQueue.IsEmpty)
            {
                _logSignal.WaitOne(100); // wait up to 100ms

                while (_logQueue.TryDequeue(out var line))
                    try
                    {
                        _writer?.WriteLine(line);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"[SafeLog WRITE ERROR] {ex}");
                    }

                _writer?.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SafeLog FlushLoop ERROR] {ex}");
        }
    }

    public static void Shutdown()
    {
        lock (_lock)
        {
            if (_repeatCount > 0 && _lastMessage != null)
                _logQueue.Enqueue($"[Repeated {_repeatCount + 1} times] {_lastMessage}");
            _repeatCount = 0;
            _lastMessage = null;
        }

        _running = false;
        _logSignal.Set();

        try
        {
            _flushThread?.Join(3000); // wait max 3 sec
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SafeLog Shutdown ERROR] {ex}");
        }

        try
        {
            while (_logQueue.TryDequeue(out var line))
                _writer?.WriteLine(line);

            _writer?.Flush();

            _writer?.Close();
            _writer?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SafeLog Final Flush ERROR] {ex}");
        }
    }
}