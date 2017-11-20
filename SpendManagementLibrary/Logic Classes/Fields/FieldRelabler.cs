namespace SpendManagementLibrary.Logic_Classes.Fields
{
    using System.Collections.Generic;
    using System.Linq;

    public class FieldRelabler : Relabler<cField>
    {
        public FieldRelabler(cAccountProperties accountProperties)
            : base(accountProperties)
        {

        }

        /// <inheritdoc />
        public override cField Relabel(cField field)
        {
            var result = new cField(field);
            result.Description = this.Replace(result.Description, result.RelabelParam);
            result.Comment = this.Replace(result.Comment, result.RelabelParam);
            result.FriendlyNameFrom = this.Replace(result.FriendlyNameFrom, result.RelabelParam);
            result.FriendlyNameTo = this.Replace(result.FriendlyNameTo, result.RelabelParam);
            result.RelabelParam = string.Empty;
            return result;
        }

        /// <inheritdoc />
        public override SortedList<TK, cField> Convert<TK>(SortedList<TK, cField> sortedList)
        {
            var result = new SortedList<TK, cField>();
            foreach (KeyValuePair<TK, cField> keyValuePair in sortedList)
            {
                result.Add(keyValuePair.Key, new cField(keyValuePair.Value));
            }

            return base.Convert(result);
        }

        /// <inheritdoc />
        public override List<cField> Convert(List<cField> list)
        {
            return base.Convert(list.Select(f => new cField(f)).ToList());
        }

        /// <inheritdoc />
        public override cField Convert(cField field)
        {
            return base.Convert(new cField(field));
        }
    }
}