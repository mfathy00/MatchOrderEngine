using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ILogger
{
    void LogInfo(string message);
    void LogError(string message, Exception? exception = null);
    T LogExecution<T>(string description, Func<T> func);
    Task<T> LogExecutionAsync<T>(string description, Func<Task<T>> func);
}

public record LogEntry(DateTime Timestamp, int ThreadId, string Level, string Message, Exception? Exception);

public abstract class LoggerBase : ILogger
{
    private readonly object _sync = new();

    public void LogInfo(string message) => Write("INFO", message, null);

    public void LogError(string message, Exception? exception = null) => Write("ERROR", message, exception);

    public T LogExecution<T>(string description, Func<T> func)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = func();
            sw.Stop();
            LogInfo($"{description} succeeded in {sw.ElapsedMilliseconds} ms");
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            LogError($"{description} failed after {sw.ElapsedMilliseconds} ms", ex);
            throw;
        }
    }

    public async Task<T> LogExecutionAsync<T>(string description, Func<Task<T>> func)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await func().ConfigureAwait(false);
            sw.Stop();
            LogInfo($"{description} succeeded in {sw.ElapsedMilliseconds} ms");
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            LogError($"{description} failed after {sw.ElapsedMilliseconds} ms", ex);
            throw;
        }
    }

    private void Write(string level, string message, Exception? exception)
    {
        var entry = new LogEntry(DateTime.UtcNow, Thread.CurrentThread.ManagedThreadId, level, message, exception);
        lock (_sync)
        {
            WriteEntry(entry);
        }
    }

    protected abstract void WriteEntry(LogEntry entry);

    protected static string FormatEntry(LogEntry entry)
    {
        var formatted = $"{entry.Timestamp:O} [T{entry.ThreadId}] {entry.Level}: {entry.Message}";
        if (entry.Exception != null)
            formatted += Environment.NewLine + entry.Exception;
        return formatted;
    }
}

public class CsvLogger : LoggerBase
{
    private readonly string _path;

    public CsvLogger(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        _path = path;
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        if (!File.Exists(_path))
        {
            File.WriteAllText(_path, "Timestamp,ThreadId,Level,Message,Exception" + Environment.NewLine);
        }
    }

    protected override void WriteEntry(LogEntry entry)
    {
        var messageEscaped = entry.Message.Replace("\"", "\"\"");
        var exceptionText = entry.Exception?.ToString().Replace("\"", "\"\"");
        var line = $"{entry.Timestamp:O},{entry.ThreadId},{entry.Level},\"{messageEscaped}\",\"{exceptionText}\"";
        File.AppendAllText(_path, line + Environment.NewLine);
    }
}


