<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" Inherits="admin_broadcastmessages" Title="Untitled Page" Codebehind="broadcastmessages.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
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
						var grid = igtbl_getGridById(contentID + 'gridbroadcast');
						
				        grid.Rows.remove(grid.getActiveRow().getIndex());
					}
				}
				</script>
				
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">

    <div class="inputpanel table-border">
    <igtbl:UltraWebGrid ID="gridbroadcast" runat="server" SkinID="gridskin" OnInitializeLayout="gridbroadcast_InitializeLayout" OnInitializeRow="gridbroadcast_InitializeRow" meta:resourcekey="gridbroadcastResource1">
        <DisplayLayout>
            <ActivationObject BorderColor="" BorderWidth="">
            </ActivationObject>
        </DisplayLayout>
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
    </igtbl:UltraWebGrid>
    </div>
    <div class="formpanel" style="padding:0px;">
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
    </div>
</asp:Content>

