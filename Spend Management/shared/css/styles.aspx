<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="styles.aspx.cs" Inherits="Spend_Management.shared.css.styles" EnableTheming="false" Theme="" MaintainScrollPositionOnPostback="false" StylesheetTheme="" %>

<%@ OutputCache Location="None" VaryByParam="None" %>


/* Modal colours */
<asp:literal id="litErrorModalColours" runat="server" />


/* tooltips colours */
.tooltipcontainer {
	color: #ffffff;
}

.invalid
{
    color: red;
}

<asp:literal id="litToolTipColours" runat="server" />


<asp:literal id="litLogonPanelHeights" runat="server" />

#logoninnercontainer
{
	background-image: url('<% = ImageRootReference %>/images/backgrounds/gradients/logonInnerContainer.png');
}

#breadcrumbbar {
	background-image: url('<% = ImageRootReference %>/images/backgrounds/gradients/breadcrumbbar.png');
}

#pagetitlebar {
	background-image: url('<% = ImageRootReference %>/images/backgrounds/gradients/pagetitlebar.png');
}

#logonpageimage 
{
    <asp:literal id="litLogonPageImage" runat="server" />
}

<%--#logonpage #breadcrumbbar 
{
	background-color: <% = Colours. %>;
}--%>



/* HTML CSS Buttons */
.buttonContainer
{
	background-image: url('<% = ImageRootReference %>/icons/16/new-icons/button.gif');
}

.buttonContainer .buttonInner
{
	background-image: url('<% = ImageRootReference %>/icons/16/new-icons/button.gif');
	color: #ffffff;
}

/* smaller version of the CSS button for "filter", etc. */
span.smallbuttonContainer 
{
	background-image: url('<% = ImageRootReference %>/images/backgrounds/buttons/button22.png');
}

span.smallbuttonContainer .smallbuttonInner
{
	background-image: url('<% = ImageRootReference %>/images/backgrounds/buttons/button22.png');
	color: #ffffff;
}
/* HTML CSS Buttons end */


/* SM Field */
/* START */

.sm_panel .sectiontitle 
{
	color: <% = (Colours == null) ? DefaultColours.greenLightSectionTextColour : Colours.greenLightSectionTextColour %>;
	border-bottom-color: <% = (Colours == null) ? DefaultColours.greenLightSectionUnderlineColour : Colours.greenLightSectionUnderlineColour %>;
    border-top-color: <% = (Colours == null) ? DefaultColours.greenLightSectionBackgroundColour : Colours.greenLightSectionBackgroundColour %>;
    background-color: <% = (Colours == null) ? DefaultColours.greenLightSectionBackgroundColour : Colours.greenLightSectionBackgroundColour %>;
}

.sm_panel label 
{
	color: <% = (Colours == null) ? DefaultColours.greenLightFieldColour : Colours.greenLightFieldColour %>;
}

.sm_panel .twocolumn 
{
    border-bottom-color: #D8D8D8;
}

.sm_panel .onecolumnpanel
{
    color: <% = (Colours == null) ? DefaultColours.greenLightFieldColour : Colours.greenLightFieldColour %>;
	border-color: #C0C0C0;
}

.sm_panel .onecolumn, .sm_panel .onecolumnlarge, .sm_panel .onecolumnsmall
{
    border-bottom-color: #D8D8D8;
}

.sm_panel .onecolumnlarge .rtePanel, .sm_panel .onecolumnlarge .rtePanelReadOnly
{
    border-color: #C0C0C0;
}

.sm_panel .onecolumnlarge .rtePanelReadOnly
{
    border-color: #aaa;
}
  
 /* EVERYTHING BELOW IS FOR THE FORM DESIGNER - TO BE UPDATED */

.sm_button
{
	border-color: #35649d;
	background-color: #b6677b;
}

.sm_button span
{
	color: #ffffff;
}

.sm_title
{
    background-image: url('/shared/images/inputpanelbg.jpg');
    background-repeat: repeat-x;
    border-color: #013473;
    background-color: <% = (Colours == null) ? DefaultColours.pageOptionsBGColour : Colours.pageOptionsBGColour %>;
    color: #ffffff;
}

.sm_comment
{
	border-color: <% = (Colours == null) ? DefaultColours.pageOptionsTxtColour : Colours.pageOptionsTxtColour %>;
}

