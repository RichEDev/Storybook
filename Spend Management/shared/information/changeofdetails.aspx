<%@ Page language="c#" Inherits="Spend_Management.changeofdetails" MasterPageFile="~/masters/smForm.master" Codebehind="changeofdetails.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <div class="formpanel">
        <div class="sectiontitle"><asp:Label ID="lblchange" runat="server" Text="Please enter any required changes in the box below" meta:resourcekey="lblchangeResource1"></asp:Label></div>
        <div class="onecolumn" style="display: block; position: relative;"><span class="inputs"><asp:TextBox id="txtchanges" runat="server" TextMode="MultiLine" meta:resourcekey="txtchangesResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqchanges" ControlToValidate="txtchanges" ErrorMessage="Please provide details of changes to be notified to the administrator" Text="*" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator></span></div>
        <div class="formbuttons"><asp:ImageButton id="cmdok" AlternateText="Save" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;<a href="mydetails.aspx"><img src="../images/buttons/cancel_up.gif" alt="Cancel" /></a></div>
    </div>
			
    </asp:Content>

