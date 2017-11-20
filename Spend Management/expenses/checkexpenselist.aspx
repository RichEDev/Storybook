<%@ Page language="c#" Inherits="Spend_Management.checkexpenselist" MasterPageFile="~/masters/smForm.master" Codebehind="checkexpenselist.aspx.cs" EnableViewState="false" EnableSessionState="ReadOnly" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>


<asp:Content runat="server" ID="CustomStyles" ContentPlaceHolderID="styles">
    <style type="text/css">
       .twocolumn>span>select {
            font-size: 12px;
    }

       #ctl00_contentmain_pnldeclaration {
           padding: 20px;
       }

       #ctl00_contentmain_pnldeclaration a {
               text-decoration: underline;
               color: #003768;
       }
    </style>
    <!--[if IE 7]>
        <style>
        #ctl00_contentmain_lblcurrentstage {
            padding-left: 32px;
        }
        </style>
    <![endif]-->
</asp:Content>		
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.claims.js?date=20161112" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/expenses/webServices/claims.asmx" />
        </Services>
    </asp:ScriptManagerProxy> 
                    
    <asp:Literal ID="litMenu" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">		
<script language="javascript" type="text/javascript">
    var hdnMessage = '<%=hdnMessage.ClientID %>';
    $(document).ready(function () {
        //adding the unique class for removing Jquery buttons and adding the custom buttons
        $('html').addClass('expenseList');

        SEL.Common.SetTextAreaMaxLength();

        SEL.Claims.IDs.UnsubmitClaimAsApproverModalId = "<%= mdlUnsubmitClaimAsApprover.ClientID %>";
        SEL.Claims.IDs.UnsubmitClaimAsApproverReasonTextId = "<%= txtUnsubmitReason.ClientID %>";
        SEL.Claims.IDs.txtReturnReasonId = "<%= txtReturnReason.ClientID %>";
        
        $('#additionalEnvelopeLink').hover(function () {
            var additionalNumbers = $('#additionalEnvelopeNumbers');

            additionalNumbers.css('left', $(this).offset().left + 70).css('top', $(this).offset().top + 15);

            additionalNumbers.stop(true, true).fadeIn(500);

        }, function () {
            $('#additionalEnvelopeNumbers').delay(300).fadeOut(400);
        });

        $('#additionalEnvelopeNumbers').hover(function () {
            $('#additionalEnvelopeNumbers').stop(true, true).fadeIn(500);
        }, function () {
            $('#additionalEnvelopeNumbers').delay(300).fadeOut(400);
        });


        $("body").on("mouseenter", ".passengersinfoicon", function () {
            var passengersInfoComment = $(".passengersinfocomment", $(this).closest("td"));
            $(passengersInfoComment).show().position({ my: "left top", at: "right+10 top", of: this });
        }).on("mouseleave", ".passengersinfoicon", function () {
            var passengersInfoComment = $(".passengersinfocomment", $(this).closest("td"));
            $(passengersInfoComment).hide();
        });

        $('#btnMasterPopup').click(function () {
            var hdnLineManagerMessage = document.getElementById('<%=hdnMessage.ClientID %>');
            if (hdnLineManagerMessage) {
                if (hdnLineManagerMessage.value == 'Since the claim items you are allowing are over your authorisation limit, the claim items have been escalated for higher level approval.') {
                window.location.href += "&UpdateItemChecker=true";
                }
            }
        });

    });
    function previousclaims_onchange(employeeid)
    {
        var claimid;
					
        claimid = document.all.previousclaims.options[document.all.previousclaims.selectedIndex].value;
        if (claimid != 0)
        {
            window.open("claimViewer.aspx?employeeid=" + employeeid + "&claimid=" + claimid);
        }
    }
