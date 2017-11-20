Imports FWClasses
Imports SpendManagementLibrary
Imports Spend_Management

Namespace Framework2006
    Partial Class FWSetupRecharge
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
            Title = "Recharge Settings"
            Master.title = Title

            '         If Me.IsPostBack = False Then
            '             Dim curUser As CurrentUser = cMisc.GetCurrentUser()
            '             Dim subaccs As New cAccountSubAccounts(curUser.Account.accountid)
            '             Dim fws As cFWSettings = cMigration.ConvertToFWSettings(curUser.Account, subaccs.getSubAccountsCollection, curUser.CurrentSubAccountId)

            '	' populate the locations list

            '	lstLocations.Items.AddRange(locs.GetListItems)
            '	If lstLocations.SelectedItem Is Nothing Then
            '		lstLocations.SelectedIndex = 0
            '	End If

            '	' populate months into ddlist
            '	Dim m As Integer
            '	For m = 1 To 12
            '		Dim lstItem As New ListItem

            '		lstItem.Value = m
            '		lstItem.Text = MonthName(m)

            '		lstFYCommences.Items.Insert(m - 1, lstItem)
            '	Next

            '	lstReferenceAs.Items.Clear()
            '	lstReferenceAs.Items.Add(New ListItem("Client"))
            '	lstReferenceAs.Items.Add(New ListItem("Customer"))

            '	GetSettings(lstLocations.SelectedValue)
            'End If

            'cmdUpdate.AlternateText = "Save"
            'cmdUpdate.ToolTip = "Store Recharge Settings to the database"
            'cmdUpdate.Attributes.Add("onmouseover", "window.status='Store Recharge Settings to the database';return true;")
            'cmdUpdate.Attributes.Add("onmouseout", "window.status='Done';")
		End Sub

		Private Sub GetSettings(ByVal loc As Integer)
            'Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
            '         Dim rscoll As New cRechargeSettings(curUser.UserFWS.MetabaseCustomerId, curUser.currentUser.userInfo.ActiveLocation, curUser.UserFWS.getConnectionString)
            '         Dim rs As cRechargeSetting = rscoll.getSettings

            'lstReferenceAs.Items.FindByValue(rs.ReferenceAs).Selected = True
            'lstStaffRef.Items.FindByValue(rs.StaffRepAs).Selected = True
            'lstRechargePeriod.SelectedIndex = rs.RechargePeriod

            'If rs.FinYearCommence > 0 Then
            '	lstFYCommences.SelectedIndex = rs.FinYearCommence - 1
            'End If

            'If rs.CP_Delete_Action > 0 Then
            '	lstCPDeleteAction.Items.FindByValue(rs.CP_Delete_Action).Selected = True
            'End If
		End Sub

		Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdUpdate.Click
            'Try
            '	Dim curUser As cCurrentUser = cUserMisc.GetCurrentUser(User.Identity)
            '	Dim db As New cFWDBConnection
            '	Dim uinfo As UserInfo = curUser.currentUser.userInfo
            '	Dim fws As cFWSettings = curUser.UserFWS
            '             Dim params As New FWClasses.cParams(uinfo, fws, uinfo.ActiveLocation)
            '             Dim rscoll As New cRechargeSettings(fws.MetabaseCustomerId, uinfo.ActiveLocation, fws.getConnectionString)

            '	db.DBOpen(fws, False)

            '	db.FWDb("D", "fwparams", "Location Id", lstLocations.SelectedValue, "Param", "REFERENCE_AS", "", "", "", "", "", "", "", "")
            '	db.SetFieldValue("Location Id", lstLocations.SelectedValue, "N", True)
            '	db.SetFieldValue("Param", "REFERENCE_AS", "S", False)
            '	db.SetFieldValue("Value", lstReferenceAs.SelectedValue, "S", False)
            '	db.FWDb("W", "fwparams", "", "", "", "", "", "", "", "", "", "", "", "")

            '	db.FWDb("D", "fwparams", "Location Id", lstLocations.SelectedValue, "Param", "STAFF_REF", "", "", "", "", "", "", "", "")
            '	db.SetFieldValue("Location Id", lstLocations.SelectedValue, "N", True)
            '	db.SetFieldValue("Param", "STAFF_REF", "S", False)
            '	db.SetFieldValue("Value", lstStaffRef.SelectedValue, "S", False)
            '	db.FWDb("W", "fwparams", "", "", "", "", "", "", "", "", "", "", "", "")

            '	db.FWDb("D", "fwparams", "Location Id", lstLocations.SelectedValue, "Param", "RECHARGE_PERIOD", "", "", "", "", "", "", "", "")
            '	db.SetFieldValue("Location Id", lstLocations.SelectedValue, "N", True)
            '	db.SetFieldValue("Param", "RECHARGE_PERIOD", "S", False)
            '	db.SetFieldValue("Value", lstRechargePeriod.SelectedValue, "S", False)
            '	db.FWDb("W", "fwparams", "", "", "", "", "", "", "", "", "", "", "", "")

            '	db.FWDb("D", "fwparams", "Location Id", lstLocations.SelectedValue, "Param", "FY_COMMENCES", "", "", "", "", "", "", "", "")
            '	db.SetFieldValue("Location Id", lstLocations.SelectedValue, "N", True)
            '	db.SetFieldValue("Param", "FY_COMMENCES", "S", False)
            '	db.SetFieldValue("Value", lstFYCommences.SelectedValue, "S", False)
            '	db.FWDb("W", "fwparams", "", "", "", "", "", "", "", "", "", "", "", "")

            '	db.FWDb("D", "fwparams", "Location Id", lstLocations.SelectedValue, "Param", "CP_DEL_ACTION", "", "", "", "", "", "", "", "")
            '	db.SetFieldValue("Location Id", lstLocations.SelectedValue, "N", True)
            '	db.SetFieldValue("Param", "CP_DEL_ACTION", "S", False)
            '	db.SetFieldValue("Value", lstCPDeleteAction.SelectedValue, "S", False)
            '	db.FWDb("W", "fwparams", "", "", "", "", "", "", "", "", "", "", "", "")

            '	db.DBClose()
            '	db = Nothing

            '	params.InvalidateCache()

            '	litMessage.Text = "Recharge Parameters Saved successfully"
            '	cmdUpdate.Visible = False
            '	cmdDone.Visible = True

            'Catch ex As Exception
            '	litMessage.Text = "<div><b>Error : </b>" & ex.Message & "</div>"
            'End Try
		End Sub

        Protected Sub cmdDone_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDone.Click
            Response.Redirect("Home.aspx", True)
        End Sub

        Protected Sub lstLocations_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstLocations.SelectedIndexChanged
            'GetSettings(lstLocations.SelectedValue)
        End Sub
    End Class

End Namespace
