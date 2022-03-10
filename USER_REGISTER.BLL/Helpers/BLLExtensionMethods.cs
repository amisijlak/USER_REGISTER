using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;


namespace USER_REGISTER
{
    public static class BLLExtensionMethods
    {
        /// <summary>
        /// Whether a given object implements the specified Interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ImplementsInterface<T>(this object item)
        {
            return typeof(T).IsAssignableFrom(item.GetType());
        }

        /// <summary>
        /// Throws an Exception if the current object is NULL
        /// </summary>
        /// <param name="item"></param>
        /// <param name="CustomErrorMessage"></param>
        public static void EnsureIsNotNULL(this object item, string CustomErrorMessage)
        {
            if (item == null)
            {
                throw new Exception(CustomErrorMessage);
            }
        }

        /// <summary>
        /// Extracts the error message from the lowest inner exception
        /// </summary>
        /// <param name="e"></param>
        /// <param name="showStackTrace">Whether or not to show the stack trace. Only works in Debug mode.</param>
        /// <returns></returns>
        public static string ExtractInnerExceptionMessage(this Exception e, bool logException = true, [CallerMemberName] string Source = null)
        {
            //if (logException) DMSLogger.Current?.LogException(Source, e);
            return _ExtractInnerExceptionMessage(e);
        }

        private static string _ExtractInnerExceptionMessage(Exception e)
        {
            return e.InnerException == null ? e.Message : _ExtractInnerExceptionMessage(e.InnerException);
        }

        public static string FormatAsParameter(this DateTime? Date, bool IncludeTime = false)
        {
            return Date?.FormatAsParameter(IncludeTime);
        }

        public static string FormatAsParameter(this DateTime Date, bool IncludeTime = false)
        {
            return Date.ToString("yyyy-MM-dd" + (IncludeTime ? " HH:mm:ss" : ""));
        }

        public static bool DatesIntersect(DateTime? Start1, DateTime? End1, DateTime? Start2, DateTime? End2)
        {
            return (Start1 <= Start2 && End1 >= Start2) || (Start1 <= End2 && End1 >= End2)
                       || (Start2 <= Start1 && End2 >= Start1);
        }

        public static bool IsSuperRole(this ISecurityRole model)
        {
            return string.Compare(model.Name, Constants.SuperRoleName, true) == 0;
        }

        public static IEnumerable<SqlParameter> DuplicateParameters(this IEnumerable<SqlParameter> parameters)
        {
            foreach (var par in parameters)
            {
                yield return new SqlParameter(par.ParameterName, par.Value);
            }
        }

        public static bool IsNullOrEmpty(this string val) => string.IsNullOrEmpty(val);

        #region Decimal

        public static string FormatForDisplay(this decimal? value)
        {
            return value?.FormatForDisplay();
        }

        public static string FormatForDisplay(this decimal value)
        {
            return value.ToString("#,##0.#");
        }

        #endregion

        #region double

        public static string FormatForDisplay(this double? value)
        {
            return value?.FormatForDisplay();
        }

        public static string FormatForDisplay(this double value)
        {
            return value.ToString("#,##0.#");
        }

        #endregion

        #region int

        public static string FormatForDisplay(this int? value)
        {
            return value?.FormatForDisplay();
        }

        public static string FormatForDisplay(this int value)
        {
            return value.ToString("#,##0.#");
        }

        #endregion

        #region DateTime

        public static string FormatForDisplay(this DateTime? Date, bool includeTime = false)
        {
            return Date?.FormatForDisplay(includeTime);
        }

        public static string FormatForDisplay(this DateTime Date, bool includeTime = false)
        {
            return Date.ToString("dd MMM yyyy" + (includeTime ? " HH:mm" : ""));
        }

        #endregion

        #region UserManager

        /// <summary>
        /// It finds a user by either their username or email
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="searchKey">Username or Email</param>
        /// <returns></returns>
        public static async Task<ApplicationUser> GetUserByUsernameOrEmailAsync(this UserManager<ApplicationUser> manager, string searchKey)
        {
            ApplicationUser user = await manager.FindByNameAsync(searchKey);

            if (user == null) user = await manager.FindByEmailAsync(searchKey);

            return user;
        }

