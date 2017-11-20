Imports Microsoft.VisualBasic
Imports SpendManagementLibrary

Namespace Framework2006
    Public Module ReportRoutines
        ' Generic routines used by the custom reporting.
        ' Functions and Procedures held in this module in order to maintain tidiness within core application code.

		'      Public item As Object

		'      Public CRSQL As String
		'      Const CRSQLOrder As String = "FJCGO"
		'      Const CRConditions As String = "EQUAL TO,NOT EQUAL TO,GREATER THAN,LESS THAN,GREATER THAN OR EQUAL,LESS THAN OR EQUAL,LIKE (% Wildcard)"
		'      Const CRConditionsSQL As String = "=,<>,>,<,>=,<=,LIKE"
		'      Const CRDefaultTable As String = "[contract_details]"
		'      Public DATE_CONDITION_TEXT() As String = {"[None]", "ON", "NOT ON", "AFTER", "BEFORE", "ON OR AFTER", "ON OR BEFORE", "BETWEEN"}
		'      Public DATE_CONDITION_SYMBOL() As String = {"", "=", "<>", ">", "<", ">=", "<=", "BETWEEN"}
		'      Public TEXT_CONDITION_TEXT() As String = {"[None]", "MATCHES", "LIKE (% Wildcard)"}
		'      Public TEXT_CONDITION_SYMBOL() As String = {"", "=", "LIKE"}
		'      Public NUMBER_CONDITION_TEXT() As String = {"[None]", "EQUAL TO", "NOT EQUAL TO", "GREATER THAN", "LESS THAN", "GREATER THAN OR EQUAL", "LESS THAN OR EQUAL"}
		'      Public NUMBER_CONDITION_SYMBOL() As String = {"", "=", "<>", ">", "<", ">=", "<="}
		'      Public CHECKBOX_CONDITION_TEXT() As String = {"[None]", "SELECTED", "NOT SELECTED"}
		'      Public CHECKBOX_CONDITION_SYMBOL() As String = {"", "= ", "="}
		'      Public QUESTION_CONDITION_TEXT() As String = {"[None]", "YES", "NO"}
		'      Public QUESTION_CONDITION_SYMBOL() As String = {"", "= ", "="}
		'      Public DDLIST_CONDITION_TEXT() As String = {"EQUAL TO", "NOT EQUAL TO"}
		'      Public DDLIST_CONDITION_SYMBOL() As String = {"=", "<>"}
		'      Public LINK_CONDITION_TEXT() As String = {"NONE", "AND", "OR"}
		'      Public LINK_CONDITION_SYMBOL() As String = {"", "AND", "OR"}

		'      Public Structure CriteriaElements
		'          Friend SQL_Text As String
		'          Friend ConditionField As String
		'          Friend ConditionType As String
		'          Friend ConditionValue As String
		'          Friend LinkValue As String
		'          Friend LinkSequence As Integer
		'      End Structure

		'Dim FWDb As New cFWDBConnection
		'Dim strSQL As String
		'Dim JoinSequence() As String = {"[contract_details]", "[contract_forecastdetails]", "[recharge_associations]", "[contract_productdetails]", "[contract_productdetails_recharge]", "[contract_productplatforms]", "[contract_producthistory]", "[savings]", _
		'	"[contract_notification]", "[codes_contractcategory]", "[codes_contracttype]", "[codes_currency]", "[codes_contractstatus]", "[codes_sites]", "[codes_rechargeentity]", _
		'	"[codes_inflatormetrics]", "[codes_invoicefrequencytype]", "[codes_licencerenewaltype]", _
		'	"[codes_notecategory]", "[codes_platformtype]", "[codes_salestax]", "[codes_termtype]", "[codes_units]", "[sublocations]", "[contract_forecastproducts]", _
		'	"[codes_sites]", "[codes_productcategory]", _
		'	"[vendor_categories]", "[vendor_status]", "[vendor_addresses]", "[product_vendors]", "[product_details]", "[invoice_details]", "[codes_invoicestatustype]", "[invoice_log]", "[invoice_productdetails]", "[vendor_details]", _
		'	"[version_registry]", "[version_history]", "[staff_details]", _
		'	"[contract_details]", "[contract_notes]", "[product_notes]", "[invoice_notes]", "[vendor_notes]", "[vendorcontact_notes]"}

		'Public Function ConstructSQL(ByVal ReportName As String, ByVal repType As ApplicationProperties.ReportType, ByVal UserInfo As UserInfo, ByVal baseTable As String, ByRef FWDb As cFWDBConnection, ByRef repfound As Boolean) As String
		'	Dim x As Integer
		'	Dim tmpStr As String = ""
		'	Dim firstpart, firstpartCriteria As Boolean
		'	Dim tmpCriteriaSQL As String = ""
		'	Dim tmpLink As String = ""
		'	Dim dsetRow As DataRow
		'	Dim CriteriaList As New System.Collections.ArrayList
		'	Dim CriteriaIdx As Integer = 0
		'	Dim tmpSQL As New System.Text.StringBuilder

		'	If ReportName = "" Or ReportName = "DefaultReport" Then
		'		Select Case repType
		'			Case ReportType.[GLOBAL]
		'				FWDb.FWDb("R", "saved_reports", "ReportName", "DefaultReport", "ReportType", repType, "Location Id", UserInfo.ActiveLocation)
		'			Case ReportType.PERSONAL
		'				FWDb.FWDb("R", "saved_reports", "ReportName", "DefaultReport", "ReportType", repType, "UserId", UserInfo.UserId, "Location Id", UserInfo.ActiveLocation)
		'		End Select

		'		If FWDb.FWDbFlag = False Then
		'			' A Default report does not exist for them in the table

		'			' Create base report
		'			For x = 3 To 4
		'				FWDb.SetFieldValue("FieldID", x, "N", True)	' Contract Number
		'				FWDb.SetFieldValue("Display", -1, "N", False)
		'				FWDb.SetFieldValue("Location Id", UserInfo.ActiveLocation, "N", False)
		'				FWDb.SetFieldValue("RecordType", "F", "S", False) ' tag record as a Field Definition
		'				FWDb.SetFieldValue("Report Order", x - 2, "N", False)

		'				Select Case x
		'					Case 3
		'						tmpStr = "[contract_details].[Contract Number]"
		'						FWDb.SetFieldValue("HeaderText", "Contract Number", "S", False)
		'					Case 4
		'						tmpStr = "[contract_details].[Contract Description]"
		'						FWDb.SetFieldValue("HeaderText", "Contract Description", "S", False)
		'					Case Else
		'						tmpStr = ""
		'				End Select

		'				FWDb.SetFieldValue("SQLText", tmpStr, "S", False)

		'				FWDb.SetFieldValue("SubTotal", 0, "N", False)
		'				FWDb.SetFieldValue("UserId", UserInfo.UserId, "N", False)
		'				FWDb.SetFieldValue("ReportName", "DefaultReport", "S", False)
		'				FWDb.SetFieldValue("ColumnWidth", 0, "N", False)
		'				FWDb.SetFieldValue("Sort", "N0", "S", False) ' N = None, zero for sort order seed - doesn't say NO!!
		'				FWDb.SetFieldValue("ReportType", 0, "N", False)	' 0 = Personal Report, 1 = Global Report
		'				FWDb.SetFieldValue("isIndirect", 0, "N", False)
		'				FWDb.FWDb("W", "saved_reports", "", "", "", "", "", "", "", "", "", "", "", "")
		'			Next

		'			' Add in the default ID field
		'			FWDb.SetFieldValue("FieldID", 1, "N", True)	' Contract Id
		'			FWDb.SetFieldValue("Display", 0, "N", False)
		'			FWDb.SetFieldValue("Location Id", UserInfo.ActiveLocation, "N", False)
		'			FWDb.SetFieldValue("RecordType", "I", "S", False) ' tag record as a Field Definition
		'			FWDb.SetFieldValue("Report Order", 3, "N", False)
		'			FWDb.SetFieldValue("HeaderText", "ID", "S", False)
		'			FWDb.SetFieldValue("SQLText", "[contract_details].[Contract Id]", "S", False)
		'			FWDb.SetFieldValue("SubTotal", 0, "N", False)
		'			FWDb.SetFieldValue("UserId", UserInfo.UserId, "N", False)
		'			FWDb.SetFieldValue("ReportName", "DefaultReport", "S", False)
		'			FWDb.SetFieldValue("ColumnWidth", 0, "N", False)
		'			FWDb.SetFieldValue("Sort", "N0", "S", False) ' N = None, zero for sort order seed - doesn't say NO!!
		'			FWDb.SetFieldValue("ReportType", 0, "N", False)	' 0 = Personal Report, 1 = Global Report
		'			FWDb.SetFieldValue("isIndirect", 0, "N", False)
		'			FWDb.FWDb("W", "saved_reports", "", "", "", "", "", "", "", "", "", "", "", "")

		'			' add in the default criteria line (Archive = 'N')
		'			FWDb.SetFieldValue("FieldID", 5, "N", True)
		'			FWDb.SetFieldValue("Display", 0, "N", False)
		'			FWDb.SetFieldValue("Location Id", UserInfo.ActiveLocation, "N", False)
		'			FWDb.SetFieldValue("RecordType", "C", "S", False)
		'			FWDb.SetFieldValue("Report Order", 0, "N", False)
		'			FWDb.SetFieldValue("HeaderText", "N", "S", False)
		'			FWDb.SetFieldValue("LinkValue", "", "S", False)
		'			FWDb.SetFieldValue("Condition", "=", "S", False)
		'			FWDb.SetFieldValue("SQLText", "[contract_details].[Archived] = 'N'", "S", False)
		'			FWDb.SetFieldValue("ReportName", "DefaultReport", "S", False)
		'			FWDb.SetFieldValue("ReportType", 0, "N", False)
		'			FWDb.SetFieldValue("UserId", UserInfo.UserId, "N", False)
		'			FWDb.FWDb("W", "saved_reports", "", "", "", "", "", "", "", "", "", "", "", "")

		'			repfound = False
		'		Else
		'			repfound = True
		'		End If
		'	End If

		'	Select Case repType
		'		Case ReportType.[GLOBAL]
		'			strSQL = "SELECT * FROM [saved_reports] WHERE [ReportName] = '" & ReportName & "' AND [Location Id] = " & UserInfo.ActiveLocation.ToString.Trim & " AND [ReportType] = " & ReportType.[GLOBAL] & " ORDER BY [Report Order],[LinkValue]"
		'		Case ReportType.PERSONAL
		'			strSQL = "SELECT * FROM [saved_reports] WHERE [ReportName] = '" & ReportName & "' AND [Location Id] = " & UserInfo.ActiveLocation.ToString.Trim & " AND [UserId] = " & UserInfo.UserId.ToString.Trim & " AND [ReportType] = " & ReportType.PERSONAL & " ORDER BY [Report Order],[LinkValue]"
		'		Case Else

		'	End Select

		'	FWDb.RunSQL(strSQL, FWDb.glDBWorkD, False, "", False)

		'	If FWDb.GetRowCount(FWDb.glDBWorkD, 0) = 0 Then
		'		ConstructSQL = ""
		'		repfound = False
		'		Exit Function
		'	Else
		'		repfound = True
		'	End If

		'	firstpart = True
		'	firstpartCriteria = True
		'	tmpSQL.Append("SELECT ")

		'	If baseTable = "[contract_details]" Then
		'		tmpSQL.Append("[Archived] AS [isArchived], ")
		'	End If

		'	CriteriaIdx = 0

		'	For Each dsetRow In FWDb.glDBWorkD.Tables(0).Rows
		'		Select Case dsetRow.Item("RecordType")
		'			Case "F", "I" ' Field Definition
		'				If firstpart = False Then
		'					tmpSQL.Append("," & vbNewLine)
		'				End If

		'				' Don't include the ID fields in the fields to be displayed
		'				If dsetRow.Item("Display") < 1 Then
		'					tmpSQL.Append(Trim(dsetRow.Item("SQLText")))
		'					tmpSQL.Append(" AS [")
		'					tmpSQL.Append(Trim(dsetRow.Item("HeaderText")))
		'					tmpSQL.Append("]")
		'					firstpart = False
		'				Else
		'					' Prevent another comma being used after ignored field
		'					firstpart = True
		'				End If
		'			Case "C", "V" ' Criteria definition
		'				If firstpartCriteria Then
		'					tmpLink = ""
		'				Else
		'					If Not IsDBNull(dsetRow.Item("LinkValue")) Then
		'						tmpLink = dsetRow.Item("LinkValue")
		'						'tmpCriteriaSQL = tmpCriteriaSQL & vbNewLine & tmpLink
		'					End If
		'				End If

		'				'tmpCriteriaSQL = tmpCriteriaSQL & Trim(dsetRow.Item("SQLText"))
		'				CriteriaIdx += 1

		'				Dim CLitem As CriteriaElements
		'				With CLitem
		'					.SQL_Text = CStr(dsetRow.Item("SQLText")).Trim
		'					.LinkValue = tmpLink.Trim
		'					FWDb.FWDb("R3", "system_dbjoins", "FieldID", dsetRow.Item("FieldID"), "", "", "", "", "", "", "", "", "", "")
		'					If FWDb.FWDb3Flag = True Then
		'						.ConditionField = FWDb.FWDbFindVal("Field", 3).Trim
		'					End If
		'					.ConditionType = CStr(dsetRow.Item("Condition")).Trim
		'					.ConditionValue = CStr(dsetRow.Item("HeaderText")).Trim
		'					.LinkSequence = CriteriaIdx
		'				End With
		'				CriteriaList.Add(CLitem)

		'				firstpartCriteria = False
		'			Case "M"
		'				' calculated Maintenance fields
		'				If firstpart = False Then
		'					tmpSQL.Append("," & vbNewLine)
		'				End If

		'				tmpSQL.Append("0 AS [")
		'				tmpSQL.Append(Trim(dsetRow.Item("HeaderText")))
		'				tmpSQL.Append("]")
		'				firstpart = False
		'			Case Else
		'		End Select
		'	Next

		'	'Dim ce As CriteriaElements
		'	'System.Diagnostics.Debug.WriteLine("*** BEFORE SORT ***")
		'	'For x = 0 To CriteriaList.Count - 1
		'	'    ce = CriteriaList(x)

		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " = " & ce.ConditionField & " " & ce.ConditionType & " " & ce.ConditionValue)
		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " LinkValue = " & ce.LinkValue)
		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " LinkSequence = " & ce.LinkSequence.ToString)
		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " SQL = " & ce.SQL_Text)
		'	'Next

		'	sortCriteria(CriteriaList)

		'	'System.Diagnostics.Debug.WriteLine("*** AFTER SORT ***")
		'	'For x = 0 To CriteriaList.Count - 1
		'	'    ce = CriteriaList(x)

		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " = " & ce.ConditionField & " " & ce.ConditionType & " " & ce.ConditionValue)
		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " LinkValue = " & ce.LinkValue)
		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " LinkSequence = " & ce.LinkSequence.ToString)
		'	'    System.Diagnostics.Debug.WriteLine(x.ToString & " SQL = " & ce.SQL_Text)
		'	'Next

		'	For x = 0 To CriteriaList.Count - 1
		'		Dim CL_item As CriteriaElements
		'		CL_item = CriteriaList(x)

		'		If x = 0 Then
		'			If CL_item.LinkValue <> "" Then
		'				' first criteria element doesn't need a link
		'				CL_item.LinkValue = ""
		'			End If
		'		Else
		'			If CL_item.LinkValue = "" Then
		'				' any subsequent blank link value, assume to be AND
		'				CL_item.LinkValue = "AND"
		'			End If
		'		End If

		'		tmpCriteriaSQL += " " & CL_item.LinkValue & " " & CL_item.SQL_Text
		'	Next

		'	tmpSQL.Append(" FROM ")
		'	tmpSQL.Append(baseTable.Trim & vbNewLine)

		'	Dim tmpWHERE As String = "WHERE"

		'	tmpSQL.Append(GetJoins(FWDb, baseTable))

		'	If tmpCriteriaSQL <> "" Then
		'		tmpSQL.Append(tmpWHERE)
		'		tmpSQL.Append(Replace(tmpCriteriaSQL, "`", "'"))
		'		tmpWHERE = ""
		'	End If

		'	Select Case baseTable
		'		Case "[contract_productdetails_recharge]"

		'		Case "[vendor_details]", "[product_details]", "[staff_details]"
		'			If tmpCriteriaSQL <> "" Then
		'				tmpSQL.Append(" AND ")
		'			Else
		'				tmpSQL.Append(tmpWHERE)
		'				tmpWHERE = ""
		'			End If

		'			tmpSQL.Append(baseTable.Trim)
		'			tmpSQL.Append(".")
		'			tmpSQL.Append("[Location Id] = ")
		'			tmpSQL.Append(UserInfo.ActiveLocation.ToString.Trim)

		'		Case Else
		'			If tmpCriteriaSQL <> "" Then
		'				tmpSQL.Append(" AND ")
		'			Else
		'				tmpSQL.Append(tmpWHERE)
		'				tmpWHERE = ""
		'			End If


		'			tmpSQL.Append(baseTable.Trim)
		'			tmpSQL.Append(".")
		'			tmpSQL.Append("[Location Id] = ")
		'			tmpSQL.Append(UserInfo.ActiveLocation.ToString.Trim)
		'			tmpSQL.Append(" AND dbo.CheckContractAccess(" & UserInfo.UserId.ToString & ",[contract_details].[Contract Id]) > 0 ")
		'			'tmpSQL.Append(" AND dbo.IsVariation([contract_details].[Contract Id]) = 0 AND dbo.CheckContractAccess(" & UserInfo.UserId.ToString & ",[contract_details].[Contract Id]) > 0 ")
		'	End Select

		'	ConstructSQL = tmpSQL.ToString
		'End Function

		'Private Function GetJoins(ByRef FWDb As cFWDBConnection, ByVal baseTable As String) As String
		'	Dim dsetRow As DataRow
		'	Dim pos, TableIdx As Integer
		'	Dim tmpTable, tmpField, tmpStr As String
		'	Dim tmpSQLText As String
		'	Dim fromTableList(20), toTableList(20), joinSQL(20) As String

		'	tmpStr = ""
		'	TableIdx = 1

		'	' Create JOINS for each table other than the baseTable
		'	For Each dsetRow In FWDb.glDBWorkD.Tables(0).Rows
		'		pos = InStr(dsetRow.Item("SQLText"), ".", CompareMethod.Text)
		'		tmpSQLText = dsetRow.Item("SQLText")
		'		If pos > 0 Then
		'			tmpField = Mid(tmpSQLText, pos + 1)
		'			tmpTable = Left(tmpSQLText, pos - 1)

		'			If dsetRow.Item("Display") > 0 Then
		'				FWDb.FWDb("R", "system_dbjoins", "Table", tmpTable, "Field", tmpField, "", "", "", "", "", "", "", "")
		'				If FWDb.FWDbFlag = True Then
		'					If FWDb.FWDbFindVal("JoinTable", 1) <> baseTable And Not Check4Join(fromTableList, toTableList, tmpTable, FWDb.FWDbFindVal("JoinTable", 1)) Then
		'						' different table, so compile the join
		'						fromTableList(TableIdx) = tmpTable
		'						toTableList(TableIdx) = FWDb.FWDbFindVal("JoinTable", 1)

		'						joinSQL(TableIdx) = FWDb.FWDbFindVal("JoinType", 1) & " " & FWDb.FWDbFindVal("JoinTable", 1) & " ON " & tmpTable & "." & tmpField & " = " & FWDb.FWDbFindVal("JoinTable", 1) & "." & FWDb.FWDbFindVal("JoinField", 1) & vbNewLine
		'						TableIdx = TableIdx + 1
		'					End If

		'					If tmpTable <> baseTable And Not Check4Join(fromTableList, toTableList, tmpTable, baseTable) Then
		'						' create join between this table and the base table if different
		'						FWDb.FWDb("R", "system_dbjoins", "Table", tmpTable, "JoinTable", baseTable, "", "", "", "", "", "", "", "")
		'						If FWDb.FWDbFlag = True Then
		'							fromTableList(TableIdx) = FWDb.FWDbFindVal("Table", 1)
		'							toTableList(TableIdx) = FWDb.FWDbFindVal("JoinTable", 1)

		'							joinSQL(TableIdx) = FWDb.FWDbFindVal("JoinType", 1) & " " & FWDb.FWDbFindVal("Table", 1) & " ON " & FWDb.FWDbFindVal("Table", 1) & "." & FWDb.FWDbFindVal("Field", 1) & " = " & FWDb.FWDbFindVal("JoinTable", 1) & "." & FWDb.FWDbFindVal("JoinField", 1) & vbNewLine
		'							TableIdx = TableIdx + 1
		'						End If
		'					End If
		'				Else
		'					' not a valid table name, check if the table is from a JoinAlias
		'					FWDb.FWDb("R", "system_dbjoins", "JoinAlias", tmpTable, "Field", tmpField, "", "", "", "", "", "", "", "")
		'					If FWDb.FWDbFlag = True Then
		'						If FWDb.FWDbFindVal("JoinTable", 1) <> baseTable And Not Check4Join(fromTableList, toTableList, tmpTable, FWDb.FWDbFindVal("JoinTable", 1)) Then
		'							' different table, so compile the join
		'							fromTableList(TableIdx) = tmpTable
		'							toTableList(TableIdx) = FWDb.FWDbFindVal("JoinAlias", 1)

		'							joinSQL(TableIdx) = FWDb.FWDbFindVal("JoinType", 1) & " " & FWDb.FWDbFindVal("JoinTable", 1) & " AS " & FWDb.FWDbFindVal("JoinAlias", 1) & " ON " & FWDb.FWDbFindVal("Table", 1) & "." & tmpField & " = " & FWDb.FWDbFindVal("JoinAlias", 1) & "." & FWDb.FWDbFindVal("JoinField", 1) & vbNewLine
		'							TableIdx = TableIdx + 1
		'						End If
		'					Else
		'						If tmpTable <> baseTable And Not Check4Join(fromTableList, toTableList, tmpTable, baseTable) Then
		'							' create join between this table and the base table if different
		'							FWDb.FWDb("R", "system_dbjoins", "Table", tmpTable, "JoinTable", baseTable, "", "", "", "", "", "", "", "")
		'							If FWDb.FWDbFlag = True Then
		'								fromTableList(TableIdx) = FWDb.FWDbFindVal("Table", 1)
		'								toTableList(TableIdx) = FWDb.FWDbFindVal("JoinAlias", 1)

		'								joinSQL(TableIdx) = FWDb.FWDbFindVal("JoinType", 1) & " " & FWDb.FWDbFindVal("Table", 1) & " AS " & FWDb.FWDbFindVal("JoinAlias", 1) & " ON " & FWDb.FWDbFindVal("Table", 1) & "." & FWDb.FWDbFindVal("Field", 1) & " = " & FWDb.FWDbFindVal("JoinTable", 1) & "." & FWDb.FWDbFindVal("JoinField", 1) & vbNewLine
		'								TableIdx = TableIdx + 1
		'							End If
		'						End If
		'					End If
		'				End If
		'			End If

		'			' ensure that join from base table not required.
		'			' eg. [Contract - Product Details]->[Product Details] created above
		'			' still requires join from [Contract Details]->[Contract - Product Details]
		'			If tmpTable <> baseTable And Not Check4Join(fromTableList, toTableList, tmpTable, baseTable) Then
		'				' create join between this table and the base table if different
		'				FWDb.FWDb("R", "system_dbjoins", "Table", tmpTable, "JoinTable", baseTable, "", "", "", "", "", "", "", "")
		'				If FWDb.FWDbFlag = True Then
		'					fromTableList(TableIdx) = FWDb.FWDbFindVal("Table", 1)
		'					toTableList(TableIdx) = FWDb.FWDbFindVal("JoinTable", 1)

		'					joinSQL(TableIdx) = FWDb.FWDbFindVal("JoinType", 1) & " " & FWDb.FWDbFindVal("Table", 1) & " ON " & FWDb.FWDbFindVal("Table", 1) & "." & FWDb.FWDbFindVal("Field", 1) & " = " & FWDb.FWDbFindVal("JoinTable", 1) & "." & FWDb.FWDbFindVal("JoinField", 1) & vbNewLine
		'					TableIdx = TableIdx + 1
		'				End If
		'			End If
		'		End If
		'	Next

		'	tmpStr = SortJoins(joinSQL)
		'	GetJoins = tmpStr
		'End Function

		'Private Function SortJoins(ByVal arrSQL() As String) As String
		'	Dim tmpStr As String = ""
		'	Dim tmpTable As String
		'	Dim x, y, pos, pos2, IdxToMove, SeqIdx As Integer
		'	Dim allmoved As Boolean

		'	allmoved = False

		'	Do While Not allmoved
		'		SeqIdx = 999
		'		For x = 1 To UBound(arrSQL)
		'			If arrSQL(x) <> "" Then

		'				pos = InStr(arrSQL(x), "AS ", CompareMethod.Text)
		'				If pos > 0 Then
		'					' must be a JoinAlias
		'					'pos += 3 ' add 3 to pass the "AS"
		'					pos = InStr(arrSQL(x), "JOIN ", CompareMethod.Text) + 5
		'					pos2 = InStr(arrSQL(x), " AS", CompareMethod.Text)
		'					'                        tmpTable = Mid(arrSQL(x), pos, pos2 - (pos - 1))
		'					tmpTable = Mid(arrSQL(x), pos, pos2 - pos)
		'				Else
		'					pos = InStr(arrSQL(x), "JOIN ", CompareMethod.Text) + 5
		'					pos2 = InStr(arrSQL(x), " ON", CompareMethod.Text)
		'					tmpTable = Mid(arrSQL(x), pos, pos2 - pos)
		'				End If

		'				For y = 1 To UBound(JoinSequence)
		'					If tmpTable = JoinSequence(y) Then
		'						If y < SeqIdx Then
		'							SeqIdx = y
		'							IdxToMove = x
		'						End If
		'						Exit For
		'					End If
		'				Next
		'			End If
		'		Next

		'		If SeqIdx = 999 Then
		'			' no further matches
		'			allmoved = True
		'		Else
		'			tmpStr = tmpStr & arrSQL(IdxToMove)
		'			arrSQL(IdxToMove) = ""
		'		End If
		'	Loop

		'	SortJoins = tmpStr
		'End Function

		'Private Function Check4Join(ByRef arr1() As String, ByRef arr2() As String, ByVal fromTable As String, ByVal toTable As String) As Boolean
		'	Dim matched As Boolean
		'	Dim x As Integer

		'	matched = False
		'	x = 1

		'	' ensure that a join between table1 and table2 has not already been established
		'	Do While Not matched And arr1(x) <> ""
		'		If arr1(x) = fromTable Then
		'			If arr2(x) = toTable Then
		'				matched = True
		'			End If
		'		End If

		'		If arr1(x) = toTable Then
		'			If arr2(x) = fromTable Then
		'				matched = True
		'			End If
		'		End If

		'		x = x + 1
		'	Loop

		'	Check4Join = matched
		'End Function


		'Public Function DisplayCheck(ByVal group As String, ByVal repBase As String, ByVal useRecharge As Boolean) As Boolean
		'	Dim hidefields As String

		'	Select Case repBase
		'		Case "[contract_details]"
		'			hidefields = "PDVDVPSD" & "RSRIRESR" ' & IIf(useRecharge, "", "RSRIRESR")
		'		Case "[product_details]"
		'			hidefields = "CDCPVDVPSDIFVBRSRIRECSSR"
		'		Case "[vendor_details]"
		'			hidefields = "CDCPPDSDIFIDVBVRVHRSRIRECSSR"
		'		Case "[staff_details]"
		'			hidefields = "CDCPPDVDIFIDVRVBVPVHRSRIRECSSR"
		'		Case "[contract_productdetails_recharge]"
		'			hidefields = "CDCPPDVDIFIDVRVBVPVHCS"
		'		Case Else
		'			hidefields = ""
		'	End Select

		'	If InStr(hidefields, group, CompareMethod.Text) = 0 Then
		'		DisplayCheck = False
		'	Else
		'		DisplayCheck = True
		'	End If
		'End Function

		'Public Function GetReportVariables(ByVal db As cFWDBConnection, ByVal userinfo As UserInfo, ByVal repName As String, ByVal repType As ReportType, ByVal NumVars As Integer) As String
		'	Dim sql As String = ""
		'	Dim x As Integer
		'	Dim drow, ddl_Row As DataRow
		'	Dim varSeq As String = ""
		'	Dim varSep As String = ""
		'	Dim tmpStr As String = ""
		'	Dim isDDList, isBetweenCondition, isUserDDList As Boolean
		'	Dim strHTML As New System.Text.StringBuilder

		'	strHTML.Append("<div class=""inputpaneltitle"">Report Variable for ")
		'	strHTML.Append(ConvertReportName(repName, ReportCnvDir.FROM_DB))
		'	strHTML.Append("</div>" & vbNewLine)
		'	strHTML.Append("<table>" & vbNewLine)

		'	sql = "SELECT * FROM [saved_reports] WHERE [ReportName] = @repName AND [ReportType] = @repType AND ([RecordType] = 'V' OR [RecordType] = 'C') AND [Location Id] = @locId"
		'	db.AddDBParam("repName", repName.Trim, True)
		'	db.AddDBParam("repType", repType)
		'	db.AddDBParam("locId", userinfo.ActiveLocation)
		'	db.RunSQL(sql, db.glDBWorkD)

		'	For Each drow In db.glDBWorkD.Tables(0).Rows
		'		isDDList = False
		'		isUserDDList = False

		'		db.FWDb("R2", "system_dbjoins", "FieldID", drow.Item("FieldID"))

		'		' decode the type of comparitor
		'		Select Case db.FWDbFindVal("FieldType", 2)
		'			Case "S", "T"
		'				For x = 0 To UBound(TEXT_CONDITION_TEXT)
		'					If TEXT_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'						tmpStr = TEXT_CONDITION_TEXT(x)
		'						Exit For
		'					End If
		'				Next
		'			Case "X"
		'				If Trim(drow.Item("HeaderText")) = "0" Then
		'					tmpStr = "IS " & CHECKBOX_CONDITION_TEXT(1)
		'				Else
		'					tmpStr = "IS " & CHECKBOX_CONDITION_TEXT(2)
		'				End If
		'			Case "Y"
		'				If Trim(drow.Item("HeaderText")) = "Y" Then
		'					tmpStr = "IS " & QUESTION_CONDITION_TEXT(1)
		'				Else
		'					tmpStr = "IS " & QUESTION_CONDITION_TEXT(2)
		'				End If
		'			Case "N"
		'				If db.FWDbFindVal("IsIDField", 2) = -1 Then
		'					isDDList = True
		'					For x = 0 To UBound(DDLIST_CONDITION_TEXT)
		'						If DDLIST_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'							tmpStr = DDLIST_CONDITION_TEXT(x)
		'							Exit For
		'						End If
		'					Next
		'				Else
		'					For x = 0 To UBound(NUMBER_CONDITION_TEXT)
		'						If NUMBER_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'							tmpStr = NUMBER_CONDITION_TEXT(x)
		'							Exit For
		'						End If
		'					Next
		'				End If
		'			Case "C", "F"
		'				For x = 0 To UBound(NUMBER_CONDITION_TEXT)
		'					If NUMBER_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'						tmpStr = NUMBER_CONDITION_TEXT(x)
		'						Exit For
		'					End If
		'				Next
		'			Case "D"
		'				For x = 0 To UBound(DATE_CONDITION_TEXT)
		'					If DATE_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'						tmpStr = DATE_CONDITION_TEXT(x)
		'						If tmpStr = "BETWEEN" Then
		'							isBetweenCondition = True
		'						End If
		'						Exit For
		'					End If
		'				Next
		'			Case "L" ' user defined ddlist (handled as a string field)
		'				isUserDDList = True
		'				For x = 0 To UBound(DDLIST_CONDITION_TEXT)
		'					If DDLIST_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'						tmpStr = DDLIST_CONDITION_TEXT(x)
		'						Exit For
		'					End If
		'				Next

		'			Case "R" ' reference lookup field type
		'				isDDList = True
		'				For x = 0 To UBound(DDLIST_CONDITION_TEXT)
		'					If DDLIST_CONDITION_SYMBOL(x) = Trim(drow.Item("Condition")) Then
		'						tmpStr = DDLIST_CONDITION_TEXT(x)
		'						Exit For
		'					End If
		'				Next

		'			Case Else
		'				tmpStr = ""
		'		End Select

		'		strHTML.Append("<tr>" & vbNewLine)
		'		strHTML.Append("<td class=""labeltd"">")
		'		strHTML.Append(db.FWDbFindVal("Description", 2))
		'		strHTML.Append("</td>" & vbNewLine)
		'		strHTML.Append("<td class=""inputtd"">")
		'		strHTML.Append(tmpStr)
		'		strHTML.Append("</td>" & vbNewLine)

		'		Dim strDDL_txtField, strDDL_idField As String

		'		Select Case drow.Item("RecordType")
		'			Case "C"
		'				' read only hard coded criteria field
		'				If IsDBNull(drow.Item("HeaderText")) = True Then
		'					tmpStr = "&nbsp;"
		'				Else
		'					Select Case db.FWDbFindVal("FieldType", 2)
		'						Case "N"
		'							If isDDList = True Then
		'								tmpStr = drow.Item("Description")
		'							End If
		'						Case Else
		'							tmpStr = drow.Item("HeaderText")
		'					End Select

		'				End If
		'				strHTML.Append("<td class=""inputtd"">")
		'				strHTML.Append(tmpStr)
		'				strHTML.Append("</td>" & vbNewLine)
		'				If IsDBNull(drow.Item("LinkValue")) = True Then
		'					tmpStr = "&nbsp;"
		'				Else
		'					tmpStr = drow.Item("LinkValue")
		'				End If
		'				strHTML.Append("<td class=""inputtd"">")
		'				strHTML.Append(tmpStr)
		'				strHTML.Append("</td>" & vbNewLine)

		'			Case "V"
		'				' variable field requiring user input
		'				If isUserDDList = True Then
		'					sql = "SELECT [Field Value] FROM [user_fieldvalues] WHERE [User Field Id] = " & Trim(Str(Val(db.FWDbFindVal("FieldID", 2)) - 10000)) & " ORDER BY [Field Value]"
		'					db.RunSQL(sql, db.glDBWorkI)

		'					Dim strDDL As New System.Text.StringBuilder

		'					strDDL.Append("<select tabindex=""" & Trim(drow.Item("Report Order")) & """ name=""V" & Trim(drow.Item("Report Order")) & """>" & vbNewLine)

		'					strDDL_txtField = "Field Value"
		'					strDDL_idField = "Field Value"

		'					strDDL.Append("<option value="""" selected>Please Select</OPTION>" & vbNewLine)

		'					For Each ddl_Row In db.glDBWorkI.Tables(0).Rows
		'						strDDL.Append("<option value=""" & Trim(ddl_Row.Item(strDDL_idField)) & """>")
		'						strDDL.Append(Trim(ddl_Row.Item(strDDL_txtField)))
		'						strDDL.Append("</option>" & vbNewLine)
		'					Next
		'					strDDL.Append("</select>" & vbNewLine)
		'					tmpStr = strDDL.ToString
		'				Else
		'					If isDDList = True Then
		'						strDDL_idField = db.FWDbFindVal("JoinField", 2)

		'						' populate a drop down list with available options
		'						db.FWDb("R3", "system_dbjoins", "FieldID", db.FWDbFindVal("Display", 2))
		'						If db.FWDb3Flag = True Then
		'							strDDL_txtField = db.FWDbFindVal("Field", 3)
		'						Else
		'							Exit Select
		'						End If

		'						' if the criteria is for ref:sites type field, use two fields for ddlist
		'						If db.FWDbFindVal("JoinTable", 2) = "[codes_sites]" Then
		'							sql = "SELECT " & strDDL_idField & ",(" & strDDL_txtField & " + ' : ' + [Site Description]) AS " & strDDL_txtField & " FROM " & db.FWDbFindVal("JoinTable", 2)
		'						Else
		'							sql = "SELECT " & strDDL_idField & "," & strDDL_txtField & " FROM " & db.FWDbFindVal("JoinTable", 2)
		'						End If

		'						If hasLocationId(db.FWDbFindVal("JoinTable", 2)) Then
		'							sql = sql & vbNewLine & "WHERE [Location Id] = " & userinfo.ActiveLocation.ToString
		'						End If
		'						sql = sql & " ORDER BY " & strDDL_txtField
		'						db.RunSQL(sql, db.glDBWorkI)

		'						Dim strDDL As New System.Text.StringBuilder

		'						strDDL.Append("<select tabindex=""" & Trim(drow.Item("Report Order")) & """ name=""V" & Trim(drow.Item("Report Order")) & """>" & vbNewLine)

		'						strDDL_txtField = Replace(strDDL_txtField, "[", "")
		'						strDDL_txtField = Replace(strDDL_txtField, "]", "")
		'						strDDL_idField = Replace(strDDL_idField, "[", "")
		'						strDDL_idField = Replace(strDDL_idField, "]", "")

		'						strDDL.Append("<option value="""" selected>Please Select</option>" & vbNewLine)

		'						For Each ddl_Row In db.glDBWorkI.Tables(0).Rows
		'							strDDL.Append("<option value=""" & Trim(ddl_Row.Item(strDDL_idField)) & """>")
		'							strDDL.Append(Trim(ddl_Row.Item(strDDL_txtField)))
		'							strDDL.Append("</option>" & vbNewLine)
		'						Next
		'						strDDL.Append("</select>" & vbNewLine)

		'						tmpStr = strDDL.ToString
		'					Else
		'						If isBetweenCondition = True Then
		'							tmpStr = "<input tabindex=""" & Trim(drow.Item("Report Order")) & """ type=text name=""V" & Trim(drow.Item("Report Order")) & "a"" /> AND <INPUT class=""datatext"" tabindex=""" & Trim(drow.Item("Report Order")) & """ type=text name=""V" & Trim(drow.Item("Report Order")) & "b"" />" & vbNewLine
		'						Else
		'							tmpStr = "<input tabindex=""" & Trim(drow.Item("Report Order")) & """ type=text name=""V" & Trim(drow.Item("Report Order")) & """ />" & vbNewLine
		'						End If
		'					End If
		'				End If

		'				strHTML.Append("<td class=""inputtd"">")
		'				strHTML.Append(tmpStr)
		'				strHTML.Append("</td>" & vbNewLine)
		'				If IsDBNull(drow.Item("LinkValue")) = True Then
		'					tmpStr = "&nbsp;"
		'				Else
		'					tmpStr = drow.Item("LinkValue")
		'				End If
		'				strHTML.Append("<td class=""inputtd"">")
		'				strHTML.Append(tmpStr)
		'				strHTML.Append("</td>" & vbNewLine)

		'				' store a hidden record type
		'				strHTML.Append("<td><input type=hidden name=""T" & Trim(drow.Item("Report Order")) & """ value=""" & IIf(isBetweenCondition = True, "v", Trim(db.FWDbFindVal("FieldType", 2))) & """ /></td>" & vbNewLine)

		'				varSeq += varSep & drow.Item("Report Order")
		'				varSep = ","
		'		End Select

		'		strHTML.Append("</tr>" & vbNewLine)
		'	Next

		'	strHTML.Append("<tr><td colspan=""4""><input type=hidden name=""varSeq"" id=""varSeq"" value=""")
		'	strHTML.Append(varSeq.Trim)
		'	strHTML.Append(""" /></td></tr>" & vbNewLine)

		'	' error text box used for client-side input validation
		'	strHTML.Append("<tr><td colspan=""4"" id=""errortext""></td></tr>" & vbNewLine)

		'	' create a run report button that run javascript RunReport() function
		'	tmpStr = "FWMain.aspx?reptype=" & Val(repType).ToString.Trim & "&viewname=" & repName.Trim
		'	strHTML.Append("<tr><td colspan=""4""><a onmouseover=""window.status='Run report with criteria provided';return true;"" onmouseout=""window.status='Done';"" href=""javascript:RunReport('")
		'	strHTML.Append(tmpStr.Trim)
		'	strHTML.Append("');""><img src=""./buttons/ok.gif"" /></a></td></tr>" & vbNewLine)
		'	strHTML.Append("</table>" & vbNewLine)

		'	GetReportVariables = strHTML.ToString
		'End Function

		'Private Sub sortCriteria(ByRef list As System.Collections.ArrayList)
		'	' the criteria must be sorted to ensure that any OR statements for the same field are enclosed in
		'	' brackets, otherwise the query doesn't return correct results.
		'	' e.g. WHERE ([Vendor Id] = 3 OR [Vendor Id] = 5) AND [Location Id] = 0
		'	Dim x, numORs As Integer
		'	Dim sortComplete, openbracket As Boolean
		'	Dim curCE As CriteriaElements
		'	Dim nextCE As CriteriaElements
		'	Dim lastCE As Boolean
		'	Dim blankLinkValIdx As Integer = -1

		'	If list.Count <= 1 Then
		'		Exit Sub
		'	End If

		'	' if there are no ORs in the criteria then don't bother!
		'	numORs = 0
		'	For x = 0 To list.Count - 1
		'		curCE = list(x)

		'		If curCE.LinkValue = "OR" Then
		'			numORs += 1
		'		End If
		'	Next

		'	If numORs = 0 Then Exit Sub

		'	Do
		'		sortComplete = True
		'		For x = 0 To list.Count - 2
		'			curCE = list(x)
		'			nextCE = list(x + 1)

		'			If nextCE.ConditionField < curCE.ConditionField Then
		'				' switch the order
		'				list.RemoveRange(x, 2)
		'				list.Insert(x, curCE)
		'				list.Insert(x, nextCE)
		'				sortComplete = False
		'			End If
		'		Next
		'	Loop Until sortComplete = True

		'	' now in field order, check if the OR is for the same field!
		'	openbracket = False
		'	lastCE = False

		'	For x = 0 To list.Count - 1
		'		curCE = list(x)

		'		If curCE.LinkValue.Trim = "" Then
		'			blankLinkValIdx = x
		'		End If

		'		If x = (list.Count - 1) Then
		'			nextCE = Nothing
		'			lastCE = True
		'		Else
		'			nextCE = list(x + 1)
		'		End If

		'		If openbracket = True And lastCE = True Then
		'			curCE.SQL_Text = curCE.SQL_Text & ")"
		'			list(x) = curCE
		'		Else
		'			If openbracket = True Then
		'				If curCE.ConditionField <> nextCE.ConditionField Then
		'					' open bracket exists and the next field is not the same, so place close bracket
		'					curCE.SQL_Text = curCE.SQL_Text & ")"
		'					list(x) = curCE
		'					openbracket = False
		'				End If
		'			End If

		'			If curCE.ConditionField = nextCE.ConditionField Then
		'				If curCE.LinkValue = "OR" Or nextCE.LinkValue = "OR" Then
		'					' is the same field so put a starting bracket around the first SQL_text
		'					If openbracket = False Then
		'						curCE.SQL_Text = "(" & curCE.SQL_Text
		'						If curCE.LinkValue = "OR" And nextCE.LinkValue = "AND" Then
		'							' switch them around as the AND needs to be the first link value
		'							curCE.LinkValue = "AND"
		'							nextCE.LinkValue = "OR"
		'						End If
		'						list(x) = curCE
		'						list(x + 1) = nextCE
		'						openbracket = True
		'					End If
		'				End If
		'			End If
		'		End If
		'	Next

		'	If blankLinkValIdx <> -1 Then
		'		' make sure that after the sort, the first criteria item has no AND/OR and that the previously blank one has the AND/OR from the new first one
		'		curCE = list(blankLinkValIdx)
		'		nextCE = list(0)
		'		curCE.LinkValue = nextCE.LinkValue
		'		nextCE.LinkValue = ""

		'		'store them back into the collection
		'		list(0) = nextCE
		'		list(blankLinkValIdx) = curCE
		'	End If
		'End Sub

		'Public Function ConvertSQLDate(ByVal dbType As Integer, ByVal dateStr As String) As String
		'	Dim CD As Date

		'	If IsDate(dateStr) Then
		'		CD = CDate(dateStr)
		'		If dbType <> cFWDBConnection.DBConnectionType.None Then
		'			ConvertSQLDate = "CONVERT(DATETIME,'" & Format(CD, "yyyy-MM-dd hh:mm:ss") & "',120)"
		'		Else
		'			ConvertSQLDate = "'" & Format(CD, "yyyy-MM-dd hh:mm:ss") & "'"
		'		End If
		'	Else
		'		ConvertSQLDate = "NULL"
		'	End If
		'End Function

        Public Function RemoveCurrencyFormat(ByVal fmtStr As String) As Double
            Dim i As Integer

            For i = 1 To fmtStr.Length
                If Mid(fmtStr, i, 1) = "." Or IsNumeric(Mid(fmtStr, i, 1)) = True Then
                    ' ok
                Else
					fmtStr = Replace(fmtStr, Mid(fmtStr, i, 1), "#", False)
                End If
            Next
            fmtStr = Replace(fmtStr, "#", "")

            If IsNumeric(fmtStr) = True Then
                RemoveCurrencyFormat = Val(fmtStr)
            Else
                RemoveCurrencyFormat = 0
            End If
        End Function
    End Module
End Namespace
