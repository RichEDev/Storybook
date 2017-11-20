<%@ Page Language="VB" MasterPageFile="~/FWMaster.master" AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.FWParams" Title="Framework Parameters" Codebehind="FWParams.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:Panel runat="server" ID="panelParamList">
        <div class="inputpanel">
            <asp:Label runat="server" ID="lblStatusMessage"></asp:Label>
        </div>
        <div class="inputpanel">
            <div class="inputpaneltitle">
                <asp:Literal ID="litBrandName" runat="server" Text="Framework"></asp:Literal> Parameters</div>
            <table>
                <tr>
                    <td class="labeltd">
                        Location Filter
                    </td>
                    <td class="inputtd">
                        <asp:DropDownList runat="server" ID="lstLocation" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:Literal runat="server" ID="litParams"></asp:Literal>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png"
                CausesValidation="false" /></div>
    </asp:Panel>
    <asp:Panel runat="server" ID="panelEditFields" Visible="false">
        <div class="inputpanel">
            <table>
                <tr>
                    <td class="labeltd">
                        Parameter
                    </td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtParameter" TabIndex="1" ReadOnly="True"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqParameter" runat="server" ControlToValidate="txtParameter"
                            ErrorMessage="Parameter field is mandatory">***</asp:RequiredFieldValidator>
                        <cc1:ValidatorCalloutExtender ID="reqexParameter" runat="server" TargetControlID="reqParameter">
                        </cc1:ValidatorCalloutExtender>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Value
                    </td>
                    <td class="inputtd">
                        <asp:TextBox runat="server" ID="txtValue" TabIndex="2"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="./buttons/update.gif" />&nbsp;
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="False" />
        </div>
    </asp:Panel>
</asp:Content>
