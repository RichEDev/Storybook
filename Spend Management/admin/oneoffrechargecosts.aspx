<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="admin_oneoffrechargecosts" Title="One Off Charges" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="oneoffrechargecosts.aspx.cs" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
<script language="javascript" type="text/javascript">
function EditCharge(editid)
{
    alert('Called EditCharge(' + editid + ');');
}

function DelInvForecastRec(InvForecastId,contractId)
{
}

function fnDelOOCComplete(strResult)
{
    if(strResult == "OK")
    {
        document.location = "oneoffrechargecosts.aspx";
    }
    else
    {
        alert(strResult);
    }
    return true;
}

function DeleteCharge(deleteid)
{
    var sPrompt = 'Click OK to confirm deletion of the selected charge';
    
    if(confirm(sPrompt))
    {
        PageMethods.DeleteOOC(deleteid, fnDelOOCComplete);
    }
}
</script>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="../svcAutoComplete.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel runat="server" ID="OOC_UpdatePanel">
        <ContentTemplate>
            <div class="inputpanel">
                <asp:Label runat="server" ID="lblMessage" ForeColor="Red" meta:resourcekey="lblMessageResource1"></asp:Label>
            </div>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    One-off Cost Recharge</div>
                <table>
                    <tr>
                        <td class="labeltd">
                            Contract</td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtContract" ValidationGroup="newooc" meta:resourcekey="txtContractResource1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqContract" runat="server" ControlToValidate="txtContract"
                                ErrorMessage="Contract is mandatory" SetFocusOnError="True" meta:resourcekey="reqContractResource1">**</asp:RequiredFieldValidator>
                            <cc1:AutoCompleteExtender ID="autocomContract" runat="server" TargetControlID="txtContract" ServicePath="../svcAutoComplete.asmx"
                                ServiceMethod="GetContracts" DelimiterCharacters="" Enabled="True">
                            </cc1:AutoCompleteExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexContract" runat="server" TargetControlID="reqContract" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            <asp:Label runat="server" ID="lblClient" meta:resourcekey="lblClientResource1">Customer</asp:Label></td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtClient" ValidationGroup="newooc" meta:resourcekey="txtClientResource1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="txtClient"
                                ErrorMessage="Field is mandatory" SetFocusOnError="True" meta:resourcekey="reqClientResource1">**</asp:RequiredFieldValidator>
                            <cc1:AutoCompleteExtender ID="autocomClient" runat="server" TargetControlID="txtClient" MinimumPrefixLength="2" ServicePath="../svcAutoComplete.asmx"
                                ServiceMethod="GetClients" Enabled="True">
                            </cc1:AutoCompleteExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexClient" runat="server" TargetControlID="reqClient" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            Date of Charge</td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtDate" ValidationGroup="newooc" meta:resourcekey="txtDateResource1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqDate" runat="server" ControlToValidate="txtDate"
                                ErrorMessage="Date field is mandatory" SetFocusOnError="True" meta:resourcekey="reqDateResource1">**</asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cmpDate" runat="server" ControlToValidate="txtDate" ErrorMessage="Invalid date format specified"
                                Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" meta:resourcekey="cmpDateResource1">**</asp:CompareValidator>
                            <cc1:CalendarExtender ID="calexDate" runat="server" TargetControlID="txtDate" Format="dd/MM/yyyy" Enabled="True">
                            </cc1:CalendarExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexDate" runat="server" TargetControlID="reqDate" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            Cost</td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtCost" ValidationGroup="newooc" meta:resourcekey="txtCostResource1"></asp:TextBox>
                            <asp:CompareValidator ID="cmpCost" runat="server" ControlToValidate="txtCost" ErrorMessage="Invalid monetary value entered"
                                Operator="DataTypeCheck" SetFocusOnError="True" Type="Double" meta:resourcekey="cmpCostResource1">**</asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="reqCost" runat="server" ControlToValidate="txtCost"
                                ErrorMessage="Cost value is mandatory" SetFocusOnError="True" meta:resourcekey="reqCostResource1">**</asp:RequiredFieldValidator>
                            <cc1:ValidatorCalloutExtender ID="mexCost" runat="server" TargetControlID="cmpCost" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexCost" runat="server" TargetControlID="reqCost" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/Buttons/update.gif" OnClick="cmdUpdate_Click" ValidationGroup="newooc" meta:resourcekey="cmdUpdateResource1" />
        <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/Buttons/cancel.gif" CausesValidation="False"
            OnClick="cmdCancel_Click" meta:resourcekey="cmdCancelResource1" />
    </div>
    <asp:UpdatePanel runat="server" ID="CostList_UpdatePanel">
        <ContentTemplate>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    Current One-off Charges</div>
                <table>
                    <tr>
                        <td class="labeltd">
                            Display Start Date</td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtStartDate" runat="server" ValidationGroup="ooclist" meta:resourcekey="txtStartDateResource1"></asp:TextBox>
                            <cc1:CalendarExtender ID="calexStart" runat="server" Format="dd/MM/yyyy" TargetControlID="txtStartDate" Enabled="True">
                            </cc1:CalendarExtender>
                            <asp:CompareValidator ID="cmpStart" runat="server" ControlToValidate="txtStartDate"
                                ErrorMessage="Invalid date format entered" Operator="DataTypeCheck" Type="Date" meta:resourcekey="cmpStartResource1" SetFocusOnError="True" Text="**" ValidationGroup="ooclist"></asp:CompareValidator>
                            <cc1:ValidatorCalloutExtender ID="cmpexStart" runat="server" TargetControlID="cmpStart" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                        <td class="labeltd">
                            Display End Date</td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtEndDate" runat="server" ValidationGroup="ooclist" meta:resourcekey="txtEndDateResource1"></asp:TextBox>
                            <cc1:CalendarExtender ID="calexEndDate" runat="server" Format="dd/MM/yyyy" TargetControlID="txtEndDate" Enabled="True">
                            </cc1:CalendarExtender>
                            <asp:CompareValidator ID="cmpEnd" runat="server" ControlToValidate="txtEndDate" ErrorMessage="Invalid Date format entered"
                                Operator="DataTypeCheck" Type="Date" meta:resourcekey="cmpEndResource1" SetFocusOnError="True" Text="**" ValidationGroup="ooclist"></asp:CompareValidator>
                            <asp:CompareValidator ID="cmpSEDates" runat="server" ControlToCompare="txtStartDate"
                                ControlToValidate="txtEndDate" ErrorMessage="Start date must preceed end date"
                                Operator="GreaterThanEqual" SetFocusOnError="True" Type="Date" ValidationGroup="ooclist">**</asp:CompareValidator>
                            <cc1:ValidatorCalloutExtender ID="cmpexEnd" runat="server" TargetControlID="cmpEnd" Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                            <cc1:ValidatorCalloutExtender ID="cmpexSEDates" runat="server" TargetControlID="cmpSEDates">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                        <td><asp:ImageButton runat="server" ID="cmdRefresh" ImageUrl="~/Buttons/refresh.gif" AlternateText="Refresh" OnClick="cmdRefresh_Click" ValidationGroup="ooclist" meta:resourcekey="cmdRefreshResource1" /></td>
                    </tr>
                </table>
            </div>
            <div class="inputpanel">
                <asp:Panel runat="server" ID="panelCostList" meta:resourcekey="panelCostListResource1">
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
