using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Interfaces;

namespace USER_REGISTER.DAL.Security
{
    public class ApplicationUser : IdentityUser<string>, IPersonName, IUserName, IPrimaryKeyEnabled<string>
    {
        [Required, MaxLength(255),DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required, MaxLength(255),DisplayName("Last Name")]
        public string LastName { get; set; }
        [MaxLength(255),DisplayName("Other Name")]
        public string OtherName { get; set; }
        [EmailAddress, MaxLength(255),NotMapped,DisplayName("Email")]
        public string UserEmail { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public string LoginToken { get; set; }

        /// <summary>
        /// NOT MAPPED
        /// </summary>
        [NotMapped]
        public string ErrorMessage { get; set; }
        [NotMapped]
        public string PassWord { get; set; }
        [NotMapped]
        public string ConfirmPassWord { get; set; }
        public int Age { get; set; }
        public bool HIVStatus { get; set; }
        public bool IsLockedOut()
        {
            return LockoutEnd == null ? false : LockoutEnd > DateTime.Now;
        }
    }

    
}
