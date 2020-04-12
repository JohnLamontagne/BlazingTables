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
        public IEnumerable<TItem> FilteredData { get; protected set; }

        protected List<IColumn<TItem>> columns = new List<IColumn<TItem>>();

        protected List<TableFilter<TItem>> filters = new List<TableFilter<TItem>>();

        protected int curPage = 1;

        private List<TItem> _data = new List<TItem>();

        private Queue<DataColumn<TItem>> _orderedSortCols = new Queue<DataColumn<TItem>>();

        /// <summary>
        /// Event which provides the opportunity to custom handle data filtering & paging
        /// by updating the <see cref="Data"/> field with the data which will be displayed in the table.
        /// In this case, <see cref="Update"/> invocation is not required after setting <see cref="Data"/>.
        /// </summary>
        [Parameter]
        public EventHandler<EventArgs> OnBeforeTableDataRead { get; set; }

        [Parameter]
        public IReadOnlyList<TItem> Data
        {
            get => _data;
            set
            {
                _data = value.ToList();
            }
        }

        [Parameter]
        public int RowsPerPage { get; set; } = 25;

        public void Remove(TItem row)
        {
            if (_data.Contains(row))
            {
                _data.Remove(row);

                this.UpdateFilteredData();
            }
        }

        public void Add(TItem row)
        {
            _data.Add(row);

            this.UpdateFilteredData();
        }

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
        }

        protected void HandleSortAZ(DataColumn<TItem> column)
        {
            column.Sorting = Sorting.Ascending;
            column.Sorted = true;

            if (!_orderedSortCols.Contains(column))
                _orderedSortCols.Enqueue(column);

            this.UpdateFilteredData();
        }

        protected void HandleSortZA(DataColumn<TItem> column)
        {
            column.Sorting = Sorting.Descending;
            column.Sorted = true;

            if (!_orderedSortCols.Contains(column))
                _orderedSortCols.Enqueue(column);

            this.UpdateFilteredData();
        }

        private void UpdateFilteredData()
        {
            this.OnBeforeTableDataRead?.Invoke(this, new EventArgs());

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