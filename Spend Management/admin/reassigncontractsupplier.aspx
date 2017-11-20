<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="admin_reassigncontractsupplier" Title="Reassign Contract" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="reassigncontractsupplier.aspx.cs" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
    <asp:ServiceReference Path="../svcAutoComplete.asmx" />
    </Services>
    </asp:ScriptManagerProxy>
    <script language="javascript" type="text/javascript">
    function TargetSupplier(source, eventArgs)
    {
        var supplier = document.getElementById('<%=txtNewSupplier.ClientID %>');
        PageMethods.ValidateSupplier(supplier.value, TargetSupplierComplete, OnTimeout, OnError);
    }
    
    function TargetSupplierComplete(retval)
    {
        var img = document.getElementById('<%=imgTargetSupplier.ClientID %>');
        if(retval == "1")
        {    
            if(img != null)
            {
                img.src = '../buttons/check.png';
            }
        }
        else
        {
            if(img != null)
            {
                img.src = '../buttons/delete2.png';
            }        
        }
    }
    
    function UpdateSupplier(source, eventArgs)
    {
        var contract = document.getElementById('<%=txtContract.ClientID%>');
        PageMethods.GetSupplierFromContract(contract.value, UpdateSupplierComplete, OnTimeout, OnError);
    }

    function UpdateSupplierComplete(retval)
    {
        var txt = document.getElementById('txtCurSupplier');
        var img = document.getElementById('<%=imgSource.ClientID %>');
        if(txt != null)
        {
            var suppStr = new String(retval);
            var supplierId = suppStr.substring(0,suppStr.indexOf('~'));
            document.getElementById('txtCurSupplier').setAttribute('value',suppStr.substring(suppStr.indexOf('~')+1));
            document.getElementById('supplierid').setAttribute('value',supplierId);
            
            if(img != null)
            {
                img.src = '../buttons/check.png';
            }
        }
        else
        {
            if(img != null)
            {
                img.src = '../buttons/delete2.png';
            }        
        }
    }
    
    function OnTimeout()
    {
        alert('Timed out waiting for response regarding contract information retrieval');
    }
    
    function OnError()
    {
        alert('An error occurred trying to retrieve contract information');
    }
    </script>
    <div class="comment" id="divComment" runat="server">
    <asp:Label runat="server" ID="lblMessage" meta:resourcekey="lblMessageResource1"></asp:Label>
    </div>
<div class="inputpanel">
<div class="inputpaneltitle">Reassign contract relationship</div>
    <table>
    <tr>
    <td class="labeltd">Contract</td>
    <td class="inputtd"><asp:TextBox runat="server" ID="txtContract" TabIndex="1" meta:resourcekey="txtContractResource1"></asp:TextBox>
        <asp:RequiredFieldValidator ID="reqContract" runat="server" ControlToValidate="txtContract"
            ErrorMessage="Contract selection is mandatory" SetFocusOnError="True" meta:resourcekey="reqContractResource1">**</asp:RequiredFieldValidator>
        <cc1:ValidatorCalloutExtender ID="reqexContract" runat="server" TargetControlID="reqContract" Enabled="True">
        </cc1:ValidatorCalloutExtender>
        <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" 
            ServiceMethod="GetContracts" ServicePath="../svcAutoComplete.asmx" 
            TargetControlID="txtContract" OnClientItemSelected="UpdateSupplier" 
            Enabled="True" MinimumPrefixLength="2">
        </cc1:AutoCompleteExtender>
    </td>
    <td>
    <asp:CompareValidator runat="server" ID="cmpSource" ControlToValidate="hiddenSourceId"
                                ErrorMessage="Invalid contract selection" Operator="GreaterThan"
                                Type="Integer" ValueToCompare="0">*</asp:CompareValidator>
                            <asp:Image runat="server" ID="imgSource" ImageUrl="~/Buttons/delete2.png" />
                            <asp:TextBox runat="server" ID="hiddenSourceId" Width="1px" Text="0"></asp:TextBox>
    </td>
    </tr>
    <tr>
    <td class="labeltd"><asp:Label runat="server" ID="lblCurSupplier" meta:resourcekey="lblCurSupplierResource1">current supplier</asp:Label></td>
    <td class="inputtd"><input type="text" ID="txtCurSupplier" readonly /><input type="hidden" id="supplierid" /></td>
    </tr>
    <tr>
    <td class="labeltd"><asp:Label runat="server" ID="lblNewSupplier" meta:resourcekey="lblNewSupplierResource1">new supplier</asp:Label></td>
    <td class="inputtd"><asp:TextBox runat="server" ID="txtNewSupplier" TabIndex="2" meta:resourcekey="txtNewSupplierResource1"></asp:TextBox>&nbsp;
        <asp:RequiredFieldValidator ID="reqNewSupplier" runat="server" ControlToValidate="txtNewSupplier"
            ErrorMessage="Supplier specification is mandatory" SetFocusOnError="True" meta:resourcekey="reqNewSupplierResource1">**</asp:RequiredFieldValidator>
        <cc1:ValidatorCalloutExtender ID="reqexNewSupplier" runat="server" TargetControlID="reqNewSupplier" Enabled="True">
        </cc1:ValidatorCalloutExtender>
        <cc1:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" 
            TargetControlID="txtNewSupplier" ServicePath="../svcAutoComplete.asmx" 
            ServiceMethod="GetSupplier" DelimiterCharacters="" Enabled="True" OnClientItemSelected="TargetSupplier" MinimumPrefixLength="2">
        </cc1:AutoCompleteExtender>
    </td>
    <td>
    <asp:CompareValidator runat="server" ID="cmpTargetSupplier" ErrorMessage="Invalid relationship selection" SetFocusOnError="true" Operator="GreaterThan" ValueToCompare="0" Type="Integer" ControlToValidate="hiddenTargetSupplier">*</asp:CompareValidator>
    <asp:Image runat="server" ID="imgTargetSupplier" ImageUrl="~/buttons/delete2.png" />
    <asp:TextBox runat="server" ID="hiddenTargetSupplier" Width="1px" Text="0"></asp:TextBox>
    </td>
    </tr>
    </table>
</div>
<div class="inputpanel">
<asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/Buttons/update.gif" AlternateText="Update" OnClick="cmdUpdate_Click" meta:resourcekey="cmdUpdateResource1" />
<asp:ImageButton runat="server"  ID="cmdCancel" ImageUrl="~/Buttons/cancel.gif" CausesValidation="False" AlternateText="Cancel" OnClick="cmdCancel_Click" meta:resourcekey="cmdCancelResource1" />
    <cc1:ConfirmButtonExtender ID="cmdexUpdate" runat="server" TargetControlID="cmdUpdate" ConfirmOnFormSubmit="True" ConfirmText="Click OK to confirm transfer action" Enabled="True">
    </cc1:ConfirmButtonExtender>
</div>
</asp:Content>

