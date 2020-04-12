using System.Collections.Generic;

namespace BlazingTables.Utils
{
    public class FilterArgs
    {
        /// <summary>
        /// Input from the user to be used in the corresponding filter.
        /// </summary>
        public Dictionary<string, string> InputValues { get; private set; }

        public FilterArgs(Dictionary<string, string> inputValues)
        {
            this.InputValues = inputValues;
        }
    }
}