using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Security
{
    /// <summary>
    /// Represents the payload sent back to an API call to authenticate a user.
    /// </summary>
    public class AuthenticateUserResponseModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public AuthenticatedUserModel User { get; set; }
    }

    /// <summary>
    /// Represents the data about an authenticated user to be returned to the API caller.
    /// </summary>
    public class AuthenticatedUserModel
    {
        /// <summary>
        /// The User Id
        /// </summary>
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// A JWT Token generated from the <see cref="Id"/> property.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Whether or not this is the Super User.
        /// </summary>
        public bool IsSuperUser { get; set; }
        /// <summary>
        /// Whether the user in in the Super Role.
        /// </summary>
        public bool IsInSuperRole { get; set; }
        /// <summary>
        /// The distinct Permissions the user has access to.
        /// </summary>
        //public List<SecurityPermission> Permissions { get; set; } = new List<SecurityPermission>();
    }
}
