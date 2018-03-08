namespace ManagementAPI.Repositories.InfoMessage
{

    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using ManagementAPI.Models;

    public class InfoMessageRepository : IInfoMessageRepository
    {
        /// <summary>
        /// Returns a single information message by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InformationMessage Get(int id)
        {
            var infoMessage = new InformationMessage();

            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.GetInfoMessage", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@informationId", id);

                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        infoMessage.InformationId = id;
                        infoMessage = BuildInfoMessageFromReader(infoMessage, reader);
                    }
            }
            return infoMessage;
        }

        /// <summary>
        /// Returns a list of all information messages.
        /// </summary>
        /// <returns></returns>
        public List<InformationMessage> GetAll()
        {
            var infoMessages = new List<InformationMessage>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.GetInfoMessages", connection) { CommandType = CommandType.StoredProcedure })
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var infoMessage = new InformationMessage
                        {
                            InformationId = reader.GetInt32(reader.GetOrdinal("informationID"))
                        };

                        infoMessage = BuildInfoMessageFromReader(infoMessage, reader);
                        infoMessages.Add(infoMessage);
                    }
                }
            }

            return infoMessages;
        }

        /// <summary>
        /// Saves an information message to the database.
        /// </summary>
        /// <param name="infoMessage"></param>
        /// <returns></returns>
        public bool Save(InformationMessage infoMessage)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.SaveInfoMessage", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@action", infoMessage.InformationId == 0 ? "Add Information Message" : "Edit Information Message");
                    command.Parameters.AddWithValue("@InformationId", infoMessage.InformationId);
                    command.Parameters.AddWithValue("@Title", infoMessage.Title);
                    command.Parameters.AddWithValue("@Message", infoMessage.Message);
                    command.Parameters.AddWithValue("@MobileInformationMessage", infoMessage.MobileMessage);
                    command.Parameters.AddWithValue("@AdministratorId", infoMessage.AdministratorId);
                    command.Parameters.AddWithValue("@DateAdded", infoMessage.DateAdded);
                    command.Parameters.AddWithValue("@DisplayOrder", infoMessage.DisplayOrder);
                    command.Parameters.AddWithValue("@Deleted", infoMessage.Deleted);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Delete an information message from the database by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.DeleteInfoMessage", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@informationId", id);

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Populates the properties of an information message using an initialised data reader.
        /// </summary>
        /// <param name="infoMessage">The empty information message object to be populated</param>
        /// <param name="reader">The SQL Server database reader</param>
        /// <returns></returns>
        private static InformationMessage BuildInfoMessageFromReader(InformationMessage infoMessage, SqlDataReader reader)
        {
            infoMessage.InformationId = reader.GetInt32(reader.GetOrdinal("informationID"));
            infoMessage.Title = reader.GetString(reader.GetOrdinal("title"));
            infoMessage.Message = reader.GetString(reader.GetOrdinal("message"));
            infoMessage.MobileMessage = reader.IsDBNull(reader.GetOrdinal("mobileInformationMessage")) ? null : reader.GetString(reader.GetOrdinal("mobileInformationMessage"));
            infoMessage.AdministratorId = reader.GetInt32(reader.GetOrdinal("administratorID"));
            infoMessage.DateAdded = reader.GetDateTime(reader.GetOrdinal("dateAdded"));
            if (reader.IsDBNull(reader.GetOrdinal("displayOrder")))
            {
                infoMessage.DisplayOrder = 0;
            }
            else
            {
                infoMessage.DisplayOrder = reader.GetInt32(reader.GetOrdinal("displayOrder"));
            }
            infoMessage.Deleted = reader.GetBoolean(reader.GetOrdinal("deleted"));

            return infoMessage;
        }
    }
}