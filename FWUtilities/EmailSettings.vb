Public Class EmailSettings
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
    Friend WithEvents grpCheckInterval As System.Windows.Forms.GroupBox
    Friend WithEvents cmdIntervalUpdate As System.Windows.Forms.Button
    Friend WithEvents txtInterval As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents cmdStopScheduler As System.Windows.Forms.Button
    Friend WithEvents cmdStartScheduler As System.Windows.Forms.Button
    Friend WithEvents cmdKillThread As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(EmailSettings))
        Me.grpCheckInterval = New System.Windows.Forms.GroupBox
        Me.cmdIntervalUpdate = New System.Windows.Forms.Button
        Me.txtInterval = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmdClose = New System.Windows.Forms.Button
        Me.cmdStopScheduler = New System.Windows.Forms.Button
        Me.cmdStartScheduler = New System.Windows.Forms.Button
        Me.cmdKillThread = New System.Windows.Forms.Button
        Me.grpCheckInterval.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpCheckInterval
        '
        Me.grpCheckInterval.Controls.Add(Me.cmdIntervalUpdate)
        Me.grpCheckInterval.Controls.Add(Me.txtInterval)
        Me.grpCheckInterval.Controls.Add(Me.Label1)
        Me.grpCheckInterval.Location = New System.Drawing.Point(8, 8)
        Me.grpCheckInterval.Name = "grpCheckInterval"
        Me.grpCheckInterval.Size = New System.Drawing.Size(208, 56)
        Me.grpCheckInterval.TabIndex = 1
        Me.grpCheckInterval.TabStop = False
        Me.grpCheckInterval.Text = "Email Wake-up Check Interval"
        '
        'cmdIntervalUpdate
        '
        Me.cmdIntervalUpdate.Image = CType(resources.GetObject("cmdIntervalUpdate.Image"), System.Drawing.Image)
        Me.cmdIntervalUpdate.Location = New System.Drawing.Point(176, 24)
        Me.cmdIntervalUpdate.Name = "cmdIntervalUpdate"
        Me.cmdIntervalUpdate.Size = New System.Drawing.Size(24, 24)
        Me.cmdIntervalUpdate.TabIndex = 2
        '
        'txtInterval
        '
        Me.txtInterval.Location = New System.Drawing.Point(104, 24)
        Me.txtInterval.Name = "txtInterval"
        Me.txtInterval.Size = New System.Drawing.Size(64, 20)
        Me.txtInterval.TabIndex = 1
        Me.txtInterval.Text = "120"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Interval (minutes)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(72, 176)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(80, 24)
        Me.cmdClose.TabIndex = 5
        Me.cmdClose.Text = "Close"
        '
        'cmdStopScheduler
        '
        Me.cmdStopScheduler.Location = New System.Drawing.Point(120, 80)
        Me.cmdStopScheduler.Name = "cmdStopScheduler"
        Me.cmdStopScheduler.Size = New System.Drawing.Size(96, 23)
        Me.cmdStopScheduler.TabIndex = 4
        Me.cmdStopScheduler.Text = "Stop Scheduler"
        '
        'cmdStartScheduler
        '
        Me.cmdStartScheduler.Location = New System.Drawing.Point(8, 80)
        Me.cmdStartScheduler.Name = "cmdStartScheduler"
        Me.cmdStartScheduler.Size = New System.Drawing.Size(96, 23)
        Me.cmdStartScheduler.TabIndex = 3
        Me.cmdStartScheduler.Text = "Start Scheduler"
        '
        'cmdKillThread
        '
        Me.cmdKillThread.Image = CType(resources.GetObject("cmdKillThread.Image"), System.Drawing.Image)
        Me.cmdKillThread.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdKillThread.Location = New System.Drawing.Point(24, 120)
        Me.cmdKillThread.Name = "cmdKillThread"
        Me.cmdKillThread.Size = New System.Drawing.Size(176, 48)
        Me.cmdKillThread.TabIndex = 6
        Me.cmdKillThread.Text = "Kill Server Email Thread"
        Me.cmdKillThread.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'EmailSettings
        '
        Me.AcceptButton = Me.cmdClose
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(224, 206)
        Me.Controls.Add(Me.cmdKillThread)
        Me.Controls.Add(Me.cmdStartScheduler)
        Me.Controls.Add(Me.cmdStopScheduler)
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.grpCheckInterval)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EmailSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Email Settings"
        Me.grpCheckInterval.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdIntervalUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdIntervalUpdate.Click
        'Dim FWService As New FWEmailServer.FWEmailServer
        'Dim fws As FWSettings

        'fws = SetApplicationProperties()

        'FWService.Url = fws.glWebEmailerURL & "/FWEmailServer.asmx"

        'If FWService.SetWebCheckInterval(txtInterval.Text) = False Then
        '    MsgBox("Error updating Web Service with Web Check Interval", MsgBoxStyle.Critical, "FWEmail Error")
        'Else
        '    MsgBox("Web Service updated successfully with Interval", MsgBoxStyle.OKOnly, "FWEmail Success")
        'End If

        'FWService.Dispose()
        'FWService = Nothing
    End Sub

    Private Sub EmailSettings_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Me.Tag Is Nothing Then
            txtInterval.Text = Trim(Str(Me.Tag))
        Else
            txtInterval.Text = "120"
        End If

    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Close()
    End Sub

    Private Sub cmdStartScheduler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStartScheduler.Click
        'Dim FWService As New FWEmailServer.FWEmailServer
        'Dim fws As FWSettings

        'fws = SetApplicationProperties()

        'FWService.Url = fws.glWebEmailerURL & "/FWEmailServer.asmx"
        'FWService.StartScheduler()
        'FWService.Dispose()
        'FWService = Nothing
    End Sub

    Private Sub cmdStopScheduler_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStopScheduler.Click
        'Dim FWService As New FWEmailServer.FWEmailServer
        'Dim fws As FWSettings

        'fws = SetApplicationProperties()

        'FWService.Url = fws.glWebEmailerURL & "/FWEmailServer.asmx"
        'FWService.StopScheduler()
        'FWService.Dispose()
        'FWService = Nothing
    End Sub

    Private Sub cmdKillThread_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdKillThread.Click
        'Dim res As Microsoft.VisualBasic.MsgBoxResult

        'res = MsgBox("This will terminate the automatic email notifications permanently!" & vbNewLine & "The server will have to be restarted. Are you Sure?", MsgBoxStyle.Critical, "Framework Critical Message")
        'If res = MsgBoxResult.OK Then
        '    ' must provide password to do this
        '    Dim pwd As New PasswordEntry
        '    pwd.ShowDialog()

        '    Dim FWService As New FWEmailServer.FWEmailServer
        '    Dim fws As FWSettings
        '    Dim tmpStr As String

        '    fws = SetApplicationProperties()

        '    FWService.Url = fws.glWebEmailerURL & "/FWEmailServer.asmx"
        '    tmpStr = FWService.KillEmailThread(pwd.Tag)

        '    FWService.Dispose()
        '    FWService = Nothing
        '    pwd.Dispose()
        '    pwd = Nothing

        '    MsgBox(tmpStr, MsgBoxStyle.Information, "Framework Utilities")
        '    Close()
        'End If
    End Sub
End Class
