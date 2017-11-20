<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="helpAndSupportTicket.aspx.cs" Inherits="Spend_Management.helpAndSupportTicket" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>


<asp:Content ID="Content3" ContentPlaceHolderID="styles" runat="server">
  
    <!--[if IE 7]>
        <style>
            
        .formpanel {
            display: inline-block;
        }

        .formpanel .twocolumn .inputs {
            display: inline-block;
        }
  
        </style>
   <![endif]-->
    <style>
        .buttonContainer {
            margin-bottom: 10px;
        }

        .formpanel .onecolumn label {
            width: 170px;
        }

        #ctl00_contentmain_uplAttachment {
            width: 174px !important;
        }
    </style>
</asp:Content>

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

            SEL.HelpAndSupportTickets.Setup("<%=this.TicketType%>");
        });
    </script>
    
    <div id="ticketDetails" class="formpanel ticketDetails formpanel_padding">
        <div class="sectiontitle">Ticket Details</div>
        <!--<div class="errortext" id="divErrorMessage" runat="server" Visible="False"></div>-->
        <div class="twocolumn" runat="server">
            <asp:Label id="lblSupportTicketId" runat="server" AssociatedControlID="litSupportTicketId">Ticket number</asp:Label><span class="inputs"><asp:Literal runat="server" ID="litSupportTicketId"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <asp:Label id="lblStatus" runat="server" AssociatedControlID="litStatus">Status</asp:Label><span class="inputs" style="width: 200px;"><asp:Literal runat="server" ID="litStatus"></asp:Literal></span>
        </div>
        <div class="twocolumn" runat="server">
            <asp:Label id="lblOpenedOn" runat="server" AssociatedControlID="litOpenedOn">Opened on</asp:Label><span class="inputs"><asp:Literal runat="server" ID="litOpenedOn"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <asp:Label id="lblUpdatedOn" runat="server" AssociatedControlID="litUpdatedOn">Updated on</asp:Label><span class="inputs"><asp:Literal runat="server" ID="litUpdatedOn"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="onecolumnsmall" runat="server">
            <asp:Label id="lblSubject" runat="server" AssociatedControlID="litSubject">Subject</asp:Label><span class="inputs readonly"><asp:Literal runat="server" ID="litSubject"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="onecolumnsmall" runat="server">
            <asp:Label id="lblDescription" runat="server" AssociatedControlID="litDescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs readonly"><asp:Literal runat="server" ID="litDescription"></asp:Literal></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
       
        <helpers:CSSButton runat="server" CausesValidation="False" ID="cmdCloseTicket" OnClick="CmdCloseTicket_Click" UseSubmitBehavior="True" Text="close ticket" />
        <helpers:CSSButton runat="server" CausesValidation="False" ID="cmdConvertTicket" OnClick="CmdConvertTicket_Click" UseSubmitBehavior="True" Text="send to administrator" />
    </div>
    
    <div id="ticketComments" class="formpanel ticketComments formpanel_padding">
        <div class="sectiontitle">Comments</div>
        <ul runat="server">
        <asp:Repeater runat="server" ID="TicketCommentsRepeater">
            <ItemTemplate>
                <li>
                    <div class="commentTitle"><span class="commentContact"><%#Eval("ContactName")%></span> (<%#Eval("CreatedOn", "{0:dd/MM/yyyy HH:mm}")%>)</div>
                    <p class="commentBody"><%#Eval("Body")%></p>
                </li>
            </ItemTemplate>
        </asp:Repeater>
        </ul><div class="onecolumnsmall" runat="server">
        <span runat="server" id="spnTicketCommentsEmpty" Visible="False">No comments to display</span></div>
    </div>

    <div id="ticketCommentForm" class="formpanel ticketCommentForm formpanel_padding">
        <div class="sectiontitle">Add a Comment</div>
        <div class="onecolumn" runat="server">
            <asp:Label id="lblComment" runat="server" CssClass="mandatory" AssociatedControlID="txtComment"><p class="labeldescription">Comment*</p></asp:Label><span class="inputs"><asp:TextBox id="txtComment" runat="server" CssClass="fillspan" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvComment" runat="server" ErrorMessage="Please enter a Comment." ControlToValidate="txtComment" Text="*"></asp:RequiredFieldValidator></span>
        </div>

        <helpers:CSSButton runat="server" ID="cmdSubmitComment" OnClick="CmdSubmitComment_Click" UseSubmitBehavior="true" Text="send comment" />
    </div>
    
    <div id="ticketAttachments" class="formpanel ticketAttachments formpanel_padding">
        <div class="sectiontitle">Attachments</div>
        <ul runat="server">
        <asp:Repeater runat="server" ID="TicketAttachmentsRepeater">
            <ItemTemplate>
                <li>
                    <a href="#download-attachment" data-attachment="<%#Eval("Id")%>" data-ticket="<%=SupportTicketId %>"><%#Eval("Filename")%></a> (<%#Math.Round(Convert.ToDouble(Eval("FileSize")) / Math.Pow(1024, 2), 2)%>MB)
                </li>
            </ItemTemplate>
        </asp:Repeater>
        </ul><div class="onecolumnsmall">
        <span runat="server" id="spnTicketAttachmentsEmpty" Visible="False">No attachments to display</span></div>
    </div>

    <div id="ticketAttachmentForm" class="formpanel ticketAttachmentForm formpanel_padding">
        <div class="sectiontitle">Add an Attachment</div>
        <div class="onecolumnsmall">
            <asp:Label id="lblAttachment" runat="server" AssociatedControlID="uplAttachment">Attachment</asp:Label><span class="inputs"><asp:FileUpload id="uplAttachment" runat="server" CssClass="fillspan"></asp:FileUpload></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>

        <helpers:CSSButton runat="server" ID="cmdSubmitAttachment" OnClick="CmdSubmitAttachment_Click" UseSubmitBehavior="true" CausesValidation="False" Text="send attachment" />
    </div>

    <div class="formpanel formpanel_padding">
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdCancel" OnClick="CmdCancel_Click" runat="server" Text="cancel" UseSubmitBehavior="true" CausesValidation="False"></helpers:CSSButton>
        </div>   
    </div>
    
</asp:Content>
