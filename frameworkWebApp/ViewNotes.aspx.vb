Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Web.Services

Namespace Framework2006
    Partial Class ViewNotes
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
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim getItemName As Boolean = False
            Dim ActiveNoteOwner As Integer
            Dim activeContractId = Request.QueryString("contractid")
            If activeContractId Is Nothing 
                activeContractId = Server.UrlDecode(Request.QueryString("ret"))
                If activeContractId is Nothing 
                    activeContractId = 0 
                End If
            End If
            ViewState("ActiveContract") = activeContractId 


            If Me.IsPostBack = False Then
                FWDb.DBOpen(fws, False)

                Session("NoteType") = Replace(Request.QueryString("notetype"), "_", " ")
                Session("CurNoteId") = Request.QueryString("id")

                If Not Request.QueryString("ret") Is Nothing Then
                    Session("NoteReturnURL") = Server.UrlDecode(Request.QueryString("ret"))
                End If

                Select Case Request.QueryString("action")
                    Case "delete"
                        DeleteNote(FWDb)

                    Case Else

                End Select

                Master.enablenavigation = False

                lblNoteDate.Text = "Date: "
                lblNoteType.Text = "Note Type"
                lblNoteCategory.Text = "Note Category"

                If Not Request.QueryString("item") Is Nothing Then
                    Session("NotesForText") = Request.QueryString("item")
                Else
                    getItemName = True
                End If

                ActiveNoteOwner = LoadNote(FWDb, getItemName)
                SetPermissions(FWDb, fws, curUser, ActiveNoteOwner)

                txtCreatedDate.MaxValue = DateTime.UtcNow.Date

                FWDb.DBClose()
                FWDb = Nothing
            End If
        End Sub

        Private Function GetNote(ByVal fwdb As cFWDBConnection, ByVal NoteIdx As Integer) As Integer
            Dim sql As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim emps As New cEmployees(curUser.AccountID)

            fwdb.FWDb("R", Session("NoteTable"), "NoteId", NoteIdx, "", "", "", "", "", "", "", "", "", "")
            If fwdb.FWDbFlag = True Then
                txtNote.Text = fwdb.FWDbFindVal("Note", 1)
                Dim empId As Integer = 0
                Integer.TryParse(fwdb.FWDbFindVal("createdby", 1), empId)
                If empId > 0 Then
                    txtCreatedBy.Text = emps.GetEmployeeById(empId).fullname
                Else
                    txtCreatedBy.Text = ""
                End If

                txtCreatedDate.Value = IIf(fwdb.FWDbFindVal("Date", 1) = "", "", Format(CDate(fwdb.FWDbFindVal("Date", 1)), cDef.DATE_FORMAT))

                If fwdb.FWDbFindVal("noteType", 1) <> "0" Then
                    lstNoteCategory.SelectedIndex = lstNoteCategory.Items.IndexOf(lstNoteCategory.Items.FindByValue(fwdb.FWDbFindVal("noteType", 1)))
                End If

                lstNoteType.SelectedIndex = lstNoteType.Items.IndexOf(lstNoteType.Items.FindByValue(fwdb.FWDbFindVal("noteCatId", 1)))

                ' repopulate the category depending on the type selected
                sql = "SELECT * FROM [codes_notecategory] WHERE [noteType] = @noteType"
                fwdb.AddDBParam("noteType", lstNoteCategory.SelectedItem.Value, True)
                fwdb.RunSQL(sql, fwdb.glDBWorkA, False, "", False)
                If fwdb.GetRowCount(fwdb.glDBWorkA, 0) > 0 Then
                    lstNoteType.Items.Clear()
                    lstNoteType.DataTextField = "fullDescription"
                    lstNoteType.DataValueField = "noteCatId"
                    lstNoteType.DataSource = fwdb.glDBWorkA
                    lstNoteType.DataBind()

                    lstNoteType.SelectedIndex = lstNoteType.Items.IndexOf(lstNoteType.Items.FindByValue(fwdb.FWDbFindVal("noteCatId", 1)))
                Else
                    lstNoteType.Items.Clear()
                    lstNoteType.Items.Add(New ListItem("[no definitions]", 0))
                    lstNoteType.Enabled = False
                End If

                Session("CurNoteId") = NoteIdx
            End If
            Return Integer.Parse(fwdb.FWDbFindVal("createdBy", 1))
        End Function

        <WebMethod(True)> _
        Public Shared Function ExitNotes() As String
            Dim appinfo As HttpApplication = CType(HttpContext.Current.ApplicationInstance, HttpApplication)
            appinfo.Session("CurNoteId") = Nothing
            appinfo.Session("NoteTable") = Nothing
            appinfo.Session("NotesIDField") = Nothing
            appinfo.Session("NotesForText") = Nothing
            appinfo.Session("NotesForID") = Nothing
            appinfo.Session("NoteType") = Nothing

            If Not appinfo.Session("NoteReturnURL") Is Nothing Then
                Return appinfo.Session("NoteReturnURL")
            Else
                Return "Home.aspx"
            End If
        End Function

        Private Sub DeleteNote(ByVal FWDb As cFWDBConnection)
            Dim ARec As New cAuditRecord
            Dim tmpStr, sql As String
            Dim att_area As AttachmentArea
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            curUser.CheckAccessRole(AccessRoleType.Delete, getNoteElement(), False, True)

            If Session("CurNoteId") > 0 Then
                FWDb.FWDb("R2", Session("NoteTable"), "noteId", Session("CurNoteId"), "", "", "", "", "", "", "", "", "", "")
                Select Case Session("NoteTable")
                    Case "contractNotes"
                        FWDb.FWDb("R", "contract_details", "ContractId", FWDb.FWDbFindVal("ContractId", 2), "", "", "", "", "", "", "", "", "", "")
                        If FWDb.FWDb2Flag = True Then
                            att_area = AttachmentArea.CONTRACT_NOTES
                            tmpStr = FWDb.FWDbFindVal("ContractKey", 1)
                            If tmpStr = "" Then
                                ARec.ContractNumber = FWDb.FWDbFindVal("ContractNumber", 1)
                            Else
                                ARec.ContractNumber = tmpStr
                            End If
                        Else
                            ARec.ContractNumber = "Unknown"
                        End If
                        ARec.ElementDesc = ""

                    Case "invoiceNotes"
                        If FWDb.FWDb2Flag = True Then
                            att_area = AttachmentArea.INVOICE_NOTES
                            FWDb.FWDb("R", "invoiceDetails", "invoiceId", FWDb.FWDbFindVal("invoiceId", 2), "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDbFlag = True Then
                                ARec.ElementDesc = "INV:" & Trim(FWDb.FWDbFindVal("invoiceNumber", 1))
                            Else
                                ARec.ElementDesc = "Unknown Inv."
                            End If
                        End If
                        ARec.ContractNumber = ""

                    Case "supplierNotes"
                        If FWDb.FWDb2Flag = True Then
                            Dim suppliers As New cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId)
                            att_area = AttachmentArea.VENDOR_NOTES
                            Dim supplier As cSupplier = suppliers.getSupplierById(CInt(FWDb.FWDbFindVal("supplierId", 2)))

                            If Not supplier Is Nothing Then
                                ARec.ElementDesc = supplier.SupplierName
                            Else
                                ARec.ElementDesc = "Unknown Supplier"
                            End If
                        End If
                        ARec.ContractNumber = ""

                    Case "productNotes"
                        If FWDb.FWDb2Flag = True Then
                            att_area = AttachmentArea.PRODUCT_NOTES
                            FWDb.FWDb("R", "productDetails", "ProductId", FWDb.FWDbFindVal("ProductId", 2), "", "", "", "", "", "", "", "", "", "")
                            If FWDb.FWDbFlag = True Then
                                ARec.ElementDesc = FWDb.FWDbFindVal("ProductName", 1)
                            Else
                                ARec.ElementDesc = "Unknown Product"
                            End If
                        End If
                        ARec.ContractNumber = ""

                    Case "supplierContactNotes"
                        If FWDb.FWDb2Flag = True Then
                            Dim contact As cSupplierContact = cSupplierContacts.getContactByContactId(curUser.AccountID, CInt(FWDb.FWDbFindVal("contactId", 2)))

                            att_area = AttachmentArea.CONTACT_NOTES
                            If Not contact Is Nothing Then
                                ARec.ElementDesc = contact.Name
                            Else
                                ARec.ElementDesc = "Unknown Contact"
                            End If
                        End If
                        ARec.ContractNumber = ""

                    Case Else
                End Select

                ARec.Action = cFWAuditLog.AUDIT_DEL
                ARec.DataElementDesc = Session("NoteTable")
                If FWDb.FWDb2Flag = True Then
                    ARec.PreVal = Left(FWDb.FWDbFindVal("Note", 2), 59)
                Else
                    ARec.PreVal = ""
                End If
                ARec.PostVal = ""
                Dim ALog As New cFWAuditLog(fws, getNoteElement(), curUser.CurrentSubAccountId)
                ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, Session("CurNoteId"))

                FWDb.FWDb("D", Session("NoteTable"), "NoteId", Session("CurNoteId"), "", "", "", "", "", "", "", "", "", "")

                ' check to see if there are any associated attachments
                sql = "SELECT * FROM [attachments] WHERE [AttachmentArea] = @attArea AND [ReferenceNumber] = @refNum"
                FWDb.AddDBParam("attArea", Val(att_area), True)
                FWDb.AddDBParam("refNum", Session("CurNoteId"), False)
                FWDb.RunSQL(sql, FWDb.glDBWorkA, False, "", False)

                If FWDb.GetRowCount(FWDb.glDBWorkA, 0) > 0 Then
                    ' must be some attachments for this note, delete reference and the actual file!
                    Dim drow As DataRow

                    For Each drow In FWDb.glDBWorkA.Tables(0).Rows
                        If CType(drow.Item("AttachmentType"), AttachmentType) <> AttachmentType.Hyperlink Then
                            tmpStr = MapPath(drow.Item("Directory") & drow.Item("Filename"))
                            If System.IO.File.Exists(tmpStr) = True Then
                                System.IO.File.Delete(tmpStr)
                            End If
                        End If

                        ' write audit log entry that attachment also delete
                        ARec.Action = cFWAuditLog.AUDIT_DEL
                        ARec.DataElementDesc = Session("NoteTable")
                        ARec.ElementDesc = "ATTACHMENT"
                        ARec.PreVal = drow.Item("Filename")
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("CurNoteId"))

                        FWDb.FWDb("D", "attachments", "AttachmentId", drow.Item("AttachmentId"), "", "", "", "", "", "", "", "", "", "")
                    Next
                End If

                Response.Redirect(Session("refreshURL") & "&id=-1", True)
            End If
        End Sub

        Protected Sub lstNoteCategory_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim fwdb As New cFWDBConnection
            Dim tmpStr As String
            Dim drow As DataRow
            Dim sql As System.Text.StringBuilder
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim activeUpdateButton As Boolean = False

            fwdb.DBOpen(fws, False)

            lstNoteList.Items.Clear()
            lstNoteType.Items.Clear()

            If lstNoteCategory.SelectedItem.Value <> 0 Then
                sql = New System.Text.StringBuilder
                sql.Append("SELECT [fullDescription],[noteCatId] FROM [codes_notecategory] WHERE [noteType] = @noteType")
                sql.Append(" ORDER BY [fullDescription]")
                fwdb.AddDBParam("noteType", lstNoteCategory.SelectedItem.Value, True)
                fwdb.RunSQL(sql.ToString, fwdb.glDBWorkA, False, "", False)

                If fwdb.glNumRowsReturned > 0 Then
                    lstNoteType.DataTextField = "fullDescription"
                    lstNoteType.DataValueField = "noteCatId"
                    lstNoteType.DataSource = fwdb.glDBWorkA
                    lstNoteType.DataBind()

                    activeUpdateButton = True
                Else
                    ' no secondary definitions, do not permit updates.
                    activeUpdateButton = False
                End If
            Else
                lstNoteType.Items.Add(New ListItem("[no definitions]", 0))
                activeUpdateButton = False
            End If

            If curUser.CheckAccessRole(AccessRoleType.Edit, getNoteElement, False) Then
                cmdUpdate.Visible = activeUpdateButton
            End If

            sql = New System.Text.StringBuilder
            sql.Append("SELECT [NoteId],[Primary].[fullDescription] AS [PN],[Secondary].[fullDescription] AS [SN],[Date] FROM [" & Session("NoteTable") & "] ")
            sql.Append("LEFT OUTER JOIN [codes_notecategory] AS [Primary] ON [Primary].[noteCatId] = [" & Session("NoteTable") & "].[NoteType] ")
            sql.Append("LEFT OUTER JOIN [codes_notecategory] AS [Secondary] ON [Secondary].[noteCatId] = [" & Session("NoteTable") & "].[noteCatId] ")
            sql.Append("WHERE [" & Session("NotesIDField") & "] = @noteId")

            If lstNoteCategory.SelectedIndex <> 0 Then
                sql.Append(" AND ")
                sql.Append("[Primary].[noteCatId] = @noteCatId")
            End If

            sql.Append(" ORDER BY [Date] DESC")
            fwdb.AddDBParam("noteId", Session("NotesForID"), True)
            If lstNoteCategory.SelectedIndex <> 0 Then
                fwdb.AddDBParam("noteCatId", lstNoteCategory.SelectedItem.Value, False)
            End If
            fwdb.RunSQL(sql.ToString, fwdb.glDBWorkA, False, "", False)

            If fwdb.GetRowCount(fwdb.glDBWorkA, 0) > 0 Then
                For Each drow In fwdb.glDBWorkA.Tables(0).Rows
                    If IsDBNull(drow.Item("Date")) = False Then
                        tmpStr = Format(drow.Item("Date"), cDef.DATE_FORMAT) & " " & Trim(drow.Item("PN")) & " : " & Trim(drow.Item("SN"))
                    Else
                        If IsDBNull(drow.Item("FD")) Then
                            tmpStr = "??/??/???? - Unknown type"
                        Else
                            tmpStr = "??/??/????" & " " & Trim(drow.Item("PN")) & " : " & Trim(drow.Item("SN"))
                        End If

                    End If
                    lstNoteList.Items.Add(New ListItem(tmpStr, drow.Item("NoteId")))
                Next
            End If

            fwdb.DBClose()
            fwdb = Nothing
            sql = Nothing
        End Sub

        Private Sub UpdateNote()
            Dim FWDb As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim noChange As Boolean
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim ALog As New cFWAuditLog(fws, getNoteElement(), curUser.CurrentSubAccountId)
            Dim tmpStr As String
            Dim newID As Integer

            If lstNoteCategory.SelectedIndex = 0 Then
                FWDb = Nothing
                ARec = Nothing
                lblErrorString.Text = "Note category of [ALL] not permitted. Please select a valid category."
                Exit Sub
            End If

            noChange = True
            FWDb.DBOpen(fws, False)

            If Session("CurNoteId") < 1 Then
                curUser.CheckAccessRole(AccessRoleType.Add, getNoteElement(), False, True)

                ARec.Action = cFWAuditLog.AUDIT_ADD
                FWDb.SetFieldValue("Note", txtNote.Text, "S", True)
                If txtCreatedDate.Text = "" Then
                    FWDb.SetFieldValue("Date", Now, "D", False)
                Else
                    FWDb.SetFieldValue("Date", txtCreatedDate.Text, "D", False)
                End If

                FWDb.SetFieldValue("createdBy", curUser.Employee.employeeid, "N", False)
                FWDb.SetFieldValue("noteType", lstNoteCategory.SelectedItem.Value, "N", False)
                FWDb.SetFieldValue("noteCatId", lstNoteType.SelectedItem.Value, "N", False)
                FWDb.SetFieldValue(Session("NotesIDField"), Session("NotesForID"), "N", False)
                FWDb.FWDb("W", Session("NoteTable"), "", "", "", "", "", "", "", "", "", "", "", "")
                newID = FWDb.glIdentity
                Session("CurNoteId") = newID
                ARec.DataElementDesc = Session("NoteTable")
                ARec.ElementDesc = "New Note"
                ARec.PostVal = Left(txtNote.Text, 30)
                ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, newID)

                lblErrorString.Text = "Note Added Successfully"
                ' if inserting a new record but user has no amend rights, disable update button from now
                cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, getNoteElement, False)

                lstNoteType.Enabled = False
                lstNoteCategory.Enabled = False
            Else
                curUser.CheckAccessRole(AccessRoleType.Edit, getNoteElement(), False, True)

                ARec.Action = cFWAuditLog.AUDIT_UPDATE

                ' Read in record as it was prior to change
                FWDb.FWDb("R2", Session("NoteTable"), "NoteId", Session("CurNoteId"), "", "", "", "", "", "", "", "", "", "")

                ' Record action in the Audit Log
                ARec.DataElementDesc = Session("NoteTable")

                If FWDb.FWDbFindVal("Note", 2) <> txtNote.Text Then
                    ARec.ElementDesc = "Note Text"
                    ARec.PreVal = FWDb.FWDbFindVal("Note", 2)
                    ARec.PostVal = Left(txtNote.Text, 30)
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, Session("CurNoteId"))
                    FWDb.SetFieldValue("Note", txtNote.Text, "S", noChange)
                    noChange = False
                End If

                tmpStr = FWDb.FWDbFindVal("Date", 2)
                If tmpStr <> "" And IsDate(tmpStr) = True Then
                    If Format(CDate(tmpStr), cDef.DATE_FORMAT) <> txtCreatedDate.Text Then
                        ARec.ElementDesc = "Created Date"
                        ARec.PreVal = FWDb.FWDbFindVal("Date", 2)
                        ARec.PostVal = txtCreatedDate.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("CurNoteId"))
                        FWDb.SetFieldValue("Date", txtCreatedDate.Text, "D", noChange)
                        noChange = False
                    End If
                End If

                If Trim(FWDb.FWDbFindVal("noteType", 2)) <> Trim(lstNoteCategory.SelectedItem.Value) Then
                    ARec.ElementDesc = "Note Category"
                    Try
                        tmpStr = lstNoteCategory.Items.FindByValue(FWDb.FWDbFindVal("noteType", 2)).Text
                    Catch ex As Exception
                        tmpStr = "Unknown"
                    End Try

                    ARec.PreVal = tmpStr
                    ARec.PostVal = lstNoteCategory.SelectedItem.Text
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, Session("CurNoteId"))
                    FWDb.SetFieldValue("noteType", lstNoteCategory.SelectedItem.Value, "N", noChange)
                    noChange = False
                End If

                If FWDb.FWDbFindVal("noteCatId", 2) <> lstNoteType.SelectedItem.Value Then
                    ARec.ElementDesc = "Note Type"
                    ARec.PreVal = ""
                    ARec.PostVal = lstNoteType.SelectedItem.Text
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, Session("CurNoteId"))
                    FWDb.SetFieldValue("noteCatId", lstNoteType.SelectedItem.Value, "N", noChange)
                    noChange = False
                End If

                ' amending an existing one
                If noChange = False Then
                    FWDb.FWDb("A", Session("NoteTable"), "NoteId", Session("CurNoteId"), "", "", "", "", "", "", "", "", "", "")
                    lblErrorString.Text = "Note Updated Successfully"
                End If
            End If

            FWDb.DBClose()
            FWDb = Nothing
        End Sub

        Private Sub NewNote()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            Session("CurNoteId") = -1
            txtNote.Text = ""
            txtNote.ReadOnly = False
            txtCreatedDate.Value = Format(Now(), cDef.DATE_FORMAT)
            txtCreatedBy.Text = curUser.Employee.Forename & " " & curUser.Employee.Surname

            cmdUpdate.Visible = True
            cmdUpdate.Enabled = True
            lstNoteType.Enabled = True
            lstNoteCategory.Enabled = True
            cmdCancel.ImageUrl = "./buttons/cancel.gif"
        End Sub

        Protected Sub lstNoteList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim idx As Integer

            idx = lstNoteList.SelectedItem.Value
            Session("CurNoteId") = idx

            Dim activeNoteOwner As Integer
            Dim db As New cFWDBConnection
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            db.DBOpen(fws, False)

            activeNoteOwner = LoadNote(db, True)

            SetPermissions(db, fws, curUser, activeNoteOwner)

            db.DBClose()
            db = Nothing
        End Sub

        Private Sub AttachFileToNote()
            Response.Redirect("Attachments.aspx?attarea=" & Trim(hiddenAttArea.Text) & "&ref=" & Trim(Session("CurNoteId")), True)
        End Sub

        Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNew.Click
            'NewNote()
            Response.Redirect(Session("RefreshURL") & "&id=-1", True)
        End Sub

        Private Function LoadNote(ByVal db As cFWDBConnection, Optional ByVal getItemName As Boolean = True) As Integer
            Dim sql As String
            Dim tmpStr As String
            Dim drow As DataRow
            Dim AttQty As String
            Dim ActiveNoteOwner As Integer = -1
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim connStr As String = cAccounts.getConnectionString(curUser.Account.accountid)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

            Select Case Session("NoteType")
                Case "Product"
                    Session("NotesForID") = Request.QueryString("productid")
                    Session("NotesIDField") = "ProductId"
                    Session("NoteTable") = "productNotes"
                    hiddenAttArea.Text = AttachmentArea.PRODUCT_NOTES
                    db.FWDb("R2", "productDetails", Session("NotesIDField"), Session("NotesForID"), "", "", "", "", "", "", "", "", "", "")
                    If getItemName Then
                        If db.FWDb2Flag = True Then
                            Session("NotesForText") = db.FWDbFindVal("ProductName", 2)
                        End If
                    End If

                    Title = "Product Notes"

                Case "Contract"
                    cLocks.RemoveLockItem(curUser.AccountID, connStr, Cache, "CD_" & curUser.AccountID.ToString, ViewState("ActiveContract"), curUser.EmployeeID)
                    cLocks.RemoveLockItem(curUser.AccountID, connStr, Cache, "CA_" & curUser.AccountID.ToString, ViewState("ActiveContract"), curUser.EmployeeID)

                    Session("NotesForID") = Request.QueryString("contractid")
                    Session("NotesIDField") = "ContractId"
                    Session("NoteTable") = "contractNotes"
                    hiddenAttArea.Text = AttachmentArea.CONTRACT_NOTES
                    If getItemName Then
                        db.FWDb("R2", "contract_details", Session("NotesIDField"), Session("NotesForID"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag = True Then
                            Session("NotesForText") = db.FWDbFindVal("contractDescription", 2)
                        End If
                    End If

                    Title = "Contract Notes"

                Case "Invoice"
                    Session("NotesForID") = Request.QueryString("invoiceid")
                    Session("NotesIDField") = "invoiceId"
                    Session("NoteTable") = "invoiceNotes"
                    hiddenAttArea.Text = AttachmentArea.INVOICE_NOTES
                    If getItemName Then
                        db.FWDb("R2", "invoiceDetails", Session("NotesIDField"), Session("NotesForID"), "", "", "", "", "", "", "", "", "", "")
                        If db.FWDb2Flag = True Then
                            Session("NotesForText") = db.FWDbFindVal("invoiceNumber", 2)
                        End If
                    End If

                    Title = "Invoice Notes"

                Case "Supplier"
                    'cLocks.RemoveLockItem(FWDb, Cache, "VD", Session("ActiveContract"), uinfo)

                    Session("NotesForID") = Request.QueryString("supplierid")
                    Session("NotesIDField") = "supplierId"
                    Session("NoteTable") = "supplierNotes"
                    hiddenAttArea.Text = AttachmentArea.VENDOR_NOTES
                    Dim suppliers As New cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId)

                    If getItemName Then
                        Dim supplier As cSupplier = suppliers.getSupplierById(Session("NotesForID"))

                        If Not supplier Is Nothing Then
                            Session("NotesForText") = supplier.SupplierName
                        End If
                    End If

                    Title = params.SupplierPrimaryTitle & " Notes"

                Case "SupplierContact"
                    Session("NotesForID") = Request.QueryString("contactid")
                    Session("NotesIDField") = "contactId"
                    Session("NoteTable") = "supplierContactNotes"
                    hiddenAttArea.Text = AttachmentArea.CONTACT_NOTES
                    If getItemName Then
                        Dim contact As cSupplierContact = cSupplierContacts.getContactByContactId(curUser.AccountID, Session("NotesForID"))

                        If Not contact Is Nothing Then
                            Session("NotesForText") = contact.Name
                        End If
                    End If

                    Title = params.SupplierPrimaryTitle & " Contact Notes"

                Case Else
                    Exit Function
            End Select

            Dim noteElement As SpendManagementElement = getNoteElement()
            curUser.CheckAccessRole(AccessRoleType.View, noteElement, False, True)

            cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, noteElement, False)
            cmdUpdate.ToolTip = "Save current changes to the system"
            cmdUpdate.AlternateText = "Update"
            cmdUpdate.Attributes.Add("onmouseover", "window.status='Save current changes to the system';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")

            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, noteElement, False)
            lnkNew.ToolTip = "Insert as a new note"
            lnkNew.Attributes.Add("onmouseover", "window.status='Insert as a new note';return true;")
            lnkNew.Attributes.Add("onmouseout", "window.status='Done';")

            cmdCancel.ToolTip = "Exit from the notes screen"
            cmdCancel.AlternateText = "Close"
            cmdCancel.Attributes.Add("onmouseover", "window.status='Exit from the notes screen';return true;")
            cmdCancel.Attributes.Add("onmouseout", "window.status='Done';")

            tmpStr = "ViewNotes.aspx?notetype=" & Session("NoteType") & "&item=" & Replace(Session("NotesForText"), "&", "%26") & "&"
            Select Case Session("NoteType")
                Case "Supplier"
                    tmpStr = tmpStr & "supplierid="
                Case "Contract"
                    tmpStr = tmpStr & "contractid="
                Case "Invoice"
                    tmpStr = tmpStr & "invoiceid="
                Case "Product"
                    tmpStr = tmpStr & "productid="
                Case "SupplierContact"
                    tmpStr = tmpStr & "contactid="
                Case Else
            End Select
            tmpStr = tmpStr & Session("NotesForID")
            Session("refreshURL") = tmpStr

            Master.title = Title

            If Session("CurNoteId") > 0 Then
                lstNoteType.Enabled = False
                lstNoteCategory.Enabled = False

                cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, noteElement, False)
                txtNote.Enabled = curUser.CheckAccessRole(AccessRoleType.Edit, noteElement, False)
            Else
                ' set permissions on buttons etc.
                If curUser.CheckAccessRole(AccessRoleType.Add, noteElement, False) Then
                    cmdUpdate.Visible = True
                    lstNoteType.Enabled = True
                    lstNoteCategory.Enabled = True
                    txtNote.Enabled = True
                Else
                    cmdUpdate.Visible = False
                    txtNote.Enabled = False
                End If
            End If

            sql = "SELECT [fullDescription],[noteCatId] FROM [codes_notecategory] WHERE [noteType] = 0 ORDER BY [fullDescription]"
            db.RunSQL(sql, db.glDBWorkA, False, "", False)
            If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                lstNoteCategory.DataTextField = "fullDescription"
                lstNoteCategory.DataValueField = "noteCatId"
                lstNoteCategory.DataSource = db.glDBWorkA
                lstNoteCategory.DataBind()

                lstNoteCategory.Items.Insert(0, New ListItem("[ALL]", 0))

                If lstNoteCategory.SelectedItem.Value <> 0 Then
                    sql = "SELECT [fullDescription],[noteCatId] FROM [codes_notecategory] WHERE [noteType] = @noteCat ORDER BY [fullDescription]"
                    db.AddDBParam("noteCat", lstNoteCategory.SelectedItem.Value, True)
                    db.RunSQL(sql, db.glDBWorkA, False, "", False)
                    If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                        lstNoteType.DataTextField = "fullDescription"
                        lstNoteType.DataValueField = "noteCatId"
                        lstNoteType.DataSource = db.glDBWorkA
                        lstNoteType.DataBind()
                    End If
                Else
                    lstNoteType.Items.Add(New ListItem("[no definitions]", 0))
                End If

                If Session("CurNoteId") > 0 Then
                    ActiveNoteOwner = GetNote(db, Session("CurNoteId"))
                    sql = "SELECT COUNT(*) AS [AttQty] FROM [attachments] WHERE [attachmentArea] = @attArea AND [referenceNumber] = @refNum"
                    db.AddDBParam("attArea", hiddenAttArea.Text, True)
                    db.AddDBParam("refNum", Session("CurNoteId"), False)
                    db.RunSQL(sql, db.glDBWorkB, False, "", False)

                    AttQty = "&nbsp;(" & Trim(db.GetFieldValue(db.glDBWorkB, "AttQty", 0, 0)) & " Attached)"
                Else
                    AttQty = "&nbsp;(0 Attached)"
                    txtCreatedBy.Text = curUser.Employee.fullname
                    txtCreatedDate.Value = Format(Now(), cDef.DATE_FORMAT)
                End If

                If Session("CurNoteId") > 0 Then
                    Session("NoteAttachmentReturnURL") = "ViewNotes.aspx" & Request.Url.Query
                    litAttach.Text = "<a onmouseover=""window.status='Attach relative document to the active note';return true;"" onmouseout=""window.status='Done';"" alt=""Attach relative document to the active note"" style=""cursor:hand;"" href=""Attachments.aspx?attarea=" & hiddenAttArea.Text.Trim & "&ref=" & Trim(Session("CurNoteId")) & """><img src=""./icons/16/plain/paperclip.gif"" /></a>" & AttQty.Trim
                Else
                    litAttach.Text = "&nbsp;"
                End If

                sql = "SELECT [noteId],[Primary].[fullDescription] AS [PN],[Secondary].[fullDescription] AS [SN],[Date] FROM [" & Session("NoteTable") & "] "
                sql += "LEFT OUTER JOIN [codes_notecategory] AS [Primary] ON [Primary].[noteCatId] = [" & Session("NoteTable") & "].[noteType] "
                sql += "LEFT OUTER JOIN [codes_notecategory] AS [Secondary] ON [Secondary].[noteCatId] = [" & Session("NoteTable") & "].[noteCatId] "
                sql += "WHERE [" & Session("NotesIDField") & "] = @noteId"

                db.AddDBParam("noteId", Session("NotesForID"), True)

                If lstNoteCategory.SelectedItem.Value <> 0 Then
                    sql += " AND [Primary].[noteCatId] = @noteCatId "
                    db.AddDBParam("noteCatId", lstNoteCategory.SelectedItem.Value, False)
                End If

                sql += " ORDER BY [Date] DESC"

                db.RunSQL(sql, db.glDBWorkA, False, "", False)

                If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                    lstNoteList.Items.Clear()

                    For Each drow In db.glDBWorkA.Tables(0).Rows
                        If IsDBNull(drow.Item("Date")) = False Then
                            tmpStr = Format(drow.Item("Date"), cDef.DATE_FORMAT) & " " & Trim(drow.Item("PN")) & " : " & Trim(drow.Item("SN"))
                        Else
                            If IsDBNull(drow.Item("PN")) = True Then
                                tmpStr = "??/??/???? - Unknown type"
                            Else
                                tmpStr = "??/??/????" & " " & Trim(drow.Item("PN")) & " : " & Trim(drow.Item("SN"))
                            End If

                        End If
                        lstNoteList.Items.Add(New ListItem(tmpStr, drow.Item("NoteId")))
                    Next
                End If
            Else
                lstNoteCategory.Items.Add(New ListItem("[no definitions]", 0))
                lstNoteType.Items.Add(New ListItem("[no definitions]", 0))
            End If

            lblSubTitle.Text = Session("NotesForText")
            lblCreatedBy.Text = "Created by"

            Return ActiveNoteOwner
        End Function

        Private Sub SetPermissions(ByVal db As cFWDBConnection, ByVal fws As cFWSettings, ByVal curUser As CurrentUser, ByVal ActiveNoteOwner As Integer)
            Dim isArchived As Boolean = False
            Dim hideDelete As Boolean = False

            If Session("NoteType") = "Contract" Then
                ' check that the active contract is not archived. If it is, don't permit update
                db.FWDb("R2", "contract_details", "contractId", ViewState("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDb2Flag = True Then
                    If db.FWDbFindVal("Archived", 2) = "Y" Then
                        isArchived = True
                    End If
                End If
            End If

            If Session("CurNoteId") <= 0 Then
                hideDelete = True
            End If

            If (fws.glAllowNotesAdd = False And isArchived = True) Or curUser.CheckAccessRole(AccessRoleType.Delete, getNoteElement, False) = False Then
                hideDelete = True
            End If

            If (fws.glAllowNotesAdd = False And isArchived = True) Or curUser.CheckAccessRole(AccessRoleType.Edit, getNoteElement, False) = False And curUser.CheckAccessRole(AccessRoleType.Add, getNoteElement, False) = False Then
                cmdUpdate.Visible = False
                cmdCancel.ImageUrl = "~/buttons/page_close.png"
                Master.useCloseNavigationMsg = True
            End If

            If (fws.glAllowNotesAdd = False And isArchived = True) Or curUser.CheckAccessRole(AccessRoleType.Add, getNoteElement, False) = False Then
                lnkNew.Visible = False
            End If

            'If Me.IsPostBack = False Then
            If Session("NoteType") = "Contract" Then
                ' if current contract is archived, do not permit update or delete
                db.FWDb("R", "contract_details", "contractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If db.FWDbFlag = True Then
                    If db.FWDbFindVal("Archived", 1) = "Y" And fws.glAllowNotesAdd = False Then
                        hideDelete = True
                        lnkNew.Visible = False
                        cmdUpdate.Visible = False
                    End If
                End If
            End If

            litDelete.Text = ""

            If Session("CurNoteId") > 0 Then
                If ActiveNoteOwner = 0 Or ActiveNoteOwner <> curUser.Employee.employeeid Then
                    ' not the note creator, so don't permit deletion, modification etc.
                    txtNote.ReadOnly = True
                    cmdUpdate.Visible = False
                    cmdUpdate.Visible = False
                    cmdCancel.ImageUrl = "~/buttons/page_close.png"
                    Master.useCloseNavigationMsg = True
                    lstNoteType.Enabled = False
                    lstNoteCategory.Enabled = False
                Else
                    If Not hideDelete Then
                        Dim delStr As New System.Text.StringBuilder
                        With delStr
                            .Append("<a href=""javascript:if(confirm('Click OK to confirm deletion of the active note')){window.location.href='ViewNotes.aspx?action=delete&id=" & Session("CurNoteId") & "'};"" ")
                            .Append("onmouseover=""window.status='Remove the current note from the system';return true;"" ")
                            .Append("onmouseout=""window.status='Done';"" ")
                            .Append(" class=""submenuitem"" title=""Remove the current note from the system"">Delete</a>")
                        End With
                        litDelete.Text = delStr.ToString
                    End If
                End If
            End If

            Master.RefreshBreadcrumbInfo()
            'End If
        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            UpdateNote()
        End Sub

        Protected Sub cmdReset_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Session("CurNoteId") = -1

            txtNote.ReadOnly = False
            txtNote.Text = ""
            lstNoteType.Enabled = True
            lstNoteCategory.Enabled = True
            litAttach.Text = ""
            litDelete.Text = ""
            txtCreatedDate.Text = DateTime.Today

            Dim curUser As CurrentUser = cMisc.GetCurrentUser()

            cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Add, getNoteElement(), True)
            txtNote.Enabled = curUser.CheckAccessRole(AccessRoleType.Add, getNoteElement(), True)

        End Sub

		Private Function getNoteElement() As SpendManagementElement
            Select Case Session("NoteTable")
                Case "contractNotes"
                    Return SpendManagementElement.ContractNotes
                Case "productNotes"
                    Return SpendManagementElement.ProductNotes
                Case "invoiceNotes"
                    Return SpendManagementElement.InvoiceNotes
                Case "supplierContactNotes"
                    Return SpendManagementElement.SupplierContactNotes
                Case "supplierNotes"
                    Return SpendManagementElement.SupplierNotes
                Case Else
                    Return SpendManagementElement.ContractNotes
            End Select
		End Function
    End Class
End Namespace
