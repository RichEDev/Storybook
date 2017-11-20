namespace SpendManagementLibrary.Logic_Classes.Fields
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Relabler<T> : IRelabler<T>
        where T : IRelabel, new()
    {
        private readonly cAccountProperties _accountProperties;

        protected Relabler(cAccountProperties accountProperties)
        {
            this._accountProperties = accountProperties;
        }

        /// <inheritdoc />
        internal string Replace(string source, string relabelParam)
        {
            if (string.IsNullOrEmpty(source)|| string.IsNullOrEmpty(relabelParam))
            {
                return source;
            }
            
            switch (relabelParam.ToUpper())
            {
                case "SUPPLIER_PRIMARY_TITLE":
                    if (!string.IsNullOrEmpty(this._accountProperties.SupplierPrimaryTitle))
                    {
                        return this.ReplaceText(source, "Supplier", this._accountProperties.SupplierPrimaryTitle);
                    }
                    break;
                case "CONTRACT_DESC_TITLE":
                    if (!string.IsNullOrEmpty(this._accountProperties.ContractDescTitle))
                    {
                        return this.ReplaceText(source, "Contract Description", this._accountProperties.ContractDescTitle);
                    }
                    break;
                case "CONTRACT_CAT_TITLE":
                    if (!string.IsNullOrEmpty(this._accountProperties.ContractCategoryTitle))
                    {
                        return this.ReplaceText(source, "Contract Category", this._accountProperties.ContractCategoryTitle);
                    }
                    break;
                case "SUPPLIER_CAT_TITLE":
                    if (!string.IsNullOrEmpty(this._accountProperties.SupplierCatTitle))
                    {
                        return this.ReplaceText(source, "Supplier Category", this._accountProperties.SupplierCatTitle);
                    }
                    break;
                case "SUPPLIER_REGION_TITLE":
                    if (!string.IsNullOrEmpty(this._accountProperties.SupplierRegionTitle))
                    {
                        return this.ReplaceText(source, "County", this._accountProperties.SupplierRegionTitle);
                    }
                    break;
                case "PENALTY_CLAUSE_TITLE":
                    if (!string.IsNullOrEmpty(this._accountProperties.PenaltyClauseTitle))
                    {
                        return this.ReplaceText(source, "Penalty Clause", this._accountProperties.PenaltyClauseTitle);
                    }
                    break;
            }
            return source;
        }

        internal string ReplaceText(string source, string replace, string replaceWith)
        {
            source = source.Replace(replace, replaceWith);
            source = source.Replace(replace.ToLower(), replaceWith.ToLower());
            return source;
        }

        public abstract T Relabel(T field);

        /// <inheritdoc />
        public virtual SortedList<TK, T> Convert<TK>(SortedList<TK, T> sortedList)
        {
            for (int i = 0; i < sortedList.Values.Count; i++)
            {
                sortedList.Values[i] = this.Relabel(sortedList.Values[i]);
            }

            return sortedList;
        }

        /// <inheritdoc />
        public virtual List<T> Convert(List<T> list)
        {
            return list.Select(this.Relabel).ToList();
        }

        /// <inheritdoc />
        public virtual T Convert(T field)
        {
            return field == null ? field : this.Relabel(field);
        }

       
    }
}