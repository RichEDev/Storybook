using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Spend_Management;
using System.Configuration;
using System.Data.SqlClient;
using SpendManagementLibrary;

namespace expenses
{
    public partial class migratecardocuments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void cmdmigrate_Click(object sender, EventArgs e)
        {
            int accountid = Convert.ToInt32(txtaccountid.Text);
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            DBConnection metadata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            string path = Server.MapPath("car_documents");
            string strsql;
            SqlDataReader reader;

            string filename;
            string strfile;
            string[] arrfile;
            string title;
            string extension;
            string parenttable;
            string parentfield;
            byte[] file;
            byte documenttype;
            int? mimeid;
            int id;
            string tablename, attachmenttable;
            string key;
            byte input;
            int bytecount;
            int attachmentid;
            int mainadmin = 0;
            strsql = "select stringvalue from accountproperties where stringkey = 'mainadministrator'";
            reader = data.GetReader(strsql);
            while (reader.Read())
            {
                int.TryParse(reader.GetString(0), out mainadmin);
            }
            reader.Close();

            strsql = "select employeeid, carid, documenttype, filename from car_documents";
            reader = data.GetReader(strsql);
            while (reader.Read())
            {
                documenttype = reader.GetByte(reader.GetOrdinal("documenttype"));
                filename = Server.MapPath(reader.GetString(reader.GetOrdinal("filename")));
                if (File.Exists(filename))
                {
                    bytecount = 0;
                    FileStream f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    strfile = filename.Replace(path + "\\", "");
                    strfile = strfile.Remove(strfile.Length - 4, 4);
                    arrfile = strfile.Split('_');
                    extension = filename.Substring(filename.Length - 3, 3);
                    // Create an instance of BinaryReader that can 
                    // read bytes from the FileStream.
                    BinaryReader sr = new BinaryReader(f);
                    file = new byte[sr.BaseStream.Length];
                    // While not at the end of the file, read lines from the file.
                    while (bytecount < sr.BaseStream.Length)
                    {
                        input = sr.ReadByte();
                        file[bytecount] = input;
                        bytecount++;
                    }
                    sr.Close();
                    f.Close();
                    tablename = "cars_attachments";
                    attachmenttable = "cars_attachmentData";
                    parenttable = "cars";
                    key = "carid";
                    if (reader.IsDBNull(reader.GetOrdinal("carid")))
                    {
                        id = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    }
                    else
                    {
                        id = reader.GetInt32(reader.GetOrdinal("carid")); ;
                    }
                    title = "";
                    parentfield = "";
                    switch (documenttype)
                    {
                        case 1:
                            title = "Driving Licence Attachment";
                            tablename = "employee_attachments";
                            attachmenttable = "employee_attachmentData";
                            parenttable = "employees";
                            parentfield = "licenceAttachID";
                            key = "employeeid";
                            break;
                        case 2:
                            title = "Tax Attachment";
                            parentfield = "taxAttachID";
                            break;
                        case 3:
                            title = "MOT Attachment";
                            parentfield = "MOTAttachID";
                            break;
                        case 4:
                            title = "Insurance Attachment";
                            parentfield = "insuranceAttachID";
                            break;
                        case 5:
                            title = "Service Attachment";
                            parentfield = "serviceAttachID";
                            break;
                    }
                    mimeid = null;
                    strsql = "select mimeID from mime_headers where fileextension = @header";
                    metadata.sqlexecute.Parameters.AddWithValue("@header", extension);
                    reader = metadata.GetReader(strsql);
                    metadata.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        mimeid = reader.GetInt32(0);
                    }
                    reader.Close();
                    try
                    {
                        strsql = "insert into " + attachmenttable + " (fileData) values (@filedata);select @identity = scope_identity();";
                        data.sqlexecute.Parameters.AddWithValue("@filedata", file);
                        data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
                        data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
                        data.ExecuteSQL(strsql);
                        attachmentid = (int)data.sqlexecute.Parameters["@identity"].Value;
                        data.sqlexecute.Parameters.Clear();

                        strsql = "insert into " + tablename + " (attachmentid, id, title, filename, mimeID,createdon,createdby) values (@attachmentid, @id,@title,@filename,@mimeid,@createdon,@createdby)";
                        data.sqlexecute.Parameters.AddWithValue("@attachmentid", attachmentid);
                        data.sqlexecute.Parameters.AddWithValue("@id", id);
                        data.sqlexecute.Parameters.AddWithValue("@title", title);
                        data.sqlexecute.Parameters.AddWithValue("@filename", filename.Replace(path + "\\", ""));
                        data.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now);
                        data.sqlexecute.Parameters.AddWithValue("@createdby", mainadmin);
                        if (mimeid == null)
                        {
                            data.sqlexecute.Parameters.AddWithValue("@mimeid", DBNull.Value);
                        }
                        else
                        {
                            data.sqlexecute.Parameters.AddWithValue("@mimeid", mimeid);
                        }

                        data.ExecuteSQL(strsql);

                        data.sqlexecute.Parameters.Clear();

                        strsql = "update " + parenttable + " set " + parentfield + " = " + attachmentid + " where " + key + " = " + id;
                        data.ExecuteSQL(strsql);

                    }
                    catch (Exception ex)
                    {
                    }
                    //move the file
                    File.Move(filename, path + "\\migrated" + filename.Replace(path, ""));
                }
            }
        }
    }
}
