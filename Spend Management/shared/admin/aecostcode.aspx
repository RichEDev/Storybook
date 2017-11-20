<%@ Page language="c#" Inherits="Spend_Management.aecostcode" MasterPageFile="~/masters/smForm.master" Codebehind="aecostcode.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">General Details</div>
            <div class="twocolumn">
            <asp:Label CssClass="mandatory" id="lblcostcode" runat="server" meta:resourcekey="lblcostcodeResource1" Text="Cost Code*" AssociatedControlID="txtcostcode"></asp:Label>
            <span class="inputs"><asp:TextBox CssClass="fillspan" id="txtcostcode" runat="server" MaxLength="50" meta:resourcekey="txtcostcodeResource1"></asp:TextBox></span>
            <span class="inputicon"></span>
            <span class="inputtooltipfield"></span>
            <span class="inputvalidatorfield"><asp:RequiredFieldValidator id="reqcostcode" runat="server" ErrorMessage="Please enter a cost code" ControlToValidate="txtcostcode" meta:resourcekey="reqcostcodeResource1">*</asp:RequiredFieldValidator></span></div>
            <div class="onecolumn">
                <asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" Text="" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label>
                <span class="inputs"><asp:TextBox id="txtdescription" runat="server" TextMode="MultiLine" MaxLength="4000" meta:resourcekey="txtdescriptionResource1"></asp:TextBox></span>
                <span class="inputicon"></span>&nbsp;<span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblOwner" Text="Cost Code Owner" AssociatedControlID="txtcostcodeowner"></asp:Label>
                <span class="inputs"><asp:TextBox runat="server" ID="txtcostcodeowner" CssClass="fillspan"></asp:TextBox><asp:TextBox runat="server" ID="txtcostcodeowner_ID" style="display:  none;"></asp:TextBox></span><span class="inputicon"></span>&nbsp;<span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
             <asp:PlaceHolder ID="holderUserdefined" runat="server"></asp:PlaceHolder> 
            <div class="formbuttons lable-heightg1">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="../images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
		</div>
	</div>
      </asp:Content>
