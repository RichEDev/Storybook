<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="helpAndSupport.aspx.cs" Inherits="Spend_Management.helpAndSupport" %>
<%@ Import Namespace="System.Data" %>
<%@ Import namespace="SpendManagementLibrary.Enumerators" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ContentPlaceHolderID="contentmenu" runat="server">
    <div id="divPageMenu">
        <a class="selectedPage" href="/shared/helpAndSupport.aspx">Online Help</a>
        <a id="ticketsLink" class="submenuitem" runat="server" href="/shared/helpAndSupportTickets.aspx">My Tickets</a>
        <a id="circleLink" runat="server" href="http://circle.software-europe.com" target="_blank" title="Access our product support portal Circle">Visit Circle</a>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">
       
    <style type="text/css">
        /* Some print-specific CSS, hides the entire page with the exception of the modal containing the knowledge article */
        @media print {
            #aspnetForm {
                visibility: hidden;
            }

            body {
                background: none;
            }

            div.viewArticle {
                width: 100% !important;
                top: 0 !important;
                left: 0 !important;
            }

            div.viewArticle .modalpanel {
                border: none;
            }

            div.viewArticle div.articleTitle {
                font-size: 16pt;
                margin-bottom: 10px;
            }

            div.viewArticle .formbuttons {
                display: none;
            }
        }

    </style>

    <asp:ScriptManagerProxy ID="scriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Name="knowledge" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            (function (a) {
                a.Panel = "<%= this.pnlViewArticle.ClientID %>";
                a.Modal = "<%= this.mdlViewArticle.ClientID %>";

            }(SEL.Knowledge.Dom.ViewArticle));

            SEL.Knowledge.ViewArticle.Setup();
        });
    </script>
    
    <asp:Panel runat="server" ID="pnlViewArticle" CssClass="viewArticle" style="display: none; height: 575px;">
        <div class="modalpanel">
            <div class="formpanel">
                <div class="articleTitle"></div>
                <div class="articleBody"></div>
                <div class="formbuttons">
                    <helpers:CSSButton runat="server" ID="btnCloseViewArticle" Text="close" OnClientClick="SEL.Knowledge.ViewArticle.Close(); return false;" UseSubmitBehavior="False" />
                    <a class="email" href="" title="Send an e-mail containing a link to this article"><img src="/static/icons/24/plain/mail.png" alt="Email button" /></a>
                    <a class="print" href="javascript:window.print();" title="Print this article"><img src="/static/icons/24/plain/printer2.png" alt="Print button" /></a>
                    <a class="popup" href="" target="_blank" title="Open this article in a separate window"><img src="/static/icons/24/plain/window_new.png" alt="Popup button" /></a>
                </div>
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="mdlViewArticle" BackgroundCssClass="modalBackground" TargetControlID="lnkViewArticle" PopupControlID="pnlViewArticle"></cc1:ModalPopupExtender>
    <asp:HyperLink runat="server" ID="lnkViewArticle" style="display: none;"></asp:HyperLink>
    
    <asp:Panel runat="server" CssClass="formpanel helpAndSupport formpanel_padding" DefaultButton="cmdSearch">
           
        <div class="welcome">
            <div class="searchForm">
                <asp:TextBox id="txtSearchTerm" runat="server" CssClass="fillspan searchQuery"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSearchTerm" Display="None" ErrorMessage="Please enter a search term."></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtSearchTerm" Display="None" ErrorMessage="Your search term must contain at least 3 alpha-numeric characters." ValidationExpression="^([^\w]*?[\w]+[^\w]*?){3,}$"></asp:RegularExpressionValidator>
                <helpers:CSSButton runat="server" ID="cmdSearch" OnClick="CmdSearch_Click" PostBackUrl="helpAndSupport.aspx" UseSubmitBehavior="true" Text="search" />
            </div>

            <div id="noSearchResults" class="noResults" runat="server" Visible="False">
                No Help &amp; Support articles found, please try a different search term or choose from the list below.
            </div>
        </div>

        <div id="searchResults" class="searchResults" Visible="False" runat="server">
            <ul>
            <asp:Repeater runat="server" ID="knowledgeArticlesRepeater" OnItemDataBound="ArticleViewItemDataBound">
            <ItemTemplate>
                <li data-identifier="<%#Eval("Identifier")%>">
                    <asp:Panel runat="server" ID="searchResult" CssClass="searchResult">
                        <asp:HyperLink runat="server" ID="searchResultAnchor" CssClass="title" ToolTip="View Article"><%#Eval("Title")%></asp:HyperLink>
                        <div class="publishedOn"><%#Eval("PublishedOn", "{0:MMMM dd, yyyy}")%></div>
                        <p class="summary"><%#Eval("Summary")%></p>
                    </asp:Panel>
                </li>
            </ItemTemplate>
            </asp:Repeater>
            </ul>
        </div>
        
        <div id="questions" class="helpAndSupportQuestions" runat="server">
            <ul>
                <asp:Repeater runat="server" ID="questionHeadingsRepeater">
                <ItemTemplate>
                <li>
                    <span><%# DataBinder.Eval(Container.DataItem, "Heading") %></span>
                    <ul>
                        <asp:Repeater runat="server" ID="questionsRepeater" OnItemDataBound="QuestionDataBound" DataSource='<%# ((DataRowView)Container.DataItem).Row.GetChildRows("HeadingQuestionGroup") %>'>
                        <ItemTemplate>
                        <li>
                        <asp:MultiView ID="questionMultiView" runat="server" ActiveViewIndex="0">
                            <!-- link to knowledge article -->
                            <asp:View ID="View1" runat="server">
                                <a class="knowledgeUrl" href="<%#DataBinder.Eval(Container.DataItem, "[\"KnowledgeArticleUrl\"]") %>" target="_blank">
                                    <%# DataBinder.Eval(Container.DataItem, "[\"Question\"]") %>
                                </a>
                            </asp:View>
                            <!-- link to new SEL ticket -->
                            <asp:View ID="View2" runat="server">
                                <a class="selTicket" href="/shared/helpAndSupportTicketNew.aspx?TicketType=<%=(int)SupportTicketType.SalesForce%>&Subject=<%# HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "[\"Question\"]").ToString()) %>">
                                    <%# DataBinder.Eval(Container.DataItem, "[\"Question\"]") %>
                                </a>
                            </asp:View>
                            <!-- link to new internal ticket -->
                            <asp:View ID="View3" runat="server">
                                <a class="internalTicket" href="/shared/helpAndSupportTicketNew.aspx?TicketType=<%=(int)SupportTicketType.Internal%>&Subject=<%# HttpUtility.UrlEncode(DataBinder.Eval(Container.DataItem, "[\"Question\"]").ToString()) %>&CustomEntityId=<%# DataBinder.Eval(Container.DataItem, "[\"CustomEntityId\"]") %>">
                                    <%# DataBinder.Eval(Container.DataItem, "[\"Question\"]") %>
                                </a>
                            </asp:View>
                        </asp:MultiView>                            
                        </li>
                        </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </li>
                </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        
        <div class="knowledgeRedirectMessage">We have an article about this topic on our support site "Knowledge", <a href="#">click here to view it</a> (opens in a new window).</div>
        <div class="ticketRedirectMessage">For us to be able to assist you, <a href="#">please raise a support ticket</a>.</div>

    </asp:Panel>

</asp:Content>