</script>	
    <div id="additionalEnvelopeNumbers" runat="server" clientidmode="Static" style="display: none;"></div>    
    <div class="formpanel formpanel_padding">
        <div class="twocolumn"><asp:Label ID="lblprevious" runat="server" Text="Previous&nbsp;Claims:&nbsp;" ></asp:Label><span class="inputs"><asp:Literal id="litprevious" runat="server"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        
        <div class="sectiontitle"><asp:Label ID="lblgeneral" runat="server" Text="General Information" ></asp:Label></div>
        <div class="twocolumn"><asp:Label id="lblname" runat="server" AssociatedControlID="lblclaimname">Claim Name</asp:Label><span class="inputs"><asp:label id="lblclaimname" runat="server">Label</asp:label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label id="lbldatesubmitted" runat="server" AssociatedControlID="lblsubmitted">Date Submitted</asp:Label><span class="inputs"><asp:label id="lblsubmitted" runat="server">Label</asp:label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn"><asp:Label id="lblclaimamount" runat="server" AssociatedControlID="lblamount">Claim Amount</asp:Label><span class="inputs"><asp:label id="lblamount" runat="server">Label</asp:label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label id="lblcurrentstage" runat="server" AssociatedControlID="lblstage">Current Stage</asp:Label><span class="inputs"><asp:label id="lblstage" runat="server">Label</asp:label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn"><asp:Label id="lblclaimantl" runat="server" AssociatedControlID="lblclaimant">Claimant</asp:Label><span class="inputs"><asp:label id="lblclaimant" runat="server">Label</asp:label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn" id="referenceNumberRow" runat="server" ClientIDMode="Static" Visible="False"><asp:Label ID="lblReferenceNumber" runat="server" Text="CR Number" AssociatedControlID="referenceNumber"></asp:Label><span class="inputs"><asp:Label ID="referenceNumber" runat="server"></asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblEnvelopeNumbers" runat="server" Text="Envelope Number" AssociatedControlID="envelopeNumbers"></asp:Label><span class="inputs"><span ID="envelopeNumbers" runat="server"></span>&nbsp;<span id="additionalEnvelopeLink" runat="server" clientidmode="Static"></span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="comment" runat="server" ID="divVerifyComment">
            <div class="commentBody" > This Claim has been sent to you for verification as one or more Expense Items have been amended outside of the acceptable threshold, after this claim has been paid.</div>
            <div class="commentBody" ></div>
        </div>
        <div id="divHistory">
                <div class="sectiontitle">Claim History</div>
                <asp:Literal ID="lithistory" runat="server"></asp:Literal>
       </div>
        <div id="receiptsContainerSingular" runat="server" Visible="False" class="comment claimReceipts">There is a receipt associated with this claim that is not allocated to an expense item, you can <a class="preview" href="{0}">view it here</a>.<br/>Alternatively you can manage all the receipts for this claim, either by clicking one of the receipt icons below, or clicking <a href="/ReceiptManagement.aspx?claimId={0}&returnTo=3" title="Manage this claim's receipts">here</a>.</div>
        <div id="receiptsContainerPlural" runat="server" Visible="False" class="comment claimReceipts">There are {0} receipts associated with this claim that are not allocated to an expense item, you can <a class="preview" href="{1}">view them here</a>.<br/>Alternatively you can manage all the receipts for this claim, either by clicking one of the receipt icons below, or clicking <a href="/ReceiptManagement.aspx?claimId={1}&returnTo=3" title="Manage this claim's receipts">here</a>.</div>       
        <div class="sectiontitle"><asp:Label ID="lblwaitingapproval" runat="server" Text="Items Waiting Approval" ></asp:Label></div>
        <asp:Literal ID="litExpensesGrid" runat="server"></asp:Literal>
        <div class="sectiontitle"><asp:Label ID="lblreturned" runat="server" Text="Returned Items"></asp:Label></div>
        <asp:Literal ID="litReturnedGrid" runat="server"></asp:Literal>
        <div class="sectiontitle"><asp:Label ID="lblapproved" runat="server" Text="Approved Items"></asp:Label></div>
        <asp:Literal ID="litApprovedGrid" runat="server"></asp:Literal>
    </div>
	
	
	
    
	
    <asp:Panel ID="pnldeclaration" runat="server" CssClass="modalpanel">
    <div class="inputpanel">
        <div class="inputpaneltitle">Declaration</div>
        <table width="400"><tr><td><asp:Literal ID="litdeclaration" runat="server"></asp:Literal></td></tr></table>
    </div>
    <div class="inputpanel" style="text-align:right;">
    <asp:LinkButton ID="lnkacceptdeclaration" runat="server">I Accept</asp:LinkButton>&nbsp;&nbsp;<asp:LinkButton ID="lnkcanceldeclaration" runat="server">I Decline</asp:LinkButton>
    </div>
    </asp:Panel>
    <cc1:ModalPopupExtender OnOkScript="SEL.Claims.CheckExpenseList.DeclarationAgreed();" OkControlID="lnkacceptdeclaration" CancelControlID="lnkcanceldeclaration" OnCancelScript="SEL.Claims.CheckExpenseList.DeclineApproverDeclaration();" ID="moddeclaration" runat="server" TargetControlID="lnkdeclaration" PopupControlID="pnldeclaration" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    
    <asp:HyperLink ID="lnkdeclaration" runat="server" style="display:none;">HyperLink</asp:HyperLink>
    
    <asp:Panel runat="server" ID="journeyDetailsContainer"></asp:Panel>

    <asp:panel ID="receiptsContainer" runat="server"></asp:panel>
    <script type="text/javascript">
        // bind (and rebind on partial reload) receipt preview buttons
        function pageLoad() {
            SEL.Receipts.BindAjax(".receiptPreview", ".claimReceipts a.preview");
        };
    </script>
    
    <asp:Panel ID="pnlReturn" runat="server" CssClass="modalpanel">
        
    <div class="formpanel">
        <div class="sectiontitle">Return Expense(s)</div>
        <div class="onecolumn"><asp:Label ID="Label1" runat="server" Text="Reason for returning*" AssociatedControlID="txtReturnReason" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtReturnReason" TextMode="MultiLine" runat="server" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqReturnReason" runat="server" ErrorMessage="Please enter a Reason for returning." ValidationGroup="valReturn" ControlToValidate="txtReturnReason" Text="*"></asp:RequiredFieldValidator></span></div>
        <div class="formbuttons"><helpers:CSSButton id="btnReturnExpense" runat="server" text="OK" ValidationGroup="valReturn" OnClientClick="SEL.Claims.CheckExpenseList.ReturnExpenses();return false;" /><helpers:CSSButton id="btnCanceReturn" runat="server" text="cancel" /></div>
    </div>
    </asp:Panel>
    <cc1:ModalPopupExtender CancelControlID="btnCanceReturn" ID="modReturn" runat="server" TargetControlID="lnkdeclaration" PopupControlID="pnlReturn" BackgroundCssClass="modalBackground">
    </cc1:ModalPopupExtender>
    
    <asp:Panel ID="pnlUnsubmitClaimAsApprover" runat="server" CssClass="modalpanel formpanel" style="display: none;">
        <div class="sectiontitle">Unsubmit Claim</div>
        <div class="onecolumn">
            <asp:Label ID="lblUnsubmitReason" runat="server" Text="Reason for unsubmitting*" AssociatedControlID="txtUnsubmitReason" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox ID="txtUnsubmitReason" TextMode="MultiLine" runat="server" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvUnsubmitReason" runat="server" ErrorMessage="Please enter a Reason for unsubmitting." ValidationGroup="vgUnsubmitReason" ControlToValidate="txtUnsubmitReason" Text="*"></asp:RequiredFieldValidator></span>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton id="btnSaveUnsubmitClaimAsApprover" runat="server" text="save" ValidationGroup="vgUnsubmitReason" OnClientClick="SEL.Claims.CheckExpenseList.UnsubmitClaimAsApprover();return false;" /><helpers:CSSButton id="btnCancelUnsubmitClaimAsApprover" runat="server" text="cancel" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="mdlUnsubmitClaimAsApprover" runat="server" CancelControlID="btnCancelUnsubmitClaimAsApprover" TargetControlID="lnkDummyForModal" PopupControlID="pnlUnsubmitClaimAsApprover" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:HyperLink ID="lnkDummyForModal" runat="server" style="display: none;">HyperLink</asp:HyperLink>

    <asp:Panel ID="pnltransaction" runat="server" BackColor="White">
    <div id="divTransactionDetails"></div>
                </asp:Panel>

                <cc1:PopupControlExtender ID="popuptransaction" runat="server" TargetControlID="lnktransaction" PopupControlID="pnltransaction">
</cc1:PopupControlExtender>
    <asp:LinkButton ID="lnktransaction" runat="server" style="display:none">LinkButton</asp:LinkButton>
            
    
    <asp:Panel ID="pnlValidation" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 750px;">
        <div id="divValidation" style="overflow-x: hidden; overflow-y: auto; min-height:120px; max-height: 800px;"></div>
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdCloseValidation" runat="server" Text="close" OnClientClick="SEL.Claims.ClaimViewer.HideValidationModal();return false;" UseSubmitBehavior="False" TabIndex="0" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modValidation" runat="server" TargetControlID="lnkValidation" PopupControlID="pnlValidation" BackgroundCssClass="modalBackground" CancelControlID="cmdCloseValidation">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkValidation" runat="server" Style="display: none;"></asp:LinkButton>

            <div id="flagSummary">
        
            <div id="divFlags"></div>
         
</div>
    
    <div id="approveClaimPrompt" class="hidden">You have allowed all of the expense items, would you like to approve the claim now?</div>

   <asp:HiddenField ID="hdnMessage" runat="server" ClientIDMode="Static" Value="" />
</asp:Content>

