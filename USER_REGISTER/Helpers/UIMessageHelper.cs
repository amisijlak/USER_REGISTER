using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER
{
    public static class UIMessageHelper
    {
        public static IHtmlContent RenderSuccessMessageControl(this IHtmlHelper Html)
        {
            return Html.Partial("Messages/_Success");
        }

        public static IHtmlContent RenderErrorMessageControl(this IHtmlHelper Html)
        {
            return Html.Partial("Messages/_Error");
        }
    }
}