.sm_controls
{
	border-color: Gray;
	background-color: #ebebfa;
}

.sm_field_comment
{    
	border-color: <% = (Colours == null) ? DefaultColours.pageOptionsTxtColour : Colours.pageOptionsTxtColour %>;
}

.sm_label
{
    background-image: url('/shared/images/inputtd_bg.jpg');
    background-repeat: repeat-y;
    background-color: #506DAB;
    border-color: #496C9B;
}

/* END */
/* SM Field */


/* Custom Entity Administration */
/* START */

.infoHelpArea
{
    font-family: Calibri;
    font-size: 14px;    
	background-color: White;	
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;     
}

#loadingArea 
{
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
    background-color: <% = (Colours == null) ? DefaultColours.rowBGColour : Colours.rowBGColour %>;
    color: <% = (Colours == null) ? DefaultColours.rowTxtColour : Colours.rowTxtColour %>;
}

/* Form Designer */
/* START */

#dialog.availablefieldhover
{
    background: transparent url('<% = ImageRootReference %>/images/backgrounds/gradients/availableFieldsDropHover.png') repeat scroll 0 0;   
}

#availableFields span
{   
    border-bottom-color: #cccccc;
}

#tabBar .drophover .sm_tabheader_middle
{      
    background: url(<% = ImageRootReference %>/images/backgrounds/tabs/tab_droppable_middle_img.png) repeat-x;
}

#tabBar .drophover .sm_tabheader_middle.selected_tab
{      
    background: url(/shared/images/backgrounds/tabs/tab_selected_middle_img.png) repeat-x;   
}

#formTabs .field_options_image_hover, #popupFormOptions .field_options_image_hover
{
    background-color: #E0E0E0;    
}

#formDesignContents .placeholder-highlight 
{
    background-color: White;    
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
}

#formTabs .placeholder-highlight-fields
{
    background-color: White;
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
}

#tabBar .selected_tab
{
     font-weight: bold;
}

#formTabs .sm_field_spacer, #dragFieldHolder .sm_field_spacer
{
	border-color: #E0E0E0;
}

#spacerArea .sm_field_spacer
{        
    border-color: #E0E0E0;
}

#formTabs .sm_form_field_options, #popupFormOptions.sm_form_field_options
{
	background-color: White;	
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;   
}

#formTabs .sectiontitle, .sectionsortables,
.sm_field, .sm_field_wide, .sm_field_tall, .sm_field_taller, .sm_field_comment, .sm_field_grid, .sm_field_spacer,
.sm_field label, .sm_field_wide label, .sm_field_tall label, .sm_field_taller label, .sm_field_comment label, .sm_field_grid label, .sm_field_spacer label,
.sm_field span, .sm_field_wide span, .sm_field_tall span, .sm_field_taller span, .sm_field_comment span, .sm_field_grid span, .sm_field_spacer span, .sm_field_lookup
{
    cursor: url(<% = ImageRootReference %>/cursors/openhand.cur), default !important;
}

.closedhand 
{
    cursor: url(<% = ImageRootReference %>/cursors/closedhand.cur), default !important;
}

.ui-dialog 
{ 
    position: absolute; 
}

/* END */
/* Form Designer */

/* Views */
/* START */

#tcPopupListItemContainer
{
    background-color: White;    
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
}

#tcPopupListItemContainer .popupListItems
{
    border-bottom-color: Gray;
}

#divFilterList .multiselect-add { background-image: url(<% = ImageRootReference %>/icons/16/new-icons/greenplus.png) !important; }
#divFilterList .multiselect-delete { background-image: url(/shared/images/icons/delete2.png) !important; }

#vakata-dragged.jstree-default 
{     
    background-color: white;    
    border-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>; 
}

#selectedIconSpan
{
    border-color: #999999;
    background-color: white;
}

#selectedIconInfo
{
    color: <% = (Colours == null) ? DefaultColours.sectionHeadingTxtColour : Colours.sectionHeadingTxtColour %>;    
}

#viewCustomIconContainer
{        
    border-color: #999999;      
}

#iconResultsLeft, #iconResultsRight
{     
    color: #999999;
}

#iconResultsLeft.active, #iconResultsRight.active
{
    color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
}

