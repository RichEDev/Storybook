<%@ Page language="c#" Inherits="expenses.myadvances" MasterPageFile="~/exptemplate.master" Codebehind="myadvances.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


	
		
						<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
						<a href="aeadvancereq.aspx" class=submenuitem>
                            <asp:Label ID="lblrequest" runat="server" Text="Request Advance" meta:resourcekey="lblrequestResource1"></asp:Label></a></asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
		<script language="javascript">
				
				function deleteAdvance(advanceid)
				{
					if (confirm('Are you sure you wish to delete the selected advance?'))
					{
						document.location = "myadvances.aspx?action=3&advanceid=" + advanceid;
					}
				}
		</script>
			<div class="inputpanel table-border2">
				<asp:Literal id="litgrid" runat="server" EnableViewState="False" meta:resourcekey="litgridResource1"></asp:Literal>
			</div>

                	<div class="formpanel">
            <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
        </div>

    </asp:Content>
		

	