using DevExtreme.AspNet.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER.DevExExtensions
{
    public class DataGridConfiguration
    {
        public DataGridDataSourceType DataSourceType { get; set; }
        /// <summary>
        /// Use Url.Content() to extract a full URL
        /// </summary>
        public string RazorPageHandlerUrl { get; set; }
        public string MvcArea { get; set; }
        public string MvcDataController { get; set; }
        public string MvcDataAction { get; set; }
        public string DataKey { get; set; }
        public object MvcParameters { get; set; }
        /// <summary>
        /// Set to NULL to disable selection. i.e. checkboxes for selecting.
        /// </summary>
        public SelectionMode? SelectionMode { get; set; }
        /// <summary>
        /// Optional. The JS function handle selection change on a data grid.
        /// </summary>
        public string CustomSelectionChangedJSFtn { get; set; }
        /// <summary>
        /// Optional. The JS function to handle row click event on a data grid.
        /// </summary>
        public string OnRowClickedJSFtn { get; set; }

        public bool DisableRowAlternation { get; set; }
        public bool DisablePaging { get; set; }

        public IEnumerable CollectionDataSource { get; set; }
        public string JsonDatasourceUrl { get; set; }

        /// <summary>
        /// Remember to set the <see cref="ExportFileName"/>.
        /// </summary>
        public bool EnableExport { get; set; }
        public string ExportFileName { get; set; }

        public DataGridConfiguration(DataGridDataSourceType dataSourceType)
        {
            this.DataSourceType = dataSourceType;
        }

        public void Validate()
        {
            switch (DataSourceType)
            {
                case DataGridDataSourceType.MVCAction:
                    if (MvcDataController.IsNullOrEmpty()) throw new ArgumentNullException(nameof(MvcDataController));
                    if (MvcDataAction.IsNullOrEmpty()) throw new ArgumentNullException(nameof(MvcDataAction));
                    break;

                case DataGridDataSourceType.JsonUrl:
                    if (JsonDatasourceUrl.IsNullOrEmpty()) throw new ArgumentNullException(nameof(JsonDatasourceUrl));
                    break;
            }
        }
    }
}
