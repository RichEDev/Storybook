
<%@ Page validateRequest="false" language="c#" Inherits="expenses.admin.adminpolicy" MasterPageFile="~/expform.master" Codebehind="adminpolicy.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

	<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	    <script type="text/javascript">
	        $(document).ready(function () {
	            $('#<%=polfile.ClientID%>').change(function () {
	                var filename = $(this).val();
	                if (! /\.ht(m|ml)?$/.test(filename)) {
	                    $(this).val('');
	                    SEL.MasterPopup.ShowMasterPopup('Please select an HTM or HTML file.', 'Message from ' + moduleNameHTML);
	                    var new_val = $(this).val();
	                    if (new_val !== "") {
	                        $(this).replaceWith($(this).clone(true));
	                    }
	                }
	            });
                $('#<%=pdffile.ClientID%>').change(function () {
	                var filename = $(this).val();
	                if (! /\.pdf$/.test(filename)) {
	                    $(this).val('');
	                    SEL.MasterPopup.ShowMasterPopup('Please select a PDF file.', 'Message from ' + moduleNameHTML);
	                    var new_val = $(this).val();
	                    if (new_val !== "") {
	                        $(this).replaceWith($(this).clone(true));
	                    }

	                }
	            });
	        });
    	</script>
		<div class="valdiv">
			<asp:ValidationSummary id="ValidationSummary1" runat="server" Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
			<asp:label id="lblmsg" runat="server" Font-Size="Small" ForeColor="Red" Visible="False" meta:resourcekey="lblmsgResource1">Label</asp:label></div>
		<div class="inputpanel">
			<div class="inputpaneltitle">
				<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">How would you like to enter your company policy?</asp:Label></div>
			<table>
				<tr>
					<td class="labeltd">
						<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Enter company policy into a free format text box:</asp:Label></td>
					<td class="inputtd"><asp:radiobutton id="optfreeformat" runat="server" AutoPostBack="True" GroupName="policy" Width="228px"
							TextAlign="Left" oncheckedchanged="optfreeformat_CheckedChanged" meta:resourcekey="optfreeformatResource1"></asp:radiobutton></td>
				</tr>
				<tr>
					<td class="labeltd">
						<asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Upload company policy as a HTML file:</asp:Label></td>
					<td class="inputtd"><asp:radiobutton id="opthtml" runat="server" AutoPostBack="True" GroupName="policy" Width="188px"
							TextAlign="Left" oncheckedchanged="opthtml_CheckedChanged" meta:resourcekey="opthtmlResource1"></asp:radiobutton></td>
				</tr>
                <tr>
					<td class="labeltd">
						<asp:Label id="Label4" runat="server" meta:resourcekey="Label4Resource1">Upload company policy as a PDF file:</asp:Label></td>
					<td class="inputtd"><asp:radiobutton id="optPdf" runat="server" AutoPostBack="True" GroupName="policy" Width="188px"
							TextAlign="Left" oncheckedchanged="optpdf_CheckedChanged" meta:resourcekey="opthtmlResource1"></asp:radiobutton></td>
				</tr>
			</table>
		</div>
		<div class="inputpanel">
			<tr>
				<td colSpan="2">
                    <asp:label id="lblenter" runat="server">Please enter the location of the HTML file:</asp:label>&nbsp;<asp:textbox id="txtpolicy" runat="server" Width="187px"></asp:textbox>
                    <div style="height:10px;"></div>
				    <input type="file" runat="server" id="polfile" NAME="polfile"/><asp:requiredfieldvalidator id="reqhtml" runat="server" ErrorMessage="Please enter a valid file name" ControlToValidate="polfile">*</asp:requiredfieldvalidator>
                    <input type="file" runat="server" id="pdffile" NAME="pdffile"/><asp:requiredfieldvalidator id="reqpdf" runat="server" ErrorMessage="Please enter a valid file name" ControlToValidate="pdffile">*</asp:requiredfieldvalidator>

				</td>
			</tr>
		</div>
		<div class="inputpanel"><asp:imagebutton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png"></asp:imagebutton>&nbsp;&nbsp;
			<asp:imagebutton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False"></asp:imagebutton></div>
	

    </asp:Content>


<asp:Content ID="Content3" runat="server" contentplaceholderid="contentmenu">

                
</asp:Content>



