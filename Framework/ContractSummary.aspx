<%@ Page Language="VB" MasterPageFile="~/FWMaster.master" AutoEventWireup="false"
    MaintainScrollPositionOnPostback="true" CodeFile="ContractSummary.aspx.vb" Inherits="Framework2006.ContractSummary" %>

<%@ Register Assembly="AjaxControlToolkit, Version=4.1.40412.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/ContractSummaryScript.js" />
            <asp:ScriptReference Path="~/shared/javascript/userdefined.js" />
            <asp:ScriptReference Path="~/shared/javascript/relationshipTextbox.js" />
            <asp:ScriptReference Path="~/callback.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcRelationshipTextbox.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel runat="server" ID="POpt_UpdatePanel">
        <ContentTemplate>
            <asp:LinkButton runat="server" ID="lnkNew" CssClass="submenuitem" Visible="false"
                OnClick="lnkNew_Click" CausesValidation="false">Add</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkNewDefine" CssClass="submenuitem" OnClick="lnkNewDefine_Click"
                CausesValidation="False">Add + Define</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkNewVariation" CssClass="submenuitem" Visible="false"
                OnClick="lnkNewVariation_Click" CausesValidation="False">Add Variation</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkDelete" CssClass="submenuitem" CausesValidation="False">Delete</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkNotes" CssClass="submenuitem" OnClick="lnkNotes_Click"
                CausesValidation="False">Notes</asp:LinkButton>
            <%--<asp:LinkButton runat="server" ID="lnkVRegistry" CssClass="submenuitem" OnClick="lnkVRegistry_Click"
                CausesValidation="False">Version Registry</asp:LinkButton>--%>
            <asp:LinkButton runat="server" ID="lnkBulkUpdate" CssClass="submenuitem" OnClick="lnkBulkUpdate_Click">Bulk Update</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkGenerate" CssClass="submenuitem" OnClick="lnkGenerate_Click"
                CausesValidation="False">Generate</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkSaving" CssClass="submenuitem" OnClick="lnkSaving_Click"
                CausesValidation="False">Contract Saving</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkAddCDTask" CssClass="submenuitem">Add Task</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkAddCPTask" CssClass="submenuitem" Visible="false">Add Task</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkAddIDTask" CssClass="submenuitem" Visible="false">Add Task</asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkAddIFTask" CssClass="submenuitem" Visible="false">Add Task</asp:LinkButton>
            <asp:Literal runat="server" ID="litHelp"></asp:Literal>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" ID="ajaxLoadingPanelPageOptions" DisplayAfter="50"
        AssociatedUpdatePanelID="POpt_UpdatePanel">
        <ProgressTemplate>
            <div class="progresspoptpanel">
                <asp:Image runat="server" ID="imgPOptPanel" ImageUrl="~/shared/images/ajax-loader.gif"
                    AlternateText="Loading..." />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:Literal ID="litUDH" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
    <div class="panel">
        <div class="paneltitle">
            Navigation</div>
        <asp:UpdatePanel ID="navUpdatePanel" runat="server">
            <ContentTemplate>
                <asp:LinkButton runat="server" ID="lnkCDnav" CssClass="submenuitem" OnClick="lnkCDnav_Click"
                    CausesValidation="False">Contract Details</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkCAnav" CssClass="submenuitem" OnClick="lnkCAnav_Click"
                    CausesValidation="False">Additional Details</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkCPnav" CssClass="submenuitem" OnClick="lnkCPnav_Click"
                    CausesValidation="False">Contract Products</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkIDnav" CssClass="submenuitem" OnClick="lnkIDnav_Click"
                    CausesValidation="False">Invoice Details</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkIFnav" CssClass="submenuitem" OnClick="lnkIFnav_Click"
                    CausesValidation="False">Invoice Forecasts</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkNSnav" CssClass="submenuitem" OnClick="lnkNSnav_Click"
                    CausesValidation="False">Note Summary</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkLCnav" CssClass="submenuitem" Visible="false"
                    OnClick="lnkLCnav_Click" CausesValidation="False">Linked Contracts</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkCHnav" CssClass="submenuitem" OnClick="lnkCHnav_Click"
                    CausesValidation="False">Contract History</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkRTnav" CssClass="submenuitem" Visible="false"
                    OnClick="lnkRTnav_Click" CausesValidation="False">Recharge Template</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkRPnav" CssClass="submenuitem" Visible="false"
                    OnClick="lnkRPnav_Click" CausesValidation="False">Recharge Payments</asp:LinkButton>
                <asp:LinkButton runat="server" ID="lnkTSnav" CssClass="submenuitem" CausesValidation="false"
                    Visible="false">Task Summary</asp:LinkButton>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="ajaxLoadingPanelNavigation" DisplayAfter="50"
            AssociatedUpdatePanelID="navUpdatePanel">
            <ProgressTemplate>
                <div class="progressnavpanel">
                    <asp:Image runat="server" ID="imgNavPrg" ImageUrl="~/shared/images/ajax-loader.gif"
                        AlternateText="Loading..." />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <script language="javascript" type="text/javascript">
        var searchwnd;
        var UF_FieldName;
        var UF_Value;

        function getTextEntry() {
            var UF_Field;
            var src_Field;
            //Form1.
            src_Field = searchwnd.document.getElementById(UF_FieldName); //eval('searchwnd.' + UF_FieldName);
            // 'Form1.' + 
            UF_Field = document.getElementById(UF_FieldName); //eval(UF_FieldName);
            UF_Field.value = src_Field.value;
            searchwnd.close();
        }

        function getSearchResult() {
            var UF_Field_txt = document.getElementById(UF_FieldName + '_TXT');
            if (UF_Field_txt != null) {
                UF_Field_txt.value = searchwnd.document.getElementById('searchResultTxt').value;
            }

            var UF_Field_val = document.getElementById(UF_FieldName);
            if (UF_Field_val != null) {
                UF_Field_val.value = searchwnd.document.getElementById('searchResultId').value;
            }
            searchwnd.close();
        }

        function doSearch(i, UF_name) {
            window.name = 'main';
            UF_FieldName = UF_name;
            searchwnd = window.open('UFSearch.aspx?searchtype=' + i + '&ufid=' + UF_name, 'search', 'width=600, height=600, scrollbars=yes');
        }

        function ValidateSEDates(oSrc, args) {
            var startDate, endDate;

            startDate = document.getElementById('txtContractDate').outerText;
            //alert('txtContractDate = ' + startDate);

            endDate = document.getElementById('txtRenewalDate');
            //alert('txtRenewalDate = ' + endDate);

            args.IsValid = (Date.parse(startDate) < Date.parse(endDate));
        }

        function doPrint() {
            alert('Please ensure printer orientation is set to landscape for best results');
            window.print();
        }

        function setFocus() {
            var cntl = document.getElementById('txtContractNumber');
            cntl.focus();
        }


        function OpenBreakdown(fId) {
            curFID = fId;
            window.name = "main";
            buWin = window.open('InvoiceBreakdown.aspx?type=forecastbulk&id=' + curFID, 'ForecastBreakdown', 'width=800, height=600,scrollbars=yes');
            return;
        }

        function UpdateBreakdown() {
            doBURefresh();
            buWin.close();
        }

        function doBURefresh() {
            var idx;

            url = 'InvoiceForecasts.aspx?ifaction=callback&cb_fid=' + curFID;

            var res;
            var data;

            res = doCallBack(url, data);

            var cntl;
            cntl = document.getElementById('divBreakdownTable');
            if (cntl != null) {
                cntl.innerHTML = res;
            }
            return;
        }
    </script>
    <asp:UpdateProgress runat="server" ID="ajaxLoadingPanelMain" DisplayAfter="50" AssociatedUpdatePanelID="ContractSummary_AjaxPanel">
        <ProgressTemplate>
            <div class="progresspanel">
                <div>
                    Loading ...</div>
                <br />
                <asp:Image ID="imgLoading" runat="server" ImageUrl="~/shared/images/ajax-loader.gif"
                    AlternateText="Loading..." />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel runat="server" ID="ContractSummary_AjaxPanel">
        <ContentTemplate>
            <div class="inputpanel">
                <div>
                    <asp:Label runat="server" ID="lblErrorString" ForeColor="Red"></asp:Label></div>
            </div>
            <asp:MultiView ID="ViewTab" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwContractDetails" runat="server">
                    <asp:Panel runat="server" ID="panelVariations" Visible="false">
                        <igmisc:WebPanel ID="igVariationsPanel" runat="server" CssClass="inputpanel" Expanded="False">
                            <Template>
                                <asp:Literal runat="server" ID="litCDVariations"></asp:Literal>
                            </Template>
                            <Header Text="0 Variations for this contract">
                                <ExpandedAppearance>
                                    <style cssclass="inputpaneltitle">
                                        
                                    </style>
                                </ExpandedAppearance>
                                <CollapsedAppearance>
                                    <style cssclass="inputpaneltitle">
                                        
                                    </style>
                                </CollapsedAppearance>
                            </Header>
                        </igmisc:WebPanel>
                    </asp:Panel>
                    <div class="formpanel"><div><asp:Literal runat="server" ID="litLockStatus"></asp:Literal></div><div class="sectiontitle">Contract Details</div><div class="twocolumn"><asp:Label ID="lblUniqueKey" runat="server" AssociatedControlID="txtUniqueKeyValue"></asp:Label><span class="inputs"><asp:TextBox ID="txtUniqueKeyValue" ReadOnly="true" runat="server" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" ID="lblContractLink" AssociatedControlID="lstLinkManage">contract links</asp:Label><span class="inputs"><asp:DropDownList ID="lstLinkManage" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"><asp:ImageButton runat="server" ID="imgLinkManage" ImageUrl="shared/images/icons/16/plain/link.png" CausesValidation="False" /></span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="?" id="imgtooltip445" onclick="showTooltip(event, 'fw', 'imgtooltip445','2cde7282-ae30-4639-acb3-4ec2b4882bff');" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="twocolumn"><asp:Label runat="server" ID="lblAudience" AssociatedControlID="txtAudience">Audience</asp:Label><span class="inputs"><asp:TextBox ReadOnly="true" ID="txtAudience" runat="server" TextMode="MultiLine" CssClass="fillspan" ToolTip="* indicates Team audience" Rows="1"></asp:TextBox></span><span class="inputicon"><asp:ImageButton runat="server" ID="imgAudience" ImageUrl="shared/images/icons/view.png" ToolTip="Define the audience of users permitted to access this contract" CausesValidation="False" /></span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip443" onclick="showTooltip(event, 'fw', 'imgtooltip443','952a480f-bb8e-4e8d-93ef-f12932918060');" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" ID="lblSchedules" AssociatedControlID="lstSchedules">Linked Schedules</asp:Label><span class="inputs"><asp:DropDownList ID="lstSchedules" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstSchedules_SelectedIndexChanged" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"><asp:ImageButton runat="server" ID="imgSchedules" ImageUrl="shared/images/icons/16/plain/documents_gear.png" CausesValidation="false" AlternateText="Go To" /></span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip444" onclick="showTooltip(event, 'fw', 'imgtooltip444','22bb9c7d-e189-451a-b827-9805df81be8a');" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="sectiontitle">
                            Contract Definition</div>
                        <div class="twocolumn"><asp:Label ID="lblContractNumber" runat="server" AssociatedControlID="txtContractNumber">contract number</asp:Label><span class="inputs"><asp:TextBox ID="txtContractNumber" TabIndex="1" runat="server" MaxLength="25" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblSupersedesContract" runat="server" AssociatedControlID="txtSupersedesContract">supersedes contract</asp:Label><span class="inputs"><asp:TextBox ID="txtSupersedesContract" TabIndex="2" runat="server" MaxLength="25"
CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH1" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="twocolumn"><asp:Label ID="lblSchedule" runat="server" AssociatedControlID="txtSchedule">schedule</asp:Label><span class="inputs"><asp:TextBox ID="txtSchedule" TabIndex="3" runat="server" MaxLength="100" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span
class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblContractType" runat="server" AssociatedControlID="lstContractType">contract type</asp:Label><span class="inputs"><asp:DropDownList ID="lstContractType" TabIndex="4" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH2" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="onecolumn"><asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" CssClass="mandatory">description</asp:Label><span class="inputs"><asp:TextBox ID="txtDescription" TabIndex="5" runat="server" MaxLength="250" TextMode="MultiLine" ValidationGroup="cddetails" CssClass="fillspan" Rows="1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Contract Description field is mandatory" ValidationGroup="cddetails" Display="Dynamic" Text="*"></asp:RequiredFieldValidator></span></div><div class="twocolumn"><asp:Label ID="lblCategory" runat="server" AssociatedControlID="lstContractCategory">category</asp:Label><span class="inputs"><asp:DropDownList ID="lstContractCategory" TabIndex="6" runat="server" ValidationGroup="cddetails" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH3" runat="server"></asp:Literal></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpContractCategory" ErrorMessage="Field is mandatory" Text="*" Operator="NotEqual" ValueToCompare="0" ControlToValidate="lstContractCategory" ValidationGroup="cddetails" Display="Dynamic"></asp:CompareValidator></span><asp:Label ID="lblSupplierCode" runat="server" AssociatedControlID="txtSupplierCode">supplier code</asp:Label><span class="inputs"><asp:TextBox ID="txtSupplierCode" TabIndex="8" runat="server" MaxLength="50" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="onecolumnsmall"><asp:Label runat="server" ID="lblVendor" AssociatedControlID="lstVendor" CssClass="mandatory">supplier</asp:Label><span class="inputs"><asp:DropDownList ID="lstVendor" TabIndex="7" runat="server" ValidationGroup="cddetails" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"><asp:Literal runat="server" ID="litVendorLink"></asp:Literal></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpVendor" runat="server" ErrorMessage="A Supplier must be specified" ControlToValidate="lstVendor" Operator="NotEqual" ValueToCompare="0" ValidationGroup="cddetails" CultureInvariantValues="False" Display="Dynamic" Text="*"></asp:CompareValidator></span></div><div class="twocolumn"><asp:Label ID="lblContractStatus" runat="server" AssociatedControlID="lstContractStatus">contract status</asp:Label><span class="inputs"><asp:DropDownList ID="lstContractStatus" TabIndex="9" runat="server" ValidationGroup="cddetails" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip446" onclick="showTooltip(event, 'fw', 'imgtooltip446','6df35f3c-8188-4970-95be-5864709eac33');" class="tooltipicon" /><asp:Literal ID="litUH4" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblContractOwner" runat="server" AssociatedControlID="lstContractOwner">owner</asp:Label><span class="inputs"><asp:DropDownList ID="lstContractOwner" TabIndex="10" runat="server" ValidationGroup="cddetails" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH5" runat="server"></asp:Literal></span></div>
                        <div class="twocolumn"><asp:Label runat="server" ID="lblNotes" AssociatedControlID="txtNotes">Contract Notes</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtNotes" CssClass="fillspan" ReadOnly="true">0 Notes</asp:TextBox></span><span class="inputicon"><asp:ImageButton runat="server" ID="cmdNotes" ImageUrl="./images/attachment.gif" OnClick="cmdNotes_Click" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblAttachments" runat="server" AssociatedControlID="attachmentDummy">attachments</asp:Label><span class="inputs"><asp:Literal ID="litAttachments" runat="server"></asp:Literal><asp:HiddenField runat="server" ID="attachmentDummy" /></span><span class="inputicon"><asp:Literal runat="server" ID="litAttachmentIcon"></asp:Literal></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="sectiontitle">
                            Financials</div>
                        <div class="twocolumn"><asp:Label ID="lblContractValue" runat="server" AssociatedControlID="txtContractValue">value</asp:Label><span class="inputs">
                        
                        <asp:TextBox ID="txtContractValue" TabIndex="12" runat="server" ValidationGroup="cddetails" CssClass="fillspan">0</asp:TextBox><cc1:FilteredTextBoxExtender ID="fteConValue" runat="server" TargetControlID="txtContractValue" FilterType="Custom, Numbers" ValidChars=".,-"></cc1:FilteredTextBoxExtender></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH6" runat="server"></asp:Literal></span><span class="inputvalidatorfield">
                        
                        <asp:CompareValidator ID="cmpContractValue" runat="server" ControlToValidate="txtContractValue" ValidationGroup="cddetails" ErrorMessage="Invalid monetary amount provided" Operator="DataTypeCheck" Type="Currency" Display="Dynamic" Text="*"></asp:CompareValidator></span><asp:Label ID="lblTotalContractValue" runat="server" Text="total contract value" AssociatedControlID="txtTotalContractValue"></asp:Label><span class="inputs"><asp:TextBox ID="txtTotalContractValue" runat="server" ReadOnly="True" ToolTip="Value including all variations where applicable" ValidationGroup="cddetails" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="twocolumn"><asp:Label ID="lblCDCurrency" runat="server" AssociatedControlID="lstCDCurrency">currency</asp:Label><span class="inputs"><asp:DropDownList ID="lstCDCurrency" TabIndex="14" runat="server" ValidationGroup="cddetails" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH7" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblAnnualContractValue" runat="server" AssociatedControlID="txtAnnualContractValue">annual contract value</asp:Label><span class="inputs">
                        
                        
                        
                        <asp:TextBox ID="txtAnnualContractValue" TabIndex="13" runat="server" ValidationGroup="cddetails" CssClass="fillspan">0</asp:TextBox><cc1:FilteredTextBoxExtender ID="fteACV" runat="server" TargetControlID="txtAnnualContractValue" FilterType="Custom, Numbers" ValidChars=".,-"></cc1:FilteredTextBoxExtender></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">

                        <asp:CompareValidator ID="cmpAnnualValue" runat="server" ControlToValidate="txtAnnualContractValue" ValidationGroup="cddetails" ErrorMessage="Invalid monetary amount provided" Operator="DataTypeCheck" Type="Currency" Text="*" Display="Dynamic"></asp:CompareValidator></span></div>
                        
                        
                        
                        <div class="twocolumn"><asp:Label ID="lblTermType" runat="server" AssociatedControlID="lstTermType">term type</asp:Label><span class="inputs"><asp:DropDownList ID="lstTermType" TabIndex="15" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litUH8" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span><asp:Placeholder runat="server" ID="panelMaintParams1"><asp:Label ID="lblMaintenanceType" runat="server" AssociatedControlID="lstMaintenanceType">maintenance type</asp:Label><span class="inputs"><asp:DropDownList ID="lstMaintenanceType" TabIndex="16" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></asp:Placeholder></div><asp:Panel runat="server" ID="panelMaintParams2">
                        <div class="twocolumn"><asp:Label ID="lblForecastType" runat="server" AssociatedControlID="lstForecastType">forecast type</asp:Label><span class="inputs"><asp:DropDownList ID="lstForecastType" TabIndex="17" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Literal ID="litInflatorHelp" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblInvoiceFreq" runat="server" AssociatedControlID="lstInvoiceFreq">invoice freq</asp:Label><span class="inputs"><asp:DropDownList ID="lstInvoiceFreq" TabIndex="18" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicons">&nbsp;</span><span class="inputttooltipfield"><asp:Literal ID="litUH9" runat="server"></asp:Literal></span><span class="inputvalidatorfield">&nbsp;</span></div><div class="twocolumn"><asp:Label ID="lblX" runat="server" AssociatedControlID="lstMaintParam1">Inflator X</asp:Label><span class="inputs"><asp:DropDownList ID="lstMaintParam1" TabIndex="19" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" ID="lblMaintParam1" AssociatedControlID="txtMaintParam1">Inflator X Percent</asp:Label><span class="inputs"><asp:TextBox ID="txtMaintParam1" TabIndex="20" runat="server" Width="50px" ValidationGroup="cddetails" CssClass="fillspan"></asp:TextBox></span><span class="inputicons">&nbsp;</span><span class="inputttooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpMaintParam1" runat="server" ErrorMessage="Annual Increase % x must be numeric" ValidationGroup="cddetails" ControlToValidate="txtMaintParam1" Operator="DataTypeCheck" Type="Double" Display="Dynamic" Text="*"></asp:CompareValidator></span></div><div class="twocolumn"><asp:Label ID="lblY" runat="server" AssociatedControlID="lstMaintParam2">Inflator Y</asp:Label><span class="inputs"><asp:DropDownList ID="lstMaintParam2" TabIndex="21" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span><asp:Label runat="server" ID="lblMaintParam2" AssociatedControlID="txtMaintParam2">Inflator Y Percent</asp:Label><span class="inputs"><asp:TextBox ID="txtMaintParam2" TabIndex="22" runat="server" Width="50px" ValidationGroup="cddetails"></asp:TextBox></span><span class="inputicons">&nbsp;</span><span class="inputttooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpMaintParam2" runat="server" ErrorMessage="Annual Increase % y must be numeric" ControlToValidate="txtMaintParam2" Operator="DataTypeCheck" Type="Double" Display="Dynamic" ValidationGroup="cddetails" Text="*"></asp:CompareValidator></span></div></asp:Panel>
                        <div class="sectiontitle">
                            Contract Dates & Notification</div>
                        <div class="twocolumn"><asp:Label ID="lblContractDate" runat="server" AssociatedControlID="txtContractDate">contract date</asp:Label><span class="inputs"><asp:TextBox ID="txtContractDate" runat="server" TabIndex="23" ValidationGroup="cddetails" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"><asp:ImageButton runat="server" ID="btnContractDate" ImageUrl="~/icons/16/plain/calendar.gif" CausesValidation="False" /><cc1:CalendarExtender runat="server" ID="calexContractDate" Format="dd/MM/yyyy" TargetControlID="txtContractDate" PopupButtonID="btnContractDate" PopupPosition="TopRight"></cc1:CalendarExtender></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpStartDate" runat="server" ControlToValidate="txtContractDate" ValidationGroup="cddetails" ErrorMessage="Invalid date entered for Start Date" Operator="DataTypeCheck" Type="Date" Text="*" Display="Dynamic"></asp:CompareValidator><asp:RequiredFieldValidator ID="reqStartDate" runat="server" ErrorMessage="Start Date is mandatory" ControlToValidate="txtContractDate" ForeColor="Red" Display="Dynamic" ValidationGroup="cddetails" Text="*"></asp:RequiredFieldValidator></span><asp:Label ID="lblRenewalDate" runat="server" AssociatedControlID="txtRenewalDate">renewal date</asp:Label><span class="inputs"><asp:TextBox ID="txtRenewalDate" runat="server" TabIndex="24" AutoPostBack="True" OnTextChanged="txtRenewalDate_TextChanged" ValidationGroup="cddetails" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"><asp:ImageButton runat="server" ID="btnRenewalDate" ImageUrl="~/icons/16/plain/calendar.gif" CausesValidation="False" /><cc1:CalendarExtender runat="server" ID="calexRenewalDate" Format="dd/MM/yyyy" TargetControlID="txtRenewalDate" PopupButtonID="btnRenewalDate" PopupPosition="TopRight"></cc1:CalendarExtender></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpEndDate" runat="server" ControlToValidate="txtRenewalDate" ErrorMessage="Invalid date entered for End / Renewal Date" Operator="DataTypeCheck" Type="Date" Display="Dynamic" Text="*" ValidationGroup="cddetails"></asp:CompareValidator><asp:CompareValidator ID="cmpSEDate" runat="server" ErrorMessage="Contract Start Date must preceed the End Date" ControlToValidate="txtRenewalDate" Operator="GreaterThan" Type="Date" ForeColor="Red" ControlToCompare="txtContractDate" Display="Dynamic" Text="*" ValidationGroup="cddetails"></asp:CompareValidator><asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="txtRenewalDate" ErrorMessage="End Date is mandatory" ForeColor="Red" Text="*" Display="Dynamic" ValidationGroup="cddetails"></asp:RequiredFieldValidator></span></div>
                        <div class="twocolumn"><asp:Label ID="lblCancellationPeriod" runat="server" AssociatedControlID="txtCancellationPeriod">cancellation period</asp:Label><span class="inputs"><asp:TextBox ID="txtCancellationPeriod" TabIndex="25" runat="server" AutoPostBack="True" OnTextChanged="txtCancellationPeriod_TextChanged" ValidationGroup="cddetails"
