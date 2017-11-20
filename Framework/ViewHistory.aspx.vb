Imports FWBase
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class ViewHistory
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
            Dim db As New cFWDBConnection
            Dim recId As Integer
            Dim Htype As String = ""
            Dim sql As New System.Text.StringBuilder
            Dim txtTitle As String = ""
            Dim hasdata As Boolean

            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            hasdata = False

            If Me.IsPostBack = False Then
                recId = Request.QueryString("id")
                Htype = Request.QueryString("type")
                txtTitle = Request.QueryString("header")

                If recId > 0 Then
                    db.DBOpen(fws, False)

                    Select Case Htype
                        Case "maint"
                            lblTitle.Text = "Maintenance History"
                            If txtTitle <> "" Then
                                lblTitle.Text = lblTitle.Text & " : [" & Trim(txtTitle) & "]"
                            End If

                            sql.Append("SELECT [Date Changed],[security].[User Name],[Product Value],[Current Maint],[Next Year Maint],[Comment] FROM [contract_producthistory] " & vbNewLine)
                            sql.Append("LEFT OUTER JOIN [security] ON [contract_producthistory].[User Id] = [security].[User Id] " & vbNewLine)
                            sql.Append("WHERE [Contract-Product Id] = @recId " & vbNewLine)
                            sql.Append("ORDER BY [Date Changed] DESC")
                            db.AddDBParam("@cpId", recId, True)
                            db.RunSQL(sql.ToString, db.glDBWorkA, False, "", False)
                            If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                                hasdata = True
                            End If

                        Case Else

                    End Select

                    db.DBClose()
                End If
            End If

            litHistoryGrid.Text = GetHistory(db, Htype, recId)

            cmdOK.AlternateText = "Close"
            cmdOK.ToolTip = "Exit the history screen"
            cmdOK.Attributes.Add("onmouseover", "window.status='Exit the history screen';return true;")
            cmdOK.Attributes.Add("onmouseout", "window.status='Done';")

            db = Nothing
        End Sub

		Private Function GetHistory(ByVal db As cFWDBConnection, ByVal HistoryType As String, ByVal id As Integer) As String
			Dim tmpStr As String
			Dim drow As DataRow
			Dim strHTML As New System.Text.StringBuilder

			strHTML.Append("<table class=""datatbl"">" & vbNewLine)
			' headers
			strHTML.Append("<tr>" & vbNewLine)
			Select Case HistoryType
				Case "maint"
					strHTML.Append("<th width=""80"">Date Changed</th>" & vbNewLine)
					strHTML.Append("<th width=""80"">Changed By</th>" & vbNewLine)
					strHTML.Append("<th width=""100"">Product Cost</th>" & vbNewLine)
					strHTML.Append("<th width=""100"">Current Maint</th>" & vbNewLine)
					strHTML.Append("<th width=""100"">Next Period<br>Maint</th>" & vbNewLine)
					strHTML.Append("<th width=""200"">Comment</th>" & vbNewLine)

				Case Else
					strHTML.Append("<th>ERROR</th>" & vbNewLine)

			End Select
			strHTML.Append("</tr>" & vbNewLine)

			Dim rowalt As Boolean = False
			Dim rowClass As String = "row1"

			' history rows
			Select Case HistoryType
				Case "maint"
					If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
						For Each drow In db.glDBWorkA.Tables(0).Rows
							rowalt = (rowalt Xor True)
							If rowalt = True Then
								rowClass = "row1"
							Else
								rowClass = "row2"
							End If

							strHTML.Append("<tr>" & vbNewLine)
							strHTML.Append("<td class=""" & rowClass & """>" & Format(drow.Item("Date Changed"), cDef.DATE_FORMAT) & "</td>" & vbNewLine)
							If IsDBNull(drow.Item("User Name")) = True Then
								tmpStr = "&nbsp;"
							Else
								tmpStr = drow.Item("User Name")
							End If
							strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
							If IsDBNull(drow.Item("Product Value")) = True Then
								tmpStr = "&nbsp;"
							Else
								tmpStr = drow.Item("Product Value")
							End If
							strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
							If IsDBNull(drow.Item("Current Maint")) = True Then
								tmpStr = "Unknown"
							Else
								tmpStr = drow.Item("Current Maint")
							End If
							strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
							If IsDBNull(drow.Item("Next Year Maint")) = True Then
								tmpStr = "Unknown"
							Else
								tmpStr = drow.Item("Next Year Maint")
							End If
							strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
							If IsDBNull(drow.Item("Comment")) = True Then
								tmpStr = "<textarea readonly>&nbsp;</textarea>"
							Else
								tmpStr = "<textarea readonly rows=""3"" cols=""20"">" & drow.Item("Comment") & "</textarea>"
							End If
							strHTML.Append("<td class=""" & rowClass & """>" & tmpStr & "</td>" & vbNewLine)
							strHTML.Append("</tr>" & vbNewLine)
						Next
					Else
						strHTML.Append("<tr><td class=""row1"" align=""center"" colspan=""6"">No history data to display</td></tr>" & vbNewLine)
					End If

				Case Else
					strHTML.Append("<tr><td class=""row1"" align=""center"">Unknown history type specified</td></tr>" & vbNewLine)
			End Select

			strHTML.Append("</table>" & vbNewLine)
			GetHistory = strHTML.ToString
		End Function

        Private Sub ExitHistory()
            Response.Redirect(Session("ReturnURL"), True)
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            ExitHistory()
        End Sub
    End Class

End Namespace
