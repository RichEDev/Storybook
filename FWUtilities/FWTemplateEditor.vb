Imports System.Collections
Imports SyntaxHighlightingTextBox

Public Class FWTemplateEditor
    Inherits System.Windows.Forms.Form
    Dim emTypes As New System.Collections.Specialized.NameValueCollection
    Dim dsFields As New DataSet
    Dim activeTemplate As Integer

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
    Friend WithEvents mnuExit As System.Windows.Forms.MenuItem
    Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuMain As System.Windows.Forms.MainMenu
    Friend WithEvents mnuOpen As System.Windows.Forms.MenuItem
    Friend WithEvents menuSave As System.Windows.Forms.MenuItem
    Friend WithEvents fdOpen As System.Windows.Forms.OpenFileDialog
    Friend WithEvents fdSave As System.Windows.Forms.SaveFileDialog
    Friend WithEvents gbEmail As System.Windows.Forms.GroupBox
    Friend WithEvents gbFields As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents mnuEdut As System.Windows.Forms.MenuItem
    Friend WithEvents lstFields As System.Windows.Forms.ListBox
    Friend WithEvents cmbType As System.Windows.Forms.ComboBox
    Friend WithEvents mnuTools As System.Windows.Forms.MenuItem
    Friend WithEvents mnuOptions As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCut As System.Windows.Forms.MenuItem
    Friend WithEvents mnuPaste As System.Windows.Forms.MenuItem
    Friend WithEvents mnuCopy As System.Windows.Forms.MenuItem
    Friend WithEvents mnuPopup As System.Windows.Forms.ContextMenu
    Friend WithEvents conCut As System.Windows.Forms.MenuItem
    Friend WithEvents conCopy As System.Windows.Forms.MenuItem
    Friend WithEvents conPaste As System.Windows.Forms.MenuItem
    Friend WithEvents mnuSelectAll As System.Windows.Forms.MenuItem
    Friend WithEvents conSelectAll As System.Windows.Forms.MenuItem
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents rbPrint As System.Windows.Forms.RadioButton
    Friend WithEvents rbCondition As System.Windows.Forms.RadioButton
    Friend WithEvents rtbEmailTemplate As SyntaxHighlightingTextBox.SyntaxHighlightingTextBox
    Friend WithEvents conUndo As System.Windows.Forms.MenuItem
    Friend WithEvents conRedo As System.Windows.Forms.MenuItem
    Friend WithEvents mnuUndo As System.Windows.Forms.MenuItem
    Friend WithEvents mnuRedo As System.Windows.Forms.MenuItem
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(FWTemplateEditor))
        Me.mnuMain = New System.Windows.Forms.MainMenu
        Me.mnuFile = New System.Windows.Forms.MenuItem
        Me.mnuOpen = New System.Windows.Forms.MenuItem
        Me.menuSave = New System.Windows.Forms.MenuItem
        Me.mnuExit = New System.Windows.Forms.MenuItem
        Me.mnuEdut = New System.Windows.Forms.MenuItem
        Me.mnuCut = New System.Windows.Forms.MenuItem
        Me.mnuCopy = New System.Windows.Forms.MenuItem
        Me.mnuPaste = New System.Windows.Forms.MenuItem
        Me.mnuSelectAll = New System.Windows.Forms.MenuItem
        Me.mnuUndo = New System.Windows.Forms.MenuItem
        Me.mnuRedo = New System.Windows.Forms.MenuItem
        Me.mnuTools = New System.Windows.Forms.MenuItem
        Me.mnuOptions = New System.Windows.Forms.MenuItem
        Me.fdOpen = New System.Windows.Forms.OpenFileDialog
        Me.fdSave = New System.Windows.Forms.SaveFileDialog
        Me.gbEmail = New System.Windows.Forms.GroupBox
        Me.rtbEmailTemplate = New SyntaxHighlightingTextBox.SyntaxHighlightingTextBox
        Me.gbFields = New System.Windows.Forms.GroupBox
        Me.lstFields = New System.Windows.Forms.ListBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.cmbType = New System.Windows.Forms.ComboBox
        Me.mnuPopup = New System.Windows.Forms.ContextMenu
        Me.conCut = New System.Windows.Forms.MenuItem
        Me.conCopy = New System.Windows.Forms.MenuItem
        Me.conPaste = New System.Windows.Forms.MenuItem
        Me.conSelectAll = New System.Windows.Forms.MenuItem
        Me.conUndo = New System.Windows.Forms.MenuItem
        Me.conRedo = New System.Windows.Forms.MenuItem
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.rbPrint = New System.Windows.Forms.RadioButton
        Me.rbCondition = New System.Windows.Forms.RadioButton
        Me.txtName = New System.Windows.Forms.TextBox
        Me.gbEmail.SuspendLayout()
        Me.gbFields.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'mnuMain
        '
        Me.mnuMain.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuEdut, Me.mnuTools})
        '
        'mnuFile
        '
        Me.mnuFile.Index = 0
        Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuOpen, Me.menuSave, Me.mnuExit})
        Me.mnuFile.Text = "File"
        '
        'mnuOpen
        '
        Me.mnuOpen.Index = 0
        Me.mnuOpen.Text = "Open"
        '
        'menuSave
        '
        Me.menuSave.Index = 1
        Me.menuSave.Text = "Save As..."
        '
        'mnuExit
        '
        Me.mnuExit.Index = 2
        Me.mnuExit.Text = "Exit"
        '
        'mnuEdut
        '
        Me.mnuEdut.Index = 1
        Me.mnuEdut.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuCut, Me.mnuCopy, Me.mnuPaste, Me.mnuSelectAll, Me.mnuUndo, Me.mnuRedo})
        Me.mnuEdut.Text = "Edit"
        '
        'mnuCut
        '
        Me.mnuCut.Index = 0
        Me.mnuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX
        Me.mnuCut.Text = "Cut"
        '
        'mnuCopy
        '
        Me.mnuCopy.Index = 1
        Me.mnuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC
        Me.mnuCopy.Text = "Copy"
        '
        'mnuPaste
        '
        Me.mnuPaste.Index = 2
        Me.mnuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV
        Me.mnuPaste.Text = "Paste"
        '
        'mnuSelectAll
        '
        Me.mnuSelectAll.Index = 3
        Me.mnuSelectAll.RadioCheck = True
        Me.mnuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA
        Me.mnuSelectAll.Text = "Select All"
        '
        'mnuUndo
        '
        Me.mnuUndo.Index = 4
        Me.mnuUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ
        Me.mnuUndo.Text = "Undo"
        '
        'mnuRedo
        '
        Me.mnuRedo.Index = 5
        Me.mnuRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY
        Me.mnuRedo.Text = "Redo"
        '
        'mnuTools
        '
        Me.mnuTools.Index = 2
        Me.mnuTools.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuOptions})
        Me.mnuTools.Text = "Tools"
        '
        'mnuOptions
        '
        Me.mnuOptions.Index = 0
        Me.mnuOptions.Text = "Options"
        '
        'gbEmail
        '
        Me.gbEmail.Controls.Add(Me.txtName)
        Me.gbEmail.Controls.Add(Me.rtbEmailTemplate)
        Me.gbEmail.Location = New System.Drawing.Point(144, 0)
        Me.gbEmail.Name = "gbEmail"
        Me.gbEmail.Size = New System.Drawing.Size(592, 408)
        Me.gbEmail.TabIndex = 0
        Me.gbEmail.TabStop = False
        Me.gbEmail.Text = "Template Editor"
        '
        'rtbEmailTemplate
        '
        Me.rtbEmailTemplate.CaseSensitive = False
        Me.rtbEmailTemplate.FilterAutoComplete = False
        Me.rtbEmailTemplate.Location = New System.Drawing.Point(8, 40)
        Me.rtbEmailTemplate.MaxUndoRedoSteps = 50
        Me.rtbEmailTemplate.Name = "rtbEmailTemplate"
        Me.rtbEmailTemplate.Size = New System.Drawing.Size(576, 360)
        Me.rtbEmailTemplate.TabIndex = 0
        Me.rtbEmailTemplate.Text = ""
        '
        'gbFields
        '
        Me.gbFields.Controls.Add(Me.lstFields)
        Me.gbFields.Location = New System.Drawing.Point(8, 120)
        Me.gbFields.Name = "gbFields"
        Me.gbFields.Size = New System.Drawing.Size(128, 288)
        Me.gbFields.TabIndex = 1
        Me.gbFields.TabStop = False
        Me.gbFields.Text = "Available Fields"
        '
        'lstFields
        '
        Me.lstFields.Location = New System.Drawing.Point(8, 16)
        Me.lstFields.Name = "lstFields"
        Me.lstFields.Size = New System.Drawing.Size(112, 264)
        Me.lstFields.TabIndex = 2
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cmbType)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(128, 48)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Template Type"
        '
        'cmbType
        '
        Me.cmbType.Location = New System.Drawing.Point(8, 16)
        Me.cmbType.Name = "cmbType"
        Me.cmbType.Size = New System.Drawing.Size(112, 21)
        Me.cmbType.TabIndex = 2
        Me.cmbType.Text = "Please Select...."
        '
        'mnuPopup
        '
        Me.mnuPopup.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.conCut, Me.conCopy, Me.conPaste, Me.conSelectAll, Me.conUndo, Me.conRedo})
        '
        'conCut
        '
        Me.conCut.Index = 0
        Me.conCut.Text = "Cut"
        '
        'conCopy
        '
        Me.conCopy.Index = 1
        Me.conCopy.Text = "Copy"
        '
        'conPaste
        '
        Me.conPaste.Index = 2
        Me.conPaste.Text = "Paste"
        '
        'conSelectAll
        '
        Me.conSelectAll.Index = 3
        Me.conSelectAll.Text = "Select All"
        '
        'conUndo
        '
        Me.conUndo.Index = 4
        Me.conUndo.Text = "Undo"
        '
        'conRedo
        '
        Me.conRedo.Index = 5
        Me.conRedo.Text = "Redo"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.rbPrint)
        Me.GroupBox2.Controls.Add(Me.rbCondition)
        Me.GroupBox2.Location = New System.Drawing.Point(8, 48)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(128, 64)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Field Type"
        '
        'rbPrint
        '
        Me.rbPrint.Checked = True
        Me.rbPrint.Location = New System.Drawing.Point(8, 16)
        Me.rbPrint.Name = "rbPrint"
        Me.rbPrint.Size = New System.Drawing.Size(112, 24)
        Me.rbPrint.TabIndex = 2
        Me.rbPrint.TabStop = True
        Me.rbPrint.Text = "Print Field"
        '
        'rbCondition
        '
        Me.rbCondition.Location = New System.Drawing.Point(8, 40)
        Me.rbCondition.Name = "rbCondition"
        Me.rbCondition.Size = New System.Drawing.Size(104, 16)
        Me.rbCondition.TabIndex = 2
        Me.rbCondition.Text = "Condition"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(8, 16)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(256, 20)
        Me.txtName.TabIndex = 1
        Me.txtName.Text = "Name"
        '
        'FWTemplateEditor
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(736, 411)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.gbFields)
        Me.Controls.Add(Me.gbEmail)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Menu = Me.mnuMain
        Me.Name = "FWTemplateEditor"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Framework Email Template Editor"
        Me.gbEmail.ResumeLayout(False)
        Me.gbFields.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Enum TemplateCSVField
        RowStatus = 0
        DataValue = 1
        DataText = 2
    End Enum

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If System.IO.File.Exists(Application.StartupPath & "\data\settings.xml") = False Then
            defaultSettings()
            WriteSettings(iSettings)
        Else
            iSettings = ReadSettings()
        End If

        LoadEmailTypes()
        LoadFields()
        SetupShaders()

        If iSettings.txtLoad = True Then
            If System.IO.File.Exists(iSettings.strLastFile) = True Then
                OpenTemplate(iSettings.strLastFile)
            End If
        End If
    End Sub

    Private Sub SetupShaders()
        With rtbEmailTemplate
            .Seperators.Add(" ")
            .Seperators.Add(Chr(10))
            .Seperators.Add(Chr(13))
            .Seperators.Add(Chr(9))
            .Seperators.Add(",")
            .Seperators.Add(".")
            .Seperators.Add("+")

            .WordWrap = True
            .ScrollBars = RichTextBoxScrollBars.Vertical
            .FilterAutoComplete = True
            .AutoWordSelection = True
            .HighlightDescriptors.Add(New HighlightDescriptor("[*", "*]", Color.Red, Nothing, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, True))
            .HighlightDescriptors.Add(New HighlightDescriptor("[-", "-]", Color.Green, Nothing, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, True))
            .HighlightDescriptors.Add(New HighlightDescriptor("else", Color.Green, Nothing, DescriptorType.Word, DescriptorRecognition.Contains, True))

        End With
    End Sub

    Private Sub LoadEmailTypes()
        Dim dsTypes As DataSet
        Dim csv As New csvParser.cCSV

        csv.DelimiterChar = ","
        dsTypes = csv.CSVToDataset(Application.StartupPath & "\data\types.csv")

        Dim tmpVal As Integer
        Dim tmpTxt As String
        Dim dr As DataRow
        For Each dr In dsTypes.Tables(0).Rows
            tmpVal = dr.Item(TemplateCSVField.DataValue)
            tmpTxt = dr.Item(TemplateCSVField.DataText)
            emTypes.Add(tmpVal, tmpTxt)
            cmbType.Items.Add(dr.Item(TemplateCSVField.DataText))
        Next

        'cmbType.Items.Insert(0, "Please select...")
    End Sub

    Private Sub LoadFields()
        Dim csv As New csvParser.cCSV
        dsFields = csv.CSVToDataset(Application.StartupPath & "\data\fields.csv")
    End Sub

    Private Sub mnuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuExit.Click
        Me.DialogResult = DialogResult.OK
        Close()
        Dispose(True)
    End Sub

    Private Sub mnuOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuOpen.Click
        fdOpen.Filter = "FW Template File (*.tpl)|*.tpl"
        fdOpen.Title = "Open Framework Email Template"
        If fdOpen.ShowDialog() = DialogResult.OK Then
            If System.IO.File.Exists(fdOpen.FileName) = True Then
                OpenTemplate(fdOpen.FileName)
            End If
        End If
        'MsgBox(fdOpen.FileName)

    End Sub
    Private Sub SaveTemplate(ByVal strPath As String)
        Dim x As New FWCommon.EmailTemplates

        x.SaveTemplate(activeTemplate, txtName.Text.Trim(), rtbEmailTemplate.Text.Trim(), strPath)

        iSettings.strLastFile = strPath
        WriteSettings(iSettings)
    End Sub

    Private Sub OpenTemplate(ByVal strPath As String)
        Dim y As New FWCommon.EmailTemplates
        Dim res As System.Collections.Specialized.NameValueCollection = y.ReadTemplate(strPath)

        Dim templateType As Integer = Integer.Parse(res("templateType"))
        cmbType.SelectedIndex = templateType - 1

        Dim templateTitle As String = res("templateTitle")
        txtName.Text = templateTitle

        Dim templateBody As String = res("templateBody")
        rtbEmailTemplate.Text = templateBody

        iSettings.strLastFile = strPath
        WriteSettings(iSettings)
    End Sub

    Private Sub menuSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSave.Click
        fdSave.Title = "Save Framework Email Template"
        fdSave.Filter = "FW Template File (*.tpl)|*.tpl"

        If fdSave.ShowDialog() = DialogResult.OK Then
            SaveTemplate(fdSave.FileName)
        End If
        'MsgBox(fdSave.FileName)
    End Sub

    Private Sub cmbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbType.SelectedIndexChanged
        rtbEmailTemplate.Clear()

        Dim i As Integer
        For i = 1 To emTypes.Count
            If cmbType.Text = emTypes.Item(i.ToString) Then
                activeTemplate = Integer.Parse(i.ToString())
                lstFields.Items.Clear()
                Dim dr As DataRow
                For Each dr In dsFields.Tables(0).Rows
                    If dr.Item(TemplateCSVField.DataValue) = i Then
                        lstFields.Items.Add(dr.Item(TemplateCSVField.DataText))
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub AddFieldToTB(ByVal fieldName As String)
        Dim i As Integer = rtbEmailTemplate.SelectionStart
        Dim front As String
        Dim back As String
        Dim selStart, selLength As Integer
        Dim out As String
        front = rtbEmailTemplate.Text.Substring(0, i)
        back = rtbEmailTemplate.Text.Substring(i)
        If rbPrint.Checked = True Then
            out = "[*" & fieldName & "*] "
            selStart = out.Length
            selLength = 0
        ElseIf rbCondition.Checked = True Then
            out = "[-" & fieldName & "-]{" & vbNewLine & "Text If True" & vbNewLine & "}else{" & vbNewLine & "Text if false" & vbNewLine & "}"
            selStart = fieldName.Length + 6
            selLength = 12
        End If

        rtbEmailTemplate.Text = front & out & back

        If iSettings.txtFocus = True Then
            rtbEmailTemplate.SelectionLength = selLength
            rtbEmailTemplate.SelectionStart = i + selStart
            rtbEmailTemplate.Focus()
        End If
    End Sub

    Private Sub lstFields_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstFields.DoubleClick
        Dim dr As DataRow
        For Each dr In dsFields.Tables(0).Rows
            If dr.Item(TemplateCSVField.DataText) = lstFields.SelectedItem Then
                AddFieldToTB(dr.Item(TemplateCSVField.DataText))
                Exit For
            End If
        Next
    End Sub

    Private Sub mnuOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuOptions.Click
        Dim x As New frmOptions
        x.Show()
        x.ApplySettings()
    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If iSettings.settSave = True Then
            WriteSettings(iSettings)
        End If

        If iSettings.fileSave = True Then
            SaveTemplate(iSettings.strLastFile)
        End If
    End Sub

    Private Sub mnuCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCut.Click
        rtbEmailTemplate.Cut()
    End Sub

    Private Sub mnuPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuPaste.Click
        rtbEmailTemplate.Paste()
    End Sub

    Private Sub mnuCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuCopy.Click
        rtbEmailTemplate.Copy()
    End Sub

    Private Sub conCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles conCut.Click
        rtbEmailTemplate.Cut()
    End Sub

    Private Sub conCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles conCopy.Click
        rtbEmailTemplate.Copy()
    End Sub

    Private Sub conPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles conPaste.Click
        rtbEmailTemplate.Paste()
    End Sub

    Private Sub mnuSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAll.Click
        rtbEmailTemplate.SelectAll()
    End Sub

    Private Sub conSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles conSelectAll.Click
        rtbEmailTemplate.SelectAll()
    End Sub

    Private Sub rtbEmailTemplate_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles rtbEmailTemplate.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim mcp As Point
            mcp.X = e.X
            mcp.Y = e.Y
            mnuPopup.Show(rtbEmailTemplate, mcp)
            mcp = mcp.Empty
        End If
    End Sub

    Private Sub conUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles conUndo.Click
        rtbEmailTemplate.Undo()

    End Sub

    Private Sub conRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles conRedo.Click
        rtbEmailTemplate.Redo()
    End Sub

    Private Sub mnuUndo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuUndo.Click
        rtbEmailTemplate.Undo()
    End Sub

    Private Sub mnuRedo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuRedo.Click
        rtbEmailTemplate.Redo()
    End Sub
End Class
