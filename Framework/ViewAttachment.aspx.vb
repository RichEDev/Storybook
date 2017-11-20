Imports System.IO
Imports FWBase
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class ViewAttachment
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
            Dim attachmentId As Integer
            Try
                If Request.QueryString("type") = "udh" Then
                    'OpenUserHelp()
                Else
                    attachmentId = Val(Request.QueryString("id"))
                    If attachmentId > 0 Then
						Dim db As New cFWDBConnection
						Dim fileToStream As String
                        Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)

                        db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)

                        db.FWDb("R", "attachments", "AttachmentId", attachmentId, "", "", "", "", "", "", "", "", "", "")
						If db.FWDbFlag = True Then
                            If db.FWDbFindVal("MimeHeader", 1) <> "" Then
                                fileToStream = db.FWDbFindVal("Directory", 1) & db.FWDbFindVal("Filename", 1)
                                OpenStreamFile(fileToStream, db.FWDbFindVal("MimeHeader", 1))
                            End If
						End If
					End If
				End If

			Catch ex As Exception
				'Response.ContentType = "text/html"
				'Response.Write("<div style=""width: 500px; font-size: 12pt; color: black;""><b>ERROR!&nbsp;</b>Unknown file content type. Please contact your administrator.</div>")
				'Response.Flush()
				'Response.End()
			End Try
		End Sub

        'Private Sub OpenUserHelp()
        '	Try
        '		Dim db As New cFWDBConnection
        '		Dim helpFile, helpPath As String
        '		Dim uhd As UserHelpDocs
        '              Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)

        '		uhd = Session("UserHelpDocs")

        '              db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)

        '		' get field to display help file for
        '		helpFile = Request.QueryString("udhf")
        '		helpPath = Request.QueryString("udhp")

        '		' establish the extension for the file and get MIME header
        '		Dim x As Integer
        '		Dim ext As String = ""
        '		For x = Len(helpFile) To 1 Step -1
        '			If helpFile.Substring(x - 1, 1) = "." Then
        '				ext = helpFile.Substring(x).ToUpper
        '				Exit For
        '			End If
        '		Next

        '              If ext.Length > 0 Then
        '                  Dim mimeTypes As New cGlobalMimeTypes
        '                  Dim mimeType As cGlobalMimeType = mimeTypes.getMimeTypeByExtension(ext)

        '                  If Not mimeType Is Nothing Then
        '                      Dim fileToStream As String

        '                      fileToStream = Request.QueryString("udhp") & Request.QueryString("udhf")

        '                      OpenStreamFile(fileToStream, mimeType.MimeHeader)
        '                  End If
        '              End If

        '		db.DBClose()
        '		db = Nothing

        '	Catch ex As Exception
        '		Response.ContentType = "text/html"
        '		Response.Write("<div style=""width: 500px; font-size: 12pt; color: black;""><b>ERROR!&nbsp;</b>Unknown file content type. Please contact your administrator.</div>")
        '		Response.Flush()
        '		Response.End()
        '	End Try
        'End Sub

        Private Sub OpenStreamFile(ByVal fileToStream As String, ByVal MIME_Header As String)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Dim fi As New FileInfo(Server.MapPath(fileToStream))

			If params.OpenSaveAttachments Then
				' Prompt to OPEN or SAVE - treat as a download
				Response.Clear()
				Response.AddHeader("Content-Disposition", "attachment; filename=" & fi.Name)
				Response.AddHeader("Content-Length", fi.Length.ToString)
				Response.ContentType = "application/octet-stream"
				Response.WriteFile(fi.FullName)
				Response.Flush()
				Response.End()
				Exit Sub
			End If

			' open the attachment using MIME Header specification
			Response.ContentType = MIME_Header
			Response.ContentEncoding = System.Text.Encoding.UTF8
			Response.HeaderEncoding = System.Text.Encoding.UTF8

			Dim sr As New System.IO.FileStream(Server.MapPath(fileToStream), FileMode.Open, FileAccess.Read)
			Dim br As New BinaryReader(sr)

			Dim x As Byte()
			x = br.ReadBytes(CType(fi.Length, Integer))

			br.Close()
			sr.Close()

			Response.BinaryWrite(x)
			Response.Flush()
			Response.End()

        End Sub
    End Class

End Namespace
