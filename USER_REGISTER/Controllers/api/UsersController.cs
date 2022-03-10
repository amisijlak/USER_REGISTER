using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Security;

namespace USER_REGISTER.Controllers.api
{
    [Route(RouteHelper.ApiRoute)]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ISecurityService _securityService;

        public UsersController(ISessionService sessionService, ISecurityService securityService)
        {
            _sessionService = sessionService;
            _securityService = securityService;
        }

        [HttpGet]
        public async Task<AuthenticatedUserModel> Authenticate(AuthenticateUserModel model)
        {
            return await _securityService.AuthenticateApiUserAsync(model.UsernameOrEmail, model.Password);
        }


    }
}
