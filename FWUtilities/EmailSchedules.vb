Imports FWBase
Imports System.Collections.Generic
Imports SpendManagementLibrary
Imports Spend_Management

Public Class EmailSchedules
    Inherits System.Windows.Forms.Form
    Friend fws As New cFWSettings
    Private tmpTop, tmpLeft, tmpTopHold, tmpLeftHold As Integer
    Friend WithEvents cboEmailTemplate As System.Windows.Forms.ComboBox
    Friend WithEvents lblLocation As System.Windows.Forms.Label
    Friend WithEvents cboLocation As System.Windows.Forms.ComboBox
    Friend WithEvents gridSchedules As System.Windows.Forms.DataGridView
    
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
    Friend WithEvents grpCurrentSchedules As System.Windows.Forms.GroupBox
    Friend WithEvents grpEmailDetail As System.Windows.Forms.GroupBox
    Friend WithEvents lblEmailType As System.Windows.Forms.Label
    Friend WithEvents lblEmailParam As System.Windows.Forms.Label
    Friend WithEvents txtEmailParam As System.Windows.Forms.TextBox
    Friend WithEvents lblEmailFreq As System.Windows.Forms.Label
    Friend WithEvents cboEmailFreq As System.Windows.Forms.ComboBox
    Friend WithEvents txtFreqParam As System.Windows.Forms.TextBox
    Friend WithEvents lblFreqParam As System.Windows.Forms.Label
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents cboEmailType As System.Windows.Forms.ComboBox
    Friend WithEvents hiddenID As System.Windows.Forms.Label
    Friend WithEvents lblRunDate As System.Windows.Forms.Label
    Friend WithEvents cboNextRunDate As Infragistics.Win.UltraWinSchedule.UltraCalendarCombo
    Friend WithEvents lblNextRunTime As System.Windows.Forms.Label
    Friend WithEvents meNextRunTime As Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit
    Friend WithEvents cmdAddEdit As System.Windows.Forms.Button
    Friend WithEvents cmdUpdate As System.Windows.Forms.Button
    Friend WithEvents cboFreqParam As System.Windows.Forms.ComboBox
    Friend WithEvents cmdDelete As System.Windows.Forms.Button
    Friend WithEvents lblFreqParamError As System.Windows.Forms.Label
    Friend WithEvents lblEmailTemplate As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim DateButton1 As Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton = New Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EmailSchedules))
        Me.grpCurrentSchedules = New System.Windows.Forms.GroupBox
        Me.gridSchedules = New System.Windows.Forms.DataGridView
        Me.grpEmailDetail = New System.Windows.Forms.GroupBox
        Me.cboEmailTemplate = New System.Windows.Forms.ComboBox
        Me.lblEmailTemplate = New System.Windows.Forms.Label
        Me.lblFreqParamError = New System.Windows.Forms.Label
        Me.cmdUpdate = New System.Windows.Forms.Button
        Me.meNextRunTime = New Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit
        Me.lblNextRunTime = New System.Windows.Forms.Label
        Me.cboNextRunDate = New Infragistics.Win.UltraWinSchedule.UltraCalendarCombo
        Me.lblRunDate = New System.Windows.Forms.Label
        Me.hiddenID = New System.Windows.Forms.Label
        Me.cboEmailType = New System.Windows.Forms.ComboBox
        Me.cboFreqParam = New System.Windows.Forms.ComboBox
        Me.lblFreqParam = New System.Windows.Forms.Label
        Me.txtFreqParam = New System.Windows.Forms.TextBox
        Me.cboEmailFreq = New System.Windows.Forms.ComboBox
        Me.lblEmailFreq = New System.Windows.Forms.Label
        Me.txtEmailParam = New System.Windows.Forms.TextBox
        Me.lblEmailParam = New System.Windows.Forms.Label
        Me.lblEmailType = New System.Windows.Forms.Label
        Me.cmdClose = New System.Windows.Forms.Button
        Me.cmdAddEdit = New System.Windows.Forms.Button
        Me.cmdDelete = New System.Windows.Forms.Button
        Me.cboLocation = New System.Windows.Forms.ComboBox
        Me.lblLocation = New System.Windows.Forms.Label
        Me.grpCurrentSchedules.SuspendLayout()
        CType(Me.gridSchedules, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpEmailDetail.SuspendLayout()
        CType(Me.cboNextRunDate, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'grpCurrentSchedules
        '
        Me.grpCurrentSchedules.Controls.Add(Me.gridSchedules)
        Me.grpCurrentSchedules.Location = New System.Drawing.Point(8, 8)
        Me.grpCurrentSchedules.Name = "grpCurrentSchedules"
        Me.grpCurrentSchedules.Size = New System.Drawing.Size(832, 192)
        Me.grpCurrentSchedules.TabIndex = 0
        Me.grpCurrentSchedules.TabStop = False
        Me.grpCurrentSchedules.Text = "Current Active Schedules"
        '
        'gridSchedules
        '
        Me.gridSchedules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gridSchedules.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridSchedules.Location = New System.Drawing.Point(3, 16)
        Me.gridSchedules.Name = "gridSchedules"
        Me.gridSchedules.Size = New System.Drawing.Size(826, 173)
        Me.gridSchedules.TabIndex = 0
        '
        'grpEmailDetail
        '
        Me.grpEmailDetail.Controls.Add(Me.lblLocation)
        Me.grpEmailDetail.Controls.Add(Me.cboLocation)
        Me.grpEmailDetail.Controls.Add(Me.cboEmailTemplate)
        Me.grpEmailDetail.Controls.Add(Me.lblEmailTemplate)
        Me.grpEmailDetail.Controls.Add(Me.lblFreqParamError)
        Me.grpEmailDetail.Controls.Add(Me.cmdUpdate)
        Me.grpEmailDetail.Controls.Add(Me.meNextRunTime)
        Me.grpEmailDetail.Controls.Add(Me.lblNextRunTime)
        Me.grpEmailDetail.Controls.Add(Me.cboNextRunDate)
        Me.grpEmailDetail.Controls.Add(Me.lblRunDate)
        Me.grpEmailDetail.Controls.Add(Me.hiddenID)
        Me.grpEmailDetail.Controls.Add(Me.cboEmailType)
        Me.grpEmailDetail.Controls.Add(Me.cboFreqParam)
        Me.grpEmailDetail.Controls.Add(Me.lblFreqParam)
        Me.grpEmailDetail.Controls.Add(Me.txtFreqParam)
        Me.grpEmailDetail.Controls.Add(Me.cboEmailFreq)
        Me.grpEmailDetail.Controls.Add(Me.lblEmailFreq)
        Me.grpEmailDetail.Controls.Add(Me.txtEmailParam)
        Me.grpEmailDetail.Controls.Add(Me.lblEmailParam)
        Me.grpEmailDetail.Controls.Add(Me.lblEmailType)
        Me.grpEmailDetail.Location = New System.Drawing.Point(8, 200)
        Me.grpEmailDetail.Name = "grpEmailDetail"
        Me.grpEmailDetail.Size = New System.Drawing.Size(832, 152)
        Me.grpEmailDetail.TabIndex = 1
        Me.grpEmailDetail.TabStop = False
        Me.grpEmailDetail.Text = "Email Detail"
        '
        'cboEmailTemplate
        '
        Me.cboEmailTemplate.FormattingEnabled = True
        Me.cboEmailTemplate.Location = New System.Drawing.Point(168, 112)
        Me.cboEmailTemplate.Name = "cboEmailTemplate"
        Me.cboEmailTemplate.Size = New System.Drawing.Size(184, 21)
        Me.cboEmailTemplate.TabIndex = 9
        '
        'lblEmailTemplate
        '
        Me.lblEmailTemplate.Location = New System.Drawing.Point(64, 112)
        Me.lblEmailTemplate.Name = "lblEmailTemplate"
        Me.lblEmailTemplate.Size = New System.Drawing.Size(100, 23)
        Me.lblEmailTemplate.TabIndex = 8
        Me.lblEmailTemplate.Text = "Email Template :"
        Me.lblEmailTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblFreqParamError
        '
        Me.lblFreqParamError.Location = New System.Drawing.Point(552, 48)
        Me.lblFreqParamError.Name = "lblFreqParamError"
        Me.lblFreqParamError.Size = New System.Drawing.Size(152, 23)
        Me.lblFreqParamError.TabIndex = 0
        '
        'cmdUpdate
        '
        Me.cmdUpdate.Location = New System.Drawing.Point(744, 120)
        Me.cmdUpdate.Name = "cmdUpdate"
        Me.cmdUpdate.Size = New System.Drawing.Size(75, 23)
        Me.cmdUpdate.TabIndex = 8
        Me.cmdUpdate.Text = "&Update"
        '
        'meNextRunTime
        '
        Me.meNextRunTime.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.Time
        Me.meNextRunTime.InputMask = "{LOC}hh:mm"
        Me.meNextRunTime.Location = New System.Drawing.Point(464, 80)
        Me.meNextRunTime.Name = "meNextRunTime"
        Me.meNextRunTime.Size = New System.Drawing.Size(72, 20)
        Me.meNextRunTime.SpinButtonDisplayStyle = Infragistics.Win.SpinButtonDisplayStyle.OnRight
        Me.meNextRunTime.TabIndex = 6
        Me.meNextRunTime.Text = ":"
        '
        'lblNextRunTime
        '
        Me.lblNextRunTime.Location = New System.Drawing.Point(360, 80)
        Me.lblNextRunTime.Name = "lblNextRunTime"
        Me.lblNextRunTime.Size = New System.Drawing.Size(96, 23)
        Me.lblNextRunTime.TabIndex = 0
        Me.lblNextRunTime.Text = "Next Run Time :"
        Me.lblNextRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cboNextRunDate
        '
        Me.cboNextRunDate.BackColor = System.Drawing.SystemColors.Window
        Me.cboNextRunDate.DateButtons.Add(DateButton1)
        Me.cboNextRunDate.Location = New System.Drawing.Point(168, 80)
        Me.cboNextRunDate.Name = "cboNextRunDate"
        Me.cboNextRunDate.NonAutoSizeHeight = 23
        Me.cboNextRunDate.Size = New System.Drawing.Size(120, 21)
        Me.cboNextRunDate.TabIndex = 5
        '
        'lblRunDate
        '
        Me.lblRunDate.Location = New System.Drawing.Point(64, 80)
        Me.lblRunDate.Name = "lblRunDate"
        Me.lblRunDate.Size = New System.Drawing.Size(100, 23)
        Me.lblRunDate.TabIndex = 0
        Me.lblRunDate.Text = "Next Run Date :"
        Me.lblRunDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'hiddenID
        '
        Me.hiddenID.Location = New System.Drawing.Point(552, 80)
        Me.hiddenID.Name = "hiddenID"
        Me.hiddenID.Size = New System.Drawing.Size(100, 23)
        Me.hiddenID.TabIndex = 0
        Me.hiddenID.Visible = False
        '
        'cboEmailType
        '
        Me.cboEmailType.Location = New System.Drawing.Point(168, 16)
        Me.cboEmailType.Name = "cboEmailType"
        Me.cboEmailType.Size = New System.Drawing.Size(184, 21)
        Me.cboEmailType.TabIndex = 1
        '
        'cboFreqParam
        '
        Me.cboFreqParam.Location = New System.Drawing.Point(624, 16)
        Me.cboFreqParam.Name = "cboFreqParam"
        Me.cboFreqParam.Size = New System.Drawing.Size(121, 21)
        Me.cboFreqParam.TabIndex = 3
        Me.cboFreqParam.Visible = False
        '
        'lblFreqParam
        '
        Me.lblFreqParam.Location = New System.Drawing.Point(360, 48)
        Me.lblFreqParam.Name = "lblFreqParam"
        Me.lblFreqParam.Size = New System.Drawing.Size(96, 23)
        Me.lblFreqParam.TabIndex = 0
        Me.lblFreqParam.Text = "Freq. Param :"
        Me.lblFreqParam.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtFreqParam
        '
        Me.txtFreqParam.Enabled = False
        Me.txtFreqParam.Location = New System.Drawing.Point(464, 48)
        Me.txtFreqParam.Name = "txtFreqParam"
        Me.txtFreqParam.Size = New System.Drawing.Size(72, 20)
        Me.txtFreqParam.TabIndex = 4
        '
        'cboEmailFreq
        '
        Me.cboEmailFreq.Location = New System.Drawing.Point(464, 16)
        Me.cboEmailFreq.Name = "cboEmailFreq"
        Me.cboEmailFreq.Size = New System.Drawing.Size(152, 21)
        Me.cboEmailFreq.TabIndex = 3
        '
        'lblEmailFreq
        '
        Me.lblEmailFreq.Location = New System.Drawing.Point(360, 16)
        Me.lblEmailFreq.Name = "lblEmailFreq"
        Me.lblEmailFreq.Size = New System.Drawing.Size(96, 23)
        Me.lblEmailFreq.TabIndex = 0
        Me.lblEmailFreq.Text = "Email Freq :"
        Me.lblEmailFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtEmailParam
        '
        Me.txtEmailParam.Location = New System.Drawing.Point(168, 48)
        Me.txtEmailParam.Name = "txtEmailParam"
        Me.txtEmailParam.Size = New System.Drawing.Size(120, 20)
        Me.txtEmailParam.TabIndex = 2
        '
        'lblEmailParam
        '
        Me.lblEmailParam.Location = New System.Drawing.Point(32, 48)
        Me.lblEmailParam.Name = "lblEmailParam"
        Me.lblEmailParam.Size = New System.Drawing.Size(128, 23)
        Me.lblEmailParam.TabIndex = 0
        Me.lblEmailParam.Text = "Email Param :"
        Me.lblEmailParam.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblEmailType
        '
        Me.lblEmailType.Location = New System.Drawing.Point(64, 16)
        Me.lblEmailType.Name = "lblEmailType"
        Me.lblEmailType.Size = New System.Drawing.Size(100, 23)
        Me.lblEmailType.TabIndex = 0
        Me.lblEmailType.Text = "Email Type :"
        Me.lblEmailType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cmdClose
        '
        Me.cmdClose.Location = New System.Drawing.Point(752, 360)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(75, 23)
        Me.cmdClose.TabIndex = 2
        Me.cmdClose.Text = "&Close"
        '
        'cmdAddEdit
        '
        Me.cmdAddEdit.Location = New System.Drawing.Point(8, 360)
        Me.cmdAddEdit.Name = "cmdAddEdit"
        Me.cmdAddEdit.Size = New System.Drawing.Size(75, 23)
        Me.cmdAddEdit.TabIndex = 3
        Me.cmdAddEdit.Text = "&Add / Edit"
        '
        'cmdDelete
        '
        Me.cmdDelete.Location = New System.Drawing.Point(96, 360)
        Me.cmdDelete.Name = "cmdDelete"
        Me.cmdDelete.Size = New System.Drawing.Size(75, 23)
        Me.cmdDelete.TabIndex = 4
        Me.cmdDelete.Text = "&Delete"
        '
        'cboLocation
        '
        Me.cboLocation.FormattingEnabled = True
        Me.cboLocation.Location = New System.Drawing.Point(464, 112)
        Me.cboLocation.Name = "cboLocation"
        Me.cboLocation.Size = New System.Drawing.Size(152, 21)
        Me.cboLocation.TabIndex = 10
        '
        'lblLocation
        '
        Me.lblLocation.AutoSize = True
        Me.lblLocation.Location = New System.Drawing.Point(402, 115)
        Me.lblLocation.Name = "lblLocation"
        Me.lblLocation.Size = New System.Drawing.Size(54, 13)
        Me.lblLocation.TabIndex = 11
        Me.lblLocation.Text = "Location :"
        '
        'EmailSchedules
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(848, 390)
        Me.Controls.Add(Me.cmdDelete)
        Me.Controls.Add(Me.cmdAddEdit)
        Me.Controls.Add(Me.grpEmailDetail)
        Me.Controls.Add(Me.grpCurrentSchedules)
        Me.Controls.Add(Me.cmdClose)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EmailSchedules"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Framework Email Schedules"
        Me.grpCurrentSchedules.ResumeLayout(False)
        CType(Me.gridSchedules, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpEmailDetail.ResumeLayout(False)
        Me.grpEmailDetail.PerformLayout()
        CType(Me.cboNextRunDate, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        cboNextRunDate.Dispose()
        meNextRunTime.Dispose()
        Close()
    End Sub

    Private Sub EmailSchedules_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Setup()

            GetCurrentSchedules()

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
            Close()
        End Try
    End Sub

    Private Sub Setup()
        cboEmailFreq.Items.Insert(cUtility.emailFreq.Once, "Once")
        cboEmailFreq.Items.Insert(cUtility.emailFreq.Daily, "Daily")
        cboEmailFreq.Items.Insert(cUtility.emailFreq.Weekly, "Weekly")
        cboEmailFreq.Items.Insert(cUtility.emailFreq.MonthlyOnFirstXDay, "Monthly On 1st X day")
        cboEmailFreq.Items.Insert(cUtility.emailFreq.MonthlyOnDay, "Monthly (Day of Month)")
        cboEmailFreq.Items.Insert(cUtility.emailFreq.Every_n_Days, "Every n Days")

        cboEmailType.Items.Insert(0, "[None]")
        cboEmailType.Items.Insert(cUtility.emailType.ContractReview, "Contract Review")
        cboEmailType.Items.Insert(cUtility.emailType.OverdueInvoice, "Overdue Invoice")
        cboEmailType.Items.Insert(cUtility.emailType.AuditCleardown, "Audit Cleardown")
        cboEmailType.Items.Insert(cUtility.emailType.LicenceExpiry, "Licence Expiry")

        cboEmailType.SelectedIndex = 0

        cboLocation.Items.AddRange(FWEmail.SubAccounts.CreateDropDown)

        tmpTop = txtFreqParam.Top
        tmpLeft = txtFreqParam.Left
        tmpTopHold = cboFreqParam.Top
        tmpLeftHold = cboFreqParam.Left
    End Sub

    Private Sub GetCurrentSchedules()
        Dim sql As New System.Text.StringBuilder
        Dim dset As DataSet
        Dim drow As DataRow
        Dim dtable As New System.Data.DataTable("schedules")

        With dtable
            .Columns.Add(New DataColumn("Select", GetType(Boolean)))
            .Columns.Add(New DataColumn("ScheduleId", GetType(Integer)))
            .Columns.Add(New DataColumn("EmailType", GetType(String)))
            .Columns.Add(New DataColumn("EmailParam", GetType(String)))
            .Columns.Add(New DataColumn("EmailFreq", GetType(String)))
            .Columns.Add(New DataColumn("FreqParam", GetType(String)))
            .Columns.Add(New DataColumn("NextRunDate", GetType(String)))
            .Columns.Add(New DataColumn("NextRunTime", GetType(String)))
            If FWEmail.ActiveDBVersion >= 18 Then
                .Columns.Add(New DataColumn("TemplateName", GetType(String)))
                .Columns.Add(New DataColumn("Description", GetType(String)))
            End If
        End With

        sql.Append("SELECT email_schedule.* ")
        sql.Append(",email_templates.[TemplateName] ")
        sql.Append(",accountsSubAccounts.[Description] ")
        sql.Append("FROM email_schedule")
        sql.Append(" LEFT JOIN email_templates ON email_schedule.[templateId] = email_templates.[templateId]")
        sql.Append(" LEFT JOIN accountsSubAccounts ON email_schedule.[runSubAccountId] = accountsSubAccounts.subAccountId")

        'If fws.glUseWebService = False Then
        ' must be connecting directly to the database
        Dim db As New DBConnection(FWEmail.ConnectionString)

        dset = db.GetDataSet(sql.ToString())
        'Else
        '' must be connecting to db via the web service
        'Dim svr As New FWEmailServer.FWEmailServer

        'svr.Url = fws.glWebEmailerURL & "/FWEmailServer.asmx"
        'dset = svr.RunSQL(sql.ToString)
        'svr = Nothing
        'End If

        Dim hasSchedules As Boolean
        Dim scheduleItem As String

        hasSchedules = False
        For Each drow In dset.Tables(0).Rows
            hasSchedules = True

            Dim iRow As DataRow
            iRow = dtable.NewRow

            iRow("Select") = False

            iRow("ScheduleId") = drow.Item("scheduleId")

            Select Case CType(drow.Item("EmailType"), cUtility.emailType)
                Case cUtility.emailType.ContractReview
                    scheduleItem = "Contract Review"
                Case cUtility.emailType.AuditCleardown
                    scheduleItem = "Audit Cleardown"
                Case cUtility.emailType.LicenceExpiry
                    scheduleItem = "Licence Expiry"
                Case cUtility.emailType.OverdueInvoice
                    scheduleItem = "Overdue Invoice"
                Case Else
                    scheduleItem = "Unknown Type"
            End Select
            iRow("EmailType") = scheduleItem

            If IsDBNull(drow.Item("emailParam")) = False Then
                scheduleItem = Trim(drow.Item("emailParam"))
            Else
                scheduleItem = ""
            End If
            iRow("EmailParam") = scheduleItem

            Select Case CType(drow.Item("emailFrequency"), cUtility.emailFreq)
                Case cUtility.emailFreq.Daily
                    scheduleItem = "Daily"
                Case cUtility.emailFreq.Once
                    scheduleItem = "Once"
                Case cUtility.emailFreq.Weekly
                    scheduleItem = "Weekly"
                    If IsDBNull(drow.Item("frequencyParam")) = False Then
                        iRow("FreqParam") = [Enum].GetName(GetType(DayOfWeek), drow.Item("frequencyParam"))
                    End If
                Case cUtility.emailFreq.MonthlyOnFirstXDay
                    scheduleItem = "First X Day of the Month"
                    If IsDBNull(drow.Item("frequencyParam")) = False Then
                        iRow("FreqParam") = [Enum].GetName(GetType(DayOfWeek), drow.Item("frequencyParam"))
                    End If
                Case cUtility.emailFreq.MonthlyOnDay
                    scheduleItem = "Monthly (Day of Month)"
                    If IsDBNull(drow.Item("frequencyParam")) = False Then
                        iRow("FreqParam") = Trim(Str(drow.Item("frequencyParam"))) & "\mm\yy"
                    End If
                Case cUtility.emailFreq.Every_n_Days
                    scheduleItem = "Every n Days"
                    If IsDBNull(drow.Item("frequencyParam")) = False Then
                        iRow("FreqParam") = Trim(Str(drow.Item("frequencyParam")))
                    End If
                Case Else
                    scheduleItem = "Invalid Frequency"
            End Select
            iRow("EmailFreq") = scheduleItem

            Dim tmpNRD As Date

            If IsDBNull(drow.Item("nextRunDate")) = False Then
                tmpNRD = CDate(drow.Item("nextRunDate"))

                iRow("NextRunDate") = Format(tmpNRD, cDef.DATE_FORMAT)
                iRow("NextRunTime") = Format(tmpNRD, "HH:mm:ss")
            Else
                iRow("NextRunDate") = "Unspecified"
                iRow("NextRunTime") = "Unspecified"
            End If

            If FWEmail.ActiveDBVersion >= 18 Then
                If IsDBNull(drow.Item("templateName")) = False Then
                    iRow("TemplateName") = drow.Item("templateName")

                    If IsDBNull(drow.Item("runSubAccountId")) = False Then
                        iRow("Description") = FWEmail.SubAccounts.getSubAccountById(drow.Item("runSubAccountId")).Description
                    End If
                End If
            End If
            dtable.Rows.Add(iRow)
        Next

        If hasSchedules = True Then
            gridSchedules.DataBindings.Clear()
            gridSchedules.DataSource = dtable
            scheduleGrid_Init()
        End If

        If FWEmail.ActiveDBVersion < 18 Then
            cboEmailTemplate.Visible = False
            lblEmailTemplate.Visible = False
        End If
    End Sub

    Private Sub scheduleGrid_Init()
        With gridSchedules
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False

            .Columns("Select").Width = 25
            .Columns("Select").HeaderText = "Tag"
            .Columns("Select").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("ScheduleId").Width = 0
            .Columns("ScheduleId").Visible = False
            .Columns("EmailType").HeaderText = "Email Type"
            .Columns("EmailType").Width = 120
            .Columns("EmailType").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("EmailParam").HeaderText = "Email Param."
            .Columns("EmailParam").Width = 60
            .Columns("EmailParam").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("EmailFreq").HeaderText = "Email Frequency"
            .Columns("EmailFreq").Width = 110
            .Columns("EmailFreq").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("FreqParam").HeaderText = "Freq. Param."
            .Columns("FreqParam").Width = 60
            .Columns("FreqParam").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("NextRunDate").HeaderText = "Next Run Date"
            .Columns("NextRunDate").Width = 95
            .Columns("NextRunDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns("NextRunTime").HeaderText = "Next Run Time"
            .Columns("NextRunTime").Width = 90
            .Columns("NextRunTime").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            If FWEmail.ActiveDBVersion >= 18 Then
                .Columns("TemplateName").Width = 120
                .Columns("TemplateName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns("TemplateName").HeaderText = "Email Template"
                .Columns("Description").HeaderText = "Location"
                .Columns("Description").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Columns("Description").Width = 100
            End If
        End With
    End Sub

    Private Sub cmdAddEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddEdit.Click
        Dim sql As String
        Dim editID As Integer
        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        ' is anything selected for edit.
        editID = 0

        Dim nRow As Integer = 0

        While nRow < gridSchedules.Rows.Count
            If gridSchedules.Rows(nRow).Cells("Select").Value = True Then
                editID = gridSchedules.Rows(nRow).Cells("ScheduleId").Value
                hiddenID.Text = editID.ToString
                Exit While
            End If
            nRow += 1
        End While

        If editID = 0 Then
            ' must be adding
            cboEmailType.SelectedIndex = 0
            cboEmailFreq.SelectedIndex = 0
            cboLocation.SelectedIndex = 0
            txtFreqParam.Text = ""
            txtEmailParam.Text = ""
            cboNextRunDate.Value = Today
            meNextRunTime.Value = Format("HH:mm:ss", Now())
        Else
            ' must be editing
            Dim scheduleId As Integer
            Dim dset As New DataSet

            scheduleId = gridSchedules.Rows(nRow).Cells("ScheduleId").Value

            sql = "SELECT * FROM email_schedule WHERE [scheduleId] = @schedId"

            'If fws.glUseWebService = False Then
            Dim db As New DBConnection(FWEmail.ConnectionString)
            db.sqlexecute.Parameters.AddWithValue("@schedId", scheduleId)

            dset = db.GetDataSet(sql)
            'Else
            '    Dim svr As New FWEmailServer.FWEmailServer
            '    svr.Url = fws.glWebEmailerURL & "/FWEmailServer.asmx"

            '    dset = svr.RunSQL(sql)

            '    svr.Dispose()
            '    svr = Nothing
            'End If

            With dset.Tables(0).Rows(0)
                GetTemplates(CType(.Item("emailType"), cUtility.emailType))

                cboEmailType.SelectedIndex = .Item("emailType")
                If IsDBNull(.Item("emailParam")) = False Then
                    txtEmailParam.Text = Trim(.Item("emailParam"))
                Else
                    txtEmailParam.Text = ""
                End If

                If IsDBNull(.Item("emailFrequency")) = False Then
                    cboEmailFreq.SelectedIndex = .Item("emailFrequency")
                Else
                    cboEmailFreq.SelectedIndex = 0
                End If

                Select Case CType(.Item("emailFrequency"), cUtility.emailFreq)
                    Case cUtility.emailFreq.MonthlyOnDay, cUtility.emailFreq.MonthlyOnFirstXDay, cUtility.emailFreq.Weekly
                        If IsDBNull(.Item("frequencyParam")) = False Then
                            cboFreqParam.SelectedIndex = .Item("frequencyParam")
                        Else
                            cboFreqParam.SelectedIndex = 0
                        End If

                    Case Else
                        If IsDBNull(.Item("frequencyParam")) = False Then
                            txtFreqParam.Text = Trim(.Item("frequencyParam"))
                        Else
                            txtFreqParam.Text = ""
                        End If

                End Select

                If IsDBNull(.Item("nextRunDate")) = False Then
                    cboNextRunDate.Value = Format(.Item("nextRunDate"), cDef.DATE_FORMAT)
                    meNextRunTime.Value = Format(.Item("nextRunDate"), "HH:mm:ss")
                End If

                If IsDBNull(.Item("templateId")) = False Then
                    Dim RowIdx As Integer = 0

                    While RowIdx <= cboEmailTemplate.Items.Count - 1
                        cboEmailTemplate.SelectedIndex = RowIdx
                        If cboEmailTemplate.SelectedValue = .Item("templateId") Then
                            Exit While
                        End If
                        RowIdx += 1
                    End While
                End If


                If IsDBNull(.Item("runSubAccountId")) = False Then
                    Dim idx As Integer
                    
                    idx = cboLocation.FindStringExact(FWEmail.SubAccounts.getSubAccountById(.Item("runSubAccountId")).Description)
                    cboLocation.SelectedIndex = idx
                End If
            End With
        End If
    End Sub

    Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        If cboEmailType.SelectedIndex <= 0 Then
            MsgBox("Please select an Email Type before updating.", MsgBoxStyle.Information, "FWUtilities")
            Exit Sub
        End If

        Dim tmpNRD As DateTime
        If DateTime.TryParse(cboNextRunDate.Text & " " & meNextRunTime.Text, tmpNRD) = False Then
            MsgBox("Please provide a valid date and run time before updating.", MsgBoxStyle.Information, "FWUtilities")
            Exit Sub
        End If

        If cboEmailFreq.SelectedIndex < 0 Then
            MsgBox("Please select an Email Frequency before updating.", MsgBoxStyle.Information, "FWUtilities")
            Exit Sub
        End If

        Select Case CType(cboEmailFreq.SelectedIndex, cUtility.emailFreq)
            Case cUtility.emailFreq.MonthlyOnDay, cUtility.emailFreq.MonthlyOnFirstXDay
                If cboFreqParam.SelectedIndex <= 0 Then
                    MsgBox("Please select an Email Frequency Parameter before updating.", MsgBoxStyle.Information, "FWUtilities")
                    Exit Sub
                End If

            Case cUtility.emailFreq.Weekly
                If cboFreqParam.SelectedIndex < 0 Then
                    MsgBox("Please select an Email Frequency Parameter before updating.", MsgBoxStyle.Information, "FWUtilities")
                    Exit Sub
                End If

            Case cUtility.emailFreq.Every_n_Days
                Dim freqParam As Integer
                Dim isNumber As Boolean = Integer.TryParse(txtFreqParam.Text, freqParam)
                If isNumber = False Or freqParam = 0 Then
                    MsgBox("Invalid Email Frequency Parameter provided. Must be numeric and greater than 0", MsgBoxStyle.Information, "FWUtilities")
                    Exit Sub
                End If

            Case Else

        End Select

        Select Case CType(cboEmailType.SelectedValue, cUtility.emailType)
            Case cUtility.emailType.ContractReview
                If txtEmailParam.Text = String.Empty Then
                    txtEmailParam.Text = "0"
                End If
            Case Else
        End Select

        If cboLocation.SelectedIndex < 0 Then
            MsgBox("A Database Location must be specified for the schedule", MsgBoxStyle.Exclamation, "FWUtilities")
            Exit Sub
        End If

        If cboEmailTemplate.SelectedIndex < 0 Then
            MsgBox("An email template must be selected to correspond to the schedule", MsgBoxStyle.Critical, "FWUtilities")
            Exit Sub
        End If

        'If fws.glUseWebService = False Then
        Dim db As New DBConnection(FWEmail.ConnectionString)

        db.sqlexecute.Parameters.AddWithValue("@emailType", cboEmailType.SelectedIndex)
        db.sqlexecute.Parameters.AddWithValue("@emailParam", txtEmailParam.Text.Trim)
        db.sqlexecute.Parameters.AddWithValue("@emailFrequency", cboEmailFreq.SelectedIndex)
        Select Case CType(cboEmailFreq.SelectedIndex, cUtility.emailFreq)
            Case cUtility.emailFreq.Daily, cUtility.emailFreq.Once, cUtility.emailFreq.Every_n_Days
                db.sqlexecute.Parameters.AddWithValue("@frequencyParam", txtFreqParam.Text.Trim)

            Case cUtility.emailFreq.MonthlyOnDay, cUtility.emailFreq.MonthlyOnFirstXDay, cUtility.emailFreq.Weekly
                db.sqlexecute.Parameters.AddWithValue("@frequencyParam", cboFreqParam.SelectedIndex)
            Case Else

        End Select

        db.sqlexecute.Parameters.AddWithValue("@nextRunDate", tmpNRD)

        If FWEmail.ActiveDBVersion >= 18 Then
            db.sqlexecute.Parameters.AddWithValue("@templateId", cboEmailTemplate.SelectedValue)

            Dim locId As Integer

            locId = FWEmail.SubAccounts.getSubAccountById(cboLocation.SelectedItem.Value).SubAccountID
            db.sqlexecute.Parameters.AddWithValue("@locId", locId)
            'For Each kvp As KeyValuePair(Of Integer, cAccountSubAccount) In subaccs.getSubAccountsCollection
            '    Dim sa As cAccountSubAccount = CType(kvp.Value, cAccountSubAccount)

            '    If sa.Description = cboLocation.Items(cboLocation.SelectedIndex) Then
            '        locId = sa.SubAccountID
            '        Exit For
            '    End If
            'Next
        End If

        'db.SetFieldValue("Run Time", meNextRunTime.Text, "S")
        Dim sql As String
        If Val(hiddenID.Text) > 0 Then
            ' must be updating an edited field
            sql = "update email_schedule set emailType=@emailType, emailParam=@emailParam, emailFrequency=@emailFrequency, frequencyParam=@frequencyParam, templateId=@templateId, runSubAccountId=@locId, nextRunDate=@nextRunDate where scheduleId=@schedId"
            db.sqlexecute.Parameters.AddWithValue("@schedId", hiddenID.Text)
        Else
            ' must be adding a new schedule to the list
            sql = "insert into email_schedule (emailType, emailParam, emailFrequency, frequencyParam, templateId, runSubAccountId, nextRunDate) values (@emailType, @emailParam, @emailFrequency, @frequencyParam, @templateId, @locId, @nextRunDate)"
        End If
        db.ExecuteSQL(sql)
        'Else
        '    Dim svr As New FWEmailServer.FWEmailServer
        '    Dim sql As String
        '    Dim dset As New DataSet

        '    svr.Url = fws.glWebEmailerURL & "\FWEmailServer.asmx"

        '    If Val(hiddenID.Text) < 1 Then
        '        sql = "INSERT INTO email_schedule ([EmailType],[EmailParam],[EmailFrequency],[FrequencyParam],[NextRunDate]) "
        '        sql += "VALUES (" & Trim(Str(cboEmailType.SelectedIndex)) & ",'" & Trim(txtEmailParam.Text) & "'," & Trim(Str(cboEmailFreq.SelectedIndex)) & ","
        '        Select Case CType(cboEmailFreq.SelectedIndex, cUtility.emailFreq)
        '            Case cUtility.emailFreq.Daily, cUtility.emailFreq.Once, cUtility.emailFreq.Every_n_Days
        '                If Trim(txtFreqParam.Text) = "" Then
        '                    sql += "0"
        '                Else
        '                    sql += Trim(txtFreqParam.Text)
        '                End If

        '            Case cUtility.emailFreq.MonthlyOnDay, cUtility.emailFreq.MonthlyOnFirstXDay, cUtility.emailFreq.Weekly
        '                If cboFreqParam.SelectedIndex >= 0 Then
        '                    sql += Trim(Str(cboFreqParam.SelectedIndex))
        '                Else
        '                    ' not selected a value, so default to first selection
        '                    sql += "0"
        '                End If
        '            Case Else

        '        End Select

        '        sql += ",CONVERT(datetime,'" & Trim(Format(CDate(cboNextRunDate.Text), "yyyy-MM-dd")) & " " & Trim(Format(CDate(meNextRunTime.Text), "HH:mm:ss")) & "',120)"
        '        sql += ")"
        '    Else
        '        sql = "UPDATE email_schedule SET "
        '        sql += "[EmailType] = " & Trim(Str(cboEmailType.SelectedIndex)) & ","
        '        sql += "[EmailParam] = '" & Trim(txtEmailParam.Text) & "',"
        '        sql += "[EmailFrequency] = " & Trim(Str(cboEmailFreq.SelectedIndex)) & ","
        '        sql += "[FrequencyParam] = "
        '        Select Case CType(cboEmailFreq.SelectedIndex, cUtility.emailFreq)
        '            Case cUtility.emailFreq.Daily, cUtility.emailFreq.Once, cUtility.emailFreq.Every_n_Days
        '                If Trim(txtFreqParam.Text) = "" Then
        '                    sql += "0"
        '                Else
        '                    sql += Trim(txtFreqParam.Text)
        '                End If

        '            Case cUtility.emailFreq.MonthlyOnDay, cUtility.emailFreq.MonthlyOnFirstXDay, cUtility.emailFreq.Weekly
        '                If cboFreqParam.SelectedIndex >= 0 Then
        '                    sql += Trim(Str(cboFreqParam.SelectedIndex))
        '                Else
        '                    ' not selected a value, so default to first selection
        '                    sql += "0"
        '                End If
        '            Case Else

        '        End Select
        '        sql += ","
        '        sql += "[NextRunDate] = CONVERT(datetime,'" & Trim(Format(CDate(cboNextRunDate.Text), "yyyy-MM-dd")) & " " & Trim(Format(CDate(meNextRunTime.Text), "HH:mm:ss")) & "',120) "
        '        'sql += "[Run Time] = '" & Trim(meNextRunTime.Text) & "' "
        '        sql += "WHERE [ScheduleId] = " & Trim(hiddenID.Text)
        '    End If
        '    dset = svr.RunSQL(sql)

        '    If Not svr Is Nothing Then
        '        svr.Dispose()
        '    End If

        '    If Not dset Is Nothing Then
        '        dset.Dispose()
        '    End If

        '    svr = Nothing
        '    dset = Nothing
        'End If

        GetCurrentSchedules()

        ' clear settings in the edit boxes
        cboEmailType.SelectedIndex = -1
        txtEmailParam.Text = ""
        cboEmailFreq.SelectedIndex = -1
        txtFreqParam.Text = "0"
        cboFreqParam.SelectedIndex = -1
        meNextRunTime.Value = "00:00"
        cboNextRunDate.Value = Now
        cboLocation.SelectedIndex = -1
        cboEmailTemplate.SelectedIndex = -1
        cboEmailTemplate.DataBindings.Clear()
        cboEmailTemplate.ResetText()
    End Sub

    Private Sub cboEmailType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboEmailType.SelectedIndexChanged
        Select Case CType(cboEmailType.SelectedIndex, cUtility.emailType)
            Case cUtility.emailType.AuditCleardown, cUtility.emailType.LicenceExpiry, cUtility.emailType.OverdueInvoice
                txtEmailParam.Enabled = False
                lblEmailParam.Text = "Email Param :"

            Case cUtility.emailType.ContractReview
                txtEmailParam.Enabled = True
                lblEmailParam.Text = "Look n days ahead :"

            Case Else
                txtEmailParam.Enabled = False
                lblEmailParam.Text = "Email Param :"
        End Select

        ' re-populate email template list with only those valid for the selected type of email
        If FWEmail.ActiveDBVersion >= 18 Then
            GetTemplates(CType(cboEmailType.SelectedIndex, cUtility.emailType))
        End If
    End Sub

    Private Sub cboEmailFreq_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboEmailFreq.SelectedIndexChanged
        Select Case CType(cboEmailFreq.SelectedIndex, cUtility.emailFreq)
            Case cUtility.emailFreq.Once, cUtility.emailFreq.Daily
                txtFreqParam.Enabled = False
                txtFreqParam.Visible = True
                cboFreqParam.Visible = False
                cboFreqParam.Enabled = False
                txtFreqParam.Left = tmpLeft
                txtFreqParam.Top = tmpTop
                cboFreqParam.Top = tmpTopHold
                cboFreqParam.Left = tmpLeftHold
                lblFreqParamError.Visible = True

            Case cUtility.emailFreq.MonthlyOnDay
                cboFreqParam.TabIndex = txtFreqParam.TabIndex
                txtFreqParam.Enabled = False
                txtFreqParam.Visible = False
                cboFreqParam.Visible = True
                cboFreqParam.Enabled = True
                txtFreqParam.Left = tmpLeftHold
                txtFreqParam.Top = tmpTopHold
                cboFreqParam.Top = tmpTop
                cboFreqParam.Left = tmpLeft
                lblFreqParamError.Visible = False

                ' populate combo with 1st,2nd,3rd etc.
                Dim x As Integer
                cboFreqParam.Items.Clear()
                cboFreqParam.Items.Add("Please select...")
                For x = 1 To 31
                    'cboFreqParam.Items.Insert(x, Trim(Format(x, "0#")) & "\mm\yyyy")
                    cboFreqParam.Items.Add(Trim(Format(x, "0#")) & "\mm\yyyy")
                Next

            Case cUtility.emailFreq.MonthlyOnFirstXDay, cUtility.emailFreq.Weekly
                cboFreqParam.TabIndex = txtFreqParam.TabIndex
                txtFreqParam.Enabled = False
                txtFreqParam.Visible = False
                cboFreqParam.Visible = True
                cboFreqParam.Enabled = True
                txtFreqParam.Left = tmpLeftHold
                txtFreqParam.Top = tmpTopHold
                cboFreqParam.Top = tmpTop
                cboFreqParam.Left = tmpLeft
                lblFreqParamError.Visible = False

                ' populate with Sun-Sat
                Dim x As Integer
                cboFreqParam.Items.Clear()
                For x = 0 To 6
                    cboFreqParam.Items.Insert(x, [Enum].GetName(GetType(DayOfWeek), x))
                Next

                cboFreqParam.SelectedIndex = -1

            Case cUtility.emailFreq.Every_n_Days
                txtFreqParam.Enabled = True
                txtFreqParam.Visible = True
                cboFreqParam.Visible = False
                cboFreqParam.Enabled = False
                txtFreqParam.Left = tmpLeft
                txtFreqParam.Top = tmpTop
                cboFreqParam.Top = tmpTopHold
                cboFreqParam.Left = tmpLeftHold
                lblFreqParamError.Visible = True
            Case Else

        End Select

        lblFreqParamError.Text = ""
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        Dim nRow As Integer = 0
        Dim dlgRes As Microsoft.VisualBasic.MsgBoxResult
        Dim delCount As Integer
        Dim delIds As String
        Dim tables As New cTables(FWEmail.ActiveDBVersion)

        delCount = 0
        delIds = ""

        While nRow <= gridSchedules.Rows.Count - 1
            If gridSchedules.Rows(nRow).Cells("Select").Value = True Then
                If delIds <> "" Then
                    delIds += ","
                End If
                delCount += 1
                delIds += gridSchedules.Rows(nRow).Cells("ScheduleId").Value.ToString
            End If
            nRow += 1
        End While

        If delCount = 0 Then
            MsgBox("You have not selected to delete any schedules from the list.", MsgBoxStyle.Information, "FWUtilities")
            Exit Sub
        Else
            dlgRes = MsgBox("You have selected " & Trim(Str(delCount)) & " schedule(s) for deletion." & vbNewLine & "Are you sure?", MsgBoxStyle.YesNo, "FWUtilities")
            If dlgRes = MsgBoxResult.Yes Then
                Dim sql As String

                sql = "DELETE FROM email_schedule WHERE [scheduleId] IN (" & delIds.Trim & ")"
                Try

                    Dim db As New DBConnection(FWEmail.ConnectionString)
                    db.ExecuteSQL(sql)

                Catch ex As Exception
                    MsgBox("An error occurred trying to delete the schedule(s).", MsgBoxStyle.Critical, "FWUtilities Error")
                    MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                End Try
            End If
        End If

        GetCurrentSchedules()
    End Sub

    Private Sub txtFreqParam_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFreqParam.TextChanged
        If txtFreqParam.Enabled = True Then
            If IsNumeric(txtFreqParam.Text) = False Then
                lblFreqParamError.Text = "** Numeric only (days) **"
                cmdUpdate.Enabled = False
            Else
                lblFreqParamError.Text = ""
                cmdUpdate.Enabled = True
            End If
        End If
    End Sub

    Private Sub GetTemplates(ByVal eType As cUtility.emailType)
        Try
            Dim sql As New System.Text.StringBuilder
            Dim dset As DataSet = New DataSet

            sql.Append("SELECT [templateId],[templateName] FROM email_templates")
            sql.Append(" WHERE [templateType] = ")
            sql.Append(eType)
            sql.Append(" ORDER BY [templateName]")

            ' using direct connection to database
            Dim db As New DBConnection(FWEmail.ConnectionString)
            dset = db.GetDataSet(sql.ToString)

            cboEmailTemplate.DataBindings.Clear()
            cboEmailTemplate.MaxDropDownItems = 5
            cboEmailTemplate.DisplayMember = "templateName"
            cboEmailTemplate.ValueMember = "templateId"
            cboEmailTemplate.DataSource = dset.Tables(0)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "FWUtilities Error")
        End Try
    End Sub
End Class
