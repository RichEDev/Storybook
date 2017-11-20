<%@ Page Language="c#" Inherits="Spend_Management.aeuserdefined" MasterPageFile="~/masters/smForm.master"
    CodeBehind="aeuserdefined.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <script type="text/javascript" language="javascript">
        var modlistitem = '<%= modlistitem.ClientID%>';
        var txtlistitem = '<%= txtlistitem.ClientID %>';
        var reqListItemText = '<%= reqListItemText.ClientID %>';
        var chkarchiveditem = '<%= chkArchiveListItem.ClientID%>';
        var cmbappliesto = '<%= cmbappliesto.ClientID %>';
        var cmbgroup = '<%= ddlstgroup.ClientID %>';
        var cmbattributetype = '<%= cmbattributetype.ClientID %>';
        var chkitemspecific = '<%= chkspecific.ClientID %>';
        var lstitems = '<%= lstitems.ClientID %>';
        var reqHyperlinkText = '<%= reqhyperlinktext.ClientID %>';
        var reqHyperlinkPath = '<%= reqhyperlinkpath.ClientID %>';
        
        var revHyperlinkPath = '<%= revHyperlinkPath.ClientID %>';
        var ddlRelatedTableID = '<%= ddlRelatedTable.ClientID %>';
        var cvRelatedTable = '<%= cvRelatedTable.ClientID %>';
        var chkattributemandatory = '<%= chkattributemandatory.ClientID %>';
        var txtgroupid = '<%=txtgroupid.ClientID %>';
        var compdefaultvalue = '<%=compdefaultvalue.ClientID %>';
        var compmaxlength = '<% =compmaxlength.ClientID %>';
        var udfMaxLengthID = '<%=txtmaxlength.ClientID %>';
        var udfMaxLengthLargeID = '<%=txtmaxlengthlarge.ClientID %>';
        var compprecision = '<% =compprecision.ClientID %>';
        var udfPrecisionID = '<%=txtprecision.ClientID %>';
        var modmatchfieldID = '<%=modmatchitem.ClientID %>';
        var custmtomatchfieldsID = '<%=custMatchFields.ClientID %>';
        var cmpDisplayFieldID = '<%=cmpDisplayField.ClientID %>';
        var cmbmtodisplayfieldID = '<%=cmbDisplayField.ClientID %>';
        var cmbfielditemlistID = '<%=cmbmatchfieldlist.ClientID %>';
        var selectedDisplayField;
        var selectedMatchFields;
        var udfNameID = '<%=txtattributename.ClientID %>';
        var udfDescID = '<%=txtattributedescription.ClientID %>';
        var udfOrderID = '<%=txtorder.ClientID %>';
        var udfDisplayFieldID = '<%=cmbDisplayField.ClientID %>';
        var udfTooltipID = '<%=txtattributetooltip.ClientID %>';
        var udfMatchFieldsID = '<%=lstmatchfields.ClientID %>';
        var udfAllowSearchID = '<%=chkallowsearch.ClientID %>';
        var udfDefaultValueID = '<%=cmbdefaultvalue.ClientID %>';
        var udfTextFormatID = '<%=cmbtextformat.ClientID %>';
        var udfTextLargeFormatID = '<%=cmbtextformatlarge.ClientID %>';
        var udfDateFormatID = '<%=cmbdateformat.ClientID %>';
        var udfHypTextID = '<%=txtHyperlinkText.ClientID %>';
        var udfHypPathID = '<%=txtHyperlinkPath.ClientID %>';
        var udfmtomaxrowsID = '<%=txtmaxrows.ClientID %>';
        var cmpMaxRowsID = '<%=cmpmaxrows.ClientID %>';
        var chkallowclaimantpopulationID = '<%=chkallowclaimantpopulation.ClientID %>';
    </script>
    <asp:ScriptManagerProxy ID="smp" runat="server">
    <Services>
    <asp:ServiceReference Path="~/shared/webServices/svcUserdefined.asmx" />
    </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">
            General Details</div>
        <div class="twocolumn"><asp:Label CssClass="mandatory" ID="lbldisplayname" runat="server" Text="Display Name*" AssociatedControlID="txtattributename"></asp:Label><span class="inputs"><asp:TextBox CssClass="fillspan" ID="txtattributename" runat="server" MaxLength="100"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtattributename" ID="reqdisplayname" runat="server" ErrorMessage="Please enter a name for this attribute in the box provided" ValidationGroup="vgAttribute" Text="*"></asp:RequiredFieldValidator></span><span style="display: none"><asp:Label ID="lblorder" runat="server" Text="Display Order" AssociatedControlID="txtorder"></asp:Label><span class="inputs"><asp:TextBox CssClass="fillspan" ID="txtorder" runat="server" meta:resourcekey="txtorderResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="comporder" runat="server" ErrorMessage="Please enter a valid value for order in the box provided" ControlToValidate="txtorder" Type="Integer" Operator="GreaterThan" ValueToCompare="0" Text="*" ValidationGroup="vgAttribute"></asp:CompareValidator></span></span></div>
        <div class="twocolumn"><asp:Label CssClass="mandatory" ID="lblappliesto" runat="server" Text="Applies To*" AssociatedControlID="cmbappliesto"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbappliesto"  runat="server" meta:resourcekey="cmbappliestoResource1"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="cvAppliesTo" runat="server" ControlToValidate="cmbappliesto" ValidationGroup="vgAttribute" Type="String" ValueToCompare="00000000-0000-0000-0000-000000000000" Operator="NotEqual" Display="Dynamic" ErrorMessage="You must choose an area the user defined field applies to" Text="*"></asp:CompareValidator></span><span id="pnlItemSpecific" runat="server" style="display: inline-block;"><asp:Label ID="lblspecific" runat="server" Text="Item Specific" AssociatedControlID="chkspecific"></asp:Label><span class="inputs"><asp:CheckBox ID="chkspecific" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></span></div>
        <div class="onecolumn"><asp:Label ID="lbldescription" runat="server" Text="" AssociatedControlID="txtattributedescription"><p class="labeldescription">Description</P></asp:Label><span class="inputs"><asp:TextBox ID="txtattributedescription" runat="server" TextMode="MultiLine"></asp:TextBox></span></div>
        <div class="onecolumn"><asp:Label ID="lbltooltip" runat="server" Text="" AssociatedControlID="txtattributetooltip"><p class="labeldescription">Tooltip</P></asp:Label><span class="inputs"><asp:TextBox ID="txtattributetooltip" runat="server" TextMode="MultiLine"></asp:TextBox></span></div>
        <div class="twocolumn"><asp:Label ID="lblgroup" runat="server" Text="Group" AssociatedControlID="ddlstgroup"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="ddlstgroup" runat="server"></asp:DropDownList><asp:HiddenField ID="txtgroupid" runat="server" />
        </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:PlaceHolder runat="server" ID="phAllowSearch"><asp:Label runat="server" Text="Allow Search?" ID="lblallowsearch" AssociatedControlID="chkallowsearch"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkallowsearch" CssClass="fillspan" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></asp:PlaceHolder><span id="spanAllowClaimantPopulation" style="display: none;"><asp:Label runat="server" Text="Allow employee to populate?" ID="lblallowclaimantpopulation" AssociatedControlID="chkallowclaimantpopulation"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkallowclaimantpopulation" CssClass="fillspan" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></span></div>
        <div class="twocolumn"><asp:Label ID="lbltype" runat="server" Text="Type" AssociatedControlID="cmbattributetype"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan" ID="cmbattributetype" runat="server">
                    <asp:ListItem Value="1" Text="Text"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Integer"></asp:ListItem>
                    <asp:ListItem Value="7" Text="Decimal"></asp:ListItem>
                    <asp:ListItem Value="6" Text="Currency"></asp:ListItem>
                    <asp:ListItem Value="5" Text="Yes/No"></asp:ListItem>
                    <asp:ListItem Value="4" Text="List"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Date"></asp:ListItem>
                    <asp:ListItem Value="10" Text="Large Text"></asp:ListItem>
                    <asp:ListItem Value="8" Text="Hyperlink"></asp:ListItem>
                    <asp:ListItem Value="16" Text="Dynamic Hyperlink" Enabled="False"></asp:ListItem>
                    <asp:ListItem Value="9" Text="Relationship"></asp:ListItem>
                </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblmandatory" runat="server" Text="Mandatory" AssociatedControlID="chkattributemandatory"></asp:Label><span class="inputs"><asp:CheckBox ID="chkattributemandatory" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div id="divTextOptions" class="twocolumn">
            <label id="lblmaxlength" for="txtmaxlength">Maximum Length</label><span class="inputs"><asp:TextBox ID="txtmaxlength" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compmaxlength" ControlToValidate="txtmaxlength" Text="*" ErrorMessage="Please enter a valid value for maximum length. Valid characters are the numbers 0-9." Operator="GreaterThanEqual" Type="Integer" ValueToCompare="0" runat="server" ValidationGroup="vgAttribute"></asp:CompareValidator></span><label id="lbltextformat" for="cmbtextformat">Format</label><span class="inputs"><asp:DropDownList ID="cmbtextformat" runat="server"><asp:ListItem Text="Single Line" Value="1"></asp:ListItem><asp:ListItem Text="Multiple Line" Value="2"></asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
        </div>
        <div id="divLargeTextOptions" style="display: none;" class="twocolumn"><label id="lblmaxlengthlarge" for="txtmaxlengthlarge">Maximum Length</label><span class="inputs"><asp:TextBox ID="txtmaxlengthlarge" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><label id="lbltextformatlarge" for="cmbtextformatlarge">Format</label><span class="inputs"><asp:DropDownList ID="cmbtextformatlarge" runat="server">
                            <asp:ListItem Text="Multiple Line" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Formatted Text Box" Value="6"></asp:ListItem>
                        </asp:DropDownList></span>
            <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"></span>
        </div>
        <div id="divDecimalOptions" style="display: none;" class="twocolumn">
            <label id="lblprecision" for="txtprecision">Precision</label><span class="inputs"><asp:TextBox ID="txtprecision" runat="server" MaxLength="2"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="compprecision" ControlToValidate="txtprecision" Text="*" ErrorMessage="Please enter a valid value for precision. Valid characters are the numbers 0-9." Operator="GreaterThanEqual" Type="Integer" ValueToCompare="0" runat="server" ValidationGroup="vgAttribute"></asp:CompareValidator></span>
        </div>
        <div id="divDateOptions" style="display: none;" class="twocolumn">
            <label id="lbldateformat" for="cmbdateformat">Format</label><span class="inputs"><asp:DropDownList ID="cmbdateformat" runat="server">
                            <asp:ListItem Text="Date Only" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Time Only" Value="5"></asp:ListItem> 
                            <asp:ListItem Text="Date and Time" Value="3"></asp:ListItem>
                        </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div id="divTickboxOptions" style="display: none;" class="twocolumn">
            <label id="lbldefaultvalue" for="cmbdefaultvalue">Default Value</label><span class="inputs"><asp:DropDownList ID="cmbdefaultvalue" runat="server">
                            <asp:ListItem Text="" Value="[None]"></asp:ListItem>
                            <asp:ListItem Text="No" Value="No"></asp:ListItem>
                            <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                        </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator
                ID="compdefaultvalue" runat="server" Enabled="false" ErrorMessage="Please select whether Yes or No should be selected by default" Text="*" ControlToValidate="cmbdefaultvalue" ValueToCompare="[None]" Type="String" Operator="NotEqual" ValidationGroup="vgAttribute"></asp:CompareValidator></span>
        </div>
        <div id="divListOptions" style="display: none">
            <div class="onecolumn">
                <label id="lblitems" for="lstitems">List Items</label><span class="inputs" style="display:inline-block !important;"><asp:ListBox ID="lstitems" runat="server"></asp:ListBox></span><span class="inputicon"><a href="javascript:showListItemModal();"><img src="../images/icons/16/plain/add2.png" alt="Add List Item" /></a><a href="javascript:editListItem();"><img src="../images/icons/edit.gif" alt="Edit List Item" /></a><a href="javascript:removeListItem();"><img src="../images/icons/delete2.gif" alt="Delete List Item" /></a></span><span class="inputtooltipfield"><asp:Image ID="imgMoveUp" runat="server" AlternateText="Move List Item Up" ImageUrl="~/shared/images/icons/16/Plain/arrow_up_blue.png" onclick="moveUpListItem();" /><asp:Image ID="imgMoveDown" runat="server" AlternateText="Move List Item Down" ImageUrl="~/shared/images/icons/16/Plain/arrow_down_blue.png" onclick="moveDownListItem();" /></span>
            </div>
            <input type="hidden" id="txtlistitems" name="txtlistitems" />
        </div>
        <div id="divHyperlinkOptions" style="display: none;" class="twocolumn">
            <label id="lblHyperlinkText" for="txtHyperlinkText">Hyperlink Text</label><span class="inputs"><asp:TextBox ID="txtHyperlinkText" runat="server" MaxLength="500"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqhyperlinktext" runat="server" ErrorMessage="Please enter a value for the Hyperlink Text" ControlToValidate="txtHyperlinkText" meta:resourcekey="reqhyperlinktextResource1" Enabled="false" ValidationGroup="vgAttribute">*</asp:RequiredFieldValidator></span><label id="lblHyperlinkPath" for="txtHyperlinkPath">Hyperlink Path/URL</label><span class="inputs"><asp:TextBox ID="txtHyperlinkPath" runat="server" MaxLength="500"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqhyperlinkpath" runat="server" ErrorMessage="Please enter a value for the Hyperlink Path/URL" ControlToValidate="txtHyperlinkPath" meta:resourcekey="reqhyperlinkpathResource1" Enabled="false" ValidationGroup="vgAttribute">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="revHyperlinkPath" runat="server" ErrorMessage="Please ensure the link path/url starts with a valid prefix such as http://, https://, ftp://, ftps:// or mailto:" Text="*" Display="Dynamic" ControlToValidate="txtHyperlinkPath" ValidationGroup="vgAttribute"></asp:RegularExpressionValidator></span>
        </div>
        <div id="divRelationshipTextBoxOptions" style="display: none;">
        <div class="onecolumnsmall">
            <asp:Label ID="lblRelatedTable" runat="server" AssociatedControlID="ddlRelatedTable" CssClass="mandatory">Related Table*</asp:Label><span class="inputs"><asp:DropDownList ID="ddlRelatedTable" runat="server"  CssClass="fillspan" onchange="getRelationshipLookupOptions();">
                <asp:ListItem Text="[None]" Value="0"></asp:ListItem>
            </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="cvRelatedTable" runat="server" ControlToValidate="ddlRelatedTable"  ValidationGroup="vgAttribute" Type="String" ValueToCompare="0" Operator="NotEqual" Display="Dynamic" ErrorMessage="You must choose a related table to use in the relationship textbox" Text="*" Enabled="false"></asp:CompareValidator></span>
            </div>
            <div class="onecolumnsmall">
            <asp:Label runat="server" ID="lblDisplayField" AssociatedControlID="cmbDisplayField" CssClass="mandatory">Display Field*</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="cmbDisplayField" CssClass="fillspan"><asp:ListItem Text="[None]" Value="0"></asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpDisplayField" ControlToValidate="cmbDisplayField" Operator="NotEqual" ValueToCompare="0" Text="*" ErrorMessage="Please select a valid display field for the relationship" Display="Dynamic" Enabled="false" ValidationGroup="vgAttribute"></asp:CompareValidator></span>
            </div>
            <div class="onecolumn">
                <label id="lblRelationshipMatchFields" for="lstmatchfields" class="mandatory">Match Fields*</label><span class="inputs"><asp:ListBox ID="lstmatchfields" runat="server"></asp:ListBox></span><span class="inputicon"><a href="javascript:showMatchFieldModal();"><img src="../images/icons/16/plain/add2.png" alt="Add Match Field" /></a><a href="javascript:removeFieldItem();"><img src="../images/icons/delete2.gif" alt="Delete Match Field" /></a></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CustomValidator runat="server" ID="custMatchFields" Text="*" ErrorMessage="Please add a Match Field." Display="Dynamic" Enabled="false" ClientValidationFunction="checkMatchFields" ValidationGroup="vgAttribute"></asp:CustomValidator></span>
                </div>
                <div class="twocolumn">
                <asp:Label runat="server" ID="lblmaxrows" AssociatedControlID="txtmaxrows">Max No. Suggestions</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtmaxrows" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpmaxrows" ControlToValidate="txtmaxrows" Operator="DataTypeCheck" Type="Integer" Text="*" ErrorMessage="Max No. Suggestions field must be numeric" Display="Dynamic" Enabled="false" ValidationGroup="vgAttribute"></asp:CompareValidator></span>
                </div>
        </div>
        <div class="formbuttons">
            <a onclick="javascript:saveUserDefined();"><asp:Image ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save"
            meta:resourcekey="cmdokResource1"></asp:Image></a>
        <asp:ImageButton ID="cmdcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel"
            CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
    </div>
    
    <div class="valdiv">
        <asp:Label ID="lblmsg" runat="server" Font-Size="Small" ForeColor="Red" Visible="False"
            meta:resourcekey="lblmsgResource1">Label</asp:Label></div>
    
    
    <asp:Panel ID="pnllistitem" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none; width: 440px;">
        <%--<div class="inputpanel">--%>
            <div class="sectiontitle">
                Add/Edit List Item</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lbllistitem" runat="server" Text="List item*" AssociatedControlID="txtlistitem"></asp:Label><span class="inputs">
                    <asp:TextBox ID="txtlistitem" runat="server" MaxLength="150" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield" onkeydown="SEL.Forms.RunOnEnter(event, addListItem);"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtlistitem" ID="reqListItemText" runat="server" ErrorMessage="Please add a List item." ValidationGroup="vgAttributeListItem" Text="*" Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator></span></div>
         <div class="twocolumn"><asp:Label runat="server" ID="lblArchiveListItem" AssociatedControlID="chkArchiveListItem" Text="Archived"></asp:Label> <span class="inputs"><asp:CheckBox runat="server" ID="chkArchiveListItem" /></span></div>
        <div class="inputpanel">
            <a href="javascript:addListItem();">
                <asp:Image id="btnSaveListitem" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" /></a>&nbsp;&nbsp;<asp:ImageButton
                    ID="cmdlistitemcancel" ImageUrl="~/shared/images/buttons/cancel_up.gif" runat="server" OnClientClick="hideListItemModal()" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modlistitem" runat="server" TargetControlID="lnklistitem"
        PopupControlID="pnllistitem" BackgroundCssClass="modalBackground" CancelControlID="cmdlistitemcancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnklistitem" runat="server" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel runat="server" ID="pnlMatchFields" CssClass="modalpanel" Style="display: none;">
    <div class="formpanel">
    <div class="sectiontitle">Select Relationship Match Fields</div>
    <div class="onecolumnsmall">
    <asp:Label runat="server" ID="lblmatchfieldlist" AssociatedControlID="cmbmatchfieldlist" CssClass="mandatory">Match Field*</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="cmbmatchfieldlist" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpMatchField" ControlToValidate="cmbmatchfieldlist" ValidationGroup="vgFieldItemList" Text="*" ErrorMessage="Please select a field from the list" Operator="NotEqual" ValueToCompare="0" Display="Dynamic"></asp:CompareValidator></span>
    </div>
    <div class="formbuttons">
        <helpers:cssspanbutton id="btnSaveMatchField" runat="server" text="save" onclick="saveFieldItem();" />
        <helpers:cssspanbutton id="btnCancelMatchField" runat="server" text="cancel" onclick="closeFieldItemListModal();" />
    </div>
    </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modmatchitem" runat="server" TargetControlID="lnkmatchitem" PopupControlID="pnlMatchFields" BackgroundCssClass="modalBackground" CancelControlID="btnCancelMatchField"></cc1:ModalPopupExtender><asp:LinkButton runat="server" ID="lnkmatchitem" Style="display: none;">LinkButton</asp:LinkButton>
</asp:Content>
