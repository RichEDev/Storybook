<%@ Page Language="VB" MasterPageFile="~/FWMaster.master"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.ContractHistory" Codebehind="ContractHistory.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
    <div class="panel">
        <div class="paneltitle">
            Navigation</div>
        <asp:LinkButton runat="server" ID="lnkCDnav" CssClass="submenuitem" CausesValidation="False">Contract Details</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkCAnav" CssClass="submenuitem" CausesValidation="False">Additional Details</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkCPnav" CssClass="submenuitem" CausesValidation="False">Contract Products</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkIDnav" CssClass="submenuitem" CausesValidation="False">Invoice Details</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkIFnav" CssClass="submenuitem" CausesValidation="False">Invoice Forecasts</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkNSnav" CssClass="submenuitem" CausesValidation="False">Note Summary</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkLCnav" CssClass="submenuitem" Visible="false" CausesValidation="False">Linked Contracts</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkCHnav" CssClass="submenuitem" CausesValidation="False">Contract History</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkRTnav" CssClass="submenuitem" Visible="false"
            CausesValidation="False">Recharge Template</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkRPnav" CssClass="submenuitem" Visible="false"
            CausesValidation="False">Recharge Payments</asp:LinkButton>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">

    <script language="javascript" type="text/javascript">
    function toggle(id)
    {
        var cntl = document.getElementById('ST' + id);
        if(cntl != null)
        {
            if(cntl.style.display == 'none')
            {
                cntl.style.display = 'block';
                var img = document.getElementById('STimg' + id);
                if(img != null)
                {
                    img.src = './buttons/close.gif';
                }
                
        	    // collapse all other branches
	            var loop;
	            for(loop=0; loop<9; loop++)
	            {
	                if(loop != id)
	                {
	                    var old_cntl = document.getElementById('ST' + loop);
	                    if(old_cntl != null)
	                    {
	                        old_cntl.style.display = 'none';
	                        var img = document.getElementById('STimg' + loop);
	                        if(img != null)
	                        {
	                            img.src = './buttons/open.gif';
	                        }
	                    }
	                }
	            }
            }
            else
            {
                cntl.style.display = 'none';
                var img = document.getElementById('STimg' + id);
                if(img != null)
                {
                    img.src = './buttons/open.gif';
                }
            }
        }
    }
    </script>

    <asp:Literal runat="server" ID="litHistory"></asp:Literal>
    <div class="formpanel formpanel_padding">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png"
            CausesValidation="false" />&nbsp;
    </div>
</asp:Content>
