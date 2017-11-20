Imports System.Collections
Imports System.Data
Imports SpendManagementLibrary
Imports FWBase
Imports FWClasses
Imports FWReportsLibrary

'Public Class cGrid
'    Dim columns As New ArrayList
'    Dim rows As New ArrayList
'    Private bHeaders, bFooters As Boolean
'    'System.Data.DataSet rcdst;
'    Dim tblview As DataView
'    Dim tableWidth As String = "0"
'    Private sAlign As String
'    Private sTableid As String
'    Private sTbodyid As String
'    Private sClass As String
'    Private sSort As String
'    Private sSortColumn As String
'    Private sSortDirection As String
'    Private sCurpage As String
'    Private sGridid As String = ""
'    Private idGColumn As cGridColumn
'    Private nLocationid As Integer
'    Private bAllowsorting As Boolean = True

'    Public Sub New(ByVal rcdsttemp As System.Data.DataSet, ByVal headers As Boolean, ByVal footers As Boolean)
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'        If rcdsttemp Is Nothing Then
'            Exit Sub
'        End If

'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(appinfo.User.Identity)
'        Dim uinfo As UserInfo = curUser.currentUser.userInfo
'        nLocationid = uinfo.ActiveLocation
'        bHeaders = headers
'        bFooters = footers
'        'rcdst = rcdsttemp;
'        tblview = rcdsttemp.Tables(0).DefaultView
'        getColumns(rcdsttemp)
'        getData()

'        sCurpage = appinfo.Request.Url.PathAndQuery
'        If Not appinfo.Request.QueryString("sortby") Is Nothing Then
'            sSort = appinfo.Request.QueryString("sort")
'            sSortColumn = appinfo.Request.QueryString("sortby")
'            sSortDirection = appinfo.Request.QueryString("direction")
'            sortData()
'        Else
'            sortData()
'        End If
'    End Sub

'    Public Sub New(ByVal rcdsttemp As System.Data.DataSet, ByVal headers As Boolean, ByVal footers As Boolean, ByVal gridid As String)
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'        If rcdsttemp Is Nothing Then
'            Return
'        End If
'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(appinfo.User.Identity)
'        Dim uinfo As UserInfo = curUser.currentUser.userInfo
'        nLocationid = uinfo.ActiveLocation
'        bHeaders = headers
'        bFooters = footers
'        'rcdst = rcdsttemp
'        tblview = rcdsttemp.Tables(0).DefaultView
'        getColumns(rcdsttemp)
'        getData()
'        sGridid = gridid
'        sCurpage = appinfo.Request.Url.PathAndQuery
'        If Not appinfo.Request.QueryString("sortby") Is Nothing Then
'            If appinfo.Request.QueryString("sort") = gridid Then
'                sSort = appinfo.Request.QueryString("sort")
'                sSortColumn = appinfo.Request.QueryString("sortby")
'                sSortDirection = appinfo.Request.QueryString("direction")
'                sortData()
'            Else
'                sortData()
'            End If
'        Else
'            sortData()
'        End If
'    End Sub

'    Public Sub New(ByVal rcdsttemp As System.Data.DataSet, ByVal headers As Boolean, ByVal footers As Boolean, ByVal gridid As String, ByVal defaultSort As String)
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'        If rcdsttemp Is Nothing Then
'            Return
'        End If
'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(appinfo.User.Identity)
'        Dim uinfo As UserInfo = curUser.currentUser.userInfo
'        nLocationid = uinfo.ActiveLocation
'        bHeaders = headers
'        bFooters = footers
'        'rcdst = rcdsttemp
'        tblview = rcdsttemp.Tables(0).DefaultView
'        getColumns(rcdsttemp)
'        getData()
'        sGridid = gridid
'        sCurpage = appinfo.Request.Url.PathAndQuery

