<%@ Page language="c#" Inherits="expenses.employeesearch" MasterPageFile="~/expform.master" Codebehind="employeeproxy.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script language="javascript" type="text/javascript">
// <!CDATA[

function IMG1_onclick() { 
}

// ]]>
</script>
			<div class="inputpanel">
				<div class="inputpaneltitle">
                    <asp:Label ID="lblemployeesearch" runat="server" Text="Employee Search" meta:resourcekey="lblemployeesearchResource1"></asp:Label></div>
				<table style="width:500px; margin-bottom:30px;">
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblsurname" runat="server" Text="Surname" meta:resourcekey="lblsurnameResource1"></asp:Label></td>
						<td>
							<asp:TextBox id="txtsurname" runat="server" meta:resourcekey="txtsurnameResource1"></asp:TextBox></td>
					</tr>
				</table>
				<asp:ImageButton id="cmdsearch" runat="server" ImageUrl="../buttons/pagebutton_search.gif" meta:resourcekey="cmdsearchResource1"></asp:ImageButton>
			</div>
            <div class="formpanel formpanel_padding">
			<asp:Panel runat="server" ID="pnlGrid">
				<asp:Literal id="litgrid" runat="server" ></asp:Literal>
			</asp:Panel>
            </div>
			<div class="inputpanel">
				<div class="inputpaneltitle">
                    <asp:Label ID="lblallocated" runat="server" Text="Allocated Employees" meta:resourcekey="lblallocatedResource1"></asp:Label></div>
				<div class="table-border2"><asp:Literal id="litallocated" runat="server" meta:resourcekey="litallocatedResource1"></asp:Literal></div>
			</div>
			<div class="inputpanel">
				<asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" />&nbsp;&nbsp;
				<a href="delegates.aspx"><img src="../buttons/cancel_up.gif" alt="Cancel" /></a>
                </div>

   </asp:Content>

