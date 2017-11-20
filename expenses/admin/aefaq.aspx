<%@ Page validateRequest="false" language="c#" Inherits="expenses.admin.aefaq" MasterPageFile="~/expform.master" Codebehind="aefaq.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

<script type="text/javascript" language="javascript">
function checkLength(sender, args) {
var tip = document.getElementById(contentID + "txttip");
    if(tip.value.length> 200) {
        args.IsValid = false;
        return false;    
    } else {
        args.IsValid = true;
        return true;
    }
}

</script>
	<div class="valdiv"><asp:ValidationSummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary></div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
		<table>
			<tr>
				<td class="labeltd">
					<asp:Label id="lblcategory" runat="server" meta:resourcekey="lblcategoryResource1">Category</asp:Label></td>
				<td class="inputtd">
					<asp:DropDownList id="cmbcategory" runat="server" meta:resourcekey="cmbcategoryResource1"></asp:DropDownList></td>
				<td><asp:RequiredFieldValidator  id="RequiredFieldValidator1" runat="server" ControlToValidate="cmbcategory" ErrorMessage="RequiredFieldValidator" meta:resourcekey="RequiredFieldValidator1Resource1"></asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top"><asp:Label id="lblquestion" runat="server" meta:resourcekey="lblquestionResource1">Question</asp:Label></td>
				<td class="inputtd"><asp:TextBox id="txtquestion" runat="server" TextMode="MultiLine" Width="500px" Height="120px" MaxLength="4000" meta:resourcekey="txtquestionResource1"></asp:TextBox></td>
				<td><asp:RequiredFieldValidator id="requestion" runat="server" ErrorMessage="Please enter the question" ControlToValidate="txtquestion" meta:resourcekey="requestionResource1">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="labeltd" valign="top"><asp:Label id="lblanswer" runat="server" meta:resourcekey="lblanswerResource1">Answer</asp:Label></td>
				<td class="inputtd"><asp:TextBox id="txtanswer" runat="server" TextMode="MultiLine" Width="500px" Height="120px" MaxLength="4000" meta:resourcekey="txtanswerResource1"></asp:TextBox></td>
				<td><asp:RequiredFieldValidator id="reqanswer" runat="server" ErrorMessage="Please enter an answer to this question" ControlToValidate="txtanswer" meta:resourcekey="reqanswerResource1">*</asp:RequiredFieldValidator></td>
				
			</tr>
			<tr>
				<td class="labeltd" valign="top"><asp:Label id="lbltip" runat="server" meta:resourcekey="lbltipResource1">Tip</asp:Label>:</td>
				<td class="inputtd"><asp:TextBox id="txttip" runat="server" TextMode="MultiLine" Width="500px" Height="50px" MaxLength="200" meta:resourcekey="txttipResource1"></asp:TextBox></td>
    			<td><asp:CustomValidator ID="cvTip" runat="server" ErrorMessage="The maximum length of a tip is 200 characters" ControlToValidate="txttip" ClientValidationFunction="checkLength"></asp:CustomValidator></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" CausesValidation="False" ImageUrl="../buttons/cancel_up.gif" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>

   </asp:Content>

