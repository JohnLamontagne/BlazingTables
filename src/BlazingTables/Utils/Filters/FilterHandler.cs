using BlazingTables.Component;
using System.Collections.Generic;

namespace BlazingTables.Utils
{
    public abstract class FilterHandler<TItem>
    {
        /// <summary>
        /// Indicates whether the filter is currently being applied to the table to which it is assigned
        /// </summary>
        internal bool Applied { get; set; }

        /// <summary>
        /// Applies the filter to the specified items and returns the result as an <see cref="ICollection{TItem}"/>
        /// </summary>
        /// <param name="items">Represents the universe of items to be filtered.</param>
        /// <returns></returns>
        public abstract List<TItem> Apply(DataColumn<TItem> column, List<TItem> items, FilterArgs args);
    }
}