'        If Not appinfo.Request.QueryString("sortby") Is Nothing Then
'            sSort = appinfo.Request.QueryString("sort")
'            sSortColumn = appinfo.Request.QueryString("sortby")
'            sSortDirection = appinfo.Request.QueryString("direction")
'            sortData()
'        ElseIf defaultSort <> "" Then
'            sSort = gridid
'            sSortColumn = defaultSort
'            sSortDirection = "asc"
'            sortData()
'        Else
'            sortData()
'        End If
'    End Sub

'    Private Sub sortData()
'        If gridid = sSort Then
'            Try
'                tblview.Sort = sortcolumn & " " & sortdirection
'            Catch exp As System.IndexOutOfRangeException

'            End Try
'        End If
'    End Sub

'    Public ReadOnly Property data() As DataView
'        Get
'            Return tblview
'        End Get
'    End Property

'    Private Sub getColumns(ByVal rcdsttemp As DataSet)
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(appinfo.User.Identity)
'        Dim uinfo As UserInfo = curUser.currentUser.userInfo
'        Dim fws As cFWSettings = curUser.UserFWS
'        Dim i As Integer
'        Dim locationid As Integer = uinfo.ActiveLocation
'        Dim description As String = ""
'        Dim tablecolumn As String
'		Dim clsfields As New cFields(uinfo, fws)
'		Dim reqfield As cField

'		Dim clsmisc As New cReportMisc
'		Dim employees As New cFWEmployees(fws, uinfo)
'		Dim clsuserdefined As New cUserDefinedFields(fws, uinfo, employees)
'        Dim column As cGridColumn

'        For i = 0 To tblview.Table.Columns.Count - 1
'            Try
'                reqfield = clsfields.getFieldByName(tblview.Table.Columns(i).ColumnName)
'				If reqfield Is Nothing Then
'					Dim fieldid As New Guid(tblview.Table.Columns(i).ColumnName)
'					reqfield = clsfields.getFieldById(fieldid)
'				End If

'                If reqfield Is Nothing Then 'userdefined?
'                    reqfield = clsuserdefined.GetUFReportField(Integer.Parse(rcdsttemp.Tables(0).Columns(i).ColumnName))
'                End If

'                ' get alternative description if field has been renamed
'                'Select Case reqfield.fieldid

'                'case else
'                description = reqfield.description

'                'end select
'                tablecolumn = reqfield.fieldid.ToString
'                column = New cGridColumn(description, reqfield.fieldtype, reqfield.comment, reqfield.cantotal, tablecolumn)
'            Catch
'                tablecolumn = rcdsttemp.Tables(0).Columns(i).ColumnName
'                column = New cGridColumn(rcdsttemp.Tables(0).Columns(i).ColumnName, "S", "", False, tablecolumn)
'            End Try
'            columns.Add(column)
'        Next
'    End Sub

'    Public Function CreateGrid() As String
'        Dim output As New System.Text.StringBuilder

'        output.Append("<table")
'        If tableid <> "" Then
'            output.Append(" id=" & tableid)
'        End If

