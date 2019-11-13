
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace WebAPIBearerTokenExample.Common
{
    public class CustomLogger
    {
        private static readonly object _syncObject = new object();
        private static string _baseDirectory;

        string _referenceId, _filePathForNormalLogs, _filePathForErrorLogs;
        StringBuilder _sbLog;
        private static IDictionary<string, CustomLogger> _listInstances;

        static CustomLogger()
        {            
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
            _listInstances = new Dictionary<string, CustomLogger>();
            DeleteFilesOlderMoreThanNdays(30);
        }

        public static CustomLogger GetLoggerInstance(string referenceId)
        {
            CustomLogger customLogger2;
            _listInstances.TryGetValue(referenceId, out customLogger2);
            return customLogger2;
        }

        public static long GetTotalInstanceCount{
            get{
                return _listInstances.Count;
            }
        }

        public static CustomLogger GetLoggerInstance(HttpRequestMessage request)
        {            
            string referenceId = string.Empty;
            IEnumerable<string> str;
            request.Headers.TryGetValues("GuidValue", out str);
            referenceId = str.FirstOrDefault();
            return GetLoggerInstance(referenceId);
        }

        public string GetReferenceId
        {
            get
            {
                return _referenceId;
            }
        }

        public string FilePathForNormalLogs
        {
            get
            {
                return _filePathForNormalLogs;
            }
        }

        public string FilePathForErrorLogs
        {
            get
            {
                return _filePathForErrorLogs;
            }
        }

        public static void DeleteFilesOlderMoreThanNdays(int noOfDays, string absoluteFolderPath = null)
        {
            var endDate = DateTime.Now.AddDays(0 - Math.Abs(noOfDays));
            var startDate = DateTime.Now.AddDays(-365); //1 year
            if (string.IsNullOrWhiteSpace(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;
            var files = ListallFiles(absoluteFolderPath, startDate, endDate);
            foreach (var file in files)
            {
                System.Diagnostics.Trace.WriteLine(file.Key);
                File.Delete(Path.Combine(absoluteFolderPath, file.Key));
            }

        }

        public CustomLogger(string referenceId = null)
        {
            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;
            _sbLog = _sbLog == null ? new StringBuilder() : _sbLog;
            _filePathForNormalLogs = string.IsNullOrWhiteSpace(_filePathForNormalLogs) ? Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "-logs.txt") : _filePathForNormalLogs;
            _filePathForErrorLogs = string.IsNullOrWhiteSpace(_filePathForErrorLogs) ? Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "-errors.txt") : _filePathForErrorLogs;
            if (_listInstances.ContainsKey(_referenceId) == false)
            {
                _listInstances.Add(_referenceId, this);
            }
        }

        public CustomLogger(HttpRequestMessage request)
        {
            _referenceId = string.IsNullOrWhiteSpace(_referenceId) ? Guid.NewGuid().ToString() : _referenceId;
            request.Headers.Add("GuidValue", _referenceId);
            _sbLog = _sbLog == null ? new StringBuilder() : _sbLog;
            _filePathForNormalLogs = string.IsNullOrWhiteSpace(_filePathForNormalLogs) ? Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "-logs.txt") : _filePathForNormalLogs;
            _filePathForErrorLogs = string.IsNullOrWhiteSpace(_filePathForErrorLogs) ? Path.Combine(_baseDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "-errors.txt") : _filePathForErrorLogs;            
                        
            if (_listInstances.ContainsKey(_referenceId) == false)
            {
                _listInstances.Add(_referenceId, this);
            }

        }

        public CustomLogger(string baseDirectoryWithAbsolutePath, string fileNameWithExtensionForLog, string FileNameWithExtensionForErrors, string referenceId = null)
        {
            if (fileNameWithExtensionForLog.Length > 0 && fileNameWithExtensionForLog.Contains('.'))
            {
                fileNameWithExtensionForLog = fileNameWithExtensionForLog.Trim();
            }

            if (FileNameWithExtensionForErrors.Length > 0 && FileNameWithExtensionForErrors.Contains('.'))
            {
                FileNameWithExtensionForErrors = FileNameWithExtensionForErrors.Trim();
            }

            baseDirectoryWithAbsolutePath = baseDirectoryWithAbsolutePath.Trim();
            try
            {
                if (!Directory.Exists(baseDirectoryWithAbsolutePath))
                {
                    Directory.CreateDirectory(baseDirectoryWithAbsolutePath);
                    _baseDirectory = baseDirectoryWithAbsolutePath;
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Unable to Create Directory with the following path - " + baseDirectoryWithAbsolutePath, Ex);
            }

            _referenceId = string.IsNullOrWhiteSpace(referenceId) ? Guid.NewGuid().ToString() : referenceId;
            _sbLog = new StringBuilder();
            _filePathForNormalLogs = Path.Combine(_baseDirectory, fileNameWithExtensionForLog);
            _filePathForErrorLogs = Path.Combine(_baseDirectory, FileNameWithExtensionForErrors);
        }

        public void AppendLog(string logText, object obj = null, Formatting formatting = Formatting.Indented)
        {
            if (_sbLog.Length == 0)
            {
                _sbLog.AppendLine(Environment.NewLine);
                _sbLog.AppendLine("--------------------------------------------------------------------------------");
                _sbLog.AppendLine($"{_referenceId} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff")}");
            }
            _sbLog.AppendLine($"\t{logText} ({DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.ffffff")})");
            if (obj != null)
            {
                if (obj.GetType().IsClass)
                {
                    _sbLog.AppendLine($"\t{JsonConvert.SerializeObject(obj, formatting)}");
                }
                else
                {
                    _sbLog.AppendLine($"\t{obj}");
                }
            }
        }

        public void CommitLog(string lastCommitMessage = null)
        {
            if (!string.IsNullOrWhiteSpace(lastCommitMessage))
            {
                _sbLog.AppendLine(lastCommitMessage);
            }
            onExit();
        }

        public void WriteException(Exception exception)
        {
            StringBuilder sb = new StringBuilder(Environment.NewLine + GetDateTimeAndReference(_referenceId) + Environment.NewLine);
            do
            {
                sb.AppendLine("\t" + exception.Source + " - " + exception.Message);
                sb.AppendLine("\t" + exception.StackTrace);
                exception = exception.InnerException;
            } while (exception != null);

            Log(sb.ToString(), _filePathForErrorLogs);
        }

        public static bool DeleteAllRootFilesAndFolders(string absoluteFolderPath = null)
        {
            if (string.IsNullOrEmpty(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;

            if (Directory.Exists(absoluteFolderPath))
            {
                DirectoryInfo di = new DirectoryInfo(absoluteFolderPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            return true;
        }

        public static Dictionary<string, DateTime> ListallFiles(string absoluteFolderPath = null, DateTime? StartingDate = null, DateTime? EndDate = null)
        {
            Dictionary<string, DateTime> _dict = new Dictionary<string, DateTime>();
            if (string.IsNullOrEmpty(absoluteFolderPath)) absoluteFolderPath = _baseDirectory;
            if (Directory.Exists(absoluteFolderPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(absoluteFolderPath);
                var listFiles = dirInfo.GetFiles("*.txt").ToList();
                Trace.WriteLine(JsonConvert.SerializeObject(listFiles));
                DateTime minDate = StartingDate.HasValue ? Convert.ToDateTime(StartingDate) : listFiles.Min(x => x.LastWriteTime);
                DateTime maxDate = EndDate.HasValue ? Convert.ToDateTime(EndDate) : listFiles.Max(x => x.LastWriteTime);
                var finalList = listFiles.Where(x => x.LastWriteTime >= minDate && x.LastWriteTime <= maxDate && x.Exists == true).OrderByDescending(x => x.LastWriteTime).ToList();
                foreach (var file in finalList)
                {
                    _dict.Add(file.Name, file.LastWriteTime);
                }
            }
            return _dict;
        }

        private static void Log(string logText, string absoluteFilePathWithExt)
        {
            lock (_syncObject)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(absoluteFilePathWithExt, true))
                    {
                        sw.WriteLine(logText);
                    }
                }
                catch (Exception Ex)
                {
                    Trace.TraceError(Ex.Message + Environment.NewLine + Ex.StackTrace);
                }
            }
        }
        private static string GetDateTimeAndReference(string referenceId)
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                + (string.IsNullOrEmpty(referenceId) ? "" : $"({referenceId})");
        }

        ~CustomLogger()
        {
            onExit();
        }


        private void onExit()
        {
            if (_sbLog.Length > 0)
            {
                _sbLog.AppendLine($"Commit performed for reference id {_referenceId}");
                _sbLog.AppendLine("--------------------------------------------------------------------------------");
                Log(_sbLog.ToString(), _filePathForNormalLogs);
            }

            if (_listInstances.ContainsKey(_referenceId) == true)
            {
                _listInstances.Remove(_referenceId);
            }
        }
    }
}