<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthoriserLevel.aspx.cs" Inherits="Spend_Management.shared.admin.AuthoriserLevel" MasterPageFile="~/masters/smForm.master" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
     <a href="AuthoriserLevelDetails.aspx" class="submenuitem">
         <asp:Label id="lblNewAuthoriserLevel" runat="server" meta:resourcekey="Label1Resource1">New Authoriser Level</asp:Label></a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentmain" runat="Server">
      <script type="text/javascript" language="javascript">
          function Save() {
              var txtDefaultApproverID = document.getElementById('<%=txtDefaultApprover_ID.ClientID %>');
              if (txtDefaultApproverID) {
                  SEL.AuthoriserLevel.Menu.UpdateEmployee(txtDefaultApproverID.value);
              }

          }
          </script>
    <asp:ScriptManagerProxy ID="smProxy" runat="server">
         <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.AuthoriserLevels.js" />
             <asp:ScriptReference Name="autocomplete" />

        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAuthoriserLevel.asmx" />

        </Services>
    </asp:ScriptManagerProxy>
     <div class="formpanel formpanel_padding">
           <div class="sectiontitle">
            <asp:Label runat="server" Text="Generals Details" meta:resourcekey="lblsystemtempsResource1"></asp:Label>
            </div>
         <div class="twocolumn">
			 <asp:Label CssClass="mandatory" ID="lblDefaultApprover" runat="server" Text="Default authoriser*" AssociatedControlID="txtDefaultApprover"></asp:Label>
             <span class="inputs"><asp:TextBox runat="server" id="txtDefaultApprover" MaxLength="320" /><asp:TextBox runat="server" style="display: none;" ID="txtDefaultApprover_ID" />
             </span>
             <span class="inputicon"></span><span class="inputtooltipfield"><img onmouseover="SEL.Tooltip.Show('EACEB7F1-952D-4180-85EA-47262258B233', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                 <asp:RequiredFieldValidator ID="rfvDefaultApprover" runat="server" ErrorMessage="Please enter a Default authoriser." Text="*" ControlToValidate="txtDefaultApprover" 
                     ValidationGroup="AuthoriserLevel" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator runat="server" ID="cvDefaultApprover" Text="*"
                          ValidationGroup="AuthoriserLevel" ControlToValidate="txtDefaultApprover_ID" ErrorMessage="Please enter a valid Default authoriser." Display="Dynamic"
                          Operator="NotEqual" ValueToCompare="-1"></asp:CompareValidator></span>
             <div class="formbuttons">
		 <helpers:CSSButton ID="cmdOk" runat="server" UseSubmitBehavior="false" OnClientClick="return Save();" Text="save" />
	    </div>

          <div class="sectiontitle">
            <asp:Label ID="lblsystemtemps" runat="server" Text="Authoriser Levels" meta:resourcekey="lblsystemtempsResource1"></asp:Label>
            </div>
             <div class="twocolumn">
            <asp:Label ID="lblnamemsg" runat="server" Text="All amounts shown are in " meta:resourcekey="lblnamemsgResource1"></asp:Label>
        </div>
             <div>

             </div>
        <asp:Panel ID="pnlAuthoriserLevelDetailGrid" runat="server">
	        <asp:Literal id="litAuthoriserLevelDetailGrid" runat="server" EnableViewState="False" meta:resourcekey="litgridResource1"></asp:Literal>
	    </asp:Panel>
             
      
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdClose" OnClick="cmdClose_Click" Text="close" runat="server" UseSubmitBehavior="false" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False" />
        </div> 
	</div>
             <asp:HiddenField ID="hfCustomerId" runat="server" />
         </div>
</asp:Content>