'        If align <> "" Then
'            output.Append(" align=""" & align & """")
'        End If
'        '    output.Append(" align=""center""")
'        'Else
'        '    output.Append(" align=""" & align & """")
'        'End If

'        If tblclass <> "" Then
'            output.Append(" class=""" & tblclass & """")
'        End If

'        output.Append(" cellspacing=0")
'        If tableWidth <> "0" Then
'            output.Append(" width=""" & tableWidth & """")
'        End If

'        output.Append(">")
'        output.Append("<tbody")
'        If tbodyid <> "" Then
'            output.Append(" id=""" & tbodyid & """")
'        End If

'        output.Append(">")
'        If bHeaders Then
'            output.Append(generateHeader())
'        End If

'        output.Append(generateTable())
'        If bFooters Then
'            output.Append(generateFooter())
'        End If

'        output.Append("</tbody>")
'        output.Append("</table>")

'        Return output.ToString
'    End Function

'    Private Function generateHeader() As String
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'        Dim i As Integer
'        Dim reqcolumn As cGridColumn
'        Dim output As New System.Text.StringBuilder
'        output.Append("<tr>")
'        Dim colpage As String

'        For i = 0 To columns.Count - 1
'            reqcolumn = CType(columns(i), cGridColumn)
'            colpage = curpage
'            If reqcolumn.hidden = False Then
'                output.Append("<th")
'                If reqcolumn.width <> "" Then
'                    output.Append(" width=""" & reqcolumn.width & """")
'                End If

'                If reqcolumn.align <> "" Then
'                    output.Append(" align=""" & reqcolumn.align & """")
'                End If

'                output.Append(">")

'                If reqcolumn.customcolumn = False Then
'                    If colpage.IndexOf("?") <> -1 Then
'                        If colpage.IndexOf("sort") <> -1 Then
'                            colpage = colpage.Remove(colpage.IndexOf("sort") - 1, colpage.Length - colpage.IndexOf("sort") + 1)
'                            'colpage = colpage.Replace("sort=" + gridid + "&sortby=" + sortcolumn + "&direction=asc","");
'                            'colpage = colpage.Replace("sort=" + gridid + "&sortby=" + sortcolumn + "&direction=desc","");
'                        End If

'                        If colpage.IndexOf("?") <> -1 Then
'                            If colpage.IndexOf("?") <> (colpage.Length - 1) Then
'                                colpage += "&"
'                            End If
'                        Else
'                            colpage += "?"
'                        End If
'                    Else
'                        colpage += "?"
'                    End If

'                    colpage += "sort=" & gridid & "&sortby=" & reqcolumn.tablecolumn
'                    If reqcolumn.tablecolumn = sortcolumn Then
'                        If sortdirection = "asc" Then
'                            colpage += "&direction=desc"
'                        Else
'                            colpage += "&direction=asc"
'                        End If
'                    Else
'                        colpage += "&direction=asc"
'                    End If

'                    If allowsorting Then
'                        output.Append("<a href=""" & colpage & "")
'                        output.Append(""">")
'                    End If
'                End If

'                output.Append(reqcolumn.description)
'                If reqcolumn.tablecolumn = sortcolumn And reqcolumn.customcolumn = False Then
'                    If sortdirection = "asc" Then
'						output.Append("&nbsp;<img src=" & cReportMisc.path & "/images/whitearrow_up.gif>")
'                    Else
'						output.Append("&nbsp;<img src=" & cReportMisc.path & "/images/whitearrow_down.gif>")
'                    End If
'                End If

'                If reqcolumn.customcolumn = False Then
'                    output.Append("</a>")
'                End If
'                output.Append("</th>")
'            End If
'        Next

'        output.Append("</tr>")

'        Return output.ToString
'    End Function

'    'Public Sub saveDefaultSort(ByVal gridname As String, ByVal columnname As String, ByVal direction As String)
'    '    Dim sortorder As Byte
'    '    Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'	'    Dim fwdb As New cFWDBConnection
'    '    Dim uinfo As UserInfo = appinfo.Session("UserInfo")
'    '    Dim fws As cFWSettings = appinfo.Application("FWSettings")
'    '    Dim strsql As String

'    '    strsql = "delete from default_sorts where gridname = @gridname and employeeid = @employeeid"
'    '    fwdb.AddDBParam("employeeid", uinfo.UserId, True)
'    '    fwdb.AddDBParam("gridname", gridname)
'    '    fwdb.ExecuteSQL(strsql)

'    '    If direction.ToLower = "asc" Then
'    '        sortorder = 1
'    '    Else
'    '        sortorder = 2
'    '    End If

