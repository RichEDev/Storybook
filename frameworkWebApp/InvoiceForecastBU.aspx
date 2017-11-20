<%@ Page Language="vb" AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.InvoiceForecastBU" Codebehind="InvoiceForecastBU.aspx.vb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Invoice Forecast Bulk Update</title>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table class="main" align="center">
				<tr>
					<td class="main" align="center">
						<asp:Label id="lblTitle" runat="server" CssClass="screentitle">Bulk Update</asp:Label></td>
				</tr>
				<tr>
					<td class="main"></td>
				</tr>
				<tr>
					<td class="main" align="center">
						<asp:Literal id="litForecastList" runat="server"></asp:Literal></td>
				</tr>
				<tr>
					<td class="main"></td>
				</tr>
				<tr>
					<td class="main"></td>
				</tr>
				<tr>
					<td class="main">
						<asp:PlaceHolder id="holderUpdateButton" runat="server"></asp:PlaceHolder></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
