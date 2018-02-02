
<%@ Page language="c#" Inherits="expenses.information.holidays" MasterPageFile="~/exptemplate.master" Codebehind="holidays.aspx.cs" %>

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
                        SEL.Grid.deleteGridRow('gridHolidays', holidayid);
					}
				}
</script>				
	
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>

    </asp:Content>


