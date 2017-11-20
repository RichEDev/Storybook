
<%@ Page language="c#" Inherits="expenses.information.holidays" MasterPageFile="~/exptemplate.master" Codebehind="holidays.aspx.cs" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<a href="aeholiday.aspx" class="submenuitem">Add Holiday</a></asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script language="javascript" type="text/javascript">

				function deleteHoliday(holidayid)
				{
					if (confirm('Are you sure you wish to delete the selected holiday?'))
					{
						PageMethods.deleteHoliday(accountid, holidayid);
						var grid = igtbl_getGridById(contentID + 'gridholidays');
						
				    grid.Rows.remove(grid.getActiveRow().getIndex());
					}
				}
</script>				
	
    <div class="inputpanel table-border">
        <igtbl:UltraWebGrid ID="gridholidays" runat="server" SkinID="gridskin" OnInitializeLayout="gridholidays_InitializeLayout" OnInitializeRow="gridholidays_InitializeRow" meta:resourcekey="gridholidaysResource1">
            
        </igtbl:UltraWebGrid>
    </div>

        <div class="formpanel formpanel_padding">
            <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
        </div>

    </asp:Content>


