using System.Collections.Generic;

namespace BlazingTables.Component
{
    public class TableDataReadEventArgs<TItem>
    {
        public IReadOnlyList<DataColumn<TItem>> Columns { get; }

        public int Page { get; }

        public TableDataReadEventArgs(List<DataColumn<TItem>> columns, int page)
        {
            this.Columns = columns;
            this.Page = page;
        }
    }
}