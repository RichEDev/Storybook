
namespace SpendManagementLibrary
{
    using System.Web.UI.WebControls;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using SpendManagementLibrary.Helpers;

    public abstract class cEmployeeCorporateCardsBase
    {
        protected int nAccountID;
        protected string sConnectionString;
        protected cFields clsFields;
        protected CardProviders clsCardProviders;

        /// <summary>
        /// Array of employees indexed by employeename containing an Array of cards indexed by cardid
        /// </summary>
        protected SortedList<int, SortedList<int, cEmployeeCorporateCard>> lstCorporateCards;

        /// <summary>
        /// Current User, Warning: check for null before use
        /// from CurrentUser if present, base is just its properties
        /// </summary>
        //protected cCurrentUserBase clsCurrentUser = null;
        protected string sSQL = "SELECT employeeid, corporatecardid, cardproviderid, cardnumber, active, createdby, createdon, modifiedby, modifiedon FROM dbo.employee_corporate_cards";

        /// <summary>
        /// SQL Query for Getting All card statements by Employee
        /// </summary>
        private string getCardStatementsByEmployeeSQL = "SELECT DISTINCT card_statements_base.statementid,[name],card_providers.creditcard,card_providers.purchasecard FROM card_statements_base " +
                  "INNER JOIN card_transactions ON card_transactions.statementid = [card_statements_base].statementid " +
                  "INNER JOIN employee_corporate_cards ON employee_corporate_cards.corporatecardid = card_transactions.corporatecardid " +
                  "INNER JOIN card_providers ON card_providers.cardproviderid=employee_corporate_cards.cardproviderid " +
                  "WHERE employee_corporate_cards.employeeid = @employeeid " +
                  "AND " +
                  "((select count(1) from savedexpenses where savedexpenses.transactionid = card_transactions.transactionid) = 0 OR (card_transactions.transaction_amount - (select sum(total) from savedexpenses where savedexpenses.transactionid = [card_transactions].transactionid)) > 0)";

        protected SortedList<int, SortedList<int, cEmployeeCorporateCard>> GetCollection()
        {
            SortedList<int, SortedList<int, cEmployeeCorporateCard>> employeeCards = new SortedList<int, SortedList<int, cEmployeeCorporateCard>>();
            DBConnection expdata = new DBConnection(sConnectionString);

            SqlDataReader reader;

            using (reader = expdata.GetReader(sSQL))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    int employeeID = reader.GetInt32(0);
                    int corporateCardID = reader.GetInt32(1);
                    cCardProvider provider = clsCardProviders.getProviderByID(reader.GetInt32(2));
                    string cardnumber = reader.GetString(3);
                    bool active = reader.GetBoolean(4);
                    int? createdBy;
                    if (reader.IsDBNull(5)) //reader.GetOrdinal("createdby")))
                    {
                        createdBy = null;
                    }
                    else
                    {
                        createdBy = reader.GetInt32(5); //reader.GetOrdinal("createdby"));
                    }
                    DateTime? createdOn;
                    if (reader.IsDBNull(6)) //reader.GetOrdinal("createdon")))
                    {
                        createdOn = null;
                    }
                    else
                    {
                        createdOn = reader.GetDateTime(6); //reader.GetOrdinal("createdon"));
                    }
                    int? modifiedBy;
                    if (reader.IsDBNull(7)) //reader.GetOrdinal("modifiedby")))
                    {
                        modifiedBy = null;
                    }
                    else
                    {
                        modifiedBy = reader.GetInt32(7); //reader.GetOrdinal("modifiedby"));
                    }
                    DateTime? modifiedOn;
                    if (reader.IsDBNull(8)) //reader.GetOrdinal("modifiedon")))
                    {
                        modifiedOn = null;
                    }
                    else
                    {
                        modifiedOn = reader.GetDateTime(8); //reader.GetOrdinal("modifiedon"));
                    }
                    cEmployeeCorporateCard card = new cEmployeeCorporateCard(corporateCardID, employeeID, provider, cardnumber, active, createdOn, createdBy, modifiedOn, modifiedBy);

                    if (employeeCards.ContainsKey(employeeID))
                    {
                        employeeCards[employeeID].Add(corporateCardID, card);
                    }
                    else
                    {
                        employeeCards.Add(employeeID, new SortedList<int, cEmployeeCorporateCard>());
                        employeeCards[employeeID].Add(corporateCardID, card);
                    }
                }
                reader.Close();
            }

