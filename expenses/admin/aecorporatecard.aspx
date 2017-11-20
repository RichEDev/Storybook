<%@ Page Language="C#" MasterPageFile="~/expform.master" AutoEventWireup="true" CodeBehind="aecorporatecard.aspx.cs" Inherits="expenses.admin.aecorporatecard" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/expform.master" %>
<asp:Content ID="CustomeStyles" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .labeltd {
            padding-bottom: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <div class="inputpanel">
        <table>
            <tr><td class="labeltd">Card Provider</td><td class="inputtd" id="provider">
                <asp:DropDownList ID="cmbprovider" runat="server">
                </asp:DropDownList>
            </td></tr>
            <tr id="fileIdentifier">
                <td class="labeltd mandatory">Card Provider Customer Identifier*</td>
                <td class="inputtd">
                    <asp:TextBox runat="server" ID="txtFileIdentifier"></asp:TextBox>
                </td>
                <td><img id="imgtooltipFileIdentifier" onclick="SEL.Tooltip.Show('96676A7F-86D4-46F8-97F5-5E94D29B73FA', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
            </tr>
            <tr><td class="labeltd">Claimant settles own bill</td><td class="inputtd">
                <asp:CheckBox ID="chkclaimantsettlesbill" runat="server" /></td></tr>
            <tr><td class="labeltd">
                <asp:Label ID="lblpcardassign" runat="server" Text="Transactions should automatically be assigned to the following item:" meta:resourcekey="lblpcardassignResource1"></asp:Label></td><td class="inputtd">
                <asp:DropDownList ID="cmbassignitem" runat="server" meta:resourcekey="cmbpurchasecardsubcatidResource1">
                </asp:DropDownList></td><td><img id="imgtooltip346" onclick="SEL.Tooltip.Show('725a8d88-b58f-4302-8eca-b359b054aa62', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td></tr>
			<tr>
			    <td class="labeltd">Do not allow items from different statements to be reconciled on the same claim</td>
			    <td class="inputtd">
                    <asp:CheckBox ID="chksingleclaim" runat="server" meta:resourcekey="chksingleclaimccResource1" /></td><td><img id="imgtooltip347" onclick="SEL.Tooltip.Show('78f48d6d-72a8-45bf-b6db-448ab68fe6ba', 'ex', this);" src="../static/icons/16/new-icons/tooltip.png" alt="" class="tooltipicon"/></td>
			</tr>
			<tr><td class="labeltd">Do not allow cash items to be submitted if there are outstanding credit card items to be reconciled</td><td class="inputtd">
                <asp:CheckBox ID="chkblockcash" runat="server" /></td></tr>
            <tr><td class="labeltd">
                <asp:Label ID="Label1" runat="server" Text="Do not allow unmatched items to be submitted"></asp:Label></td><td class="inputtd">
                    <asp:CheckBox ID="chkblockunmatched" runat="server" /></td></tr>
        </table>
    </div>
    <div class="inputpanel">
        <asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" 
            onclick="cmdok_Click" />&nbsp;&nbsp;<a href="corporate_cards.aspx"><img alt="Cancel" src="../buttons/cancel_up.gif" /></a>
    </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        function showHideIdentifier() {
            var auto = $("#provider select option:selected").attr('auto');
            if (auto === "True") {
                $('#fileIdentifier').show();
            } else {
                $('#fileIdentifier').hide();
            }
        }

        $(document).ready(function () {
            showHideIdentifier();
            $("#provider select").change(function () {
                showHideIdentifier();
            });
            $("#ctl00_contentmain_cmdok").click(function() {
                if ($("#provider select option:selected").attr('auto') === "True" &&
                    $("#ctl00_contentmain_txtFileIdentifier").val().trim() === "") {
                    SEL.MasterPopup.ShowMasterPopup("Card Provider Customer Identifier is a mandatory field. Please enter a value in the box provided.");
                    return false;
                }
            });
        });

    </script>
</asp:Content>
