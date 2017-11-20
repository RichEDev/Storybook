<%@ Page language="c#" Inherits="Spend_Management.aefolder" MasterPageFile="~/masters/smForm.master" Codebehind="aefolder.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

		
				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">				
				
			<div class="valdiv">
				<asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
			</div>
			<div class="inputpanel">
				<div class="inputpaneltitle">
                    <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
				<table>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblcategoryname" runat="server" Text="Category Name:" meta:resourcekey="lblcategorynameResource1"></asp:Label></td>
						<td class="inputtd">
							<asp:TextBox id="txtfolder" runat="server" meta:resourcekey="txtfolderResource1" MaxLength="100"></asp:TextBox></td>
						<td>
							<asp:RequiredFieldValidator id="reqfolder" runat="server" ErrorMessage="Please enter a name for this folder"
								ControlToValidate="txtfolder" meta:resourcekey="reqfolderResource1">*</asp:RequiredFieldValidator></td>
					</tr>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblpersonalcategory" runat="server" Text="Personal Category:" meta:resourcekey="lblpersonalcategoryResource1"></asp:Label></td>
						<td class="inputtd">
							<asp:CheckBox id="chkpersonal" runat="server" meta:resourcekey="chkpersonalResource1"></asp:CheckBox></td>
					</tr>
				</table>
			</div>
			<div class="inputpanel">
				<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;<a href="folders.aspx"><img src="../images/buttons/cancel_up.gif"></a></div>

    </asp:Content>


	