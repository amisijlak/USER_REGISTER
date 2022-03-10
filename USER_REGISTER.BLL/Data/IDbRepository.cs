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
using USER_REGISTER.DAL.Interfaces;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Data
{
    public interface IDbRepository
    {
        IUSER_REGISTERLogger Logger { get; }

        /// <summary>
        /// Execute the specified logic in a new Transaction
        /// </summary>
        /// <param name="LogicToExecute"></param>
        void ExecuteInNewTransaction(Action LogicToExecute);

        DbSet<T> Set<T>() where T : class;

        void UpdateDatabaseModel<T>(T dbItem, T updatedItem) where T : class;

        /// <summary>
        /// Always execute in Transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbItems"></param>
        /// <param name="updatedItems"></param>
        void UpdateCollections<T>(List<T> dbItems, List<T> updatedItems) where T : class, INumericPrimaryKey;

        void SaveChanges();

        void LoadReference<T>(T model, Expression<Func<T, object>> Property) where T : class;
        void LoadCollection<T>(T model, Expression<Func<T, IEnumerable<object>>> CollectionProperty) where T : class;

        void ExecuteSqlCommand(string query, params DbParameter[] parameters);
        IEnumerable<T> SqlQuery<T>(string query, Func<DbDataReader, T> ExtractDataFtn, params SqlParameter[] parameters);

        void LogUserAction(ISessionService _sessionService, object recordId, string description
            , ModelComparisonResult modelComparisonResult = null);
        void LogUserAction(ISessionService _sessionService, SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction action, object recordId
            , string description, ModelComparisonResult modelComparisonResult = null);
        void LogUserAction(string userId, string ipAddress, SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction action, object recordId
            , string description, ModelComparisonResult modelComparisonResult = null);

        string GetConnectionString();
        void Remove<T>(T model) where T : class;

        void SetConnectionTimeout(int timeout);
        DataTable GetQueryResultsAsDataTable(string query, params SqlParameter[] parameters);
    }
}
