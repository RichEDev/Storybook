<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="importdatawizard.aspx.cs" Inherits="Spend_Management.importdatawizard" %>
<%@ Import Namespace="SpendManagementLibrary" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

    <script type="text/javascript" language="javascript">
        function changeStep(index) {
        document.getElementById("requiredStep").value = index;
        theForm.submit();
        document.getElementById("requiredStep").value = '';
    }
</script>
<div class="inputpanel">
   <div id="timeline">
            <div id="timelineleft">
                <asp:Image runat="server" ImageUrl="~/shared/images/buttons/timeline_left.gif" /></div>
       <asp:Literal ID="littimeline" runat="server" meta:resourcekey="littimelineResource1"></asp:Literal>
            <div id="timelineright">
                <asp:Image runat="server" ImageUrl="~/shared/images/buttons/timeline_right.gif" /></div>
   </div> 
   
   </div>
<div class="inputpanel" id="importedData">
    <asp:Wizard ID="wizimport" runat="server" 
        CancelButtonImageUrl="~/shared/images/buttons/cancel_up.gif" CancelButtonType="Image" 
        DisplayCancelButton="True" 
        DisplaySideBar="False" 
        FinishCompleteButtonImageUrl="~/shared/images/buttons/pagebutton_finish.gif" 
        FinishCompleteButtonType="Image" 
        FinishPreviousButtonImageUrl="~/shared/images/buttons/pagebutton_previous.gif" 
        StartNextButtonImageUrl="~/shared/images/buttons/pagebutton_next.gif" 
        StartNextButtonType="Image" 
        StepNextButtonImageUrl="~/shared/images/buttons/pagebutton_next.gif" 
        StepNextButtonType="Image" 
        StepPreviousButtonImageUrl="~/shared/images/buttons/pagebutton_previous.gif" 
        StepPreviousButtonType="Image" FinishPreviousButtonType="Image" 
        ActiveStepIndex="3" OnNextButtonClick="wizimport_NextButtonClick" 
        OnActiveStepChanged="wizimport_ActiveStepChanged" 
            OnFinishButtonClick="wizimport_FinishButtonClick">
        <WizardSteps>
            <asp:WizardStep ID="WizardStep1" runat="server" Title="Step 1">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
                <table>
                        <tr>
                            <td class="labeltd">File Type:</td>
                            <td class="inputtd">
                        <asp:DropDownList ID="cmbfiletype" runat="server">
                            <asp:ListItem Value="1" Text="Excel Spreadsheet"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Flat File (txt/csv)"></asp:ListItem>
                            <asp:ListItem Value="3" Text="XML File"></asp:ListItem>
                            <asp:ListItem Value="4" Text="ESR Employee File"></asp:ListItem>
                        </asp:DropDownList></td></tr>

                    <tr><td>&nbsp;</td></tr>

                    <tr><td class="labeltd">File Name:</td>
                        <td class="inputtd importdatawizard-input">
                        <asp:FileUpload ID="txtfilename" Width="300px" runat="server"  CssClass="bordernone"/></td><td>
                            <asp:RequiredFieldValidator ID="reqfilename" runat="server" ErrorMessage="Please select the file you would like to import" Text="*" ControlToValidate="txtfilename"></asp:RequiredFieldValidator></td></tr>
                </table>
               
                <asp:Label ID="lblmsg" runat="server" Text="Label" ForeColor="Red" Visible="false"></asp:Label>
            </asp:WizardStep>
            <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2">
                <asp:MultiView ID="viewfiledetails" runat="server">
                    <asp:View ID="viewflat" runat="server">
                    <table>
                        
                        <%--<tr><td class="labeltd">Text qualifier:</td><td class="inputtd">
                            <asp:TextBox ID="txtqualifier" runat="server"></asp:TextBox></td></tr>--%>
                                <tr>
                                    <td class="labeltd">Header rows to skip:</td>
                                    <td class="inputtd">
                                        <asp:TextBox ID="txtheaderrowstoskip" runat="server"></asp:TextBox></td>
                                </tr>
                                
                                    <%--<tr><td class="labeltd">Row delimiter:</td><td class="inputtd">
                                        <asp:TextBox ID="txtrowdelimiter" runat="server"></asp:TextBox></td></tr>
                                        <tr><td class="labeltd">Column delimiter:</td><td class="inputtd">
                                        <asp:TextBox ID="txtcolumndelimiter" runat="server"></asp:TextBox></td></tr>--%>
                    </table>
                    </asp:View>
                    <asp:View ID="viewxml" runat="server">
                    </asp:View>
                    <asp:View ID="viewexcel" runat="server">
                        <table>
                                <tr>
                                    <td class="labeltd">First row has column names:</td>
                                    <td>
                                <asp:DropDownList ID="cmbfirstrowheader" runat="server" AutoPostBack="True" 
                                    OnSelectedIndexChanged="cmbfirstrowheader_SelectedIndexChanged">
                                    <asp:ListItem>Yes</asp:ListItem>
                                    <asp:ListItem>No</asp:ListItem>
                                </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="labeltd">Worksheet:</td>
                                    <td>
                                <asp:DropDownList ID="cmbworksheet" runat="server" AutoPostBack="True" 
                                    OnSelectedIndexChanged="cmbworksheet_SelectedIndexChanged">
                                </asp:DropDownList>
                                    </td>
                                </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
                <div class="inputpanel table-border ">
                    <div class="inputpaneltitle">File Sample</div>
                
                <igtbl:UltraWebGrid ID="gridsample" runat="server" SkinID="gridskin">
                    
