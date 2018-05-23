using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Caching;

using BusinessLogic.Modules;

using SpendManagementLibrary;

/// <summary>
/// The Database connection.
/// </summary>
[Obsolete("Use DatabaseConnection instead")]
public class DBConnection
{
	private Database db = Database.Live;

    /// <summary>
    /// The SQL execute.
    /// </summary>
    public SqlCommand sqlexecute = new SqlCommand();


    [Obsolete]
	public SqlCommand command = new SqlCommand();

	private readonly string _server;
	private readonly string _database;
    private readonly string _sDbPassword;

	public DBConnection(string connectionstring)
	{
        if (string.IsNullOrEmpty(connectionstring))
        {
            throw new Exception("Empty connection string passed.");
        }

        cSecureData crypt = new cSecureData();
        
        if (connectionstring.Contains(ConfigurationManager.AppSettings["dbpassword"]))
        {
            connectionstring = connectionstring.Replace(ConfigurationManager.AppSettings["dbpassword"], crypt.Decrypt(ConfigurationManager.AppSettings["dbpassword"]));
        }

        if (connectionstring == null || connectionstring == "")
        {
            return;
        }

		string[] temp = connectionstring.Split(';');
		_server = temp[0].Replace("Data Source=", "");
		_database = temp[1].Replace("Initial Catalog=", "");

        string sEncryptedDbPassword = ConfigurationManager.AppSettings["dbpassword"] ?? string.Empty;
        _sDbPassword = crypt.Decrypt(sEncryptedDbPassword);

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
        SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
        dataconnection.Open();
        dataconnection.ChangeDatabase(_database);
        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strSQL;
        sqlexecute.CommandType = CommandType.StoredProcedure;
        SqlDataReader sqlreader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);
        return sqlreader;
    }

    public int ExecuteSQLWithAffectedRows(string strSQL)
    {
        int affectedRows = 0;
        using (SqlConnection dataconnection = new SqlConnection(this.GetConnectionString()))
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(_database);

            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandType = CommandType.Text;
            sqlexecute.CommandText = strSQL;
            affectedRows = sqlexecute.ExecuteNonQuery();
        }

        return affectedRows;
    }

	public void ExecuteSQL(string strsql)
	{
        ExecuteSQLWithAffectedRows(strsql);
	}

	public void ExecuteProc(string strsql)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);
			sqlexecute.Connection = dataconnection;
			sqlexecute.CommandType = CommandType.StoredProcedure;
			sqlexecute.CommandText = strsql;
			sqlexecute.ExecuteNonQuery();
		}
		catch(Exception ex)
		{
            EventLog.WriteEntry("Application", ex.Message + "\n\n" + ex.StackTrace);
			Debug.WriteLine(ex);
		}
		finally
		{
			dataconnection.Close();
		}
	}

	public DataSet GetDataSet(string strsql)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		DataSet tempDataSet = null;
        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(_database);
            SqlDataAdapter sqladapter = new SqlDataAdapter();
            sqlexecute.Connection = dataconnection;
            tempDataSet = new DataSet();
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = CommandType.Text;
            sqladapter.SelectCommand = sqlexecute;

            sqladapter.Fill(tempDataSet);
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("Application", ex.Message + "\n\n" + ex.StackTrace);
            Debug.WriteLine(ex);
        }
		finally
		{
			dataconnection.Close();
		}
		return tempDataSet;
	}

	public DataSet GetProcDataSet(string strsql)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		DataSet TempDataSet;
		
		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);

			SqlDataAdapter sqladapter = new SqlDataAdapter();

			sqlexecute.Connection = dataconnection;
			TempDataSet = new DataSet();
			sqlexecute.CommandText = strsql;
			sqlexecute.CommandType = CommandType.StoredProcedure;
			sqladapter.SelectCommand = sqlexecute;

			sqladapter.Fill(TempDataSet);
		}
		finally
		{
			dataconnection.Close();
		}
		return TempDataSet;
	}

	public void GetHierDataSet(string strsql, string dsname, ref DataSet TempDataSet)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());

		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);

			SqlDataAdapter sqladapter = new SqlDataAdapter();

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
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		int id = 0;
		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);
			command.Parameters.Add("@id", SqlDbType.Int);
			command.Parameters["@id"].Direction = ParameterDirection.Output;
			command.Connection = dataconnection;
			command.CommandText = sql;
			command.CommandType = CommandType.StoredProcedure;
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
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		try
		{
			dataconnection.Open();
			command.Connection = dataconnection;
			command.CommandText = sql;
			command.CommandType = CommandType.StoredProcedure;
			command.ExecuteNonQuery();
			command.Parameters.Clear();
		}
		finally
		{
			dataconnection.Close();
		}
	}
	#endregion

    public SqlDataReader GetReader(string strsql)
    {
        SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
        SqlDataReader sqlreader = null;

        dataconnection.Open();
        dataconnection.ChangeDatabase(_database);

        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        sqlexecute.CommandType = CommandType.Text;
        sqlreader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);

        return sqlreader;
    }

    public SqlDataReader GetProcReader(string strsql)
    {
        SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
        SqlDataReader sqlreader = null;

        dataconnection.Open();
        dataconnection.ChangeDatabase(_database);

        sqlexecute.Connection = dataconnection;
        sqlexecute.CommandText = strsql;
        sqlexecute.CommandType = CommandType.StoredProcedure;

        sqlreader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection);

        return sqlreader;
    }

    public int getcount(string strsql, CommandType commandType = CommandType.Text)
	{
		int count;
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());

		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);
            
			sqlexecute.Connection = dataconnection;
			sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = commandType;
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
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());

		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);
		    
            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = CommandType.Text;

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
		string bValue = "";
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());

        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(_database);

            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = CommandType.Text;

            using (SqlDataReader reader = sqlexecute.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        bValue = reader.GetString(0);
                    }
                }
                reader.Close();
            }
        }
        finally
        {
            dataconnection.Close();
        }

	    return bValue;
	}

	public decimal getSum(string strsql)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
        decimal sum = 0;

        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(_database);

            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = CommandType.Text;

            SqlDataReader reader = GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    sum = reader.GetDecimal(0);
                }
            }
            reader.Close();
        }
        finally
        {
            dataconnection.Close();
        }
        
		return sum;
	}

    public int getIntSum(string strsql)
    {
        SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
        int sum = 0;

        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(_database);

            sqlexecute.Connection = dataconnection;
            sqlexecute.CommandText = strsql;
            sqlexecute.CommandType = CommandType.Text;

            SqlDataReader reader = GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    sum = reader.GetInt32(0);
                }
            }
            reader.Close();
        }
        finally
        {
            dataconnection.Close();
        }

        return sum;
    }

    /// <summary>
    /// Returns SqlDependency
    /// </summary>
    /// <param name="strsql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public SqlDependency CreateSQLCacheDependency(Dictionary<string, object> parameters, string strsql)
    {
        SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
        SqlDependency dep = null;

        try
        {
            dataconnection.Open();
            dataconnection.ChangeDatabase(_database);
            SqlCommand comm = new SqlCommand(strsql) {CommandType = CommandType.Text, Connection = dataconnection};

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> i in parameters)
                {
                    comm.Parameters.AddWithValue(i.Key, i.Value);
                }
            }
            dep = new SqlDependency(comm);
            comm.ExecuteNonQuery();
        }
        finally
        {
            dataconnection.Close();
        }
        return dep;
    }

    /// <summary>
    /// Returns System.Web.Cache.SqlCacheDependency
    /// </summary>
    /// <param name="strsql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
	public SqlCacheDependency CreateSQLCacheDependency(string strsql, SortedList<string, object> parameters)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		SqlCacheDependency dep = null;

		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);
		    SqlCommand comm = new SqlCommand(strsql) {CommandType = CommandType.Text, Connection = dataconnection};

		    if (parameters != null)
			{
				foreach (KeyValuePair<string, object> i in parameters)
				{
					comm.Parameters.AddWithValue(i.Key, i.Value);
				}
			}
			dep = new SqlCacheDependency(comm);
			comm.ExecuteNonQuery();
		}
		finally
		{
			dataconnection.Close();
		}

		return dep;
        
	}

    /// <summary>
    /// The get connection string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string GetConnectionString()
	{
		var builder = new SqlConnectionStringBuilder
		    {
		        Pooling = true,
		        MinPoolSize = 5,
		        MaxPoolSize = 1000,
		        InitialCatalog = "expenses_temp",
		        DataSource = this._server,
                Password = this._sDbPassword,
                ApplicationName = GlobalVariables.DefaultApplicationInstanceName
		    };

        string user = "spenduser";
        if (ConfigurationManager.AppSettings["active_module"] != null && (Modules)int.Parse(ConfigurationManager.AppSettings["active_module"]) == Modules.ESR)
        {
            // we know we are connecting from a service, as this setting is now removed from web configs
            user = "esruser";
        }

        builder.UserID = user;
        builder.Password = this._sDbPassword;
        return builder.ConnectionString;
	}

	public byte[] getImageData(string sql)
	{
		SqlConnection dataconnection = new SqlConnection(this.GetConnectionString());
		byte[] retImage = null;

		try
		{
			dataconnection.Open();
			dataconnection.ChangeDatabase(_database);

			sqlexecute.Connection = dataconnection;
			sqlexecute.CommandText = sql;
			sqlexecute.CommandType = CommandType.Text;

            System.Data.SqlTypes.SqlBinary binData = null;
            using (SqlDataReader reader = sqlexecute.ExecuteReader())
            {
                while (reader.Read())
                {
                    binData = reader.GetSqlBinary(0);
                }
                reader.Close();
            }

            retImage = binData.Value;
			sqlexecute.Parameters.Clear();
		}
		finally
		{
			dataconnection.Close();
		}

		return retImage;
	}

    public T ExecuteScalar<T>(string sql,CommandType commandType = CommandType.Text)
    {
        T count;
        using (var dataconnection = new SqlConnection(this.GetConnectionString()))
        {
            try
            {
                dataconnection.Open();
                dataconnection.ChangeDatabase(this._database);

                this.sqlexecute.Connection = dataconnection;
                this.sqlexecute.CommandText = sql;
                this.sqlexecute.CommandType = commandType;
                count = (T)this.sqlexecute.ExecuteScalar();
            }
            finally
            {
                if (this.sqlexecute.Connection.State != ConnectionState.Closed)
                {
                    dataconnection.Close();
                }
            }
        }

        return count;
    }
}

public enum Database
{
    Live, DataStore, MetaBase
}

