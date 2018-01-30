<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="aeReport.aspx.cs" Inherits="Spend_Management.shared.reports.aeReport" %>

<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.filterDialog.js?date=20180126"/>
            <asp:ScriptReference Path="~/shared/javaScript/sel.reports.js?date=201801241614" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js"/>
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

     <style>
         @charset "utf-8";
/* CSS Document  - To be removed when the new style is merged down.*/


.sectionHeader {
    background:#ececec;
	padding:10px;
    text-indent: 10px;
    font-weight: bold
    }

/*ej*/
th.e-headercell {
    color: #282827!important;
}
.ejsel-grid-header-menu-button, .ejsel-grid-header-edit-button, ejsel-grid-header-icon {
    padding-right: 7px;
}

.ejsel-grid-header-icon {
    width: 16px;
    height: 16px;
    display: inline-block;
    margin: 0 6px -2px 0;
}

.ejsel-waitingpopup {
    background-color: #BBB;
}

th.e-headercell div.e-headercelldiv {
    text-align: left !important;
}

.pagePadding {
    padding-right: 20px;
}

.tabs {
      min-height: 200px;
      float: left;
      width: 100px;
  }

#menuOwner {
    height: 555px;
}
.searching {
    -ms-opacity: .3;
    opacity: .3;
}

ul{
	padding:0px;
	margin:0px;
	}
.columnName {
    width:155px;
}
.closeArrow {
    background-image: url(/static/icons/16/new-icons/navigate_close.png);
    background-repeat: no-repeat;
    background-position: 65px 10px;
    background-size: 16px 16px;
    cursor:pointer;
}

.openArrow {
    background-image: url(/static/icons/16/new-icons/navigate_open.png);
    background-repeat: no-repeat;
    background-position: 65px 12px;
    background-size: 16px 16px;
    cursor:pointer;
}

.e-grid .e-headercelldiv{
    margin:-12px;
}

.selectGroup{
   cursor:pointer;
}
.SelectedType {
    background-color: lightgray;
}


#imgChartPreview {
    border: 1px solid; 
    border-color: <%=BorderColour%>;     
    border-image-outset:3px;
    position:absolute;
    top:95px;
    left:606px;
} 

#imgChart {
    position: absolute;
    top: 10px;
    left: 100px;
    border: 1px solid; 
    border-color: <%=BorderColour%>;     
    border-image-outset:3px;
}

.group-first {
    border-top:2px solid  <%=BorderColour%>  !important;
}

.group-last {
    border-bottom:2px solid  <%=BorderColour%>   !important;
}
td input[type="checkbox"] {
          float: left;
          margin: 0 auto;
          width: auto;
    }
.cholder {
background: rgb(199, 223, 247) none repeat scroll 0% 0%;
    border-left: 1px solid <%=BorderColour%> !important;	
}


#_st_clone_ .easytree-title{
    font-size: 20px;
    padding: 10px 20px 20px 20px;
    border: 1px solid #c8c8c8;
    color: <%=TextColour%>;
    background-color: transparent;
    background: <%=BorderColour%>;
}

.e-rowcell.easytree-droppable.easytree-accept.column-drop-border {
    border-left: 3px solid <%=BorderColour%>!important;
    padding-left: 0.5em;
}

.e-rowcell.easytree-droppable.easytree-accept.column-drop-border-right {
    border-right: 3px solid <%=BorderColour%>!important;
    padding-right: 0.47em;
}

.e-grid .e-gridheader .e-headercell.column-drop-border {
    border-left: 3px solid <%=BorderColour%>!important;
    padding-left: 0.35em;
}


.e-grid .e-gridheader .e-headercell.column-drop-border-right {
    border-right: 3px solid <%=BorderColour%>!important;
    padding-right: 0.5em;
}

.e-grid .e-reorderindicate {
  border-right:3px solid <%=BorderColour%>!important;
}

.e-grid .e-gridheader {
    border-bottom-color:  <%=BorderColour%>!important;
}

.e-grid .e-rowcell:empty, .e-rowcell.easytree-droppable.easytree-accept {
  height: 32px !important;
}

#Dropper .e-grid .e-rowcell:empty {
    height: 20px !important;
}
#Dropper .e-grid .filterInfo td {
       border-width: 1px;
}
#Dropper .e-grid .filterInfo td:nth-child(3) {
       border-width: 1px 0 1px 1px;
}
#Dropper .e-grid .filterInfo td:nth-child(4) {
    border-top: 1px solid #c8c8c8;
       border-width: 1px 1px 1px 0;
}

