using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_REGISTER;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.Controllers.api;

namespace USER_REGISTER.Controllers.api.Security
{
    [Route(RouteHelper.ApiRoute)]
    [ApiController, AuthorizeAPIUserAttribute, AllowAnonymous]
    public class CurrentUserPermissionsController : BaseApiController
    {
        public CurrentUserPermissionsController(IDbRepository repository, ISessionService sessionService) : base(repository, sessionService) { }

        [HttpGet]
        public IActionResult Get()
        {
            return this.GenerateJSONResponse(() => _sessionService.GetMobilePermissions());
        }
    }
}
