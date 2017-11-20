<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="helpInformation.aspx.cs" Inherits="Spend_Management.helpInformation" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="cke" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">Help Contact Information</div>
        <div class="twocolumn"><asp:Label ID="lblCustomerHelpContactName" runat="server" AssociatedControlID="txtCustomerHelpContactName">Help contact name</asp:Label><span class="inputs"><asp:TextBox ID="txtCustomerHelpContactName" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblCustomerHelpContactTelephone" runat="server" AssociatedControlID="txtCustomerHelpContactTelephone">Telephone number</asp:Label><span class="inputs"><asp:TextBox ID="txtCustomerHelpContactTelephone" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn"><asp:Label ID="lblCustomerHelpContactEmailAddress" runat="server" AssociatedControlID="txtCustomerHelpContactEmailAddress">Email address</asp:Label><span class="inputs"><asp:TextBox ID="txtCustomerHelpContactEmailAddress" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblCustomerHelpContactFax" runat="server" AssociatedControlID="txtCustomerHelpContactFax">Fax number</asp:Label><span class="inputs"><asp:TextBox ID="txtCustomerHelpContactFax" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="onecolumn"><asp:Label ID="lblCustomerHelpContactAddress" runat="server" AssociatedControlID="txtCustomerHelpContactAddress"><p class="labeldescription">Postal address</p></asp:Label><span class="inputs"><asp:TextBox ID="txtCustomerHelpContactAddress" runat="server" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div style="height:30px; width:100%; float:left;"></div>
        <div class="sectiontitle"><asp:Literal runat="server" ID="litModuleSectionTitle"></asp:Literal> Usage Information</div>
        <div class="onecolumnlarge editorColumnLarge">
            <asp:Label ID="lblCustomerHelpInformation" runat="server" AssociatedControlID="txtCustomerHelpInformation"><p class="labeldescription">Advisory text</p></asp:Label><span class="inputs"><cke:CKEditorControl ID="txtCustomerHelpInformation" AutoPostBack="true" runat="server" Width="584px" Height="21px" RemovePlugins="elementspath" TextMode="SingleLine" ToolTip="" CssClass="subject" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        
        <div class="formbuttons">
            <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="btnSave_Click"/> &nbsp;&nbsp;
            <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" OnClick="btnCancel_Click" />
        </div>
    </div>
</asp:Content>
