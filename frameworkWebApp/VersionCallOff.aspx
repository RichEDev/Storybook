<%@ Page MasterPageFile="~/FWMaster.master" StylesheetTheme="FrameworkTheme" Language="vb"
    AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.VersionCallOff" SmartNavigation="True" Codebehind="VersionCallOff.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <div class="inputpanel">
    <asp:Label ID="lblErrorString" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="lblTitle" runat="server">title</asp:Label></div>
        <igtbl:UltraWebGrid ID="CallOffGrid" runat="server">
            <Bands>
                <igtbl:UltraGridBand>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout JavaScriptFileName="/ig_common/webgrid2/scripts/ig_WebGrid.js" Version="2.00"
                Name="CallOffGrid">
                <RowStyleDefault BorderWidth="0px">
                </RowStyleDefault>
                <ImageUrls ImageDirectory="/ig_common/webgrid2/Images/"></ImageUrls>
            </DisplayLayout>
        </igtbl:UltraWebGrid>
    </div>
    <div class="inputpanel">
        <asp:Panel ID="panelEditFields" runat="server" Visible="false">
        <table>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblQuantity" runat="server">quantity used</asp:Label></td>
                <td class="inputtd">
                    <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox></td>
                <td>
                    <asp:CompareValidator ID="cmpQuantity" runat="server" Text="***" ErrorMessage="error"
                        ControlToValidate="txtQuantity" Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator></td>
                <td class="labeltd">
                    <asp:Label ID="lblLocale" runat="server">locale</asp:Label></td>
                <td class="inputtd">
                    <asp:TextBox ID="txtLocale" runat="server" MaxLength="60" Wrap="False"></asp:TextBox><asp:DropDownList
                        ID="lstUpgradeVersions" runat="server" Visible="False">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblComment" runat="server">comment</asp:Label></td>
                <td class="inputtd" colspan="4">
                    <asp:TextBox ID="txtComment" runat="server" MaxLength="100" Width="100%" TextMode="MultiLine"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="labeltd">
                    <asp:Label ID="lblDateObtained" runat="server">date obtained</asp:Label></td>
                <td class="inputtd">
                    <igsch:WebDateChooser ID="dateObtained" runat="server"
                        Height=" " NullDateLabel=" ">
                        <EditStyle>
                        </EditStyle>
                        <ExpandEffects ShadowColor="LightGray"></ExpandEffects>
                    </igsch:WebDateChooser>
                </td>
                <td class="inputtd" colspan="3"></td>
            </tr>
        </table>
        </asp:Panel>
    </div>
    <div class="inputpanel">
    <asp:ImageButton ID="cmdUpdate" runat="server" Visible="False"
                    ImageUrl="./buttons/update.gif" AlternateText="Update"></asp:ImageButton>&nbsp;
    <asp:ImageButton ID="cmdCancel" runat="server" ImageUrl="./buttons/cancel.gif" AlternateText="Close" CausesValidation="False">
                </asp:ImageButton>
    </div>
    <table id="InnerTable" cellspacing="0">
        <tr>
            <td align="center">
                </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
<asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkNew">New</asp:LinkButton>
<asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkDelete">Delete</asp:LinkButton>
<asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkUpgrade">Upgrade</asp:LinkButton>
</asp:Content>