            return employeeCards;
        }

        /// <summary>
        /// Returns a flattened list of all the Cards, for all employees.
        /// </summary>
        /// <returns>A list of all the cards.</returns>
        public List<cEmployeeCorporateCard> GetFlatList()
        {
            return lstCorporateCards.Values.SelectMany(employeeCards => employeeCards.Values).ToList();
        }

        /// <summary>
        /// Return an individual card.
        /// </summary>
        /// <param name="employeeId">The employee to get the card from.</param>
        /// <param name="corporateCardId">The Id of hte card to get.</param>
        /// <returns></returns>
        public cEmployeeCorporateCard GetCorporateCardByID(int employeeId, int corporateCardId)
        {
            if (lstCorporateCards.ContainsKey(employeeId))
            {
                if (lstCorporateCards[employeeId].ContainsKey(corporateCardId))
                {
                    return lstCorporateCards[employeeId][corporateCardId];
                }
            }
            return null;
        }

        /// <summary>
        /// Return an individual card.
        /// </summary>
        /// <param name="corporateCardId">The Id of the card to get.</param>
        /// <returns></returns>
        public cEmployeeCorporateCard GetCorporateCardByID(int corporateCardId)
        {
            return (from employeeCardSet in lstCorporateCards.Values
                    from card in employeeCardSet.Keys
                    where card == corporateCardId
                    select employeeCardSet[card]).FirstOrDefault();
        }

        /// <summary>
        /// Return all cards for an employee
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public SortedList<int, cEmployeeCorporateCard> GetEmployeeCorporateCards(int employeeID)
        {
            if (lstCorporateCards.ContainsKey(employeeID))
            {
                return lstCorporateCards[employeeID];
            }
            return null;
        }

        /// <summary>
        /// Save a card for a user
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        protected int SaveCorporateCardBase(cCurrentUserBase clsCurrentUser, cEmployeeCorporateCard card)
        {
            DBConnection data = new DBConnection(sConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@corporatecardid", card.corporatecardid);
            data.sqlexecute.Parameters.AddWithValue("@employeeid", card.employeeid);
            data.sqlexecute.Parameters.AddWithValue("@cardproviderid", (byte)card.cardprovider.cardproviderid);
            if (card.cardnumber == "")
            {
                data.AddWithValue("@cardnumber", DBNull.Value, clsFields.GetFieldSize("employee_corporate_cards", "cardnumber"));
            }
            else
            {
                data.AddWithValue("@cardnumber", card.cardnumber, clsFields.GetFieldSize("employee_corporate_cards", "cardnumber"));
            }

            data.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(card.active));
            if (card.corporatecardid > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@date", card.ModifiedOn);
                data.sqlexecute.Parameters.AddWithValue("@userid", card.ModifiedBy);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@date", (DateTime)card.CreatedOn);
                data.sqlexecute.Parameters.AddWithValue("@userid", (int)card.CreatedBy);
            }

            if (clsCurrentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", clsCurrentUser.EmployeeID);
                if (clsCurrentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", clsCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            data.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveEmployeeCorporateCard");
            int corporatecardid = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();
            card.corporatecardid = corporatecardid;
            return corporatecardid;
        }


        protected int DeleteCorporateCardBase(cCurrentUserBase clsCurrentUser, int corporatecardid, int employeeID = 0)
        {
            DBConnection data = new DBConnection(sConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@userid", clsCurrentUser != null ? clsCurrentUser.EmployeeID : employeeID);
            data.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            data.sqlexecute.Parameters.AddWithValue("@corporatecardid", corporatecardid);

            if (clsCurrentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", clsCurrentUser.EmployeeID);
                if (clsCurrentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", clsCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            }
            data.ExecuteProc("deleteEmployeeCorporateCard");
            data.sqlexecute.Parameters.Clear();

            return 1;
        }


        /// <summary>
        /// Provides an SQL string for use in cGridNew
        /// </summary>
        /// <returns></returns>
        protected string GetCorporateCardGrid()
        {
            return "select corporatecardid, cardtype, cardprovider, cardnumber, active from employee_corporate_cards inner join card_providers on card_providers.cardproviderid = employee_corporate_cards.cardproviderid";
        }

        /// <summary>
        /// Returns true if the employee given has an active card classified as a purchase card
        /// </summary>
        /// <param name="employeeID">The employee ID to query for</param>
        /// <returns>True if an active purchase card is found</returns>
        public bool HasPurchaseCard(int employeeID)
        {
            if (lstCorporateCards != null && lstCorporateCards.Count > 0 && lstCorporateCards.ContainsKey(employeeID) && lstCorporateCards[employeeID].Count > 0)
            {
                foreach (cEmployeeCorporateCard card in lstCorporateCards[employeeID].Values)
                {
                    if (card.cardprovider.cardtype == CorporateCardType.PurchaseCard && card.active)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the employee given has an active card classified as a credit card
        /// </summary>
        /// <param name="employeeID">The employee ID to query for</param>
        /// <returns>True if an active credit card is found</returns>
        public bool HasCreditCard(int employeeID)
        {
            if (lstCorporateCards != null && lstCorporateCards.Count > 0 && lstCorporateCards.ContainsKey(employeeID) && lstCorporateCards[employeeID].Count > 0)
            {
                foreach (cEmployeeCorporateCard card in lstCorporateCards[employeeID].Values)
                {
                    if (card.cardprovider.cardtype == CorporateCardType.CreditCard && card.active)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all card statements for Employee
        /// </summary>
        /// <param name="employeeId">Id of the employee </param>
        /// <returns>List of <see cref="CardStatement"/>CardStatement</returns>
        protected List<CardStatement> GetCardStatementsByEmployeeBase(int employeeId)
        {
            var lst = new List<CardStatement>();
            using (var data = new DatabaseConnection(this.sConnectionString))
            {
                var strsql = this.getCardStatementsByEmployeeSQL;
                data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
                using (var reader = data.GetReader(strsql))
                {
                    data.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        lst.Add(new CardStatement(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3)));
                    }

                    reader.Close();
                }
            }

            return lst;
        }
    }
}
