using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Web.Script.Services;
using SpendManagementLibrary;
using System.Net.Mail;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    using System.Net;

    /// <summary>
    /// Summary description for svcEmailTemplates
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcEmailTemplates : WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sFieldBasics> GetTableFields(string tableID, bool update)
        {
            Guid gTableID = new Guid(tableID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<sFieldBasics> lstFields = new List<sFieldBasics>();
            cFields clsFields = new cFields(currentUser.AccountID);
            lstFields = clsFields.GetFieldBasicsByTableID(gTableID, update);
            return lstFields;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<CustomEntityEmailAttribute> GetGreenLightAttributes(string baseTableString)
        {
            var result = new List<CustomEntityEmailAttribute>();
            if (User.Identity.IsAuthenticated)
            {
                var user = cMisc.GetCurrentUser();
                var tableId = new Guid(baseTableString);
                var employeesTable = new Guid("618DB425-F430-4660-9525-EBAB444ED754");
                var fields = new cFields(user.AccountID);

                var customEntities = new cCustomEntities(user);
                var entity = customEntities.getEntityByTableId(tableId);
                if (entity == null)
                {
                    return result;
                }

                foreach (cAttribute attribute in entity.attributes.Values)
                {
                    if (attribute.fieldtype == FieldType.Contact)
                    {
                        var customEntityEmailAttribute = new CustomEntityEmailAttribute
                                                             {
                                                                 Id = attribute.fieldid,
                                                                 Name = attribute.displayname,
                                                                 Owner = string.Empty
                                                             };
                        result.Add(customEntityEmailAttribute);
                    }
                    if (attribute.fieldtype == FieldType.Relationship && attribute is cManyToOneRelationship)
                    {
                        var relatedTable = ((cManyToOneRelationship)attribute).relatedtable;
                        if (relatedTable != null && relatedTable.TableID == employeesTable)
                        {
                            var targetField = fields.GetFieldByTableAndFieldName(relatedTable.TableID, "email");
                            var customEntityEmailAttribute = new CustomEntityEmailAttribute
                                                                 {
                                                                     Id = attribute.fieldid,
                                                                     Name =
                                                                         targetField
                                                                         .Description,
                                                                     Owner =
                                                                         attribute.displayname
                                                                 };

                            result.Add(customEntityEmailAttribute);
                        }
                    }
                }
            }
            return result;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<TokenInputResult> getRecipientInfo(string username, int teamid, int budgetid, string otherSenderType, string otherVal, string fieldId)
        {
            var tokenInputResults = new List<TokenInputResult>();

            if (User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();

                if (username.Length > 0)
                {
                    cEmployees clsemps = new cEmployees(user.AccountID);
                    int empid = clsemps.getEmployeeidByUsername(user.AccountID, username);
                    Employee reqemp = clsemps.GetEmployeeById(empid);

                    if (reqemp != null)
                    {
                        tokenInputResults.Add(new TokenInputResult
                        {
                            id = reqemp.EmailAddress + "; ",
                            name = reqemp.EmailAddress,
                        });
                    }
                }

                if (teamid > 0)
                {
                    cTeams clsteams = new cTeams(user.AccountID);
                    cTeam team = clsteams.GetTeamById(teamid);

                    tokenInputResults.Add(new TokenInputResult
                    {
                        id = "{Team: " + team.teamname + "}; ",
                        name = "Team: " + team.teamname
                    });
                }

                if (budgetid > 0)
                {
                    cBudgetholders clsbudget = new cBudgetholders(user.AccountID);
                    cBudgetHolder budget = clsbudget.getBudgetHolderById(budgetid);

                    tokenInputResults.Add(new TokenInputResult
                    {
                        id = "{Budget Holder: " + budget.budgetholder + "}; ",
                        name = "Budget Holder: " + budget.budgetholder
                    });
                }

                if (otherSenderType != "")
                {
                    tokenInputResults.Add(new TokenInputResult
                    {
                        id = "{" + otherVal + "}; ",
                        name = otherVal
                    });
                }

                if (!string.IsNullOrWhiteSpace(fieldId))
                {
                    if (fieldId != Convert.ToString(Guid.Empty))
                    {
                        var customEntities = new cCustomEntities(user);
                        var greenLightAttribute = customEntities.getAttributeByFieldId(new Guid(fieldId));

                        tokenInputResults.Add(new TokenInputResult
                        {
                            id = "{GreenLight: " + greenLightAttribute.displayname + "}; ",
                            name = "GreenLight: " + greenLightAttribute.displayname
                        });
                    }
                }
            }

            return tokenInputResults;
        }

        /// <summary>
        /// Save an Email template
        /// </summary>
        /// <param name="emailtemplateid">The current template ID or zero for a new template</param>
        /// <param name="templatename">The name of the Template</param>
        /// <param name="toVal">The To parameter</param>
        /// <param name="ccVal">The Carbon Copy Parameter</param>
        /// <param name="bccVal">The Blind Carbon Copy parameter</param>
        /// <param name="subject">The template subject</param>
        /// <param name="priority">Template priority</param>
        /// <param name="body">The body text of the template</param>
        /// <param name="basetableid">The ID (Guid) of the Base table used to populate placeholders in the body</param>
        /// <param name="sysTemp">True if this is a system template</param>
        /// <param name="issendnote">True if the template should create a note</param>
        /// <param name="notes">The notes</param>
        /// <param name="update">True if this is an update of an existing template</param>
        /// <param name="isSendEmail">True if this template should generate an email.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int saveEmailTemplate(int emailtemplateid, string templatename, string toVal, string ccVal, string bccVal, string subject, byte priority, string body, Guid basetableid, bool? sysTemp, bool issendnote, string notes, bool update, bool isSendEmail, bool canSendMobileNotification, string mobileNotificationMessage)
        {
            int id = 0;
            bool isSystemTemplate = false;

            if (User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                cEmailTemplates clsemailtemps = new cEmailTemplates(user);
                sBuildDetails buildSubject = new sBuildDetails();
                MailPriority emailPriority = (MailPriority)priority;
                sBuildDetails buildBody = new sBuildDetails();
                sBuildDetails buildNote = new sBuildDetails();

                cEmailTemplate emailTemp;

                List<sSendDetails> lstRecipientTypes = new List<sSendDetails>();
               
                
                if (ccVal != "")
                {
                    clsemailtemps.getRecipientEmailFields(ref lstRecipientTypes, ccVal, recipientType.cc, basetableid);
                }

                if (bccVal != "")
                {
                    clsemailtemps.getRecipientEmailFields(ref lstRecipientTypes, bccVal, recipientType.bcc, basetableid);
                }

                buildSubject.details = WebUtility.HtmlDecode(subject);
                buildSubject.fieldDetails = clsemailtemps.getFieldsFromText(subject, basetableid, user);
                buildBody.details = WebUtility.HtmlDecode(body);
                buildBody.fieldDetails = clsemailtemps.getFieldsFromText(body, basetableid, user);
                buildNote.details = WebUtility.HtmlDecode(notes);
                buildNote.fieldDetails = clsemailtemps.getFieldsFromText(notes, basetableid, user);

                if (sysTemp == null)
                {
                    var emailTemplate = clsemailtemps.getEmailTemplateById(emailtemplateid);
                    if (emailTemplate != null)
                    {
                        isSystemTemplate = emailTemplate.SystemTemplate;
                    }
                }
                else
                {
                    isSystemTemplate = (bool)sysTemp;
                }
                if (update && isSystemTemplate)
                {
                    var oldTemplate = clsemailtemps.getEmailTemplateById(emailtemplateid);
                    if (!new cEmployees(user.AccountID).GetEmployeeById(user.EmployeeID).AdminOverride) isSystemTemplate = oldTemplate.SystemTemplate;
                    if(oldTemplate.RecipientTypes!= null)
                    lstRecipientTypes.AddRange(oldTemplate.RecipientTypes.Where(sendDet => sendDet.recType == recipientType.to));
                }
                else
                {
                    clsemailtemps.getRecipientEmailFields(ref lstRecipientTypes, toVal, recipientType.to, basetableid);
                }
                if (emailtemplateid == 0)
                {
                    emailTemp = new cEmailTemplate(emailtemplateid, templatename, lstRecipientTypes, buildSubject, buildBody, isSystemTemplate, emailPriority, basetableid, DateTime.UtcNow, user.EmployeeID, null, null, isSendEmail, true, null, issendnote, buildNote, null, canSendMobileNotification, mobileNotificationMessage);
                }
                else
                {
                    emailTemp = new cEmailTemplate(emailtemplateid, templatename, lstRecipientTypes, buildSubject, buildBody, isSystemTemplate, emailPriority, basetableid, new DateTime(1900, 01, 01), 0, DateTime.UtcNow, user.EmployeeID, isSendEmail, true, null, issendnote, buildNote, null, canSendMobileNotification, mobileNotificationMessage);
                }
               
                id = clsemailtemps.saveEmailTemplate(emailTemp, update);
            }

            return id;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void deleteEmailTemplate(int emailtemplateid)
        {
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                cEmailTemplates clsemailtemps = new cEmailTemplates(user);
                clsemailtemps.deleteEmailTemplate(emailtemplateid);
            }
        }
    }

    public class CustomEntityEmailAttribute
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
    }
}