<DisplayLayout Name="wizimportxgridsample">
<ActivationObject BorderColor="" BorderWidth=""></ActivationObject>
</DisplayLayout>
<Bands>
<igtbl:UltraGridBand>
<AddNewRow Visible="NotSet" View="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>
                    
                </igtbl:UltraWebGrid>
                </div>
            </asp:WizardStep>
            <asp:WizardStep runat="server">
            
                <asp:UpdatePanel ID="upnlmatching" runat="server">
                    <ContentTemplate>
                <table>
                                <tr>
                                    <td class="labeltd">Destination:</td>
                                    <td class="inputtd">
                        <asp:DropDownList ID="cmbdestination" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbdestination_SelectedIndexChanged">
                                        </asp:DropDownList></td>
                                </tr>
                </table>
                            <table>
                                <tr>
                                    <td>
                    <igtbl:UltraWebGrid ID="gridmatching" runat="server" SkinID="gridskin" OnInitializeLayout="gridmatching_InitializeLayout" OnInitializeRow="gridmatching_InitializeRow">
                       <DisplayLayout Pager-AllowPaging="false"></DisplayLayout>
                                        </igtbl:UltraWebGrid></td>
                                </tr>
                            </table>
                </ContentTemplate>
                </asp:UpdatePanel>
            </asp:WizardStep>
            <asp:WizardStep runat="server">
                <igtbl:UltraWebGrid ID="griddefaults" runat="server" SkinID="gridskin" 
                    OnInitializeLayout="griddefaults_InitializeLayout" Height="200px" 
                    Width="325px">


<DisplayLayout Name="wizimportxgriddefaults" AllowColSizingDefault="Free" 
                        AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" 
                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" 
                        BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti" 
                        RowHeightDefault="20px" RowSelectorsDefault="No" 
                        SelectTypeRowDefault="Extended" StationaryMargins="Header" 
                        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" 
                        ViewType="OutlookGroupBy">
    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" 
        BorderStyle="Solid" BorderWidth="1px" Height="200px" Width="325px">
    </FrameStyle>
    <Pager MinimumPagesForDisplay="2">
        <PagerStyle BackColor="#ccc" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
        </PagerStyle>
    </Pager>
    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
    </EditCellStyleDefault>
    <FooterStyleDefault BackColor="#19a2e6" BorderStyle="Solid" BorderWidth="1px">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </FooterStyleDefault>
    <HeaderStyleDefault BackColor="#19a2e6" BorderStyle="Solid" 
        HorizontalAlign="Left">
        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" 
            WidthTop="1px" />
    </HeaderStyleDefault>
    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" 
        BorderWidth="1px">
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
<ActivationObject BorderColor="" BorderWidth=""></ActivationObject>
    <FilterOptionsDefault>
        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" 
            BorderWidth="1px" CustomRules="overflow:auto;" Font-Size="12px" Height="300px" 
            Width="200px">
            <Padding Left="2px" />
        </FilterDropDownStyle>
        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
        </FilterHighlightRowStyle>
        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" 
            BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" 
           Font-Size="12px">
            <Padding Left="2px" />
        </FilterOperandDropDownStyle>
    </FilterOptionsDefault>
