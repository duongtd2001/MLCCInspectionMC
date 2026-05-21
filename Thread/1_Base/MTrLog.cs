using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MLCCInspectionMC
{
    public enum LogIndex { 
        eErrorLog,
        eDeviceLog,
        eDataSave,
        eDGSLogStart,
        eDGSLogEnd,
        eLogGEIM,
        eLogGEIM_D1,
        eLogBCRSave,
        eLogGMESSave,
        eMax
    };
    public class StructLog
    {
        public LogIndex Type;
        public string Message = string.Empty;
        public StructLog(LogIndex type, string Message)
        {
            this.Type = type;
            this.Message = Message;
        }
    }
    public class MTrLog : MSystem, IDisposable
    {


        private readonly BlockingCollection<StructLog> logQueue = new BlockingCollection<StructLog>();
        private readonly object LockEDMSave = new object();

        private int m_lastLoggedHourForNgTray = -1; // log 9010
        private Timer m_periodicTimer;

        private string _currentDate = DateTime.Now.ToString("yyyyMMdd");
        private readonly Dictionary<LogIndex, (string Path, StreamWriter Writer)> _logWriters = new Dictionary<LogIndex, (string, StreamWriter)>();

        #region //Initial Class 
        public MTrLog() {

            lock (MSystem.LockLog)
            {
                lock (LockLog)
                {
                    InitializeDirectories();
                    InitializeLogWriters();
                }
               
            }
        }
        private void InitializeDirectories()
        {
            if (!Directory.Exists(Config.LogDevice)) Directory.CreateDirectory(Config.LogDevice);
            if (!Directory.Exists(Config.LogError)) Directory.CreateDirectory(Config.LogError);
            if (!Directory.Exists(Config.LogDataSave)) Directory.CreateDirectory(Config.LogDataSave);
            if (!Directory.Exists(Config.LogBCRSave)) Directory.CreateDirectory(Config.LogBCRSave);
            if (!Directory.Exists(Config.LogGMESSave)) Directory.CreateDirectory(Config.LogGMESSave);
            if (!Directory.Exists(Config.PathLogGEIM)) Directory.CreateDirectory(Config.PathLogGEIM);
        }
        private void InitializeLogWriters()
        {
            _currentDate = DateTime.Now.ToString("yyyyMMdd");
            _logWriters.Clear();
            CloseWriters();

            _logWriters[LogIndex.eDeviceLog] = (Path.Combine(Config.LogDevice, _currentDate + "_Devlog.txt"), CreateWriter(Path.Combine(Config.LogDevice, _currentDate + "_Devlog.txt")));
            _logWriters[LogIndex.eErrorLog] = (Path.Combine(Config.LogError, _currentDate + "_Error.txt"), CreateWriter(Path.Combine(Config.LogError, _currentDate + "_Error.txt")));
            _logWriters[LogIndex.eDataSave] = (Path.Combine(Config.LogDataSave, _currentDate + "_DataSave.txt"), CreateWriter(Path.Combine(Config.LogDataSave, _currentDate + "_DataSave.txt")));
            _logWriters[LogIndex.eLogBCRSave] = (Path.Combine(Config.LogBCRSave, _currentDate + "_BCRLog.txt"), CreateWriter(Path.Combine(Config.LogBCRSave, _currentDate + "_BCRLog.txt")));
            _logWriters[LogIndex.eLogGMESSave] = (Path.Combine(Config.LogGMESSave, _currentDate + "_DataGMesRun.txt"), CreateWriter(Path.Combine(Config.LogGMESSave, _currentDate + "_DataGMesRun.txt")));
        }
        private StreamWriter CreateWriter(string filePath)
        {
            try
            {
                return new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to create writer for {filePath}: {ex.Message}");
                return null;
            }
        }
        private void CloseWriters()
        {
            foreach (var entry in _logWriters.Values)
            {
                entry.Writer?.Dispose();
            }
        }
        private void CheckAndUpdateWriters()
        {
            var newDate = DateTime.Now.ToString("yyyyMMdd");
            if (newDate != _currentDate)
            {
                InitializeLogWriters();
            }
        }
        #endregion
        public void LogprocessData()
        {
            try
            {
                foreach (var logEntry in logQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        CheckAndUpdateWriters();
                        MyMessagerBottom(logEntry.Message);
                        var timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {logEntry.Message}";
                        switch (logEntry.Type)
                        {
                            case LogIndex.eErrorLog:
                            {
                                var parts = logEntry.Message.Trim().Split(',');
                                if (parts.Length >= 2)
                                {
                                    string[] dbEntry = { parts[1], $"{DateTime.Now:yyyyMMdd_HH:mm:ss}", parts[0] };

                                    MSystem.m_pDatabaseLog.InsertData(dbEntry);

                                    AppendToFile(_logWriters[LogIndex.eErrorLog].Writer, _logWriters[LogIndex.eErrorLog].Path, $"{dbEntry[1]} {dbEntry[0]} - {dbEntry[2]}");
                                }
                            }
                            break;
                            case LogIndex.eDeviceLog:
                            {
                                AppendToFile(_logWriters[LogIndex.eDeviceLog].Writer, _logWriters[LogIndex.eDeviceLog].Path, timestampedMessage);
                            }
                            break;
                            case LogIndex.eDataSave:
                            AppendToFile(_logWriters[LogIndex.eDataSave].Writer, _logWriters[LogIndex.eDataSave].Path, timestampedMessage);
                            break;
                            case LogIndex.eDGSLogStart:
                            //m_pTrsLoader.AddStartDGSLog();
                            break;
                            case LogIndex.eDGSLogEnd:
                            //m_pTrsLoader.AddLogDGSResult();
                            break;
                            case LogIndex.eLogGEIM:
                            GEIMlogSave(logEntry.Message);
                            break;
                            case LogIndex.eLogGEIM_D1:
                            break;
                            case LogIndex.eLogBCRSave:
                            AppendToFile(_logWriters[LogIndex.eLogBCRSave].Writer, _logWriters[LogIndex.eLogBCRSave].Path, timestampedMessage);
                            break;
                            case LogIndex.eLogGMESSave:
                            AppendToFile(_logWriters[LogIndex.eLogGMESSave].Writer, _logWriters[LogIndex.eLogGMESSave].Path, timestampedMessage);
                            break;
                            default: break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Internal log error: {ex.Message}");
                    }
                }

            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log processing task error: {ex.Message}");
            }
        }
        public void AddTail(LogIndex _LogType, string _strlog)
        {
            if (logQueue.IsAddingCompleted) return;
            logQueue.Add(new StructLog(_LogType, _strlog));
        }
        /// <summary>
        /// EDM logging function
        /// </summary>
        /// <param name="eventCode">The event code (Required).</param>
        /// <param name="subCode">Used for the Sub-code of Event 9020 or the Operating Mode of 9000.</param>
        /// <param name="jigNo">Used for JIG ID (9003), Jig number (9020), WIP count (9200).</param>
        /// <param name="jigStatus">Used for Jig condition/result (9020), Alarm bit (9210).</param>
        /// <param name="spare">A versatile string parameter for custom fields (NG Name, NG Tray status, TOP Code, Model Name).</param>
       
        public void DevLogSave(string _strlog)
        {
            var timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {_strlog}";
            AppendToFile(_logWriters[LogIndex.eDeviceLog].Writer, _logWriters[LogIndex.eDeviceLog].Path, timestampedMessage);

            MSystem.MyMessagerBottom(_strlog);
        }

        public static string SanitizeForFileName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            return Regex.Replace(input, @"[\\/:*?\""<>|,]", " ");
        }
        public void GEIMlogSave(string Message)
        {
            lock (LockEDMSave)
            {
                try
                {
                    string fullLogFileName = System.IO.Path.Combine(Config.PathLogGEIM, Message + ".txt");

                    if (!Directory.Exists(Config.PathLogGEIM))
                    {
                        Directory.CreateDirectory(Config.PathLogGEIM);
                    }
                    File.WriteAllText(fullLogFileName, Message);
                }
                catch { }
            }
        }
        private void AppendToFile(StreamWriter writer, string filePath, string content)
        {
            try
            {
                if (writer == null)
                {
                    using (var fallbackWriter = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        fallbackWriter.WriteLine(content);
                        fallbackWriter.Flush();
                    }
                    return;
                }
                writer.WriteLine(content);
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error writing to {filePath}: {ex.Message}");
            }
        }
        public void Dispose()
        {
            logQueue.CompleteAdding();
            m_periodicTimer?.Dispose();
            CloseWriters();
        }
    }
}
