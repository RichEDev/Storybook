Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports FWBase
Imports Spend_Management
Imports SpendManagementLibrary

Public Class FWImport
    Inherits System.Windows.Forms.Form
    Public ImpType As cUtility.ImportType
    Public ImpDesc As String
    Public ImpFile As StreamReader
    Public ImpTemplate As String
    Public ImpMap(250) As cUtility.ImportMap
    Public ImpColumns As Integer
    Public FixedColumns As Integer
    Public dt As New DataTable("dstMap")
    Public st As New DataTable("srcMap")
    Private b_dstGridLeftMouseDown As Boolean = False
    Private b_srcGridLeftMouseDown As Boolean = False
    Private uiMapping() As cUtility.UserMappings
    Private uiMappingCount As Integer
    Private slFixedMappings As New SortedList
    Public confirmUnMap As Boolean
    Private UFDDLists As New System.Collections.SortedList
    Private ImportedMapping As Boolean

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
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents grpFiles As System.Windows.Forms.GroupBox
    Friend WithEvents lblImportType As System.Windows.Forms.Label
    Friend WithEvents lblSrcFile As System.Windows.Forms.Label
    Friend WithEvents txtSrcFile As System.Windows.Forms.TextBox
    Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents lblDelimiter As System.Windows.Forms.Label
    Friend WithEvents cmdGetFile As System.Windows.Forms.Button
    Friend WithEvents grpInFields As System.Windows.Forms.GroupBox
    Friend WithEvents chkIgnoreFirstRow As System.Windows.Forms.CheckBox
    Friend WithEvents cmdImport As System.Windows.Forms.Button
    Friend WithEvents lblSrcFileStatus As System.Windows.Forms.Label
    Friend WithEvents lblImportTypeDesc As System.Windows.Forms.Label
    Friend WithEvents srcGrid As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents txtDelimiter As System.Windows.Forms.TextBox
    Friend WithEvents grpDestFields As System.Windows.Forms.GroupBox
    Friend WithEvents dstGrid As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents cmdLoadSample As System.Windows.Forms.Button
    Friend WithEvents ctxtMnu As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents FWImport_Fill_Panel As System.Windows.Forms.Panel
    Friend WithEvents _FWImport_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _FWImport_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _FWImport_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _FWImport_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents lblMapTip As System.Windows.Forms.Label
    Friend WithEvents lblUnmapTip As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblImportStatus As System.Windows.Forms.Label
    Friend WithEvents lblStatusLabel As System.Windows.Forms.Label
    Friend WithEvents prgImport As Infragistics.Win.UltraWinProgressBar.UltraProgressBar
    Friend WithEvents chkSaveMapping As System.Windows.Forms.CheckBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(FWImport))
        Dim UltraToolbar1 As Infragistics.Win.UltraWinToolbars.UltraToolbar = New Infragistics.Win.UltraWinToolbars.UltraToolbar("rClickMenu")
        Dim PopupMenuTool1 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("Mapping Action")
        Dim PopupMenuTool2 As Infragistics.Win.UltraWinToolbars.PopupMenuTool = New Infragistics.Win.UltraWinToolbars.PopupMenuTool("Mapping Action")
        Dim ButtonTool1 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Delete_Mapping")
        Dim ButtonTool2 As Infragistics.Win.UltraWinToolbars.ButtonTool = New Infragistics.Win.UltraWinToolbars.ButtonTool("Delete_Mapping")
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.grpFiles = New System.Windows.Forms.GroupBox
        Me.cmdLoadSample = New System.Windows.Forms.Button
        Me.lblImportTypeDesc = New System.Windows.Forms.Label
        Me.lblSrcFileStatus = New System.Windows.Forms.Label
        Me.chkIgnoreFirstRow = New System.Windows.Forms.CheckBox
        Me.cmdGetFile = New System.Windows.Forms.Button
        Me.txtDelimiter = New System.Windows.Forms.TextBox
        Me.lblDelimiter = New System.Windows.Forms.Label
        Me.txtSrcFile = New System.Windows.Forms.TextBox
        Me.lblSrcFile = New System.Windows.Forms.Label
        Me.lblImportType = New System.Windows.Forms.Label
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
        Me.grpInFields = New System.Windows.Forms.GroupBox
        Me.srcGrid = New Infragistics.Win.UltraWinGrid.UltraGrid
        Me.cmdImport = New System.Windows.Forms.Button
        Me.grpDestFields = New System.Windows.Forms.GroupBox
        Me.dstGrid = New Infragistics.Win.UltraWinGrid.UltraGrid
        Me.ctxtMnu = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(Me.components)
        Me.FWImport_Fill_Panel = New System.Windows.Forms.Panel
        Me.prgImport = New Infragistics.Win.UltraWinProgressBar.UltraProgressBar
        Me.lblStatusLabel = New System.Windows.Forms.Label
        Me.lblImportStatus = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblUnmapTip = New System.Windows.Forms.Label
        Me.lblMapTip = New System.Windows.Forms.Label
        Me._FWImport_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Me._FWImport_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Me._FWImport_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Me._FWImport_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
        Me.chkSaveMapping = New System.Windows.Forms.CheckBox
        Me.grpFiles.SuspendLayout()
        Me.grpInFields.SuspendLayout()
        CType(Me.srcGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpDestFields.SuspendLayout()
        CType(Me.dstGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ctxtMnu, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FWImport_Fill_Panel.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(512, 688)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(80, 24)
        Me.cmdCancel.TabIndex = 7
        Me.cmdCancel.Text = "Cancel"
        '
        'grpFiles
        '
        Me.grpFiles.Controls.Add(Me.chkSaveMapping)
        Me.grpFiles.Controls.Add(Me.cmdLoadSample)
        Me.grpFiles.Controls.Add(Me.lblImportTypeDesc)
        Me.grpFiles.Controls.Add(Me.lblSrcFileStatus)
        Me.grpFiles.Controls.Add(Me.chkIgnoreFirstRow)
        Me.grpFiles.Controls.Add(Me.cmdGetFile)
        Me.grpFiles.Controls.Add(Me.txtDelimiter)
        Me.grpFiles.Controls.Add(Me.lblDelimiter)
        Me.grpFiles.Controls.Add(Me.txtSrcFile)
        Me.grpFiles.Controls.Add(Me.lblSrcFile)
        Me.grpFiles.Controls.Add(Me.lblImportType)
        Me.grpFiles.Location = New System.Drawing.Point(8, 8)
        Me.grpFiles.Name = "grpFiles"
        Me.grpFiles.Size = New System.Drawing.Size(952, 96)
        Me.grpFiles.TabIndex = 1
        Me.grpFiles.TabStop = False
        Me.grpFiles.Text = "Import and Source File"
        '
        'cmdLoadSample
        '
        Me.cmdLoadSample.Location = New System.Drawing.Point(744, 56)
        Me.cmdLoadSample.Name = "cmdLoadSample"
        Me.cmdLoadSample.Size = New System.Drawing.Size(88, 24)
        Me.cmdLoadSample.TabIndex = 7
        Me.cmdLoadSample.Text = "Load Sample"
        '
        'lblImportTypeDesc
        '
        Me.lblImportTypeDesc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblImportTypeDesc.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblImportTypeDesc.Location = New System.Drawing.Point(80, 24)
        Me.lblImportTypeDesc.Name = "lblImportTypeDesc"
        Me.lblImportTypeDesc.Size = New System.Drawing.Size(168, 24)
        Me.lblImportTypeDesc.TabIndex = 6
        Me.lblImportTypeDesc.Text = "Unknown Type Specified"
        Me.lblImportTypeDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSrcFileStatus
        '
        Me.lblSrcFileStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSrcFileStatus.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSrcFileStatus.Location = New System.Drawing.Point(336, 56)
        Me.lblSrcFileStatus.Name = "lblSrcFileStatus"
        Me.lblSrcFileStatus.Size = New System.Drawing.Size(400, 32)
        Me.lblSrcFileStatus.TabIndex = 0
        Me.lblSrcFileStatus.Text = "** No Source File Opened **"
        Me.lblSrcFileStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'chkIgnoreFirstRow
        '
        Me.chkIgnoreFirstRow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkIgnoreFirstRow.Location = New System.Drawing.Point(8, 54)
        Me.chkIgnoreFirstRow.Name = "chkIgnoreFirstRow"
        Me.chkIgnoreFirstRow.Size = New System.Drawing.Size(200, 24)
        Me.chkIgnoreFirstRow.TabIndex = 5
        Me.chkIgnoreFirstRow.Text = "Ignore Header Row in Source File"
        '
        'cmdGetFile
        '
        Me.cmdGetFile.Image = CType(resources.GetObject("cmdGetFile.Image"), System.Drawing.Image)
        Me.cmdGetFile.Location = New System.Drawing.Point(744, 24)
        Me.cmdGetFile.Name = "cmdGetFile"
        Me.cmdGetFile.Size = New System.Drawing.Size(32, 24)
        Me.cmdGetFile.TabIndex = 3
        '
        'txtDelimiter
        '
        Me.txtDelimiter.Location = New System.Drawing.Point(896, 24)
        Me.txtDelimiter.Name = "txtDelimiter"
        Me.txtDelimiter.Size = New System.Drawing.Size(48, 20)
        Me.txtDelimiter.TabIndex = 4
        Me.txtDelimiter.Text = ","
        Me.txtDelimiter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblDelimiter
        '
        Me.lblDelimiter.Location = New System.Drawing.Point(784, 24)
        Me.lblDelimiter.Name = "lblDelimiter"
        Me.lblDelimiter.Size = New System.Drawing.Size(96, 20)
        Me.lblDelimiter.TabIndex = 0
        Me.lblDelimiter.Text = "Column Delimiter"
        Me.lblDelimiter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtSrcFile
        '
        Me.txtSrcFile.Location = New System.Drawing.Point(336, 24)
        Me.txtSrcFile.Name = "txtSrcFile"
        Me.txtSrcFile.Size = New System.Drawing.Size(400, 20)
        Me.txtSrcFile.TabIndex = 2
        Me.txtSrcFile.Text = ""
        '
        'lblSrcFile
        '
        Me.lblSrcFile.Location = New System.Drawing.Point(264, 24)
        Me.lblSrcFile.Name = "lblSrcFile"
        Me.lblSrcFile.Size = New System.Drawing.Size(72, 20)
        Me.lblSrcFile.TabIndex = 0
        Me.lblSrcFile.Text = "Source File"
        Me.lblSrcFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblImportType
        '
        Me.lblImportType.Location = New System.Drawing.Point(8, 24)
        Me.lblImportType.Name = "lblImportType"
        Me.lblImportType.Size = New System.Drawing.Size(72, 24)
        Me.lblImportType.TabIndex = 0
        Me.lblImportType.Text = "Import Type"
        Me.lblImportType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.DefaultExt = "csv"
        Me.OpenFileDialog.Title = "Framework Import Source File"
        '
        'grpInFields
        '
        Me.grpInFields.Controls.Add(Me.srcGrid)
        Me.grpInFields.Location = New System.Drawing.Point(8, 112)
        Me.grpInFields.Name = "grpInFields"
        Me.grpInFields.Size = New System.Drawing.Size(352, 600)
        Me.grpInFields.TabIndex = 2
        Me.grpInFields.TabStop = False
        Me.grpInFields.Text = "Source Fields"
        '
        'srcGrid
        '
        Me.srcGrid.DisplayLayout.AddNewBox.Prompt = "Add ..."
        Me.srcGrid.DisplayLayout.GroupByBox.Prompt = "Drag a column header here to group by that column."
        Me.srcGrid.DisplayLayout.MaxColScrollRegions = 2
        Me.srcGrid.DisplayLayout.Override.NullText = ""
        Me.srcGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
        Me.srcGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.srcGrid.Location = New System.Drawing.Point(3, 16)
        Me.srcGrid.Name = "srcGrid"
        Me.srcGrid.Size = New System.Drawing.Size(346, 581)
        Me.srcGrid.TabIndex = 0
        Me.srcGrid.Text = "Framework Import Source Fields"
        '
        'cmdImport
        '
        Me.cmdImport.Location = New System.Drawing.Point(368, 688)
        Me.cmdImport.Name = "cmdImport"
        Me.cmdImport.Size = New System.Drawing.Size(80, 24)
        Me.cmdImport.TabIndex = 6
        Me.cmdImport.Text = "Import"
        '
        'grpDestFields
        '
        Me.grpDestFields.Controls.Add(Me.dstGrid)
        Me.grpDestFields.Location = New System.Drawing.Point(600, 112)
        Me.grpDestFields.Name = "grpDestFields"
        Me.grpDestFields.Size = New System.Drawing.Size(360, 600)
        Me.grpDestFields.TabIndex = 8
        Me.grpDestFields.TabStop = False
        Me.grpDestFields.Text = "Destination Database Fields"
        '
        'dstGrid
        '
        Me.dstGrid.DisplayLayout.AddNewBox.Prompt = "Add ..."
        Me.dstGrid.DisplayLayout.GroupByBox.Prompt = "Drag a column header here to group by that column."
        Me.dstGrid.DisplayLayout.MaxColScrollRegions = 2
        Me.dstGrid.DisplayLayout.Override.NullText = ""
        Me.dstGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand
        Me.dstGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dstGrid.Location = New System.Drawing.Point(3, 16)
        Me.dstGrid.Name = "dstGrid"
        Me.dstGrid.Size = New System.Drawing.Size(354, 581)
        Me.dstGrid.TabIndex = 0
        Me.dstGrid.Text = "Framework Destination Database Fields"
        '
        'ctxtMnu
        '
        Me.ctxtMnu.DesignerFlags = 1
        Me.ctxtMnu.DockWithinContainer = Me
        Me.ctxtMnu.ShowFullMenusDelay = 500
        UltraToolbar1.DockedColumn = 0
        UltraToolbar1.DockedRow = 0
        UltraToolbar1.Text = "rClickMenu"
        UltraToolbar1.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {PopupMenuTool1})
        UltraToolbar1.Visible = False
        Me.ctxtMnu.Toolbars.AddRange(New Infragistics.Win.UltraWinToolbars.UltraToolbar() {UltraToolbar1})
        PopupMenuTool2.SharedProps.Caption = "Mapping Action"
        PopupMenuTool2.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {ButtonTool1})
        ButtonTool2.SharedProps.Caption = "Delete Mapping"
        Me.ctxtMnu.Tools.AddRange(New Infragistics.Win.UltraWinToolbars.ToolBase() {PopupMenuTool2, ButtonTool2})
        '
        'FWImport_Fill_Panel
        '
        Me.FWImport_Fill_Panel.Controls.Add(Me.prgImport)
        Me.FWImport_Fill_Panel.Controls.Add(Me.lblStatusLabel)
        Me.FWImport_Fill_Panel.Controls.Add(Me.lblImportStatus)
        Me.FWImport_Fill_Panel.Controls.Add(Me.Label2)
        Me.FWImport_Fill_Panel.Controls.Add(Me.Label1)
        Me.FWImport_Fill_Panel.Controls.Add(Me.lblUnmapTip)
        Me.FWImport_Fill_Panel.Controls.Add(Me.lblMapTip)
        Me.FWImport_Fill_Panel.Controls.Add(Me.grpDestFields)
        Me.FWImport_Fill_Panel.Controls.Add(Me.cmdImport)
        Me.FWImport_Fill_Panel.Controls.Add(Me.grpInFields)
        Me.FWImport_Fill_Panel.Controls.Add(Me.grpFiles)
        Me.FWImport_Fill_Panel.Controls.Add(Me.cmdCancel)
        Me.FWImport_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default
        Me.FWImport_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FWImport_Fill_Panel.Location = New System.Drawing.Point(0, 0)
        Me.FWImport_Fill_Panel.Name = "FWImport_Fill_Panel"
        Me.FWImport_Fill_Panel.Size = New System.Drawing.Size(968, 718)
        Me.FWImport_Fill_Panel.TabIndex = 0
        '
        'prgImport
        '
        Me.prgImport.Location = New System.Drawing.Point(368, 656)
        Me.prgImport.Name = "prgImport"
        Me.prgImport.Size = New System.Drawing.Size(224, 23)
        Me.prgImport.TabIndex = 15
        Me.prgImport.Text = "[Formatted]"
        Me.prgImport.Visible = False
        '
        'lblStatusLabel
        '
        Me.lblStatusLabel.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatusLabel.Location = New System.Drawing.Point(360, 624)
        Me.lblStatusLabel.Name = "lblStatusLabel"
        Me.lblStatusLabel.Size = New System.Drawing.Size(56, 23)
        Me.lblStatusLabel.TabIndex = 14
        Me.lblStatusLabel.Text = "Status :"
        Me.lblStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblImportStatus
        '
        Me.lblImportStatus.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblImportStatus.Location = New System.Drawing.Point(416, 624)
        Me.lblImportStatus.Name = "lblImportStatus"
        Me.lblImportStatus.Size = New System.Drawing.Size(176, 24)
        Me.lblImportStatus.TabIndex = 13
        Me.lblImportStatus.Text = "Idle"
        Me.lblImportStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(368, 488)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(224, 24)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Double click the destination field."
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(368, 216)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(224, 24)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Drag and drop element left to right"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblUnmapTip
        '
        Me.lblUnmapTip.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUnmapTip.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnmapTip.Location = New System.Drawing.Point(368, 448)
        Me.lblUnmapTip.Name = "lblUnmapTip"
        Me.lblUnmapTip.Size = New System.Drawing.Size(224, 32)
        Me.lblUnmapTip.TabIndex = 10
        Me.lblUnmapTip.Text = "To remove a mapping"
        Me.lblUnmapTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblMapTip
        '
        Me.lblMapTip.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblMapTip.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMapTip.Location = New System.Drawing.Point(368, 176)
        Me.lblMapTip.Name = "lblMapTip"
        Me.lblMapTip.Size = New System.Drawing.Size(224, 32)
        Me.lblMapTip.TabIndex = 9
        Me.lblMapTip.Text = "To map a field"
        Me.lblMapTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        '_FWImport_Toolbars_Dock_Area_Left
        '
        Me._FWImport_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._FWImport_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control
        Me._FWImport_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        Me._FWImport_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText
        Me._FWImport_Toolbars_Dock_Area_Left.Location = New System.Drawing.Point(0, 0)
        Me._FWImport_Toolbars_Dock_Area_Left.Name = "_FWImport_Toolbars_Dock_Area_Left"
        Me._FWImport_Toolbars_Dock_Area_Left.Size = New System.Drawing.Size(0, 718)
        Me._FWImport_Toolbars_Dock_Area_Left.ToolbarsManager = Me.ctxtMnu
        '
        '_FWImport_Toolbars_Dock_Area_Right
        '
        Me._FWImport_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._FWImport_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control
        Me._FWImport_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        Me._FWImport_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText
        Me._FWImport_Toolbars_Dock_Area_Right.Location = New System.Drawing.Point(968, 0)
        Me._FWImport_Toolbars_Dock_Area_Right.Name = "_FWImport_Toolbars_Dock_Area_Right"
        Me._FWImport_Toolbars_Dock_Area_Right.Size = New System.Drawing.Size(0, 718)
        Me._FWImport_Toolbars_Dock_Area_Right.ToolbarsManager = Me.ctxtMnu
        '
        '_FWImport_Toolbars_Dock_Area_Top
        '
        Me._FWImport_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._FWImport_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control
        Me._FWImport_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        Me._FWImport_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText
        Me._FWImport_Toolbars_Dock_Area_Top.Location = New System.Drawing.Point(0, 0)
        Me._FWImport_Toolbars_Dock_Area_Top.Name = "_FWImport_Toolbars_Dock_Area_Top"
        Me._FWImport_Toolbars_Dock_Area_Top.Size = New System.Drawing.Size(968, 0)
        Me._FWImport_Toolbars_Dock_Area_Top.ToolbarsManager = Me.ctxtMnu
        '
        '_FWImport_Toolbars_Dock_Area_Bottom
        '
        Me._FWImport_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me._FWImport_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control
        Me._FWImport_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        Me._FWImport_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText
        Me._FWImport_Toolbars_Dock_Area_Bottom.Location = New System.Drawing.Point(0, 718)
        Me._FWImport_Toolbars_Dock_Area_Bottom.Name = "_FWImport_Toolbars_Dock_Area_Bottom"
        Me._FWImport_Toolbars_Dock_Area_Bottom.Size = New System.Drawing.Size(968, 0)
        Me._FWImport_Toolbars_Dock_Area_Bottom.ToolbarsManager = Me.ctxtMnu
        '
        'chkSaveMapping
        '
        Me.chkSaveMapping.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkSaveMapping.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.chkSaveMapping.Location = New System.Drawing.Point(8, 77)
        Me.chkSaveMapping.Name = "chkSaveMapping"
        Me.chkSaveMapping.Size = New System.Drawing.Size(200, 16)
        Me.chkSaveMapping.TabIndex = 8
        Me.chkSaveMapping.Text = "Save Import Mapping"
        '
        'FWImport
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(968, 718)
        Me.Controls.Add(Me.FWImport_Fill_Panel)
        Me.Controls.Add(Me._FWImport_Toolbars_Dock_Area_Left)
        Me.Controls.Add(Me._FWImport_Toolbars_Dock_Area_Right)
        Me.Controls.Add(Me._FWImport_Toolbars_Dock_Area_Top)
        Me.Controls.Add(Me._FWImport_Toolbars_Dock_Area_Bottom)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FWImport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Framework Import Utility"
        Me.grpFiles.ResumeLayout(False)
        Me.grpInFields.ResumeLayout(False)
        CType(Me.srcGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpDestFields.ResumeLayout(False)
        CType(Me.dstGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ctxtMnu, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FWImport_Fill_Panel.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Close()

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private nActiveImportLocationId As Integer
    Private Property ImportLocation() As Integer
        Get
            Return nActiveImportLocationId
        End Get
        Set(ByVal value As Integer)
            nActiveImportLocationId = value
        End Set
    End Property

    Private nUserFieldId As Integer
    Public Property UserFieldId() As Integer
        Get
            Return nUserFieldId
        End Get
        Set(ByVal value As Integer)
            nUserFieldId = value
        End Set
    End Property

    Private Sub cmdGetFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGetFile.Click
        OpenFileDialog.Title = "Open source file for import"
        OpenFileDialog.Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*"
        'OpenFileDialog.InitialDirectory = "C:\"


        If OpenFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            txtSrcFile.Text = OpenFileDialog.FileName
            Try
                ImpFile = New StreamReader(txtSrcFile.Text)

            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
                Exit Sub
            End Try

            ' get the first line of the src file to display in the src grid
            Dim srcLine As String = ""
            If ImpFile.Peek >= 0 Then
                srcLine = ImpFile.ReadLine
            End If

            Dim srcElements() As String
            Dim x As Integer

            srcElements = Split(srcLine, txtDelimiter.Text.Trim)
            With srcGrid.DisplayLayout
                .ValueLists.Clear()
                .ValueLists.Add("SourceColumns")

                For x = 1 To srcElements.Length
                    With .ValueLists("SourceColumns").ValueListItems
                        .Add(x, "srcColumn " & Trim(Str(x)))
                    End With
                Next
            End With
        End If
        'srcGrid.DataBind()
    End Sub

    Private Sub cmdImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdImport.Click
        NewImportClick()
    End Sub

    Private Sub FWImport_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Select Case CType(Me.Tag, cUtility.ImportType)
            Case cUtility.ImportType.ContractDetails
                lblImportTypeDesc.Text = "Contract Details"
                ImpTemplate = "CD Import.csv"
            
            Case cUtility.ImportType.ProductDetails
                lblImportTypeDesc.Text = "Product Details"
                ImpTemplate = "PD Import.csv"

            Case cUtility.ImportType.StaffDetails
                lblImportTypeDesc.Text = "Employee Details"
                ImpTemplate = "SD Import.csv"

            Case cUtility.ImportType.VendorContacts
                lblImportTypeDesc.Text = "Supplier Contacts"
                ImpTemplate = "VC Import.csv"

            Case cUtility.ImportType.VendorDetails
                lblImportTypeDesc.Text = "Supplier Details"
                ImpTemplate = "VD Import.csv"

            Case cUtility.ImportType.ContractProducts
                lblImportTypeDesc.Text = "Contract Product Details"
                ImpTemplate = "CP Import.csv"

            Case cUtility.ImportType.CodesUnits
                lblImportTypeDesc.Text = "Units"
                ImpTemplate = "C-Units Import.csv"

            Case cUtility.ImportType.CodesInflator
                lblImportTypeDesc.Text = "Inflator Metrics"
                ImpTemplate = "C-Inflator Import.csv"

            Case cUtility.ImportType.CodesSalesTax
                lblImportTypeDesc.Text = "Sales Tax"
                ImpTemplate = "C-SalesTax Import.csv"

            Case cUtility.ImportType.CodesContractStatus
                lblImportTypeDesc.Text = "Contract Status"
                ImpTemplate = "C-Contract Status.csv"

            Case cUtility.ImportType.CodesInvFrequency
                lblImportTypeDesc.Text = "Invoice Frequency"
                ImpTemplate = "C-Invoice Frequency.csv"

            Case cUtility.ImportType.CodesRechargeEntity
                lblImportTypeDesc.Text = "Recharge Entities"
                ImpTemplate = "C-RechargeEntity.csv"

            Case cUtility.ImportType.CodesSites
                lblImportTypeDesc.Text = "Recharge Sites"
                ImpTemplate = "C-Sites.csv"

            Case cUtility.ImportType.CodesVendorCategory
                lblImportTypeDesc.Text = "Supplier Categories"
                ImpTemplate = "C-VendorCat Import.csv"

            Case cUtility.ImportType.ContractProductRecharge
                lblImportTypeDesc.Text = "Contract Product - Recharge"
                ImpTemplate = "CP-Recharge Import.csv"

            Case cUtility.ImportType.ProductVendorAssoc
                lblImportTypeDesc.Text = "Product Supplier Association"
                ImpTemplate = "PV Import.csv"

            Case cUtility.ImportType.ContractSavings
                lblImportTypeDesc.Text = "Contract Savings"
                ImpTemplate = "CS Import.csv"

            Case cUtility.ImportType.CodesAccountCodes
                lblImportTypeDesc.Text = "Recharge Account Codes"
                ImpTemplate = "C-AccountCodes.csv"

            Case cUtility.ImportType.RechargeAssociations
                lblImportTypeDesc.Text = "Recharge Associations"
                ImpTemplate = "C-RechargeAssoc Import.csv"

            Case cUtility.ImportType.RechargeAssociationsUnq
                lblImportTypeDesc.Text = "Recharge Association (Unique index)"
                ImpTemplate = "C-RechargeAssocUq Import.csv"

            Case cUtility.ImportType.CodesProductCategory
                lblImportTypeDesc.Text = "Product Category"
                ImpTemplate = "C-ProductCat Import.csv"

            Case cUtility.ImportType.RechargeAssociationsBespoke
                lblImportTypeDesc.Text = "Recharge Association (Bespoke)"
                ImpTemplate = "C-RechargeAssocBespoke Import.csv"

            Case cUtility.ImportType.InvoiceDetails
                lblImportTypeDesc.Text = "Invoice Details"
                ImpTemplate = "ID Import.csv"

            Case cUtility.ImportType.UFListItems
                lblImportTypeDesc.Text = "User Field List Items"
                ImpTemplate = "UF-ListItems Import.csv"

            Case cUtility.ImportType.CodesContractCategory
                lblImportTypeDesc.Text = "Contract Category"
                ImpTemplate = "C-Contract Category.csv"

            Case Else
                lblImportTypeDesc.Text = "Unknown Type Specified"
                ImpTemplate = ""
                cmdImport.Enabled = False
                Exit Sub
        End Select

        ImpType = CType(Me.Tag, cUtility.ImportType)

        If LoadTemplate() = True Then
            CreateDestinationTable()
        Else
            Close()
        End If
    End Sub

    Private Sub cmdLoadSample_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadSample.Click
        If txtSrcFile.Text = "" Then
            MsgBox("Source file must be specified", MsgBoxStyle.Critical, "Framework Import Utility")
            Exit Sub
        End If

        With srcGrid
            .DataSource = Nothing
            .ResetDisplayLayout()
            .Layouts.Clear()
            st.Rows.Clear()
            st.Columns.Clear()
        End With

        Dim dc As DataColumn
        Dim dr As DataRow
        Dim x As Integer

        Dim csvParser As New csvParser.cCSV
        Dim csv_dset As DataTable
        Dim importRow As DataRow

        Try
            csvParser.DelimiterChar = txtDelimiter.Text.Trim
            csv_dset = csvParser.CSVToDataset(txtSrcFile.Text).Tables(0)

        Catch ex As Exception
            MsgBox("An Error occurred trying to parse the source file", MsgBoxStyle.Critical, "FWUtilities Error")
            Exit Sub
        End Try

        If csv_dset.Rows.Count > 0 Then
            If chkIgnoreFirstRow.Checked = False Then
                importRow = csv_dset.Rows(0)
            Else
                If csv_dset.Rows.Count > 1 Then
                    importRow = csv_dset.Rows(1)
                End If
            End If
        End If

        ImpColumns = csv_dset.Columns.Count

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.String")
        dc.ColumnName = "ColId"
        dc.ReadOnly = True
        st.Columns.Add(dc)

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.String")
        dc.ColumnName = "SourceField"
        st.Columns.Add(dc)

        For x = 1 To ImpColumns - 1
            dr = st.NewRow
            dr("ColId") = "Column " & (x).ToString
            dr("SourceField") = importRow.Item(x)
            st.Rows.Add(dr)
        Next

        srcGrid.DataSource = st
        srcGrid.DataBind()

        lblSrcFileStatus.Text = "Source File Opened Successfully"

        If LoadImportMapping() = True Then
            If MsgBox("An import mapping for this file has been found. Would you like to use it?", MsgBoxStyle.YesNo, "Import File") = MsgBoxResult.No Then
                Me.ImportedMapping = False
                srcGrid.Enabled = True
                dstGrid.Enabled = True
                chkIgnoreFirstRow.Enabled = True
                chkSaveMapping.Enabled = True
                txtDelimiter.Enabled = True
            Else
                srcGrid.Enabled = False
                dstGrid.Enabled = False
                chkIgnoreFirstRow.Enabled = False
                chkSaveMapping.Checked = False
                chkSaveMapping.Enabled = False
                txtDelimiter.Enabled = False
                lblSrcFileStatus.Text = "Import Mapping Loaded Successfully"
            End If
        End If
    End Sub

    Private Function LoadTemplate() As Boolean
        Dim strR As StreamReader
        Dim strLine As String
        Dim idx As Integer = 0
        Dim strElements(11) As String
        Dim fws As New cFWSettings

        If ImpTemplate = "" Then
            LoadTemplate = False
            Exit Function
        End If

        If File.Exists(Application.StartupPath & "\templates\" & Trim(ImpTemplate)) = False Then
            MsgBox("Template file for the selected import is missing." & vbNewLine & "Please contact Administrator for assistance", MsgBoxStyle.Critical, "Framework Import Utility")
            LoadTemplate = False
            Exit Function
        End If

        strR = New StreamReader(Application.StartupPath & "\templates\" & Trim(ImpTemplate))
        strR.DiscardBufferedData()

        If strR.Peek >= 0 Then
            ' skip the first row in the template as this is header text
            strLine = strR.ReadLine()

            While strR.Peek >= 0
                strLine = strR.ReadLine
                strElements = Split(strLine, ",")
                With ImpMap(idx)
                    .CoreTable = strElements(0)
                    .Field = strElements(1)
                    .AltDescription = strElements(2)
                    .Type = strElements(3)
                    .MaxSize = Val(strElements(4))
                    .isUnique = IIf(strElements(5).ToUpper = "N", False, True)
                    .isMandatory = IIf(strElements(6).ToUpper = "Y", True, False)
                    .RefField = strElements(7)
                    .LinkRef = strElements(8)
                    .LinkMatch = strElements(9)
                    .DefaultValueIfBlank = strElements(10)
                End With
                idx += 1
            End While
        End If

        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        ' load any user defined fields into the template
        If idx > 0 Then
            Dim db As New DBConnection(FWEmail.ConnectionString)
            Dim dRow As DataRow

            Dim dset As DataSet = GetUserFields(db, ImpMap(0).CoreTable)

            For Each dRow In dset.Tables(0).Rows
                With ImpMap(idx)
                    Select Case dRow.Item("udfTableName").ToString.ToLower
                        Case "userdefinedcontractdetails"
                            .CoreTable = "Contract_Details"
                            .UDF_IDFieldName = "contractid"
                        Case "userdefinedcontractproductdetails"
                            .CoreTable = "Contract_ProductDetails"
                            .UDF_IDFieldName = "contractproductid"
                        Case "userdefinedproductdetails"
                            .CoreTable = "ProductDetails"
                            .UDF_IDFieldName = "productid"
                        Case "userdefinedsupplierdetails"
                            .CoreTable = "supplier_details"
                            .UDF_IDFieldName = "supplierid"
                        Case "userdefinedinvoicedetails"
                            .CoreTable = "invoices"
                            .UDF_IDFieldName = "invoiceid"
                        Case "userdefinedsuppliercontacts"
                            .CoreTable = "supplier_contacts"
                            .UDF_IDFieldName = "contactid"
                        Case Else

                    End Select
                    .UDF_TableName = Trim(dRow.Item("udfTableName"))
                    .Field = "UDF" & Trim(Str(dRow.Item("UserDefineId")))
                    .AltDescription = Trim(dRow.Item("display_name"))
                    .isUnique = False
                    .isMandatory = False
                    .IsUserdefined = True
                    .RefField = "N"
                    .LinkRef = ""
                    .LinkMatch = ""
                    Select Case CType(dRow.Item("fieldType"), FieldType)
                        Case FieldType.Text, FieldType.DynamicHyperlink
                            .Type = "S"
                            .DefaultValueIfBlank = ""
                        Case FieldType.LargeText
                            .Type = "T"
                            .DefaultValueIfBlank = ""
                        Case FieldType.TickBox
                            .Type = "X"
                            .DefaultValueIfBlank = "0"
                        Case FieldType.DateTime
                            .Type = "D"
                            .DefaultValueIfBlank = ""
                        Case FieldType.List
                            .Type = "U"
                            .DefaultValueIfBlank = ""
                        Case FieldType.Integer, FieldType.Number, FieldType.Currency
                            .Type = "N"
                            .DefaultValueIfBlank = "0"
                        Case FieldType.RelationshipTextbox
                            Select Case CStr(dRow.Item("relatedTableName")).ToLower
                                Case "codes_rechargeentity"
                                    .Type = "S"
                                    .RefField = "Y"
                                    .LinkRef = "[Codes_RechargeEntity].[EntityId]"
                                    .LinkMatch = "[Codes_RechargeEntity].[Name]"
                                    .DefaultValueIfBlank = "0"
                                Case "companies"
                                    .Type = "S"
                                    .RefField = "Y"
                                    .LinkRef = "[companies].[companyId]"
                                    .LinkMatch = "[companies].[company]"
                                    .DefaultValueIfBlank = "0"
                                Case "employees"
                                    .Type = "S"
                                    .RefField = "Y"
                                    .LinkRef = "[employees].[employeeId]"
                                    .LinkMatch = "dbo.getEmployeeFullName(employeeId)" '"[employees].[username]"
                                    .DefaultValueIfBlank = "0"

                            End Select
                    End Select

                    Select Case .Type
                        Case "S", "U"
                            If IsDBNull(dRow.Item("maxlength")) Then
                                .MaxSize = 4000
                            Else
                                .MaxSize = dRow.Item("maxlength")
                            End If

                        Case Else
                            .MaxSize = 0
                    End Select

                End With
                idx += 1
            Next
        End If

        strR.Close()
        strR = Nothing
        LoadTemplate = True
    End Function

    Private Sub CreateDestinationTable()
        Dim dc As DataColumn
        Dim dr As DataRow
        Dim x As Integer

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.String")
        dc.ColumnName = "xmapField"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.String")
        dc.ColumnName = "FieldName"
        dc.ReadOnly = True
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.Int32")
        dc.ColumnName = "Idx"
        dc.ReadOnly = True
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.String")
        dc.ColumnName = "srcIdx"
        dt.Columns.Add(dc)

        dc = New DataColumn
        dc.DataType = System.Type.GetType("System.Object")
        dc.ColumnName = "default"
        dt.Columns.Add(dc)

        For x = 0 To ImpMap.Length - 1
            If ImpMap(x).Field <> "" Then
                dr = dt.NewRow

                If ImpMap(x).isMandatory = True Then
                    dr("xmapField") = "Mandatory"
                Else
                    dr("xmapField") = ""
                End If

                Select Case Mid(ImpMap(x).RefField, 1, 1)
                    Case "F" ' fixed input for all rows - will display ddlist
                        dr("xmapField") = "F=" & Trim(ImpMap(x).LinkMatch)
                        slFixedMappings.Add(Trim(ImpMap(x).LinkMatch), Trim(ImpMap(x).LinkRef))

                    Case "R"
                        dr("xmapField") = "R="

                    Case Else

                End Select

                ' log if a Default Value for blank mappings is present
                If ImpMap(x).DefaultValueIfBlank <> "" Then
                    dr("default") = ImpMap(x).DefaultValueIfBlank
                End If

                If ImpMap(x).AltDescription <> "" Then
                    dr("FieldName") = ImpMap(x).AltDescription
                Else
                    dr("FieldName") = ImpMap(x).Field
                End If
                dr("Idx") = x
                dt.Rows.Add(dr)
            End If
        Next

        dstGrid.DataSource = dt
        dstGrid.DataBind()
    End Sub

    Private Sub dstGrid_InitializeLayout(ByVal sender As Object, ByVal e As Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs) Handles dstGrid.InitializeLayout
        e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns

        e.Layout.Bands(0).Columns("FieldName").Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button
        e.Layout.Bands(0).Columns("FieldName").CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center
        e.Layout.Bands(0).Columns("FieldName").CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly
        e.Layout.Bands(0).Columns("FieldName").ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always
        e.Layout.Bands(0).Columns("FieldName").Header.Caption = "Database Field"
        e.Layout.Bands(0).Columns("FieldName").Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
        e.Layout.Bands(0).Columns("FieldName").CellButtonAppearance.ForeColor = Color.Black

        e.Layout.Bands(0).Columns("xmapField").Header.Caption = "Mapped Field"
        e.Layout.Bands(0).Columns("xmapField").Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True

        e.Layout.Bands(0).Columns("Idx").Hidden = True
        e.Layout.Bands(0).Columns("srcIdx").Hidden = True
        e.Layout.Bands(0).Columns("default").Hidden = True

        ' initialise for drag & drop
        With e.Layout.Override
            .SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Extended
            .CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
        End With

        ' turn on allowdrop
        Dim ug As Infragistics.Win.UltraWinGrid.UltraGrid = sender
        ug.AllowDrop = True

    End Sub

    Private Sub srcGrid_InitializeLayout(ByVal sender As Object, ByVal e As Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs) Handles srcGrid.InitializeLayout
        e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns

        e.Layout.Bands(0).Columns("ColId").Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button
        e.Layout.Bands(0).Columns("ColId").CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center
        e.Layout.Bands(0).Columns("ColId").CellButtonAppearance.ForeColor = Color.Black
        e.Layout.Bands(0).Columns("ColId").CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly
        e.Layout.Bands(0).Columns("ColId").ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always
        e.Layout.Bands(0).Columns("ColId").Header.Caption = "Column Id"
        e.Layout.Bands(0).Columns("ColId").Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True

        e.Layout.Bands(0).Columns("SourceField").Header.Caption = "Source Field"
        e.Layout.Bands(0).Columns("SourceField").Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
        e.Layout.Bands(0).Columns("SourceField").Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit
        e.Layout.Bands(0).Columns("SourceField").AutoCompleteMode = False
        e.Layout.Bands(0).Columns("SourceField").CellDisplayStyle = Infragistics.Win.UltraWinGrid.CellDisplayStyle.PlainText

        ' initialise for drag & drop
        With e.Layout.Override
            .SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Extended
            .CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect
        End With

        ' turn on allowdrop
        Dim ug As Infragistics.Win.UltraWinGrid.UltraGrid = sender
        ug.AllowDrop = True

    End Sub

    Private Sub dstGrid_InitializeRow(ByVal sender As Object, ByVal e As Infragistics.Win.UltraWinGrid.InitializeRowEventArgs) Handles dstGrid.InitializeRow
        If Mid(e.Row.Cells("xmapField").Value, 1, 2) = "F=" Then
            e.Row.Hidden = True
        End If

        If Mid(e.Row.Cells("xmapField").Value, 1, 2) = "R=" Then
            e.Row.Hidden = True
        End If
    End Sub

    Private Sub srcGrid_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles srcGrid.MouseDown
        Try
            Select Case e.Button
                Case Windows.Forms.MouseButtons.Left
                    b_srcGridLeftMouseDown = True

                Case Else

            End Select

        Catch ex As Exception

        End Try
    End Sub

    Private Sub srcGrid_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles srcGrid.MouseMove
        Try
            Select Case e.Button
                Case Windows.Forms.MouseButtons.Left
                    If b_srcGridLeftMouseDown = True Then
                        If srcGrid.Selected.Cells(0).Column.Index = cUtility.srcField.FieldName Then
                            Dim cd As Infragistics.Win.UltraWinGrid.UltraGridCell

                            cd = srcGrid.Selected.Cells(0).Row.Cells(cUtility.srcField.ColumnId)

                            srcGrid.DoDragDrop(CStr(cd.Value), DragDropEffects.Copy)
                        Else
                            b_srcGridLeftMouseDown = False
                        End If
                    End If
            End Select
        Catch ex As Exception

        End Try

    End Sub

    Private Sub srcGrid_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles srcGrid.MouseUp
        ' turn off the mouse down property
        b_srcGridLeftMouseDown = False
    End Sub

    Private Sub dstGrid_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dstGrid.DragEnter
        If b_srcGridLeftMouseDown = True Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub dstGrid_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dstGrid.DragOver
        If b_srcGridLeftMouseDown = True Then
            ' retrieve reference to grid
            Dim ug As Infragistics.Win.UltraWinGrid.UltraGrid = sender

            ' retrieve reference to cell
            Dim uiElement As Infragistics.Win.UIElement = ug.DisplayLayout.UIElement.ElementFromPoint(ug.PointToClient(New Point(e.X, e.Y)))

            Dim dstCell As Infragistics.Win.UltraWinGrid.UltraGridCell = uiElement.GetContext(GetType(Infragistics.Win.UltraWinGrid.UltraGridCell))

            If Not dstCell Is Nothing Then
                If dstCell.Column.Index = cUtility.dstField.xmapField Then
                    ' only allow copy if destination cell is in column 0
                    e.Effect = DragDropEffects.Copy
                Else
                    e.Effect = DragDropEffects.None
                End If
            Else
                e.Effect = DragDropEffects.None
            End If
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub dstGrid_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles dstGrid.DragDrop
        If b_srcGridLeftMouseDown = True Then
            ' retrieve reference to grid
            Dim ug As Infragistics.Win.UltraWinGrid.UltraGrid = sender

            ' retrieve reference to cell
            Dim uiElement As Infragistics.Win.UIElement = ug.DisplayLayout.UIElement.ElementFromPoint(ug.PointToClient(New Point(e.X, e.Y)))

            Dim dstCell As Infragistics.Win.UltraWinGrid.UltraGridCell = uiElement.GetContext(GetType(Infragistics.Win.UltraWinGrid.UltraGridCell))

            If Not dstCell Is Nothing Then
                ' retrieve data
                Dim dr As Infragistics.Win.UltraWinGrid.UltraGridRow

                dr = CType(e.Data.GetData("System.Object", True), Infragistics.Win.UltraWinGrid.UltraGridRow)

                Dim colStr As String
                Dim colIdx As Integer = Val(Replace(e.Data.GetData("System.String", True), "Column ", ""))

                colStr = srcGrid.Rows(colIdx - 1).Cells(cUtility.srcField.FieldName).Value

                ' dstCell.Value = e.Data.GetData("System.String", True)
                dstCell.Row.Cells(cUtility.dstField.srcIdx).Value = (colIdx - 1).ToString
                dstCell.Value = colStr
            End If
        End If
    End Sub

    Private Sub dstGrid_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dstGrid.MouseUp
        Select Case e.Button
            Case Windows.Forms.MouseButtons.Left
                b_srcGridLeftMouseDown = False

            Case Windows.Forms.MouseButtons.Right
                ' retrieve reference to cell
                Dim dlgres As DialogResult

                Dim ug As Infragistics.Win.UltraWinGrid.UltraGrid = sender

                Dim uiElement As Infragistics.Win.UIElement = ug.DisplayLayout.UIElement.ElementFromPoint(ug.PointToClient(New Point(Cursor.Position.X, Cursor.Position.Y)))

                Dim dstCell As Infragistics.Win.UltraWinGrid.UltraGridCell = uiElement.GetContext(GetType(Infragistics.Win.UltraWinGrid.UltraGridCell))

                If dstCell.Column.Index = cUtility.dstField.xmapField Then
                    If dstCell.Value <> "" Then
                        dlgres = MsgBox("Confirm deletion of the current mapping?", MsgBoxStyle.YesNo, "FWImport")
                        If dlgres = Windows.Forms.DialogResult.Yes Then
                            ' must be data in the cell, clear contents from the mapping
                            dstCell.Value = ""
                            dstCell.Row.Cells(cUtility.dstField.srcIdx).Value = ""
                        End If
                    End If
                End If

            Case Else

        End Select
    End Sub

    Private Function GetColumnMappings() As Boolean
        Dim x As Integer = 0
        Dim hasMappings As Boolean = False
        Dim nRow As Infragistics.Win.UltraWinGrid.RowEnumerator

        ReDim uiMapping(dstGrid.Rows.Count)

        nRow = dstGrid.Rows.GetEnumerator

        While nRow.MoveNext = True
            If IsDBNull(nRow.Current.Cells(cUtility.dstField.srcIdx).Value) = False Then
                If nRow.Current.Cells(cUtility.dstField.srcIdx).Value = "Mandatory field" Then
                    MsgBox("A mandatory field has no mapping.", MsgBoxStyle.Critical, "Framework Import")
                    hasMappings = False
                    Exit While
                End If

                uiMapping(x).Source_ColumnNo = Val(nRow.Current.Cells(cUtility.dstField.srcIdx).Value)
                uiMapping(x).ImpMap_idx = Val(nRow.Current.Cells(cUtility.dstField.Idx).Value)
                If IsDBNull(nRow.Current.Cells(cUtility.dstField.defaultvalue).Value) = False Then
                    uiMapping(x).DefaultValue = nRow.Current.Cells(cUtility.dstField.defaultvalue).Text
                End If
                x += 1
                hasMappings = True
            Else
                ' check if Dual Reference field (R=)
                If Mid(nRow.Current.Cells(cUtility.dstField.xmapField).Value, 1, 2) = "R=" Then
                    uiMapping(x).Source_ColumnNo = ColumnMappingCodes.DualReference
                    uiMapping(x).ImpMap_idx = Val(nRow.Current.Cells(cUtility.dstField.Idx).Value)
                    x += 1
                    hasMappings = True
                End If

                ' check DefaultIfBlank field and populate fixed value if so
                If IsDBNull(nRow.Current.Cells(cUtility.dstField.defaultvalue).Value) = False Then
                    uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault
                    uiMapping(x).ImpMap_idx = Val(nRow.Current.Cells(cUtility.dstField.Idx).Value)
                    uiMapping(x).DefaultValue = nRow.Current.Cells(cUtility.dstField.defaultvalue).Text
                    x += 1
                    hasMappings = True
                End If
            End If

            If Mid(nRow.Current.Cells(cUtility.dstField.xmapField).Value, 1, 2) = "F=" Then
                ' fixed mapping field, must be mapped
                uiMapping(x).Source_ColumnNo = ColumnMappingCodes.FixedSelection
                uiMapping(x).ImpMap_idx = Val(nRow.Current.Cells(cUtility.dstField.Idx).Value)
                x += 1
                hasMappings = True
            End If
        End While

        If x = 0 Then
            MsgBox("No fields have been mapped.", MsgBoxStyle.Information, "Framework Import")
        Else
            uiMappingCount = x - 1
            ReDim Preserve uiMapping(uiMappingCount)
        End If

        GetColumnMappings = hasMappings
    End Function

    Private Function ImportDataRow(ByVal data As DataRow, ByVal params As cAccountProperties, ByVal curRowNum As Integer) As Boolean
        Dim x As Integer
        Dim Success As Boolean

        Success = False
        Dim db As New DBConnection(FWEmail.ConnectionString)

        Try
            Dim sql, safe_sql, dataVal As String
            Dim collParams As New System.Collections.SortedList
            Dim collParams2 As New System.Collections.SortedList
            Dim sqlValues As New System.Text.StringBuilder
            Dim sqlFields As New System.Text.StringBuilder
            Dim sql2Values As New System.Text.StringBuilder
            Dim sql2Fields As New System.Text.StringBuilder
            Dim PrimaryTable, PrimaryField As String
            Dim isFirst, is2First As Boolean
            Dim ufFields As New Dictionary(Of String, Object)
            Dim ufIdField As String = ""
            Dim ufTableName As String = ""

            PrimaryTable = Trim(ImpMap(0).CoreTable)
            Select Case PrimaryTable
                Case "supplier_details"
                    PrimaryField = "primary_addressId"
                Case "supplier_contacts"
                    PrimaryField = "business_addressId"
                Case Else
                    PrimaryField = ""
            End Select

            safe_sql = "INSERT INTO [%table%] (%fields%) VALUES (%values%)"
            sql = safe_sql
            isFirst = True
            is2First = True

            If UserFieldId > 0 Then
                ' must insert user field id and value as static
                sqlFields.Append("[userdefineId]")
                sqlValues.Append(UserFieldId.ToString)
                isFirst = False
            End If

            For x = 0 To uiMapping.Length - 1
                Dim tmpRefField As String = ""
                Dim tmpLookupVal As String = ""

                tmpRefField = Mid(ImpMap(uiMapping(x).ImpMap_idx).RefField, 1, 1)

                Select Case tmpRefField 'Mid(ImpMap(uiMapping(x).ImpMap_idx).RefField, 1, 1)
                    Case "L", "T"
                        ' ignore because these ref types will be used by R

                    Case "R"
                        ' reference field that require >1 lookup ids to obtain the actual id to be stored in this field!
                        ' e.g. Contract-Product Id requires contract id & product id
                        Dim numVars, pos As Integer
                        Dim sql_criteria As New System.Text.StringBuilder
                        Dim arrMatchFields() As String

                        pos = InStr(ImpMap(uiMapping(x).ImpMap_idx).LinkRef, ".")
                        arrMatchFields = Split(ImpMap(uiMapping(x).ImpMap_idx).LinkMatch, "~")

                        sql_criteria.Append("SELECT " & Trim(ImpMap(uiMapping(x).ImpMap_idx).LinkRef))
                        sql_criteria.Append(" FROM " & Trim(Mid(ImpMap(uiMapping(x).ImpMap_idx).LinkRef, 1, pos - 1)) & " WHERE ")

                        numVars = Val(Mid(ImpMap(uiMapping(x).ImpMap_idx).RefField, 2))

                        Dim nVarLoop As Integer
                        Dim firstcriteria As Boolean
                        Dim refId As Integer
                        Dim sqlMatchVal As String = ""
                        Dim sqlMatchField As String = ""

                        firstcriteria = True

                        For nVarLoop = 1 To numVars
                            ' find impmap fields for L1,L2 etc.
                            Dim mapRow, n As Integer
                            For n = 0 To uiMapping.Length - 1
                                System.Diagnostics.Debug.WriteLine("ImpMap(uiMapping(n).ImpMap_idx).RefField = " & ImpMap(uiMapping(n).ImpMap_idx).RefField)
                                If ImpMap(uiMapping(n).ImpMap_idx).RefField = "L" & nVarLoop.ToString.Trim Or ImpMap(uiMapping(n).ImpMap_idx).RefField = "T" & nVarLoop.ToString.Trim Then
                                    ' found the field
                                    mapRow = n

                                    tmpLookupVal = Trim(data(uiMapping(mapRow).Source_ColumnNo + 1))

                                    If ImpMap(uiMapping(n).ImpMap_idx).RefField.Substring(0, 1) = "L" Then
                                        Dim SearchTable As String

                                        SearchTable = Mid(ImpMap(uiMapping(mapRow).ImpMap_idx).LinkMatch, 1, InStr(ImpMap(uiMapping(mapRow).ImpMap_idx).LinkMatch, ".") - 1)
                                        If SearchTable = "dbo" Then
                                            ' will be dbo if using a function to match value, so use ref to obtain the search table
                                            SearchTable = Mid(ImpMap(uiMapping(mapRow).ImpMap_idx).LinkRef, 1, InStr(ImpMap(uiMapping(mapRow).ImpMap_idx).LinkRef, ".") - 1)
                                        End If

                                        refId = LookupReference(False, SearchTable, ImpMap(uiMapping(mapRow).ImpMap_idx).LinkMatch, tmpLookupVal, ImpMap(uiMapping(mapRow).ImpMap_idx).LinkRef)

                                        If refId <= 0 Then
                                            ' lookup failed for part of the multi-part ref lookup
                                            Log("Multi-part lookup failure for L" & nVarLoop.ToString.Trim & " for " & tmpLookupVal.Trim & " for source row " & curRowNum.ToString)
                                            Exit Function
                                        End If

                                        sqlMatchField = arrMatchFields(nVarLoop - 1)
                                        sqlMatchVal = refId.ToString.Trim
                                    Else
                                        ' must be matching a string Field
                                        sqlMatchField = "LOWER(" & arrMatchFields(nVarLoop - 1) & ")"
                                        sqlMatchVal = "LOWER('" & tmpLookupVal.Trim & "')"
                                    End If

                                    If firstcriteria = False Then
                                        sql_criteria.Append(" AND ")
                                    End If
                                    sql_criteria.Append(sqlMatchField & " = " & sqlMatchVal)
                                    firstcriteria = False
                                    Exit For
                                End If
                            Next
                        Next

                        ' should have full lookup statement to find the id to store
                        Dim IDdset As New DataSet

                        IDdset = doQuery(sql_criteria.ToString)

                        If Not IDdset Is Nothing Then
                            Try
                                Dim lookupField As String = ""
                                pos = InStr(ImpMap(uiMapping(x).ImpMap_idx).LinkRef, ".")

                                lookupField = Mid(ImpMap(uiMapping(x).ImpMap_idx).LinkRef, pos + 1)
                                lookupField = lookupField.Replace("[", "")
                                lookupField = lookupField.Replace("]", "")

                                refId = IDdset.Tables(0).Rows(0).Item(lookupField)

                            Catch ex As Exception
                                Log("Indirect Lookup failed for " & tmpLookupVal)
                                'Debug.WriteLine(ex.Message)
                                refId = -1
                            End Try

                            IDdset.Dispose()
                            IDdset = Nothing

                            If refId > 0 Then
                                If isFirst = False Then
                                    sqlFields.Append(",")
                                    sqlValues.Append(",")
                                End If
                                sqlFields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")
                                sqlValues.Append(refId.ToString.Trim)
                                isFirst = False
                            Else
                                Success = False
                                'If isFirst = False Then
                                '    sqlFields += ","
                                '    sqlValues += ","
                                'End If
                                'sqlFields += "[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]"
                                'sqlValues += "0"
                                'isFirst = False
                                ImportDataRow = Success

                                If refId = -1 Then
                                    Log("Lookup for " & ImpMap(uiMapping(x).ImpMap_idx).AltDescription & " reference failed for " & data(uiMapping(x).Source_ColumnNo + 1))
                                End If
                                Exit Function
                            End If
                        End If

                    Case "S" ' static reference
                        Dim staticRef As String
                        staticRef = Replace(Trim(ImpMap(uiMapping(x).ImpMap_idx).LinkRef), "%", "")

                        Dim refId As Integer = -1

                        If uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault Then
                            ' must be unmapped so use the default value
                            refId = uiMapping(x).DefaultValue
                        Else
                            Select Case staticRef
                                Case "ApportionType"
                                    Select Case Trim(LCase(data(uiMapping(x).Source_ColumnNo + 1)))
                                        Case "%", "percent", "percentage"
                                            refId = RechargeApportionType.Percentage
                                        Case "fixed", "$", "£"
                                            refId = RechargeApportionType.Fixed
                                        Case "units", "n_units"
                                            refId = RechargeApportionType.n_Units
                                        Case Else
                                            refId = -1
                                    End Select

                                Case "SurchargeType"
                                    Select Case Trim(LCase(data(uiMapping(x).Source_ColumnNo + 1)))
                                        Case "%", "percent", "percentage"
                                            refId = SurchargeType.Percentage
                                        Case "fixed", "$", "£"
                                            refId = SurchargeType.Fixed
                                        Case Else
                                            refId = -1
                                    End Select

                                Case "MaintForecastTypes"
                                    Select Case Trim(LCase(data(uiMapping(x).Source_ColumnNo + 1)))
                                        Case ">", "greater", ">xy", ">x&y"
                                            refId = MaintType.GreaterXY
                                        Case "<", "lesser", "<xy", "<x&y"
                                            refId = MaintType.LesserXY
                                        Case "=", "single", "x"
                                            refId = MaintType.SingleInflator
                                        Case Else
                                            refId = -1
                                    End Select

                                Case "MaintTypes"
                                    Select Case Trim(LCase(data(uiMapping(x).Source_ColumnNo + 1)))
                                        Case "single", "inflator", "inflatoronly"
                                            refId = ForecastType.InflatorOnly
                                        Case "compare", "v", "product", "both"
                                            refId = ForecastType.Prod_v_Inflator
                                        Case "staged", "stepped", "steps", "step"
                                            refId = ForecastType.Staged
                                        Case Else
                                            refId = -1
                                    End Select
                                Case Else
                            End Select
                        End If

                        If refId > -1 Then
                            If isFirst = False Then
                                sqlFields.Append(",")
                                sqlValues.Append(",")
                            End If
                            sqlFields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")
                            collParams.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), refId)
                            sqlValues.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field).Trim)
                            isFirst = False
                        Else
                            Success = False
                            ImportDataRow = Success

                            Log("Lookup for " & ImpMap(uiMapping(x).ImpMap_idx).AltDescription & " reference failed for " & data(uiMapping(x).Source_ColumnNo + 1))
                            Exit Function
                        End If

                    Case "Y" ' yes - standard lookup reference
                        ' must obtain a reference for the src value, so search and hopefully return a numeric id to store in the dst field
                        Dim refId As Integer
                        Dim SearchTable As String

                        If uiMapping(x).Source_ColumnNo <> ColumnMappingCodes.UnmappedDefault Then
                            ' make sure we are doing lookup for a default value!
                            SearchTable = Mid(ImpMap(uiMapping(x).ImpMap_idx).LinkMatch, 1, InStr(ImpMap(uiMapping(x).ImpMap_idx).LinkMatch, ".") - 1)
                            If SearchTable = "dbo" Then
                                ' using a function to match, so get table from link reference table (e.g dbo.getEmployeeFullName()
                                SearchTable = Mid(ImpMap(uiMapping(x).ImpMap_idx).LinkRef, 1, InStr(ImpMap(uiMapping(x).ImpMap_idx).LinkRef, ".") - 1)
                            End If
                            refId = LookupReference(False, SearchTable, ImpMap(uiMapping(x).ImpMap_idx).LinkMatch, Trim(data(uiMapping(x).Source_ColumnNo + 1)), ImpMap(uiMapping(x).ImpMap_idx).LinkRef)

                            If refId > 0 Then
                                If ImpMap(uiMapping(x).ImpMap_idx).IsUserdefined = False Then
                                    If ImpMap(uiMapping(x).ImpMap_idx).CoreTable = PrimaryTable Then
                                        If isFirst = False Then
                                            sqlFields.Append(",")
                                            sqlValues.Append(",")
                                        End If
                                        sqlFields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")
                                        collParams.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), refId)
                                        sqlValues.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                                    Else
                                        If is2First = False Then
                                            sql2Fields.Append(",")
                                            sql2Values.Append(",")
                                        End If
                                        sql2Fields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")
                                        collParams2.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), refId)
                                        sql2Values.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                                    End If
                                    isFirst = False
                                Else
                                    ufFields.Add(ImpMap(uiMapping(x).ImpMap_idx).Field, refId)
                                    ufIdField = ImpMap(uiMapping(x).ImpMap_idx).UDF_IDFieldName
                                    ufTableName = ImpMap(uiMapping(x).ImpMap_idx).UDF_TableName
                                End If
                            Else
                                ' if return is -2, means blank search value for LookupReference, so just skip over, otherwise report as failed row
                                If refId = -1 Then
                                    Success = False
                                    ImportDataRow = Success

                                    Log("Lookup for " & ImpMap(uiMapping(x).ImpMap_idx).AltDescription & " reference failed for " & data(uiMapping(x).Source_ColumnNo + 1))
                                    Exit Function
                                End If
                            End If
                        End If

                    Case "F" ' fixed selection e.g. SubAccount
                        If PrimaryTable <> "contract_productdetails" Then
                            ' subaccountid is picked as part of import, but the field doesn't exist on the table, so don't include in the import
                            If isFirst = False Then
                                sqlFields.Append(",")
                                sqlValues.Append(",")
                            End If
                            sqlFields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")
                            collParams.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), Trim(Str(slFixedMappings(ImpMap(uiMapping(x).ImpMap_idx).LinkMatch))))
                            sqlValues.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                            isFirst = False
                        End If
                    Case Else
                        Select Case ImpMap(uiMapping(x).ImpMap_idx).Type
                            Case "S", "T"
                                If uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault Then
                                    ' must be populating a default value
                                    dataVal = Trim(uiMapping(x).DefaultValue)
                                Else
                                    If IsDBNull(data(uiMapping(x).Source_ColumnNo + 1)) = False Then
                                        dataVal = Trim(data(uiMapping(x).Source_ColumnNo + 1))
                                    Else
                                        dataVal = ""
                                    End If
                                End If

                            Case "D"
                                If uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault Then
                                    ' must be populating a default value
                                    If IsDate(ImpMap(uiMapping(x).DefaultValue)) = True Then
                                        dataVal = Format(CDate(uiMapping(x).DefaultValue), "yyyy-MM-dd 00:00:00")
                                    Else
                                        dataVal = "NULL"
                                    End If
                                Else
                                    If data(uiMapping(x).Source_ColumnNo + 1) <> "" Then
                                        If IsDate(data(uiMapping(x).Source_ColumnNo + 1)) = True Then
                                            dataVal = Trim(Format(CDate(data(uiMapping(x).Source_ColumnNo + 1)), "yyyy-MM-dd 00:00:00"))
                                        Else
                                            dataVal = "NULL"
                                        End If

                                    Else
                                        dataVal = "NULL"
                                    End If
                                End If

                            Case "X"
                                If uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault Then
                                    ' must be populating a default value
                                    If uiMapping(x).DefaultValue <> "1" Then
                                        dataVal = "0"
                                    Else
                                        dataVal = "1"
                                    End If
                                Else
                                    Select Case UCase(Trim(data(uiMapping(x).Source_ColumnNo + 1)))
                                        Case "YES", "Y", "1", "TRUE"
                                            dataVal = "1"
                                        Case Else
                                            dataVal = "0"
                                    End Select
                                End If

                            Case "U"
                                ' user defined ddlist - check text entry exists before accepting
                                Dim tmpListItems As New Dictionary(Of String, Integer)

                                If UFDDLists.ContainsKey(ImpMap(uiMapping(x).ImpMap_idx).Field) = False Then
                                    tmpListItems = GetUFListItems(ImpMap(uiMapping(x).ImpMap_idx).Field.Replace("UDF", ""))
                                    If Not tmpListItems Is Nothing Then
                                        UFDDLists.Add(ImpMap(uiMapping(x).ImpMap_idx).Field, tmpListItems)
                                    End If
                                Else
                                    tmpListItems = UFDDLists(ImpMap(uiMapping(x).ImpMap_idx).Field)
                                End If

                                Dim tmpItem As String
                                If uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault Then
                                    dataVal = uiMapping(x).DefaultValue
                                Else
                                    tmpItem = Trim(data(uiMapping(x).Source_ColumnNo + 1))
                                    If tmpItem.Trim <> "" Then
                                        If tmpListItems.ContainsKey(tmpItem.ToLower) = True Then
                                            dataVal = tmpListItems(tmpItem.ToLower)
                                        Else
                                            Success = False
                                            ImportDataRow = Success

                                            Log("User Field DDList Lookup for " & ImpMap(uiMapping(x).ImpMap_idx).AltDescription & " failed for " & tmpItem)
                                            Exit Function
                                        End If
                                    Else
                                        dataVal = ""
                                    End If
                                End If
                            Case Else
                                ' only number, float left
                                If uiMapping(x).Source_ColumnNo = ColumnMappingCodes.UnmappedDefault Then
                                    ' must be populating a default value
                                    dataVal = uiMapping(x).DefaultValue
                                Else
                                    If Trim(data(uiMapping(x).Source_ColumnNo + 1)) = "" Then
                                        dataVal = uiMapping(x).DefaultValue
                                    Else
                                        dataVal = Trim(data(uiMapping(x).Source_ColumnNo + 1))
                                    End If
                                End If
                        End Select

                        ' if field is unique, check for duplicates
                        If ImpMap(uiMapping(x).ImpMap_idx).isUnique = True Then
                            Dim additionalF, additionalV As Object

                            If PrimaryTable = "employees" Or slFixedMappings("accountsSubAccounts.Description") Is Nothing Then
                                additionalF = ""
                                additionalV = Nothing
                            Else
                                additionalF = "[subAccountId]"
                                additionalV = slFixedMappings("accountsSubAccounts.Description")
                            End If

                            If UserFieldId > 0 Then
                                ' must be importing uf list items
                                additionalF = "[userdefineId]"
                                additionalV = UserFieldId.ToString
                            End If

                            If duplicateExists(params, ImpMap(uiMapping(x).ImpMap_idx).CoreTable, ImpMap(uiMapping(x).ImpMap_idx).Field, dataVal, additionalF, additionalV) = True Then
                                Log("Field must be Unique. Duplicate value for " & dataVal.Trim & ". Skipping row")
                                ImportDataRow = False
                                Exit Function
                            End If
                        End If

                        If ImpMap(uiMapping(x).ImpMap_idx).IsUserdefined = False Then
                            If ImpMap(uiMapping(x).ImpMap_idx).CoreTable = PrimaryTable Then
                                If isFirst = False Then
                                    sqlFields.Append(",")
                                    sqlValues.Append(",")
                                End If

                                sqlFields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")

                                If ImpMap(uiMapping(x).ImpMap_idx).Type = "D" Then
                                    If dataVal = "NULL" Then
                                        sqlValues.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                                        collParams.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), DBNull.Value)
                                    Else
                                        collParams.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), dataVal)
                                        sqlValues.Append("CONVERT(datetime,@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field) & ",120)")
                                    End If
                                Else
                                    collParams.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), dataVal)
                                    sqlValues.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                                End If

                                isFirst = False
                            Else
                                If is2First = False Then
                                    sql2Fields.Append(",")
                                    sql2Values.Append(",")
                                End If

                                sql2Fields.Append("[" & ImpMap(uiMapping(x).ImpMap_idx).Field & "]")
                                If ImpMap(uiMapping(x).ImpMap_idx).Type = "D" Then
                                    If dataVal = "NULL" Then
                                        sql2Values.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                                        collParams2.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), DBNull.Value)
                                    Else
                                        sql2Values.Append("CONVERT(datetime,@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field) & ",120)")
                                        collParams2.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), dataVal)
                                    End If
                                Else
                                    sql2Values.Append("@" & ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field))
                                    collParams2.Add(ConvertToKey(ImpMap(uiMapping(x).ImpMap_idx).Field), dataVal)
                                End If
                                is2First = False
                            End If
                        Else
                            If ImpMap(uiMapping(x).ImpMap_idx).Type = "D" Then
                                If dataVal = "NULL" Then
                                    ufFields.Add(ImpMap(uiMapping(x).ImpMap_idx).Field, DBNull.Value)
                                Else
                                    ufFields.Add(ImpMap(uiMapping(x).ImpMap_idx).Field, dataVal)
                                End If
                            Else
                                ufFields.Add(ImpMap(uiMapping(x).ImpMap_idx).Field, dataVal)
                            End If
                            ufIdField = ImpMap(uiMapping(x).ImpMap_idx).UDF_IDFieldName
                            ufTableName = ImpMap(uiMapping(x).ImpMap_idx).UDF_TableName
                        End If
                End Select
            Next

            If needsCreatedOn(ImpMap(0).CoreTable) Then
                sqlFields.Append(", createdOn, createdBy")
                sqlValues.Append(", @createdOn, @createdBy")
                collParams.Add("createdOn", Now)
                collParams.Add("createdBy", FWEmail.AdminId)
            ElseIf ImpMap(0).CoreTable = "contract_productdetails" Then
                sqlFields.Append(", createdDate")
                sqlValues.Append(", @createdDate")
                collParams.Add("createdDate", Now)
            End If

            sql = Replace(sql, "%table%", PrimaryTable)
            sql = Replace(sql, "%fields%", sqlFields.ToString)
            sql = Replace(sql, "%values%", sqlValues.ToString)
            sql += "; SELECT @@IDENTITY AS [NewId]"

            Dim dset As New DataSet
            Dim xParam As Integer
            db.sqlexecute.Parameters.Clear()

            For xParam = 0 To collParams.Count - 1
                db.sqlexecute.Parameters.AddWithValue("@" & collParams.GetKey(xParam), collParams(collParams.GetKey(xParam)))
            Next

            Try
                dset = db.GetDataSet(sql)
            Catch ex As Exception
                Log("SQL Error : " & ex.Message)
            End Try

            If dset.Tables(0).Rows(0).Item("NewId") > 0 Then
                Dim newId As Integer = dset.Tables(0).Rows(0).Item("NewId")

                ' add the entries for any udfs
                If ufFields.Count > 0 Then
                    Dim tmpUFields As String = ""
                    Dim tmpUFVals As String = ""
                    Dim comma As String = ""
                    Dim ufSQLStr As String = safe_sql

                    db.sqlexecute.Parameters.Clear()

                    For Each rec As KeyValuePair(Of String, Object) In ufFields
                        tmpUFields += comma & "[" & rec.Key & "]"
                        tmpUFVals += comma & "@" & rec.Key.Replace(" ", "_")
                        db.sqlexecute.Parameters.AddWithValue("@" & rec.Key.Replace(" ", "_"), rec.Value)
                        comma = ","
                    Next

                    ' add the id field and value
                    tmpUFields += ", [" & ufIdField & "]"
                    tmpUFVals += ", @" & ufIdField.Replace(" ", "_")
                    db.sqlexecute.Parameters.AddWithValue("@" & ufIdField.Replace(" ", "_"), newId)

                    ufSQLStr = ufSQLStr.Replace("%fields%", tmpUFields)
                    ufSQLStr = ufSQLStr.Replace("%values%", tmpUFVals)
                    ufSQLStr = ufSQLStr.Replace("%table%", ufTableName)

                    db.ExecuteSQL(ufSQLStr)
                End If

                Success = True

                db.sqlexecute.Parameters.Clear()

                ' must split the Supplier Details over two tables
                Select Case ImpMap(0).CoreTable
                    Case "supplier_details"
                        If is2First = False Then
                            sql2Fields.Append(",")
                            sql2Values.Append(",")
                        End If

                        sql2Fields.Append("[supplierId],[createdOn]")
                        sql2Values.Append(newId.ToString & ", getdate()")
                        is2First = False

                        ' write back reference id to the primary table
                        sql = safe_sql
                        sql = Replace(sql, "%table%", "supplier_addresses")
                        sql = Replace(sql, "%fields%", sql2Fields.ToString)
                        sql = Replace(sql, "%values%", sql2Values.ToString)
                        sql += " SELECT @@IDENTITY AS [AdrId]"

                        Dim Adr_dset As New DataSet

                        Dim innerdb As New DBConnection(FWEmail.ConnectionString)

                        Dim innerxParam As Integer
                        innerdb.sqlexecute.Parameters.Clear()
                        For innerxParam = 0 To collParams2.Count - 1
                            innerdb.sqlexecute.Parameters.AddWithValue("@" & collParams2.GetKey(innerxParam), collParams2(collParams2.GetKey(innerxParam)))
                        Next

                        Try
                            Adr_dset = innerdb.GetDataSet(sql)

                            If Adr_dset.Tables(0).Rows(0).Item("AdrId") > 0 Then
                                sql = "UPDATE [" & PrimaryTable & "] SET [modifiedOn] = getdate(), [" & PrimaryField & "] = @AdrId WHERE [supplierId] = @supplierId"
                                innerdb.sqlexecute.Parameters.Clear()
                                innerdb.sqlexecute.Parameters.AddWithValue("@supplierId", newId)
                                innerdb.sqlexecute.Parameters.AddWithValue("@AdrId", Adr_dset.Tables(0).Rows(0).Item("AdrId"))
                                'db.RunSQL(sql, db.glDBWorkB)
                                innerdb.ExecuteSQL(sql)
                            End If
                        Catch ex As Exception
                            Log("SQL Error : " & ex.Message)
                        End Try

                    Case "supplier_contacts"
                        If is2First = False Then
                            sql2Fields.Append(",")
                            sql2Values.Append(",")
                        End If

                        sql2Fields.Append("[createdOn]")
                        sql2Values.Append("getdate()")
                        is2First = False

                        ' write back reference id to the primary table
                        sql = safe_sql
                        sql = Replace(sql, "%table%", "supplier_addresses")
                        sql = Replace(sql, "%fields%", sql2Fields.ToString)
                        sql = Replace(sql, "%values%", sql2Values.ToString)
                        sql += " SELECT @@IDENTITY AS [AdrId]"

                        Dim Adr_dset As New DataSet

                        Dim innerdb As New DBConnection(FWEmail.ConnectionString)

                        Dim innerxParam As Integer
                        innerdb.sqlexecute.Parameters.Clear()
                        For innerxParam = 0 To collParams2.Count - 1
                            innerdb.sqlexecute.Parameters.AddWithValue("@" & collParams2.GetKey(innerxParam), collParams2(collParams2.GetKey(innerxParam)))
                        Next

                        Try
                            Adr_dset = innerdb.GetDataSet(sql)

                            If Adr_dset.Tables(0).Rows(0).Item("AdrId") > 0 Then
                                sql = "UPDATE [" & PrimaryTable & "] SET [modifiedOn] = getdate(), [" & PrimaryField & "] = @AdrId WHERE [contactId] = @contactId"
                                innerdb.sqlexecute.Parameters.Clear()
                                innerdb.sqlexecute.Parameters.AddWithValue("@contactId", newId)
                                innerdb.sqlexecute.Parameters.AddWithValue("@AdrId", Adr_dset.Tables(0).Rows(0).Item("AdrId"))
                                'db.RunSQL(sql, db.glDBWorkB)
                                innerdb.ExecuteSQL(sql)
                            End If
                        Catch ex As Exception
                            Log("SQL Error : " & ex.Message)
                        End Try
                    Case "contract_details"
                        ' update the contract key
                        Dim cKey As String
                        Dim c_dset As New DataSet

                        Try
                            If params.ContractKey <> "" Then
                                sql = "SELECT [contractId],[supplierId] FROM contract_details WHERE [ContractId] = @conId"
                                db.sqlexecute.Parameters.AddWithValue("@conId", newId)
                                c_dset = db.GetDataSet(sql)

                                cKey = params.ContractKey & "/" & Trim(c_dset.Tables(0).Rows(0).Item("contractId")) & "/" & Trim(c_dset.Tables(0).Rows(0).Item("supplierId"))

                                Dim condb As New DBConnection(FWEmail.ConnectionString)

                                sql = "UPDATE contract_details SET [contractKey] = @cKey, [createdOn] = getdate() WHERE [contractId] = @conId"
                                condb.sqlexecute.Parameters.AddWithValue("@conId", c_dset.Tables(0).Rows(0).Item("contractId"))
                                condb.sqlexecute.Parameters.AddWithValue("@cKey", cKey)

                                Try
                                    condb.ExecuteSQL(sql)
                                Catch ex As Exception
                                    Log("SQL Error : " & ex.Message)
                                End Try
                            End If

                        Catch ex As Exception
                            Log("Error updating the Contract Key")
                            Log(ex.Message)
                        End Try

                        c_dset.Dispose()
                        c_dset = Nothing

                    Case "contract_productdetails"
                        Try
                            sql = "UPDATE contract_productdetails SET [CreatedDate] = GETDATE() WHERE [ContractProductId] = @cpId"
                            db.sqlexecute.Parameters.Clear()
                            db.sqlexecute.Parameters.AddWithValue("@cpId", newId)

                            Try
                                db.ExecuteSQL(sql)
                            Catch ex As Exception
                                Log("SQL Error : " & ex.Message)
                            End Try

                        Catch ex As Exception
                            Log("Error updating the Contract Product create date")
                            Log(ex.Message)

                        End Try
                End Select
            End If

        Catch ex As Exception
            Log("Error occurred during import to Framework at source row " & curRowNum.ToString.Trim)
            Log("Message : " & ex.Message)
            Success = False
        End Try

        ImportDataRow = Success
    End Function

    Private Function ValidateRow(ByVal importRow As DataRow, ByVal currentRow As Integer) As Boolean
        Try
            ' check mapped columns for type checks
            ' NOTE: Column 0 from csvParser is validate columns, so +1 offset for source column references.
            Dim x As Integer
            Dim isValid As Boolean

            isValid = True

            For x = 0 To uiMapping.Length - 1
                ' if field is reference, no need to validate
                If ImpMap(uiMapping(x).ImpMap_idx).RefField <> "F" And Mid(ImpMap(uiMapping(x).ImpMap_idx).RefField, 1, 1) <> "R" Then

                    ' check if the field is mandatory and if so, ensure data is present
                    If ImpMap(uiMapping(x).ImpMap_idx).isMandatory = True Then
                        If IsDBNull(importRow.Item(uiMapping(x).Source_ColumnNo + 1)) = True Then
                            ' field cannot be blank
                            isValid = False
                            Log("Mandatory field missing : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)

                        ElseIf CStr(importRow.Item(uiMapping(x).Source_ColumnNo + 1)).Trim = "" Then
                            ' field cannot be blank
                            isValid = False
                            Log("Mandatory field missing : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)
                        End If
                    End If

                    If uiMapping(x).Source_ColumnNo <> ColumnMappingCodes.UnmappedDefault Then
                        If IsDBNull(importRow.Item(uiMapping(x).Source_ColumnNo + 1)) = False Then
                            If Trim(importRow.Item(uiMapping(x).Source_ColumnNo + 1) <> "") Then
                                ' look up field type in the destination template
                                Select Case ImpMap(uiMapping(x).ImpMap_idx).Type
                                    Case "S", "U" ' string or uf ddlist
                                        ' check lengh of string field
                                        If Len(importRow.Item(uiMapping(x).Source_ColumnNo + 1)) > ImpMap(uiMapping(x).ImpMap_idx).MaxSize Then
                                            isValid = False
                                            Log("String input field is too long : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)
                                        End If
                                    Case "T"

                                    Case "N", "F" ' integer or float
                                        If IsNumeric(Trim(importRow.Item(uiMapping(x).Source_ColumnNo + 1))) = False Then
                                            isValid = False
                                            Log("Input field is not numeric : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)
                                        End If

                                    Case "D" ' date
                                        If IsDate(importRow.Item(uiMapping(x).Source_ColumnNo + 1)) = False Then
                                            isValid = False
                                            Log("Input field is not a valid date format : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)
                                        End If

                                    Case "X" ' checkbox or boolean (translated to 0 or 1
                                        Select Case UCase(importRow.Item(uiMapping(x).Source_ColumnNo + 1).ToString)
                                            Case "YES", "Y", "N", "NO", "1", "0"
                                                ' valid row
                                            Case Else
                                                isValid = False
                                                Log("Field invalid for checkbox type : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)

                                        End Select
                                    Case Else
                                        Log("Invalid field type encountered : column " & Trim(Str(uiMapping(x).Source_ColumnNo + 1)) & " : row " & currentRow.ToString)

                                End Select
                            End If
                        End If
                    End If
                End If
            Next

            Return isValid

        Catch ex As Exception
            ' bad error occurred
            Log("Error occurred validating import row")
            Log(ex.Message)
            MsgBox("An error occurred validating an import row" & vbNewLine & ex.Message)
            Return False
        End Try
    End Function

    Private Sub Log(ByVal Msg As String)
        ' This routine time-stamps the message and writes it to the log file
        Dim logfile As StreamWriter
        Dim filename As String
        Dim newfile As Boolean
        Dim ImpType, ImpDesc, datestamp As String

        Try
            Select Case CType(Me.Tag, cUtility.ImportType)
                Case cUtility.ImportType.ContractDetails
                    ImpDesc = "Contract Details"
                    ImpType = "CD"

                Case cUtility.ImportType.ContractProducts
                    ImpDesc = "Contract Products"
                    ImpType = "CP"

                Case cUtility.ImportType.ProductDetails
                    ImpDesc = "Product Details"
                    ImpType = "PD"

                Case cUtility.ImportType.StaffDetails
                    ImpDesc = "Employees"
                    ImpType = "SD"

                Case cUtility.ImportType.VendorContacts
                    ImpDesc = "Supplier Contacts"
                    ImpType = "VC"

                Case cUtility.ImportType.VendorDetails
                    ImpDesc = "Supplier Details"
                    ImpType = "VD"

                Case cUtility.ImportType.CodesRechargeEntity
                    ImpDesc = "Recharge Entities"
                    ImpType = "CM-RE"

                Case cUtility.ImportType.CodesContractStatus
                    ImpDesc = "Contract Status"
                    ImpType = "CM-CS"

                Case cUtility.ImportType.CodesInflator
                    ImpDesc = "Inflator Metrics"
                    ImpType = "CM-IM"

                Case cUtility.ImportType.CodesInvFrequency
                    ImpDesc = "Invoice Frequency"
                    ImpType = "CM-IF"

                Case cUtility.ImportType.CodesSalesTax
                    ImpDesc = "Sales Tax"
                    ImpType = "CM-ST"

                Case cUtility.ImportType.CodesUnits
                    ImpDesc = "Units"
                    ImpType = "CM-U"

                Case cUtility.ImportType.CodesSites
                    ImpDesc = "Recharge Sites"
                    ImpType = "CM-RS"

                Case cUtility.ImportType.CodesVendorCategory
                    ImpDesc = "Supplier Categories"
                    ImpType = "CM-VC"

                Case cUtility.ImportType.ProductVendorAssoc
                    ImpDesc = "Product Supplier Association"
                    ImpType = "PV"

                Case cUtility.ImportType.ContractProductRecharge
                    ImpDesc = "Contract Product - Recharge"
                    ImpType = "CP-R"

                Case cUtility.ImportType.ContractSavings
                    ImpDesc = "Contract Savings"
                    ImpType = "CS"

                Case cUtility.ImportType.CodesAccountCodes
                    ImpDesc = "Recharge Account Codes"
                    ImpType = "C-AC"

                Case cUtility.ImportType.RechargeAssociations
                    ImpDesc = "Contract Recharge Associations"
                    ImpType = "C-RA"

                Case cUtility.ImportType.RechargeAssociationsUnq
                    ImpDesc = "Contract Recharge Associations (Unique Index)"
                    ImpType = "C-RA-Unique"

                Case cUtility.ImportType.CodesProductCategory
                    ImpDesc = "Product Category Definitions"
                    ImpType = "C-PC"

                Case cUtility.ImportType.RechargeAssociationsBespoke
                    ImpDesc = "Contract Recharge Associations (Bespoke)"
                    ImpType = "C-RA-Bespoke"

                Case cUtility.ImportType.CodesContractCategory
                    ImpDesc = "Contract Categories"
                    ImpType = "C-CT"

                Case Else
                    ImpDesc = "Unknown data type"
                    ImpType = "XX"
            End Select

            filename = Application.StartupPath & "\logs\FWImport-" & Trim(ImpType) & "-" & Trim(Format(Today(), "ddMMyyyy")) & ".log"
            If File.Exists(filename) = False Then
                newfile = True
            Else
                newfile = False
            End If

            If System.IO.Directory.Exists(Application.StartupPath & "\logs") = False Then
                System.IO.Directory.CreateDirectory(Application.StartupPath & "\logs")
            End If

            logfile = New StreamWriter(filename, True)

            If newfile = True Then
                ' Store some header information into the log file
                Msg = "Framework Import for " & ImpDesc

                logfile.WriteLine(Msg)
            End If

            datestamp = Format(Now, "dd/MM/yyyy hh:mm:ss")

            logfile.WriteLine(datestamp & " : " & Msg)

            logfile.Close()

            LastLogFile = filename

        Catch ex As Exception
            MsgBox("Error writing to FWImport.log file", MsgBoxStyle.Information, "FWImport Error")
        End Try
    End Sub

    Private Function doQuery(ByVal sql As String) As DataSet
        Try
            Dim db As New DBConnection(FWEmail.ConnectionString)
            Dim dset As DataSet
            dset = db.GetDataSet(sql)
            Return dset

        Catch ex As Exception
            Log("Error performing database query")
            Log(ex.Message)
            Return Nothing
        End Try
    End Function

    Private Function LookupReference(ByVal isWebService As Boolean, ByVal searchTable As String, ByVal searchField As String, ByVal searchValue As String, ByVal returnField As String) As Integer
        Dim rf As String = ""
        Dim x As Integer

        If Trim(searchValue) = "" Then
            LookupReference = -2
            Exit Function
        End If

        rf = Replace(returnField, "[", "")
        rf = Replace(rf, "]", "")
        x = InStr(rf, ".")
        If x > 0 Then
            rf = Mid(rf, x + 1)
        End If


        Dim db As New DBConnection(FWEmail.ConnectionString)
        'st = searchTable.Replace("[", "")
        'st = st.Replace("]", "")

        'sf = Replace(searchField, "[", "")
        'sf = Replace(sf, "]", "")
        'x = InStr(sf, ".")
        'If x > 0 Then
        '    sf = Mid(sf, x + 1)
        'End If

        'db.FWDb("R2", st, sf, Trim(searchValue))
        Dim sql As String
        Dim dset As New DataSet
        sql = "SELECT TOP 1 " & returnField & " FROM " & searchTable & " WHERE LOWER(" & searchField & ") = LOWER(@searchVal)"
        db.sqlexecute.Parameters.AddWithValue("@searchVal", searchValue.Trim)

        If SMRoutines.hasLocationId(searchTable) Then
            sql += " AND [subAccountId] = @locId"
            db.sqlexecute.Parameters.AddWithValue("@locId", ImportLocation)
        End If

        Try
            dset = db.GetDataSet(sql)

            If dset.Tables(0).Rows.Count > 0 Then
                Dim retVal As Integer
                Integer.TryParse(dset.Tables(0).Rows(0).Item(rf), retVal)
                LookupReference = retVal 'Val(db.GetFieldValue(dset, rf, 0))
            Else
                System.Diagnostics.Debug.WriteLine("Failed to find *" & searchValue & "*")
                LookupReference = -1
            End If
        Catch ex As Exception

        End Try
    End Function

    Private Function duplicateExists(ByVal params As cAccountProperties, ByVal SearchTable As String, ByVal SearchField As String, ByVal SearchValue As String, Optional ByVal AdditionalField As String = "", Optional ByVal AdditionalValue As Object = Nothing) As Boolean
        Dim sql As String
        Dim dset As New DataSet
        Dim CountVal As Integer = 0

        Try
            sql = "SELECT COUNT(*) AS [RetCount] FROM [" & SearchTable & "] WHERE LOWER([" & SearchField & "]) = LOWER(@SearchVal)"

            If AdditionalField <> "" Then
                sql += " AND " & AdditionalField & " = @AddVal"
            End If

            Dim db As New DBConnection(FWEmail.ConnectionString)

            db.sqlexecute.Parameters.AddWithValue("@SearchVal", SearchValue)
            If AdditionalField <> "" Then
                db.sqlexecute.Parameters.AddWithValue("@AddVal", AdditionalValue)
            End If
            CountVal = db.getcount(sql)

            If CountVal > 0 Then
                duplicateExists = True
            Else
                duplicateExists = False
            End If

        Catch ex As Exception
            Log("Error encountered check for duplicate entry in the database.")
            Log("Error: " & ex.Message)
            duplicateExists = True
        End Try
    End Function

    Private Sub SendLineToUnProcessed(ByVal strLine As String, ByVal txtSrcFile As String)
        Try
            txtSrcFile = Replace(txtSrcFile, ".csv", ".unp")

            Dim strW As New StreamWriter(txtSrcFile, True)

            strW.WriteLine(strLine)

            strW.Close()
            strW = Nothing

        Catch ex As Exception
            Log("Failure outputting the unprocessed row from the source file")
            Log(ex.Message)
        End Try
    End Sub

    Private Sub NewImportClick()
        'Dim fws As New cFWSettings
        Dim eRow As Infragistics.Win.UltraWinGrid.RowEnumerator
        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        ImportLocation = FWEmail.ActiveSubAccount.SubAccountID

        'fws = LoadXMLSettings() '  SetApplicationProperties()

        ' check that all mandatory fields have been mapped, halt if not
        eRow = dstGrid.Rows.GetEnumerator
        If Me.ImportedMapping = False Then
            While eRow.MoveNext = True
                If eRow.Current.Cells(cUtility.dstField.xmapField).Value = "Mandatory" Then
                    MsgBox("A Mandatory field remains unmapped. Cannot proceed with the import.", MsgBoxStyle.Critical, "Framework Import Utility")
                    Exit Sub
                End If
            End While

            If GetColumnMappings() = False Then
                MsgBox("Import abandoned.", MsgBoxStyle.Exclamation, "Framework Import Utility")
                Exit Sub
            End If

            If slFixedMappings.Count > 0 Then
                Dim dlg As New dlgFixedSelection

                eRow = dstGrid.Rows.GetEnumerator
                While eRow.MoveNext = True
                    Dim isLocationId As Boolean = False

                    If Mid(eRow.Current.Cells(cUtility.dstField.xmapField).Value, 1, 2) = "F=" Then
                        Dim fmKey As String = Mid(eRow.Current.Cells(cUtility.dstField.xmapField).Value, 3)
                        dlg.Tag = slFixedMappings(fmKey)
                        If dlg.Tag = "accountsSubAccounts.subAccountId" Then
                            isLocationId = True
                        End If

                        dlg.ShowDialog(Me)
                        If dlg.DialogResult = Windows.Forms.DialogResult.OK Then
                            If isLocationId Then
                                ImportLocation = CInt(dlg.Tag)
                            End If
                            slFixedMappings(Mid(eRow.Current.Cells(cUtility.dstField.xmapField).Value, 3)) = dlg.Tag
                            FixedColumns += 1
                        End If
                        dlg.Dispose()
                        dlg = Nothing
                    End If
                End While
            End If
        End If

        Dim curCursor As Cursor

        curCursor = Windows.Forms.Cursor.Current
        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Dim csvParser As New csvParser.cCSV
        Dim csv_dset As DataTable

        Try
            lblImportStatus.Text = "Parsing Input File"
            lblImportStatus.Update()
            csvParser.hasHeaderRow = chkIgnoreFirstRow.Checked
            csv_dset = csvParser.CSVToDataset(txtSrcFile.Text).Tables(0)

        Catch ex As Exception
            MsgBox("Parsing of input file failed" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
            Exit Sub
        End Try

        Dim rejectTable As New DataTable
        ' make the rejectTable the same number of columns as the one read in (minus the column0 status)
        Dim xCol As DataColumn
        For Each xCol In csv_dset.Columns
            If xCol.Ordinal > 0 Then ' skip first status column
                rejectTable.Columns.Add(New DataColumn(xCol.ColumnName, xCol.DataType))
            End If
        Next

        Dim successRows, failRows, totalRows, CompleteRows As Integer

        successRows = 0
        failRows = 0
        totalRows = 0
        CompleteRows = csv_dset.Rows.Count
        prgImport.Maximum = CompleteRows
        prgImport.Minimum = 0
        prgImport.Visible = True
        prgImport.Value = 0

        Dim properties As cAccountProperties
        If Not slFixedMappings("accountsSubAccounts.Description") Is Nothing Then
            properties = FWEmail.SubAccounts.getSubAccountById(slFixedMappings("accountsSubAccounts.Description")).SubAccountProperties
        Else
            properties = FWEmail.SubAccounts.GetFirstSubAccount().SubAccountProperties
        End If

        Try
            Log("**************** New Import Started ****************")
            Dim drow As DataRow
            Dim skipFirstRow As Boolean = chkIgnoreFirstRow.Checked

            If chkSaveMapping.Checked = True Then
                Log("Saving import map...")
                lblImportStatus.Text = "Saving Import Map"
                SaveImportMapping()
            End If

            lblImportStatus.Text = "Importing Data"
            lblImportStatus.Update()

            For Each drow In csv_dset.Rows
                If skipFirstRow = False Then
                    totalRows += 1
                    prgImport.Value = totalRows
                    prgImport.Refresh()

                    Select Case CInt(drow(0))
                        Case 0 ' column count equal = good
                            Try
                                If ValidateRow(drow, totalRows) = True Then
                                    If ImportDataRow(drow, properties, totalRows) = False Then
                                        ' error occurred on the row, log to file
                                        Log("Import of source row " & totalRows.ToString & " failed.")

                                        rejectRow(rejectTable, drow)
                                        failRows += 1
                                    Else
                                        ' successful import of row
                                        successRows += 1
                                    End If
                                Else
                                    Log("Import Row " & totalRows.ToString & " skipped.")
                                    rejectRow(rejectTable, drow)
                                    failRows += 1
                                End If

                            Catch ex As Exception
                                Log(ex.Message)
                                Log("Import Row skipped.")
                                rejectRow(rejectTable, drow)
                                failRows += 1
                            End Try

                        Case 1 ' too many columns
                            Log("Columns count mismatch (too many columns) @ row " & totalRows.ToString)
                            Log("Skipping row " & Trim(Str(totalRows)))
                            rejectRow(rejectTable, drow)
                            failRows += 1

                        Case 2 ' too few columns
                            Log("Columns count mismatch (too few columns) @ row " & totalRows.ToString)
                            Log("Skipping row " & Trim(Str(totalRows)))
                            rejectRow(rejectTable, drow)
                            failRows += 1
                        Case Else

                    End Select
                Else
                    ' still output the header row to the unprocessed file
                    rejectRow(rejectTable, drow)
                End If
                skipFirstRow = False
            Next

            Log("")
            Log("Summary Statistics of FWImport for " & txtSrcFile.Text.Trim)
            Dim x As Integer
            Dim tmpstr As String = ""
            For x = 1 To Len(Trim(txtSrcFile.Text)) + 35
                tmpstr += "-"
            Next
            Log("")
            Log(tmpstr)
            Log("Total Import Rows processed : " & totalRows.ToString)
            Log("Successful Rows Imported : " & successRows.ToString)
            Log("Failed Rows Ignored : " & failRows.ToString)
            Log("**************** FWImport finished ****************")

            ' write out failed rows the .unp file
            If rejectTable.Rows.Count > 0 Then
                Try
                    lblImportStatus.Text = "Writing unprocessed datafile..."
                    lblImportStatus.Update()
                    csvParser.WriteCSVTable(rejectTable, False, Replace(txtSrcFile.Text, ".csv", ".unp"))
                Catch ex As Exception

                End Try
            End If

            PerformPostProcessing()

            lblImportStatus.Text = "Import Complete"
            lblImportStatus.Update()
            Windows.Forms.Cursor.Current = curCursor
            Me.DialogResult = Windows.Forms.DialogResult.OK

        Catch ex As Exception
            Log("Error experienced in Import : " & ex.Message)
            Log("Import Aborted.")
            Me.DialogResult = Windows.Forms.DialogResult.Abort
        End Try

        If failRows > 0 Then
            MsgBox(Trim(Str(failRows)) & " rows from source file failed to import." & vbNewLine & "Refer to log file for details", MsgBoxStyle.Critical, "FWImport Error")
        End If

        Close()
    End Sub

    Private Sub rejectRow(ByRef rejectTable As DataTable, ByRef drow As DataRow)
        Try
            Dim newRow As DataRow = rejectTable.NewRow

            Dim i As Integer
            For i = (drow.ItemArray.GetLowerBound(0) + 1) To drow.ItemArray.GetUpperBound(0)
                ' skipped the first columns (csv parser row status column)
                newRow(i - 1) = drow.Item(i)
            Next
            rejectTable.Rows.Add(newRow)

        Catch ex As Exception
            Log("A rejected import row failed to be added to the unprocessed list")
        End Try

    End Sub

    Private Function GetUFListItems(ByVal udfId As Integer) As Dictionary(Of String, Integer)
        Try
            Dim DDListItems As New Dictionary(Of String, Integer)

            Dim sql As String = "select valueid, item from userdefined_list_items where userdefineid = @udfId order by [order]"
            Dim db As New DBConnection(FWEmail.ConnectionString)
            db.sqlexecute.Parameters.AddWithValue("@udfId", udfId)
            Using reader As SqlClient.SqlDataReader = db.GetReader(sql)
                While reader.Read
                    Dim valueid As Integer = reader.GetInt32(0)
                    Dim item As String = reader.GetString(1)

                    If DDListItems.ContainsKey(valueid) = False Then
                        DDListItems.Add(item.ToLower, valueid)
                    End If
                End While
                reader.Close()
            End Using

            Return DDListItems

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub SaveImportMapping()
        Dim importMapping As New cImportMaps.ImportMapping
        With importMapping
            .FileName = txtSrcFile.Text
            .IgnoreHeader = chkIgnoreFirstRow.Checked
            .DelimiterChar = txtDelimiter.Text
            .uiMapping = uiMapping
            .uiMappingCount = uiMappingCount
            .FixedMappings = slFixedMappings
        End With

        Dim mappingClass As New cImportMaps

        Dim path As String = Application.StartupPath + "\templates\mappings\" & ImpType.ToString() & ".dat"

        If System.IO.Directory.Exists(Application.StartupPath & "\templates\mappings") = False Then
            System.IO.Directory.CreateDirectory(Application.StartupPath & "\templates\mappings")
        End If

        mappingClass.SaveImport(path, importMapping)
    End Sub

    Private Function LoadImportMapping() As Boolean
        Dim mappingClass As New cImportMaps
        Dim importMapping As cImportMaps.ImportMapping

        Dim path As String = Application.StartupPath + "\templates\mappings\" & ImpType.ToString() & ".dat"
        If File.Exists(path) = True Then
            importMapping = mappingClass.GetImport(path)

            With importMapping
                If .FileName = txtSrcFile.Text Then
                    chkIgnoreFirstRow.Checked = .IgnoreHeader
                    txtDelimiter.Text = .DelimiterChar
                    uiMapping = .uiMapping
                    uiMappingCount = .uiMappingCount
                    slFixedMappings = .FixedMappings

                    If slFixedMappings.ContainsKey("accountsSubAccounts.Description") Then
                        ImportLocation = slFixedMappings("accountsSubAccounts.Description")
                    End If
                    Me.ImportedMapping = True
                    Return True
                End If
            End With
        End If
        Return False
    End Function

    Private Function ConvertToKey(ByVal fieldName As String) As String
        Dim tmpKey As String

        tmpKey = fieldName.Replace(" ", "_")
        ConvertToKey = tmpKey.Replace("-", "_")
    End Function

    Private Sub PerformPostProcessing()
        Dim sql As New System.Text.StringBuilder

        Select Case CType(Me.Tag, cUtility.ImportType)
            Case cUtility.ImportType.ProductVendorAssoc
                Dim db As New DBConnection(FWEmail.ConnectionString)

                sql.Append("declare @supplierID int " & vbNewLine)
                sql.Append("declare @productID int " & vbNewLine)
                sql.Append("declare @productsupplierID int " & vbNewLine)
                sql.Append("declare loop_cursor cursor for " & vbNewLine)
                sql.Append("select [ProductSupplierId], [ProductId], [supplierId] from product_suppliers " & vbNewLine)
                sql.Append("open loop_cursor " & vbNewLine)
                sql.Append("fetch next from loop_cursor into @productsupplierID,@productID,@supplierID " & vbNewLine)
                sql.Append("while @@FETCH_STATUS = 0 " & vbNewLine)
                sql.Append("begin " & vbNewLine)
                sql.Append("delete from product_suppliers where [ProductId] = @productID and [supplierId] = @supplierID and [ProductSupplierId] <> @productsupplierID " & vbNewLine)
                sql.Append("fetch next from loop_cursor into @productsupplierID,@productID,@supplierID " & vbNewLine)
                sql.Append("end " & vbNewLine)
                sql.Append("close loop_cursor " & vbNewLine)
                sql.Append("deallocate loop_cursor")
                db.ExecuteSQL(sql.ToString)

            Case Else

        End Select
    End Sub

    Private Function needsCreatedOn(ByVal tableName As String) As Boolean
        Dim required As Boolean = False

        Select Case tableName.ToLower
            Case "codes_contractcategory", "codes_contracttype", "codes_contractstatus", "codes_salestax", "codes_termtype", "codes_units", "codes_invoicefrequencytype"
                required = True
            Case "contract_details"
                required = True
            Case "employees"
                required = True
            Case "invoicestatustype", "licencerenewaltypes"
                required = True
            Case "productcategories", "productdetails", "productlicences"
                required = True
            Case "supplier_categories", "supplier_details", "supplier_status", "supplier_contacts"
                required = True
            Case Else
        End Select

        Return required
    End Function
End Class
