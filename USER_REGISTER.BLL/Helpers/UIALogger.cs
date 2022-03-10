using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Helpers
{
    public class USER_REGISTERLogger : IUSER_REGISTERLogger
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private static USER_REGISTERLogger _logger;
        public static USER_REGISTERLogger Current => _logger;

        public USER_REGISTERLogger(IHostingEnvironment _hostingEnvironment)
        {
            this._hostingEnvironment = _hostingEnvironment;
            _logger = this;
        }

        public IEnumerable<FileInfo> GetLogFiles()
        {
            var directoryInfo = new DirectoryInfo(GetLogFolder());

            return directoryInfo.GetFiles().Where(r => !r.Name.StartsWith("useless"));
        }

        public bool DeleteLogs(DateTime DeleteBeforeDate, out string ErrorMessage)
        {
            ErrorMessage = null;

            try
            {
                foreach (var entry in GetLogFiles().Where(r => r.CreationTime < DeleteBeforeDate).OrderBy(r => r.CreationTime).ToList())
                {
                    entry.Delete();
                }

                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
                LogException("KilimoTrustLogger:DeleteLogs", e);
            }

            return false;
        }

        public void DeleteLogs(DateTime DeleteBeforeDate)
        {
            string ErrorMessage;

            DeleteLogs(DeleteBeforeDate, out ErrorMessage);
        }

        public string GetLogContents(DateTime Date)
        {
            try
            {
                var fileName = _GetFileName(Date);

                using (var reader = new StreamReader(fileName))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public void LogException(string Source, Exception e, string optionalTitle = null)
        {
            _LogMessage(Source, (string.IsNullOrEmpty(optionalTitle) ? "" : $"{optionalTitle}|") + e.ExtractInnerExceptionMessage(false) + "\n" + e.StackTrace, true);
        }

        public void LogMessage(string Source, string Message, bool BreakLineAfter)
        {
            _LogMessage(Source, Message, BreakLineAfter);
        }

        public string GetLogFolder()
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, "Logs");
        }

        private string _GetFileName(DateTime Date)
        {
            var folder = GetLogFolder();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, Date.FormatAsParameter() + ".txt");
        }

        private void _LogMessage(string Source, string Message, bool BreakLineAfter)
        {
            try
            {
                var fileName = _GetFileName(DateTime.Today);

                using (var writer = new StreamWriter(fileName, true))
                {
                    writer.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}|{Source}|{Message}");
                    if (BreakLineAfter) writer.WriteLine("");

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
            }
        }

        public void MaintainLogFilesFolder()
        {
            DeleteLogs(DateTime.Today.AddMonths(-1));
        }
    }
}
