<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master"
    Inherits="admin_reassignconprods" Title="Contract Product Re-assignment" Culture="auto"
    meta:resourcekey="PageResource1" UICulture="auto" Codebehind="reassignconprods.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
    <asp:UpdatePanel runat="server" ID="menuUpdatePanel">
        <ContentTemplate>
            <asp:LinkButton runat="server" ID="lnkSelectAll" CssClass="submenuitem" Visible="False"
                CausesValidation="False" OnClick="lnkSelectAll_Click" meta:resourcekey="lnkSelectAllResource1"></asp:LinkButton>
            <asp:LinkButton runat="server" ID="lnkDeselectAll" CssClass="submenuitem" Visible="False"
                CausesValidation="False" OnClick="lnkDeselectAll_Click" meta:resourcekey="lnkDeselectAllResource1"></asp:LinkButton>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">

    <script language="javascript" type="text/javascript">
    function ChangeIcon(iconId)
    {
        
    }
    
function doCheck()
{
    var chkCount = 0;
    var chk_cntl = document.getElementsByName('chk');
    if(chk_cntl != null)
    {
        for(var i=0; i<chk_cntl.length; i++)
        {
            var chk = document.getElementById('chk' + chk_cntl[i].value);
            if(chk != null)
            {
                if(chk.checked == true)
                {
                    chkCount++;
                }
            }
        }
    }
    
    var status = document.getElementById('hiddenAbortStatus');
    if(confirm('Click OK to confirm reassignment of ' + chkCount + ' items'))
    {
        if(status != null)
        {
            status.value = '1';
        }
    }
    else
    {
        if(status != null)
        {
            status.value = '0';
        }
    }
}
    </script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="../svcAutoComplete.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ShowMessageBox="True" ShowSummary="False" />
    <input type="hidden" id="hiddenAbortStatus" name="hiddenAbortStatus" />
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Source and Target Contracts</div>
        <asp:UpdatePanel runat="server" ID="updatePanelGetData">
            <ContentTemplate>
                <table>
                    <tr>
                        <td class="labeltd">
                            Source Contract
                        </td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtSource" meta:resourcekey="txtSourceResource1"
                                AutoPostBack="True" OnTextChanged="txtSource_TextChanged"></asp:TextBox>&nbsp;
                            <asp:RequiredFieldValidator ID="reqSource" runat="server" ControlToValidate="txtSource"
                                ErrorMessage="Field is mandatory" SetFocusOnError="True" meta:resourcekey="reqSourceResource1">*</asp:RequiredFieldValidator>
                            <cc1:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" ServiceMethod="GetContracts"
                                MinimumPrefixLength="2" ServicePath="../svcAutoComplete.asmx" TargetControlID="txtSource"
                                Enabled="True">
                            </cc1:AutoCompleteExtender>
                            <cc1:ValidatorCalloutExtender ID="valexSource" runat="server" TargetControlID="reqSource"
                                Enabled="True">
                            </cc1:ValidatorCalloutExtender>
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
                        <td class="labeltd">
                            Target Contract
                        </td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtTarget" meta:resourcekey="txtTargetResource1"
                                AutoPostBack="True" OnTextChanged="txtTarget_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqTarget" runat="server" ControlToValidate="txtTarget"
                                ErrorMessage="Field is mandatory" SetFocusOnError="True" meta:resourcekey="reqTargetResource1">*</asp:RequiredFieldValidator>
                            <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" ServiceMethod="GetContracts"
                                MinimumPrefixLength="2" ServicePath="../svcAutoComplete.asmx" TargetControlID="txtTarget"
                                Enabled="True">
                            </cc1:AutoCompleteExtender>
                            <cc1:ValidatorCalloutExtender ID="valexTarget" runat="server" TargetControlID="reqTarget"
                                Enabled="True">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                        <td>
                            <asp:CompareValidator ID="cmpTarget" runat="server" ControlToValidate="hiddenTargetId"
                                ErrorMessage="Invalid contract selection" Operator="GreaterThan"
                                Type="Integer" ValueToCompare="0">*</asp:CompareValidator>
                            <asp:Image runat="server" ID="imgTarget" ImageUrl="../Buttons/delete2.png" />
                            <asp:TextBox Width="1px" runat="server" ID="hiddenTargetId"
                                Text="0"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblCPFilter" runat="server" meta:resourcekey="lblCPFilterResource1"></asp:Label>
                        </td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtCPFilter" meta:resourcekey="txtCPFilterResource1"></asp:TextBox>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="~/Buttons/update.gif" OnClick="cmdOK_Click"
            meta:resourcekey="cmdOKResource1" />&nbsp;
        <asp:ImageButton runat="server" ID="cmdGetDataCancel" ImageUrl="~/Buttons/cancel.gif"
            CausesValidation="false" OnClick="cmdGetDataCancel_Click" />
    </div>
    <asp:UpdatePanel runat="server" ID="resultUpdatePanel">
        <ContentTemplate>
            <div class="comment" id="divMessage" runat="server">
                <asp:Label runat="server" ID="lblMessage" meta:resourcekey="lblMessageResource1"></asp:Label>
            </div>
            <div class="inputpanel">
                <asp:Panel runat="server" ID="resultPanel" meta:resourcekey="resultPanelResource1">
                </asp:Panel>
            </div>
            <div class="inputpanel">
                <asp:ImageButton runat="server" ID="btnMove" ImageUrl="~/Buttons/update.gif" 
                    onclick="btnMove_Click" Visible="False" />&nbsp;
                <asp:ImageButton runat="server" ID="btnMoveCancel" 
                    ImageUrl="~/Buttons/cancel.gif" CausesValidation="false" Visible="False" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
