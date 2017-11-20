<%@ Page MasterPageFile="~/FWMaster.master" Language="vb"
    AutoEventWireup="false" Inherits="Framework2006.InvoiceForecastGenerate" CodeFile="InvoiceForecastGenerate.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <asp:UpdatePanel runat="server" ID="forecast_UpdatePanel">
        <ContentTemplate>
            <div class="inputpanel">
                <div class="inputpaneltitle">
                    Invoice Forecasts<asp:Label ID="lblIFTitle" runat="server"></asp:Label></div>
                <table>
                    <tr>
                        <td class="labeltd">
                            <asp:Label ID="lblFirstDate" runat="server">start date</asp:Label></td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="dateFirst"></asp:TextBox>
                        </td>
                        <td><asp:ImageButton runat="server" ID="btnStartDate" 
                                ImageUrl="~/icons/16/plain/calendar.gif"  CausesValidation="false" />
                                                    <cc1:CalendarExtender ID="calexStartDate" runat="server" Format="dd/MM/yyyy" 
                                TargetControlID="dateFirst" PopupButtonID="btnStartDate" PopupPosition="BottomRight">
                            </cc1:CalendarExtender>
            <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ErrorMessage="Start Date field is mandatory" ControlToValidate="dateFirst" SetFocusOnError="True" Text="*" ValidationGroup="daterange"></asp:RequiredFieldValidator>           
                            <asp:CompareValidator ID="cmpFirstDate" runat="server" 
                                ControlToValidate="dateFirst" ErrorMessage="Invalid Date format provided" 
                                Operator="DataTypeCheck" SetFocusOnError="True" Type="Date">*</asp:CompareValidator>
                            <cc1:ValidatorCalloutExtender ID="cmpexStartDate" runat="server" 
                                TargetControlID="cmpFirstDate">
                            </cc1:ValidatorCalloutExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexStartDate" runat="server" 
                                TargetControlID="reqStartDate">
                            </cc1:ValidatorCalloutExtender>
                        
                        </td>
                        <td class="labeltd">
                            <asp:Label ID="lblSecondDate" runat="server">end date</asp:Label></td>
                        <td class="inputtd">
                            <asp:TextBox runat="server" ID="dateSecond"></asp:TextBox>
                            
                        </td>
                        <td><asp:ImageButton runat="server" ID="btnEndDate" CausesValidation="false" ImageUrl="~/icons/16/plain/calendar.gif" />
                                                <cc1:CalendarExtender ID="calexEndDate" runat="server" Format="dd/MM/yyyy" 
                                TargetControlID="dateSecond" PopupButtonID="btnEndDate" PopupPosition="BottomRight">
                            </cc1:CalendarExtender><asp:RequiredFieldValidator ID="reqEndDate" runat="server" ForeColor="Red" ErrorMessage="End Date field is mandatory" ControlToValidate="dateSecond" SetFocusOnError="True" Text="*" ValidationGroup="daterange"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cmpSEDates" runat="server" ForeColor="Red" Type="Date" Operator="LessThan" ControlToCompare="dateSecond" ControlToValidate="dateFirst" ErrorMessage="End date must be later than Start Date" SetFocusOnError="True" Text="*" ValidationGroup="daterange"></asp:CompareValidator>
                            <asp:CompareValidator ID="cmpSecondDate" runat="server" 
                                ControlToValidate="dateSecond" ErrorMessage="Invalid Date format provided" 
                                Operator="DataTypeCheck" SetFocusOnError="True" Type="Date">*</asp:CompareValidator>
                            <cc1:ValidatorCalloutExtender ID="cmpexSecondDate" runat="server" 
                                TargetControlID="cmpSecondDate">
                            </cc1:ValidatorCalloutExtender>
                            <cc1:ValidatorCalloutExtender ID="cmpexSEDates" runat="server" 
                                TargetControlID="cmpSEDates">
                            </cc1:ValidatorCalloutExtender>
                            <cc1:ValidatorCalloutExtender ID="reqexEndDate" runat="server" 
                                TargetControlID="reqEndDate">
                            </cc1:ValidatorCalloutExtender>
                        </td>
                        <td>
                            <asp:ImageButton ID="cmdReGenerate" runat="server" 
                                ImageUrl="buttons/refresh.gif" OnClick="cmdReGenerate_Click" 
                                ValidationGroup="daterange"></asp:ImageButton></td>
                    </tr>
                </table>
            </div>
            <div class="inputpanel">
                <asp:Label ID="lblStatusMessage" runat="server"></asp:Label>
            </div>
            <div class="inputpanel">
                <asp:Literal runat="server" ID="litForecastGrid"></asp:Literal>
                <div>
                    <table>
                        <tr>
                            <td class="labeltd">
                                Number of Payments</td>
                            <td class="inputtd">
                                <asp:Label ID="lblNoPayments" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="inputpanel">
        <asp:ImageButton runat="server" ID="cmdIFUpdate" 
            ImageUrl="./buttons/update.gif" ValidationGroup="forecastgen" />&nbsp;
        <asp:ImageButton runat="server" ID="cmdIFCancel" ImageUrl="./buttons/cancel.gif"
            CausesValidation="False" ValidationGroup="forecastgen" />
    </div>
    <div class="inputpanel">
        <igtbl:UltraWebGrid ID="dGrid" runat="server" Width="560px" Height="200px" Visible="False">
            <DisplayLayout RowHeightDefault="20px" Version="3.00" BorderCollapseDefault="Separate"
                EnableInternalRowsManagement="True" Name="dGrid">
                <AddNewBox>
                    <BoxStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </BoxStyle>
                </AddNewBox>
                <Pager>
                    <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </PagerStyle>
                </Pager>
                <HeaderStyleDefault BorderStyle="Solid" BackColor="LightGray">
                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                </HeaderStyleDefault>
                <FrameStyle Width="560px" BorderWidth="1px" Font-Size="8pt" Font-Names="Verdana"
                    BorderStyle="Solid" Height="200px">
                </FrameStyle>
                <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                </FooterStyleDefault>
                <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                </EditCellStyleDefault>
                <RowStyleDefault BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid">
                    <Padding Left="3px"></Padding>
                    <BorderDetails WidthLeft="0px" WidthTop="0px"></BorderDetails>
                </RowStyleDefault>
                <ActivationObject BorderColor="" BorderWidth="">
                </ActivationObject>
            </DisplayLayout>
            <Bands>
                <igtbl:UltraGridBand>
                    <Columns>
                        <igtbl:UltraGridColumn Key="Date">
                            <Header Caption="Forecast Date">
                            </Header>
                        </igtbl:UltraGridColumn>
                        <igtbl:UltraGridColumn Key="Amount">
                            <Header Caption="Forecast Amount">
                                <RowLayoutColumnInfo OriginX="1" />
                            </Header>
                            <Footer>
                                <RowLayoutColumnInfo OriginX="1" />
                            </Footer>
                        </igtbl:UltraGridColumn>
                        <igtbl:UltraGridColumn Key="Breakdown">
                            <Header Caption="Product Breakdown">
                                <RowLayoutColumnInfo OriginX="2" />
                            </Header>
                            <Footer>
                                <RowLayoutColumnInfo OriginX="2" />
                            </Footer>
                        </igtbl:UltraGridColumn>
                        <igtbl:UltraGridColumn Key="Details">
                            <Header Caption="Breakdown Detail">
                                <RowLayoutColumnInfo OriginX="3" />
                            </Header>
                            <Footer>
                                <RowLayoutColumnInfo OriginX="3" />
                            </Footer>
                        </igtbl:UltraGridColumn>
                        <igtbl:UltraGridColumn Key="PeriodEnd">
                            <Header Caption="Period End">
                                <RowLayoutColumnInfo OriginX="4" />
                            </Header>
                            <Footer>
                                <RowLayoutColumnInfo OriginX="4" />
                            </Footer>
                        </igtbl:UltraGridColumn>
                    </Columns>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
        </igtbl:UltraWebGrid>
    </div>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="contentmenu">
    <a href="./help_text/default_csh.htm#1098" target="_blank" class="submenuitem">Help</a>                
</asp:Content>

