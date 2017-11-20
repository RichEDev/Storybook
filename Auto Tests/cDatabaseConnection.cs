using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

using System.Collections.Generic;
using System.Diagnostics;
/// <summary>
/// Summary description for DBConnection.
/// </summary>
///


public class cDatabaseConnection
{
    private Database db = Database.Live;

    public System.Data.SqlClient.SqlCommand sqlexecute = new System.Data.SqlClient.SqlCommand();


    public SqlCommand command = new SqlCommand();

    //string sConnectionString;

    private string server;
    private string database;
    private string userId;
    private string password;




    #region properties
    //public string connectionstring
    //{
    //    get { return sConnectionString; }
    //}




    #endregion

    public cDatabaseConnection(string connectionstring)
    {
        //sConnectionString = connectionstring;






        string[] temp = connectionstring.Split(';');
        server = temp[0].Replace("Data Source=", "");
        database = temp[1].Replace("Initial Catalog=", "");
        userId = temp[3].Replace("User ID=", "");
        password = temp[4].Replace("Password=", "");



















    }

    public void changeDB(Database newdb)
    {
        db = newdb;
    }


    /// <summary>
    /// Read data from a stored procedure.
    /// </summary>
    /// <param name="strSQL">Name of the stored procedure.</param>
    /// <returns></returns>
    public SqlDataReader GetStoredProcReader(string strSQL)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
        System.Data.SqlClient.SqlDataReader sqlreader;
        //strsql = "use " + database + ";" + strsql;