.e-grid .e-headercell, .e-grid .e-grouptopleftcell {
        background: #fcfcfc!important;
}

.e-grid .e-headercell.e-headercellactive, .e-dragclone ,.e-grid .e-groupdroparea {
     background: <%=BorderColour%>!important;
    color: <%=TextColour%>!important;
}

.filterInfo.ui-sortable-placeholder {
    height: 50px;
}

.e-grid .e-groupdroparea.e-hover {
  background: #f6f6f6!important;
    color: #333333 !important;
}

     </style>

    <script type="text/javascript" language="javascript">
        $(function () {
            $('#maindiv').css('margin-left', '20px');
            $('#Tabs').css('width', '100%');
            $('#Tabs').tabs({
                selected: <%=this.DefaultTab%>,
                beforeActivate: function (event, ui) {
                    var active = $("#Tabs").tabs("option", "active"); // Currently active before the change
                    if (active === 1) {
                        if ($('#divFilter').dialog("isOpen")) {
                            return false;
                        }
                        $('.state-disabled').addClass('iconDisabled');

                        if (SEL.Reports.Preview.GridLoadingMask) {
                            SEL.Reports.Preview.GridLoadingMask.hide();
                        }
                    } else {
                        $('.state-disabled').removeClass('iconDisabled');
                    }

                    return true;
                },
                activate: function (event, ui) {
                    if (ui.newPanel[0].id === "pgChart" && SEL.Reports.Columns.SelectedTreeNodes.length) {
                        SEL.Reports.Preview.Refresh(false);
                    }
                }
            });
            
            var filterModal = $.filterModal({ domainRoot: SEL.Reports });
            $('#criteriaContainer').removeClass("openArrow").addClass("closeArrow");
            SEL.Reports.SetupAvailableFields();

        });

        (function(general) {
            general.ReportName = '<%= txtreport.ClientID %>';
            general.Description = '<%= txtdescription.ClientID %>';
            general.Category = '<%= cmbfolders.ClientID %>';
            general.ReportOn = '<%= cmbreporton.ClientID %>';
            general.Claimant = '<%= chkclaimant.ClientID %>';
            general.LimitReport = '<%= txtLimitReport.ClientID %>';
            general.LimitReportValidator = '<%= reqLimitResults.ClientID %>';
            general.FilterInput = '<%=filterInput.ClientID%>';
            general.CriteriaDetail = '<%=divCriteriaValue.ClientID%>';
            general.ItemDetail = '<%=divItemvalue.ClientID%>';
            general.CurrencySymbol = '<%=divCurrencySymbol.ClientID%>';
            general.CalculatedSaveButton = '<%=btnSaveCalc.ClientID%>';
        }(SEL.Reports.IDs.General));

        (function(chartGeneral) {
            chartGeneral.imgAreaChart = '<%= imgAreaChart.ClientID %>';
            chartGeneral.imgBarChart = '<%= imgBarChart.ClientID %>';
            chartGeneral.imgColumnChart = '<%= imgColumnChart.ClientID %>';
            chartGeneral.imgDonutChart = '<%= imgDonutChart.ClientID %>';
            chartGeneral.imgDotChart = '<%= imgDotChart.ClientID %>';
            chartGeneral.imgLineChart = '<%= imgLineChart.ClientID %>';
            chartGeneral.imgPieChart = '<%= imgPieChart.ClientID %>';
            chartGeneral.imgFunnelChart = '<%= imgFunnelChart.ClientID %>';
            chartGeneral.txtChartTitle = '<%= txtChartTitle.ClientID %>';
            chartGeneral.chkShowLegend = '<%= chkShowLegend.ClientID %>';
            chartGeneral.ddlXAxis = '<%= ddlXAxis.ClientID %>';
            chartGeneral.ddlYAxis = '<%= ddlYAxis.ClientID %>';
            chartGeneral.ddlGroupBy = '<%= ddlGroupBy.ClientID %>';

            chartGeneral.ddlChartTitleFont = '<%= ddlChartTitleFont.ClientID %>';
            chartGeneral.txtChartTitleColour = '<%= txtChartTitleColour.ClientID %>';
            chartGeneral.ddlTextFont = '<%= ddlTextFont.ClientID %>';
            chartGeneral.txtTextFontColour = '<%= txtTextFontColour.ClientID %>';
            chartGeneral.txtTextBackgroundColour = '<%= txtTextBackgroundColour.ClientID %>';
            chartGeneral.chkShowLabels = '<%= chkShowLabels.ClientID %>';
            chartGeneral.chkShowValues = '<%= chkShowValues.ClientID %>';
            chartGeneral.chkShowPercent = '<%= chkShowPercent.ClientID %>';
            chartGeneral.ddlChartSize = '<%= ddlChartSize.ClientID %>';
            chartGeneral.ddlLegendPosition = '<%= ddlLegendPosition.ClientID %>';
            chartGeneral.ddlCombineOthers = '<%= ddlCombineOthers.ClientID %>';
            chartGeneral.ddlShowChart = '<%= ddlShowChart.ClientID %>';
        }(SEL.Reports.IDs.Charts.General));

        $(document).ready(function () {
            if (!Array.prototype.indexOf) {
                Array.prototype.indexOf = function (obj, start) {
                    for (var i = (start || 0), j = this.length; i < j; i++) {
                        if (this[i] === obj) { return i; }
                    }
                    return -1;
                }
            }
            SEL.Common.SetTextAreaMaxLength();
            SEL.Reports.SetupEnterKeyBindings();
            SEL.Reports.SetupMouseAndResizeEvents();
            
            $('#divFormulaButtons button').button();
            $('.formbuttons button').button();
            $('#imgFilter').css('cursor', 'pointer');
            $('#imgCalc').css('cursor', 'pointer');
            $('#imgStatic').css('cursor', 'pointer');
            SEL.Reports.Criteria.SetupSortable();
            SEL.Reports.Criteria.Collapse();
            $('#pgChart').find('select').change(function(){SEL.Reports.Preview.Refresh(false);});
            $('#pgChart').find('input').change(function(){SEL.Reports.Preview.Refresh(false);});
            $('#imgChartPreview')
                .mouseenter(function() {
                    $('#imgChart').css('display', '');
                });
            $('#imgChart').mouseleave(function() {
                $('#imgChart').css('display', 'none');
            });
            $('.filterHeader').show();
            $('#criteriaList').hide();
        });
        function ColourChange(object) {
            SEL.Reports.Preview.Refresh(false);
        }
    </script>
    
    <div id="divPages" class="pagePadding">
        <div id="Tabs" class="tabs">
            <ul>
                <li><a href="#pgGeneral" style="padding-top: 16px;">General Details</a></li>
                <li><a href="#pgPreview" style="padding-top: 16px;">Columns</a></li>
                <li><a href="#pgOptions" style="padding-top: 16px;">Options</a></li>
                <li><a href="#pgChart" style="padding-top: 16px;">Chart</a></li>
           </ul>
 
           <div id="pgGeneral" class="ui-widget ui-widget-content">
               <div class="sm_panel">
                    <div class="sectiontitle">General Details</div>
                    <div class="twocolumn">
                        <asp:Label CssClass="mandatory" ID="lblreportnamelbl" runat="server" Text="Report name*" AssociatedControlID="txtreport"></asp:Label>
                        <span class="inputs"><asp:TextBox ID="txtreport" runat="server" MaxLength="150"></asp:TextBox></span>
                        <span class="inputicon">&nbsp;</span>
                        <span class="inputtooltipfield">
                            <asp:Image ID="imgTooltipReportName" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('9ec4beac-c7e3-4dbb-981a-a5e38517087d', 'sm', this);" /></span>
                        <span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqreport" runat="server" ControlToValidate="txtreport" ErrorMessage="Please enter a Report name." ValidationGroup="vgMain">*</asp:RequiredFieldValidator></span>
                    </div>
                    <div class="onecolumn"><asp:Label ID="lbldescription" runat="server" Text="Description" AssociatedControlID="txtdescription"></asp:Label><span class="inputs"><asp:TextBox ID="txtdescription" runat="server" TextMode="MultiLine" MaxLength="2000"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="imgTooltipReportDescription" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('93664cdc-680a-498a-aec1-fa4e24374d6c', 'sm', this);" /></span></div>
                    <div class="twocolumn"><asp:Label ID="lblcategory" runat="server" Text="Report category" AssociatedControlID="cmbfolders"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbfolders" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="imgTooltipReportCategory" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('cd662977-b68c-4d36-b26a-134adcd8addd', 'sm', this);" /></span></div>
                    <div class="twocolumn"><asp:Label ID="lblreporton" runat="server" Text="What would you like to report on" AssociatedControlID="cmbreporton"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbreporton" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="imgTooltipReportOn" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('71d326cf-bc0b-48df-9948-12c0528d8fa0', 'sm', this);" /></span></div>
                    
                </div>
            </div>

            <div id="pgPreview" class="ui-widget ui-widget-content">
                
                <div class="treeview_left_main" style="position: relative;">
                    <div class="treeview_top">

                            <h4 id="availablefieldstitle" style="display: inline; float: none;">Available Fields </h4>
                            <div class="treeview_top_button" style="border-left: none;">
                                <img width="19" height="19" alt="Calculated column" title="Calculated Column" src="/static/icons/16/new-icons/calculator.png" id="imgCalc" /></div>
                            <div class="treeview_top_button">
                                <img width="19" height="19" style="margin-right: 5px;" alt="Static column" title="Static Column" src="/static/icons/16/new-icons/text.png" id="imgStatic" /></div>
     
                            
                            <input type="text" id="txtsearch" maxlength="20" />
                    </div>
                    <div id="spacer">
                        <div id="menuOwner">
                            <ul id="menu" class="fieldList" style="padding-left: 0;"></ul>
                        </div>
                    </div>
                
                    <div id="treeviewLoading" style="position: absolute; left: 0; top: 0; width: 100%; height: 100%; background-color: #aaa; opacity: .5;">
                        <div style="width: 100px; margin: auto; text-align: center; margin-top: 200px; font-size: 28px;color:rgb(236, 236, 236);">Loading</div>
                    </div>

                </div>

                <div class="right_main">
                    <div id="criteriaContainer" class="sectionHeader">
                        <h4 style="display: inline;" > Filters</h4>
                        </div>
                    <div class="comment" id="divScheduleCriteriaComment" style="display:none;margin-top:5px;">Filters cannot be modified for this report as it has been scheduled.</div>
                    <div class="dropper easytree-droppable" id="Dropper" style="padding: 10px 0 10px 10px; overflow: hidden;">
                        <div class="comment" id="divRuntimeCriteriaComment" style="display:none;margin-top:5px;">One or more of your selected report filters will be decided when running the report. These filters will only be applied when running the full report and will not be reflected in the report preview.</div>
                        
                        <div class="filterHeader dropTarget"><span>Drag a field here to filter the report data.</span></div>
                        <table class="e-grid e-table e-headercontent"id="criteriaList" style="width:100%;">
                            
                        <tr id="criteriaListHeader" class="e-columnheader e-gridheader ">
                            <td class="e-headercell" style="width:20px;"><div class="e-headercelldiv"></div></td><td class="e-headercell" style="width:20px;"><div class="e-headercelldiv"></div></td><td class="groupIconHeader e-headercell" style="width:20px;border-right-style:none;" ><div class="e-headercelldiv"><img alt="" src="/shared/images/icons/group_disabled.png" style="margin-top:5px;" class="groupIcon" onclick="" title="Group two or more selected filters"/></div></td><td class="e-headercell" style="width:20px;border-left-style:none;"><div class="e-headercelldiv"></div></td><td class="e-headercell" style="width:45px;"><div class="e-headercelldiv">And/Or</div></td><td class="e-headercell"><div class="e-headercelldiv">Column</div></td><td class="e-headercell"><div class="e-headercelldiv">Filter Criteria</div></td><td class="e-headercell"><div class="e-headercelldiv">Value</div></td>
                            </tr>
                            <tfoot>
                                
                            </tfoot>
                            </table>
                    </div>
                   
                    <div runat="server" ID="divFilter" clientidmode="Static" class="sm_panel" style="display: none"></div>

                    <div class="sectionHeader">Preview
                         <span id="previewLoadingText" style="display: none;"><img alt="loading" src="../images/ajax-loader.gif" style="margin-right: 8px" height ="12"/>Loading </span>
                    </div>
                   <div class="comment" id="divSchedulePreviewComment" style="display:none;margin-top:5px;">Columns cannot be modified for this report as it has been scheduled.</div>
                    <div id="previewWindow">
                        <div id="previewInstructions" class="easytree-droppable easytree-accept dropTarget dropTargetGridFont" style="height: 416px; "><div style="padding-top: 158px;">Drag a field here to add a report column.</div></div>
                        
                        <div id="preview"></div>
                        
                        <ul id="previewColumnHeaderMenu" style="display: none;">
                            <li class="preview-menu-item-count"><a href="#"><span class="ui-icon"></span><span class="selectable">&nbsp;Count</span></a></li>
                            <li class="preview-menu-item-avg"><a href="#"><span class="ui-icon"></span><span class="selectable">&nbsp;Average</span></a></li>
                            <li class="preview-menu-item-sum"><a href="#"><span class="ui-icon"></span><span class="selectable">&nbsp;Sum</span></a></li>
                            <li class="preview-menu-item-max"><a href="#"><span class="ui-icon"></span><span class="selectable">&nbsp;Maximum</span></a></li>
                            <li class="preview-menu-item-min"><a href="#"><span class="ui-icon"></span><span class="selectable">&nbsp;Minimum</span></a></li>
                            <li class="preview-menu-item-hide"><a href="#"><span class="ui-icon"></span><span class="selectable">&nbsp;Hide</span></a></li>
                            <li class="preview-menu-item-delete"><a href="#"><span class="ui-icon ui-icon-delete"></span>&nbsp;Delete</a></li>
                        </ul> 
                    </div>

                </div>
            
                <div id="divCalculation" class="sm_panel" style="display: none;">
                    <div>
                        <asp:Label id="lblcolumn" style="width:100px" runat="server" AssociatedControlID="txtcolumnname">Column Name</asp:Label>
                        <span class="inputs"><asp:TextBox ID="txtcolumnname" MaxLength="50" runat="server" meta:resourcekey="txtcolumnnameResource1" CssClass="columnName"></asp:TextBox></span>
                    </div>
                    <div id="staticRuntime">
                        <label for="chkStaticRuntime" id="lblStaticRuntime">I'll decide when I run the report</label>
                        <span class="inputs"><input id="chkStaticRuntime" name="chkStaticRuntime" onclick="$.filterModal.Filters.FilterModal.ChangeFilterCriteria(false, true);" type="checkbox"></span>
                        </div>
                    <div style="width: 250px; float:left; padding-right:2px;margin-top: 6px;  border: solid 1px rgb(169,169,169); overflow: hidden; overflow-x: no-display; height:270px;" id="divMenuHolder">
                        <div  id="menuHolder" style="border: none; height:265px;" >
                        <ul id="functionMenu" style="display: none;">
                            <li class="ui-menu-item">Available Items<ul id="availableItemsMenu"></ul></li>
                            <li class="ui-menu-item">Math<ul id="MathMenu"></ul></li>
                            <li class="ui-menu-item">Date & Time<ul id="DateAndTimeMenu"></ul></li>
                            <li class="ui-menu-item">Logical<ul id="LogicalMenu"></ul></li>
                            <li class="ui-menu-item">Statistical<ul id="StatisticalMenu"></ul></li>
                            <li class="ui-menu-item">Text & Data<ul id="TextAndDataMenu"></ul></li>
                            <li class="ui-menu-item">Financial<ul id="FinancialMenu"></ul></li>
                            <li class="ui-menu-item">Information<ul id="InformationMenu"></ul></li>
                            <li class="ui-menu-item">Lookup & reference<ul id="LookupAndReferenceMenu"></ul></li>
                        </ul>
                        </div>
                    </div>
                    <div>
                        <img id="formulaOk" src="/static/icons/48/new/check.png" alt="Ok" style="display: run-in; position: absolute; left: 722px; top: 251px;" class="iconDisabled"/>
                        <img id="formulaError" src="/static/icons/48/new/delete.png" alt="Error in formula" style="display: run-in; position: absolute; left: 722px; top: 251px;" class="iconDisabled"/>
                    </div>
                    <div style="margin-top:6px; width:555px;" id="divFormula">
                        <div style="height: 271px; border: solid 1px rgb(169,169,169);">
                            <div id="txtFormula" style="width: 550px; height: 271px; padding-left: 3px; margin-top: 0; overflow:auto; overflow-x:hidden;" contenteditable="true"></div>
                        </div>
                        
                    </div>
                    <div class="formpanel formbuttons calculatedButtons" style="position:relative;left:-19px;height:30px;z-index:0;float:left;">
                        <helpers:CSSButton id="btnSaveCalc" runat="server" text="save" onclientclick="SEL.Reports.Calculated.Save();return false; " UseSubmitBehavior="False"/>
                        <helpers:CSSButton id="btnCancelCalc" runat="server" text="cancel" onclientclick="SEL.Reports.Calculated.Cancel();return false; " UseSubmitBehavior="False"/>
                        <helpers:CSSButton id="btnClearCalc" runat="server" text="clear" onclientclick="SEL.Reports.Calculated.Functions.ClearText();return false; " UseSubmitBehavior="False" Visible="False"/>
                    </div>
                </div>
            
            </div>
            
            <div id="pgOptions" class="ui-widget ui-widget-content">
               <div class="sm_panel">
                    <div class="sectiontitle">Data Options</div>
                    <div class="twocolumn"><asp:Label ID="lblclaimantdate" runat="server" Text="Can users view this report for their own data?" AssociatedControlID="chkclaimant"></asp:Label><span class="inputs"><asp:CheckBox ID="chkclaimant" runat="server" /></span></div>
                    <div class="twocolumn"><asp:Label ID="lblLimitReport" runat="server" Text="Limit results" AssociatedControlID="chkLimitReport" CssClass="chkLimitReport"></asp:Label><span class="inputs"><asp:CheckBox ID="chkLimitReport" runat="server" /></span><asp:Label ID="lbRowLimit" runat="server" Text="Maximum number of rows to return when running or exporting the report*" AssociatedControlID="txtLimitReport" CssClass="mandatory limitReport"></asp:Label><span class="inputs limitReport"><asp:TextBox ID="txtLimitReport" runat="server" CssClass="limitReport"/><cc1:FilteredTextBoxExtender ID="ftbLimitReport" runat="server" TargetControlID="txtLimitReport" FilterMode="ValidChars" ValidChars="0123456789" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cvLimitReport" Type="Integer" ErrorMessage="Please enter a maximum number of rows less than 10,000." Text="*" ControlToValidate="txtLimitReport" Operator="LessThanEqual" ValueToCompare="10000" ValidationGroup="vgLimit" Display="Dynamic" /><asp:CompareValidator runat="server" ID="cvLimitReport2" Type="Integer" ErrorMessage="Please enter a maximum number of rows greater than zero." Text="*" ControlToValidate="txtLimitReport" Operator="GreaterThan" ValueToCompare="0" ValidationGroup="vgLimit" Display="Dynamic" /><asp:RequiredFieldValidator runat="server" ID="reqLimitResults" ControlToValidate="txtLimitReport" ErrorMessage="Please enter a maximum number of rows to return." ValidationGroup="vgLimit">*</asp:RequiredFieldValidator></span>
                    </div>
                </div>
            </div>

             <div id="pgChart" style="display: none">
                <div class="sm_panel" style="width: 882px;">
                    <div class="sectiontitle">Report Chart</div>
                    
                    <div class="onecolumn">
                        <asp:Label ID="lblType" runat="server" Text="Chart type" AssociatedControlID="txtChartTitle"></asp:Label>
                        <span >
                            <asp:Image runat="server" ID="imgAreaChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/area.png" ChartType="0"/>
                            <asp:Image runat="server" ID="imgBarChart"  CssClass="SelectedType" ImageUrl="/static/icons/Custom/48/bar.png" ChartType="1"/>
                            <asp:Image runat="server" ID="imgColumnChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/column.png" ChartType="2"/>
                            <asp:Image runat="server" ID="imgDonutChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/doughnut.png" ChartType="3"/>
                            <asp:Image runat="server" ID="imgDotChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/point.png" ChartType="4"/>
                            <asp:Image runat="server" ID="imgLineChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/line.png" ChartType="5"/>
                            <asp:Image runat="server" ID="imgPieChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/pie.png" ChartType="6"/>
                            <asp:Image runat="server" ID="imgFunnelChart"  CssClass="Type" ImageUrl="/static/icons/Custom/48/funnel.png" ChartType="7"/>
                        </span>
                        <span class="inputtooltipfield" style="margin-top: 10px">
                            <asp:Image ID="imgTooltipChartType" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('E61A5F9E-BC4E-4CF9-B985-1A2836ACFC68', 'sm', this);" /></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblChartTitle" runat="server" Text="Chart title" AssociatedControlID="txtChartTitle"></asp:Label>
                        <span class="inputs"><asp:TextBox ID="txtChartTitle" runat="server"  ></asp:TextBox></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <div id="chartPreview" style="display:none;"><img id="imgChartPreview" src="//:0"  alt="Preview" width="300"; height="240"/></div>
                        <img id="imgChart" src="//:0"  alt="Preview" style="display: none;" />
                    </div>
                    <div class="twocolumn" >
                        <asp:Label ID="lblShowLegend" runat="server" Text="Show legend" AssociatedControlID="chkShowLegend"></asp:Label>
                        <span class="inputs"><asp:CheckBox ID="chkShowLegend" runat="server" CssClass="showLegend"></asp:CheckBox></span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblLegendPosition" runat="server" Text="Legend position" AssociatedControlID="ddlLegendPosition" CssClass="legendPosition"></asp:Label>
                        <span class="inputs legendPosition">
                            <asp:DropDownList runat="server" ID="ddlLegendPosition" CssClass="legendPosition"/>
                        </span>
                        <span class="inputtooltipfield "></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblShowChart" runat="server" Text="Show" AssociatedControlID="ddlShowChart"></asp:Label>
                        <span class="inputs ">
                            <asp:DropDownList runat="server" ID="ddlShowChart"/>
                        </span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="sectiontitle">Chart Data</div>
                    <div class="twocolumn">
                        <asp:Label ID="lblXAxis" runat="server" Text="X-axis" AssociatedControlID="ddlXAxis" CssClass="xaxislabel"></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlXAxis" runat="server"></asp:DropDownList></span>
                        <span class="inputtooltipfield"><asp:Image ID="Image2" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon " onmouseover="SEL.Tooltip.Show('0DE5A6DA-F2F4-4B99-A6D3-2F3A673EF96C', 'sm', this);"/></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblYAxis" runat="server" Text="Y-axis" AssociatedControlID="ddlYAxis" CssClass="yaxislabel"></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlYAxis" runat="server" ></asp:DropDownList></span>
                        <span class="inputtooltipfield"><asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('74344EBD-A887-4DE4-B7D8-18914B927D0C', 'sm', this);"/></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn groupby" >
                        <asp:Label ID="lblChartGroupBy" runat="server" Text="Group by" AssociatedControlID="ddlGroupBy" CssClass="groupbylabel"></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlGroupBy" runat="server"></asp:DropDownList></span>
                        <span class="inputtooltipfield"><asp:Image ID="imgTooltipLegendPosition" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon groupby" onmouseover="SEL.Tooltip.Show('C68D1CCC-0645-4AF4-81B6-7ACD1DBE98EA', 'sm', this);"/></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn combine">
                        <asp:Label ID="lblCombineOthers" runat="server" Text="Collected threshold percent" AssociatedControlID="ddlCombineOthers" ></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlCombineOthers" runat="server"></asp:DropDownList></span>
                        <span class="inputtooltipfield"><asp:Image ID="imgTooltipShowLegend" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon combine" onmouseover="SEL.Tooltip.Show('5DDB2E2B-65B6-4ADF-971C-EEB9CF840502', 'sm', this);" /></span>
                        <span class="inputvalidatorfield"></span>
                        <span id="spanOthers">
                        </span>
                    </div>
                    <div class="sectiontitle">Formatting</div>        
                    <div class="twocolumn">
                        <asp:Label ID="lblChartTitleSize" runat="server" Text="Chart title font" AssociatedControlID="ddlChartTitleFont"></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlChartTitleFont" runat="server"></asp:DropDownList>
                        </span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <span class="inputs"><asp:TextBox runat="server" ID="txtChartTitleColour" ></asp:TextBox></span>
                        <asp:Label ID="pnlChartTitleColour" runat="server" style="width: 18px; height: 18px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>
                        <span class="inputicon">
                            <cc1:ColorPickerExtender ID="colmenubg" runat="server" TargetControlID="txtChartTitleColour" PopupButtonID="imgmenubg" SampleControlID="pnlChartTitleColour" OnClientColorSelectionChanged="ColourChange"></cc1:ColorPickerExtender>
                            <asp:ImageButton ID="imgmenubg" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />
                        </span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="validTitleColour" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Chart Title Font colour." ControlToValidate="txtChartTitleColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label1" runat="server" Text="Text font" AssociatedControlID="ddlTextFont"></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlTextFont" runat="server"></asp:DropDownList>
                        </span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"></span>
                        <span class="inputs"><asp:TextBox runat="server" ID="txtTextFontColour" ></asp:TextBox></span>
                        <asp:Label ID="pnlTextColour" runat="server" style="width: 18px; height: 18px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>
                        <span class="inputicon">
                            <cc1:ColorPickerExtender ID="ColorPickerExtender1" runat="server" TargetControlID="txtTextFontColour" PopupButtonID="imgTextFontColour" SampleControlID="pnlTextColour" OnClientColorSelectionChanged="ColourChange"></cc1:ColorPickerExtender>
                            <asp:ImageButton ID="imgTextFontColour" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />
                        </span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="validColour" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Text Font colour." ControlToValidate="txtTextFontColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label6" runat="server" Text="Show labels" AssociatedControlID="chkShowLabels"></asp:Label>
                        <span class="inputs"><asp:CheckBox ID="chkShowLabels" runat="server"></asp:CheckBox></span>
                        <span class="inputtooltipfield"><asp:Image ID="imgTooltipShowLabels" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon " onmouseover="SEL.Tooltip.Show('C1BDF625-99A2-4E99-B194-4E53B46E79BE', 'sm', this);"/></span>
                        <span class="inputvalidatorfield"></span>
                        <asp:Label ID="Label7" runat="server" Text="Show values" AssociatedControlID="chkShowValues"></asp:Label>
                        <span class="inputs"><asp:CheckBox ID="chkShowValues" runat="server"></asp:CheckBox></span>
                        <span class="inputtooltipfield"><asp:Image ID="imgTooltipShowValues" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon " onmouseover="SEL.Tooltip.Show('77581902-E002-46EF-9432-BE9D9DE3A0A7', 'sm', this);"/></span>
                        <span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label8" runat="server" Text="Show percent" AssociatedControlID="chkShowPercent" CssClass="percent"></asp:Label>
                        <span class="inputs percent"><asp:CheckBox ID="chkShowPercent" runat="server" CssClass="percent"></asp:CheckBox></span>
                        <span class="inputtooltipfield percent"><asp:Image ID="imgTooltipShowPercent" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon " onmouseover="SEL.Tooltip.Show('74A89C11-A440-46BD-9FA2-0F58AAA87BD6', 'sm', this);"/></span>
                        <span class="inputvalidatorfield percent"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="Label9" runat="server" Text="Chart size" AssociatedControlID="ddlChartSize"></asp:Label>
                        <span class="inputs"><asp:DropDownList ID="ddlChartSize" runat="server"></asp:DropDownList></span>
                        <span class="inputtooltipfield chartsize"><asp:Image ID="imgTooltipChartSize" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon chartSize" onmouseover="SEL.Tooltip.Show('B76FAA0B-16EC-42CD-9A30-3251A130F55C', 'sm', this);"/></span>

                        <span class="inputvalidatorfield"></span>
                        <asp:Label runat="server" AssociatedControlID="txtTextBackgroundColour">Background colour</asp:Label> 
                        <span class="inputs"> <asp:TextBox runat="server" ID="txtTextBackgroundColour" ></asp:TextBox></span>
                        <asp:Label ID="pnlTextbackgroundColour" runat="server" style="width: 18px; height: 18px; border: 1px solid #000; margin: 0 3px; display: inline-block;"></asp:Label>
                        <span class="inputicon">
                            <cc1:ColorPickerExtender ID="ColorPickerExtender2" runat="server" TargetControlID="txtTextBackgroundColour" PopupButtonID="imgTextBackgroundColour" SampleControlID="pnlTextbackgroundColour" OnClientColorSelectionChanged="ColourChange"></cc1:ColorPickerExtender>
                            <asp:ImageButton ID="imgTextBackgroundColour" ImageUrl="~/shared/images/icons/16/plain/palette.png" runat="server" />
                        </span>
                        <span class="inputtooltipfield"></span>
                        <span class="inputvalidatorfield"><asp:RegularExpressionValidator ID="validBackgroundColour" runat="server" Text="*" Display="Dynamic" ErrorMessage="Please enter a valid value for Background colour." ControlToValidate="txtTextBackgroundColour" ValidationGroup="vgMain" ValidationExpression="^[0-9a-fA-F]{6}$"></asp:RegularExpressionValidator></span>
                    
                    </div>
                </div>
            </div>
    
           
            </div>
        </div>

        <div class="formpanel formbuttons reportsButtons">
            <helpers:CSSButton id="btnSaveReport" runat="server" text="save" onclientclick=" SEL.Reports.Misc.SaveReport('main');return false; " UseSubmitBehavior="False"/>
            <helpers:CSSButton id="btnCancelReport" runat="server" text="cancel" onclientclick=" SEL.Reports.Report.Cancel();return false; " UseSubmitBehavior="False"/>
        </div>

        <div id="editPanel" style="display: none;">
            <asp:TextBox runat="server" ID="txtInput" CssClass="numberInput" /><cc1:FilteredTextBoxExtender ID="filterInput" runat="server" TargetControlID="txtInput" FilterMode="ValidChars" ValidChars="0123456789." />
            <asp:TextBox runat="server" ID="txtStringInput" CssClass="stringInput" />
            <ul id="editItems"></ul>
        </div>
        
    <div id="dialog-confirm-column-delete" title="Delete column?" style="display: none; max-height: 80px;">
        <div id="confirmationMessage" style="height: 15px;">Deleting this column will remove it from the chart.</div>
        <div id="confirmationButtons"><span id="confirmColumnDelete"><span class="buttonContainer"><input class="buttonInner" value="confirm" type="button" style="width: 65px"/></span></span><span id="cancelColumnDelete"><span class="buttonContainer"><input class="buttonInner" value="cancel" type="button" style="width: 65px"/></span></span></div>
    </div>

    <asp:HiddenField runat="server" ID="divItemvalue"/>
    <asp:HiddenField runat="server" ID="divCurrencySymbol"/>
    
    <asp:HiddenField runat="server" ID="divCriteriaValue"/>
    <img id="removeColumnIcon" src="/static/icons/16/plain/garbage.png" style="display:none"/>
</asp:Content>