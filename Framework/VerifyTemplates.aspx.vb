Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class VerifyTemplates
        Inherits System.Web.UI.Page

        Private Enum ErrorType
            None = 0
            CantDelete = 1
            TemplateAssigned = 2
            OtherUnknown = 3
        End Enum

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
            If Me.IsPostBack = False Then
                Try
                    Dim strHTML As New System.Text.StringBuilder
					Dim db As New cFWDBConnection
					Dim actionStatus As ErrorType
                    Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
                    Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
                    Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

                    Title = "Email Templates"
                    Master.title = Title

					db.DBOpen(fws, False)

					Dim action As String
					action = Request.QueryString("action")
					If action = "delete" Then
						actionStatus = DeleteTemplate(db)
					End If

					If actionStatus = ErrorType.None Then
                        litTemplates.Text = GetEmailTemplates(db, curUser)
					Else
						litTemplates.Text = ReportProblem(actionStatus)
					End If

                    lnkUpload.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Emails, False)

					db.DBClose()
					db = Nothing

				Catch ex As Exception
					Dim strError As New System.Text.StringBuilder

					strError.Append("<table class=""datatbl"">" & vbNewLine)
					strError.Append("<tr>" & vbNewLine)
					strError.Append("<th width=""300"">Error Encountered</th>")
					strError.Append("</tr>" & vbNewLine)
					strError.Append("<tr>" & vbNewLine)
					strError.Append("<td class=""row1"">" & ex.Message & "</td>")
					strError.Append("</tr>" & vbNewLine)
					strError.Append("</table>")
					litTemplates.Text = strError.ToString
				End Try
			End If
		End Sub

        Private Function GetEmailTemplates(ByVal db As cFWDBConnection, ByVal curuser As CurrentUser) As String
            Dim strHTML As New System.Text.StringBuilder

            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th><img src=""./icons/16/plain/document_exchange.gif"" /></th>" & vbNewLine)
            strHTML.Append("<th>Template Name</th>" & vbNewLine)
            strHTML.Append("<th>Template Type</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            db.RunSQL("SELECT * FROM [email_templates] ORDER BY [templateName]", db.glDBWorkA, False, "", False)
            Dim drow As DataRow

            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"

            If db.glNumRowsReturned > 0 Then
                For Each drow In db.glDBWorkA.Tables(0).Rows
                    rowalt = (rowalt Xor True)
                    If rowalt = True Then
                        rowClass = "row1"
                    Else
                        rowClass = "row2"
                    End If

                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>")
                    If curuser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Emails, False) Then
                        strHTML.Append("<a onmouseover=""window.status='Delete this uploaded template?';return true;"" onmouseout=""window.status='Done';"" href=""javascript:if(confirm('Are you sure you want to delete this template?'));{window.location.href='VerifyTemplates.aspx?action=delete&id=" & drow.Item("templateId") & "';}""><img src=""./icons/delete2.gif"" /></a>")
                    End If
                    strHTML.Append("</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>")
                    If curuser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Emails, False) Then
                        strHTML.Append("<a onmouseover=""window.status='Replace Template " & drow.Item("templateName") & "';return true"" onmouseout=""window.status='Done';"" onclick=""javascript:document.location='ImportReportFields.aspx?action=emailtemplate&method=replace&id=" & drow.Item("templateId") & "&tname=" & drow.Item("templateName") & "';""><img src=""./icons/16/plain/document_exchange.gif"" /></a>")
                    End If
                    strHTML.Append("</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("templateName") & "</td>" & vbNewLine)
                    strHTML.Append("<td class=""" & rowClass & """>")

                    Select Case CType(drow.Item("templateType"), emailType)
                        Case emailType.AuditCleardown
                            strHTML.Append("Audit Cleardown")
                        Case emailType.ContractReview
                            strHTML.Append("Contract Review")
                        Case emailType.LicenceExpiry
                            strHTML.Append("Licence Expiry")
                        Case emailType.OverdueInvoice
                            strHTML.Append("Overdue Invoice")
                        Case Else
                            strHTML.Append("<b><i>Invalid Type</i></b>")
                    End Select

                    strHTML.Append("</td>" & vbNewLine)
                    strHTML.Append("</tr>" & vbNewLine)
                Next
            Else
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1"" align=""center"" colspan=""4"">No Email Templates are currently uploaded</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
            End If

            strHTML.Append("</table>" & vbNewLine)

            Return strHTML.ToString
        End Function

		Private Function DeleteTemplate(ByVal db As cFWDBConnection) As ErrorType
			Try
				Dim delID As Integer

				delID = Request.QueryString("id")
				If delID > 0 Then
					' check that the template isn't currently assigned to an email schedule
                    db.FWDb("R2", "email_schedule", "templateId", delID, "", "", "", "", "", "", "", "", "", "")
					If db.FWDb2Flag = True Then
						Return ErrorType.TemplateAssigned
						Exit Function
					End If

					' attempt to remove the physical file
                    db.FWDb("R", "email_templates", "templateId", delID, "", "", "", "", "", "", "", "", "", "")
					If db.FWDbFlag = True Then
						Try
                            Dim path As String = Server.MapPath(System.IO.Path.Combine(db.FWDbFindVal("templatePath", 1), db.FWDbFindVal("templateFilename", 1)))

							If System.IO.File.Exists(path) = True Then
								System.IO.File.Delete(path)
							End If

                            db.FWDb("D", "email_templates", "templateId", delID, "", "", "", "", "", "", "", "", "", "")

						Catch ex As Exception
                            'Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
                            'Dim cErr As New FWClasses.cErrors(curUser.currentUser.userInfo, curUser.UserFWS.getConnectionString)
                            'cErr.ReportError(CType(ex, System.Web.HttpException))
                            'cErr = Nothing

							Return ErrorType.CantDelete
							Exit Function
						End Try
					End If
				End If

				Return ErrorType.None

			Catch ex As Exception
                'Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
                'Dim cErr As New FWClasses.cErrors(curUser.currentUser.userInfo, curUser.UserFWS.getConnectionString)
                'cErr.ReportError(CType(ex, System.Web.HttpException))
                'cErr = Nothing

				Return ErrorType.OtherUnknown
			End Try
		End Function

        Private Function ReportProblem(ByVal actionstatus As ErrorType) As String
            Dim strError As New System.Text.StringBuilder

            strError.Append("<table class=""datatbl"">" & vbNewLine)
            strError.Append("<tr>" & vbNewLine)
            strError.Append("<th width=""300"">Error Encountered</th>")
            strError.Append("</tr>" & vbNewLine)
            strError.Append("<tr>" & vbNewLine)
            strError.Append("<td class=""row1"" align=""center"">")
            Select Case actionstatus
                Case ErrorType.None
                    strError.Append("No error encountered.")

                Case ErrorType.CantDelete
                    strError.Append("Unable to delete the physical template file")

                Case ErrorType.OtherUnknown
                    strError.Append("Unknown error encountered trying to delete the template")

                Case ErrorType.TemplateAssigned
                    strError.Append("Template is currently assigned to an active Email Schedule")

                Case Else
                    strError.Append("Unknown error type returned from DeleteTemplate function")

            End Select
            strError.Append("</td>" & vbNewLine)
            strError.Append("</tr>" & vbNewLine)
            strError.Append("</table>")

            Return strError.ToString
        End Function

        Protected Sub lnkUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkUpload.Click
            Response.Redirect("ImportReportFields.aspx?action=emailtemplate", True)
        End Sub

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click

            If (SiteMap.CurrentNode.ParentNode Is Nothing) Then
                Response.Redirect("Home.aspx", True)
            Else
                Response.Redirect(SiteMap.CurrentNode.ParentNode.Url, True)
            End If

        End Sub
    End Class
End Namespace
