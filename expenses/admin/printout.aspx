<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Page language="c#" Inherits="expenses.admin.printout" MasterPageFile="~/exptemplate.master" Codebehind="printout.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
	<div class="inputpanel">&nbsp;
		<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">By default when an employee prints their claim form, the Claim Name, their name and today's date will be printed at the top of the form.</asp:Label>
	</div>
    <div class="table-border">
	<div class="inputpanel">
		<igtbl:ultrawebgrid id="gridprintout" runat="server" meta:resourcekey="gridprintoutResource1">
			<DisplayLayout StationaryMargins="Header" AllowSortingDefault="OnClient" RowHeightDefault="20px"
				RowSizingDefault="Free" Version="2.00" SelectTypeRowDefault="Single" HeaderClickActionDefault="SortSingle"
				AllowColSizingDefault="Free" RowSelectorsDefault="No" Name="gridprintout" TableLayout="Fixed"
				CellClickActionDefault="Edit">
				<AddNewBox Hidden="False" Prompt="">
                    <BoxStyle BackColor="LightGray">
                    </BoxStyle>
				</AddNewBox>
				<Pager>
                    <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px" />
				</Pager>
				<HeaderStyleDefault Cursor="Hand" CssClass="th">
					<BorderDetails></BorderDetails>
				</HeaderStyleDefault>
				<FrameStyle Cursor="Default" Font-Names="Verdana" CssClass="datatbl"></FrameStyle>
				<FooterStyleDefault>
					<BorderDetails ></BorderDetails>
				</FooterStyleDefault>
				<ActivationObject></ActivationObject>
				<EditCellStyleDefault ></EditCellStyleDefault>
				<RowAlternateStyleDefault Wrap="True" CssClass="row2"></RowAlternateStyleDefault>
				<RowStyleDefault  Wrap="True" CssClass="row1">
					<Padding></Padding>
					<BorderDetails ></BorderDetails>
				</RowStyleDefault>
			</DisplayLayout>
			<Bands>
				<igtbl:UltraGridBand meta:resourcekey="UltraGridBandResource1">
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
			</Bands>
		</igtbl:ultrawebgrid>
	</div>
         </div>
	<div class="inputpanel"><asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="ImageButton2" runat="server" ImageUrl="../buttons/cancel_up.gif" OnClick="ImageButton2_Click" meta:resourcekey="ImageButton2Resource1"></asp:ImageButton></div>
       
    </asp:Content>

