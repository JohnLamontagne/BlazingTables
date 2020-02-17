using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingTables.Utils
{
    public static class Filters
    {
        public static Func<object, ColumnFilterArgs, bool> Contains()
        {
           return (val, args) =>
            {

                return val.ToString().Contains(((ContainsColumnFilterArgs)args).Text);
            };
        }
    }

    public class ColumnFilterArgs
    {
    }

    public class ContainsColumnFilterArgs : ColumnFilterArgs
    {
        public string Text { get; }

        public ContainsColumnFilterArgs(string text)
        {
            this.Text = text;
        }
    }
}