</DisplayLayout>
<Bands>
<igtbl:UltraGridBand>
<AddNewRow Visible="NotSet" View="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>


                </igtbl:UltraWebGrid>
            </asp:WizardStep>
            <asp:WizardStep runat="server">
                <asp:UpdatePanel ID="upnlimport" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="tmrimports" EventName="Tick" />
                    </Triggers>
                    <ContentTemplate>
                            <div id="statusdiv" style="width: 400px; height: 400px; border: 1px black solid; overflow: auto">
                        <asp:Literal ID="litimportstatus" runat="server"></asp:Literal>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Timer ID="tmrimports" runat="server" Interval="1000" OnTick="tmrimports_Tick">
                </asp:Timer>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
    </div>
    
</asp:Content>

<asp:Content ID="scriptsContent" runat="server" ContentPlaceHolderID="scripts">
         <script type="text/javascript" src="<%=GlobalVariables.StaticContentLibrary%>/js/expense/jquery.smoothscroll.js"></script> 
     <script type="text/javascript">
         //Bootstrap td & th style override for buttons
         $(document).ready(function () {
             $('#<%=wizimport.ClientID%> table').find('[id$="ImageButton"]').css('margin-right', '15px');
             $('#<%=wizimport.ClientID%> table').find('[id$="ImageButton"]').css('margin-top', '10px');
             $('#<%=wizimport.ClientID%> table').find('[id$="CancelImageButton"]').css('margin-right', '0');

             //Add the custom scroll to the wizard when required only!
             $('#importedData').css({ overflow: "auto", display: "table" });
             var h1 = $(this).outerWidth();
             $('#importedData').css({ overflow: "hidden", display: "block" });
             var h2 = $(this).outerWidth();
             if (h2 >= h1) {
             } else {
                 $('#importedData').css('overflow', 'auto');
                 $('#importedData').css('margin-bottom', '32px');
                 $('#importedData').customScroll({ cursorcolor: "#19a2e6", autohidemode: false, enablemousewheel: false });
                 var node = $('#<%=wizimport.ClientID%> table').find('[id$="StepPreviousImageButton"]');
                 node.closest('table').css('position', 'absolute');
                 node.closest('table').css('left', '81%');
                 node.closest('table').css('margin-top', '7px');
             }

             //Scrollbar bug fix
             $(window).resize(function () {
                 $('#importedData').getNiceScroll().resize();
                 setTimeout(function () {
                     $('#importedData').getNiceScroll().show();
                 }, 200);
             });

             $("#menu-toggle").click(function () {
                 $('#importedData').getNiceScroll().hide();
                 setTimeout(function () {
                     $('#importedData').getNiceScroll().resize();
                     $('#importedData').getNiceScroll().show();
                 }, 500);

             });

             (function ($) {
                 $.fn.hasScrollBar = function () {
                     return this.get(0).scrollWidth > this.innerWidth();
                     
                 }
             })(jQuery);
             var src = $('#activeStepCheck').attr('src').split('/');
             var file = src[src.length - 1];
             if (file == "timeline_event2_sel.gif") {
                 if (!$('#importedData').hasScrollBar() && (($('#importedData').innerWidth() - $('#importedData > table').innerWidth()) < 150)) {
                     $('body').css('display', 'inline-block');
                 }
             }


         });
     </script>
</asp:Content>
