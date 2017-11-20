<%@ Page Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="supplier_details.aspx.cs" Inherits="Spend_Management.supplier_detailsPage" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmenu" runat="server">
<link rel="stylesheet" type="text/css" media="screen" href="css/layout.css" /><link rel="stylesheet" type="text/css"  media="screen" href="css/styles.aspx" />
    <a href="javascript:changePage('SupplierDetails');" id="lnkSupplierDetails" class="selectedPage">General Details</a>
    <asp:Label ID="lblLinkContactDetails" runat="server"><a href="javascript:changePage('ContactDetails');" id="lnkContactDetails">Contacts</a></asp:Label>
    <asp:Label ID="lblLinkSupplierContracts" runat="server"><a href="javascript:changePage('SupplierContracts');" id="lnkSupplierContracts">Contracts</a></asp:Label>
    <asp:LinkButton runat="server" ID="lnkTaskSummary" CssClass="submenuitem" OnClick="lnkTaskSummary_Click">Task Summary</asp:LinkButton>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentoptions">
    <asp:LinkButton runat="server" ID="lnkAddTask" CssClass="submenuitem" Visible="False" OnClick="lnkAddTask_Click">Add Task</asp:LinkButton>
    <div id="pgOptSupplierDetails"><asp:LinkButton runat="server" CssClass="submenuitem" ID="lnkSupplierNotes" OnClick="lnkSupplierNotes_Click">Notes</asp:LinkButton></div>
    <div id="pgOptSupplierContracts" style="display: none;"><asp:Literal runat="server" ID="litAddContract"></asp:Literal></div>
    <div id="pgOptContactDetails" style="display: none;"><a class="submenuitem" id="lnkNewContact" href="javascript:void(0);" onclick="javascript:AddContact();" runat="server">New Contact</a></div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="contentleft" runat="server"></asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmain">
    <script language="javascript" type="text/javascript">
        var contactAddLnk = '<%=lnkNewContact.ClientID %>';
    </script>

    <script language="javascript" type="text/javascript">
        var contactPanel = '<%=mdlContact.ClientID %>';
        var statusMsg = '<%=lblMessage.ClientID %>';
        var hiddenSupplierId = '<%=hiddenSupplierId.ClientID %>';
        var hiddenContactId = '<%=hiddenContactId.ClientID %>';
        var contactGridId = '<%=cntlContactGrid %>';
        var tabContainer = '<%=supplierTabs.ClientID %>';
        var updatePanelID = '<%=UpdatePanel.ClientID %>';
        var hdnSupCatID = '<%=hdnSupCatID.ClientID %>';
        var attlst = '<%=cntlAttList %>';

    </script>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webservices/svcSuppliers.asmx" />
            <asp:ServiceReference Path="~/shared/webservices/svcCountries.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/suppliers.js" />
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="tabDiv">
        <div id="divPages">
            <div id="pgSupplierDetails" class="primaryPage">
                <cc1:TabContainer runat="server" ActiveTabIndex="0" ID="supplierTabs">
                    <cc1:TabPanel runat="server" ID="SDetailsTab">
                        <HeaderTemplate>General Details</HeaderTemplate>
                        <ContentTemplate>
                            <div class="formpanel">
                                <div class="sectiontitle">General Details</div>
                                <asp:PlaceHolder runat="server" ID="SDplaceholder"></asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="phSDUserFields"></asp:PlaceHolder>
                                <asp:HiddenField runat="server" ID="hiddenSupplierId" />
                            </div>
                       </ContentTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel runat="server" ID="SDAdditionalTab">
                        <HeaderTemplate>Additional Information</HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel runat="server" id="UpdatePanel" OnLoad="UpdatePanel_Load">
                            
                                <ContentTemplate>
                                <div class="formpanel">
                                    <asp:PlaceHolder runat="server" ID="phSAUserFields"></asp:PlaceHolder>
                                </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </cc1:TabPanel>
                </cc1:TabContainer>
            </div>
            <asp:HiddenField ID="hdnSupCatID" runat="server" Value="-1"/>
            <div id="pgContactDetails" class="subPage" style="display: none;">
                <div class="formpanel formpanel_padding">
                    <div class="sectiontitle">Contact Details</div>
                    <asp:Label runat="server" ID="lblMessage"></asp:Label>
                    <asp:PlaceHolder runat="server" ID="SCplaceholder"></asp:PlaceHolder>
                </div>
            </div>
            <div id="pgSupplierContracts" class="subPage" style="display: none;">
                <div class="formpanel formpanel_padding">
                    <div class="sectiontitle"><asp:Literal ID="litSupplierText3" runat="server"></asp:Literal> Contracts</div>
                    <div class="comment"><asp:Literal runat="server" ID="litSupplierContractMsg"></asp:Literal></div>
                    <asp:PlaceHolder runat="server" ID="phSupplierContracts"></asp:PlaceHolder>
                </div>
            </div>
            <div class="formpanel formpanel_padding">
                <asp:Panel ID="pnlButtons" runat="server" CssClass="formbuttons"></asp:Panel>
            </div>
        </div>
    </div>


    <asp:Panel runat="server" CssClass="modalpanel" ID="pnlContactDetail" Style="display: none;">
        <asp:HiddenField runat="server" ID="hiddenContactId" />
        <div id="pnlContactForm" class="formpanel" style="height: 500px; overflow: auto;">
            <div class="sectiontitle">Contact Details</div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblContactName" AssociatedControlID="txtcontactname" CssClass="mandatory">Contact Name*</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtcontactname" CssClass="fillspan" ValidationGroup="scontacts" TabIndex="1" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqcontactname" ControlToValidate="txtcontactname" ErrorMessage="Contact name field is mandatory" Text="*" ValidationGroup="scontacts" Display="Dynamic"></asp:RequiredFieldValidator></span><label id="lblposition" for="txtposition">Position</label><span class="inputs"><asp:TextBox runat="server" ID="txtposition" TabIndex="2" CssClass="fillspan" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgTooltipPosition" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" onmouseover="SEL.Tooltip.Show('cbac2789-e19b-44c9-ba05-07f0a1d9665c', 'sm', this);" CssClass="tooltipicon" AlternateText="" /></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblemail" AssociatedControlID="txtemail">Email</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtemail" ValidationGroup="scontacts" CssClass="fillspan" TabIndex="3" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"><asp:Image ID="imgContactEmail" runat="server" ImageUrl="~/shared/images/icons/16/plain/mail_earth.png" /></span><span class="inputtooltipfield"><asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" onmouseover="SEL.Tooltip.Show('106aa741-202c-41d4-acb8-7e5772b8b291', 'sm', this);" CssClass="tooltipicon" AlternateText="" /></span><span class="inputvalidatorfield"><asp:RegularExpressionValidator runat="server" ID="regEmail" ControlToValidate="txtemail" ErrorMessage="Invalid email address specified" Text="*" ValidationGroup="scontacts" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"></asp:RegularExpressionValidator></span><asp:Label runat="server" ID="lblmobile" AssociatedControlID="txtmobile">Mobile</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtmobile" CssClass="fillspan" TabIndex="4" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblMainContact" AssociatedControlID="chkmaincontact">Is A Main Contact</asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkmaincontact" TabIndex="5" onkeypress="return RunOnEnter(event, 'saveContact()');" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image2" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" onmouseover="SEL.Tooltip.Show('382ac25e-4215-40f1-b0bf-17064257607d', 'sm', this);" CssClass="tooltipicon" AlternateText="" /></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="onecolumn">
                <asp:Label runat="server" ID="lblcomment" AssociatedControlID="txtcomments">Comment</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtcomments" CssClass="fillspan" TextMode="MultiLine" TabIndex="6" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="sectiontitle">Business Address</div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lbladdresstitle" AssociatedControlID="txtbaddresstitle">Address Title</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbaddresstitle" CssClass="fillspan" TabIndex="7" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblbpcode" AssociatedControlID="txtbpcode">Post Code</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbpcode" CssClass="fillspan" TabIndex="12" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblbaddress1" AssociatedControlID="txtbaddress1">Address Line 1</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbaddress1" CssClass="fillspan" TabIndex="8" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblbcountry" AssociatedControlID="lstbcountry">Country</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstbcountry" TabIndex="13" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblbaddress2" AssociatedControlID="txtbaddress2">Address Line 2</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbaddress2" CssClass="fillspan" TabIndex="9" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblbswitchboard" AssociatedControlID="txtbswitchboard">Telephone</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbswitchboard" CssClass="fillspan" TabIndex="14" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblbtown" AssociatedControlID="txtbtown">City</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbtown" CssClass="fillspan" TabIndex="10" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblbfax" AssociatedControlID="txtbfax">Fax</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbfax" CssClass="fillspan" TabIndex="15" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblbcounty" AssociatedControlID="txtbcounty">County</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtbcounty" CssClass="fillspan" TabIndex="11" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="sectiontitle">Personal Address</div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblpaddresstitle" AssociatedControlID="txtpaddresstitle">Address Title</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtpaddresstitle" CssClass="fillspan" TabIndex="16" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblppcode" AssociatedControlID="txtppcode">Post Code</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtppcode" CssClass="fillspan" TabIndex="21" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblpaddress1" AssociatedControlID="txtpaddress1">Address Line 1</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtpaddress1" CssClass="fillspan" TabIndex="17" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblpcountry" AssociatedControlID="lstpcountry">Country</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstpcountry" TabIndex="22" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblpaddress2" AssociatedControlID="txtpaddress2">Address Line 2</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtpaddress2" CssClass="fillspan" TabIndex="18" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblpswitchboard" AssociatedControlID="txtpswitchboard">Telephone</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtpswitchboard" CssClass="fillspan" TabIndex="23" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblptown" AssociatedControlID="txtptown">City</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtptown" CssClass="fillspan" TabIndex="19" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblpfax" AssociatedControlID="txtpfax">Fax</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtpfax" CssClass="fillspan" TabIndex="24" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblpcounty" AssociatedControlID="txtpcounty">County</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtpcounty" CssClass="fillspan" TabIndex="20" onkeypress="return RunOnEnter(event, 'saveContact()');"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <asp:PlaceHolder runat="server" ID="phSCUserFields"></asp:PlaceHolder>
        </div>
        <div class="formbuttons" style="padding: 5px;">
            <asp:Image runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" onclick="saveContact();" ID="btnOKContact" CssClass="btn" />&nbsp;&nbsp;<asp:Image runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" onclick="javascript:cancelContact();" ID="btnCancelContact" CssClass="btn" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" OnOkScript="saveContact" OnCancelScript="cancelContact" TargetControlID="lnkLaunchModal" PopupControlID="pnlContactDetail" BackgroundCssClass="modalBackground" ID="mdlContact">
    </cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkLaunchModal" Style="display: none;">&nbsp;</asp:LinkButton>
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
</asp:Content>
