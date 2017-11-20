using System;
using System.Data;

using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

using Spend_Management;

public partial class broadcastProvider : System.Web.UI.Page
{
    string page;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            cEmployees clsemployees = new cEmployees(user.AccountID);
            Employee employee = clsemployees.GetEmployeeById(user.EmployeeID);
            

            int broadcastid = 0;

            if (Request.Form["broadcastid"] != null)
            {
                page = Request.Form["page"];
                broadcastLocation location = broadcastLocation.notSet;
                switch (page.ToLower())
                {
                    case "home.aspx":
                        location = broadcastLocation.HomePage;
                        break;
                    case "submitclaim.aspx":
                        location = broadcastLocation.SubmitClaim;
                        break;
                }

                
                broadcastid = int.Parse(Request.Form["broadcastid"]);
                cBroadcastMessages clsmessages = new cBroadcastMessages(user.AccountID);
                DataTable broadcast = clsmessages.getMessagesToDisplay(location, employee);

                if (broadcastid != 0)
                {
                    //see if the broadcastid exists or they've hit back
                    DataRow[] temp = broadcast.Select("broadcastid=" + broadcastid);
                    if (temp.GetLength(0) == 0)
                    {
                        broadcastid = 0;
                    }
                }
                if (broadcast.Rows.Count != 0 || broadcastid != 0)
                {
                    if (broadcastid == 0)
                    {
                        broadcastid = (int)broadcast.Rows[0]["broadcastid"];

                    }
                    DataRow[] currentMessage = broadcast.Select("broadcastid=" + broadcastid);
                    int previous = 0;
                    int next = 0;
                    for (int index = 0; index < broadcast.Rows.Count; index++)
                    {
                        if ((int)broadcast.Rows[index]["broadcastid"] == broadcastid)
                        {
                            if (index == 0)
                            {
                                previous = 0;
                            }
                            else
                            {
                                previous = (int)broadcast.Rows[index - 1]["broadcastid"];
                            }
                            if (index == (broadcast.Rows.Count - 1))
                            {
                                next = 0;
                            }
                            else
                            {
                                next = (int)broadcast.Rows[index + 1]["broadcastid"];
                            }
                            break;
                        }
                    }
                    if ((bool)currentMessage[0]["expirewhenread"] == true)
                    {
                        employee.GetBroadcastMessagesRead().Add(broadcastid);
                    }
                    if ((bool)currentMessage[0]["oncepersession"] == true)
                    {
                        Session["broadcast" + currentMessage[0]["broadcastid"]] = 1;
                    }
                    Response.Write(createBroadcastMessage(previous, next, currentMessage));
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    Response.Write("empty");
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }

    private string createBroadcastMessage(int previousid, int nextid, DataRow[] currentMessage)
    {
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("<div id=\"broadcastheader\">");
        output.Append("<span style=\"margin-left: 5px; margin-top: 6px; font-size: 24px;\">Broadcast Message</span>");

        if (previousid != 0)
        {
            output.Append("<img style=\"cursor: pointer; cursor: hand;\" onclick=\"displayBroadcastMessage(" + previousid + ",'" + cMisc.Path + "/broadcastprovider.aspx','" + page + "');\" id=\"broadcastleft\" src=\"" + cMisc.Path + "/icons/24/plain/arrow_left_green.gif\" onmouseover=\"document.getElementById('broadcastleft').src = '" + cMisc.Path + "/icons/24/shadow/arrow_left_green.gif';\" onmouseout=\"document.getElementById('broadcastleft').src = '" + cMisc.Path + "/icons/24/plain/arrow_left_green.gif';\">");
        }
        else
        {
            output.Append("<img src=\"" + cMisc.Path + "/icons/24/plain/arrow_left_green_grey.gif\" id=\"broadcastleft\">");
        }

        if (nextid != 0)
        {
            output.Append("<img style=\"cursor: pointer; cursor: hand;\" onclick=\"displayBroadcastMessage(" + nextid + ",'" + cMisc.Path + "/broadcastprovider.aspx','" + page + "');\" id=\"broadcastright\" src=\"" + cMisc.Path + "/icons/24/plain/arrow_right_green.gif\" onmouseover=\"document.getElementById('broadcastright').src = '" + cMisc.Path + "/icons/24/shadow/arrow_right_green.gif';\" onmouseout=\"document.getElementById('broadcastright').src = '" + cMisc.Path + "/icons/24/plain/arrow_right_green.gif';\">");
        }
        else
        {
            output.Append("<img src=\"" + cMisc.Path + "/icons/24/plain/arrow_right_green_grey.gif\" id=\"broadcastright\">");
        }

        output.Append("<img style=\"curor:pointer; cursor:hand;\" onclick=\"document.getElementById('broadcastmsg').style.display = 'none';\" id=\"broadcastclose\" src=\"" + cMisc.Path + "/icons/24/plain/delete2.gif\" onmouseover=\"document.getElementById('broadcastclose').src = '" + cMisc.Path + "/icons/24/shadow/delete2.gif';\" onmouseout=\"document.getElementById('broadcastclose').src = '" + cMisc.Path + "/icons/24/plain/delete2.gif';\">");
        output.Append("</div>");
        output.Append("<div id=\"broadcasttxt\">");
        output.Append("<div id=\"broadcasttitle\">" + currentMessage[0]["title"] + "</div>");
        output.Append("<hr style=\"color: #ccc; line-height: 3px; border-width: 1px; height: 1px;\">");
        output.Append(currentMessage[0]["message"]);
        output.Append("</div>");
        return output.ToString();
    }
}
