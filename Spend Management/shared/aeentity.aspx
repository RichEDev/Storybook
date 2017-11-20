<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeentity.aspx.cs" Inherits="Spend_Management.aeentity" ValidateRequest="false" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<%@ Register src="~/shared/usercontrols/CustomEntityRecordLockingControl.ascx" tagName="customEntityLocking" tagPrefix="elementLocking" %>
<%@Reference Control="~/shared/usercontrols/Selectinator.ascx" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<%@ Register Src="~/shared/usercontrols/audienceList.ascx" TagName="audienceList"
    TagPrefix="alg" %>
<%@ Register Src="~/shared/usercontrols/workflow.ascx" TagName="wf" TagPrefix="wf" %>
<asp:content id="Content4" contentplaceholderid="styles" runat="server">
    <style type="text/css">   
        .modalpanel .formpanel {
            width: 850px;
        }  
    </style>

</asp:content>
<asp:content id="Content1" contentplaceholderid="contentmenu" runat="server">
</asp:content>
<asp:content id="Content2" contentplaceholderid="contentleft" runat="server">
</asp:content>
<asp:content id="Content3" contentplaceholderid="contentmain" runat="server">
    <elementLocking:customEntityLocking runat="server" ID="aelocking" element="CustomEntities" ></elementLocking:customEntityLocking>
    <asp:ScriptManagerProxy runat="server" ID="scrmgrProxy">
		<Scripts>
			<asp:ScriptReference Path="~/shared/javaScript/minify/sel.docMerge.js" />
			<asp:ScriptReference Path="~/shared/javaScript/customEntities.js" />
			<asp:ScriptReference Path="~/shared/javaScript/workflows.js" />
			<asp:ScriptReference Path="~/shared/javaScript/ajaxEditor.js" />
            <asp:ScriptReference  Name="tooltips" />
            <asp:ScriptReference Path="~/shared/javaScript/Attachments.js?v=1" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.filterDialog.js"/>
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/SEL.CustomEntityRecordLocking.js" />
		</Scripts>
		<Services>
			<asp:ServiceReference Path="~/shared/webServices/svcCustomEntities.asmx" />
			<asp:ServiceReference Path="~/shared/webServices/svcWorkflows.asmx" InlineScript="false" />
            <asp:ServiceReference Path="~/shared/webServices/svcTooltip.asmx" InlineScript="false" />
            <asp:ServiceReference InlineScript="true" Path="~/shared/webServices/svcDocumentMerge.asmx" />
		</Services>
	</asp:ScriptManagerProxy>
	<script language="javascript" type="text/javascript">
	    $(window).load(function() {
	        dynamicButtonsPlacement();
	        $('.ajax__tab_tab').click(function () {
	            dynamicButtonsPlacement();
	        });
	    });

	    $(document).ready(function () {
	        $(document).keydown(function (e) {
	            if (e.keyCode === 27) // esc
	            {
	                e.preventDefault();
	                closeOpenModal();
	            }
	        });
            
	        $('#divMasterPopup').click(function (e) {
	            $('#hrefMasterPopup').focus();
	            return false;
	        });

	        jQuery.ImageBrowser.options['SearchFileName'] = '<%= txtViewCustomIconSearch.ClientID %>';
	        jQuery.ImageBrowser.options['StaticLibPath'] = '<% = GetStaticLibPath() %>';

	    });

	    var rteModal = '<%=mdlRTEditModal.ClientID %>';
	    var rteEditor = '<%=rtEditorCntl %>';
	    var hiddenCETabCntlID = '<%=hiddenCETab.ClientID %>';
	    var hiddenCETabIDCntlID = '<%=hiddenCETabID.ClientID %>';
	    var imageLibraryModalId = '<%= mdlImageLibrary.ClientID %>';

	    var btnPerformMerge = '<%=btnPerformMerge.ClientID %>';
	    var btnPerformMergeAndSave = '<%=btnPerformMergeAndSave.ClientID %>';
	    var btnCancelMerge = '<%=btnCancelMerge.ClientID %>';

	    function pageLoad() {
	        if (modmasterpopupid !== undefined && modmasterpopupid !== null && modmasterpopupid !== '') {
	            var popup = $f(modmasterpopupid);
	            if (popup !== null && popup !== undefined) {
	                popup.add_shown(function () {
	                    $('#hrefMasterPopup').focus();
	                    $('#divMasterPopup').click(function (e) {
	                        $('#hrefMasterPopup').focus();
	                        return false;
	                    });
	                });
	            }
	        }

	        SetupDateAndTimeFields();
	    };

	    var pnlLibraryPopup = '<%= pnlImageLibrary.ClientID %>';

	    function closeOpenModal() {
	        if ($g(pnlMasterPopupId).style.display == '') {
	            SEL.MasterPopup.HideMasterPopup();
	            return;
	        }

	        if ($g(pnlLibraryPopup).style.display == '') {
	            $f(imageLibraryModalId).hide();
	            return;
	        }
	    }

	    function pastePlainText(e) {
	        var text, sel, range;
	        e.returnValue = false;
	        e.preventDefault();
	        if (window !== undefined && window.clipboardData !== undefined) {
	            text = window.clipboardData.getData("Text");
	            text = getTextFromHTML(text);
	            if (text !== null && text !== undefined) {


	                if (navigator.appVersion.indexOf("MSIE 7.") != -1) {
	                    $('.ajax__html_editor_extender_texteditor').text(text);
	                } else {
	                    sel = window.getSelection();
	                    if (sel.getRangeAt && sel.rangeCount) {
	                        range = sel.getRangeAt(0);
	                        range.deleteContents();

	                        // Range.createContextualFragment() would be useful here but is
	                        // only relatively recently standardized and is not supported in
	                        // some browsers (IE9, for one)
	                        var el = document.createElement("div");
	                        el.innerHTML = text;
	                        var frag = document.createDocumentFragment(), node, lastNode;
	                        while ((node = el.firstChild)) {
	                            lastNode = frag.appendChild(node);
	                        }
	                        range.insertNode(frag);

	                        // Preserve the selection
	                        if (lastNode) {
	                            range = range.cloneRange();
	                            range.setStartAfter(lastNode);
	                            range.collapse(true);
	                            sel.removeAllRanges();
	                            sel.addRange(range);
	                        }
	                    }

	                }
	            } else {
	                pasteError();
	            }
	            return;
	        }


	        if (text === undefined) {
	            text = (e.originalEvent || e).clipboardData.getData('text/plain');
	            if (text !== null && text !== undefined && text !== "") {
	                text = getTextFromHTML(text);


	                document.execCommand('insertHtml', false, text);
	            } else {
	                pasteError();
	            }

	            return;

	        }
	    }

	    function pasteError() {
	        SEL.MasterPopup.ShowMasterPopup('No text found to paste.', 'Message from ' + moduleNameHTML);
	    }
	    function getTextFromHTML(text) {
	        //Set html of a new div to be "text", then get text from same div.
	        var newDiv = '<div style="display:none" id="dummyTextDiv"></div>';
	        var outputText = '';
	        $('body').append(newDiv);
	        var dummyDiv = $('#dummyTextDiv');
	        dummyDiv.html(text);

	        if (dummyDiv.children().length !== 0) {
	            dummyDiv.children().each(function () {
	                outputText += '<p>' + $(this).text() + '</p>';
	            });
	        } else {
	            outputText = text;
	        }

	        dummyDiv.remove();
	        outputText.replace(/\r/g, "");

	        if (outputText.indexOf("\n") > -1) {
	            outputText = outputText + "</p>";
	        }
	        outputText = outputText.replace(/\n/, "<p>");
	        outputText = outputText.replace(/\n/g, "</p><p>");

	        return outputText;
	    }
	    //override for cancelling and saving pages. Prevents script error when attempting to read editableDiv.firstChild
	    if (Sys.Extended && Sys.Extended.UI && Sys.Extended.UI.HtmlEditorExtenderBehavior && Sys.Extended.UI.HtmlEditorExtenderBehavior.prototype && Sys.Extended.UI.HtmlEditorExtenderBehavior.prototype._editableDiv_submit) {
	        Sys.Extended.UI.HtmlEditorExtenderBehavior.prototype._editableDiv_submit = function () {
	            var uagent = navigator.userAgent.toLowerCase();
	            var char = 3;
	            var sel = null;
	            if (Sys.Browser.agent != Sys.Browser.Firefox && uagent.match(/trident\/4.0/g) != true);
	            {
	                if (document.selection) {
	                    sel = document.selection.createRange();
	                    sel.moveStart('character', char);
	                    sel.select();
	                } else {
	                    sel = window.getSelection();
	                }
	            }
	            this._textbox._element.value = this._encodeHtml();
	        };
	    };
	    var txtProjectName = '<%= txtProjectName.ClientID %>';
	    var txtProjectDescription = '<%= txtProjectDescription.ClientID %>';
	    var divFields = '<%=divFields.ClientID %>';
	    var divDocGroupingConfigs = '<%=divDocGroupingConfigs.ClientID %>';

	    (function (configs) {
	        configs.txtConfigLabel = '<%= this.txtConfigLabel.ClientID %>';
	        configs.txtConfigDescription = '<%= this.txtConfigDescription.ClientID %>';
	        configs.tabMaintainConfig = '<% = this.tabMaintainConfig.ClientID %>';
	        configs.hdnCurrentConfigId = '<% = this.hdnCurrentConfigId.ClientID %>';
	        configs.chkIsDefaultDocumentGroupingConfig = '<% = this.chkIsDefaultDocumentGroupingConfig.ClientID %>';
	        configs.hdnDefaultDocumentGroupingId = '<% = this.hdnDefaultDocumentGroupingId.ClientID %>';
	        configs.hdnReportSortingName = '<% = this.hdnReportSortingName.ClientID %>';
	        configs.ddlReportSource = '<%=ddlReportSource.ClientID %>';
	    }(SEL.DocMerge.DomIDs.Config));
	    var entityId = '<%=ViewState["entityid"] %>';
	    var ceID = '<%=ViewState["ceID"] %>';
	    var currentProjectId = -1;

	</script>
