
<%@ Page language="c#" Inherits="expenses.hotelsearch" Codebehind="hotelsearch.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat=server>
		<title>hotelsearch</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="images/styles.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function populateHotelDetails(hotelid, hotelname)
			{
				Form1.hotelid.value = hotelid;
				Form1.hotelname.value = hotelname;
				
				
			}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<asp:Literal id="litstyles" runat="server" meta:resourcekey="litstylesResource1"></asp:Literal>
			<div class="inputpanel">
				<div class="inputpaneltitle"><input type="radio" name="searchtype" value="1"><asp:Label ID="lblbyname" runat="server"
                        Text="By &#13;&#10;&#9;&#9;&#9;&#9;&#9;Name" meta:resourcekey="lblbynameResource1"></asp:Label> </div>
				<table>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblhotelname" runat="server" Text="Hotel Name:" meta:resourcekey="lblhotelnameResource1"></asp:Label></td>
						<td class="inputtd">
							<asp:TextBox id="txthotelname" runat="server" meta:resourcekey="txthotelnameResource1"></asp:TextBox></td>
					</tr>
				</table>
			</div>
			<div class="inputpanel">
				<div class="inputpaneltitle"><input type="radio" name="searchtype" value="2" checked>
                    <asp:Label ID="lblbylocation" runat="server" Text="Search by address" meta:resourcekey="lblbylocationResource1"></asp:Label></div>
				<table>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblcounty" runat="server" Text="County:" meta:resourcekey="lblcountyResource1"></asp:Label></td>
						<td class="inputtd"><asp:DropDownList id="cmbcounty" runat="server" AutoPostBack="True" onselectedindexchanged="cmbcounty_SelectedIndexChanged" meta:resourcekey="cmbcountyResource1"></asp:DropDownList></td>
					</tr>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblcity" runat="server" Text="City:" meta:resourcekey="lblcityResource1"></asp:Label></td>
						<td class="inputtd">
							<asp:DropDownList id="cmbcity" runat="server" meta:resourcekey="cmbcityResource1"></asp:DropDownList></td>
					</tr>
				</table>
			</div>
			<div class="inputpanel">
				<asp:LinkButton id="cmdsearch" runat="server" onclick="cmdsearch_Click" meta:resourcekey="cmdsearchResource1">Search</asp:LinkButton></div>
			<div class="inputpanel">
				<asp:Literal id="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal>
			</div>
			<div class="inputpanel">
				<a target="main" href="javascript:getHotel();"><img border="0" src="/shared/images/buttons/btn_save.png"></a>&nbsp;&nbsp;<a href="javascript:window.close();"><img border="0" src="buttons/cancel_up.gif"></a></div>
			<input type="hidden" name="hotelid"><input type="hidden" name="hotelname">
		</form>
	</body>
</HTML>