'    '    'strsql = "insert into default_sorts (employeeid, gridname, columnname, defaultorder) " & _
'    '    ' "values (@employeeid, @gridname, @columnname, @defaultorder)"
'    '    fwdb.SetFieldValue("columnname", columnname, "S", True)
'	'    fwdb.SetFieldValue("defaultorder", sortorder, "N", False)
'	'    fwdb.SetFieldValue("employeeid", uinfo.UserId, "N", False)
'	'    fwdb.SetFieldValue("gridname", gridname, "S", False)
'    '    fwdb.FWDb("W", "default_sorts")
'    '    'fwdb.ExecuteSQL(strsql)
'    'End Sub

'    Public Property width() As Integer
'        Get
'            Return tableWidth
'        End Get
'        Set(ByVal value As Integer)
'            tableWidth = value
'        End Set
'    End Property

'    Public Property allowsorting() As Boolean
'        Get
'            Return bAllowsorting
'        End Get
'        Set(ByVal value As Boolean)
'            bAllowsorting = value
'        End Set
'    End Property

'    Public Property align() As String
'        Get
'            Return sAlign
'        End Get
'        Set(ByVal value As String)
'            sAlign = value
'        End Set
'    End Property

'    Public Property tblclass() As String
'        Get
'            Return sClass
'        End Get
'        Set(ByVal value As String)
'            sClass = value
'        End Set
'    End Property

'    Public ReadOnly Property sortcolumn() As String
'        Get
'            Return sSortColumn
'        End Get
'    End Property

'    Private ReadOnly Property sortdirection() As String
'        Get
'            Return sSortDirection
'        End Get
'    End Property

'    Private Property curpage() As String
'        Get
'            Return sCurpage
'        End Get
'        Set(ByVal value As String)
'            sCurpage = value
'        End Set
'    End Property

'    Public ReadOnly Property gridid() As String
'        Get
'            Return sGridid
'        End Get
'    End Property

'    Private Function generateTable() As String
'        Dim i As Integer
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(appinfo.User.Identity)
'        Dim ui As UserInfo = curUser.currentUser.userInfo
'        Dim fws As cFWSettings = curUser.UserFWS
'		Dim clscurrencies As New cFWCurrencies(fws, ui)
'        Dim reqcurrency As cCurrencyItem
'        Dim rownum As String = "row1"
'        Dim x As Integer
'        Dim val As String
'        Dim clsrow As cGridRow
'        Dim reqcell As cGridCell
'        Dim output As New System.Text.StringBuilder

'        For i = 0 To rows.Count - 1
'            clsrow = CType(rows(i), cGridRow)
'            output.Append("<tr ")
'            If Not idColumn Is Nothing Then
'                output.Append("id=""" & clsrow.getCellByName(idColumn.name).thevalue & """")
'            End If

'            output.Append(">")

'            For x = 0 To clsrow.rowCells.Count - 1
'                reqcell = CType(clsrow.rowCells(x), cGridCell)

'                If reqcell.columninfo.hidden = False Then
'                    output.Append("<td class=""" & rownum & """ ")

'                    Select Case reqcell.columninfo.fieldtype
'                        Case "X", "Y"
'                            output.Append("align=""center""")
'                        Case "C", "M", "N", "A", "F"
'                            output.Append("align=""right""")
'                        Case Else
'                    End Select

'                    If reqcell.bgcolor <> "" Then
'                        output.Append(" style=""background-color:" & reqcell.bgcolor & ";""")
'                    End If

'                    output.Append(">")

'                    If reqcell.columninfo.listitems.count <> 0 Then
'                        If reqcell.columninfo.listitems.exists(reqcell.thevalue) = True Then
'                            output.Append(reqcell.columninfo.listitems.getValue(reqcell.thevalue))
'                        End If
'                    ElseIf reqcell.thevalue Is Nothing Or IsDBNull(reqcell.thevalue) Or reqcell.thevalue.ToString = "" Then
'                        output.Append("&nbsp;")
'                    Else
'                        val = reqcell.thevalue.ToString
'                        Select Case reqcell.columninfo.fieldtype
'                            Case "S", "N", "F"
'                                output.Append(reqcell.thevalue)

