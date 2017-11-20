Imports FWBase
Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Collections.Generic

Public Class BulkRemoval
    Private sumTotalDeletes As Integer = 0
    Private hasAttachments As Boolean = False
    Private pRunningTotal As Integer
    Private pCollateRunningTotal As Boolean

    Private Property collateRunningTotal() As Boolean
        Get
            Return pCollateRunningTotal
        End Get
        Set(ByVal value As Boolean)
            pCollateRunningTotal = value
        End Set
    End Property

    Private Property runningTotal() As Integer
        Get
            Return pRunningTotal
        End Get
        Set(ByVal value As Integer)
            pRunningTotal = value
        End Set
    End Property

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        ' perform search to analyse specified contract
        If txtKey.Text.Trim <> "" Then
            cmdOK.Enabled = False
            hasAttachments = False

            Dim prevCursor As Cursor = Cursor.Current
            Dim sql As String

            Cursor.Current = Cursors.WaitCursor

            Dim db As New DBConnection(FWEmail.ConnectionString)
            sql = "select count(*) from contract_details where contractKey = @ck"

            db.sqlexecute.Parameters.AddWithValue("@ck", txtKey.Text.Trim)
            Dim count As Integer = db.getcount(sql)

            If count = 0 Then
                MsgBox("Failed to retrieve the count information for the specified contract." & vbNewLine & "Contract Key not found.", MsgBoxStyle.Critical, "FWUtilities Error")
            Else
                Dim dset As DataSet
                dset = GetContractStats(db, txtKey.Text.Trim)
                ShowStats(dset, resGrid)
                cmdDelete.Enabled = True
            End If

            Cursor.Current = prevCursor
            cmdOK.Enabled = True
        End If
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        ' get them to confirm that the deletion is to take place
        If MsgBox("This action is about to delete " & sumTotalDeletes.ToString & " records from the database" & vbNewLine & "Are you sure you want to perform this action?", MsgBoxStyle.YesNo, "Bulk Removal") = MsgBoxResult.Yes Then
            Dim db As New DBConnection(FWEmail.ConnectionString)
            db.sqlexecute.Parameters.AddWithValue("@ck", txtKey.Text.Trim)
            Dim cId As Integer = db.getcount("select contractId from contract_details where contractKey = @ck")
            If cId > 0 Then
                DeleteContractAll(db, cId)

                MsgBox("Contract " & txtKey.Text.Trim & " has been deleted along with all it's dependencies")

                If hasAttachments Then
                    MsgBox("References to file attachments were deallocated from the deleted contracts." & vbNewLine & "It is recommended that an administrator accesses the 'Verify Attachments'" & vbNewLine & "option to remove the physical files.")
                End If
            End If
        End If

        Close()
    End Sub

    Private Sub BulkRemoval_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtKey.Text = FWEmail.FieldKey & "/?/?"
        'txtKey.Text = FWEmail.fws.KeyPrefix & "/4708/7"
        
        Dim accs As New cUtilSubAccounts
        cboLocation.Items.AddRange(accs.CreateDropDown)
    End Sub

    Private Sub ShowStats(ByVal dset As DataSet, ByRef grid As DataGridView)
        ' create a datatable showing the count stats for the contract found
        grid.Rows.Clear()
        sumTotalDeletes = 0

        grid.ColumnCount = 2
        With grid.ColumnHeadersDefaultCellStyle
            .BackColor = Color.Navy
            .ForeColor = Color.White
            .Font = New Font(resGrid.Font, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        With grid
            .Columns(0).Width = 250
            .Columns(1).Width = 180
            .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Name = "resGrid"
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .GridColor = Color.Black
            .RowHeadersVisible = False

            .Columns(0).Name = "Application Area"
            .Columns(1).Name = "No. Associated Records"

            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
        End With

        With grid.Rows
            Dim colName As String
            Dim colCount As Integer

            Dim col As DataColumn
            For Each col In dset.Tables(0).Columns
                colName = col.ColumnName
                colCount = dset.Tables(0).Rows(0).Item(col.Ordinal)
                sumTotalDeletes += colCount

                Select Case colName
                    Case "Contract and Notes Attachment Count", "Invoice Notes Attachment Count"
                        If colCount > 0 Then
                            hasAttachments = True
                        End If

                    Case Else
                End Select
                Dim rowVal As String() = {colName, colCount.ToString}
                .Add(rowVal)
            Next

        End With
    End Sub

    Private Function GetContractStats(ByVal db As DBConnection, ByVal contractKey As String) As DataSet
        Dim sql As New System.Text.StringBuilder
        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        sql.Append("SELECT " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_history] WHERE [ContractId] = @conId) AS [Contract History Count]," & vbNewLine)
        If FWEmail.ActiveDBVersion > 18 Then
            sql.Append("(SELECT COUNT(*) FROM [contract_audience] WHERE [ContractId] = @conId) AS [Contract Audience Count]," & vbNewLine)
        End If
        sql.Append("(SELECT COUNT(*) FROM [link_matrix] WHERE [ContractId] = @conId) AS [Contract Link Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [link_variations] WHERE [PrimaryContractId] = @conId OR [VariationContractId] = @conId) AS [Variation Links]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_notification] WHERE [ContractId] = @conId) AS [Contract Notify Count], " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_notes] WHERE [ContractId] = @conId) AS [Contract Notes Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [attachments] WHERE [ReferenceNumber] = @conId AND [AttachmentType] IN (@conAttachmentType,@conNoteAttType)) AS [Contract and Notes Attachment Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productdetails] WHERE [ContractId] = @conId) AS [Contract Product Count], " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productinformation] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)) AS [Contract Product Info Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_producthistory] WHERE [Contract-Product Id] IN (SELECT [Contract-Product Id] FROM [contract_productdetails] WHERE [ContractId] = @conId)) AS [Contract Producy History Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productdetails_calloff] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)) AS [Contract Product Calloff Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productdetails_recharge] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)) AS [Contract Product Recharge Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productplatforms] WHERE [ContractId] = @conId) AS [Contract Product Platforms Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [recharge_associations] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)) AS [Recharge Association Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [attachment_audience] WHERE [AttachmentId] IN (SELECT [AttachmentId] FROM [attachments] WHERE [ReferenceNumber] = @conId AND [AttachmentType] IN (@conAttachmentType,@conNoteAttType))) AS [Attachment Audience Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoice_details] WHERE [ContractId] = @conId) AS [Invoice Details Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoice_productdetails] WHERE [ContractId] = @conId) AS [Invoice Product Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoice_notes]" & vbNewLine)
        sql.Append("INNER JOIN [invoice_details] ON [invoice_details].[InvoiceId] = [invoice_notes].[Invoice Id] " & vbNewLine)
        sql.Append("WHERE [ContractId] = @conId " & vbNewLine)
        sql.Append(") AS [Invoice Notes Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [attachments] " & vbNewLine)
        sql.Append("INNER JOIN [invoice_notes] ON [invoice_notes].[InvoiceId] = [attachments].[ReferenceNumber] " & vbNewLine)
        sql.Append("INNER JOIN [invoices] ON [invoices].[InvoiceId] = [invoiceNotes].[InvoiceId] " & vbNewLine)
        sql.Append("WHERE [ContractId] = @conId AND [AttachmentType] = @invNoteAttType " & vbNewLine)
        sql.Append(") AS [Invoice Notes Attachment Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoice_log] WHERE [InvoiceId] IN (SELECT [InvoiceId] FROM [invoices] WHERE [ContractId] = @conId)) AS [Invoice Status History Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_forecastdetails] WHERE [ContractId] = @conId) AS [Invoice Forecast Count]" & vbNewLine)

        db.sqlexecute.Parameters.AddWithValue("@ck", contractKey)
        Dim dset As DataSet
        Dim cId As Integer = db.getcount("select contractId from contract_details where contractKey = @ck")
        If cId > 0 Then
            db.sqlexecute.Parameters.Clear()
            db.sqlexecute.Parameters.AddWithValue("conId", cId)
            db.sqlexecute.Parameters.AddWithValue("invNoteAttType", AttachmentArea.INVOICE_NOTES)
            db.sqlexecute.Parameters.AddWithValue("conNoteAttType", AttachmentArea.CONTRACT_NOTES)
            db.sqlexecute.Parameters.AddWithValue("conAttachmentType", AttachmentArea.CONTRACT)
            dset = db.GetDataSet(sql.ToString)
        Else
            dset = New DataSet
        End If

        Return dset
    End Function

    Private Function DeleteContractAll(ByVal db As DBConnection, ByVal cId As Integer) As Boolean
        Dim sql As String

        Try
            db.sqlexecute.Parameters.AddWithValue("conId", cId)
            db.sqlexecute.Parameters.AddWithValue("invNoteAttType", AttachmentArea.INVOICE_NOTES)
            db.sqlexecute.Parameters.AddWithValue("conNoteAttType", AttachmentArea.CONTRACT_NOTES)
            db.sqlexecute.Parameters.AddWithValue("conAttachmentType", AttachmentArea.CONTRACT)

            sql = "DELETE FROM [contract_history] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [link_matrix] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [link_variations] WHERE [PrimaryContractId] = @conId OR [VariationContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_notification] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "UPDATE [attachments] SET [ReferenceNumber] = -1 WHERE [ReferenceNumber] = @conId AND [AttachmentType] IN (@conAttachmentType,@conNoteAttType)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_productinformation] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_producthistory] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_productdetails_calloff] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_productdetails_recharge] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [recharge_associations] WHERE [ContractProductId] IN (SELECT [ContractProductId] FROM [contract_productdetails] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_productdetails] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_productplatforms] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [attachment_audience] WHERE [AttachmentId] IN (SELECT [AttachmentId] FROM [attachments] WHERE [ReferenceNumber] = @conId AND [AttachmentType] IN (@conAttachmentType,@conNoteAttType))" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [invoice_productdetails] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "UPDATE [attachments] SET [ReferenceNumber] = -1 WHERE [ReferenceNumber] IN (SELECT [NoteId] FROM [invoiceNotes] INNER JOIN [invoices] ON [invoices].[InvoiceId] = [invoiceNotes].[InvoiceId] WHERE [ContractId] = @conId) AND [AttachmentType] = @invNoteAttType" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [invoiceNotes] WHERE [InvoiceId] IN (SELECT [InvoiceId] FROM [invoices] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [invoice_log] WHERE [InvoiceId] IN (SELECT [InvoiceId] FROM [invoices] WHERE [ContractId] = @conId)" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_forecastdetails] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [invoices] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)
            sql = "DELETE FROM [contract_details] WHERE [ContractId] = @conId" & vbNewLine
            db.ExecuteSQL(sql)

            Return True

        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub cmdLocOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLocOK.Click
        Dim db As New DBConnection(FWEmail.ConnectionString)
        Dim dset As New DataSet("locStats")

        cmdLocOK.Enabled = False

        Dim prevCursor As Cursor = Cursor.Current

        Cursor.Current = Cursors.WaitCursor

        Try
            dset = GetLocationStats(db, cboLocation.SelectedValue)

            ShowStats(dset, locResultGrid)

            cmdLocDelete.Enabled = True

        Catch ex As Exception
            MsgBox("The following error occurred trying to retrieve the statistics" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try

        cmdLocOK.Enabled = True
        Cursor.Current = prevCursor
    End Sub

    Private Sub cmdLocCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLocCancel.Click
        Me.Close()
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        runningTotal = 0
        collateRunningTotal = False
    End Sub

    Private Function GetLocationStats(ByVal db As DBConnection, ByVal locId As Integer) As DataSet
        Dim sql As New System.Text.StringBuilder

        sql.Append("SELECT " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [audit_log] WHERE [subAccountId] = @locId) AS [Audit Log Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_accountcodes] WHERE [subAccountId] = @locId) AS [Account Code Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_contractcategory] WHERE [subAccountId] = @locId) AS [Contract Category Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_contractstatus] WHERE [subAccountId] = @locId) AS [Contract Status Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_contracttype] WHERE [subAccountId] = @locId) AS [Contract Type Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_currency] WHERE [subAccountId] = @locId) AS [Currency Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_financialstatus] WHERE [subAccountId] = @locId) AS [Financial Status Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_inflatormetrics] WHERE [subAccountId] = @locId) AS [Inflator Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_invoicefrequencytype] WHERE [subAccountId] = @locId) AS [Invoice Freq Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_invoicestatustype] WHERE [subAccountId] = @locId) AS [Invoice Status Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_licencerenewaltype] WHERE [subAccountId] = @locId) AS [Licence Renewal Type Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_platformtype] WHERE [subAccountId] = @locId) AS [Platform Type Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_productcategory] WHERE [subAccountId] = @locId) AS [Product Category Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_rechargeentity] WHERE [subAccountId] = @locId) AS [Recharge Entity Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_salestax] WHERE [subAccountId] = @locId) AS [Sales Tax Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_sites] WHERE [subAccountId] = @locId) AS [Sites Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_termtype] WHERE [subAccountId] = @locId) AS [Term Type Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [codes_units] WHERE [subAccountId] = @locId) AS [Unit Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [email_schedule] WHERE [Run subAccountId] = @locId) AS [Email Schedule Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [favourite_reports] WHERE [subAccountId] = @locId) AS [Favourite Report Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [fwparams] WHERE [subAccountId] = @locId) AS [FWParams Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [link_definitions] WHERE [subAccountId] = @locId) AS [Link Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [link_matrix] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [link_matrix].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract Link Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [link_variations] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [link_variations].[PrimaryContractId] = [contract_details].[ContractId] " & vbNewLine)
        sql.Append("WHERE [subAccountId] = @locId) AS [Variation Primary Link Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [link_variations] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [link_variations].[VariationContractId] = [contract_details].[ContractId] " & vbNewLine)
        sql.Append("WHERE [subAccountId] = @locId) AS [Variation Link Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [platform_lpars] " & vbNewLine)
        'sql.Append("INNER JOIN [product_platforms] ON [product_platforms].[Platform Id] = [platform_lpars].[Platform Id]" & vbNewLine)
        'sql.Append("WHERE [product_platforms].[subAccountId] = @locId) AS [Platform LPAR Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [product_platforms] WHERE [subAccountId] = @locId) AS [Product Platform Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [product_usage] " & vbNewLine)
        'sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [product_usage].[Product Id]" & vbNewLine)
        'sql.Append("WHERE [productDetails].[subAccountId] = @locId) AS [Product Usage Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [product_suppliers]" & vbNewLine)
        sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [product_suppliers].[ProductId]" & vbNewLine)
        sql.Append("WHERE [productDetails].[subAccountId] = @locId) AS [Product Supplier Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [productDetails] WHERE [subAccountId] = @locId) AS [Product Details Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [productNotes] " & vbNewLine)
        sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [productNotes].[ProductId]" & vbNewLine)
        sql.Append("WHERE [productDetails].[subAccountId] = @locId) AS [Product Note Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [location_access] WHERE [subAccountId] = @locId) AS [Location Access Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [security_locations] WHERE [subAccountId] = @locId) AS [Logon Locations Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [saved_reports] WHERE [subAccountId] = @locId) AS [Saved Report Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [staff_details] WHERE [subAccountId] = @locId) AS [Employee Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [sublocations] WHERE [subAccountId] = @locId) AS [Sublocation Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [teamemps] " & vbNewLine)
        sql.Append("INNER JOIN [teams] ON [teams].[TeamId] = [teamemps].[TeamId]" & vbNewLine)
        sql.Append("WHERE [teams].[subAccountId] = @locId) AS [Team Member Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [teams] WHERE [subAccountId] = @locId) AS [Team Def Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_libraries] " & vbNewLine)
        'sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [ubq_libraries].[ProductId]" & vbNewLine)
        'sql.Append("WHERE [subAccountId] = @locId) AS [UBQ Library Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_locations] " & vbNewLine)
        'sql.Append("INNER JOIN [productDetails] ON [productDetails].[Product Id] = [ubq_locations].[Product Id]" & vbNewLine)
        'sql.Append("WHERE [subAccountId] = @locId) AS [UBQ Location Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_productassociation] " & vbNewLine)
        'sql.Append("INNER JOIN [productDetails] ON [productDetails].[Product Id] = [ubq_productassociation].[FW Product Id]" & vbNewLine)
        'sql.Append("WHERE [productDetails].[subAccountId] = @locId) AS [UBQ ProdAssoc Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_productvendors] " & vbNewLine)
        'sql.Append("INNER JOIN [productDetails] ON [productDetails].[Product Id] = [ubq_productvendors].[Product Id]" & vbNewLine)
        'sql.Append("WHERE [productDetails].[subAccountId] = @locId) AS [UBQ ProdVendor Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_vendorassociation] " & vbNewLine)
        'sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [ubq_vendorassociation].[FW supplierId]" & vbNewLine)
        'sql.Append("WHERE [supplier_details].[subAccountId] = @locId) AS [UBQ VendorAssoc Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_vendors]" & vbNewLine)
        'sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [ubq_vendors].[supplierId]" & vbNewLine)
        'sql.Append("WHERE [supplier_details].[subAccountId] = @locId) AS [UBQ Vendor Count]," & vbNewLine)
        'sql.Append("(SELECT COUNT(*) FROM [ubq_version] " & vbNewLine)
        'sql.Append("INNER JOIN [productDetails] ON [productDetails].[Product Id] = [ubq_version].[Product Id]" & vbNewLine)
        'sql.Append("WHERE [subAccountId] = @locId) AS [UBQ Version Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [userdefinedGroupings] WHERE [subAccountId] = @locId) AS [UF Group Alloc Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [userdefinedGroupingAssociation] " & vbNewLine)
        sql.Append("INNER JOIN [codes_contractcategory] ON [codes_contractcategory].[CategoryId] = [userdefinedGroupingAssociation].[ContractCategoryId]" & vbNewLine)
        sql.Append("WHERE [codes_contractcategory].[subAccountId] = @locId) AS [UF Grouping Cat Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [supplier_categories] WHERE [subAccountId] = @locId) AS [Supplier Category Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [supplier_status] WHERE [subAccountId] = @locId) AS [Supplier Status Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [supplier_addresses] " & vbNewLine)
        sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [supplier_addresses].[supplierId]" & vbNewLine)
        sql.Append("WHERE [supplier_details].[subAccountId] = @locId) AS [Supplier Address Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [supplierNotes] " & vbNewLine)
        sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [supplierNotes].[supplierId]" & vbNewLine)
        sql.Append("WHERE [supplier_details].[subAccountId] = @locId) AS [Supplier Notes Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [supplierContactNotes]" & vbNewLine)
        sql.Append("INNER JOIN [supplier_contacts] ON [supplierContactNotes].[contactId] = [supplier_contacts].[contactId]" & vbNewLine)
        sql.Append("INNER JOIN [supplier_details] ON [supplier_details].[supplierId] = [supplier_addresses].[supplierId]" & vbNewLine)
        sql.Append("WHERE [supplier_details].[subAccountId] = @locId) AS [Supplier Contact Notes Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [supplier_details] WHERE [subAccountId] = @locId) AS [Supplier Def Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [version_history] " & vbNewLine)
        sql.Append("INNER JOIN [version_registry] ON [version_registry].[RegistryId] = [version_history].[RegistryId]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [version_registry].[ContractId] = [contract_details].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Version History Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [version_registry_calloff]" & vbNewLine)
        sql.Append("INNER JOIN [version_registry] ON [version_registry].[RegistryId] = [version_registry_calloff].[RegistryId]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [version_registry].[ContractId] = [contract_details].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Version Registry Calloff Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [version_registry] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [version_registry].[ContractId] = [contract_details].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Version Registry Count], " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_history] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_history].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract History Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_audience]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_audience].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract Audience Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_notification] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_notification].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract Notify Count], " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contractNotes] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contractNotes].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract Notes Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [attachments] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [attachments].[ReferenceNumber]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId AND [AttachmentType] IN (@conAttachmentType,@conNoteAttType)) AS [Contract and Notes Attachment Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productdetails]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract Product Count], " & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productinformation] WHERE [ContractProductId] IN " & vbNewLine)
        sql.Append("(SELECT [ContractProductId] FROM [contract_productdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId)" & vbNewLine)
        sql.Append(") AS [Contract Product Info Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_producthistory] WHERE [ContractProductId] IN " & vbNewLine)
        sql.Append("(SELECT [ContractProductId] FROM [contract_productdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId)" & vbNewLine)
        sql.Append(") AS [Contract Producy History Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productdetails_calloff] WHERE [ContractProductId] IN " & vbNewLine)
        sql.Append("(SELECT [ContractProductId] FROM [contract_productdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId)) AS [Contract Product Calloff Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productdetails_recharge] WHERE [ContractProductId] IN " & vbNewLine)
        sql.Append("(SELECT [ContractProductId] FROM [contract_productdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId)" & vbNewLine)
        sql.Append(") AS [Contract Product Recharge Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_productplatforms] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productplatforms].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Contract Product Platforms Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [recharge_associations] WHERE [ContractProductId] IN" & vbNewLine)
        sql.Append("(SELECT [ContractProductId] FROM [contract_productdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId)" & vbNewLine)
        sql.Append(") AS [Recharge Association Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [attachment_audience] WHERE [AttachmentId] IN " & vbNewLine)
        sql.Append("(SELECT [AttachmentId] FROM [attachments] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [attachments].[ReferenceNumber]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId AND [AttachmentType] IN (@conAttachmentType,@conNoteAttType))" & vbNewLine)
        sql.Append(") AS [Attachment Audience Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoices] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoices].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Invoice Details Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoice_productdetails]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoice_productdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Invoice Product Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoiceNotes]" & vbNewLine)
        sql.Append("INNER JOIN [invoices] ON [invoices].[InvoiceId] = [invoiceNotes].[InvoiceId] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoices].[Contract`Id]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId " & vbNewLine)
        sql.Append(") AS [Invoice Notes Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [attachments] " & vbNewLine)
        sql.Append("INNER JOIN [invoiceNotes] ON [invoiceNotes].[InvoiceId] = [attachments].[ReferenceNumber] " & vbNewLine)
        sql.Append("INNER JOIN [invoices] ON [invoices].[InvoiceId] = [invoiceNotes].[InvoiceId]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoices].[ContractId] " & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId AND [Attachment Type] = @invNoteAttType " & vbNewLine)
        sql.Append(") AS [Invoice Notes Attachment Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [invoice_log] WHERE [InvoiceId] IN " & vbNewLine)
        sql.Append("(SELECT [InvoiceId] FROM [invoices]" & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [invoices].[ContractId] " & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId)" & vbNewLine)
        sql.Append(") AS [Invoice Status History Count]," & vbNewLine)
        sql.Append("(SELECT COUNT(*) FROM [contract_forecastdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_forecastdetails].[ContractId]" & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId) AS [Invoice Forecast Count]" & vbNewLine)

        db.sqlexecute.Parameters.AddWithValue("@locId", locId)
        db.sqlexecute.Parameters.AddWithValue("invNoteAttType", AttachmentArea.INVOICE_NOTES)
        db.sqlexecute.Parameters.AddWithValue("conNoteAttType", AttachmentArea.CONTRACT_NOTES)
        db.sqlexecute.Parameters.AddWithValue("conAttachmentType", AttachmentArea.CONTRACT)
        Dim dset As DataSet = db.GetDataSet(sql.ToString())

        Return dset
    End Function

    Private Sub RemovalTabs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemovalTabs.SelectedIndexChanged
        If RemovalTabs.SelectedIndex = 1 Then
            ' populate the location ddlist

            sumTotalDeletes = 0
            cmdLocDelete.Enabled = False
            cmdDelete.Enabled = False

            If FWEmail.SubAccounts.Count > 0 Then
                cboLocation.Items.AddRange(FWEmail.SubAccounts.CreateDropDown)

                'Dim drow As DataRow
                'For Each drow In db.glDBWorkA.Tables(0).Rows
                '    cboLocation.Items.Add(drow.Item("Description"))
                'Next
            End If
        End If
    End Sub

    Private Sub cboLocation_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboLocation.SelectedIndexChanged
        cmdLocOK.Enabled = True
        cmdLocDelete.Enabled = False
    End Sub

    Private Sub cmdLocDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLocDelete.Click
        If MsgBox("Are you sure, you are about to delete " & runningTotal.ToString & " elements? All data will be deleted permanently.", MsgBoxStyle.YesNo, "Warning") = MsgBoxResult.Yes Then
            DeleteLocation()
        End If
    End Sub

    Private Sub DeleteLocation()

    End Sub
End Class