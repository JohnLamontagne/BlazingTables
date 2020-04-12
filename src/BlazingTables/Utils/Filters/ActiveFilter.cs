using BlazingTables.Component;
using BlazingTables.Component.TableFilters;

namespace BlazingTables.Utils
{
    public class ActiveFilter<TItem>
    {
        public TableFilter<TItem> Filter { get; set; }

        public FilterArgs FilterArgs { get; }

        public ActiveFilter(TableFilter<TItem> filter, FilterArgs args)
        {
            this.Filter = filter;
            this.FilterArgs = args;
        }
    }
}