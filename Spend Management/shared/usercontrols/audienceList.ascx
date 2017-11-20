<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="audienceList.ascx.cs"
    Inherits="Spend_Management.audienceList" %>
<asp:ScriptManagerProxy ID="smp" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.audiences.js" />
    </Scripts>
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcAudiences.asmx" InlineScript="true" />
    </Services>
</asp:ScriptManagerProxy>
<div>

    <script type="text/javascript">
        //<![CDATA[
        var audId;
        var entityIdentifier;
        var baseTableId;
        var parentRecordId;
        var ucAudienceModalID = '<%=mdlAudienceSearchGridUC.ClientID %>';
        var pnlAudienceSearchGrid = '<%=pnlAudienceSearchGridUC.ClientID %>';
        var btnDummy = '<%=lnkDummy.ClientID %>';
        var pnlAudienceGrid = '<%=pnlAudienceGridUC.ClientID %>';
        var audienceCanDelete;
        var audienceCanEdit;
        //]]>
    </script>

    <div class="formpanel">
        <div class="sectiontitle">
            Current Audiences</div>
        <div>
            <asp:HyperLink runat="server" ID="lnkAddAudience">New Audience</asp:HyperLink>
        </div>
        <div style="height:7px;"></div>
        <div>
            <asp:Panel ID="pnlAudienceGridUC" runat="server">
                <asp:Literal ID="litAudienceGridUC" runat="server"></asp:Literal>
            </asp:Panel>
        </div>
    </div>
    <cc1:ModalPopupExtender ID="mdlAudienceSearchGridUC" runat="server" CancelControlID="btnCancelAudienceUC"
        PopupControlID="pnlAudienceSearchGridUCModal" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>

    <asp:Panel ID="pnlAudienceSearchGridUCModal" runat="server" CssClass="modalpanel formpanel">
            <div class="sectiontitle">Audience Security Permissions</div>
            <div class="twocolumn">
			    <label for="chkCanView">Can view?</label>
                <span class="inputs">
                    <input type="checkbox" id="chkCanView" class="fillspan" onclick="SEL.Audience.CheckPermissionState();" />
			    </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
			    <label for="chkCanEdit">Can edit?</label>
                <span class="inputs">
                    <input type="checkbox" id="chkCanEdit" class="fillspan" onclick="SEL.Audience.CheckPermissionState();" />
			    </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
			    <label for="chkCanDelete">Can delete?</label>
                <span class="inputs">
                    <input type="checkbox" id="chkCanDelete" class="fillspan"  onclick="SEL.Audience.CheckPermissionState();" />
			    </span>
                <span class="inputicon"></span>
                <span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield"></span>
            </div>
            <div class="sectiontitle" id="divAudience">Add Audience</div>
            <asp:Panel ID="pnlAudienceSearchGridUC" runat="server" CssClass="twocolumn" ScrollBars="auto">
            <asp:Literal ID="litAudienceSearchGridUC" runat="server"></asp:Literal></asp:Panel>
            <div class="formbuttons">
                <asp:Image ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" runat="server"
                    ID="btnSaveAudienceUC" onclick="javascript:SEL.Audience.SaveAudienceRecord();" />&nbsp;<asp:Image
                        ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" runat="server"
                        ID="btnCancelAudienceUC" />
            </div>
    </asp:Panel>
    <asp:HyperLink runat="server" ID="lnkDummy" Style="display: none;"></asp:HyperLink>
</div>
