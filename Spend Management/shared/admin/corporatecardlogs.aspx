<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" CodeBehind="corporatecardlogs.aspx.cs" Inherits="Spend_Management.shared.admin.corporatecardlogs" EnableViewState="false" EnableSessionState="True"%>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.corporatecardlogs.js"/>
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js"/>
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel">
        
        <asp:Panel runat="server" ID="gridCardsLog">   
            <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        </asp:Panel>

        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div> 
    </div>

    <div id="divViewLogsModal" class="sm_panel" style="display: none;">
        <textarea id="txtAreaViewLogs" class="modal-body" readonly="readonly"></textarea>
    </div>

</asp:Content>
