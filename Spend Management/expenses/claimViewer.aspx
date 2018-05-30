<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" EnableViewState="true" AutoEventWireup="true" CodeBehind="claimViewer.aspx.cs" Inherits="Spend_Management.expenses.claimViewer" EnableSessionState="ReadOnly" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.claimSelector.js" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.claims.js?date=20171214" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/jquery-selui-dialog.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/expenses/webServices/claims.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:Literal ID="litmenu" runat="server"></asp:Literal>
    <div class="panel" id="divViewFilter">
        <div class="paneltitle">View Filter</div>
        <a class="submenuitem" href="javaScript:SEL.Claims.ClaimViewer.FilterExpenseGrid(0);">All Expenses</a>
        <div id="cashItemLink"><a class="submenuitem" href="javaScript:SEL.Claims.ClaimViewer.FilterExpenseGrid(1);" id="">Cash Expenses</a></div>
        <div id="creditCardItemLink"><a class="submenuitem" href="javaScript:SEL.Claims.ClaimViewer.FilterExpenseGrid(2);">Credit Card Expenses</a></div>
        <div id="purchaseCardItemLink"><a class="submenuitem" href="javaScript:SEL.Claims.ClaimViewer.FilterExpenseGrid(3);">Purchase Card Expenses</a></div>
    </div>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#ctl00_contentmain_cmdback").hide();
            //adding the unique class for removing Jquery buttons and adding the custom buttons
            $('html').addClass('claimView');
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

            if (SEL.ClaimSelector.RootClaimSelector === 'true') {
                var siteLink = $("a[href*='claimsmenu.aspx']").eq(0);
                siteLink.attr('href', siteLink.attr('href').replace('claimsmenu.aspx', 'expenses/claimselector.aspx'));
                siteLink.text(' / Claim Viewer');
            }

            $("body").on("mouseenter", ".passengersinfoicon", function () {
                var passengersInfoComment = $(".passengersinfocomment", $(this).closest("td"));
                $(passengersInfoComment).show().position({ my: "left top", at: "right+10 top", of: this });
            }).on("mouseleave", ".passengersinfoicon", function () {
                var passengersInfoComment = $(".passengersinfocomment", $(this).closest("td"));
                $(passengersInfoComment).hide();
            });

            var fromClaimSelector = eval(SEL.ClaimSelector.GetParameterValues('claimSelector'));
            if (fromClaimSelector === true) {
                $('#ctl00_contentmain_cmdback').show();
            }
        });
       

    </script>
    <div id="additionalEnvelopeNumbers" runat="server" clientidmode="Static" style="display: none;"></div>
    <div class="formpanel formpanel_padding">
            <div id="ScanAttachCompletionNotice" Visible="False" runat="server" class="error-comment" clientidmode="Static">
                Your envelopes have been scanned and attached, but you have expense items below that do not have receipt images attached.
                There are receipt images on the claim header which indicate that matching could not be completed by the Expedite service.
                You need to complete the matching service manually, or declare that all matching is complete, in order for your claim to progress.
                Do this by clicking here: <a href="/ReceiptManagement.aspx?declare=true&returnTo=1&claimid={0}">Receipt Management</a>.
            </div>
            <div id="expenseValidationReturnedNotice" Visible="False" runat="server" class="error-comment" clientidmode="Static">One or more expense items have failed receipt validation and may be declined by your approver. They have been returned to you for correction, and you can view them <a href="#divExpenseGrid">here</a>.</div>
    
        <div class="sectiontitle">General Information</div>

        <div class="twocolumn">
            <asp:Label ID="lblclaimname" runat="server" Text="Claim Name" AssociatedControlID="lblname"></asp:Label><span class="inputs"><asp:Label ID="lblname" runat="server">Label</asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblclaimtotal" runat="server" Text="Claim Total" AssociatedControlID="lbltotal"></asp:Label><span class="inputs"><span id="divClaimTotal"><asp:Label ID="lbltotal" runat="server">Label</asp:Label></span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="onecolumnsmall">
            <asp:Label ID="lblclaimdescription" runat="server" Text="Claim Description" AssociatedControlID="lbldescription"></asp:Label><span class="inputs"><asp:Label ID="lbldescription" runat="server">Label</asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn">
            <asp:Label ID="lblnumitemslbl" runat="server" Text="Number of Items" AssociatedControlID="lblnumitems"></asp:Label><span class="inputs"><span id="divNumberOfItems"><asp:Label ID="lblnumitems" runat="server">Label</asp:Label></span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblAmountPayable" runat="server" Text="Amount Payable" AssociatedControlID="lblAmountPay"></asp:Label><span class="inputs" id="spanAmountPayable"><asp:Label ID="lblAmountPay" runat="server">Label</asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div id="divDateSubmittedRow" style="display: none;" class="twocolumn">
            <asp:Label ID="lblstagelbl" runat="server" Text="Current Stage" AssociatedControlID="lblstage"></asp:Label><span class="inputs"><asp:Label ID="lblstage" runat="server" Text="This claim has not yet been submitted.">Label</asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lbldatesubmitted" runat="server" Text="Date Submitted" AssociatedControlID="lbldate"></asp:Label><span class="inputs"><asp:Label ID="lbldate" runat="server">Label</asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn">
            <asp:Label Visible="false" ID="lblcurrentapprover" runat="server" Text="Current&amp;nbsp;Approver" AssociatedControlID="lblapprover"></asp:Label><span class="inputs" id="spanCurrentApproverInputs" style="display: none;"><asp:Label ID="lblapprover" runat="server">Label</asp:Label></span><span class="inputicon" id="spanCurrentApproverIcon" style="display: none;"></span><span class="inputtooltipfield" id="spanCurrentApproverTooltip" style="display: none;"></span><span class="inputvalidatorfield" id="spanCurrentApproverValidator" style="display: none;"></span><asp:Label ID="lbldatepaid" runat="server" Visible="False" Text="Date Approved" AssociatedControlID="lblpaid"></asp:Label><span class="inputs" id="spanDatePaidInputs" style="display: none"><asp:Label ID="lblpaid" runat="server">Label</asp:Label></span><span class="inputicon" id="spanDatePaidIcon" style="display: none"></span><span class="inputtooltipfield" id="spanDatePaidTooltip" style="display: none;"></span><span class="inputvalidatorfield" id="spanDatePaidValidator" style="display: none;"></span></div>

        <div class="twocolumn" id="referenceNumberRow" runat="server">
            <asp:Label ID="lblReferenceNumber" runat="server" Text="CR Number" AssociatedControlID="referenceNumber"></asp:Label><span class="inputs"><asp:Label ID="referenceNumber" runat="server"></asp:Label></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblEnvelopeNumbers" runat="server" Text="Envelope Number" AssociatedControlID="envelopeNumbers"></asp:Label><span class="inputs"><span ID="envelopeNumbers" runat="server"></span>&nbsp;<span id="additionalEnvelopeLink" runat="server" clientidmode="Static"></span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>

    <div id="EnvelopeAttachInfo" runat="server" clientidmode="Static" class="comment"></div>
                           
        <div id="divHistory">
            <div class="sectiontitle">Claim History</div>
            <asp:Literal ID="lithistory" runat="server"></asp:Literal>
        </div>

    <div id="divEnvelopeMissingNotice" runat="server" clientidmode="Static" class="comment claimEnvelopes">One or more envelopes have not been received for scan & attach within the given timeframe. In order for your claim to progress, you need to update the status of these by clicking <a href="#" onclick="SEL.Claims.ClaimViewer.ShowMissingEnvelopeModal();return false;" title="Update missing envelope statuses">here</a>.</div>
        <div id="receiptsContainerSingular" runat="server" visible="False" class="comment claimReceipts">There is a receipt associated with this claim that is not allocated to an expense item, you can <a class="preview" href="{0}">view it here</a>.<br/>Alternatively you can manage all the receipts for this claim, either by clicking one of the receipt icons below, or clicking <a href="/ReceiptManagement.aspx?claimId={0}&returnTo=1" title="Manage this claim's receipts">here</a>.</div>
        <div id="receiptsContainerPlural" runat="server" visible="False" class="comment claimReceipts">There are {0} receipts associated with this claim that are not allocated to an expense item, you can <a class="preview" href="{1}">view them here</a>.<br/>Alternatively you can manage all the receipts for this claim, either by clicking one of the receipt icons below, or clicking <a href="/ReceiptManagement.aspx?claimId={1}&returnTo=1" title="Manage this claim's receipts">here</a>.</div>
        <div id="expenseReturnNotice" runat="server" Visible="False" class="comment claimReturn">Please Note: Once a returned item has been edited, deleted or disputed your approver will be automatically notified and no further action is required by yourself.</div>
        <div class="sectiontitle">Expense Items</div>
        <div id="divExpenseGrid">
            <asp:Literal ID="litClaimGrid" runat="server"></asp:Literal>
        </div>
        <div id="divCardItems">
            <div class="sectiontitle">Corporate Card Statements</div>

            <div class="onecolumnsmall">
                <asp:Label ID="Label2" runat="server" Text="Statement" AssociatedControlID="ddlstStatements"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstStatements" runat="server"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
            <div id="divCardTransactionGrid">
                <asp:Literal ID="litCardTransactionsGrid" runat="server"></asp:Literal>
            </div>
        </div>
        <div id="divExpenses360Warning" runat="server" Visible="False" clientidmode="Static" class="comment claimExpenses360Warning">We are phasing out Expenses360 so that we can continue to improve, support and add features to Expenses Mobile.
             You will no longer be able to use Expenses360 from 14th July 2018, so please ensure that you have synced and reconciled any outstanding expenses.
            <br/>
            <br/> More information can be found here LINK.
        </div>
        <div class="sectiontitle">My Mobile Items</div>
        <div id="divMobileItems">
            <asp:Literal ID="litmobileitems" runat="server"></asp:Literal>
        </div>

        <div class="sectiontitle">My Mobile Journeys</div>
        <div id="divMobileJourneys">
            <asp:Literal ID="litMobileJourneys" runat="server"></asp:Literal>
         </div>
           <div class="formbuttons">
            <asp:ImageButton ID="cmdback" AlternateText="Back" runat="server" ImageUrl="~/shared/images/buttons/btn_back.png" meta:resourcekey="cmdbackResource1" OnClick="cmdback_Click"></asp:ImageButton>
        </div>    

    </div>

    <asp:Panel ID="pnltransaction" runat="server" BackColor="White">
        <div id="divTransactionDetails"></div>
    </asp:Panel>

    <cc1:PopupControlExtender ID="popuptransaction" runat="server" TargetControlID="lnktransaction" PopupControlID="pnltransaction">
    </cc1:PopupControlExtender>
    <asp:LinkButton ID="lnktransaction" runat="server" Style="display: none">LinkButton</asp:LinkButton>

    <asp:Panel ID="pnlmatch" runat="server" CssClass="modalpanel">

        <div class="formpanel">
            <div class="sectiontitle">Match item</div>
            <div style="max-height: 500px; overflow: auto" ID="litMatchTransationGrid"></div>
           <div class="formbuttons"><a href="javaScript:SEL.Claims.ClaimViewer.MatchTransaction();"><img src="../shared/images/buttons/btn_save.png"/></a>&nbsp;&nbsp;<asp:ImageButton ID="cmdallocatecancel" ImageUrl="~/shared/images/buttons/cancel_up.gif" runat="server" /></div>
        </div>

    </asp:Panel>
    <cc1:ModalPopupExtender ID="modUnallocated" runat="server" TargetControlID="lnkunallocated" PopupControlID="pnlmatch" BackgroundCssClass="modalBackground" CancelControlID="cmdallocatecancel">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkunallocated" runat="server" Style="display: none;"></asp:LinkButton>
    <asp:Panel runat="server" ID="journeyDetailsContainer"></asp:Panel>


    <div id="submitClaim">
        <div class="sm_panel">
            <div id="flagWarning" class="flagTitle comment"></div>
            <div class="sectiontitle">Claim Summary</div>
            <div class="twocolumn"><asp:Label ID="Label1" runat="server" CssClass="mandatory" Text="Claim name*" AssociatedControlID="txtClaimName"></asp:Label><span class="inputs"><asp:TextBox ID="txtClaimName" runat="server" MaxLength="50"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqclaim" runat="server" ErrorMessage="Please enter a Claim name." ControlToValidate="txtClaimName" Text="*" ValidationGroup="vgSubmit"></asp:RequiredFieldValidator></span></div>
            <div class="onecolumn"><asp:Label ID="Label3" runat="server" Text="Description" AssociatedControlID="txtClaimDescription"></asp:Label><span class="inputs"><asp:TextBox id="txtClaimDescription" runat="server" TextMode="MultiLine" MaxLength="10" textareamaxlength="2000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
            <div id="claimApprover"></div>
            <div id="partSubmit"></div>
            <div id="divOdometerReadings">
                <div class="sectiontitle">Odometer Readings</div>
                <div class="comment" id="divFuelCardMileage">Please note that the value of your business mileage and personal mileage will now be calculated and displayed correctly once you’ve entered your closing odometer reading and submitted your claim.  The value of your business mileage will be shown for information only and the value of your personal mileage will be deducted from the amount payable, reducing your claim.</div>
                <table id="odometerReadings">
                    <tr>
                        <th colspan="2" class="car">Vehicle</th>
                        <th style="width: 170px;">Last reading date</th>
                        <th style="width: 120px;">Last reading</th>
                        <th style="width: 170px;">New reading</th>
                        <th style="width: 20px;"></th>
                        <th style="width: 20px;"></th>
                    </tr>
                </table>
                <div id="divBusinessMileage" style="display:none;"><asp:Label CssClass="odometerReadingsBusinessMileage" ID="Label4" runat="server" Text="Have you incurred business mileage since your last reading?*&nbsp;&nbsp;"></asp:Label><span class="inputs" style="vertical-align: middle;"><asp:DropDownList ID="ddlstBusinessMileage" runat="server"><asp:ListItem Value="0" Text="[None]" /><asp:ListItem Value="2" Text="Yes" /><asp:ListItem Value="2" Text="No" /></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="reqBusinessMileage" ControlToValidate="ddlstBusinessMileage" runat="server" ErrorMessage="Please declare if you have incurred business mileage since your last reading." Type="Integer" ValueToCompare="0" Operator="GreaterThan" Text="*" ValidationGroup="vgSubmit"></asp:CompareValidator></span></div>
            </div>
          
        </div>
    </div>
    <div id="declaration">
        
            <div id="declarationText"></div>
    </div>
    <div id="flagSummary">
        
        <div id="divFlags"></div>
