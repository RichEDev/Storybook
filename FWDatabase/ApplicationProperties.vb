Imports System
Imports System.Data
Imports System.Web
Imports System.Web.Mail
Imports System.Drawing
Imports System.Collections.Specialized
Imports System.Xml.Serialization
Imports Infragistics.WebUI.UltraWebGrid

Public Module ApplicationProperties
	Public glVendorCopyright As String
	Public DBTitleBar As String

	Public Structure ForecastBreakdownItem
		Dim ForecastProductId As Integer
		Dim ProductId As Integer
		Dim ProductName As String
		Dim ProductAmount As Double
	End Structure

	'Public Function SetApplicationProperties(Optional ByRef appServer As System.Web.HttpServerUtility = Nothing) As cFWSettings
	'    Dim f_INI As System.IO.StreamReader
	'    Dim sysPath As String = ""
	'    Dim INILine, iniItem As String
	'    Dim iniSetting As Object
	'    Dim equalPos As Integer
	'    Dim AppSettings As New cFWSettings
	'    Dim cCrypt As New FWCrypt(FWCrypt.Providers.RC2)
	'    Dim pwdMethod As String = "0"
	'    Dim tmpDBPwd As String = ""

	'    glVendorCopyright = "Software (Europe) Ltd. 2004"

	'    If appServer Is Nothing Then
	'        Dim appinfo As System.Web.HttpApplication

	'        appinfo = System.Web.HttpContext.Current.ApplicationInstance

	'        sysPath = appinfo.Server.MapPath("Framework.ini")
	'    Else
	'        sysPath = appServer.MapPath("Framework.ini")
	'    End If

	'    f_INI = System.IO.File.OpenText(sysPath)

	'    ' set any defaults
	'    AppSettings.hasDorana = False
	'    AppSettings.glAllowMenuAdd = False
	'    AppSettings.glDBUserId = "FrameworkUser"
	'    AppSettings.glDBPassword = "halstead"
	'    AppSettings.glDoranaCode = ""

	'    While f_INI.Peek <> -1
	'        INILine = f_INI.ReadLine
	'        equalPos = InStr(INILine, "=", CompareMethod.Text)
	'        iniItem = Left(INILine, equalPos - 1).Trim
	'        iniSetting = Mid(INILine, equalPos + 1, Len(INILine) - equalPos).Trim
	'        Select Case iniItem
	'            Case "DBENGINE"
	'                AppSettings.glDBEngine = Integer.Parse(iniSetting)
	'            Case "DB_USER"
	'                If iniSetting = "" Then
	'                    AppSettings.glDBUserId = "FrameworkUser"
	'                Else
	'                    AppSettings.glDBUserId = iniSetting
	'                End If
	'            Case "PWD_METHOD"
	'                pwdMethod = iniSetting
	'            Case "DB_PWD"
	'                tmpDBPwd = iniSetting
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
	'            Case "PWD_CONSTRAINT"
	'                AppSettings.glPwdLengthSetting = Integer.Parse(iniSetting)
	'            Case "PWD_L1"
	'                AppSettings.glPwdLength1 = Integer.Parse(iniSetting)
	'            Case "PWD_L2"
	'                AppSettings.glPwdLength2 = Integer.Parse(iniSetting)
	'            Case "PWD_MCU"
	'                AppSettings.glPwdUCase = IIf(Integer.Parse(iniSetting) = "1", True, False)
	'            Case "PWD_MCN"
	'                AppSettings.glPwdNums = IIf(Integer.Parse(iniSetting) = "1", True, False)
	'            Case "KEEPFORECAST"
	'                AppSettings.glKeepForecast = Integer.Parse(iniSetting)
	'            Case "USE_DUNS"
	'                AppSettings.glUseDUNS = IIf(Integer.Parse(iniSetting) = 0, False, True)
	'            Case "ALLOW_MENU_ADD"
	'                AppSettings.glAllowMenuAdd = IIf(Integer.Parse(iniSetting) = 0, False, True)
	'            Case "DORANA"
	'                AppSettings.glDoranaCode = iniSetting
	'            Case "MAX_RETRIES"
	'                AppSettings.glMaxRetries = Integer.Parse(iniSetting)
	'            Case "PWD_HISTORY_NUM"
	'                AppSettings.glNumPwdHistory = Integer.Parse(iniSetting)
	'            Case "WEBURL"
	'                AppSettings.glWebEmailerURL = iniSetting
	'            Case "WEB_EMAILADMIN"
	'                AppSettings.glWebEmailerAdmin = iniSetting
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
	'            Case "USE_SAVINGS"
	'                AppSettings.glUseSavings = IIf(Integer.Parse(iniSetting) = 1, True, False)
	'            Case "SHOW_PRODUCT"
	'                AppSettings.glShowProductonSearch = IIf(Integer.Parse(iniSetting) = 1, True, False)
	'            Case "COMPANYNO"
	'                AppSettings.glCompanyNo = Integer.Parse(iniSetting)
	'            Case "USESSO"
	'                AppSettings.glUseSSO = IIf(Integer.Parse(iniSetting) = 1, True, False)
	'            Case Else
	'        End Select
	'    End While

	'    f_INI.Close()

	'    Select Case pwdMethod
	'        Case "0"
	'            AppSettings.glDBPassword = Crypt(tmpDBPwd.Trim, 2)
	'        Case "1"
	'AppSettings.glDBUserId = cCrypt.Decrypt(AppSettings.glDBUserId.Trim, cDef.EncryptionKey)
	'AppSettings.glDBPassword = cCrypt.Decrypt(tmpDBPwd, cDef.EncryptionKey)
	'AppSettings.glSecureDocRepository = cCrypt.Decrypt(AppSettings.glSecureDocRepository.Trim, cDef.EncryptionKey)
	'AppSettings.glDocRepository = cCrypt.Decrypt(AppSettings.glDocRepository.Trim, cDef.EncryptionKey)
	'AppSettings.glDatabase = cCrypt.Decrypt(AppSettings.glDatabase.Trim, cDef.EncryptionKey)
	'AppSettings.glMailServer = cCrypt.Decrypt(AppSettings.glMailServer.Trim, cDef.EncryptionKey)
	'AppSettings.glServer = cCrypt.Decrypt(AppSettings.glServer.Trim, cDef.EncryptionKey)
	'AppSettings.glErrorSubmitEmail = cCrypt.Decrypt(AppSettings.glErrorSubmitEmail.Trim, cDef.EncryptionKey)
	'AppSettings.glErrorSubmitFrom = cCrypt.Decrypt(AppSettings.glErrorSubmitFrom.Trim, cDef.EncryptionKey)
	'If AppSettings.glDoranaCode.Trim <> "" Then
	'	AppSettings.glDoranaCode = cCrypt.Decrypt(AppSettings.glDoranaCode.Trim, cDef.EncryptionKey)
	'End If

	'' temporary generation code
	''Dim tmpCode As String
	''tmpCode = cCrypt.Encrypt("026" & UBQ_LINK_KEY, cFWSettings.EncryptionKey)
	'            'System.Diagnostics.Debug.WriteLine("Code = " & tmpCode)

	'        Case Else
	'            ' nothing or unencrypted
	'            AppSettings.glDBPassword = "halstead"
	'    End Select

	'    SetApplicationProperties = AppSettings
	'    AppSettings = Nothing
	'    cCrypt = Nothing
	'End Function

    'Public Structure RechargeSettings
    '    Public ReferenceAs As String
    '    Public StaffRepAs As String
    '    Public RechargePeriod As Integer
    '    Public FinYearCommence As Integer
    '    Public CP_Delete_Action As Integer
    '    Public Additional_CPInfo_UF_1 As Integer
    '    Public Additional_CPInfo_UF_2 As Integer
    'End Structure

    Public Structure UserHelpDocs
        Public ContractHelpDir As String
        Public Supercedes As String
        Public ConType As String
        Public ConCategory As String
        Public ConStatus As String
        Public ConOwner As String
        Public ConValue As String
        Public ConAnnualValue As String
        Public ConTermType As String
        Public ConInvFreq As String
        Public ContractAdditional As String
        Public VendorHelpDir As String
        Public VenFinancialStatus As String
        Public VenFinancialYE As String
        Public VenCategory As String
        Public VenStatus As String
        Public VenCustomerNumber As String
        Public VenName As String
    End Structure

	'Public Function LoadUserHelpDocs(ByVal db As cFWDBConnection, ByVal ui As UserInfo) As UserHelpDocs
	'	Dim uhd As UserHelpDocs
	'	Dim sql As String
	'	Dim drow As DataRow
	'	Dim tmpDef As String

	'	sql = "SELECT * FROM [user_helpdocs]"
	'	db.RunSQL(sql, db.glDBWorkA, False, "", False)

	'	For Each drow In db.glDBWorkA.Tables(0).Rows
	'		tmpDef = drow.Item("Path Def")
	'		Select Case CStr(drow.Item("Field Identifier"))
	'			' contract defintions
	'			Case "ContractHelpDir"
	'				uhd.ContractHelpDir = tmpDef
	'			Case "ContractSupercedes"
	'				uhd.Supercedes = tmpDef
	'			Case "ContractType"
	'				uhd.ConType = tmpDef
	'			Case "ContractCategory"
	'				uhd.ConCategory = tmpDef
	'			Case "ContractStatus"
	'				uhd.ConStatus = tmpDef
	'			Case "ContractOwner"
	'				uhd.ConOwner = tmpDef
	'			Case "ContractValue"
	'				uhd.ConValue = tmpDef
	'			Case "ContractAnnualValue"
	'				uhd.ConAnnualValue = tmpDef
	'			Case "ContractTermType"
	'				uhd.ConTermType = tmpDef
	'			Case "ContractInvFreq"
	'				uhd.ConInvFreq = tmpDef

	'				' contract additional
	'			Case "ContractAdditional"
	'				uhd.ContractAdditional = tmpDef

	'				' vendor definitions
	'			Case "VendorHelpDir"
	'				uhd.VendorHelpDir = tmpDef
	'			Case "VendorFinStatus"
	'				uhd.VenFinancialStatus = tmpDef
	'			Case "VendorName"
	'				uhd.VenName = tmpDef
	'			Case "VendorStatus"
	'				uhd.VenStatus = tmpDef
	'			Case "CustNumber"
	'				uhd.VenCustomerNumber = tmpDef
	'			Case "VendorFinYE"
	'				uhd.VenFinancialYE = tmpDef
	'			Case "VendorCategory"
	'				uhd.VenCategory = tmpDef

	'			Case Else

	'		End Select
	'	Next
	'	LoadUserHelpDocs = uhd
	'End Function

	'Public Sub CheckDoranaCode(ByRef fws As cFWSettings, ByVal db As cFWDBConnection)
	'	If fws.glDoranaCode.Trim <> "" Then
	'		' dorana code has not yet been validated
	'		Dim tmpCompanyNo As Integer
	'		tmpCompanyNo = Integer.Parse(db.FWDbFindVal("Contract No", 3))

	'		Dim tmpKeyNo As String
	'		tmpKeyNo = fws.glDoranaCode.Substring(0, 3)
	'		If IsNumeric(tmpKeyNo) = True Then
	'			If Integer.Parse(tmpKeyNo) = tmpCompanyNo Then
	'				fws.hasDorana = True
	'			End If
	'		End If
	'	End If
	'End Sub
End Module