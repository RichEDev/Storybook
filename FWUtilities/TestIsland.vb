Public Class TestIsland
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
    Friend WithEvents cmdTestFunc As System.Windows.Forms.Button

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Button1 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.cmdTestFunc = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(104, 24)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(128, 24)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Dump Serialized File"
        '
        'cmdTestFunc
        '
        Me.cmdTestFunc.Location = New System.Drawing.Point(104, 70)
        Me.cmdTestFunc.Name = "cmdTestFunc"
        Me.cmdTestFunc.Size = New System.Drawing.Size(128, 23)
        Me.cmdTestFunc.TabIndex = 1
        Me.cmdTestFunc.Text = "Test Function"
        Me.cmdTestFunc.UseVisualStyleBackColor = True
        '
        'TestIsland
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(376, 374)
        Me.Controls.Add(Me.cmdTestFunc)
        Me.Controls.Add(Me.Button1)
        Me.Name = "TestIsland"
        Me.Text = "TestIsland"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim x As New cImportMaps.ImportMapping
        x.IgnoreHeader = True
        Dim y As New cImportMaps
        y.SaveImport("C:\test.dat", x)
    End Sub

    Private Sub cmdTestFunc_Click(sender As System.Object, e As System.EventArgs) Handles cmdTestFunc.Click
        Dim csv As New csvParser.cCSV
        csv.hasHeaderRow = True

        Dim dset As DataSet = csv.CSVToDataset("c:\WeeklyImport.csv")

    End Sub
End Class
