Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports System.Configuration
Imports FWBase
Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Data.SqlClient

Public Class FWEmail
    Inherits System.Windows.Forms.Form
    Friend WithEvents prgProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents mnuUpgRepFields As System.Windows.Forms.MenuItem
    Friend WithEvents lblPercent As System.Windows.Forms.Label
    Friend WithEvents mnuLocationRemoval As System.Windows.Forms.MenuItem
    Private curDBVersion As Integer
    Friend WithEvents mnuEncryption As System.Windows.Forms.MenuItem
    Friend WithEvents mnuTools As System.Windows.Forms.MenuItem
    Friend WithEvents mnuApplyLicence As System.Windows.Forms.MenuItem
    Friend WithEvents mnuInvDetailsImport As System.Windows.Forms.MenuItem
    Friend WithEvents mnuUFListItemImport As System.Windows.Forms.MenuItem
    Private bAdminFuncActivated As Boolean = False
    Friend WithEvents mnuChangeSubAcc As System.Windows.Forms.MenuItem
    Private accountList As List(Of Integer)
    Private sConnStr As String
    Friend WithEvents mnuCodesContractCategory As System.Windows.Forms.MenuItem
    Private nAdminId As Integer

    Public Property ActiveDBVersion() As Integer
        Get
            Return curDBVersion
        End Get
        Set(ByVal value As Integer)
            curDBVersion = value
        End Set
    End Property

    Private Property AdminFunctionsEnabled() As Boolean
        Get
            Return bAdminFuncActivated
        End Get
        Set(ByVal value As Boolean)
            bAdminFuncActivated = value
        End Set
    End Property

    Public Property ConnectionString() As String
        Get
            Return sConnStr
        End Get
        Set(ByVal value As String)
            sConnStr = value
        End Set
    End Property

    Public Property AdminId() As Integer
        Get
            Return nAdminId
        End Get
        Set(ByVal value As Integer)
            nAdminId = value
        End Set
    End Property

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents mnuSettings As System.Windows.Forms.MenuItem
    Friend WithEvents cmdExit As System.Windows.Forms.MenuItem
    Friend WithEvents FWMenu As System.Windows.Forms.MainMenu
    Friend WithEvents DBStatus As System.Windows.Forms.StatusBarPanel
    Friend WithEvents Version As System.Windows.Forms.StatusBarPanel
    Friend WithEvents Copyright As System.Windows.Forms.StatusBarPanel
    Friend WithEvents StatusBar As System.Windows.Forms.StatusBar
    Friend WithEvents mnuExecute As System.Windows.Forms.MenuItem
    Friend WithEvents mnuDoReview As System.Windows.Forms.MenuItem
    Friend WithEvents mnuDoOverdue As System.Windows.Forms.MenuItem
    Friend WithEvents mnuDoAuditCleardown As System.Windows.Forms.MenuItem
    Friend WithEvents mnuDoRenewal As System.Windows.Forms.MenuItem
    Friend WithEvents lblMainTitle As System.Windows.Forms.Label
    Friend WithEvents mnuImportVendorDetails As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportProductDetails As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportContractDetails As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportVendorContacts As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImport As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportContactProducts As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportStaffDetails As System.Windows.Forms.MenuItem
    Friend WithEvents mnuTestEmail As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodeMaint As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodesUnits As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodesInflator As System.Windows.Forms.MenuItem
    Friend WithEvents grpLogFile As System.Windows.Forms.GroupBox
    Friend WithEvents txtLogFile As System.Windows.Forms.TextBox
    Friend WithEvents mnuCodesTax As System.Windows.Forms.MenuItem
    Friend WithEvents mnuLogs As System.Windows.Forms.MenuItem
    Friend WithEvents mnuOpenLog As System.Windows.Forms.MenuItem
    Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents mnuConnectionSettings As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEmailSettings As System.Windows.Forms.MenuItem
    Friend WithEvents mnuViewUnproc As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodesInvFreq As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodesConStatus As System.Windows.Forms.MenuItem
    Friend WithEvents mnuSeparator As System.Windows.Forms.MenuItem
    Friend WithEvents mnuUtilities As System.Windows.Forms.MenuItem
    Friend WithEvents mnuReviewEmailSchedules As System.Windows.Forms.MenuItem
    Friend WithEvents mnuFWUpdate As System.Windows.Forms.MenuItem
    Friend WithEvents mnuRechargeEntity As System.Windows.Forms.MenuItem
    Friend WithEvents mnuSites As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodesVendorCategory As System.Windows.Forms.MenuItem
    Friend WithEvents mnuContractProductRecharge As System.Windows.Forms.MenuItem
    Friend WithEvents mnuProductVendorAssoc As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportSavings As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportRAC As System.Windows.Forms.MenuItem
    Friend WithEvents mnuImportSettings As System.Windows.Forms.MenuItem
    Friend WithEvents mnuRechargeAssoc As System.Windows.Forms.MenuItem
    Friend WithEvents mnuConRAUnq As System.Windows.Forms.MenuItem
    Friend WithEvents mnuConRA As System.Windows.Forms.MenuItem
    Friend WithEvents mnuBulkUpdate As System.Windows.Forms.MenuItem
    Friend WithEvents mnuBUConCurrency As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCPCurrency As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents mnuBUConCategory As System.Windows.Forms.MenuItem
    Friend WithEvents mnuConStatus As System.Windows.Forms.MenuItem
    Friend WithEvents mnuEmailTemplateEditor As System.Windows.Forms.MenuItem
    Friend WithEvents mnuProdCategory As System.Windows.Forms.MenuItem
    Friend WithEvents MenuTest As System.Windows.Forms.MenuItem
    Friend WithEvents mnuShowTestIsland As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCodesProductCategory As System.Windows.Forms.MenuItem
    Friend WithEvents mnuBespoke As System.Windows.Forms.MenuItem
    Friend WithEvents mnuRechargeElements As System.Windows.Forms.MenuItem
    Friend WithEvents mnuPortion As System.Windows.Forms.MenuItem
    Friend WithEvents mnuPWPortion As System.Windows.Forms.MenuItem
    Friend WithEvents mnuApportionBy As System.Windows.Forms.MenuItem
    Friend WithEvents mnuPWApportionBy As System.Windows.Forms.MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FWEmail))
        Me.FWMenu = New System.Windows.Forms.MainMenu(Me.components)
        Me.mnuSettings = New System.Windows.Forms.MenuItem()
        Me.mnuConnectionSettings = New System.Windows.Forms.MenuItem()
        Me.mnuEmailSettings = New System.Windows.Forms.MenuItem()
        Me.mnuImportSettings = New System.Windows.Forms.MenuItem()
        Me.mnuChangeSubAcc = New System.Windows.Forms.MenuItem()
        Me.mnuUtilities = New System.Windows.Forms.MenuItem()
        Me.mnuFWUpdate = New System.Windows.Forms.MenuItem()
        Me.mnuUpgRepFields = New System.Windows.Forms.MenuItem()
        Me.mnuLocationRemoval = New System.Windows.Forms.MenuItem()
        Me.mnuExecute = New System.Windows.Forms.MenuItem()
        Me.mnuDoReview = New System.Windows.Forms.MenuItem()
        Me.mnuDoAuditCleardown = New System.Windows.Forms.MenuItem()
        Me.mnuDoOverdue = New System.Windows.Forms.MenuItem()
        Me.mnuDoRenewal = New System.Windows.Forms.MenuItem()
        Me.mnuSeparator = New System.Windows.Forms.MenuItem()
        Me.mnuTestEmail = New System.Windows.Forms.MenuItem()
        Me.mnuImport = New System.Windows.Forms.MenuItem()
        Me.mnuCodeMaint = New System.Windows.Forms.MenuItem()
        Me.mnuCodesContractCategory = New System.Windows.Forms.MenuItem()
        Me.mnuCodesConStatus = New System.Windows.Forms.MenuItem()
        Me.mnuCodesInflator = New System.Windows.Forms.MenuItem()
        Me.mnuCodesInvFreq = New System.Windows.Forms.MenuItem()
        Me.mnuCodesProductCategory = New System.Windows.Forms.MenuItem()
        Me.mnuImportRAC = New System.Windows.Forms.MenuItem()
        Me.mnuRechargeEntity = New System.Windows.Forms.MenuItem()
        Me.mnuCodesTax = New System.Windows.Forms.MenuItem()
        Me.mnuSites = New System.Windows.Forms.MenuItem()
        Me.mnuCodesVendorCategory = New System.Windows.Forms.MenuItem()
        Me.mnuCodesUnits = New System.Windows.Forms.MenuItem()
        Me.mnuImportContractDetails = New System.Windows.Forms.MenuItem()
        Me.mnuImportContactProducts = New System.Windows.Forms.MenuItem()
        Me.mnuContractProductRecharge = New System.Windows.Forms.MenuItem()
        Me.mnuRechargeAssoc = New System.Windows.Forms.MenuItem()
        Me.mnuConRAUnq = New System.Windows.Forms.MenuItem()
        Me.mnuConRA = New System.Windows.Forms.MenuItem()
        Me.mnuBespoke = New System.Windows.Forms.MenuItem()
        Me.mnuImportSavings = New System.Windows.Forms.MenuItem()
        Me.mnuImportStaffDetails = New System.Windows.Forms.MenuItem()
        Me.mnuInvDetailsImport = New System.Windows.Forms.MenuItem()
        Me.mnuImportProductDetails = New System.Windows.Forms.MenuItem()
        Me.mnuProductVendorAssoc = New System.Windows.Forms.MenuItem()
        Me.mnuImportVendorContacts = New System.Windows.Forms.MenuItem()
        Me.mnuImportVendorDetails = New System.Windows.Forms.MenuItem()
        Me.MenuItem1 = New System.Windows.Forms.MenuItem()
        Me.mnuBulkUpdate = New System.Windows.Forms.MenuItem()
        Me.mnuBUConCategory = New System.Windows.Forms.MenuItem()
        Me.mnuBUConCurrency = New System.Windows.Forms.MenuItem()
        Me.mnuCPCurrency = New System.Windows.Forms.MenuItem()
        Me.mnuConStatus = New System.Windows.Forms.MenuItem()
        Me.mnuProdCategory = New System.Windows.Forms.MenuItem()
        Me.mnuRechargeElements = New System.Windows.Forms.MenuItem()
        Me.mnuPortion = New System.Windows.Forms.MenuItem()
        Me.mnuPWPortion = New System.Windows.Forms.MenuItem()
        Me.mnuApportionBy = New System.Windows.Forms.MenuItem()
        Me.mnuPWApportionBy = New System.Windows.Forms.MenuItem()
        Me.mnuUFListItemImport = New System.Windows.Forms.MenuItem()
        Me.mnuLogs = New System.Windows.Forms.MenuItem()
        Me.mnuOpenLog = New System.Windows.Forms.MenuItem()
        Me.mnuViewUnproc = New System.Windows.Forms.MenuItem()
        Me.mnuTools = New System.Windows.Forms.MenuItem()
        Me.mnuApplyLicence = New System.Windows.Forms.MenuItem()
        Me.mnuEmailTemplateEditor = New System.Windows.Forms.MenuItem()
        Me.mnuReviewEmailSchedules = New System.Windows.Forms.MenuItem()
        Me.cmdExit = New System.Windows.Forms.MenuItem()
        Me.MenuTest = New System.Windows.Forms.MenuItem()
        Me.mnuShowTestIsland = New System.Windows.Forms.MenuItem()
        Me.mnuEncryption = New System.Windows.Forms.MenuItem()
        Me.StatusBar = New System.Windows.Forms.StatusBar()
        Me.DBStatus = New System.Windows.Forms.StatusBarPanel()
        Me.Version = New System.Windows.Forms.StatusBarPanel()
        Me.Copyright = New System.Windows.Forms.StatusBarPanel()
        Me.lblMainTitle = New System.Windows.Forms.Label()
        Me.grpLogFile = New System.Windows.Forms.GroupBox()
        Me.lblPercent = New System.Windows.Forms.Label()
        Me.prgProgress = New System.Windows.Forms.ProgressBar()
        Me.txtLogFile = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        CType(Me.DBStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Version, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Copyright, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpLogFile.SuspendLayout()
        Me.SuspendLayout()
        '
        'FWMenu
        '
        Me.FWMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuSettings, Me.mnuUtilities, Me.mnuExecute, Me.mnuImport, Me.mnuLogs, Me.mnuTools, Me.cmdExit, Me.MenuTest, Me.mnuEncryption})
        '
        'mnuSettings
        '
        Me.mnuSettings.Index = 0
        Me.mnuSettings.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuConnectionSettings, Me.mnuEmailSettings, Me.mnuImportSettings, Me.mnuChangeSubAcc})
        Me.mnuSettings.Text = "&Settings"
        '
        'mnuConnectionSettings
        '
        Me.mnuConnectionSettings.Index = 0
        Me.mnuConnectionSettings.Text = "&Connection Options"
        Me.mnuConnectionSettings.Visible = False
        '
        'mnuEmailSettings
        '
        Me.mnuEmailSettings.Index = 1
        Me.mnuEmailSettings.Text = "&Email Settings"
        Me.mnuEmailSettings.Visible = False
        '
        'mnuImportSettings
        '
        Me.mnuImportSettings.Index = 2
        Me.mnuImportSettings.Text = "&Import Settings"
        '
        'mnuChangeSubAcc
        '
        Me.mnuChangeSubAcc.Index = 3
        Me.mnuChangeSubAcc.Text = "Change &SubAccount"
        '
        'mnuUtilities
        '
        Me.mnuUtilities.Index = 1
        Me.mnuUtilities.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFWUpdate, Me.mnuUpgRepFields, Me.mnuLocationRemoval})
        Me.mnuUtilities.Text = "&Utilities"
        Me.mnuUtilities.Visible = False
        '
        'mnuFWUpdate
        '
        Me.mnuFWUpdate.Index = 0
        Me.mnuFWUpdate.Text = "&DB Update (FWUpdate)"
        Me.mnuFWUpdate.Visible = False
        '
        'mnuUpgRepFields
        '
        Me.mnuUpgRepFields.Index = 1
        Me.mnuUpgRepFields.Text = "&Upgrade Report Fields"
        '
        'mnuLocationRemoval
        '
        Me.mnuLocationRemoval.Index = 2
        Me.mnuLocationRemoval.Shortcut = System.Windows.Forms.Shortcut.AltF12
        Me.mnuLocationRemoval.Text = "&Bulk Removal"
        Me.mnuLocationRemoval.Visible = False
        '
        'mnuExecute
        '
        Me.mnuExecute.Index = 2
        Me.mnuExecute.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuDoReview, Me.mnuDoAuditCleardown, Me.mnuDoOverdue, Me.mnuDoRenewal, Me.mnuSeparator, Me.mnuTestEmail})
        Me.mnuExecute.Text = "E&xecute Email"
        Me.mnuExecute.Visible = False
        '
        'mnuDoReview
        '
        Me.mnuDoReview.Index = 0
        Me.mnuDoReview.Text = "&Contract Review"
        '
        'mnuDoAuditCleardown
        '
        Me.mnuDoAuditCleardown.Index = 1
        Me.mnuDoAuditCleardown.Text = "&Audit Cleardown"
        '
        'mnuDoOverdue
        '
        Me.mnuDoOverdue.Index = 2
        Me.mnuDoOverdue.Text = "&Overdue Invoices"
        '
        'mnuDoRenewal
        '
        Me.mnuDoRenewal.Index = 3
        Me.mnuDoRenewal.Text = "&Licence Renewal"
        '
        'mnuSeparator
        '
        Me.mnuSeparator.Index = 4
        Me.mnuSeparator.Text = "-"
        '
        'mnuTestEmail
        '
        Me.mnuTestEmail.Index = 5
        Me.mnuTestEmail.Text = "&Test Email"
        '
        'mnuImport
        '
        Me.mnuImport.Index = 3
        Me.mnuImport.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuCodeMaint, Me.mnuImportContractDetails, Me.mnuImportContactProducts, Me.mnuContractProductRecharge, Me.mnuRechargeAssoc, Me.mnuImportSavings, Me.mnuImportStaffDetails, Me.mnuInvDetailsImport, Me.mnuImportProductDetails, Me.mnuProductVendorAssoc, Me.mnuImportVendorContacts, Me.mnuImportVendorDetails, Me.MenuItem1, Me.mnuBulkUpdate, Me.mnuUFListItemImport})
        Me.mnuImport.Text = "&Import"
        '
        'mnuCodeMaint
        '
        Me.mnuCodeMaint.Index = 0
        Me.mnuCodeMaint.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuCodesContractCategory, Me.mnuCodesConStatus, Me.mnuCodesInflator, Me.mnuCodesInvFreq, Me.mnuCodesProductCategory, Me.mnuImportRAC, Me.mnuRechargeEntity, Me.mnuCodesTax, Me.mnuSites, Me.mnuCodesVendorCategory, Me.mnuCodesUnits})
        Me.mnuCodeMaint.Text = "&Base Information"
        '
        'mnuCodesContractCategory
        '
        Me.mnuCodesContractCategory.Index = 0
        Me.mnuCodesContractCategory.Text = "Co&ntract Category"
        '
        'mnuCodesConStatus
        '
        Me.mnuCodesConStatus.Index = 1
        Me.mnuCodesConStatus.Text = "&Contract Status"
        '
        'mnuCodesInflator
        '
        Me.mnuCodesInflator.Index = 2
        Me.mnuCodesInflator.Text = "In&flator Metrics"
        '
        'mnuCodesInvFreq
        '
        Me.mnuCodesInvFreq.Index = 3
        Me.mnuCodesInvFreq.Text = "&Invoice Frequency"
        '
        'mnuCodesProductCategory
        '
        Me.mnuCodesProductCategory.Index = 4
        Me.mnuCodesProductCategory.Text = "&Product Category"
        '
        'mnuImportRAC
        '
        Me.mnuImportRAC.Index = 5
        Me.mnuImportRAC.Text = "Recharge &Account Codes"
        '
        'mnuRechargeEntity
        '
        Me.mnuRechargeEntity.Index = 6
        Me.mnuRechargeEntity.Text = "&Recharge Clients"
        '
        'mnuCodesTax
        '
        Me.mnuCodesTax.Index = 7
        Me.mnuCodesTax.Text = "Sales &Tax"
        '
        'mnuSites
        '
        Me.mnuSites.Index = 8
        Me.mnuSites.Text = "&Sites"
        '
        'mnuCodesVendorCategory
        '
        Me.mnuCodesVendorCategory.Index = 9
        Me.mnuCodesVendorCategory.Text = "Suppli&er Category"
        '
        'mnuCodesUnits
        '
        Me.mnuCodesUnits.Index = 10
        Me.mnuCodesUnits.Text = "&Units"
        '
        'mnuImportContractDetails
        '
        Me.mnuImportContractDetails.Index = 1
        Me.mnuImportContractDetails.Text = "Contract &Details"
        '
        'mnuImportContactProducts
        '
        Me.mnuImportContactProducts.Index = 2
        Me.mnuImportContactProducts.Text = "C&ontract Products"
        '
        'mnuContractProductRecharge
        '
        Me.mnuContractProductRecharge.Index = 3
        Me.mnuContractProductRecharge.Text = "Co&ntract Product - Recharge"
        '
        'mnuRechargeAssoc
        '
        Me.mnuRechargeAssoc.Index = 4
        Me.mnuRechargeAssoc.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuConRAUnq, Me.mnuConRA, Me.mnuBespoke})
        Me.mnuRechargeAssoc.Text = "Contract - Recharge &Associations"
        '
        'mnuConRAUnq
        '
        Me.mnuConRAUnq.Index = 0
        Me.mnuConRAUnq.Text = "Product Name is &Unique"
        '
        'mnuConRA
        '
        Me.mnuConRA.Index = 1
        Me.mnuConRA.Text = "Product &Name "
        '
        'mnuBespoke
        '
        Me.mnuBespoke.Index = 2
        Me.mnuBespoke.Text = "Product Name+&Other (Bespoke)"
        '
        'mnuImportSavings
        '
        Me.mnuImportSavings.Index = 5
        Me.mnuImportSavings.Text = "Contract Sa&vings"
        '
        'mnuImportStaffDetails
        '
        Me.mnuImportStaffDetails.Index = 6
        Me.mnuImportStaffDetails.Text = "&Employee Details"
        '
        'mnuInvDetailsImport
        '
        Me.mnuInvDetailsImport.Index = 7
        Me.mnuInvDetailsImport.Text = "Invoice Details"
        '
        'mnuImportProductDetails
        '
        Me.mnuImportProductDetails.Index = 8
        Me.mnuImportProductDetails.Text = "&Product Details"
        '
        'mnuProductVendorAssoc
        '
        Me.mnuProductVendorAssoc.Index = 9
        Me.mnuProductVendorAssoc.Text = "Product - S&upplier Association"
        '
        'mnuImportVendorContacts
        '
        Me.mnuImportVendorContacts.Index = 10
        Me.mnuImportVendorContacts.Text = "Supp&lier Contacts"
        '
        'mnuImportVendorDetails
        '
        Me.mnuImportVendorDetails.Index = 11
        Me.mnuImportVendorDetails.Text = "Supplie&r Details"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 12
        Me.MenuItem1.Text = "-"
        '
        'mnuBulkUpdate
        '
        Me.mnuBulkUpdate.Index = 13
        Me.mnuBulkUpdate.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuBUConCategory, Me.mnuBUConCurrency, Me.mnuCPCurrency, Me.mnuConStatus, Me.mnuProdCategory, Me.mnuRechargeElements})
        Me.mnuBulkUpdate.Text = "Bul&k Update"
        '
        'mnuBUConCategory
        '
        Me.mnuBUConCategory.Index = 0
        Me.mnuBUConCategory.Text = "&Contract Category"
        '
        'mnuBUConCurrency
        '
        Me.mnuBUConCurrency.Index = 1
        Me.mnuBUConCurrency.Text = "Contract C&urrency"
        '
        'mnuCPCurrency
        '
        Me.mnuCPCurrency.Index = 2
        Me.mnuCPCurrency.Text = "Con&tract Product Currency"
        '
        'mnuConStatus
        '
        Me.mnuConStatus.Index = 3
        Me.mnuConStatus.Text = "Co&ntract Status"
        '
        'mnuProdCategory
        '
        Me.mnuProdCategory.Index = 4
        Me.mnuProdCategory.Text = "&Product Category"
        '
        'mnuRechargeElements
        '
        Me.mnuRechargeElements.Enabled = False
        Me.mnuRechargeElements.Index = 5
        Me.mnuRechargeElements.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuPortion, Me.mnuPWPortion, Me.mnuApportionBy, Me.mnuPWApportionBy})
        Me.mnuRechargeElements.Text = "Recharge Elements"
        '
        'mnuPortion
        '
        Me.mnuPortion.Index = 0
        Me.mnuPortion.Text = "Portion"
        '
        'mnuPWPortion
        '
        Me.mnuPWPortion.Index = 1
        Me.mnuPWPortion.Text = "PW Portion"
        '
        'mnuApportionBy
        '
        Me.mnuApportionBy.Index = 2
        Me.mnuApportionBy.Text = "Apportion By"
        '
        'mnuPWApportionBy
        '
        Me.mnuPWApportionBy.Index = 3
        Me.mnuPWApportionBy.Text = "PW Apportion By"
        '
        'mnuUFListItemImport
        '
        Me.mnuUFListItemImport.Index = 14
        Me.mnuUFListItemImport.Text = "User Field &List Items"
        '
        'mnuLogs
        '
        Me.mnuLogs.Index = 4
        Me.mnuLogs.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuOpenLog, Me.mnuViewUnproc})
        Me.mnuLogs.Text = "&Logs"
        '
        'mnuOpenLog
        '
        Me.mnuOpenLog.Index = 0
        Me.mnuOpenLog.Text = "&Open Log"
        '
        'mnuViewUnproc
        '
        Me.mnuViewUnproc.Index = 1
        Me.mnuViewUnproc.Text = "&View Unprocessed"
        '
        'mnuTools
        '
        Me.mnuTools.Index = 5
        Me.mnuTools.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuApplyLicence, Me.mnuEmailTemplateEditor, Me.mnuReviewEmailSchedules})
        Me.mnuTools.Text = "&Tools"
        '
        'mnuApplyLicence
        '
        Me.mnuApplyLicence.Index = 0
        Me.mnuApplyLicence.Text = "&Apply Licence"
        '
        'mnuEmailTemplateEditor
        '
        Me.mnuEmailTemplateEditor.Index = 1
        Me.mnuEmailTemplateEditor.Text = "&Email Template Editor"
        '
        'mnuReviewEmailSchedules
        '
        Me.mnuReviewEmailSchedules.Index = 2
        Me.mnuReviewEmailSchedules.Text = "&Review Email Schedules"
        '
        'cmdExit
        '
        Me.cmdExit.Index = 6
        Me.cmdExit.Text = "&Exit"
        '
        'MenuTest
        '
        Me.MenuTest.Index = 7
        Me.MenuTest.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuShowTestIsland})
        Me.MenuTest.Text = "Test Island"
        Me.MenuTest.Visible = False
        '
        'mnuShowTestIsland
        '
        Me.mnuShowTestIsland.Index = 0
        Me.mnuShowTestIsland.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftT
        Me.mnuShowTestIsland.Text = "Show"
        '
        'mnuEncryption
        '
        Me.mnuEncryption.Index = 8
        Me.mnuEncryption.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftF12
        Me.mnuEncryption.Text = "Encryption"
        Me.mnuEncryption.Visible = False
        '
        'StatusBar
        '
        Me.StatusBar.Location = New System.Drawing.Point(0, 321)
        Me.StatusBar.Name = "StatusBar"
        Me.StatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.DBStatus, Me.Version, Me.Copyright})
        Me.StatusBar.ShowPanels = True
        Me.StatusBar.Size = New System.Drawing.Size(792, 24)
        Me.StatusBar.TabIndex = 0
        '
        'DBStatus
        '
        Me.DBStatus.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.DBStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.DBStatus.Name = "DBStatus"
        Me.DBStatus.Width = 10
        '
        'Version
        '
        Me.Version.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.Version.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.Version.Name = "Version"
        Me.Version.Width = 10
        '
        'Copyright
        '
        Me.Copyright.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.Copyright.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
        Me.Copyright.Name = "Copyright"
        Me.Copyright.Text = "Software (Europe) Ltd. 2010"
        Me.Copyright.Width = 755
        '
        'lblMainTitle
        '
        Me.lblMainTitle.Font = New System.Drawing.Font("Arial", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMainTitle.Location = New System.Drawing.Point(8, 8)
        Me.lblMainTitle.Name = "lblMainTitle"
        Me.lblMainTitle.Size = New System.Drawing.Size(776, 32)
        Me.lblMainTitle.TabIndex = 0
        Me.lblMainTitle.Text = "Framework Application Utilities"
        Me.lblMainTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpLogFile
        '
        Me.grpLogFile.Controls.Add(Me.lblPercent)
        Me.grpLogFile.Controls.Add(Me.prgProgress)
        Me.grpLogFile.Controls.Add(Me.txtLogFile)
        Me.grpLogFile.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpLogFile.Location = New System.Drawing.Point(0, 0)
        Me.grpLogFile.Name = "grpLogFile"
        Me.grpLogFile.Size = New System.Drawing.Size(792, 321)
        Me.grpLogFile.TabIndex = 1
        Me.grpLogFile.TabStop = False
        Me.grpLogFile.Text = "Log File"
        '
        'lblPercent
        '
        Me.lblPercent.AutoSize = True
        Me.lblPercent.Location = New System.Drawing.Point(264, 296)
        Me.lblPercent.Name = "lblPercent"
        Me.lblPercent.Size = New System.Drawing.Size(108, 13)
        Me.lblPercent.TabIndex = 2
        Me.lblPercent.Text = "0 Records processed"
        Me.lblPercent.Visible = False
        '
        'prgProgress
        '
        Me.prgProgress.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.prgProgress.Location = New System.Drawing.Point(6, 290)
        Me.prgProgress.Name = "prgProgress"
        Me.prgProgress.Size = New System.Drawing.Size(252, 25)
        Me.prgProgress.Step = 5
        Me.prgProgress.TabIndex = 1
        Me.prgProgress.Visible = False
        '
        'txtLogFile
        '
        Me.txtLogFile.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtLogFile.Location = New System.Drawing.Point(3, 16)
        Me.txtLogFile.Multiline = True
        Me.txtLogFile.Name = "txtLogFile"
        Me.txtLogFile.ReadOnly = True
        Me.txtLogFile.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtLogFile.Size = New System.Drawing.Size(786, 302)
        Me.txtLogFile.TabIndex = 0
        '
        'FWEmail
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(792, 345)
        Me.Controls.Add(Me.grpLogFile)
        Me.Controls.Add(Me.lblMainTitle)
        Me.Controls.Add(Me.StatusBar)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Menu = Me.FWMenu
        Me.Name = "FWEmail"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Framework Utility"
        CType(Me.DBStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Version, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Copyright, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpLogFile.ResumeLayout(False)
        Me.grpLogFile.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExit.Click
        If doClose() = System.Windows.Forms.DialogResult.Yes Then
            Application.Exit()
        End If
    End Sub

    Private Sub FWEmail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        initMetabase()

        StatusBar.Panels(1).Text = "Version: " & Application.ProductVersion
        'CancelButton = cmdExit
        Dim FWEmailer As New FWEmailServer.FWEmailServer
        ActiveDBVersion = ConfigurationManager.AppSettings("ActiveDBVersion")
        Dim accs As New cUtilAccounts
        clAccounts = accs

        ' get the Framework settings from Framework.ini - if present
        Try
            Dim activateMenu As Boolean = True
            If AccountId = 0 Then
                nActiveSubAccount = Nothing
                Dim acc As New dlgActiveConnection
                acc.Tag = -1
                While acc.Tag = -1
                    If acc.ShowDialog() = DialogResult.OK Then
                        AccountId = acc.Tag

                        If AccountId >= 0 Then
                            ConnectionString = cUtilAccounts.getConnectionString(AccountId)

                            ' if connection string override exists, do so
                            If ConfigurationManager.AppSettings("ConnectionStringOverride") = "1" Then
                                Dim ipMatch, ipReplace As String
                                ipMatch = ConfigurationManager.AppSettings("IPMatch")
                                ipReplace = ConfigurationManager.AppSettings("IPPatch")

                                ConnectionString = ConnectionString.Replace(ipMatch, ipReplace)
                            End If

                            clSubAccounts = New cUtilSubAccounts
                            If SubAccounts.Count > 1 Then
                                Dim dlg As New dlgFixedSelection()
                                dlg.Tag = "accountsSubAccounts.subAccountId"
                                While dlg.Tag.GetType().ToString() = "System.String"
                                    If dlg.ShowDialog(Me) = DialogResult.OK Then
                                        If dlg.Tag Is Nothing Then
                                            dlg.Tag = "accountsSubAccounts.subAccountId"
                                        Else
                                            nActiveSubAccount = SubAccounts.getSubAccountById(CInt(dlg.Tag))
                                        End If
                                    End If
                                End While
                            Else
                                nActiveSubAccount = SubAccounts.GetFirstSubAccount
                            End If
                            clActiveAccount = accs.getAccountById(AccountId)

                            ' get ID of admin user for use by Imports
                            Dim sql As String = "select top 1 employeeid from employees where username like 'admin%'"
                            Dim db As New DBConnection(ConnectionString)
                            Try
                                AdminId = db.getIntSum(sql)
                            Finally
                                sql = "select top 1 employeeid from employees"
                                AdminId = db.getIntSum(sql)
                            End Try

                        Else
                            activateMenu = False
                            AccountId = 0
                        End If
                    Else
                        Application.Exit()
                        Exit Sub
                    End If
                End While
            End If

            mnuImportSettings.Enabled = activateMenu
            mnuImport.Enabled = activateMenu
            mnuLogs.Enabled = activateMenu
            mnuUtilities.Enabled = activateMenu
            mnuTools.Enabled = activateMenu

            If AccountId > 0 Then
                StatusBar.Panels(0).Text = "Active Connection: " + ActiveAccount.dbserver.ToUpper() + " : " + ActiveAccount.companyname + " (SubAccount: " + ActiveSubAccount.Description + ")"
            Else
                StatusBar.Panels(0).Text = "No connection specified"
            End If

        Catch ex As Exception
            MsgBox("An error occurred attempting to start the application." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
            Application.Exit()
        End Try
    End Sub

    Private Function checkDBVersion(ByVal ServerVersion As Integer) As Boolean
        ActiveDBVersion = ServerVersion

        Return True
        ' CHECK NOT COMPATIBLE WITH SM Version
        'If ServerVersion = 23 Then ' DBVERSION
        '    'Version is ok
        '    mnuImportSettings.Enabled = True
        '    mnuImport.Enabled = True
        '    mnuLogs.Enabled = True
        '    mnuUtilities.Enabled = True
        '    mnuTools.Enabled = True
        '    Return True
        'End If

        'If ServerVersion = -1 Then
        '    mnuImportSettings.Enabled = False
        '    mnuImport.Enabled = False
        '    mnuLogs.Enabled = False
        '    mnuUtilities.Enabled = False
        '    mnuTools.Enabled = False
        '    Return False
        'End If

        'If ServerVersion > 23 Then
        '    'Utility is older than the current database implementation
        '    mnuImportSettings.Enabled = False
        '    mnuImport.Enabled = False
        '    mnuLogs.Enabled = False
        '    mnuUtilities.Enabled = False
        '    mnuTools.Enabled = False
        '    MsgBox("The FWUtilities build is designed for a database version that is older than the current implementation. Please contact Software (Europe) Ltd for the new version of the FWUtilities program.", MsgBoxStyle.OkOnly, "FWUtilities Database Incompatibility")
        '    Return False
        'End If

        'If ServerVersion < 23 Then
        '    'Utility is newer than the latest framework version
        '    mnuImportSettings.Enabled = False
        '    mnuImport.Enabled = False
        '    mnuLogs.Enabled = False
        '    mnuReviewEmailSchedules.Enabled = False
        '    mnuEmailTemplateEditor.Enabled = False
        '    mnuApplyLicence.Enabled = False
        '    mnuTools.Enabled = False
        '    MsgBox("The FWUtilities build is designed for a database version that is newer than the current implementation. Please use the framework update utility to implement the new database version.", MsgBoxStyle.OkOnly, "FWUtilities Database Incompatibility")
        '    Return False
        'End If
    End Function

    Private Sub mnuConnectionSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuConnectionSettings.Click
        Dim dbs As New dlgActiveConnection

        dbs.ShowDialog()

        ' refresh the fwsettings
        'fws = LoadXMLSettings() 'SetApplicationProperties()
        ValidateConnection()

        dbs = Nothing
    End Sub

    Private Sub Log(ByVal Msg As String)
        ' This routine time-stamps the message and writes it to the log file
        Dim logfile As StreamWriter
        Dim filename As String

        Try
            filename = Application.StartupPath & "\logs\FWEmail" & Trim(Format(Today(), "ddMMyyyy")) & ".log"
            logfile = New StreamWriter(filename, True)

            logfile.WriteLine(Msg)

            logfile.Close()

            LastLogFile = filename

        Catch ex As Exception
            MsgBox("Error writing to FWUpdate.log file", MsgBoxStyle.Information, "FWUpdate Error")

        End Try

    End Sub

    Private Sub mnuDoReview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDoReview.Click
        Dim dlg As New ExecuteNotify

        dlg.Tag = "review"
        dlg.ShowDialog()

        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuDoAuditCleardown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDoAuditCleardown.Click
        Dim dlg As New ExecuteNotify

        dlg.Tag = "audit"
        dlg.ShowDialog()

        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuDoOverdue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDoOverdue.Click
        Dim dlg As New ExecuteNotify

        dlg.Tag = "overdue"
        dlg.ShowDialog()

        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuDoRenewal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDoRenewal.Click
        Dim dlg As New ExecuteNotify

        dlg.Tag = "licence"
        dlg.ShowDialog()

        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportContractDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportContractDetails.Click
        Dim dlg As New FWImport
        dlg.Tag = cUtility.ImportType.ContractDetails
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportContactProducts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportContactProducts.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.ContractProducts
        Dim res As DialogResult = dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()

        If res = Windows.Forms.DialogResult.OK Then
            ' temporary TRY until dbo.UpdateCPAnnualCost proc updated on live (GreenLight - Iteration 3)
            Try
                ' update the annual contract value for all contract products
                Dim db As New DBConnection(ConnectionString)
                'fws = LoadXMLSettings() 'SetApplicationProperties()

                db.ExecuteSQL("EXECUTE dbo.UpdateCPAnnualCost 0," & IIf(ActiveSubAccount.SubAccountProperties.AutoUpdateAnnualContractValue, "1", "0"))

            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub mnuImportProductDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportProductDetails.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.ProductDetails
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportStaffDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportStaffDetails.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.StaffDetails
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportVendorContacts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportVendorContacts.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.VendorContacts
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportVendorDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportVendorDetails.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.VendorDetails
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuTestEmail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuTestEmail.Click
        Dim dlg As New TestEmail

        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuCodesInflator_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesInflator.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesInflator
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuCodesUnits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesUnits.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesUnits
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuCodesTax_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesTax.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesSalesTax
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub LoadLastLog()
        If Not LastLogFile Is Nothing Then
            If LastLogFile <> "" Then
                Dim r As New StreamReader(LastLogFile)

                grpLogFile.Text = "Log File : " & LastLogFile

                If r.Peek > 0 Then
                    txtLogFile.Text = r.ReadToEnd
                    txtLogFile.ScrollToCaret()
                End If

                r.Close()
                r = Nothing
            End If
        End If
    End Sub

    Private Sub mnuOpenLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuOpenLog.Click
        Dim LogFileName As String
        Dim LogFile As StreamReader

        Try
            OpenFileDialog.Title = "Open Log File"
            OpenFileDialog.Filter = "log files (*.log)|*.log|All files (*.*)|*.*"
            OpenFileDialog.InitialDirectory = Application.StartupPath & "\logs"

            If OpenFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                LogFileName = OpenFileDialog.FileName
                LogFile = New StreamReader(LogFileName)

                grpLogFile.Text = LogFileName

                If LogFile.Peek > 0 Then
                    txtLogFile.Text = LogFile.ReadToEnd
                Else
                    txtLogFile.Text = "Log File selected is empty"
                End If

                LogFile.Close()
                LogFile = Nothing

            End If

        Catch ex As Exception
            MsgBox("Error Opening Log File." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    Private Sub mnuEmailSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEmailSettings.Click
        Dim dlgES As New EmailSettings

        dlgES.Tag = 10
        dlgES.ShowDialog()

        dlgES.Dispose()
        dlgES = Nothing

        'fws = LoadXMLSettings() ' SetApplicationProperties()


        mnuEmailSettings.Enabled = False
    End Sub

    Private Sub mnuViewUnproc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuViewUnproc.Click
        Dim unpFileName As String
        Dim unpFile As StreamReader

        Try
            OpenFileDialog.Title = "Open Unprocessed Import File"
            OpenFileDialog.Filter = "log files (*.unp)|*.unp|All files (*.*)|*.*"
            'OpenFileDialog.InitialDirectory = Application.StartupPath & "\logs"

            If OpenFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                unpFileName = OpenFileDialog.FileName
                unpFile = New StreamReader(unpFileName)

                grpLogFile.Text = unpFileName

                If unpFile.Peek > 0 Then
                    txtLogFile.Text = unpFile.ReadToEnd
                Else
                    txtLogFile.Text = "Log File selected is empty"
                End If

                unpFile.Close()
                unpFile = Nothing

            End If

        Catch ex As Exception
            MsgBox("Error Opening Unprocessed File." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    Private Sub mnuCodesInvFreq_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesInvFreq.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesInvFrequency
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuCodesConStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesConStatus.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesContractStatus
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuReviewEmailSchedules_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuReviewEmailSchedules.Click
        Dim dlg As New EmailSchedules

        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
    End Sub

    Private Sub mnuRechargeEntity_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuRechargeEntity.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesRechargeEntity
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuSites_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSites.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesSites
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuCodesVendorCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesVendorCategory.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesVendorCategory
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuContractProductRecharge_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuContractProductRecharge.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.ContractProductRecharge
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuProductVendorAssoc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuProductVendorAssoc.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.ProductVendorAssoc
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    'Private Sub mnuFWUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFWUpdate.Click
    '    Dim dlg As New FWUpdate

    '    dlg.ShowDialog()
    '    dlg.Dispose()
    '    dlg = Nothing
    '    LoadLastLog()
    'End Sub

    Private Sub mnuImportSavings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportSavings.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.ContractSavings
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportRAC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportRAC.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesAccountCodes
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuImportSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuImportSettings.Click
        Dim dlg As New ImportSettings

        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuConRA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuConRA.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.RechargeAssociations
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuConRAUnq_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuConRAUnq.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.RechargeAssociationsUnq
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuBUConCurrency_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuBUConCurrency.Click
        Try
            Dim loc_dlg As New dlgFixedSelection
            Dim dlgRes As DialogResult

            Dim currencyId, locationId As Integer
            Dim tables As New cTables(ActiveDBVersion)

            loc_dlg.Tag = "accountsSubAccounts.subAccountId"
            dlgRes = loc_dlg.ShowDialog()

            If dlgRes = System.Windows.Forms.DialogResult.OK Then
                If IsNumeric(loc_dlg.Tag) = True Then
                    locationId = loc_dlg.Tag

                    Dim dlg As New dlgFixedSelection
                    dlg.Tag = "[currencyView].[CurrencyId]"
                    dlg.ActiveLocation = locationId

                    dlgRes = dlg.ShowDialog()

                    If dlgRes = System.Windows.Forms.DialogResult.OK Then
                        If IsNumeric(dlg.Tag) = True Then
                            currencyId = dlg.Tag

                            Dim sql As String
                            Dim msgRes As MsgBoxResult
                            msgRes = MsgBox("Apply only to contracts where currency has NOT been applied?", MsgBoxStyle.YesNo, "FWUtilties")
                            If msgRes = MsgBoxResult.Yes Then
                                sql = "UPDATE [contract_details] SET [ContractCurrency] = " & currencyId.ToString & " WHERE ([ContractCurrency] IS NULL OR [ContractCurrency] <= 0) AND [subAccountId] = " & locationId.ToString
                            Else
                                msgRes = MsgBox("Apply to ALL contracts (overwriting any existing selection)?", MsgBoxStyle.YesNo, "FWUtilities")
                                If msgRes = MsgBoxResult.Yes Then
                                    sql = "UPDATE [contract_details] SET [ContractCurrency] = " & currencyId.ToString & " WHERE [subAccountId] = " & locationId.ToString
                                Else
                                    sql = ""
                                End If
                            End If

                            If sql <> "" Then
                                ' two valid ids, so perform global update for that location
                                Dim db As New DBConnection(ConnectionString)

                                'fws = LoadXMLSettings() 'SetApplicationProperties()

                                db.ExecuteSQL(sql)

                                MsgBox("Bulk update of Contract Currency completed successfully.", MsgBoxStyle.OkOnly, "FWUtilties")
                            Else
                                MsgBox("Bulk update abandoned. No changes made.", MsgBoxStyle.Information, "FWUtilities")
                            End If
                        Else
                            MsgBox("Invalid result returned for Currency Selection", MsgBoxStyle.Critical, "FWUtilities Error")
                        End If
                    End If
                Else
                    MsgBox("Invalid result returned for Location Selection", MsgBoxStyle.Critical, "FWUtilities Error")
                End If
            End If

        Catch ex As Exception
            MsgBox("An unexpected error has occurred." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    Private Sub mnuCPCurrency_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCPCurrency.Click
        Try
            Dim dlgRes As DialogResult
            Dim loc_dlg As New dlgFixedSelection
            Dim currencyId, locationId As Integer
            Dim tables As New cTables(ActiveDBVersion)

            loc_dlg.Tag = "accountsSubAccounts.subAccountId"
            dlgRes = loc_dlg.ShowDialog()

            If dlgRes = System.Windows.Forms.DialogResult.OK Then
                If IsNumeric(loc_dlg.Tag) = True Then
                    locationId = loc_dlg.Tag

                    Dim dlg As New dlgFixedSelection
                    dlg.Tag = "[currencyView].[CurrencyId]"
                    dlg.ActiveLocation = locationId

                    dlgRes = dlg.ShowDialog()

                    If dlgRes = System.Windows.Forms.DialogResult.OK Then
                        If IsNumeric(dlg.Tag) = True Then
                            currencyId = dlg.Tag

                            Dim sql As String
                            Dim msgRes As MsgBoxResult
                            msgRes = MsgBox("Apply only to contract products where currency has NOT been applied?", MsgBoxStyle.YesNo, "FWUtilties")
                            If msgRes = MsgBoxResult.Yes Then
                                sql = "UPDATE [contract_productdetails] SET [CurrencyId] = " & currencyId.ToString
                                sql += " WHERE [ContractId] IN (SELECT [ContractId] FROM [contract_details] WHERE [subAccountId] = " & locationId.ToString & ") AND ([CurrencyId] IS NULL OR [CurrencyId] <= 0) "
                            Else
                                msgRes = MsgBox("Apply to ALL contract products (overwriting any existing selection)?", MsgBoxStyle.YesNo, "FWUtilities")
                                If msgRes = MsgBoxResult.Yes Then
                                    sql = "UPDATE [contract_productdetails] SET [CurrencyId] = " & currencyId.ToString
                                    sql += " WHERE [ContractId] IN (SELECT [ContractId] FROM [contract_details] WHERE [subAccountId] = " & locationId.ToString & ")"
                                Else
                                    sql = ""
                                End If
                            End If

                            ' two valid ids, so perform global update for that location
                            If sql <> "" Then
                                Dim db As New DBConnection(ConnectionString)
                                'fws = LoadXMLSettings() ' SetApplicationProperties()

                                db.ExecuteSQL(sql)

                                MsgBox("Bulk update of Contract Currency completed successfully.", MsgBoxStyle.OkOnly, "FWUtilties")
                            Else
                                MsgBox("Bulk update abandoned. No changes made.", MsgBoxStyle.Information, "FWUtilities")
                            End If
                        Else
                            MsgBox("Invalid result returned for Currency Selection", MsgBoxStyle.Critical, "FWUtilities Error")
                        End If
                    End If
                Else
                    MsgBox("Invalid result returned for Location Selection", MsgBoxStyle.Critical, "FWUtilities Error")
                End If
            End If

        Catch ex As Exception
            MsgBox("An unexpected error has occurred." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    Private Sub mnuBUConCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuBUConCategory.Click
        Try
            Dim res As MsgBoxResult
            Dim db As New DBConnection(ConnectionString)
            Dim sql As String
            Dim fixedLookup As dlgFixedSelection
            Dim dlgRes As DialogResult
            Dim UpdateLocation, UpdateCategory As Integer
            Dim tables As New cTables(ActiveDBVersion)

            'fws = LoadXMLSettings() ' SetApplicationProperties()

            fixedLookup = New dlgFixedSelection

            fixedLookup.Tag = "accountsSubAccounts.subAccountId"
            dlgRes = fixedLookup.ShowDialog()
            If dlgRes = System.Windows.Forms.DialogResult.OK Then
                UpdateLocation = fixedLookup.Tag
                fixedLookup.Dispose()

                fixedLookup = New dlgFixedSelection
                fixedLookup.Tag = "[codes_contractcategory]"
                fixedLookup.ActiveLocation = UpdateLocation
                dlgRes = fixedLookup.ShowDialog()

                If dlgRes = System.Windows.Forms.DialogResult.OK Then
                    UpdateCategory = fixedLookup.Tag
                    If UpdateCategory > 0 Then
                        res = MsgBox("Update only those currently without a category set?", MsgBoxStyle.YesNo, "FWUtilities")

                        If res = MsgBoxResult.Yes Then
                            sql = "UPDATE [contract_details] SET [CategoryId] = " & UpdateCategory.ToString & " WHERE [subAccountId] = " & UpdateLocation.ToString & " AND ([CategoryId] IS NULL OR [CategoryId] = 0)"
                            db.ExecuteSQL(sql)
                        Else
                            Dim res2 As MsgBoxResult

                            res2 = MsgBox("Are you sure you want to set the category for ALL contracts" & vbNewLine & "without exception in the selected location?", MsgBoxStyle.YesNo, "FWUtilities")
                            If res2 = MsgBoxResult.Yes Then
                                sql = "UPDATE [contract_details] SET [CategoryId] = " & UpdateCategory.ToString & " WHERE [subAccountId] = " & UpdateLocation.ToString
                                db.ExecuteSQL(sql)
                            Else
                                MsgBox("Failed to clarify scope of update." & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                            End If
                        End If
                        MsgBox("Bulk update of Contract Category completed successfully.", MsgBoxStyle.OkOnly, "FWUtilties")
                    Else
                        MsgBox("Failed to select a category" & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                    End If
                End If
            End If

            fixedLookup.Dispose()
            fixedLookup = Nothing

        Catch ex As Exception
            MsgBox("An unexpected error has occurred." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As System.ComponentModel.CancelEventArgs)
        If doClose() <> System.Windows.Forms.DialogResult.Yes Then
            e.Cancel = True
        End If
        deinitMetabase()
    End Sub

    Private Function doClose() As DialogResult
        doClose = MsgBox("Confirm Exit of FWUtilities?", MsgBoxStyle.YesNo, "Exit FWUtilities")
    End Function

    Private Sub mnuConStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuConStatus.Click
        Try
            Dim res As MsgBoxResult
            Dim db As New DBConnection(ConnectionString)
            Dim sql As String
            Dim fixedLookup As dlgFixedSelection
            Dim dlgRes As DialogResult
            Dim UpdateLocation, UpdateStatus As Integer
            Dim ArchiveYN As String

            'fws = LoadXMLSettings() 'SetApplicationProperties()

            fixedLookup = New dlgFixedSelection

            fixedLookup.Tag = "accountsSubAccounts.subAccountId"
            dlgRes = fixedLookup.ShowDialog()
            If dlgRes = System.Windows.Forms.DialogResult.OK Then
                UpdateLocation = fixedLookup.Tag
                fixedLookup.Dispose()

                fixedLookup = New dlgFixedSelection
                fixedLookup.Tag = "[codes_contractstatus]"
                fixedLookup.ActiveLocation = UpdateLocation
                dlgRes = fixedLookup.ShowDialog()

                If dlgRes = System.Windows.Forms.DialogResult.OK Then
                    UpdateStatus = fixedLookup.Tag
                    sql = "select IsArchive from codes_contractstatus where statusId = @id"
                    db.sqlexecute.Parameters.AddWithValue("@id", UpdateStatus)
                    Dim archiveVal As Integer = db.getcount(sql)

                    If archiveVal = "1" Then
                        ArchiveYN = "Y"
                    Else
                        ArchiveYN = "N"
                    End If

                    db.sqlexecute.Parameters.Clear()

                    If UpdateStatus > 0 Then
                        res = MsgBox("Update only those currently without a status set?", MsgBoxStyle.YesNo, "FWUtilities")

                        If res = MsgBoxResult.Yes Then
                            sql = "UPDATE [contract_details] SET [ContractStatusId] = " & UpdateStatus.ToString.Trim & ", [Archived] = '" & ArchiveYN & "' WHERE [subAccountId] = " & UpdateLocation.ToString & " AND [ContractStatusId] IS NULL OR [ContractStatusId] = 0"
                            db.ExecuteSQL(sql)
                        Else
                            Dim res2 As MsgBoxResult

                            res2 = MsgBox("Are you sure you want to set the status for ALL contracts" & vbNewLine & "without exception in the selected location?", MsgBoxStyle.YesNo, "FWUtilities")
                            If res2 = MsgBoxResult.Yes Then
                                sql = "UPDATE [contract_details] SET [ContractStatusId] = " & UpdateStatus.ToString.Trim & ", [Archived] = '" & ArchiveYN & "' WHERE [subAccountId] = " & UpdateLocation.ToString
                                db.ExecuteSQL(sql)
                            Else
                                MsgBox("Failed to clarify scope of update." & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                            End If
                        End If

                        MsgBox("Bulk update of Contract Status completed successfully.", MsgBoxStyle.OkOnly, "FWUtilties")
                    Else
                        MsgBox("Failed to select a contract status" & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                    End If
                End If
            End If

            fixedLookup.Dispose()
            fixedLookup = Nothing

        Catch ex As Exception
            MsgBox("An unexpected error has occurred." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try

    End Sub

    Private Sub mnuEmailTemplateEditor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEmailTemplateEditor.Click
        Dim dlg As New FWTemplateEditor

        dlg.ShowDialog()

        dlg.Dispose()
        dlg = Nothing
    End Sub

    Private Sub mnuProdCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuProdCategory.Click
        Try
            Dim res As MsgBoxResult
            Dim db As New DBConnection(ConnectionString)
            Dim sql As String
            Dim fixedLookup As dlgFixedSelection
            Dim dlgRes As DialogResult
            Dim UpdateLocation, UpdateCategory As Integer
            Dim tables As New cTables(ActiveDBVersion)

            'fws = LoadXMLSettings() ' SetApplicationProperties()

            fixedLookup = New dlgFixedSelection

            fixedLookup.Tag = "accountsSubAccounts.subAccountId"
            dlgRes = fixedLookup.ShowDialog()
            If dlgRes = System.Windows.Forms.DialogResult.OK Then
                UpdateLocation = fixedLookup.Tag
                fixedLookup.Dispose()

                fixedLookup = New dlgFixedSelection
                fixedLookup.Tag = "[productCategories]"
                fixedLookup.ActiveLocation = UpdateLocation
                dlgRes = fixedLookup.ShowDialog()

                If dlgRes = System.Windows.Forms.DialogResult.OK Then
                    UpdateCategory = fixedLookup.Tag
                    If UpdateCategory > 0 Then
                        res = MsgBox("Update only those currently without a product category set?", MsgBoxStyle.YesNo, "FWUtilities")

                        If res = MsgBoxResult.Yes Then
                            sql = "UPDATE [productDetails] SET [ProductCategoryId] = " & UpdateCategory.ToString.Trim & " WHERE [subAccountId] = " & UpdateLocation.ToString & " AND [ProductCategoryId] IS NULL OR [ProductCategoryId] = 0"
                            db.ExecuteSQL(sql)
                        Else
                            Dim res2 As MsgBoxResult

                            res2 = MsgBox("Are you sure you want to set the product category for ALL contracts" & vbNewLine & "without exception in the selected location?", MsgBoxStyle.YesNo, "FWUtilities")
                            If res2 = MsgBoxResult.Yes Then
                                sql = "UPDATE [productDetails] SET [ProductCategoryId] = " & UpdateCategory.ToString.Trim & " WHERE [subAccountId] = " & UpdateLocation.ToString
                                db.ExecuteSQL(sql)
                            Else
                                MsgBox("Failed to clarify scope of update." & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                            End If
                        End If

                        MsgBox("Bulk update of Product Category completed successfully.", MsgBoxStyle.OkOnly, "FWUtilties")
                    Else
                        MsgBox("Failed to select a product category" & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                    End If
                End If
            End If

            fixedLookup.Dispose()
            fixedLookup = Nothing

        Catch ex As Exception
            MsgBox("An unexpected error has occurred." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    Private Sub mnuShowTestIsland_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuShowTestIsland.Click

        Dim x As New TestIsland
        x.ShowDialog()
    End Sub

    Private Sub mnuCodesProductCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCodesProductCategory.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesProductCategory
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuBespoke_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuBespoke.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.RechargeAssociationsBespoke
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuPortion_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPortion.Click
        Try
            Dim res As MsgBoxResult
            Dim db As New DBConnection(ConnectionString)
            Dim sql As String
            Dim fixedLookup As dlgFixedSelection
            Dim dlgRes As DialogResult
            Dim UpdateLocation As Integer
            Dim UpdatePortion As Double
            Dim tables As New cTables(ActiveDBVersion)

            'fws = LoadXMLSettings() 'SetApplicationProperties()

            fixedLookup = New dlgFixedSelection

            fixedLookup.Tag = "accountsSubAccounts.subAccountId"
            dlgRes = fixedLookup.ShowDialog()
            If dlgRes = System.Windows.Forms.DialogResult.OK Then
                UpdateLocation = fixedLookup.Tag
                fixedLookup.Dispose()

                fixedLookup = New dlgFixedSelection
                fixedLookup.Tag = "Portion"
                fixedLookup.ActiveLocation = UpdateLocation
                dlgRes = fixedLookup.ShowDialog()

                If dlgRes = System.Windows.Forms.DialogResult.OK Then
                    UpdatePortion = fixedLookup.Tag
                    res = MsgBox("Update only those with ZERO portion?", MsgBoxStyle.YesNo, "FWUtilities")

                    If res = MsgBoxResult.Yes Then
                        sql = "UPDATE [recharge_associations] SET [Portion] = " & UpdatePortion.ToString & " WHERE [subAccountId] = " & UpdateLocation.ToString & " AND [Portion] IS NULL OR [Portion] = 0"
                        db.ExecuteSQL(sql)
                    Else
                        Dim res2 As MsgBoxResult

                        res2 = MsgBox("Are you sure you want to set the portion for ALL recharge elements" & vbNewLine & "without exception in the selected location?", MsgBoxStyle.YesNo, "FWUtilities")
                        If res2 = MsgBoxResult.Yes Then
                            sql = "UPDATE [recharge_associations] SET [Portion] = " & UpdatePortion.ToString & " WHERE [subAccountId] = " & UpdateLocation.ToString
                            db.ExecuteSQL(sql)
                        Else
                            MsgBox("Failed to clarify scope of update." & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                        End If
                    End If

                    MsgBox("Bulk update of Recharge Portion completed successfully.", MsgBoxStyle.OkOnly, "FWUtilties")
                Else
                    MsgBox("Failed to select a Recharge Portion" & vbNewLine & "Update aborted", MsgBoxStyle.Critical, "FWUtilties Error")
                End If
            End If

            fixedLookup.Dispose()
            fixedLookup = Nothing

        Catch ex As Exception
            MsgBox("An unexpected error has occurred." & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub

    'Private Sub mnuUpgRepFields_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUpgRepFields.Click
    '    Dim db As New DBConnection(cUtilAccounts.getConnectionString(AccountId))
    '    Dim curDBVersion As Integer = -1
    '    Dim OK2Run As Boolean = True
    '    Dim currentCursor As Cursor = Windows.Forms.Cursor.Current
    '    Dim fwTables As New cTables(ActiveDBVersion)

    '    Windows.Forms.Cursor.Current = Cursors.WaitCursor

    '    If ActiveDBVersion < 19 Then
    '        MsgBox("This option is not relevant for you current database version." & vbNewLine & "Please run FWUpdate first to ensure you are on the latest database version.")
    '        OK2Run = False
    '    End If

    '    If OK2Run Then
    '        Dim keyCollection As System.Collections.ICollection = fwTables.GetKeys
    '        Dim tmpKey As System.Collections.IEnumerator = keyCollection.GetEnumerator

    '        Dim dbjoinsSQL As String = "UPDATE [system_dbjoins] SET [Table] = @newTable WHERE [Table] = @oldTable"
    '        Dim dbjoinsSQL2 As String = "UPDATE [system_dbjoins] SET [joinTable] = @newTable WHERE [joinTable] = @oldTable"

    '        Do While tmpKey.MoveNext = True
    '            db.AddDBParam("oldTable", "[" & tmpKey.Current.ToString & "]", True)
    '            db.AddDBParam("newTable", "[" & fwTables.GetTable(tmpKey.Current.ToString) & "]")
    '            db.ExecuteSQL(dbjoinsSQL)
    '            db.ExecuteSQL(dbjoinsSQL2)
    '        Loop

    '        db.RunSQL("SELECT [ReportId],[SQLText] FROM [saved_reports]", db.glDBWorkA)

    '        Dim drow As DataRow
    '        Dim dotPos As Integer = 0
    '        Dim tmpTable As String = ""
    '        Dim tmpFullDesc As String = ""
    '        Dim prgCount As Integer = 0

    '        lblPercent.Visible = True
    '        prgProgress.Visible = True
    '        prgProgress.Minimum = 0
    '        prgProgress.Maximum = db.glNumRowsReturned
    '        prgProgress.Value = 0

    '        For Each drow In db.glDBWorkA.Tables(0).Rows
    '            tmpFullDesc = drow.Item("SQLText")
    '            If tmpFullDesc <> "" Then
    '                dotPos = tmpFullDesc.IndexOf(".")
    '                If dotPos > 0 Then
    '                    Dim remainingStr As String = tmpFullDesc.Substring(dotPos)
    '                    Dim startStr As String = tmpFullDesc.Substring(0, (tmpFullDesc.Length - remainingStr.Length))
    '                    'MsgBox("startstr = " & startStr)
    '                    'MsgBox("remainingStr = " & remainingStr)

    '                    Dim newstartStr As String = ""
    '                    startStr = startStr.Replace("[", "")
    '                    startStr = startStr.Replace("]", "")

    '                    If fwTables.TableExists(startStr) = True Then
    '                        newstartStr = fwTables.GetTable(startStr, True)
    '                    Else
    '                        newstartStr = "[" & startStr & "]"
    '                    End If

    '                    db.AddDBParam("sql_text", newstartStr.Trim & remainingStr.Trim, True)
    '                    db.AddDBParam("reportId", drow.Item("ReportId"))
    '                    Try
    '                        db.ExecuteSQL("UPDATE [saved_reports] SET [SQLText] = @sql_text WHERE [ReportId] = @reportId")
    '                    Catch ex As Exception
    '                        MsgBox(ex.Message)
    '                    End Try

    '                    'MsgBox("New [SQLText] = " & newstartStr.Trim & remainingStr.Trim)
    '                    prgCount += 1

    '                    prgProgress.Value = prgCount
    '                    lblPercent.Text = prgCount.ToString & " Records processed"
    '                    lblPercent.Refresh()
    '                    prgProgress.Refresh()
    '                End If
    '            End If
    '        Next

    '        db.ExecuteSQL("UPDATE [saved_reports] SET [ReportBase] = '[contract_details]' WHERE [ReportBase] = '[Contract Details]'")
    '        db.ExecuteSQL("UPDATE [saved_reports] SET [ReportBase] = '[vendor_details]' WHERE [ReportBase] = '[Vendor Details]'")
    '        db.ExecuteSQL("UPDATE [saved_reports] SET [ReportBase] = '[product_details]' WHERE [ReportBase] = '[Product Details]'")
    '        db.ExecuteSQL("UPDATE [saved_reports] SET [ReportBase] = '[contract_productdetails_recharge]' WHERE [ReportBase] = '[Contract - Product Details - Recharge]'")
    '        db.ExecuteSQL("UPDATE [system_dbjoins] SET [JoinAlias] = '[owner_link]' WHERE [JoinAlias] = '[Owner Link]'")
    '    End If

    '    Windows.Forms.Cursor.Current = currentCursor

    '    lblPercent.Visible = False
    '    prgProgress.Visible = False
    '    MsgBox("Field Updates are Complete")
    'End Sub

    Private Sub mnuLocationRemoval_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuLocationRemoval.Click
        If Me.ActiveDBVersion >= 19 Then
            If Not AdminFunctionsEnabled Then
                Dim pwd_dlg As New dlgPassword

                If pwd_dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                    ' ok to proceed
                    AdminFunctionsEnabled = True

                    mnuLocationRemoval.Visible = True
                End If
                pwd_dlg.Dispose()
            End If

            If AdminFunctionsEnabled Then
                Dim bulk_dlg As New BulkRemoval
                bulk_dlg.ShowDialog()
                bulk_dlg.Dispose()
            End If
        Else
            MsgBox("ERROR! Apologies, but this reserved function is only available for database version 19 or greater" & vbNewLine & "Your current version is " & Me.ActiveDBVersion.ToString)
        End If

    End Sub

    Private Sub ValidateConnection()
        Dim db As New DBConnection(ConnectionString)

        Try
            Dim count As Integer = db.getcount("select count(*) from employees")

        Catch ex As Exception
            ' failed, so try new table name
            MsgBox("Unable to open the database" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
            checkDBVersion(-1)
            Exit Sub
        End Try

        'If checkDBVersion(Integer.Parse(db.FWDbFindVal("DBVersion"))) = False Then
        '    db.DBClose()
        '    db = Nothing
        '    Exit Sub
        'Else
        '    MsgBox("Current Database Version = " & db.FWDbFindVal("DBVersion"), MsgBoxStyle.Information, "FWUtilties")
        'End If
        Dim accs As New cUtilAccounts
        Dim acc As cAccount = accs.getAccountById(AccountId)
        StatusBar.Panels(0).Text = acc.dbserver & " : " & acc.dbname

        ' disable menu options that cannot be available if not a web service
        mnuEmailSettings.Enabled = False
        mnuExecute.Enabled = False
    End Sub

    Private Sub mnuEncryption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuEncryption.Click
        ' force specification of password to prevent access to encryption method
        Dim pwd As New dlgPassword

        If pwd.ShowDialog = Windows.Forms.DialogResult.OK Then
            ' check password returned
            Dim dlg As New dlgLicenseEncoding

            dlg.ShowDialog()
        End If
    End Sub

    Private Sub mnuApplyLicence_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuApplyLicence.Click
        Dim dlg As New ApplyLicence
        dlg.ShowDialog()
    End Sub

    Private Sub mnuInvDetailsImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuInvDetailsImport.Click
        Dim dlg As New FWImport
        dlg.Tag = cUtility.ImportType.InvoiceDetails
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub

    Private Sub mnuUFListItemImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUFListItemImport.Click
        Dim dlg As New UserFieldSelect()

        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim fieldId As Integer = 0
            If IsNumeric(dlg.Tag) Then
                fieldId = Integer.Parse(dlg.Tag)
            End If

            dlg.Dispose()
            dlg = Nothing

            Dim dlgImport As New FWImport
            dlgImport.Tag = cUtility.ImportType.UFListItems
            dlgImport.UserFieldId = fieldId
            dlgImport.ShowDialog()

            dlgImport.Dispose()
            dlgImport = Nothing

            LoadLastLog()
        End If
    End Sub

    Private nAccountID As Integer
    Private clActiveAccount As cAccount
    Public Property AccountId() As Integer
        Get
            Return nAccountID
        End Get
        Set(ByVal value As Integer)
            nAccountID = value

            Dim accs As New cUtilAccounts
            clActiveAccount = accs.getAccountById(nAccountID)
        End Set
    End Property

    Public ReadOnly Property ActiveAccount() As cAccount
        Get
            Return clActiveAccount
        End Get
    End Property

    Private nActiveSubAccount As cAccountSubAccount
    Public ReadOnly Property ActiveSubAccount() As cAccountSubAccount
        Get
            Return nActiveSubAccount
        End Get
    End Property

    Private clSubAccounts As cUtilSubAccounts
    Public ReadOnly Property SubAccounts As cUtilSubAccounts
        Get
            Return clSubAccounts
        End Get
    End Property

    Private clAccounts As cUtilAccounts
    Public ReadOnly Property Accounts As cUtilAccounts
        Get
            Return clAccounts
        End Get
    End Property

    Private sContractKey As String
    Public Property FieldKey() As String
        Get
            Return sContractKey
        End Get
        Set(ByVal value As String)
            sContractKey = value
        End Set
    End Property

    Private Sub initMetabase()
        'SqlDependency.Start(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
    End Sub

    Private Sub deinitMetabase()
        'SqlDependency.Stop(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
    End Sub

    Private Sub mnuChangeSubAcc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuChangeSubAcc.Click
        Dim dlg As New dlgFixedSelection()
        dlg.Tag = "accountsSubAccounts.subAccountId"

        If dlg.ShowDialog(Me) = DialogResult.OK Then
            Dim subaccs As New cUtilSubAccounts()

            nActiveSubAccount = subaccs.getSubAccountById(CInt(dlg.Tag))

            StatusBar.Panels(0).Text = "Active Connection: " + ActiveAccount.dbserver.ToUpper() + " : " + ActiveAccount.companyname + " (SubAccount: " + ActiveSubAccount.Description + ")"
            StatusBar.Refresh()
        End If
    End Sub

    Private Sub mnuCodesContractCategory_Click(sender As System.Object, e As System.EventArgs) Handles mnuCodesContractCategory.Click
        Dim dlg As New FWImport

        dlg.Tag = cUtility.ImportType.CodesContractCategory
        dlg.ShowDialog()
        dlg.Dispose()
        dlg = Nothing
        LoadLastLog()
    End Sub
End Class