</div>
    
    <div id="approveronHoliday">
        
                <div>This approver is currently set as “on holiday”, this may mean that your claim will not be processed until they return. Alternatively, you may cancel and change your selected approver.<br/><br/>Would you like continue and send your claim to this person anyway?</div>
        
    </div>
    
    <div id="envelopeInfo" clientidmode="Static" runat="server">
        <div id="loadingContainer" class="offPage" style="display:none;"><img src="<% = GetStaticLibraryPath() %>/icons/Custom/128/ajax-loader-grey-128.gif" alt="Loading..."/></div>        
        <div class="receiptProcessBox" id="sendReceiptBox" clientidmode="Static">
            <img class="receiptProcessImg" src="<% = GetStaticLibraryPath() %>/icons/128/plain/mail2.png" alt="Send receipts by mail"/>
            <span class="receiptProcessHeader">Send receipts by post</span>
            <span class="receiptProcessText">Select this option to send the receipts for this claim to the Selenity Expedite Service Centre.</span>
        </div>
        <div class="receiptProcessBox" id="alreadyAttachedBox" clientidmode="Static">
            <img class="receiptProcessImg" src="<% = GetStaticLibraryPath() %>/icons/128/plain/document_attachment.png" alt="Receipts already attached"/>
            <span class="receiptProcessHeader">No need to send receipts</span>
            <span class="receiptProcessText">Select this option if you do not need to send receipts to Selenity.</span>
        </div>

        <div class="receiptProcessBox" id="envelopeNumberBox" clientidmode="Static">
            <img class="receiptProcessImg" id="envelopeNumberImage" src="<% = GetStaticLibraryPath() %>/icons/128/plain/mail2_view.png" alt="Envelope Number"/>
            <span class="receiptProcessHeader">Envelope Number</span>
            <span class="receiptProcessText">Please enter the envelope numbers for <b>all</b> of the envelopes used for this claim. This can be found on the back of the envelope and should be in the format A-ABC-123</span>
            <span class="receiptProcessInputContainer"><input class="receiptProcessInput" maxlength="9"/><span class="deleteEnvelopeNumber"><img src="<% = GetStaticLibraryPath() %>/icons/custom/16/delete.png" alt="Delete this envelope number" title="Delete this envelope number"/></span></span>
        </div>
    
        <div class="receiptProcessBox" id="referenceNumberBox" clientidmode="Static">
            <img class="receiptProcessImg" id="referenceNumberImage" src="<% = GetStaticLibraryPath() %>/icons/128/plain/mail_write.png" alt="Reference number"/>
            <span class="receiptProcessHeader">CR Number</span>
            <span class="receiptProcessText"><span>Write this CR number on all envelopes used for this claim.<p>Please note, without this number, Selenity may not be able to process your receipts.</p></span></span>            
            <span><span id="referenceNumber"></span></span>
        </div>

        <div id="buttonContainer"></div>
    </div>
    <div ID="submitClaimInfo" runat="server" ClientIDMode="Static" style="display: none" />

    <div id="dialog-confirm" title="Receipts Numbered?" style="display: none">
        <div id="confirmationMessage">I confirm I have numbered all my receipts using the Expense Item Reference Number found on my claim summary and removed all staples from my receipts</div>
        <div id="confirmationButtons"><span id="confirmButton"><span class="buttonContainer"><input class="buttonInner" value="confirm" type="button" style="width: 65px"/></span></span><span id="cancelButton"><span class="buttonContainer"><input class="buttonInner" value="cancel" type="button" style="width: 65px"/></span></span></div>
    </div>
