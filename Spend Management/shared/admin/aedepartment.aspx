<%@ Page language="c#" Inherits="Spend_Management.aedepartment" MasterPageFile="~/masters/smForm.master" Codebehind="aedepartment.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js?date=20180417" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel">
        <div class="sectiontitle">General Details</div>
        <div class="twocolumn">
            <asp:Label CssClass="mandatory" id="lbldepartment" runat="server" meta:resourcekey="lbldepartmentResource1" AssociatedControlID="txtdepartment">Department*</asp:Label>
            <span class="inputs"><asp:TextBox CssClass="fillspan" id="txtdepartment" runat="server" MaxLength="50" meta:resourcekey="txtdepartmentResource1"></asp:TextBox></span>
            <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqdepartment" runat="server" ErrorMessage="Please enter a department name."
						ControlToValidate="txtdepartment" meta:resourcekey="reqdepartmentResource1">*</asp:RequiredFieldValidator></span>
        </div>
        <div class="onecolumn">
            <asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label>
            <span class="inputs"><asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></span>
        </div>
        <asp:PlaceHolder ID="holderUserdefined" runat="server"></asp:PlaceHolder> 
        <div class="formbuttons"><asp:ImageButton id="cmdok" runat="server" ImageUrl="/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
    </div>
	
	
	
	
	
        </asp:Content>