#iconResultsHeader
{
    background-color: <% = (Colours == null) ? DefaultColours.sectionHeadingUnderlineColour : Colours.sectionHeadingUnderlineColour %>;
    border-color: #999999;
}

#iconSearchBox .searchBox
{
    border-color: #999999;
    color: #222222;
}

#iconSearchBox .searchButton
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_454545_256x240.png);
}

/* END */
/* Views */


/* END */
/* Custom Entity Administration */

/* Receipt Processing */
/* START */

.receiptProcessBox
{
    background-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
    color: <% = (Colours == null) ? DefaultColours.headerBreadcrumbTxtColour : Colours.headerBreadcrumbTxtColour %>;
    -webkit-box-shadow: 6px 6px 10px #C0C0C0;
    box-shadow: 6px 6px 10px #C0C0C0;
}

#envelopeNumberBox .receiptProcessInput.invalidEntry
{
    color: #E80000;    
    border-color: #E80000;
}

#envelopeNumberBox .receiptProcessInput.addRow
{    
    color: gray;    
}

#referenceNumber
{
    background-color: #FFF;
    color: #000;
}

/* END */
/* Receipt Processing */

/* Software Europe Extensions to jQuery CSS */
/* START */

.ui-widget-header .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_222222_256x240.png); 
}

.ui-widget-overlay 
{ 
    background: #aaaaaa url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-bg_flat_0_aaaaaa_40x100.png) 50% 50% repeat-x !important; 
}

.ui-widget-shadow 
{ 
    background: #aaaaaa url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-bg_flat_0_aaaaaa_40x100.png) 50% 50% repeat-x !important;
}

.ui-state-default, .ui-widget-content .ui-state-default, .ui-widget-header .ui-state-default 
{ 
    background: #e6e6e6 url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-bg_glass_75_e6e6e6_1x400.png) 50% 50% repeat-x !important; 
}

.ui-state-hover, .ui-widget-content .ui-state-hover, .ui-widget-header .ui-state-hover, .ui-state-focus, .ui-widget-content .ui-state-focus, .ui-widget-header .ui-state-focus 
{ 
    background: #dadada url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-bg_glass_75_dadada_1x400.png) 50% 50% repeat-x !important; 
}

.ui-state-hover .ui-icon, .ui-state-focus .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_454545_256x240.png) !important; 
}

.ui-state-active .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_454545_256x240.png !important) ; 
}

.ui-state-highlight, .ui-widget-content .ui-state-highlight, .ui-widget-header .ui-state-highlight  
{
    background: #fbf9ee url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-bg_glass_55_fbf9ee_1x400.png) 50% 50% repeat-x !important; 
}

.ui-state-error, .ui-widget-content .ui-state-error, .ui-widget-header .ui-state-error 
{
    background: #fef1ec url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-bg_glass_95_fef1ec_1x400.png) 50% 50% repeat-x !important;
}

.ui-icon 
{ 
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_222222_256x240.png); 
}

.ui-widget-content .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_222222_256x240.png); 
}

.ui-widget-header .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_ffffff_256x240.png); 
}

.ui-state-default .ui-icon 
{ 
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_888888_256x240.png);
}

.ui-state-hover .ui-icon, .ui-state-focus .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_454545_256x240.png);
}

.ui-state-active .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_454545_256x240.png);
}

.ui-state-highlight .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_2e83ff_256x240.png);
}

.ui-state-error .ui-icon, .ui-state-error-text .ui-icon 
{
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_cd0a0a_256x240.png);
}

.ui-icon-light 
{ 
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_ffffff_256x240.png) !important; 
}

.ui-icon-dark 
{ 
    background-image: url(<% = ImageRootReference %>/js/jquery/themes/base/images/ui-icons_ffffff_256x240.png) !important; 
}

.ui-state-default.ui-corner-top.ui-tabs-active.ui-state-active 
{ 
    background: white !important; 
}

/* END */
/* Software Europe Extensions to jQuery CSS */

.standardHeaderColours {
    background-color: <% = (Colours == null) ? DefaultColours.defaultHeaderBGColour : Colours.defaultHeaderBGColour %>;
    color: <% = (Colours == null) ? DefaultColours.defaultHeaderBreadcrumbTxtColour : Colours.defaultHeaderBreadcrumbTxtColour %>;
}