        dataconnection.Open();
        dataconnection.ChangeDatabase(database);


        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strSQL;
        sqlexecute.CommandType = System.Data.CommandType.StoredProcedure;
        sqlreader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);

        return sqlreader;
    }

    //TODO : Remove method due to redundant
    public void ExecuteSQL(string strsql)
    {
        //strsql = "use " + database + ";" + strsql;
        using (System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString()))
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);


            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandType = System.Data.CommandType.Text;
            sqlexecute.CommandText = strsql;
            sqlexecute.ExecuteNonQuery();
        }

    }

    public int ExecuteSQL2(string strsql)
    {
        int result = -1;
        using (System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString()))
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);


            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandType = System.Data.CommandType.Text;
            sqlexecute.CommandText = strsql;
            result = sqlexecute.ExecuteNonQuery();
        }
        return result;

    }

    public void ExecuteProc(string strsql)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;

        try
        {

            dataconnection.Open();
            dataconnection.ChangeDatabase(database);


            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandType = System.Data.CommandType.StoredProcedure;
            sqlexecute.CommandText = strsql;
            sqlexecute.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("Application", ex.Message + "\n\n" + ex.StackTrace);
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            dataconnection.Close();
        }
    }


    public System.Data.DataSet GetDataSet(string strsql)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
        DataSet TempDataSet = null;
        //strsql = "use " + database + ";" + strsql;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);

            System.Data.SqlClient.SqlDataAdapter sqladapter = new System.Data.SqlClient.SqlDataAdapter();

            sqlexecute.Connection = dataconnection;
            TempDataSet = new System.Data.DataSet();
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = System.Data.CommandType.Text;
            sqladapter.SelectCommand = sqlexecute;

            sqladapter.Fill(TempDataSet);

        }
        catch (Exception)
        {

        }
        finally
        {
            dataconnection.Close();
        }
        return TempDataSet;

    }

    public System.Data.DataSet GetProcDataSet(string strsql)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
        System.Data.DataSet TempDataSet;
        //strsql = "use " + database + ";" + strsql;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);

            System.Data.SqlClient.SqlDataAdapter sqladapter = new System.Data.SqlClient.SqlDataAdapter();

            sqlexecute.Connection = dataconnection;
            TempDataSet = new System.Data.DataSet();
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = System.Data.CommandType.StoredProcedure;
            sqladapter.SelectCommand = sqlexecute;

            sqladapter.Fill(TempDataSet);
        }
        finally
        {
            dataconnection.Close();
        }
        return TempDataSet;

    }

    public void GetHierDataSet(string strsql, string dsname, ref System.Data.DataSet TempDataSet)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);

            System.Data.SqlClient.SqlDataAdapter sqladapter = new System.Data.SqlClient.SqlDataAdapter();

            sqlexecute.Connection = dataconnection;

            sqlexecute.CommandText = strsql;
            sqladapter.SelectCommand = sqlexecute;
            sqladapter.Fill(TempDataSet, dsname);
        }
        finally
        {
            dataconnection.Close();
        }



    }

    #region new methods
    //public SqlDataReader getReader(string sql)
    //{
    //    System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
    //    System.Data.SqlClient.SqlDataReader sqlreader;
    //    //sql = "use " + database + ";" + sql;

    //    dataconnection.Open();
    //    dataconnection.ChangeDatabase(database);
    //    command.Connection = dataconnection;
    //    command.CommandText = sql;
    //    command.CommandType = System.Data.CommandType.StoredProcedure;

    //    sqlreader = command.ExecuteReader(CommandBehavior.CloseConnection);
    //    command.Parameters.Clear();

    //    return sqlreader;
    //}

    /// <summary>
    /// Interim function to allow call from framework VB routines because won't expose the others as doesn't recognise case sensitivity. Says the get and Get routines are ambiguous
    /// </summary>
    /// <param name="sql">SQL to execute reader for</param>
    /// <returns></returns>
    [Obsolete("GetReader is now not ambiguous", true)]
    public SqlDataReader getDataReader(string sql)
    {
        return GetReader(sql);
    }

    /// <summary>
    /// Add an NVarChar to the sqlexecute.Parameters collection
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="value">Value of the paramter</param>
    /// <param name="size">The length of the column</param>
    public void AddWithValue(string name, object value, int size)
    {
        sqlexecute.Parameters.Add(name, SqlDbType.NVarChar, size);
        sqlexecute.Parameters[name].Value = value;
    }

    /// <summary>
    /// Add a decimal parameter to the sqlexecute.Parameters collection
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="value">Value of the paramter</param>
    /// <param name="precision">Precision of the decimal</param>
    /// <param name="scale">Scale of the decimal</param>
    public void AddWithValue(string name, decimal value, byte precision, byte scale)
    {
        sqlexecute.Parameters.Add(name, SqlDbType.Decimal);
        sqlexecute.Parameters[name].Value = value;
        sqlexecute.Parameters[name].Precision = precision;
        sqlexecute.Parameters[name].Scale = scale;
    }

    public void addParameter(string name, object value)
    {
        command.Parameters.AddWithValue(name, value);
    }

    public int executeSaveCommand(string sql)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
        int id = 0;
        //sql = "use " + database + ";" + sql;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);


            command.Parameters.Add("@id", System.Data.SqlDbType.Int);
            command.Parameters["@id"].Direction = System.Data.ParameterDirection.Output;

            command.Connection = dataconnection;
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.ExecuteNonQuery();

            id = (int)command.Parameters["@id"].Value;
            command.Parameters.Clear();
        }
        finally
        {
            dataconnection.Close();
        }
        return id;
    }
    public void executeCommand(string sql)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //sql = "use " + database + ";" + sql;
        try
        {
            dataconnection.Open();


            command.Connection = dataconnection;
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }
        finally
        {
            dataconnection.Close();
        }
    }
    #endregion

    public System.Data.SqlClient.SqlDataReader GetReader(string strsql)
    {


        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
        System.Data.SqlClient.SqlDataReader sqlreader;
        //strsql = "use " + database + ";" + strsql;

        dataconnection.Open();
        dataconnection.ChangeDatabase(database);


        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        sqlexecute.CommandType = System.Data.CommandType.Text;
        sqlreader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);

        return sqlreader;
    }

    public System.Data.SqlClient.SqlDataReader GetProcReader(string strsql)
    {

        System.Data.SqlClient.SqlDataReader sqlreader;
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;

        dataconnection.Open();
        System.Data.SqlClient.SqlCommand sqlexecute = new System.Data.SqlClient.SqlCommand();

        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        sqlexecute.CommandType = System.Data.CommandType.StoredProcedure;

        sqlreader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);

        return sqlreader;
    }

    public int getcount(string strsql)
    {
        int count;
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);


            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = System.Data.CommandType.Text;
            count = (int)sqlexecute.ExecuteScalar();
        }
        finally
        {
            dataconnection.Close();
        }
        return count;


    }

    public byte getTinyValue(string strsql)
    {
        byte bValue;
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);
            System.Data.SqlClient.SqlCommand sqlexecute = new System.Data.SqlClient.SqlCommand();

            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = strsql;
            bValue = (byte)sqlexecute.ExecuteScalar();
        }
        finally
        {
            dataconnection.Close();
        }

        return bValue;
    }

    public string getStringValue(string strsql)
    {
        System.Data.SqlClient.SqlDataReader reader;
        string bValue = "";
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;
        dataconnection.Open();
        dataconnection.ChangeDatabase(database);


        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        reader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                bValue = reader.GetString(0);
            }
        }


        reader.Close();

        return bValue;
    }

    public Guid getGuidValue(string strsql)
    {
        System.Data.SqlClient.SqlDataReader reader;
        Guid bValue = Guid.Empty;
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;
        dataconnection.Open();
        dataconnection.ChangeDatabase(database);


        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        reader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                bValue = reader.GetGuid(0);
            }
        }


        reader.Close();

        return bValue;
    }

    public List<Guid> getGuidList(string strsql)
    {
        System.Data.SqlClient.SqlDataReader reader;
        List<Guid> bValue = new List<Guid>();
        Guid Value = Guid.Empty;
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        //strsql = "use " + database + ";" + strsql;
        dataconnection.Open();
        dataconnection.ChangeDatabase(database);

        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        reader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                Value = reader.GetGuid(0);
                bValue.Add(Value);
            }
        }
        reader.Close();

        return bValue;
    }

    public List<string> getStringList(string strsql)
    {
        SqlDataReader reader;
        List<String> sValues = new List<string>();
        String Value = string.Empty;
        SqlConnection dataconnection = new SqlConnection(getConnectionString());
        dataconnection.Open();
        dataconnection.ChangeDatabase(database);

        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        reader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                Value = reader.GetString(0);
                sValues.Add(Value);
            }
        }
        return sValues;
    }


    public decimal getSum(string strsql)
    {
        System.Data.SqlClient.SqlDataReader reader;

        decimal sum = 0;
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());

        reader = GetReader(strsql);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                sum = reader.GetDecimal(0);
            }
        }

        reader.Close();

        return sum;


    }
    public int getIntSum(string strsql)
    {
        System.Data.SqlClient.SqlDataReader reader;

        int sum = 0;




        reader = GetReader(strsql);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                sum = reader.GetInt32(0);
            }
        }

        reader.Close();


        return sum;


    }
    public int getMax(string strsql)
    {
        System.Data.SqlClient.SqlDataReader reader;
        int max = 0;
        reader = GetReader(strsql);
        while (reader.Read())
        {
            if (reader.IsDBNull(0) == false)
            {
                max = reader.GetInt32(0);
            }
        }
        reader.Close();


        return max;
    }

    private string getConnectionString()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.Pooling = true;
        builder.MinPoolSize = 5;
        builder.MaxPoolSize = 1000;
        builder.InitialCatalog = database;
        builder.DataSource = server;
        builder.UserID = userId;
        builder.Password = password;
        return builder.ConnectionString;

    }

    public byte[] getImageData(string sql)
    {
        System.Data.SqlClient.SqlConnection dataconnection = new System.Data.SqlClient.SqlConnection(getConnectionString());
        byte[] retImage = null;

        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(database);

            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = sql;
            sqlexecute.CommandType = CommandType.Text;

            retImage = (byte[])sqlexecute.ExecuteScalar();
            sqlexecute.Parameters.Clear();
        }
        finally
        {
            dataconnection.Close();
        }

        return retImage;
    }


}

public enum Database
{
    Live, DataStore, MetaBase
}


