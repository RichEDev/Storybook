<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="receipts.ascx.cs" Inherits="Spend_Management.Receipts" %>

<asp:ScriptManagerProxy runat="server" ID="scriptManager">
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.receipts.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<p id="ReceiptDeletionReason"><%= this.ReceiptDeletionReason %></p>
<div runat="server" ID="receiptsList" class="receiptsList colorbox">
    <p id="thumbnailsEmptyMessage" class="emptyMessage" runat="server">There are currently no associated receipts.</p>
    <ul>
        <asp:Repeater runat="server" OnItemDataBound="ReceiptsViewItemDataBound" ID="receiptsRepeater">
            <ItemTemplate>
                <li data-id="<%# Eval("receiptid").ToString().ToLower() %>">
                    <asp:MultiView ID="receiptMultiView" runat="server" ActiveViewIndex="0">
                        <!-- the default item template, for receipts that are images-->
                        <asp:View ID="View1" runat="server">
                            <a class="receiptImage" href="<%# Eval("filename") %>">
                                <img src="<%# Eval("filename") %>" />
                            </a>
                        </asp:View>
                        <!-- template for receipts that aren't images-->
                        <asp:View ID="View2" runat="server">
                            <a class="receiptOther" href="<%# Eval("filename") %>">
                                <div class="noReceiptImage <%# Eval("extension").ToString().ToLower() %>"></div>
                            </a>
                        </asp:View>
                    </asp:MultiView>
                    <asp:HyperLink ID="Delete" runat="server" Visible='<%# this.CanDeleteReceipts %>' NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "receiptid", "{0}") %>' CssClass="deleteReceipt" ToolTip="Delete Receipt"/>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>
<div runat="server" id="receiptsBackgroundList" class="receiptsBackgroundList colorbox" visible="false"></div>