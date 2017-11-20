<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthoriserLevelDetails.aspx.cs"  MasterPageFile="~/masters/smForm.master" Inherits="Spend_Management.AuthoriserLevelDetails" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1" />
    <asp:ScriptManagerProxy runat="server" ID="smp">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.AuthoriserLevels.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAuthoriserLevel.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
     <script type="text/javascript">
         var txtAmount = '<%=txtAmount.ClientID %>';
         var txtDescription = '<%=txtDescription.ClientID %>';
         $(document).ready(function () {
             $('#<%=txtAmount.ClientID%>').on("keyup input change", function (e) {
                 this.value = this.value.replace(/[^0-9\.]/g, '');
             });
             SEL.Common.SetTextAreaMaxLength();
            
         });
         </script>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">
            <asp:Label runat="server" ID="lblAuthoriserLevelHeader" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label ID="lblAmount" runat="server" AssociatedControlID="txtAmount" meta:resourcekey="lblteamnameResource1" CssClass="mandatory">Amount*</asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtAmount" runat="server" MaxLength="14" meta:resourcekey="txtteamnameResource1" CssClass="fillspan">
                </asp:TextBox></span>
            <span class="inputicon"></span><span class="inputtooltipfield"><img onmouseover="SEL.Tooltip.Show('2C2E2414-C441-446F-9DDF-4090B134193C', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ID="rfvAuthoriserLevel" runat="server" ValidationGroup="AuthoriserLevel" ErrorMessage="Please enter an Amount" ControlToValidate="txtAmount" meta:resourcekey="reqteamResource1">*</asp:RequiredFieldValidator></span>
        </div>
        <div class="onecolumn">
            <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription"><p class="labeldescription" style="margin:0px;">Description</p></asp:Label>
            <span class="inputs">
                <asp:TextBox ID="txtDescription" textareamaxlength="4000" runat="server" TextMode="MultiLine" ></asp:TextBox>
            </span>
        </div>
        <div class="formbuttons">
         <helpers:CSSButton  Text="save" OnClientClick="SEL.AuthoriserLevel.Menu.Save(); return false;" runat="server"  ValidationGroup="AuthoriserLevel" CausesValidation="true" UseSubmitBehavior="false" />
             <helpers:CSSButton Text="cancel" OnClientClick="SEL.AuthoriserLevel.Menu.CancelAuthoriserLevel(); return false;" runat="server" />
        </div>
    </div>
</asp:Content>
