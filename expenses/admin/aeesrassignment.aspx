<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aeesrassignment.aspx.cs" MasterPageFile="~/expform.master" Inherits="expenses.admin.aeesrassignment" %>

<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <div class="valdiv" id="valdiv">
		<asp:validationsummary id="ValidationSummary1" runat="server" Width="100%" meta:resourcekey="ValidationSummary1Resource1"></asp:validationsummary>
		<asp:Label id="lblmsg" runat="server" Visible="False" Font-Size="Small" ForeColor="Red" meta:resourcekey="lblmsgResource1">Label</asp:Label>
	</div>

    <div class="inputpanel">
        <div class="inputpaneltitle">
        <asp:Label id="lblESRAssignments" runat="server" meta:resourcekey="lblESRAssignmentsResource1">Add ESR Assignment</asp:Label></div>
    </div>
    
    <div class="inputpanel">
        <table>
            <tr>
                <td>
                    
                    <asp:Label id="lblESRAssignment" runat="server" CssClass="labeltd" meta:resourcekey="lblESRAssignmentResource1">ESR Assignment Number:</asp:Label>
                </td>
                <td>  
                    <asp:TextBox ID="txtESRNum" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valESRNum" runat="server" ErrorMessage="Please enter a value for the Assignment Number." ControlToValidate="txtESRNum"></asp:RequiredFieldValidator>
                </td>    
            </tr>
        </table>
    </div>
    
    <div class="inputpanel">
        <asp:ImageButton id="cmdok" runat="server" ImageUrl="../buttons/ok_up.gif" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
	    <asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton>
    </div>

</asp:Content>
