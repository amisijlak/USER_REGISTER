using Microsoft.AspNetCore.Mvc.Rendering;

namespace USER_REGISTER
{
    public static class DevExpressExtensionMethods
    {
        /// <summary>
        /// Configure the Layout Mode that determines the scripts to embed
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="mode"></param>
        public static void SetLayoutMode(this IHtmlHelper Html, DevExpressLayoutMode mode)
        {
            Html.ViewData["DevExpressLayoutMode"] = mode;
        }

        /// <summary>
        /// Gets the Layout Mode that determines the scripts to embed
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static DevExpressLayoutMode GetLayoutMode(this IHtmlHelper Html)
        {
            return (DevExpressLayoutMode?)Html.ViewData["DevExpressLayoutMode"] ?? DevExpressLayoutMode.DevExtreme;
        }
    }

    public enum DevExpressLayoutMode
    {
        DevExtreme,
        Dashboard
    }
}