.sm_availablefield_linkedfield
{
    color: #666666;
}


/* Expense Item Receipts START */

#attachedReceipts ul li a.deleteReceipt 
{
    background: transparent url(<% = ImageRootReference %>/icons/16/plain/delete.png) no-repeat;
}

#cboxTitle a.title 
{
    color: #ccc;
}

.noReceiptImage 
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_unknown.png) no-repeat;
}

.noReceiptImage.pdf 
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_pdf.png) no-repeat;
}

.noReceiptImage.doc, .noReceiptImage.docx, .noReceiptImage.rtf 
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_doc.png) no-repeat;
}

.noReceiptImage.xls, .noReceiptImage.xlsx 
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_xls.png) no-repeat;
}

.noReceiptImage.ppt, .noReceiptImage.pptx 
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_ppt.png) no-repeat;
}

.noReceiptImage.txt
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_txt.png) no-repeat;
}

.noReceiptImage.zip
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_zip.png) no-repeat;
}

.noReceiptImage.tif, .noReceiptImage.tiff
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_tiff.png) no-repeat;
}

.noReceiptImage.ppt
{
    background: transparent url(<% = ImageRootReference %>/icons/custom/128/file_ppt.png) no-repeat;
}


/* Expense Item Receipts END */



/* System health page */
/* START */

#healthTitle
{
    background-color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;
    color: <% = (Colours == null) ? DefaultColours.headerBreadcrumbTxtColour : Colours.headerBreadcrumbTxtColour %>;
}

#healthPage .informationBlock
{    
    color: <% = (Colours == null) ? DefaultColours.headerBGColour : Colours.headerBGColour %>;    
}

#healthPage .informationBlock .infonode-highlightproperty
{
    color: <% = (Colours == null) ? DefaultColours.sectionHeadingUnderlineColour : Colours.sectionHeadingUnderlineColour %>;   
}

/* Ajax Uploader */


    .ajax__fileupload_selectFileButton 
    {
        background-color: #B6677B !important;
        color: white !important;
        cursor: pointer;
        display: block !important;
        font-size: 13px !important;
        height: 24px !important;
        line-height: 24px !important;
        margin-right: 4px !important;
        text-align: center !important;
        width: 80px !important;
    }

        .ajax__fileupload_uploadbutton  
   {
        background-color: #B6677B !important;
        color: white !important;
        cursor: pointer;
        display: block !important;
        font-size: 13px !important;
        height: 24px !important;
        line-height: 24px !important;
        margin-right: 4px !important;
        text-align: center !important;
        width: 80px !important;
    }

        .ajax_fileupload_cancelbutton 
    {
        background-color: #B6677B !important;
        color: white !important;
        cursor: pointer;
        display: block !important;
        font-size: 13px !important;
        height: 24px !important;
        line-height: 24px !important;
        margin-right: 4px !important;
        text-align: center !important;
        width: 80px !important;
    }

       .ajax__fileupload_fileItemInfo .removeButton 
    {
        background-color: #B6677B !important;
        color: white !important;
        cursor: pointer;
        display: block !important;
        font-size: 13px !important;
        height: 24px !important;
        line-height: 24px !important;
        margin-right: 4px !important;
        text-align: center !important;
        width: 80px !important;
    }

         .asyncFileUpload input
     {
         width:100%!important;
     }

/* System health page */
/* END */

#odometerReadings th 
{ 
    color: <% = (Colours == null) ? DefaultColours.greenLightFieldColour : Colours.greenLightFieldColour %>; 
}

#odometerReadings td 
{ 
     color: <% = (Colours == null) ? DefaultColours.greenLightFieldColour : Colours.greenLightFieldColour %>;
}

.odometerReadingsBusinessMileage 
{
    color: <% = (Colours == null) ? DefaultColours.greenLightFieldColour : Colours.greenLightFieldColour %>;
}

#flags div.justification {
	border-left-color: <% = (Colours == null) ? DefaultColours.sectionHeadingUnderlineColour : Colours.sectionHeadingUnderlineColour %>;
}
.errorRow{
    background-color: #B6677B !important;
    color: white !important;
    padding: 0px 5px 0px 5px;
}