<style>
    #maindiv {
    margin-left: 10px;
    margin-right: 10px;
}
    table tbody tr th {
    height: 34px;
    border: #ededed solid 1px !important;
    font: 12px Arial,sans-serif;
    color: #333;
    text-align: left;
    background-color: #f2f4f8;
    text-decoration: none;
   
}

</style>

	<div style="margin-left: 5px; margin-top: 5px;">
	    <asp:PlaceHolder ID="holderForm" runat="server"></asp:PlaceHolder>
        <asp:HiddenField runat="server" ID="hiddenCETab" /><asp:HiddenField runat="server" ID="hiddenCETabID" />
	</div>
	<asp:Panel ID="pnlWorkflowHolder" runat="server"></asp:Panel>
	
	<div class="formpanel" style="padding-left:5px;">
	<div class="formbuttons floatingFormButtons">
		<asp:Panel ID="pnlButtons" runat="server"></asp:Panel>
	</div>
	</div>
    <div style="display: none;" class="dialogHolder dialogHolder_doc_height">
        <div class="formpanel">
        <div class="sectiontitle">
            Document Configuration Definition
        </div>
        <div>
            <asp:Label runat="server" ID="lblDefMessage"></asp:Label>
        </div>
        <div class="twocolumn">
            <asp:Label runat="server" ID="lblProjectName" Text="Configuration name*" AssociatedControlID="txtProjectName"
                Enabled="false"></asp:Label>
            <span class="inputs">
                <asp:TextBox runat="server" ID="txtProjectName" TabIndex="1" ValidationGroup="project" MaxLength="150"
                    CssClass="fillspan" Enabled="False"></asp:TextBox>
            </span>
            <span class="inputicon"></span><span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"></span>
            <input id="hdnDefaultDocumentGroupingId" type="hidden" runat="server" value="false" />
            <input id="hdnReportSortingName" type="hidden" runat="server" value="false" />
            <input id="txtProjectDescription" type="hidden" runat="server" value="false" />
            <input id="txtBlanktext" type="hidden" runat="server" value="false" class="blankText"/>
        </div>
        </div>
        <div ID="tabDocGroupingConfigs" runat="server">
			<div class="formpanel">
			    <div class="sectiontitle">
                    Current Document Grouping Configurations
				</div>
                <span>
                    <a href="javascript:SEL.DocMerge.LaunchGroupingModal(0);">New Document Grouping Configuration</a>
                </span>
                <div id="divDocGroupingConfigs" runat="server" style="margin-top: 10px; height:auto;" class="docGroupingConfigs">
                    <asp:Literal runat="server" ID="ltGroupingConfigurations"></asp:Literal>
                </div>
            </div>
        </div>
     <div style="display: none; height:auto !important;" class="documentGroupingConfigurationDiv">
        <div class="sectiontitle documentGroupingConfigurationTitle" id="documentGroupingConfigurationTitle">Title</div>
        <cc1:TabContainer ID="tabMaintainConfig" runat="server" OnClientActiveTabChanged="SEL.DocMerge.tabMaintainConfigClick" CssClass="ajax__tab_xp formpanel">
            <cc1:TabPanel ID="configDetailsTab" runat="server">
                <HeaderTemplate>
                    General Details
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">
                       General Details
                    </div>                  
                        <div class="twocolumn">
                        <label id="lblConfiglabel" class="mandatory" for="txtConfigName">Configuration label*</label>
                            <span class="inputs">
                            <asp:TextBox runat="server" ID="txtConfigLabel" CssClass="fillspan" MaxLength="100" ValidationGroup="vgConfigLabel"></asp:TextBox>
                            </span>
                            <span class="inputvalidatorfield">
                            <asp:RequiredFieldValidator runat="server" ID="reqConfigLabel" ControlToValidate="txtConfigLabel"
                                ValidationGroup="vgConfigLabel" Text="*" ErrorMessage="Please enter a Configuration label in the box provided." SetFocusOnError="true"></asp:RequiredFieldValidator>
                            </span>
                            <span id="spanDefaultDocumentGroupingChk">
                            <asp:Label runat="server" ID="labelDefaultConfig" Text="Default grouping configuration" AssociatedControlID="chkIsDefaultDocumentGroupingConfig"></asp:Label>
                                 <span class="inputs">
                                <asp:CheckBox runat="server" ID="chkIsDefaultDocumentGroupingConfig" />
                                 </span>
                            </span>
                        <input type="hidden" runat="server" id="hdnCurrentConfigId" />
                        </div>
                    <div class="onecolumn">
                        <label id="lblConfigDescription" for="txtConfigDescription" >Description</label>
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
                    <div class="formpanel">
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
                    <div id="div1" runat="server">
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
            <helpers:CSSButton ID="cssMergeConfig" runat="server" Text="merge" OnClientClick="javascript:SEL.DocMerge.MergeCurrentConfig();return false;" UseSubmitBehavior="False" TabIndex="0" CssClass="mergeConfig"/>
            <helpers:CSSButton ID="cssSaveConfig" runat="server" Text="save" OnClientClick="javascript:SEL.DocMerge.SaveConfiguration(false);return false;" UseSubmitBehavior="False" TabIndex="1" />
            <helpers:CSSButton ID="cssCancelConfig" runat="server" Text="cancel" OnClientClick="javascript:SEL.DocMerge.CancelGroupingConfiguration();return false;" UseSubmitBehavior="False" TabIndex="2" />
            <div id="filteringButtons" style="display: inline; float: left; padding-right: 20px">
            <div id="ajaxLoaderConfig" style="width: 32px; display: none; position: relative; margin-left: 8px;">
                <img src="/shared/images/ajax-loader.gif" alt="" />
            </div>
            </div>
        </div>
    </div>
        <div class="formbuttons" id="mergeDialogButtons">
            <helpers:CSSButton ID="btnCancelMergeConfig" runat="server" Text="cancel" OnClientClick="javascript:$('.dialogHolder').dialog('close');return false;" UseSubmitBehavior="False" TabIndex="0" />
            <div id="mergingButtons" style="display: inline; float: left; padding-right: 20px">
            </div>
        </div>
    </div>
    
	<asp:Panel runat="server" ID="pnlRTEdit"  Style="display: none; position: fixed; height: 510px" CssClass="modalpanel">
	    <div class="formpanel">
	         
     <cc1:HtmlEditorExtender runat="server" TargetControlID="txtHTMLEditor" DisplaySourceTab="true"  ClientIDMode="Static" ID="editor" EnableSanitization="False"  OnImageUploadComplete="savefile" >
          <Toolbar>   
              <cc1:Undo />
                <cc1:Redo />
                <cc1:Bold />
                <cc1:Italic />
                <cc1:Underline />
                <cc1:StrikeThrough />
                <cc1:Subscript />
                <cc1:Superscript />
                <cc1:JustifyLeft />
                <cc1:JustifyCenter />
                <cc1:JustifyRight />
                <cc1:JustifyFull />
                <cc1:InsertOrderedList />
                <cc1:InsertUnorderedList />
                <cc1:CreateLink />
                <cc1:UnLink />
                <cc1:RemoveFormat />
                <cc1:SelectAll />
                <cc1:UnSelect />
                <cc1:Delete />
                <cc1:Cut />
                <cc1:Copy />
                <cc1:Paste />
                <cc1:BackgroundColorSelector />
                <cc1:ForeColorSelector />
                <cc1:FontNameSelector />
                <cc1:FontSizeSelector />
                <cc1:Indent />
                <cc1:Outdent />
                <cc1:InsertHorizontalRule />
                <cc1:HorizontalSeparator />
                <cc1:InsertImage />
                 <cc1:Cut />
                <cc1:Copy />
                <cc1:Paste />               
                </Toolbar>
         </cc1:HtmlEditorExtender>
         <asp:TextBox runat="server" Height="400px"  ClientIDMode="Static" Width="850px" ID="txtHTMLEditor"></asp:TextBox>
      	  <asp:PlaceHolder runat="server" ID="phRTEdit"></asp:PlaceHolder>
	    </div>
	    <div class="formpanel formbuttons" style="padding-top: 10px">
            <asp:ImageButton runat="server" ID="btnRTESave" AlternateText="save" ImageUrl="~/shared/images/buttons/btn_save.png" OnClientClick="javascript:SaveRichTextEditor(); return false;" />
            &nbsp;&nbsp;
            <asp:ImageButton runat="server" ID="btnCancelRTEdit" AlternateText="cancel" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="false" />
        </div>
	</asp:Panel>
	
	<cc1:ModalPopupExtender runat="server" TargetControlID="lnkLaunchModal" PopupControlID="pnlRTEdit"
		BackgroundCssClass="modalBackground" ID="mdlRTEditModal" Y="100" CancelControlID="btnCancelRTEdit" DynamicServicePath="">
	</cc1:ModalPopupExtender>
	<asp:LinkButton runat="server" ID="lnkLaunchModal" Style="display: none;">&nbsp;</asp:LinkButton>
    <cc1:ModalPopupExtender runat="server" TargetControlID="lnkLaunchModal" PopupControlID="pnlSaveErrorMsg"
		BackgroundCssClass="modalBackground" ID="mdlSaveErrorModal" 
		CancelControlID="btnCloseErrorMsg" OkControlID="btnCloseErrorMsg">
	</cc1:ModalPopupExtender>
    <asp:Panel runat="server" ID="pnlSaveErrorMsg" CssClass="errorModal" style="display:none;z-index:16000099;">
        <div id="divMasterPopup">
        <div id = "divErrorTitle" runat="server" class="errorModalSubject">Record Save Message</div><br /><div id="divErrorText" class="errorModalBody" runat="server">Cannot save record as one of the field values already exists in another record.</div>
        </div>
    <div style="padding: 0px 5px 5px 5px;"><asp:ImageButton runat="server" ID="btnCloseErrorMsg" AlternateText="Close" ImageUrl="~/shared/images/buttons/btn_close.png"></asp:ImageButton></div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlImageLibrary"  Style="display: none; position: fixed; height: 460px" CssClass="modalpanel">
	    <div class="formpanel">
            <div class="sectiontitle">Image Library</div>
            <div class="modalcontentssmall">
	            <div class="onecolumnpanel">Use this editor to pick an attachment from the image library, or browse for one from your own file system.</div>
	            <div class="twocolumn">
	                <div id="iconSearchArea">
		                <div id="iconResultsHeader">
			                <span id="selectedIconSpan"></span>
			                <span id="selectedIconInfo"><span style="font-weight: bold">Selected icon</span><span id="selectedIconName">window_dialog.png</span></span>
			                <span id="iconSearchBox"><span id="iconSearchRemoveButton" class="ui-icon-close searchButton" title="Clear search options"></span><span id="iconSearchButton" class="ui-icon-search searchButton" title="Search"></span><asp:TextBox ID="txtViewCustomIconSearch" runat="server" MaxLength="21" CssClass="searchBox"></asp:TextBox></span>
		                </div>
		                <div id="viewCustomIconContainer">
			                <span id="iconResultsLeft">&lt;</span>
			                <span id="viewIconResults"></span>
			                <span id="iconResultsRight">&gt;</span>
		                </div>
		                <div id="selectedIconContainer"></div>
	                </div>
                </div>
	        </div>
        </div>
        <div class="formpanel formbuttons formpanel_padding" style="padding-top: 10px">
            <span class="buttonContainer" id="pluginBrowseButton" runat="server">
                <input id="btnImageLibraryBrowse" type="button" runat="server" value="browse" class="buttonInner"/>            
            </span>
            <span class="buttonContainer">
                <input id="btnImageLibraryOk" type="button" runat="server" value="  save  " class="buttonInner"/>
            </span>
            <span class="buttonContainer">
                <input id="btnImageLibraryCancel" type="button" runat="server" value=" cancel " class="buttonInner" />
            </span>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ClientIDMode="Static" TargetControlID="lnkLaunchModal" PopupControlID="pnlImageLibrary"
		BackgroundCssClass="modalBackground" ID="mdlImageLibrary" 
		CancelControlID="" OkControlID="">
	</cc1:ModalPopupExtender>

    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
