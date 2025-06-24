using System;
using System.Threading.Tasks;

public static class LoggingSetup
{
    public static void RegisterGlobalExceptionHandlers(ILogger logger)
    {
        if (logger == null) throw new ArgumentNullException(nameof(logger));

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                logger.LogError("Unhandled exception", ex);
            else
                logger.LogError($"Unhandled exception: {e.ExceptionObject}");
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            logger.LogError("Unobserved task exception", e.Exception);
            e.SetObserved();
        };
    }
} 