Imports Spend_Management

Partial Class FWTasksPopupSummary
    Inherits System.Web.UI.UserControl

    Protected Sub FWTasksPopupSummary_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim currentUser As CurrentUser = cMisc.getCurrentUser()

        If HttpContext.Current.User.Identity.IsAuthenticated Then

            Dim clsTasks As cTasks = New cTasks(currentUser.AccountID, currentUser.CurrentSubAccountId)
            Dim tasks As svcTasks = New svcTasks()
            Dim gridData As String() = tasks.getTasksToCompleteGrid()
            litTaskSummaryPopup.Text = gridData(2)

            ' set the sel.grid javascript variables
            Dim jsGridObjects As New List(Of String)
            jsGridObjects.Add(gridData(1))

            Page.ClientScript.RegisterStartupScript(Me.GetType(), "taskPopupSummaryGridVars", cGridNew.generateJS_init("taskPopupSummaryGridVars", jsGridObjects, currentUser.CurrentActiveModule), True)
        End If


    End Sub
End Class
