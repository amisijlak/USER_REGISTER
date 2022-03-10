using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using DevExtreme.AspNet.Mvc.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER.DevExExtensions
{
    public class USER_REGISTEROptionsButtonsConfiguration
    {
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanViewAuditLog { get; set; }
        public string EditButtonVisibilityJSFtn { get; set; }
        public string DeleteButtonVisibilityJSFtn { get; set; }
        public string AuditLogButtonVisibilityJSFtn { get; set; }
        public Action<CollectionFactory<DataGridColumnButtonBuilder>> ConfigureAdditionalButtonsFtn { get; set; }
        public HorizontalEdge? HorizontalEdge { get; set; }
        public string Caption { get; set; }
        public bool IsNotFixed { get; set; }
    }
}
