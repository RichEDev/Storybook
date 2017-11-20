Imports FWClasses
Imports FWReportsLibrary
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class ImportReportFields
        Inherits System.Web.UI.Page
        Const NUM_REPORT_COLS As Integer = 17
        Const NUM_LANG_COLS As Integer = 2
        Const NUM_DUNS_COLS As Integer = 8
        Const NUM_HELP_COLS As Integer = 3

        Private file1 As String
        Private file2 As String
        Private file3 As String
        Private file4 As String
        Private file5 As String
        Private file6 As String
        Private file7 As String
        Private file8 As String

        Private Enum ReportCols
            ParseCode = 0
            Table = 1
            Field = 2
            IsIDField = 3
            JoinTable = 4
            JoinField = 5
            JoinType = 6
            JoinAlias = 7
            Description = 8
            Display = 9
            FieldID = 10
            FieldType = 11
            UseForImport = 12
            UseForExport = 13
            CanTotal = 14
            FieldGroup = 15
            IsUserDefined = 16
        End Enum

#Region "File Structures"
        Private Enum enumTables
            ParseCode = 0
            TableID = 1
            TableName = 2
            JoinType = 3
            AllowedToReportOn = 4
            Description = 5
			PrimaryKeyFieldID = 6
			UniqueKeyFieldID = 7
			TableID_Old = 8
        End Enum

        Private Enum enumFields
            ParseCode = 0
            FieldID = 1
            TableID = 2
            Field = 3
            FieldType = 4
            Description = 5
            Comment = 6
            IDField = 7
            ViewGroup = 8
            GenList = 9
            Width = 10
            CanTotal = 11
            Printout = 12
            ValueList = 13
            Relabel = 14
			ParamAlias = 15
			FieldID_Old = 16
        End Enum

        Private Enum enumQueryJoins
            ParseCode = 0
            JoinTableId = 1
            TableId = 2
            BaseTableId = 3
			Description = 4
			JoinTableId_Old = 5
        End Enum

        Private Enum enumQJBreakdown
            ParseCode = 0
            JoinBreakdownId = 1
            JoinTableId = 2
            Order = 3
            TableId = 4
            TableKeyField = 5
            SrcTable = 6
			SrcTableField = 7
			JoinBreakdownId_Old = 8
        End Enum

        Private Enum enumViewGroups
            ParseCode = 0
            ViewGroupId = 1
            GroupName = 2
            ParentId = 3
            Level = 4
			ParamAlias = 5
			ViewGroupId_Old = 6
        End Enum

        Private Enum enumAllowed
            ParseCode = 0
            BaseTableId = 1
            TableId = 2
        End Enum

        Private Enum enumRepListItems
            ParseCode = 0
            FieldId = 1
            ListItem = 2
            ListValue = 3
            ValueType = 4
        End Enum

        Private Enum enumCommonFields
            ParseCode = 0
            TableId = 1
            FieldId = 2
        End Enum
