Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses
Imports Spend_Management.shared.code.GreenLight

Partial Class MenuMain
    Inherits System.Web.UI.Page

    Private mnuSection As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim curUser As CurrentUser = cMisc.GetCurrentUser()
        Dim clsModules As New cModules()
        Dim moduleType As cModule = clsModules.GetModuleByID(curUser.CurrentActiveModule)
        Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
        Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
        Dim recharge_entity As String = "Entity"
        Dim CE_menuId As Integer = 0

        Master.iconSize = fwIconSize.Large
        Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties

        Dim titleStr As String = params.SupplierPrimaryTitle

        If fws.glUseRechargeFunction Then

            recharge_entity = params.RechargeSettings.ReferenceAs
        End If

        mnuSection = Request.QueryString("menusection")

        Select Case mnuSection
            Case "admin"
                Title = "Administrative Settings"
                CE_menuId = 2

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractCategories, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractStatus, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialStatus, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InflatorMetrics, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceFrequencyTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceStatus, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LicenceRenewalTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductCategories, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicenceTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SalesTax, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierCategory, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierStatus, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TermTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TaskTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Units, False) Or
                    (params.EnableRecharge And (curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeClients, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAccountCodes, False))) Then
                    Master.addMenuItem("data_preferences", Master.iconSize, "Base Information", "Configure core components", "MenuMain.aspx?menusection=baseinfo")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, False) Then
                    Master.addMenuItem("user_preferences", Master.iconSize, "Employee Management", "Define and configure users, employees, groups and roles", "MenuMain.aspx?menusection=employee")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, False) Or _
                    (
                        curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, False) AndAlso
                        curUser.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) _
                    ) Then
                    Master.addMenuItem("preferences", Master.iconSize, "System Options", "Application configuration area, including audit log and attachment types", "MenuMain.aspx?menusection=sysoptions")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.NoteCategories, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, False) Or _
                    curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyHelpAndSupportInformation, False) Then
                    Master.addMenuItem("thread", Master.iconSize, "Tailoring", "Configure and tailor generic elements", "MenuMain.aspx?menusection=tailoring")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAccountCodes, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeClients, False) Then
                    If params.EnableRecharge Then
                        Master.addMenuItem("cashier", Master.iconSize, "Recharge Configuration", "Define and manage elements specifically used by the recharge functionality", "MenuMain.aspx?menusection=recharge")
                    End If
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSupplierReassignment, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProductReassignment, False) Then
                    Master.addMenuItem("businessman", Master.iconSize, "Data Management", "Perform data management or housekeeping tasks against the database", "MenuMain.aspx?menusection=management")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, False) Then
                    Master.addMenuItem("objects_exchange", Master.iconSize, "Imports / Exports", "Import and export data", "MenuMain.aspx?menusection=importsexports")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, False) Then
                    Master.addMenuItem("eye", Master.iconSize, "Torch", "Create and modify templates for generating customised mail merge report documents. For example, you can create a customised report document to detail spend analysis from contracted spend or employee travel expenses.", "MenuMain.aspx?menusection=torch", extension:="png")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, false) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GreenLightMenu, False) Then
                    Master.addMenuItem("trafficlight_green", 48, "GreenLight Management", "Create and configure GreenLights on the system.", "MenuMain.aspx?menusection=greenlights")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyHelpAndSupportInformation, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupportTickets, False) Then
                    Master.addMenuItem("help2", 48, "Help &amp; Support Management", "Respond to support tickets, customise the help &amp; support information that is presented to users and administer custom knowledge articles.", "shared/menu.aspx?area=helpandsupport")
                End If
            Case "baseinfo"
                Title = "Base Information"
                CE_menuId = 3

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractCategories, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractStatus, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractTypes, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InflatorMetrics, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TermTypes, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SalesTax, True) Then
                    Master.addMenuItem("document_certificate", Master.iconSize, "Contract Configuration Options", "Configure contract related elements including categories, types and status", "MenuMain.aspx?menusection=contractinfo")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceFrequencyTypes, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceStatus, True) Then
                    Master.addMenuItem("document", Master.iconSize, "Invoice Information", "Configure invoice frequency and status", "MenuMain.aspx?menusection=invoiceinfo")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LicenceRenewalTypes, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicenceTypes, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Units, True) Then
                    Master.addMenuItem("product_blue", Master.iconSize, "Product and Service Configuration", "Configure product/service related information such as categories, details and units", "MenuMain.aspx?menusection=productinfo")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TaskTypes, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, True) Then
                    Master.addMenuItem("earth_location", Master.iconSize, "Generic Information", "Configure addresses, task types and other system wide definitions", "MenuMain.aspx?menusection=sandl")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierCategory, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierStatus, True) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialStatus, True) Then
                    Master.addMenuItem("handshake", Master.iconSize, params.SupplierPrimaryTitle & " Configuration", "Configure " & params.SupplierPrimaryTitle & " related information such as category and status", "MenuMain.aspx?menusection=supplierinfo")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, True) Then
                    Master.addMenuItem("earth_view", 48, "Countries", "The countries listed here will be available for use within the application.", "shared/admin/admincountries.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, True) Then
                    Master.addMenuItem("currency_pound", Master.iconSize, "Currencies", "Define and manage currencies, specify base currency and manage associated exchange rates", cMisc.Path + "/shared/admin/admincurrencies.aspx")
                End If

            Case "employee"
                Title = "Employee Management"
                CE_menuId = 6

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, True) Then
                    Master.addMenuItem("user", Master.iconSize, "Employees", "Define users permitted to access " & moduleType.BrandNameHTML & " and assign their access levels", "shared/admin/selectemployee.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, True) Then
                    Master.addMenuItem("users2", Master.iconSize, "Teams", "Define and manage teams of employees for email notifications or access control", "shared/admin/adminteams.aspx")
                End If
                'Master.addMenuItem("users4", Master.iconSize, "Users", "Define users permitted to access framework and assign their access levels", "UserSetup.aspx")
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, True) Then
                    Master.addMenuItem("users3_preferences", Master.iconSize, "Access Roles", "Define and maintain user access roles to determine different levels of access across the system.", "shared/admin/accessRoles.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, True) Then
                    Master.addMenuItem("data_find", 48, "Audiences", "Add, edit or delete audiences. Users can be grouped into audiences, which are then attached to entities to make them invisible to anyone outside of those audiences.", "shared/admin/adminAudiences.aspx")
                End If

            Case "sysoptions"
                Title = "System Options"
                CE_menuId = 7

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, True) Then
                    Master.addMenuItem("policeman_bobby", Master.iconSize, "Audit Log", "View the audit log of activity, which includes logons, logon failures, updates and deletes", "shared/admin/auditlog.aspx")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, True) Then
                    Master.addMenuItem("key_preferences", 48, "IP Address Filtering", "Specify which IP Addresses should be allowed to access " + moduleType.BrandNameHTML + ".", "shared/admin/adminIPfilters.aspx")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, True) Then
                    Master.addMenuItem("document_ok", Master.iconSize, "Attachment Types", "Define and manage the file attachment types permitted to be uploaded, including the associated MIME content type definition required by the browser", "shared/admin/AttachmentTypes.aspx", "", "png")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, True) Then
                    Master.addMenuItem("document_edit", Master.iconSize, "Custom Attachment Types", "Add and edit custom file attachment types, including the associated MIME content type definition required by the browser", "shared/admin/CustomAttachmentTypes.aspx", "")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, True) AndAlso curUser.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) Then
                    Me.Master.addMenuItem(
                        "application_certificate",
                        Master.iconSize,
                        "Single Sign-on",
                        "Upload encryption certificates and configure SSO attributes and SAML responses. Download the Selenity Limited public certificate.",
                        "shared/admin/sso.aspx",
                        String.Empty,
                        "png")
                End If

            Case "tailoring"
                Title = "Tailoring"
                CE_menuId = 4

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.NoteCategories, True) Then
                    Master.addMenuItem("index_preferences", Master.iconSize, "Note Categories", "Define and manage note category options", "TieredDefinitions.aspx?item=notecategory")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, True) Then
                    Master.addMenuItem("symbol_at", Master.iconSize, "Email Templates", "Verify and manage email templates for use by email notifications", "VerifyTemplates.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, True) Then
                    Master.addMenuItem("data_edit", Master.iconSize, "Userdefined Fields", "Administer and manage user defined fields across the various application areas", "shared/admin/adminuserdefined.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, True) Then
                    Master.addMenuItem("elements_selection", Master.iconSize, "Userdefined Groupings", "Define and maintain the user defined field groupings", "shared/admin/userdefinedFieldGroupings.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, False) Or curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, False) Then
                    Master.addMenuItem("sort_ascending", Master.iconSize, "Userdefined Ordering", "Modify the order of userdefined groups/fields", "shared/admin/userdefinedOrdering.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, True) Then
                    Master.addMenuItem("help2", Master.iconSize, "Tooltips", "Modify the field tooltips that are available", "shared/admin/tooltips/tooltips.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, True) Then
                    Master.addMenuItem("palette", Master.iconSize, "Colours", "Modify the colour scheme of the application", "shared/admin/colours.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, True) Then
                    Master.addMenuItem("gear_preferences", Master.iconSize, "General Options", "Enable or disable various features within " & moduleType.BrandNameHTML & ".", "shared/admin/accountOptions.aspx")
                End If

            Case "reports"
                Title = "Reporting Options"
                Master.addMenuItem("document_pinned", Master.iconSize, "Standard Reports", "Display a list of available standard reports", "StandardReports.aspx")
                'Master.addMenuItem("document_gear", Master.iconSize, "User Reports", "Display a list of available user reports", "ReportList.aspx")
                Master.addMenuItem("document_gear", Master.iconSize, "User Reports", "Display a list of available user reports", "reports/rptlist.aspx")

            Case "contractinfo"
                Title = "Contract Configuration Options"
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractCategories, True) Then
                    Dim conccattitle As String = params.ContractCategoryTitle
                    Master.addMenuItem("document_pinned", Master.iconSize, conccattitle, "Define and manage " & conccattitle & " definitions, such as 'IT', 'services' and 'supplies'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.ContractCategories)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractStatus, True) Then
                    Master.addMenuItem("trafficlight_green", Master.iconSize, "Contract Status", "Configure contract status definitions such as 'live' and 'archived'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.ContractStatus)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractTypes, True) Then
                    Master.addMenuItem("document_edit", Master.iconSize, "Contract Types", "Define and manage contract types, such as 'licence agreement' and 'support contract'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.ContractTypes)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InflatorMetrics, True) Then
                    Master.addMenuItem("symbol_percent", Master.iconSize, "Inflator Metrics", "Define and manage inflator metric definitions such as 'RPI' or 'CEL'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.InflatorMetrics)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SalesTax, True) Then
                    Master.addMenuItem("money", Master.iconSize, "Sales Tax", "Define and manage sales tax definitions such as VAT", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=114")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TermTypes, True) Then
                    Master.addMenuItem("date_time", Master.iconSize, "Term Types", "Define and manage term type definitions such as 'quarterly' or 'annual'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.TermTypes)
                End If

            Case "invoiceinfo"
                Title = "Invoice Information"
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceFrequencyTypes, True) Then
                    Master.addMenuItem("document_time", Master.iconSize, "Invoice Frequencies", "Define and manage invoice frequency definitions", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.InvoiceFrequencyTypes)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InvoiceStatus, True) Then
                    Master.addMenuItem("document_warning", Master.iconSize, "Invoice Status", "Define and manage invoice status definitions", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.InvoiceStatus)
                End If

            Case "productinfo"
                Title = "Product and Service Configuration"
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LicenceRenewalTypes, True) Then
                    Master.addMenuItem("certificate_information", Master.iconSize, "Licence Renewal Types", "Define and manage licence renewal type definitions, such as 'annual' or 'perpetual'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.LicenceRenewalTypes)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductCategories, True) Then
                    Master.addMenuItem("box_preferences", Master.iconSize, "Product / Service Categories", "Define product / service category definitions, such as 'hardware' or 'office supplies'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.ProductCategories)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, True) Then
                    Master.addMenuItem("product_blue", Master.iconSize, "Product / Service Details", "Define products and services available for association with a " & titleStr.ToLower & " and contract.", "ProductDetails.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Units, True) Then
                    Master.addMenuItem("tape_measure", Master.iconSize, "Units", "Define and manage unit definitions", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.Units)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicenceTypes, True) Then
                    Master.addMenuItem("product_green", Master.iconSize, "Product Licence Types", "Define product licence types, such as 'OEM' or 'site licence'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.ProductLicenceTypes)
                End If

            Case "sandl"
                Title = "Generic Information"
                'If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SubAccounts, True) Then
                '    Master.addMenuItem("earth_location", Master.iconSize, "Location Partitions", "Administer the isolated location partitions within the database.", cMisc.path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.SubAccounts)
                'End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, True) Then
                    Master.addMenuItem("office_building", 48, "Organisations", "A list of organisation definitions.", "shared/admin/organisations.aspx")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, True) Then
                    Master.addMenuItem("earth_location", 48, "Addresses", "A list of addresses / geographical site definitions.", "shared/admin/addresses.aspx")
                    'Master.addMenuItem("office-building", Master.iconSize, "Addresses", "Define and manage geographical site definitions", "BaseDefinitions.aspx?item=sites")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TaskTypes, True) Then
                    Master.addMenuItem("toolbox", Master.iconSize, "Task Types", "Define and manage the different types of tasks available", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.TaskTypes)
                End If
            Case "supplierinfo"
                Title = titleStr & " Configuration"
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialStatus, True) Then
                    Master.addMenuItem("chart_area", Master.iconSize, "Financial Status", "Define and manage " & titleStr.ToLower & "'s financial status options, such as 'good' or 'blacklisted'", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.FinancialStatus)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierCategory, True) Then
                    Master.addMenuItem("users3_preferences", Master.iconSize, params.SupplierCatTitle, "Configure specific categories by which to group " & params.SupplierCatTitle.ToLower, cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.SupplierCategory)
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierStatus, True) Then
                    Master.addMenuItem("trafficlight_red", Master.iconSize, titleStr & " Status", "Configure " & titleStr.ToLower & " status option (for example, RAG status)", cMisc.Path + "/contracts/admin/basedefinitions.aspx?bdt=" & SpendManagementElement.SupplierStatus)
                End If

            Case "recharge"
                Title = "Recharge Configuration"
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAccountCodes, True) Then
                    Master.addMenuItem("credit_cards", Master.iconSize, "Account Codes (Recharge)", "Define and manage account codes used against contract recharge functionality", "BaseDefinitions.aspx?item=accountcodes")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeClients, True) Then
                    Master.addMenuItem("user_information", Master.iconSize, "Recharge " & recharge_entity & " Management", "Define and maintain the recharge clients/customers within the database", "BaseDefinitions.aspx?item=rechargeentity")
                End If

            Case "management"
                Title = "Data Management"
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProductReassignment, True) = True Then
                    Master.addMenuItem("objects_transform", Master.iconSize, "Reassign Contract Products", "Transfer contract products from one contract to another", "admin/reassignconprods.aspx")
                End If

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSupplierReassignment, True) = True Then
                    Master.addMenuItem("user_refresh", Master.iconSize, "Reassign Contract " & titleStr, "Transfer a contract to a new " & titleStr.ToLower, "admin/reassigncontractsupplier.aspx")
                End If

                If params.EnableRecharge Then
                    Master.addMenuItem("money", Master.iconSize, "One-off Charges", "Assign one-off charges against a " & titleStr.ToLower & " and " & recharge_entity.ToLower, "admin/oneoffrechargecosts.aspx")
                    Master.addMenuItem("businessman_delete", Master.iconSize, "Impact Analysis", "Evaluate the financial loss of a " & recharge_entity.ToLower & " and action by setting end date to all assigned elements", "admin/recharge_impactanalysis.aspx")
                    Master.addMenuItem("gear_replace", Master.iconSize, "Generate Recharge Payments", "Generate recharge payments for multiple contracts (will include ALL contract products by default)", "admin/recharge_paymentgeneration.aspx")
                End If

            Case "mydetails"
                Title = "My Details"
                Master.addMenuItem("user_information", Master.iconSize, "Change My Details", "Update your basic details, such as name, position or email address. This option can also be used to change your password", "shared/information/mydetails.aspx")
                Master.addMenuItem("calendar_preferences", Master.iconSize, "My Tasks", "View tasks currently assigned either directly to you or to a team, of which you are a member", "shared/tasks/MyTasks.aspx")

            Case "importsexports"
                Title = "Imports / Exports"
                Master.title = Title

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, True) = True Then
                    Master.addMenuItem("import2", Master.iconSize, "Import Data Wizard", "Import data", "shared/importsexports/importdatawizard.aspx")
                End If

            Case "torch"
                Title = "Torch"
                Master.title = Title

                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, True) Then
                    Master.addMenuItem("document_into", Master.iconSize, "Document Configurations", "Define and configure merge tags for merging into a Microsoft Word document", "shared/admin/admindocmergeprojects.aspx")
                End If
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, True) Then
                    Master.addMenuItem("document_pinned", Master.iconSize, "Document Templates", "Manage document templates for use with Torch functionality", "shared/admin/admindoctemplates.aspx")
                End If

            Case "greenlights"
                Title = "GreenLight Management"
                Master.title = Title
                If curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, True) Then
                    Master.addMenuItem("trafficlight_green", 48, "GreenLights", "Create and configure GreenLights for all products.", "shared/admin/custom_entities.aspx")
                End If              

            Case Else
                Title = ""
        End Select

        If CE_menuId > 0 Then
            GetCustomEntityOptions(curUser, CE_menuId)
        End If

        Master.title = Title
        ' TO BE DECIDED
        '        Master.addMenuItem("money2", Master.iconSize, "Cost Codes", "Define two-tiered cost codes for use in invoicing area of Framework", "TieredDefinitions.aspx?item=costcodes")
        'Master.addMenuItem("earth_location", Master.iconSize, "Sublocations", "Define and manage Sublocations options", "BaseDefinitions.aspx?item=sublocations")
    End Sub

    Private Sub GetCustomEntityOptions(ByVal curuser As CurrentUser, menuid As Integer)
        Dim centities As New cCustomEntities(curuser)
        Dim menu_list As List(Of cCustomEntityView) = centities.getViewsByMenuId(menuid)

        If menu_list.Count > 0 Then
            Dim disabledModuleMenuViews As New DisabledModuleMenuViews(curuser.AccountID, curuser.CurrentActiveModule)
            For Each ce As cCustomEntityView In menu_list
                Dim centity As cCustomEntity = centities.getEntityById(ce.entityid)
                If Not disabledModuleMenuViews.IsViewDisabled(menuid, ce.viewid) Then
                    If curuser.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, ce.entityid, ce.viewid, False) Then
                        Dim entityURL As String

                        entityURL = cMisc.Path + "/shared/viewentities.aspx?entityid=" & centity.entityid.ToString & "&viewid=" & ce.viewid.ToString
                        Master.addMenuItem(ce.MenuIcon, fwIconSize.Large, ce.viewname, ce.MenuDescription, True, entityURL)
                    End If
                End If
            Next
        End If
    End Sub
End Class
