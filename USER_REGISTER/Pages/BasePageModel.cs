using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER.Pages
{
    public abstract class BasePageModel : PageModel, IErrorMessage
    {
        public int? CurrentPage { get; set; }
        public string SearchTerm { get; set; }
        public string ErrorMessage { get; set; }
        public int? PageSize { get; set; }
        [DisplayName("Enter Code")]
        public string Code { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        protected async Task<bool> ExecuteUnsafeCodeAsync(Func<Task> codeToExecuteAsync)
        {
            if (codeToExecuteAsync == null) throw new ArgumentNullException(nameof(codeToExecuteAsync));

            try
            {
                await codeToExecuteAsync();
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.ExtractInnerExceptionMessage();
            }

            return false;
        }
    }
}
