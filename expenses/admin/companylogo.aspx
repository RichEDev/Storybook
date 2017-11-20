<%@ Page language="c#" Inherits="expenses.admin.companylogo" MasterPageFile="~/expform.master" Codebehind="companylogo.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
			<div class="valdiv"><asp:label id="lblmsg" runat="server" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:label></div>
			<div class="inputpanel">
				<div class="inputpaneltitle">
					<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Current Logo</asp:Label></div>
				<asp:literal id="litlogo" runat="server" meta:resourcekey="litlogoResource1"></asp:literal>
			</div>
			<div class="inputpanel">
				<asp:LinkButton id="cmdremove" runat="server" onclick="cmdremove_Click" meta:resourcekey="cmdremoveResource1">Remove Logo</asp:LinkButton></div>
			<div class="inputpanel">
				<div class="inputpaneltitle">
					<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Upload Logo</asp:Label></div>
				<table>
					<tr>
						<td colSpan="2">
							<asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Please Note: To upload your company logo it must be in a JPG format or GIF format. The image must be no taller than 60 pixels.</asp:Label></td>
					</tr>
                    <tr>
                        <td>&nbsp;</td>
                         <td>&nbsp;</td>

                    </tr>
					<tr>
						<td class="labeltd">
							<asp:Label id="Label4" runat="server" meta:resourcekey="Label4Resource1">File Name</asp:Label>:</td>
					    <td><INPUT id="filelogo" type="file" name="filelogo" runat="server"/></td>
					</tr>
				</table>
			    <br/><br/><br/>
				<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;
				<asp:ImageButton runat="server" ID="cmdCancel" 
                    ImageUrl="~/shared/images/buttons/cancel_up.gif" onclick="cmdCancel_Click" />
				</div>

    </asp:Content>
		<asp:Content ID="Content3" runat="server" 
    contentplaceholderid="contentmenu">

                
</asp:Content>

		