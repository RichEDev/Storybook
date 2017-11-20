using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using SpendManagementLibrary.Employees;

namespace SpendManagementLibrary
{
    public struct UFCollection
    {
        public int UF_FieldNumber;
        public string UF_DBFieldName;
        public string UF_FieldName;
        public UserFieldType UF_FieldType;
        public string UF_FieldGroupingName;
    }

    [Serializable]
    public struct MaintParams
    {
        public double CurMaintVal;
        public double ListPrice;
        public double PctOfLP;
        public int MaintCalcType;
        public int MaintTypeX;
        public double MaintTypeValueX;
        public double MaintExtraPercentX;
        public int MaintTypeY;
        public double MaintTypeValueY;
        public double MaintExtraPercentY;
        public ForecastType ForecastType;
    }

    [Serializable]
    public struct NYMResult
    {
        public double NYMValue;
        public string NYMCalculation;
    }

    public class SMRoutines
    {

        public const string DEFAULT_COLPX = "190";

        public static bool CheckRefIntegrity(cFWDBConnection db, string dbTable, int recordID)
        {
            // returns false if(reference doesn//t exist in any related tables, true if(it does
            bool retVal = true;

            switch (dbTable.ToLower())
            {
                case "codes_accountcodes":
                    if (RI(db, "contract_rechargeentity", "AccountCodeId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_contractcategory":
                    if (RI(db, "contract_details", "CategoryId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_contracttype":
                    if (RI(db, "contract_details", "ContractTypeId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_contractstatus":
                    if (RI(db, "contract_details", "ContractStatusId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_financialstatus":
                    if (RI(db, "supplier_details", "financial_statusid", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_inflatormetrics":
                    if (RI(db, "contract_details", "MaintenanceInflatorX", recordID, "", "")) { break; }

                    if (RI(db, "contract_details", "MaintenanceInflatorY", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "invoiceStatusType":
                    if (RI(db, "invoices", "InvoiceStatus", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_invoicefrequencytype":
                    if (RI(db, "contract_details", "InvoiceFrequencyTypeId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_licencerenewaltype":
                    if (RI(db, "productDetails", "RenewalType", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_notecategory":
                    if (RI(db, "codes_notecategory", "noteType", recordID, "", "")) { break; }
                    if (RI(db, "contractNotes", "noteCatId", recordID, "", "")) { break; }
                    if (RI(db, "contractNotes", "noteType", recordID, "", "")) { break; }
                    if (RI(db, "supplierNotes", "noteCatId", recordID, "", "")) { break; }
                    if (RI(db, "supplierNotes", "noteType", recordID, "", "")) { break; }
                    if (RI(db, "productNotes", "noteCatId", recordID, "", "")) { break; }
                    if (RI(db, "productNotes", "noteType", recordID, "", "")) { break; }
                    if (RI(db, "invoiceNotes", "noteCatId", recordID, "", "")) { break; }
                    if (RI(db, "invoiceNotes", "noteType", recordID, "", "")) { break; }
                    if (RI(db, "supplierContactNotes", "noteCatId", recordID, "", "")) { break; }
                    if (RI(db, "supplierContactNotes", "noteType", recordID, "", "")) { break; }
                    retVal = false;
                    break;

                case "productCategories":
                    if (RI(db, "productDetails", "productCategoryId", recordID, "", "")) { break; }
                    retVal = false;
                    break;

                case "codes_sites":
                    // check for allocations within user defined fields
                    db.FWDb("R3", "codes_sites", "Site Id", recordID, "", "", "", "", "", "", "", "", "", "");
                    if (db.FWDb3Flag)
                    {
                        db.AddDBParam("fieldType", UserFieldType.Site_Ref, true);
                        db.RunSQL("SELECT [User Field Id], [AppArea] FROM [user_fields] WHERE [Field Type] = @fieldType", db.glDBWorkA, false, "", false);

                        foreach (DataRow drow in db.glDBWorkA.Tables[0].Rows)
                        {
                            switch ((AppAreas)drow["AppArea"])
                            {
                                case AppAreas.CONTRACT_DETAILS:
                                case AppAreas.CONTRACT_GROUPING:
                                    db.FWDb("R2", "userdefinedContractDetails", "udf" + drow["User Field Id"], db.FWDbFindVal("Site Id", 3), "", "", "", "", "", "", "", "", "", "");
                                    break;
                                case AppAreas.CONTRACT_PRODUCTS:
                                case AppAreas.CONPROD_GROUPING:
                                case AppAreas.RECHARGE_GROUPING:
                                    db.FWDb("R2", "userdefinedContractProductDetails", "udf" + drow["User Field Id"], db.FWDbFindVal("Site Id", 3), "", "", "", "", "", "", "", "", "", "");
                                    break;
                                case AppAreas.PRODUCT_DETAILS:
                                    db.FWDb("R2", "userdefinedProductDetails", "udf" + drow["User Field Id"], db.FWDbFindVal("Site Id", 3), "", "", "", "", "", "", "", "", "", "");
                                    break;
                                case AppAreas.STAFF_DETAILS:
                                    db.FWDb("R2", "staff_details", "udf" + drow["User Field Id"], db.FWDbFindVal("Site Id", 3), "", "", "", "", "", "", "", "", "", "");
                                    break;
                                case AppAreas.VENDOR_DETAILS:
                                    db.FWDb("R2", "userdefinedSupplierDetails", "udf" + drow["User Field Id"], db.FWDbFindVal("Site Id", 3), "", "", "", "", "", "", "", "", "", "");
                                    break;
                                default:
                                    break;
                            }

                            if (db.FWDb2Flag)
                            {
                                // found an assigned value
                                retVal = true;
                                break;
                            }
                        }
                    }

                    retVal = false;
                    break;

                case "codes_tasktypes":
                    if (RI(db, "tasks", "taskTypeId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_termtype":
                    if (RI(db, "contract_details", "termTypeId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_rechargeentity":
                    if (RI(db, "codes_sites", "RechargeEntityId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_units":
                    if (RI(db, "contract_productdetails", "unitId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "codes_salestax":
                    if (RI(db, "contract_productdetails", "SalesTaxRate", recordID, "", "")) { break; }
                    if (RI(db, "invoices", "SalesTaxRate", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "contract_details":
                    if (RI(db, "contract_productdetails", "contractId", recordID, "", "")) { break; }
                    if (RI(db, "link_variations", "primaryContractId", recordID, "", "")) { break; }
                    if (RI(db, "invoices", "ContractId", recordID, "", "")) { break; }
                    if (RI(db, "contractNotes", "contractID", recordID, "", "")) { break; }
                    if (RI(db, "contract_forecastdetails", "ContractId", recordID, "", "")) { break; }
                    if (RI(db, "attachments", "ReferenceNumber", recordID, "AttachmentArea", AttachmentArea.CONTRACT)) { break; }
                    retVal = false;
                    break;

                case "contract_productdetails":
                    if (RI(db, "recharge_associations", "contractProductId", recordID, "", "")) { break; }
                    retVal = false;
                    break;
                case "mime_headers":
                    retVal = false;
                    break;

                case "productdetails":
                    if (RI(db, "contract_productdetails", "ProductId", recordID, "", "")) { break; }

                    if (RI(db, "invoiceproductdetails", "ProductId", recordID, "", "")) { break; }

                    if (RI(db, "productNotes", "ProductId", recordID, "", "")) { break; }

                    if (RI(db, "contract_forecastproducts", "ProductId", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "staff_details":
                    if (RI(db, "contract_details", "Contract Owner", recordID, "", "")) { break; }

                    if (RI(db, "contract_notification", "Staff Id", recordID, "", "")) { break; }

                    string sql = "SELECT COUNT(*) AS [Cnt] FROM team_members LEFT JOIN [teams] ON [teams].[teamid] = [team_members].[Team Id] WHERE [Member Id] = @employeeId AND [teamtype] = 0";
                    db.AddDBParam("employeeId", recordID, true);
                    db.RunSQL(sql, db.glDBWorkA, false, "", false);
                    if ((int)db.GetFieldValue(db.glDBWorkA, "Cnt", 0, 0) > 0)
                    {
                        break;
                    }

                    // check any user defined fields that are of type staff ref
                    DataSet delDSET = new DataSet();
                    string tmpTable;

                    sql = "SELECT [User Field Id],[AppArea] FROM [user_fields] WHERE [Field Type] = @fieldType";
                    db.AddDBParam("fieldType", UserFieldType.StaffName_Ref, true);
                    db.RunSQL(sql, delDSET, false, "", false);

                    foreach (DataRow drow in delDSET.Tables[0].Rows)
                    {
                        switch ((AppAreas)drow["AppArea"])
                        {
                            case AppAreas.CONPROD_GROUPING:
                            case AppAreas.CONTRACT_PRODUCTS:
                            case AppAreas.RECHARGE_GROUPING:
                                tmpTable = "contract_productdetails";
                                break;
                            case AppAreas.CONTRACT_DETAILS:
                            case AppAreas.CONTRACT_GROUPING:
                                tmpTable = "contract_details";
                                break;
                            case AppAreas.PRODUCT_DETAILS:
                                tmpTable = "productDetails";
                                break;
                            case AppAreas.STAFF_DETAILS:
                                tmpTable = "staff_details";
                                break;
                            case AppAreas.VENDOR_DETAILS:
                                tmpTable = "vendor_details";
                                break;
                            default:
                                tmpTable = "";
                                break;
                        }

                        if (tmpTable != "")
                        {
                            if (RI(db, tmpTable, "UF" + ((string)drow["User Field Id"]).Trim(), recordID, "", "")) { break; }
                        }
                    }
                    retVal = false;
                    break;

                case "sublocations":
                    if (RI(db, "contract_productplatforms", "Sublocation Id", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "supplier_details":
                    if (RI(db, "supplier_contacts", "supplierid", recordID, "", "")) { break; }

                    if (RI(db, "contract_details", "supplierId", recordID, "", "")) { break; }

                    if (RI(db, "supplierNotes", "supplierId", recordID, "", "")) { break; }

                    if (RI(db, "attachments", "ReferenceNumber", recordID, "AttachmentArea", AttachmentArea.VENDOR)) { break; }

                    retVal = false;
                    break;

                case "supplier_categories":
                    if (RI(db, "supplier_details", "categoryid", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "supplier_status":
                    if (RI(db, "supplier_details", "statusid", recordID, "", "")) { break; }

                    retVal = false;
                    break;

                case "version_registry":
                    if (RI(db, "version_registry_calloff", "Registry Id", recordID, "", "")) { break; }

                    retVal = false;
                    break;
                default:
                    break;
            }

            return retVal;
        }

        private static bool RI(cFWDBConnection db, string dbTable, string IDField, int recID, string otherFld, object otherVal)
        {
            if (otherFld == "")
            {
                db.FWDb("R", dbTable, IDField, recID, "", "", "", "", "", "", "", "", "", "");
            }
            else
            {
                db.FWDb("R", dbTable, IDField, recID, otherFld, otherVal, "", "", "", "", "", "", "", "");
            }
            return db.FWDbFlag;
        }

        public static bool DefExists(cFWDBConnection db, string dbTableName, string CheckField, string CheckValue, string LocField, int LocID)
        {
            if (LocField != "")
            {
                db.FWDb("R3", dbTableName, CheckField, CheckValue, LocField, LocID, "", "", "", "", "", "", "", "");
            }
            else
            {
                db.FWDb("R3", dbTableName, CheckField, CheckValue, "", "", "", "", "", "", "", "", "", "");
            }

            return db.FWDb3Flag;
        }

        public static int CheckListIndex(object Val)
        {
            int retVal = 0;
            int.TryParse(Val.ToString(), out retVal);

            if (retVal == -1)
            {
                return 0;
            }
            else
            {
                return retVal;
            }
        }

        public static string GetDescriptionFromID(cFWDBConnection db, int id, string dbtable, string IDfldname, string descfldname)
        {
            db.FWDb("R2", dbtable, IDfldname, id, "", "", "", "", "", "", "", "", "", "");
            return db.FWDb2Flag ? db.FWDbFindVal(descfldname, 2) : "<unknown>";
        }

        public static bool hasLocationId(string table)
        {
            bool retVal = false;

            // remove brackets if(present
            table = table.Replace("[", "");
            table = table.Replace("]", "");

            if (table.StartsWith("custom_"))
            {
                return retVal;
            }

            switch (table.ToLower())
            {
                case "annual_cost_summary":
                case "attachment_audience":
                case "attachments":
                case "codes_notecategory":
                case "contract_audience":
                case "contract_forecastdetails":
                case "contract_forecastproducts":
                case "contract_history":
                case "contract_notes_view":
                case "contract_productdetails":
                case "contract_producthistory":
                case "contractnotes":
                case "employees":
                case "invoiceproductdetails":
                case "invoices":
                case "invoicenotes":
                case "invoice_notes_view":
                case "mime_headers":
                case "platform_lpars":
                case "primarynotecategory_view":
                case "productnotes":
                case "product_notes_view":
                case "product_usage":
                case "product_suppliers":
                case "secondarynotecategory_view":
                case "supplier_addresses":
                case "suppliercontact_summary_view":
                case "supplier_notes_view":
                case "suppliercontact_notes_view":
                case "supplier_contacts":
                case "suppliercontactnotes":
                case "suppliernotes":
                case "version_registry":
                    break;
                default:
                    retVal = true;
                    break;
            }
            return retVal;
        }

        public static bool hasExtraPercent(cFWSettings fw, int id)
        {
            cFWDBConnection db = new cFWDBConnection();

            db.DBOpen(fw, false);

            bool reply = false;

            db.FWDb("R", "codes_inflatormetrics", "metricId", id, "", "", "", "", "", "", "", "", "", "");
            if (db.FWDbFlag)
            {
                if (int.Parse(db.FWDbFindVal("requiresExtraPct", 1)) == 1)
                {
                    reply = true;
                }
            }

            db.DBClose();

            return reply;
        }

        public static void GetFieldKeys(string dbTableName, ref string IDFieldName, ref string DupName, ref string redir, ref string AudName, cAccountProperties FWParams)
        {
            switch (dbTableName)
            {
                case "codes_accountcodes":
                    IDFieldName = "Code Id";
                    DupName = "Account Code";
                    redir = "basedefinitions.aspx?item=accountcodes";
                    AudName = "ACCOUNT CODE";
                    break;
                case "codes_contractcategory":
                    IDFieldName = "Category Id";
                    DupName = "Category Description";
                    redir = "basedefinitions.aspx?item=contractcategory";
                    AudName = FWParams.ContractCategoryTitle + "MAINT";
                    break;
                case "codes_contracttype":
                    IDFieldName = "Contract Type Id";
                    DupName = "Contract Type Description";
                    redir = "basedefinitions.aspx?item=contracttype";
                    AudName = "CONTRACT TYPE MAINT";
                    break;
                case "codes_contractstatus":
                    IDFieldName = "Status Id";
                    DupName = "Status Description";
                    redir = "basedefinitions.aspx?item=contractstatus";
                    AudName = "CONTRACT STATUS MAINT";
                    break;
                case "codes_financialstatus":
                    IDFieldName = "Financial Status Id";
                    DupName = "Status Description";
                    redir = "basedefinitions.aspx?item=financialstatus";
                    AudName = "FINANCIAL STATUS";
                    break;
                case "codes_inflatormetrics":
                    IDFieldName = "Metric Id";
                    DupName = "Name";
                    redir = "basedefinitions.aspx?item=inflator";
                    AudName = "INFLATOR METRIC MAINT";
                    break;
                case "invoiceStatusType":
                    IDFieldName = "InvoiceStatusTypeId";
                    DupName = "description";
                    redir = "basedefinitions.aspx?item=invoicestatus";
                    AudName = "INVOICE STATUS MAINT";
                    break;
                case "codes_invoicefrequencytype":
                    IDFieldName = "Invoice Frequency Type Id";
                    DupName = "Invoice Frequency Type Desc";
                    redir = "basedefinitions.aspx?item=invoicefrequency";
                    AudName = "INVOICE FREQ MAINT";
                    break;
                case "codes_licencerenewaltype":
                    IDFieldName = "Renewal Type";
                    DupName = "Renewal Type Description";
                    redir = "basedefinitions.aspx?item=licencetype";
                    AudName = "LICENCE RENEWAL MAINT";
                    break;
                case "productCategories":
                    IDFieldName = "CategoryId";
                    DupName = "Description";
                    redir = "basedefinitions.aspx?item=productcategory";
                    AudName = "PRODUCT CATEGORY";
                    break;
                case "codes_rechargeentity":
                    IDFieldName = "Entity Id";
                    DupName = "Name";
                    redir = "basedefinitions.aspx?item=rechargeentity";
                    AudName = "RECHARGE ENTITY";
                    break;
                case "codes_salestax":
                    IDFieldName = "Sales Tax Id";
                    DupName = "Sales Tax Description";
                    redir = "basedefinitions.aspx?item=salestax";
                    AudName = "SALES TAX MAINT";
                    break;
                case "codes_sites":
                    IDFieldName = "Site Id";
                    DupName = "Site Code";
                    redir = "basedefinitions.aspx?item=sites";
                    AudName = "SITE DEFINITION";
                    break;
                case "codes_tasktypes":
                    IDFieldName = "Type Id";
                    DupName = "Type Description";
                    redir = "basedefinitions.aspx?item=tasktypes";
                    AudName = "TASK TYPES";
                    break;
                case "codes_termtype":
                    IDFieldName = "Term Type Id";
                    DupName = "Term Type Description";
                    redir = "basedefinitions.aspx?item=termtype";
                    AudName = "TERM TYPE MAINT";
                    break;
                case "codes_units":
                    IDFieldName = "Unit Id";
                    DupName = "Unit Description";
                    redir = "basedefinitions.aspx?item=units";
                    AudName = "UNIT MAINT";
                    break;
                case "mime_headers":
                    IDFieldName = "mimeId";
                    DupName = "fileExtension";
                    redir = "basedefinitions.aspx?item=mimeheaders";
                    AudName = "MIME HEADERS";
                    break;
                case "staff_details":
                    IDFieldName = "Staff Id";
                    DupName = "Staff Name";
                    redir = "basedefinitions.aspx?item=staff";
                    AudName = "EMPLOYEE DETAILS";
                    break;
                case "sublocations":
                    IDFieldName = "Sublocation Id";
                    DupName = "Sublocation Desc";
                    redir = "basedefinitions.aspx?item=sublocations";
                    AudName = "SUBLOCATIONS MAINT";
                    break;
                case "supplier_categories":
                    IDFieldName = "categoryid";
                    DupName = "Description";
                    redir = "basedefinitions.aspx?item=vendorcategory";
                    AudName = FWParams.SupplierPrimaryTitle + " CATEGORY";
                    break;
                case "supplier_status":
                    IDFieldName = "statusid";
                    DupName = "Description";
                    redir = "basedefinitions.aspx?item=vendorstatus";
                    AudName = FWParams.SupplierPrimaryTitle + " STATUS";
                    break;
                default:
                    break;
            }
        }
        public static string ClearDownAudit(int accountid, string connStr, System.Web.UI.Page activePage, string AbsolutePath, Employee employee, cAccountProperties properties)
        {
            DBConnection db = new DBConnection(connStr);
            string retVal = "";

            string brandName = System.Configuration.ConfigurationManager.AppSettings["ApplicationInstanceName"] ?? "The Application";

            try
            {
                string app_Path;
                if (activePage == null)
                {
                    if (AbsolutePath != "")
                    {
                        if (File.Exists(Path.Combine(AbsolutePath, "Framework.ini")))
                        {
                            app_Path = AbsolutePath;
                        }
                        else
                        {
                            retVal = "";
                            return retVal;
                        }
                    }
                    else
                    {
                        if (File.Exists("c:\\inetpub\\wwwroot\\Framework\\Framework.ini"))
                        {
                            // use the default path as this appears to be valid
                            app_Path = "c:\\inetpub\\wwwroot\\Framework\\";
                        }
                        else
                        {
                            retVal = "";
                            return retVal;
                        }
                    }
                }
                else
                {
                    app_Path = activePage.Request.PhysicalApplicationPath;
                    if (app_Path == "")
                    {
                        // without the application path, cannot proceed, check if(the default is to apply
                        if (File.Exists("c:\\inetpub\\wwwroot\\Framework\\Framework.ini"))
                        {
                            // use the default path as this appears to be valid
                            app_Path = "c:\\inetpub\\wwwroot\\Framework\\";
                        }
                        else
                        {
                            retVal = "";
                            return retVal;
                        }
                    }
                }

                string sysPath = Path.Combine(app_Path, "temp\\tmpAuditOut.csv");

                // Create the file.
                if (File.Exists(sysPath))
                {
                    File.Delete(sysPath);
                }
                StreamWriter fs = new StreamWriter(sysPath, false);

                DateTime cutoffDateTime = DateTime.Now;

                string sql = "SELECT companyid, username, Datestamp,[Action],Category,field,[OldValue],[NewValue] FROM [auditLog] WHERE ";
                sql += "[Datestamp] <= @cutoffDate ORDER BY [Datestamp] DESC";
                db.sqlexecute.Parameters.AddWithValue("@cutoffDate", cutoffDateTime);
                DataSet ds = db.GetDataSet(sql);

                bool isFirstRow = true;
                foreach (DataRow drow in ds.Tables[0].Rows)
                {
                    string dataLine;
                    if (isFirstRow)
                    {
                        // output the heading row
                        dataLine = "Company Id, Username, Date / Time,Action,Category,Field,Value Before,Value After\n";
                        fs.WriteLine(dataLine);
                        isFirstRow = false;
                    }

                    bool firstItem = true;
                    dataLine = "";

                    foreach (DataColumn dcol in ds.Tables[0].Columns)
                    {
                        if (!firstItem)
                        {
                            dataLine += ",";
                        }

                        if (drow[dcol.ColumnName] == DBNull.Value)
                        {
                            dataLine += "";
                        }
                        else
                        {
                            if (drow[dcol.ColumnName].GetType() == typeof(DateTime))
                            {
                                dataLine += ((DateTime)drow[dcol.ColumnName]).ToShortDateString();
                            }
                            else
                            {
                                dataLine += (string)drow[dcol.ColumnName];
                            }
                        }

                        firstItem = false;
                    }
                    fs.WriteLine(dataLine);
                }

                fs.Close();

                sql = "DELETE FROM [auditLog] WHERE ";

                db.sqlexecute.Parameters.Clear();
                sql += "[Datestamp] <= @cutoffDate";
                db.sqlexecute.Parameters.AddWithValue("@cutoffDate", cutoffDateTime);
                db.ExecuteSQL(sql);

                string newAuditOutFile = Path.Combine(app_Path, "temp\\AuditOut" + cutoffDateTime.ToShortDateString().Replace("/", "").Trim() + ".csv");
                if (File.Exists(newAuditOutFile))
                {
                    // add hour,mins & secs to the filename to ensure unique!!
                    newAuditOutFile = Path.Combine(app_Path, "temp\\AuditOut" + cutoffDateTime.ToShortDateString().Replace("/", "").Trim() + cutoffDateTime.ToShortTimeString().Replace(":", "").Trim() + ".csv");
                }

                File.Copy(sysPath, newAuditOutFile);

                // construct an email and issue to all those nominated to receive the audit log
                if (properties.EmailServerAddress != "")
                {
                    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(properties.EmailServerFromAddress, properties.AuditorEmailAddress);

                    msg.Subject = brandName + " Audit Log";
                    if (employee == null)
                    {
                        msg.Body = brandName + " has had it//s Audit Log purged. The cleardown was automatically started by the " + brandName + " schedule//\n\n.CSV file attachment contains the audit log to date since the last purge.";
                    }
                    else
                    {
                        msg.Body = brandName + " has had it//s Audit Log purged. The cleardown was manually started by //" + employee.Forename + " " + employee.Surname + " (" + employee.Username + ").//.CSV file attachment contains the audit log to date since the last purge.";
                    }

                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(newAuditOutFile);

                    msg.Attachments.Add(attachment);

                    EmailSender sender = new EmailSender(properties.EmailServerAddress);

                    retVal = sender.SendEmail(msg) ? "OK" : "Email Failed";

                    msg.Dispose();
                    attachment.Dispose();
                }
            }
            catch (Exception ex)
            {
                retVal = ex.Message;
            }

            return retVal;
        }

        public static bool Check4Notes(cFWSettings fws, AttachmentArea NoteArea, int cId)
        {
            bool hasNotes = false;
            cFWDBConnection db = new cFWDBConnection();

            db.DBOpen(fws, false);

            switch (NoteArea)
            {
                case AttachmentArea.CONTRACT_NOTES:
                    db.FWDb("R", "contractNotes", "ContractId", cId, "", "", "", "", "", "", "", "", "", "");
                    break;
                case AttachmentArea.PRODUCT_NOTES:
                    db.FWDb("R", "productNotes", "ProductId", cId, "", "", "", "", "", "", "", "", "", "");
                    break;
                case AttachmentArea.VENDOR_NOTES:
                    db.FWDb("R", "supplierNotes", "supplierId", cId, "", "", "", "", "", "", "", "", "", "");
                    break;
                case AttachmentArea.INVOICE_NOTES:
                    db.FWDb("R", "invoiceNotes", "InvoiceId", cId, "", "", "", "", "", "", "", "", "", "");
                    break;
                case AttachmentArea.CONTACT_NOTES:
                    db.FWDb("R", "supplierContactNotes", "contactId", cId, "", "", "", "", "", "", "", "", "", "");
                    break;
                default:
                    db.FWDb("R", "contractNotes", "contractId", -1, "", "", "", "", "", "", "", "", "", "");
                    break;
            }

            hasNotes = db.FWDbFlag;

            db.DBClose();

            return hasNotes;
        }

        public static DateTime GetNextRunDate(DateTime curDate, emailFreq FreqType, int FreqParam)
        {
            DateTime retDate = DateTime.MinValue;

            retDate = curDate;
            try
            {
                switch (FreqType)
                {
                    case emailFreq.Daily:
                        retDate = curDate.AddDays(1);
                        break;
                    case emailFreq.Every_n_Days:
                        retDate = curDate.AddDays(FreqParam);
                        break;
                    case emailFreq.MonthlyOnDay:
                        retDate = curDate.AddMonths(1);
                        break;
                    case emailFreq.MonthlyOnFirstXDay:
                        for (int x = 1; x < 8; x++)
                        {
                            retDate = DateTime.Parse(x.ToString().Trim() + "\\" + (curDate.Month + 1).ToString().Trim() + "\\" + (curDate.Year).ToString().Trim());
                            if (retDate.DayOfWeek == (DayOfWeek)FreqParam)
                            {
                                // found the 1st occurrence of the required day
                                break;
                            }
                        }
                        break;
                    case emailFreq.Weekly:
                        retDate = curDate.AddDays(7);
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }

            return retDate;
        }

        public static void SubmitError(UserInfo ui, cFWSettings fws)
        {

        }

        public static string GetDDListData(cFWDBConnection db, int UFid, string strValue, int tabindex)
        {
            StringBuilder strHTML = new StringBuilder();
            string sql;
            StringBuilder options = new StringBuilder();
            bool hasselected;

            sql = "SELECT * FROM [user_fieldvalues] WHERE [User Field Id] = @ufId ORDER BY [Field Value]";
            db.AddDBParam("ufId", UFid, true);
            db.RunSQL(sql, db.glDBWorkD, false, "", false);

            strHTML.Append("<SELECT name=\"UF" + UFid.ToString() + "\" tabindex=\"" + tabindex.ToString() + "\" style=\"width: " + DEFAULT_COLPX + "px\">\n");

            hasselected = false;
            foreach (DataRow drow in db.glDBWorkD.Tables[0].Rows)
            {
                options.Append("<OPTION value=\"" + (string)drow["Field Value"] + "\" ");
                if (((string)drow["Field Value"]).Trim() == strValue.Trim())
                {
                    options.Append("selected");
                    hasselected = true;
                }

                options.Append(">" + (string)drow["Field Value"] + "</OPTION>\n");
            }

            string tmpText = "";
            if (!hasselected)
            {
                tmpText = "selected";
            }
            strHTML.Append("<OPTION value=\"\" " + tmpText + ">[None]</OPTION>\n");
            strHTML.Append(options);
            strHTML.Append("</SELECT>\n");

            return strHTML.ToString();
        }

        public static string GetRefListData(cFWDBConnection db, UserInfo ui, UserFieldType UFType, int UFid, string strValue, int tabindex)
        {
            StringBuilder strHTML = new StringBuilder();
            string sql;
            StringBuilder options = new StringBuilder();
            bool hasselected;
            string tmpId;
            string tmpDef;
            string tmpDef2;

            switch (UFType)
            {
                case UserFieldType.StaffName_Ref:
                    sql = "SELECT [Staff Id],[Staff Name] FROM [staff_details] WHERE [Location Id] = @Location_Id ORDER BY [Staff Name]";
                    tmpId = "Staff Id";
                    tmpDef = "Staff Name";
                    tmpDef2 = "";
                    break;
                case UserFieldType.Site_Ref:
                    sql = "SELECT [Site Id],[Site Code],[Site Description] FROM [codes_sites] WHERE [Location Id] = @Location_Id ORDER BY [Site Code]";
                    tmpId = "Site Id";
                    tmpDef = "Site Code";
                    tmpDef2 = "Site Description";
                    break;
                case UserFieldType.RechargeClient_Ref:
                    sql = "SELECT [Entity Id],[Name] FROM [codes_rechargeentity] WHERE [Location Id] = @Location_Id ORDER BY [Name]";
                    tmpId = "Entity Id";
                    tmpDef = "Name";
                    tmpDef2 = "";
                    break;
                case UserFieldType.RechargeAcc_Code:
                    sql = "SELECT [Code Id],[Account Code],[Description] FROM [codes_accountcodes] WHERE [Location Id] = @Location_Id ORDER BY [Description]";
                    tmpId = "Code Id";
                    tmpDef = "Account Code";
                    tmpDef2 = "Description";
                    break;
                default:
                    // should never get here, so abort
                    return "";
            }
            db.AddDBParam("Location Id", ui.ActiveLocation.ToString(), true);
            db.RunSQL(sql, db.glDBWorkD, false, "", false);

            if (db.glNumRowsReturned > cDef.UF_MAXCOUNT)
            {
                string dispTxt = "";

                foreach (DataRow drow in db.glDBWorkD.Tables[0].Rows)
                {
                    string cmpVal = (string)drow[tmpId];
                    if (cmpVal == strValue)
                    {
                        dispTxt = (string)drow[tmpDef];
                        if (tmpDef2 != "")
                        {
                            dispTxt = " : " + (string)drow[tmpDef2];
                        }
                        break;
                    }
                }

                strHTML.Append("<input type=\"text\" readonly style=\"width: " + (int.Parse(DEFAULT_COLPX) - 16).ToString() + "px;\" class=\"datatext\" name=\"UF" + UFid.ToString().Trim() + "_TXT\" value=\"" + dispTxt.Trim() + "\"/>");
                strHTML.Append("<input type=\"hidden\" name=\"UF" + UFid.ToString().Trim() + "\" value=\"" + strValue.Trim() + "\"/>&nbsp;");
                strHTML.Append("<a onclick=\"javascript:doSearch(" + UFType + ",//UF" + UFid.ToString().Trim() + "//);\" onmouseover=\"window.status=//Search for field value//;return true;\" onmouseout=\"window.status=//Done//;\">");
                strHTML.Append("<img src=\"./icons/16/plain/view.gif\" alt=\"Search\" />");
                strHTML.Append("</a>");
            }
            else
            {
                strHTML.Append("<SELECT name=\"UF" + UFid.ToString().Trim() + "\" tabindex=\"" + tabindex.ToString().Trim() + "\" style=\"width: " + DEFAULT_COLPX + "px\">\n");

                hasselected = false;
                foreach (DataRow drow in db.glDBWorkD.Tables[0].Rows)
                {
                    options.Append("<OPTION value=\"" + (string)drow[tmpId] + "\" ");
                    if ((string)drow[tmpId] == strValue.Trim())
                    {
                        options.Append("selected");
                        hasselected = true;
                    }

                    options.Append(">" + (string)drow[tmpDef]);
                    if (tmpDef2 != "")
                    {
                        options.Append(" [" + (string)drow[tmpDef2] + "]");
                    }
                    options.Append("</OPTION>\n");
                }
                string tmpTxt = "";
                if (!hasselected)
                {
                    tmpTxt = "selected";
                }
                strHTML.Append("<OPTION value=\"0\" " + tmpTxt + ">[None]</OPTION>\n");
                strHTML.Append(options);
                strHTML.Append("</SELECT>\n");
            }
            return strHTML.ToString();
        }

        public static string GetGroupedUFData(cFWDBConnection db, DataRow drow, UFCollection[] UFields)
        {
            StringBuilder strHTML = new StringBuilder();
            StringBuilder strDataHTML = new StringBuilder();
            string strDataValue;
            string curGroup;
            int cellIdx;
            int p;
            bool firstpass = true;

            curGroup = "";
            cellIdx = 0;

            if (UFields.Length > 0)
            {
                for (int x = 0; x < UFields.Length; x++)
                {
                    if (curGroup != UFields[x].UF_FieldGroupingName)
                    {
                        if (!firstpass)
                        {
                            // must pad out the remaining cells
                            if (cellIdx == 1)
                            {
                                strHTML.Append("<td colspan=\"2\">&nbsp;</td>\n");
                                strHTML.Append("</tr>\n");
                                strHTML.Append("<tr><td colspan=\"4\"><hr /></td></tr>\n");
                            }
                            strHTML.Append("</table>\n");
                            cellIdx = 0;
                        }

                        strHTML.Append("<div id=\"broadcasttitle\">" + UFields[x].UF_FieldGroupingName + "</div>\n");
                        strHTML.Append("<table>\n");

                        curGroup = UFields[x].UF_FieldGroupingName;
                        strHTML.Append("<tr>\n");
                        firstpass = false;
                    }

                    if (cellIdx == 2)
                    {
                        cellIdx = 0;
                        strHTML.Append("</tr><tr>\n");
                    }

                    cellIdx += 1;

                    if (drow[UFields[x].UF_DBFieldName] == DBNull.Value)
                    {
                        strDataValue = "";
                    }
                    else
                    {
                        strDataValue = (string)drow[UFields[x].UF_DBFieldName];
                    }

                    switch (UFields[x].UF_FieldType)
                    {
                        case UserFieldType.Character:
                        case UserFieldType.Number:
                        case UserFieldType.DDList:
                        case UserFieldType.Float:
                            strDataHTML.Append("<INPUT type=\"text\" readonly value=\"");
                            strDataHTML.Append(strDataValue);
                            strDataHTML.Append("\" />");
                            break;
                        case UserFieldType.DateField:
                            if (strDataValue != "")
                            {
                                DateTime tmpDate;
                                if (DateTime.TryParse(strDataValue, out tmpDate))
                                {
                                    strDataValue = tmpDate.ToString(cDef.DATE_FORMAT);
                                }
                            }
                            strDataHTML.Append("<INPUT type=\"text\" readonly value=\"");
                            strDataHTML.Append(strDataValue);
                            strDataHTML.Append("\" />");
                            break;
                        case UserFieldType.Checkbox:
                            string checkedstring = "";
                            if (strDataValue == "1")
                            {
                                checkedstring = " checked";
                            }
                            strDataHTML.Append("<INPUT type=\"checkbox\" disabled name=\"" + UFields[x].UF_DBFieldName + "\"" + checkedstring + " />\n");
                            break;
                        case UserFieldType.Text:
                            strDataHTML.Append("<textarea readonly>");
                            strDataHTML.Append(strDataValue);
                            strDataHTML.Append("</textarea>");
                            break;
                        case UserFieldType.StaffName_Ref:
                            db.FWDb("R3", "staff_details", "Staff Id", strDataValue, "", "", "", "", "", "", "", "", "", "");

                            strDataHTML.Append("<INPUT type=\"text\" readonly value=\"");
                            if (db.FWDb3Flag)
                            {
                                strDataHTML.Append(db.FWDbFindVal("Staff Name", 3));
                            }
                            else
                            {
                                strDataHTML.Append("");
                            }
                            strDataHTML.Append("\" />");
                            break;
                        case UserFieldType.Site_Ref:
                            db.FWDb("R3", "codes_sites", "Site Id", strDataValue, "", "", "", "", "", "", "", "", "", "");

                            strDataHTML.Append("<INPUT type=\"text\" readonly value=\"");
                            if (db.FWDb3Flag)
                            {
                                strDataHTML.Append(db.FWDbFindVal("Site Code", 3) + ":" + db.FWDbFindVal("Site Description", 3));
                            }
                            else
                            {
                                strDataHTML.Append("");
                            }
                            strDataHTML.Append("\" />");
                            break;
                        case UserFieldType.RechargeClient_Ref:
                            db.FWDb("R3", "codes_rechargeentity", "Entity Id", strDataValue, "", "", "", "", "", "", "", "", "", "");

                            strDataHTML.Append("<INPUT type=\"text\" readonly value=\"");
                            if (db.FWDb3Flag)
                            {
                                strDataHTML.Append(db.FWDbFindVal("Name", 3));
                            }
                            else
                            {
                                strDataHTML.Append("");
                            }
                            strDataHTML.Append("\" />");
                            break;
                        case UserFieldType.RechargeAcc_Code:
                            db.FWDb("R3", "codes_accountcodes", "Code Id", strDataValue, "", "", "", "", "", "", "", "", "", "");

                            strDataHTML.Append("<INPUT type=\"text\" readonly value=\"");
                            if (db.FWDb3Flag)
                            {
                                strDataHTML.Append(db.FWDbFindVal("Account Code", 3));
                                strDataHTML.Append(":" + db.FWDbFindVal("Description", 3));
                            }
                            else
                            {
                                strDataHTML.Append("");
                            }
                            strDataHTML.Append("\" />");
                            break;
                        default:
                            strDataHTML.Append("<i>Unknown Field Type Defined</i>");
                            break;
                    }

                    strHTML.Append("<td class=\"labeltd\">" + UFields[x].UF_FieldName + "</td>\n");  // label
                    strHTML.Append("<td class=\"inputtd\">");
                    strHTML.Append(strDataHTML);
                    // blank the data html for next round
                    strDataHTML = new StringBuilder();
                    strHTML.Append("</td>\n");	  // data
                }

                // must pad out the remaining cells
                for (p = cellIdx; p > 0; p--)
                {
                    strHTML.Append("<td>&nbsp;</td>\n");
                    strHTML.Append("<td>&nbsp;</td>\n");
                }

                strHTML.Append("</tr>\n");
                strHTML.Append("</table>\n");
                strHTML.Append("</div>\n");
            }

            return strHTML.ToString();
        }

        public static MaintParams GetMaintParams(ref cFWDBConnection fwdb, int contractID)
        {
            string sql;
            MaintParams paramstruct = new MaintParams();
            System.Data.DataSet dset = new DataSet();

            sql = "SELECT [ForecastTypeId],[MaintenanceType],[MaintenanceInflatorX],[MaintenancePercentX],[X].[Percentage] AS [PercentageX],[MaintenanceInflatorY],[MaintenancePercentY],[Y].[Percentage] AS [PercentageY] FROM [contract_details]\n";
            sql += "LEFT OUTER JOIN [codes_inflatormetrics] AS [X] ON [contract_details].[MaintenanceInflatorX] = [X].[MetricId]\n";
            sql += "LEFT OUTER JOIN [codes_inflatormetrics] AS [Y] ON [contract_details].[MaintenanceInflatorY] = [Y].[MetricId]";
            sql += "WHERE [ContractId] = @conId";
            fwdb.AddDBParam("conId", contractID, true);
            fwdb.RunSQL(sql, dset, false, "", false);

            if (dset != null && dset.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drow in dset.Tables[0].Rows)
                {
                    if (drow["ForecastTypeId"] == DBNull.Value)
                    {
                        paramstruct.ForecastType = ForecastType.Prod_v_Inflator;
                    }
                    else
                    {
                        paramstruct.ForecastType = (ForecastType)drow["ForecastTypeId"];
                    }
                    if (drow["MaintenanceType"] == DBNull.Value)
                    {
                        paramstruct.MaintCalcType = 0;
                    }
                    else
                    {
                        paramstruct.MaintCalcType = (int)drow["MaintenanceType"];
                    }
                    if (drow["MaintenanceInflatorX"] == DBNull.Value)
                    {
                        paramstruct.MaintTypeX = 0;
                    }
                    else
                    {
                        paramstruct.MaintTypeX = (int)drow["MaintenanceInflatorX"];
                    }

                    if (drow["PercentageX"] != DBNull.Value)
                    {
                        paramstruct.MaintTypeValueX = Convert.ToDouble(drow["PercentageX"]);
                    }
                    else
                    {
                        paramstruct.MaintTypeValueX = 0;
                    }

                    if (drow["MaintenancePercentX"] == DBNull.Value)
                    {
                        paramstruct.MaintExtraPercentX = 0;
                    }
                    else
                    {
                        paramstruct.MaintExtraPercentX = (double)drow["MaintenancePercentX"];
                    }

                    if (drow["MaintenanceInflatorY"] == DBNull.Value)
                    {
                        paramstruct.MaintTypeY = 0;
                    }
                    else
                    {
                        paramstruct.MaintTypeY = (int)drow["MaintenanceInflatorY"];
                    }

                    if (drow["PercentageY"] != DBNull.Value)
                    {
                        paramstruct.MaintTypeValueY = Convert.ToDouble(drow["PercentageY"]);
                    }
                    else
                    {
                        paramstruct.MaintTypeValueY = 0;
                    }

                    if (drow["MaintenancePercentY"] == DBNull.Value)
                    {
                        paramstruct.MaintExtraPercentY = 0;
                    }
                    else
                    {
                        paramstruct.MaintExtraPercentY = (double)drow["MaintenancePercentY"];
                    }
                }
            }
            else
            {
                paramstruct.ForecastType = ForecastType.Prod_v_Inflator;
                paramstruct.MaintCalcType = 0;
                paramstruct.MaintTypeX = 0;
                paramstruct.MaintTypeValueX = 0;
                paramstruct.MaintExtraPercentX = 0;
                paramstruct.MaintTypeY = 0;
                paramstruct.MaintTypeValueY = 0;
                paramstruct.MaintExtraPercentY = 0;
            }

            return paramstruct;
        }

        public static MaintParams GetMaintParams(ref DBConnection db, int contractID)
        {
            string sql;
            MaintParams paramstruct = new MaintParams();
            System.Data.DataSet dset;

            sql = "SELECT [ForecastTypeId],[MaintenanceType],[MaintenanceInflatorX],[MaintenancePercentX],[X].[Percentage] AS [PercentageX],[MaintenanceInflatorY],[MaintenancePercentY],[Y].[Percentage] AS [PercentageY] FROM [contract_details]\n";
            sql += "LEFT OUTER JOIN [codes_inflatormetrics] AS [X] ON [contract_details].[MaintenanceInflatorX] = [X].[MetricId]\n";
            sql += "LEFT OUTER JOIN [codes_inflatormetrics] AS [Y] ON [contract_details].[MaintenanceInflatorY] = [Y].[MetricId]";
            sql += "WHERE [ContractId] = @conId";
            db.sqlexecute.Parameters.AddWithValue("@conId", contractID);
            dset = db.GetDataSet(sql);

            if (dset != null && dset.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drow in dset.Tables[0].Rows)
                {
                    if (drow["ForecastTypeId"] == DBNull.Value)
                    {
                        paramstruct.ForecastType = ForecastType.Prod_v_Inflator;
                    }
                    else
                    {
                        paramstruct.ForecastType = (ForecastType)drow["ForecastTypeId"];
                    }
                    if (drow["MaintenanceType"] == DBNull.Value)
                    {
                        paramstruct.MaintCalcType = 0;
                    }
                    else
                    {
                        paramstruct.MaintCalcType = (int)drow["MaintenanceType"];
                    }
                    if (drow["MaintenanceInflatorX"] == DBNull.Value)
                    {
                        paramstruct.MaintTypeX = 0;
                    }
                    else
                    {
                        paramstruct.MaintTypeX = (int)drow["MaintenanceInflatorX"];
                    }

                    if (drow["PercentageX"] != DBNull.Value)
                    {
                        paramstruct.MaintTypeValueX = Convert.ToDouble(drow["PercentageX"]);
                    }
                    else
                    {
                        paramstruct.MaintTypeValueX = 0;
                    }

                    if (drow["MaintenancePercentX"] == DBNull.Value)
                    {
                        paramstruct.MaintExtraPercentX = 0;
                    }
                    else
                    {
                        paramstruct.MaintExtraPercentX = (double)drow["MaintenancePercentX"];
                    }

                    if (drow["MaintenanceInflatorY"] == DBNull.Value)
                    {
                        paramstruct.MaintTypeY = 0;
                    }
                    else
                    {
                        paramstruct.MaintTypeY = (int)drow["MaintenanceInflatorY"];
                    }

                    if (drow["PercentageY"] != DBNull.Value)
                    {
                        paramstruct.MaintTypeValueY = Convert.ToDouble(drow["PercentageY"]);
                    }
                    else
                    {
                        paramstruct.MaintTypeValueY = 0;
                    }

                    if (drow["MaintenancePercentY"] == DBNull.Value)
                    {
                        paramstruct.MaintExtraPercentY = 0;
                    }
                    else
                    {
                        paramstruct.MaintExtraPercentY = (double)drow["MaintenancePercentY"];
                    }
                }
            }
            else
            {
                paramstruct.ForecastType = ForecastType.Prod_v_Inflator;
                paramstruct.MaintCalcType = 0;
                paramstruct.MaintTypeX = 0;
                paramstruct.MaintTypeValueX = 0;
                paramstruct.MaintExtraPercentX = 0;
                paramstruct.MaintTypeY = 0;
                paramstruct.MaintTypeValueY = 0;
                paramstruct.MaintExtraPercentY = 0;
            }

            return paramstruct;
        }

        public static NYMResult CalcNYM(MaintParams mp, int maintYear)
        {
            // This routine assumes that the variables have all been set...
            double resultLP = 0;
            double resultX = 0;
            double resultY = 0;
            double tmpResult = 0;
            NYMResult retVal;
            StringBuilder tmpStr = new StringBuilder();
            string[] calcLine = new string[12];
            string[] headerLine = new string[2];

            headerLine[0] = "NEXT PERIOD COST CALCULATION :- ";
            if (mp.ForecastType == ForecastType.Prod_v_Inflator)
            {
                headerLine[1] = "<b>THE LESSER OF </b>";
            }
            else
            {
                headerLine[1] = "";
            }

            tmpResult = mp.CurMaintVal;

            do
            {
                switch (mp.ForecastType)
                {
                    case ForecastType.Prod_v_Inflator:
                        resultLP = (mp.ListPrice * (mp.PctOfLP / 100));
                        calcLine[1] = "<b>Result 1</b> = " + mp.PctOfLP.ToString().Trim() + "% of Product Cost (" + String.Format("{0:F2}", mp.ListPrice) + ") = " + String.Format("{0:F2}", resultLP);
                        calcLine[2] = "<b>OR</b>";
                        break;
                    case ForecastType.InflatorOnly:
                        resultLP = 0;
                        calcLine[1] = "<b>Result 1</b> = <i>Percentage of Product Cost calculation bypassed</i>";
                        calcLine[2] = "<b>THEREFORE</b>";
                        break;
                    case ForecastType.Staged:
                        break;
                }

                // always use result X for any of the three options
                resultX = tmpResult + (tmpResult * ((mp.MaintTypeValueX + mp.MaintExtraPercentX) / 100));

                switch (mp.MaintCalcType)
                {
                    case 0:  // flat rate - use X vals only
                        calcLine[3] = "Fixed % increase on current cost";
                        calcLine[4] = "<b>Result 2</b> = Current Cost (" + String.Format("{0:F2}", mp.CurMaintVal) + ") x (" + mp.MaintTypeValueX.ToString().Trim() + "% + " + mp.MaintExtraPercentX.ToString().Trim() + "%) = " + String.Format("{0:F2}", resultX);
                        break;
                    case 1:
                    case 2: // greater / lesser of x and y
                        resultY = tmpResult + (tmpResult * ((mp.MaintTypeValueY + mp.MaintExtraPercentY) / 100));
                        if (mp.MaintCalcType == 1)
                        {
                            calcLine[3] = "The Greater of calculations X and Y :- ";
                        }
                        else
                        {
                            calcLine[3] = "The Lesser of calculations X and Y :- ";
                        }

                        calcLine[4] = "<b>Result X</b> = Current Cost (" + String.Format("{0:F2}", mp.CurMaintVal) + ") x (" + mp.MaintTypeValueX.ToString().Trim() + "% + " + mp.MaintExtraPercentX.ToString().Trim() + "%) = " + String.Format("{0:F2}", resultX);
                        calcLine[5] = "<b>AND</b>";
                        calcLine[6] = "<b>Result Y</b> = Current Cost (" + String.Format("{0:F2}", mp.CurMaintVal) + ") x (" + mp.MaintTypeValueY.ToString().Trim() + "% + " + mp.MaintExtraPercentY.ToString().Trim() + "%) = " + String.Format("{0:F2}", resultY);
                        break;
                    default:
                        break;
                }

                // take the desired value
                switch (mp.MaintCalcType)
                {
                    case 0:
                        tmpResult = resultX;
                        break;
                    case 1: // take greater value
                        calcLine[7] = "<b>Result Used (Result 2)</b> = ";
                        if (resultX >= resultY)
                        {
                            if (resultX != 0)
                            {
                                tmpResult = resultX;
                                calcLine[8] = "Result X (" + String.Format("{0:F2}", resultX) + ")";
                            }
                            else
                            {
                                tmpResult = resultY;
                                calcLine[8] = "Result Y " + "(" + String.Format("{0:F2}", resultY) + ")";
                            }
                        }
                        else
                        {
                            if (resultY != 0)
                            {
                                tmpResult = resultY;
                                calcLine[8] = "Result Y (" + String.Format("{0:F2}", resultY) + ")";
                            }
                            else
                            {
                                tmpResult = resultX;
                                calcLine[8] = "Result X (" + String.Format("{0:F2}", resultX) + ")";
                            }
                        }
                        break;
                    case 2: // take lesser value
                        calcLine[7] = "Result 2 = ";
                        if (resultX >= resultY)
                        {
                            if (resultY != 0)
                            {
                                tmpResult = resultY;
                                calcLine[8] = "Result Y (" + String.Format("{0:F2}", resultY) + ")";
                            }
                            else
                            {
                                tmpResult = resultX;
                                calcLine[8] = "Result X (" + String.Format("{0:F2}", resultX) + ")";
                            }
                        }
                        else
                        {
                            if (resultX != 0)
                            {
                                tmpResult = resultX;
                                calcLine[8] = "Result X (" + String.Format("{0:F2}", resultX) + ")";
                            }
                            else
                            {
                                tmpResult = resultY;
                                calcLine[8] = "Result Y (" + String.Format("{0:F2}", resultY) + ")";
                            }
                        }
                        break;
                    default:
                        break;
                }

                maintYear = maintYear - 1;
            }
            while (maintYear >= 0);

            //take the lesser of the calculated value and % of LP
            calcLine[9] = "<b><u>Next Period Cost = ";

            if (resultLP < tmpResult)
            {
                if (resultLP != 0)
                {
                    retVal.NYMValue = resultLP;
                    calcLine[10] = "Result 1 (" + String.Format("{0:F2}", resultLP) + ")";
                }
                else
                {
                    retVal.NYMValue = tmpResult;
                    calcLine[10] = "Result 2 (" + String.Format("{0:F2}", tmpResult) + ")";
                }
            }
            else
            {
                if (tmpResult != 0)
                {
                    retVal.NYMValue = tmpResult;
                    calcLine[10] = "Result 2 (" + String.Format("{0:F2}", tmpResult) + ")";
                }
                else
                {
                    retVal.NYMValue = resultLP;
                    calcLine[10] = "Result 1 (" + String.Format("{0:F2}", resultLP) + ")";
                }
            }

            calcLine[10] = calcLine[10] + "</u></b>";

            // construct HTML table for output of results
            tmpStr.Append("<table class=\"datatbl\">\n");
            tmpStr.Append("<tr>\n");
            tmpStr.Append("<th>" + headerLine[0] + "</td></tr>\n");
            tmpStr.Append("<tr>\n");
            tmpStr.Append("<td class=\"row1\">" + headerLine[1] + "</td></tr>\n");

            string rowClass = "row1";
            bool rowalt = true;

            for (int x = 1; x < 11; x++)
            {
                rowalt = (rowalt ^ true);
                if (rowalt)
                {
                    rowClass = "row1";
                }
                else
                {
                    rowClass = "row2";
                }

                tmpStr.Append("<tr>\n");
                tmpStr.Append("<td class=\"" + rowClass + "\" align=\"center\">" + calcLine[x] + "</td>");
                tmpStr.Append("</tr>\n");
            }

            tmpStr.Append("</table>");

            retVal.NYMCalculation = tmpStr.ToString();

            return retVal;
        }
    }
}
