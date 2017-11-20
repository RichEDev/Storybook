Imports Spend_Management

Partial Class FWSubAccountPopup
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim currentUser As CurrentUser = cMisc.GetCurrentUser()

        If HttpContext.Current.User.Identity.IsAuthenticated Then

            Dim clsSubAccs As New cAccountSubAccounts(currentUser.AccountID)

            Dim tmpLstSA As ListItem() = clsSubAccs.CreateFilteredDropDown(currentUser.Employee, currentUser.CurrentSubAccountId)
            If tmpLstSA.Length > 0 Then
                If tmpLstSA.Length > 1 Or (tmpLstSA.Length = 1 And tmpLstSA(0).Value <> currentUser.CurrentSubAccountId.ToString) Then
                    Dim rowCount As Integer
                    Dim gridData As String() = clsSubAccs.generateSubaccountGrid(rowCount)

                    litSubAccounts.Text = gridData(2)

                    ' set the sel.grid javascript variables
                    Dim jsGridObj As New List(Of String)
                    jsGridObj.Add(gridData(1))

                    Page.ClientScript.RegisterStartupScript(Me.GetType(), "subAccPopupSummaryGridVars", cGridNew.generateJS_init("subAccPopupSummaryGridVars", jsGridObj, currentUser.CurrentActiveModule), True)

                    If rowCount > 10 Then
                        divSubAccounts.Style.Add("overflow", "auto")
                        divSubAccounts.Style.Add("height", "350px")
                    End If

                End If
            End If
        End If

    End Sub

End Class
