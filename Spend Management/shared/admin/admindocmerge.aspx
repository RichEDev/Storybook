<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDocMerge.aspx.cs"
    Inherits="Spend_Management.AdminDocMerge" MasterPageFile="~/masters/smForm.master" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="styles">
   
    <style type="text/css"> 
        .formpanel .formbuttons {
            padding-top: 7px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmenu">
    <link rel="stylesheet" type="text/css" media="screen" href="<%= ResolveUrl("~/shared/css/layout.css") %>" />
    <link rel="stylesheet" type="text/css" media="screen" href="<% = ResolveUrl("~/shared/css/styles.aspx") %>" />
    <a href="javascript:SEL.DocMerge.LaunchModal(pnlRepSrc, repSrcModal);" id="lnkAddReportSource" class="submenuitem">New Report Source</a>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmain">

    <asp:ScriptManagerProxy runat="server" ID="SMgrProxy">
        <Services>
            <asp:ServiceReference InlineScript="true" Path="~/shared/webServices/svcDocumentMerge.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Name="tooltips" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.docMerge.js" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.filterDialog.js?date=20180126"/>
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">
        // variables for merge process
        var currentMergeProjectId;
        var currentMergeRequestNumber;

        // variables for merge configuration
        var currentReportId;
        var blnNewMTS = false;
        var repSrcModal = '<%=mdlRepSource.ClientID %>';
        var currentProjectId = '<%=ViewState["docMergeProjectId"] %>';
        var reportSrcLiteral = '<%=litReportSources.ClientID %>';
        var tabContainer = '<%=configurationTabContainer.ClientID %>';
        var txtProjectName = '<%= txtProjectName.ClientID %>';
        var txtProjectDescription = '<%= txtProjectDescription.ClientID %>';
        var ddlReport = '<%= ddlReport.ClientID %>';
        var ddlReportCategory = '<%= ddlReportCategory.ClientID %>';
        var mdlValidation = '<%= mdlValidation.ClientID %>';
        var divReportSources = '<%= divReportSources.ClientID %>';
        var pnlRepSrc = '<%= pnlRepSrc.ClientID %>';
        var divFields = '<%=divFields.ClientID %>';
        var divDocGroupingConfigs = '<%=divDocGroupingConfigs.ClientID %>';

        (function (configs) {
            configs.txtConfigLabel = '<%= this.txtConfigLabel.ClientID %>';
            configs.txtConfigDescription = '<%= this.txtConfigDescription.ClientID %>';
            configs.tabMaintainConfig = '<% = this.tabMaintainConfig.ClientID %>';
            configs.hdnCurrentConfigId = '<% = this.hdnCurrentConfigId.ClientID %>';
            configs.chkIsDefaultDocumentGroupingConfig = '<% = this.chkIsDefaultDocumentGroupingConfig.ClientID %>';
            configs.hdnDefaultDocumentGroupingId = '<% = this.hdnDefaultDocumentGroupingId.ClientID %>';
            configs.ddlReportSource = '<%=ddlReportSource.ClientID %>';
            configs.hdnReportSortingName = '<%=hdnReportSortingName.ClientID %>';
        }(SEL.DocMerge.DomIDs.Config));



    </script>
    
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">
            Document Configuration Definition
        </div>
        <div>
            <asp:Label runat="server" ID="lblDefMessage"></asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label runat="server" ID="lblProjectName" Text="Configuration name*" AssociatedControlID="txtProjectName"
                CssClass="mandatory"></asp:Label>
            <span class="inputs">
                <asp:TextBox runat="server" ID="txtProjectName" TabIndex="1" ValidationGroup="project" MaxLength="150"
                    CssClass="fillspan"></asp:TextBox>
            </span>
            <span class="inputicon"></span><span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield">
                <asp:RequiredFieldValidator runat="server" ID="reqProjectName" ControlToValidate="txtProjectName"
                    ValidationGroup="project" Text="*" ErrorMessage="Please enter a Configuration name" SetFocusOnError="true"></asp:RequiredFieldValidator>
            </span>
        </div>
        <div class="onecolumn">
            <asp:Label runat="server" ID="lblProjectDesc" Text="" AssociatedControlID="txtProjectDescription"><p class="labeldescription">Description</p></asp:Label>
            <span class="inputs">
                <asp:TextBox runat="server" ID="txtProjectDescription" TextMode="MultiLine" CssClass="fillspan" MaxLength="500"></asp:TextBox></span>
            <span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
               <input id="hdnDefaultDocumentGroupingId" type="hidden" runat="server" value="false" />
        </div>
        <div class="twocolumn">
            <asp:Label id="lblBlanktext" AssociatedControlID="txtBlankText" runat="server">Blank table text</asp:Label>
            <span class="inputs"><asp:TextBox runat="server" CssClass="fillspan blankText" ID="txtBlankText" maxlength="100"/></span>
            <span class="inputvalidatorfield"></span>
        </div>
    </div>
    <cc1:TabContainer ID="configurationTabContainer" runat="server" OnClientActiveTabChanged="SEL.DocMerge.TabClick" CssClass="ajax__tab_xp formpanel formpanel_padding">
        <cc1:TabPanel ID="reportSourcesTab" runat="server" Enabled="true">
            <HeaderTemplate>
                Source Selections
            </HeaderTemplate>
            <ContentTemplate>
                <div class="sectiontitle">
                    Current Source Selections
                </div>
                <div id="divReportSources" runat="server">
                    <asp:Literal runat="server" ID="litReportSources"></asp:Literal>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel ID="tabDocGroupingConfigs" runat="server" Enabled="true">
            <HeaderTemplate>
                Document Grouping Configurations
            </HeaderTemplate>
            <ContentTemplate>
                <div class="sectiontitle">
                    Current Document Grouping Configurations
                </div>
                <span>
                    <a href="javascript:SEL.DocMerge.LaunchGroupingModal(0);">New Document Grouping Configuration</a>
                </span>
                <div id="divDocGroupingConfigs" runat="server" style="margin-top: 10px;" class="docGroupingConfigs">
                    <asp:Literal runat="server" ID="ltGroupingConfigurations"></asp:Literal>
                </div>
            </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
    <div style="display: none" class="documentGroupingConfigurationDiv">
        <div class="sectiontitle documentGroupingConfigurationTitle" id="documentGroupingConfigurationTitle">Title</div>
        <cc1:TabContainer ID="tabMaintainConfig" runat="server" OnClientActiveTabChanged="SEL.DocMerge.tabMaintainConfigClick" CssClass="ajax__tab_xp formpanel formpanel_padding">
            <cc1:TabPanel ID="configDetailsTab" runat="server">
                <HeaderTemplate>
                    General Details
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">
                       General Details
                    </div>                  
                        <div class="twocolumn">
                        <label id="lblConfiglabel" class="mandatory" for="txtConfigLabel">Configuration label*</label>
                            <span class="inputs">
                            <asp:TextBox runat="server" ID="txtConfigLabel" CssClass="fillspan" MaxLength="100" ValidationGroup="vgConfigLabel"></asp:TextBox>
                            </span>
                            <span class="inputvalidatorfield">
                            <asp:RequiredFieldValidator runat="server" ID="reqConfigLabel" ControlToValidate="txtConfigLabel"
                                ValidationGroup="vgConfigLabel" Text="*" ErrorMessage="Please enter a Configuration label in the box provided." SetFocusOnError="true"></asp:RequiredFieldValidator>
                            </span>
                            <span id="spanDefaultDocumentGroupingChk">
                            <asp:Label runat="server" ID="labelDefaultConfig" Text="Default grouping configuration" AssociatedControlID="chkIsDefaultDocumentGroupingConfig" CssClass="Doesthisvehicle_lable"></asp:Label>
                                 <span class="inputs">
                                <asp:CheckBox runat="server" ID="chkIsDefaultDocumentGroupingConfig" />
                                 </span>
                            </span>
                        <input type="hidden" runat="server" id="hdnCurrentConfigId" />
                        </div>
                    <div class="onecolumn">
                        <label id="lblConfigDescription" for="txtConfigDescription" ><div style="width:160px;">Description</div></label>
                            <span class="inputs">
                            <asp:TextBox runat="server" ID="txtConfigDescription" CssClass="fillspan" MaxLength="250" TextMode="MultiLine"></asp:TextBox>
                            </span>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="groupingTab" runat="server">
                <HeaderTemplate>
                    Grouping
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">
                        Grouping Field Selections
                    </div>				    
                    <div id="divFields" runat="server">
                    <div id="divGroupingFields"></div>
                    <div class="formpanel formpanel_padding">
                    </div>
                </div>
                    <span id="infoGrouping" class="infoHelpArea" runat="server" clientidmode="Static" style="z-index: 7878787878">Move the columns up or down to re-order them.  Columns can be dragged and dropped between the Included and Excluded areas.<br />
                        <br />
                        Grouping will allow you to define the fields that you would like to use to re-order your document
                       (the fields that are common between the report sources you decide to use).
                    <br />
                    <br />
                     The fields residing in the ‘Included Grouping’ area will be active fields used to re-order a document. Drag and drop your preferred field to the top of the stack to make it the primary grouping column.
                    <br />
                    <br />
                  The fields residing in the ‘Excluded Grouping’ area will be inactive, and therefore have no effect on grouping. Columns can be dragged between the Included and Excluded areas.
                    <br />
                    <br />                   
                    Click Save when finished to confirm the Grouping Field Selections for the document configuration.
                </span>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel runat="server" ID="sortingTab">
            <HeaderTemplate>
                Sorting
            </HeaderTemplate>
            <ContentTemplate>
                <div class="sectiontitle">
                    Sorting Field Selections
                </div>
                    <div id="div1">
                    <div id="divSortingFields"></div>
                    <div class="formpanel">
                    </div>
                </div>
                    <span id="infoSorting" class="infoHelpArea" runat="server" clientidmode="Static" style="z-index: 7878787878">Move the columns up or down to re-order them.  Columns can be dragged and dropped between the Included and Excluded areas.<br />
                        <br />
                        Sorting will allow you to define the Grouping fields that are used for sorting.
                    <br />
                </span>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel runat="server" ID="filteringTab">
            <HeaderTemplate>
                Filtering
            </HeaderTemplate>
            <ContentTemplate>
                    <div id="div2" runat="server">
                <div class="sectiontitle">
                    Filtering Field Selections
                </div>
                    <div id="divFilteringFields"></div>
                    <div class="formpanel">
                    </div>
                </div>
                    <span id="infoFiltering" class="infoHelpArea" runat="server" clientidmode="Static" style="z-index: 7878787878">Move the columns up or down to re-order them.  Columns can be dragged and dropped between the Included and Excluded areas.<br />
                        <br />
                        Filtering will allow you to define the filters on the Grouping fields in your document.
                    <br />
                </span>
            </ContentTemplate>
        </cc1:TabPanel>
            <cc1:TabPanel runat="server" ID="filterAllTab">
            <HeaderTemplate>
                Sorting All Columns
            </HeaderTemplate>
            <ContentTemplate>
                    <div id="div3" runat="server">
                <div class="sectiontitle">
                    Sorting Field Selections for reports
                </div>
           <div class="twocolumn">
               <input type="hidden" runat="server" id="hdnReportSortingName" />
            <asp:Label runat="server" ID="Label1" Text="Report Source" AssociatedControlID="ddlReportSource"></asp:Label>
            <span class="inputs">
                <asp:DropDownList runat="server" ID="ddlReportSource" TabIndex="1"  CssClass="fillspan" style="width:350px;" onchange="SEL.DocMerge.ReportSourceSelected()"></asp:DropDownList>
            </span>
            <span class="inputicon"></span><span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield">
            </span>
        </div>
                <div id="divSortedColumnFields"></div>
                    <div class="formpanel">
                    </div>
                </div>
                    <span id="infoSortedColumn" class="infoHelpArea" runat="server" clientidmode="Static" style="z-index: 7878787878">Move the columns up or down to re-order them.  Columns can be dragged and dropped between the Included and Excluded areas.<br />
                        <br />
                        Sorting will allow you to define the sort order on any field in your document.
                    <br />
                </span>
            </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
        <div class="formbuttons">
            <helpers:CSSButton ID="cssSaveConfig" runat="server" Text="save" OnClientClick="javascript:SEL.DocMerge.SaveConfiguration();return false;" UseSubmitBehavior="False" TabIndex="0" />
            <helpers:CSSButton ID="cssCancelConfig" runat="server" Text="cancel" OnClientClick="javascript:SEL.DocMerge.CancelGroupingConfiguration();return false;" UseSubmitBehavior="False" TabIndex="0" />
            <div id="filteringButtons" style="display: inline; float: left; padding-right: 20px">
                <div id="ajaxLoaderConfig" style="width: 32px; display: none; position: relative; margin-left: 8px;">
                <img src="/shared/images/ajax-loader.gif" alt="" />
            </div>
            </div>
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlAddReportSource" Style="display: none;" CssClass="modalpanel">
        <div id="pnlRepSrc" class="formpanel " runat="server">
            <div class="sectiontitle">
                New Report Source
            </div>
            <div class="onecolumnsmall">
                <label id="lblReportCategory" for="ddlReportCategory" class="mandatory">
                    Report category*</label>
                <span class="inputs">
                    <select id="ddlReportCategory" runat="server" onchange="SEL.DocMerge.ReportCategoryIndexChanged();" class="fillspan">
                    </select>
                </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield">
                    <asp:Image ID="imgTPReportCategory" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('d26d4280-c8b7-4253-aba7-92202e581359', 'sm', this);" />
                </span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator runat="server" ID="reqReportCategory" ControlToValidate="ddlReportCategory"
                            ValidationGroup="vgReport" Text="*" ErrorMessage="Report category is mandatory" SetFocusOnError="true"></asp:RequiredFieldValidator>
                </span>
            </div>
            <div class="onecolumnsmall">
                <label id="lblReport" for="ddlReport" class="mandatory">
                    Report source*</label>
                <span class="inputs">
                    <select id="ddlReport" runat="server" enabled="false" cssclass="fillspan">
                        <option value="">[None]</option>
                    </select> 
                </span><span class="inputicon"></span>
                <span class="inputtooltipfield">
                    <asp:Image ID="imgTPReport" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('a8b4defc-f8ea-4733-be32-cc5841006805', 'sm', this);" />
                </span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator runat="server" ID="reqReport" ControlToValidate="ddlReport"
                            ValidationGroup="vgReport" Text="*" ErrorMessage="Report source is mandatory" SetFocusOnError="true"></asp:RequiredFieldValidator>
                </span>
            </div>
            <div class="formbuttons">
                <img runat="server" id="btnAddSource" src="/shared/images/buttons/btn_save.png" alt="Save report source" style="cursor: pointer;" onclick="javascript:SEL.DocMerge.AddReportSource();" />
                &nbsp;&nbsp;
                <img id="btnCancelReportSource" src="/shared/images/buttons/cancel_up.gif" alt="Cancel" style="cursor: pointer;" onclick="javascript:SEL.DocMerge.CancelModal();" />                                                                                                      
                <div id="ajaxLoaderReport" style="width: 32px; display: inline; position: relative; margin-left: 8px;">
                    <img src="../images/ajax-loader.gif" alt="" />
                </div>
            </div>
        </div>        
    </asp:Panel>

    <asp:Label runat="server" Text="" ID="lblDummy2" />
    <cc1:ModalPopupExtender runat="server" TargetControlID="lblDummy2" PopupControlID="pnlAddReportSource"
        BackgroundCssClass="modalBackground" ID="mdlRepSource"
        OnCancelScript="javascript:void(0);" CancelControlID="lblDummy">
    </cc1:ModalPopupExtender>

    <asp:Label runat="server" Text="" ID="lblDummy" />
    <asp:Panel ID="pnlValidation" runat="server" CssClass="errorModal" Style="display: none;">
        <div id="divValidation" runat="server">
            <asp:Label ID="lblValidation" runat="server" Text="" />
        </div>
        <div style="padding: 0px 5px 5px 5px;">
            <img src="/shared/images/buttons/btn_close.png" id="btnValidationClose" alt="OK" style="cursor: pointer;" />
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender
        ID="mdlValidation"
        TargetControlID="lblDummy"
        runat="server"
        BackgroundCssClass="modalMasterBackground"
        PopupControlID="pnlValidation"
        DropShadow="False"
        CancelControlID="btnValidationClose" />

    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>

    <div class="formpanel formpanel_padding">
        <div class="formbuttons lable-heightg1">
            <asp:ImageButton ID="btnSave" runat="server" src="/shared/images/buttons/btn_save.png" alt="Save" OnClientClick="javascript:return validateform('project');" OnClick="BtnSaveClick" Style="cursor: pointer;" />&nbsp;&nbsp;
                    <asp:ImageButton ID="btnCancel" runat="server" src="/shared/images/buttons/cancel_up.gif" alt="Cancel" OnClick="BtnCancelClick" Style="cursor: pointer;" />&nbsp;&nbsp;
                        <div id="ajaxLoaderMaster" style="width: 32px; display: none; position: relative; margin-left: 8px;">
                            <img src="../images/ajax-loader.gif" alt="" />
                        </div>
        </div>
    </div>
    
    <div id="torchPlugin"></div>

</asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentleft">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('a:contains("New Document Grouping Configuration")').click(function () {
                setTimeout(
                        function () {
                            $('.ajax__tab_disabled').css("cssText", "text-decoration: none; background-color: #A0A0A0!important; pointer-events: none; cursor: default;");
                        }, 100);
            });
        });
    </script>

</asp:Content>
