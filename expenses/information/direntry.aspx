<%@ Page language="c#" Inherits="expenses.information.direntry" MasterPageFile="~/expform.master" Codebehind="direntry.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1">Name:</asp:Label></td>
				<td class="inputtd">
					<asp:TextBox id="txtname" runat="server" meta:resourcekey="txtnameResource1"></asp:TextBox></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblcontactdetails" runat="server" Text="Employment Contact Details" meta:resourcekey="lblcontactdetailsResource1"></asp:Label></div>
		<TABLE>
			<TR>
				<TD class="labeltd" width="99">
					<asp:Label id="lblext" runat="server" meta:resourcekey="lblextResource1">Extension No:</asp:Label></TD>
				<TD class="inputtd">
					<asp:TextBox id="txtextension" runat="server" meta:resourcekey="txtextensionResource1"></asp:TextBox></TD>
			</TR>
			<TR>
				<TD class="labeltd" width="99">
					<asp:Label id="lblmobileno" runat="server" meta:resourcekey="lblmobilenoResource1">Mobile No:</asp:Label></TD>
				<TD class="inputtd">
					<asp:TextBox id="txtmobileno" runat="server" meta:resourcekey="txtmobilenoResource1"></asp:TextBox></TD>
			</TR>
			<TR>
				<TD class="labeltd" width="99">
					<asp:Label id="lblpagerno" runat="server" meta:resourcekey="lblpagernoResource1">Pager No:</asp:Label></TD>
				<TD class="inputtd">
					<asp:TextBox id="txtpagerno" runat="server" meta:resourcekey="txtpagernoResource1"></asp:TextBox></TD>
			</TR>
			<TR>
				<TD class="labeltd" width="99">
					<asp:Label id="lblemail" runat="server" meta:resourcekey="lblemailResource1">E-mail Address:</asp:Label></TD>
				<TD class="inputtd">
					<asp:textbox id="txtemail" runat="server" MaxLength="200" meta:resourcekey="txtemailResource1"></asp:textbox></TD>
			</TR>
		</TABLE>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lbladdress" runat="server" Text="Contact Details" meta:resourcekey="lbladdressResource1"></asp:Label></div>
		<TABLE>
			<TR>
				<TD class="labeltd">
					<asp:Label id="lbltelno" runat="server" meta:resourcekey="lbltelnoResource1">Tel No:</asp:Label></TD>
				<TD class="inputtd">
					<asp:textbox id="txttelno" runat="server" MaxLength="50" meta:resourcekey="txttelnoResource1"></asp:textbox></TD>
			</TR>
			<TR>
				<TD class="labeltd">
					<asp:Label id="lblfaxno" runat="server" meta:resourcekey="lblfaxnoResource1">Fax No:</asp:Label></TD>
				<TD class="inputtd">
					<asp:TextBox id="txtfaxno" runat="server" meta:resourcekey="txtfaxnoResource1"></asp:TextBox></TD>
			</TR>
			<TR>
				<TD class="labeltd">
					<asp:Label id="lblemailhome" runat="server" meta:resourcekey="lblemailhomeResource1">E-mail Address:</asp:Label></TD>
				<TD class="inputtd">
					<asp:TextBox id="txtemailhome" runat="server" meta:resourcekey="txtemailhomeResource1"></asp:TextBox></TD>
			</TR>
		</TABLE>
	</div>
	<div class="inputpanel newbuttonbg">
		<a href="directory.aspx">Back</a></div>
   </asp:Content>

