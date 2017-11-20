<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aecurrency.aspx.cs" MasterPageFile="~/masters/smForm.master" Inherits="Spend_Management.aecurrency" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
    
    <asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
        <asp:LinkButton id="cmdAddExchange" runat="server" CssClass="submenuitem"  
            OnClientClick="viewfilter = 1;" meta:resourcekey="cmdAddExchangeResource1" 
            onclick="cmdAddExchange_Click">New Exchange Rate</asp:LinkButton>
	</asp:Content>
	
	
    <asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    
     <script type="text/javascript" language="javascript">
         var cmbcurrencyid = '<%=cmbCurrency.ClientID %>';
         var txtalphacodeid = '<%=txtAlphaCode.ClientID %>';
         var txtnumericcodeid = '<%=txtNumericCode.ClientID %>';
         var txtsymbolid = '<%=txtSymbol.ClientID %>';
         
    </script>
    
    <script src="../javascript/currencies.js" type="text/javascript" language="javascript"></script>
    <script src="../javascript/validate.js" type="text/javascript" language="javascript"></script>
    
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">General Details</div>
            <div class="onecolumnsmall">
                <asp:Label id="lblcurrency" CssClass="mandatory" runat="server" meta:resourcekey="lblcurrencyResource1" Text="Currency" AssociatedControlID="cmbCurrency"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbCurrency" runat="server" meta:resourcekey="cmbCurrencyResource1"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
				<asp:Label id="lblSymbol" runat="server" meta:resourcekey="lblSymbolResource1" Text="Symbol" AssociatedControlID="txtSymbol"></asp:Label><span class="inputs"><asp:textbox id="txtSymbol" runat="server" CssClass="fillspan" MaxLength="3" Enabled="False" meta:resourcekey="txtSymbolResource1"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
				<asp:Label id="lblCurrencyCode" runat="server" meta:resourcekey="lblCurrencyCodeResource1" Text="Alpha Currency Code" AssociatedControlID="txtAlphaCode"></asp:Label><span class="inputs"><asp:textbox id="txtAlphaCode" CssClass="fillspan" MaxLength="5" runat="server" Enabled="False" meta:resourcekey="txtAlphaCodeResource1"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                <asp:Label id="lblNumericCode" runat="server" meta:resourcekey="lblNumericCodeResource1" Text="Numeric Currency Code" AssociatedControlID="txtNumericCode"></asp:Label><span class="inputs"><asp:TextBox id="txtNumericCode"  CssClass="fillspan" MaxLength="5" runat="server" Enabled="False" meta:resourcekey="txtNumericCodeResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
				<asp:Label id="lblPositiveFormat" runat="server" meta:resourcekey="lblPositiveFormatResource1" Text="Positive Format" AssociatedControlID="cmbPositiveFormat"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbPositiveFormat" runat="server" meta:resourcekey="cmbPositiveFormatResource1"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                <asp:Label id="lblNegativeFormat" runat="server" meta:resourcekey="lblNegativeFormatResource1" Text="Negative Format" AssociatedControlID="cmbNegativeFormat"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbNegativeFormat" runat="server" meta:resourcekey="cmbNegativeFormatResource1"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
        </div>
        
        <div class="formpanel formpanel_padding">
            <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        </div>
        
        <div id="staticCurrencies" runat="server" visible="false">
            <div class="formpanel formpanel_padding">
		        <div class="sectiontitle">Exchange Rates</div>
                <asp:Literal ID="litexchangerates" runat="server" meta:resourcekey="litexchangeratesResource1"></asp:Literal>
		    </div>
		</div>
        
        <div class="formbuttons">
	        <asp:ImageButton id="cmdok" runat="server" 
                ImageUrl="../images/buttons/btn_save.png" meta:resourcekey="cmdokResource1" 
                onclick="cmdok_Click"></asp:ImageButton>&nbsp;&nbsp;
	        <asp:ImageButton id="cmdcancel" runat="server" 
                ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" 
                meta:resourcekey="cmdcancelResource1" onclick="cmdcancel_Click"></asp:ImageButton>
	    </div>

	</asp:Content>
