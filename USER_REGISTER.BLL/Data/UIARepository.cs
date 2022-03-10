using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Helpers;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL;
using USER_REGISTER.DAL.Interfaces;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Data
{
    public class USER_REGISTERRepository : IDbRepository
    {
        private readonly USER_REGISTERDBContext _database;
        private readonly IUSER_REGISTERLogger _logger;

        public IUSER_REGISTERLogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public USER_REGISTERRepository(USER_REGISTERDBContext db/*, IUSER_REGISTERLogger _logger*/)
        {
            this._database = db;
            //this._logger = _logger;
        }

        public void ExecuteInNewTransaction(Action LogicToExecute)
        {
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LogicToExecute();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _database.Set<T>();
        }

        public void UpdateDatabaseModel<T>(T dbItem, T updatedItem) where T : class
        {
            _database.Entry(dbItem).CurrentValues.SetValues(updatedItem);
        }

        public void UpdateCollections<T>(List<T> dbItems, List<T> updatedItems) where T : class, INumericPrimaryKey
        {
            var newListIds = updatedItems.Select(r => r.Id).ToList();

            //Delete removed entries
            foreach (var entry in dbItems.Where(a => newListIds.Contains(a.Id) == false).ToList())
            {
                this.Set<T>().Remove(entry);
            }

            this.SaveChanges();

            //Update existing
            foreach (var origEntry in dbItems.Where(r => newListIds.Contains(r.Id)).ToList())
            {
                var updatedEntry = updatedItems.Where(r => r.Id == origEntry.Id).Single();

                this.UpdateDatabaseModel(origEntry, updatedEntry);
            }

            this.SaveChanges();

            //Add new entries

            foreach (var entry in updatedItems.Where(r => r.Id == 0).ToList())
            {
                this.Set<T>().Add(entry);
            }

            this.SaveChanges();
        }

        public void SaveChanges()
        {
            _database.SaveChanges();
        }

        public void LoadReference<T>(T model, Expression<Func<T, object>> Property) where T : class
        {
            _database.Entry(model).Reference(Property).Load();
        }

        public void LoadCollection<T>(T model, Expression<Func<T, IEnumerable<object>>> CollectionProperty) where T : class
        {
            _database.Entry(model).Collection(CollectionProperty).Load();
        }

        public void ExecuteSqlCommand(string query, params DbParameter[] parameters)
        {
            _database.Database.ExecuteSqlRaw(query, parameters);
        }

        public IEnumerable<T> SqlQuery<T>(string query, Func<DbDataReader, T> ExtractDataFtn, params SqlParameter[] parameters)
        {
            using (var con = new SqlConnection(_database.ConnectionString))
            {
                con.Open();

                var cmd = new SqlCommand(query, con);
                if (parameters.Any()) cmd.Parameters.AddRange(parameters);

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return ExtractDataFtn(reader);
                    }
                }
            }

        }

        public void LogUserAction(ISessionService _sessionService, object recordId, string description
            , ModelComparisonResult modelComparisonResult = null)
        {
            //if (_sessionService.GetCurrentActionSecurityModule() == null) return;

            //LogUserAction(_sessionService, _sessionService.GetCurrentActionSecurityModule().Value, _sessionService.GetCurrentActionSecuritySubModule().Value
            //    , _sessionService.GetCurrentActionSecuritySystemAction().Value, recordId, description, modelComparisonResult);
        }

        public void LogUserAction(ISessionService _sessionService, SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction action, object recordId
            , string description, ModelComparisonResult modelComparisonResult = null)
        {
            //LogUserAction(_sessionService.GetUserId(), _sessionService.GetIPAddress(), Module, SubModule, action, recordId, description, modelComparisonResult);
        }

        public void LogUserAction(string userId, string ipAddress, SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction action, object recordId
            , string description, ModelComparisonResult modelComparisonResult = null)
        {
            this.Set<AuditLog>().Add(new AuditLog
            {
                SystemAction = action,
                Module = Module,
                SubModule = SubModule,
                Date = DateTime.Now,
                Description = description,
                RecordId = recordId?.ToString(),
                UserId = userId,
                IPAddress = ipAddress,
                ObjectJSON = SerializeAsJSON(modelComparisonResult)
            });

            SaveChanges();
        }

        public string SerializeAsJSON(ModelComparisonResult model)
        {
            return model == null ? (string)null : Newtonsoft.Json.JsonConvert.SerializeObject(model);
        }

        public string GetConnectionString()
        {
            return _database?.ConnectionString;
        }

        public void Remove<T>(T model) where T : class
        {
            _database.Remove(model);
        }

        public void SetConnectionTimeout(int timeout)
        {
            _database?.Database.SetCommandTimeout(timeout);
        }

        public DataTable GetQueryResultsAsDataTable(string query, params SqlParameter[] parameters)
        {
            DataTable data = new DataTable();

            using (var con = new SqlConnection(_database?.ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, con);

                foreach (var parameter in parameters)//add parameters
                {
                    command.Parameters.Add(parameter);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    adapter.Fill(data);
                }
                catch (Exception e)
                {
                    _logger.LogMessage("GetQueryResultsAsDataTable", query, true);
                    throw e;
                }

                con.Close();
            }

            return data;
        }
    }
}
