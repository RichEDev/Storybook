namespace UnitTest2012Ultimate.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using global::BusinessLogic.DataConnections;

    using Microsoft.SqlServer.TransactSql.ScriptDom;

    /// <summary>
    /// A static class to assist with parsing inline SQL queries
    /// </summary>
    public static class SqlParser
    {
        /// <summary>
        /// Gets the table names from a SQL query. 
        /// </summary>
        /// <param name="query">The SQL query</param>
        /// <returns>A list of SQL tables that are in the SQL Query</returns>
        public static List<string> GetTableNamesFromQueryString(string query)
        {
            var output = new List<string>();
            var sb = new StringBuilder();
            var parser = new TSql120Parser(true);

            var fromTokenTypes = new[]
                                     {
                                         TSqlTokenType.From,
                                         TSqlTokenType.Join
                                     };

            var identifierTokenTypes = new[]
                                           {
                                               TSqlTokenType.Identifier,
                                               TSqlTokenType.QuotedIdentifier
                                           };

            using (System.IO.TextReader textReader = new System.IO.StringReader(query))
            {
                IList<ParseError> errors;
                var queryTokens = parser.GetTokenStream(textReader, out errors);
                if (errors.Any())
                {
                    return errors
                        .Select(e => string.Format("Error: {0}; Line: {1}; Column: {2}; Offset: {3};  Message: {4};", e.Number, e.Line, e.Column, e.Offset, e.Message))
                        .ToList();
                }

                for (var i = 0; i < queryTokens.Count; i++)
                {
                    if (fromTokenTypes.Contains(queryTokens[i].TokenType))
                    {
                        for (var j = i + 1; j < queryTokens.Count; j++)
                        {
                            if (queryTokens[j].TokenType == TSqlTokenType.WhiteSpace)
                            {
                                continue;
                            }

                            if (identifierTokenTypes.Contains(queryTokens[j].TokenType))
                            {
                                sb.Clear();
                                GetQuotedIdentifier(queryTokens[j], sb);

                                while (j + 2 < queryTokens.Count
                                       && queryTokens[j + 1].TokenType == TSqlTokenType.Dot
                                       && (queryTokens[j + 2].TokenType == TSqlTokenType.Dot || identifierTokenTypes.Contains(queryTokens[j + 2].TokenType)))
                                {
                                    sb.Append(queryTokens[j + 1].Text);

                                    if (queryTokens[j + 2].TokenType == TSqlTokenType.Dot)
                                    {
                                        if (queryTokens[j - 1].TokenType == TSqlTokenType.Dot)
                                        {
                                            GetQuotedIdentifier(queryTokens[j + 1], sb);
                                        }

                                        j++;
                                    }
                                    else
                                    {
                                        GetQuotedIdentifier(queryTokens[j + 2], sb);
                                        j += 2;
                                    }
                                }

                                output.Add(sb.ToString());
                            }

                            break;
                        }
                    }
                }

                return output.Distinct().OrderBy(tableName => tableName).ToList();
            }
        }

        /// <summary>
        /// Gets the Where Clause from a intline SQL query
        /// </summary>
        /// <param name="query">The SQL Querty</param>
        /// <param name="parameters">The <see cref="DataParameters">DataParameters</see></param>
        /// <param name="dataTable">The datatable we are to apply the SQL query to</param>
        /// <returns>The generated Where clause</returns>
        public static string GetWhereClaseFromQueryString(string query, DataParameters parameters, DataTable dataTable)
        {
            var whereIndex = query.ToUpper().IndexOf(" WHERE ", StringComparison.Ordinal);

            if (whereIndex == -1)
            {
                return string.Empty;
            }

            string whereClause = query.Substring(whereIndex + 7);

            foreach (KeyValuePair<string, object> dataParameter in parameters)
            {
                var columnName = dataParameter.Key.Replace("@", string.Empty);
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    if (dataColumn.ColumnName.ToLower() == columnName.ToLower())
                    {
                        columnName = dataColumn.ColumnName;
                    }
                }

                if (dataParameter.Value is Guid || dataParameter.Value is string)
                {
                    whereClause = whereClause.Replace(dataParameter.Key, "'" + dataParameter.Value + "'");
                }
                else
                {
                    whereClause = whereClause.Replace(dataParameter.Key, dataParameter.Value.ToString());
                }
            }

            var tables = new Regex(@"\S+?\.{1}");

            whereClause = tables.Replace(whereClause, " ");
          
            return whereClause;
        }

        /// <summary>
        /// The get quoted identifier.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="sb">
        /// The stering builder. </param>
        /// <exception cref="ArgumentException">
        /// Must be a valid token
        /// </exception>
        private static void GetQuotedIdentifier(TSqlParserToken token, StringBuilder sb)
        {
            switch (token.TokenType)
            {
                case TSqlTokenType.Identifier:
                    sb.Append(token.Text);
                    break;
                case TSqlTokenType.QuotedIdentifier:
                case TSqlTokenType.Dot:
                    sb.Append(token.Text);
                    break;

                default: throw new ArgumentException("Error: expected TokenType of token should be TSqlTokenType.Dot, TSqlTokenType.Identifier, or TSqlTokenType.QuotedIdentifier");
            }
        }
    }
}