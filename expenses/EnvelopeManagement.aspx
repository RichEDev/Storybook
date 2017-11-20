<%@ Page Language="c#" Inherits="expenses.EnvelopeManagement" MasterPageFile="~/exptemplate.master" CodeBehind="EnvelopeManagement.aspx.cs" %>

<%@ MasterType VirtualPath="~/exptemplate.master" %>
<%-- ReSharper disable Html.PathError --%>
<%@ Register Src="~/shared/usercontrols/ClaimSelection.ascx" TagPrefix="sm" TagName="ClaimSelection" %>
<%-- ReSharper restore Html.PathError --%>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <%-- ReSharper disable Html.PathError --%>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.EnvelopeManagement.js" />
            <%-- ReSharper restore Html.PathError --%>
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/webServices/receipts.asmx" />
        </Services>
    </asp:ScriptManagerProxy>

    <script type="text/javascript">
        
        $(document).ready(function() {

            SEL.EnvelopeManagement.Ids.AccountId = <%= AccountId %>;

            Sys.Application.add_init(function() {
                SEL.EnvelopeManagement.General.Init();
            });

        });

    </script>

    <div runat="server" class="formpanel">
        <div class="inputpaneltitle">
            <asp:Label ID="EnvelopesLabel" runat="server" Text="Manage Envelopes"></asp:Label>
        </div>
        <p>Identify an Employees's envelope from the tree below. Clicking a receipt will preview it. Clicking the envelope itself will allow you to assign it.</p>
    </div>

    <div class="formpanel clearfix">
        <div id="EnvelopeContainer" class="clearfix">
            <div id="SourceTree"></div>
            <div id="ReceiptPreview"></div>
        </div>
    </div>

    <div class="formpanel" id="ClaimSelection">
        <div class="inputpaneltitle">
            <asp:Label ID="ClaimSelectionLabel" runat="server" Text="Find Claim"></asp:Label>
        </div>
        <p>
            Now search for the claim that you wish to attach the envelope to. You can search by claimant or claim name.
            <br />
            Once you have selected the claim, the 'assign' button will appear at the bottom of the page.
        </p>
        <sm:ClaimSelection runat="server" ID="ClaimSelector" />
    </div>

    <div class="formpanel formbuttons" id="AssignmentButtons">
        <helpers:CSSButton ID="ButtonAssign" runat="server" Text="assign" OnClientClick="SEL.EnvelopeManagement.General.ShowConfirmationModal();return false;" />
    </div>

    <div class="formpanel formbuttons" id="DefaultCancelContainer">
        <helpers:CSSButton ID="ButtonCancel" runat="server" Text="close" OnClientClick="SEL.ClaimSelector.Cancel('/adminmenu.aspx');return false;" />
    </div>


    <asp:Panel ID="ConfirmModal" ClientIDMode="Static" runat="server" CssClass="modalpanel">
        <div class="formpanel">
            <div class="sectiontitle">Confirm Assigment</div>
            <div>
                Are you sure you would like to assign envelope <strong id="ConfirmEnvelopeNumber"></strong>&nbsp;to the selected claim: <strong id="ConfirmClaimName"></strong>?
                <br />
                <strong>Please note: this is a one way operation and it cannot be undone.</strong>
            </div>
            <br />
            <div class="formbuttons" id="ConfirmButtons">
                <helpers:CSSButton ID="ButtonConfirmAssign" runat="server" Text="accept" OnClientClick="SEL.EnvelopeManagement.General.AssignToClaim();return false;" />
                <helpers:CSSButton ID="ButtonConfirmCancel" runat="server" Text="cancel" OnClientClick="SEL.EnvelopeManagement.General.HideConfirmationModal();return false;" />
            </div>
        </div>
    </asp:Panel>

</asp:Content>
