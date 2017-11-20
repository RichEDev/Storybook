<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
    CodeBehind="aeuserdefinedgrouping.aspx.cs" Inherits="Spend_Management.aeuserdefinedgrouping" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smgrProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/userdefinedGroupings.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcUserFieldGroupings.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <script language="javascript" type="text/javascript">
        var ddlArea = '<%=ddlstassociatedtable.ClientID %>';
        var groupingID = '<%=nCurrentGroupingId %>';
        var txtgroupname = '<%=txtgroupname.ClientID %>';
    </script>

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">General Details</div>
        <div class="twocolumn">
            <asp:Label ID="lblgroupname" runat="server" Text="Group Name*" CssClass="mandatory" AssociatedControlID="txtgroupname"></asp:Label><span class="inputs"><asp:TextBox ID="txtgroupname" CssClass="fillspan" MaxLength="50" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqgroupname" runat="server" ErrorMessage="Please enter a Group Name in the box provided" Text="*" ControlToValidate="txtgroupname"></asp:RequiredFieldValidator></span><asp:Label ID="lblproductarea" runat="server" Text="Applies To*" CssClass="mandatory" AssociatedControlID="ddlstassociatedtable"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstassociatedtable" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
    </div>
    <div class="formpanel formpanel_padding" id="divAssociations">
        <asp:Literal runat="server" ID="litAssociations"></asp:Literal>
    </div>
    <div class="formpanel formpanel_padding">
        <div class="formbuttons imgbuttons">
            <asp:Image ID="cmdsave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" /><asp:ImageButton ID="cmbcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" OnClick="cmbcancel_Click" CausesValidation="False" />
        </div>
    </div>

</asp:Content>
