<%@ Page language="c#" Inherits="expenses.information.addreview" MasterPageFile="~/expform.master" Codebehind="addreview.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="valdiv">
		<asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblgeneraldetails" runat="server" Text="General Details" meta:resourcekey="lblgeneraldetailsResource1"></asp:Label></div>
		<table class="tr-height">
			<TBODY>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblhotellbl" runat="server" Text="Hotel:" meta:resourcekey="lblhotellblResource1"></asp:Label></td>
					<td class="inputtd">
						<asp:Label id="lblhotel" runat="server" meta:resourcekey="lblhotelResource1">Label</asp:Label></td>
				</tr>
				<tr>
					<td class="labeltd"></td>
					<td>
					<table cellspacing=5 border=0 style="width: 155px;"><tr><td align=center>1</td><td align=center>2</td><td align=center>3</td><td align=center>4</td><td align=center>5</td></tr></table></td>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblrating" runat="server" Text="Overall Rating:" meta:resourcekey="lblratingResource1"></asp:Label></td>
					<td>
						<asp:RadioButtonList id="optrating" runat="server" RepeatDirection="Horizontal" CellSpacing="5" meta:resourcekey="optratingResource1">
							<asp:ListItem Value="1" meta:resourcekey="ListItemResource1"></asp:ListItem>
							<asp:ListItem Value="2" meta:resourcekey="ListItemResource2"></asp:ListItem>
							<asp:ListItem Value="3" Selected=True meta:resourcekey="ListItemResource3"></asp:ListItem>
							<asp:ListItem Value="4" meta:resourcekey="ListItemResource4"></asp:ListItem>
							<asp:ListItem Value="5" meta:resourcekey="ListItemResource5"></asp:ListItem>
						</asp:RadioButtonList></td>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblstandard" runat="server" Text="Standard of Rooms:" meta:resourcekey="lblstandardResource1"></asp:Label></td>
					<td>
						<asp:RadioButtonList id="optrooms" runat="server" RepeatDirection="Horizontal" CellSpacing="5" meta:resourcekey="optroomsResource1">
							<asp:ListItem Value="1" meta:resourcekey="ListItemResource6"></asp:ListItem>
							<asp:ListItem Value="2" meta:resourcekey="ListItemResource7"></asp:ListItem>
							<asp:ListItem Value="3" Selected=True meta:resourcekey="ListItemResource8"></asp:ListItem>
							<asp:ListItem Value="4" meta:resourcekey="ListItemResource9"></asp:ListItem>
							<asp:ListItem Value="5" meta:resourcekey="ListItemResource10"></asp:ListItem>
						</asp:RadioButtonList></td>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblfacilities" runat="server" Text="Hotel Facilities:" meta:resourcekey="lblfacilitiesResource1"></asp:Label>
						</td>
					<td>
						<asp:RadioButtonList id="optfacilities" runat="server" RepeatDirection="Horizontal" CellSpacing="5" meta:resourcekey="optfacilitiesResource1">
							<asp:ListItem Value="1" meta:resourcekey="ListItemResource11"></asp:ListItem>
							<asp:ListItem Value="2" meta:resourcekey="ListItemResource12"></asp:ListItem>
							<asp:ListItem Value="3" Selected=True meta:resourcekey="ListItemResource13"></asp:ListItem>
							<asp:ListItem Value="4" meta:resourcekey="ListItemResource14"></asp:ListItem>
							<asp:ListItem Value="5" meta:resourcekey="ListItemResource15"></asp:ListItem>
						</asp:RadioButtonList>
					</td>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblvalueformoney" runat="server" Text="Value for Money:" meta:resourcekey="lblvalueformoneyResource1"></asp:Label></td>
					<td>
						<asp:RadioButtonList id="optvalue" runat="server" RepeatDirection="Horizontal" CellSpacing="5" meta:resourcekey="optvalueResource1">
							<asp:ListItem Value="1" meta:resourcekey="ListItemResource16"></asp:ListItem>
							<asp:ListItem Value="2" meta:resourcekey="ListItemResource17"></asp:ListItem>
							<asp:ListItem Value="3" Selected="True" meta:resourcekey="ListItemResource18"></asp:ListItem>
							<asp:ListItem Value="4" meta:resourcekey="ListItemResource19"></asp:ListItem>
							<asp:ListItem Value="5" meta:resourcekey="ListItemResource20"></asp:ListItem>
						</asp:RadioButtonList>
					</td>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblperformance" runat="server" Text="Performance of Employees:" meta:resourcekey="lblperformanceResource1"></asp:Label></td>
					<td>
						<asp:RadioButtonList id="optperformance" runat="server" RepeatDirection="Horizontal" CellSpacing="5" meta:resourcekey="optperformanceResource1">
							<asp:ListItem Value="1" meta:resourcekey="ListItemResource21"></asp:ListItem>
							<asp:ListItem Value="2" meta:resourcekey="ListItemResource22"></asp:ListItem>
							<asp:ListItem Value="3" Selected="True" meta:resourcekey="ListItemResource23"></asp:ListItem>
							<asp:ListItem Value="4" meta:resourcekey="ListItemResource24"></asp:ListItem>
							<asp:ListItem Value="5" meta:resourcekey="ListItemResource25"></asp:ListItem>
						</asp:RadioButtonList></td>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lbllocation" runat="server" Text="Hotel Address:" meta:resourcekey="lbllocationResource1"></asp:Label></td>
					<td>
						<asp:RadioButtonList id="optlocation" runat="server" RepeatDirection="Horizontal" CellSpacing="5" meta:resourcekey="optlocationResource1">
							<asp:ListItem Value="1" meta:resourcekey="ListItemResource26"></asp:ListItem>
							<asp:ListItem Value="2" meta:resourcekey="ListItemResource27"></asp:ListItem>
							<asp:ListItem Value="3" Selected="True" meta:resourcekey="ListItemResource28"></asp:ListItem>
							<asp:ListItem Value="4" meta:resourcekey="ListItemResource29"></asp:ListItem>
							<asp:ListItem Value="5" meta:resourcekey="ListItemResource30"></asp:ListItem>
						</asp:RadioButtonList></td>
				</tr>
				<tr>
					<td class=labeltd></td>
					<td class=inputtd>
						<table cellspacing="3">
							<tr>
								<td><asp:Label ID="lblpoor" runat="server" Text="1 = Poor" meta:resourcekey="lblpoorResource1"></asp:Label></td>
                                <td style="width:50px;">&nbsp;</td>
								<td><asp:Label ID="lblbelowavg" runat="server" Text="2 = Below Average" meta:resourcekey="lblbelowavgResource1"></asp:Label></td>
                                <td style="width:50px;">&nbsp;</td>
								<td><asp:Label ID="lblavg" runat="server" Text="3 = Average" meta:resourcekey="lblavgResource1"></asp:Label></td>
                              <td style="width:50px;">&nbsp;</td>
								<td><asp:Label ID="lblgood" runat="server" Text="4 = Good" meta:resourcekey="lblgoodResource1"></asp:Label></td>
                                <td style="width:50px;">&nbsp;</td>
								<td><asp:Label ID="lblexcellent" runat="server" Text="5 = Excellent" meta:resourcekey="lblexcellentResource1"></asp:Label></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="labeltd" valign="top">
                        <asp:Label ID="lblcomments" runat="server" Text="Other Comments:" meta:resourcekey="lblcommentsResource1"></asp:Label></td>
					<td>
						<asp:TextBox id="txtreview" runat="server" TextMode="MultiLine" Width="363px" Height="88px" MaxLength="4000" meta:resourcekey="txtreviewResource1"></asp:TextBox></td>
					
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblreviewdetails" runat="server" Text="Display the following info with my review:" meta:resourcekey="lblreviewdetailsResource1"></asp:Label></td>
					<td class="inputtd tr-height">
						<asp:RadioButton id="optdispname" runat="server" Text="My Name" GroupName="disp" Checked="True" meta:resourcekey="optdispnameResource1"></asp:RadioButton>
						<asp:RadioButton id="optdispnamecomp" runat="server" Text="My Name &amp; Company" GroupName="disp" meta:resourcekey="optdispnamecompResource1"></asp:RadioButton>
						<asp:RadioButton id="optdispanon" runat="server" Text="Anonymous" GroupName="disp" meta:resourcekey="optdispanonResource1"></asp:RadioButton></td>
					<TD></TD>
				</tr>
				<tr>
					<td class="labeltd">
                        <asp:Label ID="lblamountpernight" runat="server" Text="Amount paid per night:" meta:resourcekey="lblamountpernightResource1"></asp:Label></td>
					<td class=inputtd>
						<asp:TextBox id="txtpaid" runat="server" Width="75px" meta:resourcekey="txtpaidResource1"></asp:TextBox></td>
					<TD>
						<asp:CompareValidator id="compamount" runat="server" ErrorMessage="The value entered into Amount paid per night is invalid. This must be a numeric amount."
							ControlToValidate="txtpaid" Operator="DataTypeCheck" Type="Currency" meta:resourcekey="compamountResource1">*</asp:CompareValidator></TD>
				</tr>
			</TBODY>
		</table>
		
		<div class=inputpanel>
			<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
			<asp:ImageButton id="cmdcancel" runat="server" CausesValidation="False" ImageUrl="../buttons/cancel_up.gif" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
		</div>
        
           </asp:Content>

