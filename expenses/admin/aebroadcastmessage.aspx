<%@ Page Language="C#" validateRequest="false" MasterPageFile="~/expform.master" AutoEventWireup="true" Inherits="admin_aebroadcastmessage" Title="Untitled Page" Codebehind="aebroadcastmessage.aspx.cs" Culture="en-GB" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/expform.master" %>

    
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
    
<div class="inputpanel">

    <div class="inputpaneltitle">General Details</div>
    <table>
        <tr><td class="labeltd">
            <asp:Label ID="lbltitle" runat="server" Text="Title:" meta:resourcekey="lbltitleResource1"></asp:Label></td><td class="inputtd">
            <asp:TextBox ID="txttitle" runat="server" meta:resourcekey="txttitleResource1"></asp:TextBox></td><td>
                <asp:RequiredFieldValidator ID="reqtitle" runat="server" ControlToValidate="txttitle"
                    ErrorMessage="Please enter a title for this Broadcast Message in the box provided" meta:resourcekey="reqtitleResource1" ValidationGroup="vgBroadcast" Text="*" Display="Dynamic">*</asp:RequiredFieldValidator></td></tr>
        <tr><td class="labeltd">
            <asp:Label ID="lblstartdate" runat="server" Text="Start Date:" meta:resourcekey="lblstartdateResource1"></asp:Label></td><td class="inputtd">
            <asp:TextBox ID="txtstartdate" runat="server" meta:resourcekey="txtstartdateResource1"></asp:TextBox>&nbsp;<asp:Image runat="server" ID="imgstartdate" ImageUrl="../icons/cal.gif" /></td><td>
                <asp:CompareValidator ID="compstartdate" runat="server" ControlToValidate="txtstartdate"
                    ErrorMessage="The Start Date you have entered is invalid." Operator="DataTypeCheck"
                    Type="Date" meta:resourcekey="compstartdateResource1" Display="Dynamic" ValidationGroup="vgBroadcast" Text="*"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpstartmin" ControlToValidate="txtstartdate" Display="Dynamic" Operator="GreaterThanEqual" ValidationGroup="vgBroadcast" ValueToCompare="01/01/1900" Text="*" ErrorMessage="The Start Date must be on or after 01/01/1900" Type="Date"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpstartmax" ControlToValidate="txtstartdate" Display="Dynamic" Operator="LessThanEqual" Type="Date" ValidationGroup="vgBroadcast" ValueToCompare="31/12/3000" Text="*" ErrorMessage="The Start Date must preceed 31/12/3000"></asp:CompareValidator><cc1:CalendarExtender runat="server" ID="calexstartdate" TargetControlID="txtstartdate" PopupButtonID="imgstartdate" PopupPosition="Right" Format="dd/MM/yyyy"></cc1:CalendarExtender></td></tr>
        <tr><td class="labeltd">
            <asp:Label ID="lblenddate" runat="server" Text="End Date:" meta:resourcekey="lblenddateResource1"></asp:Label></td><td class="inputtd">
            <asp:TextBox ID="txtenddate" runat="server" meta:resourcekey="txtenddateResource1"></asp:TextBox>&nbsp;<asp:Image runat="server" ID="imgenddate" ImageUrl="../icons/cal.gif" /><cc1:CalendarExtender runat="server" ID="calexenddate" TargetControlID="txtenddate" PopupButtonID="imgenddate" PopupPosition="Right" Format="dd/MM/yyyy"></cc1:CalendarExtender></td><td>
                <asp:CompareValidator ID="cmpenddate" runat="server" ControlToValidate="txtenddate"
                    ErrorMessage="The End Date you have entered is invalid." Operator="DataTypeCheck" Display="Dynamic"
                    Type="Date" meta:resourcekey="compenddateResource1" Text="*" ValidationGroup="vgBroadcast"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpstartend" Operator="GreaterThanEqual" Type="Date" ControlToValidate="txtenddate" ControlToCompare="txtstartdate" Text="*" Display="Dynamic" ErrorMessage="The End Date must be on or after the date specified in the start date" ValidationGroup="vgBroadcast"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpminend" Type="Date" Operator="GreaterThanEqual" Display="Dynamic" Text="*" ErrorMessage="The End Date specified must be on or after today" ValidationGroup="vgBroadcast" ControlToValidate="txtenddate"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpmaxend" Operator="LessThanEqual" ValidationGroup="vgBroadcast" ValueToCompare="31/12/3000" Text="*" ControlToValidate="txtenddate" ErrorMessage="The End Date must preceed 31/12/3000" Display="Dynamic" Type="Date"></asp:CompareValidator> </td></tr>
        <tr><td class="labeltd">    
            <asp:Label ID="lbllocation" runat="server" Text="Location:" meta:resourcekey="lbllocationResource1"></asp:Label></td><td class="inputtd">
            <asp:DropDownList ID="cmblocation" runat="server" meta:resourcekey="cmblocationResource1">
                <asp:ListItem Value="1" meta:resourcekey="ListItemResource1">Home Page</asp:ListItem>
                <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Submit Claim</asp:ListItem>
            </asp:DropDownList></td><td></td><td><img id="imgtooltip343" onclick="SEL.Tooltip.Show('ceb158ba-808f-4374-9032-70244826d043', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td></tr>
        <tr><td class="labeltd">
            <asp:Label ID="lblexpirewhenread" runat="server" Text="Expire When Read:" meta:resourcekey="lblexpirewhenreadResource1"></asp:Label></td><td class="inputtd">
            <asp:CheckBox ID="chkexpirewhenread" runat="server" meta:resourcekey="chkexpirewhenreadResource1" /></td><td></td><td><img id="imgtooltip344" onclick="SEL.Tooltip.Show('0f9d45bb-c3dd-4e37-94ce-bfdd440fda15', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td></tr>
        <tr><td class="labeltd">
            <asp:Label ID="lbldisplayonce" runat="server" Text="Only Display Once Per Session:" meta:resourcekey="lbldisplayonceResource1"></asp:Label></td><td class="inputtd">
            <asp:CheckBox ID="chkoncepersession" runat="server" meta:resourcekey="chkoncepersessionResource1" /></td><td></td><td><img id="imgtooltip345" onclick="SEL.Tooltip.Show('49bd1d8f-bd55-4480-902d-461e4d7bddb3', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td></tr>
    </table>
</div>
<div class="inputpanel">
    <div class="inputpaneltitle"><asp:Label ID="lblbroadcastmsg" runat="server" Text="Broadcast Message" meta:resourcekey="lblbroadcastmsgResource1"></asp:Label></div>
    <cc1:HtmlEditorExtender ID="HtmlEditorExtender1" runat="server" TargetControlID="txtmessage" EnableSanitization="False" />
    <asp:TextBox ID="txtmessage" runat="server" Height="260px" Width="600px" TextMode="MultiLine"></asp:TextBox>
    <asp:HiddenField runat="server" ID="hiddenHTMLTxt"/>
    <asp:RequiredFieldValidator ID="reqmessage" runat="server" ControlToValidate="txtmessage" ErrorMessage="Please enter the BroadCast Message in the box provided." meta:resourcekey="reqmessageResource1">*</asp:RequiredFieldValidator></div>
<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;<a href="broadcastmessages.aspx" class="pagebutton"><img border="0" src="../buttons/cancel_up.gif" /></a>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1" /></div>
</asp:Content>

