<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.ContractProductInfo" Codebehind="ContractProductInfo.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript">
			function Check4CloseForm()
			{
				var fullparams;
				var param;
				
				fullparams = window.location.search;
				// alert('fullparams = ' + fullparams);
				param = fullparams.replace('?action=','');
				// alert('param extracted = ' + param);
				if(param=='close')
				{
					// alert('going to close the window then!!');
					window.close();
				}
			}
    </script>

    <div class="inputpanel">
        <div class="inputpaneltitle">Product Information
            <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
        <div>
            <asp:Label ID="lblErrorMsg" runat="server"></asp:Label></div>
        <asp:TextBox ID="txtInfo" runat="server" TextMode="MultiLine" Height="300px" Width="600px"></asp:TextBox>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />
        <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="False" />
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
</asp:Content>

