Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Collections.Generic
Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management
Imports System.Data

<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
<System.Web.Script.Services.ScriptService()> _
Public Class svcAutoComplete
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function GetContracts(ByVal prefixText As String, ByVal count As Integer) As String()
        Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
        Dim retStr As New List(Of String)
        Dim db As New cFWDBConnection
        db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)

        Dim sql As String
        sql = "SELECT TOP 10 [contractKey],[contractDescription] FROM contract_details WHERE [contractDescription] LIKE @conDesc AND [subAccountId] = @locId AND dbo.IsVariation(contract_details.[contractId]) = 0"
        db.AddDBParam("conDesc", "%" & prefixText & "%", True)
        db.AddDBParam("locId", curUser.CurrentSubAccountId, False)
        db.RunSQL(sql, db.glDBWorkA, False, "", False)

        Dim drow As DataRow
        For Each drow In db.glDBWorkA.Tables(0).Rows
            retStr.Add(drow("contractKey") & " ~ " & drow("contractDescription"))
        Next

        db.DBClose()
        db = Nothing
        Return retStr.ToArray()
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function GetSupplier(ByVal prefixText As String, ByVal count As Integer) As String()
        Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
        Dim retStr As New List(Of String)
        Dim db As New cFWDBConnection
        db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)

        Dim sql As String
        sql = "SELECT TOP 10 [suppliername] FROM supplier_details WHERE [suppliername] LIKE '%' + @supplier + '%' AND [subAccountId] = @locId"
        db.AddDBParam("supplier", prefixText, True)
        db.AddDBParam("locId", curUser.CurrentSubAccountId, False)
        db.RunSQL(sql, db.glDBWorkA, False, "", False)

        Dim drow As DataRow
        For Each drow In db.glDBWorkA.Tables(0).Rows
            retStr.Add(drow("suppliername"))
        Next

        db.DBClose()
        db = Nothing
        Return retStr.ToArray()
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function GetSites(ByVal prefixText As String, ByVal count As Integer) As String()
        Dim retStr As New List(Of String)
        Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
        Dim db As New cFWDBConnection
        db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)

        Dim sql As String
        sql = "SELECT TOP 10 [Site Code], [Site Description] FROM codes_sites WHERE ([Site Description] LIKE '%' + @site + '%' OR [Site Code] LIKE '%' + @site + '%') AND [Location Id] = @locId"
        db.AddDBParam("site", prefixText, True)
        db.AddDBParam("locId", curUser.CurrentSubAccountId, False)
        db.RunSQL(sql, db.glDBWorkA, False, "", False)

        Dim drow As DataRow
        For Each drow In db.glDBWorkA.Tables(0).Rows
            retStr.Add(drow("Site Code") & " ~ " & drow("Site Description"))
        Next

        db.DBClose()
        db = Nothing
        Return retStr.ToArray()
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function GetClients(ByVal prefixText As String, ByVal count As Integer) As String()
        Dim curUser As CurrentUser = cMisc.getCurrentUser(User.Identity.Name)
        Dim retStr As New List(Of String)
        Dim db As New cFWDBConnection
        db.DBOpen(cMigration.ConvertToFWSettings(curUser.Account, New cAccountSubAccounts(curUser.Account.accountid).getSubAccountsCollection, curUser.CurrentSubAccountId), False)

        Dim sql As String
        sql = "SELECT TOP 10 [Name] FROM codes_rechargeentity WHERE [Name] LIKE '%' + @client + '%' AND [Location Id] = @locId"
        db.AddDBParam("client", prefixText, True)
        db.AddDBParam("locId", curUser.CurrentSubAccountId, False)
        db.RunSQL(sql, db.glDBWorkA, False, "", False)

        Dim drow As DataRow
        For Each drow In db.glDBWorkA.Tables(0).Rows
            retStr.Add(drow("Name"))
        Next

        db.DBClose()
        db = Nothing
        Return retStr.ToArray()
    End Function
End Class
