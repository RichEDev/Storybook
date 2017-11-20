namespace SpendManagementLibrary.Logic_Classes.Fields
{
    using System.Collections.Generic;
    using System.Linq;

    public class TableRelabler : Relabler<cTable>

    {
        public TableRelabler(cAccountProperties accountProperties)
            : base(accountProperties)
        {
        }

        /// <inheritdoc />
        public override cTable Relabel(cTable table)
        {
            var result = new cTable(table);
            result.Description = this.Replace(result.Description, result.RelabelParam);
            return result;
        }

        /// <inheritdoc />
        public override SortedList<TK, cTable> Convert<TK>(SortedList<TK, cTable> sortedList)
        {
            var result = new SortedList<TK, cTable>();
            foreach (KeyValuePair<TK, cTable> keyValuePair in sortedList)
            {
                result.Add(keyValuePair.Key, new cTable(keyValuePair.Value));
            }

            return base.Convert(result);
        }

        /// <inheritdoc />
        public override List<cTable> Convert(List<cTable> list)
        {
            return base.Convert(list.Select(t => new cTable(t)).ToList());
        }

        /// <inheritdoc />
        public override cTable Convert(cTable table)
        {
            return base.Convert(new cTable(table));
        }
    }
}