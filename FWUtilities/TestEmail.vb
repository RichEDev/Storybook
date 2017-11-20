Imports FWBase
Imports FWCommon
Imports Spend_Management
Imports SpendManagementLibrary

Public Class TestEmail
    Inherits System.Windows.Forms.Form
    Private subAccountId As Integer
    Private properties As cAccountProperties

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
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblToAddress As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents lblFromAddress As System.Windows.Forms.Label
    Friend WithEvents txtToAddress As System.Windows.Forms.TextBox
    Friend WithEvents lblSubject As System.Windows.Forms.Label
    Friend WithEvents lblSubjectText As System.Windows.Forms.Label
    Friend WithEvents grpMsgBody As System.Windows.Forms.GroupBox
    Friend WithEvents cmdSend As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents txtMsgBody As System.Windows.Forms.TextBox
    Friend WithEvents lblMailServer As System.Windows.Forms.Label
    Friend WithEvents lblSMTPServer As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(TestEmail))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.lblSMTPServer = New System.Windows.Forms.Label
        Me.lblMailServer = New System.Windows.Forms.Label
        Me.lblSubjectText = New System.Windows.Forms.Label
        Me.lblSubject = New System.Windows.Forms.Label
        Me.txtToAddress = New System.Windows.Forms.TextBox
        Me.lblFromAddress = New System.Windows.Forms.Label
        Me.lblFrom = New System.Windows.Forms.Label
        Me.lblToAddress = New System.Windows.Forms.Label
        Me.grpMsgBody = New System.Windows.Forms.GroupBox
        Me.txtMsgBody = New System.Windows.Forms.TextBox
        Me.cmdSend = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.grpMsgBody.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblSMTPServer)
        Me.GroupBox1.Controls.Add(Me.lblMailServer)
        Me.GroupBox1.Controls.Add(Me.lblSubjectText)
        Me.GroupBox1.Controls.Add(Me.lblSubject)
        Me.GroupBox1.Controls.Add(Me.txtToAddress)
        Me.GroupBox1.Controls.Add(Me.lblFromAddress)
        Me.GroupBox1.Controls.Add(Me.lblFrom)
        Me.GroupBox1.Controls.Add(Me.lblToAddress)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 8)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(616, 80)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Email Header"
        '
        'lblSMTPServer
        '
        Me.lblSMTPServer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSMTPServer.Location = New System.Drawing.Point(384, 48)
        Me.lblSMTPServer.Name = "lblSMTPServer"
        Me.lblSMTPServer.Size = New System.Drawing.Size(216, 24)
        Me.lblSMTPServer.TabIndex = 3
        Me.lblSMTPServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMailServer
        '
        Me.lblMailServer.Location = New System.Drawing.Point(288, 48)
        Me.lblMailServer.Name = "lblMailServer"
        Me.lblMailServer.Size = New System.Drawing.Size(80, 24)
        Me.lblMailServer.TabIndex = 2
        Me.lblMailServer.Text = "SMTP Server:"
        Me.lblMailServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSubjectText
        '
        Me.lblSubjectText.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSubjectText.Location = New System.Drawing.Point(56, 48)
        Me.lblSubjectText.Name = "lblSubjectText"
        Me.lblSubjectText.Size = New System.Drawing.Size(224, 16)
        Me.lblSubjectText.TabIndex = 0
        Me.lblSubjectText.Text = "Framework Test Email"
        Me.lblSubjectText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSubject
        '
        Me.lblSubject.Location = New System.Drawing.Point(8, 48)
        Me.lblSubject.Name = "lblSubject"
        Me.lblSubject.Size = New System.Drawing.Size(48, 16)
        Me.lblSubject.TabIndex = 0
        Me.lblSubject.Text = "Subject:"
        '
        'txtToAddress
        '
        Me.txtToAddress.Location = New System.Drawing.Point(56, 16)
        Me.txtToAddress.Name = "txtToAddress"
        Me.txtToAddress.Size = New System.Drawing.Size(240, 20)
        Me.txtToAddress.TabIndex = 1
        Me.txtToAddress.Text = ""
        '
        'lblFromAddress
        '
        Me.lblFromAddress.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFromAddress.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblFromAddress.Location = New System.Drawing.Point(360, 20)
        Me.lblFromAddress.Name = "lblFromAddress"
        Me.lblFromAddress.Size = New System.Drawing.Size(240, 16)
        Me.lblFromAddress.TabIndex = 0
        Me.lblFromAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFrom
        '
        Me.lblFrom.Location = New System.Drawing.Point(312, 20)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(40, 16)
        Me.lblFrom.TabIndex = 0
        Me.lblFrom.Text = "From:"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblToAddress
        '
        Me.lblToAddress.Location = New System.Drawing.Point(8, 20)
        Me.lblToAddress.Name = "lblToAddress"
        Me.lblToAddress.Size = New System.Drawing.Size(40, 16)
        Me.lblToAddress.TabIndex = 0
        Me.lblToAddress.Text = "To :"
        Me.lblToAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'grpMsgBody
        '
        Me.grpMsgBody.Controls.Add(Me.txtMsgBody)
        Me.grpMsgBody.Location = New System.Drawing.Point(8, 96)
        Me.grpMsgBody.Name = "grpMsgBody"
        Me.grpMsgBody.Size = New System.Drawing.Size(616, 168)
        Me.grpMsgBody.TabIndex = 0
        Me.grpMsgBody.TabStop = False
        Me.grpMsgBody.Text = "Message Body"
        '
        'txtMsgBody
        '
        Me.txtMsgBody.Location = New System.Drawing.Point(8, 16)
        Me.txtMsgBody.Multiline = True
        Me.txtMsgBody.Name = "txtMsgBody"
        Me.txtMsgBody.Size = New System.Drawing.Size(600, 144)
        Me.txtMsgBody.TabIndex = 0
        Me.txtMsgBody.Text = "TEST MESSAGE ISSUED BY THE FRAMEWORK EMAIL TEST UTILITY. PLEASE IGNORE."
        '
        'cmdSend
        '
        Me.cmdSend.Location = New System.Drawing.Point(248, 272)
        Me.cmdSend.Name = "cmdSend"
        Me.cmdSend.Size = New System.Drawing.Size(128, 24)
        Me.cmdSend.TabIndex = 2
        Me.cmdSend.Text = "Send Email"
        '
        'cmdCancel
        '
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(8, 272)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(96, 24)
        Me.cmdCancel.TabIndex = 3
        Me.cmdCancel.Text = "Cancel"
        '
        'TestEmail
        '
        Me.AcceptButton = Me.cmdSend
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(632, 302)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdSend)
        Me.Controls.Add(Me.grpMsgBody)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TestEmail"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Framework Email Test Utility"
        Me.GroupBox1.ResumeLayout(False)
        Me.grpMsgBody.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub TestEmail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'fws = LoadXMLSettings() ' SetApplicationProperties()
        Dim prefix As String

        Dim dlg As New dlgFixedSelection()
        dlg.Tag = "accountsSubAccounts.subAccountId"
        If dlg.ShowDialog(Me) = DialogResult.OK Then
            subAccountId = CInt(dlg.Tag)
            properties = FWEmail.SubAccounts.getSubAccountById(subAccountId).SubAccountProperties

            prefix = "Direct Communication : "

            If properties.EmailServerAddress <> "" Then
                lblSMTPServer.Text = prefix & properties.EmailServerAddress
            Else
                lblSMTPServer.Text = prefix & "Unknown Mail Server"
            End If

            lblFromAddress.Text = properties.EmailServerFromAddress
        End If

    End Sub

    Private Sub cmdSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSend.Click
        Try
            ' communicate directly
            Dim fwEmail As New System.Net.Mail.MailMessage(properties.EmailServerFromAddress, txtToAddress.Text, lblSubject.Text, txtMsgBody.Text)
            Dim fwSender As New System.Net.Mail.SmtpClient(properties.EmailServerAddress)

            Try
                fwSender.Send(fwEmail)

            Catch ex As Exception
                MsgBox("Test Email Failed." & vbNewLine & "Reason: " & ex.Message, MsgBoxStyle.Critical, "Framework Email Test")

            End Try

            fwEmail.Dispose()
            fwEmail = Nothing
            fwSender = Nothing
 
        Catch ex As Exception
            MsgBox("Test Email Error" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "FWUtility Test Email")
        End Try

        Me.DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Close()
    End Sub
End Class