CssClass="fillspan">0</asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip448" onclick="showTooltip(event, 'fw', 'imgtooltip448','fa1bb5ca-be60-448c-9813-90789a33831e');" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqCancellationPd" runat="server" ControlToValidate="txtCancellationPeriod" ErrorMessage="A cancellation period must be specified" Display="Dynamic" Text="*" ValidationGroup="cddetails"></asp:RequiredFieldValidator><cc1:FilteredTextBoxExtender ID="fteCancellationPd" runat="server" TargetControlID="txtCancellationPeriod" FilterType="Numbers"></cc1:FilteredTextBoxExtender></span><asp:Label runat="server" ID="lblCancellationPeriodType" AssociatedControlID="lstCancellationPeriodType">Cancellation Period Type</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstCancellationPeriodType" AutoPostBack="True" TabIndex="26"><asp:ListItem Value="0">mths</asp:ListItem><asp:ListItem Value="1">days</asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="twocolumn"><asp:Label ID="lblCancellationDate" runat="server" AssociatedControlID="txtCancellationDate">cancellation date</asp:Label><span class="inputs"><asp:TextBox ID="txtCancellationDate" runat="server" TabIndex="27" ValidationGroup="cddetails" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"><asp:ImageButton runat="server" ID="btnCancellationDate" ImageUrl="~/icons/16/plain/calendar.gif" CausesValidation="False" /><cc1:CalendarExtender runat="server" ID="calexCancellationDate" PopupPosition="TopRight" Format="dd/MM/yyyy" PopupButtonID="btnCancellationDate" TargetControlID="txtCancellationDate"></cc1:CalendarExtender></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpCancellationDate" runat="server" ControlToValidate="txtCancellationDate" ErrorMessage="Invalid date entered for Cancellation Date" Operator="DataTypeCheck" Type="Date" Text="*" Display="Dynamic" ValidationGroup="cddetails"></asp:CompareValidator><asp:RequiredFieldValidator runat="server" ID="reqCancellationDate" ControlToValidate="txtCancellationDate" Text="*" ErrorMessage="Cancellation date is mandatory" Display="Dynamic" ValidationGroup="cddetails"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cmpcancellationdateafterstartdate" ControlToValidate="txtCancellationDate" ControlToCompare="txtContractDate" Operator="GreaterThanEqual" Type="Date" ErrorMessage="Cancellation Date must be on or after the Start Date" ValidationGroup="cddetails" Display="Dynamic" Text="*"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpcancellationdatebeforeenddate" ControlToCompare="txtRenewalDate" ControlToValidate="txtCancellationDate" Operator="LessThanEqual" Type="Date" Display="Dynamic" Text="*" ErrorMessage="The Cancellation Date must be on or before the Renewal Date" ValidationGroup="cddetails"></asp:CompareValidator></span></div>
                        <div class="twocolumn"><asp:Label ID="lblReviewPeriod" runat="server" AssociatedControlID="txtReviewPeriod">review period</asp:Label><span class="inputs"><asp:TextBox ID="txtReviewPeriod" TabIndex="28" runat="server" AutoPostBack="True" OnTextChanged="txtReviewPeriod_TextChanged" ValidationGroup="cddetails" CssClass="fillspan">0</asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip449" onclick="showTooltip(event, 'fw', 'imgtooltip449','f16f2faa-5ee1-4137-b6e2-7797cfa56f29');" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqReviewPd" ControlToValidate="txtReviewPeriod" ErrorMessage="A review period is required" Text="*" Display="Dynamic" ValidationGroup="cddetails"></asp:RequiredFieldValidator><cc1:FilteredTextBoxExtender ID="fteReviewPd" runat="server" TargetControlID="txtReviewPeriod" FilterType="Numbers"></cc1:FilteredTextBoxExtender></span>
                        <asp:Label runat="server" ID="lblReviewPeriodType" AssociatedControlID="lstReviewPeriodType">Review Period Type</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstReviewPeriodType" AutoPostBack="True" TabIndex="29"><asp:ListItem Value="0">mths</asp:ListItem><asp:ListItem Value="1">days</asp:ListItem></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="twocolumn"><asp:Label ID="lblReviewDate" runat="server" AssociatedControlID="txtReviewDate">review date</asp:Label><span class="inputs"><asp:TextBox ID="txtReviewDate" runat="server" TabIndex="30" ValidationGroup="cddetails"></asp:TextBox></span><span class="inputicon"><asp:ImageButton runat="server" ID="btnReviewDate" ImageUrl="~/icons/16/plain/calendar.gif" CausesValidation="False" /><cc1:CalendarExtender runat="server" ID="calexReviewDate" PopupButtonID="btnReviewDate" TargetControlID="txtReviewDate" Format="dd/MM/yyyy" PopupPosition="TopRight"></cc1:CalendarExtender></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator ID="cmpReviewDate" runat="server" ControlToValidate="txtReviewDate" ErrorMessage="Invalid date entered for Review Date" Operator="DataTypeCheck" Type="Date" Display="Dynamic" Text="*" ValidationGroup="cddetails"></asp:CompareValidator><asp:RequiredFieldValidator runat="server" ID="reqReviewDate" ErrorMessage="Review date is mandatory" Text="*" ControlToValidate="txtReviewDate" Display="Dynamic" ValidationGroup="cddetails"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cmpreviewdateafterstartdate" ControlToValidate="txtReviewDate" ControlToCompare="txtContractDate" Operator="GreaterThanEqual" Type="Date" Text="*" ErrorMessage="Review date must be on or after the Start Date" ValidationGroup="cddetails" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpreviewdatebeforeenddate" ControlToCompare="txtRenewalDate" ControlToValidate="txtReviewDate" Text="*" ErrorMessage="The Review Date must on or before the Renewal Date" Operator="LessThanEqual" Type="Date" ValidationGroup="cddetails" Display="Dynamic"></asp:CompareValidator></span></div>
                        <div class="twocolumn"><asp:Label ID="lblPenaltyClause" runat="server" AssociatedControlID="chkPenaltyClause">penalty clause</asp:Label><span class="inputs"><asp:CheckBox ID="chkPenaltyClause" TabIndex="31" runat="server" CssClass="fillspan"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip450" onclick="showTooltip(event, 'fw', 'imgtooltip450','00bebe38-1f00-4d6a-95af-3a1af582f6a2');" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblReviewCompleteDate" runat="server" AssociatedControlID="txtReviewCompleteDate">review complete date</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtReviewCompleteDate" TabIndex="32" ValidationGroup="cddetails" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">
