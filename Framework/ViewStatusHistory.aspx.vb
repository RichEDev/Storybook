Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class ViewStatusHistory
        Inherits System.Web.UI.Page
    
#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Dim FWDb As New cFWDBConnection
			Dim InvId As Integer
			Dim sql, InvNumber As String
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.AccountID).getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim hasdata As Boolean

            FWDb.DBOpen(fws, False)

			If Request.Form("hiddenComment") <> "" Then
				UpdateComment(FWDb)
			End If

			If Me.IsPostBack = False Then
				Title = "Invoice Status History"
				Master.title = Title

				InvId = Request.QueryString("id")
				hiddenInvID.Text = Str(InvId)
				InvNumber = Request.QueryString("desc")
				hiddenDesc.Text = InvNumber

				If InvId = 0 Then
					FWDb.DBClose()
					FWDb = Nothing
					Response.Redirect("ContractSummary.aspx?id=" & Session("ActiveContract") & "&tab=" & SummaryTabs.InvoiceDetail, True)
				End If

				lblTitle.Text = " - " & InvNumber

				' obtain history for the selected invoice
                sql = "SELECT [invoiceLogID],[employees].[Firstname] + ' ' + employees.[Surname] as fullname,[dateChanged],[invoiceStatusType].[description],[invoiceStatusType].[archived],[Comment] FROM [invoiceLog]" & vbNewLine & _
                "INNER JOIN [employees] ON [invoiceLog].[userID] = [employees].[employeeid]" & vbNewLine & _
                "INNER JOIN [invoiceStatusType] ON [invoiceLog].[invoiceStatus] = [invoiceStatusType].[invoiceStatusTypeId]" & vbNewLine & _
                "WHERE [invoiceID] = @InvId ORDER BY [dateChanged]"
				FWDb.AddDBParam("InvId", InvId, True)
				FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

				If FWDb.glNumRowsReturned > 0 Then
					hasdata = True
				End If
			End If

			cmdClose.AlternateText = "Close"
			cmdClose.ToolTip = "Exit the status history screen"
			cmdClose.Attributes.Add("onmouseover", "window.status='Exit the status history screen';return true;")
			cmdClose.Attributes.Add("onmouseout", "window.status='Done';")

			litStatusData.Text = GetStatusData(FWDb, hasdata)

			FWDb.DBClose()
			FWDb = Nothing
		End Sub

		Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
			Response.Redirect("ContractSummary.aspx?id=" & Session("ActiveContract") & "&tab=" & SummaryTabs.InvoiceDetail, True)
		End Sub

		Private Function GetStatusData(ByVal db As cFWDBConnection, ByVal hasdata As Boolean) As String
			Dim strHTML As New System.Text.StringBuilder
			Dim drow As DataRow
			Dim tmpstr As String

			strHTML.Append(vbNewLine & "<script language=""javascript"" type=""text/javascript"">" & vbNewLine)
			strHTML.Append("function UpdateComment(logid,invid,desc) {" & vbNewLine)
			strHTML.Append("var ctl;" & vbNewLine)
			strHTML.Append("document.getElementById('hiddenComment').value = ctl.value;" & vbNewLine)
			strHTML.Append("document.submit();")
			strHTML.Append("}" & vbNewLine)
			strHTML.Append("</script>" & vbNewLine)

			strHTML.Append("<table class=""datatbl"">" & vbNewLine)
			strHTML.Append("<tr>" & vbNewLine)
			strHTML.Append("<th>Updated by</th>" & vbNewLine)
			strHTML.Append("<th>Date Changed</th>" & vbNewLine)
			strHTML.Append("<th>Status Description</th>" & vbNewLine)
			strHTML.Append("<th>Is Archive State?</th>" & vbNewLine)
			strHTML.Append("<th>Comment</b></th>" & vbNewLine)
			strHTML.Append("</tr>" & vbNewLine)

			If hasdata = True Then
				Dim rowalt As Boolean = False
				Dim rowClass As String = "row1"

				For Each drow In db.glDBWorkA.Tables(0).Rows
					rowalt = (rowalt Xor True)
					If rowalt = True Then
						rowClass = "row1"
					Else
						rowClass = "row2"
					End If

					strHTML.Append("<tr>" & vbNewLine)
                    tmpstr = IIf(IsDBNull(drow.Item("fullname")) = True, "&nbsp;", Trim(drow.Item("fullname")))
					strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
                    tmpstr = IIf(IsDBNull(drow.Item("dateChanged")) = True, "&nbsp;", Trim(Format(CDate(drow.Item("dateChanged")), cDef.DATE_FORMAT)))
					strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
                    tmpstr = IIf(IsDBNull(drow.Item("description")) = True, "&nbsp;", Trim(drow.Item("description")))
					strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
                    tmpstr = IIf(drow.Item("archived") = "0", "No", "Yes")
					strHTML.Append("<td class=""" & rowClass & """>" & tmpstr & "</td>" & vbNewLine)
					tmpstr = IIf(IsDBNull(drow.Item("Comment")) = True, "", Trim(drow.Item("Comment")))
                    strHTML.Append("<td class=""" & rowClass & """><textarea cols=""25"" rows=""2"" onchange=""UpdateComment('" & Trim(drow.Item("invoiceLogID")) & "','" & Trim(hiddenInvID.Text) & "','" & Trim(hiddenDesc.Text) & "');"" name=""TXT" & Trim(drow.Item("invoiceLogID")) & """>" & tmpstr & "</textarea></td>" & vbNewLine)
					strHTML.Append("</tr>" & vbNewLine)
				Next
			Else
				strHTML.Append("<tr>" & vbNewLine)
				strHTML.Append("<td class=""row1"" colspan=""5"">No status information to display</td>" & vbNewLine)
				strHTML.Append("</tr>" & vbNewLine)
			End If

			strHTML.Append("</table>" & vbNewLine)

			GetStatusData = strHTML.ToString
		End Function

		Private Sub UpdateComment(ByVal db As cFWDBConnection)
			Dim tmpComment As String
			Dim tmpLogId As String
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.AccountID).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.InvoiceStatusHistory, curUser.CurrentSubAccountId)
			Dim ARec As New cAuditRecord

			tmpComment = Request.Form("hiddenComment")
			tmpLogId = Request.Form("hiddenLogId")
			'tmpComment = hiddenCommentText.Text

            db.FWDb("R2", "invoiceLog", "InvoiceLogId", tmpLogId, "", "", "", "", "", "", "", "", "", "")
			If db.FWDb2Flag = True Then
				ARec.Action = cFWAuditLog.AUDIT_UPDATE
				ARec.DataElementDesc = "INV. STATUS HISTORY"
				ARec.ElementDesc = "Comment"
				ARec.PreVal = Trim(Left(db.FWDbFindVal("Comment", 2), 60))
				ARec.PostVal = Trim(Left(tmpComment, 60))
				ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, tmpLogId)

				db.SetFieldValue("Comment", tmpComment, "S", True)
                db.FWDb("A", "invoiceLog", "invoiceLogId", tmpLogId, "", "", "", "", "", "", "", "", "", "")
			End If
		End Sub
    End Class

End Namespace
