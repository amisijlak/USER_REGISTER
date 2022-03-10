using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Interfaces;


namespace USER_REGISTER.DAL.BaseModels
{
    public abstract class _BaseNumericModel : INumericPrimaryKey
    {
        #region Fields

        [Key]
        public long Id { get; set; }

        #endregion
    }

    public abstract class _BasePersonModel : _BaseNumericModel, IPersonName, IPersonalDetails
    {
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid Name")]
        public string LastName { get; set; }
        [Display(Name = "Other Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid Name")]
        public string OtherName { get; set; }
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{6}", ErrorMessage = "Invalid Phone Number (256) 826-327321")]
        [Display(Name = "Phone Number"), MaxLength(30), Phone]
        public string PhoneNumber { get ; set ; }
        [EmailAddress, MaxLength(255)]
        public string Email { get ; set ; }
        public Gender Gender { get ; set ; }
    }
    public enum Gender
    {
        Male =1,
        Female,
    }
  
}
