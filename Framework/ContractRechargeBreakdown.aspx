<%@ Page Language="vb" AutoEventWireup="false" Inherits="Framework2006.ContractRechargeBreakdown"
    CodeFile="ContractRechargeBreakdown.aspx.vb"
    MasterPageFile="~/FWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkAddClient">Add Client</asp:LinkButton>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkBulkEdit">Bulk Edit</asp:LinkButton>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkOneOff">One-time Charge</asp:LinkButton>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkGenerate">Generate Selected</asp:LinkButton>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkGenerateAll">Generate All</asp:LinkButton>
    <cc1:ConfirmButtonExtender ID="cnfexGenerateAll" runat="server" TargetControlID="lnkGenerateAll"
        ConfirmText="Click OK to generate payments for whole contract">
    </cc1:ConfirmButtonExtender>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkSelectAll" Visible="false">Select All</asp:LinkButton>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkDeselectAll" Visible="false">Deselect All</asp:LinkButton>
    <asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkOnOffLine">On-Off Line Dates</asp:LinkButton>
    <a href="./help_text/default_csh.htm#1120" target="_blank" class="submenuitem">Help</a>
</asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <script language="javascript" type="text/javascript" src="rechargescript.js"></script>

    <script language="javascript">
			function CallCP(cpId)
			{
				var idx;
				//alert('called CallCP with id ' + cpId);
				url='ContractRechargeBreakdown.aspx?action=callback&cpid=' + cpId;
				//alert('URL = ' + url);
				
				var res;
				var data;
				
				res = doCallBack(url,data);
				
				//alert('returned from doCallBack();');
				//alert('res = ' + res);
				document.getElementById('CP' + cpId).innerHTML = res;
			}
			
			function doCallBack(url, dataToSend) 
			{ 
				var xmlRequest; 
								
				try 
				{ 
					xmlRequest = new XMLHttpRequest(); 
				} 
				catch (e) 
				{ 
					try 
					{ 
						xmlRequest = new ActiveXObject("Microsoft.XMLHTTP"); 
					} 
					catch (f) 
					{ 
						xmlRequest = null; 
					} 
				} 
				xmlRequest.open("POST",url,false); 
				xmlRequest.setRequestHeader("Content-Type","application/x-www-form-urlencoded"); 
				xmlRequest.send(dataToSend); 
				return xmlRequest.responseText;
}
    </script>

    <asp:Panel runat="server" ID="RTFilterPanel">
        <div class="inputpanel">
            <div class="inputpaneltitle">
                View Filter</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblFilter" runat="server">Filter:</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtFilter" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:ImageButton ID="cmdRefresh" runat="server" ImageUrl="./buttons/refresh.gif"
                            ToolTip="Apply filter and refresh list" AlternateText="Refresh"></asp:ImageButton></td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="RTDisplayPanel">
        <div class="inputpanel">
            <asp:Literal ID="litMessage" runat="server"></asp:Literal></div>
        <div class="inputpanel">
            <div class="inputpaneltitle">
                <asp:Label runat="server" ID="lblAddCustomer">Add New Recharge Client</asp:Label></div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label runat="server" ID="lblAddClient">Select client</asp:Label></td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstEntityList" runat="server">
                        </asp:DropDownList></td>
                    <td>
                        <asp:ImageButton runat="server" ID="cmdAddClient" ImageUrl="./buttons/update.gif" /></td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:Literal runat="server" ID="litReturnCount"></asp:Literal>
        </div>
        <div class="inputpanel">
            <asp:Panel runat="server" ID="RTData">
            </asp:Panel>
            &nbsp;&nbsp;
            <asp:Panel runat="server" ID="RTDataButtons">
                <div>
                    <asp:ImageButton runat="server" ID="cmdTemplateUpdate" ImageUrl="./buttons/update.gif" />
                    <asp:ImageButton runat="server" ID="cmdTemplateCancel" ImageUrl="./buttons/cancel.gif"
                        CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.gif"
                CausesValidation="false" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="RTGeneratePanel" Visible="false">
        <div class="inputpanel">
            <asp:Literal runat="server" ID="litGenMessage"></asp:Literal>
        </div>
        <div class="inputpanel">
            <div class="inputpaneltitle">
                Recharge Payment Generation</div>
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblGenStart" runat="server">Start Date :</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtStartDate"></asp:TextBox>
                    </td>
                    <td>
                        <asp:ImageButton ID="cmdCalendarFrom" runat="server" ImageUrl="~/icons/16/plain/calendar.gif"
                            CausesValidation="False" />
                        <cc1:CalendarExtender ID="calexGenStart" runat="server" BehaviorID="calexGenStart"
                            Format="dd/MM/yyyy" PopupButtonID="cmdCalendarFrom" TargetControlID="txtStartDate">
                        </cc1:CalendarExtender>
                        <asp:RequiredFieldValidator runat="server" ID="reqSD" ControlToValidate="txtStartDate"
                            Text="**" ErrorMessage="Start Date is mandatory"></asp:RequiredFieldValidator>
                        <asp:CompareValidator runat="server" ID="cmpSD" Operator="DataTypeCheck" Type="Date"
                            Text="**" ErrorMessage="Date format is invalid" ControlToValidate="txtStartDate"></asp:CompareValidator>
                        <cc1:ValidatorCalloutExtender runat="server" ID="cmpexSD" TargetControlID="cmpSD">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td class="labeltd">
                        <asp:Label ID="lblGenEnd" runat="server">End Date:</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtEndDate"></asp:TextBox>
                    </td>
                    <td>
                        <asp:ImageButton ID="cmdCalendarTo" runat="server" ImageUrl="~/icons/16/plain/calendar.gif"
                            CausesValidation="False" />
                        <cc1:CalendarExtender ID="calexGenEnd" runat="server" Format="dd/MM/yyyy" PopupButtonID="cmdCalendarTo"
                            TargetControlID="txtEndDate">
                        </cc1:CalendarExtender>
                        <asp:RequiredFieldValidator runat="server" ID="reqED" ControlToValidate="txtEndDate"
                            Text="**" ErrorMessage="End Date is mandatory"></asp:RequiredFieldValidator>
                        <asp:CompareValidator runat="server" ID="cmpSDED" ControlToValidate="txtEndDate"
                            ControlToCompare="txtStartDate" Operator="GreaterThan" ErrorMessage="Start Date must preceed the End Date"
                            Type="Date">**</asp:CompareValidator>
                        <cc1:ValidatorCalloutExtender runat="server" ID="reqexED" TargetControlID="reqED">
                        </cc1:ValidatorCalloutExtender>
                        <cc1:ValidatorCalloutExtender runat="server" ID="cmpexSDED" TargetControlID="cmpSDED">
                        </cc1:ValidatorCalloutExtender>
                        <asp:CompareValidator runat="server" ID="cmpED" ControlToValidate="txtEndDate" Type="Date"
                            Operator="DataTypeCheck" Text="**" ErrorMessage="Date Format is invalid"></asp:CompareValidator>
                        <cc1:ValidatorCalloutExtender runat="server" ID="cmpexED" TargetControlID="cmpED">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                    <td>
                        <asp:ImageButton runat="server" ID="cmdGenDateRefresh" ImageUrl="./buttons/refresh.gif"
                            Visible="False" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdGenUpdate" ImageUrl="./buttons/update.gif" />
            <asp:ImageButton runat="server" ID="cmdGenCancel" ImageUrl="./buttons/cancel.gif"
                CausesValidation="False" />
        </div>
    </asp:Panel>
    <div class="inputpanel">
        <asp:TextBox ID="hiddenEntityList" runat="server" Visible="False"></asp:TextBox></div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentleft">
    <div class="panel">
        <div class="paneltitle">
            Navigation</div>
        <asp:LinkButton runat="server" ID="lnkCDnav" CssClass="submenuitem" CausesValidation="False">Contract Details</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkCAnav" CssClass="submenuitem" CausesValidation="False">Additional Details</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkCPnav" CssClass="submenuitem" CausesValidation="False">Contract Products</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkIDnav" CssClass="submenuitem" CausesValidation="False">Invoice Details</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkIFnav" CssClass="submenuitem" CausesValidation="False">Invoice Forecasts</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkNSnav" CssClass="submenuitem" CausesValidation="False">Note Summary</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkLCnav" CssClass="submenuitem" Visible="false"
            CausesValidation="False">Linked Contracts</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkCHnav" CssClass="submenuitem" CausesValidation="False">Contract History</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkRTnav" CssClass="submenuitem" CausesValidation="False">Recharge Template</asp:LinkButton>
        <asp:LinkButton runat="server" ID="lnkRPnav" CssClass="submenuitem" CausesValidation="False">Recharge Payments</asp:LinkButton>
    </div>
</asp:Content>
