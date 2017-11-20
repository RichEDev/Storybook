Imports System.Text
Imports System.Data
Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Public Class cStandardReports

	'   Private monthOffset As Integer = 0
	'   Private FWSS As New cFWSettings
	'   Private UInfo As UserInfo
	'Private DB As cFWDBConnection
	'Private rs As cRechargeSetting

	'Public Property FWS() As cFWSettings
	'	Get
	'		Return FWSS
	'	End Get
	'	Set(ByVal Value As cFWSettings)
	'		FWSS = Value
	'	End Set
	'End Property

	'Public Property SetRechargeSettings() As cRechargeSetting
	'	Get
	'		Return rs
	'	End Get
	'	Set(ByVal Value As cRechargeSetting)
	'		rs = Value
	'	End Set
	'End Property

	'Public WriteOnly Property SetUserInfo() As UserInfo
	'	Set(ByVal newuInfo As UserInfo)
	'		UInfo = newuInfo
	'	End Set
	'End Property

	'Public Function RechargeCost(ByVal repType As StandardReportsType, ByVal monthNum As Int32, ByVal yearNum As Int32, ByVal searchItem As Int32) As DataSet
	'	Dim gSQL As New StringBuilder
	'	Dim joinSQL As String = ""
	'	Dim andStr As String = ""
	'	Dim primaryField As String = ""
	'	Dim primaryId As String = ""
	'	DB = New cFWDBConnection

	'	DB.DBOpen(FWS, False)

	'	With gSQL
	'		.Append("SELECT [Product Details].[Product Name], [Recharge Period], [Recharge Amount]")
	'		Select Case repType
	'			Case StandardReportsType.RechargeCostByCustomer
	'				primaryField = "[codes_rechargeentity].[Name]"
	'				primaryId = "[contract_productdetails_recharge].[Recharge Entity Id]"
	'				joinSQL = " LEFT JOIN [codes_rechargeentity] ON [codes_rechargeentity].[Entity Id] = [contract_productdetails_recharge].[Recharge Entity Id] "

	'			Case StandardReportsType.RechargeCostBySite

	'			Case StandardReportsType.RechargeCostBySupplier
	'			Case Else

	'		End Select

	'		.Append(",")
	'		.Append(primaryField & " AS [Primary]")
	'		.Append(" FROM [contract_productdetails_recharge]")
	'		.Append(" LEFT JOIN [contract_productdetails] ON [contract_productdetails].[Contract-Product Id] = [contract_productdetails_recharge].[Contract-Product Id]")
	'		.Append(" LEFT JOIN [product_details] ON [product_details].[Product Id] = [contract_productdetails].[Product Id]")
	'		.Append(joinSQL)
	'		.Append(" WHERE")
	'		If monthNum > 0 And yearNum > 0 Then
	'			.Append(" DATEPART(mm, [Recharge Period]) >= " & monthNum.ToString)
	'			.Append(" AND DATEPART(yy,[Recharge Period]) >= " & yearNum.ToString)
	'			andStr = " AND"
	'		End If
	'		If searchItem > 0 Then
	'			.Append(andStr)
	'			.Append(" " & primaryId & " = " & searchItem.ToString)
	'		End If
	'		.Append(" ORDER BY [Primary],[Recharge Period]")
	'	End With

	'	Dim ds As New DataSet
	'	DB.RunSQL(gSQL.ToString, ds, False, "", False)
	'	DB.DBClose()
	'	DB = Nothing

	'	Return ds
	'End Function

	'Public Function MaintenanceAnalysis(ByVal archived As Byte, ByVal location As Int32, ByVal monthNum As Int32, ByVal yearNum As Int32, Optional ByVal Vendor As Int32 = -1, Optional ByVal contractType As Int32 = -1, Optional ByVal GLCode As Int32 = -1) As DataSet
	'	Dim gSQL As New StringBuilder
	'	DB = New cFWDBConnection
	'	DB.DBOpen(FWS, False)

	'	gSQL.Append("SELECT DISTINCT " & vbNewLine)
	'	gSQL.Append(" [vendor_details].[Vendor Name]," & vbNewLine)
	'	gSQL.Append(" [codes_contracttype].[Contract Type Description] AS [Contract Type]," & vbNewLine)
	'	gSQL.Append(" ISNULL([product_details].[Product Name],'** product undefined **') AS [Product Name]," & vbNewLine)
	'	gSQL.Append(" [contract_details].[Start Date]," & vbNewLine)
	'	gSQL.Append(" [contract_details].[End Date]," & vbNewLine)
	'	gSQL.Append(" ([invoice_details].[Total Invoice Amount] / [codes_currency].[Conversion Rate]) AS [Current Month Actual]," & vbNewLine)
	'	gSQL.Append(" ([contract_forecastdetails].[Forecast Amount] / [codes_currency].[Conversion Rate]) AS [Current Month Budget]," & vbNewLine)
	'	gSQL.Append(" [contract_details].[Contract Id]" & vbNewLine)
	'	gSQL.Append(" FROM ((((((([contract_details]" & vbNewLine)

	'	gSQL.Append(" LEFT OUTER JOIN [vendor_details]" & vbNewLine)
	'	gSQL.Append(" ON [contract_details].[Vendor Id] = [vendor_details].[Vendor Id])" & vbNewLine)
	'	gSQL.Append(" LEFT OUTER JOIN [codes_contracttype]" & vbNewLine)
	'	gSQL.Append(" ON [contract_details].[Contract Type Id] = [codes_contracttype].[Contract Type Id])" & vbNewLine)
	'	gSQL.Append(" LEFT OUTER JOIN [contract_forecastdetails]" & vbNewLine)
	'	gSQL.Append(" ON [contract_details].[Contract Id] = [contract_forecastdetails].[Contract Id])" & vbNewLine)
	'	gSQL.Append(" LEFT OUTER JOIN [codes_currency]" & vbNewLine)
	'	gSQL.Append(" ON [contract_details].[Contract Currency] = [codes_currency].[Currency Id])" & vbNewLine)
	'	gSQL.Append(" LEFT OUTER JOIN [contract_forecastproducts]" & vbNewLine)
	'	gSQL.Append(" ON [contract_forecastdetails].[Contract-Forecast Id] = [contract_forecastproducts].[Forecast Id])" & vbNewLine)
	'	gSQL.Append(" LEFT OUTER JOIN [product_details]" & vbNewLine)
	'	gSQL.Append(" ON [product_details].[Product Id] = [contract_forecastproducts].[Product Id])" & vbNewLine)
	'	gSQL.Append(" LEFT OUTER JOIN [invoice_details]" & vbNewLine)
	'	gSQL.Append(" ON [invoice_details].[Contract Id] = [contract_details].[Contract Id])" & vbNewLine)
	'	gSQL.Append(" WHERE ([contract_details].[Contract Id] <> 0)" & vbNewLine)

	'	If archived <> 0 Then
	'		gSQL.Append(" AND ([contract_details].[Archived] = '")
	'		If archived = 1 Then
	'			gSQL.Append("Y')")
	'		Else
	'			gSQL.Append("N')")
	'		End If
	'		gSQL.Append(vbNewLine)
	'	End If

	'	If GLCode <> -1 Then
	'		DB.FWDb("R3", "user_fieldvalues", "User Value Id", GLCode, "", "", "", "", "", "", "", "", "", "")
	'		If DB.FWDb3Flag = True Then
	'			gSQL.Append("AND ([contract_details].[UF1] = '" & DB.FWDbFindVal("Field Value", 1).Trim & "')" & vbNewLine)
	'		End If
	'	End If

	'	gSQL.Append(" AND ([contract_details].[Location Id] = " & location & ")" & vbNewLine)

	'	gSQL.Append(" AND ((DATEPART(mm,[contract_forecastdetails].[Payment Date]) = " & monthNum & vbNewLine)
	'	gSQL.Append(" AND DATEPART(yyyy,[contract_forecastdetails].[Payment Date]) = " & yearNum & "))" & vbNewLine)

	'	If Vendor <> -1 Then
	'		gSQL.Append(" AND ([contract_details].[Vendor Id] = " & Vendor & ")" & vbNewLine)
	'	End If

	'	If contractType <> -1 Then
	'		gSQL.Append("AND (([contract_details].[Contract Type Id] = " & contractType & ")" & vbNewLine)
	'	End If

	'	gSQL.Append(" ORDER BY [vendor_details].[Vendor Name]")

	'	Dim ds As New DataSet

	'	DB.RunSQL(gSQL.ToString(), ds, False, "", False)

	'	'System.Diagnostics.Debug.WriteLine(gSQL.ToString)

	'	DB.DBClose()

	'	Return ds
	'End Function

	'Public Function MonthlyBudget(ByVal locationid As Integer, ByVal archived As Byte, ByVal monthNum As Int32, ByVal yearNum As Int32, Optional ByVal Vendor As Int32 = -1, Optional ByVal contractType As Int32 = -1, Optional ByVal GLCode As Int32 = -1) As DataSet
	'	Dim gSQL As New StringBuilder
	'	DB = New cFWDBConnection
	'	DB.DBOpen(FWS, False)

	'	'Stuff To Get [SELECT]
	'	gSQL.Append("SELECT " & vbNewLine)
	'	gSQL.Append("[vendor_details].[Vendor Name], " & vbNewLine)
	'	gSQL.Append("[codes_contracttype].[Contract Type Description] AS [Contract Type], " & vbNewLine)
	'	gSQL.Append("ISNULL([product_details].[Product Name],'** product undefined **') AS [Product Name],  " & vbNewLine)
	'	gSQL.Append("ISNULL([product_details].[Product Id],0) AS [Product Id], " & vbNewLine)
	'	gSQL.Append("[contract_details].[Start Date], " & vbNewLine)
	'	gSQL.Append("[contract_details].[End Date], " & vbNewLine)
	'	gSQL.Append("[contract_forecastdetails].[Payment Date],  " & vbNewLine)
	'	gSQL.Append("[contract_details].[Contract Id], " & vbNewLine)
	'	gSQL.Append("([contract_forecastdetails].[Forecast Amount] / [codes_currency].[Conversion Rate]) AS [Forecast Amount] " & vbNewLine)

	'	'Where To Get It From [FROM]
	'	gSQL.Append("FROM (((((([contract_details] " & vbNewLine)
	'	gSQL.Append("LEFT OUTER JOIN [vendor_details] " & vbNewLine)
	'	gSQL.Append("ON [contract_details].[Vendor Id] = [vendor_details].[Vendor Id]) " & vbNewLine)
	'	gSQL.Append("LEFT OUTER JOIN [codes_contracttype] " & vbNewLine)
	'	gSQL.Append("ON [contract_details].[Contract Type Id] = [codes_contracttype].[Contract Type Id]) " & vbNewLine)
	'	gSQL.Append("LEFT OUTER JOIN [Contract_forecastdetails] " & vbNewLine)
	'	gSQL.Append("ON [contract_details].[Contract Id] = [contract_forecastdetails].[Contract Id]) " & vbNewLine)
	'	gSQL.Append("LEFT OUTER JOIN [codes_currency] " & vbNewLine)
	'	gSQL.Append("ON [contract_details].[Contract Currency] = [codes_currency].[Currency Id]) " & vbNewLine)
	'	gSQL.Append("LEFT OUTER JOIN [contract_forecastproducts] " & vbNewLine)
	'	gSQL.Append("ON [contract_forecastdetails].[Contract-Forecast Id] = [contract_forecastproducts].[Forecast Id]) " & vbNewLine)
	'	gSQL.Append("LEFT OUTER JOIN [product_details] " & vbNewLine)
	'	gSQL.Append("ON [product_details].[Product Id] = [contract_forecastproducts].[Product Id]) " & vbNewLine)

	'	'Conditions [WHERE]
	'	gSQL.Append("WHERE ([contract_details].[Contract Id] <> 0)" & vbNewLine)

	'	If Vendor <> -1 Then
	'		gSQL.Append(" AND ([contract_details].[Vendor Id] = " & Vendor & ")" & vbNewLine)
	'	End If

	'	If contractType <> -1 Then
	'		gSQL.Append("AND ([contract_details].[Contract Type Id] = " & contractType & ")" & vbNewLine)
	'	End If

	'	If GLCode <> -1 Then
	'		DB.FWDb("R3", "user_fieldvalues", "User Value Id", GLCode, "", "", "", "", "", "", "", "", "", "")
	'		If DB.FWDb3Flag = True Then
	'			gSQL.Append("AND ([contract_details].[UF1] = '" & DB.FWDbFindVal("Field Value", 1).Trim & "')" & vbNewLine)
	'		End If
	'	End If

	'	If archived <> 0 Then
	'		gSQL.Append(" AND ([contract_details].[Archived] = '")
	'		If archived = 1 Then
	'			gSQL.Append("Y')")
	'		Else
	'			gSQL.Append("N')")
	'		End If
	'		gSQL.Append(vbNewLine)
	'	End If

	'	Dim tmpDate As New DateTime(yearNum, monthNum, 1)

	'	gSQL.Append(" AND ([contract_forecastdetails].[Payment Date] >= CONVERT(datetime, '" & tmpDate.ToString("yyyy-MM-dd") & "',120)")

	'	tmpDate = tmpDate.AddMonths(11)

	'	gSQL.Append(" AND [contract_forecastdetails].[Payment Date] <= CONVERT(datetime, '" & tmpDate.ToString("yyyy-MM-dd") & "', 120))" & vbNewLine)

	'	gSQL.Append(" AND ([contract_details].[Location Id] = " & locationid & ")" & vbNewLine)

	'	'Order results
	'	gSQL.Append("ORDER BY [Vendor Name] ASC, [contract_details].[Contract Id], [Contract Type Description], [Product Id], [Payment Date]")

	'	System.Diagnostics.Debug.Write(gSQL.ToString())

	'	Dim ds As New DataSet

	'	DB.RunSQL(gSQL.ToString(), ds, False, "", False)
	'	DB.DBClose()

	'	Return ds

	'End Function

	'Private Function WorkOutNextMonth(ByVal startMonth As Integer, ByVal startyear As Integer) As DateTime
	'	Dim tempDate As New DateTime(startyear, startMonth, 1)
	'	tempDate = tempDate.AddMonths(monthOffset)
	'	Return tempDate
	'End Function

	'Public Function DoDatasetMonthly(ByVal ds As DataTable, ByVal startMonth As Integer, ByVal startYear As Integer) As DataSet
	'	'Return the report dataset schema only
	'	Dim tDs As DataSet = GenerateBudgetCols()

	'	'Month
	'	Dim tmpMonth, curMonth As Integer

	'	'Declare variables needed in construction loop
	'	Dim endOf As Boolean = False
	'	Dim curContractId As Integer
	'	Dim firstRow As Boolean
	'	Dim curProduct As Integer = -1
	'	Dim curRow As Integer = 0
	'	Dim reportRow As Integer = 0
	'	Dim rowData As DataRow
	'	Dim yearEnd As Decimal
	'	Dim lastRow As Integer = ds.Rows.Count

	'	If ds.Rows.Count > 0 Then
	'		tDs.Tables(0).Rows.Add(tDs.Tables(0).NewRow())

	'		With ds
	'			Do While endOf = False
	'				rowData = .Rows(curRow)
	'				curContractId = rowData("Contract Id")
	'				curProduct = rowData("Product Id")
	'				curMonth = 0
	'				firstRow = True

	'				Do While curContractId = rowData("Contract Id")
	'					If rowData("Contract Id") = curContractId Then
	'						tDs.Tables(0).Rows(reportRow)("Vendor Name") = rowData("Vendor Name")
	'						tDs.Tables(0).Rows(reportRow)("Contract Type") = rowData("Contract Type")

	'						tDs.Tables(0).Rows(reportRow)("Start Date") = rowData("Start Date")
	'						tDs.Tables(0).Rows(reportRow)("End Date") = rowData("End Date")

	'						If rowData("Product Id") <> curProduct Then
	'							curProduct = rowData("Product Id")
	'							firstRow = False
	'							reportRow = reportRow + 1
	'							tDs.Tables(0).Rows.Add(tDs.Tables(0).NewRow())
	'						End If

	'						tDs.Tables(0).Rows(reportRow)("Product Name") = rowData("Product Name")

	'						If firstRow = True Then	'ensure it is the first row for this contract, otherwise we dont want the totals

	'							curMonth = startMonth

	'							For repCol As Integer = 5 To 17	'loop over the 12 columns in the report for monthly values
	'								If curMonth > 12 Then
	'									tmpMonth = curMonth - 12
	'								Else
	'									tmpMonth = curMonth
	'								End If

	'								Dim payDate As DateTime = DateTime.Parse(rowData("Payment Date"))

	'								If IsDBNull(rowData("Payment Date")) = False Then
	'									If payDate.Month = tmpMonth Then
	'										If IsDBNull(rowData("Forecast Amount")) = False Then
	'											tDs.Tables(0).Rows(reportRow)(repCol) = rowData("Forecast Amount")
	'											yearEnd = yearEnd + rowData("Forecast Amount")
	'										Else
	'											tDs.Tables(0).Rows(reportRow)(repCol) = "0"

	'										End If
	'										Exit For
	'									End If
	'								End If
	'								curMonth = curMonth + 1
	'							Next
	'						End If
	'					End If

	'					curRow = curRow + 1

	'					If curRow >= lastRow Then
	'						Exit Do
	'					End If

	'					rowData = .Rows(curRow)
	'				Loop

	'				If curRow >= lastRow Then
	'					'tDs.Tables(0).Rows(curRow - 1)("Year End") = yearEnd
	'					endOf = True
	'				Else
	'					tDs.Tables(0).Rows.Add(tDs.Tables(0).NewRow())
	'					reportRow = reportRow + 1
	'				End If
	'			Loop
	'		End With
	'	End If

	'	Return tDs
	'End Function

	'Private Function GenerateBudgetCols() As DataSet
	'	Dim tDs As New DataSet
	'	Dim tmpCol As DataColumn

	'	tDs.Tables.Add(0)

	'	tmpCol = New DataColumn
	'	tmpCol.ColumnName = "Vendor Name"
	'	tmpCol.DataType = GetType(String)
	'	tDs.Tables(0).Columns.Add(tmpCol)

	'	tmpCol = New DataColumn
	'	tmpCol.ColumnName = "Contract Type"
	'	tmpCol.DataType = GetType(String)
	'	tDs.Tables(0).Columns.Add(tmpCol)

	'	tmpCol = New DataColumn
	'	tmpCol.ColumnName = "Product Name"
	'	tmpCol.DataType = GetType(String)
	'	tDs.Tables(0).Columns.Add(tmpCol)

	'	tmpCol = New DataColumn
	'	tmpCol.ColumnName = "Start Date"
	'	tmpCol.DataType = GetType(DateTime)
	'	tDs.Tables(0).Columns.Add(tmpCol)

	'	tmpCol = New DataColumn
	'	tmpCol.ColumnName = "End Date"
	'	tmpCol.DataType = GetType(DateTime)
	'	tDs.Tables(0).Columns.Add(tmpCol)

	'	For x As Integer = 1 To 12
	'		tmpCol = New DataColumn
	'		tmpCol.ColumnName = "Month" & x
	'		tmpCol.DataType = GetType(Decimal)
	'		tDs.Tables(0).Columns.Add(tmpCol)
	'	Next

	'	tmpCol = New DataColumn
	'	tmpCol.ColumnName = "Year End"
	'	tmpCol.DataType = GetType(Decimal)
	'	tDs.Tables(0).Columns.Add(tmpCol)

	'	Return tDs
	'End Function

	'Public Function GetClients(ByVal locationid As Integer) As System.Collections.ArrayList
	'	Dim results As New System.Collections.ArrayList
	'	Dim ds As New DataSet
	'	Dim gSQL As New System.Text.StringBuilder

	'	gSQL.Append("SELECT [Entity Id],[Name] FROM [codes_rechargeentity]")
	'	gSQL.Append(" WHERE [Location Id] = " & locationid.ToString)
	'	gSQL.Append(" ORDER BY [Name]")
	'	DB = New cFWDBConnection
	'	DB.DBOpen(FWS, False)
	'	DB.RunSQL(gSQL.ToString(), ds, False, "", False)
	'	DB.DBClose()

	'	For Each dRow As DataRow In ds.Tables(0).Rows
	'		Dim dOut(1) As String
	'		dOut(0) = dRow.Item("Entity Id")
	'		dOut(1) = dRow.Item("Name")
	'		results.Add(dOut)
	'	Next

	'	Return results
	'End Function

	'Public Function GetVendors(ByVal locationid As Integer) As System.Collections.ArrayList
	'	Dim results As New System.Collections.ArrayList
	'	Dim ds As New DataSet
	'	Dim gSQL As New System.Text.StringBuilder

	'	gSQL.Append("SELECT [Vendor Id],[Vendor Name] FROM [vendor_details]")
	'	gSQL.Append(" WHERE [Location Id] = " & locationid.ToString)
	'	gSQL.Append(" ORDER BY [Vendor Name]")
	'	DB = New cFWDBConnection
	'	DB.DBOpen(FWS, False)
	'	DB.RunSQL(gSQL.ToString(), ds, False, "", False)
	'	DB.DBClose()

	'	For Each dRow As DataRow In ds.Tables(0).Rows
	'		Dim dOut(1) As String
	'		dOut(0) = dRow.Item("Vendor Id")
	'		dOut(1) = dRow.Item("Vendor Name")
	'		results.Add(dOut)
	'	Next

	'	Return results
	'End Function

	'Public Function GetContractTypes(ByVal locationid As Integer) As System.Collections.ArrayList
	'	Dim results As New System.Collections.ArrayList
	'	Dim ds As New DataSet
	'	Dim gSQL As New System.Text.StringBuilder

	'	gSQL.Append("SELECT [Contract Type Id],[Contract Type Description] FROM [codes_contracttype]")
	'	gSQL.Append(" WHERE [Location Id] = " & locationid.ToString)
	'	gSQL.Append(" ORDER BY [Contract Type Description]")
	'	DB = New cFWDBConnection
	'	DB.DBOpen(FWS, False)
	'	DB.RunSQL(gSQL.ToString(), ds, False, "", False)
	'	DB.DBClose()

	'	For Each dRow As DataRow In ds.Tables(0).Rows
	'		Dim dOut(1) As String
	'		dOut(0) = dRow.Item("Contract Type Id")
	'		dOut(1) = dRow.Item("Contract Type Description")
	'		results.Add(dOut)
	'	Next

	'	Return results
	'End Function

	'Public Function GetGLCodes() As System.Collections.ArrayList
	'	Dim results As New System.Collections.ArrayList
	'	Dim ds As New DataSet
	'	Dim gSQL As New System.Text.StringBuilder

	'	gSQL.Append("SELECT * FROM [user_fieldvalues]")
	'	gSQL.Append(" WHERE [User Field Id] = (SELECT [User Field Id] FROM [user_fields] WHERE RTRIM([Field Name]) = 'GL Code')")
	'	gSQL.Append(" ORDER BY [Field Value]")

	'	DB = New cFWDBConnection
	'	DB.DBOpen(FWS, False)
	'	DB.RunSQL(gSQL.ToString(), ds, False, "", False)
	'	DB.DBClose()

	'	For Each dRow As DataRow In ds.Tables(0).Rows
	'		Dim dOut(1) As String
	'		dOut(0) = dRow.Item("User Value Id")
	'		dOut(1) = dRow.Item("Field Value")
	'		results.Add(dOut)
	'	Next

	'	Return results
	'End Function

	'   'Public Sub New(ByRef FWSc As FWSettings)
	'   '    FWSS = FWSc
	'   'End Sub

	'   Public Function RenderCriteria(ByVal lit As System.Web.UI.WebControls.Literal, ByVal report As Integer, ByVal CriteriaString As String, ByVal ccn As Integer) As String
	'       Dim tmpStr As New System.Text.StringBuilder
	'       Dim criteriaKey As String
	'       Dim x As Integer

	'       tmpStr.Append("<div class=""inputpaneltitle"">Please specify report criteria...</div>" & vbNewLine)
	'       tmpStr.Append("<table>")

	'       For x = 1 To CriteriaString.Length Step 2
	'           criteriaKey = Mid(CriteriaString, x, 2)

	'           Select Case criteriaKey.ToUpper
	'               Case "YY"
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">Year</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddYear"" name=""ddYear""> ")
	'                   tmpStr.Append(YearList(DateTime.Now.Year - 5, DateTime.Now.Year + 5, DateTime.Now.Year))
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "MM"
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">Month</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddMonth"" name=""ddMonth"">")
	'                   tmpStr.Append(MonthList(DateTime.Now.Month))
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "AR"
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">Status</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddArchived"" name=""ddArchived"">")
	'                   tmpStr.Append("<option selected value=""0"">Both</option>")
	'                   tmpStr.Append("<option value=""1"">Archived</option>")
	'                   tmpStr.Append("<option value=""2"">Live</option>")
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "VE"
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">Supplier</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddVendor"" name=""ddVendor"">")
	'                   tmpStr.Append("<option selected value=""-1"">{All Suppliers}</option>")
	'                   tmpStr.Append(VendorList())
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "CT"
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">Contract Types</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddContractType""  name=""ddContractType"">")
	'                   tmpStr.Append("<option selected value=""-1"">{All Contract Types}</option>")
	'                   tmpStr.Append(TypeList())
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "GL" ' GL Code contained in UF1 - only for Virgin Mobile
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">GL Code</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddGLCode""  name=""ddGLCode"">")
	'                   tmpStr.Append("<option selected value=""-1"">{All GL Codes}</option>")
	'                   tmpStr.Append(GLCodeList())
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "RC" ' recharge customer
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">" & rs.ReferenceAs & "</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<select id=""ddClient"" name=""ddClient"">")
	'                   tmpStr.Append("<option selected value=""-1"">{All Clients}</option>")
	'                   tmpStr.Append(ClientList())
	'                   tmpStr.Append("</select>")
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)

	'               Case "RS" ' recharge site

	'               Case Else
	'                   tmpStr.Append("<tr>" & vbCrLf)
	'                   tmpStr.Append("<td class=""labeltd"">Unknown Criteria Type</td>" & vbCrLf)
	'                   tmpStr.Append("<td class=""inputtd"">" & vbCrLf)
	'                   tmpStr.Append("<b>ERROR!</b>" & vbCrLf)
	'                   tmpStr.Append("</td>" & vbCrLf)
	'                   tmpStr.Append("</tr>" & vbCrLf)
	'           End Select
	'       Next

	'       tmpStr.Append("<input type=""hidden"" id=""ViewReport""  name=""ViewReport"" value=""1"">")
	'       tmpStr.Append("<input type=""hidden"" id=""ReportId""  name=""ReportId"" value=""" & report.ToString & """>")
	'       tmpStr.Append("<input type=""hidden"" id=""ccn""  name=""ccn"" value=""" & ccn.ToString & """>")
	'       tmpStr.Append("<input type=""hidden"" id=""Paging""  name=""Paging"" value=""1"">")
	'       tmpStr.Append("<input type=""hidden"" id=""RepCriteria"" name=""RepCriteria"" value=""" & CriteriaString & """>")

	'       tmpStr.Append("</table>" & vbNewLine)

	'       Return tmpStr.ToString
	'   End Function

	'   Public Sub RenderPostbackButton(ByRef ph As System.Web.UI.WebControls.PlaceHolder, ByRef btnPostback As System.Web.UI.WebControls.Button)
	'       ph.Controls.Clear()
	'       btnPostback.Text = "Run Report"
	'       ph.Controls.Add(btnPostback)
	'   End Sub

	'   Public Sub RenderReportExport(ByRef phPdf As System.Web.UI.WebControls.PlaceHolder, ByRef btnPdf As System.Web.UI.WebControls.LinkButton, ByRef phExcel As System.Web.UI.WebControls.PlaceHolder, ByRef btnExcel As System.Web.UI.WebControls.LinkButton)
	'       phPdf.Controls.Clear()
	'       btnPdf.Text = "Export To PDF"
	'       phPdf.Controls.Add(btnPdf)
	'       phExcel.Controls.Clear()
	'       btnExcel.Text = "Export To Excel"
	'       phExcel.Controls.Add(btnExcel)
	'   End Sub

	'   'Public Sub RenderReportControl(ByRef ph As System.Web.UI.WebControls.PlaceHolder, ByRef repviewer As DataDynamics.ActiveReports.Web.WebViewer)
	'   '    ph.Controls.Clear()
	'   '    ph.Controls.Add(repviewer)
	'   'End Sub

	'   Private Function YearList(ByVal startYear As Integer, ByVal endYear As Integer, ByVal selectedYear As Integer) As String
	'       Dim retVal As New System.Text.StringBuilder
	'       For x As Integer = startYear To endYear
	'           If x = selectedYear Then
	'               retVal.Append("<option selected value=""" & x & """>" & x & "</option>" & vbCrLf)
	'           Else
	'               retVal.Append("<option value=""" & x & """>" & x & "</option>" & vbCrLf)
	'           End If
	'       Next
	'       Return retVal.ToString
	'   End Function

	'   Private Function MonthList(ByVal selectedMonth As Integer) As String
	'       Dim retVal As New System.Text.StringBuilder
	'       Dim curMonth As String
	'       For x As Integer = 1 To 12
	'           curMonth = WorkMonth(x)
	'           If x = selectedMonth Then
	'               retVal.Append("<option selected value=""" & x & """>" & curMonth & "</option>" & vbCrLf)
	'           Else
	'               retVal.Append("<option value=""" & x & """>" & curMonth & "</option>" & vbCrLf)
	'           End If
	'       Next
	'       Return retVal.ToString
	'   End Function

	'   Public Function WorkMonth(ByVal month As Integer) As String
	'       Dim myDT As New DateTime(1, month, 1)
	'       Return myDT.ToString("MMMM")
	'   End Function

	'   Private Function ClientList() As String
	'       Dim clients As ArrayList = GetClients(UInfo.ActiveLocation)
	'       Dim retVal As New System.Text.StringBuilder

	'       Dim myEnum As System.Collections.IEnumerator = clients.GetEnumerator()
	'       While myEnum.MoveNext = True
	'           Dim dOut() As String = myEnum.Current
	'           retVal.Append("<option value=""" & dOut(0) & """>" & dOut(1) & "</option>" & vbCrLf)
	'       End While

	'       Return retVal.ToString
	'   End Function

	'   Private Function VendorList() As String
	'       Dim vendors As ArrayList = GetVendors(UInfo.ActiveLocation)
	'       Dim retVal As New System.Text.StringBuilder

	'       Dim myEnum As System.Collections.IEnumerator = vendors.GetEnumerator()
	'       While myEnum.MoveNext = True
	'           Dim dOut() As String = myEnum.Current
	'           retVal.Append("<option value=""" & dOut(0) & """>" & dOut(1) & "</option>" & vbCrLf)
	'       End While

	'       Return retVal.ToString
	'   End Function

	'   Private Function TypeList() As String
	'       Dim types As ArrayList = GetContractTypes(UInfo.ActiveLocation)
	'       Dim retVal As New System.Text.StringBuilder

	'       Dim myEnum As System.Collections.IEnumerator = types.GetEnumerator()
	'       While myEnum.MoveNext = True
	'           Dim dOut() As String = myEnum.Current
	'           retVal.Append("<option value=""" & dOut(0) & """>" & dOut(1) & "</option>" & vbCrLf)
	'       End While

	'       Return retVal.ToString
	'   End Function

	'   Private Function GLCodeList() As String
	'       Dim types As ArrayList = GetGLCodes()
	'       Dim retVal As New System.Text.StringBuilder

	'       Dim myEnum As System.Collections.IEnumerator = types.GetEnumerator
	'       While myEnum.MoveNext = True
	'           Dim dOut() As String = myEnum.Current
	'           retVal.Append("<option value=""" & dOut(0) & """>" & dOut(1) & "</option>" & vbCrLf)
	'       End While

	'       Return retVal.ToString
	'   End Function
End Class

Public Enum StandardReportsType
    MonthlyBudget = 1
    VarianceReport = 2
    SavingsReport = 3
    RechargeCostByCustomer = 4
    RechargeCostBySupplier = 5
    RechargeCostBySite = 6
End Enum