#End Region

        Private Enum HelpCols
            ParseCode = 0
            HelpID = 1
            Help_Page = 2
            Help_Description = 3
            Help_Text = 4
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

        Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim action As String = ""

            action = Request.QueryString("action")

            If Me.IsPostBack = False Then
                'Session("Action") = action

                lblUploadLoc.Text = "File to Upload"

                Select Case LCase(action)
                    Case "reportfields"
                        lblTitle.Text = "Upload Report Columns"

                    Case "language"
                        lblTitle.Text = "Upload Language File"

                    Case "dunsimport"
                        lblTitle.Text = "Import Vendor DUNS data"

                    Case "emailtemplate"
                        lblTitle.Text = "Upload Email Template"
                        Dim bGetList As Boolean = True

                        Master.enablenavigation = False

                        Select Case Request.QueryString("method")
                            Case "replace"
                                curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Emails, False, True)
                                Dim tplID As Integer

                                tplID = Request.QueryString("id")
                                lblTitle.Text = "Upload template to replace '" & Request.QueryString("tname") & "'"

                                bGetList = False
                                UploadSingleFile.Visible = True

                            Case Else
                                curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Emails, False, True)
                        End Select

                        If bGetList = True Then
                            GetTemplateList()
                        End If

                    Case "reportfiles"
                        lblTitle.Text = "Upload Reporting Files"

                        UploadSingleFile.Visible = False
                        UploadGroupedFiles.Visible = True

                    Case "helptext"
                        lblTitle.Text = "Upload Help Text"

                    Case Else
                        Exit Sub
                End Select
            End If

            Title = lblTitle.Text
            Master.title = Title

            cmdCancel.AlternateText = "Cancel"
            cmdCancel.ToolTip = "Exit the screen and return to the Home screen"

            Select Case LCase(action)
                Case "language"
                    cmdOK.ToolTip = "Upload the new language elements into the database"
                Case "importfields"
                    cmdOK.ToolTip = "Upload the new report columns list into the database"
                Case "dunsexport"
                    cmdOK.ToolTip = "Export the key Vendor information from the database"
                Case "dunsimport"
                    cmdOK.ToolTip = "Upload the Vendor related DUNS information to the database"
                Case "emailtemplate"
                    cmdOK.ToolTip = "Upload the Email Template to the database"
                Case "helptext"
                    cmdOK.ToolTip = "Upload Help Text to the database"
                Case Else
                    cmdOK.ToolTip = ""
            End Select
            cmdOK.AlternateText = "OK"
        End Sub

        Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
            doUpload()
        End Sub

        Private Sub doUpload()
            If attachment.PostedFile IsNot Nothing Then
                If attachment.PostedFile.FileName <> "" Then
                    '.tpl
                    Select Case Request.QueryString("action") 'Session("Action")
                        Case "reportfields"
                            UploadReportFile()
                        Case "language"
                            UploadLanguageFile()
                        Case "dunsimport"
                            ImportDUNS()
                        Case "emailtemplate"
                            If attachment.PostedFile.FileName.EndsWith(".tpl") = True Then
                                Try
                                    UploadTemplate()
                                Catch ex As Exception
                                    lblResultMsg.Text = "The file is in an invalid format (must be .tpl)"
                                End Try
                            Else
                                lblResultMsg.Text = "The file is in an invalid format (must be .tpl)"
                            End If
                        Case "helptext"
                            UploadHelpText()
                        Case Else
                            Exit Sub
                    End Select
                Else
                    lblResultMsg.Text = "A file to upload must be specified"
                End If
            End If
        End Sub

        Private Sub UploadReportFile()
            Dim path As String = ""
            Dim sql As String = ""
            Dim fieldarray(NUM_REPORT_COLS) As String
            Dim FWDb As New cFWDBConnection
            Dim rowCount As Integer = 0
            Dim csvParser As New csvParser.cCSV
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)

            FWDb.DBOpen(fws, False)

            path = Server.MapPath("./temp/tmpFieldImport.csv")

            attachment.PostedFile.SaveAs(path)

            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim drow As DataRow

            csvParser.DelimiterChar = ","
            csvParser.hasHeaderRow = False
            csvParser.TagChar = """"

            csvDataTable = csvParser.CSVToDataset(path).Tables(0)

            Dim errstr As String = ""
            rowCount = 1

            For Each drow In csvDataTable.Rows
                errstr = AddRow(FWDb, drow, rowCount)
                If errstr <> "" Then
                    lblResultMsg.Text = "Upload of file aborted at " & errstr
                    Exit For
                End If
                rowCount += 1
            Next

            FWDb.DBClose()
            FWDb = Nothing
            If errstr = "" Then
                lblResultMsg.Text = "Report fields uploaded to database successfully"
            End If
        End Sub

        Private Sub UploadTemplate()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim path, sql As String
            Dim errstr As String = ""
            Dim FWDb As New cFWDBConnection
            Dim isErr As Boolean = False
            Dim storePath As String = "./email templates/" & curUser.Account.companyid

            Dim fileIn() As String = attachment.PostedFile.FileName.Split("\")
            Dim fileName As String = fileIn(fileIn.GetUpperBound(0))

            path = Server.MapPath(System.IO.Path.Combine(storePath, fileName))

            FWDb.DBOpen(fws, False)

            If Request.QueryString("method") <> "replace" Then
                'Check for existing template
                FWDb.AddDBParam("path", storePath, True)
                FWDb.AddDBParam("filename", fileName, False)
                sql = "SELECT * FROM [email_templates] WHERE [templatePath] = @path AND [templateFilename] = @filename"
                FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

                If FWDb.glNumRowsReturned > 0 Then
                    With FWDb.glDBWorkA.Tables(0)
                        errstr = "There is already an email template with this filename (""" & .Rows(0)("templateName") & """, """ & fileName & """).<br> You cannot upload another without first deleting the existing file."
                        isErr = True
                    End With
                End If
            End If

            'Error?
            If isErr = False Then
                If System.IO.Directory.Exists(Server.MapPath(storePath)) = False Then
                    System.IO.Directory.CreateDirectory(Server.MapPath(storePath))
                End If

                If System.IO.File.Exists(path) = True Then
                    System.IO.File.Delete(path)
                End If
                attachment.PostedFile.SaveAs(path)

                ' open file, get headers
                Dim x As New FWCommon.EmailTemplates
                Dim res As System.Collections.Specialized.NameValueCollection = x.ReadTemplate(path)

                Dim templateType As Integer = Integer.Parse(res("templateType"))
                Dim templateName As String = res("templateTitle")

                If Request.QueryString("method") <> "replace" Then
                    FWDb.AddDBParam("templatePath", storePath, True)
                    FWDb.AddDBParam("templateFilename", fileName, False)
                    FWDb.AddDBParam("templateName", templateName, False)
                    FWDb.AddDBParam("templateType", templateType, False)

                    FWDb.ExecuteSQL("INSERT INTO [email_templates] ([templateName], [templateType], [templatePath], [templateFilename]) VALUES (@templateName, @templateType, @templatePath,@templateFilename)")

                    lblResultMsg.Text = "Email template upload succeeded."
                Else
                    Dim tplId As Integer = Request.QueryString("id")
                    Dim firstpass As Boolean = True

                    FWDb.FWDb("R2", "email_templates", "templateId", tplId, "", "", "", "", "", "", "", "", "", "")
                    If FWDb.FWDb2Flag Then
                        If FWDb.FWDbFindVal("templatePath", 2) <> storePath Then
                            FWDb.SetFieldValue("templatePath", storePath, "S", firstpass)
                            firstpass = False
                        End If

                        If FWDb.FWDbFindVal("templateName", 2) <> templateName Then
                            FWDb.SetFieldValue("templateName", templateName, "S", firstpass)
                            firstpass = False
                        End If

                        If FWDb.FWDbFindVal("templateFilename", 2) <> fileName Then
                            FWDb.SetFieldValue("templateFilename", fileName, "S", firstpass)
                            firstpass = False
                        End If

                        If Integer.Parse(FWDb.FWDbFindVal("templateType", 2)) <> templateType Then
                            FWDb.SetFieldValue("templateType", templateType, "N", firstpass)
                            firstpass = False
                        End If

                        If Not firstpass Then
                            FWDb.FWDb("A", "email_templates", "templateId", tplId, "", "", "", "", "", "", "", "", "", "")
                        End If

                        lblResultMsg.Text = "Email template replaced successfully."
                    Else
                        lblResultMsg.Text = "Email template not found."
                    End If
                End If

                GetTemplateList()

            ElseIf isErr = True Then
                lblResultMsg.Text = errstr
            End If

            FWDb.DBClose()
            FWDb = Nothing
        End Sub

        Private Function AddRow(ByVal db As cFWDBConnection, ByVal drow As DataRow, ByVal rownum As Integer) As String
            Dim fldCount As Integer
            Dim isError As Boolean
            Dim retVal As String = ""

            If drow.ItemArray.Length <> NUM_REPORT_COLS Then
                isError = True
            Else
                For fldCount = 1 To NUM_REPORT_COLS
                    isError = False
                    Select Case CType(fldCount, ReportCols)
                        Case ReportCols.Table, ReportCols.Field, ReportCols.Description
                            ' check for not null fields
                            If drow(fldCount) = "" Then
                                isError = True
                            Else
                                Select Case CType(fldCount, ReportCols)
                                    Case ReportCols.Table
                                        ' table
                                        db.SetFieldValue("Table", drow(ReportCols.Table), "S", True)

                                    Case ReportCols.Field
                                        ' table
                                        db.SetFieldValue("Field", drow(ReportCols.Field), "S", False)

                                    Case ReportCols.Description
                                        ' description
                                        db.SetFieldValue("Description", drow(ReportCols.Description), "S", False)
                                End Select
                            End If

                        Case ReportCols.IsIDField, ReportCols.Display, ReportCols.FieldID, ReportCols.UseForImport, ReportCols.UseForExport, ReportCols.CanTotal, ReportCols.IsUserDefined                    ' check numeric fields
                            If IsNumeric(drow(fldCount)) = False Then
                                isError = True
                            Else
                                If drow(fldCount) <> "" Then
                                    Select Case CType(fldCount, ReportCols)
                                        Case ReportCols.IsIDField
                                            db.SetFieldValue("IsIDField", drow(ReportCols.IsIDField), "N", False)

                                        Case ReportCols.Display
                                            db.SetFieldValue("Display", drow(ReportCols.Display), "N", False)

                                        Case ReportCols.FieldID
                                            db.SetFieldValue("FieldID", drow(ReportCols.FieldID), "N", False)

                                        Case ReportCols.UseForImport
                                            db.SetFieldValue("useForImport", drow(ReportCols.UseForImport), "N", False)

                                        Case ReportCols.UseForExport
                                            db.SetFieldValue("useForExport", drow(ReportCols.UseForExport), "N", False)

                                        Case ReportCols.CanTotal
                                            db.SetFieldValue("canTotal", drow(ReportCols.CanTotal), "N", False)

                                        Case ReportCols.IsUserDefined
                                            db.SetFieldValue("IsUserDefined", drow(ReportCols.IsUserDefined), "N", False)

                                    End Select
                                Else
                                    ' check only those that don't have a default value
                                    If fldCount = ReportCols.FieldID Then ' field id
                                        isError = True
                                    End If
                                End If
                            End If

                        Case ReportCols.JoinTable, ReportCols.JoinField, ReportCols.JoinType, ReportCols.JoinAlias, ReportCols.FieldType, ReportCols.FieldGroup
                            ' fields can be null
                            If drow(fldCount) <> "" Then
                                Select Case CType(fldCount, ReportCols)
                                    Case ReportCols.JoinTable
                                        db.SetFieldValue("JoinTable", drow(ReportCols.JoinTable), "S", False)

                                    Case ReportCols.JoinField
                                        db.SetFieldValue("JoinField", drow(ReportCols.JoinField), "S", False)

                                    Case ReportCols.JoinType
                                        db.SetFieldValue("JoinType", drow(ReportCols.JoinType), "S", False)

                                    Case ReportCols.JoinAlias
                                        db.SetFieldValue("JoinAlias", drow(ReportCols.JoinAlias), "S", False)

                                    Case ReportCols.FieldType
                                        db.SetFieldValue("FieldType", drow(ReportCols.FieldType), "S", False)

                                    Case ReportCols.FieldGroup
                                        db.SetFieldValue("FieldGroup", drow(ReportCols.FieldGroup), "S", False)

                                End Select
                            End If
                    End Select

                    If isError = True Then
                        retVal = "column " & fldCount.ToString & " : row " & rownum.ToString
                        Exit For
                    End If
                Next
            End If

            If isError = False Then
                retVal = ""
            End If

            Return retVal
        End Function

        Private Sub UploadLanguageFile()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As cAccountSubAccounts = New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection(), curUser.CurrentSubAccountId)
            Dim path As String
            Dim strLangCode, strLangNum, strLangText As String
            Dim FWDb As New cFWDBConnection
            Dim rowcount As Integer

            lblResultMsg.Text = ""

            strLangCode = Mid(attachment.PostedFile.FileName, Len("lang"), 2)
            rowcount = 1

            FWDb.DBOpen(fws, False)

            path = Server.MapPath("./temp/tmpLangImport.csv")

            attachment.PostedFile.SaveAs(path)

            Dim file As New System.IO.StreamReader(path)
            Dim oneLine As String

            oneLine = file.ReadLine()

            ' first line should indicate the language being loaded
            If Left(oneLine, 1) = "#" Then
                strLangText = Mid(oneLine, 3)
                FWDb.FWDb("D", "languages", "Code", strLangCode, "", "", "", "", "", "", "", "", "", "")
                FWDb.SetFieldValue("Code", strLangCode, "S", True)
                FWDb.SetFieldValue("Description", strLangText, "S", False)
                FWDb.FWDb("W", "languages", "", "", "", "", "", "", "", "", "", "", "", "")
                FWDb.FWDb("D", "language_texts", "LCode", strLangCode, "", "", "", "", "", "", "", "", "", "")
                rowcount = rowcount + 1
                oneLine = file.ReadLine()
            End If

            Do While (oneLine <> "")
                If Left(oneLine, 1) = "%" Then
                    ' comment line so skip
                Else
                    strLangNum = Left(oneLine, 4) ' read in the four digit code

                    If IsNumeric(strLangNum) = False Then
                        lblResultMsg.Text = "Non numeric Language element encountered at line " & Str(rowcount)
                        Exit Do
                    End If

                    strLangText = Mid(oneLine, 5)

                    FWDb.SetFieldValue("LCode", strLangCode, "S", True)
                    FWDb.SetFieldValue("LNum", Val(strLangNum), "N", False)
                    FWDb.SetFieldValue("LText", Trim(strLangText), "S", False)
                    FWDb.FWDb("W", "language_texts", "", "", "", "", "", "", "", "", "", "", "", "")
                End If

                oneLine = file.ReadLine()
                rowcount = rowcount + 1
            Loop

            file.Close()
            FWDb.DBClose()
            FWDb = Nothing

            If lblResultMsg.Text = "" Then
                lblResultMsg.Text = "Report fields uploaded to database successfully"
            End If
        End Sub

        Private Sub ImportDUNS()
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim inFile, path As String
            Dim rowcount As Integer
            Dim x, matches As Integer

            lblResultMsg.Text = ""

            inFile = attachment.PostedFile.FileName

            If inFile = "" Then
                lblResultMsg.Text = "Must specify a file to upload. Import Aborted"
                Exit Sub
            End If

            path = Server.MapPath("./temp/tmpDUNSImport.csv")
            If System.IO.File.Exists(path) = True Then
                System.IO.File.Delete(path)
            End If
            attachment.PostedFile.SaveAs(path)

            rowcount = 0
            matches = 0
            db.DBOpen(fws, False)

            Dim file As New System.IO.StreamReader(path)
            Dim oneLine As String = ""
            Dim items(NUM_DUNS_COLS) As String
            Dim vendorId As String
            Dim firstentry As Boolean

            oneLine = file.ReadLine()

            Do While Trim(oneLine) <> ""
                items = Split(oneLine, ",")
                rowcount = rowcount + 1
                firstentry = True

                If Trim(items(0)) = "" Then
                    lblResultMsg.Text = "Missing Supplier Id in row " & rowcount.ToString.Trim & " - Import Aborted!"
                    Exit Do
                Else
                    vendorId = Trim(items(0))
                End If

                db.FWDb("R2", "supplier_details", "supplierId", vendorId, "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag = True Then
                    matches = matches + 1

                    For x = 0 To NUM_DUNS_COLS - 1
                        Select Case x
                            Case 0
                                ' Primary Key 1 (MUST BE PRESENT) - Vendor ID

                            Case 1
                                ' Primary Key 2 (MUST BE PRESENT) - Vendor Name
                                'If Trim(items(x)) = "" Then
                                '    lblResultMsg.Text = "Missing Vendor Name in row " & Trim(Str(rowcount)) & " - Import Aborted!"
                                '    Exit For
                                'Else
                                '    db.SetFieldValue("Vendor Name", Trim(items(x)), "S", True)
                                'End If
                            Case 2
                                ' DUNS Number
                                db.SetFieldValue("DUNS Number", Trim(items(x)), "S", firstentry)
                                firstentry = False
                            Case 3
                                ' DU-Name
                                db.SetFieldValue("DUNS-Name", Trim(items(x)), "S", firstentry)
                                firstentry = False
                            Case 4
                                ' HQ-DUNS
                                db.SetFieldValue("Parent DUNS Number", Trim(items(x)), "S", firstentry)
                                firstentry = False
                            Case 5
                                ' HQ-Name
                                db.SetFieldValue("Parent-Name", Trim(items(x)), "S", firstentry)
                                firstentry = False
                            Case 6
                                ' GU-DUNS
                                db.SetFieldValue("Global Ultimate DUNS Number", Trim(items(x)), "S", firstentry)
                                firstentry = False
                            Case 7
                                ' GU-Name
                                db.SetFieldValue("Global Ultimate-Name", Trim(items(x)), "S", firstentry)
                                firstentry = False
                            Case Else
                                ' skip because shouldn't get this far!
                        End Select
                    Next

                    db.FWDb("A", "supplier_details", "supplierId", vendorId, "", "", "", "", "", "", "", "", "", "")
                End If

                oneLine = file.ReadLine()
            Loop

            file.Close()

            db.DBClose()
            db = Nothing

            If lblResultMsg.Text = "" Then
                lblResultMsg.Text = "DUNS file imported to database successfully. " & matches.ToString & "/" & rowcount.ToString & " vendor matched."
            End If
        End Sub

        Private Sub GetTemplateList()
            Try
                Dim db As New cFWDBConnection
                Dim strHTML As New System.Text.StringBuilder
                Dim sql As String
                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)

                sql = "SELECT * FROM [email_templates] ORDER BY [templateName]"

                db.DBOpen(fws, False)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                Dim drow As DataRow

                strHTML.Append("<table class=""datatbl"">" & vbNewLine)
                strHTML.Append("<thead>" & vbNewLine)
                strHTML.Append("<tr>" & vbNewLine)
                'strHTML.Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
                'strHTML.Append("<th><img src=""./icons/16/plain/document_exchange.gif"" /></th>" & vbNewLine)
                strHTML.Append("<th>Template Name</th>" & vbNewLine)
                strHTML.Append("<th>Template Type</th>" & vbNewLine)
                strHTML.Append("<th>Template Filename</th>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
                strHTML.Append("</thead>" & vbNewLine)

                strHTML.Append("<tbody>" & vbNewLine)

                If db.glNumRowsReturned > 0 Then
                    Dim rowClass As String = "row1"
                    Dim rowalt As Boolean = False

                    For Each drow In db.glDBWorkA.Tables(0).Rows
                        rowalt = (rowalt Xor True)
                        If rowalt Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        strHTML.Append("<tr>" & vbNewLine)

                        'strHTML.Append("<td class=""" & rowClass & """>")
                        'If uinfo.permDelete Then
                        '    strHTML.Append("<a onmouseover=""window.status='Delete Template " & drow.Item("Template Name") & "';return true"" onmouseout=""window.status='Done';"" onclick=""javascript:if(confirm('Are you sure you want to delete this template?')){window.navigate('ImportReportFields.aspx?action=emailtemplate&method=delete&id=" & drow.Item("TemplateId") & "')};""><img src=""./icons/delete2.gif"" /></a>")
                        'End If
                        'strHTML.Append("</td>" & vbNewLine)
                        'strHTML.Append("<td class=""" & rowClass & """>")
                        'If uinfo.permAmend Then
                        '    strHTML.Append("<a onmouseover=""window.status='Replace Template " & drow.Item("Template Name") & "';return true"" onmouseout=""window.status='Done';"" onclick=""javascript:document.location('ImportReportFields.aspx?action=emailtemplate&method=replace&id=" & drow.Item("templateId") & "')};""><img src=""./icons/16/plain/document_exchange.gif"" /></a>")
                        'End If
                        'strHTML.Append("</td>" & vbNewLine)
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
                                strHTML.Append("Unknown Template Type")
                        End Select
                        strHTML.Append("</td>" & vbNewLine)
                        strHTML.Append("<td class=""" & rowClass & """>" & drow.Item("templateFilename") & "</td>" & vbNewLine)
                        strHTML.Append("</tr>" & vbNewLine)
                    Next
                Else
                    strHTML.Append("<tr>" & vbNewLine)
                    strHTML.Append("<td class=""row1"" align=""center"" colspan=""3""><i>No email templates currently uploaded</i></td>" & vbNewLine)
                    strHTML.Append("</tr>" & vbNewLine)
                End If
                strHTML.Append("</tbody>" & vbNewLine)
                strHTML.Append("<tfoot>" & vbNewLine)
                strHTML.Append("</tfoot>" & vbNewLine)
                strHTML.Append("</table>" & vbNewLine)

                db.DBClose()
                db = Nothing

                litUploadData.Text = strHTML.ToString

                strHTML = Nothing

            Catch ex As Exception
                Dim errHTML As New System.Text.StringBuilder

                errHTML.Append("<table class=""datatbl"">" & vbNewLine)
                errHTML.Append("<thead>" & vbNewLine)
                errHTML.Append("<tr>" & vbNewLine)
                errHTML.Append("<th>An error has occurred retrieving templates!!</th>" & vbNewLine)
                errHTML.Append("</tr>" & vbNewLine)
                errHTML.Append("</thead>" & vbNewLine)
                errHTML.Append("<tbody>" & vbNewLine)
                errHTML.Append("<tr>" & vbNewLine)
                errHTML.Append("<td class=""row1"">" & ex.Message & "</td>" & vbNewLine)
                errHTML.Append("</tr>" & vbNewLine)
                errHTML.Append("</tbody>" & vbNewLine)
                errHTML.Append("<tfoot>" & vbNewLine)
                errHTML.Append("</tfoot>" & vbNewLine)
                errHTML.Append("</table>" & vbNewLine)

                litUploadData.Text = errHTML.ToString

                errHTML = Nothing
            End Try
        End Sub

        Private Function DeleteTemplate(ByVal tplID As Integer) As Boolean
            Try
                Dim db As New cFWDBConnection
                Dim retVal As Boolean
                Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)

                db.DBOpen(fws, False)

                ' check that the template is not in use by an email schedule
                db.FWDb("R", "email_schedule", "templateId", tplID, "", "", "", "", "", "", "", "", "", "")
                If db.FWDbFlag = False Then
                    db.FWDb("R2", "email_templates", "templateId", tplID, "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb2Flag = True Then
                        ' delete the physical template file
                        Dim path As String = Server.MapPath(System.IO.Path.Combine(db.FWDbFindVal("templatePath", 2), db.FWDbFindVal("templateFilename", 2)))
                        If System.IO.File.Exists(path) Then
                            System.IO.File.Delete(path)
                        End If

                        ' delete the template entry
                        db.FWDb("D", "email_templates", "templateId", tplID, "", "", "", "", "", "", "", "", "", "")
                    End If
                    retVal = True
                Else
                    Dim errHTML As New System.Text.StringBuilder

                    errHTML.Append("<table class=""datatbl"" width=""500"">" & vbNewLine)
                    errHTML.Append("<thead>" & vbNewLine)
                    errHTML.Append("<tr>" & vbNewLine)
                    errHTML.Append("<th>Deletion of template denied!!</th>" & vbNewLine)
                    errHTML.Append("</tr>" & vbNewLine)
                    errHTML.Append("</thead>" & vbNewLine)
                    errHTML.Append("<tbody>" & vbNewLine)
                    errHTML.Append("<tr>" & vbNewLine)
                    errHTML.Append("<td class=""row1"" align=""center""><b>Reason: </b>The template is currently assigned to an active email schedule.</td>" & vbNewLine)
                    errHTML.Append("</tr>" & vbNewLine)
                    errHTML.Append("</tbody>" & vbNewLine)
                    errHTML.Append("<tfoot>" & vbNewLine)
                    errHTML.Append("</tfoot>" & vbNewLine)
                    errHTML.Append("</table>" & vbNewLine)

                    litUploadData.Text = errHTML.ToString

                    errHTML = Nothing
                    retVal = False
                End If

                db.DBClose()
                db = Nothing

                Return retVal

            Catch ex As Exception
                Dim errHTML As New System.Text.StringBuilder

                errHTML.Append("<table class=""datatbl"">" & vbNewLine)
                errHTML.Append("<thead>" & vbNewLine)
                errHTML.Append("<tr>" & vbNewLine)
                errHTML.Append("<th>An error has occurred trying to delete a template!!</th>" & vbNewLine)
                errHTML.Append("</tr>" & vbNewLine)
                errHTML.Append("</thead>" & vbNewLine)
                errHTML.Append("<tbody>" & vbNewLine)
                errHTML.Append("<tr>" & vbNewLine)
                errHTML.Append("<td class=""row1"">" & ex.Message & "</td>" & vbNewLine)
                errHTML.Append("</tr>" & vbNewLine)
                errHTML.Append("</tbody>" & vbNewLine)
                errHTML.Append("<tfoot>" & vbNewLine)
                errHTML.Append("</tfoot>" & vbNewLine)
                errHTML.Append("</table>" & vbNewLine)

                litUploadData.Text = errHTML.ToString

                errHTML = Nothing
                Return False
            End Try
        End Function

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            Select Case Request.QueryString("action")
                Case "emailtemplate"
                    Response.Redirect("VerifyTemplates.aspx", True)

                Case Else
                    Response.Redirect("Home.aspx", True)
            End Select

        End Sub

        Protected Sub cmdGUCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdGUCancel.Click
            Response.Redirect("Home.aspx", True)
        End Sub

        Protected Sub cmdGroupUpload_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdGroupUpload.Click
            'ImportFileGroup(treeDirectories.SelectedNode.Value)
            'If ValidateFiles() = True Then
            ImportFileGroup()
            'Else
            'lblError.Text = "One or more required file locations have not been specified"
            'End If
        End Sub

        Private Function ValidateFiles() As Boolean
            Dim QueueURL As String = "icons/24/plain/document_check.gif"
            Dim MissingURL As String = "icons/24/plain/document_warning.gif"

            Dim filesPresent As Boolean = True

            If fuTableDef.HasFile = False Then
                filesPresent = False
                imgTableDef.ImageUrl = MissingURL
            Else
                imgTableDef.ImageUrl = QueueURL
                file1 = fuTableDef.FileName
            End If

            If fuFieldDef.HasFile = False Then
                filesPresent = False
                imgFieldDef.ImageUrl = MissingURL
            Else
                imgFieldDef.ImageUrl = QueueURL
                file2 = fuFieldDef.FileName
            End If

            If fuQueryJDef.HasFile = False Then
                filesPresent = False
                imgQueryJDef.ImageUrl = MissingURL
            Else
                file3 = fuQueryJDef.FileName
                imgQueryJDef.ImageUrl = QueueURL
            End If

            If fuQueryJBDef.HasFile = False Then
                filesPresent = False
                imgQueryJBDef.ImageUrl = MissingURL
            Else
                file4 = fuQueryJBDef.FileName
                imgQueryJBDef.ImageUrl = QueueURL
            End If

            If fuViewGroupDef.HasFile = False Then
                filesPresent = False
                imgViewGroupDef.ImageUrl = MissingURL
            Else
                file5 = fuViewGroupDef.FileName
                imgViewGroupDef.ImageUrl = QueueURL
            End If

            If fuAllowedDef.HasFile = False Then
                filesPresent = False
                imgAllowedDef.ImageUrl = MissingURL
            Else
                file6 = fuAllowedDef.FileName
                imgAllowedDef.ImageUrl = QueueURL
            End If

            If fuRptListItemsDef.HasFile = False Then
                filesPresent = False
                imgRptListItemsDef.ImageUrl = MissingURL
            Else
                file7 = fuRptListItemsDef.FileName
                imgRptListItemsDef.ImageUrl = QueueURL
            End If

            If fuCommonFieldsDef.HasFile = False Then
                filesPresent = False
                imgCommonFieldsDef.ImageUrl = MissingURL
            Else
                file8 = fuCommonFieldsDef.FileName
                imgCommonFieldsDef.ImageUrl = QueueURL
            End If

            Return filesPresent
        End Function

        Private Enum uploadStatus
            Failed = 0
            Success = 1
            Bypassed = 2
        End Enum

        Private Sub ImportFileGroup()
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim tickURL As String = "~/buttons/tick.gif"
            Dim bypassURL As String = "~/buttons/redo.png"

            db.DBOpen(fws, False)

            Select Case ProcessTables(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Tables' file"
                    imgTableDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub

                Case uploadStatus.Success
                    imgTableDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgTableDef.ImageUrl = bypassURL

            End Select

            Select Case ProcessFields(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Fields' file"
                    imgFieldDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgFieldDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgFieldDef.ImageUrl = bypassURL
            End Select

            Select Case ProcessQJ(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Joins' file"
                    imgQueryJDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgQueryJDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgQueryJDef.ImageUrl = bypassURL
            End Select

            Select Case ProcessQJB(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Join Breakdown' file"
                    imgQueryJBDef.ImageUrl = "./buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgQueryJBDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgQueryJBDef.ImageUrl = bypassURL
            End Select

            Select Case ProcessAllowed(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Allowed Tables' file"
                    imgAllowedDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgAllowedDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgAllowedDef.ImageUrl = bypassURL
            End Select

            Select Case ProcessViewGroups(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'View Groups' file"
                    imgViewGroupDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgViewGroupDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgViewGroupDef.ImageUrl = bypassURL
            End Select

            Select Case ProcessRLI(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Report List Items' file"
                    imgRptListItemsDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgRptListItemsDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgRptListItemsDef.ImageUrl = bypassURL
            End Select

            Select Case ProcessCF(db)
                Case uploadStatus.Failed
                    ' error occurred parsing table
                    lblError.Text = "An error occurred parsing the 'Report List Items' file"
                    imgCommonFieldsDef.ImageUrl = "~/buttons/information.gif"
                    db.DBClose()
                    Exit Sub
                Case uploadStatus.Success
                    imgCommonFieldsDef.ImageUrl = tickURL
                Case uploadStatus.Bypassed
                    imgCommonFieldsDef.ImageUrl = bypassURL
            End Select

            ' if first time upload, then run migration routines
            Dim doRollBack As Boolean = False

            db.FWDb("R", "fwparams", "Param", "MIGRATE_UF", "Location Id", 0, "", "", "", "", "", "", "", "")
            If db.FWDbFlag Then
                If db.FWDbFindVal("Value", 1) = "0" Then
                    db.ExecuteSQL("BEGIN TRANSACTION migration_routines")
                    If Migrate_UserFields(db) Then
                        If Not Migrate_Reports(db) Then
                            doRollBack = True
                        End If
                    Else
                        doRollBack = True
                    End If

                    If doRollBack Then
                        db.ExecuteSQL("ROLLBACK TRANSACTION migration_routines")
                    Else
                        db.ExecuteSQL("COMMIT TRANSACTION migration_routines")

                        db.SetFieldValue("Value", 1, "N", True)
                        db.FWDb("A", "fwparams", "Param", "MIGRATE_UF", "Location Id", 0, "", "", "", "", "", "", "", "")
                    End If
                End If
            End If

            db.DBClose()
            db = Nothing

            ' invalidate any cached data as reporting configuration has changed
            Try
                Dim reports As IReports = CType(Activator.GetObject(GetType(IReports), ConfigurationManager.AppSettings("ReportsServicePath") & "/reports.rem"), IReports)

                'reports.InvalidateReportCache(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, curUser.CurrentSubAccountId.Value)
                'reports.InvalidateReportDefinitions(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid)
                'Dim f As New cFields(uinfo, fws)
                'f.InvalidateCache()
                'Dim t As New cTables(uinfo, fws)
                't.InvalidateCache()
                'Dim j As New cFWJoins(uinfo, fws)
                'j.InvalidateCache()
            Catch ex As Exception

            End Try

            If doRollBack Then
                lblError.Text = "Import Completed Successfully but *** Migration routines failed ***"
            Else
                lblError.Text = "Import Completed Successfully"
            End If
        End Sub

        Private Function ProcessTables(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuTableDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/tables.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuTableDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "tables.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = headerRow
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                'db.FWDb("D", "tables")
                db.ExecuteSQL("DELETE FROM tables_base WHERE tableid_old < 10000")

                For Each drow In csvDataTable.Rows
                    ' need to skip the first row
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumTables.TableID_Old + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("tableid", drow(enumTables.TableID), "G", True)
                            db.SetFieldValue("tablename", drow(enumTables.TableName), "S", False)
                            db.SetFieldValue("jointype", drow(enumTables.JoinType), "N", False)
                            db.SetFieldValue("allowreporton", drow(enumTables.AllowedToReportOn), "N", False)
                            db.SetFieldValue("description", drow(enumTables.Description), "S", False)
                            db.SetFieldValue("primarykey", drow(enumTables.PrimaryKeyFieldID), "G", False)
                            db.SetFieldValue("keyfield", drow(enumTables.UniqueKeyFieldID), "G", False)
                            db.SetFieldValue("tableid_old", drow(enumTables.TableID_Old), "N", False)
                            db.FWDb("WX", "tables_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Function ProcessFields(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuFieldDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/fields.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuFieldDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "fields.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                db.FWDb("D", "fields_base", "isuserdefined", 0, "", "", "", "", "", "", "", "", "", "")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumFields.FieldID_Old + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("fieldid", drow(enumFields.FieldID), "G", True)
                            db.SetFieldValue("tableid", drow(enumFields.TableID), "G", False)
                            db.SetFieldValue("field", drow(enumFields.Field), "S", False)
                            db.SetFieldValue("fieldtype", drow(enumFields.FieldType), "S", False)
                            db.SetFieldValue("description", drow(enumFields.Description), "S", False)
                            db.SetFieldValue("comment", drow(enumFields.Comment), "S", False)
                            db.SetFieldValue("idfield", drow(enumFields.IDField), "N", False)
                            db.SetFieldValue("viewgroupid", drow(enumFields.ViewGroup), "G", False)
                            db.SetFieldValue("genlist", drow(enumFields.GenList), "N", False)
                            db.SetFieldValue("width", drow(enumFields.Width), "N", False)
                            db.SetFieldValue("cantotal", drow(enumFields.CanTotal), "N", False)
                            db.SetFieldValue("printout", drow(enumFields.Printout), "N", False)
                            db.SetFieldValue("valuelist", drow(enumFields.ValueList), "N", False)
                            db.SetFieldValue("relabel", drow(enumFields.Relabel), "N", False)
                            db.SetFieldValue("alias", drow(enumFields.ParamAlias), "S", False)
                            db.SetFieldValue("fieldid_old", drow(enumFields.FieldID_Old), "S", False)

                            db.FWDb("WX", "fields_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If
            Return parseRetVal
        End Function

        Private Function ProcessQJ(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuQueryJDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/jointables.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuQueryJDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "jointables.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                'db.FWDb("D", "jointables_base")
                db.ExecuteSQL("DELETE FROM jointables_base WHERE jointableid_old < 10000")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumQueryJoins.JoinTableId_Old + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("jointableid", drow(enumQueryJoins.JoinTableId), "S", True)
                            db.SetFieldValue("tableid", drow(enumQueryJoins.TableId), "G", False)
                            db.SetFieldValue("basetableid", drow(enumQueryJoins.BaseTableId), "G", False)
                            db.SetFieldValue("description", drow(enumQueryJoins.Description), "S", False)
                            db.SetFieldValue("jointableid_old", drow(enumQueryJoins.JoinTableId_Old), "S", False)
                            db.FWDb("WX", "jointables_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Function ProcessQJB(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuQueryJBDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/joinbreakdown.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuQueryJBDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "joinbreakdown.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                'db.FWDb("D", "joinbreakdown_base")
                db.ExecuteSQL("DELETE FROM joinbreakdown_base") ' WHERE joinbreakdownid_old < 10000")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumQJBreakdown.JoinBreakdownId_Old + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("joinbreakdownid", drow(enumQJBreakdown.JoinBreakdownId), "N", True)
                            db.SetFieldValue("jointableid", drow(enumQJBreakdown.JoinTableId), "N", False)
                            db.SetFieldValue("order", drow(enumQJBreakdown.Order), "N", False)
                            db.SetFieldValue("tableid", drow(enumQJBreakdown.TableId), "G", False)
                            db.SetFieldValue("table_fieldkey", drow(enumQJBreakdown.TableKeyField), "G", False)
                            db.SetFieldValue("sourcetable", drow(enumQJBreakdown.SrcTable), "G", False)
                            db.SetFieldValue("joinkey", drow(enumQJBreakdown.SrcTableField), "G", False)
                            'db.SetFieldValue("joinbreakdownid_old", drow(enumQJBreakdown.JoinBreakdownId_Old), "S", False)

                            db.FWDb("WX", "joinbreakdown_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Function ProcessViewGroups(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuViewGroupDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/viewgroups_base.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuViewGroupDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "viewgroups_base.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                db.FWDb("D", "viewgroups_base", "", "", "", "", "", "", "", "", "", "", "", "")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumViewGroups.ViewGroupId_Old + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("viewgroupid", drow(enumViewGroups.ViewGroupId), "G", True)
                            db.SetFieldValue("groupname", drow(enumViewGroups.GroupName), "S", False)
                            db.SetFieldValue("parentid", drow(enumViewGroups.ParentId), "G", False)
                            db.SetFieldValue("level", drow(enumViewGroups.Level), "N", False)
                            db.SetFieldValue("alias", drow(enumViewGroups.ParamAlias), "S", False)
                            db.SetFieldValue("viewgroupid_old", drow(enumViewGroups.ViewGroupId_Old), "S", False)
                            db.FWDb("WX", "viewgroups_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Function ProcessAllowed(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuAllowedDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/allowedtables.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuAllowedDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "allowedtables.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                'db.FWDb("D", "reports_allowedtables_base")
                db.ExecuteSQL("DELETE FROM reports_allowedtables_base WHERE tableid not in (select tableid from tables_base where tableid_old > 9999)")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumAllowed.TableId + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("basetableid", drow(enumAllowed.BaseTableId), "G", True)
                            db.SetFieldValue("tableid", drow(enumAllowed.TableId), "G", False)
                            db.FWDb("WX", "reports_allowedtables_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Function ProcessRLI(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuRptListItemsDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/rptlistitems.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuRptListItemsDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "rptlistitems.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                db.FWDb("D", "rptlistitems_base", "", "", "", "", "", "", "", "", "", "", "", "")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumRepListItems.ValueType + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("fieldid", drow(enumRepListItems.FieldId), "G", True)
                            db.SetFieldValue("listitem", drow(enumRepListItems.ListItem), "N", False)
                            db.SetFieldValue("listvalue", drow(enumRepListItems.ListValue), "N", False)
                            db.SetFieldValue("valuetype", drow(enumRepListItems.ValueType), "S", False)
                            db.FWDb("WX", "rptlistitems_base", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Function ProcessCF(ByVal db As cFWDBConnection) As uploadStatus
            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim csvParser As New csvParser.cCSV
            Dim drow As DataRow
            Dim parseRetVal As uploadStatus = uploadStatus.Success
            Dim headerRow As Boolean = True

            If fuCommonFieldsDef.PostedFile.FileName <> "" Then
                Dim path As String = Server.MapPath("./temp/common_fields.csv")
                If System.IO.File.Exists(path) Then
                    System.IO.File.Delete(path)
                End If

                fuCommonFieldsDef.PostedFile.SaveAs(path)
                'My.Computer.FileSystem.CopyFile(System.IO.Path.Combine(srcDir, "common_fields.csv"), path)

                csvParser.DelimiterChar = ","
                csvParser.hasHeaderRow = True
                csvParser.TagChar = """"

                csvDataTable = csvParser.CSVToDataset(path).Tables(0)

                ' clear current values out of the table
                db.FWDb("D", "reports_common_columns", "", "", "", "", "", "", "", "", "", "", "", "")

                For Each drow In csvDataTable.Rows
                    If headerRow Then
                        headerRow = False
                    Else
                        If drow.ItemArray.Length <> enumCommonFields.FieldId + 1 Then
                            parseRetVal = uploadStatus.Failed
                            Exit For
                        Else
                            ' import the row
                            db.SetFieldValue("tableid", drow(enumCommonFields.TableId), "G", True)
                            db.SetFieldValue("fieldid", drow(enumCommonFields.FieldId), "G", False)
                            db.FWDb("WX", "reports_common_columns", "", "", "", "", "", "", "", "", "", "", "", "")
                        End If
                    End If
                Next
            Else
                parseRetVal = uploadStatus.Bypassed
            End If

            Return parseRetVal
        End Function

        Private Sub UploadHelpText()
            Dim db As New cFWDBConnection
            Dim path As String
            Dim csvParser As New csvParser.cCSV
            'Dim metaDB As New cMetabase(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)

            db.DBOpenMetabase(ConfigurationManager.ConnectionStrings("metabase").ConnectionString, False)

            path = Server.MapPath("./temp/tmpHelpText.csv")

            attachment.PostedFile.SaveAs(path)

            ' clear out existing [help_text] fields
            db.FWDb("D", "help_text", "", "", "", "", "", "", "", "", "", "", "", "")

            ' open file, validate line by line and import to the database
            Dim csvDataTable As DataTable
            Dim drow As DataRow

            csvParser.DelimiterChar = ","
            csvParser.hasHeaderRow = True
            csvParser.TagChar = """"

            csvDataTable = csvParser.CSVToDataset(path).Tables(0)

            Dim skiprow As Boolean = csvParser.hasHeaderRow
            Dim rowCount As Integer = 0
            Dim success As Boolean = True

            For Each drow In csvDataTable.Rows
                If Not skiprow Then
                    If drow.ItemArray.Length <> (HelpCols.Help_Text + 1) Then
                        lblResultMsg.Text = "Insufficient required entries in file at row " & (rowCount + 1).ToString
                        success = False
                        Exit For
                    End If

                    If IsNumeric(drow.Item(HelpCols.HelpID)) = False Then
                        lblResultMsg.Text = "Help ID (column 1) contains non-numeric value at row " & (rowCount + 1).ToString
                        success = False
                        Exit For
                    End If

                    db.SetFieldValue("helpid", drow.Item(HelpCols.HelpID), "N", True)
                    db.SetFieldValue("page", drow.Item(HelpCols.Help_Page), "S", False)
                    db.SetFieldValue("description", drow.Item(HelpCols.Help_Description), "S", False)
                    db.SetFieldValue("helptext", drow.Item(HelpCols.Help_Text), "S", False)
                    db.FWDb("W", "help_text", "", "", "", "", "", "", "", "", "", "", "", "")

                    rowCount += 1
                End If

                skiprow = False
            Next

            If success Then
                lblResultMsg.Text = rowCount.ToString & " help items imported successfully"
            End If

            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim helptext As New cHelp(curUser)
            helptext.InvalidateCache()

            db.DBClose()
            db = Nothing
        End Sub
    End Class

End Namespace
