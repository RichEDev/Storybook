
using System.Security.Cryptography.X509Certificates;
using SpendManagementLibrary.Helpers;

namespace Expenses_Reports.Formula
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using SpendManagementLibrary;

    /// <summary>
    /// A factory to create <see cref="Calculation"/> instances from a formula string and <see cref="DataColumnCollection"/>
    /// </summary>
    public class CalculationFactory
    {
        /// <summary>
        /// A private <see cref="SortedList{TKey,TValue}"/> of Data Column Ordinal and Report Column Name
        /// </summary>
        public readonly List<ColumnLookup> ColumnLookups;

        /// <summary>
        /// Create a new instance of <see cref="CalculationFactory"/>
        /// </summary>
        /// <param name="columns">An instance of <see cref="DataColumnCollection"/>used to collect the column ordinals</param>
        /// <param name="reportColumns">An instance of <see cref="ArrayList"/> containing a list of <seealso cref="cReportColumn"/>Which is used to generate the column names</param>
        public CalculationFactory(DataColumnCollection columns, ArrayList reportColumns)
        {
            this.ColumnLookups = this.GetColumnOrdinals(columns, reportColumns);
            this.UpdateCalculations();
        }

        /// <summary>
        /// Update the private <see cref="ColumnLookups"/> with the formula from the columns
        /// </summary>
        private void UpdateCalculations()
        {
            foreach (ColumnLookup columnLookup in this.ColumnLookups)
            {
                if (columnLookup.ReportColumn is cCalculatedColumn)
                {
                    columnLookup.Calculation = this.New(((cCalculatedColumn) columnLookup.ReportColumn).formattedFormula);
                }                   
            }
        }

        /// <summary>
        /// Create a new instance if <see cref="ICalculationPart"/>
        /// </summary>
        /// <param name="formula">The formula to base the calculation on</param>
        /// <returns>A newly formed <see cref="ICalculationPart"/></returns>
        public Calculation New(string formula)
        {
            var result = new Calculation();
            formula = formula.Replace('\xa0', ' ').Replace("\n", string.Empty);

            // Split the given string into parts
            var reg = new Regex(@"\[(.*?)\]");
            var idx = 0;
            var insideConcatenate = false;
            foreach (Match match in reg.Matches(formula))
            {
                var startIndex = match.Index;
                if (idx < startIndex)
                {
                    var part = new StaticText(formula.Substring(idx, startIndex - idx ));
                    if (insideConcatenate)
                    {
                        if (part.Value.StartsWith(")"))
                        {
                            insideConcatenate = false;
                        }
                    }
                    
                    if (part.Value.Contains("CONCATENATE("))
                    {
                        insideConcatenate = true;
                    }
                    result.Add(part);
                    
                }

                if (match.Value == formula)
                {
                    insideConcatenate = true;
                }

                var referencepart = this.CreateColumnReference(match.Value, insideConcatenate) ?? new StaticText(match.Value);
                idx = startIndex + match.Length;
                result.Add(referencepart);
            }

            if (idx < formula.Length)
            {
                var part = new StaticText(formula.Substring(idx));
                result.Add(part);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchValue"></param>
        /// <param name="insideConcatenate"></param>
        /// <returns></returns>
        private ICalculationPart CreateColumnReference(string matchValue, bool insideConcatenate)
        {
            matchValue = matchValue.Replace("[", string.Empty).Replace("]", string.Empty);
            var column = this.ColumnLookups.FirstOrDefault(c => c.Name == matchValue || c.FunctionName == matchValue);
            if (column == null)
            {
                var trimmed = this.TrimFunctionFromFieldName(matchValue);
                column = this.ColumnLookups.FirstOrDefault(c => c.Name == trimmed);
            }

            if (column == null)
            {
                return null;
            }

            if (insideConcatenate)
            {
                return new DateReference(column.DataColumn, column.Name, matchValue);
            }

            return new ColumnReference(column.DataColumn, column.Name, matchValue);
        }

        /// <summary>
        /// Trim any SQL functions from the start of a given string.
        /// </summary>
        /// <param name="matchValue">The string to trim</param>
        /// <returns>The trimmed string</returns>
        private string TrimFunctionFromFieldName(string matchValue)
        {
            if (matchValue.StartsWith("AVG of ", "MAX of ", "MIN of ", "SUM of "))
            {
                return matchValue.Substring(7);
            }

            if (matchValue.StartsWith("COUNT of "))
            {
                return matchValue.Substring(9);
            }

            return matchValue;
        }


        /// <summary>
        /// Seed the private <see cref="SortedList{TKey,TValue}"/> of Column Ordinal and Column name
        /// </summary>
        /// <param name="columns">An instance of <see cref="DataColumnCollection"/>used to collect the column ordinals</param>
        /// <param name="reportColumns">An instance of <see cref="ArrayList"/> containing a list of <seealso cref="cReportColumn"/>Which is used to generate the column names</param>
        /// <returns>The sorte list of ordinal and name</returns>
        private List<ColumnLookup> GetColumnOrdinals(DataColumnCollection columns, ArrayList reportColumns)
        {
            var columnLookup = new List<ColumnLookup>();
            if (columns.Count != reportColumns.Count)
            {
                return columnLookup;
            }
            foreach (DataColumn dataColumn in columns)
            {
                var reportColumn = (cReportColumn)reportColumns[dataColumn.Ordinal];
                switch (reportColumn.columntype)
                {
                    case ReportColumnType.Standard:
                        var standardColumn = (cStandardColumn)reportColumn;
                        columnLookup.Add(new ColumnLookup(string.IsNullOrEmpty(standardColumn.DisplayName) ? standardColumn.field.Description : standardColumn.DisplayName, dataColumn, standardColumn));
                        break;
                    case ReportColumnType.Static:
                        var staticColumn = (cStaticColumn)reportColumn;
                        columnLookup.Add(new ColumnLookup(staticColumn.literalname, dataColumn, staticColumn));
                        break;
                    case ReportColumnType.Calculated:
                        var calculatedColumn = (cCalculatedColumn)reportColumn;
                        columnLookup.Add(new ColumnLookup(calculatedColumn.columnname, dataColumn, calculatedColumn));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return columnLookup;
        }

        /// <summary>
        /// Determine the columnn type from the data in the collection.
        /// </summary>
        /// <returns>The <see cref="Type"/>of the data held.</returns>
        public Type GetColumnType(List<object[]> tableData, int columnIndex)
        {
            Type type = null;
            ITypeSelector typeSelector = null;
            var foundError = false;
            foreach (object[] row in tableData)
            {
                var columnValue = row[columnIndex];
                if (columnValue == null || columnValue.ToString().StartsWith("Calculated column failed to export"))
                {
                    foundError = true;
                    continue;
                }

                typeSelector = TypeSelectorFactory.New(columnValue);

                if (typeSelector.Type != type)
                {
                    if (type != null && !columnValue.ToString().StartsWith("Calculated column failed to export")) // Got multiple types
                    {
                        return typeof(string);
                    }

                    type = typeSelector.Type;
                }
            }

            if (foundError && typeSelector != null)
            {
                foreach (object[] row in tableData)
                {
                    var columnValue = row[columnIndex];
                    if (columnValue == null || columnValue.ToString().StartsWith("Calculated column failed to export"))
                    {
                        row[columnIndex] = typeSelector.DefaultValue;
                    }
                }
            }

            return type == null ? typeof(string) : type;
        }

        
    }
}
