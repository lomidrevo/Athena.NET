using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Athena.Core.Tools
{
	/// <summary>
	/// 
	/// </summary>
	public enum LogLevel
	{
		Anything,

		Debug,
		Info,
		Warning,
		Error,

		Nothing
	}

	/// <summary>
	/// 
	/// </summary>
	public class LogEntry
	{
		public DateTime TimeStamp { get; private set; }
		public LogLevel Level { get; private set; }
		public string Message { get; private set; }

		public LogEntry(LogLevel level, string message)
		{
			TimeStamp = DateTime.Now;
			Level = level;
			Message = message;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class Log
	{
		#region singleton

		/// <summary>
		/// instance of type T
		/// </summary>
		private static volatile Log instance;

		/// <summary>
		/// sync object for multithread access
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// singleton instance property
		/// </summary>
		public static Log Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new Log();
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private Log()
		{
			Filename = Path.Combine(Config.DefaultLogDirectory, DateTime.Now.ToString("yyyyddMM.HHmmss") + ".athena.log");
			logHistory = new ConcurrentBag<LogEntry>();

			// thread body for background saving
			backgroundSavingThread = new Thread(new ThreadStart(SaveInBackground));
			backgroundSavingThread.Start();
		}

		#endregion

		#region properties

		/// <summary>
		/// min message log level
		/// </summary>
		public LogLevel MinLogLevel { get; set; }

		public delegate void LogHandler(string logLine);
		public event LogHandler LogMessage;

		/// <summary>
		/// log history as array of lines
		/// </summary>
		public string[] History
		{ 
			get 
			{
				return (from h in logHistory orderby h.TimeStamp select h.Message).ToArray();
			} 
		}

		/// <summary>
		/// output log filename
		/// </summary>
		public string Filename { get; set; }
		
		#endregion

		#region private variables

		private ConcurrentBag<LogEntry> logHistory = null;
		private Thread backgroundSavingThread = null;
		private bool endBackgroundSaving = false;
		private ManualResetEvent endOfBackgroundSavingThreadEvent = null;
		private bool changed = false;

		#endregion

		/// <summary>
		/// add new message to log
		/// </summary>
		/// <param name="logLevel"></param>
		/// <param name="message"></param>
		public void AddMsg(LogLevel logLevel, string message)
		{
			lock (logHistory)
			{
				if (logLevel < MinLogLevel)
					return;

				var logLine = DateTime.Now.ToString("HH:mm:ss.fff") + " [" + logLevel.ToString().ToUpper() + "]: " + message;

				logHistory.Add(new LogEntry(logLevel, logLine));
				LogMessage(logLine);

				changed = true;
			}
		}

		/// <summary>
		/// add exception to log (with stacktrace)
		/// </summary>
		/// <param name="ex"></param>
		public void AddException(Exception ex)
		{
			lock (logHistory)
			{
				logHistory.Add(new LogEntry(LogLevel.Error, string.Empty));
				LogMessage("");

				var logLine = DateTime.Now.ToString("HH:mm:ss.fff") + " [" + LogLevel.Error.ToString().ToUpper() + "]:";

				logHistory.Add(new LogEntry(LogLevel.Error, logLine));
				LogMessage(logLine);

				var e = ex;
				while (e != null)
				{
					logLine = "[" + e.GetType().FullName + "] " + e.Message;
					logHistory.Add(new LogEntry(LogLevel.Error, logLine));
					LogMessage(logLine);

					e = e.InnerException;
				}

				logLine = "StackTrace:";
				logHistory.Add(new LogEntry(LogLevel.Error, logLine));
				LogMessage(logLine);

				var stackTraceLines = ex.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var line in stackTraceLines)
				{
					logHistory.Add(new LogEntry(LogLevel.Error, line));
					LogMessage(line);
				}

				logHistory.Add(new LogEntry(LogLevel.Error, string.Empty));
				LogMessage("");

				changed = true;
			}
		}

		/// <summary>
		/// flushes log to file specified by Filename property
		/// </summary>
		public void DumpToFile()
		{
			lock (logHistory)
			{
				if (!changed)
					return;

				File.WriteAllLines(Filename, History);
				changed = false;			
			}
		}

		#region private methods

		/// <summary>
		/// 
		/// </summary>
		private void SaveInBackground()
		{
			var cpuCounter = new PerformanceCounter() { CategoryName = "Processor", CounterName = "% Processor Time", InstanceName = "_Total" };
			endOfBackgroundSavingThreadEvent = new ManualResetEvent(false);

			var errorCount = 0;
			while (!endBackgroundSaving)
			{
				try
				{
					if (cpuCounter.NextValue() > 70f)
						continue;

					DumpToFile();
				}
				catch (Exception)
				{
					errorCount++;
					if (errorCount == 5)
						endBackgroundSaving = true;
				}

				Thread.Sleep(5 * 1000);
			}

			endOfBackgroundSavingThreadEvent.Set();
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if (!endBackgroundSaving && backgroundSavingThread != null)
			{
				endBackgroundSaving = true;
				if (!endOfBackgroundSavingThreadEvent.WaitOne(5 * 1000))
					backgroundSavingThread.Abort();

				backgroundSavingThread = null;
			}
		}
	}
}
