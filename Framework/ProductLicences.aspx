<%@ Page Language="C#" MasterPageFile="~/FWMaster.master" AutoEventWireup="true"
    CodeFile="ProductLicences.aspx.cs" Inherits="ProductLicences" Title="Product Licences" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
    <asp:LinkButton runat="server" ID="lnkAdd" CssClass="submenuitem" OnClick="lnkAdd_Click">Add</asp:LinkButton>
    <a href="./help_text/default_csh.htm#1177" target="_blank" class="submenuitem">Help</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="shared/javascript/userdefined.js" />
        <asp:ScriptReference Path="shared/javascript/validate.js" />
    </Scripts>
    <Services>
        <asp:ServiceReference Path="shared/webServices/svcRelationshipTextbox.asmx" InlineScript="false" />
    </Services>

    </asp:ScriptManagerProxy>
    <script language="javascript" type="text/javascript">
        var ProductID;

        function DeleteLicence(prodID, licID) {
            if (confirm('Click OK to confirm deletion')) {
                ProductID = prodID;
                PageMethods.DeleteLicence(prodID, licID, DeleteLicenceComplete, null);
            }
        }

        function DeleteLicenceComplete(result) {
            if (result === -1) {
                alert('You have no permissions to delete product licences');
            }
            else {
                window.location.href = 'ProductLicences.aspx?pid=' + ProductID;
            }

        }

    </script>
    <asp:HiddenField runat="server" ID="currentProductId" />
    <div class="formpanel">
        <div class="sectiontitle">
            Current Licences</div>
        <div class="twocolumn">
            <asp:Label runat="server" ID="lblProductName" Text="Product Name" AssociatedControlID="txtProductName"></asp:Label>
            <span class="inputs">
                <asp:TextBox runat="server" ReadOnly="true" ID="txtProductName" CssClass="fillspan"></asp:TextBox></span><span
                    class="inputicon">&nbsp;</span> <span class="inputtooltip">&nbsp;</span>
            <span class="inputvalidatorfield">&nbsp;</span>
        </div>
    </div>
    <div class="formpanel">
        <asp:Label runat="server" ID="lblStatusMessage"></asp:Label></div>
    <div class="formpanel">
        <asp:Panel runat="server" ID="panelLicenceList">
        </asp:Panel>
    </div>
    <asp:Panel runat="server" ID="panelEdit">
        <div class="formpanel"><div class="sectiontitle">Licence Details</div><div class="onecolumn"><asp:Label runat="server" Text="Licence Key" ID="lblLicenceKey" AssociatedControlID="txtKey"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtKey" MaxLength="250" TabIndex="1" CssClass="fillspan"
 TextMode="MultiLine"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div><div class="onecolumn"><asp:Label runat="server" ID="lblLocation" AssociatedControlID="txtLocation" Text="Location"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtLocation" MaxLength="250" TabIndex="2" CssClass="fillspan" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div><div class="twocolumn"><asp:Label runat="server" ID="lblLicenceType" AssociatedControlID="lstProdLicenceType" Text="Licence Type"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstProdLicenceType" TabIndex="3" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" ID="lblRenewalType" AssociatedControlID="lstRenewalType" Text="Licence Renewal Type"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstRenewalType" TabIndex="4" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div><div class="twocolumn"><asp:Label runat="server" ID="lblExpiry" AssociatedControlID="dateExpiry" Text="Expiry Date"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="dateExpiry" TabIndex="5" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"><cc1:CalendarExtender ID="calexExpiry" runat="server" Format="dd/MM/yyyy" TargetControlID="dateExpiry"></cc1:CalendarExtender><asp:CompareValidator ID="cmpExpiry" runat="server" ControlToValidate="dateExpiry" ErrorMessage="Invalid Date format entered" Operator="DataTypeCheck" Type="Date" Display="Dynamic" ValidationGroup="plicences">*</asp:CompareValidator></span><asp:Label runat="server" ID="lblUnlimited" AssociatedControlID="chkUnlimited" Text="Unlimited Units?"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkUnlimited" TabIndex="6" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div><div class="twocolumn"><asp:Label runat="server" ID="lblNotifyDays" AssociatedControlID="txtNotifyDays" Text="Notify days"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtNotifyDays" TabIndex="7" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpNotifyDays" runat="server" ControlToValidate="txtNotifyDays" ErrorMessage="Numeric values only are permitted" Operator="DataTypeCheck" Type="Integer" Display="Dynamic" ValidationGroup="plicences">*</asp:CompareValidator></span><asp:Label runat="server" ID="lblNotify" AssociatedControlID="lstNotify" Text="Notify"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstNotify" TabIndex="8" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span>
<span class="inputvalidatorfield">&nbsp;</span></div><div class="twocolumn"><asp:Label runat="server" ID="lblSoftCopy" AssociatedControlID="chkSoftCopy" Text="Soft Copy Held"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkSoftCopy" TabIndex="9" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" Text="Hard Copy Held" AssociatedControlID="chkHardCopy" ID="lblHardCopy"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkHardCopy" TabIndex="10" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span> <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div><div class="twocolumn"><asp:Label runat="server" ID="lblNumLicences" AssociatedControlID="txtNumberHeld" Text="Number Licences Held"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtNumberHeld" TabIndex="11" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpNumberHeld" runat="server" ControlToValidate="txtNumberHeld" ErrorMessage="Value entered must be numeric" Operator="DataTypeCheck" Type="Integer" Display="Dynamic" ValidationGroup="plicences">*</asp:CompareValidator></span><asp:HiddenField runat="server" ID="hiddenLicenceId" /></div><asp:PlaceHolder runat="server" ID="phPLUFields"></asp:PlaceHolder></div><div class="formpanel"><asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/Buttons/update.gif" AlternateText="Update" OnClick="cmdUpdate_Click" ValidationGroup="plicences" />&nbsp;<asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/Buttons/cancel.gif" AlternateText="Cancel" CausesValidation="false" OnClick="cmdCancel_Click" /></div>
        <div style="height: 50px;">
            &nbsp;</div>
    </asp:Panel>
    <div class="formpanel">
    <div class="formbuttons">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="~/Buttons/page_close.gif"
            CausesValidation="false" AlternateText="Close" OnClick="cmdClose_Click" /></div>
    </div>
</asp:Content>
