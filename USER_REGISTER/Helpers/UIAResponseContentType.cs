using System.ComponentModel;

namespace USER_REGISTER.Helpers
{
    public enum USER_REGISTERResponseContentType
    {
        [Description("application/json")]
        JSON,
        [Description("text/plain")]
        PlainText
    }
}