'                            Case "D"
'                                output.Append(DateTime.Parse(val).ToShortDateString)

'                            Case "T"
'                                output.Append(DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"))

'                            Case "X"
'                                output.Append("<input type=""checkbox"" disabled ")
'                                If CBool(reqcell.thevalue) = True Then
'                                    output.Append("checked")
'                                End If
'                                output.Append(">")

'                            Case "Y"
'                                output.Append("<input type=""checkbox"" disabled ")
'                                If reqcell.thevalue = "Y" Then
'                                    output.Append("checked")
'                                End If
'                                output.Append(">")

'                            Case "C", "A"
'                                Select Case reqcell.columninfo.name
'                                    Case "Global Total"
'                                        If Not clsrow.getCellByName("globalbasecurrency") Is Nothing Then
'                                            reqcurrency = clscurrencies.GetCurrencyById(CInt(clsrow.getCellByName("globalbasecurrency").thevalue))
'                                            output.Append(reqcurrency.CurrencySymbol)
'                                        End If

'                                    Case "Total Prior To Convert"
'                                        If Not clsrow.getCellByName("originalcurrency") Is Nothing Then
'                                            If IsDBNull(clsrow.getCellByName("originalcurrency").thevalue) = False Then
'                                                reqcurrency = clscurrencies.GetCurrencyById(CInt(clsrow.getCellByName("originalcurrency").thevalue))
'                                                output.Append(reqcurrency.CurrencySymbol)
'                                            End If
'                                        End If

'                                    Case Else
'                                        If Not clsrow.getCellByName("basecurrency") Is Nothing Then
'                                            If IsDBNull(clsrow.getCellByName("basecurrency").thevalue) = False Then
'                                                If CInt(clsrow.getCellByName("basecurrency").thevalue) <> 0 Then
'                                                    reqcurrency = clscurrencies.GetCurrencyById(CInt(clsrow.getCellByName("basecurrency").thevalue))
'                                                    output.Append(reqcurrency.CurrencySymbol)
'                                                Else
'                                                    output.Append("£")
'                                                End If
'                                            Else
'                                                output.Append("£")
'                                            End If
'                                        End If
'                                End Select
'                                output.Append(Decimal.Parse(val).ToString("#,###,##0.00"))

'                            Case "M"
'                                output.Append(Decimal.Parse(val).ToString("#,###,##0.00"))

'                            Case "DT"
'                                output.Append(DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"))
'                            Case Else
'                        End Select
'                    End If
'                    output.Append("</td>")
'                End If
'            Next

'            If rownum = "row1" Then
'                rownum = "row2"
'            Else
'                rownum = "row1"
'            End If
'            output.Append("</tr>")
'        Next
'        Return output.ToString
'    End Function

'    Public Function getFooterValues() As String
'        Dim i As Integer
'        Dim x As Integer
'        Dim total As Double = 0
'        Dim output As New System.Text.StringBuilder
'        Dim reqcolumn As cGridColumn
'        Dim reqrow As cGridRow
'        Dim reqcell As cGridCell

'        For i = 0 To columns.Count - 1
'            reqcolumn = CType(columns(i), cGridColumn)
'            If reqcolumn.hidden = False Then
'                If reqcolumn.cantotal = False Then
'                    output.Append(",")
'                Else
'                    total = 0
'                    For x = 0 To rows.Count - 1
'                        reqrow = CType(rows(x), cGridRow)
'                        reqcell = CType(reqrow.rowCells(i), cGridCell)
'                        total += Decimal.Parse(reqcell.thevalue.ToString)
'                    Next

'                    Select Case reqcolumn.fieldtype
'                        Case "C"
'                            output.Append(total.ToString("£#,###,##0.00"))

'                        Case "M"
'                            output.Append(total.ToString("#,###,##0.00"))

