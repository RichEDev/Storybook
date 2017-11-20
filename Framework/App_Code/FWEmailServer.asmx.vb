Imports Microsoft.VisualBasic
Imports System.Web.Services
Imports System.Diagnostics
Imports System.Data
Imports SpendManagementLibrary
Imports FWClasses
Imports FWBase
Imports System

Namespace Framework2006

    <System.Web.Services.WebService(Namespace:="http://tempuri.org/Framework/FWEmailServer")> _
    Public Class FWEmailServer
        Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Web Services Designer.
            InitializeComponent()

            'Add your own initialization code after the InitializeComponent() call

        End Sub

        'Required by the Web Services Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Web Services Designer
        'It can be modified using the Web Services Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            'CODEGEN: This procedure is required by the Web Services Designer
            'Do not modify it using the code editor.
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region

        <WebMethod()> _
        Public Function RequestAuthToken(ByVal UserName As String, ByVal Password As String) As String
            Dim db As New cFWDBConnection
            Dim fws As New cFWSettings
            Dim retVal As String
            Dim isValid As Boolean

            fws = Application("FWSettings")
            db.DBOpen(fws, False)

            db.FWDb("R", "security", "User Name", UserName, "", "", "", "", "", "", "", "", "", "")
            Select Case CType(db.FWDbFindVal("Password Method", 1), PwdMethod)
                Case PwdMethod.FWBasic
                    If Password.Trim = cPassword.Crypt(db.FWDbFindVal("Password", 1), 2) Then
                        isValid = True
                    End If

                Case PwdMethod.Hash
                    If cPassword.HashPassword(Password) = db.FWDbFindVal("Password", 1) Then
                        isValid = True
                    End If

                Case PwdMethod.SHA_Hash
                    If cPassword.SHA_HashPassword(Password) = db.FWDbFindVal("Password", 1) Then
                        isValid = True
                    End If

                Case Else
                    isValid = False

            End Select

            Dim sessID As String
            sessID = cPassword.SHA_HashPassword(db.FWDbFindVal("User Id", 1) & Now)

            If isValid = True Then
                retVal = sessID
            Else
                retVal = "-1"
            End If

            db.DBClose()
            db = Nothing
            Return retVal
        End Function

        '     <WebMethod()> _
        '     Public Function SetSchedule(ByVal ScheduleId As Integer, ByVal EmailName As emailType, ByVal EmailFreq As emailFreq, ByVal FreqParam As String, ByVal EmailDate As String, ByVal EmailTime As String, ByVal EmailParam As Integer) As Boolean
        'Dim db As New cFWDBConnection
        '         Dim fws As New cFWSettings
        '         Dim nextDate As String

        '         fws = Application("FWSettings")
        'db.DBOpen(fws, False)
        '         Session("fred") = 1

        '         If ScheduleId = 0 Then
        '             ' adding a new schedule
        '             db.SetFieldValue("Email Type", EmailName, "N", True)
        '             db.SetFieldValue("Frequency Param", FreqParam, "N", False)
        '             db.SetFieldValue("Email Param", EmailParam, "S", False)
        '             db.SetFieldValue("Email Frequency", EmailFreq, "N", False)
        '             If CDate(EmailDate) < Today Then
        '                 nextDate = GetNextRunDate(CDate(EmailDate), EmailFreq, FreqParam)
        '             Else
        '                 nextDate = EmailDate
        '             End If
        '             db.SetFieldValue("Next Run Date", nextDate, "D", False)
        '             db.SetFieldValue("Run Time", EmailTime, "S", False)
        '             db.FWDb("W", "email_schedule")
        '         Else
        '             ' amending an existing schedule
        '             Dim firstchange As Boolean = True

        '             db.SetFieldValue("Email Type", EmailName, "N", firstchange)
        '             firstchange = False

        '             If EmailFreq <> 0 Then
        '                 db.SetFieldValue("Email Frequency", EmailFreq, "N", firstchange)
        '                 firstchange = False
        '             End If

        '             If Not EmailDate Is Nothing Then
        '                 If CDate(EmailDate) < Today Then
        '                     nextDate = GetNextRunDate(CDate(EmailDate), EmailFreq, FreqParam)
        '                 Else
        '                     nextDate = EmailDate
        '                 End If
        '                 db.SetFieldValue("Next Run Date", nextDate, "D", firstchange)
        '                 firstchange = False
        '             End If

        '             If Not EmailTime Is Nothing Then
        '                 db.SetFieldValue("Run Time", EmailTime, "S", firstchange)
        '                 firstchange = False
        '             End If

        '             ' set email param anyway in case it is being 
        '             If firstchange = False Then
        '                 db.FWDb("A", "email_schedule", "Schedule Id", ScheduleId)
        '             End If
        '         End If

        '         db.DBClose()
        '         db = Nothing
        '         SetSchedule = True
        '     End Function

        '     <WebMethod()> _
        '     Public Function SetSTMPServerName(ByVal ServerName As String) As Boolean
        '         Dim success As Boolean
        '         Dim fws As New cFWSettings

        '         success = False
        '         Try
        '             fws = Application("FWSettings")
        '             fws.glMailServer = ServerName
        '             Application.Lock()
        '             Application("FWSettings") = fws
        '             Application.UnLock()

        '             ResaveINI()
        '             success = True

        '         Catch ex As Exception

        '         End Try

        '         SetSTMPServerName = success
        '     End Function

        '     <WebMethod()> _
        '     Public Function SetAuditorList(ByVal auditorList As String) As Boolean
        '         Dim fws As New cFWSettings
        '         Dim success As Boolean

        '         success = False
        '         Try
        '             fws = Application("FWSettings")
        '             fws.glAuditorList = auditorList
        '             Application.Lock()
        '             Application("FWSettings") = fws
        '             Application.UnLock()

        '             ResaveINI()
        '             success = True

        '         Catch ex As Exception

        '         End Try

        '         SetAuditorList = success
        '     End Function

        '     <WebMethod()> _
        '     Public Function GetAuditorList() As String
        '         Dim fws As New cFWSettings

        '         Try
        '             fws = Application("FWSettings")

        '             GetAuditorList = fws.glAuditorList
        '         Catch ex As Exception
        '             Return ""
        '         End Try
        '     End Function

        '     <WebMethod()> _
        '     Public Function SetFromAddress(ByVal fromAddress As String) As Boolean
        '         Dim success As Boolean
        '         Dim fws As New cFWSettings

        '         success = False
        '         Try
        '             fws = Application("FWSettings")
        '             fws.glMailFrom = fromAddress
        '             Application.Lock()
        '             Application("FWSettings") = fws
        '             Application.UnLock()

        '             ResaveINI()
        '             success = True

        '         Catch ex As Exception

        '         End Try

        '         SetFromAddress = success
        '     End Function

        '<WebMethod()> _
        'Public Function SetWebCheckInterval(ByVal minutes As Integer) As Boolean
        '    Dim success As Boolean
        '    Dim fws As FWSettings

        '    success = False

        '    Try
        '        fws = Application("FWSettings")
        '        fws.glWebEmailerCheckInterval = minutes
        '        Application.Lock()
        '        Application("FWSettings") = fws
        '        Application.UnLock()

        '        ResaveINI()
        '        success = True

        '    Catch ex As Exception

        '    End Try
        '    SetWebCheckInterval = success
        'End Function

        '<WebMethod()> _
        'Public Sub StartScheduler()
        '    Dim EmailThread As cEmail

        '    Try
        '        EmailThread = Application("EmailThread")

        '        EmailThread.StartTimer()

        '    Catch ex As Exception

        '    End Try
        'End Sub

        '<WebMethod()> _
        'Public Sub StopScheduler()
        '    Dim EmailThread As cEmail

        '    Try
        '        EmailThread = Application("EmailThread")

        '        EmailThread.StopTimer()

        '    Catch ex As Exception

        '    End Try
        'End Sub

        '<WebMethod()> _
        'Public Function KillEmailThread(ByVal password As String) As String
        '    Dim EmailThread As cEmail
        '    Dim cCrypt As New FWCrypt(FWCrypt.Providers.RC2)

        '    Try
        '        If cCrypt.Decrypt(password, EncryptionKey) = "halstead" Then
        '            EmailThread = Application("EmailThread")

        '            KillEmailThread = EmailThread.KillThread()
        '        Else
        '            KillEmailThread = "Incorrect password supplied"
        '        End If

        '        cCrypt = Nothing
        '    Catch ex As Exception
        '        KillEmailThread = ex.Message
        '    End Try
        'End Function

        '<WebMethod()> _
        'Public Function GetFWSettings() As cFWSettings
        '    Dim fws As New cFWSettings
        '    fws = Application("FWSettings")
        '    GetFWSettings = fws
        'End Function

        '<WebMethod()> _
        '     Public Function RunSQL(ByVal sql As String, ByVal AuthToken As String) As DataSet
        'Dim db As New cFWDBConnection
        '         Dim return_dSet As New DataSet

        '         Try
        '             If Session(AuthToken) <> 1 Then
        '                 Debug.WriteLine("Web Service RunSQL:Invalid AuthToken received")
        '             End If

        '             Debug.WriteLine("WebService:RunSQL:sql = " & Trim(sql))

        '             db.DBOpen(Application("FWSettings"))
        '             db.RunSQL(sql, db.glDBWorkA)
        '             return_dSet = db.glDBWorkA
        '             RunSQL = return_dSet

        '             Debug.WriteLine("WebService:RunSQL:rows returned = " & Trim(Str(return_dSet.Tables(0).Rows.Count)))

        '             If Not return_dSet Is Nothing Then
        '                 return_dSet.Dispose()
        '             End If


        '         Catch ex As Exception
        '             Debug.WriteLine("WebService:RunSQL:Error = " & ex.Message)

        '         Finally
        '             return_dSet = Nothing
        '             db.DBClose()
        '             db = Nothing

        '         End Try

        '         Return return_dSet
        '     End Function

        '     Private Sub ResaveINI()
        '         Dim fws As New cFWSettings
        '         Dim iniFile As System.IO.StreamWriter = New System.IO.StreamWriter(Server.MapPath("Framework.ini"), False)
        '         Dim cCrypt As New FWCrypt(FWCrypt.Providers.RC2)

        '         fws = Application("FWSettings")

        '         iniFile.WriteLine("DATABASE = " & cCrypt.Encrypt(Trim(fws.glDatabase), EncryptionKey))
        '         iniFile.WriteLine("DB_USER = " & cCrypt.Encrypt(Trim(fws.glDBUserId), EncryptionKey))
        '         'iniFile.WriteLine("DB_PWD = " & Crypt(Trim(fws.glDBPassword), 1))
        '         iniFile.WriteLine("PWD_METHOD = 1")
        '         iniFile.WriteLine("DB_PWD = " & cCrypt.Encrypt(Trim(fws.glDBPassword), EncryptionKey))
        '         iniFile.WriteLine("DBENGINE = " & Trim(Str(fws.glDBEngine)))
        '         iniFile.WriteLine("EMAILLOG = " & fws.glEmailLog)
        '         iniFile.WriteLine("LANGUAGE = WE")
        '         iniFile.WriteLine("MSERVER = " & cCrypt.Encrypt(Trim(fws.glMailServer), EncryptionKey))
        '         iniFile.WriteLine("MFROM = " & Trim(fws.glMailFrom))
        '         iniFile.WriteLine("SERVERNAME = " & cCrypt.Encrypt(Trim(fws.glServer), EncryptionKey))
        '         iniFile.WriteLine("TIMEOUT = " & Trim(fws.glDBTimeout))
        '         iniFile.WriteLine("FWLOGO = " & Trim(fws.glFWLogo))
        '         iniFile.WriteLine("PAGESIZE = " & Trim(Str(fws.glPageSize)))
        '         iniFile.WriteLine("UNIQUE_KEY_PREFIX = " & Trim(fws.KeyPrefix))
        '         iniFile.WriteLine("DOC_REPOSITORY = " & cCrypt.Encrypt(Trim(fws.glDocRepository), EncryptionKey))
        '         iniFile.WriteLine("SECURE_DOC_REPOSITORY = " & cCrypt.Encrypt(Trim(fws.glSecureDocRepository), EncryptionKey))
        '         iniFile.WriteLine("AUDITOR_LIST = " & Trim(fws.glAuditorList))
        '         iniFile.WriteLine("PWD_EXPIRY = " & Trim(IIf(fws.glPwdExpiry = True, "1", "0")))
        '         iniFile.WriteLine("PWD_EXPIRY_DAYS = " & Trim(Str(fws.glPwdExpiryDays)))
        '         iniFile.WriteLine("PWD_CONSTRAINT = " & Trim(Str(fws.glPwdLengthSetting)))
        '         iniFile.WriteLine("PWD_L1 = " & Trim(fws.glPwdLength1))
        '         iniFile.WriteLine("PWD_L2 = " & Trim(fws.glPwdLength2))
        '         iniFile.WriteLine("PWD_MCU = " & IIf(fws.glPwdUCase = True, "1", "0"))
        '         iniFile.WriteLine("PWD_MCN = " & IIf(fws.glPwdNums = True, "1", "0"))
        '         iniFile.WriteLine("INIT_VIEW = 0")
        '         iniFile.WriteLine("KEEPFORECAST = " & Trim(Str(fws.glKeepForecast)))
        '         iniFile.WriteLine("USE_DUNS = " & Trim(IIf(fws.glUseDUNS = True, "1", "0")))
        '         iniFile.WriteLine("ALLOW_MENU_ADD = " & Trim(IIf(fws.glAllowMenuAdd = True, "1", "0")))
        '         iniFile.WriteLine("AUTOUPDATECV = " & Trim(IIf(fws.glAutoUpdateCV = True, "1", "0")))

        '         If fws.hasDorana = True Then
        '             ' if this has been activated, then must keep it. Must have been activated manually by SEL.
        '             iniFile.WriteLine("DORANA_USER = 1")
        '         End If
        '         iniFile.WriteLine("MAX_RETRIES = " & Trim(Str(fws.glMaxRetries)))
        '         iniFile.WriteLine("PWD_HISTORY_NUM = " & Trim(Str(fws.glNumPwdHistory)))
        '         If fws.glWebEmailerURL <> "" Then
        '             iniFile.WriteLine("WEBURL = " & Trim(fws.glWebEmailerURL))
        '         End If

        '         If fws.glWebEmailerCheckInterval > 0 Then
        '             iniFile.WriteLine("WEB_CHECK_INTERVAL = " & Trim(Str(fws.glWebEmailerCheckInterval)))
        '         Else
        '             iniFile.WriteLine("WEB_CHECK_INTERVAL = 0")
        '         End If
        '         iniFile.WriteLine("APP_PATH = " & Trim(Str(fws.glApplicationPath)))
        '         iniFile.WriteLine("USE_RECHARGE = " & Trim(IIf(fws.glUseRechargeFunction = True, "1", "0")))
        '         iniFile.WriteLine("USE_SAVINGS = " & Trim(IIf(fws.glUseSavings = True, "1", "0")))

        '         iniFile.WriteLine("ERROR_SUBMIT = " & Trim(fws.glErrorSubmitEmail))
        '         iniFile.WriteLine("ERROR_SUBMIT_FROM = " & Trim(fws.glErrorSubmitFrom))

        '         iniFile.Close()
        '         iniFile = Nothing
        '         cCrypt = Nothing
        '     End Sub

        <WebMethod()> _
        Public Sub DoAuditCleardown()
            System.Diagnostics.Debug.WriteLine("WebService:remote call of AuditCleardown()")
            'ClearDownAudit(Application("FWSettings"), Nothing, Nothing, Server.MapPath("."))
        End Sub

        <WebMethod()> _
        Public Function DoReviewEmail(ByVal runDate As Date, ByVal daysahead As Integer) As Boolean
            Dim success As Boolean = False
            Dim LogFilename, sql As String
            Dim fws As New cFWSettings
            Dim db As New cFWDBConnection
            Dim drow, drow2 As DataRow
            Dim ShowDate, NotifyDate As Date
            Dim NYMParams As MaintParams
            Dim MsgCount As Integer

            fws = Application("FWSettings")
            LogFilename = "FWEmail" & Trim(Format(Today(), "ddMMyyyy")) & ".log"

            ' This routine sends any emails required to notify the user that contract renewals are due
            Dim Recipients, comma, Increment, ProdMsgs, ProdMsgPreamble
            'Dim Mval, PVal, MPct, Result

            FWLog(Me.Context.Current.ApplicationInstance, LogFilename, "Starting Review/Email4 (Licences Renewal Reminders)")

            ' Set the cutoff date for emails to two days ahead
            sql = "SELECT [Contract Number]," & vbNewLine & _
               "[Contract Id]," & vbNewLine & _
               "[Contract Description]," & vbNewLine & _
               "[Notice Period]," & vbNewLine & _
               "[Review Period]," & vbNewLine & _
               "[Review Date]," & vbNewLine & _
               "[Review Complete]," & vbNewLine & _
               "[Notice Period]," & vbNewLine & _
               "[Maintenance Type]," & vbNewLine & _
               "[Maintenance Pct]," & vbNewLine & _
               "[End Date]," & vbNewLine & _
               "[vendor_details].[Vendor Name]" & vbNewLine & _
               "FROM [contract_details]" & vbNewLine & _
               "INNER JOIN [vendor_details] ON [vendor_details].[Vendor Id] = [contract_details].[Vendor Id] " & vbNewLine & _
               "WHERE ([Review Complete] = 0 OR [Review Complete] IS NULL) AND [Archived] = 'N'"

            db.RunSQL(sql, db.glDBWorkA, False, "", False)

            For Each drow In db.glDBWorkA.Tables(0).Rows
                If Not IsDate(drow.Item("Review Complete")) Then
                    If IsDate(drow.Item("End Date")) Then
                        Increment = Val(drow.Item("Notice Period") & "") + Val(drow.Item("Review Period") & "")

                        ShowDate = Format(DateAdd("m", -Increment, drow.Item("End Date")), "dd/MM/yyyy")
                        NotifyDate = Format(ShowDate, "yyyyMMdd")
                        If NotifyDate <= runDate Then

                            ' Collect the recipients
                            sql = "SELECT [Email Address]" & vbNewLine & _
                              "FROM (([contract_details]" & vbNewLine & _
                              "LEFT OUTER JOIN [contract_notification]" & vbNewLine & _
                              "  ON [contract_details].[Contract Id] = [contract_notification].[Contract Id])" & vbNewLine & _
                              "LEFT OUTER JOIN [staff_details]" & vbNewLine & _
                              "   ON [contract_notification].[Staff Id] = [staff_details].[Staff Id])" & vbNewLine & _
                              "WHERE [contract_details].[Contract Id] = " & drow.Item("Contract Id")

                            db.RunSQL(sql, db.glDBWorkB, False, "", False)

                            ' Only send the email if there are recipients...
                            For Each drow2 In db.glDBWorkB.Tables(0).Rows
                                Recipients = ""
                                comma = ""

                                If IsDBNull(drow2.Item("Email Address")) = False Then
                                    Recipients = Recipients & comma & Trim(drow2.Item("Email Address"))
                                    comma = "; "
                                End If
                            Next

                            If Trim(Recipients) <> "" Then
                                ' Check the products, and generate warnings for each one
                                ' with NYM > Projected Saving
                                NYMParams.MaintCalcType = drow.Item("Maintenance Type")
                                NYMParams.PctOfLP = drow.Item("Maintenance Pct")
                                sql = "SELECT [productDetails].[ProductName]," & vbNewLine & _
                                  "[contract_productdetails].[Product Value]," & vbNewLine & _
                                  "[contract_productdetails].[Currency Id]," & vbNewLine & _
                                  "[contract_productdetails].[Maintenance Value]," & vbNewLine & _
                                  "[contract_productdetails].[Maintenance Percent]," & vbNewLine & _
                                  "[contract_productdetails].[Projected Saving]" & vbNewLine & _
                                  "FROM [contract_productdetails]" & vbNewLine & _
                                  "LEFT OUTER JOIN [productDetails]" & vbNewLine & _
                                  "  ON [contract_productdetails].[ProductId] = [productDetails].[ProductId]" & vbNewLine & _
                                  "WHERE [contract_productdetails].[ContractId] = " & drow.Item("ContractId")
                                db.RunSQL(sql, db.glDBWorkC, False, "", False)

                                ProdMsgs = ""
                                ProdMsgPreamble = "Maintenance from <001> is greater than the Annual Projected Saving for the following products." & vbNewLine

                                'For Each drow2 In db.glDBWorkC.Tables(0).Rows
                                '    If Val(drow2.Item("Projected Saving") & "") > 0 Then
                                '        NYMParams.CurMaintVal = drow2.Item("Maintenance Value")
                                '        NYMParams.ListPrice = drow2.Item("Product Value")
                                '        NYMParams.PctOfLP = drow2.Item("Maintenance Percent")

                                '        NYMparams.MaintTypeX= = glRPIVal
                                '        glNYM.MaintYear = 0
                                '        glNYM.ContMaintPct = glCMPct
                                '        glNYM.MaintType = glMType
                                '        glNYM.MaintVal = Mval
                                '        glNYM.ProdVal = PVal
                                '        glNYM.ProdMaintPct = MPct
                                '        glNYM.Calculate()
                                '        Result = glNYM.Result
                                '        If Result > glDbWorkC.Fields("Projected Saving") Then
                                '            ProdMsgs = ProdMsgs & ProdMsgPreamble
                                '            ProdMsgPreamble = ""
                                '            ProdMsgs = ProdMsgs & zz1GL(8301) & " " & glDbWorkC.Fields("Product Name") & vbNewLine
                                '            ProdMsgs = ProdMsgs & zz1GL(8302) & " " & Result & " "
                                '            ProdMsgs = ProdMsgs & zz1GL(8303) & " " & glDbWorkC.Fields("Projected Saving") & vbNewLine
                                '        End If
                                '    End If
                                'Next
                            End If

                            'Msg = zz1GL(6731) & vbNewLine & vbNewLine & _
                            '          zz1GL(6732) & " " & .Fields("Contract Number") & vbNewLine & _
                            '          zz1GL(6733) & " " & .Fields("Contract Description") & vbNewLine & _
                            '          zz1GL(6713) & " " & .Fields("Vendor Name") & vbNewLine & _
                            '          zz1GL(6734) & " " & ShowDate & vbNewLine & vbNewLine & _
                            '          zz1GL(517) & " " & .Fields("Notice Period") & vbNewLine & _
                            '          zz1GL(513) & " " & Format(.Fields("End Date"), "dd/mm/yyyy") & vbNewLine & _
                            '          zz1GL(6700) & " " & Now & vbNewLine & vbNewLine & ProdMsgs

                            'SendEmail(Recipients, zz1GL(6730), Msg)
                            MsgCount += 1

                        End If
                    End If
                End If
            Next

            FWLog(Me.Context.Current.ApplicationInstance, LogFilename, "End of Review/Email4: Messages sent: " & Str(MsgCount))

            DoReviewEmail = success
        End Function

        <WebMethod()> _
        Public Function SendEmail(ByVal EmailType As String, ByVal toAddr As String, ByVal fromAddr As String, ByVal Subject As String, ByVal Body As String) As Boolean
            Dim fwEmail As New System.Web.Mail.MailMessage
            Dim fwSender As System.Web.Mail.SmtpMail
            Dim fws As New cFWSettings

            fws = Application("FWSettings")

            Select Case LCase(EmailType)
                Case "review"
                Case Else
                    ' send email as it is literally received
                    fwEmail.Subject = Subject
                    fwEmail.From = fromAddr
                    fwEmail.To = toAddr
                    fwEmail.Body = Body

            End Select

            Try
                fwSender.SmtpServer = fws.glMailServer
                fwSender.Send(fwEmail)
                SendEmail = True

            Catch ex As Exception
                SendEmail = False

            End Try

            fwEmail = Nothing
        End Function
    End Class
End Namespace
