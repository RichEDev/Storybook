<%@ Page language="c#" Inherits="expenses.information.reviews" MasterPageFile="~/exptemplate.master" Codebehind="reviews.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
		<div class="inputpanel table-border2">
			<asp:Literal id="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal>
            <div class="formpanel formpanel_left">
                <div class="formbuttons">
                    <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
                </div>
            </div>
	</div>

</asp:Content>
