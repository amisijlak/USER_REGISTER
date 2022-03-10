using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using DevExtreme.AspNet.Mvc.Factories;
using System;

namespace USER_REGISTER.DevExExtensions
{
    public static class DevExpressDataGridHelper
    {
        private const string DefaultHandleSelectionJSFtn = "app.components.dataGrid.handleSelectionChange";

        public static void AddRowNumberColumn<T>(this CollectionFactory<DataGridColumnBuilder<T>> columns) where T : class
        {
            columns.Add().Name("#").Caption("#").CellTemplate(new JS("app.components.dataGrid.computeRowNumber"));
        }

        public static void AddOptionsButtons<T>(this CollectionFactory<DataGridColumnBuilder<T>> columns, USER_REGISTEROptionsButtonsConfiguration configuration) where T : class
        {
            if (columns == null) throw new ArgumentNullException(nameof(columns));
            else if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            else if (!configuration.CanEdit && !configuration.CanDelete && !configuration.CanViewAuditLog && configuration.ConfigureAdditionalButtonsFtn == null) return;

            columns.Add().Name("Options").Caption(configuration.Caption ?? "Options").Type(GridCommandColumnType.Buttons)
                .Fixed(!configuration.IsNotFixed).FixedPosition(configuration.HorizontalEdge ?? HorizontalEdge.Left).Buttons(button =>
               {
                   if (configuration.CanEdit)
                   {
                       var bt = button.Add().Name("Edit").Icon("fa fa-edit").OnClick("app.components.dataGrid.editItem").Hint("Edit");
                       if (configuration.EditButtonVisibilityJSFtn != null) bt.Visible(new JS(configuration.EditButtonVisibilityJSFtn));
                   }

                   configuration.ConfigureAdditionalButtonsFtn?.Invoke(button);

                   if (configuration.CanDelete)
                   {
                       var bt = button.Add().Name("Delete").Icon("fa fa-trash text-danger").OnClick("app.components.dataGrid.deleteItem").Hint("Delete");
                       if (configuration.DeleteButtonVisibilityJSFtn != null) bt.Visible(new JS(configuration.DeleteButtonVisibilityJSFtn));
                   }
                   if (configuration.CanViewAuditLog)
                   {
                       var bt = button.Add().Name("AuditLog").Icon("fa fa-history text-dark").OnClick("app.components.dataGrid.viewItemAuditLog").Hint("View Audit Log");
                       if (configuration.AuditLogButtonVisibilityJSFtn != null) bt.Visible(new JS(configuration.AuditLogButtonVisibilityJSFtn));
                   }
               });
        }

        public static DataGridBuilder<T> Configure<T>(this DataGridBuilder<T> dataGrid, DataGridConfiguration configuration)
            where T : class
        {
            if (dataGrid == null) throw new ArgumentNullException(nameof(dataGrid));
            else if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            configuration.Validate();

            dataGrid = ConfigureDataGrid(dataGrid, configuration);
            return ConfigureDataSources<T>(dataGrid, configuration);
        }

        private static DataGridBuilder<T> ConfigureDataSources<T>(DataGridBuilder<T> dataGrid, DataGridConfiguration configuration)
            where T : class
        {
            var dataKey = configuration.DataKey ?? "Id";

            switch (configuration.DataSourceType)
            {
                case DataGridDataSourceType.CollectionOfRecords:
                    dataGrid = dataGrid.DataSource(configuration.CollectionDataSource, dataKey);
                    break;

                case DataGridDataSourceType.MVCAction:
                    dataGrid = dataGrid.DataSource(d =>
                    {
                        var config = d.Mvc().Controller(configuration.MvcDataController).LoadAction(configuration.MvcDataAction)
                    .Key(dataKey).LoadParams(configuration.MvcParameters);

                        if (!configuration.MvcArea.IsNullOrEmpty()) config = config.Area(configuration.MvcArea);

                        return config;
                    });
                    break;

                case DataGridDataSourceType.JsonUrl:
                    dataGrid = dataGrid.DataSource(r => r.StaticJson().Url(configuration.JsonDatasourceUrl));
                    break;

                case DataGridDataSourceType.RazorPageHandler:
                    dataGrid = dataGrid.DataSource(d => d.RemoteController().Key(dataKey).LoadUrl(configuration.RazorPageHandlerUrl));
                    break;
            }

            return dataGrid;
        }

        private static DataGridBuilder<T> ConfigureDataGrid<T>(DataGridBuilder<T> dataGrid, DataGridConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            dataGrid = dataGrid.ID("gridContainer")
    .ShowBorders(true)
    .ShowRowLines(true)
    .RowAlternationEnabled(!configuration.DisableRowAlternation)
    .ColumnAutoWidth(true)
    .ColumnChooser(c => c.Enabled(true))
    .RemoteOperations(true)
    .FilterRow(filterRow => filterRow
        .Visible(true)
        .ApplyFilter(GridApplyFilterMode.Auto)
    )
    .SearchPanel(searchPanel => searchPanel
        .Visible(true)
        .Width(240)
        .Placeholder("Search...")
    )
    .HeaderFilter(headerFilter => headerFilter.Visible(true))
    .Paging(paging =>
    {
        if (!configuration.DisablePaging)
        {
            paging.PageSize(Constants.DEFAULT_PAGE_SIZE);
        }
    })
    .Pager(pager =>
    {
        if (!configuration.DisablePaging)
        {
            pager.ShowPageSizeSelector(true);
            pager.AllowedPageSizes(IViewModel.PageSizes);
            pager.ShowInfo(true);
            pager.ShowNavigationButtons(true);
        }
    });

            if (!configuration.OnRowClickedJSFtn.IsNullOrEmpty())
            {
                dataGrid = dataGrid.OnRowClick(configuration.OnRowClickedJSFtn);
            }

            if (configuration.EnableExport)
            {
                dataGrid = dataGrid.Export(e => e.Enabled(true).FileName(configuration.ExportFileName ?? "Data Export").AllowExportSelectedData(true));
            }

            if (configuration.SelectionMode.HasValue)
            {
                dataGrid = EnableSelectionForGrid(dataGrid, configuration.SelectionMode.Value, configuration.CustomSelectionChangedJSFtn);
            }

            return dataGrid;
        }

        private static DataGridBuilder<T> EnableSelectionForGrid<T>(DataGridBuilder<T> dataGrid, SelectionMode selectionMode, string selectionChangedJSFtn)
        {
            dataGrid = dataGrid.Selection(s => s.Mode(selectionMode)).OnSelectionChanged(selectionChangedJSFtn ?? DefaultHandleSelectionJSFtn);

            return dataGrid;
        }
    }
}
