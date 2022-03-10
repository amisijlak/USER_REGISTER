using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using USER_REGISTER.Helpers;

namespace USER_REGISTER
{
    /// <summary>
    /// Provide extended functionality for generating client responses.
    /// </summary>
    public static class ClientResponseHelper
    {
        /// <summary>
        /// Generates a JSON response with status 200 on success or returns a plain text response of status 500 on failure.
        /// </summary>
        /// <param name="unsafeCodeToExecute"></param>
        /// <returns></returns>
        public static IActionResult GenerateJSONResponse(this ControllerBase controller, Func<object> unsafeCodeToExecute)
        {
            try
            {
                return GenerateContentResult(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(unsafeCodeToExecute()), USER_REGISTERResponseContentType.JSON);
            }
            catch (Exception e)
            {
                return GenerateContentResult(HttpStatusCode.InternalServerError, e.ExtractInnerExceptionMessage(), USER_REGISTERResponseContentType.PlainText);
            }
        }

        /// <summary>
        /// Generates a JSON response with status 200 on success or returns a plain text response of status 500 on failure.
        /// </summary>
        /// <param name="unsafeCodeToExecuteAsync"></param>
        /// <returns></returns>
        public static async Task<IActionResult> GenerateJSONResponseAsync(this ControllerBase controller, Func<Task<object>> unsafeCodeToExecuteAsync)
        {
            try
            {
                var content = await unsafeCodeToExecuteAsync();
                return GenerateContentResult(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(content), USER_REGISTERResponseContentType.JSON);
            }
            catch (Exception e)
            {
                return GenerateContentResult(HttpStatusCode.InternalServerError, e.ExtractInnerExceptionMessage(), USER_REGISTERResponseContentType.PlainText);
            }
        }

        /// <summary>
        /// Generates a PlainText response with status 200 on success or returns a plain text response of status 500 on failure.
        /// </summary>
        /// <param name="unsafeCodeToExecute"></param>
        /// <returns></returns>
        public static IActionResult GeneratePlainTextResponse(this ControllerBase controller, Func<string> unsafeCodeToExecute)
        {
            try
            {
                return GenerateContentResult(HttpStatusCode.OK, unsafeCodeToExecute(), USER_REGISTERResponseContentType.PlainText);
            }
            catch (Exception e)
            {
                return GenerateContentResult(HttpStatusCode.InternalServerError, e.ExtractInnerExceptionMessage(), USER_REGISTERResponseContentType.PlainText);
            }
        }

        /// <summary>
        /// Generates a PlainText response with status 200 on success or returns a plain text response of status 500 on failure.
        /// </summary>
        /// <param name="unsafeCodeToExecuteAsync"></param>
        /// <returns></returns>
        public static async Task<IActionResult> GeneratePlainTextResponseAsync(this ControllerBase controller, Func<Task<string>> unsafeCodeToExecuteAsync)
        {
            try
            {
                var content = await unsafeCodeToExecuteAsync();
                return GenerateContentResult(HttpStatusCode.OK, content, USER_REGISTERResponseContentType.PlainText);
            }
            catch (Exception e)
            {
                return GenerateContentResult(HttpStatusCode.InternalServerError, e.ExtractInnerExceptionMessage(), USER_REGISTERResponseContentType.PlainText);
            }
        }

        /// <summary>
        /// Generates a response of a given status code, content and content type.
        /// </summary>
        /// <param name="statusCode">The status of the response to communicate</param>
        /// <param name="content">The payload to send client</param>
        /// <param name="contentType">the content type of the payload</param>
        /// <returns></returns>
        public static ContentResult GenerateContentResult(HttpStatusCode statusCode, string content, USER_REGISTERResponseContentType contentType)
        {
            return new ContentResult { StatusCode = (int)statusCode, Content = content, ContentType = contentType.GetEnumName() };
        }
    }
}

