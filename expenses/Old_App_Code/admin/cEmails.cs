using System;
using System.Collections.Generic;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
	/// <summary>
	/// Summary description for cEmails.
	/// </summary>
	public class cEmails
	{
		string strsql = "";
		int accountid = 0;
		public System.Collections.SortedList list;

        public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
		 

        List<cExpenseItem> lstitems;
        private cClaim reqclaim;
        private cFloat reqfloat;
        #region properties
        public cClaim claim
        {
            get { return reqclaim; }
            set { reqclaim = value; }
        }
        public cFloat advance
        {
            get { return reqfloat; }
            set { reqfloat = value; }
        }
        public List<cExpenseItem> items
        {
            get { return lstitems; }
            set { lstitems = value; }
        }
        #endregion
        

		public cEmails(int nAccountid)
		{
			accountid = nAccountid;
            
			
			InitialiseData();
		}

		private void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["messages" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			

			
		}

		private System.Collections.SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cMessage reqmsg;
			System.Data.SqlClient.SqlDataReader reader;
			byte messageid, direction;
			bool send, sendnote;
			string subject, description, message, note;
			System.Collections.SortedList list = new System.Collections.SortedList();
			
			strsql = "select messageid, message, send, subject, description, direction, sendnote, note from dbo.messages";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				messageid = reader.GetByte(reader.GetOrdinal("messageid"));
				direction = reader.GetByte(reader.GetOrdinal("direction"));
				send = reader.GetBoolean(reader.GetOrdinal("send"));
				sendnote = reader.GetBoolean(reader.GetOrdinal("sendnote"));
				subject = reader.GetString(reader.GetOrdinal("subject"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
				if (reader.IsDBNull(reader.GetOrdinal("message")) == false)
				{
					message = reader.GetString(reader.GetOrdinal("message"));
				}
				else
				{
					message = "";
				}
				if (reader.IsDBNull(reader.GetOrdinal("note")) == false)
				{
					note = reader.GetString(reader.GetOrdinal("note"));
				}
				else
				{
					note = "";
				}
				reqmsg = new cMessage(messageid, send, subject, message, direction, sendnote, note,description);
				list.Add(messageid, reqmsg);
			}
			reader.Close();
			
			Cache.Insert("messages" + accountid,list,dep);

			return list;
		}

		public System.Data.DataSet getGrid()
		{
			cMessage reqmsg;
			System.Data.DataSet ds = new System.Data.DataSet();
			object[] values;
			int i;
			System.Data.DataTable tbl = new System.Data.DataTable();
			tbl.Columns.Add("messageid",System.Type.GetType("System.Byte"));
			tbl.Columns.Add("send",System.Type.GetType("System.Boolean"));
			tbl.Columns.Add("sendnote",System.Type.GetType("System.Boolean"));
			tbl.Columns.Add("subject",System.Type.GetType("System.String"));
			tbl.Columns.Add("description",System.Type.GetType("System.String"));
			
			for (i = 0; i < list.Count; i++)
			{
				reqmsg = (cMessage)list.GetByIndex(i);
				values = new object[5];
				values[0] = reqmsg.messageid;
				values[1] = reqmsg.send;
				values[2] = reqmsg.sendnote;
				values[3] = reqmsg.subject;
				values[4] = reqmsg.description;
				tbl.Rows.Add(values);
			}

			ds.Tables.Add(tbl);
			return ds;
		}

		public void updateServer(string server, byte sourceaddress)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@server",server);
			
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			strsql = "update [other] set server = @server, sourceaddress = @sourceaddress";
			expdata.sqlexecute.Parameters.AddWithValue("@sourceaddress",sourceaddress);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			

            cMisc clsmisc = new cMisc(accountid);
            clsmisc.InvalidateGlobalProperties(accountid);
		}
		public void updateMessage(byte messageid, bool send, string subject, string message, bool sendnote, string note)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cAuditLog clsaudit = new cAuditLog();
			cMessage reqmsg = getMessageById(messageid);
			expdata.sqlexecute.Parameters.AddWithValue("@send",Convert.ToByte(send));
			expdata.sqlexecute.Parameters.AddWithValue("@subject",subject);
			expdata.sqlexecute.Parameters.AddWithValue("@message",message);
			expdata.sqlexecute.Parameters.AddWithValue("@messageid",messageid);
			
			expdata.sqlexecute.Parameters.AddWithValue("@sendnote",Convert.ToByte(sendnote));
			expdata.sqlexecute.Parameters.AddWithValue("@note",note);
			strsql = "update messages set send = @send, subject = @subject, message = @message, sendnote = @sendnote, note = @note where messageid = @messageid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			

			#region auditlog
			if (reqmsg.send != send)
			{
				clsaudit.editRecord(messageid.ToString(),"Send","E-mails",reqmsg.send.ToString(),send.ToString());
			}
			if (reqmsg.subject != subject)
			{
				clsaudit.editRecord(messageid.ToString(),"Subject","E-mails",reqmsg.subject,subject);
			}
			if (reqmsg.message != message)
			{
				clsaudit.editRecord(messageid.ToString(),"Message","E-mails",reqmsg.message,message);
			}
			if (reqmsg.sendnote != sendnote)
			{
				clsaudit.editRecord(messageid.ToString(),"Send Note","E-mails",reqmsg.sendnote.ToString(),sendnote.ToString());
			}
			if (reqmsg.note != note)
			{
				clsaudit.editRecord(messageid.ToString(),"Note","E-mails",reqmsg.note,note);
			}
			#endregion
		}

		public void sendHint(string email, string hint)
		{
			string message = "";
			message = "The password hint you have saved is below. If this does not help please consult your Expenses Administrator.";
			message = message + "\n\n";
			message = message + hint;

			sendMail(email,email,"Expenses Password Reminder",message);

		}
		public cMessage getMessageById(byte messageid)
		{
			return (cMessage)list[messageid];
		}

		public void sendMessage(byte messageid, int senderid, int[] recipientid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			//System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			cMessage reqmsg;
			int i;
			string audit;
			bool send = false;
			bool sendnote;
			string subject = "";
			string message = "";
			string origmessage = "";
			string sender = "";
			string note;
            cAuditLog clsaudit;

            if (senderid == 0)
            {
                clsaudit = new cAuditLog();
            }
            else
            {
                clsaudit = new cAuditLog(accountid,senderid);
            }

			System.Collections.ArrayList recipients;
			
			reqmsg = getMessageById(messageid);
			
			send = reqmsg.send;
			sendnote = reqmsg.sendnote;
			subject = reqmsg.subject;
			origmessage = reqmsg.message;
			
			if (send == false && sendnote == false)
			{
				return;
			}
			sender = getSender(senderid);
			//if (sender == "")
			//{
			//	return;
			//}
			recipients = getRecipients(recipientid);
			
			if (send == true)
			{
				//loop through each recipient, prepare mail and send message
				for (i = 0; i < recipients.Count; i++)
				{
					if (recipients[i].ToString() != "")
					{
						message = prepareMessage((byte)messageid, origmessage, senderid, recipientid[i]);
						
                        //if (appinfo.User.Identity.Name != "")
                        if (senderid != 0)
						{
							audit = "E-mail Sent (Subject:" + subject +",Sender:" + sender + ", recipient:" + recipients[i] + ")";
							clsaudit.addRecord("E-mail Server",audit);
						}
						if (senderid == 0)
						{
							sender = recipients[i].ToString();
						}
						sendMail(sender, recipients[i].ToString(),subject,message);
					}
				}
			}

			if (sendnote == true) //insert send note msg
			{
				for (i = 0; i < recipientid.Length; i++)
				{
					note = prepareMessage(messageid,reqmsg.note,senderid,recipientid[i]);
					strsql = "insert into notes (employeeid, note) values (@employeeid, @note)";
					expdata.sqlexecute.Parameters.AddWithValue("@employeeid",recipientid[i]);
					expdata.sqlexecute.Parameters.AddWithValue("@note",note);
					expdata.ExecuteSQL(strsql);
					expdata.sqlexecute.Parameters.Clear();
				}
			}

		}

		private System.Collections.ArrayList getRecipients(int[] recipients)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			if (recipients.Length == 0)
			{
				return null;
			}


			int i = 0;
			System.Collections.ArrayList arrecipients = new System.Collections.ArrayList();
			System.Data.SqlClient.SqlDataReader recipreader;
			strsql = "select email from employees where ";
			for (i = 0; i < recipients.Length; i++)
			{
				strsql = strsql + "employeeid = @employeeid" + i + " or ";
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid" + i, recipients[i]);
			}
			strsql = strsql.Remove(strsql.Length-4,4);
			recipreader = expdata.GetReader(strsql);
			while (recipreader.Read())
			{
				if (recipreader.IsDBNull(0) == false)
				{
					if (recipreader.GetString(0) != "")
					{
						arrecipients.Add(recipreader.GetString(0));
					}
				}
			}
			recipreader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
			return arrecipients;
			

		}
		private string getSender(int senderid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			string sender;
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",senderid);
			strsql = "select email from employees where employeeid = @employeeid";
			sender = expdata.getStringValue(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return sender;
		}
		private string prepareMessage(byte messageid, string tempmsg, int senderid, int recipientid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int employeeid = 0;
			cEmployees clsemployees = new cEmployees(accountid);
			cEmployee sender;
			cEmployee recipient;
			cMessage reqmessage;
			int noitems = 0;
			string name = "";
			string description = "";
			int claimno = 0;
			decimal total = 0;
			System.Data.SqlClient.SqlDataReader claimreader;
			sender = clsemployees.GetEmployeeById(senderid);
			recipient = clsemployees.GetEmployeeById(recipientid);

			reqmessage = getMessageById(messageid);


			//replace variables
			switch (reqmessage.direction)
			{
				case 1:
					tempmsg = tempmsg.Replace("{user title}",sender.title);
					tempmsg = tempmsg.Replace("{user firstname}",sender.firstname);
					tempmsg = tempmsg.Replace("{user surname}", sender.surname);
					tempmsg = tempmsg.Replace("{user email}", sender.email);
					tempmsg = tempmsg.Replace("{username}",sender.username);
					tempmsg = tempmsg.Replace("{admin title}",recipient.title);
					tempmsg = tempmsg.Replace("{admin firstname}",recipient.firstname);
					tempmsg = tempmsg.Replace("{admin surname}",recipient.surname);
					tempmsg = tempmsg.Replace("{admin email}",recipient.email);
					employeeid = sender.employeeid;
					break;
				case 2:
					tempmsg = tempmsg.Replace("{user title}",recipient.title);
					tempmsg = tempmsg.Replace("{user firstname}",recipient.firstname);
					tempmsg = tempmsg.Replace("{user surname}", recipient.surname);
					tempmsg = tempmsg.Replace("{user email}", recipient.email);
					tempmsg = tempmsg.Replace("{username}",recipient.username);
					if (senderid == 0)
					{
						tempmsg = tempmsg.Replace("{admin title}","");
						tempmsg = tempmsg.Replace("{admin firstname}","");
						tempmsg = tempmsg.Replace("{admin surname}","");
						tempmsg = tempmsg.Replace("{admin email}","");
					}
					else
					{
						tempmsg = tempmsg.Replace("{admin title}",sender.title);
						tempmsg = tempmsg.Replace("{admin firstname}",sender.firstname);
						tempmsg = tempmsg.Replace("{admin surname}",sender.surname);
						tempmsg = tempmsg.Replace("{admin email}",sender.email);
					}
					employeeid = recipient.employeeid;
					break;
			}

			
            if (reqclaim != null)
            {
                tempmsg = tempmsg.Replace("{no of items}", reqclaim.noitems.ToString());
                tempmsg = tempmsg.Replace("{total due}", reqclaim.total.ToString("£###,##.00"));
                tempmsg = tempmsg.Replace("{Claim Name}", reqclaim.name);
                tempmsg = tempmsg.Replace("{Claim No}", reqclaim.claimno.ToString());
                tempmsg = tempmsg.Replace("{Claim Description}", reqclaim.description);
            }


            if (items != null && reqclaim != null)
            {
                tempmsg = tempmsg.Replace("{Item Details}", getItemDetails(reqclaim, items));
            }

			//advance details
            if (advance != null)
            {
                tempmsg = tempmsg.Replace("{advance details}", getAdvanceDetails(advance));
            }
		expdata.sqlexecute.Parameters.Clear();
			return tempmsg;
		}

		private string getAdvanceDetails (cFloat reqfloat)
		{
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			
			output.Append("Advance Name: " + reqfloat.name + "\n");
			output.Append("Advance Amount: " + reqfloat.floatamount.ToString("£###,###,##0.00") + "\n");
			output.Append("Advance issue in currency: ");
			if (reqfloat.currencyid != 0)
			{
				cCurrencies clscurrencies = new cCurrencies(accountid);
				cCurrency reqcur = clscurrencies.getCurrencyById(reqfloat.currencyid);
				output.Append(clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label);
			}
			else
			{
				output.Append("GBP");
			}
			output.Append("\nIssue Number: " + reqfloat.issuenum);

			return output.ToString();

		}
		private string getItemDetails(cClaim reqclaim, List<cExpenseItem> items)
		{
		
			int i;
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			cMisc clsmisc = new cMisc(accountid);
            cGlobalCountries clsglobalcountries = new cGlobalCountries();
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			cCompanies clscompanies = new cCompanies(accountid);
			cReasons clsreason = new cReasons(accountid);
			
			cSubcats clssubcats = new cSubcats(accountid);
			cCountries clscountries = new cCountries(accountid);
			cSubcat reqsubcat;
			cCurrencies clscurrencies = new cCurrencies(accountid);
			cAllowances clsallowances = new cAllowances(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
			output.Append("Please find details of the item(s) in question below:\n\n\n");
            i = 1;
			foreach (cExpenseItem reqitem in items)
			{
				
				
				reqsubcat = clssubcats.getSubcatById(reqitem.subcatid);

				

				if (items.Count > 1)
				{
					output.Append("Item " + (i+1) + "\n\n");
				}
				//general details
				output.Append("Date:" + reqitem.date.ToShortDateString() + "\n");

				if (reqitem.fromid != 0)
				{
					if (clscompanies.GetCompanyById(reqitem.fromid) != null)
					{
						output.Append(clsmisc.getGeneralFieldByCode("from").description + ": " + clscompanies.GetCompanyById(reqitem.fromid).company + "\n");
					}
				}
				if (reqitem.companyid != 0)
				{
                    output.Append(clsmisc.getGeneralFieldByCode("company").description + ": " + clscompanies.GetCompanyById(reqitem.companyid).company + "\n");
				}
				if (reqitem.reasonid != 0)
				{
                    output.Append(clsmisc.getGeneralFieldByCode("reason").description + ": " + clsreason.getReasonById(reqitem.reasonid).reason + "\n");
				}
				if (reqitem.countryid != 0)
				{
                    output.Append(clsmisc.getGeneralFieldByCode("country").description + ": " + clsglobalcountries.getGlobalCountryById(clscountries.getCountryById(reqitem.countryid).globalcountryid).country + "\n");
				}
				if (reqitem.currencyid != 0)
				{
                    output.Append(clsmisc.getGeneralFieldByCode("currency").description + ": " + clsglobalcurrencies.getGlobalCurrencyById(clscurrencies.getCurrencyById(reqitem.currencyid).globalcurrencyid).label + "\n");
				}
				if (reqitem.reason != "")
				{
                    output.Append(clsmisc.getGeneralFieldByCode("otherdetails").description + ": " + reqitem.reason + "\n");
				}

			
				//sub category specific
				output.Append("Expense Item: " + reqsubcat.subcat + "\n");
				if (reqsubcat.mileageapp == true)
				{
					output.Append("No Miles:" + reqitem.miles + "\n");
				}
				if (reqsubcat.passengersapp == true)
				{
					output.Append("No Passengers: " + reqitem.nopassengers + "\n");
				}

				if (reqsubcat.bmilesapp == true)
				{
					output.Append("No Miles (Business): " + reqitem.bmiles + "\n");
					
				}

				if (reqsubcat.pmilesapp == true)
				{
					output.Append("No Miles (Personal): " + reqitem.pmiles + "\n");
					
				}
					

				if (reqsubcat.staffapp == true)
				{
					output.Append("No of Staff: " + reqitem.staff + "\n");
					
				}
				if (reqsubcat.othersapp == true)
				{
					output.Append("No of Others: " + reqitem.others + "\n");
					
				}
				if (reqsubcat.attendeesapp == true)
				{
					output.Append("Attendees: " + reqitem.attendees + "\n");
					
						
				}
				if (reqsubcat.nonightsapp == true)
				{
					output.Append("No Nights: " + reqitem.nonights + "\n");
					
				}
				if (reqsubcat.tipapp == true)
				{
					output.Append("Tip: " + reqitem.tip + "\n");
					
						
				}

				if (reqsubcat.receiptapp == true)
				{

					output.Append("Do you have a receipt:");
					
					if (reqitem.normalreceipt == true)
					{
						output.Append(" Yes");
					}
					else
					{
						output.Append(" No");
					}
					output.Append("\n");
					
				}
                cSubcatVatRate vatrate = reqsubcat.getVatRateByDate(reqitem.date);
                if (vatrate != null)
                {
                    if (reqsubcat.receiptapp == true)
                    {
                        output.Append("Does it include VAT Details:");

                        if (reqitem.receipt == true)
                        {
                            output.Append(" Yes");
                        }
                        else
                        {
                            output.Append(" No");
                        }
                        output.Append("\n");
                    }
                }
				if (reqsubcat.eventinhomeapp == true)
				{
					output.Append("Event in home city:");
					
					if (reqitem.home == true)
					{
						output.Append(" Yes");
					}
					else
					{
						output.Append(" No");
					}
					output.Append("\n");
					
				}
				

				if (reqsubcat.allowance == true)
				{
					output.Append("Claim Allowance:");
					
					if (reqitem.total != 0)
					{
						output.Append(" Yes");
					}
					else
					{
						output.Append(" No");
					}
					output.Append("\n");
					output.Append("No Allowances: " + reqitem.quantity + "\n");
					
				}

				if (reqsubcat.calculation == CalculationType.DailyAllowance) //daily allowance stuff
				{
					
					
					output.Append("Allowance: " + clsallowances.getAllowanceById(reqitem.allowanceid).allowance + "\n");
					
					
					
					output.Append("Start Date: " + reqitem.allowancestartdate.ToString("dd/MM/yyyy hh:mm") + "\n");
						
					
					
					output.Append("End Date:" + reqitem.allowanceenddate.ToString("dd/MM/yyyy hh:mm") + "\n");
					
					output.Append("Deduct amount (in GBP):" + reqitem.allowancededuct.ToString("£###,##,##0.00") + "\n");
			

				}
				if (reqsubcat.calculation != CalculationType.PencePerMile && reqsubcat.calculation != CalculationType.DailyAllowance && reqsubcat.allowance == false)
				{
					output.Append("Total (");
					if (reqsubcat.addasnet == true)
					{
						output.Append("NET");
					}
					else
					{
						output.Append("Gross");
					}
					output.Append("):");
					
					if (reqitem.currencyid == clsproperties.basecurrency)
					{
						output.Append(reqitem.total.ToString("######0.00"));
					}
					else
					{
						output.Append(reqitem.convertedtotal.ToString("######0.00"));
					}
					output.Append("\n");
				}

                i++;
			}
			return output.ToString();
			
		}
        public void sendMail(string sender, string recipient, string subject, string message)
        {
            if (sender == "")
            {
                sender = "admin@sel-expenses.com";
            }

            if (recipient == "")
            {
                return;
            }

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.Clear();
            System.Web.Mail.MailMessage msg = new System.Web.Mail.MailMessage();
            System.Data.SqlClient.SqlDataReader servreader;
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            string server = "";
            byte sourceaddress = 0;

            if (accountid == 0)
            {
                server = "127.0.0.1";
                sourceaddress = 1;
            }
            else
            {
                
                strsql = "select server, sourceaddress from [other]";

                servreader = expdata.GetReader(strsql);
                while (servreader.Read())
                {
                    if (servreader.IsDBNull(0) == false)
                    {
                        server = servreader.GetString(0);
                    }
                    sourceaddress = servreader.GetByte(1);
                }
                servreader.Close();
                expdata.sqlexecute.Parameters.Clear();
            }

            msg.To = recipient;
            if (sourceaddress == 1)
            {
                msg.From = "admin@sel-expenses.com";
            }
            else
            {
                msg.From = sender;
            }
            msg.Subject = subject;
            msg.Body = message;

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            if (server == "")
            {
                server = clsproperties.server;
            }
            System.Web.Mail.SmtpMail.SmtpServer = server;
            try
            {
                System.Web.Mail.SmtpMail.Send(msg);
            }
            catch (System.Exception exp)
            {
                cErrors clserrors = new cErrors();
                clserrors.ReportError(exp);
                return;
            }

        }
	}

	

	public class cMessage
	{
		private byte bMessageid;
		private bool bSend;
		private string sSubject;
		private string sMessage;
		private byte bDirection;
		private bool bSendnote;
		private string sNote;
		private string sDescription;

		public cMessage (byte messageid, bool send, string subject, string message, byte direction, bool sendnote, string note, string description)
		{
			bMessageid = messageid;
			bSend = send;
			sSubject = subject;
			sMessage = message;
			bDirection = direction;
			bSendnote = sendnote;
			sNote = note;
			sDescription = description;
		}

		#region properties
		public byte messageid
		{
			get {return bMessageid;}
		}
		public bool send
		{
			get {return bSend;}
		}
		public string subject
		{
			get {return sSubject;}
		}
		public string message
		{
			get {return sMessage;}
		}
		public byte direction
		{
			get {return bDirection;}
		}
		public bool sendnote
		{
			get{return bSendnote;}
		}
		public string note
		{
			get {return sNote;}
		}
		public string description
		{
			get {return sDescription;}
		}
		#endregion
	}
}
