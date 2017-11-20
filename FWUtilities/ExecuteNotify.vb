Imports FWBase
Imports Spend_Management
Imports SpendManagementLibrary

Public Class ExecuteNotify
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
    Friend WithEvents txtEmailParam As System.Windows.Forms.TextBox
    Friend WithEvents lblEmailParam As System.Windows.Forms.Label
    Friend WithEvents lblRunDate As System.Windows.Forms.Label
    Friend WithEvents rdoToday As System.Windows.Forms.RadioButton
    Friend WithEvents rdoOtherDate As System.Windows.Forms.RadioButton
    Friend WithEvents dateSpecific As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblDays As System.Windows.Forms.Label
    Friend WithEvents cmdRun As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents lblSpecific As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ExecuteNotify))
        Me.lblTitle = New System.Windows.Forms.Label
        Me.txtEmailParam = New System.Windows.Forms.TextBox
        Me.lblEmailParam = New System.Windows.Forms.Label
        Me.lblRunDate = New System.Windows.Forms.Label
        Me.rdoToday = New System.Windows.Forms.RadioButton
        Me.rdoOtherDate = New System.Windows.Forms.RadioButton
        Me.lblSpecific = New System.Windows.Forms.Label
        Me.dateSpecific = New System.Windows.Forms.DateTimePicker
        Me.lblDays = New System.Windows.Forms.Label
        Me.cmdRun = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(8, 8)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(280, 24)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "notify title"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtEmailParam
        '
        Me.txtEmailParam.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEmailParam.Location = New System.Drawing.Point(128, 56)
        Me.txtEmailParam.Name = "txtEmailParam"
        Me.txtEmailParam.Size = New System.Drawing.Size(48, 23)
        Me.txtEmailParam.TabIndex = 1
        Me.txtEmailParam.Text = "0"
        '
        'lblEmailParam
        '
        Me.lblEmailParam.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEmailParam.Location = New System.Drawing.Point(8, 56)
        Me.lblEmailParam.Name = "lblEmailParam"
        Me.lblEmailParam.Size = New System.Drawing.Size(112, 20)
        Me.lblEmailParam.TabIndex = 2
        Me.lblEmailParam.Text = "Email parameter"
        Me.lblEmailParam.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblRunDate
        '
        Me.lblRunDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRunDate.Location = New System.Drawing.Point(24, 104)
        Me.lblRunDate.Name = "lblRunDate"
        Me.lblRunDate.Size = New System.Drawing.Size(96, 16)
        Me.lblRunDate.TabIndex = 0
        Me.lblRunDate.Text = "Run as when?"
        Me.lblRunDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'rdoToday
        '
        Me.rdoToday.Checked = True
        Me.rdoToday.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoToday.Location = New System.Drawing.Point(128, 96)
        Me.rdoToday.Name = "rdoToday"
        Me.rdoToday.Size = New System.Drawing.Size(120, 32)
        Me.rdoToday.TabIndex = 2
        Me.rdoToday.TabStop = True
        Me.rdoToday.Text = "Today"
        '
        'rdoOtherDate
        '
        Me.rdoOtherDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoOtherDate.Location = New System.Drawing.Point(128, 144)
        Me.rdoOtherDate.Name = "rdoOtherDate"
        Me.rdoOtherDate.Size = New System.Drawing.Size(120, 32)
        Me.rdoOtherDate.TabIndex = 3
        Me.rdoOtherDate.Text = "Specific Date"
        '
        'lblSpecific
        '
        Me.lblSpecific.Enabled = False
        Me.lblSpecific.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpecific.Location = New System.Drawing.Point(16, 192)
        Me.lblSpecific.Name = "lblSpecific"
        Me.lblSpecific.Size = New System.Drawing.Size(96, 24)
        Me.lblSpecific.TabIndex = 0
        Me.lblSpecific.Text = "Specify Date"
        Me.lblSpecific.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'dateSpecific
        '
        Me.dateSpecific.Enabled = False
        Me.dateSpecific.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dateSpecific.Format = System.Windows.Forms.DateTimePickerFormat.Short
        Me.dateSpecific.Location = New System.Drawing.Point(120, 192)
        Me.dateSpecific.MinDate = New Date(1990, 1, 1, 0, 0, 0, 0)
        Me.dateSpecific.Name = "dateSpecific"
        Me.dateSpecific.Size = New System.Drawing.Size(104, 23)
        Me.dateSpecific.TabIndex = 4
        '
        'lblDays
        '
        Me.lblDays.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDays.Location = New System.Drawing.Point(168, 56)
        Me.lblDays.Name = "lblDays"
        Me.lblDays.Size = New System.Drawing.Size(120, 24)
        Me.lblDays.TabIndex = 5
        Me.lblDays.Text = "days in advance"
        Me.lblDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmdRun
        '
        Me.cmdRun.Location = New System.Drawing.Point(176, 232)
        Me.cmdRun.Name = "cmdRun"
        Me.cmdRun.Size = New System.Drawing.Size(96, 32)
        Me.cmdRun.TabIndex = 6
        Me.cmdRun.Text = "Run"
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(24, 232)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(96, 32)
        Me.cmdCancel.TabIndex = 7
        Me.cmdCancel.Text = "Cancel"
        '
        'ExecuteNotify
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(296, 270)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdRun)
        Me.Controls.Add(Me.lblDays)
        Me.Controls.Add(Me.dateSpecific)
        Me.Controls.Add(Me.lblSpecific)
        Me.Controls.Add(Me.rdoOtherDate)
        Me.Controls.Add(Me.rdoToday)
        Me.Controls.Add(Me.lblRunDate)
        Me.Controls.Add(Me.lblEmailParam)
        Me.Controls.Add(Me.txtEmailParam)
        Me.Controls.Add(Me.lblTitle)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ExecuteNotify"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Execute Email Notify"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Close()
    End Sub

    Private Sub rdoOtherDate_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdoOtherDate.CheckedChanged
        lblSpecific.Enabled = rdoOtherDate.Checked
        dateSpecific.Enabled = rdoOtherDate.Checked

        dateSpecific.Value = Today
    End Sub

    Private Sub ExecuteNotify_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Select Case Me.Tag.ToString
            Case "review"
                lblTitle.Text = "Execute Review Notifier"
                lblDays.Text = "days to look ahead"
                txtEmailParam.Text = "15"

            Case "overdue"
                lblTitle.Text = "Execute Overdue Invoice Notifier"
                lblDays.Text = "days grace"
                txtEmailParam.Text = "0"

            Case "licence"
                lblTitle.Text = "Execute Licence Renewal Notifier"
                lblDays.Text = "days in advance"
                txtEmailParam.Text = "7"
            Case "audit"
                lblTitle.Text = "Execute Audit Cleardown"
                rdoOtherDate.Enabled = False
                txtEmailParam.Enabled = False
                lblDays.Text = ""
            Case Else
                lblTitle.Text = "Unknown Notifier"
                cmdRun.Enabled = False
        End Select
    End Sub

    Private Sub cmdRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRun.Click
        Try
            Select Case Me.Tag.ToString
                Case "review"

                Case Else
            End Select

        Catch ex As Exception

        End Try
    End Sub
End Class
