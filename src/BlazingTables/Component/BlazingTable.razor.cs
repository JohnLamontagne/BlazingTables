using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using BlazingTables.Utils;
using System.Collections;

namespace BlazingTables.Component
{
    public class BlazingTableBase<T> : ComponentBase
    {
        public IEnumerable<T> FilteredData { get; protected set; }

        private List<T> _data;

        protected List<DataColumn<T>> _columns = new List<DataColumn<T>>();

        private Queue<DataColumn<T>> _orderedCols;
        private Queue<Tuple<DataColumn<T>, ColumnFilterArgs>> _colFilters;

        [Parameter]
        public IReadOnlyList<T> Data 
        { 
            get => _data; 
            set 
            {
                _data = value.ToList();
            } 
        }

        public BlazingTableBase()
        {
            _data = new List<T>();
            _orderedCols = new Queue<DataColumn<T>>();
            _colFilters = new Queue<Tuple<DataColumn<T>, ColumnFilterArgs>>();
        }

        public void Remove(T row)
        {
            if (_data.Contains(row))
            {
                _data.Remove(row);

                this.UpdateFilteredData();
            }
        }

        public void Add(T row)
        {
            _data.Add(row);

            this.UpdateFilteredData();
        }

        public void Update()
        {
            this.UpdateFilteredData();
        }


        protected void HandleSortAZ(DataColumn<T> column)
        {
            
            column.Sorting = Sorting.Ascending;

            if (!_orderedCols.Contains(column))
                _orderedCols.Enqueue(column);

            this.UpdateFilteredData();
        }

        protected void HandleSortZA(DataColumn<T> column)
        {
            column.Sorting = Sorting.Descending;

            if (!_orderedCols.Contains(column))
                _orderedCols.Enqueue(column);

            this.UpdateFilteredData();
        }

        protected void HandleContains(DataColumn<T> column, string text)
        {
            var item = new Tuple<DataColumn<T>, ColumnFilterArgs>(column, new ContainsColumnFilterArgs(text));

            if (!_colFilters.Contains(item))
            {
                _colFilters.Enqueue(item);
                column.Filter = Filters.Contains();
                this.UpdateFilteredData();
            }
        }

        private void UpdateFilteredData()
        {
            var filterOrderLst = _orderedCols.ToList();

            IQueryable<T> query = null;
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
                        query = ((IOrderedQueryable<T>)query).ThenBy(x => col.Value(x));
                    }
                    else if (col.Sorting == Sorting.Descending)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(x => col.Value(x));
                    }
                }
            }
            else
            {
                query = _data.AsQueryable();
            }


          
            var colFiltersLst = _colFilters.ToList();
            foreach (var col in colFiltersLst)
            {
                query = query.Where(x => col.Item1.Filter(col.Item1.Value(x), col.Item2));
            }

            this.FilteredData = query.ToList();

            this.StateHasChanged();
        }

       
    }
}
