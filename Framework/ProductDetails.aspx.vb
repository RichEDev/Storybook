Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Collections.Generic

Namespace Framework2006
    Partial Class ProductDetails
        Inherits System.Web.UI.Page
        Private ARec As cAuditRecord
        Private Sql As String

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
			Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
			Dim tmpstr As String
			Dim action As String
            Dim FWDb As New cFWDBConnection
            Dim ufields As New cUserdefinedFields(curUser.AccountID)

            Dim clsBaseDefs As New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories)
            Dim tables As New cTables(curUser.AccountID)
            Dim ptable As cTable = tables.getTableByName("productDetails")

            ufields.createFieldPanel(phPUFields, ptable.UserdefinedTable, "productUFields", New StringBuilder())

            If Me.IsPostBack = False Then
                curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, False, True)

                FWDb.DBOpen(fws, False)

                Title = "Product/Service Details"
                Master.title = Title

                If Request.QueryString("id") <> 0 Then
                    Session("ProductId") = Request.QueryString("id")
                Else
                    Session("ProductId") = 0
                End If

                Session("ProductText") = Request.QueryString("item")

                'Screen Labels
                SetHeadings(FWDb)

                action = Request.QueryString("action")
                If action <> "" Then
                    Select Case action
                        Case "delete"
                            curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False, True)

                            lblErrorString.Text = DeleteProduct(FWDb, Request.QueryString("id"))

                        Case "add"
                            curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Products, False, True)

                            lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                            PD_SearchPanel.Visible = False
                            PD_EditFieldsPanel.Visible = True
                            Master.enablenavigation = False
                            lnkLicences.Visible = False

                            BlankProduct()

                        Case "notes"
                            GetNotes()

                        Case "edit"
                            If Session("ProductId") <> 0 Then
                                curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, False, True)

                                Master.enablenavigation = False
                                lnkDelete.Visible = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False)
                                lnkNotes.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductNotes, False)
                                lnkLicences.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicences, False)

                                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, False) Then
                                    Master.useCloseNavigationMsg = False
                                Else
                                    Master.useCloseNavigationMsg = True
                                End If
                                ShowProduct(FWDb, Session("ProductId"))
                            End If

                        Case Else
                    End Select
                End If

                tmpstr = "ERROR!"
                reqProductName.ErrorMessage = "Required definition not specified"

                ' populate the category filter
                lstCategoryFilter.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))

                lstCategoryFilter.Items.Insert(0, New ListItem("All", 0))
                lstCategoryFilter.Items.Insert(0, New ListItem("Select Product..", -1))

                If Not Session("ProductFilter") Is Nothing Then
                    txtFilter.Text = Session("ProductFilter")
                End If

                FWDb.DBClose()
                FWDb = Nothing
            End If

            lnkNew.ToolTip = "Insert a new product definition into the system"
            lnkNew.Attributes.Add("onmouseover", "window.status='Insert a new product definition into the system';return true;")
            lnkNew.Attributes.Add("onmouseout", "window.status='Done';")
            lnkNew.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Products, False)

            lnkDelete.ToolTip = "Delete the current product definition"
            lnkDelete.Attributes.Add("onmouseover", "window.status='Delete the current product definition';return true;")
            lnkDelete.Attributes.Add("onmouseout", "window.status='Done';")
            lnkDelete.Visible = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False)

            cmdUpdate.AlternateText = "Update"
            cmdUpdate.ToolTip = "Commit changes to the system"
            cmdUpdate.Attributes.Add("onmouseover", "window.status='Commit changes to the system';return true;")
            cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")


            lnkNotes.ToolTip = "Review any notes associated with the current product"
            lnkNotes.Attributes.Add("onmouseover", "window.status='Review any notes associated with the current product';return true;")
            lnkNotes.Attributes.Add("onmouseout", "window.status='Done';")
            lnkNotes.Visible = curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductNotes, False)

            If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False) And Session("ProductId") <> 0 Then
                lnkDelete.Attributes.Add("onclick", "javascript:if(confirm('Are you sure? OK to confirm deletion.')){window.location.href='ProductDetails.aspx?action=delete&id=" & Trim(Session("ProductId")) & "';}")
            End If

            If Session("ProductId") = 0 Then
                lnkNotes.Visible = False
                lnkDelete.Visible = False
                lnkLicences.Visible = False
                lnkTaskSummary.Visible = False
                lnkAddTask.Visible = False
                cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Products, False)
            Else
                'lnkLicences.Visible = True

                cmdUpdate.Visible = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, False)
                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, False) = False Then
                    cmdCancel.ImageUrl = "~/buttons/page_close.gif"
                End If

                lnkTaskSummary.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)

                If curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False) Then
                    lnkAddTask.ToolTip = "Create a task associated with the current product"
                    lnkAddTask.Attributes.Add("onmouseover", "window.status='Create a task associated with the current product';return true;")
                    lnkAddTask.Attributes.Add("onmouseout", "window.status='Done';")
                    lnkAddTask.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, False)
                End If
            End If

            cmdUpdate.Attributes.Add("onclick", "if(validateform() == false) { return false; }")
		End Sub

		Private Sub SetHeadings(ByVal db As cFWDBConnection)
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim tmpLstItem As New System.Web.UI.WebControls.ListItem
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

			' Create a [None] entry for lists for db entries where nothing selected
			tmpLstItem.Text = "[None]"
			tmpLstItem.Value = "0"
			tmpLstItem.Selected = True

			' Field definitions
			lblProductCode.Text = "Product Code"
            lblProductName.Text = "Product Name (*)"
			lblProductDescription.Text = "Product Description"
			lblProductCategory.Text = "Product Category"
			lblInstalledVersion.Text = "Installed Version Number"
			lblAvailableVersion.Text = "Available Version Number"
			lblDateInstalled.Text = "Date Installed"
			lblNoLicensedCopies.Text = "No. Licensed Copies"
			lblUserCode.Text = "User Code"
            Dim supplierStr As String = params.SupplierPrimaryTitle

            lnkProductVendorAssoc.Text = "Product - " & supplierStr & " Association"

			If Session("ProductId") <> 0 Then
                Sql = "SELECT COUNT(*) AS [NumNotes] FROM [productnotes] WHERE [ProductId] = " & Session("ProductId")
				db.RunSQL(Sql, db.glDBWorkB, False, "", False)
				Dim numNotes As Integer
				numNotes = db.GetFieldValue(db.glDBWorkB, "NumNotes", 0, 0)
				lblNotes.Text = numNotes.ToString & " Notes"
				If numNotes > 0 Then
					litNotes.Text = "<a onmouseout=""window.status='Done';"" onmouseover=""window.status='Review any notes associated with the current product';return true;"" href=""ProductDetails.aspx?action=notes&id=" & Session("ProductId") & """><img src=""./images/attachment-flashing.gif"" /></a>"
				Else
					litNotes.Text = ""
				End If
			Else
				lblNotes.Text = "0 Notes"
				litNotes.Text = ""
			End If

		End Sub

		Private Sub ShowProduct(ByVal FWDb As cFWDBConnection, ByVal ProductID As Integer)
			' Display product information for the ID passed
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
			Dim tmpStr As String
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim ufields As New cUserdefinedFields(fws.MetabaseCustomerId)
            Dim tables As New cTables(fws.MetabaseCustomerId)
            Dim fields As New cFields(fws.MetabaseCustomerId)
            Dim ptable As cTable = tables.getTableByName("productDetails")
            Dim produfields As List(Of cUserDefinedField) = ufields.getFieldsByTable(tables.getTableByName("productDetails"))
            Dim clsBaseDefs As New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories)

			PD_SearchPanel.Visible = False
			PD_EditFieldsPanel.Visible = True

            FWDb.FWDb("R", "productDetails", "ProductId", ProductID, "", "", "", "", "", "", "", "", "", "")

			If FWDb.FWDbFlag = True Then
                lblTitle.Text = " - " & FWDb.FWDbFindVal("ProductName", 1)
                txtProductName.Text = FWDb.FWDbFindVal("ProductName", 1)
                Session("ProductText") = FWDb.FWDbFindVal("ProductName", 1)
                txtProductCode.Text = FWDb.FWDbFindVal("ProductCode", 1)
                txtProductDescription.Text = FWDb.FWDbFindVal("Description", 1)
                txtInstalledVersion.Text = FWDb.FWDbFindVal("InstalledVersionNumber", 1)
                txtAvailableVersion.Text = FWDb.FWDbFindVal("AvailableVersionNumber", 1)

                Dim prodCatID As Integer = 0

                If Integer.TryParse(FWDb.FWDbFindVal("productCategoryId", 1).ToString(), prodCatID) Then
                    lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, prodCatID))
                    lstProductCategory.ClearSelection()
                    lstProductCategory.Items.FindByValue(SMRoutines.CheckListIndex(FWDb.FWDbFindVal("productCategoryId", 1))).Selected = True
                Else
                    lstProductCategory.Items.AddRange(clsBaseDefs.CreateDropDown(True, 0))
                End If
                tmpStr = FWDb.FWDbFindVal("DateInstalled", 1)
                If tmpStr = "" Then
                    txtDateInstalled.Text = ""
                Else
                    txtDateInstalled.Text = Format(CDate(tmpStr), cDef.DATE_FORMAT)
                End If
                txtNumLicensedCopies.Text = FWDb.FWDbFindVal("NumLicencedCopies", 1)
                txtUserCode.Text = FWDb.FWDbFindVal("UserCode", 1)
                tmpStr = FWDb.FWDbFindVal("LicenceExpires", 1)

                Dim udfs As SortedList(Of Integer, Object)
                If ProductID > 0 Then
                    udfs = ufields.GetRecord(ptable.UserdefinedTable, ProductID, tables, fields) ' cUFInterim.GetUFRecordValues(curUser.Account.accountid, ProductID, ufields.getFieldsByTable(ptable))
                    ufields.populateRecordDetails(phPUFields, ptable.UserdefinedTable, udfs)
                Else
                    udfs = New SortedList(Of Integer, Object)
                End If
                ViewState("record") = udfs

                'For Each uf As KeyValuePair(Of Integer, cUserField) In produfields
                '	Dim ufield As cUserField = CType(uf.Value, cUserField)

                '	Select Case ufield.FieldType
                '		Case UserFieldType.Character, UserFieldType.DateField, UserFieldType.Float, UserFieldType.Number, UserFieldType.Text
                '			Dim txt As TextBox = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), TextBox)
                '			If Not txt Is Nothing Then
                '				txt.Text = FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)
                '			End If
                '			Dim hiddenURLText As HiddenField = CType(panelUFields.FindControl(ufield.FieldId.ToString & "_URLTEXT"), HiddenField)
                '			If Not hiddenURLText Is Nothing Then
                '				hiddenURLText.Value = txt.Text
                '			End If
                '		Case UserFieldType.Checkbox
                '			Dim chk As CheckBox = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), CheckBox)
                '			If Not chk Is Nothing Then
                '				If FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) = "1" Then
                '					chk.Checked = True
                '				End If
                '			End If
                '		Case UserFieldType.DDList
                '			Dim lst As DropDownList = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), DropDownList)
                '			If Not lst Is Nothing Then
                '				If FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "" And FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "0" Then
                '					lst.Items.FindByText(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)).Selected = True
                '				End If
                '			End If
                '		Case UserFieldType.RechargeAcc_Code
                '			Dim raccs As New cRechargeAccountCodes(userinfo, fws)
                '			If raccs.Count > cDef.UF_MAXCOUNT Then
                '				Dim hId As HiddenField = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), HiddenField)
                '				Dim txt As TextBox = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString & "_TXT"), TextBox)

                '				If (Not txt Is Nothing) And (Not hId Is Nothing) Then
                '					Dim racc As cRechargeAccountCode = raccs.GetCodeById(CInt(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)))

                '					If racc Is Nothing Then
                '						txt.Text = ""
                '						hId.Value = "0"
                '					Else
                '						txt.Text = racc.AccountCode
                '						hId.Value = racc.CodeId.ToString
                '					End If
                '				End If
                '			Else
                '				Dim lst As DropDownList = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), DropDownList)
                '				If Not lst Is Nothing Then
                '					If FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "" And FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "0" Then
                '						lst.Items.FindByValue(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)).Selected = True
                '					End If
                '				End If
                '			End If

                '		Case UserFieldType.Site_Ref
                '			Dim sites As New cSites(fws, userinfo)
                '			If sites.Count > cDef.UF_MAXCOUNT Then
                '				Dim hId As HiddenField = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), HiddenField)
                '				Dim txt As TextBox = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString & "_TXT"), TextBox)

                '				If (Not txt Is Nothing) And (Not hId Is Nothing) Then
                '					Dim site As cSite = sites.GetSiteById(CInt(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)))

                '					If site Is Nothing Then
                '						txt.Text = ""
                '						hId.Value = "0"
                '					Else
                '						txt.Text = site.SiteCode & " : " & site.SiteDescription
                '						hId.Value = site.SiteId.ToString
                '					End If
                '				End If
                '			Else
                '				Dim lst As DropDownList = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), DropDownList)
                '				If Not lst Is Nothing Then
                '					If FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "" And FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "0" Then
                '						lst.Items.FindByValue(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)).Selected = True
                '					End If
                '				End If
                '			End If
                '		Case UserFieldType.StaffName_Ref
                '			Dim emps As New cFWEmployees(fws, userinfo)
                '			If emps.Count > cDef.UF_MAXCOUNT Then
                '				Dim hId As HiddenField = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), HiddenField)
                '				Dim txt As TextBox = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString & "_TXT"), TextBox)

                '				If (Not txt Is Nothing) And (Not hId Is Nothing) Then
                '					Dim emp As cFWEmployee = emps.GetEmployeeById(CInt(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)))

                '					If emp Is Nothing Then
                '						txt.Text = ""
                '						hId.Value = "0"
                '					Else
                '						txt.Text = emp.EmployeeName
                '						hId.Value = emp.EmployeeId.ToString
                '					End If
                '				End If
                '			Else
                '				Dim lst As DropDownList = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), DropDownList)
                '				If Not lst Is Nothing Then
                '					If FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "" And FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "0" Then
                '						lst.Items.FindByValue(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)).Selected = True
                '					End If
                '				End If
                '			End If
                '		Case UserFieldType.RechargeClient_Ref
                '			Dim rcs As New cRechargeClientList(userinfo, fws)
                '			If rcs.Count > cDef.UF_MAXCOUNT Then
                '				Dim hId As HiddenField = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), HiddenField)
                '				Dim txt As TextBox = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString & "_TXT"), TextBox)

                '				If (Not txt Is Nothing) And (Not hId Is Nothing) Then
                '					Dim rc As cRechargeClient = rcs.GetClientById(CInt(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)))

                '					If rc Is Nothing Then
                '						txt.Text = ""
                '						hId.Value = "0"
                '					Else
                '						txt.Text = rc.ClientName
                '						hId.Value = rc.EntityId.ToString
                '					End If
                '				End If
                '			Else
                '				Dim lst As DropDownList = CType(panelUFields.FindControl("UF" & ufield.FieldId.ToString), DropDownList)
                '				If Not lst Is Nothing Then
                '					If FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "" And FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1) <> "0" Then
                '						lst.Items.FindByValue(FWDb.FWDbFindVal("UF" & ufield.FieldId.ToString, 1)).Selected = True
                '					End If
                '				End If
                '			End If
                '	End Select
                'Next

                Dim products As New cProducts(curUser.Account.accountid, curUser.CurrentSubAccountId)
                Dim licences As New cProductLicences(curUser.Account.accountid, curUser.CurrentSubAccountId, curUser.Employee.employeeid, ProductID, cAccounts.getConnectionString(curUser.AccountID))

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicences, False) Then
                    litLicences.Text = "<a href=""ProductLicences.aspx?pid=" & ProductID.ToString & """ onmouseover=""window.status='View / Modify licences for this product';return true;"" onmouseout=""window.status='Done';"">Licences</a>"
                Else
                    litLicences.Text = "Licences"
                End If
                txtNumLicences.Text = licences.Count.ToString & " Licences"
            Else
                lblErrorString.Text = "ERROR! No data found in database"
                litLicences.Text = "Licences"
            End If

            ' populate the product - vendor association list
            If Session("ProductId") <> 0 Then
                Sql = "SELECT [product_suppliers].[supplierId], [supplier_details].[suppliername] FROM [product_suppliers]" & vbNewLine & _
    "INNER JOIN [supplier_details] ON [product_suppliers].[supplierId] = [supplier_details].[supplierid]" & vbNewLine & _
    "WHERE [product_suppliers].[productId] = @productId ORDER BY [supplier_details].[suppliername]"
                FWDb.AddDBParam("productId", Session("ProductId"), True)
                FWDb.RunSQL(Sql, FWDb.glDBWorkB, False, "", False)
                lstProductVendorAssoc.DataSource = FWDb.glDBWorkB
                lstProductVendorAssoc.DataTextField = "suppliername"
                lstProductVendorAssoc.DataValueField = "supplierid"
                lstProductVendorAssoc.DataBind()

                ' enable the amendment of associations
                lnkProductVendorAssoc.Enabled = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, False)
            Else
                lstProductVendorAssoc.Items.Clear()
            End If

            'cmdUpdate.Visible = True
        End Sub

        Private Sub UpdateProduct()
            Dim FWDb As New cFWDBConnection
            Dim ARec As New cAuditRecord
            Dim nochange As Boolean = True
            Dim tmpStr As String = ""
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim inputVal As String = ""

            nochange = True
            FWDb.DBOpen(fws, False)

            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Products, curUser.CurrentSubAccountId)

            If Session("ProductId") > 0 Then
                ' must be updating an existing product, get a copy of the record prior to update
                FWDb.FWDb("R2", "productDetails", "ProductId", Session("ProductId"), "", "", "", "", "", "", "", "", "", "")
                If FWDb.FWDb2Flag = True Then
                    ' found existing record
                    ARec.Action = cFWAuditLog.AUDIT_UPDATE
                    ARec.ElementDesc = "PRODUCT DETAIL"

                    If txtProductCode.Text <> FWDb.FWDbFindVal("ProductCode", 2) Then
                        ' product code changed
                        FWDb.SetFieldValue("ProductCode", txtProductCode.Text, "S", nochange)
                        nochange = False

                        ARec.DataElementDesc = "ProductCode"
                        ARec.PreVal = FWDb.FWDbFindVal("ProductCode", 2)
                        ARec.PostVal = txtProductCode.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    tmpStr = Replace(txtProductName.Text, "'", "`")
                    If tmpStr <> FWDb.FWDbFindVal("ProductName", 2) Then
                        ' product name changed
                        FWDb.SetFieldValue("ProductName", tmpStr, "S", nochange)
                        nochange = False
                        ARec.DataElementDesc = "Product Name"
                        ARec.PreVal = FWDb.FWDbFindVal("ProductName", 2)
                        ARec.PostVal = tmpStr
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    tmpStr = Replace(txtProductDescription.Text, "'", "`")
                    If tmpStr <> FWDb.FWDbFindVal("Product Description", 2) Then
                        ' product description changed
                        FWDb.SetFieldValue("Description", tmpStr, "S", nochange)
                        nochange = False
                        ARec.DataElementDesc = "Product Desc"
                        ARec.PreVal = FWDb.FWDbFindVal("Description", 2)
                        ARec.PostVal = tmpStr
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    If txtUserCode.Text <> FWDb.FWDbFindVal("UserCode", 2) Then
                        ' User code has changed
                        FWDb.SetFieldValue("UserCode", txtUserCode.Text, "S", nochange)
                        nochange = False
                        ARec.DataElementDesc = "User Code"
                        ARec.PreVal = FWDb.FWDbFindVal("UserCode", 2)
                        ARec.PostVal = txtUserCode.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    If txtInstalledVersion.Text <> FWDb.FWDbFindVal("InstalledVersionNumber", 2) Then
                        ' Installed version modified
                        FWDb.SetFieldValue("InstalledVersionNumber", txtInstalledVersion.Text, "S", nochange)
                        nochange = False
                        ARec.DataElementDesc = "Installed Version"
                        ARec.PreVal = FWDb.FWDbFindVal("InstalledVersionNumber", 2)
                        ARec.PostVal = txtInstalledVersion.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    If Trim(txtDateInstalled.Text) <> FWDb.FWDbFindVal("DateInstalled", 2) Then
                        ' date installed changed
                        If txtDateInstalled.Text.Trim = "" Then
                            FWDb.SetFieldValue("DateInstalled", DBNull.Value, "#", nochange)
                            nochange = False
                            ARec.DataElementDesc = "Date Installed"
                            tmpStr = FWDb.FWDbFindVal("DateInstalled", 2)
                            If tmpStr <> "" Then
                                tmpStr = Format(CDate(tmpStr), cDef.DATE_FORMAT)
                            End If
                            ARec.PreVal = tmpStr
                            ARec.PostVal = ""
                            ALog.AddAuditRec(ARec, True)
                            ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                        Else
                            If IsDate(txtDateInstalled.Text) Then
                                If CDate(txtDateInstalled.Text).Year >= cDef.SQL_MIN_YEAR Then
                                    FWDb.SetFieldValue("DateInstalled", txtDateInstalled.Text, "D", nochange)
                                    nochange = False
                                    ARec.DataElementDesc = "Date Installed"
                                    tmpStr = FWDb.FWDbFindVal("DateInstalled", 2)
                                    If tmpStr <> "" Then
                                        tmpStr = Format(CDate(tmpStr), cDef.DATE_FORMAT)
                                    End If
                                    ARec.PreVal = tmpStr
                                    ARec.PostVal = txtDateInstalled.Text
                                    ALog.AddAuditRec(ARec, True)
                                    ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                                End If
                            End If
                        End If
                    End If

                    If txtAvailableVersion.Text <> FWDb.FWDbFindVal("AvailableVersionNumber", 2) Then
                        ' available version changed
                        FWDb.SetFieldValue("AvailableVersionNumber", txtAvailableVersion.Text, "S", nochange)
                        nochange = False
                        ARec.DataElementDesc = "Available Version"
                        ARec.PreVal = FWDb.FWDbFindVal("AvailableVersionNumber", 2)
                        ARec.PostVal = txtAvailableVersion.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    If txtNumLicensedCopies.Text <> FWDb.FWDbFindVal("NumLicencedCopies", 2) Then
                        ' number of licensed copies changed
                        FWDb.SetFieldValue("NumLicencedCopies", txtNumLicensedCopies.Text, "N", nochange)
                        nochange = False
                        ARec.DataElementDesc = "Num Licenced Copies"
                        ARec.PreVal = FWDb.FWDbFindVal("NumLicencedCopies", 2)
                        ARec.PostVal = txtNumLicensedCopies.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    ' check the selections of the product category
                    If lstProductCategory.SelectedItem.Value <> Val(FWDb.FWDbFindVal("ProductCategoryId", 2)) Then
                        If lstProductCategory.SelectedItem.Value <> "0" Then
                            FWDb.SetFieldValue("ProductCategoryId", lstProductCategory.SelectedItem.Value, "N", nochange)
                        Else
                            FWDb.SetFieldValue("ProductCategoryId", DBNull.Value, "#", nochange)
                        End If

                        nochange = False

                        ARec.DataElementDesc = "Product Category"
                        Dim clsBaseDefs As New cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductCategories)

                        If FWDb.FWDbFindVal("ProductCategoryId", 2) <> "0" Then
                            ARec.PreVal = clsBaseDefs.GetDefinitionByID(Integer.Parse(FWDb.FWDbFindVal("ProductCategoryId", 2))).Description
                        Else
                            ARec.PreVal = ""
                        End If
                        ARec.PostVal = lstProductCategory.SelectedItem.Text
                        ALog.AddAuditRec(ARec, True)
                        ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    End If

                    If nochange = False Then
                        FWDb.SetFieldValue("modifiedon", DateTime.Now, "D", False)
                        FWDb.SetFieldValue("modifiedby", curUser.Employee.employeeid, "N", False)
                        FWDb.FWDb("A", "productDetails", "ProductId", Session("ProductId"), "", "", "", "", "", "", "", "", "", "")
                    End If

                    ' do user field update
                    ' update the user defined fields

                    '    For Each uf As KeyValuePair(Of Integer, Object) In newrec
                    '        Dim ufield As cUserDefinedField = ufields.getUserDefinedById(uf.Key)
                    '        Dim change = True
                    '        Dim oldVal As String
                    '        If oldrec.ContainsKey(uf.Key) Then
                    '            oldVal = CStr(oldrec(uf.Key))
                    '        Else
                    '            oldVal = ""
                    '        End If

                    '        Select Case ufield.fieldtype
                    '            Case FieldType.AutoCompleteTextbox, FieldType.Hyperlink, FieldType.LargeText, FieldType.Text
                    '                FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "S", nochange)

                    '                ARec.PreVal = oldVal
                    '                ARec.PostVal = newrec(uf.Key)
                    '                ALog.AddAuditRec(ARec, True)
                    '                ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    '                nochange = False

                    '            Case FieldType.Currency, FieldType.Integer, FieldType.List, FieldType.Number, FieldType.Relationship, FieldType.RelationshipTextbox
                    '                FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "N", nochange)
                    '                nochange = False

                    '                ARec.PreVal = oldVal
                    '                ARec.PostVal = newrec(uf.Key)
                    '                ALog.AddAuditRec(ARec, True)
                    '                ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))

                    '            Case FieldType.DateTime
                    '                FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "D", nochange)
                    '                nochange = False

                    '                ARec.PreVal = oldVal
                    '                ARec.PostVal = newrec(uf.Key)
                    '                ALog.AddAuditRec(ARec, True)
                    '                ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))

                    '            Case FieldType.TickBox
                    '                FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "X", nochange)
                    '                nochange = False

                    '                ARec.PreVal = oldVal
                    '                ARec.PostVal = newrec(uf.Key)
                    '                ALog.AddAuditRec(ARec, True)
                    '                ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))
                    '            Case Else

                    '        End Select
                    '    Next

                    '    If nochange = False Then
                    '        FWDb.FWDb("R2", "userdefinedProductDetails", "productid", Session("ProductId"), "", "", "", "", "", "", "", "", "", "")
                    '        If FWDb.FWDb2Flag Then
                    '            FWDb.FWDb("A", "userdefinedProductDetails", "productid", Session("ProductId"), "", "", "", "", "", "", "", "", "", "")
                    '        Else
                    '            FWDb.SetFieldValue("productid", Session("ProductId"), "N", False)
                    '            FWDb.FWDb("WX", "userdefinedProductDetails", "", "", "", "", "", "", "", "", "", "", "", "")
                    '        End If
                    '    End If

                End If
            ElseIf Session("ProductId") < 1 Then
                ' must be inserting a new product
                ARec.Action = cFWAuditLog.AUDIT_ADD
                ARec.ElementDesc = "PRODUCT DETAIL"
                ARec.DataElementDesc = "New Product"
                ARec.PreVal = ""
                ARec.PostVal = Replace(txtProductName.Text, "'", "`")

                FWDb.SetFieldValue("subAccountId", curUser.CurrentSubAccountId, "N", True)
                FWDb.SetFieldValue("ProductCode", txtProductCode.Text, "S", False)
                FWDb.SetFieldValue("ProductName", Replace(txtProductName.Text, "'", "`"), "S", False)
                FWDb.SetFieldValue("Description", Replace(txtProductDescription.Text, "'", "`"), "S", False)
                If lstProductCategory.SelectedItem.Value <> "0" Then
                    FWDb.SetFieldValue("ProductCategoryId", lstProductCategory.SelectedItem.Value, "N", False)
                End If
                FWDb.SetFieldValue("UserCode", txtUserCode.Text, "S", False)
                FWDb.SetFieldValue("InstalledVersionNumber", txtInstalledVersion.Text, "S", False)
                tmpStr = txtDateInstalled.Text
                If IsDate(tmpStr) = True Then
                    If CDate(tmpStr).Year >= cDef.SQL_MIN_YEAR Then
                        FWDb.SetFieldValue("DateInstalled", tmpStr, "D", False)
                    End If
                End If
                FWDb.SetFieldValue("AvailableVersionNumber", txtAvailableVersion.Text, "S", False)
                FWDb.SetFieldValue("NumLicencedCopies", txtNumLicensedCopies.Text, "N", False)
                FWDb.SetFieldValue("Archived", False, "B", False)
                FWDb.SetFieldValue("createdon", DateTime.Now, "D", False)
                FWDb.SetFieldValue("createdby", curUser.Employee.employeeid, "N", False)
                FWDb.FWDb("W", "productDetails", "", "", "", "", "", "", "", "", "", "", "", "")
                Session("ProductId") = FWDb.glIdentity

                ' Update any user defined fields
                'Dim ufields As New cUserdefinedFields(fws.MetabaseCustomerId)
                'Dim tables As New cTables(fws.MetabaseCustomerId)
                'Dim pdTable As cTable = tables.getTableByName("productDetails")
                'Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phPUFields, tables.getTableById(pdTable.userdefinedtable))
                'nochange = True

                'For Each uf As KeyValuePair(Of Integer, Object) In newrec
                '    Dim ufield As cUserDefinedField = ufields.getUserDefinedById(uf.Key)

                '    Select Case ufield.fieldtype
                '        Case FieldType.AutoCompleteTextbox, FieldType.Hyperlink, FieldType.LargeText, FieldType.Text
                '            FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "S", nochange)
                '            nochange = False

                '        Case FieldType.Currency, FieldType.Integer, FieldType.List, FieldType.Number, FieldType.Relationship, FieldType.RelationshipTextbox
                '            FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "N", nochange)
                '            nochange = False

                '        Case FieldType.DateTime
                '            FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "D", nochange)
                '            nochange = False

                '        Case FieldType.TickBox
                '            FWDb.SetFieldValue("udf" & ufield.userdefineid.ToString, newrec(uf.Key), "X", nochange)
                '            nochange = False

                '        Case Else

                '    End Select
                'Next

                'If nochange = False Then
                '    FWDb.SetFieldValue("productid", Session("ProductId"), "N", False)
                '    FWDb.FWDb("WX", "userdefinedProductDetails", "", "", "", "", "", "", "", "", "", "", "", "")
                'End If

                ALog.AddAuditRec(ARec, True)
                ALog.CommitAuditLog(curUser.Employee, Session("ProductId"))

                ' Update the session variable with new entry
                If FWDb.glIdentity > 0 Then
                    Session("ProductId") = FWDb.glIdentity
                    Session("ProductText") = txtProductName.Text
                Else
                    Session("ProductId") = 0
                    Session("ProductText") = ""
                End If
            End If

            Dim ufields As New cUserdefinedFields(fws.MetabaseCustomerId)
            Dim tables As New cTables(fws.MetabaseCustomerId)
            Dim fields As New cFields(fws.MetabaseCustomerId)
            Dim pdTable As cTable = tables.getTableByName("productDetails")
            Dim newrec As SortedList(Of Integer, Object) = ufields.getItemsFromPanel(phPUFields, pdTable.UserdefinedTable)

            ufields.SaveValues(pdTable.UserdefinedTable, Session("ProductId"), newrec, tables, fields, curUser)

            FWDb.DBClose()
            FWDb = Nothing

            tmpStr = "ProductDetails.aspx" '?action=edit&id=" & Trim(Session("ProductId")) & "&item=" & Trim(Session("ProductText"))
            Response.Redirect(tmpStr, True)
        End Sub

        Private Sub GetNotes()
            If Session("ProductId") <> 0 Then
                Session("NoteReturnURL") = "ProductDetails.aspx?id=" & Session("ProductId") & "&item=" & Trim(Session("ProductText"))
                Response.Redirect("ViewNotes.aspx?notetype=Product&id=-1&productid=" & Trim(Session("ProductId")) & "&item=" & Trim(Session("ProductText")), True)
            Else
                Response.Redirect("ProductDetails.aspx?item=New Product&id=0", True)
            End If
        End Sub

        Private Sub BlankProduct()
            Session("ProductId") = 0
            txtProductName.Text = ""
            txtProductCode.Text = ""
            txtProductDescription.Text = ""
            txtUserCode.Text = ""
            txtInstalledVersion.Text = ""
            txtDateInstalled.Text = ""
            txtAvailableVersion.Text = ""
            txtNumLicensedCopies.Text = "0"
            lnkLicences.Text = "Licences"
            txtNumLicences.Text = "Not available on addition"
            litLicences.Text = "Licences"
            litNotes.Text = "Not available on addition"
        End Sub

        Private Function DeleteProduct(ByVal db As cFWDBConnection, ByVal prodId As Integer) As String
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim retVal As String = ""
            Dim ALog As New cFWAuditLog(fws, SpendManagementElement.Products, curUser.CurrentSubAccountId)

            ' check referential integrity before attempting deletion
            If SMRoutines.CheckRefIntegrity(db, "productDetails", prodId) = False Then
                db.FWDb("R", "productDetails", "ProductId", prodId, "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "")
                If db.FWDbFlag = True Then
                    ' log deletion in audit log
                    Dim ARec As New cAuditRecord
                    ARec.Action = cFWAuditLog.AUDIT_DEL
                    ARec.DataElementDesc = "PRODUCT DETAIL"
                    ARec.ElementDesc = ""
                    ARec.PreVal = db.FWDbFindVal("ProductName", 1)
                    ARec.PostVal = ""
                    ARec.ContractNumber = ""
                    ALog.AddAuditRec(ARec, True)
                    ALog.CommitAuditLog(curUser.Employee, prodId)

                    db.FWDb("D", "product_suppliers", "ProductId", prodId, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "product_usage", "Product Id", prodId, "", "", "", "", "", "", "", "", "", "")
                    db.FWDb("D", "productDetails", "ProductId", prodId, "subAccountId", curUser.CurrentSubAccountId, "", "", "", "", "", "", "", "")
                End If

                Session("ProductId") = 0
                Session("ProductText") = ""

                retVal = "Product Deleted Successfully."
            Else
                retVal = "ERROR! Cannot perform action as entity is currently assigned."
            End If

            Return retVal
        End Function

		Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
			UpdateProduct()
		End Sub

		Private Sub lstCategoryFilter_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCategoryFilter.SelectedIndexChanged
			Dim filterVal As Integer

			filterVal = lstCategoryFilter.SelectedItem.Value
			litSearchResults.Text = GetProducts("", filterVal)
		End Sub

		Protected Sub lnkNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNew.Click
			Response.Redirect("ProductDetails.aspx?action=add&item=New Product", True)
		End Sub

		Protected Sub lnkNotes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNotes.Click
			GetNotes()
		End Sub

		Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdCancel.Click
			Dim url As String = "ProductDetails.aspx"

			Select Case Request.QueryString("ret")
				Case "1"
					Dim conId As String = Request.QueryString("cid")
					If conId.Trim <> "" Then
						url = "ContractSummary.aspx?cid=" & conId & "&tab=" & SummaryTabs.ContractProduct
					End If

				Case "2"
					url = "SummaryPortal.aspx"

				Case Else

			End Select

			Response.Redirect(url, True)
		End Sub

		Protected Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdOK.Click
			Session("ProductFilter") = txtFilter.Text
			litSearchResults.Text = GetProducts(txtFilter.Text, 0)
		End Sub

		Private Function GetProducts(ByVal filterStr As String, ByVal filterCategory As Integer) As String
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
			Dim strHTML As New System.Text.StringBuilder
			Dim searchSQL As New System.Text.StringBuilder
			Dim db As New cFWDBConnection

			If filterCategory = -1 Then
				Return ""
				Exit Function
			End If

            db.DBOpen(fws, False)

            searchSQL.Append("SELECT [ProductId],[ProductName],ISNULL([productDetails].[Description],'') AS [Description],ISNULL([productCategories].[Description],'') AS [Category Description] FROM [productDetails] ")
            searchSQL.Append("LEFT JOIN [productCategories] ON [productCategories].[CategoryId] = [productDetails].[ProductCategoryId] ")
            searchSQL.Append("WHERE [productDetails].[subAccountId] = @subAccId")
            db.AddDBParam("subAccId", curUser.CurrentSubAccountId, True)
			If filterCategory <> 0 Then
                searchSQL.Append(" AND [ProductCategoryId] = @filter")
                db.AddDBParam("filter", filterCategory, False)
			End If
			If filterStr <> "" Then
                searchSQL.Append(" AND LOWER([ProductName]) LIKE LOWER('%' + @filterVal + '%')")
                db.AddDBParam("filterVal", filterStr, False)
            End If

            searchSQL.Append(" ORDER BY [ProductName]")

			db.RunSQL(searchSQL.ToString, db.glDBWorkA, False, "", False)

			Dim drow As DataRow
            With strHTML
                .Append("<table class=""datatbl"">" & vbNewLine)
                .Append("<tr>" & vbNewLine)

                If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, True) Then
                    .Append("<th><img src=""./icons/edit.gif"" /></th>" & vbNewLine)
                End If

                If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False) Then
                    .Append("<th><img src=""./icons/delete2.gif"" /></th>" & vbNewLine)
                End If
                .Append("<th>Product Name</th>" & vbNewLine)
                .Append("<th>Product Description</th>" & vbNewLine)
                .Append("<th>Product Category</th>" & vbNewLine)
                .Append("</tr>" & vbNewLine)

                Dim rowalt As Boolean = False
                Dim rowClass As String = "row1"

                If db.glNumRowsReturned > 0 Then
                    For Each drow In db.glDBWorkA.Tables(0).Rows
                        rowalt = (rowalt Xor True)
                        If rowalt Then
                            rowClass = "row1"
                        Else
                            rowClass = "row2"
                        End If

                        .Append("<tr>" & vbNewLine)

                        If curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Products, True) Then
                            .Append("<td class=""" & rowClass & """><a href=""ProductDetails.aspx?action=edit&id=" & drow.Item("ProductId") & "&item=" & drow.Item("ProductName") & """ onmouseover=""window.status='Edit product details for " & drow.Item("ProductName") & "';return true;"" onmouseout=""window.status='Done';""><img src=""./icons/edit.gif"" /></a></td>" & vbNewLine)
                        End If

                        If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False) Then
                            .Append("<td class=""" & rowClass & """><a onclick=""javascript:if(confirm('Click OK to confirm deletion of " & drow.Item("ProductName") & "')){window.location.href='ProductDetails.aspx?action=delete&id=" & drow.Item("ProductId") & "'};"" onmouseover=""window.status='Delete product " & drow.Item("ProductName") & "';return true;"" onmouseout=""window.status='Done';"" style=""cursor: hand;""><img src=""./icons/delete2.gif"" /></a></td>" & vbNewLine)
                        End If
                        .Append("<td class=""" & rowClass & """>" & drow.Item("ProductName") & "</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>")
                        If CStr(drow.Item("Description")).Length > 50 Then
                            .Append("<textarea style=""width: 300px;"" rows=""3"" readonly>" & vbNewLine)
                            .Append(drow.Item("Description"))
                            .Append("</textarea>" & vbNewLine)
                        Else
                            .Append(drow.Item("Description"))
                        End If
                        .Append("</td>" & vbNewLine)
                        .Append("<td class=""" & rowClass & """>" & drow.Item("Category Description") & "</td>" & vbNewLine)
                        .Append("</tr>" & vbNewLine)
                    Next
                Else
                    .Append("<tr>" & vbNewLine)
                    .Append("<td class=""row1"" ")
                    If curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Products, False) Then
                        .Append("colspan=""5""")
                    Else
                        .Append("colspan=""4""")
                    End If
                    .Append(">No matching results found</td>" & vbNewLine)
                    .Append("</tr>" & vbNewLine)
                End If

                .Append("</table>" & vbNewLine)
            End With
			Return strHTML.ToString
		End Function

		Protected Sub cmdClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdClose.Click
			Response.Redirect("MenuMain.aspx?menusection=productinfo", True)
		End Sub

		Protected Sub lnkDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkDelete.Click
			Response.Redirect("ProductDetails.aspx?action=delete&id=" & Session("ProductId") & "&item=" & Session("ProductText"), True)
		End Sub

		Private Function GetUserFields(ByVal db As cFWDBConnection, ByVal productID As Integer) As String
            Dim litStr As New System.Text.StringBuilder
            Dim sql, tmpStr, tmpDate As String
            Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
            Dim drow, d2row As DataRow
            Dim firstpass As Boolean
            Dim fieldcount As Integer
            Dim employees As New cEmployees(curUser.Account.accountid)
            Dim sites As New cSites(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, cAccounts.getConnectionString(curUser.AccountID))

            firstpass = True

            ' literal part of the form is a table that is 4 cells across with literal in the first cell
            ' label1</td><td>data1</td><td>label2</td><td>data2</td>
            ' leading and trailing td are already present surrounding the literal

            ' obtain user defined fields. If hasData param present, use db.fwdbfindval to populate fields
            sql = "SELECT * FROM [user_fields] WHERE [AppArea] = " & AppAreas.PRODUCT_DETAILS & " ORDER BY [Field Sequence]"
            db.RunSQL(sql, db.glDBWorkA, False, "", False)

            fieldcount = 1

            If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                litStr.Append("<div class=""inputpaneltitle"">Product User Defined Fields</div>" & vbNewLine)
                litStr.Append("<table>" & vbNewLine)
                litStr.Append("<tr>" & vbNewLine)

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    litStr.Append("<td class=""labeltd"">")

                    litStr.Append(Trim(drow.Item("Field Name")) & "</td>" & vbNewLine)

                    Select Case CType(drow.Item("Field Type"), UserFieldType)
                        Case UserFieldType.Character ' character
                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            litStr.Append("<td class=""inputtd""><input type=text id=""" & tmpStr & """ name=""" & tmpStr & """ maxlength=50 ")
                            If productID <> 0 Then
                                litStr.Append(" value=""" & db.FWDbFindVal(tmpStr, 1) & """ alt=""" & db.FWDbFindVal(tmpStr, 1) & """")
                            End If

                            litStr.Append(">" & vbNewLine)

                        Case UserFieldType.Number, UserFieldType.Float ' numeric
                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            litStr.Append("<td class=""inputtd""><input type=text id=""" & tmpStr & """ name=""" & tmpStr & """ onchange=""validateItem('" & Trim(tmpStr) & "',1,'" & Trim(drow.Item("Field Name")) & "');"" ")
                            If productID <> 0 Then
                                litStr.Append(" value=""" & db.FWDbFindVal(tmpStr, 1) & """ alt=""" & db.FWDbFindVal(tmpStr, 1) & """")
                            End If

                            litStr.Append(">" & vbNewLine)

                        Case UserFieldType.DateField ' date
                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            litStr.Append("<td class=""inputtd""><input type=text id=""" & tmpStr & """ name=""" & tmpStr & """ onchange=""validateItem('" & Trim(tmpStr) & "',4,'" & Trim(drow.Item("Field Name")) & "');"" ")
                            If productID <> 0 Then
                                tmpDate = db.FWDbFindVal(tmpStr, 1)
                                litStr.Append(" value=""" & String.Format(cDef.DATE_FORMAT, CDate(tmpDate)) & """")
                            End If

                            litStr.Append(">" & vbNewLine)

                        Case UserFieldType.DDList ' ddlist
                            Dim selectedIdx As String
                            Dim Options As New System.Text.StringBuilder
                            Dim hasSelected As Boolean

                            hasSelected = False

                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            If productID <> 0 Then
                                selectedIdx = Trim(db.FWDbFindVal(tmpStr, 1))
                            Else
                                selectedIdx = ""
                            End If

                            litStr.Append("<td class=""inputtd""><select name=""" & tmpStr & """")
                            litStr.Append(">" & vbNewLine)

                            sql = "SELECT * FROM [user_fieldvalues] WHERE [User Field Id] = @UFI ORDER BY [Field Value]"
                            db.AddDBParam("UFI", Trim(drow.Item("User Field Id")), True)
                            db.RunSQL(sql, db.glDBWorkB, False, "", False)

                            If db.GetRowCount(db.glDBWorkB, 1) > 0 Then
                                For Each d2row In db.glDBWorkB.Tables(0).Rows
                                    Options.Append("<option value=""" & Trim(d2row.Item("Field Value")) & """")
                                    If Trim(d2row.Item("Field Value")) = selectedIdx Then
                                        Options.Append(" selected")
                                        hasSelected = True
                                    End If
                                    Options.Append(">" & Trim(d2row.Item("Field Value")) & "</option>" & vbNewLine)
                                Next
                            End If

                            litStr.Append("<option value=""[None]"" " & IIf(hasSelected = False, "selected", "") & ">[None]</option>" & vbNewLine)
                            litStr.Append(Options)

                            litStr.Append("</select>" & vbNewLine)

                        Case UserFieldType.Checkbox ' checkbox
                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))

                            litStr.Append("<td class=""inputtd""><input type=checkbox name=""" & tmpStr & """")
                            If productID <> 0 Then
                                If db.FWDbFindVal(tmpStr, 1) = "1" Then
                                    litStr.Append(" checked ")
                                End If
                            End If

                            litStr.Append("/>")

                        Case UserFieldType.StaffName_Ref
                            Dim selectedIdx As String
                            Dim Options As New System.Text.StringBuilder
                            Dim hasSelected As Boolean

                            hasSelected = False

                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            If productID <> 0 Then
                                selectedIdx = Trim(db.FWDbFindVal(tmpStr, 1))
                            Else
                                selectedIdx = "0"
                            End If

                            litStr.Append("<td class=""inputtd"">")

                            If employees.getCount(curUser.Account.accountid) > cDef.UF_MAXCOUNT Then
                                ' if too many elements for ddlist, display as search box
                                Dim emp As cEmployee = employees.GetEmployeeById(selectedIdx)
                                
                                litStr.Append("<input type=""text"" readonly name=""" & tmpStr & "_TXT"" value=""" & emp.firstname + " " + emp.surname & """/>")
                                litStr.Append("<input type=""hidden"" name=""" & tmpStr & """ value=""" & selectedIdx.Trim & """/>&nbsp;")
                                litStr.Append("<a onclick=""javascript:doSearch(" & UserFieldType.StaffName_Ref & ",'" & tmpStr & "');"" onmouseover=""window.status='Search for field value';return true;"" onmouseout=""window.status='Done';"">")
                                litStr.Append("<img src=""./buttons/find.gif"" alt=""Search"" />")
                                litStr.Append("</a>")
                            Else
                                litStr.Append("<SELECT name=""" & tmpStr & """")

                                litStr.Append(">" & vbNewLine)

                                sql = "SELECT [Staff Id],[Staff Name] FROM [staff_details] WHERE [Location Id] = @Location_Id ORDER BY [Staff Name]"
                                db.AddDBParam("Location Id", curUser.CurrentSubAccountId, True)
                                db.RunSQL(sql, db.glDBWorkB, False, "", False)

                                If db.GetRowCount(db.glDBWorkB, 1) > 0 Then
                                    For Each d2row In db.glDBWorkB.Tables(0).Rows
                                        Options.Append("<OPTION value=""" & Trim(d2row.Item("Staff Id")) & """")
                                        If Trim(d2row.Item("Staff Id")) = selectedIdx Then
                                            Options.Append(" selected")
                                            hasSelected = True
                                        End If
                                        Options.Append(">" & Trim(d2row.Item("Staff Name")) & "</OPTION>" & vbNewLine)
                                    Next
                                End If

                                litStr.Append("<OPTION value=""0"" " & IIf(hasSelected = False, "selected", "") & ">[None]</OPTION>" & vbNewLine)
                                litStr.Append(Options)

                                litStr.Append("</SELECT>" & vbNewLine)
                            End If

                        Case UserFieldType.Site_Ref
                            Dim selectedIdx As String
                            Dim Options As New System.Text.StringBuilder
                            Dim hasSelected As Boolean

                            hasSelected = False

                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            If productID <> 0 Then
                                selectedIdx = Trim(db.FWDbFindVal(tmpStr, 1))
                            Else
                                selectedIdx = "0"
                            End If

                            litStr.Append("<td class=""inputtd"">")

                            If sites.Count > cDef.UF_MAXCOUNT Then
                                ' if too many elements for ddlist, display as search box
                                db.FWDb("R3", "codes_sites", "Site Id", selectedIdx, "", "", "", "", "", "", "", "", "", "")
                                Dim tmpName As String
                                If db.FWDb3Flag = True Then
                                    tmpName = db.FWDbFindVal("Site Code", 3) & ":" & db.FWDbFindVal("Site Description", 3)
                                Else
                                    tmpName = ""
                                End If
                                litStr.Append("<input type=""text"" readonly name=""" & tmpStr & "_TXT"" value=""" & Trim(tmpName) & """/>")
                                litStr.Append("<input type=""hidden"" name=""" & tmpStr & """ value=""" & Trim(selectedIdx) & """/>&nbsp;")
                                litStr.Append("<a onclick=""javascript:doSearch(" & UserFieldType.Site_Ref & ",'" & tmpStr & "');"" onmouseover=""window.status='Search for field value';return true;"" onmouseout=""window.status='Done';"">")
                                litStr.Append("<img src=""./buttons/find.gif"" alt=""Search"" />")
                                litStr.Append("</a>")
                            Else
                                litStr.Append("<select class=""udf150"" name=""" & tmpStr & """")

                                litStr.Append(">" & vbNewLine)

                                sql = "SELECT [Site Id],[Site Code],[Site Description] FROM [codes_sites] WHERE [Location Id] = @Location_Id ORDER BY [Site Code]"
                                db.AddDBParam("Location Id", curUser.CurrentSubAccountId, True)
                                db.RunSQL(sql, db.glDBWorkB, False, "", False)

                                If db.GetRowCount(db.glDBWorkB, 1) > 0 Then
                                    For Each d2row In db.glDBWorkB.Tables(0).Rows
                                        Options.Append("<option value=""" & Trim(d2row.Item("Site Id")) & """")
                                        If Trim(d2row.Item("Site Id")) = selectedIdx Then
                                            Options.Append(" selected")
                                            hasSelected = True
                                        End If
                                        Options.Append(">" & Trim(d2row.Item("Site Code")) & " [" & Trim(d2row.Item("Site Description")) & "]" & "</option>" & vbNewLine)
                                    Next
                                End If

                                litStr.Append("<option value=""0"" " & IIf(hasSelected = False, "selected", "") & ">[None]</option>" & vbNewLine)
                                litStr.Append(Options)

                                litStr.Append("</select>" & vbNewLine)
                            End If

                        Case UserFieldType.RechargeClient_Ref
                            Dim selectedIdx As String
                            Dim Options As New System.Text.StringBuilder
                            Dim hasSelected As Boolean
                            Dim rClients As New cRechargeClientList(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID))

                            hasSelected = False

                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            If productID <> 0 Then
                                selectedIdx = Trim(db.FWDbFindVal(tmpStr, 1))
                            Else
                                selectedIdx = "0"
                            End If

                            litStr.Append("<td class=""inputtd"">")

                            If rClients.Count > cDef.UF_MAXCOUNT Then
                                ' if too many elements for ddlist, display as search box
                                db.FWDb("R3", "codes_rechargeentity", "Entity Id", selectedIdx, "", "", "", "", "", "", "", "", "", "")
                                Dim tmpName As String
                                If db.FWDb3Flag = True Then
                                    tmpName = db.FWDbFindVal("Name", 3)
                                Else
                                    tmpName = ""
                                End If
                                litStr.Append("<input type=""text"" readonly name=""" & tmpStr & "_TXT"" value=""" & Trim(tmpName) & """/>")
                                litStr.Append("<input type=""hidden"" name=""" & tmpStr & """ value=""" & Trim(selectedIdx) & """/>&nbsp;")
                                litStr.Append("<a onclick=""javascript:doSearch(" & UserFieldType.RechargeClient_Ref & ",'" & tmpStr & "');"" onmouseover=""window.status='Search for field value';return true;"" onmouseout=""window.status='Done';"">")
                                litStr.Append("<img src=""./buttons/find.gif"" alt=""Search"" />")
                                litStr.Append("</a>")
                            Else
                                litStr.Append("<select name=""" & tmpStr & """")
                                litStr.Append(">" & vbNewLine)

                                sql = "SELECT [Entity Id],[Name] FROM [codes_rechargeentity] WHERE [Location Id] = @Location_Id ORDER BY [Name]"
                                db.AddDBParam("Location Id", curUser.CurrentSubAccountId, True)
                                db.RunSQL(sql, db.glDBWorkB, False, "", False)

                                If db.GetRowCount(db.glDBWorkB, 1) > 0 Then
                                    For Each d2row In db.glDBWorkB.Tables(0).Rows
                                        Options.Append("<option value=""" & Trim(d2row.Item("Entity Id")) & """")
                                        If Trim(d2row.Item("Entity Id")) = selectedIdx Then
                                            Options.Append(" selected")
                                            hasSelected = True
                                        End If
                                        Options.Append(">" & Trim(d2row.Item("Name")) & "</option>" & vbNewLine)
                                    Next
                                End If

                                litStr.Append("<option value=""0"" " & IIf(hasSelected = False, "selected", "") & ">[None]</option>" & vbNewLine)
                                litStr.Append(Options)

                                litStr.Append("</select>" & vbNewLine)
                            End If

                        Case UserFieldType.RechargeAcc_Code
                            Dim selectedIdx As String
                            Dim Options As New System.Text.StringBuilder
                            Dim hasSelected As Boolean
                            Dim accs As New cRechargeAccountCodes(curUser.AccountID, curUser.CurrentSubAccountId, cAccounts.getConnectionString(curUser.AccountID))
                            hasSelected = False

                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            If productID <> 0 Then
                                selectedIdx = Trim(db.FWDbFindVal(tmpStr, 1))
                            Else
                                selectedIdx = "0"
                            End If

                            litStr.Append("<td class=""inputtd"">")

                            If accs.Count > cDef.UF_MAXCOUNT Then
                                ' if too many elements for ddlist, display as search box
                                db.FWDb("R3", "codes_accountcodes", "Code Id", selectedIdx, "", "", "", "", "", "", "", "", "", "")
                                Dim tmpName As String
                                If db.FWDb3Flag = True Then
                                    tmpName = db.FWDbFindVal("Account Code", 3) & ":" & db.FWDbFindVal("Description", 3)
                                Else
                                    tmpName = ""
                                End If

                                litStr.Append("<input type=""text"" readonly name=""" & tmpStr & "_TXT"" value=""" & Trim(tmpName) & """/>")
                                litStr.Append("<input type=""hidden"" name=""" & tmpStr & """ value=""" & Trim(selectedIdx) & """/>&nbsp;")
                                litStr.Append("<a onclick=""javascript:doSearch(" & Trim(Str(UserFieldType.RechargeAcc_Code)) & ",'" & tmpStr & "');"" onmouseover=""window.status='Search for field value';return true;"" onmouseout=""window.status='Done';"">")
                                litStr.Append("<img src=""./buttons/find.gif"" alt=""Search"" />")
                                litStr.Append("</a>")
                            Else
                                litStr.Append("<select name=""" & tmpStr & """")
                                litStr.Append(">" & vbNewLine)

                                sql = "SELECT [Code Id],[Account Code] FROM [codes_accountcodes] WHERE [Location Id] = @Location_Id ORDER BY [Account Code]"
                                db.AddDBParam("Location Id", curUser.CurrentSubAccountId, True)
                                db.RunSQL(sql, db.glDBWorkB, False, "", False)

                                If db.GetRowCount(db.glDBWorkB, 1) > 0 Then
                                    For Each d2row In db.glDBWorkB.Tables(0).Rows
                                        Options.Append("<option value=""" & Trim(d2row.Item("Code Id")) & """")
                                        If Trim(d2row.Item("Code Id")) = selectedIdx Then
                                            Options.Append(" selected")
                                            hasSelected = True
                                        End If
                                        Options.Append(">" & Trim(d2row.Item("Account Code")) & "</option>" & vbNewLine)
                                    Next
                                End If

                                litStr.Append("<option value=""0"" " & IIf(hasSelected = False, "selected", "") & ">[None]</option>" & vbNewLine)
                                litStr.Append(Options)

                                litStr.Append("</select>" & vbNewLine)
                            End If

                        Case UserFieldType.Text
                            Dim txtData As String

                            tmpStr = "UF" & Trim(drow.Item("User Field Id"))
                            If productID <> 0 Then
                                txtData = db.FWDbFindVal(tmpStr, 1).Trim
                            Else
                                txtData = ""
                            End If

                            litStr.Append("<td class=""inputtd"">")
                            litStr.Append("<input type=""text"" readonly name=""" & tmpStr & """ value=""" & txtData.Trim & """/>")
                            litStr.Append("<a onclick=""javascript:doTextEntry('" & tmpStr & "'," & productID.ToString & "," & AppAreas.PRODUCT_DETAILS & ");"" onmouseover=""window.status='Modify extended text entry';return true;"" onmouseout=""window.status='Done';"">")
                            litStr.Append("<img src=""./buttons/edit.gif"" alt=""Edit"" />")
                            litStr.Append("</a>")

                        Case Else
                            ' shouldn't be possible for anything else
                    End Select

                    fieldcount = fieldcount + 1

                    If fieldcount = 3 Then
                        fieldcount = 1
                        ' start a new row
                        litStr.Append("</td></tr>" & vbNewLine & "<tr>" & vbNewLine)
                    Else
                        litStr.Append("</td>" & vbNewLine)
                    End If
                Next

                ' pad out the remains if incomplete table
                If fieldcount = 2 Then
                    litStr.Append("<td></td>" & vbNewLine)
                End If
                litStr.Append("</tr></table>" & vbNewLine)
            End If

			Return litStr.ToString
		End Function

        Protected Sub lnkLicences_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkLicences.Click
            Response.Redirect("ProductLicences.aspx?pid=" & Session("ProductId"), True)
        End Sub

        Protected Sub lnkAddTask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkAddTask.Click
            Dim varURL As String
            varURL = "tid=0&rid=" & Session("ProductId") & "&rtid=" & AppAreas.PRODUCT_DETAILS
            Session("TaskRetURL") = Server.UrlEncode("~/ProductDetails.aspx?action=edit&id=" & Session("ProductId") & "&item=" & Session("ProductText"))
            Response.Redirect("~/shared/tasks/ViewTask.aspx?" & varURL, True)
        End Sub

        Protected Sub lnkTaskSummary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkTaskSummary.Click
            Dim varURL As String
            varURL = "?pid=" & Session("ProductId") & "&paa=" & AppAreas.PRODUCT_DETAILS
            varURL += "&ret=" & Server.UrlEncode(cMisc.path + "/ProductDetails.aspx?action=edit&id=" & Session("ProductId") & "&item=" & Session("ProductText"))
            Session("TaskRetURL") = Server.UrlEncode(cMisc.path + "/ProductDetails.aspx?action=edit&id=" & Session("ProductId") & "&item=" & Session("ProductText"))

            Response.Redirect(cMisc.path + "/shared/tasks/TaskSummary.aspx" & varURL, True)
        End Sub
    End Class
End Namespace
