Imports FWBase
Imports Spend_Management
Imports SpendManagementLibrary

Public Class dlgFixedSelection
    Inherits System.Windows.Forms.Form
    Public ActiveLocation As Integer = 0
    Private isDDList As Boolean = True
    Private Enum txtType
        Text = 0
        DateTime = 1
        Number = 2
    End Enum
    Private ValidTxtType As txtType

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
    Friend WithEvents lblField As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cboSelection As Infragistics.Win.UltraWinEditors.UltraComboEditor
    Friend WithEvents txtSelection As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(dlgFixedSelection))
        Me.lblField = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdOK = New System.Windows.Forms.Button
        Me.cboSelection = New Infragistics.Win.UltraWinEditors.UltraComboEditor
        Me.txtSelection = New System.Windows.Forms.TextBox
        CType(Me.cboSelection, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblField
        '
        Me.lblField.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblField.Location = New System.Drawing.Point(8, 48)
        Me.lblField.Name = "lblField"
        Me.lblField.Size = New System.Drawing.Size(160, 23)
        Me.lblField.TabIndex = 0
        Me.lblField.Text = "field"
        Me.lblField.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(8, 8)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(320, 23)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "title"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmdCancel
        '
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(200, 88)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(56, 24)
        Me.cmdCancel.TabIndex = 2
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(96, 88)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(56, 24)
        Me.cmdOK.TabIndex = 3
        Me.cmdOK.Text = "OK"
        '
        'cboSelection
        '
        Appearance1.FontData.Name = "Microsoft Sans Serif"
        Appearance1.FontData.SizeInPoints = 10.0!
        Me.cboSelection.Appearance = Appearance1
        Appearance2.FontData.Name = "Microsoft Sans Serif"
        Appearance2.FontData.SizeInPoints = 10.0!
        Me.cboSelection.ItemAppearance = Appearance2
        Me.cboSelection.Location = New System.Drawing.Point(168, 48)
        Me.cboSelection.Name = "cboSelection"
        Me.cboSelection.Size = New System.Drawing.Size(160, 24)
        Me.cboSelection.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
        Me.cboSelection.TabIndex = 1
        '
        'txtSelection
        '
        Me.txtSelection.Location = New System.Drawing.Point(8, 8)
        Me.txtSelection.Name = "txtSelection"
        Me.txtSelection.Size = New System.Drawing.Size(160, 20)
        Me.txtSelection.TabIndex = 4
        Me.txtSelection.Text = "0"
        Me.txtSelection.Visible = False
        '
        'dlgFixedSelection
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(336, 118)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtSelection)
        Me.Controls.Add(Me.cboSelection)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblField)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgFixedSelection"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Fixed Import Selection"
        CType(Me.cboSelection, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub dlgFixedSelection_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim sql As String = ""
        Dim drow As DataRow
        Dim IdFieldName As String = ""
        Dim DescFieldName As String = ""
        Dim usePleaseRequest As Boolean = False
        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        Try
            cboSelection.Items.Clear()
            'fws = LoadXMLSettings() '  SetApplicationProperties()

            Select Case CType(Me.Tag.ToString.Replace("[", "").Replace("]", "").ToLower, String)
                Case "accountssubaccounts.subaccountid"
                    lblField.Text = "Sub-Account to Import to: "
                    lblTitle.Text = "Repository Sub-Account Selection"

                    sql = "SELECT [subAccountId],[Description] FROM accountsSubAccounts"
                    IdFieldName = "subAccountId"
                    DescFieldName = "Description"

                Case "currencyview.currencyid"
                    lblField.Text = "Currency: "
                    lblTitle.Text = "Currency Selection"

                    sql = "SELECT [CurrencyId],[currencyName] FROM currencyView WHERE [subAccountId] = " & ActiveLocation.ToString & " ORDER BY [currencyName]"
                    IdFieldName = "CurrencyId"
                    DescFieldName = "currencyName"
                    usePleaseRequest = True

                Case "codes_contractcategory"
                    lblField.Text = "Category to set to:"
                    lblTitle.Text = "Contract Category Selection"

                    sql = "SELECT [CategoryId],[CategoryDescription] FROM Codes_ContractCategory WHERE [subAccountId] = " & ActiveLocation.ToString & " ORDER BY [CategoryDescription]"
                    IdFieldName = "CategoryId"
                    DescFieldName = "CategoryDescription"
                    usePleaseRequest = True

                Case "codes_contractstatus"
                    lblField.Text = "Status: "
                    lblTitle.Text = "Contract Status Selection"

                    sql = "SELECT [StatusId],[StatusDescription] FROM Codes_ContractStatus WHERE [subAccountId] = " & ActiveLocation.ToString
                    IdFieldName = "StatusId"
                    DescFieldName = "StatusDescription"
                    usePleaseRequest = True

                Case "productcategories"
                    lblField.Text = "Product Category: "
                    lblTitle.Text = "Product Category Selection"

                    sql = "SELECT [CategoryId],[Description] FROM ProductCategories WHERE [subAccountId] = " & ActiveLocation.ToString & " ORDER BY [Description]"
                    IdFieldName = "CategoryId"
                    DescFieldName = "Description"
                    usePleaseRequest = True

                Case "portion"
                    lblField.Text = "Portion Amount: "
                    lblTitle.Text = "Recharge Portion"
                    cboSelection.Visible = False
                    With txtSelection
                        .Visible = True
                        .Left = cboSelection.Left
                        .Top = cboSelection.Top
                    End With
                    isDDList = False
                    ValidTxtType = txtType.Number

                Case Else
                    Exit Sub
            End Select

            If isDDList = True Then
                Dim db As New DBConnection(FWEmail.ConnectionString)
                Dim dset As DataSet = db.GetDataSet(sql)
                For Each drow In dset.Tables(0).Rows
                    cboSelection.Items.Add(drow.Item(IdFieldName), drow.Item(DescFieldName))
                Next

                If usePleaseRequest = True Then
                    Dim lstItem As New Infragistics.Win.ValueListItem
                    lstItem.DisplayText = "Please select..."
                    lstItem.DataValue = 0
                    cboSelection.Items.Insert(0, lstItem)
                    cboSelection.SelectedIndex = cboSelection.Items.IndexOf(lstItem)
                    lstItem.Dispose()
                    lstItem = Nothing
                End If
            End If

        Catch ex As Exception
            MsgBox("An error has occurred!" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
            Me.DialogResult = DialogResult.Abort
        End Try
    End Sub

    Private Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        Me.DialogResult = DialogResult.OK
        If isDDList = True Then
            Me.Tag = cboSelection.Value
        Else
            If txtSelection.Text.Trim = "" Then
                MsgBox("Entry is invalid. Please try again.", MsgBoxStyle.Critical, "Entry Error")
                Exit Sub
            Else
                Select Case ValidTxtType
                    Case txtType.DateTime
                        If IsDate(txtSelection.Text) = False Then
                            MsgBox("Entry is invalid date. Please try again.", MsgBoxStyle.Critical, "Entry Error")
                            Exit Sub
                        End If

                    Case txtType.Number
                        If IsNumeric(txtSelection.Text) = False Then
                            MsgBox("Entry is invalid number. Please try again.", MsgBoxStyle.Critical, "Entry Error")
                            Exit Sub
                        End If

                    Case Else

                End Select

                Me.Tag = txtSelection.Text
            End If
        End If
        Close()
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Tag = ""
        Close()
    End Sub
End Class
