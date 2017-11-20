Public Class PasswordEntry
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
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblHeader As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.cmdOK = New System.Windows.Forms.Button
        Me.lblPassword = New System.Windows.Forms.Label
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.lblHeader = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(192, 32)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.TabIndex = 2
        Me.cmdOK.Text = "OK"
        '
        'lblPassword
        '
        Me.lblPassword.Location = New System.Drawing.Point(8, 32)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(72, 23)
        Me.lblPassword.TabIndex = 0
        Me.lblPassword.Text = "Password :"
        Me.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(80, 32)
        Me.txtPassword.MaxLength = 10
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.TabIndex = 1
        Me.txtPassword.Text = ""
        '
        'lblHeader
        '
        Me.lblHeader.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader.Location = New System.Drawing.Point(8, 0)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(256, 23)
        Me.lblHeader.TabIndex = 0
        Me.lblHeader.Text = "Password required to authenticate action"
        Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PasswordEntry
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(272, 62)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.txtPassword)
        Me.Controls.Add(Me.lblPassword)
        Me.Controls.Add(Me.cmdOK)
        Me.Name = "PasswordEntry"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Authenticate Action"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        Me.Tag = Crypt(txtPassword.Text, 1)
        If Mid(Me.Tag, 1, 1) = "$" Then
            MsgBox(Mid(Me.Tag, 2))
        Else
            Close()
        End If
    End Sub

    Private Function Crypt(ByVal pw As Object, ByVal p As Object) As String
        Dim x As Integer
        Dim retVal As String = ""

        ' Encrypt or decrypt passwords
        ' Errors returned prefixed with '$'

        Try
            Dim Q%, CH$, op, EC, n
            n = Len(pw)
            If n < 6 Or n > 10 Then
                retVal = "$" & "Password must be 6 to 10 characters"
                Return retVal
                Exit Function
            End If

            For x = 1 To n
                If Asc(Mid(pw, n, 1)) > 122 Or Asc(Mid(pw, n, 1)) < 48 Then
                    retVal = "$" & "Password contains an invalid character"
                    Return retVal
                    Exit Function
                End If
            Next x
            Select Case CType(p, Integer)
                Case 1  ' Encrypt
                    op = ""
                    For Q% = 1 To Len(pw)
                        CH$ = Mid(pw, Q%, 1)
                        EC = Asc(CH$) + n
                        If EC > 122 Then EC = EC - 75
                        op = op & Chr(EC)
                        n = n + 1
                    Next Q%
                    retVal = op
                Case 2  ' Decrypt
                    op = ""
                    For Q% = 1 To Len(pw)
                        CH$ = Mid(pw, Q%, 1)
                        EC = Asc(CH$) - n
                        If EC < 48 Then EC = EC + 75
                        op = op & Chr(EC)
                        n = n + 1
                    Next Q%
                    retVal = op
                Case Else
                    retVal = "$Unknown param issued to Crypt() function"
            End Select

        Catch ex As Exception

        End Try
        Return retVal
    End Function

    Private Sub PasswordEntry_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtPassword.Text = ""
        txtPassword.Focus()
    End Sub
End Class
