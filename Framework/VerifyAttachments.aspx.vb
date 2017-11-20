Imports System.IO
Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class VerifyAttachments
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
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim attHTML As String = ""
			Dim displayOK As Boolean = False

			If Me.IsPostBack = False Then
				Dim db As New cFWDBConnection

				Title = "Verify Attachments"
				Master.title = Title

				db.DBOpen(fws, False)

				' check if any action is being requested
				Select Case Request.QueryString("a")
					Case "u" ' upload
						doUpload(db, Val(Request.QueryString("id")))
						displayOK = True

					Case "r" ' repair
						'doRepair(db, Val(Request.QueryString("id")))
						'displayOK = True

					Case "d" ' delete link
						doDelete(db, Val(Request.QueryString("id")))
						attHTML = GetVerifyList(db, fws)

					Case Else
						attHTML = GetVerifyList(db, fws)
				End Select

				litAttachments.Text = attHTML


				db.DBClose()
				db = Nothing
			End If

			cmdCancel.AlternateText = "Close"
			cmdCancel.ToolTip = "Exit the verification screen"
			cmdCancel.Attributes.Add("onmouseover", "window.status='Exit the verification screen';return true;")
			cmdCancel.Attributes.Add("onmouseout", "window.status='Done';")

			cmdOK.AlternateText = "OK"
			cmdOK.ToolTip = "Confirm attachment load"
			cmdOK.Attributes.Add("onmouseover", "window.status='Confirm attachment load';return true;")
			cmdOK.Attributes.Add("onmouseout", "window.status='Done';")
			cmdOK.Visible = displayOK
		End Sub

		Private Function GetVerifyList(ByVal db As cFWDBConnection, ByVal fws As cFWSettings) As String
			Dim strHTML As New System.Text.StringBuilder
			Dim sql As String
			Dim drow As DataRow
			Dim fullPath As String
			Dim validPath, virtualPath As Boolean
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)

            sql = "SELECT * FROM [attachments] WHERE [AttachmentType] <> @attType ORDER BY [Description]"
			db.AddDBParam("attType", AttachmentType.Hyperlink, True)
			db.RunSQL(sql, db.glDBWorkA, False, "", False)

			With strHTML
                .Append("<div style=""overflow: auto; height: 550px;"" >" & vbNewLine)
				.Append("<table class=""datatbl"" width=""750"">" & vbNewLine)
				.Append("<thead>" & vbNewLine)
				.Append("<tr>" & vbNewLine)
				.Append("<th>&nbsp;</th>" & vbNewLine)
				.Append("<th width=""300"">File Path</th>" & vbNewLine)
				.Append("<th width=""190"">Description</th>" & vbNewLine)
				.Append("<th width=""60"">Action</th>" & vbNewLine)
				.Append("<th width=""60"">Att. Area</th>" & vbNewLine)
				.Append("<th width=""70"">Remove</th>" & vbNewLine)
				.Append("</tr>" & vbNewLine)
				.Append("</thead>" & vbNewLine)
				.Append("<tfoot>")
				.Append("</tfoot>" & vbNewLine)

				.Append("<tbody>" & vbNewLine)

				Dim rowalt As Boolean = False
				Dim rowClass As String = "row1"

				For Each drow In db.glDBWorkA.Tables(0).Rows
					rowalt = (rowalt Xor True)
					If rowalt = True Then
						rowClass = "row1"
					Else
						rowClass = "row2"
					End If

					fullPath = Path.Combine(drow.Item("directory"), drow.Item("Filename")).Replace("`", "'")

					.Append("<tr>" & vbNewLine)
					.Append("<td class=""" & rowClass & """>")

					' check to see if the path is already a web virtual directory or not
					If InStr(fullPath, fws.glDocRepository, CompareMethod.Text) > 0 Then
						virtualPath = True
					Else
						If InStr(fullPath, fws.glSecureDocRepository, CompareMethod.Text) > 0 Then
							virtualPath = True
						Else
							virtualPath = False
						End If
					End If

					If virtualPath = True Then
						Try
							If File.Exists(Server.MapPath(fullPath)) = True Then
								validPath = True
							Else
								validPath = False
							End If
						Catch ex As Exception
							validPath = False
						End Try

					Else
						validPath = False
					End If

					If validPath = True Then
						.Append("<img alt=""valid"" src=""./buttons/tick.gif"" />")
					Else
						.Append("<img alt=""invalid"" src=""./buttons/cross.gif"" />")
					End If

					.Append("</td>" & vbNewLine)
					.Append("<td class=""" & rowClass & """>")
					.Append(fullPath)
					.Append("</td>" & vbNewLine)
					.Append("<td class=""" & rowClass & """>")
					If IsDBNull(drow.Item("Description")) = False Then
						.Append(drow.Item("Description"))
					Else
						.Append("")
					End If
					.Append("</td>" & vbNewLine)
					.Append("<td class=""" & rowClass & """>" & vbNewLine)
					If validPath = False Then
						If virtualPath = False Then
                            .Append("<a href=""VerifyAttachments.aspx?id=" & CStr(drow.Item("AttachmentId")) & "&a=u"">Upload</a>")
						Else
							.Append("<b>Unresolved</b>")
							'    .Append("<a class=""hyperlink"" href=""VerifyAttachments.aspx?id=" & CStr(drow.Item("Attachment Id")) & "&a=r"">Repair</a>")
						End If
					Else
						.Append("n/a")
					End If
					.Append("</td>" & vbNewLine)

					.Append("<td class=""" & rowClass & """>")
                    Select Case CType(drow.Item("AttachmentArea"), AttachmentArea)
                        Case AttachmentArea.CONTRACT
                            .Append("Contract")
                        Case AttachmentArea.CONTACT_NOTES
                            .Append("Contract Notes")
                        Case AttachmentArea.INVOICE_NOTES
                            .Append("Invoice Notes")
                        Case AttachmentArea.PRODUCT_NOTES
                            .Append("Product Notes")
                        Case AttachmentArea.CONTRACT_NOTES
                            .Append("Contract Notes")
                        Case AttachmentArea.VENDOR
                            .Append("Supplier")
                        Case AttachmentArea.VENDOR_NOTES
                            .Append("Supplier Notes")
                        Case Else
                            .Append("<i>Unknown</i>")
                    End Select
					.Append("</td>" & vbNewLine)

					If validPath = False Then
                        .Append("<td class=""" & rowClass & """><a href=""VerifyAttachments.aspx?a=d&id=" & CStr(drow.Item("AttachmentId")) & """>Delete Link</a></td>" & vbNewLine)
					Else
						.Append("<td class=""" & rowClass & """>&nbsp;</td>" & vbNewLine)
					End If
					.Append("</tr>" & vbNewLine)
				Next

				.Append("</tbody>" & vbNewLine)
				.Append("</table>" & vbNewLine)
				.Append("</div>" & vbNewLine)
			End With

			GetVerifyList = strHTML.ToString
		End Function

		Private Sub doDelete(ByVal db As cFWDBConnection, ByVal attID As Integer)
			' decided to just remove the link as it is not going to be repaired
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim connectionstring As String = cAccounts.getConnectionString(curUser.AccountID)

			Try

                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Attachments, curUser.CurrentSubAccountId)

                db.FWDb("R", "Attachments", "AttachmentId", attID, "", "", "", "", "", "", "", "", "", "")
				If db.FWDbFlag = True Then
					Dim ARec As New cAuditRecord
					ARec.Action = cFWAuditLog.AUDIT_DEL
					ARec.DataElementDesc = "ATTACHMENT VERIFY"
					ARec.ElementDesc = "REMOVE BAD LINK"
					ARec.PreVal = db.FWDbFindVal("Description", 1)
					ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, attID)

                    db.FWDb("D", "attachments", "AttachmentId", attID, "", "", "", "", "", "", "", "", "", "")
				End If

			Catch ex As Exception
                'Dim cErr As New FWClasses.cErrors(User.Identity, connectionstring)
                'cErr.ReportError(CType(ex, System.Web.HttpException))
                'cErr = Nothing
			End Try
		End Sub

		Private Sub doUpload(ByVal db As cFWDBConnection, ByVal attID As Integer)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)

            db.FWDb("R", "attachments", "AttachmentId", attID, "", "", "", "", "", "", "", "", "", "")
			If db.FWDbFlag = True Then
				panelUpload.Visible = True
				cmdCancel.ImageUrl = "./buttons/cancel.gif"
				lblFilename.Text = db.FWDbFindVal("Directory", 1) & db.FWDbFindVal("Filename", 1)
				hiddenAttID.Text = attID.ToString
			End If
		End Sub

		Private Sub UploadFile()
			Dim strSplit() As String
			Dim filename, directory As String
			Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim x As Integer

			' establish whether more than one '.' exist in full path. Error. This will prevent file type ident
			Dim tmpFilename As String
			Dim extension As String = ""
			Dim preExtension As String = ""
			Dim xref, dotPos As Integer

			tmpFilename = attachment.PostedFile.FileName
			For dotPos = Len(tmpFilename) To 1 Step -1
				If Mid(tmpFilename, dotPos, 1) = "." Then
					extension = Mid(tmpFilename, dotPos + 1)
					Exit For
				End If
			Next

			db.DBOpen(fws, False)

			ReDim strSplit(50)
			strSplit = Split(attachment.PostedFile.FileName, "\")
			directory = ""

			For x = 0 To strSplit.Length - 2
				directory = directory & Trim(strSplit(x)) & "\"
			Next

			filename = strSplit(strSplit.Length - 1)

			If Trim(filename) = "" Then
				Exit Sub
			End If

			Select Case rdoAttachmentType.SelectedValue
				Case AttachmentType.Open
					' open
					directory = "/" & Trim(fws.glDocRepository)
				Case AttachmentType.Secure
					' secure
					directory = "/" & Trim(fws.glSecureDocRepository)
				Case Else
					db.DBClose()
					db = Nothing
					Exit Sub
			End Select

			If Right(directory, 1) <> "/" Then
				directory = directory & "/"
			End If

			Dim bIsUnique As Boolean
			Dim seqNum As Integer
			seqNum = 1
			bIsUnique = False

			Do While Not bIsUnique
				' ensure that file of that name doesn't already exist. if so, append seq no on the end
				If System.IO.File.Exists(Server.MapPath(Trim(directory) & Trim(filename))) = True Then
					' file already exists, try next filename sequence
					Dim newDotPos As Integer
					xref = InStr(filename, "_")
					For newDotPos = Len(filename) To 1 Step -1
						If Mid(filename, newDotPos, 1) = "." Then
							Exit For
						End If
					Next

					'dotPos = InStr(filename, ".")
					'extension = Mid(filename, newDotPos + 1)
					preExtension = Mid(filename, 1, Len(filename) - ((Len(filename) - newDotPos) + 1))

					If xref > 0 Then
						preExtension = Left(preExtension, xref - 1)
					End If

					filename = Trim(preExtension) & "_" & seqNum.ToString.Trim & "." & Trim(extension)
					seqNum = seqNum + 1
				Else
					' store the file as the filename is now unique
					bIsUnique = True
				End If
			Loop

			attachment.PostedFile.SaveAs(Server.MapPath(Trim(directory) & Trim(filename)))

			' just replace the existing file link with virtual path and filename
			db.SetFieldValue("Filename", filename, "S", True)
			db.SetFieldValue("Directory", directory, "S", False)
            db.SetFieldValue("AttachmentType", rdoAttachmentType.SelectedValue, "N", False)

            ' find the mime header for the file extension type and store it
            Dim mimeTypes As New cGlobalMimeTypes(curUser.AccountID)
            Dim mimeType = mimeTypes.getMimeTypeByExtension(extension.Trim)

            If Not mimeType Is Nothing Then
                db.SetFieldValue("MimeHeader", mimeType.MimeHeader, "S", False)
            Else
                db.SetFieldValue("MimeHeader", "text/html", "S", False)
            End If

            db.FWDb("A", "attachments", "AttachmentId", hiddenAttID.Text, "", "", "", "", "", "", "", "", "", "")

			Response.Redirect("VerifyAttachments.aspx", True)
		End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Select Case Request.QueryString("a")
                Case "u", "r"
                    Response.Redirect("VerifyAttachments.aspx", True)
                Case Else
                    Response.Redirect("MenuMain.aspx?menusection=sysoptions", True)
            End Select
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            UploadFile()
        End Sub
    End Class
End Namespace
