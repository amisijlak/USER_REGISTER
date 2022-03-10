using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Helpers
{
    public interface IUSER_REGISTERLogger
    {
        void LogMessage(string Source, string Message, bool BreakLineAfter);
        void LogException(string Source, Exception e, string optionalTitle = null);
        bool DeleteLogs(DateTime DeleteBeforeDate, out string ErrorMessage);
        void DeleteLogs(DateTime DeleteBeforeDate);
        string GetLogContents(DateTime Date);
        IEnumerable<FileInfo> GetLogFiles();
        void MaintainLogFilesFolder();
        string GetLogFolder();
    }
}
