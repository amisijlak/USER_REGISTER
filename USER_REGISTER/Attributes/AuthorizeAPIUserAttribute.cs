using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;
using USER_REGISTER.Helpers;

namespace USER_REGISTER
{
    /// <summary>
    /// Ensure the user is authenticated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAPIUserAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var authorization = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (authorization == null)
                {
                    throw new Exception("Authorization Header Missing!");
                }

                var parameters = Encoding.UTF8.GetString(Convert.FromBase64String(authorization.Split(' ').Last())).Split(':');

                if (parameters.Length != 2)
                {
                    throw new Exception("Invalid Credentials Parameters!");
                }

                var _signInManager = (SignInManager<ApplicationUser>)context.HttpContext.RequestServices.GetService(typeof(SignInManager<ApplicationUser>));
                var _userManager = (UserManager<ApplicationUser>)context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));

                var dbUserTask = _userManager.GetUserByUsernameOrEmailAsync(parameters[0]);

                Task.WaitAll(dbUserTask);

                var result = _signInManager.PasswordSignInAsync(dbUserTask.Result?.UserName ?? parameters[0], parameters[1], false, true);

                Task.WaitAll(result);

                if (result.Result.Succeeded)
                {
                    var username = parameters[0];
                    var dbUser = dbUserTask.Result;

                    var repository = (IDbRepository)context.HttpContext.RequestServices.GetService(typeof(IDbRepository));

                    var sessionService = (ISessionService)context.HttpContext.RequestServices.GetService(typeof(ISessionService));
                    sessionService.InitializeForApiUser(dbUser);
                }
                else if (result.Result.IsLockedOut)
                {
                    throw new Exception("Your Account has been Locked Out!");
                }
                else
                {
                    throw new Exception("Invalid UserName Or Password!");
                }
            }
            catch (Exception e)
            {
                context.Result = ClientResponseHelper.GenerateContentResult(HttpStatusCode.Unauthorized, e.ExtractInnerExceptionMessage(), USER_REGISTERResponseContentType.PlainText);
            }
        }
    }
}
