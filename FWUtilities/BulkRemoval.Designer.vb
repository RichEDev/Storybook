<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BulkRemoval
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BulkRemoval))
        Me.RemovalTabs = New System.Windows.Forms.TabControl
        Me.tabContract = New System.Windows.Forms.TabPage
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.lblKey = New System.Windows.Forms.Label
        Me.txtKey = New System.Windows.Forms.TextBox
        Me.cmdOK = New System.Windows.Forms.Button
        Me.cmdDelete = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.resGrid = New System.Windows.Forms.DataGridView
        Me.tabLocation = New System.Windows.Forms.TabPage
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.cmdLocOK = New System.Windows.Forms.Button
        Me.cboLocation = New System.Windows.Forms.ComboBox
        Me.lblLocation = New System.Windows.Forms.Label
        Me.cmdLocDelete = New System.Windows.Forms.Button
        Me.cmdLocCancel = New System.Windows.Forms.Button
        Me.locResultGrid = New System.Windows.Forms.DataGridView
        Me.dsetResult = New System.Data.DataSet
        Me.RemovalTabs.SuspendLayout()
        Me.tabContract.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.resGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabLocation.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.locResultGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dsetResult, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RemovalTabs
        '
        Me.RemovalTabs.Controls.Add(Me.tabContract)
        Me.RemovalTabs.Controls.Add(Me.tabLocation)
        Me.RemovalTabs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RemovalTabs.Location = New System.Drawing.Point(0, 0)
        Me.RemovalTabs.Name = "RemovalTabs"
        Me.RemovalTabs.SelectedIndex = 0
        Me.RemovalTabs.Size = New System.Drawing.Size(488, 517)
        Me.RemovalTabs.TabIndex = 0
        '
        'tabContract
        '
        Me.tabContract.Controls.Add(Me.SplitContainer1)
        Me.tabContract.Location = New System.Drawing.Point(4, 22)
        Me.tabContract.Name = "tabContract"
        Me.tabContract.Padding = New System.Windows.Forms.Padding(3)
        Me.tabContract.Size = New System.Drawing.Size(480, 491)
        Me.tabContract.TabIndex = 0
        Me.tabContract.Text = "Contract Removal"
        Me.tabContract.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblKey)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtKey)
        Me.SplitContainer1.Panel1.Controls.Add(Me.cmdOK)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.cmdDelete)
        Me.SplitContainer1.Panel2.Controls.Add(Me.cmdCancel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.resGrid)
        Me.SplitContainer1.Size = New System.Drawing.Size(474, 485)
        Me.SplitContainer1.SplitterDistance = 49
        Me.SplitContainer1.TabIndex = 0
        '
        'lblKey
        '
        Me.lblKey.AutoSize = True
        Me.lblKey.Location = New System.Drawing.Point(81, 17)
        Me.lblKey.Name = "lblKey"
        Me.lblKey.Size = New System.Drawing.Size(74, 13)
        Me.lblKey.TabIndex = 2
        Me.lblKey.Text = "Contract Key :"
        '
        'txtKey
        '
        Me.txtKey.Location = New System.Drawing.Point(161, 13)
        Me.txtKey.Name = "txtKey"
        Me.txtKey.Size = New System.Drawing.Size(124, 20)
        Me.txtKey.TabIndex = 1
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(291, 12)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(75, 23)
        Me.cmdOK.TabIndex = 0
        Me.cmdOK.Text = "OK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'cmdDelete
        '
        Me.cmdDelete.Enabled = False
        Me.cmdDelete.Location = New System.Drawing.Point(313, 400)
        Me.cmdDelete.Name = "cmdDelete"
        Me.cmdDelete.Size = New System.Drawing.Size(75, 23)
        Me.cmdDelete.TabIndex = 2
        Me.cmdDelete.Text = "Delete"
        Me.cmdDelete.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(394, 400)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdCancel.TabIndex = 1
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'resGrid
        '
        Me.resGrid.AllowUserToAddRows = False
        Me.resGrid.AllowUserToDeleteRows = False
        Me.resGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.resGrid.Location = New System.Drawing.Point(3, 3)
        Me.resGrid.Name = "resGrid"
        Me.resGrid.ReadOnly = True
        Me.resGrid.Size = New System.Drawing.Size(466, 390)
        Me.resGrid.TabIndex = 0
        '
        'tabLocation
        '
        Me.tabLocation.Controls.Add(Me.SplitContainer2)
        Me.tabLocation.Location = New System.Drawing.Point(4, 22)
        Me.tabLocation.Name = "tabLocation"
        Me.tabLocation.Padding = New System.Windows.Forms.Padding(3)
        Me.tabLocation.Size = New System.Drawing.Size(480, 491)
        Me.tabLocation.TabIndex = 1
        Me.tabLocation.Text = "Location Removal"
        Me.tabLocation.UseVisualStyleBackColor = True
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.cmdLocOK)
        Me.SplitContainer2.Panel1.Controls.Add(Me.cboLocation)
        Me.SplitContainer2.Panel1.Controls.Add(Me.lblLocation)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.cmdLocDelete)
        Me.SplitContainer2.Panel2.Controls.Add(Me.cmdLocCancel)
        Me.SplitContainer2.Panel2.Controls.Add(Me.locResultGrid)
        Me.SplitContainer2.Size = New System.Drawing.Size(474, 485)
        Me.SplitContainer2.SplitterDistance = 63
        Me.SplitContainer2.TabIndex = 0
        '
        'cmdLocOK
        '
        Me.cmdLocOK.Location = New System.Drawing.Point(345, 15)
        Me.cmdLocOK.Name = "cmdLocOK"
        Me.cmdLocOK.Size = New System.Drawing.Size(75, 23)
        Me.cmdLocOK.TabIndex = 2
        Me.cmdLocOK.Text = "OK"
        Me.cmdLocOK.UseVisualStyleBackColor = True
        '
        'cboLocation
        '
        Me.cboLocation.FormattingEnabled = True
        Me.cboLocation.Location = New System.Drawing.Point(166, 15)
        Me.cboLocation.Name = "cboLocation"
        Me.cboLocation.Size = New System.Drawing.Size(173, 21)
        Me.cboLocation.TabIndex = 1
        '
        'lblLocation
        '
        Me.lblLocation.AutoSize = True
        Me.lblLocation.Location = New System.Drawing.Point(50, 19)
        Me.lblLocation.Name = "lblLocation"
        Me.lblLocation.Size = New System.Drawing.Size(109, 13)
        Me.lblLocation.TabIndex = 0
        Me.lblLocation.Text = "Location to Remove :"
        '
        'cmdLocDelete
        '
        Me.cmdLocDelete.Enabled = False
        Me.cmdLocDelete.Location = New System.Drawing.Point(313, 382)
        Me.cmdLocDelete.Name = "cmdLocDelete"
        Me.cmdLocDelete.Size = New System.Drawing.Size(75, 23)
        Me.cmdLocDelete.TabIndex = 2
        Me.cmdLocDelete.Text = "Delete"
        Me.cmdLocDelete.UseVisualStyleBackColor = True
        '
        'cmdLocCancel
        '
        Me.cmdLocCancel.Location = New System.Drawing.Point(394, 382)
        Me.cmdLocCancel.Name = "cmdLocCancel"
        Me.cmdLocCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdLocCancel.TabIndex = 1
        Me.cmdLocCancel.Text = "Cancel"
        Me.cmdLocCancel.UseVisualStyleBackColor = True
        '
        'locResultGrid
        '
        Me.locResultGrid.AllowUserToAddRows = False
        Me.locResultGrid.AllowUserToDeleteRows = False
        Me.locResultGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.locResultGrid.Dock = System.Windows.Forms.DockStyle.Top
        Me.locResultGrid.Location = New System.Drawing.Point(0, 0)
        Me.locResultGrid.Name = "locResultGrid"
        Me.locResultGrid.ReadOnly = True
        Me.locResultGrid.Size = New System.Drawing.Size(474, 376)
        Me.locResultGrid.TabIndex = 0
        '
        'dsetResult
        '
        Me.dsetResult.DataSetName = "NewDataSet"
        '
        'BulkRemoval
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(488, 517)
        Me.Controls.Add(Me.RemovalTabs)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BulkRemoval"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Bulk Data Removal"
        Me.RemovalTabs.ResumeLayout(False)
        Me.tabContract.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.resGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabLocation.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.locResultGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dsetResult, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RemovalTabs As System.Windows.Forms.TabControl
    Friend WithEvents tabContract As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents lblKey As System.Windows.Forms.Label
    Friend WithEvents txtKey As System.Windows.Forms.TextBox
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents tabLocation As System.Windows.Forms.TabPage
    Friend WithEvents resGrid As System.Windows.Forms.DataGridView
    Friend WithEvents dsetResult As System.Data.DataSet
    Friend WithEvents cmdDelete As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents cboLocation As System.Windows.Forms.ComboBox
    Friend WithEvents lblLocation As System.Windows.Forms.Label
    Friend WithEvents locResultGrid As System.Windows.Forms.DataGridView
    Friend WithEvents cmdLocDelete As System.Windows.Forms.Button
    Friend WithEvents cmdLocCancel As System.Windows.Forms.Button
    Friend WithEvents cmdLocOK As System.Windows.Forms.Button
End Class
