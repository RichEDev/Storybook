<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="false"
    Inherits="frameworkWebApp.Framework2006.SummaryPortal" Codebehind="SummaryPortal.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">

    <asp:ScriptManagerProxy ID="smp" runat="server">
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">
        function ShowResults()
        {
            var cntl = document.getElementById('searchresults');
            if(cntl != null)
            {
                cntl.style.display = 'block';
            }
        }
    </script>

    <asp:Panel runat="server" ID="SearchFieldsPanel">
        <div class="formpanel formpanel_padding">
            <asp:Label ID="lblErrorStatus" runat="server" ForeColor="Red"></asp:Label>
        </div>
        <igmisc:WebPanel ID="igContractPanel" runat="server" CssClass="inputpanel">
            <Template>
                <table>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblContractCategory" runat="server">Contract Category</asp:Label>
                        </td>
                        <td class="inputtd">
                            <asp:DropDownList ID="lstContractCategory" runat="server" TabIndex="7">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="cmdSearchContractCategory" runat="server" AlternateText="Find" CssClass="advance_search_icon_spacing"
                                ImageUrl="./icons/16/plain/find.png" />
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip435" onclick="SEL.Tooltip.Show('6bee20d8-3fa3-49f8-817c-300610d75568', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            Contract&nbsp;Type
                        </td>
                        <td class="inputtd">
                            <asp:DropDownList ID="lstContractType" runat="server" TabIndex="8">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="cmdSearchContractType" runat="server" CssClass="advance_search_icon_spacing" AlternateText="Find" ImageUrl="./icons/16/plain/find.png" />
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip438" onclick="SEL.Tooltip.Show('6ce43582-716d-426e-866d-efeaffbff171', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td class="labeltd">
                            Contract Status Returned
                        </td>
                        <td class="inputradio">
                            <asp:RadioButtonList ID="rdoContractStatus" runat="server" RepeatDirection="Horizontal"
                                TabIndex="9">
                                <asp:ListItem Value="N">Live</asp:ListItem>
                                <asp:ListItem Value="Y">Archived</asp:ListItem>
                                <asp:ListItem Value="B">All</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td style="width: 16px;">
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip480" onclick="SEL.Tooltip.Show('16dd9212-2ba6-41b6-8d70-220b345e43d4', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            Include Variations?
                        </td>
                        <td class="inputtd">
                            <asp:CheckBox runat="server" ID="chkIncludeVariations" />
                        </td>
                        <td style="width: 16px;">
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip481" onclick="SEL.Tooltip.Show('67d448ea-4871-4794-8724-88c3073534d6', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                </table>
            </Template>
            <Header Text="Contract Search">
                <ExpandedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </ExpandedAppearance>
                <CollapsedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </CollapsedAppearance>
            </Header>
        </igmisc:WebPanel>
        <igmisc:WebPanel ID="igUFSearchPanel" runat="server" CssClass="inputpanel" Visible="false">
            <Template>
                <asp:Panel runat="server" ID="UFSearchPanel">
                </asp:Panel>
            </Template>
            <Header Text="Bespoke Search">
                <ExpandedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </ExpandedAppearance>
                <CollapsedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </CollapsedAppearance>
            </Header>
        </igmisc:WebPanel>
        <igmisc:WebPanel ID="igLinkPanel" runat="server" CssClass="inputpanel" Expanded="False">
            <Template>
                <table>
                    <tr>
                        <td class="labeltd">
                            Contract Link Definitions
                        </td>
                        <td class="inputtd">
                            <asp:DropDownList ID="lstLinkDefs" runat="server" AutoPostBack="True" TabIndex="2">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 16px;">
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip439" onclick="SEL.Tooltip.Show('5b65f062-5ee7-4249-82e6-3c4522dc6717', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                </table>
            </Template>
            <Header Text="Link Definition Search">
                <ExpandedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </ExpandedAppearance>
                <CollapsedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </CollapsedAppearance>
            </Header>
        </igmisc:WebPanel>
        <igmisc:WebPanel runat="server" ID="igInvPanel" Expanded="false" CssClass="inputpanel">
            <Template>
                <table>
                    <tr>
                        <td class="labeltd">
                            Invoice Number
                        </td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtInvoiceNo" runat="server" TabIndex="10"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton runat="server" ID="cmdInvNo" CssClass="advance_search_icon_spacing" ImageUrl="./icons/16/plain/find.png" />
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip441" onclick="SEL.Tooltip.Show('618f9379-d039-42c0-862e-065a0553a6c8', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                    <tr>
                        <td class="labeltd">
                            PO Number
                        </td>
                        <td class="inputtd">
                            <asp:TextBox ID="txtPONumber" runat="server" TabIndex="11"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton runat="server" ID="cmdPONumber" CssClass="advance_search_icon_spacing" ImageUrl="./icons/16/plain/find.png" />
                        </td>
                        <td>
                            <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip442" onclick="SEL.Tooltip.Show('12fe44a5-e3aa-4675-aef1-b9369ea11e77', 'fw', this);"
                                class="tooltipicon" />
                        </td>
                    </tr>
                </table>
            </Template>
            <Header Text="Invoice &amp; PO Search">
                <ExpandedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </ExpandedAppearance>
                <CollapsedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </CollapsedAppearance>
            </Header>
        </igmisc:WebPanel>
        <div class="formpanel formpanel_padding">
            <asp:ImageButton runat="server" ID="cmdClose" AlternateText="Close" ImageUrl="~/Buttons/page_close.png"
                CausesValidation="false" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="ResultsPanel" Visible="false">
        <div class="formpanel formpanel_padding">
            <div class="sectiontitle">
                Search Results</div>
            <div style="padding-top: 5px;">
                <asp:Literal ID="litResults" runat="server"></asp:Literal>
            </div>
        <div class="formbuttons">
            <asp:ImageButton  runat="server" ID="cmdSearch" 
                ImageUrl="./buttons/page_close.png" OnClick="cmdSearch_Click" />
        </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="contentmenu">
</asp:Content>
