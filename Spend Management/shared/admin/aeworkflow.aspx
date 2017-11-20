<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeworkflow.aspx.cs" Inherits="Spend_Management.aeworkflow" Title="Workflow" %>

<%@ Register TagPrefix="fieldSel" TagName="fieldSel" Src="~/shared/usercontrols/fields_selector.ascx" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
        #stepControls a{
            text-decoration: underline;
            color: #003768;
        }

        .formbuttons {
            padding-top: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/fields_selector.js" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.workflows.js" />
            <asp:ScriptReference Path="~/shared/javaScript/workflows.js" />
        </Scripts>
        <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcAutoComplete.asmx" />
        </Services>
</asp:ScriptManagerProxy>

<script type="text/javascript" language="javascript">
    var workflowID = '<% = nWorkflowID %>';
    var ddlStepAction = '<%=ddlStepAction.ClientID %>';
    var popupOptions = '<%=popOptions.ClientID %>';
    var popupModal = '<%=mdlStep.ClientID %>';
    var workflowNameClientID = '<% = txtworkflowname.ClientID %>';
    var workflowDescriptionClientID = '<% = txtdescription.ClientID %>';
    var workflowTypeClientID = '<% = cmbworkflowtype.ClientID %>';
    var workflowAvailableAsAChildWorkflowClientID = '<% = chkchildworkflow.ClientID  %>';
    var workflowRunOnCreationClientID = '<% = chkrunoncreation.ClientID %>';
    var workflowRunOnChangeClientID = '<% = chkrunonchange.ClientID %>';
    var workflowRunOnDeleteClientID = '<% = chkrunondelete.ClientID %>';
    var tabContainerClientID = '<% = tcWorkflow.ClientID %>';
    var currentSteps = new Array();
</script>

<div class="formpanel formpanel_padding">
        <cc1:TabContainer ID="tcWorkflow" runat="server" OnClientActiveTabChanged="SaveWorkflow">
            <cc1:TabPanel ID="tabGeneralDetails" runat="server">
                <HeaderTemplate>General Details</HeaderTemplate>
                <ContentTemplate>
                    <div class="formpanel">
                        <div class="twocolumn">
                            <asp:Label ID="lblworkflowname" runat="server" AssociatedControlID="txtworkflowname" CssClass="mandatory">Name</asp:Label><span class="inputs"><asp:TextBox ID="txtworkflowname" runat="server" title="Name" CssClass="fillspan" onchange="SetWorkflowChanged();"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label ID="lblworkflowtype" runat="server" AssociatedControlID="cmbworkflowtype" CssClass="mandatory">Element</asp:Label><span class="inputs"><asp:DropDownList ID="cmbworkflowtype" runat="server" title="Workflow Type" CssClass="fillspan" onchange="SetWorkflowChanged();"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn" style="display:none;">
                            <asp:Label ID="lblrunoncreate" runat="server" AssociatedControlID="chkrunoncreation">Run on record creation</asp:Label><span class="inputs"><asp:CheckBox ID="chkrunoncreation" runat="server" title="Run on record creation" onchange="SetWorkflowChanged();" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                            <asp:Label ID="lblrunonchange" runat="server" AssociatedControlID="chkrunonchange">Run on record change</asp:Label><span class="inputs"><asp:CheckBox ID="chkrunonchange" runat="server" title="Run on record change" onchange="SetWorkflowChanged();" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn" style="display:none;">
                            <asp:Label ID="lblrunondelete" runat="server" AssociatedControlID="chkrunondelete">Run on record delete</asp:Label><span class="inputs"><asp:CheckBox ID="chkrunondelete" runat="server" title="Run on record deletion" onchange="SetWorkflowChanged();" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label ID="lblchildworkflow" runat="server" AssociatedControlID="chkchildworkflow">Child Workflow</asp:Label><span class="inputs"><asp:CheckBox ID="chkchildworkflow" runat="server" title="Available as a child workfow\" onchange="SetWorkflowChanged();" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumn">
                            <asp:Label ID="lbldescription" runat="server" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtdescription" TextMode="MultiLine" runat="server" title="Workflow description" onchange="SetWorkflowChanged();"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span>
                        </div>                        
                       
                            <input type="hidden" id="workflowID" value="0" />
                            
                        </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabSteps" runat="server">
                <HeaderTemplate>Steps</HeaderTemplate>
                <ContentTemplate>
                    <div class="inputpanel">
                        <div class="inputpaneltitle">Workflow Steps</div>
                        <div id="stepControls"><a href="javascript:void(0);" onclick="showStepOptions(event, popupOptions);" title="Step Options" id="stepOptions">Show step options</a></div>
                        
                            <div id="stepsHelp">Please use the Steps Options menu to begin creating your workflow.</div>
                            <div id="stepsLayout" style="overflow: auto;"></div>
                            <!-- DIV THAT WILL HOLD THE WORKFLOW STEPS. -->
                        
                                            </div>
                    
                    <asp:Panel ID="pnlStep" runat="server" Style="display: none; height: 600px;  width:800px; overflow: auto;" CssClass="modalpanel">
                        <div class="formpanel">
                            <div class="sectiontitle">Workflow Steps</div>
                                <div class="twocolumn">
                                    <asp:Label ID="lblStepAction" runat="server" AssociatedControlID="ddlStepAction">Action</asp:Label>
                                    <span class="inputs">
                                        <asp:DropDownList ID="ddlStepAction" runat="server" onchange="changeSpecificDetails();" style="width: 200px;">
                                            <asp:ListItem Value="0" Text="Select the step action"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Approval"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="Change Value"></asp:ListItem>
                                            <asp:ListItem Value="14" Text="Change Form"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="Check Condition"></asp:ListItem>
                                            <asp:ListItem Value="11" Text="Create Dynamic Value"></asp:ListItem>
                                            <asp:ListItem Value="4" Text="Decision"></asp:ListItem>
                                            <asp:ListItem Value="9" Text="Finish Workflow"></asp:ListItem>
                                            <asp:ListItem Value="5" Text="Move To Step"></asp:ListItem>
                                            <asp:ListItem Value="6" Text="Run Sub-Workflow"></asp:ListItem>
                                            <asp:ListItem Value="7" Text="Send Email"></asp:ListItem>
                                            <asp:ListItem Value="13" Text="Show Message"></asp:ListItem>
                                        </asp:DropDownList>
                                    </span>
                                </div>
                        
                            <div class="sectiontitle">Step Details</div>
                            

                                <asp:Panel ID="pnlSpecificDetails" runat="server">
                                <div id="specificDetails">Please select a step action.</div>
                                <fieldSel:fieldSel ID="fieldSel" runat="server" />
                                </asp:Panel>                                
                            
                                <div class="formbuttons"><asp:Image ID="imgAddStep" runat="server" ImageUrl="~/shared/images/buttons/btn_addstep.gif" onclick="addStep();" style="cursor: pointer;" /> <asp:Image ID="imgCancelstep" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" onclick="cancelStepModal();" style="cursor: pointer;" /></div>

                        </div>

                        </asp:Panel>
                        
                        
                    <cc1:ModalPopupExtender ID="mdlStep" runat="server" TargetControlID="lnkNewStep"
                        PopupControlID="pnlStep" OnCancelScript="cancelStepModal" OnOkScript="addStep"
                        BackgroundCssClass="modalBackground" Drag="True">
                    </cc1:ModalPopupExtender>
                    <asp:HyperLink ID="lnkNewStep" runat="server" NavigateUrl="#" Style="display: none;">Add Step</asp:HyperLink>
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>
        
        <div class="formbuttons imgbuttons">
            <asp:Image ID="imgSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="SaveWorkflow(true);" />
             <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" OnClientClick="window.location='/shared/admin/workflows.aspx'; return false;" />
        </div>
</div>

    <asp:Panel ID="pnlOptions" runat="server" Style="visibility: hidden;">
        <div id="divStepOptions" style="margin-top: 20px; line-height: 20px; padding: 4px;
            background-color: #ffffff; border: solid 1px #000000;">
        </div>
    </asp:Panel>
    <cc1:PopupControlExtender ID="popOptions" runat="server" PopupControlID="pnlOptions"
        TargetControlID="lnkPageOptions">
    </cc1:PopupControlExtender>
    <asp:HyperLink ID="lnkPageOptions" runat="server" NavigateUrl="javascript:void(0);" Style="display: none;">Page Options</asp:HyperLink>
    <%--<script type="text/javascript" language="javascript" src="/shared/javascript/workflows.js"></script>--%>
</asp:Content>
