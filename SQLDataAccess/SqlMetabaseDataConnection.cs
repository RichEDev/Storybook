using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataAccess
{
    using System.Data.SqlClient;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Logging;

    public class SqlMetabaseDataConnection : SqlDataConnection
    {
        public SqlMetabaseDataConnection(ILogger logger, DataParameters dataParameters, SqlConnectionStringBuilder connectionStringBuilder)
            : base(logger, dataParameters, connectionStringBuilder)
        {
        }
    }
}
