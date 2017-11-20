<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="customFields.aspx.cs" Inherits="Spend_Management.customFields" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:showCustomFieldModal(true);" class="submenuitem">Add Custom Field</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<script type="text/javascript" language="javascript">
    var fieldID = '';
    var modCustomFields = '<%=modCustomFields.ClientID %>';
    var ddlCustFieldType = '<%=ddlCustFieldType.ClientID %>';
    var ddlTablesID = '<%=ddlTables.ClientID %>';
    var ddlFieldsID = '<%=ddlFields.ClientID %>';
    var txtCustFieldName = '<%=txtCustFieldName.ClientID %>';
    var txtCustFieldDesc = '<%=txtCustFieldDesc.ClientID %>';
    var ddlDataTypeID = '<%=ddlDataType.ClientID %>';
    var pnlFieldID = '<%=pnlFieldGrid.ClientID %>';
</script>
<asp:ScriptManagerProxy runat="server" ID="smProxy">
<Scripts>
    <asp:ScriptReference Path="~/shared/javaScript/customFields.js" />
</Scripts>
<Services>
    <asp:ServiceReference Path="~/shared/webServices/svcCustomFields.asmx" />
</Services>
</asp:ScriptManagerProxy>
    <div class="formpanel">
        <div class="sectiontitle">Fields</div>
        <asp:Panel ID="pnlFieldGrid" runat="server">
            <asp:Literal ID="litFieldGrid" runat="server"></asp:Literal>
        </asp:Panel> 
    </div>
    
    <asp:Panel ID="pnlCustomFields" runat="server" CssClass="modalpopup formpanel" Style="display: none;">
        <div class="sectiontitle">Add/Edit Custom Field</div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblCustFieldType" AssociatedControlID="ddlCustFieldType" Text="Custom Field Type"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlCustFieldType"><asp:ListItem Text="Alias Field" Value="1"></asp:ListItem><asp:ListItem Text="Function Field" Value="2"></asp:ListItem></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="onecolumnsmall">
                <asp:Label runat="server" ID="lblTable" AssociatedControlID="ddlTables" Text="Table"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlTables" runat="server" onchange="LoadFields();"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div id="divAlias">
                <div class="onecolumnsmall">
                    <asp:Label runat="server" ID="lblField" AssociatedControlID="ddlFields" Text="Field"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlFields" runat="server"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                </div>
            </div>
            <div id="divFunction" style="display:none;">
                <div class="onecolumnsmall">
                    <asp:Label CssClass="mandatory" runat="server" ID="lblCustFieldName" AssociatedControlID="txtCustFieldName" Text="Name*"></asp:Label><span class="inputs"><asp:TextBox ID="txtCustFieldName" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqCustFieldName" Text="*" runat="server" ControlToValidate="txtCustFieldName" ErrorMessage="Please enter a value into the name" ValidationGroup="vgCustomFieldName"></asp:RequiredFieldValidator></span>
                </div>
                <div class="twocolumn">
                 <asp:Label runat="server" ID="lblDataType" AssociatedControlID="ddlDataType" Text="Data Type"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlDataType" runat="server"><asp:ListItem Value="C" Text="Currency"></asp:ListItem>
                        <asp:ListItem Value="D" Text="Date"></asp:ListItem>
                        <asp:ListItem Value="M" Text="Money"></asp:ListItem>
                        <asp:ListItem Value="N" Text="Integer"></asp:ListItem>
                        <asp:ListItem Value="S" Text="String"></asp:ListItem>
                        <asp:ListItem Value="G" Text="Unique Identifier"></asp:ListItem>
                        <asp:ListItem Value="X" Text="Boolean"></asp:ListItem>
                        </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            </div>
            <div class="onecolumn">
                <asp:Label ID="lblCustFieldDesc" runat="server" Text="Description" AssociatedControlID="txtCustFieldDesc"></asp:Label><span class="inputs"><asp:TextBox ID="txtCustFieldDesc" runat="server" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            
        <div class="formbuttons">
                <a href="javascript:SaveCustomField();"><img src="/shared/images/buttons/btn_save.png" alt="OK" /></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdCustomFieldsCancel" ImageUrl="~/shared/images/buttons/cancel_up.gif" runat="server" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modCustomFields" runat="server" TargetControlID="lnkCustomFields" PopupControlID="pnlCustomFields" BackgroundCssClass="modalBackground" CancelControlID="cmdCustomFieldsCancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkCustomFields" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    
</asp:Content>
