<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="CustomMenu.aspx.cs" Inherits="Spend_Management.shared.admin.CustomMenu" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="contentmain" runat="server">
    <div class="formpanel formpanel_padding" id="menuWrapper" style="display: none;">
        <div id="ctl00_contentmain_tabConViews_tabViewFilters_tcFilters" style="width: 100%;">
            <span style="display: inline-block; vertical-align: top; width: 24%">
                <div class="sectiontitle">
                    Available Menu Structure
                </div>
                <div id="baseTree" style="height: 460px;">
                    <ul>
                        <li style="height: 30px; float: right;" id="tooltipCustomMenuHolder" >
                            <img class="btn customMenuEditor" src="/shared/images/icons/16/add2.png" alt="Add New Menu" id="AddNewMenu" title="Add New Menu">
                            <img class="btn customMenuEditor" src="../images/icons/16/Plain/delete2.png" alt="Delete" id="DeleteMenu" style="width: 13%;" title="Delete">
                            <img class="btn customMenuEditor" id="sortUp" src="/shared/images/icons/16/navigate_double_up.png" alt="Move selection up" style="width: 14%;" title="Sort Up">
                            <img class="btn" id="sortDown" src="/shared/images/icons/16/navigate_double_down.png" alt="Move selection down" style="border-width: 0px; width: 14%;"  title="Sort Down">
                        </li>
                    </ul>
                    <div id="CustomMenuTree" style="height: 420px;">
                    </div>

                </div>

            </span><span style="display: inline-block; vertical-align: top; width: 74%; margin-left: 10px;">
                <div class="sectiontitle">
                    Custom Menu Details
                </div>
                <div id="tabs" style="height: 460px; margin-top: 1px;" class="ui-tabs ui-widget ui-widget-content ui-corner-all">
                    <ul>
                        <li><a id="GeneralDetailsTab" href="#tabViewGeneralDetails">General Details</a></li>
                        <li><a id="IconSelectorTab"  href="#iconSelector">Icon</a></li>
                    </ul>
                    <div id="tabViewGeneralDetails" style="padding-top: 5px; padding-left: 5px; display: none;">

                        <div class="sectiontitle">
                            General Details
                        </div>
                        <div class="modalcontentssmall">
                            <div class="onecolumnsmall">
                                <label for="txtMenuName" id="lblMenuName" class="mandatory">Menu name*</label><span class="inputs"><input name="txtMenuName" type="text" maxlength="50" id="txtMenuName"></span><span class="inputicon"></span><span class="inputtooltipfield"></span>
                            </div>
                            <div class="onecolumn">
                                <label for="txtviewdescription" id="lblViewDescription">Description</label><span class="inputs"><textarea name="txtviewdescription" rows="2" cols="20" id="txtviewdescription" textareamaxlength="500"></textarea></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                            </div>
                        </div>
                    </div>

                    <div id="iconSelector">
                        <div class="sectiontitle">Icon</div>
                        <div class="modalcontentssmall">
                            <div class="onecolumnpanel">Use this editor to pick an icon for the view. This is what will be displayed on the menu.</div>
                            <div class="twocolumn">
                                <div id="iconSearchArea">
                                    <div id="iconResultsHeader">
                                        <span id="selectedIconSpan">
                                            <img src="/static/icons/48/new/window_dialog.png" class="selectedIcon"></span>
                                        <span id="selectedIconInfo"><span style="font-weight: bold">Selected icon</span><span id="selectedIconName">window_dialog.png</span></span>
                                        <span id="iconSearchBox"><span id="iconSearchRemoveButton" class="ui-icon-close searchButton" title="Clear search options"></span><span id="iconSearchButton" class="ui-icon-search searchButton" title="Search"></span>
                                            <asp:TextBox ID="txtViewCustomIconSearch" runat="server" MaxLength="21" CssClass="searchBox"></asp:TextBox></span>
                                    </div>
                                    <div id="viewCustomMenuIconContainer">
                                        <span id="iconResultsLeft">&lt</span>
                                        <span id="viewCustomMenuIconResults"></span>
                                        <span id="iconResultsRight">&gt</span>
                                    </div>
                                    <div id="selectedIconContainer"></div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </span>

        </div>
        <div class="formbuttons lable-heightg1">
            <input type="image" name="CmdOk" id="CmdOk" src="../images/buttons/btn_save.png" style="border-width: 0px;" />&nbsp;&nbsp;
        
           <input type="image" name="CmdCancel" onclick="SEL.CustomMenuStructure.Cancel(); return false;" id="CmdCancel" src="../images/buttons/cancel_up.gif" style="border-width: 0px;" />
        </div>
    </div>
    <div>
        <img src="../images/easytree_loading.gif" class="menuPreLoader"/>
    </div>
    <div id="editedItems" style="display: none;">
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript" src="/static/js/jQuery/jquery-ui-1.9.2.custom.js"></script>
    <script type="text/javascript" src="/shared/javaScript/minify/CustomMenuStructure.js"></script>
</asp:Content>
