
<%@ Page language="c#" Inherits="expenses.information.directory" MasterPageFile="~/exptemplate.master" Codebehind="directory.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="styles">
    <style type="text/css">
        .newbuttonbg a {
            padding: 3px;
        }

    </style>
      <!--[if IE 7]>
          <style>
              .newbuttonbg A {
                    display: inline-block;
                }
          </style>
        
      <![endif]-->
    </asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
			<div class=inputpanel>
			<table class="newbuttonbg">
				<tr>
					<td>
                        <asp:Label ID="lblsurname" runat="server" Text="Please&nbsp;enter&nbsp;a&nbsp;surname:" meta:resourcekey="lblsurnameResource1"></asp:Label></td>
                    <td>&nbsp;</td>
					<td><asp:TextBox id="txtsearch" runat="server" meta:resourcekey="txtsearchResource1"></asp:TextBox></td>
                    <td>&nbsp;</td>
					<td>
                        <asp:Label ID="lblletter" runat="server" Text="or&nbsp;select&nbsp;a&nbsp;letter" meta:resourcekey="lblletterResource1"></asp:Label></td>
                    <td>&nbsp;</td>
					<td valign="middle"><a href="directory.aspx?letter=A">A</a></td>
					<TD><a href="directory.aspx?letter=B">B</a></TD>
					<TD><a href="directory.aspx?letter=C">C</a></TD>
					<TD><a href="directory.aspx?letter=D">D</a></TD>
					<TD><a href="directory.aspx?letter=E">E</a></TD>
					<TD><a href="directory.aspx?letter=F">F</a></TD>
					<TD><a href="directory.aspx?letter=G">G</a></TD>
					<TD><a href="directory.aspx?letter=H">H</a></TD>
					<TD><a href="directory.aspx?letter=I">I</a></TD>
					<TD><a href="directory.aspx?letter=J">J</a></TD>
					<TD><a href="directory.aspx?letter=K">K</a></TD>
					<TD><a href="directory.aspx?letter=L">L</a></TD>
					<TD><a href="directory.aspx?letter=M">M</a></TD>
					<TD><a href="directory.aspx?letter=N">N</a></TD>
					<TD><a href="directory.aspx?letter=O">O</a></TD>
					<TD><a href="directory.aspx?letter=P">P</a></TD>
					<TD><a href="directory.aspx?letter=Q">Q</a></TD>
					<TD><a href="directory.aspx?letter=R">R</a></TD>
					<TD><a href="directory.aspx?letter=S">S</a></TD>
					<TD><a href="directory.aspx?letter=T">T</a></TD>
					<TD><a href="directory.aspx?letter=U">U</a></TD>
					<TD><a href="directory.aspx?letter=V">V</a></TD>
					<TD><a href="directory.aspx?letter=W">W</a></TD>
					<td><a href="directory.aspx?letter=X">XYZ</a></td>
                   <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td align="right">
						<asp:LinkButton id="LinkButton1" runat="server" onclick="LinkButton1_Click" meta:resourcekey="LinkButton1Resource1">Search</asp:LinkButton>
                    </td>
				</tr>
				<tr>
					<td></td>
					
				</tr>
			</table>
		</div>
			
			<div class="inputpanel table-border2">
			
			<asp:Literal id="litnames" runat="server" EnableViewState="False" meta:resourcekey="litnamesResource1"></asp:Literal>
            <div class="formbuttons">
                <asp:ImageButton ID="ImageButton1" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
			</div>

   </asp:Content>
