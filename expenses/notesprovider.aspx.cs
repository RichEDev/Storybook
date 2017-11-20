using System;
using System.Data;

using SpendManagementLibrary.Employees;

using Spend_Management;

public partial class notesprovider : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
        int noteid = 0;
        CurrentUser user = cMisc.GetCurrentUser();
        ViewState["accountid"] = user.AccountID;
        ViewState["employeeid"] = user.EmployeeID;
        cEmployees clsemployees = new cEmployees(user.AccountID);
        Employee reqemp = clsemployees.GetEmployeeById(user.EmployeeID);
        if (Request.Form["noteid"] != null)
        {
            noteid = int.Parse(Request.Form["noteid"]);
        }

        DataTable notes = clsemployees.getNotes(reqemp.EmployeeID);
        if (noteid != 0)
        {
            //see if the broadcastid exists or they've hit back
            DataRow[] temp = notes.Select("noteid=" + noteid);
            if (temp.GetLength(0) == 0)
            {
                noteid = 0;
            }
        }
        if (notes.Rows.Count != 0 || noteid != 0)
        {
            System.Text.StringBuilder noteOutput = new System.Text.StringBuilder();
            if (noteid == 0)
            {
                noteid = (int)notes.Rows[0]["noteid"];

            }
            DataRow[] currentMessage = notes.Select("noteid=" + noteid);
            int previous = 0;
            int next = 0;
            for (int index = 0; index < notes.Rows.Count; index++)
            {
                if ((int)notes.Rows[index]["noteid"] == noteid)
                {
                    if (index == 0)
                    {
                        previous = 0;
                    }
                    else
                    {
                        previous = (int)notes.Rows[index - 1]["noteid"];
                    }
                    if (index == (notes.Rows.Count - 1))
                    {
                        next = 0;
                    }
                    else
                    {
                        next = (int)notes.Rows[index + 1]["noteid"];
                    }
                    break;
                }
            }

            clsemployees.markNoteAsRead(noteid);


            Response.Write(createNotesMessage(previous, next, currentMessage));
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

    private string createNotesMessage(int previousid, int nextid, DataRow[] currentMessage)
    {
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("<div id=\"notesheader\">");
        output.Append("<span style=\"margin-left: 5px; margin-top: 6px; font-size: 24px; color: #f0f0f0;\">New Note</span>");

        if (previousid != 0)
        {
            output.Append("<img style=\"cursor: pointer; cursor: hand;\" onclick=\"displayNotes(" + previousid + ");\" id=\"notesleft\" src=\"" + cMisc.Path + "/icons/24/plain/arrow_left_green.gif\" onmouseover=\"document.getElementById('notesleft').src = '" + cMisc.Path + "/icons/24/shadow/arrow_left_green.gif';\" onmouseout=\"document.getElementById('notesleft').src = '" + cMisc.Path + "/icons/24/plain/arrow_left_green.gif';\">");
        }
        else
        {
            output.Append("<img src=\"" + cMisc.Path + "/icons/24/plain/arrow_left_green_grey.gif\" id=\"notesleft\">");
        }

        if (nextid != 0)
        {
            output.Append("<img style=\"cursor: pointer; cursor: hand;\" onclick=\"displayNotes(" + nextid + ");\" id=\"notesright\" src=\"" + cMisc.Path + "/icons/24/plain/arrow_right_green.gif\" onmouseover=\"document.getElementById('notesright').src = '" + cMisc.Path + "/icons/24/shadow/arrow_right_green.gif';\" onmouseout=\"document.getElementById('notesright').src = '" + cMisc.Path + "/icons/24/plain/arrow_right_green.gif';\">");
        }
        else
        {
            output.Append("<img src=\"" + cMisc.Path + "/icons/24/plain/arrow_right_green_grey.gif\" id=\"broadcastright\">");
        }

        DateTime date = (DateTime)currentMessage[0]["datestamp"];
        output.Append("<img style=\"curor:pointer; cursor:hand;\" onclick=\"document.getElementById('notes').style.display = 'none';\" id=\"notesclose\" src=\"" + cMisc.Path + "/icons/24/plain/delete2.gif\" onmouseover=\"document.getElementById('notesclose').src = '" + cMisc.Path + "/icons/24/shadow/delete2.gif';\" onmouseout=\"document.getElementById('notesclose').src = '" + cMisc.Path + "/icons/24/plain/delete2.gif';\">");
        output.Append("</div>");
        output.Append("<div id=\"notestxt\">");
        output.Append("<div id=\"notestitle\">Message Sent: " + date.ToShortDateString() + " " + date.ToString("HH:mm:ss") + "</div>");
        output.Append("<hr style=\"color: #ccc; line-height: 3px; border-width: 1px; height: 1px;\">");
        output.Append(currentMessage[0]["note"]);
        output.Append("</div>");
        return output.ToString();
    }
}
