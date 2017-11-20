<%@ Page language="c#" Inherits="expenses.information.reviewdetails" MasterPageFile="~/expform.master" Codebehind="reviewdetails.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

					<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<asp:LinkButton id="cmdreview" runat="server" CssClass="submenuitem" onclick="cmdreview_Click" meta:resourcekey="cmdreviewResource1">Review This Hotel</asp:LinkButton></asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblhoteldetails" runat="server" Text="Label">Hotel Details</asp:Label></div>
		<table class="tr-height">
			<tr>
				<td vAlign="top" class="labeltd" height="20"><strong>
                    <asp:Label ID="lblhotelname" runat="server" Text="Label">Hotel&nbsp;Name:</asp:Label></strong></td>
				<td vAlign="top" class="inputtd" height="20"><asp:label id="lblname" runat="server">Label</asp:label></td>
				<td class="labeltd" vAlign="top"><STRONG>
                    <asp:Label ID="lbltelnum" runat="server" Text="Label">Tel No:</asp:Label></STRONG></td>
				<td class="inputtd" vAlign="top">
					<asp:Label id="lbltelno" runat="server">Label</asp:Label></td>
			</tr>
			<tr>
				<td valign="top" class="labeltd"><strong><asp:Label ID="lbladdresslbl" runat="server" Text="Label">Hotel&nbsp;Address:</asp:Label></strong></td>
				<td class="inputtd"><asp:Label id="lbladdress" runat="server">Label</asp:Label></td>
				<td valign="top" class="labeltd"><strong><asp:Label ID="lblemaillbl" runat="server" Text="Label">E-mail Address:</asp:Label></strong></td>
				<td class="inputtd"><asp:Label id="lblemail" runat="server"></asp:Label></td></tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblreviews" runat="server" Text="Label">Reviews</asp:Label></div>
		<asp:Literal id="litreviews" runat="server"></asp:Literal>
	</div>

    </asp:Content>


