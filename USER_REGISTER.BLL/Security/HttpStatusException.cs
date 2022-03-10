using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Security
{
    /// <summary>
    /// Thrown when <see cref="HttpService"/> throws an unsuccessful status code
    /// </summary>
    public class HttpStatusException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string Content { get; private set; }

        /// <summary>
        /// Thrown when <see cref="HttpService"/> throws an unsuccessful status code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="content"></param>
        public HttpStatusException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}