'                        Case "N"
'                            output.Append(Integer.Parse(total.ToString))
'                        Case "F"
'                            output.Append(Decimal.Parse(total.ToString))
'                    End Select
'                    output.Append(",")
'                End If
'            End If
'        Next

'        If output.Length <> 0 Then
'            output.Remove(output.Length - 1, 1)
'        End If
'        Return output.ToString
'    End Function

'    Public Function generateFooter() As String
'        Dim appinfo As System.Web.HttpApplication = CType(System.Web.HttpContext.Current.ApplicationInstance, System.Web.HttpApplication)
'        Dim i As Integer
'        Dim x As Integer
'        Dim total As Double = 0
'		Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(appinfo.User.Identity)
'        Dim ui As UserInfo = curUser.currentUser.userInfo
'        Dim fws As cFWSettings = curUser.UserFWS
'		Dim clscurrencies As New cFWCurrencies(fws, ui)
'        Dim reqcurrency As cCurrencyItem
'        Dim output As New System.Text.StringBuilder
'        Dim reqcolumn As cGridColumn
'        Dim reqrow As cGridRow
'        Dim reqcell As cGridCell
'        Dim symbol As String = ""

'        output.Append("<tr id=""footerRow"">")

'        For i = 0 To columns.Count - 1
'            reqcolumn = CType(columns(i), cGridColumn)
'            If reqcolumn.hidden = False Then
'                output.Append("<th ")
'                Select Case reqcolumn.fieldtype
'                    Case "C", "M", "N", "A", "F"
'                        output.Append("style=""text-align:right;""")

'                    Case Else
'                End Select

'                output.Append(">")
'                If reqcolumn.cantotal = False Then
'                    output.Append("&nbsp;")
'                Else
'                    total = 0
'                    For x = 0 To rows.Count - 1
'                        reqrow = CType(rows(x), cGridRow)
'                        reqcell = CType(reqrow.rowCells(i), cGridCell)
'                        If IsDBNull(reqcell.thevalue) = False Then
'                            total += Math.Round(Decimal.Parse(reqcell.thevalue.ToString), 2)
'                        End If

'                        Select Case reqcell.columninfo.name
'                            Case "Global Total"
'                                If Not reqrow.getCellByName("globalbasecurrency") Is Nothing Then
'                                    If IsDBNull(reqrow.getCellByName("Global Total").thevalue) = False Then
'                                        reqcurrency = clscurrencies.GetCurrencyById(CInt(reqrow.getCellByName("globalbasecurrency").thevalue))
'                                        symbol = reqcurrency.CurrencySymbol
'                                    End If
'                                End If

'                            Case "Total Prior To Convert"
'                                If Not reqrow.getCellByName("originalcurrency") Is Nothing Then
'                                    If IsDBNull(reqrow.getCellByName("originalcurrency").thevalue) = False Then
'                                        reqcurrency = clscurrencies.GetCurrencyById(CInt(reqrow.getCellByName("originalcurrency").thevalue))
'                                        symbol = reqcurrency.CurrencySymbol
'                                    End If
'                                End If

'                            Case Else
'                                If Not reqrow.getCellByName("basecurrency") Is Nothing Then
'                                    If IsDBNull(reqrow.getCellByName("basecurrency").thevalue) = False Then
'                                        reqcurrency = clscurrencies.GetCurrencyById(CInt(reqrow.getCellByName("basecurrency").thevalue))
'                                        If Not reqcurrency Is Nothing Then
'                                            symbol = reqcurrency.CurrencySymbol
'                                        End If
'                                    Else
'                                        symbol = "£"
'                                    End If
'                                End If
'                        End Select
'                    Next

'                    Select Case reqcolumn.fieldtype
'                        Case "C", "A"
'                            output.Append(symbol)
'                            output.Append(total.ToString("#,###,##0.00"))

'                        Case "M"
'                            output.Append(total.ToString("#,###,##0.00"))

