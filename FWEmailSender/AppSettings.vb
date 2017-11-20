Imports SpendManagementLibrary
Imports FWClasses
Imports FWBase

Public Module AppSettings

    'Cipher Key
    Public EncryptionKey As String = "qJdK3e4Ms4dU4sBfJ5sK43udKdT543TsB65ANdjTsmG23pLdYsNs23Hys23kQnc42Ld3ju"

    Public EmailSig As String = "Framework Contract Management" & vbNewLine & "Copyright Software (Europe) Ltd 2003 - 2010" & vbNewLine & "support@software-europe.co.uk" & vbNewLine & "Tel: 01522 881300 || Fax: 01522 881355"

    'Email Types
    Public Enum emailType
        ContractReview = 1
        OverdueInvoice = 2
        AuditCleardown = 3
        LicenceExpiry = 4
    End Enum

    'Email Frequency Options
    Public Enum emailFreq
        Once = 0
        Daily = 1
        Weekly = 2
        MonthlyOnFirstXDay = 3
        MonthlyOnDay = 4
        Every_n_Days = 5
    End Enum

    'Email Templaces
    'Public Enum emailTemplate
    '    ContractReview = 1
    '    OverdueInvoice = 2
    '    LicenceExpiry = 3
    'End Enum

    Public Structure FWSettings
        'Misc
        Public glWebURL As String

        'Database Settings
        Public glDBServer As String
        Public glDatabaseName As String
        Public glDBUsername As String
        Public glDBPassword As String
        Public glDBEngine As String
        Public glDBTimeout As Integer

        'Mail Settings
        Public glMailServer As String
        Public glMailFromAddress As String
    End Structure

    Public Function SetApplicationProperties() As cFWSettings
        Dim f_INI As System.IO.StreamReader
        Dim sysPath As String = ""
        Dim INILine, iniItem As String
        Dim iniSetting As Object
        Dim equalPos As Integer = 0
        Dim AppSettings As New cFWSettings
        Dim cCrypt As New FWCrypt(FWCrypt.Providers.RC2)
        Dim pwdMethod As String = "0"
        Dim tmpDBPwd As String = ""

        'glVendorCopyright = "Software (Europe) Ltd. 2010"

        sysPath = System.IO.Path.Combine(My.Application.Info.DirectoryPath, "Framework.ini")
        f_INI = System.IO.File.OpenText(sysPath)

        ' set any defaults
        AppSettings.hasDorana = False
        AppSettings.glAllowMenuAdd = False
        AppSettings.glDBUserId = "FrameworkUser"
        AppSettings.glDBPassword = "halstead"
        AppSettings.glDoranaCode = ""


        While f_INI.Peek <> -1
            INILine = f_INI.ReadLine
            equalPos = InStr(INILine, "=", CompareMethod.Text)
            iniItem = Left(INILine, equalPos - 1).Trim
            iniSetting = Mid(INILine, equalPos + 1, Len(INILine) - equalPos).Trim
            Select Case iniItem
                Case "DBENGINE"
                    AppSettings.glDBEngine = Integer.Parse(iniSetting)
                Case "DB_USER"
                    If iniSetting = "" Then
                        AppSettings.glDBUserId = "FrameworkUser"
                    Else
                        AppSettings.glDBUserId = iniSetting
                    End If
                Case "PWD_METHOD"
                    pwdMethod = iniSetting
                Case "DB_PWD"
                    tmpDBPwd = iniSetting
                Case "DATABASE"
                    AppSettings.glDatabase = iniSetting
                Case "EMAILLOG"
                    AppSettings.glEmailLog = iniSetting
                Case "LANGUAGE"
                    AppSettings.glLanguage = iniSetting
                Case "LOGSQL"
                    AppSettings.glLogSQL = iniSetting
                Case "SERVERNAME"
                    AppSettings.glServer = iniSetting
                    If AppSettings.glServer = "" Then
                        AppSettings.glServer = "."
                    End If
                    'AppSettings.glServer = "MARTINLT"
                Case "TIMEOUT"
                    AppSettings.glDBTimeout = iniSetting
                Case "MSERVER"
                    AppSettings.glMailServer = iniSetting
                Case "MFROM"
                    AppSettings.glMailFrom = iniSetting
                Case "INIT_VIEW"
                    AppSettings.glInitViewType = Integer.Parse(iniSetting)
                Case "FWLOGO"
                    AppSettings.glFWLogo = iniSetting
                Case "PAGESIZE"
                    AppSettings.glPageSize = Integer.Parse(iniSetting)
                Case "ATTACHMENTHANDLING"
                    AppSettings.glAttachmentHandling = Integer.Parse(iniSetting)
                Case "UNIQUE_KEY_PREFIX"
                    AppSettings.KeyPrefix = iniSetting
                Case "DOC_REPOSITORY"
                    AppSettings.glDocRepository = iniSetting
                Case "SECURE_DOC_REPOSITORY"
                    AppSettings.glSecureDocRepository = iniSetting
                Case "AUDITOR_LIST"
                    AppSettings.glAuditorList = iniSetting
                Case "PWD_EXPIRY_DAYS"
                    AppSettings.glPwdExpiryDays = Integer.Parse(iniSetting)
                Case "PWD_EXPIRY"
                    AppSettings.glPwdExpiry = IIf(Integer.Parse(iniSetting) = 0, False, True)
                Case "PWD_CONSTRAINT"
                    AppSettings.glPwdLengthSetting = Integer.Parse(iniSetting)
                Case "PWD_L1"
                    AppSettings.glPwdLength1 = Integer.Parse(iniSetting)
                Case "PWD_L2"
                    AppSettings.glPwdLength2 = Integer.Parse(iniSetting)
                Case "PWD_MCU"
                    AppSettings.glPwdUCase = IIf(Integer.Parse(iniSetting) = "1", True, False)
                Case "PWD_MCN"
                    AppSettings.glPwdNums = IIf(Integer.Parse(iniSetting) = "1", True, False)
                Case "KEEPFORECAST"
                    AppSettings.glKeepForecast = Integer.Parse(iniSetting)
                Case "USE_DUNS"
                    AppSettings.glUseDUNS = IIf(Integer.Parse(iniSetting) = 0, False, True)
                Case "ALLOW_MENU_ADD"
                    AppSettings.glAllowMenuAdd = IIf(Integer.Parse(iniSetting) = 0, False, True)
                Case "DORANA"
                    AppSettings.glDoranaCode = iniSetting
                Case "MAX_RETRIES"
                    AppSettings.glMaxRetries = Integer.Parse(iniSetting)
                Case "PWD_HISTORY_NUM"
                    AppSettings.glNumPwdHistory = Integer.Parse(iniSetting)
                Case "WEBURL"
                    AppSettings.glWebEmailerURL = iniSetting
                Case "WEB_EMAILADMIN"
                    AppSettings.glWebEmailerAdmin = iniSetting
                Case "WEB_CHECK_INTERVAL"
                    AppSettings.glWebEmailerCheckInterval = Integer.Parse(iniSetting)
                Case "ALLOW_NOTES_ADD"
                    AppSettings.glAllowNotesAdd = IIf(Integer.Parse(iniSetting) = 1, True, False)
                Case "AUTOUPDATECV"
                    AppSettings.glAutoUpdateCV = IIf(Integer.Parse(iniSetting) = 1, True, False)
                Case "APP_PATH"
                    AppSettings.glApplicationPath = iniSetting
                Case "ERROR_SUBMIT"
                    AppSettings.glErrorSubmitEmail = iniSetting
                Case "ERROR_SUBMIT_FROM"
                    AppSettings.glErrorSubmitFrom = iniSetting
                Case "USE_RECHARGE"
                    AppSettings.glUseRechargeFunction = IIf(Integer.Parse(iniSetting) = 1, True, False)
                Case "USE_SAVINGS"
                    AppSettings.glUseSavings = IIf(Integer.Parse(iniSetting) = 1, True, False)
                Case "SHOW_PRODUCT"
                    AppSettings.glShowProductonSearch = IIf(Integer.Parse(iniSetting) = 1, True, False)
                Case "COMPANYNO"
                    AppSettings.glCompanyNo = Integer.Parse(iniSetting)
                Case Else
            End Select
        End While

        f_INI.Close()

        Select Case pwdMethod
            Case "0"
                AppSettings.glDBPassword = cPassword.Crypt(tmpDBPwd.Trim, 2)
            Case "1"
                AppSettings.glDBUserId = cCrypt.Decrypt(AppSettings.glDBUserId.Trim, EncryptionKey)
                AppSettings.glDBPassword = cCrypt.Decrypt(tmpDBPwd, EncryptionKey)
                AppSettings.glSecureDocRepository = cCrypt.Decrypt(AppSettings.glSecureDocRepository.Trim, EncryptionKey)
                AppSettings.glDocRepository = cCrypt.Decrypt(AppSettings.glDocRepository.Trim, EncryptionKey)
                AppSettings.glDatabase = cCrypt.Decrypt(AppSettings.glDatabase.Trim, EncryptionKey)
                AppSettings.glMailServer = cCrypt.Decrypt(AppSettings.glMailServer.Trim, EncryptionKey)
                AppSettings.glServer = cCrypt.Decrypt(AppSettings.glServer.Trim, EncryptionKey)
                AppSettings.glErrorSubmitEmail = cCrypt.Decrypt(AppSettings.glErrorSubmitEmail.Trim, EncryptionKey)
                AppSettings.glErrorSubmitFrom = cCrypt.Decrypt(AppSettings.glErrorSubmitFrom.Trim, EncryptionKey)
                If AppSettings.glDoranaCode.Trim <> "" Then
                    AppSettings.glDoranaCode = cCrypt.Decrypt(AppSettings.glDoranaCode.Trim, EncryptionKey)
                End If

            Case Else
                ' nothing or unencrypted
                AppSettings.glDBPassword = "halstead"
        End Select

        SetApplicationProperties = AppSettings
        AppSettings = Nothing
        cCrypt = Nothing
    End Function
End Module