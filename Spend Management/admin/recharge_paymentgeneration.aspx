<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="admin_recharge_paymentgeneration"
    Title="Untitled Page" Codebehind="recharge_paymentgeneration.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Recharge Payment Generation</div>
        <table>
            <tr>
                <td class="labeltd">
                    Contract
                </td>
                <td class="inputtd">
                    <asp:TextBox runat="server" ID="txtContract" TabIndex="1" ValidationGroup="contractvalidation"></asp:TextBox>
                </td>
                <td>
                    <asp:UpdatePanel runat="server" ID="contractsearchUpdatePanel">
                        <ContentTemplate>
                            <asp:ImageButton runat="server" ID="cmdContactSearch" ImageUrl="~/icons/16/plain/find.gif"
                                ValidationGroup="contractvalidation" OnClick="cmdContactSearch_Click" /></ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <asp:RequiredFieldValidator runat="server" ID="reqContract" ControlToValidate="txtContract"
                        ErrorMessage="Wildcard filter value is required" Text="*" ValidationGroup="contractvalidation"
                        SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <cc1:ValidatorCalloutExtender ID="reqexContract" runat="server" TargetControlID="reqContract">
                    </cc1:ValidatorCalloutExtender>
                </td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label runat="server" ID="lblSupplier"></asp:Label>
                </td>
                <td class="inputtd">
                    <asp:TextBox runat="server" ID="txtSupplier" TabIndex="2" ValidationGroup="suppliervalidation"></asp:TextBox>
                </td>
                <td>
                    <asp:UpdatePanel runat="server" ID="suppliersearchUpdatePanel">
                        <ContentTemplate>
                            <asp:ImageButton runat="server" ID="cmdSupplierSearch" ImageUrl="~/icons/16/plain/find.gif"
                                ValidationGroup="suppliervalidation" OnClick="cmdSupplierSearch_Click" /></ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <asp:RequiredFieldValidator runat="server" ID="reqSupplier" ControlToValidate="txtSupplier"
                        ErrorMessage="Wildcard filter value is required" Text="*" ValidationGroup="suppliervalidation"
                        SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <cc1:ValidatorCalloutExtender ID="reqexSupplier" runat="server" TargetControlID="reqSupplier">
                    </cc1:ValidatorCalloutExtender>
                </td>
            </tr>
        </table>
    </div>
    <div class="inputpanel">
    <asp:Literal runat="server" ID="litMessage"></asp:Literal>
    </div>
    <asp:UpdatePanel runat="server" ID="resultUpdatePanel">
        <ContentTemplate>
            <asp:Panel runat="server" ID="resultPanel" Visible="false">
                <div class="inputpanel">
                <div class="inputpaneltitle">Generate for Period</div>
                    <table>
                        <tr>
                            <td class="labeltd">
                                Generate From
                            </td>
                            <td class="inputtd">
                                <asp:TextBox runat="server" ID="dateFrom" ValidationGroup="generatevalidation"></asp:TextBox>
                            </td>
                            <td>
                                <asp:CompareValidator ID="cmpFrom" runat="server" ControlToValidate="dateFrom" ErrorMessage="Invalid From date specified"
                                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="generatevalidation">*</asp:CompareValidator>
                                <cc1:ValidatorCalloutExtender ID="cmpexFrom" runat="server" TargetControlID="cmpFrom">
                                </cc1:ValidatorCalloutExtender>
                                <cc1:ValidatorCalloutExtender ID="reqexFrom" runat="server" TargetControlID="reqFrom">
                                </cc1:ValidatorCalloutExtender>
                                <cc1:CalendarExtender ID="calexFrom" runat="server" Format="dd/MM/yyyy" TargetControlID="dateFrom">
                                </cc1:CalendarExtender>
                                <asp:RequiredFieldValidator ID="reqFrom" runat="server" ControlToValidate="dateFrom"
                                    ErrorMessage="From date is mandatory" SetFocusOnError="True" ValidationGroup="generatevalidation">*</asp:RequiredFieldValidator>
                            </td>
                            <td class="labeltd">
                                Generate To
                            </td>
                            <td class="inputtd">
                                <asp:TextBox runat="server" ID="dateTo" ValidationGroup="generatevalidation"></asp:TextBox>
                            </td>
                            <td>
                                <cc1:CalendarExtender ID="calexTo" runat="server" Format="dd/MM/yyyy" TargetControlID="dateTo">
                                </cc1:CalendarExtender>
                                <cc1:ValidatorCalloutExtender ID="cmpexTo" runat="server" TargetControlID="cmpTo">
                                </cc1:ValidatorCalloutExtender>
                                <cc1:ValidatorCalloutExtender ID="reqexTo" runat="server" TargetControlID="reqTo">
                                </cc1:ValidatorCalloutExtender>
                                <asp:CompareValidator ID="cmpTo" runat="server" ControlToValidate="dateTo" ErrorMessage="Invalid To date specified"
                                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="generatevalidation">*</asp:CompareValidator>
                                <asp:RequiredFieldValidator ID="reqTo" runat="server" ControlToValidate="dateTo"
                                    ErrorMessage="To date is mandatory" SetFocusOnError="True" ValidationGroup="generatevalidation">*</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="cmpFromTo" runat="server" ControlToCompare="dateFrom" ControlToValidate="dateTo"
                                    ErrorMessage="From date must preceed To date" Operator="GreaterThan" SetFocusOnError="True"
                                    Type="Date" ValidationGroup="generatevalidation">*</asp:CompareValidator>
                                <cc1:ValidatorCalloutExtender ID="cmpexFromTo" runat="server" TargetControlID="cmpFromTo">
                                </cc1:ValidatorCalloutExtender>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <div class="inputpanel">
                <asp:Panel runat="server" ID="searchResults">
                </asp:Panel>
            </div>
            <div class="inputpanel">
                <asp:ImageButton runat="server" ID="cmdGenerate" ImageUrl="~/Buttons/ok.gif" Visible="false"
                    OnClick="cmdGenerate_Click" />&nbsp;&nbsp;
                <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/Buttons/cancel.gif" CausesValidation="false"
                    Visible="false" OnClick="cmdCancel_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" CausesValidation="false" ImageUrl="~/Buttons/page_close.gif"
            OnClick="cmdClose_Click" />
    </div>
    <asp:UpdateProgress runat="server" ID="ajaxLoadingPanelConSearch" DisplayAfter="50"
        AssociatedUpdatePanelID="contractsearchUpdatePanel">
        <ProgressTemplate>
            <div class="progresspanel">
                <img src=".././images/loading.gif" alt="Please wait, loading..." />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress runat="server" ID="ajaxLoadingPanelSupplierSearch" 
        DisplayAfter="50" AssociatedUpdatePanelID="suppliersearchUpdatePanel">
        <ProgressTemplate>
            <div class="progresspanel">
                <img src=".././images/loading.gif" alt="Please wait, loading..." />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
