Imports FWBase

Public Module FWUtilityRoutines
    Public LastLogFile As String

    <Serializable()> _
    Public Structure testStruct
        Public myStr As String
        Public myStr2 As String
    End Structure

    'Public Sub TestXMLSave(ByVal mystruct As testStruct)
    '    Dim xml As New FWCommon.xml

    '    xml.SaveObj(Application.StartupPath & "\test.xml", GetType(testStruct), mystruct)
    'End Sub

    'Public Sub SaveXMLSettings(ByVal DatIn As cFWSettings)
    '    Dim xml As New FWCommon.xml
    '    Dim crypto As New FWCommon.crypto

    '    DatIn.xmlFileVersion = FWEmail.ActiveDBVersion
    '    If DatIn.Empty = False Then
    '        If DatIn.xmlFileVersion <= 19 Then
    '            DatIn.glDBPassword = crypto.EncryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, DatIn.glDBPassword)
    '            DatIn.glDBUserId = crypto.EncryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, DatIn.glDBUserId)
    '            DatIn.glDatabase = crypto.EncryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, DatIn.glDatabase)
    '            DatIn.glWebEmailerURL = crypto.EncryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, DatIn.glWebEmailerURL)
    '            DatIn.glServer = DatIn.glServer
    '            DatIn.glEmailLog = DatIn.glEmailLog
    '            DatIn.glMailServer = DatIn.glMailServer
    '            DatIn.glMailFrom = DatIn.glErrorSubmitFrom
    '            DatIn.glAuditorList = DatIn.glAuditorList
    '            DatIn.glUseWebService = DatIn.glUseWebService
    '        End If
    '    End If

    '    xml.SaveObj(Application.StartupPath & "\FWSettings.xml", GetType(cFWSettings), DatIn)
    'End Sub

    'Public Function LoadXMLSettings() As cFWSettings
    '    Dim xml As New FWCommon.xml
    '    Dim crypto As New FWCommon.crypto
    '    Dim fws As New cFWSettings

    '    Try
    '        If System.IO.File.Exists(Application.StartupPath & "\FWSettings.xml") = True Then
    '            fws = xml.ReadObject(Application.StartupPath & "\FWSettings.xml", GetType(cFWSettings))

    '            If fws.Empty = False Then
    '                If fws.XmlFileVersion <= 19 Then
    '                    fws.glDBPassword = crypto.DecryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, fws.glDBPassword)
    '                    fws.glDBUserId = crypto.DecryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, fws.glDBUserId)
    '                    fws.glDatabase = crypto.DecryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, fws.glDatabase)
    '                    fws.glWebEmailerURL = crypto.DecryptRC2(FWCommon.FWCommon.rc2Key, FWCommon.FWCommon.rc2IV, fws.glWebEmailerURL)
    '                End If
    '            End If
    '        Else
    '            fws.glDatabase = ""
    '            fws.glDBPassword = "FrameworkUser"
    '            fws.glDBUserId = "halstead"
    '            fws.glWebEmailerURL = ""
    '            fws.XmlFileVersion = 19
    '            fws.glServer = ""
    '            fws.glMailServer = ""
    '            fws.glMailFrom = ""
    '            fws.glAuditorList = ""
    '            fws.glUseWebService = False
    '        End If

    '    Catch ex As Exception
    '        fws.glDatabase = ""
    '        fws.glDBPassword = ""
    '        fws.glDBUserId = ""
    '        fws.glWebEmailerURL = ""
    '        fws.XmlFileVersion = 19
    '        fws.glServer = ""
    '        fws.glMailServer = ""
    '        fws.glMailFrom = ""
    '        fws.glAuditorList = ""
    '        fws.glUseWebService = False
    '    End Try

    '    Return fws
    'End Function

    'Public Function OldSetApplicationProperties(Optional ByRef appServer As System.Web.HttpServerUtility = Nothing) As cFWSettings
    '    Dim f_INI As System.IO.StreamReader
    '    Dim sysPath As String
    '    Dim INILine, iniItem As String
    '    Dim iniSetting As Object
    '    Dim equalPos As Integer
    '    Dim AppSettings As New cFWSettings

    '    glVendorCopyright = "Software (Europe) Ltd. 2007"

    '    sysPath = Application.StartupPath & "\Framework.ini"
    '    If System.IO.File.Exists(sysPath) = True Then
    '        f_INI = System.IO.File.OpenText(sysPath)
    '    Else
    '        AppSettings.Empty = True
    '        OldSetApplicationProperties = AppSettings
    '        Exit Function
    '    End If

    '    ' set any defaults
    '    AppSettings.hasDorana = False
    '    AppSettings.glAllowMenuAdd = False
    '    AppSettings.glDBUserId = "FrameworkUser"
    '    AppSettings.glDBPassword = "halstead"

    '    While f_INI.Peek <> -1
    '        INILine = f_INI.ReadLine
    '        equalPos = InStr(INILine, "=", CompareMethod.Text)
    '        iniItem = Trim(Left(INILine, equalPos - 1))
    '        iniSetting = Trim(Mid(INILine, equalPos + 1, Len(INILine) - equalPos))
    '        Select Case iniItem
    '            Case "DBENGINE"
    '                AppSettings.glDBEngine = Integer.Parse(iniSetting)
    '            Case "DB_USER"
    '                If iniSetting = "" Then
    '                    AppSettings.glDBUserId = "FrameworkUser"
    '                Else
    '                    AppSettings.glDBUserId = iniSetting
    '                End If
    '            Case "DB_PWD"
    '                If iniSetting = "" Then
    '                    AppSettings.glDBPassword = "halstead"
    '                Else
    '                    AppSettings.glDBPassword = cUtility.Crypt(Trim(iniSetting), 2)
    '                End If
    '            Case "DATABASE"
    '                AppSettings.glDatabase = iniSetting
    '            Case "EMAILLOG"
    '                AppSettings.glEmailLog = iniSetting
    '            Case "LANGUAGE"
    '                AppSettings.glLanguage = iniSetting
    '            Case "LOGSQL"
    '                AppSettings.glLogSQL = iniSetting
    '            Case "SERVERNAME"
    '                AppSettings.glServer = iniSetting
    '                If AppSettings.glServer = "" Then
    '                    AppSettings.glServer = "."
    '                End If
    '                'AppSettings.glServer = "MARTINLT"
    '            Case "TIMEOUT"
    '                AppSettings.glDBTimeout = iniSetting
    '            Case "MSERVER"
    '                AppSettings.glMailServer = iniSetting
    '            Case "MFROM"
    '                AppSettings.glMailFrom = iniSetting
    '            Case "INIT_VIEW"
    '                AppSettings.glInitViewType = Integer.Parse(iniSetting)
    '            Case "FWLOGO"
    '                AppSettings.glFWLogo = iniSetting
    '            Case "PAGESIZE"
    '                AppSettings.glPageSize = Integer.Parse(iniSetting)
    '            Case "ATTACHMENTHANDLING"
    '                AppSettings.glAttachmentHandling = Integer.Parse(iniSetting)
    '            Case "UNIQUE_KEY_PREFIX"
    '                AppSettings.KeyPrefix = iniSetting
    '            Case "DOC_REPOSITORY"
    '                AppSettings.glDocRepository = iniSetting
    '            Case "SECURE_DOC_REPOSITORY"
    '                AppSettings.glSecureDocRepository = iniSetting
    '            Case "AUDITOR_LIST"
    '                AppSettings.glAuditorList = iniSetting
    '            Case "PWD_EXPIRY_DAYS"
    '                AppSettings.glPwdExpiryDays = Integer.Parse(iniSetting)
    '            Case "PWD_EXPIRY"
    '                AppSettings.glPwdExpiry = IIf(Integer.Parse(iniSetting) = 0, False, True)
    '            Case "KEEPFORECAST"
    '                AppSettings.glKeepForecast = Integer.Parse(iniSetting)
    '            Case "USE_DUNS"
    '                AppSettings.glUseDUNS = IIf(Integer.Parse(iniSetting) = 0, False, True)
    '            Case "ALLOW_MENU_ADD"
    '                AppSettings.glAllowMenuAdd = IIf(Integer.Parse(iniSetting) = 0, False, True)
    '            Case "DORANA_USER"
    '                AppSettings.hasDorana = IIf(Integer.Parse(iniSetting) = 1, True, False)
    '            Case "MAX_RETRIES"
    '                AppSettings.glMaxRetries = Integer.Parse(iniSetting)
    '            Case "PWD_HISTORY_NUM"
    '                AppSettings.glNumPwdHistory = Integer.Parse(iniSetting)
    '            Case "WEBURL"
    '                AppSettings.glWebEmailerURL = iniSetting
    '            Case "WEB_CHECK_INTERVAL"
    '                AppSettings.glWebEmailerCheckInterval = Integer.Parse(iniSetting)
    '            Case "ALLOW_NOTES_ADD"
    '                AppSettings.glAllowNotesAdd = IIf(Integer.Parse(iniSetting) = 1, True, False)
    '            Case "AUTOUPDATECV"
    '                AppSettings.glAutoUpdateCV = IIf(Integer.Parse(iniSetting) = 1, True, False)
    '            Case "APP_PATH"
    '                AppSettings.glApplicationPath = iniSetting
    '            Case "ERROR_SUBMIT"
    '                AppSettings.glErrorSubmitEmail = iniSetting
    '            Case "ERROR_SUBMIT_FROM"
    '                AppSettings.glErrorSubmitFrom = iniSetting
    '            Case "USE_RECHARGE"
    '                AppSettings.glUseRechargeFunction = IIf(Integer.Parse(iniSetting) = 1, True, False)
    '            Case "USE_WSVC"
    '                AppSettings.glUseWebService = IIf(Integer.Parse(iniSetting) = 1, True, False)
    '            Case Else
    '        End Select
    '    End While
    '    f_INI.Close()
    '    OldSetApplicationProperties = AppSettings
    '    AppSettings = Nothing
    'End Function
End Module
