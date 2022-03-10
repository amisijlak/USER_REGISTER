using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Helpers;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Interfaces;

namespace USER_REGISTER.BLL
{
    public abstract class BaseService
    {
        protected readonly IDbRepository _repository;
        protected readonly ISessionService _sessionService;
        protected readonly IUSER_REGISTERLogger _logger;

        public BaseService(IDbRepository repository, ISessionService sessionService, IUSER_REGISTERLogger logger)
        {
            this._repository = repository;
            this._sessionService = sessionService;
            _logger = logger;
        }

        protected void _SaveModel<T, O>(T Model) where T : class, IPrimaryKeyEnabled<O>
        {
            List<string> logXML = new List<string>();

            var dbSet = _repository.Set<T>();

            var dbModel = dbSet.Find(Model.Id);

            var logData = dbModel.CompareWith(Model, r => r.Id);
            var logMessage = dbModel == null ? "Created" : "Updated";

            if (dbModel == null)//New
            {
                dbSet.Add(Model);
            }
            else//existing
            {
                _repository.UpdateDatabaseModel(dbModel, Model);
            }

            _repository.SaveChanges();

            _repository.LogUserAction(_sessionService, Model.Id, logMessage, logData);
        }

        protected void _DeleteModel<T, O>(O Id) where T : class, IPrimaryKeyEnabled<O>
        {
            var dbSet = _repository.Set<T>();

            var dbModel = dbSet.Find(Id);

            dbModel.EnsureIsNotNULL("Record Not Found!");

            var logData = dbModel.CompareWith(null, r => r.Id);

            dbSet.Remove(dbModel);

            _repository.SaveChanges();

            _repository.LogUserAction(_sessionService, Id, null, logData);
        }

        protected string GetTempFolder() => Path.Combine(_logger.GetLogFolder(), $"temp-{Guid.NewGuid()}");
    }
}
