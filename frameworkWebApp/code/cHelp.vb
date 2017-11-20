Imports SpendManagementLibrary
Imports Spend_Management
Imports FWClasses
Imports System.Data
Imports System.Configuration
Imports System.Web.Caching

Public Class cHelp
    Public list As System.Collections.SortedList

    Public Cache As System.Web.Caching.Cache = CType(System.Web.HttpContext.Current.Cache, System.Web.Caching.Cache)
    Private db As New cFWDBConnection
    Private strsql As String
    Private currentUser As CurrentUser
    Private helpKey As String

    Public Sub New(ByVal curUser As CurrentUser)
        currentUser = curUser
        helpKey = currentUser.Account.companyid
        CreateDependency()
        InitialiseData()
    End Sub

    Public Sub New()
        helpKey = ""
        CreateDependency()
        InitialiseData()
    End Sub

    Private Sub CreateDependency()
        If GlobalVariables.GetAppSettingAsBoolean("EnableBrokers") AndAlso Cache("helpdependency_" & helpKey) Is Nothing Then
            Cache.Insert("helpdependency_" & helpKey, 1)
        End If
    End Sub

    Private Function getDependency() As System.Web.Caching.CacheDependency
        Dim dependency(0) As String
        dependency(0) = "helpdependency_" & helpKey

        Dim dep As New System.Web.Caching.CacheDependency(Nothing, dependency)

        Return dep
    End Function

    Public Sub InvalidateCache()
        Cache.Remove("helpdependency_" & helpKey)
        CreateDependency()
    End Sub

    Private Sub InitialiseData()
        If Cache("help_" & helpKey) Is Nothing Then
            list = CacheList()
        Else
            list = CType(Cache("help_" & helpKey), System.Collections.SortedList)
        End If
    End Sub

    Private Function CacheList() As System.Collections.SortedList
        Dim list As New System.Collections.SortedList
        Dim helpid As Guid
        Dim helptext, description, helppage As String
        Dim reqhelp As cHelpItem
        Dim db As New cFWDBConnection

        If helpKey.Trim = "" Then
            db.DBOpenMetabase(ConfigurationManager.ConnectionStrings("metabase").ConnectionString, False)
        Else
            db.DBOpen(cMigration.ConvertToFWSettings(currentUser.Account, New cAccountSubAccounts(currentUser.Account.accountid).getSubAccountsCollection, currentUser.CurrentSubAccountId), False)
        End If

        strsql = "select tooltipID, page, description, helptext from dbo.help_text"

        Using reader As System.Data.SqlClient.SqlDataReader = db.GetReader(strsql)

            While reader.Read()
                helpid = reader.GetGuid(reader.GetOrdinal("tooltipID"))
                If reader.IsDBNull(reader.GetOrdinal("helptext")) Then
                    helptext = ""
                Else
                    helptext = reader.GetString(reader.GetOrdinal("helptext"))
                End If
                If reader.IsDBNull(reader.GetOrdinal("page")) Then
                    helppage = ""
                Else
                    helppage = reader.GetString(reader.GetOrdinal("page"))
                End If
                If reader.IsDBNull(reader.GetOrdinal("description")) Then
                    description = ""
                Else
                    description = reader.GetString(reader.GetOrdinal("description"))
                End If
                reqhelp = New cHelpItem(helpid, helppage, description, helptext)
                list.Add(helpid, reqhelp)
            End While

            reader.Close()
        End Using

        db.DBClose()

        If GlobalVariables.GetAppSettingAsBoolean("EnableBrokers") AndAlso list.Count > 0 Then
            Cache.Insert("help_" & helpKey, list, getDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(CInt(Caching.CacheTimeSpans.Medium)), CacheItemPriority.Default, Nothing)
        End If
        Return list
    End Function

    Public Function getHelpById(ByVal helpid As Guid) As cHelpItem
        Dim tmpHI As cHelpItem = CType(list(helpid), cHelpItem)
        If tmpHI Is Nothing Then
            tmpHI = New cHelpItem(Guid.Empty, "", "", "")
        End If

        Return tmpHI
    End Function

    Public Function getGrid() As DataTable
        Dim tbl As New DataTable
        tbl.Columns.Add("helpid", GetType(System.Int32))
        tbl.Columns.Add("page", GetType(System.String))
        tbl.Columns.Add("description", GetType(System.String))
        tbl.Columns.Add("text", GetType(System.String))

        Dim includeLogonTooltips As Boolean = True
        Dim accounts As New cAccounts()

        If accounts.CachedAccounts.Count > 1 Then
            includeLogonTooltips = False
        End If

        For Each item As cHelpItem In list.Values
            If includeLogonTooltips = False And item.page = "Logon" Then
                ' skip logon tooltips for hosted systems
            Else
                tbl.Rows.Add(New Object() {item.helpid, item.page, item.description, item.helptext})
            End If
        Next
        Return tbl
    End Function

    Public Function containsTip(ByVal id As Guid) As Boolean
        Dim item As cHelpItem = getHelpById(id)
        If item.helpid = Guid.Empty Then
            Return False
        End If

        If item.helptext.Trim.Length = 0 Then
            Return False
        End If

        Return True
    End Function

    Public Sub saveTooltip(ByVal helpid As Guid, ByVal text As String)
        Dim curItem As cHelpItem = getHelpById(helpid)
        curItem.helptext = text

        Dim db As New cFWDBConnection
        db.DBOpen(cMigration.ConvertToFWSettings(currentUser.Account, New cAccountSubAccounts(currentUser.Account.accountid).getSubAccountsCollection, currentUser.CurrentSubAccountId), False)
        db.AddDBParam("helpid", helpid, True)
        db.AddDBParam("text", text, False)
        db.ExecuteSQL("EXEC saveTooltip @helpid, @text")
        db.DBClose()
    End Sub

    Public Sub restoreTooltip(ByVal helpid As Guid)
        Dim db As New cFWDBConnection
        db.DBOpen(cMigration.ConvertToFWSettings(currentUser.Account, New cAccountSubAccounts(currentUser.Account.accountid).getSubAccountsCollection, currentUser.CurrentSubAccountId), False)
        db.AddDBParam("helpid", helpid, True)
        db.ExecuteSQL("EXEC restoreDefaultTooltip @helpid")
        db.DBClose()

        InvalidateCache()
    End Sub
End Class

Public Class cHelpItem
    Private gHelpId As Guid
    Private sDescription As String
    Private sHelptext As String
    Private sPage As String

    Public Sub New(ByVal helpid As Guid, ByVal helppage As String, ByVal description As String, ByVal helptext As String)
        gHelpId = helpid
        sPage = helppage
        sDescription = description
        sHelptext = helptext.Replace("'", "`")
    End Sub

#Region "properties"
    Public ReadOnly Property helpid() As Guid
        Get
            Return gHelpId
        End Get
    End Property

    Public ReadOnly Property page() As String
        Get
            Return sPage
        End Get
    End Property

    Public ReadOnly Property description() As String
        Get
            Return sDescription
        End Get
    End Property

    Public Property helptext() As String
        Get
            Return sHelptext
        End Get
        Set(ByVal value As String)
            sHelptext = value
        End Set
    End Property
#End Region

End Class
