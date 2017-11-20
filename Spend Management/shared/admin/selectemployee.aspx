<%@ Page language="c#" Inherits="Spend_Management.selectemployee" MasterPageFile="~/masters/smForm.master" Codebehind="selectemployee.aspx.cs" EnableViewState="true" Title="Select Employee" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="styles">

        <style type="text/css"> 
       
        #expcriteria .onecolumnsmall .inputs{
            margin-left: 2px;
        }

        </style>

   
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:Panel ID="pnlNewEmployee" runat="server">
		<a class="submenuitem" href="aeemployee.aspx"><asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">New Employee</asp:Label></a>
	</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
    <div class="formpanel formpanel_padding">
	    <div class="sectiontitle">
            <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Search Options</asp:Label>
	    </div>
	    <div class="twocolumn">
            <asp:Label id="lblsurname" AssociatedControlID="txtsurname" runat="server" meta:resourcekey="lblsurnameResource1">Enter surname of employee (or lead characters)</asp:Label><span class="inputs"><asp:TextBox id="txtsurname" runat="server" meta:resourcekey="txtsurnameResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label id="lblusername" runat="server" meta:resourcekey="lblusernameResource1" AssociatedControlID="txtusername">Username</asp:Label><span class="inputs"><asp:TextBox id="txtusername" runat="server" meta:resourcekey="txtusernameResource1" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="onecolumnsmall">
            <asp:Label id="lblrole" runat="server" meta:resourcekey="lblroleResource1" AssociatedControlID="cmbroles">Role</asp:Label><span class="inputs"><asp:DropDownList id="cmbroles" runat="server" meta:resourcekey="cmbrolesResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div id="expcriteria">
            <div id="expsignoffgroup" class="onecolumnsmall">
                <asp:Label id="lblsignoff" runat="server" meta:resourcekey="lblsignoffResource1" AssociatedControlID="cmbgroups">Signoff Group</asp:Label><span class="inputs"><asp:DropDownList id="cmbgroups" runat="server" meta:resourcekey="cmbgroupsResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="onecolumnsmall">
                <asp:Label id="lbldepartment" runat="server" meta:resourcekey="lbldepartmentResource1" AssociatedControlID="cmbdepartments">Default Department</asp:Label><span class="inputs"><asp:DropDownList id="cmbdepartments" runat="server" meta:resourcekey="cmbdepartmentsResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
            <div class="onecolumnsmall">
                <asp:Label id="lblcostcode" runat="server" meta:resourcekey="lblcostcodeResource1" AssociatedControlID="cmbcostcodes">Default Costcode</asp:Label><span class="inputs"><asp:DropDownList id="cmbcostcodes" runat="server" meta:resourcekey="cmbcostcodesResource1" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            </div>
        </div>
	    <div class="formbuttons">
            <asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/pagebutton_search.gif" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;
            <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" onclick="cmdClose_Click" />
	    </div>
    </div>		
</asp:Content>


