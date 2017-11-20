<%@ Page language="c#" Inherits="expenses.information.about" MasterPageFile="~/exptemplate.master" Codebehind="about.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">		
		<div class=inputpanel>
			<div class=inputpaneltitle>
                <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
			<table>
				<tr>
					<td class="labeltd"><STRONG>
                        <asp:Label ID="lblproductname" runat="server" Text="Product Name:" meta:resourcekey="lblproductnameResource1"></asp:Label></STRONG>:</td>
					<td class=inputtd>
						<asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1">Expenses</asp:Label></td>
				</tr>
				<tr>
					<td class="labeltd"><STRONG>
                        <asp:Label ID="lblproductversion" runat="server" Text="Product Version:" meta:resourcekey="lblproductversionResource1"></asp:Label></STRONG>:</td>
					<td class=inputtd>
						<asp:Label id="lblversion" runat="server" meta:resourcekey="lblversionResource1">Label</asp:Label></td>
				</tr>
			</table>
		</div>
			
		<div class=inputpanel>
			<div class=inputpaneltitle>
                <asp:Label ID="lbllicence" runat="server" Text="Licence Information" meta:resourcekey="lbllicenceResource1"></asp:Label></div>		
					
			<table>
				<tr>
					<td class="labeltd"><STRONG>
                        <asp:Label ID="lblnumusers" runat="server" Text="No of Users:" meta:resourcekey="lblnumusersResource1"></asp:Label></STRONG></td>
					<td class=inputtd>
						<asp:Label id="lblusers" runat="server" meta:resourcekey="lblusersResource1">Label</asp:Label></td>
				</tr>
				<tr>
					<td class="labeltd"><STRONG>
                        <asp:Label ID="lblexpirydate" runat="server" Text="Expiry Date:" meta:resourcekey="lblexpirydateResource1"></asp:Label></STRONG></td>
					<td class=inputtd>
						<asp:Label id="lblexpiry" runat="server" meta:resourcekey="lblexpiryResource1">Label</asp:Label></td>
				</tr>
			</table>
						
		</div>
		<div class=inputpanel>
			<div class=inputpaneltitle>
                <asp:Label ID="lblcontact" runat="server" Text="Contact Information" meta:resourcekey="lblcontactResource1"></asp:Label></div>		
				<table class="borderedtable" width="99%">
					<tr>
						<td class="labeltd"><STRONG>
                            <asp:Label ID="lblcompany" runat="server" Text="Company&nbsp;Name:" meta:resourcekey="lblcompanyResource1"></asp:Label></STRONG></td>
						<td class=inputtd>Software (Europe) Ltd</td>
					</tr>
					<tr>
						<td class="labeltd" valign="top"><STRONG>Address:</STRONG></td>
						<td class=inputtd>
							Nibley&nbsp;House<br>
							Low&nbsp;Moor&nbsp;Road<br>
							Doddington&nbsp;Road<br>
							Lincoln Lincolnshire LN6&nbsp;3JY
						</td>
					</tr>
					<tr>
						<td class="labeltd"><STRONG>
                            <asp:Label ID="lbltelnum" runat="server" Text="Tel No:" meta:resourcekey="lbltelnumResource1"></asp:Label></STRONG></td>
						<td class=inputtd>+44 (0)1522 881300</td>
					</tr>
					<tr>
						<td class="labeltd"><STRONG>
                            <asp:Label ID="lblfaxnum" runat="server" Text="Fax No:" meta:resourcekey="lblfaxnumResource1"></asp:Label></STRONG></td>
						<td class=inputtd>+44 (0)1522 881355</td>
					</tr>
					<tr>
						<td class="labeltd"><STRONG>
                            <asp:Label ID="lblemail" runat="server" Text="E-mail Address:" meta:resourcekey="lblemailResource1"></asp:Label></STRONG></td>
						<td class=inputtd>info@software-europe.co.uk</td>
					</tr>
				</table>
			</div>

    </asp:Content>