'                        Case "N"
'                            output.Append(Integer.Parse(total.ToString))
'                        Case "F"
'                            output.Append(Decimal.Parse(total.ToString))
'                        Case Else
'                    End Select
'                End If
'                output.Append("</th>")
'            End If
'        Next
'        output.Append("</tr>")
'        Return output.ToString
'    End Function

'    Public Sub getData()
'        Dim i, x As Integer
'        Dim tblcol As Integer = 0
'        Dim cells As ArrayList
'        Dim clsrow As cGridRow
'        Dim clscell As cGridCell
'        Dim curcol As cGridColumn

'        rows.Clear()

'        For i = 0 To tblview.Count - 1
'            tblcol = 0
'            cells = New ArrayList
'            For x = 0 To columns.Count - 1

'                curcol = CType(columns(x), cGridColumn)
'                If curcol.customcolumn = False Then
'                    clscell = New cGridCell(curcol, tblview(i)(tblcol))
'                    tblcol += 1
'                Else
'                    clscell = New cGridCell(curcol, curcol.description)
'                End If

'                cells.Add(clscell)
'            Next
'            clsrow = New cGridRow(cells)
'            rows.Add(clsrow)
'        Next
'    End Sub

'    Public Function getColumn(ByVal description As String) As cGridColumn
'        Dim i As Integer
'        Dim reqcolumn As cGridColumn

'        For i = 0 To columns.Count - 1
'            reqcolumn = CType(columns(i), cGridColumn)

'            If reqcolumn.name = description Then
'                Return reqcolumn
'                Exit Function
'            End If
'        Next

'        Return Nothing
'    End Function

'#Region "properties"
'    Public ReadOnly Property gridcolumns() As ArrayList
'        Get
'            Return columns
'        End Get
'    End Property

'    Public ReadOnly Property gridrows() As ArrayList
'        Get
'            Return rows
'        End Get
'    End Property

'    Public Property tableid() As String
'        Get
'            Return sTableid
'        End Get
'        Set(ByVal value As String)
'            sTableid = value
'        End Set
'    End Property

'    Public Property tbodyid() As String
'        Get
'            Return sTbodyid
'        End Get
'        Set(ByVal value As String)
'            sTbodyid = value
'        End Set
'    End Property

'    Public Property idcolumn() As cGridColumn
'        Get
'            Return idGColumn
'        End Get
'        Set(ByVal value As cGridColumn)
'            idGColumn = value
'        End Set
'    End Property

'    Public ReadOnly Property locationid() As Integer
'        Get
'            Return nLocationid
'        End Get
'    End Property
'#End Region
'End Class

'Public Class cGridRow
'    Dim cells As New ArrayList

'    Public Sub New(ByVal thecells As ArrayList)
'        cells = thecells
'    End Sub

'    Public ReadOnly Property rowCells() As ArrayList
'        Get
'            Return cells
'        End Get
'    End Property

'    Public Function getCellByName(ByVal name As String) As cGridCell
'        Dim i As Integer
'        Dim reqcell As cGridCell

'        For i = 0 To cells.Count - 1
'            reqcell = CType(cells(i), cGridCell)
'            If reqcell.columninfo.name = name Then
'                Return reqcell
'                Exit Function
'            End If
'        Next
'        Return Nothing
'    End Function
'End Class

'Public Class cGridColumn

'    Private sDescription As String
'    Private sName As String
'    Private sFieldtype As String
'    Private sComment As String
'    Private bHidden As Boolean
'    Private bCustomColumn As Boolean
'    Private bCantotal As Boolean
'    Private valuelist As New cColumnList
'    Private sWidth As String
'    Private sAlign As String
'    Private sTableColumn As String

'    Public Sub New(ByVal description As String, ByVal fieldtype As String, ByVal comment As String, ByVal cantotal As Boolean, ByVal tablecolumn As String)
'        sName = description
'        sDescription = description
'        sFieldtype = fieldtype
'        sComment = comment
'        bCantotal = cantotal
'        sTableColumn = tablecolumn
'    End Sub

