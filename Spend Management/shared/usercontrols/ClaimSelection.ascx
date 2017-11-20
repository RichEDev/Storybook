<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClaimSelection.ascx.cs" Inherits="Spend_Management.shared.usercontrols.ClaimSelection" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('.buttonInner').focus(function () {
            $(this).css('background-position', 'left -155px');
            $(this).parent().css('background-position', 'right -155px');
        });
        $('.buttonInner').focusout(function () {
            $(this).css('background-position', 'left top');
            $(this).parent().css('background-position', 'right top');
        });
        $('.smallbuttonInner').focus(function () {
            $(this).css('background-position', 'left -34px');
            $(this).parent().css('background-position', 'right -34px');
        });
        $('.smallbuttonInner').focusout(function () {
            $(this).css('background-position', 'left top');
            $(this).parent().css('background-position', 'right top');
        });
        $(document).keydown(function (e) {
            if (e.keyCode === 27) // esc
            {
                e.preventDefault();
                SEL.Common.HideModal('ClaimantSearchModal');
            }
        });
        $('#divMasterPopup').click(function () {
            $('#hrefMasterPopup').focus();
            return false;
        });
        SEL.ClaimSelector.Initialise();
        if (SEL.ClaimSelector.NumberOfClaimants == 0) {
            $('#divNoResults').show(100);
        }
        SEL.ClaimSelector.PageLoadFunctions();
    });
    
</script> 

<div class="formpanel no-padding">
    <div class="sectiontitle">Search by Claimant</div>
    <div class="onecolumnsmall">
        <asp:Label runat="server" ID="ClaimantLabel" AssociatedControlID="ClaimantText">Claimant</asp:Label>
        <span class="inputs">
            <asp:DropDownList ID="ClaimantCombo" runat="server" ClientIDMode="Static" CssClass="fillspan" onchange="SEL.ClaimSelector.ClearClaimName();"></asp:DropDownList>
            <asp:TextBox runat="server" ClientIDMode="Static" ID="ClaimantText" MaxLength="301"></asp:TextBox>
            <asp:TextBox runat="server" ID="ClaimantText_ID" ClientIDMode="Static" Style="display: none;"></asp:TextBox>
        </span>
        <span class="inputicon">
            <%-- ReSharper disable Html.PathError --%>
            <asp:Image runat="server" ID="ClaimantTextSearchIcon" ClientIDMode="Static" ImageUrl="/static/icons/16/new-icons/find.png" onclick="SEL.ClaimSelector.ClaimantCombo.Search(SEL.ClaimSelector.ConditionType);" />
            <%-- ReSharper restore Html.PathError --%>
        </span>
        <span class="inputtooltipfield">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('f54644b6-a224-4e61-96d0-7ac4879062d2', 'sm', this);" />
        </span>
        <span class="inputmultiplevalidatorfield">
            <asp:RequiredFieldValidator runat="server" ID="reqClaimant" ControlToValidate="ClaimantText_ID" ClientIDMode="static" ValidationGroup="vgClaimSelector" Text="*" ErrorMessage="Please select a valid Claimant." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CompareValidator runat="server" ID="cmpValidatorEmployeeId" ControlToValidate="ClaimantText_ID" ClientIDMode="Static" Type="Integer" Operator="GreaterThanEqual" ValidationGroup="vgClaimSelector" Text="*" ErrorMessage="Please select a valid Claimant." Display="Dynamic" ValueToCompare="1"></asp:CompareValidator>
        </span>

    </div>
    <div class="sectiontitle">Search by Claim Name</div>
    <div class="twocolumn">
        <asp:Label runat="server" ID="lblClaimName" AssociatedControlID="ClaimNameText">Claim name</asp:Label>
        <span class="inputs">
            <asp:TextBox runat="server" ID="ClaimNameText" ClientIDMode="Static" MaxLength="50"></asp:TextBox>
        </span>
        <span class="inputicon"></span>
        <span class="inputtooltipfield">
            <asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('B93B80BD-B19E-4B29-9EC2-A3D0110D343D', 'sm', this);" />
        </span>
        <span class="inputmultiplevalidatorfield">
            <asp:RequiredFieldValidator runat="server" ID="reqClaimName" ControlToValidate="ClaimNameText" ClientIDMode="static" ValidationGroup="vgClaimSelector" Text="*" ErrorMessage="Please enter a Claim name." Display="Dynamic"></asp:RequiredFieldValidator>
        </span>
    </div>
    <div class="onecolumnpanel" id="divNoResults" style="display: none;">Under the current configuration of the system, there are either no claimants or no claims you can view.</div>
    <div class="formbuttons" id="SearchButtonContainer">
        <helpers:CSSButton ID="SearchButton" ClientIDMode="Static" runat="server" Text="search" OnClientClick="SEL.ClaimSelector.SearchEmployee();return false;" UseSubmitBehavior="False" />
        <helpers:CSSButton ID="CloseButton" ClientIDMode="Static" runat="server" Text="cancel" OnClientClick="SEL.ClaimSelector.Cancel(returnPage);return false;" UseSubmitBehavior="False" />
    </div>
    <div id="SearchGrid"></div>
</div>
<asp:Panel ID="ClaimantSearchPanel" runat="server" CssClass="modalpanel formpanel" Style="display: none;">
    <div class="sectiontitle">Claimant Search</div>
    <%-- ReSharper disable once UnknownCssClass --%>
    <div class="searchgrid"></div>
    <div class="formbuttons">
        <helpers:CSSButton runat="server" ID="ClaimantSearchCancel" Text="cancel" OnClientClick="SEL.ClaimSelector.HideSearchModal();return false;" UseSubmitBehavior="False" />
    </div>
</asp:Panel>
<cc1:ModalPopupExtender ClientIDMode="Static" runat="server" ID="ClaimantSearchModal" BackgroundCssClass="modalBackground" TargetControlID="ClaimantSearchLink" PopupControlID="ClaimantSearchPanel" CancelControlID="ClaimantSearchCancel"></cc1:ModalPopupExtender>
<asp:LinkButton runat="server" ID="ClaimantSearchLink" Style="display: none;"></asp:LinkButton>