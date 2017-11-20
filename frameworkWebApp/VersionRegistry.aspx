<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.VersionRegistry" Codebehind="VersionRegistry.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <script language="vb" runat="server">
        Sub ChangeHint(ByVal sender As Object, ByVal e As EventArgs)
            Select Case lstType.SelectedItem.Value
                Case 0 ' Purchase
                    txtHint.Text = "TIP: The quantity field should reflect the number of licences purchase for this version."
                Case 1 ' Upgrade
                    txtHint.Text = "TIP: The quantity field should reflect the number of licences being upgrade from this version."
                Case 2 ' Adjustment
                    txtHint.Text = "TIP: The quantity field should reflect the number of licences being amended (e.g. -3 to take away, 3 to add on)."
                Case Else
                    txtHint.Text = ""
            End Select
        End Sub
    </script>

    <div class="inputpanel">
        <asp:Label ID="lblErrorString" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Version Registry
            <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
    </div>
    <asp:Panel ID="panelEditFields" runat="server" Visible="false">
        <div class="inputpanel">
            <table>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblVersion" runat="server">version</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtVersion" TabIndex="1" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqVersion" runat="server" Enabled="False" ErrorMessage="A version description must be specified"
                            ControlToValidate="txtVersion">***</asp:RequiredFieldValidator></td>
                    <td class="labeltd">
                        <asp:Label ID="lblFirstObtained" runat="server">first obained</asp:Label></td>
                    <td class="inputtd">
                        <igtxt:WebDateTimeEdit ID="dateFirstObtained" TabIndex="2" runat="server" ToolTip="Provide date of when the version was first obtained"
                            DisplayModeFormat="d" SelectionOnFocus="CaretToBeginning">
                        </igtxt:WebDateTimeEdit>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblQuantity" runat="server">quantity</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtQuantity" TabIndex="3" runat="server" ToolTip="Specify number being purchased, upgraded etc."></asp:TextBox></td>
                    <td>
                        <asp:CompareValidator ID="cmpQuantity" runat="server" ErrorMessage="error" ControlToValidate="txtQuantity"
                            Type="Integer" Operator="DataTypeCheck">***</asp:CompareValidator></td>
                    <td class="labeltd">
                        <asp:Label ID="lblComment" runat="server">comment</asp:Label></td>
                    <td class="inputtd">
                        <asp:TextBox ID="txtComment" TabIndex="6" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblType" runat="server">type</asp:Label></td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstType" TabIndex="4" runat="server" OnSelectedIndexChanged="ChangeHint">
                            <asp:ListItem Value="0" Selected="True">Purchase</asp:ListItem>
                            <asp:ListItem Value="1">Upgrade</asp:ListItem>
                            <asp:ListItem Value="2">Adjustment</asp:ListItem>
                        </asp:DropDownList></td>
                    <td align="center">
                        <asp:Label ID="lblTo" runat="server" Visible="False">to</asp:Label></td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstToVersion" TabIndex="5" runat="server">
                        </asp:DropDownList></td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label ID="lblReseller" runat="server">reseller</asp:Label></td>
                    <td class="inputtd">
                        <asp:DropDownList ID="lstReseller" TabIndex="7" runat="server">
                        </asp:DropDownList></td>
                    <td colspan="3">
                    </td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="./buttons/update.gif" />&nbsp;
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="panelDataTable">
        <div class="inputpanel">
            <asp:Literal runat="server" ID="litVRGrid"></asp:Literal>
        </div>
    </asp:Panel>
    <div class="inputpanel">
        <asp:Label ID="txtHint" runat="server"></asp:Label></div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png"
            CausesValidation="false" />
    </div>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="lnkNew" CssClass="submenuitem">New</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkHistory" CssClass="submenuitem">View History</asp:LinkButton>
</asp:Content>
