using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER
{
    public static class CustomExtensionMethods
    {
        /// <summary>
        /// Splits the SearchTerm using the space character
        /// </summary>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        public static string[] GetSearchTerms(this string SearchTerm)
        {
            return SearchTerm?.Split(' ') ?? new string[0];
        }

        #region Partials


        public static IHtmlContent RenderFilterCardFooterControl(this IHtmlHelper Html, string ActionName)
        {
            Html.ViewData["_FilterCardFooterAction"] = ActionName;
            return Html.Partial("Controls/_FilterCardFooter");
        }

        public static IHtmlContent RenderSearchTermControl(this IHtmlHelper Html, bool displayAsInlineControl)
        {
            Html.ViewData["_SearchTermContainerClass"] = displayAsInlineControl ? "display-inline" : "";
            return Html.Partial("Controls/_SearchTerm");
        }

        public static IHtmlContent RenderSaveButtonsControl(this IHtmlHelper Html, bool renderSaveAndContinueButtonAsWell)
        {
            return Html.Partial("Controls/_SaveButtons", renderSaveAndContinueButtonAsWell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="ActionName">MVC Action</param>
        /// <param name="saveButtonType"></param>
        /// <returns></returns>
        public static IHtmlContent RenderReloadForm(this IHtmlHelper Html, string ActionName, SaveButtonType? saveButtonType = null)
        {
            Html.ViewData["_ReloadFormSuffix"] = saveButtonType == null ? "" : "-" + saveButtonType.GetIntValue();
            Html.ViewData["ReloadFormUseAction"] = false;
            return Html.Partial("Controls/_ReloadForm", ActionName);
        }

        public static IHtmlContent RenderReloadFormUsingUrl(this IHtmlHelper Html, string url, SaveButtonType? saveButtonType = null)
        {
            Html.ViewData["_ReloadFormSuffix"] = saveButtonType == null ? "" : "-" + saveButtonType.GetIntValue();
            Html.ViewData["ReloadFormUseAction"] = true;
            return Html.Partial("Controls/_ReloadForm", url);
        }


        #endregion

        #region Export Functionality

        /// <summary>
        /// Formats a table tag for Export purposes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static HtmlString FormatTableForExport(this IExportable model)
        {
            return _FormatTableForExport(model.Export);
        }

        private static HtmlString _FormatTableForExport(bool ExportEnabled)
        {
            return new HtmlString(ExportEnabled ? "border='1' cellspacing='0' cellpadding='2'" : "");
        }

        #endregion

        #region Enums

        public static List<T> GetUSER_REGISTEREnumValues<T>(this IHtmlHelper Html)
        {
            List<T> enumTypes = new List<T>();

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                enumTypes.Add((T)value);
            }

            return enumTypes;
        }

        public static SelectList GetUSER_REGISTEREnumSelectList<T>(this IHtmlHelper Html)
        {
            List<object> enumTypes = new List<object>();

            foreach (Enum value in Enum.GetValues(typeof(T)))
            {
                enumTypes.Add(new { Key = value.GetEnumName(), Value = value.GetIntValue() });
            }

            return new SelectList(enumTypes, "Value", "Key");
        }

        #endregion

        #region Redirection

        public static RedirectToActionResult RedirectToUnAuthorizedPage(this ActionExecutingContext context)
        {
            return new RedirectToActionResult("UnAuthorized", "Home", new { area = "" }, true);
        }

        public static RedirectToActionResult RedirectToLockoutFeature(this ActionExecutingContext context)
        {
            return new RedirectToActionResult("Lockout", "Account", new { area = "" }, true);
        }

        #endregion

        #region DatePickers

        public static IHtmlContent DatePickerFor<T, TProperty>(this IHtmlHelper<T> Html, Expression<Func<T, TProperty>> Property)
        {
            return Html.TextBoxFor(Property, "{0:yyyy-MM-dd}", new { @class = "form-control datepicker" });
        }

        #endregion

        /// <summary>
        /// Formats the Fields with Errors
        /// </summary>
        /// <param name="ModelState"></param>
        public static string FormatModelStateErrorsIfAny(this ModelStateDictionary ModelState)
        {
            string html = "<ol>";

            var ModelStateValues = ModelState.Values.ToList();
            var ModelStateKeys = ModelState.Keys.ToList();

            for (int i = 0; i < ModelState.Keys.Count(); i++)
            {
                if (ModelStateValues[i].Errors.Count > 0)
                {
                    foreach (var error in ModelStateValues[i].Errors)
                    {
                        html += $"<li>{ModelStateKeys[i]}: {error.ErrorMessage}</li>";
                    }
                }
            }


            html += "</ol>";

            return html;
        }

        /// <summary>
        /// Display as Raw Html i.e. without escaping any characters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HtmlString Raw(this object value)
        {
            return new HtmlString(value?.ToString());
        }
    }

    public enum SaveButtonType
    {
        Save = 1,
        Save_And_Continue = 2
    }
}

