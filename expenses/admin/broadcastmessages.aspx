<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" Inherits="admin_broadcastmessages" Title="Untitled Page" Codebehind="broadcastmessages.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
<a class="submenuitem" href="aebroadcastmessage.aspx">
    <asp:Label ID="lbladdbroadcast" runat="server" Text="Add Broadcast Message" meta:resourcekey="lbladdbroadcastResource1"></asp:Label></a>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
<script language="javascript" type="text/javascript">
function deleteBroadcast(broadcastid)
				{
					if (confirm('Are you sure you wish to delete the selected Broadcast Message?'))
					{
						PageMethods.deleteBroadcast(accountid,broadcastid);

                        SEL.Grid.deleteGridRow('gridBroadcastMessages', broadcastid);
					}
				}
				</script>
				
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
</asp:Content>

