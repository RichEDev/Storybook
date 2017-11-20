Imports System.Web.Configuration
Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class Attachments
        Inherits System.Web.UI.Page
        Protected WithEvents attachement As System.Web.UI.HtmlControls.HtmlInputFile

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
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim db As New cFWDBConnection
            Dim sql As String
            Dim action As String
            Dim attId, refNum As Integer
            Dim ARec As New cAuditRecord
            Dim hasdata As Boolean
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim suppStr As String = params.SupplierPrimaryTitle

            Dim clsModules As New cModules
            Dim clsModule As cModule = clsModules.GetModuleByID(CInt(curUser.CurrentActiveModule))
            Dim brandName As String = clsModule.BrandNameHTML

            db.DBOpen(fws, False)

            hasdata = False

            If params.EnableAttachmentUpload Then
                lnkAdd.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Attachments, False)
            End If

            If params.EnableAttachmentHyperlink Then
                lnkLink.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Attachments, False)
            End If

            If Me.IsPostBack = False Then
                Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Attachments, curUser.CurrentSubAccountId)

                action = Request.QueryString("action")
                att_area.Text = Request.QueryString("attarea")
                refNum = Request.QueryString("ref")
                Session("RefNum") = refNum

                PopulateAttachmentTypes(CType(att_area.Text, AttachmentArea))

                Select Case CType(att_area.Text, AttachmentArea)
                    Case AttachmentArea.CONTRACT
                        db.FWDb("R", "contract_details", "ContractId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDbFlag = True Then
                            lblTitle.Text = "Contract: - " & db.FWDbFindVal("ContractDescription", 1)
                        End If

                    Case AttachmentArea.CONTRACT_NOTES
                        db.FWDb("R2", "contractNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            db.FWDb("R", "contract_details", "ContractId", db.FWDbFindVal("ContractId", 2), "", "", "", "", "", "", "", "", "", "")
                            If db.FWDbFlag = True Then
                                lblTitle.Text = "Contract Note: - " & db.FWDbFindVal("ContractDescription", 1)
                            End If
                        End If

                    Case AttachmentArea.INVOICE_NOTES
                        db.FWDb("R2", "invoiceNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            db.FWDb("R", "invoices", "InvoiceId", db.FWDbFindVal("InvoiceId", 2), "", "", "", "", "", "", "", "", "", "")
                            If db.FWDbFlag = True Then
                                lblTitle.Text = "Invoice Note: - Inv.No. " & db.FWDbFindVal("InvoiceNumber", 1)
                            Else
                                lblTitle.Text = "Invoice Note - Unknown"
                            End If
                        End If

                    Case AttachmentArea.PRODUCT_NOTES
                        db.FWDb("R2", "productNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            db.FWDb("R", "productDetails", "ProductId", db.FWDbFindVal("ProductId", 2), "", "", "", "", "", "", "", "", "", "")
                            If db.FWDbFlag = True Then
                                lblTitle.Text = "Product Note: - " & db.FWDbFindVal("ProductName", 1)
                            End If
                        End If

                    Case AttachmentArea.VENDOR_NOTES
                        Dim suppliers As New cSuppliers(curUser.Account.accountid, curUser.CurrentSubAccountId)

                        db.FWDb("R2", "supplierNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            Dim supplier As cSupplier = suppliers.getSupplierById(Integer.Parse(db.FWDbFindVal("supplierId", 2)))

                            lblTitle.Text = suppStr & " Note: - " & supplier.SupplierName
                        End If

                    Case AttachmentArea.VENDOR
                        Dim suppliers As New cSuppliers(curUser.Account.accountid, curUser.CurrentSubAccountId)
                        Dim supplier As cSupplier = suppliers.getSupplierById(CInt(Session("RefNum")))

                        lblTitle.Text = suppStr & ": - " & supplier.SupplierName

                    Case AttachmentArea.CONTACT_NOTES
                        db.FWDb("R2", "supplierContactNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            db.FWDb("R", "supplier_contacts", "contactId", db.FWDbFindVal("contactId", 2), "", "", "", "", "", "", "", "", "", "")
                            If db.FWDbFlag = True Then
                                lblTitle.Text = suppStr & " Contact: - " & db.FWDbFindVal("contactName", 1)
                            End If
                        End If

                    Case AttachmentArea.TASKS
                        Dim tasks As New cTasks(curUser.Account.accountid, curUser.CurrentSubAccountId)
                        Dim task As cTask = tasks.GetTaskById(Session("RefNum"))
                        If Not task Is Nothing Then
                            lblTitle.Text = "Task: - " & task.Subject
                        End If

                    Case Else

                End Select

                ' set headings
                lblDescription.Text = "Description"
                lblStorageDir.Text = "Storage Directory"
                lblFileToAttach.Text = "File to Attach"
                txtNewSubDir.ToolTip = "Insert new subdirectory to the currently selected destination."
                txtNewSubDir.Text = ""
                lblNewSubDir.Text = "New Subdirectory"
                txtNewSubDir.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Attachments, False)
                lblNewSubDir.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Attachments, False)

                Title = "File Attachments"
                Master.title = Title
                Master.enablenavigation = False

                Select Case action
                    Case "open"

                    Case "delete"
                        attId = Request.QueryString("id")
                        If attId <> 0 Then
                            db.FWDb("R2", "attachments", "AttachmentId", attId, "AttachmentArea", att_area.Text, "", "", "", "", "", "", "", "")
                            If db.FWDb2Flag = True Then
                                db.FWDb("D", "attachment_audience", "AttachmentId", attId, "", "", "", "", "", "", "", "", "", "")
                                db.FWDb("D", "attachments", "AttachmentId", attId, "AttachmentArea", att_area.Text, "", "", "", "", "", "", "", "")
                                ARec.Action = cFWAuditLog.AUDIT_DEL
                                Select Case CType(att_area.Text, AttachmentArea)
                                    Case AttachmentArea.CONTRACT, AttachmentArea.CONTRACT_NOTES
                                        db.FWDb("R3", "contract_details", "ContractId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                                        If db.FWDb3Flag Then
                                            ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                                        End If
                                    Case AttachmentArea.VENDOR, AttachmentArea.VENDOR_NOTES
                                        db.FWDb("R3", "supplier_details", "supplierId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                                        If db.FWDb3Flag Then
                                            ARec.ContractNumber = db.FWDbFindVal("supplierName", 3)
                                        End If
                                    Case AttachmentArea.INVOICE_NOTES
                                        db.FWDb("R3", "invoices", "InvoiceId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                                        If db.FWDb3Flag Then
                                            ARec.ContractNumber = "INV:" & db.FWDbFindVal("InvoiceNumber", 3)
                                        End If

                                    Case AttachmentArea.PRODUCT_NOTES
                                        db.FWDb("R3", "productDetails", "ProductId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                                        If db.FWDb3Flag Then
                                            ARec.ContractNumber = db.FWDbFindVal("ProductName", 3)
                                        End If

                                    Case AttachmentArea.CONTACT_NOTES
                                        db.FWDb("R3", "supplier_contacts", "contactId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                                        If db.FWDb3Flag Then
                                            ARec.ContractNumber = db.FWDbFindVal("contactName", 3)
                                        End If
                                    Case AttachmentArea.TASKS
                                        Dim tasks As New cTasks(curUser.Account.accountid, curUser.CurrentSubAccountId)
                                        Dim task As cTask = tasks.GetTaskById(Session("RefNum"))
                                        If Not task Is Nothing Then
                                            If task.Subject.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                                                ARec.ContractNumber = task.Subject.Substring(0, 50)
                                            Else
                                                ARec.ContractNumber = task.Subject
                                            End If
                                        End If
                                    Case Else
                                        ARec.ContractNumber = ""
                                End Select

                                ARec.DataElementDesc = "FILE ATTACHMENT"
                                ARec.ElementDesc = db.FWDbFindVal("Description", 2)
                                If CType(db.FWDbFindVal("AttachmentType", 2), AttachmentType) <> AttachmentType.Hyperlink Then
                                    ARec.PreVal = db.FWDbFindVal("Filename", 2)
                                Else
                                    If db.FWDbFindVal("Directory", 2).Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                                        ARec.PreVal = db.FWDbFindVal("Directory", 2).Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                                    Else
                                        ARec.PreVal = db.FWDbFindVal("Directory", 2)
                                    End If

                                End If
                                ALog.AddAuditRec(ARec, True)
                                ALog.CommitAuditLog(curUser.Employee, attId)

                                If CType(db.FWDbFindVal("AttachmentType", 2), AttachmentType) <> AttachmentType.Hyperlink Then
                                    ' delete the actual attachment file from the repository
                                    Dim sysPath As String
                                    sysPath = Server.MapPath(db.FWDbFindVal("Directory", 2))
                                    If System.IO.File.Exists(sysPath & Trim(db.FWDbFindVal("Filename", 2))) = True Then
                                        System.IO.File.Delete(sysPath & Trim(db.FWDbFindVal("Filename", 2)))
                                    End If
                                End If
                            End If
                        End If

                    Case "email"
                        If fws.glMailServer <> "" Then
                            ' only proceed if a mailserver has been specified
                            Dim att_sent As Boolean
                            attId = Request.QueryString("id")

                            If attId <> 0 Then
                                'att_sent = SMRoutines.SendAttachmentRequest(db, fws, curUser.Employee, attId, att_area.Text, Session("ActiveContract"), Server)
                                'If att_sent = False Then
                                lblStatusMsg.Text = "Email Attachment request failed. Contact your " & brandName & " Administrator."
                                'End If
                            End If
                        Else
                            lblStatusMsg.Text = "ERROR! Cannot request attachment - Mail Server not configured. Contact " & brandName & " Administrator."
                        End If

                    Case "edit"
                        EditAttachment(db, att_area.Text, Request.QueryString("id"))
                        Master.useCloseNavigationMsg = False
                    Case Else
                        Master.useCloseNavigationMsg = True
                End Select

                ' populate (recursively) the directory list
                GetFolderList(fws, fws.glDocRepository, Server.MapPath(fws.glDocRepository), fws.glDocRepository)
                lstStorageDir.Items.Insert(0, "[Root]")

                sql = "SELECT [AttachmentId],[ReferenceNumber],[Description],[Directory],[Filename],[DateAttached],[AttachmentType]"
                If cDef.DBVERSION >= 18 Then
                    sql += ",[AttachedBy],dbo.CheckAttachmentAccess([AttachmentId],@userId) AS [PermitVal]"
                End If
                sql += " FROM [attachments] WHERE [ReferenceNumber] = @refNum"
                sql += " AND [AttachmentArea] = @attArea"
                sql += " ORDER BY [Description]"
                db.AddDBParam("userId", curUser.Employee.employeeid, True)
                db.AddDBParam("refNum", Session("RefNum"), False)
                db.AddDBParam("attArea", att_area.Text, False)
                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                    hasdata = True
                End If

                litAttachments.Text = GetSimpleAttachmentData(db)

                If Request.QueryString("ret") <> "" Then
                    returnURL.Value = Request.QueryString("ret")
                End If

                Dim config As System.Configuration.Configuration = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath)
                Dim configSection As HttpRuntimeSection = config.GetSection("system.web/httpRuntime")

                Dim maxVal As Integer = params.MaxUploadSize

                configSection.MaxRequestLength = maxVal
                lblMaxUploadValue.Text = (maxVal \ 1024).ToString
                
            End If

            cmdUpdate.ToolTip = "Add attachment association to the contract"
            cmdUpdate.Attributes.Add("onmouseover", "window.status='Add attachment association to the contract';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")

            'litClose.Text = "<a class=""linkbutton"" onmouseover=""window.status='Close the Attachments screen';return true;"" onmouseout=""window.status='Done';"" href=""javascript:this.opener.navigate(this.opener.location); window.close();"">Close</a>"

            If litAttachments.Text = "" Then
                Dim tmpstr As String
                tmpstr = Session("AttachmentHTML")
                litAttachments.Text = tmpstr
            End If

            db.DBClose()
            db = Nothing

            Master.RefreshBreadcrumbInfo()
        End Sub

        Private Sub AddAttachment(ByVal refNum As Integer, ByVal att_Area As AttachmentArea)
            Dim strSplit() As String
            Dim filename, directory, storeDir As String
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim x As Integer
            Dim ARec As New cAuditRecord
            Dim clsAccountSubAccounts As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, clsAccountSubAccounts.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim clsAccountProperties As cAccountProperties
            clsAccountProperties = clsAccountSubAccounts.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            ' establish whether more than one '.' exist in full path. Error. This will prevent file type ident
            Dim tmpFilename As String
            Dim preExtension As String
            Dim extension As String = ""
            Dim xref, dotPos As Integer
            Dim uploadSize As Integer
            uploadSize = attbrowser.PostedFile.ContentLength / 1024
            If uploadSize > clsAccountProperties.MaxUploadSize Then
                lblStatusMsg.Text = "File size exceeded maximum allowed (" + clsAccountProperties.MaxUploadSize.ToString() + ")."
                Exit Sub
            End If

            tmpFilename = attbrowser.PostedFile.FileName '  attachment.PostedFile.FileName
            For dotPos = tmpFilename.Length To 1 Step -1
                If Mid(tmpFilename, dotPos, 1) = "." Then
                    extension = Mid(tmpFilename, dotPos + 1)
                    Exit For
                End If
            Next

            db.DBOpen(fws, False)

            If ValidExtension(db, extension) = False Then
                lblStatusMsg.Text = "Attempt to attach file rejected. File Type not permitted."
                db.DBClose()
                Exit Sub
            End If

            ReDim strSplit(50)
            strSplit = Split(attbrowser.PostedFile.FileName, "\")
            directory = ""

            For x = 0 To strSplit.Length - 2
                directory = directory & Trim(strSplit(x)) & "\"
            Next

            filename = strSplit(strSplit.Length - 1)

            If Trim(filename) = "" Then
                db.DBClose()
                Exit Sub
            End If

            storeDir = lstStorageDir.SelectedItem.Text
            If storeDir = "[Root]" Then
                Select Case CType(lstAttachmentType.SelectedItem.Value, AttachmentType)
                    Case AttachmentType.Open, AttachmentType.Audience
                        ' open
                        directory = Trim(fws.glDocRepository)
                    Case AttachmentType.Secure
                        ' secure
                        directory = Trim(fws.glSecureDocRepository)
                    Case Else
                        db.DBClose()
                        db = Nothing
                        Exit Sub
                End Select
            Else
                directory = Replace(storeDir, "\", "/")
                Select Case CType(lstAttachmentType.SelectedItem.Value, AttachmentType)
                    Case AttachmentType.Open, AttachmentType.Audience
                        directory = Replace(directory, "[Root]", fws.glDocRepository.Trim)

                    Case AttachmentType.Secure
                        directory = Replace(directory, "[Root]", fws.glSecureDocRepository.Trim)

                    Case Else
                        db.DBClose()
                        db = Nothing
                        Exit Sub
                End Select
            End If

            If txtNewSubDir.Text <> "" Then
                ' new directory to be created
                Dim strDir, strDir2 As String
                Dim sysPath As String

                sysPath = Server.MapPath(directory)
                Dim tmpDir As New System.IO.DirectoryInfo(sysPath)

                If tmpDir.Exists = False Then
                    tmpDir.Create()
                End If

                strDir2 = Replace(txtNewSubDir.Text, "\", "/")
                If Left(strDir2, 1) = "/" Then
                    strDir = Mid(strDir2, 2)
                Else
                    strDir = strDir2
                End If

                Dim subDir As System.IO.DirectoryInfo = tmpDir.CreateSubdirectory(strDir)

                If Left(strDir, 1) <> "/" Then
                    strDir = "/" & strDir
                End If

                If Right(strDir, 1) <> "/" Then
                    strDir = strDir & "/"
                End If

                directory = directory & Trim(strDir)
            Else
                If Right(directory, 1) <> "/" Then
                    directory = directory & "/"
                End If
            End If

            Dim bIsUnique As Boolean
            Dim seqNum As Integer
            seqNum = 1
            bIsUnique = False

            Do While Not bIsUnique
                ' ensure that file of that name doesn't already exist. if so, append seq no on the end
                If System.IO.File.Exists(Server.MapPath(directory.Trim & filename.Trim)) = True Then
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
                    preExtension = Mid(filename, 1, filename.Length - (filename.Length - newDotPos) - 1)

                    If xref > 0 Then
                        preExtension = Left(preExtension, xref - 1)
                    End If

                    filename = Trim(preExtension) & "_" & seqNum.ToString.Trim & "." & extension.Trim
                    seqNum = seqNum + 1
                Else
                    ' store the file as the filename is now unique
                    bIsUnique = True
                End If
            Loop

            attbrowser.PostedFile.SaveAs(Server.MapPath(directory.Trim & filename.Trim))

            db.SetFieldValue("ReferenceNumber", refNum, "N", True)
            db.SetFieldValue("Filename", filename, "S", False)
            db.SetFieldValue("Directory", directory, "S", False)
            db.SetFieldValue("Description", Left(Trim(txtDescription.Text), 50), "S", False)
            db.SetFieldValue("DateAttached", Now(), "D", False)
            If cDef.DBVERSION >= 18 Then
                db.SetFieldValue("AttachedBy", curUser.Employee.Forename & " " & curUser.Employee.Surname, "S", False)
            End If
            db.SetFieldValue("AttachmentType", lstAttachmentType.SelectedItem.Value, "N", False)
            db.SetFieldValue("AttachmentArea", att_Area, "N", False)

            ' find the mime header for the file extension type and store it
            Dim mimeTypes As New cGlobalMimeTypes(curUser.AccountID)
            Dim mimeType As cGlobalMimeType = mimeTypes.getMimeTypeByExtension(extension.Trim)

            If Not mimeType Is Nothing Then
                db.SetFieldValue("MimeHeader", mimeType.MimeHeader, "S", False)
            Else
                db.SetFieldValue("MimeHeader", "text/html", "S", False)
            End If

            db.FWDb("W", "attachments", "", "", "", "", "", "", "", "", "", "", "", "")

            ARec.Action = cFWAuditLog.AUDIT_ADD
            If db.FWDbFindVal("Contract Key", 2) <> "" Then
                ARec.ContractNumber = db.FWDbFindVal("ContractKey", 2)
            Else
                If db.FWDbFindVal("ContractNumber", 2) <> "" Then
                    ARec.ContractNumber = db.FWDbFindVal("ContractNumber", 2)
                Else
                    ARec.ContractNumber = "n/a"
                End If
            End If
            ARec.DataElementDesc = "FILE ATTACHMENT"
            ARec.ElementDesc = txtDescription.Text
            ARec.PreVal = ""
            If filename.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                ARec.PostVal = filename.Substring(filename.Length - cFWAuditLog.MAX_AUDITVAL_LEN)
            Else
                ARec.PostVal = filename
            End If

            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Attachments, curUser.CurrentSubAccountId)
            ALog.AddAuditRec(ARec, True)
            ALog.CommitAuditLog(curUser.Employee, 0)

            'myfile.PostedFile.SaveAs(newfile)
            Dim retParams As String = ""
            If returnURL.Value <> "" Then
                retParams = "&ret=" & returnURL.Value
            End If
            Response.Redirect("Attachments.aspx?attarea=" & Trim(att_Area) & "&ref=" & refNum.ToString.Trim & retParams, True)
        End Sub

        Private Function GetSimpleAttachmentData(ByVal db As cFWDBConnection) As String
            Dim strHTML As New System.Text.StringBuilder
            Dim drow As DataRow
            Dim tmpstr As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim rowalt As Boolean = False
            Dim rowClass As String = ""

            strHTML.Append("<table class=""datatbl shortTable"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th style=""width:21px""><img src=""./icons/16/plain/folder_out.png"" alt=""Open"" /></th>" & vbNewLine)
            strHTML.Append("<th style=""width:21px""><img src=""./icons/edit.gif"" alt=""Edit"" /></th>" & vbNewLine)
            strHTML.Append("<th style=""width:21px""><img src=""./icons/delete2.gif"" alt=""Delete"" /></th>" & vbNewLine)
            strHTML.Append("<th style=""width:21px""><img src=""./icons/16/plain/view.png"" alt=""Audience"" /></th>" & vbNewLine)
            strHTML.Append("<th width=""200"">Description</th>" & vbNewLine)
            strHTML.Append("<th width=""200"">File Location</th>" & vbNewLine)
            strHTML.Append("<th width=""100"">Date Attached</th>" & vbNewLine)
            strHTML.Append("<th width=""100"">Attached By</th>" & vbNewLine)
            strHTML.Append("</tr>" & vbNewLine)

            If db.GetRowCount(db.glDBWorkA, 0) = 0 Then
                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""row1""")
                strHTML.Append("colspan=""8"" ")
                strHTML.Append("align=""center"">There are currently no attachments against this contract.</td>" & vbNewLine)
                strHTML.Append("</tr>" & vbNewLine)
            Else
                Dim crypt As New cSecureData

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    If drow.Item("PermitVal") > 0 Then
                        ' only display attachment if permitted to access
                        rowalt = (rowalt Xor True)

                        If rowalt = True Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        strHTML.Append("<tr>" & vbNewLine)

                        Select Case CType(drow.Item("AttachmentType"), AttachmentType)
                            Case AttachmentType.Secure
                                strHTML.Append("<td class=""" & rowClass & """ align=""center""><a href=""Attachments.aspx?action=email&attarea=" & Trim(att_area.Text) & "&ref=" & Trim(Session("RefNum")) & "&id=" & Trim(drow.Item("AttachmentId")) & """ title=""Open attachment"" onmouseover=""window.status='Open attachment in new window';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/16/plain/folder_out.png"" alt=""Open"" /></a></td>" & vbNewLine)
                            Case AttachmentType.Audience
                                strHTML.Append("<td class=""" & rowClass & """ align=""center""><a target=""_blank"" href=""ViewAttachment.aspx?id=" & Server.UrlEncode(crypt.Encrypt(CStr(drow.Item("AttachmentId")))) & """ onmouseover=""window.status='Open the attachment in a new window';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/16/plain/folder_out.png"" alt=""Open"" /></a></td>" & vbNewLine)
                            Case AttachmentType.Hyperlink
                                strHTML.Append("<td class=""" & rowClass & """ align=""center""><a target=""_blank"" href=""" & drow.Item("Directory") & """ onmouseover=""window.status='Open hyperlink document';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/16/plain/folder_out.png"" alt=""Open"" /></a></td>" & vbNewLine)
                            Case Else
                                strHTML.Append("<td class=""" & rowClass & """ align=""center""><a onclick=""javascript:window.open('ViewAttachment.aspx?id=" & Server.UrlEncode(crypt.Encrypt(CStr(drow.Item("AttachmentId")))) & "');"" onmouseover=""window.status='Open the attachment in a new window';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/16/plain/folder_out.png"" alt=""Open"" /></a></td>" & vbNewLine)
                        End Select

                        If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Attachments, False) Then
                            strHTML.Append("<td class=""" & rowClass & """ align=""center""><a href=""Attachments.aspx?action=edit&attarea=" & att_area.Text.Trim & "&ref=" & Session("RefNum") & "&id=" & Trim(drow.Item("AttachmentId")) & """ title=""Edit attachment definition"" onmouseover=""window.status='Edit the attachment definition';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/edit.gif"" alt=""Edit"" /></a></td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """ align=""center"">&nbsp;</td>" & vbNewLine)
                        End If

                        If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Attachments, False) Then
                            strHTML.Append("<td class=""" & rowClass & """ align=""center""><a href=""javascript:if(confirm('Are you sure you want to delete attachment? OK to confirm.')){window.location.href='Attachments.aspx?action=delete&attarea=" & Trim(att_area.Text) & "&id=" & Trim(drow.Item("AttachmentId")) & "&ref=" & Trim(Session("RefNum")) & "';}""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """ align=""center"">&nbsp;</td>" & vbNewLine)
                        End If

                        If (drow.Item("PermitVal") > 0 Or drow.Item("permitVal") = 999) And (CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Audience Or CType(drow.Item("AttachmentType"), AttachmentType) = AttachmentType.Hyperlink) And curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.AttachmentAudience, False) Then
                            ' has an audience, but is not open to all, so allow this user access to the permit list
                            Session("AudienceReturnURL") = "Attachments.aspx?" & Page.ClientQueryString
                            strHTML.Append("<td class=""" & rowClass & """ align=""center""><a href=""SetAudience.aspx?type=attachment&id=" & drow.Item("AttachmentId") & """ title=""Update the permitted Audience""><img src=""./icons/16/plain/view.png"" alt=""Amend"" /></a></td>" & vbNewLine)
                        Else
                            strHTML.Append("<td class=""" & rowClass & """ align=""center"">&nbsp;</td>" & vbNewLine)
                        End If

                        If IsDBNull(drow.Item("Description")) = True Then
                            tmpstr = "&nbsp;"
                        Else
                            tmpstr = drow.Item("Description")
                        End If
                        'tmpstr = IIf(IsDBNull(drow.Item("Description")) = True, "&nbsp;", Trim(drow.Item("Description")))
                        strHTML.Append("<td class=""" & rowClass & """>" & tmpstr.Trim & "</td>" & vbNewLine)

                        If IsDBNull(drow.Item("Directory")) = True Then
                            tmpstr = "&nbsp;"
                        Else
                            tmpstr = drow.Item("Directory")
                        End If
                        'tmpstr = IIf(IsDBNull(drow.Item("Directory")) = True, "", Trim(drow.Item("Directory")))
                        If IsDBNull(drow.Item("Filename")) = False Then
                            tmpstr += drow.Item("Filename")
                        End If
                        'tmpstr = tmpstr & IIf(IsDBNull(drow.Item("Filename")) = True, "", Trim(drow.Item("Filename")))
                        strHTML.Append("<td class=""" & rowClass & """>" & IIf(tmpstr.Trim = "", "&nbsp;", tmpstr.Trim) & "</td>" & vbNewLine)

                        If IsDBNull(drow.Item("DateAttached")) = True Then
                            tmpstr = ""
                        Else
                            tmpstr = Format(CDate(drow.Item("DateAttached")), cDef.DATE_FORMAT)
                        End If

                        strHTML.Append("<td class=""" & rowClass & """>" & IIf(tmpstr.Trim = "", "&nbsp;", tmpstr.Trim) & "</td>" & vbNewLine)
                        If cDef.DBVERSION >= 18 Then
                            If IsDBNull(drow.Item("AttachedBy")) = False Then
                                tmpstr = drow.Item("AttachedBy")
                            Else
                                tmpstr = "Unknown"
                            End If
                            strHTML.Append("<td class=""" & rowClass & """>" & tmpstr.Trim & "</td>" & vbNewLine)
                        End If
                        strHTML.Append("</tr>" & vbNewLine)
                    End If
                Next
            End If

            strHTML.Append("</table>" & vbNewLine)

            Session("AttachmentHTML") = strHTML.ToString
            GetSimpleAttachmentData = strHTML.ToString
        End Function

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
            If uploadFilePanel.Visible Then
                If attbrowser.HasFile Then
                    AddAttachment(Session("RefNum"), CType(att_area.Text, AttachmentArea))
                Else
                    lblStatusMsg.Text = "An upload file must be specified."
                End If
            Else
                UpdateAttachment()
            End If
        End Sub

        Private Sub GetFolderList(ByVal fws As cFWSettings, ByVal startdir As String, ByVal absPath As String, ByVal virtualPath As String)
            ' populate the drop down list with available subdirectories
            Dim storePath As String
            Dim dirList() As String
            Dim curIdx As Integer
            Dim tmpStr As String

            tmpStr = Trim(Replace(startdir, absPath, virtualPath))
            storePath = Server.MapPath(tmpStr)
            dirList = System.IO.Directory.GetDirectories(storePath)

            If dirList.Length = 0 Then
                Exit Sub
            End If

            For curIdx = 0 To dirList.Length - 1
                lstStorageDir.Items.Add(Replace(dirList(curIdx), absPath, "[Root]"))
                GetFolderList(fws, dirList(curIdx), absPath, virtualPath)
            Next
        End Sub

        Protected Sub lstAttachmentType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstAttachmentType.SelectedIndexChanged
            ' switch the folder list to represent the selected repository file structure
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)

            lstStorageDir.Items.Clear()

            Select Case CType(lstAttachmentType.SelectedItem.Value, AttachmentType)
                Case AttachmentType.Open, AttachmentType.Audience
                    ' open
                    GetFolderList(fws, fws.glDocRepository, Server.MapPath(fws.glDocRepository), fws.glDocRepository)

                Case AttachmentType.Secure
                    ' secure
                    GetFolderList(fws, fws.glSecureDocRepository, Server.MapPath(fws.glSecureDocRepository), fws.glSecureDocRepository)

                Case Else
                    Exit Sub
            End Select

            lstStorageDir.Items.Insert(0, "[Root]")
        End Sub

        Private Function ValidExtension(ByVal db As cFWDBConnection, ByVal ext As String) As Boolean

            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim globMimeTypes As New cGlobalMimeTypes(curUser.AccountID)

            Dim globMimeType As cGlobalMimeType = globMimeTypes.getMimeTypeByExtension(ext)

            If IsNothing(globMimeType) Then
                Return False
            End If

            Dim mimeTypes As New cMimeTypes(curUser.AccountID, curUser.CurrentSubAccountId)
            Dim mimeType = mimeTypes.GetMimeTypeByGlobalID(globMimeType.GlobalMimeID)

            If IsNothing(mimeType) Then
                Return False
            Else
                If mimeType.Archived Then
                    Return False
                End If
            End If

            Return True
        End Function

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
            If Request.QueryString("action") = "edit" Then
                Dim retParams As String = ""
                If returnURL.Value <> "" Then
                    retParams = "&ret=" & returnURL.Value
                End If
                Response.Redirect("Attachments.aspx?attarea=" & CType(att_area.Text, AttachmentArea) & "&ref=" & Session("RefNum") & retParams, True)
            Else
                AddPanel.Visible = False
                Master.enablenavigation = False
                Master.useCloseNavigationMsg = True
                Master.RefreshBreadcrumbInfo()
            End If
        End Sub

        Private Sub PopulateAttachmentTypes(ByVal attArea As AttachmentArea)
            With lstAttachmentType
                .Items.Add(New ListItem("Open Attachment", AttachmentType.Open))
                .Items.Add(New ListItem("Reserved Audience", AttachmentType.Audience))
            End With
        End Sub

        Protected Sub lnkAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAdd.Click
            HyperlinkPanel.Visible = False
            AddPanel.Visible = True
            SetLinkValidators()
            Master.enablenavigation = False
            Master.useCloseNavigationMsg = False
            Master.RefreshBreadcrumbInfo()
        End Sub

        Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
            Dim ref As Integer
            ref = Request.QueryString("ref")

            Select Case CType(att_area.Text, AttachmentArea)
                Case AttachmentArea.CONTRACT
                    Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.ContractDetail & "&id=" & ref.ToString, True)

                Case AttachmentArea.CONTRACT_NOTES, AttachmentArea.CONTACT_NOTES, AttachmentArea.INVOICE_NOTES, AttachmentArea.PRODUCT_NOTES, AttachmentArea.VENDOR_NOTES
                    Response.Redirect(Session("NoteAttachmentReturnURL"), True)

                Case AttachmentArea.VENDOR
                    Response.Redirect("~/shared/supplier_details.aspx?sid=" & ref.ToString, True)

                Case AttachmentArea.TASKS
                    Dim curUser As CurrentUser = cMisc.GetCurrentUser()
                    Dim tasks As New cTasks(curUser.Account.accountid, curUser.CurrentSubAccountId)
                    Dim task As cTask = tasks.GetTaskById(ref)
                    Dim retParams As String = ""
                    If returnURL.Value <> "" Then
                        retParams = "&ret=" & returnURL.Value
                    End If
                    Response.Redirect(cMisc.Path & "/shared/tasks/ViewTask.aspx?tid=" & ref.ToString & "&rid=" & task.RegardingId.ToString & "&rtid=" & task.RegardingArea & retParams, True)
                Case Else

            End Select
        End Sub

        Protected Sub lnkLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkLink.Click
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim linkDefault As Integer = 0
            Dim helpID As Integer = 1012

            linkDefault = params.LinkAttachmentDefault

            AddPanel.Visible = False
            HyperlinkPanel.Visible = True
            SetLinkValidators()

            rdoLinkType.Items(linkDefault).Selected = True
            Select Case linkDefault
                Case 0
                    panelFileLink.Visible = False
                    panelWebLink.Visible = True
                    txtWebLink.Text = "http://"
                    helpID = 1168
                Case 1
                    panelFileLink.Visible = True
                    panelWebLink.Visible = False
                    txtCurFileLink.Text = "New Link"
                    txtCurFileLink.ReadOnly = True
                    helpID = 1218
                Case Else

            End Select

            Master.enablenavigation = False
            Master.useCloseNavigationMsg = False
            Master.RefreshBreadcrumbInfo()
        End Sub

        Protected Sub cmdLinkCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdLinkCancel.Click
            If Request.QueryString("action") = "edit" Then
                Dim retParams As String = ""
                If returnURL.Value <> "" Then
                    retParams = "&ret=" & returnURL.Value
                End If
                Response.Redirect("Attachments.aspx?attarea=" & CType(att_area.Text, AttachmentArea) & "&ref=" & Session("RefNum") & retParams, True)
            Else
                HyperlinkPanel.Visible = False
                Master.enablenavigation = False
                Master.useCloseNavigationMsg = True
                Master.RefreshBreadcrumbInfo()
            End If
        End Sub

        Private Sub SetLinkValidators()
            reqLinkURL.Enabled = HyperlinkPanel.Visible
            reqLinkDescription.Enabled = HyperlinkPanel.Visible
        End Sub

        Protected Sub cmdLinkUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdLinkUpdate.Click
            ' stores the link in the Directory field and sets attachment type = 3 (attachmenttype_Linked)
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ARec As New cAuditRecord
            Dim prevURL As String = ""
            Dim prevDesc As String = ""
            Dim isEdit As Boolean = False

            db.DBOpen(fws, False)
            If editID.Text = "0" Then
                isEdit = False
            Else
                db.FWDb("R", "attachments", "AttachmentId", editID.Text, "", "", "", "", "", "", "", "", "", "")
                If db.FWDbFlag Then
                    prevURL = db.FWDbFindVal("Directory", 1)
                    prevDesc = db.FWDbFindVal("Description", 1)
                End If
                isEdit = True
            End If

            If Not isEdit Then
                db.SetFieldValue("ReferenceNumber", Session("RefNum"), "N", True)
                db.SetFieldValue("DateAttached", Now, "D", False)
                db.SetFieldValue("AttachedBy", curUser.Employee.Forename & " " & curUser.Employee.Surname, "S", False)
                db.SetFieldValue("AttachmentType", AttachmentType.Hyperlink, "N", False)
                db.SetFieldValue("AttachmentArea", CType(att_area.Text, AttachmentArea), "N", False)
            End If

            Select Case rdoLinkType.SelectedItem.Value
                Case "0"
                    ' web link
                    db.SetFieldValue("Directory", txtWebLink.Text, "S", isEdit)
                    ARec.ElementDesc = "WEB LINK"
                Case "1"
                    ' file link
                    If lnkFileSelect.PostedFile.FileName.StartsWith("file://") = False Then
                        db.SetFieldValue("Directory", "file://" & lnkFileSelect.PostedFile.FileName, "S", isEdit)
                    Else
                        db.SetFieldValue("Directory", lnkFileSelect.PostedFile.FileName, "S", isEdit)
                    End If
                    ARec.ElementDesc = "FILE LINK"

                Case Else
                    db.SetFieldValue("Directory", txtWebLink.Text, "S", False)
                    ARec.ElementDesc = "WEB LINK"
            End Select

            'db.SetFieldValue("Directory", txtLinkURL.Text.Trim, "S", IIf(editID.Text = "0", False, True))
            db.SetFieldValue("Description", txtLinkDescription.Text, "S", False)

            If Not isEdit Then
                db.FWDb("W", "attachments", "", "", "", "", "", "", "", "", "", "", "", "")
                ARec.Action = cFWAuditLog.AUDIT_ADD
            Else
                db.FWDb("A", "attachments", "AttachmentId", editID.Text, "", "", "", "", "", "", "", "", "", "")
                ARec.Action = cFWAuditLog.AUDIT_UPDATE
            End If

            UpdateAuditRecValues(db, ARec)

            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Attachments, curUser.CurrentSubAccountId)
            ARec.DataElementDesc = ARec.DataElementDesc & ":LINK ATTACHMENT"

            If editID.Text = "0" Then
                ARec.PreVal = ""
                If txtLinkDescription.Text.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                    ARec.PostVal = txtLinkDescription.Text.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                Else
                    ARec.PostVal = txtLinkDescription.Text
                End If
                ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, editID.Text)
            Else
                If prevDesc <> txtLinkDescription.Text Then
                    ARec.ElementDesc = "LINK DESCRIPTION"
                    ARec.PreVal = prevDesc
                    If txtLinkDescription.Text.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                        ARec.PostVal = txtLinkDescription.Text.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                    Else
                        ARec.PostVal = txtLinkDescription.Text
                    End If
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, editID.Text)
                End If

                Select Case rdoLinkType.SelectedItem.Value
                    Case "0"
                        If prevURL <> txtWebLink.Text Then
                            ARec.ElementDesc = "WEB LINK"
                            ARec.PreVal = prevURL
                            ARec.PostVal = txtWebLink.Text
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, editID.Text)
                        End If

                    Case "1"
                        If prevURL <> lnkFileSelect.PostedFile.FileName Then
                            ARec.ElementDesc = "FILE LINK"
                            ARec.PreVal = prevURL
                            ARec.PostVal = lnkFileSelect.PostedFile.FileName
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, editID.Text)
                        End If

                    Case Else

                End Select

            End If

            db.DBClose()

            Dim retParams As String = ""
            If returnURL.Value <> "" Then
                retParams = "&ret=" & returnURL.Value
            End If
            Response.Redirect("Attachments.aspx?attarea=" & CType(att_area.Text, AttachmentArea) & "&ref=" & Session("RefNum") & retParams, True)
        End Sub

        Private Sub EditAttachment(ByVal db As cFWDBConnection, ByVal att_area As AttachmentArea, ByVal att_ID As Integer)
            db.FWDb("R2", "attachments", "AttachmentId", att_ID, "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag Then
                editID.Text = att_ID
                Select Case CType(db.FWDbFindVal("AttachmentType", 2), AttachmentType)
                    Case AttachmentType.Open, AttachmentType.Audience
                        HyperlinkPanel.Visible = False
                        AddPanel.Visible = True
                        uploadFilePanel.Visible = False
                        lstStorageDir.Enabled = False
                        txtNewSubDir.Enabled = False
                        SetLinkValidators()

                        txtDescription.Text = db.FWDbFindVal("Description", 2)

                    Case AttachmentType.Hyperlink
                        AddPanel.Visible = False
                        HyperlinkPanel.Visible = True
                        SetLinkValidators()

                        Dim tmpLink As String = db.FWDbFindVal("Directory", 2)
                        If tmpLink.StartsWith("http://") Then
                            rdoLinkType.Items(0).Selected = True
                            txtWebLink.Text = tmpLink
                            panelFileLink.Visible = False
                            panelWebLink.Visible = True
                        Else
                            rdoLinkType.Items(1).Selected = True
                            txtCurFileLink.Text = tmpLink
                            txtCurFileLink.ReadOnly = True
                            panelFileLink.Visible = True
                            panelWebLink.Visible = False
                        End If

                        'lnkFileSelect.PostedFile.FileName = db.FWDbFindVal("Directory", 2) 'txtLinkURL.Text =
                        txtLinkDescription.Text = db.FWDbFindVal("Description", 2)

                End Select
            End If
        End Sub

        Private Sub UpdateAttachment()
            Dim db As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)

            db.DBOpen(fws, False)
            ARec.Action = cFWAuditLog.AUDIT_UPDATE
            ARec.ElementDesc = txtDescription.Text

            UpdateAuditRecValues(db, ARec)

            Dim prevDesc As String = ""
            db.FWDb("R2", "attachments", "AttachmentId", editID.Text, "", "", "", "", "", "", "", "", "", "")
            If db.FWDb2Flag Then
                prevDesc = db.FWDbFindVal("Description", 2)

                If prevDesc <> txtDescription.Text Then
                    Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Attachments, curUser.CurrentSubAccountId)

                    ' must have been amended
                    If prevDesc.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                        ARec.PreVal = prevDesc.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                    Else
                        ARec.PreVal = prevDesc
                    End If

                    If txtDescription.Text.Length > cFWAuditLog.MAX_AUDITVAL_LEN Then
                        ARec.PostVal = txtDescription.Text.Substring(1, cFWAuditLog.MAX_AUDITVAL_LEN)
                    Else
                        ARec.PostVal = txtDescription.Text
                    End If

                    db.SetFieldValue("Description", txtDescription.Text, "S", True)
                    db.FWDb("A", "attachments", "AttachmentId", editID.Text, "", "", "", "", "", "", "", "", "", "")

                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, editID.Text)
                End If
            End If

            db.DBClose()

            Dim retParams As String = ""
            If returnURL.Value <> "" Then
                retParams = "&ret=" & returnURL.Value
            End If
            Response.Redirect("Attachments.aspx?attarea=" & CType(att_area.Text, AttachmentArea) & "&ref=" & Session("RefNum") & retParams, True)
        End Sub

        Private Sub UpdateAuditRecValues(ByVal db As cFWDBConnection, ByRef ARec As cAuditRecord)
            Select Case CType(att_area.Text, AttachmentArea)
                Case AttachmentArea.CONTRACT
                    ARec.DataElementDesc = "CON.DETAILS"
                    db.FWDb("R3", "contract_details", "ContractId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                    End If

                Case AttachmentArea.CONTACT_NOTES
                    ARec.DataElementDesc = "CONTACT NOTES"
                    ARec.ElementDesc = ""
                    db.FWDb("R3", "supplierContactNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        db.FWDb("R2", "supplier_contacts", "contactId", db.FWDbFindVal("Reference Number", 3), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            ARec.ElementDesc = db.FWDbFindVal("contactName", 2)
                        End If
                    End If
                    ARec.ContractNumber = ""

                Case AttachmentArea.CONTRACT_NOTES
                    ARec.DataElementDesc = "CON.NOTES"
                    db.FWDb("R3", "contractNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.ContractNumber = db.FWDbFindVal("ContractKey", 3)
                    End If

                Case AttachmentArea.INVOICE_NOTES
                    ARec.DataElementDesc = "INV.NOTES"
                    db.FWDb("R3", "invoiceNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.ContractNumber = ""
                        ARec.ElementDesc = "INV:" & db.FWDbFindVal("InvoiceNumber", 3)
                    End If

                Case AttachmentArea.PRODUCT_NOTES
                    ARec.DataElementDesc = "PROD.NOTES"
                    db.FWDb("R3", "productNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.ContractNumber = ""
                        db.FWDb("R2", "productDetails", "ProductId", db.FWDbFindVal("ProductId", 3), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            ARec.ContractNumber = ""
                            ARec.ElementDesc = db.FWDbFindVal("ProductName", 2)
                        End If
                    End If

                Case AttachmentArea.VENDOR
                    ARec.DataElementDesc = "SUPP.DETAILS"
                    db.FWDb("R3", "supplier_details", "supplierId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        ARec.ContractNumber = ""
                        ARec.ElementDesc = db.FWDbFindVal("supplierName", 3)
                    End If

                Case AttachmentArea.VENDOR_NOTES
                    ARec.DataElementDesc = "SUPP.NOTES"
                    db.FWDb("R3", "supplierNotes", "NoteId", Session("RefNum"), "", "", "", "", "", "", "", "", "", "")
                    If db.FWDb3Flag Then
                        db.FWDb("R2", "supplier_details", "supplierId", db.FWDbFindVal("supplierId", 3), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag Then
                            ARec.ContractNumber = ""
                            ARec.ElementDesc = db.FWDbFindVal("supplierName", 2)
                        End If
                    End If
                Case Else
                    ARec.DataElementDesc = "UNKNOWN"
            End Select
        End Sub

        Protected Sub rdoLinkType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdoLinkType.SelectedIndexChanged
            Dim helpID As Integer = 1280

            Select Case rdoLinkType.SelectedItem.Value
                Case "0"
                    panelFileLink.Visible = False
                    panelWebLink.Visible = True
                    helpID = 1168
                Case "1"
                    panelFileLink.Visible = True
                    panelWebLink.Visible = False
                    helpID = 1218
                Case Else

            End Select
        End Sub
    End Class
End Namespace
