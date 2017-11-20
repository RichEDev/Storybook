<%@ Page language="c#" Inherits="Spend_Management.aebudget" MasterPageFile="~/masters/smForm.master"  Codebehind="aebudget.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content
        ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="formpanel formpanel_padding">
		<div class="sectiontitle"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<div class="twocolumn">
		    <asp:Label id="lbllabel" CssClass="mandatory" runat="server" meta:resourcekey="lbllabelResource1" AssociatedControlID="txtlabel">Label*</asp:Label><span class="inputs">
            <asp:textbox id="txtlabel" runat="server" MaxLength="50" 
                meta:resourcekey="txtlabelResource1" ValidationGroup="aebudget"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield">
            <asp:requiredfieldvalidator id="reqlabel" runat="server" 
                ControlToValidate="txtlabel" 
                ErrorMessage="Please enter a label for this budget holder" 
                meta:resourcekey="reqlabelResource1" ValidationGroup="aebudget">*</asp:requiredfieldvalidator></span><asp:Label id="lblemployees" CssClass="mandatory" runat="server" meta:resourcekey="lblemployeesResource1" AssociatedControlID="txtUser">Employee responsible*</asp:Label><span class="inputs">
            <asp:TextBox runat="server" ID="txtUser" CssClass="fillspan" 
                ValidationGroup="aebudget"></asp:TextBox><asp:TextBox runat="server" style="display:  none;" ID="txtUser_ID"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield" id="spanEmployeeVal">
            <asp:RequiredFieldValidator runat="server" ID="reqtxtUser" 
                ControlToValidate="txtUser" Display="Dynamic" 
                ErrorMessage="Please enter a valid employee responsible" Text="*" 
                ValidationGroup="aebudget"></asp:RequiredFieldValidator>
                <asp:CompareValidator runat="server" ID="cmptxtUser"
                ControlToValidate="txtUser_ID" Display="Dynamic" ErrorMessage="Invalid entry provided for Employee responsible - Type three or more characters and select a valid entry from the available options." Text="*" Operator="NotEqual" ValueToCompare="-1" Type="Integer"
                ValidationGroup="aebudget"></asp:CompareValidator>
                </span>
		</div>
		<div class="onecolumn">
			<asp:Label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:textbox id="txtdescription" runat="server" MaxLength="4000" TextMode="MultiLine" meta:resourcekey="txtdescriptionResource1"></asp:textbox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
	    </div>
	    <div class="formbuttons">
	        <asp:imagebutton id="cmdok" runat="server" 
                ImageUrl="~/shared/images/buttons/btn_save.png" 
                meta:resourcekey="cmdokResource1" ValidationGroup="aebudget"></asp:imagebutton>&nbsp;&nbsp;
		    <asp:imagebutton id="cmdcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:imagebutton>
	    </div>
	</div>
</asp:Content>
