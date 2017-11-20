Imports SpendManagementLibrary
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Collections.Generic

Public Class cUtilAccounts
    Protected accs As SortedList(Of Integer, cAccount)

    Public Sub New()
        accs = New SortedList(Of Integer, cAccount)

        Dim db As New DBConnection(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
        Dim clssecure As New cSecureData()
        Dim accnt As cAccount
        Dim accountid, dbserverid As Integer
        Dim companyname, companyid, contact, dbname, dbusername, dbpassword As String
        Dim dbserver As String = ""
        Dim accounttype As Byte
        Dim archived As Boolean
        Dim nousers As Integer
        Dim expiry As DateTime
        Dim autologEnabled, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled As Boolean
        Dim expensesConnectLicenses As Integer
        Dim reportDatabaseID As Integer?
        Dim hostnameID As Integer
        Dim hostname As String
        Dim isNHSCustomer As Boolean = False
        Dim contactHelpDeskAllowed As Boolean
        Dim startYear As Integer
        Dim postcodeAnywhereKey As String
        Dim licencedUsers As String

        Dim servers As SortedList(Of Integer, String) = getDBServers()
        Dim lstAccountModuleLicenses As New List(Of cAccountModuleLicenses) ' = GetAccountModules()

        '                  0           1           2          3        4           5         6        7        8       9           10          11          12                  13                  14                          15              16                  17                      18                       19                     20              21          22                  23                      24               25
        Dim strsql As String = "SELECT accountid, companyname, companyid, contact, nousers, accounttype, expiry, dbserver, dbname, dbusername, dbpassword, archived, autologEnabled, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled, expensesConnectLicenses, reportDatabaseID, hostnameID, isNHSCustomer, contactHelpDeskAllowed, postcodeAnywhereKey, licencedUsers FROM dbo.registeredusers"

        Using reader As SqlDataReader = db.GetReader(strsql)

            While reader.Read()
                Try
                    accountid = reader.GetInt32(reader.GetOrdinal("accountid"))
                    companyname = reader.GetString(reader.GetOrdinal("companyname"))
                    companyid = reader.GetString(reader.GetOrdinal("companyid"))
                    contact = reader.GetString(reader.GetOrdinal("contact"))
                    accounttype = reader.GetByte(reader.GetOrdinal("accounttype"))
                    nousers = reader.GetInt32(reader.GetOrdinal("nousers"))
                    expiry = reader.GetDateTime(reader.GetOrdinal("expiry"))
                    If reader.IsDBNull(reader.GetOrdinal("dbserver")) = True Then
                        dbserverid = 0
                    Else
                        dbserverid = reader.GetInt32(reader.GetOrdinal("dbserver"))
                    End If
                    If dbserverid > 0 Then
                        If servers.Count > 0 Then
                            dbserver = servers(dbserverid)
                        End If
                    Else
                        dbserver = ""
                    End If
                    If reader.IsDBNull(reader.GetOrdinal("dbname")) = True Then
                        dbname = ""
                    Else
                        dbname = reader.GetString(reader.GetOrdinal("dbname"))
                    End If
                    If reader.IsDBNull(reader.GetOrdinal("dbusername")) = True Then
                        dbusername = ""
                    Else
                        dbusername = reader.GetString(reader.GetOrdinal("dbusername"))
                    End If
                    If reader.IsDBNull(reader.GetOrdinal("dbpassword")) = True Then
                        dbpassword = ""
                    Else
                        dbpassword = reader.GetString(reader.GetOrdinal("dbpassword"))
                    End If
                    archived = reader.GetBoolean(reader.GetOrdinal("archived"))

                    autologEnabled = reader.GetBoolean(reader.GetOrdinal("autologEnabled"))
                    quickEntryFormsEnabled = reader.GetBoolean(reader.GetOrdinal("quickEntryFormsEnabled"))
                    employeeSearchEnabled = reader.GetBoolean(reader.GetOrdinal("employeeSearchEnabled"))
                    hotelReviewsEnabled = reader.GetBoolean(reader.GetOrdinal("hotelReviewsEnabled"))
                    advancesEnabled = reader.GetBoolean(reader.GetOrdinal("advancesEnabled"))
                    postcodeAnyWhereEnabled = reader.GetBoolean(reader.GetOrdinal("postcodeAnyWhereEnabled"))
                    corporateCardsEnabled = reader.GetBoolean(reader.GetOrdinal("corporateCardsEnabled"))
                    If reader.IsDBNull(reader.GetOrdinal("expensesConnectLicenses")) = True Then
                        expensesConnectLicenses = 0
                    Else
                        expensesConnectLicenses = reader.GetInt32(reader.GetOrdinal("expensesConnectLicenses"))
                    End If

                    If reader.IsDBNull(reader.GetOrdinal("reportDatabaseID")) = True Then
                        reportDatabaseID = Nothing
                    Else
                        reportDatabaseID = reader.GetInt32(reader.GetOrdinal("reportDatabaseID"))
                    End If

                    hostnameID = reader.GetInt32(reader.GetOrdinal("hostnameID"))
                    hostname = GetHostnameByID(hostnameID)
                    isNHSCustomer = reader.GetBoolean(reader.GetOrdinal("isNHSCustomer"))
                    contactHelpDeskAllowed = reader.GetBoolean(reader.GetOrdinal("contactHelpDeskAllowed"))

                    If Not archived Then
                        Try
                            Dim connectionString As String = "Data Source=" + dbserver + ";Initial Catalog=" + dbname + ";User ID=" + dbusername + ";Password=" + clssecure.Decrypt(dbpassword) + ";Max Pool Size=10000"
                            startYear = 0 'getCompanyStartYear(connectionString)
                        Catch

                        Finally
                            startYear = 0
                        End Try
                    Else
                        startYear = 0
                    End If

                    If reader.IsDBNull(reader.GetOrdinal("postcodeAnywhereKey")) = False Then
                        postcodeAnywhereKey = reader.GetString(reader.GetOrdinal("postcodeAnywhereKey"))
                    Else
                        postcodeAnywhereKey = String.Empty
                    End If
                    If Not reader.IsDBNull(25) Then
                        licencedUsers = reader.GetString(25)
                    Else
                        licencedUsers = ""
                    End If

                    accnt = New cAccount(accountid, companyname, companyid, contact, nousers, expiry, accounttype, dbserverid, dbserver, dbname, dbusername, dbpassword, archived, autologEnabled, quickEntryFormsEnabled, employeeSearchEnabled, hotelReviewsEnabled, advancesEnabled, postcodeAnyWhereEnabled, corporateCardsEnabled, expensesConnectLicenses, reportDatabaseID, hostnameID, hostname, isNHSCustomer, startYear, contactHelpDeskAllowed, postcodeAnywhereKey, licencedUsers, getConnectionString(dbserver, dbname, dbusername, dbpassword))
                    accs.Add(accountid, accnt)
                    
                Catch ex As Exception
                    System.Diagnostics.EventLog.WriteEntry("Application", ex.Message)
                End Try

            End While
            reader.Close()
        End Using
    End Sub

    Private Function getConnectionString(dbServer As String, dbName As String, dbUsername As String, dbPassword As String) As String
        Dim connectionStringBuilder As New System.Data.SqlClient.SqlConnectionStringBuilder
        Dim clsSecureData As New cSecureData

        connectionStringBuilder = New SqlConnectionStringBuilder()
        connectionStringBuilder.DataSource = dbServer
        connectionStringBuilder.InitialCatalog = dbName
        connectionStringBuilder.UserID = dbUsername
        connectionStringBuilder.Password = clsSecureData.Decrypt(dbPassword)
        connectionStringBuilder.MaxPoolSize = 10000

        Return connectionStringBuilder.ConnectionString
    End Function

    Private Function getDBServers() As SortedList(Of Integer, String)
        Dim servers As New SortedList(Of Integer, String)()

        Dim db As New DBConnection(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)

        Dim databaseid As Integer
        Dim hostname As String

        Dim strsql As String = "select databaseid, hostname from databases"
        Using reader As SqlDataReader = db.GetReader(strsql)
            While (reader.Read())
                databaseid = reader.GetInt32(0)
                hostname = reader.GetString(1)
                servers.Add(databaseid, hostname)
            End While
            reader.Close()
        End Using
        Return servers
    End Function

    Private Function GetHostnameByID(ByVal hostnameID As Integer) As String
        Dim db As New DBConnection(ConfigurationManager.ConnectionStrings("metabase").ConnectionString)
        Dim hostname As String = String.Empty

        db.sqlexecute.Parameters.AddWithValue("@hostnameID", hostnameID)

        Dim strsql As String = "select hostname from hostnames WHERE hostnameID=@hostnameID"

        Using reader As SqlDataReader = db.GetReader(strsql)
            While reader.Read()
                hostname = reader.GetString(0)
            End While
            reader.Close()
        End Using
        db.sqlexecute.Parameters.Clear()

        Return hostname
    End Function

    Public Function getAccountById(ByVal accountid As Integer) As cAccount
        Dim acc As cAccount = Nothing
        If accs.ContainsKey(accountid) Then
            acc = accs(accountid)
        End If
        Return acc
    End Function

    Public Shared Function getConnectionString(ByVal accountid As Integer) As String
        Dim connectionstring As String = ""

        Dim clsaccounts As New cUtilAccounts
        Dim account As cAccount = clsaccounts.getAccountById(accountid)
        If Not account Is Nothing Then
            Dim clssecure As New cSecureData()
            connectionstring = "Data Source=" + account.dbserver + ";Initial Catalog=" + account.dbname + ";User ID=" + account.dbusername + ";Password=" + clssecure.Decrypt(account.dbpassword) + ";Max Pool Size=10000;"
        End If
        Return connectionstring
    End Function

    Public ReadOnly Property Accounts
        Get
            Return accs
        End Get
    End Property
End Class
