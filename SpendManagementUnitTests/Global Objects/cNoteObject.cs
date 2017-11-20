using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cNoteObject
    {

        /// <summary>
        /// Create the notes global object which is a DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable CreateNote()
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));

            string note = "This is a test note for unit testing";

            string strsql = "insert into notes (employeeid, note) values (@employeeID, @note)";
            db.sqlexecute.Parameters.AddWithValue("@employeeID", cGlobalVariables.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@note", note);
            db.sqlexecute.Parameters.Add("@returnID", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@returnID"].Direction = System.Data.ParameterDirection.ReturnValue;
			
			db.ExecuteSQL(strsql);

            int NoteID = (int)db.sqlexecute.Parameters["@returnID"].Value;

            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);

            DataTable notes = clsEmps.getNotes(NoteID);
            cGlobalVariables.NoteID = NoteID;

			db.sqlexecute.Parameters.Clear();

            return notes; 
        }

        /// <summary>
        /// Delete the note from the database
        /// </summary>
        public static void DeleteNote()
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "DELETE FROM notes WHERE noteid = @noteID";
            db.sqlexecute.Parameters.AddWithValue("@noteID", cGlobalVariables.NoteID);
            db.ExecuteSQL(strsql);
            db.sqlexecute.Parameters.Clear();
        }
    }
}