<asp:Panel ID="pnlValidation" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 600px;">
        <div id="divValidation" style="overflow-x: hidden; overflow-y: auto; min-height:120px; max-height: 800px;"></div>
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdCloseValidation" runat="server" Text="close" OnClientClick="SEL.Claims.ClaimViewer.HideValidationModal();return false;" UseSubmitBehavior="False" TabIndex="0" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modValidation" runat="server" TargetControlID="lnkValidation" PopupControlID="pnlValidation" BackgroundCssClass="modalBackground" CancelControlID="cmdCloseValidation">
    </cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkValidation" runat="server" Style="display: none;"></asp:LinkButton>

    <asp:Panel ID="pnlEnvelopeMissing" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 750px">
        <div class="sectiontitle">Missing Envelopes</div>
        <div id="divEnvelopeMissing" clientidmode="Static" style="overflow: auto; min-height:200px; max-height: 500px">
            <p>The envelope(s) below have been recorded as sent, however we have not received them within the timeframe set up for your company, which is <%= DaysToWaitUntilEnvelopeCanBeDeclaredMissing %> days. For each envelope, can you please confirm whether it has been sent (Presumed Lost), or that you have yet to send it (Posted Late).</p>
            <p><strong>Posted late</strong> - date of posting will be reset to today's date and we will wait a further <%= DaysToWaitUntilEnvelopeCanBeDeclaredMissing %> days for your envelope.
            <br/><strong>Presume Lost</strong> - envelopes containing receipts will be declared lost and may impact the authorisation of your claim's payment.</p>
            <br/>
            <table id="ulMissingEnvelopeSwitches" clientidmode="Static"></table>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdCloseEnvelopeMissingConfirmed" runat="server" Text="save" OnClientClick="SEL.Claims.ClaimViewer.HideEnvelopeMissingModal(true);return false;" UseSubmitBehavior="False" TabIndex="0" />
            <helpers:CSSButton ID="cmdCloseEnvelopeMissingNotSent" runat="server" Text="cancel" OnClientClick="SEL.Claims.ClaimViewer.HideEnvelopeMissingModal(false);return false;" UseSubmitBehavior="False" TabIndex="0" />
        </div>
</asp:Panel>
    <cc1:ModalPopupExtender ID="modEnvelopeMissing" runat="server" TargetControlID="cmdCloseEnvelopeMissingConfirmed" PopupControlID="pnlEnvelopeMissing" BackgroundCssClass="modalBackground" CancelControlID="cmdCloseEnvelopeMissingNotSent">
</cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkEnvelopeMissingConfirm" runat="server" Style="display: none;"></asp:LinkButton>
    <asp:panel ID="receiptsContainer" runat="server"></asp:panel>
    <script type="text/javascript">
        // bind receipt preview buttons
        SEL.Receipts.BindAjax(".receiptPreview", ".claimReceipts a.preview");
    </script>
   
</asp:Content>
