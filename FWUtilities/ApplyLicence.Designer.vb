<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ApplyLicence
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ApplyLicence))
        Me.grpMain = New System.Windows.Forms.GroupBox
        Me.lblLicenceSize = New System.Windows.Forms.Label
        Me.txtLicence = New System.Windows.Forms.TextBox
        Me.cmdApply = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdDecode = New System.Windows.Forms.Button
        Me.grpMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpMain
        '
        Me.grpMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpMain.Controls.Add(Me.cmdDecode)
        Me.grpMain.Controls.Add(Me.lblLicenceSize)
        Me.grpMain.Controls.Add(Me.txtLicence)
        Me.grpMain.Controls.Add(Me.cmdApply)
        Me.grpMain.Controls.Add(Me.cmdCancel)
        Me.grpMain.Location = New System.Drawing.Point(12, 12)
        Me.grpMain.Name = "grpMain"
        Me.grpMain.Size = New System.Drawing.Size(295, 106)
        Me.grpMain.TabIndex = 0
        Me.grpMain.TabStop = False
        Me.grpMain.Text = "Licence Application"
        '
        'lblLicenceSize
        '
        Me.lblLicenceSize.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblLicenceSize.AutoSize = True
        Me.lblLicenceSize.Location = New System.Drawing.Point(6, 58)
        Me.lblLicenceSize.Name = "lblLicenceSize"
        Me.lblLicenceSize.Size = New System.Drawing.Size(107, 13)
        Me.lblLicenceSize.TabIndex = 3
        Me.lblLicenceSize.Text = "0 Licences detected."
        '
        'txtLicence
        '
        Me.txtLicence.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLicence.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLicence.Location = New System.Drawing.Point(6, 20)
        Me.txtLicence.Name = "txtLicence"
        Me.txtLicence.Size = New System.Drawing.Size(246, 35)
        Me.txtLicence.TabIndex = 1
        '
        'cmdApply
        '
        Me.cmdApply.Enabled = False
        Me.cmdApply.Location = New System.Drawing.Point(133, 77)
        Me.cmdApply.Name = "cmdApply"
        Me.cmdApply.Size = New System.Drawing.Size(75, 23)
        Me.cmdApply.TabIndex = 3
        Me.cmdApply.Text = "Apply"
        Me.cmdApply.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.Location = New System.Drawing.Point(214, 77)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdCancel.TabIndex = 4
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdDecode
        '
        Me.cmdDecode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdDecode.Image = Global.FWUtilities.My.Resources.Resources.refresh
        Me.cmdDecode.Location = New System.Drawing.Point(258, 20)
        Me.cmdDecode.Name = "cmdDecode"
        Me.cmdDecode.Size = New System.Drawing.Size(31, 35)
        Me.cmdDecode.TabIndex = 2
        Me.cmdDecode.UseVisualStyleBackColor = True
        '
        'ApplyLicence
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(319, 130)
        Me.Controls.Add(Me.grpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ApplyLicence"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Apply User Licence"
        Me.grpMain.ResumeLayout(False)
        Me.grpMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents grpMain As System.Windows.Forms.GroupBox
    Friend WithEvents lblLicenceSize As System.Windows.Forms.Label
    Friend WithEvents txtLicence As System.Windows.Forms.TextBox
    Friend WithEvents cmdApply As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdDecode As System.Windows.Forms.Button
End Class
