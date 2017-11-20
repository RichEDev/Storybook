<%@ Page Language="C#" MasterPageFile="~/expform.master" AutoEventWireup="true" CodeBehind="mergelocations.aspx.cs" Inherits="expenses.admin.mergelocations" Title="Untitled Page" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <div class="inputpanel">
    		<div class="inputpaneltitle"><asp:Label id="Label2" runat="server" 
                    meta:resourcekey="Label2Resource1">Locations</asp:Label></div>
            <table>
            <tr><td class="labeltd" style="width: 100px" valign="top"><asp:Label id="lblToMerge" runat="server" meta:resourcekey="lblToMergeResource1">Locations to merge:</asp:Label></td><td class="inputtd">
                <asp:ListBox ID="cmbFromCompany" runat="server" Rows="15" 
                    SelectionMode="Multiple" meta:resourcekey="cmbFromCompanyResource1"></asp:ListBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server" 
                    ControlToValidate="cmbFromCompany" 
                    ErrorMessage="Please select a location to merge." MaximumValue="999999999" 
                    MinimumValue="1" Type="Integer"></asp:RangeValidator>
                </td></tr>
            <tr><td class="labeltd" style="width: 100px" valign="top"><asp:Label id="lblToMergeInto" runat="server" meta:resourcekey="lblToMergeIntoResource1">Location to merge into:</asp:Label></td><td class="inputtd">
                <asp:DropDownList ID="cmbToCompany" runat="server" 
                    meta:resourcekey="cmbToCompanyResource1"></asp:DropDownList>
                <asp:RangeValidator ID="rvLocMergeInto" runat="server" 
                    ControlToValidate="cmbToCompany" 
                    ErrorMessage="Please select a location to merge into." MaximumValue="999999999" 
                    MinimumValue="1" Type="Integer"></asp:RangeValidator>
                </td></tr>
            </table>
            <div><asp:Label ID="lblError" runat="server" meta:resourcekey="lblErrorResource1" ForeColor="Red"></asp:Label>
            </div>
            <div class="inputpanel"><asp:ImageButton id="cmdok" runat="server" 
                    ImageUrl="~/shared/images/buttons/btn_save.png" onclick="cmdok_Click" 
                    meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;<asp:ImageButton 
                    id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" 
                    CausesValidation="False" onclick="cmdcancel_Click" 
                    meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>
    </div>
</asp:Content>
