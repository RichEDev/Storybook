<%@ Page Title="" Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="auditLog.aspx.cs" Inherits="Spend_Management.shared.admin.auditLog" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
<script type="text/javascript" language="javascript" src="../javaScript/auditLog.js"></script>
    <a href="javascript:generateReportRequest();" class="submenuitem"><asp:Label id="Label8" runat="server" meta:resourcekey="Label2Resource1">Export to Excel</asp:Label></a>
    <asp:Literal ID="litclear" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<script type="text/javascript" language="javascript">
    var ddlstElement = '<%=ddlstElement.ClientID %>';
    var ddlstAction = '<%=ddlstAction.ClientID %>';
    var txtstartdate = '<%=txtstartdate.ClientID %>';
    var txtenddate = '<%=txtenddate.ClientID %>';
    var txtusername = '<%=txtusername.ClientID %>';
</script>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle"><asp:Label ID="Label1" runat="server" Text="Search Criteria"></asp:Label></div>
        <div class="onecolumnsmall"><asp:Label ID="Label2" runat="server" Text="Element" AssociatedControlID="ddlstElement"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlstElement" runat="server"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn">
            <asp:Label ID="Label3" runat="server" Text="Action" AssociatedControlID="ddlstAction"></asp:Label><span class="inputs"><asp:DropDownList
                ID="ddlstAction" runat="server">
                    <asp:ListItem Value="0" Text="[None]"></asp:ListItem>
                    <asp:ListItem Value="1" Text="Add"></asp:ListItem>
                    <asp:ListItem Value="2" Text="Update"></asp:ListItem>
                    <asp:ListItem Value="3" Text="Delete"></asp:ListItem>
                    <asp:ListItem Value="4" Text="Logged On"></asp:ListItem>
                    <asp:ListItem Value="5" Text="Logged Off"></asp:ListItem>
                    <asp:ListItem Value="6" Text="View"></asp:ListItem>
            </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="Label6" runat="server" Text="Username" AssociatedControlID="txtusername"></asp:Label><span class="inputs"><asp:TextBox ID="txtusername" runat="server"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn"><asp:Label ID="Label4" runat="server" Text="Start Date" AssociatedControlID="txtstartdate"></asp:Label><span class="inputs"><asp:TextBox ID="txtstartdate" runat="server"></asp:TextBox><cc1:CalendarExtender ID="calstartdate" runat="server" TargetControlID="txtstartdate" PopupButtonID="imgstartdate" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender><cc1:MaskedEditExtender ID="MaskedEditExtender1" MaskType="Date" Mask="99/99/9999" TargetControlID="txtstartdate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender></span><span class="inputicon"><asp:Image ID="imgstartdate" ImageUrl="~/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgstartResource1" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqstartdate" ControlToValidate="txtstartdate" Text="*" ErrorMessage="Start date is mandatory" ValidationGroup="vgAuditLog" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cmpstartdate" ControlToValidate="txtstartdate" Text="*" ErrorMessage="Invalid start date specified" Type="Date" Display="Dynamic" ValidationGroup="vgAuditLog" Operator="DataTypeCheck"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpminstartdate" Text="*" ErrorMessage="Start date must be on or after 01/01/1900" ControlToValidate="txtstartdate" Display="Dynamic" ValidationGroup="vgAuditLog" ValueToCompare="01/01/1900" Type="Date" Operator="GreaterThanEqual" SetFocusOnError="False"></asp:CompareValidator></span><asp:Label ID="Label5" runat="server" Text="End Date" AssociatedControlID="txtenddate"></asp:Label><span class="inputs"><asp:TextBox ID="txtenddate" runat="server"></asp:TextBox><cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtenddate" PopupButtonID="imgenddate" Format="dd/MM/yyyy" Enabled="True"></cc1:CalendarExtender><cc1:MaskedEditExtender ID="MaskedEditExtender4" MaskType="Date" Mask="99/99/9999" TargetControlID="txtenddate" CultureName="en-GB" runat="server"></cc1:MaskedEditExtender></span><span class="inputicon"><asp:Image ID="imgenddate" ImageUrl="~/shared/images/icons/cal.gif" runat="server" meta:resourcekey="imgenddate" /></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqenddate" ControlToValidate="txtenddate" Display="Dynamic" Text="*" ErrorMessage="End date is a mandatory field" ValidationGroup="vgAuditLog"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cmpenddate" ControlToValidate="txtenddate" Text="*" ErrorMessage="Invalid end date value specified" Display="Dynamic" ValidationGroup="vgAuditLog" Type="Date" Operator="DataTypeCheck"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmaxenddate" Operator="LessThanEqual" ValidationGroup="vgAuditLog" ValueToCompare="31/12/3000" Text="*" ErrorMessage="End date must be on or before 31/12/3000" ControlToValidate="txtenddate" Display="Dynamic" Type="Date"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmppoststartdate" ControlToCompare="txtstartdate" ControlToValidate="txtenddate" Display="Dynamic" Text="*" Type="Date" ValidationGroup="vgAuditLog" Operator="GreaterThanEqual" ErrorMessage="The start date must preceed the end date value specified"></asp:CompareValidator></span></div>

        <div class="formbuttons"><asp:ImageButton ID="cmdsearch" runat="server" 
                ImageUrl="~/shared/images/buttons/pagebutton_search.gif" 
                meta:resourcekey="cmdsearchResource1" onclick="cmdsearch_Click" /></div>
        <div class="sectiontitle"><asp:Label ID="Label7" runat="server" Text="Search Results"></asp:Label></div>
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons"><asp:ImageButton runat="server" ID="cmdClose" 
                CausesValidation="false" AlternateText="Close" 
                ImageUrl="~/shared/images/buttons/btn_close.png" onclick="cmdClose_Click" /></div>
        </div>
</asp:Content>
