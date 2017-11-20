Imports SpendManagementLibrary
Imports FWClasses
Imports Spend_Management

Namespace Framework2006
    Partial Class InvoiceForecastGenerate
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim FWDb As New cFWDBConnection

            litForecastGrid.Text = ""

            If Me.IsPostBack = False Then
                curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.InvoiceForecasts, False, True)

                lblFirstDate.Text = "Start Date"
                lblSecondDate.Text = "End Date"

                Title = "Invoice Forecasts Generation"
                Master.title = Title
                Master.enablenavigation = False

                FWDb.DBOpen(fws, False)

                FWDb.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
                If FWDb.FWDb2Flag = True Then
                    lblIFTitle.Text = " - " & FWDb.FWDbFindVal("ContractDescription", 2)
                End If

                PopulateForecasts(FWDb)

                FWDb.DBClose()
            End If

            cmdIFUpdate.AlternateText = "Update"
            cmdIFUpdate.ToolTip = "Accept generated forecasts"
            cmdIFUpdate.Attributes.Add("onmouseover", "window.status='Accept generated forecasts';return true;")
            cmdIFUpdate.Attributes.Add("onmouseout", "window.status='Done';")

            cmdIFCancel.AlternateText = "Cancel"
            cmdIFCancel.ToolTip = "Accept generated forecasts"
            cmdIFCancel.Attributes.Add("onmouseover", "window.status='Abandon forecast generation without saving data';return true;")
            cmdIFCancel.Attributes.Add("onmouseout", "window.status='Done';")

            FWDb = Nothing
        End Sub

        Private Sub PopulateForecasts(ByVal db As cFWDBConnection)
            Dim tmpStr As String = ""
            Dim OK2Gen As Boolean = True

            If Session("GenFromDate") Is Nothing Then
                tmpStr = Trim(db.FWDbFindVal("StartDate", 2))
                If tmpStr <> "" Then
                    dateFirst.Text = Format(CDate(tmpStr), cDef.DATE_FORMAT)
                Else
                    OK2Gen = False
                End If
            Else
                dateFirst.Text = Session("GenFromDate")
            End If

            If Session("GenToDate") Is Nothing Then
                tmpStr = Trim(db.FWDbFindVal("EndDate", 2))
                If tmpStr <> "" Then
                    dateSecond.Text = Format(CDate(tmpStr), cDef.DATE_FORMAT)
                Else
                    OK2Gen = False
                End If
            Else
                dateSecond.Text = Session("GenToDate")
            End If

            If OK2Gen = True Then
                litForecastGrid.Text = GenerateForecasts(db)
            End If
        End Sub

        Private Function GenerateForecasts(ByVal db As cFWDBConnection) As String
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim GenDate As Date, CutOff As Date
            Dim GenPeriodEnd As Date
            Dim PFValue, TotalValue As Double
            Dim ProductDetails, ProductText As String
            'Dim rep As Integer, y As Integer, VBr As String
            Dim tmpGenDate As Date, NumPayments As Integer
            'Dim curMaintYear, NumYears As Integer
            Dim curDate As Date, tmpCutOff As Date
            Dim ContractEndDate As Date
            Dim glSql As System.Text.StringBuilder
            Dim glNYMParams As MaintParams
            Dim glNYMresult As NYMResult
            Dim InvoiceFreq, InvFreqId, tmpYear, MaintYear As Integer
            Dim drow As DataRow
            Dim VBr As String
            Dim strHTML As New System.Text.StringBuilder

            ClearForecasts()

            GenDate = dateFirst.Text
            CutOff = dateSecond.Text

            db.FWDb("R2", "contract_details", "ContractId", Session("ActiveContract"), "", "", "", "", "", "", "", "", "", "")
            If db.FWDbFindVal("EndDate", 2) <> "" Then
                ContractEndDate = CDate(db.FWDbFindVal("EndDate", 2))
            End If

            InvFreqId = Val(db.FWDbFindVal("InvoiceFrequencyTypeId", 2))
            If InvFreqId > 0 Then
                db.FWDb("R", "codes_invoicefrequencytype", "InvoiceFrequencyTypeId", InvFreqId, "", "", "", "", "", "", "", "", "", "")
                If db.FWDbFlag = True Then
                    InvoiceFreq = Val(db.FWDbFindVal("FrequencyInMonths", 1))
                Else
                    lblStatusMessage.Text = "ERROR! Cannot generate forecasts. Unknown Invoice Frequency specified."
                    db.DBClose()
                    db = Nothing
                    Return ""
                    Exit Function
                End If
            Else
                lblStatusMessage.Text = "ERROR! Cannot generate forecasts. No Invoice Frequency specified."
                db.DBClose()
                db = Nothing
                Return ""
                Exit Function
            End If

            curDate = Now

            glSql = New System.Text.StringBuilder
            glSql.Append("SELECT COUNT([ContractForecastId]) AS FC" & vbNewLine)
            glSql.Append("FROM [contract_forecastdetails]" & vbNewLine)
            glSql.Append("WHERE [ContractId] = " & Session("ActiveContract") & vbNewLine)
            glSql.Append("AND [PaymentDate] > CONVERT(datetime,'" & Format(GenDate, "yyyy-MM-dd") & "',120) " & vbNewLine)
            glSql.Append("AND [PaymentDate] < CONVERT(datetime,'" & Format(CutOff, "yyyy-MM-dd") & "',120)")

            db.RunSQL(glSql.ToString, db.glDBWorkA, False, "", False)
            If db.GetRowCount(db.glDBWorkA, 0) > 0 Then
                If db.GetFieldValue(db.glDBWorkA, "FC", 0, 0) > 0 Then
                    lblStatusMessage.Text = "NOTE: Forecast data already exists with the specified date range."
                End If
            End If

            glNYMParams = New MaintParams
            glNYMParams = SMRoutines.GetMaintParams(db, Session("ActiveContract"))

            glSql = New System.Text.StringBuilder
            glSql.Append("SELECT [contract_productdetails].[ProductValue]," & vbNewLine)
            glSql.Append("[contract_productdetails].[MaintenanceValue]," & vbNewLine)
            glSql.Append("[contract_productdetails].[MaintenancePercent]," & vbNewLine)
            glSql.Append("[productDetails].[ProductId]," & vbNewLine)
            glSql.Append("[productDetails].[ProductName]" & vbNewLine)
            glSql.Append("FROM [contract_productdetails]" & vbNewLine)
            glSql.Append("LEFT OUTER JOIN [productDetails]" & vbNewLine)
            glSql.Append(" ON [contract_productdetails].[ProductId] = [productDetails].[ProductId]" & vbNewLine)
            glSql.Append("WHERE [ContractId] = " & Session("ActiveContract"))
            db.RunSQL(glSql.ToString, db.glDBWorkA, False, "", False)

            ' Calculate the number of payments that will result. Ensure correct division value for payment
            tmpGenDate = GenDate
            NumPayments = 0
            Do While tmpGenDate < CutOff
                If (DateAdd("m", InvoiceFreq, tmpGenDate) > CutOff) Then
                    ' check we are not missing it by just a couple of days (e.g. 01/01/2004-31/12/2004)
                    tmpCutOff = DateAdd("d", 3, CutOff)
                    If DateAdd("m", InvoiceFreq, tmpGenDate) > tmpCutOff Then
                        ' not long enough for another forecast payment
                        Exit Do
                    End If
                End If
                tmpGenDate = DateAdd("m", InvoiceFreq, tmpGenDate)
                NumPayments = NumPayments + 1
            Loop

            ' Indicate on form the number of payments
            lblNoPayments.Text = "No. of Payments : " & Trim(Str(NumPayments))

            Dim rowalt As Boolean = False
            Dim rowClass As String = "row1"
            Dim subaccs As New cAccountSubAccounts(curUser.AccountID)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim includePONum As Boolean = params.PONumberGenerate
            Dim PONumStart As Integer = 0
            Dim PONumFormat As String = "{0}"

            If includePONum Then
                PONumStart = params.PONumberSequence
                PONumFormat = "{0}"
                If params.PONumberFormat <> "" Then
                    PONumFormat = params.PONumberFormat
                End If
            End If

            ' compile HTML table
            strHTML.Append("<table class=""datatbl"">" & vbNewLine)
            strHTML.Append("<tr>" & vbNewLine)
            strHTML.Append("<th>Forecast Date</th>" & vbNewLine)
            strHTML.Append("<th>Forecast Amount</th>" & vbNewLine)
            strHTML.Append("<th>Forecast Products</th>" & vbNewLine)
            If includePONum Then
                strHTML.Append("<th>PO Number</th>" & vbNewLine)
            End If
            strHTML.Append("</tr>" & vbNewLine)

            Do While GenDate < CutOff
                ' Don't do the last loop unless at least one more payment period remains - MW
                If (DateAdd("m", InvoiceFreq, GenDate) > CutOff) Then
                    ' check not missing additional payment by a few days
                    tmpCutOff = DateAdd("d", 3, CutOff)

                    If DateAdd("m", InvoiceFreq, GenDate) > tmpCutOff Then
                        ' not long enough for another forecast payment
                        Exit Do
                    End If
                End If

                TotalValue = 0
                ProductText = ""
                ProductDetails = ""
                VBr = ""

                GenPeriodEnd = DateAdd(DateInterval.Day, -1, DateAdd(DateInterval.Month, InvoiceFreq, GenDate))

                For Each drow In db.glDBWorkA.Tables(0).Rows
                    ' establish whether payment is for this term or subsequent
                    If GenDate > ContractEndDate Then
                        ' establish which maintenance year we are calculating for
                        tmpYear = DateDiff("yyyy", ContractEndDate, GenDate)
                        MaintYear = tmpYear
                    Else
                        MaintYear = 0
                    End If

                    glNYMParams.CurMaintVal = drow.Item("MaintenanceValue")
                    glNYMParams.ListPrice = drow.Item("ProductValue")
                    glNYMParams.PctOfLP = drow.Item("MaintenancePercent")

                    glNYMresult = SMRoutines.CalcNYM(glNYMParams, MaintYear)

                    PFValue = glNYMresult.NYMValue / (12 / InvoiceFreq)
                    'If NumPayments / InvoiceFreq > 1 Then
                    '    PFValue = glNYM.Result / NumPayments
                    'Else
                    '    PFValue = glNYM.Result
                    'End If

                    TotalValue = TotalValue + PFValue

                    ProductText = ProductText & VBr & drow.Item("ProductName") & " : " & Format(PFValue, "0.00")
                    VBr = "<br>" 'vbNewLine
                    ProductDetails = ProductDetails & drow.Item("ProductId") & ":" & Format(PFValue, "0.00") & "@"
                Next

                rowalt = (rowalt Xor True)
                If rowalt = True Then
                    rowClass = "row1"
                Else
                    rowClass = "row2"
                End If

                strHTML.Append("<tr>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & GenDate.ToShortDateString & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """>" & Format(TotalValue, "0.00") & "</td>" & vbNewLine)
                strHTML.Append("<td class=""" & rowClass & """><textarea cols=""20"" readonly>" & ProductText & "</textarea></td>" & vbNewLine)
                If includePONum Then
                    strHTML.Append("<td class=""" & rowClass & """>" & String.Format(PONumFormat, PONumStart) & " (provisional)</td>" & vbNewLine)
                    PONumStart += 1
                End If
                strHTML.Append("</tr>" & vbNewLine)

                ' add data to the datagrid
                With dGrid
                    .DisplayLayout.AllowAddNewDefault = Infragistics.WebUI.UltraWebGrid.AllowAddNew.Yes
                    Dim gridRow As New Infragistics.WebUI.UltraWebGrid.UltraGridRow
                    '.Rows.Add()
                    gridRow = .Bands(0).AddNew

                    gridRow.Cells.FromKey("Date").Value = GenDate.ToShortDateString
                    gridRow.Cells.FromKey("Amount").Value = Format(TotalValue, "0.00")
                    gridRow.Cells.FromKey("Breakdown").Value = Replace(ProductText, "<br>", ",")
                    gridRow.Cells.FromKey("Details").Value = ProductDetails
                    gridRow.Cells.FromKey("PeriodEnd").Value = GenPeriodEnd.ToShortDateString

                    gridRow = Nothing
                End With

                GenDate = DateAdd("m", InvoiceFreq, GenDate)
            Loop

            strHTML.Append("</table>")

            GenerateForecasts = strHTML.ToString

            glNYMParams = Nothing
        End Function

        Private Sub ClearForecasts()
            With dGrid
                .Rows.Clear()
                .DisplayLayout.Reset()
            End With
        End Sub

        Private Sub WriteProducts(ByVal db As cFWDBConnection, ByVal Products As String, ByVal ForecastId As Integer)
            Dim x As Integer, y As Integer, Z As Integer, Prod As String
            x = 1
            Do
                y = InStr(x, Products, "@")
                If y > 0 Then
                    Prod = Mid(Products, x, y - x)
                    Z = InStr(Prod, ":")
                    db.SetFieldValue("ForecastId", ForecastId, "N", True)
                    db.SetFieldValue("ProductId", Val(Left(Prod, Z - 1)), "N", False)
                    db.SetFieldValue("ProductAmount", Val(Mid(Prod, Z + 1, 99)), "N", False)
                    db.FWDb("W", "contract_forecastproducts", "", "", "", "", "", "", "", "", "", "", "", "")
                End If
                x = y + 1
            Loop While y > 0
        End Sub

        Private Sub ConfirmGenerate()
            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)
            Dim params As cAccountProperties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties
            Dim includePONum As Boolean = False
            Dim poNumSeq As Integer = 0
            Dim poNumFormat As String = "{0}"

            Dim db As New cFWDBConnection
            Dim gridRow As Infragistics.WebUI.UltraWebGrid.UltraGridRow

            If params.PONumberGenerate Then
                includePONum = True

                If params.PONumberFormat <> "" Then
                    poNumFormat = params.PONumberFormat
                End If
                poNumSeq = params.PONumberSequence
            End If

            db.DBOpen(fws, False)

            For Each gridRow In dGrid.Rows
                db.SetFieldValue("ContractId", Session("ActiveContract"), "N", True)
                db.SetFieldValue("PaymentDate", gridRow.Cells.FromKey("Date").Value, "D", False)
                db.SetFieldValue("ForecastAmount", gridRow.Cells.FromKey("Amount").Value, "N", False)
                db.SetFieldValue("CoverPeriodEnd", gridRow.Cells.FromKey("PeriodEnd").Value, "D", False)
                If includePONum Then
                    db.SetFieldValue("poNumber", String.Format(poNumFormat, poNumSeq), "N", False)
                    poNumSeq += 1
                    subaccs.IncrementPONumber(curUser.CurrentSubAccountId, curUser.EmployeeID)
                End If
                db.FWDb("W", "contract_forecastdetails", "", "", "", "", "", "", "", "", "", "", "", "")

                WriteProducts(db, gridRow.Cells.FromKey("Details").Value, db.glIdentity)
            Next

            db.DBClose()
            db = Nothing
            Session("IFAction") = Nothing
            Session("GenFromDate") = Nothing
            Session("GenToDate") = Nothing
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub

        Protected Sub cmdReGenerate_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            Session("GenFromDate") = dateFirst.Text
            Session("GenToDate") = dateSecond.Text

            Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            Dim db As New cFWDBConnection

            db.DBOpen(fws, False)
            PopulateForecasts(db)
            db.DBClose()
            db = Nothing
            'Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
            'Response.Redirect("InvoiceForecastGenerate.aspx", True)
        End Sub

        Protected Sub cmdIFUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdIFUpdate.Click
            ConfirmGenerate()
        End Sub

        Protected Sub cmdIFCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdIFCancel.Click
            Session("IFAction") = Nothing
            Response.Redirect("ContractSummary.aspx?tab=" & SummaryTabs.InvoiceForecast & "&id=" & Session("ActiveContract"), True)
        End Sub
    End Class

End Namespace
