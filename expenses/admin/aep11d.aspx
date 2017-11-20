<%@ Page language="c#" Inherits="expenses.aep11d" MasterPageFile="~/expform.master" Codebehind="aep11d.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
		<div class="valdiv">
			<asp:ValidationSummary id="ValidationSummary1" runat="server" Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
			<asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" Font-Size="Small" meta:resourcekey="lblmsgResource1">Label</asp:Label>
		</div>
		<div class="inputpanel">
			<div class="inputpaneltitle">
				<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
			<table>
				<tr>
					<td class="labeltd">
						<asp:Label id="lblp11d" runat="server" meta:resourcekey="lblp11dResource1">P11d Category:</asp:Label></td>
					<td class="inputtd">
						<asp:TextBox id="txtpdcat" runat="server" MaxLength="50" meta:resourcekey="txtpdcatResource1"></asp:TextBox></td>
					<td><asp:RequiredFieldValidator id="reqp11d" runat="server" ErrorMessage="Please enter a value for the field P11D Category"
							ControlToValidate="txtpdcat" meta:resourcekey="reqp11dResource1">*</asp:RequiredFieldValidator></td>
				</tr>
			</table>
		</div>
		<div class="inputpanel">
			<div class="inputpaneltitle">
				<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Assigned Expense Items</asp:Label></div>
			<asp:Literal id="litsubcats" runat="server" meta:resourcekey="litsubcatsResource1"></asp:Literal>
		</div>
		<div class="inputpanel">
			<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
			<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
		<asp:TextBox id="txtaction" runat="server" Visible="False" meta:resourcekey="txtactionResource1"></asp:TextBox>
		<asp:TextBox id="txtpdcatid" runat="server" Visible="False" meta:resourcekey="txtpdcatidResource1"></asp:TextBox>
            </asp:Content>

<asp:Content runat="server" ID="customScipts" ContentPlaceHolderID="scripts">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.datatbl').css('width', '30%');
            $('.datatbl').css('border', '1px solid #CCC');
            $('.datatbl td,.datatbl th').css('padding-left', '5px');
            $('.datatbl td').css('padding-bottom', '3px');
        });
    </script>
</asp:Content>