using BlazingTables.Component;
using System.Collections.Generic;
using System.Linq;

namespace BlazingTables.Utils.Filters
{
    public class ContainsTextFilterHandler<TItem> : FilterHandler<TItem>
    {
        public override List<TItem> Apply(DataColumn<TItem> column, List<TItem> items, FilterArgs args)
        {
            return items.Where(x => column.Value(x).ToString().Contains(args.InputValues["Contains"])).ToList();
        }
    }
}