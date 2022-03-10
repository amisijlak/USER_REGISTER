using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.BaseModels;

namespace USER_REGISTER.DAL.Security
{
    public class AuditLog : _BaseNumericModel
    {
        #region Fields

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [MaxLength(255)]
        public string IPAddress { get; set; }

        public SecurityModule Module { get; set; }
        public SecuritySubModule SubModule { get; set; }
        public SecuritySystemAction SystemAction { get; set; }

        [MaxLength(255)]
        public string RecordId { get; set; }
        public string Description { get; set; }
        public string ObjectJSON { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
