<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smForm.master" CodeBehind="adminHelpAndSupportTicket.aspx.cs" Inherits="Spend_Management.adminHelpAndSupportTicket" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="styles" runat="server">
  <style type="text/css">
       .buttonContainer{
                margin-top: 10px;
            }
      
  </style>
 
</asp:Content>

<asp:Content ContentPlaceHolderID="contentmenu" runat="server">
    <!--<div id="divPageMenu">
        <a href="/shared/helpAndSupport.aspx">Online Help</a>
        <a href="/shared/helpAndSupportTickets.aspx">My Tickets</a>
    </div>-->
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">
       
    <asp:ScriptManagerProxy ID="scriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Name="knowledge" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {

            SEL.HelpAndSupportTickets.Setup("Internal");
        });
    </script>
    
    <div id="ticketDetails" class="formpanel ticketDetails formpanel_padding">
        <div class="sectiontitle">Ticket Details</div>
        <!--<div class="errortext" id="divErrorMessage" runat="server" Visible="False"></div>-->
        <div class="twocolumn" runat="server">
            <asp:Label id="lblSupportTicketId" runat="server" AssociatedControlID="litSupportTicketId">Ticket number</asp:Label><span class="inputs"><asp:Literal runat="server" ID="litSupportTicketId"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <asp:Label id="lblStatus" runat="server" AssociatedControlID="ddlStatus">Status</asp:Label><span class="inputs" style="width: 200px;"><asp:DropDownList runat="server" ID="ddlStatus"></asp:DropDownList></span>
        </div>
        <div class="twocolumn" runat="server">
            <asp:Label id="lblOpenedOn" runat="server" AssociatedControlID="litOpenedOn">Opened on</asp:Label><span class="inputs"><asp:Literal runat="server" ID="litOpenedOn"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <asp:Label id="lblUpdatedOn" runat="server" AssociatedControlID="litUpdatedOn">Updated on</asp:Label><span class="inputs"><asp:Literal runat="server" ID="litUpdatedOn"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="onecolumn" runat="server">
            <asp:Label id="lblSubject" runat="server" AssociatedControlID="litSubject">Subject</asp:Label><span class="inputs readonly"><asp:Literal runat="server" ID="litSubject"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="onecolumn" runat="server">
            <asp:Label id="lblDescription" runat="server" AssociatedControlID="litDescription">Description</asp:Label><span class="inputs readonly"><asp:Literal runat="server" ID="litDescription"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
    </div>
    
    <div id="ticketComments" class="formpanel ticketComments formpanel_padding">
        <div class="sectiontitle">Comments1</div>
        <ul runat="server">
        <asp:Repeater runat="server" ID="TicketCommentsRepeater">
            <ItemTemplate>
                <li>
                    <div class="commentTitle"><span class="commentContact"><%#Eval("ContactName")%></span> (<%#Eval("CreatedOn", "{0:dd/MM/yyyy HH:mm}")%>)</div>
                    <p class="commentBody"><%#Eval("Body")%></p>
                </li>
            </ItemTemplate>
        </asp:Repeater>
        </ul>
        <span runat="server" id="spnTicketCommentsEmpty" Visible="False">No comments to display</span>
    </div>

    <div id="ticketCommentForm" class="formpanel ticketCommentForm formpanel_padding" style="margin-top:50px;">
        <div class="sectiontitle">Add a Comment</div>
        <div class="onecolumn" runat="server">
            <asp:Label id="lblComment" runat="server" CssClass="mandatory" AssociatedControlID="txtComment"><div class="labeldescription">Comment*</div></asp:Label><span class="inputs"><asp:TextBox id="txtComment" runat="server" CssClass="fillspan" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvComment" runat="server" ErrorMessage="Please enter a Comment." ControlToValidate="txtComment" Text="*"></asp:RequiredFieldValidator></span>
        </div>

        <helpers:CSSButton runat="server" ID="cmdSubmitComment" OnClick="CmdSubmitComment_Click" UseSubmitBehavior="true" Text="send comment" />
    </div>
    
    <div id="ticketAttachments" class="formpanel ticketAttachments formpanel_padding" style="margin-top:50px;">
        <div class="sectiontitle">Attachments</div>
        <ul runat="server">
        <asp:Repeater runat="server" ID="TicketAttachmentsRepeater">
            <ItemTemplate>
                <li>
                    <a href="#download-attachment" data-attachment="<%#Eval("Id")%>" data-ticket="<%=SupportTicketId %>"><%#Eval("Filename")%></a> (<%#Math.Round(Convert.ToDouble(Eval("FileSize")) / Math.Pow(1024, 2), 2)%>MB)
                </li>
            </ItemTemplate>
        </asp:Repeater>
        </ul>
        <span runat="server" id="spnTicketAttachmentsEmpty" Visible="False">No attachments to display</span>
    </div>

    <div class="formpanel formpanel_padding">
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdSave" OnClick="CmdSubmit_Click" runat="server" Text="save" UseSubmitBehavior="true" CausesValidation="False"></helpers:CSSButton>
            <helpers:CSSButton ID="cmdCancel" OnClick="CmdCancel_Click" runat="server" Text="cancel" UseSubmitBehavior="true" CausesValidation="False"></helpers:CSSButton>
        </div>   
    </div>
    
</asp:Content>
