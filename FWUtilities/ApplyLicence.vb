Imports FWBase
Imports SpendManagementLibrary
Imports Spend_Management

Public Class ApplyLicence
    Private crypt As New FWCrypt(FWCrypt.Providers.RC2)
    Private encrypt As New cSecureData()

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Close()
    End Sub

    Private Sub cmdApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdApply.Click
        Dim db As New DBConnection(FWEmail.ConnectionString)
        Dim sql As String = "update registeredusers set licencedUsers = @Licensed_Users where accountId = @accountId"

        Try
            db.sqlexecute.Parameters.AddWithValue("@Licensed_Users", txtLicence.Text.Trim)
            db.sqlexecute.Parameters.AddWithValue("@accountId", FWEmail.AccountId)
            db.ExecuteSQL(sql)

            MsgBox("Licence Applied successfully.", MsgBoxStyle.Information, "Licence Application")
            Close()

        Catch ex As Exception
            MsgBox("An error occurred trying to apply the licence code" & vbNewLine & ex.Message, MsgBoxStyle.Critical, "Licence Error")
        End Try
    End Sub

    Private Sub ApplyLicence_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub cmdDecode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDecode.Click
        Dim decodeStr As String = ""

        Try
            If txtLicence.Text.Trim <> "" Then
                decodeStr = encrypt.Decrypt(txtLicence.Text.Trim) 'crypt.Decrypt(txtLicence.Text.Trim, EncryptionKey)
                If IsNumeric(decodeStr) Then
                    lblLicenceSize.Text = decodeStr & " User Licence detected"
                    cmdApply.Enabled = True
                Else
                    MsgBox("Invalid Licence code submitted.", MsgBoxStyle.Critical, "Licence Error")
                    txtLicence.Text = ""
                    cmdApply.Enabled = False
                End If
            End If

        Catch ex As Exception
            MsgBox("Invalid Licence code submitted.", MsgBoxStyle.Critical, "Licence Error")
        End Try
    End Sub
End Class