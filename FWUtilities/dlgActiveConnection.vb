Imports System.Windows.Forms
Imports System.Collections
Imports System.Collections.Generic
Imports Spend_Management
Imports SpendManagementLibrary

Public Class dlgActiveConnection

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Dim acc As cAccount = cboAccounts.SelectedItem
        If acc Is Nothing Then
            Me.Tag = -1
        Else
            Me.Tag = acc.accountid
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Tag = -1
        Me.Close()
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        Dim idx As Integer
        Dim acc As cAccount

        For Each kvp As KeyValuePair(Of Integer, cAccount) In FWEmail.Accounts.Accounts
            acc = CType(kvp.Value, cAccount)
            If acc.archived = False Then
                idx = cboAccounts.Items.Add(acc)
            End If
        Next
        cboAccounts.ValueMember = "companyid"
        cboAccounts.DisplayMember = "companyname"
    End Sub
End Class
