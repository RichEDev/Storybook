<%@ Page Language="c#" Inherits="expenses.ReceiptManagement" MasterPageFile="~/exptemplate.master" CodeBehind="ReceiptManagement.aspx.cs" %>

<%@ MasterType VirtualPath="~/exptemplate.master" %>

<%-- ReSharper disable Html.PathError --%>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/jquery.form.js" ScriptMode="Release" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/jquery.ajaxfileupload.js" ScriptMode="Release" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/knockout-3.1.0.debug.js" ScriptMode="Release" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/knockout.mapping-latest.debug.js" ScriptMode="Release" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/knockout.sortable.js" ScriptMode="Release" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.ReceiptManagement.js?date=20180116" ScriptMode="Release" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/webServices/receipts.asmx" />
        </Services>
    </asp:ScriptManagerProxy>

    <script type="text/html" id="expense-template">
        <div class="receipt-management-expense-info sectiontitle clearfix" data-bind="css: { emptyExpense: ($data.Children().length == 0 && $root.declarationMode && $data.NeedsReceipts()) }">
            <span class="receipt-management-expense-info-total" data-bind="text: Total"></span>
            <span class="receipt-management-expense-info-id" data-bind="text: Id"></span>
            <span class="receipt-management-expense-info-name" data-bind="text: Name"></span>
            <span class="receipt-management-expense-info-ref" data-bind="text: Reference"></span>
            <span class=".receipt-management-expense-info-date" data-bind="text: Date"></span>
        </div>
        <ul class="receipt-management-expense-children" data-bind="sortable: { template: 'receipt-template', data: Children, options: { cancel: '.prevent' } }, css: { emptyExpense: ($data.Children().length == 0 && $root.declarationMode && $data.NeedsReceipts()) }"></ul>
    </script>

    <script type="text/html" id="receipt-template">
        <li class="receipt-management-receipt" data-bind="css: { prevent: $data.PreventDelete() }, attr: { title: EditMessage }">
            <img data-bind="src: $data.IsImage() ? $data.Url() : $data.Icon()" alt="" />
            <span class="receipt-management-drag" title="Move or copy this receipt" data-bind="visible: !$data.PreventDelete()"></span>
            <span class="receipt-management-preview" title="Preview this receipt" data-bind="click: function () { $root.preview($data); }"></span>
            <a class="receipt-management-delete" href="#" title="Delete this receipt" data-bind="visible: !$data.PreventDelete(), click: function () { $root.deleteItem($data, $parent, $root); }"></a>
            <a class="receipt-management-lock" href="#" data-bind="visible: $data.PreventDelete(), attr: { title: EditMessage }"></a>
        </li>
    </script>

    <script type="text/javascript">

        $(document).ready(function () {

            SEL.ReceiptManagement.Ids.CurrentClaimId = "<%= ClaimId %>";
            SEL.ReceiptManagement.Ids.CurrentExpenseId = "<%= ExpenseId %>";
            SEL.ReceiptManagement.Ids.FromClaimSelector = <%= (FromClaimSelector ? "true" : "false") %>;
            SEL.ReceiptManagement.Ids.DeleteReasonModalId = "<%= modalDeleteReason.ClientID %>";
            SEL.ReceiptManagement.Ids.DeleteReasonTextId = "<%= TextDeleteReason.ClientID %>";
            SEL.ReceiptManagement.Ids.DeleteValidationId = "<%= DeleteReasonValidator.ClientID %>";
            SEL.ReceiptManagement.Ids.MustGiveDeleteReason = "<%= MustGiveDeleteReason %>";
            SEL.ReceiptManagement.Ids.IsDeclaration = "<%= Declare %>";
            SEL.ReceiptManagement.Ids.DeclarationModalId = "<%= modalDeclareMatchingComplete.ClientID %>";
            SEL.ReceiptManagement.Ids.DeclarationAcceptButtonId = "<%= ButtonDeclareAccept.ClientID %>";
            SEL.ReceiptManagement.Ids.SaveButtonId = "<%= Save.ClientID %>";
            SEL.ReceiptManagement.Ids.CancelButtonId = "<%= Cancel.ClientID %>";

            SEL.ReceiptManagement.General.Init();
        });

    </script>


    <div runat="server" class="widepanel formpanel formpanel_padding">
        <div class="inputpaneltitle">
            <asp:Label ID="Receipts" runat="server" Text="Manage Receipts"></asp:Label>
        </div>
        <div class="comment error-comment" runat="server" id="DeclareMatchingCompleteNotice">
            <p>You have been directed to this page because you have expense items that will be validated, but have no receipt images attached.</p>
            <ul>
                <li>By clicking save at the bottom of the page you will declare all receipt-expense matching as complete and your claim can proceed to the next stage.</li>
                <li>Note that this is your final chance to modify the receipts before your claim is locked and progressed.</li>
                <li>Validatable expenses that do not have receipts are shown in red.</li>
            </ul>
        </div>
        <div class="comment" id="instructions">
            <span>Your claim and its receipt images are below. <a href="#instruction-detail" id="instruction-prompt">Show help</a>.</span>
            <ul id="instruction-detail">
                <li>The claim itself and each expense item has a “receipt area”. This is how you manage the receipt images assigned to expense items.</li>
                <li>You can click an expense item to hide or show the attached receipt images.</li>
                <li>Hovering over an expense item’s “receipt area” will display the upload image button.</li>
                <li>Clicking the magnifying glass on a receipt image will preview it.</li>
                <li>Receipt images can be assigned to multiple expense items, and you should aim to minimise the number of receipt images attached to the claim header.</li>
                <li>Move and copy receipt images until you are satisfied. This is achieved by dragging and dropping receipt images.</li>
                <li>When you drop the receipt image, you will be prompted to move or copy.</li>
                <li>Depending on the status of your claim, and/or whether you are an approver, you may not be able to edit or delete receipt images.</li>
                <li>Clicking cancel will revert any changes you have made.</li>
            </ul>
        </div>
        <div class="inputpanel" id="ReceiptUploadError"></div>
        <br />
        <div id="ReceiptsAndButtons">
            <div id="ReceiptTree">
                <div class="receipt-management-claim-info sectiontitle clearfix">
                    <span class="receipt-management-claim-info-total" data-bind="text: Total"></span>
                    <span class="receipt-management-claim-info-id" data-bind="text: Id"></span>
                    <span class="receipt-management-claim-info-name" data-bind="text: Name"></span>
                    <span class="receipt-management-claim-info-count" data-bind="text: NumberOfItems"></span>
                    <span class="receipt-management-claim-info-date" data-bind="text: Date"></span>
                </div>
                <ul class="receipt-management-claim-children sectiontitle" data-bind="sortable: { template: 'receipt-template', data: Header.Children, options: { cancel: '.prevent' } }"></ul>
                <ul data-bind="event: { mouseover: $root.showUpload, mouseout: $root.hideUpload }, template: { name: 'expense-template', foreach: Children, as: 'expense' }"></ul>
            </div>
            <div id="ReceiptsButtons">
                <asp:ImageButton ID="Save" CausesValidation="False" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" />
                <asp:ImageButton ID="Cancel" CausesValidation="False" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" />
            </div>
        </div>
        <div id="ReceiptManagementPreview"></div>
    </div>
    <br />



    <asp:Panel ID="modalDeleteReason" runat="server" CssClass="modalpanel">
        <div class="formpanel">
            <div class="sectiontitle">Delete Receipt</div>
            <div class="onecolumn">
                <asp:Label ID="LabelDeleteReceipt" runat="server" Text="Reason for deleting*" AssociatedControlID="TextDeleteReason" CssClass="mandatory"></asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="TextDeleteReason" TextMode="MultiLine" runat="server" textareamaxlength="4000"></asp:TextBox>
                </span>
            </div>
            <span runat="server" id="DeleteReasonValidator" clientidmode="Static" class="errortext">Please enter a reason for deleting.</span>
            <div class="formbuttons">
                <helpers:CSSButton ID="ButtonDeleteReceiptAccept" runat="server" Text="continue with delete" ValidationGroup="ValidationDelete" OnClientClick="SEL.ReceiptManagement.General.DeleteReceipt();return false;" />
                <helpers:CSSButton ID="ButtonDeleteCancel" runat="server" Text="cancel" OnClientClick="SEL.ReceiptManagement.General.HideDeleteModal();return false;" />
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="modalDeclareMatchingComplete" runat="server" CssClass="modalpanel" ClientIDMode="Static">
        <div class="formpanel">
            <div class="sectiontitle">I Declare Matching Is Complete</div>
            <p>I confirm that all matching is now complete on this claim and its expense items. Please progress my claim to the next stage.</p>
            <div class="formbuttons">
                <helpers:CSSButton ID="ButtonDeclareAccept" CausesValidation="False" ClientIDMode="Static" runat="server" Text="accept" OnClientClick="SEL.ReceiptManagement.General.Confirm();return false;" />
                <helpers:CSSButton ID="ButtonDeclareCancel" CausesValidation="False" runat="server" Text="cancel" OnClientClick="SEL.ReceiptManagement.General.HideConfirmModal();return false;" />
            </div>
        </div>
    </asp:Panel>

</asp:Content>

<%-- ReSharper restore Html.PathError --%>

