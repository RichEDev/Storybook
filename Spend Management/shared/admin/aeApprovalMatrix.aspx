<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="aeApprovalMatrix.aspx.cs" Inherits="Spend_Management.shared.admin.aeApprovalMatrix" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');$g(SEL.ApprovalMatrices.DomIDs.General.MatrixName).focus();" id="lnkGeneral" class="selectedPage">Approval Matrix Details</a> <a href="javascript:SEL.ApprovalMatrices.Level.Focus();" id="lnkLevels">Levels</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentoptions" runat="server">
    <div id="pgOptLevels" style="display: none;">
        <a href="javascript:SEL.ApprovalMatrices.Level.New();" class="submenuitem" ID="lnkNewLevel" runat="server">New Level</a> 
    </div>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.approvalMatrices.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

<script type="text/javascript" language="javascript">
    (function(r)
    {
        r.General.MatrixName = '<%= txtmatrixname.ClientID %>';
        r.General.MatrixDescription = '<%= txtdescription.ClientID %>';
        r.General.DefaultApprover = '<%= txtDefaultApprover.ClientID %>';
        r.General.DefaultApproverKey = '<%= txtDefaultApprover_ID.ClientID %>';
        r.General.DefaultApproverMandatoryValidator = '<%= rfvDefaultApprover.ClientID %>';
        r.General.DefaultApproverComparisonValidator = '<%= cvDefaultApprover.ClientID %>';
        r.Level.Modal = '<%= modLevel.ClientID %>';
        r.Level.Panel = '<%= pnlLevel.ClientID %>';
        r.Level.ApprovalLimit = '<%= txtApprovalLimit.ClientID %>';
        r.Level.ApproverKey = '<%= txtLevelApprover_ID.ClientID %>';
        r.Level.Approver = '<%= txtLevelApprover.ClientID %>';
        r.Level.Grid = '<%= pnlLevelGrid.ClientID %>';
        r.Level.ApprovalLimitMandatoryValidator = '<%= reqApprovalLimit.ClientID %>';
        r.Level.ApprovalLimitComparisonValidator = '<%= cvApprovalLimit.ClientID %>';
        r.Level.ApproverMandatoryValidator = '<%= rfvLevelApprover.ClientID %>';
        r.Level.ApproverComparisonValidator = '<%= cvApprover.ClientID %>';
    }(SEL.ApprovalMatrices.DomIDs));
    
    $(document).ready(function ()
    {
        SEL.Common.SetTextAreaMaxLength();
        SEL.ApprovalMatrices.SetupEnterKeyBindings();
    });
    
</script>
    <div id="divPages">
        <div id="pgGeneral" class="primaryPage">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">General Details</div>
                <div class="twocolumn">
                    <asp:Label CssClass="mandatory" ID="lblmatrixname" runat="server" Text="Approval matrix name*" AssociatedControlID="txtmatrixname"></asp:Label><span class="inputs"><asp:TextBox ID="txtmatrixname" runat="server" CssClass="fillspan" MaxLength="250" ></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftbeMatrixName" runat="server" TargetControlID="txtmatrixname" FilterMode="InvalidChars" InvalidChars="<>" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqmatrixname" runat="server" ErrorMessage="Please enter an Approval matrix name." Text="*" ControlToValidate="txtmatrixname" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator></span>
                </div>
                <div class="onecolumn">
                    <asp:Label ID="lbldescription" runat="server" Text="" AssociatedControlID="txtdescription" ><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtdescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span>
                </div>
                <div class="twocolumn">
                    <asp:Label CssClass="mandatory" ID="lblDefaultApprover" runat="server" Text="Default approver*" AssociatedControlID="txtDefaultApprover"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtDefaultApprover" MaxLength="320" /><asp:TextBox runat="server" style="display: none;" ID="txtDefaultApprover_ID" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgToolTip590" onmouseover="SEL.Tooltip.Show('f76a3fc1-a4ca-4e60-8530-869bf929f363', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvDefaultApprover" runat="server" ErrorMessage="Please enter a Default approver." Text="*" ControlToValidate="txtDefaultApprover" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cvDefaultApprover" Text="*" ValidationGroup="vgMain" ControlToValidate="txtDefaultApprover_ID" ErrorMessage="Please enter a valid Default approver." Display="Dynamic" Operator="NotEqual" ValueToCompare="-1"></asp:CompareValidator></span>
                </div>
            </div>
        </div>
        <div id="pgLevels" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <asp:Panel ID="pnlLevelGrid" runat="server">
                    <asp:Literal runat="server" ID="litgrid"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div class="formpanel formbuttons formpanel_padding">
            <helpers:CSSButton id="btnSaveEntity" runat="server" text="save" onclientclick="SEL.ApprovalMatrices.Matrix.Save('matrix');return false;" UseSubmitBehavior="False"/>
            <helpers:CSSButton id="btnCancelEntity" runat="server" text="cancel" onclientclick="SEL.ApprovalMatrices.Matrix.Cancel();return false;" UseSubmitBehavior="False"/>
        </div>
    </div>
        <asp:Panel ID="pnlLevel" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div>
            <div class="sectiontitle" id="divApprovalMatrixLevelTitle">Level</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblApprovalLimit" runat="server" Text="Approval limit*" AssociatedControlID="txtApprovalLimit"></asp:Label><span class="inputs"><asp:TextBox ID="txtApprovalLimit" runat="server" CssClass="fillspan" maxlength="15" ></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftbLevelAmount" runat="server" TargetControlID="txtApprovalLimit" FilterMode="ValidChars" ValidChars="0123456789." /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqApprovalLimit" Text="*" ValidationGroup="vgLevel" ControlToValidate="txtApprovalLimit" ErrorMessage="Please enter an Approval limit." Display="Dynamic" ></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cvApprovalLimit" Type="Currency" ErrorMessage="Please enter a valid Approval limit.  Valid characters are the numbers 0-9 and a full stop (.) limited to two decimal places." Text="*" ControlToValidate="txtApprovalLimit" Operator="DataTypeCheck" ValidationGroup="vgLevel" Display="Dynamic" /></span>
            </div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblLevelApprover" runat="server" Text="Approver*" AssociatedControlID="txtLevelApprover"></asp:Label><span class="inputs"><asp:TextBox runat="server" id="txtLevelApprover" MaxLength="320" /><asp:TextBox runat="server" style="display: none;" ID="txtLevelApprover_ID" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvLevelApprover" runat="server" ErrorMessage="Please enter an Approver." Text="*" ControlToValidate="txtLevelApprover" ValidationGroup="vgLevel" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cvApprover" Text="*" ValidationGroup="vgLevel" ControlToValidate="txtLevelApprover_ID" ErrorMessage="Please enter a valid Approver." Display="Dynamic" Operator="NotEqual" ValueToCompare="-1"></asp:CompareValidator></span>
            </div>
        </div>
        <div class="formbuttons formpanel_padding">
            <helpers:CSSButton ID="btnSaveLevel" runat="server" Text="save" onclientclick="SEL.ApprovalMatrices.Level.Save();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelLevel" runat="server" Text="cancel" onclientclick="SEL.ApprovalMatrices.Level.Cancel();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modLevel" runat="server" TargetControlID="lnkLevel" PopupControlID="pnlLevel" BackgroundCssClass="modalBackground" CancelControlID="btnCancelLevel" Enabled="True"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkLevel" runat="server" Style="display: none;"></asp:LinkButton>
    
    
    <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
</asp:Content>