<asp:ImageButton runat="server" ID="btnReviewComplete" ImageUrl="~/icons/16/plain/calendar.gif" CausesValidation="False" /><cc1:CalendarExtender runat="server" ID="calexReviewComplete"
Format="dd/MM/yyyy" TargetControlID="txtReviewCompleteDate" PopupButtonID="btnReviewComplete" PopupPosition="TopRight"></cc1:CalendarExtender>
</span><span class="inputtooltipfield"><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip447" onclick="showTooltip(event, 'fw', 'imgtooltip447','40980c7a-82d3-46a9-857e-19d44b2828a9');" class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <div class="twocolumn"><asp:Label runat="server" ID="lblNotify" AssociatedControlID="lstNotify">Notify</asp:Label><span class="inputs"><asp:TextBox ID="lstNotify" TabIndex="33" runat="server" TextMode="MultiLine" ReadOnly="True" Rows="1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"><asp:ImageButton ID="lnkNotify" runat="server" OnClick="lnkNotify_Click" AlternateText="Go To" ImageUrl="shared/images/icons/view.png" /></span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span></div>
                        <asp:PlaceHolder runat="server" ID="phCDUFields"></asp:PlaceHolder>
                        <%--<asp:Panel runat="server" ID="CDUFPanel">
                        </asp:Panel>--%>
                        <%--<asp:Literal ID="litUserFields" runat="server"></asp:Literal>--%>
                        <div>
                            <div style="text-align: center; width: auto;">
                                <b>Last Changed Date:</b><asp:Label runat="server" ID="lblLastChangedValue"></asp:Label></div>
                        </div>
                        <div class="formbuttons">
                            <asp:ImageButton runat="server" ImageUrl="./buttons/update.gif" ID="cmdCDUpdate" />&nbsp;
                            <asp:ImageButton runat="server" ImageUrl="./buttons/cancel.gif" ID="cmdCDCancel"
                                CausesValidation="false" />
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="vwContractAdditional" runat="server">
                    <div class="formpanel">
                        <asp:Literal runat="server" ID="litCALockMsg"></asp:Literal>
                        <div class="sectiontitle">
                            Contract Additional<asp:Label ID="lblCATitle" runat="server"></asp:Label></div>
                        <%-- <asp:Panel ID="CAPanel" runat="server">
                    </asp:Panel>--%>
                        <asp:PlaceHolder runat="server" ID="phCAUFields"></asp:PlaceHolder>
                        <div class="formbuttons">
                            <asp:ImageButton runat="server" ID="cmdCAUpdate" ImageUrl="./buttons/update.gif" />&nbsp;
                            <asp:ImageButton runat="server" ID="cmdCACancel" ImageUrl="./buttons/cancel.gif"
                                CausesValidation="false" />
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="vwContractProducts" runat="server">
                    <div class="formpanel">
                        <asp:Label runat="server" ID="lblCPMessage" ForeColor="red"></asp:Label>
                        <div class="sectiontitle">
                            Contract Products<asp:Label ID="lblCPTitle" runat="server"></asp:Label></div>
                        <asp:Panel runat="server" ID="CP_FilterPanel" Visible="false">
                            <div class="onecolumn">
                                <asp:Label ID="lblFilter" runat="server">Filter</asp:Label>
                                <span class="inputs">
                                    <asp:TextBox ID="txtFilter" runat="server"></asp:TextBox>
                                </span><span class="inputicon">
                                    <asp:ImageButton ID="cmdRefresh" runat="server" ImageUrl="./buttons/refresh.gif"
                                        OnClick="cmdRefresh_Click"></asp:ImageButton>
                                </span>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="panelCPViewStatus" Visible="false">
                            <asp:Label runat="server" ID="lblViewStatus" AssociatedControlID="rdoCPStatus">View Status</asp:Label>
                            <span class="inputs">
                                <asp:RadioButtonList ID="rdoCPStatus" runat="server" RepeatDirection="Horizontal"
                                    AutoPostBack="True" OnSelectedIndexChanged="rdoCPStatus_SelectedIndexChanged">
                                    <asp:ListItem Value="0">Live</asp:ListItem>
                                    <asp:ListItem Value="1">Archived</asp:ListItem>
                                </asp:RadioButtonList>
                            </span>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="panelAddAnother" Visible="false">
                            <div class="twocolumn">
                                <asp:Label runat="server" ID="lblAddAnother" AssociatedControlID="chkAddAnother"
                                    Text="Add Another?"></asp:Label>
                                <span class="inputs">
                                    <asp:CheckBox ID="chkAddAnother" runat="server"></asp:CheckBox></span>
                            </div>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="CPData">
                        </asp:Panel>
                        <asp:Panel runat="server" ID="CP_EditFieldsPanel" Visible="false">
                            <div class="twocolumn">
                                <asp:Label ID="lblProductName" runat="server" Text="Product Name" AssociatedControlID="lstProductName"></asp:Label><span
                                    class="inputs"><asp:DropDownList ID="lstProductName" TabIndex="1" runat="server"
                                        ValidationGroup="cpdetails">
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtProductName" TabIndex="1" runat="server" Visible="False" Enabled="False"
                                        ValidationGroup="cpdetails"></asp:TextBox></span><span class="inputicon"></span><span
                                            class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                                                ID="reqProductName" runat="server" ForeColor="Red" ErrorMessage="Product Name is mandatory"
                                                ControlToValidate="txtProductName">*</asp:RequiredFieldValidator>
                                            </span>
                                <asp:Label ID="lblProductValue" runat="server" Text="Product Value" AssociatedControlID="txtProductValue"></asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtProductValue" TabIndex="2" runat="server" ValidationGroup="cpdetails"></asp:TextBox><cc1:FilteredTextBoxExtender
                                        ID="ftexCProdValue" runat="server" TargetControlID="txtProductValue" FilterType="Custom, Numbers"
                                        ValidChars=".">
                                    </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblPricePaid" runat="server" Text="Price Paid" AssociatedControlID="txtPricePaid"></asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtPricePaid" TabIndex="3" runat="server" ValidationGroup="cpdetails"></asp:TextBox>
                                    <cc1:FilteredTextBoxExtender ID="ftexPricePaid" runat="server" TargetControlID="txtPricePaid"
                                        FilterType="Custom, Numbers" ValidChars=".">
                                    </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                                <asp:Label ID="lblMaintValue" runat="server" Text="Maintenance Value" AssociatedControlID="txtMaintValue"></asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtMaintValue" TabIndex="4" runat="server" ValidationGroup="cpdetails"></asp:TextBox><cc1:FilteredTextBoxExtender
                                        ID="ftexMaintAmount" runat="server" TargetControlID="txtMaintValue" FilterType="Custom, Numbers"
                                        ValidChars=".">
                                    </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblMaintPercent" runat="server" Text="Maintenance Percentage" AssociatedControlID="txtMaintPercent"></asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtMaintPercent" TabIndex="5" runat="server" MaxLength="6"
                                        ValidationGroup="cpdetails"></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftexMaintPct"
                                            runat="server" TargetControlID="txtMaintPercent" FilterType="Custom, Numbers"
                                            ValidChars=".">
                                        </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                                <asp:Label ID="lblUnits" runat="server" Text="Units" AssociatedControlID="lstUnits"></asp:Label><span
                                    class="inputs"><asp:DropDownList ID="lstUnits" TabIndex="6" runat="server" ValidationGroup="cpdetails">
                                    </asp:DropDownList>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblCPCurrency" runat="server" Text="Currency" AssociatedControlID="lstCPCurrency"></asp:Label><span
                                    class="inputs"><asp:DropDownList ID="lstCPCurrency" TabIndex="7" runat="server" ValidationGroup="cpdetails">
                                    </asp:DropDownList>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                                <asp:Label ID="lblQuantity" runat="server" Text="Quantity" AssociatedControlID="txtQuantity"></asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtQuantity" TabIndex="8" runat="server" ValidationGroup="cpdetails"></asp:TextBox><cc1:FilteredTextBoxExtender
                                        ID="ftexCPQty" runat="server" TargetControlID="txtQuantity" FilterType="Numbers">
                                    </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblUnitCost" runat="server" AssociatedControlID="txtUnitCost" Text="Unit Cost"></asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtUnitCost" TabIndex="9" runat="server" ValidationGroup="cpdetails"></asp:TextBox><cc1:FilteredTextBoxExtender
                                        ID="ftexUnitCost" runat="server" TargetControlID="txtUnitCost" FilterType="Custom, Numbers"
                                        ValidChars=".">
                                    </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                                <asp:Label ID="lblProjectedSaving" runat="server" AssociatedControlID="txtProjectedSaving">proj saving</asp:Label><span
                                    class="inputs"><asp:TextBox ID="txtProjectedSaving" TabIndex="10" runat="server"
                                        ValidationGroup="cpdetails"></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftexProjSaving"
                                            runat="server" TargetControlID="txtProjectedSaving" FilterType="Custom, Numbers"
                                            ValidChars=".">
                                        </cc1:FilteredTextBoxExtender>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span>
                            </div>
                            <div class="twocolumn">
                                <asp:Label ID="lblCPSalesTax" runat="server" Text="sales tax" AssociatedControlID="lstCPSalesTax"></asp:Label><span
                                    class="inputs"><asp:DropDownList ID="lstCPSalesTax" TabIndex="11" runat="server"
                                        ValidationGroup="cpdetails">
                                    </asp:DropDownList>
                                </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                    class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon">
                                    </span><span class="inputtooltipfield"></span><span class="inputvalidatorfield">
                                </span>
                            </div>
                            <asp:Panel runat="server" ID="CPproductcategorypanel" Visible="false">
                                <div class="twocolumn">
                                    <asp:Label ID="lblProductCategory" runat="server" AssociatedControlID="lstProductCategory">product category</asp:Label><span
                                        class="inputs"><asp:DropDownList ID="lstProductCategory" TabIndex="12" runat="server"
                                            ValidationGroup="cpdetails">
                                        </asp:DropDownList>
                                    </span>
                                </div>
                            </asp:Panel>
                            <asp:PlaceHolder runat="server" ID="phCPUFields"></asp:PlaceHolder>
                            <div class="formbuttons">
                                <asp:ImageButton runat="server" OnClientClick="ValidateCP()" ID="cmdCPUpdate" ImageUrl="./buttons/update.gif"
                                    Visible="false" CausesValidation="false" />&nbsp;
                                <asp:ImageButton runat="server" ID="cmdCPCancel" ImageUrl="./buttons/cancel.gif"
                                    Visible="false" CausesValidation="false" /></div>
                        </asp:Panel>
                    </div>
                    <div id="PopUpPanel" style="background-color: #ffffff; border: solid 1px #000000;
                        visibility: hidden; line-height: 25px; height: 50px; padding: 4px;">
                    </div>
                    <cc1:PopupControlExtender ID="popOptions" runat="server" TargetControlID="testLink"
                        PopupControlID="PopUpPanel" Position="Right">
                    </cc1:PopupControlExtender>
                    <asp:HyperLink ID="testLink" runat="server" Text="Test" NavigateUrl="javascript:void(0);"
                        Style="visibility: hidden;"></asp:HyperLink>
                </asp:View>
                <asp:View ID="vwInvoiceDetails" runat="server">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            Invoice Details<asp:Label ID="lblIDTitle" runat="server"></asp:Label></div>
                        <asp:Panel runat="server" ID="panelIDFilter">
                            <table>
                                <tr>
                                    <td class="labeltd">
                                        Invoice Status Filter
                                    </td>
                                    <td class="inputtd">
                                        <asp:DropDownList runat="server" ID="lstInvArchived" AutoPostBack="true" OnSelectedIndexChanged="lstInvArchived_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                    <div class="inputpanel">
                        <asp:Literal runat="server" ID="litInvoiceData"></asp:Literal>
                    </div>
                    <asp:Panel runat="server" ID="invEditFieldsPanel" Visible="false">
                        <div class="inputpanel">
                            <table>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="invID" runat="server" Visible="false"></asp:Label><asp:Label ID="lblInvoiceNumber"
                                            runat="server">invoice number</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtInvoiceNumber" TabIndex="1" runat="server" MaxLength="15"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblReceivedDate" runat="server">received</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="txtReceivedDate" TabIndex="6"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <asp:ImageButton runat="server" ID="calIDReceivedDate" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIDReceivedDate" Format="dd/MM/yyyy"
                                            TargetControlID="txtReceivedDate" PopupButtonID="calIDReceivedDate" PopupPosition="BottomRight">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDReceivedDate" ControlToValidate="txtReceivedDate"
                                            Text="*" ErrorMessage="Invalid Recieved Date entered" SetFocusOnError="true"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDAmount" runat="server">amount</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtIDAmount" TabIndex="2" runat="server"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <cc1:FilteredTextBoxExtender ID="ftexAmount" runat="server" TargetControlID="txtIDAmount"
                                            FilterType="Custom, Numbers" ValidChars=".,-">
                                        </cc1:FilteredTextBoxExtender>
                                        <asp:CompareValidator runat="server" ID="cmptxtIDAmount" ControlToValidate="txtIDAmount"
                                            Operator="DataTypeCheck" Type="Double" ErrorMessage="Invalid Amount entered"
                                            Text="*" SetFocusOnError="True"></asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender runat="server" ID="cmpextxtIDAmount" TargetControlID="cmptxtIDAmount">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblDueDate" runat="server">due date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="txtDueDate" TabIndex="7"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <asp:ImageButton runat="server" ID="calIDDueDate" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIDDueDate" TargetControlID="txtDueDate"
                                            PopupButtonID="calIDDueDate" PopupPosition="BottomRight" Format="dd/MM/yyyy">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDDueDate" ControlToValidate="txtDueDate"
                                            ErrorMessage="Invalid Due Date entered" Text="*" SetFocusOnError="true" Operator="DataTypeCheck"
                                            Type="Date" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                        <asp:RequiredFieldValidator runat="server" ID="reqIDDueDate" ControlToValidate="txtDueDate"
                                            SetFocusOnError="True" ErrorMessage="Due Date is mandatory" Text="*" ValidationGroup="vgInvoiceDetails"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblStatus" runat="server">status</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:DropDownList ID="lstStatus" TabIndex="3" runat="server">
                                        </asp:DropDownList>
                                        <asp:CompareValidator runat="server" ID="reqStatus" Type="Integer" ValueToCompare="0"
                                            Operator="GreaterThan" ControlToValidate="lstStatus" ErrorMessage="Invoice Status is a required field"
                                            Text="*" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                    <td class="inputtd_padded">
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblInvoicePaidDate" runat="server">paid date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="txtInvoicePaidDate" TabIndex="8"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <asp:ImageButton runat="server" ID="calIDPaidDate" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIDPaidDate" TargetControlID="txtInvoicePaidDate"
                                            PopupButtonID="calIDPaidDate" PopupPosition="BottomRight" Format="dd/MM/yyyy">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDPaidDate" ControlToValidate="txtInvoicePaidDate"
                                            Operator="DataTypeCheck" Type="Date" ErrorMessage="Invalid Paid Date entered"
                                            Text="*" SetFocusOnError="true" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblPaymentRef" runat="server">payment ref</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtPaymentRef" TabIndex="4" runat="server" MaxLength="25"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDCoverEnd" runat="server" Text="cover period ends"></asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="txtIDCoverEnd" TabIndex="9"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <asp:ImageButton runat="server" ID="calIDCoverEnd" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIDCoverEnd" TargetControlID="txtIDCoverEnd"
                                            PopupButtonID="calIDCoverEnd" PopupPosition="BottomRight" Format="dd/MM/yyyy">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDCoverEnd" ControlToValidate="txtIDCoverEnd"
                                            Operator="DataTypeCheck" Type="Date" Text="*" ErrorMessage="Invalid Cover Period Ends date entered"
                                            SetFocusOnError="true" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblInvSalesTax" runat="server">sales tax</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:DropDownList ID="lstInvSalesTax" TabIndex="5" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="inputtd_padded">
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDComment" runat="server">comment</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtInvComment" TabIndex="10" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="inputpanel">
                            <div class="inputpaneltitle">
                                Purchase Order Details</div>
                            <table>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDPONumber" runat="server">po number</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtIDPONumber" runat="server" MaxLength="30" TabIndex="11"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDPOStart" runat="server">po start date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="dateIDPOStart" TabIndex="13"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <asp:ImageButton runat="server" ID="calIDPOStart" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIDPOStart" Format="dd/MM/yyyy" PopupButtonID="calIDPOStart"
                                            PopupPosition="BottomRight" TargetControlID="dateIDPOStart">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDPOStart" ControlToValidate="dateIDPOStart"
                                            ErrorMessage="Invalid PO Strat Date entered" Text="*" SetFocusOnError="true"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDPOMaxValue" runat="server">po max value</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtIDPOMaxValue" runat="server" TabIndex="12"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <cc1:FilteredTextBoxExtender ID="ftexPOMaxVal" runat="server" TargetControlID="txtIDPOMaxValue"
                                            FilterType="Custom, Numbers" ValidChars=".">
                                        </cc1:FilteredTextBoxExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDPOMaxValue" ControlToValidate="txtIDPOMaxValue"
                                            ErrorMessage="Invalid PO Max Value entered" Operator="DataTypeCheck" Type="Double"
                                            Text="*" SetFocusOnError="true" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIDPOExpiry" runat="server">po expiry date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="dateIDPOExpiry" TabIndex="14"></asp:TextBox>
                                    </td>
                                    <td class="inputtd_padded">
                                        <asp:ImageButton runat="server" ID="calIDPOExpiry" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIDPOExpiry" TargetControlID="dateIDPOExpiry"
                                            PopupButtonID="calIDPOExpiry" PopupPosition="BottomRight" Format="dd/MM/yyyy">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIDPOExpiry" ControlToValidate="dateIDPOExpiry"
                                            ErrorMessage="Invalid PO Expiry Date entered" Text="*" SetFocusOnError="true"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="vgInvoiceDetails"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>
                    <div class="inputpanel">
                        <asp:ImageButton runat="server" ID="cmdIDUpdate" ImageUrl="./buttons/update.gif"
                            Visible="false" OnClick="cmdIDUpdate_Click" OnClientClick="validateform('vgInvoiceDetails');"
                            CausesValidation="false" />&nbsp;
                        <asp:ImageButton runat="server" ID="cmdIDCancel" ImageUrl="./buttons/cancel.gif"
                            Visible="false" CausesValidation="false" OnClick="cmdIDCancel_Click" />
                        <asp:ImageButton runat="server" ID="cmdIDClose" ImageUrl="./buttons/page_close.gif"
                            CausesValidation="false" />
                    </div>
                </asp:View>
                <asp:View ID="vwInvoiceForecasts" runat="server">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            Invoice Forecasts<asp:Label ID="lblIFTitle" runat="server"></asp:Label></div>
                    </div>
                    <div class="inputpanel">
                        <asp:Label ID="lblStatusMsg" runat="server"></asp:Label>
                    </div>
                    <div class="inputpanel">
                        <asp:Literal runat="server" ID="litForecastData"></asp:Literal>
                    </div>
                    <asp:Panel runat="server" ID="ForecastEditFieldsPanel" Visible="false">
                        <div class="inputpanel">
                            <table>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblForecastDate" runat="server">forecast date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtForecastDate" runat="server" TabIndex="1"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton runat="server" ID="calForecastDate" ImageUrl="~/icons/16/plain/calendar.gif"
                                            CausesValidation="false" /><asp:RequiredFieldValidator runat="server" ID="reqForecastDate"
                                                ControlToValidate="txtForecastDate" ErrorMessage="Forecast date is mandatory"
                                                Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <cc1:CalendarExtender ID="calexForecastDate" runat="server" Format="dd/MM/yyyy" TargetControlID="txtForecastDate"
                                            PopupPosition="BottomRight" PopupButtonID="calForecastDate">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpForecastDate" ControlToValidate="txtForecastDate"
                                            Text="*" ErrorMessage="Invalid date value entered" SetFocusOnError="true" Operator="DataTypeCheck"
                                            Type="Date"></asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender runat="server" ID="cmpexForecastDate" TargetControlID="cmpForecastDate">
                                        </cc1:ValidatorCalloutExtender>
                                        <cc1:ValidatorCalloutExtender ID="reqexForecastDate" runat="server" TargetControlID="reqForecastDate">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblForecastAmount" runat="server">forecast amount</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtForecastAmount" TabIndex="2" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <cc1:FilteredTextBoxExtender ID="ftexForecastAmount" runat="server" TargetControlID="txtForecastAmount"
                                            FilterType="Custom, Numbers" ValidChars=".">
                                        </cc1:FilteredTextBoxExtender>
                                        <asp:RequiredFieldValidator runat="server" ID="reqForecastAmount" ControlToValidate="txtForecastAmount"
                                            ErrorMessage="Forecast Amount is mandatory" Text="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="cmpForecastAmount" runat="server" ControlToValidate="txtForecastAmount"
                                            ErrorMessage="Invalid amount provided" Operator="DataTypeCheck" SetFocusOnError="True"
                                            Type="Double">*</asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender ID="cmpexForecastAmount" runat="server" TargetControlID="cmpForecastAmount">
                                        </cc1:ValidatorCalloutExtender>
                                        <cc1:ValidatorCalloutExtender runat="server" ID="reqexForecastAmount" TargetControlID="reqForecastAmount">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label runat="server" ID="lblIFCoverEnd">period cover end</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" TabIndex="3" ID="txtIFCoverEnd"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton runat="server" ID="calIFCoverEnd" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" /><cc1:CalendarExtender
                                            runat="server" ID="calexIFCoverEnd" Format="dd/MM/yyyy" PopupButtonID="calIFCoverEnd"
                                            TargetControlID="txtIFCoverEnd" PopupPosition="BottomRight">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIFCoverEnd" Operator="DataTypeCheck"
                                            ControlToValidate="txtIFCoverEnd" Type="Date" Text="*" ErrorMessage="Invalid date provided"
                                            SetFocusOnError="true"></asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender runat="server" ID="cmpexIFCoverEnd" TargetControlID="cmpIFCoverEnd">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                    <td colspan="3">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIFPONumber" runat="server">po number</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtIFPONumber" TabIndex="4" runat="server" MaxLength="30"></asp:TextBox>
                                    </td>
                                    <td>
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIFPOStart" runat="server">po start date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="dateIFPOStart" TabIndex="5"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton runat="server" ID="calIFPOStart" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIFPOStart" TargetControlID="dateIFPOStart"
                                            PopupButtonID="calIFPOStart" Format="dd/MM/yyyy" PopupPosition="BottomRight">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIFPOStart" Operator="DataTypeCheck" Type="Date"
                                            ErrorMessage="Invalid date entered" Text="*" SetFocusOnError="true" ControlToValidate="dateIFPOStart"></asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender runat="server" TargetControlID="cmpIFPOStart" ID="cmpexIFPOStart">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIFPOExpiry" runat="server">po expiry date</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox runat="server" ID="dateIFPOExpiry" TabIndex="6"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton runat="server" ID="calIFPOExpiry" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                        <cc1:CalendarExtender runat="server" ID="calexIFPOExpiry" Format="dd/MM/yyyy" PopupButtonID="calIFPOExpiry"
                                            TargetControlID="dateIFPOExpiry" PopupPosition="BottomRight">
                                        </cc1:CalendarExtender>
                                        <asp:CompareValidator runat="server" ID="cmpIFPOExpiry" ControlToValidate="dateIFPOExpiry"
                                            Text="*" ErrorMessage="Invalid date entered" SetFocusOnError="true" Type="Date"
                                            Operator="DataTypeCheck"></asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender runat="server" TargetControlID="cmpIFPOExpiry" ID="cmpexIFPOExpiry">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIFPOMaxValue" runat="server">po max value</asp:Label>
                                    </td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtIFPOMaxValue" runat="server" TabIndex="7"></asp:TextBox>
                                    </td>
                                    <td>
                                        <cc1:FilteredTextBoxExtender ID="ftexIFPOMaxVal" runat="server" TargetControlID="txtIFPOMaxValue"
                                            FilterType="Custom, Numbers" ValidChars=".">
                                        </cc1:FilteredTextBoxExtender>
                                        <asp:CompareValidator ID="cmpPOMaxValue" runat="server" ControlToValidate="txtIFPOMaxValue"
                                            ErrorMessage="Invalid value provided" Operator="DataTypeCheck" SetFocusOnError="True"
                                            Type="Double">*</asp:CompareValidator>
                                        <cc1:ValidatorCalloutExtender ID="cmpexPOMaxValue" runat="server" TargetControlID="cmpPOMaxValue">
                                        </cc1:ValidatorCalloutExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">
                                        <asp:Label ID="lblIFComment" runat="server">comment</asp:Label>
                                    </td>
                                    <td class="inputtd" colspan="5">
                                        <asp:TextBox ID="txtIFComment" TabIndex="8" runat="server" MaxLength="250" TextMode="MultiLine"
                                            Rows="2" Width="100%"></asp:TextBox>
                                    </td>
                                </tr>
                                <asp:Panel ID="IFProductBreakdownPanel" runat="server" Visible="false">
                                    <tr>
                                        <td class="labeltd">
                                            <asp:Label ID="lblProductAmount" runat="server">product / amount</asp:Label>
                                        </td>
                                        <td class="inputtd">
                                            <asp:PlaceHolder ID="holderBreakdownButton" runat="server"></asp:PlaceHolder>
                                        </td>
                                        <td>
                                        </td>
                                        <td colspan="3">
                                        </td>
                                    </tr>
                                </asp:Panel>
                            </table>
                        </div>
                        <div class="inputpanel">
                            <asp:ImageButton runat="server" ID="cmdIFUpdate" ImageUrl="~/buttons/update.gif"
                                OnClick="cmdIFUpdate_Click" />&nbsp;
                            <asp:ImageButton runat="server" ID="cmdIFCancel" ImageUrl="~/buttons/cancel.gif"
                                CausesValidation="false" OnClick="cmdIFCancel_Click" />
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="IFBreakdownPanel" Visible="false">
                        <div class="inputpanel" id="divBreakdownTable">
                            <asp:Literal ID="litBreakdownTable" runat="server"></asp:Literal></div>
                        <div class="inputpanel">
                            <asp:ImageButton runat="server" ID="cmdBUUpdate" ImageUrl="~/buttons/update.gif"
                                OnClick="cmdBUUpdate_Click" />&nbsp;
                            <asp:ImageButton runat="server" ID="cmdBUCancel" ImageUrl="~/buttons/cancel.gif"
                                CausesValidation="false" OnClick="cmdBUCancel_Click" />
                        </div>
                    </asp:Panel>
                    <div class="inputpanel">
                        <asp:ImageButton runat="server" ID="cmdIFClose" ImageUrl="~/buttons/page_close.gif"
                            CausesValidation="false" OnClick="cmdIFClose_Click" /></div>
                </asp:View>
                <asp:View ID="vwNoteSummary" runat="server">
                    <div class="inputpanel">
                        <div class="inputpaneltitle">
                            Note Summary<asp:Label ID="lblNSTitle" runat="server"></asp:Label></div>
                        <asp:Literal runat="server" ID="litNoteSummaryData"></asp:Literal>
                    </div>
                    <div class="inputpanel">
                        <asp:ImageButton runat="server" ID="cmdNSClose" ImageUrl="~/buttons/page_close.gif"
                            CausesValidation="false" />
                    </div>
                </asp:View>
                <asp:View ID="vwLinkedContracts" runat="server">
                    <asp:Literal ID="litLinkData" runat="server"></asp:Literal>
                    <div class="inputpanel">
                        <asp:ImageButton runat="server" ID="cmdLCCancel" CausesValidation="false" ImageUrl="~/buttons/page_close.gif" />
                    </div>
                </asp:View>
            </asp:MultiView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script language="javascript" type="text/javascript">
        var rteModal = '<%=mdlTextEditor.ClientID %>';        
    </script>
    <asp:Panel runat="server" Style="display: none;" CssClass="modalpanel" ID="pnlRTEdit">
        <div class="formpanel">
            <div class="sectiontitle">
                <span id="lblTextEditorHeading"></span>
            </div>
            <div class="onecolumnlarge">
                <label id="lblRTE" for="txtRTE">
                </label>
                <span class="inputs">
                    <textarea id="txtRTE" style="width: 100%;" rows="14"></textarea></span>
            </div>
            <div class="formbuttons">
                <asp:ImageButton runat="server" ID="cmdTESave" AlternateText="Save" ImageUrl="~/Buttons/update.gif"
                    OnClientClick="javascript:saveRTEdit();" CausesValidation="false" />&nbsp;&nbsp;<asp:ImageButton
                        runat="server" ID="cmdTECancel" AlternateText="Cancel" ImageUrl="~/Buttons/cancel.gif"
                        CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="mdlTextEditor" TargetControlID="lnkLaunchModal"
        BackgroundCssClass="modalBackground" CancelControlID="cmdTECancel" OkControlID="cmdTESave"
        PopupControlID="pnlRTEdit" OnOkScript="saveRTEdit">
    </cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkLaunchModal" Style="display: none;">&nbsp;</asp:LinkButton>
    <div id="Div1">
    </div>
    <div id="broadcastmsg">
    </div>
    <script language="javascript" type="text/javascript">
        function showCPTaskOptions(evt, clickControlID, cpID, retURL) {
            var e = (evt) ? evt : window.event;

            if (window.event) {
                e.cancelBubble = true;
            } else {
                e.stopPropagation();
            }

            var optionsDiv = document.getElementById('PopUpPanel');
            //optionsDiv.innerHTML = "<a href=\"javascript:window.location.href='Home.aspx';\" title=\"Test Link 1\">Link 1</a>";
            //optionsDiv.innerHTML += "<a href=\"javascript:window.location.href='Home.aspx';\" title=\"Test Link 2\">Link 2</a>";
            optionsDiv.innerHTML = "<a href=\"javascript:window.location.href='shared/tasks/TaskSummary.aspx?pid=" + cpID + "&paa=2&ret=" + retURL + "';\" title=\"Show Task Summary\" style=\"line-height: 25px;\">Show Task Summary</a>";
            optionsDiv.innerHTML += "<br /><a href=\"javascript:window.location.href='shared/tasks/ViewTask.aspx?tid=0&rid=" + cpID + "&rtid=2&ret=" + retURL + "';\" title=\"Add New Task\" style=\"line-height: 25px;\">Add New Task</a>";

            $find('<%= popOptions.ClientID %>')._popupBehavior._parentElement = document.getElementById('ctl00_contentmain_' + clickControlID);
            $find('<%= popOptions.ClientID %>').showPopup();
            return;
        }

        function ValidateCP() {
            if (validateform() === false) {
                return;
            }
        }
    </script>
    <asp:Panel ID="pnlDummyRTB" runat="server">
    </asp:Panel>
</asp:Content>
