<%@ Page language="c#" Inherits="Spend_Management.aereason" MasterPageFile="~/masters/smForm.master" Codebehind="aereason.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/reasons.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/expenses/webservices/svcReasons.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>

    <script type="text/javascript">
        //<![CDATA[
        var reasonID = <% = ReasonID %>;
        var reasonNameID = '<% = txtreason.ClientID %>';
        var descriptionID = '<% = txtdescription.ClientID %>';
        var codeWithVATID = '<% = txtaccountcodevat.ClientID %>';
        var codeWithoutVATID = '<% = txtaccountcodenovat.ClientID %>';
        //]]>
    </script>
    
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
        <div class="twocolumn"><asp:Label id="lblreason" runat="server" meta:resourcekey="lblreasonResource1" AssociatedControlID="txtreason" CssClass="mandatory">Reason*</asp:Label><span class="inputs"><asp:TextBox id="txtreason" runat="server" MaxLength="50" meta:resourcekey="txtreasonResource1"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqreason" runat="server" ErrorMessage="Please enter a name for this Reason"
						    ControlToValidate="txtreason" meta:resourcekey="reqreasonResource1">*</asp:RequiredFieldValidator></span><span class="inputs"></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="onecolumn"><asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>						
        <div class="twocolumn"><asp:Label ID="lblaccountcodevat" runat="server" Text="Account Code (VAT)" AssociatedControlID="txtaccountcodevat" meta:resourcekey="lblaccountcodevatResource1"></asp:Label><span class="inputs"><asp:textbox id="txtaccountcodevat" runat="server" meta:resourcekey="txtaccountcodevatResource1" MaxLength="50"></asp:textbox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"></span><asp:Label ID="lblaccountcodenovat" runat="server" AssociatedControlID="txtaccountcodenovat" Text="Account Code (No VAT)" meta:resourcekey="lblaccountcodenovatResource1"></asp:Label><span class="inputs"><asp:textbox id="txtaccountcodenovat" runat="server" meta:resourcekey="txtaccountcodenovatResource1" MaxLength="50"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="formbuttons">
            <asp:Image ID="btnSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="javascript:SaveReason();" meta:resourcekey="cmdokResource1"></asp:Image>&nbsp;<asp:Image ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" onclick="javascript:CancelReason();" meta:resourcekey="cmdcancelResource1"></asp:Image>
		</div>
    </div>
	
</asp:Content>

