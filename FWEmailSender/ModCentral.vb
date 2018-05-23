Imports System
Imports System.Xml.Serialization
Imports System.IO
Imports System.Object
Imports System.Web
Imports System.Data
Imports System.Configuration
Imports System.ComponentModel
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports BusinessLogic.Modules
Imports FWBase
Imports SpendManagementLibrary
Imports Spend_Management

Module ModMain
    'Public FWS As New cFWSettings
    Public Enc As New Crypto(Crypto.Providers.RC2)
    Dim LogInfo As New System.Text.StringBuilder
    Public currentAccountId As Integer
    Public currentAccount As cAccount

    Sub Main()
        'Settings
        Console.WriteLine("Reading Settings...")
        'ReadSettings()
        'FWS = SetApplicationProperties()

        'Database Stuff
        Try
            Dim config As Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim connectionStrings As ConnectionStringsSection = config.GetSection("connectionStrings")
            Dim metaConnStr As String = connectionStrings.ConnectionStrings("metabase").ConnectionString
            Dim crypt As New cSecureData()

            If metaConnStr.Contains(ConfigurationManager.AppSettings("dbpassword").ToString()) = True Then
                metaConnStr = metaConnStr.Replace(ConfigurationManager.AppSettings("dbpassword").ToString(), crypt.Decrypt(ConfigurationManager.AppSettings("dbpassword").ToString()))
            End If

            GlobalVariables.MetabaseConnectionString = metaConnStr
            GlobalVariables.DefaultModule = Modules.contracts

            Dim metabase As New cAccounts
            metabase.CacheList()
            HostManager.SetHostInformation()
            ' check schedules for each database running on the server
            For Each i As KeyValuePair(Of Integer, cAccount) In cAccounts.CachedAccounts
                Dim acc As cAccount = CType(i.Value, cAccount)
                If acc.archived = False Then
                    Dim subaccs As New cAccountSubAccounts(acc.accountid)

                    currentAccountId = acc.accountid
                    currentAccount = acc

                    'If Not FWS Is Nothing Then
                    Console.WriteLine("Settings Read for " & acc.companyname)
                    AddToLog("Settings Read for " & acc.companyname)
                    'OpenDBConn()
                    HandleDBStuff()
                    'CloseDBConn()
                    AddToLog("Exiting FWEmailSender for " & acc.companyname, True)
                    'End If
                End If
            Next

            CleanOldLogFiles()

        Catch ex As Exception
            Console.WriteLine("Failed to interact with database.")
            AddToLog("FAIL - A database error occurred. " & ex.Message, True)
        End Try


    End Sub

   

    Public Sub AddToLog(ByVal StrIn As String, Optional ByVal DumpLog As Boolean = False)
        Debug.AutoFlush = True
        LogInfo.Append(Now & " " & StrIn & vbCrLf)
        Debug.WriteLine(StrIn)
        If DumpLog = True Then
            WriteLog()
        End If
    End Sub

    Public Sub WriteLog()
        If System.IO.Directory.Exists("logs") = False Then
            System.IO.Directory.CreateDirectory("logs")
        End If
        Dim accs As New cAccounts
        Dim acc As cAccount = accs.getAccountById(currentAccountId)

        Dim Writer As New StreamWriter("logs\FWEmail_" & acc.companyid & "_" & Format(Now, "yyyyMMdd") & ".log", True)
        Writer.Write(LogInfo.ToString)
        Writer.Close()
        LogInfo.Remove(0, LogInfo.Length)
    End Sub

    Private Function EncryptString(ByVal Strin As String) As String
        Return Enc.Encrypt(Strin, AppSettings.EncryptionKey)
    End Function

    Private Function DecryptString(ByVal StrIn As String) As String
        Return Enc.Decrypt(StrIn, AppSettings.EncryptionKey)
    End Function

    Private Sub CleanOldLogFiles()
        Dim fileList() As System.IO.FileInfo

        fileList = My.Computer.FileSystem.GetDirectoryInfo("logs").GetFiles

        For x As Integer = 0 To fileList.Length - 1
            Dim fileAge As Long = DateDiff(DateInterval.Day, fileList(x).LastWriteTime, Now)

            If fileList(x).Extension = ".log" And fileAge > 30 Then
                Try
                    System.IO.File.Delete(Path.Combine("logs", fileList(x).Name))

                    AddToLog("Deleted old log file [" & fileList(x).Name & "]")

                Catch ex As Exception
                    AddToLog("Failed to delete file --> " & ex.Message)
                End Try

            End If
        Next
    End Sub
End Module
