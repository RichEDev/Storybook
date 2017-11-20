using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITester
{
    using System.Data;
    using System.Data.SqlClient;

    public class Database
    {
        private SqlConnection connection;

        public Database(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
            this.connection.Open();
        }

        public DataSet Select(string sql)
        {
            var selectCommand = new SqlCommand(sql, this.connection);
            DataSet dataSet = null;
            using (var sqlAdapter = new SqlDataAdapter())
            {
                dataSet = new DataSet();
                sqlAdapter.SelectCommand = selectCommand;
                sqlAdapter.Fill(dataSet);
            }

            return dataSet;
        }

        public int Update(string sql)
        {
            var sqlexecute = new SqlCommand(sql, this.connection)
                                 {
                                     Connection = this.connection,
                                     CommandType = CommandType.Text,
                                     CommandText = sql
                                 };
            return sqlexecute.ExecuteNonQuery();
        }
    }
}
