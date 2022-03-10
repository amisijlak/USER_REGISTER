using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER
{
    public static class UIPaginationHelper
    {
        public static IHtmlContent RenderPageSizeControl(this IHtmlHelper Html)
        {
            return Html.Partial("Controls/_PageSizeControl");
        }
    }
}
