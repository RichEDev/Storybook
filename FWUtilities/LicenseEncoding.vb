Imports FWBase
Imports Spend_Management
Imports SpendManagementLibrary

Public Class dlgLicenseEncoding
    Private crypt As New FWCrypt(FWCrypt.Providers.RC2)
    Private encrypt As New cSecureData()

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub dlgLicenseEncoding_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
        If rdoEncrypt.Checked Then
            If txtRawString.Text.Trim <> "" Then
                txtCryptString.Text = encrypt.Encrypt(txtRawString.Text.Trim) 'crypt.Encrypt(txtRawString.Text.Trim, EncryptionKey)
            Else
                MsgBox("Value to encrypt cannot be blank", MsgBoxStyle.Information, "License Encoding")
            End If
        Else
            If txtCryptString.Text.Trim <> "" Then
                txtRawString.Text = encrypt.Decrypt(txtCryptString.Text.Trim) ' crypt.Decrypt(txtCryptString.Text.Trim, EncryptionKey)
            Else
                MsgBox("Value to decrypt cannot be blank", MsgBoxStyle.Information, "License Encoding")
            End If
        End If
    End Sub
End Class