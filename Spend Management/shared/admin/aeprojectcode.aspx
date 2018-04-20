<%@ Page language="c#" Inherits="Spend_Management.aeprojectcode" MasterPageFile="~/masters/smForm.master" Codebehind="aeprojectcode.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js?date=20180417" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">General Details</div>
        <div class="twocolumn">
            <asp:Label CssClass="mandatory" id="lblprojectcode" runat="server" meta:resourcekey="lblprojectcodeResource1" AssociatedControlID="txtprojectcode">Project Code*</asp:Label>
            <span class="inputs"><asp:TextBox CssClass="fillspan" MaxLength="50" id="txtprojectcode" runat="server" meta:resourcekey="txtprojectcodeResource1"></asp:TextBox></span>
            <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqcode" runat="server" ErrorMessage="Please enter a name for this Project Code in the box provided"
						ControlToValidate="txtprojectcode" meta:resourcekey="reqcodeResource1">*</asp:RequiredFieldValidator></span>
            <asp:Label ID="lblrechargeable" runat="server" Text="Label" AssociatedControlID="chkrechargeable">Rechargeable</asp:Label>			
            <span class="inputs" ><asp:CheckBox ID="chkrechargeable" runat="server" meta:resourcekey="chkrechargeableResource1" /></span>
        </div>
        <div class="onecolumn">
            <asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label>
            <span class="inputs"><asp:TextBox MaxLength="2000" id="txtdescription" runat="server" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></span>
        </div>
        <asp:PlaceHolder ID="holderUserdefined" runat="server"></asp:PlaceHolder> 
        <div class="formbuttons">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
	</div>
    </div>
	
	
	
	

    </asp:Content>

