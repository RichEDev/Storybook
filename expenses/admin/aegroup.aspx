<%@ Page language="c#" Inherits="expenses.aegroup" MasterPageFile="~/expform.master" Codebehind="aegroup.aspx.cs" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <script type="text/javascript" language="javascript">

        function deleteStage(groupid, signoffid)
        {
            if (confirm('Are you sure you wish to delete the selected stage?'))
            {
                PageMethods.DeleteStage(accountid, groupid, signoffid,
                    function (deletedStageNo)
                    {
                        var grid = igtbl_getGridById(contentID + 'gridstages');
                        var currentStageNo = grid.getActiveRow().getIndex() + 1;
                        if (deletedStageNo != currentStageNo)
                        {
                            grid.Rows.remove(deletedStageNo - 1);
                        }
                        grid.Rows.remove(currentStageNo - 1);
                    },
                    function (error)
                    {
                        SEL.MasterPopup.ShowMasterPopup(error._message, 'Message from ' + moduleNameHTML);
                    });
            }
        }
    </script>
				<asp:LinkButton id="lnkAddStage" runat="server" CssClass="submenuitem" onclick="lnkAddStage_Click" meta:resourcekey="LinkButton1Resource1">Add Stage</asp:LinkButton></asp:Content>
				
				<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
                    <div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
		<asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" Font-Size="Small" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>
    <div class="inputpanel"><asp:Label Visible="false" ID="lblclaimsinprocess" runat="server" Text="This signoff group cannot currently be amended as there are one or more claims in the approval process relating to this signoff group." ForeColor="Red"></asp:Label></div>
	<div class="inputpanel">
        
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table style="width:500px;">
			<tr>
				<td class="labeltd">
					<asp:Label id="lblgroupname" runat="server" meta:resourcekey="lblgroupnameResource1">Group Name:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtgroupname" runat="server" MaxLength="50" meta:resourcekey="txtgroupnameResource1"></asp:textbox></td>
				<td>
					<asp:RequiredFieldValidator id="reqgroupname" runat="server" ErrorMessage="Please enter a Group Name" ControlToValidate="txtgroupname" meta:resourcekey="reqgroupnameResource1">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top">
					<asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Description:</asp:Label></td>
				<td class="inputtd"><asp:textbox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1"></asp:textbox></td>
			</tr>
            <tr>
            <td class="labeltd"><asp:Label runat="server" ID="lblAllowOneStepAuthorisation">Allow One Step Authorisation:</asp:Label></td> 
            <td class="inputtd" style="width:170px"><asp:CheckBox runat="server" ID="chkAllowOneStepAuthorisation" /></td>
            <td><img id="imgtooltip428" onclick="SEL.Tooltip.Show('3dcba94d-4a5a-4b89-bb49-a9613eaa14df', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
            </tr>
		</table>
	</div>
	<div class="inputpanel table-border">
		<div class="inputpaneltitle">
			<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Signoff Hierarchy</asp:Label></div>
		<igtbl:ultrawebgrid id="gridstages" runat="server" Width="660px" meta:resourcekey="gridstagesResource1" SkinID="gridskin">
			
			
		</igtbl:ultrawebgrid>
	</div>
	<div class="inputpanel">
		<asp:imagebutton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:imagebutton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
	<asp:textbox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:textbox><asp:textbox id="txtgroupid" runat="server" Visible="False" meta:resourcekey="txtgroupidResource1"></asp:textbox>
        </asp:Content>


