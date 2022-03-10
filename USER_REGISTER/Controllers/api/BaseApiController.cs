using Microsoft.AspNetCore.Mvc;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;

namespace USER_REGISTER.Controllers.api
{
    /// <summary>
    /// Represents a base API Controller for implementing REST.
    /// Every action MUST call one of it's 3 Generate methods to return a result to the client.
    /// </summary>
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ISessionService _sessionService;
        protected readonly IDbRepository _repository;

        public BaseApiController(IDbRepository repository, ISessionService sessionService)
        {
            _sessionService = sessionService;
            _repository = repository;
        }
    }
}
