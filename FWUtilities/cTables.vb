Public Class cTables
    Private slTables As System.Collections.SortedList
    Private currentDBVersion As Integer

    Public Property CurDBVersion() As Integer
        Get
            Return currentDBVersion
        End Get
        Set(ByVal VersionNum As Integer)
            currentDBVersion = VersionNum
        End Set
    End Property

    Public Sub New(ByVal DBVersion As Integer)
        LoadTables()
        CurDBVersion = DBVersion
    End Sub

    Private Sub LoadTables()
        slTables = New System.Collections.SortedList

        With slTables
            .Add("Attachments", "attachments")
            .Add("Audit Log", "audit_log")
            .Add("Code Tables", "code_tables")
            .Add("Codes - Account Codes", "codes_accountcodes")
            .Add("Codes - Contract Category", "codes_contractcategory")
            .Add("Codes - Contract Status", "codes_contractstatus")
            .Add("Codes - Contract Type", "codes_contracttype")
            .Add("Codes - Currency", "codes_currency")
            .Add("Codes - Financial Status", "codes_financialstatus")
            .Add("Codes - Inflator Metrics", "codes_inflatormetrics")
            .Add("Codes - Invoice Frequency Type", "codes_invoicefrequencytype")
            .Add("Codes - Invoice Status Type", "codes_invoicestatustype")
            .Add("Codes - Licence Renewal Type", "codes_licencerenewaltype")
            .Add("Codes - Note Category", "codes_notecategory")
            .Add("Codes - Platform Type", "codes_platformtype")
            .Add("Codes - Product Category", "codes_productcategory")
            .Add("Codes - Recharge Entity", "codes_rechargeentity")
            .Add("Codes - Sales Tax", "codes_salestax")
            .Add("Codes - Sites", "codes_sites")
            .Add("Codes - Term Type", "codes_termtype")
            .Add("Codes - Units", "codes_units")
            .Add("Codes - User Field Grouping", "codes_userfieldgrouping")
            .Add("Contract - Forecast Details", "contract_forecastdetails")
            .Add("Contract - Forecast Products", "contract_forecastproducts")
            .Add("Contract - Notification", "contract_notification")
            .Add("Contract - Product Details", "contract_productdetails")
            .Add("Contract - Product Details - Calloff", "contract_productdetails_calloff")
            .Add("Contract - Product Details - Recharge", "contract_productdetails_recharge")
            .Add("Contract - Product-History", "contract_producthistory")
            .Add("Contract - Product Information", "contract_productinformation")
            .Add("Contract - Product-Platforms", "contract_productplatforms")
            .Add("Contract Details", "contract_details")
            .Add("Contract Notes", "contract_notes")
            .Add("Email Schedule", "email_schedule")
            .Add("Email Templates", "email_templates")
            .Add("Favourite Reports", "favourite_reports")
            .Add("FWParams", "fwparams")
            .Add("Invoice - Product Details", "invoice_productdetails")
            .Add("Invoice Details", "invoice_details")
            .Add("Invoice Log", "invoice_log")
            .Add("Invoice Notes", "invoice_notes")
            .Add("Link Definitions", "link_definitions")
            .Add("Link Matrix", "link_matrix")
            .Add("Location", "location")
            .Add("MIME Headers", "mime_headers")
            .Add("Platform LPars", "platform_lpars")
            .Add("Product Details", "product_details")
            .Add("Product Notes", "product_notes")
            .Add("Product Platforms", "product_platforms")
            .Add("Product Usage", "product_usage")
            .Add("Product Vendors", "product_vendors")
            .Add("Recharge Associations", "recharge_associations")
            .Add("Saved Reports", "saved_reports")
            .Add("Savings", "savings")
            .Add("Security", "security")
            .Add("Security History", "security_history")
            .Add("Security Locations", "security_locations")
            .Add("Staff Details", "staff_details")
            .Add("Standard Reports", "standard_reports")
            .Add("Sublocations", "sublocations")
            .Add("System-DBJoins", "system_dbjoins")
            .Add("System Parameters", "system_parameters")
            .Add("UBQ Libraries", "ubq_libraries")
            .Add("UBQ Locations", "ubq_locations")
            .Add("UBQ Product Association", "ubq_productassociation")
            .Add("UBQ Products", "ubq_products")
            .Add("UBQ Product Vendors", "ubq_productvendors")
            .Add("UBQ Vendor Association", "ubq_vendorassociation")
            .Add("UBQ Vendors", "ubq_vendors")
            .Add("UBQ Version", "ubq_version")
            .Add("UF Grouping Categories", "uf_groupingcategories")
            .Add("UF Group Allocation", "uf_groupallocation")
            .Add("User Field Values", "user_fieldvalues")
            .Add("User Fields", "user_fields")
            .Add("User Help Docs", "user_helpdocs")
            .Add("User Roles", "user_roles")
            .Add("User Roles - Tabs", "user_roles_tabs")
            .Add("Vendor Addresses", "vendor_addresses")
            .Add("Vendor Categories", "vendor_categories")
            .Add("Vendor Contact Notes", "vendorcontact_notes")
            .Add("Vendor Details", "vendor_details")
            .Add("Vendor Notes", "vendor_notes")
            .Add("Vendor Status", "vendor_status")
            .Add("Version Registry", "version_registry")
            .Add("Version Registry CallOff", "version_registry_calloff")
            .Add("Version History", "version_history")
            .Add("Owner Link", "owner_link") ' is a joinAlias in reports only
        End With
    End Sub

    Public Function GetTable(ByVal TableName As String, Optional ByVal returnWithBrackets As Boolean = False) As String
        TableName = TableName.Replace("[", "")
        TableName = TableName.Replace("]", "")

        If slTables.ContainsKey(TableName) = True Then
            If CurDBVersion <= 18 Then
                If returnWithBrackets Then
                    Return "[" & TableName & "]"
                Else
                    Return TableName
                End If
            Else
                If returnWithBrackets Then
                    Return "[" & slTables(TableName) & "]"
                Else
                    Return slTables(TableName)
                End If

            End If
        Else
            If returnWithBrackets Then
                Return "[" & TableName & "]"
            Else
                Return TableName
            End If
        End If
    End Function

    Public Function GetKeys() As System.Collections.ICollection
        Return slTables.Keys
    End Function

    Public Function TableExists(ByVal TableName As String) As Boolean
        Return slTables.ContainsKey(TableName)
    End Function
End Class
