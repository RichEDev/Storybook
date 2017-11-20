Imports System.IO
Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Partial Class Backup
    Inherits System.Web.UI.Page

    Protected Sub cmdBackup_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim curUser As CurrentUser = cMisc.GetCurrentUser()
		Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId)

        ' get a list of all 'live' attachments for copying
        Dim sql As String
        sql = "SELECT * FROM [attachments] WHERE [Attachment Type] <> @attType"

		Dim db As New cFWDBConnection
		db.DBOpen(fws, False)
        db.AddDBParam("attType", AttachmentType.Hyperlink, True)
        db.RunSQL(sql, db.glDBWorkA, False, "", False)

        Dim drow As DataRow
        Dim success As Integer = 0
        Dim failed As Integer = 0
        Dim dirName As String = "att_backup_" & DateTime.Now.ToShortDateString.Replace("/", "_")

        If System.IO.Directory.Exists(Server.MapPath(dirName)) = False Then
            System.IO.Directory.CreateDirectory(Server.MapPath(dirName))
        End If

        For Each drow In db.glDBWorkA.Tables(0).Rows
            ' copy file to the live
            lblcopystatus.text = "Copy in progress"

            Try
                'If drow.Item("Directory").Substring(0, 1) = "/" Then
                'System.IO.File.Copy(Server.MapPath(Path.Combine(drow.Item("Directory"), drow.Item("Filename"))), Server.MapPath(Path.Combine(dirName & "/", drow.Item("Filename"))))
                If System.IO.File.Exists(Server.MapPath(Path.Combine(dirName & "/", drow.Item("Filename")))) = False Then
                    Dim filearray() As Byte
                    filearray = System.IO.File.ReadAllBytes(Server.MapPath(Path.Combine(drow.Item("Directory"), drow.Item("Filename"))))
                    System.IO.File.WriteAllBytes(Server.MapPath(Path.Combine(dirName & "/", drow.Item("Filename"))), filearray)
                    'Else
                    'System.IO.File.Copy(Path.Combine(drow.Item("Directory"), drow.Item("Filename")), Server.MapPath(Path.Combine(dirName & "/", drow.Item("Filename"))))
                    'End If
                    success += 1
                End If

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("Error Copying file : " & ex.Message)
                failed += 1
            End Try
        Next

        lblCopyStatus.Text = success.ToString & " successful, " & failed.ToString & " failed"

        db.DBClose()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Title = "File Attachment Backup"
        Master.title = Title

        lblCopyStatus.Text = "Ready to copy"
    End Sub
End Class
