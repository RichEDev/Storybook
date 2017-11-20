<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="helpAndSupportTicketNew.aspx.cs" Inherits="Spend_Management.helpAndSupportTicketNew" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ContentPlaceHolderID="contentmenu" runat="server">
    <div id="divPageMenu">
        <a href="/shared/helpAndSupport.aspx">Online Help</a>
        <a href="/shared/helpAndSupportTickets.aspx">My Tickets</a>
        <a id="circleLink" runat="server" href="http://circle.software-europe.com" target="_blank" title="Access our product support portal Circle">Visit Circle</a>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">
       
    <asp:ScriptManagerProxy ID="scriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Name="knowledge" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            SEL.HelpAndSupportTickets.Setup();
        });
    </script>
    
    <div id="ticketForm" class="formpanel formpanel_padding ticketForm" runat="server">
        <div class="sectiontitle">Create New Ticket</div>
        <div class="comment" runat="server" ID="internalWelcomeMessage" style="margin-bottom: 4px;">You can contact a system administrator directly by raising a support ticket, please complete the form below to continue.</div>
        <div class="comment" runat="server" ID="selWelcomeMessage" style="margin-bottom: 4px;">Selenity can help, you can contact them directly by raising a support ticket.</div>
        <div class="errortext" id="divErrorMessage" runat="server" Visible="False"></div>
        <div class="onecolumnsmall" runat="server">
            <asp:Label id="lblSubject" runat="server" AssociatedControlID="txtSubject" CssClass="mandatory">Subject*</asp:Label><span class="inputs"><asp:TextBox id="txtSubject" runat="server" MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvSubject" runat="server" ErrorMessage="Please enter a Subject." ControlToValidate="txtSubject" Text="*"></asp:RequiredFieldValidator></span>
        </div>
        <div class="onecolumnsmall" runat="server">
            <asp:Label id="lblAttachment" runat="server" AssociatedControlID="uplAttachment">Attachment</asp:Label><span class="inputs"><asp:FileUpload id="uplAttachment" runat="server" CssClass="fillspan"></asp:FileUpload></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="onecolumn" runat="server">
            <asp:Label id="lblDescription" runat="server" AssociatedControlID="txtDescription" CssClass="mandatory">Description*</asp:Label><span class="inputs"><asp:TextBox id="txtDescription" runat="server" CssClass="fillspan" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="Please enter a Description." ControlToValidate="txtDescription" Text="*"></asp:RequiredFieldValidator></span>
        </div>

        <div class="onecolumnsmall" runat="server" id="selDisclaimer">
            <span class="inputs checkboxAgreement"><asp:CheckBox id="chkDisclaimer" runat="server"></asp:CheckBox><div><label><strong>Notice:</strong> The information I have provided does not contain any sensitive information and I understand that it will be recorded on Selenity's ticketing system which is hosted in a safe harbour outside of the EEA.</label></div></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CustomValidator runat="server" EnableClientScript="true" ClientValidationFunction="SEL.HelpAndSupportTickets.Page.Validators.Disclaimer" ErrorMessage="You must agree to the notice by checking the box to proceed.">*</asp:CustomValidator></span>
        </div>
        
        <asp:HiddenField runat="server" ID="hdnSearchTerm" />
        
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="cmdSubmit" OnClick="CmdSubmit_Click" UseSubmitBehavior="true" Text="submit" />
        <helpers:CSSButton ID="cmdCancel" OnClick="CmdClose_Click" runat="server" Text="cancel" UseSubmitBehavior="False" CausesValidation="False"></helpers:CSSButton>
        </div>   
    </div>

    <div class="formpanel formpanel_padding" runat="server" id="customerHelpText">
        <div class="sectiontitle" id="customercontactandadvice">Customer Contact and Advice</div>
        <div class="comment" runat="server" style="margin-bottom: 4px;">You can contact your administrator using the contact details provided below.</div>
        <div class="onecolumnpanel">
            <asp:Literal ID="litCustomerHelpText" runat="server"></asp:Literal>
        </div>                   
    </div>
    
    <div class="formpanel formpanel_padding" runat="server" id="successMessage" Visible="False"> 
        <div class="sectiontitle">Ticket Created</div>
        <div class="comment" runat="server" id="successCommentInternal" Visible="False" style="margin-bottom: 4px;">Your support ticket has been created and your administrator has been notified, you can view the progress of your ticket by visiting <a href="/shared/helpAndSupportTickets.aspx">My Tickets</a>.</div>
        <div class="comment" runat="server" id="successCommentSel" Visible="False" style="margin-bottom: 4px;">Your support ticket has been created and Selenity have been notified, you can view the progress of your ticket by visiting <a href="/shared/helpAndSupportTickets.aspx">My Tickets</a>.</div>
    </div>

    <div class="formbuttons">
        <helpers:CSSButton ID="cmdClose" OnClick="CmdClose_Click" runat="server" Text="close" UseSubmitBehavior="False" CausesValidation="False"></helpers:CSSButton>
    </div>
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="scripts" ID="customScripts">
    <script type="text/javascript">
        $(document).ready(function () {
            var width = $(window).width(), height = $(window).height();
            if ((width <= 1024) && (height <= 768)) {
                $('.formpanel .formbuttons').attr('style', 'padding-bottom:0px; margin-top:47px;');
                $('.formpanel .inputs.checkboxAgreement input[type="checkbox"]').css('margin-left', '130px');
                $('.formpanel .inputs.checkboxAgreement div').attr('style', 'width:346px; padding-left: 145px; margin-top:-17px;');
                $('body').css('display','inline');
            }
        });
    </script>
</asp:Content>