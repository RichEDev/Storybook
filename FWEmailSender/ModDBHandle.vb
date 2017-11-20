Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Data.SqlClient

Module ModDBHandle
    Public slInvalidRecipients As System.Collections.SortedList
    Public nNumBlankNotifies As Integer = 0

    

    Public Function HandleDBStuff() As Boolean
        Try
            Console.WriteLine("Database connection is active.")
            AddToLog("Database connection is active.")
            GetScheds()

            Return True

        Catch ex As Exception
            Console.WriteLine("No database connection.")
            AddToLog("FAIL - No database connection.")
            Return False
        End Try
    End Function

    Private Sub GetScheds()
        Dim rDset As New DataSet
        Dim dRow As DataRow
        Dim numScheds As Integer = 0

        Try
            Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))
            rDset = db.GetDataSet("SELECT * FROM [email_schedule] WHERE [nextRunDate] <= CONVERT(DateTime,GetDate(), 120)")
            numScheds = rDset.Tables(0).Rows.Count
            Console.WriteLine("Got email schedules.")
            AddToLog("Got email schedules.")

        Catch ex As Exception
            Console.WriteLine("Failed to get email schedules.")
            AddToLog("FAIL - Could not got email schedules.")
        End Try

        If numScheds > 0 Then
            For Each dRow In rDset.Tables(0).Rows
                DealWithRow(dRow)
            Next
        Else
            Console.WriteLine("No current scheduled email notifications outstanding")
            AddToLog("No current scheduled email notifications outstanding")
        End If
    End Sub

    Private Sub DealWithRow(ByVal dRow As DataRow)
        Try
            Dim SchedId, EmailType As Integer
            Dim emParam As String
            Dim templateFile As String
            Dim templateId As Integer
            Dim dbLocationId As Integer
            Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))

            SchedId = CType(dRow.Item("scheduleId"), Integer)
            If IsDBNull(dRow.Item("templateId")) Then
                'MsgBox("Cannot execute scheduled notification due to missing Email Template" & vbNewLine & "Schedule being skipped", MsgBoxStyle.Critical, "FWEmailSender error")
                AddToLog("Cannot execute scheduled notification due to missing Email Template. Schedule being skipped")
                Exit Sub
            End If
            templateId = CType(dRow.Item("templateId"), Integer)
            EmailType = CType(dRow.Item("emailType"), Integer)
            If IsDBNull(dRow.Item("emailParam")) Then
                emParam = ""
            Else
                emParam = CType(dRow.Item("emailParam"), String)
            End If
            dbLocationId = CType(dRow.Item("runSubAccountId"), Integer)

            Dim tPath As String = ""
            Dim tFilename As String = ""

            db.sqlexecute.Parameters.AddWithValue("@templateId", templateId)
            Using reader As SqlDataReader = db.GetReader("select templatePath, templateFilename from email_templates where templateId = @templateId")
                While reader.Read
                    tPath = reader.GetString(0)
                    tFilename = reader.GetString(1)
                End While
                reader.Close()

                If tFilename <> "" Then
                    templateFile = System.IO.Path.Combine(tPath, tFilename)
                Else
                    templateFile = ""
                End If
            End Using

            HandleRow(SchedId, EmailType, emParam, templateFile, dbLocationId)

            Dim tmpLRD As String = dRow("nextRunDate")
            Dim tmpCurDateWithRunTime As DateTime = DateTime.Parse(tmpLRD)

            Dim nrd As DateTime = GetNextRunDate(DateTime.Now, tmpCurDateWithRunTime, Integer.Parse(dRow.Item("emailFrequency")), Integer.Parse(dRow.Item("frequencyParam")))
            db.sqlexecute.Parameters.Clear()
            db.sqlexecute.Parameters.AddWithValue("@schedId", SchedId)
            db.ExecuteSQL("UPDATE [email_schedule] SET [nextRunDate] = CONVERT(datetime,'" & nrd.ToString("yyyy-MM-dd hh:mm:ss") & "',120) WHERE [scheduleId] = @schedId")

            AddToLog("Next Scheduled Run of email notification set to " & nrd.ToString("yyyy-MM-dd hh:mm:ss"))

        Catch ex As Exception
            Console.Write("Failed to cast objects into their correct type.")
            AddToLog("Failed to cast objects into their correct type.")
            AddToLog(ex.Message)
        End Try
    End Sub

    Private Sub HandleRow(ByVal SchedId As Integer, ByVal emType As Integer, ByVal emParam As String, ByVal templateFile As String, ByVal dbLocationId As Integer)
        Console.WriteLine("Handling Schedule " & SchedId & " using Template " & templateFile)
        AddToLog("Handling Schedule " & SchedId & " using Template " & templateFile)

        Select Case emType
            Case emailType.AuditCleardown
                'Dim wSFWE As New localhost.FWEmailServer
                'If Microsoft.VisualBasic.Right(FWS.glWebEmailerURL, 1) = "/" Then
                '    wSFWE.Url = FWS.glWebEmailerURL & "FWEmailServer.asmx"
                'Else
                '    wSFWE.Url = FWS.glWebEmailerURL & "/FWEmailServer.asmx"
                'End If
                'Call wSFWE.DoAuditCleardown()

            Case emailType.ContractReview
                If emParam <> "" Then
                    ContractReview(templateFile, Integer.Parse(emParam), dbLocationId)
                Else
                    ContractReview(templateFile, 0, dbLocationId)
                End If

            Case emailType.LicenceExpiry
                LicenceExpiry(templateFile, dbLocationId)

            Case emailType.OverdueInvoice
                InvoiceOverdue(templateFile, dbLocationId)

            Case Else

        End Select
    End Sub

    Private Sub InvoiceOverdue(ByVal template As String, ByVal dbLocationId As Integer)
        Dim ds As New DataSet
        Dim sql As New System.Text.StringBuilder
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))

        sql.Append("SELECT * FROM [contract_forecastdetails] " & vbNewLine)
        sql.Append("INNER JOIN [contract_details] ON [contract_details].[ContractId] = [contract_forecastdetails].[ContractId] " & vbNewLine)
        sql.Append("WHERE [contract_details].[subAccountId] = @locId ")
        sql.Append("AND [contract_details].[Archived] = 'N' ")
        sql.Append("AND [paymentDate] <= getdate() ")

        db.sqlexecute.Parameters.AddWithValue("@locId", dbLocationId)
        ds = db.GetDataSet(sql.ToString)

        For Each dRow As DataRow In ds.Tables(0).Rows
            sql.Remove(0, sql.Length)
            sql.Append(GetRecpListSQL(dbLocationId, dRow.Item("ContractId")))

            Dim dsrcp As New DataSet
            dsrcp = db.GetDataSet(sql.ToString)
            ModMailer.RunLocationId = dbLocationId
            ModMailer.OverdueInvoiceMail(dRow, dsrcp, template)
        Next
    End Sub

    Private Sub LicenceExpiry(ByVal template As String, ByVal dbLocationId As Integer)
        Dim ds As New DataSet
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))

        db.sqlexecute.Parameters.AddWithValue("@locId", dbLocationId)
        Dim sql As New System.Text.StringBuilder
        sql.Append("SELECT productLicences.[LicenceId], [LicenceKey], [LicenceType],[Location],[Expiry],productLicences.[RenewalType], productLicences.[NotifyId], [NotifyType], productLicences.[NotifyDays] FROM [productLicences] ")
        sql.Append("INNER JOIN productDetails ON productDetails.[ProductId] = productLicences.[ProductId] ")
        sql.Append("WHERE productDetails.[subAccountId] = @locId AND [Expiry] IS NOT NULL ")

        ds = db.GetDataSet(sql.ToString)

        For Each dRow As DataRow In ds.Tables(0).Rows
            Dim expiryD As DateTime = DateTime.Parse(dRow.Item("Expiry"))

            If IsDBNull(dRow.Item("notifyDays")) = False Then
                Dim takeOff As Double = Double.Parse("-" & dRow.Item("NotifyDays"))
                expiryD = expiryD.AddDays(takeOff)
            End If

            If expiryD < DateTime.Now Then 'Licence is within review period
                ModMailer.RunLocationId = dbLocationId
                ModMailer.LicenceExpiryMail(dRow, template)
            End If

            'sql = GetRecpListSQL(CType(dRow.Item("Contract Id"), Integer))
            'Dim dsrcp As New DataSet
            'DB.RunSQL(sql, dsrcp)
            'ModMailer.OverdueInvoiceMail(dRow, dsrcp)
        Next
    End Sub

    Private Sub ContractReview(ByVal template As String, ByVal emParam As Integer, ByVal dbLocationId As Integer)
        Dim SQL As String
        Dim dSet As New DataSet
        Dim RcpdSet As New DataSet
        Dim dRow As DataRow
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))

        SQL = CReviewSQL(emParam)
        Try
            Console.WriteLine("Fetching contracts up for review...")
            AddToLog("Fetching contracts up for review...")
            db.sqlexecute.Parameters.AddWithValue("@locId", dbLocationId)
            dSet = db.GetDataSet(SQL)
            Console.WriteLine("Contracts retrieved.")
            AddToLog("Contracts retrieved.")
        Catch ex As Exception
            Console.WriteLine("Could not get contracts up for review.")
            AddToLog("FAIL - Could not get contracts up for review.")
            Exit Sub
        End Try

        nNumBlankNotifies = 0
        If slInvalidRecipients Is Nothing Then
            slInvalidRecipients = New SortedList
        Else
            slInvalidRecipients.Clear()
        End If

        For Each dRow In dSet.Tables(0).Rows
            Try
                Dim Cid As Integer
                Cid = dRow.Item("ContractId")
                If IsDBNull(dRow.Item("ContractKey")) Then
                    dRow.Item("ContractKey") = "No Key Specified"
                End If
                AddToLog("Fetching recipient list for contract " & dRow.Item("ContractKey") & "...")
                RcpdSet = db.GetDataSet(GetRecpListSQL(dbLocationId, Cid))
                AddToLog("Got recipient list for contract " & dRow.Item("ContractKey") & ".")
            Catch ex As Exception
                AddToLog("FAIL - Could not get recipient list. " & ex.ToString())
            End Try

            Try
                ModMailer.RunLocationId = dbLocationId
                ModMailer.ContractReviewMail(dRow, RcpdSet, template)

            Catch ex As Exception
                AddToLog("Failed in email module. " & ex.ToString())
            End Try
        Next

        AddToLog("Number of administration notifications for unspecified email addresses : " & slInvalidRecipients.Count.ToString)
        AddToLog("Number of contracts without Notification specified : " & nNumBlankNotifies.ToString)
    End Sub
End Module