Public Class frmOptions
    Inherits System.Windows.Forms.Form

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
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents chkShader As System.Windows.Forms.CheckBox
    Friend WithEvents chkFocus As System.Windows.Forms.CheckBox
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents chkLoad As System.Windows.Forms.CheckBox
    Friend WithEvents chkSaveSettings As System.Windows.Forms.CheckBox
    Friend WithEvents chkFileSave As System.Windows.Forms.CheckBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmOptions))
        Me.MainMenu1 = New System.Windows.Forms.MainMenu
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.chkFileSave = New System.Windows.Forms.CheckBox
        Me.chkSaveSettings = New System.Windows.Forms.CheckBox
        Me.chkLoad = New System.Windows.Forms.CheckBox
        Me.chkFocus = New System.Windows.Forms.CheckBox
        Me.chkShader = New System.Windows.Forms.CheckBox
        Me.btnSave = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkFileSave)
        Me.GroupBox1.Controls.Add(Me.chkSaveSettings)
        Me.GroupBox1.Controls.Add(Me.chkLoad)
        Me.GroupBox1.Controls.Add(Me.chkFocus)
        Me.GroupBox1.Controls.Add(Me.chkShader)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 8)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(272, 144)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Global Options"
        '
        'chkFileSave
        '
        Me.chkFileSave.Location = New System.Drawing.Point(16, 120)
        Me.chkFileSave.Name = "chkFileSave"
        Me.chkFileSave.Size = New System.Drawing.Size(216, 16)
        Me.chkFileSave.TabIndex = 4
        Me.chkFileSave.Text = "Save Current Template On Close"
        '
        'chkSaveSettings
        '
        Me.chkSaveSettings.Location = New System.Drawing.Point(16, 96)
        Me.chkSaveSettings.Name = "chkSaveSettings"
        Me.chkSaveSettings.Size = New System.Drawing.Size(232, 16)
        Me.chkSaveSettings.TabIndex = 3
        Me.chkSaveSettings.Text = "Automatically Save Settings On Close"
        '
        'chkLoad
        '
        Me.chkLoad.Location = New System.Drawing.Point(16, 72)
        Me.chkLoad.Name = "chkLoad"
        Me.chkLoad.Size = New System.Drawing.Size(232, 16)
        Me.chkLoad.TabIndex = 2
        Me.chkLoad.Text = "Automatically Load Last Active Template"
        '
        'chkFocus
        '
        Me.chkFocus.Location = New System.Drawing.Point(16, 48)
        Me.chkFocus.Name = "chkFocus"
        Me.chkFocus.Size = New System.Drawing.Size(240, 16)
        Me.chkFocus.TabIndex = 1
        Me.chkFocus.Text = "Focus Text After Field Insert"
        '
        'chkShader
        '
        Me.chkShader.Enabled = False
        Me.chkShader.Location = New System.Drawing.Point(16, 24)
        Me.chkShader.Name = "chkShader"
        Me.chkShader.Size = New System.Drawing.Size(240, 16)
        Me.chkShader.TabIndex = 0
        Me.chkShader.Text = " Syntax Highlighting (Deprecated)"
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(8, 160)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(64, 24)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(216, 160)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(64, 24)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        '
        'frmOptions
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(288, 192)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Menu = Me.MainMenu1
        Me.MinimizeBox = False
        Me.Name = "frmOptions"
        Me.Text = "Framework Email Template Builder - Options"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public Sub ApplySettings()
        chkShader.Checked = iSettings.txtShading '(Deprecated, is always active now)
        chkFocus.Checked = iSettings.txtFocus
        chkLoad.Checked = iSettings.txtLoad
        chkSaveSettings.Checked = iSettings.settSave
        chkFileSave.Checked = iSettings.fileSave
    End Sub

    Public Sub SaveSettings()
        iSettings.txtShading = True '(Deprecated, is always active now)
        iSettings.txtFocus = chkFocus.Checked
        iSettings.txtLoad = chkLoad.Checked
        modTemplateEditor.WriteSettings(iSettings)
        iSettings.settSave = chkSaveSettings.Checked
        iSettings.fileSave = chkFileSave.Checked
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        SaveSettings()
        Dispose(True)
    End Sub

    Private Sub frmOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Dispose(True)
    End Sub
End Class
