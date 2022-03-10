using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER
{
    public static class Constants
    {
        /// <summary>
        /// Used in Emails
        /// </summary>
        public const string APPLICATION_NAME = "USER REGISTER";

        /// <summary>
        /// 50 MB
        /// </summary>
        public const long MaxUploadSize = 52428800;

        public const string SuperUserName = "admin";
        public const string SuperRoleName = "administrator";
        public const int DEFAULT_PAGE_SIZE = 15;

        /// <summary>
        /// Use this to annotate decimal data type properties.
        /// </summary>
        public const string DecimalDatabaseType = "decimal(18,6)";
    }
}
