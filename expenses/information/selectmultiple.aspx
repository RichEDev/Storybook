<%@ Page language="c#" Inherits="expenses.information.selectmultiple" MasterPageFile="~/expform.master" Codebehind="selectmultiple.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

	<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
			<div class=valdiv>
			<asp:Label id="lblmsg" runat="server" Font-Size="Small" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:Label></div>
			
			<div class=inputpanel>
                <asp:Label ID="lblmultiple" runat="server" Text="The items ticked here will appear on your Add New Expense screen." meta:resourcekey="lblmultipleResource1"></asp:Label>
			    <br/><br/><asp:Label id="lbllimits" runat="server" meta:resourcekey="lbllimitsResource1">Label</asp:Label>
			</div>
			<div class=" inputpanel table-border2 ">
						<asp:Literal id="litgrid" runat="server" EnableViewState="False" meta:resourcekey="litgridResource1"></asp:Literal>
			</div>
					
			<div class=inputpanel>
			<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
				<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>

    </asp:Content>	
		
	