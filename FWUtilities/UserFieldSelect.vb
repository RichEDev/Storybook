Imports FWBase
Imports System.Collections
Imports System.Collections.Generic
Imports Spend_Management
Imports SpendManagementLibrary

Public Class UserFieldSelect
    Private ufields As Dictionary(Of String, Integer)

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        Me.Tag = ufields(lstFields.SelectedItem).ToString
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub UserFieldSelect_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ufields = New Dictionary(Of String, Integer)

        Dim ufs As New cUserdefinedFields(FWEmail.AccountId)

        For Each kvp As KeyValuePair(Of Integer, cUserDefinedField) In ufs.UserdefinedFields
            Dim uf As cUserDefinedField = kvp.Value

            If uf.fieldtype = FieldType.List Then
                lstFields.Items.Add(uf.description)
                ufields.Add(uf.attribute.attributename, uf.attribute.attributeid)
            End If
        Next

        lstFields.Items.Insert(0, "Please select...")
        ufields.Add("Please select...", 0)

        lstFields.SelectedIndex = 0
    End Sub
End Class