'    Public Sub New(ByVal name As String, ByVal description As String, ByVal fieldtype As String, ByVal comment As String, ByVal cantotal As Boolean, ByVal custom As Boolean)
'        sName = name
'        sDescription = description
'        sFieldtype = fieldtype
'        sComment = comment
'        bCustomColumn = custom
'    End Sub

'    Public Sub New(ByVal name As String, ByVal description As String, ByVal fieldtype As String, ByVal comment As String, ByVal cantotal As Boolean, ByVal custom As Boolean, ByVal tablecolumn As String)
'        sName = name
'        sDescription = description
'        sFieldtype = fieldtype
'        sComment = comment
'        bCustomColumn = custom
'        sTableColumn = tablecolumn
'    End Sub

'    Public Property description() As String
'        Get
'            Return sDescription
'        End Get
'        Set(ByVal value As String)
'            sDescription = value
'        End Set
'    End Property

'    Public ReadOnly Property name() As String
'        Get
'            Return sName
'        End Get
'    End Property

'    Public Property fieldtype() As String
'        Get
'            Return sFieldtype
'        End Get
'        Set(ByVal value As String)
'            sFieldtype = value
'        End Set
'    End Property

'    Public ReadOnly Property comment() As String
'        Get
'            Return sComment
'        End Get
'    End Property

'    Public Property hidden() As Boolean
'        Get
'            Return bHidden
'        End Get
'        Set(ByVal value As Boolean)
'            bHidden = value
'        End Set
'    End Property

'    Public Property customcolumn() As Boolean
'        Get
'            Return bCustomColumn
'        End Get
'        Set(ByVal value As Boolean)
'            bCustomColumn = value
'        End Set
'    End Property

'    Public ReadOnly Property cantotal() As Boolean
'        Get
'            Return bCantotal
'        End Get
'    End Property

'    Public Property listitems() As cColumnList
'        Get
'            Return valuelist
'        End Get
'        Set(ByVal value As cColumnList)
'            valuelist = value
'        End Set
'    End Property

'    Public Property width() As String
'        Get
'            Return sWidth
'        End Get
'        Set(ByVal value As String)
'            sWidth = value
'        End Set
'    End Property

'    Public Property align() As String
'        Get
'            Return sAlign
'        End Get
'        Set(ByVal value As String)
'            sAlign = value
'        End Set
'    End Property

'    Public ReadOnly Property tablecolumn() As String
'        Get
'            Return sTableColumn
'        End Get
'    End Property
'End Class

'Public Class cGridCell
'    Private column As cGridColumn
'    Private cellvalue As Object
'    Private sBgcolor As String

'    Public Sub New(ByVal theColumn As cGridColumn, ByVal thevalue As Object)
'        column = theColumn
'        cellvalue = thevalue
'    End Sub

'    Public ReadOnly Property columninfo() As cGridColumn
'        Get
'            Return column
'        End Get
'    End Property

'    Public Property thevalue() As Object
'        Get
'            Return cellvalue
'        End Get
'        Set(ByVal value As Object)
'            cellvalue = value
'        End Set
'    End Property

'    Public Property bgcolor() As String
'        Get
'            Return sBgcolor
'        End Get
'        Set(ByVal value As String)
'            sBgcolor = value
'        End Set
'    End Property
'End Class

'Public Class cColumnList
'    Private items As New System.Collections.Hashtable

'    Public Sub addItem(ByVal key As Object, ByVal val As String)
'        items.Add(key, val)
'    End Sub

'    Public ReadOnly Property count() As Integer
'        Get
'            Return items.Count
'        End Get
'    End Property

'    Public Function exists(ByVal item As Integer) As Boolean
'        Return items.Contains(item)
'    End Function

'    Public Function getValue(ByVal item As Object) As String
'        Return CStr(items(item))
'    End Function
'End Class
