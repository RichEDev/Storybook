<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="KnowledgeCustomArticles.aspx.cs" Inherits="Spend_Management.shared.admin.KnowledgeCustomArticles" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=4.1.7.123, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:HyperLink runat="server" ID="lnkNewArticle" CssClass="submenuitem" NavigateUrl="javascript:SEL.Knowledge.CustomArticle.New();">New Knowledge Article</asp:HyperLink>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    
    <asp:ScriptManagerProxy ID="scriptManager" runat="server">
        <Scripts>
            <asp:ScriptReference Name="knowledge" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript">
        $(document).ready(function()
        {
            (function(a)
            {
                a.PanelTitle = "txtAddressModalTitle";
                a.Panel = "<% = this.pnlCustomArticle.ClientID %>";
                a.FormPanel = "<% = this.pnlCustomArticleForm.ClientID %>";
                a.Modal = "<% = this.mdlCustomArticle.ClientID %>";
                a.HtmlEditor = "<% = this.txtBodyHtml.ClientID %>";

                a.Title = "<%= this.txtTitle.ClientID %>";
                a.ProductCategory = "<%= this.cmbProductCategory.ClientID %>";
                a.Summary = "<% = this.txtSummary.ClientID %>";
                a.Body = "<% = this.txtBody.ClientID %>";
                
            } (SEL.Knowledge.Dom.CustomArticle));

            SEL.Knowledge.CustomArticle.Setup();
            
        }());
    </script>
    
    <asp:Panel ID="pnlGrid" runat="server" CssClass="formpanel formpanel_padding">
        <asp:Literal ID="litGrid" runat="server"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel runat="server" ID="pnlCustomArticle" style="display: none; height: 575px;">
        <div class="modalpanel">
            <asp:Panel runat="server" CssClass="formpanel" ID="pnlCustomArticleForm" HeaderText="Article">
                <div id="txtAddressModalTitle" class="sectiontitle">New Knowledge Article</div>
                <div class="onecolumnsmall">
                    <asp:Label runat="server" ID="lblProductCategory" AssociatedControlID="cmbProductCategory">Product area</asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="cmbProductCategory" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label runat="server" ID="lblTitle" AssociatedControlID="txtTitle" CssClass="mandatory">Title / Question*</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtTitle" maxlength="250" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="rfvTitle" ValidationGroup="vgArticle" Text="*" ErrorMessage="Please enter a Title / Question." ControlToValidate="txtTitle"></asp:RequiredFieldValidator></span>
                </div>
                <div class="onecolumnsmall">
                    <asp:Label runat="server" ID="lblSummary" AssociatedControlID="txtSummary">Summary</asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtSummary" Rows="3" MaxLength="250" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                </div>
                <div class="onecolumnlarge">
                    <asp:Label runat="server" ID="lblBody" AssociatedControlID="txtBody"><p class="labeldescription">Contents / Answer</p></asp:Label><span class="inputs"><ajaxToolkit:HtmlEditorExtender ID="txtBodyHtml" ClientIDMode="Static" runat="server" TargetControlID="txtBody" EnableSanitization="False"></ajaxToolkit:HtmlEditorExtender><asp:TextBox runat="server" ID="txtBody" TextMode="MultiLine" Width="566" Height="256" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                </div>
            </asp:Panel>
            <div class="formpanel">
                <div class="formbuttons">
                    <helpers:CSSButton runat="server" ID="btnSaveArticle" Text="save" OnClientClick="SEL.Knowledge.CustomArticle.Save(); return false;" UseSubmitBehavior="False" /> 
                    <helpers:CSSButton runat="server" ID="btnCancelSaveArticle" Text="cancel" OnClientClick=" SEL.Knowledge.CustomArticle.Cancel(); return false; " UseSubmitBehavior="False" />
                </div>
            </div>
        </div>
    </asp:Panel>
    
    <cc1:ModalPopupExtender runat="server" ID="mdlCustomArticle" BackgroundCssClass="modalBackground" TargetControlID="lnkCustomArticle" PopupControlID="pnlCustomArticle"></cc1:ModalPopupExtender>
    <asp:HyperLink runat="server" ID="lnkCustomArticle" style="display: none;"></asp:HyperLink>

   
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
   

</asp:Content>
