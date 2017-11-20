<%@ Page language="c#" Inherits="expenses.adminfloats" MasterPageFile="~/exptemplate.master" Codebehind="adminfloats.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


	
	<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Advances Awaiting Approval</asp:Label></div>
		<asp:Literal id="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal>

	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Active Advances</asp:Label></div>
		<asp:Literal id="litactive" runat="server" meta:resourcekey="litactiveResource1"></asp:Literal>

     </div>

        	<div class="formpanel formpanel_padding">
                        <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
            </div>

    </asp:Content>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="settledadvances.aspx" class="submenuitem">Settled Advances</a>

    </asp:Content>

