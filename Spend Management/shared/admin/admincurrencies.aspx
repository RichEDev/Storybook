<%@ Page language="c#" Inherits="Spend_Management.admincurrencies" MasterPageFile="~/masters/smTemplate.master" Codebehind="admincurrencies.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a id="lnkAddCurrency" runat="server" class="submenuitem" href="javascript:addCurrency();"><asp:Label id="Label5" runat="server" meta:resourcekey="Label1Resource1">New Currency</asp:Label></a>
				
				</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
    <script src="../javascript/currencies.js" type="text/javascript" language="javascript"></script>
    
    <script type="text/javascript" language="javascript">
        var cmbFilterID = '<%=cmbfilter.ClientID %>';
        var pnlGridID = '<%=pnlGrid.ClientID %>';
    </script>
    			
    				
    <div class="formpanel formpanel_padding" id="ERTypeDiv">
        <div class="sectiontitle">Exchange Rate is</div>
        <div class="comment" id="exchangeRateComment" runat="server" style="display: none"><span>Automatic daily update of exchange rates is enabled.</span></div>
	    <div class="twocolumn">
		    <asp:Label id="lblStatic" runat="server" meta:resourcekey="lblStaticResource1" AssociatedControlID="rButStatic">Static</asp:Label>
            <span class="inputs"><asp:RadioButton id="rButStatic" runat="server" GroupName="currency" Width="153px" meta:resourcekey="rButStaticResource1"></asp:RadioButton></span>
	        <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"></span>
            <asp:Label id="lblMonth" runat="server" meta:resourcekey="lblMonthResource1" AssociatedControlID="rButMonth">Updated Monthly</asp:Label>
            <span class="inputs"><asp:RadioButton id="rButMonth" runat="server" GroupName="currency" Width="153px" meta:resourcekey="rButMonthResource1"></asp:RadioButton></span>
	        <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"></span>
        </div>
	     <div class="twocolumn">
		    <asp:Label id="lblRange" runat="server" meta:resourcekey="lblRangeResource1" AssociatedControlID="rButRange">Determined by a Date Range</asp:Label>
            <span class="inputs"><asp:RadioButton id="rButRange" runat="server" GroupName="currency" Width="153px" meta:resourcekey="rButRangeResource1"></asp:RadioButton></span>
	        <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"></span>
        </div>
    </div>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">Currencies</div>
	     <div class="twocolumn">
	        <asp:Label id="lblFilter" runat="server" meta:resourcekey="Label1Resource1" AssociatedControlID="cmbfilter">Display Filter</asp:Label>
	        <span class="inputs"><asp:DropDownList id="cmbfilter" runat="server" onchange="javascript:SEL.Grid.filterGridCmb('gridCurrencies',event);" meta:resourcekey="cmbfilterResource1">
	            <asp:ListItem Value="2" meta:resourcekey="ListItemResource1">All Currencies</asp:ListItem>
		        <asp:ListItem Value="0" meta:resourcekey="ListItemResource2">Un-archived Currencies</asp:ListItem>
		        <asp:ListItem Value="1" meta:resourcekey="ListItemResource3">Archived Currencies</asp:ListItem>
	        </asp:DropDownList></span>
	    </div><%--
	</div>
        
    <div class="formpanel">--%>
        <asp:Panel runat="server" ID="pnlGrid">
            <asp:Literal ID="litgrid" runat="server"></asp:Literal>        
        </asp:Panel>

    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div> 

    </div>


</asp:Content>
	


