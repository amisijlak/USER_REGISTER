using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;

namespace USER_REGISTER.Controllers.api.Security
{
    /// <summary>
    /// Handles user related data.
    /// </summary>
    [Route(RouteHelper.ApiRoute)]
    [ApiController, AuthorizeAPIUserAttribute, AllowAnonymous]
    public class CurrentUserController : BaseApiController
    {
        public CurrentUserController(IDbRepository repository, ISessionService sessionService) : base(repository, sessionService) { }

        /// <summary>
        /// Returns the user data of the authenticated logged in user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return this.GenerateJSONResponse(() => _sessionService.CastToAPIUser());
        }
    }
}
