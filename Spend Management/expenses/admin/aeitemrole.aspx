<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="admin_aeitemrole" Codebehind="aeitemrole.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" EnableViewState="false" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentleft" Runat="Server">

    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentmain" Runat="Server">
    <script type="text/javascript" src="/expenses/javaScript/sel.ItemRoles.js?date=201803130853"></script>
        <script type="text/javascript" language="javascript">
            (function (r) {

                r.ItemRole.roleName = '#<%= this.txtrolename.ClientID %>';
                r.ItemRole.description = '#<%= this.txtdescription.ClientID %>';
                r.AssociatedExpenseItem.expenseItem = '#<%= this.ddlstExpenseItem.ClientID %>';
                r.AssociatedExpenseItem.limitWithoutReceipt = '#<%= this.txtLimitWithoutReceipt.ClientID %>';
                r.AssociatedExpenseItem.limitWithReceipt = '#<%= this.txtLimitWithReceipt.ClientID %>';
                r.AssociatedExpenseItem.addToTemplate = '#<%= this.chkAddToTemplate.ClientID %>';
            }(SEL.ItemRoles.DomIDs));
    </script>
    <div class="sm_panel">
	    <div class="sectiontitle"><asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
        <div class="twocolumn"><asp:Label id="lblname" runat="server" meta:resourcekey="lblnameResource1" AssociatedControlID="txtrolename" CssClass="mandatory" Text="Role Name*"></asp:Label><span class="inputs"><asp:textbox id="txtrolename" runat="server" MaxLength="50" meta:resourcekey="txtrolenameResource1"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:requiredfieldvalidator id="reqrolename" runat="server" ControlToValidate="txtrolename" ErrorMessage="Please enter a Role Name" meta:resourcekey="reqrolenameResource1" ValidationGroup="vgMain">*</asp:requiredfieldvalidator></span></div>
        <div class="onecolumn"><asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription">Description</asp:Label><span class="inputs"><asp:textbox id="txtdescription" runat="server" MaxLength="4000" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
    
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle"><asp:Label ID="lblallocateditems" runat="server" Text="Allocated Expense Items" meta:resourcekey="lblallocateditemsResource1"></asp:Label></div>
        <div><p><a href="javascript:SEL.ItemRoles.ExpenseItems.CreateDropDown();SEL.ItemRoles.ShowAssociatedExpenseItemModal = true;SEL.ItemRoles.ItemRole.Save();">Link Expense Item</a></p></div>
        <asp:Literal runat="server" id="litAssociatedExpenseItems"></asp:Literal>
    </div>
    <div class="formbuttons">
        <helpers:CSSButton runat="server" ID="btnSave" Text="save" OnClientClick="SEL.ItemRoles.ShowAssociatedExpenseItemModal = false;SEL.ItemRoles.ItemRole.Save(); return false;" UseSubmitBehavior="False" />
        <helpers:CSSButton runat="server" ID="btnCancel" Text="cancel" OnClientClick="document.location='itemroles.aspx';return false;" UseSubmitBehavior="False" />
    </div>
    </div>

    <div id="LinkExpenseItemModal" style="display: none; overflow: auto; max-height: 600px;">
	    <div class="sm_panel">
	        <div class="sectiontitle">
	            <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label>
	        </div>
            <div class="twocolumn"><asp:Label runat="server" Text="Expense item*" AssociatedControlID="ddlstExpenseItem" CssClass="mandatory"></asp:Label><span class="inputs"><asp:DropDownList runat="server" id="ddlstExpenseItem"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" Text="Maximum limit without receipt" AssociatedControlID="txtLimitWithoutReceipt"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtLimitWithoutReceipt"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ErrorMessage="Please enter a valid maximum limit without a receipt." ControlToValidate="txtLimitWithoutReceipt" Operator="GreaterThanEqual" ValueToCompare="0" Type="Double" meta:resourcekey="compamountResource1" ValidationGroup="vgAssociatedExpenseItem" text="*"></asp:CompareValidator></span></div>
            <div class="twocolumn"><asp:Label runat="server" Text="Maximum limit with receipt" AssociatedControlID="txtLimitWithReceipt"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtLimitWithReceipt"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ErrorMessage="Please enter a valid maximum limit with a receipt." ControlToValidate="txtLimitWithReceipt" Operator="GreaterThanEqual" ValueToCompare="0" Type="Double" meta:resourcekey="compamountResource1" ValidationGroup="vgAssociatedExpenseItem" text="*"></asp:CompareValidator></span><asp:Label runat="server" Text="Add to template" AssociatedControlId="chkAddToTemplate"></asp:Label><span class="inputs"><asp:CheckBox runat="server" id="chkAddToTemplate"></asp:CheckBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        </div>
    </div>
</asp:Content>

