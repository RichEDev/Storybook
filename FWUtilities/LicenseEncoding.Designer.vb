<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgLicenseEncoding
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgLicenseEncoding))
        Me.grpEncryption = New System.Windows.Forms.GroupBox
        Me.cmdUpdate = New System.Windows.Forms.Button
        Me.rdoDecrypt = New System.Windows.Forms.RadioButton
        Me.rdoEncrypt = New System.Windows.Forms.RadioButton
        Me.txtCryptString = New System.Windows.Forms.TextBox
        Me.txtRawString = New System.Windows.Forms.TextBox
        Me.lblCryptString = New System.Windows.Forms.Label
        Me.lblRawString = New System.Windows.Forms.Label
        Me.cmdClose = New System.Windows.Forms.Button
        Me.grpEncryption.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpEncryption
        '
        Me.grpEncryption.Controls.Add(Me.cmdUpdate)
        Me.grpEncryption.Controls.Add(Me.rdoDecrypt)
        Me.grpEncryption.Controls.Add(Me.rdoEncrypt)
        Me.grpEncryption.Controls.Add(Me.txtCryptString)
        Me.grpEncryption.Controls.Add(Me.txtRawString)
        Me.grpEncryption.Controls.Add(Me.lblCryptString)
        Me.grpEncryption.Controls.Add(Me.lblRawString)
        Me.grpEncryption.Location = New System.Drawing.Point(12, 12)
        Me.grpEncryption.Name = "grpEncryption"
        Me.grpEncryption.Size = New System.Drawing.Size(386, 175)
        Me.grpEncryption.TabIndex = 0
        Me.grpEncryption.TabStop = False
        Me.grpEncryption.Text = "Encryption Parameters"
        '
        'cmdUpdate
        '
        Me.cmdUpdate.Location = New System.Drawing.Point(304, 132)
        Me.cmdUpdate.Name = "cmdUpdate"
        Me.cmdUpdate.Size = New System.Drawing.Size(75, 23)
        Me.cmdUpdate.TabIndex = 6
        Me.cmdUpdate.Text = "Update"
        Me.cmdUpdate.UseVisualStyleBackColor = True
        '
        'rdoDecrypt
        '
        Me.rdoDecrypt.AutoSize = True
        Me.rdoDecrypt.Location = New System.Drawing.Point(106, 132)
        Me.rdoDecrypt.Name = "rdoDecrypt"
        Me.rdoDecrypt.Size = New System.Drawing.Size(62, 17)
        Me.rdoDecrypt.TabIndex = 5
        Me.rdoDecrypt.Text = "Decrypt"
        Me.rdoDecrypt.UseVisualStyleBackColor = True
        '
        'rdoEncrypt
        '
        Me.rdoEncrypt.AutoSize = True
        Me.rdoEncrypt.Checked = True
        Me.rdoEncrypt.Location = New System.Drawing.Point(10, 132)
        Me.rdoEncrypt.Name = "rdoEncrypt"
        Me.rdoEncrypt.Size = New System.Drawing.Size(61, 17)
        Me.rdoEncrypt.TabIndex = 4
        Me.rdoEncrypt.TabStop = True
        Me.rdoEncrypt.Text = "Encrypt"
        Me.rdoEncrypt.UseVisualStyleBackColor = True
        '
        'txtCryptString
        '
        Me.txtCryptString.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCryptString.Location = New System.Drawing.Point(10, 96)
        Me.txtCryptString.Name = "txtCryptString"
        Me.txtCryptString.Size = New System.Drawing.Size(370, 29)
        Me.txtCryptString.TabIndex = 3
        '
        'txtRawString
        '
        Me.txtRawString.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtRawString.Location = New System.Drawing.Point(10, 33)
        Me.txtRawString.Name = "txtRawString"
        Me.txtRawString.Size = New System.Drawing.Size(370, 29)
        Me.txtRawString.TabIndex = 2
        '
        'lblCryptString
        '
        Me.lblCryptString.AutoSize = True
        Me.lblCryptString.Location = New System.Drawing.Point(7, 80)
        Me.lblCryptString.Name = "lblCryptString"
        Me.lblCryptString.Size = New System.Drawing.Size(85, 13)
        Me.lblCryptString.TabIndex = 1
        Me.lblCryptString.Text = "Encrypted Value"
        '
        'lblRawString
        '
        Me.lblRawString.AutoSize = True
        Me.lblRawString.Location = New System.Drawing.Point(6, 16)
        Me.lblRawString.Name = "lblRawString"
        Me.lblRawString.Size = New System.Drawing.Size(127, 13)
        Me.lblRawString.TabIndex = 0
        Me.lblRawString.Text = "Raw Value (unencrypted)"
        '
        'cmdClose
        '
        Me.cmdClose.CausesValidation = False
        Me.cmdClose.Location = New System.Drawing.Point(316, 193)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(75, 23)
        Me.cmdClose.TabIndex = 1
        Me.cmdClose.Text = "Close"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'dlgLicenseEncoding
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(410, 217)
        Me.ControlBox = False
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.grpEncryption)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "dlgLicenseEncoding"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "License Encoding"
        Me.grpEncryption.ResumeLayout(False)
        Me.grpEncryption.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents grpEncryption As System.Windows.Forms.GroupBox
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents rdoDecrypt As System.Windows.Forms.RadioButton
    Friend WithEvents rdoEncrypt As System.Windows.Forms.RadioButton
    Friend WithEvents txtCryptString As System.Windows.Forms.TextBox
    Friend WithEvents txtRawString As System.Windows.Forms.TextBox
    Friend WithEvents lblCryptString As System.Windows.Forms.Label
    Friend WithEvents lblRawString As System.Windows.Forms.Label
    Friend WithEvents cmdUpdate As System.Windows.Forms.Button
End Class
