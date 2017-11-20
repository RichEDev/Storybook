<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="admin_recharge_impactanalysis" Title="Recharge Impact Analysis" Culture="auto"  meta:resourcekey="PageResource1" UICulture="auto" Codebehind="recharge_impactanalysis.aspx.cs" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="../svcAutoComplete.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <div class="inputpanel">
        <div class="inputpaneltitle">
            Recharge Impact Analysis</div>
        <asp:UpdatePanel runat="server" ID="SelectionUpdatePanel">
            <ContentTemplate>
                <table>
                    <tr>
                        <td class="labeltd">
                            <asp:Label runat="server" ID="lblRechargeClient" meta:resourcekey="lblRechargeClientResource1"></asp:Label></td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="txtRechargeClient" TabIndex="1" ValidationGroup="validatefilter"
                                meta:resourcekey="txtRechargeClientResource1"></asp:TextBox></td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="txtRechargeClient"
                                ErrorMessage="Field is mandatory" SetFocusOnError="True" ValidationGroup="validatefilter"
                                meta:resourcekey="reqClientResource1">**</asp:RequiredFieldValidator>
                            <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" ServiceMethod="GetClients"
                                ServicePath="../svcAutoComplete.asmx" TargetControlID="txtRechargeClient" DelimiterCharacters=""
                                Enabled="True">
                            </cc1:AutoCompleteExtender>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblStatus" meta:resourcekey="lblStatusResource1"></asp:Label></td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="inputpanel">
                <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="~/Buttons/ok.gif" OnClick="cmdOK_Click"
                    meta:resourcekey="cmdOKResource1" TabIndex="2" />
            </div>
            <asp:Panel runat="server" ID="panelResults" Visible="False" meta:resourcekey="panelResultsResource1">
                <div class="inputpanel">
                    <igtbl:UltraWebGrid ID="igResultsGrid" runat="server" OnInitializeLayout="igResultsGrid_InitializeLayout"
                        OnInitializeRow="igResultsGrid_InitializeRow" Browser="Xml"
                        meta:resourcekey="igResultsGridResource1" SkinID="igGridSkin">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout Name="ctl17xigResultsGrid">
                            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
                                BorderStyle="Solid" BorderWidth="1px" Font-Names="Microsoft Sans Serif" 
                                Font-Size="8.25pt">
                            </FrameStyle>
                            <Pager MinimumPagesForDisplay="2">
                                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                                    WidthTop="1px" />
                                </PagerStyle>
                            </Pager>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                                    WidthTop="1px" />
                            </FooterStyleDefault>
                            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" 
                                HorizontalAlign="Left">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                                    WidthTop="1px" />
                            </HeaderStyleDefault>
                            <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
                                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                                <Padding Left="3px" />
                                <BorderDetails ColorLeft="Window" ColorTop="Window" />
                            </RowStyleDefault>
                            <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                            </GroupByRowStyleDefault>
                            <GroupByBox>
                                <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                                </BoxStyle>
                            </GroupByBox>
                            <AddNewBox Hidden="False">
                                <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" 
                                    BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
                                        WidthTop="1px" />
                                </BoxStyle>
                            </AddNewBox>
                            <ActivationObject BorderColor="" BorderWidth="">
                            </ActivationObject>
                            <FilterOptionsDefault>
                                <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
                                    BorderWidth="1px" CustomRules="overflow:auto;" 
                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Height="300px" 
                                    Width="200px">
                                    <Padding Left="2px" />
                                </FilterDropDownStyle>
                                <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                                </FilterHighlightRowStyle>
                                <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
                                    BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px">
                                    <Padding Left="2px" />
                                </FilterOperandDropDownStyle>
                            </FilterOptionsDefault>
                        </DisplayLayout>
                    </igtbl:UltraWebGrid>
                </div>
                <div class="inputpanel">
                    <div class="inputpaneltitle">
                        <asp:Label runat="server" ID="lblSupportEndDate" meta:resourcekey="lblSupportEndDateResource1"></asp:Label></div>
                    <table>
                        <tr>
                            <td class="labeltd">
                                <asp:Label runat="server" ID="lblEndDate" meta:resourcekey="lblEndDateResource1"></asp:Label></td>
                            <td class="inputtd">
                                <asp:TextBox runat="server" ID="txtEndDate" meta:resourcekey="txtEndDateResource1" ValidationGroup="vgEndDate"></asp:TextBox></td>
                            <td>
                                <cc1:CalendarExtender ID="calexEndDate" runat="server" Format="dd/MM/yyyy" TargetControlID="txtEndDate">
                                </cc1:CalendarExtender>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ErrorMessage="Specification of the end date is mandatory"
                                    Text="**" ValidationGroup="vgEndDate" ControlToValidate="txtEndDate" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                <cc1:ValidatorCalloutExtender ID="reqexEndDate" runat="server" TargetControlID="reqEndDate">
                                </cc1:ValidatorCalloutExtender>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="inputpanel">
                    <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="~/Buttons/update.gif" meta:resourcekey="cmdUpdateResource1"
                        OnClick="cmdUpdate_Click" ValidationGroup="vgEndDate" />
                    <cc1:ConfirmButtonExtender ID="cnfexUpdate" runat="server" TargetControlID="cmdUpdate"
                        ConfirmText="Click OK to confirm Support End Date to be applied to ALL items" Enabled="True">
                    </cc1:ConfirmButtonExtender>
                    <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="~/Buttons/cancel.gif" CausesValidation="False" ValidationGroup="vgEndDate"
                        OnClick="cmdCancel_Click" meta:resourcekey="cmdCancelResource1" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="~/Buttons/page_close.gif"
            AlternateText="Close" CausesValidation="false" OnClick="cmdClose_Click" TabIndex="3" />
    </div>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <div class="progresspanel">
                <img src="../images/loading.gif" alt="Please wait, loading..." />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
