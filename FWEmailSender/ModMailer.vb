Imports System.IO
Imports System.Net
Imports FWBase
Imports SpendManagementLibrary
Imports Spend_Management
Imports System.Configuration
Imports BusinessLogic.Modules

Module ModMailer
    Private CurrentRunLocationId As Integer


    Public Enum csvDB_Fields
        RowStatus = 0
        FriendlyName = 1
        TableName = 2
        FieldName = 3
        JoinsReq = 4
        FieldAlias = 5
    End Enum

    Public Property RunLocationId() As Integer
        Get
            Return CurrentRunLocationId
        End Get
        Set(ByVal value As Integer)
            CurrentRunLocationId = value
        End Set
    End Property

    Public Sub DespatchEmail(ByVal SendTo As String, ByVal msgBody As String, ByVal msgSubject As String)
        Dim accs As New cAccounts
        Dim subaccs As New cAccountSubAccounts(currentAccountId)
        Dim properties As cAccountProperties = subaccs.getSubAccountById(RunLocationId).SubAccountProperties

        AddToLog("Emailing '" & SendTo & "'")
        Dim Msg As New Mail.MailMessage(properties.EmailServerFromAddress, SendTo, msgSubject.Replace(vbNewLine, ""), msgBody & vbNewLine & vbNewLine & EmailSig)
        'Dim ds As New DataSet
        'DB.RunSQL("SELECT TOP 1 [Staff Name] FROM [Staff Details] WHERE [Staff Details].[Email Address] = 'joe.hegarty@software-europe.co.uk'", ds)
        'msgBody = Replace(msgBody, "[*Staff To Notify*]", ds.Tables(0).Rows(0).Item("Staff Name"))
        'Msg.Priority = Mail.MailPriority.High

        Dim msender As New Mail.SmtpClient(properties.EmailServerAddress)

        msender.Send(Msg)
        AddToLog("Send request completed.")

        Msg.Dispose()
        msender = Nothing
        Msg = Nothing
    End Sub

    Private Function LoadMailTemplate(ByVal tplId As Integer, ByVal tplName As String) As String
        Try
            AddToLog("Loading Mail Template '" & tplName & "'")
            Dim tPathPrefix As String = ConfigurationManager.AppSettings("templatePathPrefix")
            Dim y As New FWCommon.EmailTemplates
            Dim res As System.Collections.Specialized.NameValueCollection = y.ReadTemplate(System.IO.Path.Combine(tPathPrefix, tplName))

            Dim templateType As Integer = Integer.Parse(res("templateType"))
            Dim templateTitle As String = res("templateTitle")
            Dim templateBody As String = res("templateBody")

            'Dim tempDat As New System.Text.StringBuilder
            'Dim fileReader As New System.IO.FileStream(tplName, FileMode.Open)
            'Dim Reader As New StreamReader(fileReader, System.Text.Encoding.UTF7)

            'tempDat.Append(Reader.ReadToEnd())

            'fileReader.Close()
            'Reader.Close()

            AddToLog("Template loaded.")

            Return templateBody

        Catch ex As Exception
            AddToLog("Template " & tplName & " failed to load")
            AddToLog(ex.Message)

            Return ""
        End Try
    End Function

    Private Function GetFieldsFromTemplate(ByVal TmpCont As String) As String()
        AddToLog("Parsing Template (Fields)...")
        Dim StopStack As Boolean
        Dim intStart As Integer
        Dim strOut(0) As String
        Try
            Do Until StopStack = True
                intStart = InStr(TmpCont, "[*")
                If intStart Then
                    TmpCont = Mid(TmpCont, intStart)
                    intStart = InStr(TmpCont, "*]")

                    strOut(strOut.Length - 1) = Mid(TmpCont, 1, intStart + 1)

                    ReDim Preserve strOut(strOut.Length)

                    TmpCont = Mid(TmpCont, intStart + 2)
                Else
                    StopStack = True
                End If
            Loop
            AddToLog("Template Parsed, " & strOut.Length & " fields.")

        Catch ex As Exception
            AddToLog("FAIL - Failed to get fields from templates. " & ex.Message)
            strOut(0) = ""
        End Try

        Return strOut
    End Function

    Private Function GetConditionsFromTemplate(ByVal TmpCont As String) As String()
        AddToLog("Parsing Template (Conditions)...")
        Dim StopStack As Boolean
        Dim intStart As Integer
        Dim strOut(0) As String
        Do Until StopStack = True
            intStart = InStr(TmpCont, "[-")
            If intStart Then
                TmpCont = Mid(TmpCont, intStart)
                intStart = InStr(TmpCont, "-]")

                strOut(strOut.Length - 1) = Mid(TmpCont, 1, intStart + 1)

                ReDim Preserve strOut(strOut.Length)

                TmpCont = Mid(TmpCont, intStart + 2)
            Else
                StopStack = True
            End If
        Loop
        AddToLog("Template Parsed, " & strOut.Length & " conditions.")
        Return strOut

    End Function

    Public Sub OverdueInvoiceMail(ByVal CDetails As DataRow, ByVal Recplist As DataSet, ByVal template As String)
        Dim Fields() As String
        Dim tpl As String
        Dim ds As New DataSet
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))

        Dim brandName As String = "Framework"

        AddToLog("Handling overdue invoice.")

        tpl = LoadMailTemplate(emailType.OverdueInvoice, template)

        Fields = GetFieldsFromTemplate(tpl)

        Dim sql As String
        sql = ModBuildSQL.ConstructQuery(Fields, "contract_forecastdetails", "ContractForecastId", CDetails.Item("contractForecastId"), "[contract_details].[contractId],")

        ds = db.GetDataSet(sql)

        tpl = PopulateFields(tpl, Fields, ds.Tables(0).Rows(0))
        If tpl <> "" Then
            For Each dRow As DataRow In Recplist.Tables(0).Rows
                DespatchEmail(dRow.Item("Email"), tpl, brandName & ": Invoice Overdue")
            Next
        End If
    End Sub

    Public Sub LicenceExpiryMail(ByVal PDetails As DataRow, ByVal template As String)
        Dim Fields() As String
        Dim tpl As String
        Dim ds As New DataSet
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))
        Dim emps As New cEmployees(currentAccountId)
        Dim teams As New cTeams(currentAccountId, RunLocationId)

        Dim brandName As String = "Framework"

        AddToLog("Handling Licence Expiry.")

        tpl = LoadMailTemplate(emailType.LicenceExpiry, template.Replace("/", "\"))
        If tpl <> "" Then
            Fields = GetFieldsFromTemplate(tpl)

            Dim sql As String
            sql = ModBuildSQL.ConstructQuery(Fields, "productLicences", "LicenceId", PDetails.Item("LicenceId"), "[productDetails].[subAccountId],productLicences.[NotifyType] AS [NotifyType],productLicences.[NotifyId] AS [Recipient], ")
            'AddToLog("SQL Returned = " & sql.Trim)

            ds = db.GetDataSet(sql)

            tpl = PopulateFields(tpl, Fields, ds.Tables(0).Rows(0))
            Dim emailAddress As String = ""
            Select Case CType(ds.Tables(0).Rows(0).Item("NotifyType"), AudienceType)
                Case AudienceType.Individual
                    If Not emps.GetEmployeeById(ds.Tables(0).Rows(0).Item("Recipient")) Is Nothing Then
                        emailAddress = emps.GetEmployeeById(ds.Tables(0).Rows(0).Item("Recipient")).EmailAddress
                    End If

                Case AudienceType.Team
                    Dim team As cTeam = teams.GetTeamById(ds.Tables(0).Rows(0).Item("Recipient"))
                    Dim separator As String = ""
                    For Each empId As Integer In team.teammembers
                        emailAddress = separator & emps.GetEmployeeById(empId).EmailAddress
                        separator = ";"
                    Next
            End Select

            If emailAddress.Trim <> "" Then
                DespatchEmail(emailAddress, tpl, brandName & ": Licence Expiry")
            End If
        End If
    End Sub

    Public Sub ContractReviewMail(ByVal CDetails As DataRow, ByVal Recplist As DataSet, ByVal template As String)
        Dim RcpList As String
        Dim rRow As DataRow
        Dim tpl As String
        Dim ToGo As String
        Dim Fields() As String
        Dim Cid As Integer
        Dim strNYM As String
        Dim mParams As New MaintParams
        Dim sql As String
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))
        Dim accounts As New cAccounts()
        Dim account As cAccount = accounts.GetAccountByID(currentAccountId)
        Dim hostname As String = HostManager.GetHostName(account.HostnameIds, Modules.contracts, account.companyid)
        Dim subaccs As New cAccountSubAccounts(currentAccountId)
        Dim properties As cAccountProperties = subaccs.getSubAccountById(RunLocationId).SubAccountProperties
        Dim brandName As String = "Framework"

        tpl = LoadMailTemplate(emailType.ContractReview, template)

        If tpl <> "" Then
            Fields = GetFieldsFromTemplate(tpl)

            Cid = CDetails("ContractId")

            mParams = SMRoutines.GetMaintParams(db, Cid)
            strNYM = GetProducts(mParams, Cid)

            sql = ModBuildSQL.ConstructQuery(Fields, "contract_details", "ContractId", Cid.ToString())

            Dim ds As New DataSet

            ds = db.GetDataSet(sql)

            ToGo = PopulateFields(tpl, Fields, ds.Tables(0).Rows(0), strNYM)

            Dim curEmailList As New System.Collections.ArrayList

            If Recplist.Tables(0).Rows.Count = 0 Then
                ' no staff specified for contract notification
                nNumBlankNotifies += 1
                AddToLog("No recipients specified for notification. Informing Email administrator [" & properties.EmailAdministrator & "]")
                Try
                    Dim errMailMsg As New Mail.MailMessage
                    '(FWS.glMailFrom, FWS.glWebEmailerAdmin)
                    With errMailMsg
                        .From = New System.Net.Mail.MailAddress(properties.EmailServerFromAddress)
                        Dim recipients() As String = Split(properties.EmailAdministrator.Replace(",", ";"), ";")
                        Dim recpLoop As Integer
                        For recpLoop = 0 To recipients.Length - 1
                            AddToLog("Setting recipient [" & recipients(recpLoop) & "]")
                            .To.Add(recipients(recpLoop))
                        Next
                        .Subject = brandName & " Contract Notification Failure"
                    End With

                    Dim msg As New System.Text.StringBuilder
                    With msg
                        .Append("An error has occurred trying to send automatic email notifications from " & brandName & vbNewLine & vbNewLine)
                        .Append("The contract detailed below does not have any employees nominated for notification" & vbNewLine & vbNewLine)
                        .Append("Contract Key         : " & CDetails("contractKey") & vbNewLine)
                        .Append("Contract Description : " & CDetails("contractDescription") & vbNewLine)
                        .Append("Supplier             : " & CDetails("supplierName") & vbNewLine & vbNewLine)
                        .Append("Click the link below to access the contract" & vbNewLine & vbNewLine)
                        .Append("http://" & hostname & "/ContractSummary.aspx?tab=0&loc=" & RunLocationId.ToString.Trim & "&id=" & Cid.ToString)
                        .Append(vbNewLine & vbNewLine)
                        .Append("** NOTE ** This is an automated email. Do not reply.")
                    End With
                    errMailMsg.Body = msg.ToString

                    Dim sender As New Mail.SmtpClient(properties.EmailServerAddress)
                    sender.Send(errMailMsg)

                    sender = Nothing

                Catch ex As Exception
                    AddToLog("Error report to Email Administrator failed to send")
                    AddToLog(ex.Message)
                End Try
            End If

            For Each rRow In Recplist.Tables(0).Rows
                ' if email address already in the curEmailList, then don't bother to send email > once! Probably named in >1 team
                ' or as an individual and a team member
                Dim isValidEmail As Boolean = True

                If IsDBNull(rRow.Item("Email")) Then
                    isValidEmail = False
                Else
                    If rRow.Item("Email") = "" Then
                        isValidEmail = False
                    End If
                End If

                If Not isValidEmail Then
                    ' cannot issue an email as the recipient's email address is invalid.
                    ' Notify the email admin
                    AddToLog("An email recipient specified for notification does not have an email address specified. Informing Email administrator [" & properties.EmailAdministrator & "]")

                    If slInvalidRecipients.ContainsKey(rRow.Item("employeeId")) = False Then
                        ' store that message has been sent to ensure multiple messages for the same employee are not sent
                        slInvalidRecipients.Add(rRow.Item("employeeId"), rRow.Item("memberName"))

                        Try
                            Dim errMailMsg As New Mail.MailMessage
                            With errMailMsg
                                .From = New System.Net.Mail.MailAddress(properties.EmailServerFromAddress)
                                Dim recipients() As String = Split(properties.EmailAdministrator.Replace(",", ";"), ";")
                                Dim recpLoop As Integer
                                For recpLoop = 0 To recipients.Length - 1
                                    AddToLog("Setting recipient [" & recipients(recpLoop) & "]")
                                    .To.Add(recipients(recpLoop))
                                Next
                                .Subject = brandName & " Contract Notification Failure"
                            End With

                            Dim msg As New System.Text.StringBuilder
                            With msg
                                .Append("An error has occurred trying to send automatic email notifications from " & brandName & vbNewLine & vbNewLine)
                                .Append("The employee named below was nominated to receive an email notification but does not have an email address associated with their details." & vbNewLine & vbNewLine)
                                .Append("Employee : " & rRow.Item("memberName") & vbNewLine)
                                .Append(vbNewLine & vbNewLine)
                                .Append("Click the link below to access the employee record" & vbNewLine & vbNewLine)
                                .Append("http://" & hostname & "/shared/admin/aeemployee.aspx?employeeid=" & CStr(rRow.Item("EmployeeId")).Trim)
                                .Append(vbNewLine & vbNewLine)
                                .Append("** NOTE ** This is an automated email. Do not reply.")
                            End With
                            errMailMsg.Body = msg.ToString

                            Dim sender As New Mail.SmtpClient(properties.EmailServerAddress)
                            sender.Send(errMailMsg)

                            sender = Nothing

                        Catch ex As Exception
                            AddToLog("Error report to Email Administrator failed to send")
                            AddToLog(ex.Message)
                        End Try
                    Else
                        AddToLog("Administrator already been informed by email. Skipping duplicate notification")
                    End If
                Else
                    If curEmailList.Contains(rRow.Item("Email")) = False Then
                        RcpList = rRow.Item("Email")

                        curEmailList.Add(rRow.Item("Email"))

                        Console.WriteLine("Emailing " & RcpList)
                        AddToLog("Emailing " & RcpList)

                        Try
                            DespatchEmail(RcpList, ToGo, "Contract Up For Review: " & CDetails("contractDescription"))
                        Catch ex As Exception
                            Console.WriteLine("Emailing failed.")
                            AddToLog("Emailing failed. " & ex.Message)
                        End Try
                    Else
                        Console.WriteLine("Email duplicated for current contract : " & rRow.Item("Email") & " - skipped")
                        AddToLog("Email duplicated for current contract : " & rRow.Item("Email") & " - skipped")
                    End If
                End If
            Next
        End If
    End Sub

    Public Function PopulateFields(ByVal Tpl As String, ByVal Fields() As String, ByRef resRow As DataRow, Optional ByVal NYM As String = "") As String
        AddToLog("Populating fields with database values.")
        Dim X As Integer
        Dim csvIN As New csvParser.cCSV
        'Dim csv As New csvParser.cCSVParse("database.csv")
        Dim ds As DataSet
        Dim subaccs As New cAccountSubAccounts(currentAccountId)
        Dim properties As cAccountProperties = subaccs.getSubAccountById(RunLocationId).SubAccountProperties
        Dim isNull As Boolean = False
        Dim dataType As String
        Dim accounts As New cAccounts()
        Dim account As cAccount = accounts.GetAccountByID(currentAccountId)
        Dim hostname As String = HostManager.GetHostName(account.HostnameIds, Modules.contracts, account.companyid)

        ds = csvIN.CSVToDataset("database.csv")

        Tpl = ParseConditions(Tpl, resRow, ds)

        For X = 0 To Fields.Length - 1
            For Each dRow As DataRow In ds.Tables(0).Rows
                If Fields(X) = "[*" & dRow.Item(csvDB_Fields.FriendlyName) & "*]" Then
                    AddToLog("Database field relation found, replacing template value.")

                    If dRow.Item(csvDB_Fields.TableName) = "dbo" Then
                        isNull = IsDBNull(resRow.Item(dRow.Item(csvDB_Fields.FieldAlias)))
                        dataType = resRow.Item(dRow.Item(csvDB_Fields.FieldAlias)).GetType.ToString
                    Else
                        isNull = IsDBNull(resRow.Item(dRow.Item(csvDB_Fields.FieldName)))
                        dataType = resRow.Item(dRow.Item(csvDB_Fields.FieldName)).GetType.ToString
                    End If

                    If isNull = False Then
                        If dataType = "System.DateTime" Then
                            Tpl = Replace(Tpl, "[*" & dRow.Item(csvDB_Fields.FriendlyName) & "*]", Format(CDate(resRow.Item(dRow.Item(csvDB_Fields.FieldName))), cDef.DATE_FORMAT))
                        Else
                            If dRow.Item(csvDB_Fields.TableName) = "dbo" Then
                                Tpl = Replace(Tpl, "[*" & dRow.Item(csvDB_Fields.FriendlyName) & "*]", resRow.Item(dRow.Item(csvDB_Fields.FieldAlias)))
                            Else
                                Tpl = Replace(Tpl, "[*" & dRow.Item(csvDB_Fields.FriendlyName) & "*]", resRow.Item(dRow.Item(csvDB_Fields.FieldName)))
                            End If
                        End If
                    Else
                        Tpl = Replace(Tpl, "[*" & dRow.Item(csvDB_Fields.FriendlyName) & "*]", "")
                    End If
                    AddToLog("Field added to email.")

                ElseIf Fields(X) = "[*Contract Link*]" Then
                    If Microsoft.VisualBasic.Right(hostname, 1) = "/" Then
                        Tpl = Replace(Tpl, Fields(X), "http://" & hostname & "ContractSummary.aspx?tab=0&loc=" & RunLocationId.ToString.Trim & "&id=" & resRow.Item("contractId"))
                    Else
                        Tpl = Replace(Tpl, Fields(X), "http://" & hostname & "/ContractSummary.aspx?tab=0&loc=" & RunLocationId.ToString.Trim & "&id=" & resRow.Item("contractId"))
                    End If

                ElseIf Fields(X) = "[*NYM Info*]" Then
                    Tpl = Replace(Tpl, Fields(X), NYM)

                End If
            Next
        Next
        Return Tpl
    End Function

    Private Function ParseConditions(ByVal Tpl As String, ByVal resRow As DataRow, ByVal ds As DataSet) As String
        Dim EntireTrue, EntireFalse, Entire, Out, Conditions() As String
        Dim IsBlank As Boolean = False
        Conditions = GetConditionsFromTemplate(Tpl)

        For i As Integer = 0 To Conditions.Length - 1
            For Each dRow As DataRow In ds.Tables(0).Rows
                IsBlank = False
                If Conditions(i) = "[-" & dRow.Item(csvDB_Fields.FriendlyName) & "-]" Then
                    EntireTrue = ComputeEntireTrueCondition(Tpl, Conditions(i))
                    EntireFalse = ComputeEntireFalseCondition(Tpl, Conditions(i))
                    Entire = ComputeEntireCondition(Tpl, Conditions(i))

                    If IsDBNull(resRow.Item(dRow.Item(csvDB_Fields.FieldName))) = True Then
                        IsBlank = True
                    End If

                    If IsBlank = False Then
                        If resRow.Item(dRow.Item(csvDB_Fields.FieldName)) = "" Then
                            IsBlank = True
                        End If
                    End If

                    If IsBlank = False Then
                        Out = Replace(EntireTrue, "[-" & dRow.Item(csvDB_Fields.FriendlyName) & "-]{", "")
                        Out = Replace(Out, "}", "")
                        Out = Replace(Out, vbNewLine & vbNewLine, vbNewLine)
                        Tpl = Replace(Tpl, Entire, Out)
                    Else
                        Out = Replace(EntireFalse, "}else{", "")
                        Out = Replace(Out, "}", "")
                        Out = Replace(Out, vbNewLine & vbNewLine, vbNewLine)
                        Tpl = Replace(Tpl, Entire, Out)
                    End If
                End If
            Next
        Next

        Return Tpl
    End Function

    Private Function ComputeEntireTrueCondition(ByVal Tpl As String, ByVal Condition As String) As String
        Dim intStart As Integer
        Dim intEnd As Integer

        intStart = Tpl.IndexOf(Condition & "{")
        If intStart > -1 Then
            intEnd = Tpl.IndexOf("}", intStart)
            Tpl = Tpl.Substring(intStart, intEnd - intStart + 1)
            Return Tpl
        End If
        Return ""
    End Function

    Private Function ComputeEntireFalseCondition(ByVal Tpl As String, ByVal Condition As String) As String
        Dim intStart As Integer
        Dim intEnd As Integer
        Dim intVeryEnd As Integer
        intStart = Tpl.IndexOf(Condition & "{")
        If intStart > -1 Then
            intEnd = Tpl.IndexOf("}else{", intStart + 1)
            If intEnd > -1 Then
                intVeryEnd = Tpl.IndexOf("}", intEnd + 1)
                Tpl = Tpl.Substring(intEnd, intVeryEnd - intEnd + 1)
                Return Tpl
            End If
        End If
        Return ""
    End Function

    Private Function ComputeEntireCondition(ByVal Tpl As String, ByVal Condition As String) As String
        Dim intStart As Integer
        Dim intEnd As Integer
        Dim intVeryEnd As Integer
        intStart = Tpl.IndexOf(Condition & "{")
        If intStart > -1 Then
            intEnd = Tpl.IndexOf("}else{", intStart + 1)
            If intEnd > -1 Then
                intVeryEnd = Tpl.IndexOf("}", intEnd + 1)
                Tpl = Tpl.Substring(intStart, intVeryEnd - intStart + 1)
                Return Tpl
            Else
                intVeryEnd = Tpl.IndexOf("}", intStart + 1)
                Tpl = Tpl.Substring(intStart, intVeryEnd - intStart + 1)
                Return Tpl
            End If
        End If
        Return ""
    End Function

    Public Function GetProducts(ByVal mParams As MaintParams, ByVal ContractID As Long) As String
        Dim sql As New System.Text.StringBuilder
        Dim dRow As DataRow
        Dim strOut As New System.Text.StringBuilder
        Dim NYmRes As NYMResult
        Dim db As New DBConnection(cAccounts.getConnectionString(currentAccountId))
        Dim ds As DataSet

        sql.Append("SELECT [productDetails].[ProductName],[MaintenanceValue],[ProductValue],[MaintenancePercent] FROM [contract_productdetails] ")
        sql.Append("INNER JOIN [productDetails] ON [productDetails].[ProductId] = [contract_productdetails].[ProductId] ")
        sql.Append("WHERE [ContractId] = @conId")
        db.sqlexecute.Parameters.AddWithValue("@conId", ContractID)
        ds = db.GetDataSet(sql.ToString)

        If ds.Tables(0).Rows.Count > 0 Then
            For Each dRow In ds.Tables(0).Rows
                mParams.CurMaintVal = dRow.Item("MaintenanceValue")

                mParams.ListPrice = dRow.Item("ProductValue")

                mParams.PctOfLP = dRow.Item("MaintenancePercent")

                NYmRes = SMRoutines.CalcNYM(mParams, 0)

                strOut.Append(dRow.Item("ProductName") & " - Current annual cost (£" & dRow.Item("MaintenanceValue") & ") increase is capped not to exceed £" & NYmRes.NYMValue & " next year." & vbNewLine)
            Next
            Return "Product Annual Cost: " & vbNewLine & strOut.ToString
        Else
            Return ""
        End If
    End Function

End Module
