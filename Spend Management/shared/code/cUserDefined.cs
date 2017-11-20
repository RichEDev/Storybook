using System;
using System.Collections.Generic;
using System.Collections;
using SpendManagementLibrary;
using Spend_Management;
namespace expenses
{
    /// <summary>
    /// Summary description for cUserDefined.
    /// </summary>
    public class cUserDefined
    {
        DBConnection expdata;
        string strsql;
        int accountid = 0;

        private System.Collections.SortedList list;
        private System.Collections.SortedList fieldDefinitions;

        public cUserDefined(int nAccountid)
        {
            accountid = nAccountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        }

       

        public int count
        {
            get { return list.Count; }
        }

        public cUserDefinedField getByIndex(int index)
        {
            return (cUserDefinedField)list.GetByIndex(index);
        }

        

        public cField getUserDefinedDefinition(int fieldid)
        {
            return (cField)fieldDefinitions[fieldid];
        }

        public cUserDefinedField getUserDefinedById(int userdefineid)
        {
            return (cUserDefinedField)list[userdefineid];
        }

        
        public void tickUserDefined(ref System.Web.UI.WebControls.CheckBoxList templist, int subcatid)
        {
            System.Data.SqlClient.SqlDataReader udreader;
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            strsql = "select userdefineid from [subcats_userdefined] where subcatid = @subcatid";
            using (udreader = expdata.GetReader(strsql))
            {
                while (udreader.Read())
                {
                    templist.Items.FindByValue(udreader.GetInt32(0).ToString()).Selected = true;
                }
                udreader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();
        }

        
        public System.Data.DataSet getSpecificUserDefined(int subcatid)
        {

            System.Data.DataSet rcdsttemp = new System.Data.DataSet();
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            
            if (subcatid == 0)
            {
                strsql = "select userdefined.label, userdefined.userdefineid, null as subuserdefineid from userdefined where specific = 1";
            }
            else
            {
                strsql = "select userdefined.label, [subcats_userdefined].userdefineid, [subcats_userdefined].subuserdefineid from [subcats_userdefined] inner join userdefined on userdefined.userdefineid = [subcats_userdefined].userdefineid where subcatid = @subcatid" +
                    " union " +
                    "select userdefined.label, userdefined.userdefineid, null as subuserdefineid from userdefined where specific = 1 and userdefineid not in (select [subcats_userdefined].userdefineid from [subcats_userdefined] inner join userdefined on userdefined.userdefineid = [subcats_userdefined].userdefineid where subcatid = @subcatid)";
            }
            rcdsttemp = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return rcdsttemp;
        }

    }

    
    
}