<div id="ajaxLoaderMaster" style="width: 32px; display: none; position: relative; margin-left: 8px;"><img src="/shared/images/ajax-loader.gif" alt="" /></div> 
    <div id="mergeDialog" style="display: none" >
        <div class="sectiontitle">Perform Document Merge</div>
        <div>
            <div id="tblReportStatus" style="height: 100px;">
                <div>
                    <div id="loadingMessage" align="left" class="onecolumnpanel inputs">
                        Perform a document merge by clicking the "Perform Merge" button, if you want to save a PDF copy of the document (which can be published later) click the "Perform Merge and add to Torch History" button.
                    </div>
                    <div valign="middle" align="center" style="display: none" id="reportStatus">
                        <div >
                            Processing Request
                        </div>
                        <div id="reportProgress" style="width: 250px; background-image: url('../shared/images/exportReportBackground250px.png'); height: 20px;" >
                            <div id="reportDone" style="background-image: url('../shared/images/exportReport250px.png'); height: 20px; text-align: left; float: left;">
                                &nbsp;
                            </div>
                        </div>
                        <div id="reportPercentDone">
                            0%
                        </div>
                    </div>
                </div>
                <div>
                    <div style="vertical-align: central" align="center">
                        <span id="divMergeProgress">
                            <asp:Literal ID="litMergeProgress" runat="server"></asp:Literal>
                        </span>
                    </div>
                </div>
                <div id="documentHyperLinkRow" runat="server" style="display: none;">
                    <div valign="middle" align="center">
                        <span id="divDocumentUrl">
                            <asp:HyperLink ID="hyperLinkDocumentUrl" runat="server"></asp:HyperLink>
                        </span>
                    </div>
                </div>
            </div>
            
            <div >
                <span id="performMergeButtons">
                          <helpers:CSSButton ID="btnPerformMerge" runat="server" Text="Perform Merge"  UseSubmitBehavior="False" TabIndex="0" OnClientClick="return false;"/>                
                          <helpers:CSSButton ID="btnPerformMergeAndSave" runat="server" Text="Perform Merge and add to Torch History"  UseSubmitBehavior="False" TabIndex="0" OnClientClick="return false;"/>                
                          <helpers:CSSButton ID="btnCancelMerge" runat="server" Text="cancel"  UseSubmitBehavior="False" TabIndex="0" OnClientClick="return false;"/>                
                      </span>
            </div>
        </div>
        
    </div>
</asp:content>
<asp:Content runat="server" ID="customScripts" ContentPlaceHolderID="scripts">
    <script type="text/javascript">
    $(document).ready(function () {
        if ($(".submenuholder").find("#submenu").length > 0) {
            var width = $(window).width(), height = $(window).height();
            if ((width <= 1024) && (height <= 768)) {
                $('#maindiv').css('margin-left', '180px');
                $('body').css('display', 'inline-block');
            } else {
                $('#maindiv').css('margin-left', '205px');
            }
        }
    });</script>
</asp:Content>
