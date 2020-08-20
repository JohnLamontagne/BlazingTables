using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using BlazingTables.Component.TableFilters;
using System;

namespace BlazingTables.Component
{
    public class BlazingTableBase<TItem> : ComponentBase
    {
        /// <summary>
        /// The current active data in the table after filtering is applied.
        /// </summary>
        public IEnumerable<TItem> FilteredData { get; protected set; }

        protected List<IColumn<TItem>> columns = new List<IColumn<TItem>>();

        protected List<TableFilter<TItem>> filters = new List<TableFilter<TItem>>();

        protected int curPage = 1;

        private List<TItem> _data = new List<TItem>();

        private List<DataColumn<TItem>> _orderedSortCols = new List<DataColumn<TItem>>();

        /// <summary>
        /// Enables an informational footer in the bottom left corner of the t
        /// table that displays currently viewed items along with total number of items when set to <see cref="true" />.
        /// </summary>
        [Parameter]
        public bool InfoFooterEnabled { get; set; } = true;

        /// <summary>
        /// Enables pagination for the table when set to true.
        /// </summary>
        [Parameter]
        public bool PaginationEnabled { get; set; } = true;

        /// <summary>
        /// Raised whenever the table page changes if <see cref="PaginationEnabled"/> is set to true.
        /// </summary>
        public EventHandler<EventArgs> PageChanged { get; set; }

        /// <summary>
        /// Raised whenever a new filter is applied to a <see cref="DataColumn{TItem}"/> in the table.
        /// </summary>
        public EventHandler<EventArgs> ActiveFiltersChanged { get; set; }

        /// <summary>
        /// If specified, disables <see cref="Data"></see> and instead polls for information from handler.
        /// </summary>
        [Parameter]
        public Func<List<TItem>> ReadData { get; set; }

        /// <summary>
        /// Data source for the table.
        /// </summary>
        [Parameter]
        public IReadOnlyList<TItem> Data
        {
            get => _data;
            set
            {
                _data = value.ToList();
            }
        }

        /// <summary>
        /// Amount of rows to be displayed in the table.
        /// If <see cref="PaginationEnabled"/> is set to true, any remaining rows after this limit will be moved to a successive page.
        /// </summary>
        [Parameter]
        public int RowsPerPage { get; set; } = 25;

        /// <summary>
        /// Removes the specified item as a row in the table.
        /// </summary>
        /// <param name="row">Item to be removed</param>
        public void Remove(TItem row)
        {
            if (_data.Contains(row))
            {
                _data.Remove(row);

                this.UpdateFilteredData();
            }
        }

        /// <summary>
        /// Adds the specified item to the table & data source.
        /// </summary>
        /// <param name="row">Item to be added</param>
        public void Add(TItem row)
        {
            _data.Add(row);

            this.UpdateFilteredData();
        }

        /// <summary>
        /// Tells the table to update with any new data.
        /// </summary>
        public void Update()
        {
            this.UpdateFilteredData();
        }

        protected void HandleApplyFilterSubmit(TableFilter<TItem> filter, DataColumn<TItem> column)
        {
            if (column.TryDisableFilter(filter))
            {
                this.UpdateFilteredData();
            }
            else
            {
                filter.OnApply((args) =>
                {
                    column.ActiveFilters.Add(new Utils.ActiveFilter<TItem>(filter, args));

                    this.UpdateFilteredData();
                });
            }

            this.ActiveFiltersChanged?.Invoke(this, new EventArgs());
        }

        protected void HandleSortAZ(DataColumn<TItem> column)
        {
            if (column.Sorting == Sorting.Ascending && column.Sorted)
            {
                column.Sorting = Sorting.None;
                column.Sorted = false;

                _orderedSortCols.Remove(column);
            }
            else
            {
                column.Sorting = Sorting.Ascending;
                column.Sorted = true;

                if (!_orderedSortCols.Contains(column))
                    _orderedSortCols.Add(column);
            }

            this.UpdateFilteredData();
        }

        protected void HandleSortZA(DataColumn<TItem> column)
        {
            if (column.Sorting == Sorting.Descending && column.Sorted)
            {
                column.Sorting = Sorting.None;
                column.Sorted = false;

                _orderedSortCols.Remove(column);
            }
            else
            {
                column.Sorting = Sorting.Descending;
                column.Sorted = true;

                if (!_orderedSortCols.Contains(column))
                    _orderedSortCols.Add(column);
            }

            this.UpdateFilteredData();
        }

        private void UpdateFilteredData()
        {
            if (this.ReadData != null)
            {
                _data = this.ReadData();
            }

            var filterOrderLst = _orderedSortCols.ToList();

            IQueryable<TItem> query = null;
            if (filterOrderLst.Count > 0)
            {
                if (filterOrderLst[0].Sorting == Sorting.Ascending)
                {
                    query = _data.AsQueryable().OrderBy(x => filterOrderLst[0].Value(x));
                }
                else if (filterOrderLst[0].Sorting == Sorting.Descending)
                {
                    query = _data.AsQueryable().OrderByDescending(x => filterOrderLst[0].Value(x));
                }

                foreach (var col in filterOrderLst.Skip(1))
                {
                    if (col.Sorting == Sorting.Ascending)
                    {
                        query = ((IOrderedQueryable<TItem>)query).ThenBy(x => col.Value(x));
                    }
                    else if (col.Sorting == Sorting.Descending)
                    {
                        query = ((IOrderedQueryable<TItem>)query).ThenByDescending(x => col.Value(x));
                    }
                }
            }
            else
            {
                query = _data.AsQueryable();
            }

            var filteredData = query.ToList();

            foreach (var column in columns.OfType<DataColumn<TItem>>())
            {
                foreach (var activeFilter in column.ActiveFilters)
                {
                    filteredData = activeFilter.Filter.FilterHandler.Apply(column, filteredData, activeFilter.FilterArgs);
                }
            }

            this.FilteredData = filteredData;

            this.StateHasChanged();
        }
    }
}