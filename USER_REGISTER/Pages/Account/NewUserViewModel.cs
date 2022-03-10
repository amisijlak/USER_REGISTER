using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.Pages.Account
{
    public class NewUserViewModel : ApplicationUser
    {
      public List<UserDetails> users { get; set; }
        public void  LoadDetails(ISecurityService UserDetails)
        {
            var Data = UserDetails.GetAllUsers();
            var CurrentDateTime = DateTime.UtcNow;

            var qry = from a in Data
                      select new UserDetails
                      {
                          Email = a.Email,
                          FirstName = a.FirstName,
                          Id = a.Id,
                          LastName = a.LastName,
                          UserName = a.UserName,
                          Islocked = a.LockoutEnabled,
                          OtherName = a.OtherName,
                          Phonenumber = a.PhoneNumber,
                          Age = Age,
                          LockedOut = a.LockoutEnd == null ? false : a.LockoutEnd > CurrentDateTime
                      };

            users = qry.ToList();
        }

    }


    public class UserDetails
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Phonenumber { get; set; }
        public bool Islocked { get; set; }
        public bool LockedOut { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public int Age { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1} {2}", FirstName, LastName, OtherName);
            }
        }


    }
}
