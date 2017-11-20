<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeKnowledgebaseArticle.aspx.cs" Inherits="Spend_Management.aeKnowledgebaseArticle" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/help.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference InlineScript="false" Path="~/shared/webServices/svcHelp.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        //<![CDATA[
        var ddlArticleProductAreaID = '<% = ddlArticleProductArea.ClientID %>';
        var ddlArticleQueryTypeID = '<% = ddlArticleQueryType.ClientID %>';
        var txtArticleQuestionID = '<% = txtArticleQuestion.ClientID %>';
        var txtArticleAnswerID = '<% = txtArticleAnswer.ClientID %>';
        var kbID = <% = kbID.ToString() %>;
        var nSupportPortalProductID = <% = SupportPortalProductID.ToString() %>;
        //]]>
    </script>
    
    <div class="formpanel">
        <div class="sectiontitle">Knowlegde Base Article</div>
        <div class="onecolumnsmall">
            <asp:Label ID="lblArticleProductArea" runat="server" AssociatedControlID="ddlArticleProductArea">Product Area</asp:Label><span class="inputs"><asp:DropDownList ID="ddlArticleProductArea" runat="server"><asp:ListItem Value="0" Text="[None]"></asp:ListItem></asp:DropDownList></span><span class="inputicons"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="cvProductArea" runat="server" ControlToValidate="ddlArticleProductArea" ValueToCompare="0" Display="Dynamic" Operator="GreaterThan" Type="Integer" Text="*" ErrorMessage="Please choose a product area"></asp:CompareValidator></span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label ID="lblArticleQueryType" runat="server" AssociatedControlID="ddlArticleQueryType">Query Type</asp:Label><span class="inputs"><asp:DropDownList ID="ddlArticleQueryType" runat="server"><asp:ListItem Value="0" Text="[None]"></asp:ListItem></asp:DropDownList></span><span class="inputicons"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator ID="cvQueryType" runat="server" ControlToValidate="ddlArticleQueryType" ValueToCompare="0" Display="Dynamic" Operator="GreaterThan" Type="Integer" Text="*" ErrorMessage="Please choose a query type"></asp:CompareValidator></span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label ID="lblArticleQuestion" runat="server" AssociatedControlID="txtArticleQuestion">Question</asp:Label><span class="inputs"><asp:TextBox ID="txtArticleQuestion" runat="server"></asp:TextBox></span><span class="inputicons"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvArticleQuestion" runat="server" ControlToValidate="txtArticleQuestion" Display="Dynamic" Text="*" ErrorMessage="Please give the article a title"></asp:RequiredFieldValidator></span>
        </div>
        <div class="onecolumnlarge">
            <asp:Label ID="lblArticleAnswer" runat="server" AssociatedControlID="txtArticleAnswerText">Answer Text</asp:Label><span class="inputs"><cc1:HtmlEditorExtender ID="txtArticleAnswer" ClientIDMode="Static" runat="server" TargetControlID="txtArticleAnswerText" EnableSanitization="False"></cc1:HtmlEditorExtender><asp:TextBox runat="server" ID="txtArticleAnswerText" runat="server" TextMode="MultiLine" Columns="60" Rows="15"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="rfvArticleAnswer" runat="server" ControlToValidate="txtArticleAnswerText" Display="Dynamic" Text="*" ErrorMessage="Please give the article an answer"></asp:RequiredFieldValidator></span>
        </div>
        <div class="formbuttons">
            <asp:Image ID="btnSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save Article" onclick="SaveArticle();" />&nbsp;<asp:Image ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" onclick="CancelArticle();" />
        </div>
    </div>
</asp:Content>