using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER.DevExExtensions
{
    public enum DataGridDataSourceType
    {
        /// <summary>
        /// <para>Use MVC Action to load data.</para>
        /// Set <see cref="DataGridConfiguration.MvcDataController"/>, <see cref="DataGridConfiguration.MvcDataAction"/> and 
        /// optionally <see cref="DataGridConfiguration.MvcParameters"/>.
        /// </summary>
        MVCAction = 0,
        /// <summary>
        /// <para>Use a Url that returns JSON data.</para>
        /// Set <see cref="DataGridConfiguration.JsonDatasourceUrl"/>
        /// </summary>
        JsonUrl = 1,
        /// <summary>
        /// <para>Use a collection of records attached to <see cref="DataGridConfiguration.CollectionDataSource"/></para>
        /// </summary>
        CollectionOfRecords = 2,
        /// <summary>
        /// <para>Use Razor Page Handler</para>
        /// Set <see cref=" DataGridConfiguration.RazorPageHandlerUrl"/>
        /// </summary>
        RazorPageHandler=3
    }
}