        /// <summary>
        /// It finds a user by either their username or email and validates their password
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="searchKey">Username or Email</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<bool> CheckUserPasswordByUsernameOrEmailAsync(this UserManager<ApplicationUser> manager, string searchKey, string password)
        {
            var user = await manager.GetUserByUsernameOrEmailAsync(searchKey);

            if (user != null) return await manager.CheckPasswordAsync(user, password);

            return false;
        }

        #endregion

        #region Security Role

        public static SecurityRole GetSuperRole(this IDbRepository repository)
        {
            return repository.Set<SecurityRole>().Where(r => r.Name == Constants.SuperRoleName).Single();
        }

        public static IQueryable<SecurityRole> GetAccessibleRoles(this IDbRepository repository, ISessionService sessionService)
        {
            var query = repository.Set<SecurityRole>().Select(r => r);

            //if (!sessionService.IsInSuperRole()) query = query.Where(r => r.Name != Constants.SuperRoleName);

            return query;
        }

        public static bool IsAccessible(this SecurityRole role, IDbRepository repository, ISessionService sessionService)
        {
            return repository.GetAccessibleRoles(sessionService).Any(r => r.Id == role.Id);
        }

        public static List<UserRoleModel> GetUserRolesForEdit(this IDbRepository repository, string userId)
        {
            return (from a in repository.Set<IdentityUserRole<string>>().Where(r => r.UserId == userId)
                    join b in repository.Set<SecurityRole>() on a.RoleId equals b.Id
                    select new UserRoleModel
                    {
                        RoleId = b.Id,
                        Name = b.Name
                    }).ToList();
        }

        #endregion

        #region Application User

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="sessionService"></param>
        /// <returns></returns>
        public static IQueryable<ApplicationUser> GetAccessibleUsers(this IDbRepository repository, ISessionService sessionService)
        {
            var query = repository.Set<ApplicationUser>().Select(r => r);

            //if (!sessionService.IsInSuperRole())
            //{
            //    var superRole = repository.GetSuperRole();
            //    var superAdminIds = repository.Set<IdentityUserRole<string>>().Where(r => r.RoleId == superRole.Id).Select(r => r.UserId);

            //    query = query.Where(r => !superAdminIds.Contains(r.Id));
            //}

            return query;
        }

        public static bool IsAccessible(this ApplicationUser user, IDbRepository repository, ISessionService sessionService)
        {
            return repository.GetAccessibleUsers(sessionService).Any(r => r.Id == user.Id);
        }

        public static bool IsUserInSuperRole(this IDbRepository repository, string userId)
        {
            var superRole = repository.GetSuperRole();
            return repository.Set<IdentityUserRole<string>>().Any(r => r.RoleId == superRole.Id && r.UserId == userId);
        }

        public static bool InSuperRole(this ApplicationUser user, IDbRepository repository)
        {
            var superRole = repository.GetSuperRole();
            return repository.Set<IdentityUserRole<string>>().Any(r => r.RoleId == superRole.Id && r.UserId == user.Id);
        }

        //public static bool IsCurrentUser(this ApplicationUser user, ISessionService sessionService)
        //{
        //    return string.Compare(user.Id, sessionService.GetUserId(), true) == 0;
        //}

        #endregion

        #region Cloning Properties
        /// <summary>
        /// Clone the Properties of a given object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetObject"></param>
        /// <param name="sourceObject"></param>
        public static void CloneProperties<T>(this T targetObject, T sourceObject, params string[] columnsToIgnore) where T : class
        {
            Type[] acceptedTypes = new[] { typeof(Decimal), typeof(int), typeof(string), typeof(DateTime), typeof(bool), typeof(double)
            ,typeof(float),typeof(Guid)};

            foreach (var property in targetObject.GetType().GetProperties().Where(r => !columnsToIgnore.Contains(r.Name)).ToList())
            {
                Type actualPropertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                try
                {
                    if (actualPropertyType.IsEnum || acceptedTypes.Contains(actualPropertyType))
                    {
                        targetObject.GetType().GetProperty(property.Name).SetValue(targetObject, sourceObject.GetType().GetProperty(property.Name)
                        .GetValue(sourceObject));
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
        #endregion
    }
}

