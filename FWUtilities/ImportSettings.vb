Imports System.IO
Imports FWBase

Public Class ImportSettings
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
    Friend WithEvents grpImportSettings As System.Windows.Forms.GroupBox
    Friend WithEvents lblKeyPrefix As System.Windows.Forms.Label
    Friend WithEvents txtKeyPrefix As System.Windows.Forms.TextBox
    Friend WithEvents cmdUpdate As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ImportSettings))
        Me.grpImportSettings = New System.Windows.Forms.GroupBox
        Me.lblKeyPrefix = New System.Windows.Forms.Label
        Me.txtKeyPrefix = New System.Windows.Forms.TextBox
        Me.cmdUpdate = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.grpImportSettings.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpImportSettings
        '
        Me.grpImportSettings.Controls.Add(Me.txtKeyPrefix)
        Me.grpImportSettings.Controls.Add(Me.lblKeyPrefix)
        Me.grpImportSettings.Location = New System.Drawing.Point(8, 8)
        Me.grpImportSettings.Name = "grpImportSettings"
        Me.grpImportSettings.Size = New System.Drawing.Size(248, 64)
        Me.grpImportSettings.TabIndex = 0
        Me.grpImportSettings.TabStop = False
        Me.grpImportSettings.Text = "Import Options"
        '
        'lblKeyPrefix
        '
        Me.lblKeyPrefix.Location = New System.Drawing.Point(8, 24)
        Me.lblKeyPrefix.Name = "lblKeyPrefix"
        Me.lblKeyPrefix.Size = New System.Drawing.Size(112, 24)
        Me.lblKeyPrefix.TabIndex = 0
        Me.lblKeyPrefix.Text = "Contract Key Prefix :"
        Me.lblKeyPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtKeyPrefix
        '
        Me.txtKeyPrefix.Location = New System.Drawing.Point(128, 24)
        Me.txtKeyPrefix.MaxLength = 5
        Me.txtKeyPrefix.Name = "txtKeyPrefix"
        Me.txtKeyPrefix.Size = New System.Drawing.Size(96, 20)
        Me.txtKeyPrefix.TabIndex = 1
        Me.txtKeyPrefix.Text = ""
        '
        'cmdUpdate
        '
        Me.cmdUpdate.Location = New System.Drawing.Point(184, 80)
        Me.cmdUpdate.Name = "cmdUpdate"
        Me.cmdUpdate.Size = New System.Drawing.Size(72, 24)
        Me.cmdUpdate.TabIndex = 2
        Me.cmdUpdate.Text = "Update"
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(8, 80)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(72, 24)
        Me.cmdCancel.TabIndex = 3
        Me.cmdCancel.Text = "Cancel"
        '
        'ImportSettings
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(264, 110)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdUpdate)
        Me.Controls.Add(Me.grpImportSettings)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ImportSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Framework Import Settings"
        Me.grpImportSettings.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub ImportSettings_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'fws = LoadXMLSettings() ' SetApplicationProperties()

        txtKeyPrefix.Focus()
        txtKeyPrefix.Text = FWEmail.FieldKey
    End Sub

    Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
        Try
            FWEmail.FieldKey = txtKeyPrefix.Text.Trim

            'SaveXMLSettings(fws)

            MsgBox("Settings updated successfully", MsgBoxStyle.Information, "Import Settings")

            Me.Close()

        Catch ex As Exception
            MsgBox("Failed to update settings" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "Import Settings")
        End Try
    End Sub
End Class
