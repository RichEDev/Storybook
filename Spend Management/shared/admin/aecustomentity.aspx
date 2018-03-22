<%@ Page Language="C#" MasterPageFile="~/masters/smPagedForm.master" AutoEventWireup="true" CodeBehind="aecustomentity.aspx.cs" Inherits="Spend_Management.aecustomentity" %>

<%@ Register TagPrefix="HTMLEditor" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>
<%@ Register Src="~/shared/usercontrols/Selectinator.ascx" TagName="selectinator" TagPrefix="controls" %>
<%@ Register TagPrefix="ajaxcontrol" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=4.1.7.123, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>

<asp:Content ID="Content5" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
        .modalpopup{
            height: auto;
            overflow-y: auto;
        }


        #ctl00_contentmain_tabConViews_tabViewGeneralDetails_treecustommenu table{
            width: 100% !important;
            
        }

        #ctl00_contentmain_tabConViews_tabViewGeneralDetails_treecustommenu table td {
            float :left;
        }

        #ctl00_contentmain_tabConViews_body{
            overflow-y:scroll;
        }

        #ctl00_contentmain_tabConViews_tabViewGeneralDetails_treecustommenu table{
            margin-bottom:0;
        }

        #ctl00_contentmain_tabConViews_tabViewGeneralDetails_treecustommenu table tbody tr{
            height:24px;
        }

        .ctl00_contentmain_tabConViews_tabViewGeneralDetails_treecustommenu_2{
            margin-top:5px;
            margin-left:5px;
        }

        #ctl00_contentmain_pnlntoOnerelationship{
            top:71px!important;
        }

        .ellide span {
            width: 170px;
        }
    </style>
    </asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');$g(txtentitynameid).focus();" id="lnkGeneral" class="selectedPage">GreenLight
        Details</a> <a href="javascript:changePage('Attributes');" id="lnkAttributes">Attributes</a>
    <a href="javascript:changePage('Forms');" id="lnkForms">Forms</a> <a href="javascript:changePage('Views');"
        id="lnkViews">Views</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentoptions" runat="server">
    
    <div id="pgOptAttributes" style="display: none;">
        <a href="javascript:NewAttribute();" class="submenuitem">
            New Attribute</a> <a href="javascript:SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Add();"
                class="submenuitem">New n:1 Relationship</a> <a href="javascript:SEL.CustomEntityAdministration.Attributes.Relationship.OneToMany.Add();"
                    class="submenuitem">New 1:n Relationship</a> <!--
                        
                        Summary attribute link hidden until the functionality is fixed

                        <a href="javascript:SEL.CustomEntityAdministration.Attributes.Summary.Add();"
                        class="submenuitem">New Summary Attribute</a>
                        -->
    </div>
    <div id="pgOptForms" style="display: none;">
        <a href="javascript:SEL.CustomEntityAdministration.Forms.NewForm();"
            class="submenuitem">New Form</a>
    </div>
    <div id="pgOptViews" style="display: none;">
        <a href="javascript:SEL.CustomEntityAdministration.Views.Add();" class="submenuitem">New View</a>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="contentmain" runat="server">
    
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcCustomEntities.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/jquery-selui-dialog.js" />
            <asp:ScriptReference Path="~/shared/javaScript/customEntityAdmin.js?v=2017081202" />
            <asp:ScriptReference Path="/shared/javaScript/minify/CustomMenuStructure.js"/>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.customEntityAdministration.js?v=2017081202" />
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.selectinator.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript" language="javascript">
        var modattributeid = '<%=modattribute.ClientID %>';
        var modformid = '<%=modform.ClientID %>';
        var modviewid = '<%=modview.ClientID %>';
        var modlistitemid = '<%=modlistitem.ClientID %>';
        var modsectionid = '<%=modsection.ClientID %>';
        var lstitemsid = '<%=lstitems.ClientID %>';
        var txtlistitemid = '<%=txtlistitem.ClientID %>';
        var chkarchiveitemid = '<%=chkArchiveListItem.ClientID %>';
        var chkDisplayInMobile = '<% = chkDisplayInMobile.ClientID %>';
        var txtentitynameid = '<%=txtentityname.ClientID %>';
        var txtpluralnameid = '<%=txtpluralname.ClientID %>';
        var txtdescriptionid = '<%=txtdescription.ClientID %>';
        var txtattributenameid = '<% =txtattributename.ClientID %>';
        var txtattributedescriptionid = '<% =txtattributedescription.ClientID %>';
        var txtattributetooltipid = '<% =txtattributetooltip.ClientID %>';
        var chkattributebuiltinid = '<%= chkAttributeBuiltIn.ClientID %>';
        var chkattributemandatoryid = '<% =chkattributemandatory.ClientID %>';
        var cmbattributetypeid = '<% =cmbattributetype.ClientID %>';
        var txtmaxlengthid = '<%=txtmaxlength.ClientID %>';
        var cmbtextformatid = '<%=cmbtextformat.ClientID %>';
        var cmbdefaultvalueid = '<%=cmbdefaultvalue.ClientID %>';
        var cmbdateformatid = '<%=cmbdateformat.ClientID %>';
        var txtprecisionid = '<%=txtprecision.ClientID %>';
        var txtmaxlengthlargeid = '<%=txtmaxlengthlarge.ClientID %>';
        var chkRemoveFont = '<%=chkStripFont.ClientID %>';
        var cmbtextformatlargeid = '<%=cmbtextformatlarge.ClientID %>';
        var cmbattributeworkflowid = '<%=cmbattributeworkflow.ClientID %>';
        var txtformnameid = '<%=txtformname.ClientID %>';
        var txtformdescriptionid = '<%=txtformdescription.ClientID %>';
        var pnlAttributeID = '<%=pnlattributegrid.ClientID %>';
        var pnlFormID = '<%=pnlformgrid.ClientID %>';
        var chkenableattachmentsid = '<%=chkenableattachments.ClientID %>';
        var chkEnableAudiencesID = '<% = chkEnableAudiences.ClientID %>';
        var ddlAudienceBehaviourID = '<% = ddlAudienceBehaviour.ClientID%>';
        var cmpAudienceID = '<% = cmpAudiences.ClientID%>';
        var chkallowdocmergeid = '<%=chkallowdocmerge.ClientID %>';
        var lblDefaultCurrencyID = '<%=lblDefaultCurrency.ClientID %>';
        var chkEnableCurrenciesID = '<% = chkEnableCurrencies.ClientID %>';
        var ddlDefaultCurrencyID = '<% = ddlDefaultCurrency.ClientID %>';
        var chkEnablePopupWindowID = '<% = chkEnablePopupWindow.ClientID %>';
        var ddlPopupWindowViewID = '<% = ddlPopupWindowView.ClientID %>';
        var cmbcontactformatid = '<% = cmbContactFormat.ClientID %>';
        var reqContactFormat = '<%= reqContactFormat.ClientID %>';
        var chkEnableLockingID = '<%=chkenablelocking.ClientID%>';
        var attributeid = 0;
        var viewid = 0;
        var formid = 0;
        var formTabs;
        var tabConFormsID = '<%=tabConForms.ClientID %>';
        var currentTab;
        var treeviewid;
        var txtformsavebuttontextid = '<%=txtsavebuttontext.ClientID %>';
        var txtformsaveandduplicatebuttontextid = '<%=txtsaveandduplicatebuttontext.ClientID %>';
        var txtformsaveandstaybuttontextid = '<%=txtsaveandstaybuttontext.ClientID %>';
        var txtformsaveandnewbuttontextid = '<%=txtsaveandnewbuttontext.ClientID %>';
        var txtformcancelbuttontextid = '<%=txtcancelbuttontext.ClientID %>';
        var chkshowsubmenuid = '<%=chkshowsubmenu.ClientID %>';
        var chkHideTorch = '<%=chkHideTorch.ClientID %>';
        var chkHideAttachments = '<%=chkHideAttachments.ClientID %>';
        var chkHideAudiences = '<%=chkHideAudiences.ClientID %>';
        var chkFormBuiltIn = '<%=chkFormBuiltIn.ClientID %>';
        var chkshowbreadcrumbsid = '<%=chkshowbreadcrumbs.ClientID %>';
        var modcopyformid = '<%=modcopyform.ClientID %>';
        var txtcopyformnameid = '<%=txtcopyformname.ClientID %>';
        var pnlactualformID = '<%=pnlform.ClientID %>';
        var pnlcopyformID = '<%=pnlcopyform.ClientID %>';
        var reqNewFormNameID = '<%=reqNewFormName.ClientID %>';
        var modtabid = '<%=modtab.ClientID %>';
        var modfieldid = '<%=modfield.ClientID %>';
        var txtsectionid = '<%=txtsectionheader.ClientID %>';
        var txtfieldlabelID = '<%=txtfieldlabel.ClientID %>';
        var txttabid = '<%=txttabheader.ClientID %>';
        var txtAdviceText = '<%=txtAdviceText.ClientID %>';
        var txtSectionHeaderID = '<%=txtsectionheader.ClientID %>';
        var txtTabHeaderID = '<%=txttabheader.ClientID %>';
        var popupFormOptionsID = '<%=popupFormOptions.ClientID %>';
        var popupFieldOptionsID = '<%=popupFieldOptions.ClientID %>';
        var auditidentifierid = '<%=chkAuditIdentifier.ClientID %>';
        var isuniqueid = '<%=chkUnique.ClientID %>';
        var encrypt = '<%=this.chkEncrypt.ClientID%>';
        var hdnAuditAttributeID = '<%=hdnAuditAttributeID.ClientID %>';
        var hdnAuditAttributeDisplayNameID = '<%=hdnAuditAttributeDisplayName.ClientID %>';
        var reqPrecision = '<%=reqPrecision.ClientID %>';
        var cmpPrecision1 = '<%=cmpPrecision1.ClientID %>';
        var cmpPrecision2 = '<%=cmpPrecision2.ClientID %>';
        var listbox_tovalidate = '<%= lstitems.ClientID %>';
        var custAttListItem = '<%=custAttListItem.ClientID %>';
        var tabFormDescID = '<%=tabFormDes.ClientID %>';
        var cmbDisplayWidthID = '<%=cmbDisplayWidth.ClientID %>';
        var selectedViewID;
        var selectedFormID;
        var reqAttributeType = '<%=reqAttributeType.ClientID %>';
        var reqTextFormat = '<%=reqTextFormat.ClientID %>';
        var reqDateFormat = '<%=reqDateFormat.ClientID %>';
        var reqDefaultYesNo = '<%=reqDefaultYesNo.ClientID %>';
        var reqDisplayWidth = '<%=reqDisplayWidth.ClientID %>';
        var reqLargeTextFormat = '<%=reqLargeTextFormat.ClientID %>';
        var cvDefaultCurrencyID = '<%=cvDefaultCurrency.ClientID %>';
        var reqDisplayName = '<%=reqdisplayname.ClientID %>';
        var cmpSingleLineMaxLength = '<%=cmpSingleLineMaxLength.ClientID %>';
        var reqFieldItemList = '<%= reqFieldItemList.ClientID %>';
        var reqComment = '<%= reqComment.ClientID %>';
        var lblsavebuttontextid = '<%= lblsavebuttontext.ClientID %>';
        var lblsaveandduplicatebuttontextid = '<%= lblsaveandduplicatebuttontext.ClientID %>';
        var lblsaveandstaybuttontextid = '<%= lblsaveandstaybuttontext.ClientID %>';
        var lblsaveandnewbuttontextid = '<%= lblsaveandnewbuttontext.ClientID %>';
        var lblcancelbuttontextid = '<%= lblcancelbuttontext.ClientID %>';
        var cvTextSaveID = '<%= cvTextSave.ClientID %>';
        var cvTextSaveAndDuplicateID = '<%= cvTextsaveandduplicate.ClientID %>';
        var cvTextSaveAndStayID = '<%= cvTextSaveAndStay.ClientID %>';
        var cvTextSaveAndNewID = '<%= cvTextSaveAndNew.ClientID %>';
        var cvTextCancelID = '<%= cvTextCancel.ClientID %>';
        var pnladdattributeid = '<%= pnladdattribute.ClientID %>';
        var pnllistitemid = '<%= pnllistitem.ClientID %>';
        var pnlviewid = '<%= pnlview.ClientID %>';
        var pnltabid = '<%= pnltab.ClientID %>';
        var pnlsectionid = '<%= pnlsection.ClientID %>';
        var pnlfieldid = '<%= pnlfield.ClientID %>';

        var btnSaveOnetonRelationshipid = '<%=btnSaveOnetonrelationship.ClientID %>'
        var btnCancelOnetonRelationshipid = '<%=btnCancelOnetonrelationship.ClientID %>'

        var btnSaventoOneRelationshipid = '<%= btnSaventoOnerelationship.ClientID %>';
        var btnCancentoOnelRelationshipid = '<%= btnCancelntoOnerelationship.ClientID %>';

        var reqTabHeaderID = '<%= reqTabHeader.ClientID %>';
        var reqSectionHeaderID = '<%= reqSectionHeader.ClientID %>';
        var btnSaveSectionid = '<%= btnSaveSection.ClientID %>';
        SEL.CustomEntityAdministration.DomIDs.Base.FormSelectionAttribute = '<%= ddlFormSelectionAttribute.ClientID %>';
        
        SEL.CustomEntityAdministration.DomIDs.Forms.RichTextEditor = '<%=txtHTMLEditor.ClientID %>';
        SEL.CustomEntityAdministration.DomIDs.Forms.EditorExtender = '<%=EditorExtender1.ClientID %>';
        (function (r) {
            r.Modal.Control = '<%=modntoOnerelationship.ClientID %>';
            r.Modal.Panel = '<%=pnlntoOnerelationship.ClientID %>';
            r.Modal.TabContainer = '<%=tabConRelFields.ClientID %>';

            r.Modal.General.Tab = '<%=tabRelDefinition.ClientID %>';
            r.Modal.General.RelationshipName = '<%=txtntoOnerelationshipname.ClientID %>';
            r.Modal.General.RelationshipNameReqValidator = '<%=reqntoOnerelationshipname.ClientID %>';
            r.Modal.General.Description = '<%=txtntoOnerelationshipdescription.ClientID %>';
            r.Modal.General.Mandatory = '<%=chkntoOnerelationshipmandatory.ClientID %>';
            r.Modal.General.MandatoryLabel = '<%=lblntoOnerelationshipmandatory.ClientID %>';
            r.Modal.General.RelationshipEntity = '<%=cmbntoOnerelationshipentity.ClientID %>';
            r.Modal.General.RelationshipEntityReqValidator = '<%=cmpntoOnerelationshipentity.ClientID %>';
            r.Modal.General.Tooltip = '<%=txtntoOnerelationshiptooltip.ClientID %>';
            r.Modal.General.BuiltIn = '<%=chkNToOneRelationshipBuiltIn.ClientID%>';
            //            r.Modal.General.RelationshipView = '< % = cmbntoOnerelationshipview.ClientID %>';
            //            r.Modal.General.RelationshipViewReqValidator = '< % = cmpntoOnerelationshipview.ClientID %>';

            r.Modal.Fields.Tab = '<%=tabRelFields.ClientID %>';
            r.Modal.Fields.DisplayField = '<%=cmbmtodisplayfield.ClientID %>';
            r.Modal.Fields.DisplayFieldReqValidator = '<%=cmpmtodisplayfield.ClientID %>';
            r.Modal.Fields.MatchFields = '<%=cmbmtomatchfields.ClientID %>';
            r.Modal.Fields.MatchFieldsReqValidator = '<%=custmtomatchfields.ClientID %>';
            r.Modal.Fields.MaxRows = '<%=txtmtomaxrows.ClientID %>';
            r.Modal.Fields.MaxRowsCmpValidator = '<%=cmpmtomaxrows.ClientID %>';

            r.Modal.LookupDisplayFields.Tab = '<%=tabLDF.ClientID %>';
            r.Modal.LookupDisplayFields.Tree = '<%=tcLDF.TreeClientID %>';
            r.Modal.LookupDisplayFields.Drop = '<%=tcLDF.TreeDropClientID %>';

            r.Modal.Filters.Tab = '<%=tabRelFilters.ClientID %>';
            r.Modal.Filters.Tree = '<%=tcRelFilters.TreeClientID %>';
            r.Modal.Filters.Drop = '<%=tcRelFilters.TreeDropClientID %>';

            r.Modal.Fields.MatchFieldSelector.Modal.Control = '<%=modfielditemlist.ClientID %>';
            r.Modal.Fields.MatchFieldSelector.Modal.Panel = '<%= pnlFieldItems.ClientID %>';
            r.Modal.Fields.MatchFieldSelector.FieldItems = '<%=lstFieldItemList.ClientID %>';

            r.Modal.AutocompleteFields.Tab = '<%=this.tabAutocompleteSearchResultsFields.ClientID %>';
            r.Modal.AutocompleteFields.AutocompleteDisplayFields = '<%=txtAutoLookupDisplayField.ClientID %>';
            r.Modal.AutocompleteFields.AutocompleteFieldSelector.Modal.Control = '<%=modAutocompleteFields.ClientID %>';
            r.Modal.AutocompleteFields.AutocompleteFieldSelector.Modal.Panel = '<%= pnlAutocompleteFieldItems.ClientID %>';
            r.Modal.AutocompleteFields.AutocompleteFieldSelector.FieldItems = '<%=ddlAutocompleteFieldToDisplay.ClientID %>';

        }(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne));


        (function (otn) {
            otn.Modal.Panel = '<%=pnlOnetonrelationship.ClientID %>';
            otn.Modal.Control = '<%=modOnetonrelationship.ClientID %>';
            //            t.Modal.TabContainer = '< %=tabConOnetonRelFields.ClientID %>';
            //            t.Modal.General.Tab = '< %=tabOnetonRelDefinition.ClientID %>';
            otn.Modal.General.RelationshipName = '<%=txtOnetonrelationshipname.ClientID %>';
            otn.Modal.General.RelationshipNameReqValidator = '<%=reqOnetonrelationshipname.ClientID %>';
            otn.Modal.General.Description = '<%=txtOnetonrelationshipdescription.ClientID %>';
            otn.Modal.General.BuiltIn = '<%=chkOneToNRelationshipBuiltIn.ClientID%>';
            //            otn.Modal.General.Mandatory = '< %=chkOnetonrelationshipmandatory.ClientID %>';
            //            otn.Modal.General.MandatoryLabel = '< %=lblOnetonrelationshipmandatory.ClientID %>';
            otn.Modal.General.RelationshipEntity = '<%=cmbOnetonrelationshipentity.ClientID %>';
            otn.Modal.General.RelationshipEntityReqValidator = '<%=cmpOnetonrelationshipentity.ClientID %>';
            //otn.Modal.General.Tooltip = '< %=txtOnetonrelationshiptooltip.ClientID %>';
            otn.Modal.General.RelationshipView = '<%=cmbOnetonrelationshipview.ClientID %>';
            otn.Modal.General.RelationshipViewReqValidator = '<%=cmpOnetonrelationshipview.ClientID %>';

        }(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.OnetoN));

        (function (s) {
            s.Modal.Control = '<%=modsummary.ClientID %>';
            s.Modal.Panel = '<%= pnlsummary.ClientID %>';
            s.Modal.PanelHeader = 'divSummaryHeading';
            s.Modal.TabContainer = '<%=tabConSummary.ClientID %>';

            s.Modal.General.Tab = '<%=tabSummaryGenDet.ClientID %>';
            s.Modal.General.Name = '<%=txtsummaryname.ClientID %>';
            s.Modal.General.Description = '<%=txtsummarydescription.ClientID %>';
            s.Modal.General.SummarySource = '<%=cmbsourceentity.ClientID %>';

            s.Modal.Relationships.Tab = '<%= tabSummaryRels.ClientID %>';
            s.Modal.Relationships.Control = 'divavailablerelationships';

            s.Modal.Columns.Tab = '<%= tabSummaryCols.ClientID %>';
            s.Modal.Columns.Control = 'divrelationshipcolumns';

            s.Modal.DisplayFieldModal.Control = '<%=modmtodisplayfield.ClientID %>';
            s.Modal.DisplayFieldModal.Panel = '<%=pnlSummaryMTOFieldSelect.ClientID %>';
            s.Modal.DisplayFieldModal.Tree = '<%=tcMTODisplayField.ClientID %>';

        }(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary));

        (function (v) {

            // Views DomIDs

            v.Grid = '<%= pnlviewgrid.ClientID %>';
            v.Modal.Control = '<%= modview.ClientID %>';
            v.Modal.Panel = '<%= pnlview.ClientID %>';
            v.Modal.PanelHeader = 'divViewHeader';
            v.Modal.TabContainer = '<%= tabConViews.ClientID %>';

            v.Modal.General.Tab = '';
            v.Modal.General.Name = '<%= txtviewname.ClientID %>';
            v.Modal.General.Description = '<%= txtviewdescription.ClientID %>';
            v.Modal.General.Menu = 'divMenu';
            v.Modal.General.MenuDescription = '<%= txtViewMenuDescription.ClientID %>';
            v.Modal.General.ShowRecordCount='<%= chkViewRecordCount.ClientID %>'
            v.Modal.General.AddForm = '<%= cmbviewaddform.ClientID %>';
            v.Modal.General.AddFormSelectionMappings = '<%= imgViewAddFormMappings.ClientID %>';
            v.Modal.General.EditForm = '<%= cmbvieweditform.ClientID %>';
            v.Modal.General.EditFormSelectionMappings = '<%= imgViewEditFormMappings.ClientID %>';
            v.Modal.General.AllowApproval = '<%= chkviewallowapproval.ClientID %>';
            v.Modal.General.AllowDelete = '<%= chkviewallowdelete.ClientID %>';
            v.Modal.General.AllowArchive = '<%= chkviewallowarchive.ClientID %>';
            v.Modal.General.BuiltIn = '<%= chkViewBuiltIn.ClientID %>';

            v.Modal.Fields.Tab = '';
            v.Modal.Fields.TreeContainer = '<%= tcFields.ClientID %>';
            v.Modal.Fields.Tree = '<%= tcFields.TreeClientID %>';
            v.Modal.Fields.Drop = '<%= tcFields.TreeDropClientID %>';

            v.Modal.Filters.Tab = '';
            v.Modal.Filters.TreeContainer = '<%= tcFilters.ClientID %>';
            v.Modal.Filters.Tree = '<%= tcFilters.TreeClientID %>';
            v.Modal.Filters.Drop = '<%= tcFilters.TreeDropClientID %>';

            v.Modal.Sort.Tab = '<%= tabViewSort.ClientID %>';
            v.Modal.Sort.Column = '<%= ddlSortColumn.ClientID %>';
            v.Modal.Sort.Direction = '<%= ddlSortOrder.ClientID %>';
            v.Modal.Sort.OrderValidator = '<%= cmpSortOrder.ClientID %>';

            v.Modal.Icon.SearchFileName = '<%= txtViewCustomIconSearch.ClientID %>';

            v.Modal.General.FormSelectionMappings.Panel = '<%= pnlViewFormSelectionMappings.ClientID %>';
            v.Modal.General.FormSelectionMappings.Modal = '<%= mdlViewFormSelectionMappings.ClientID %>';
            v.Modal.General.FormSelectionMappings.Header = '<%= pnlViewFormSelectionMappingsHeader.ClientID %>';
            v.Modal.General.FormSelectionMappings.Body = '<%= pnlViewFormSelectionMappingsBody.ClientID %>';
       
            v.Modal.General.CustomMenuStructure.Panel = '<%= pnlCustomMenuStructure.ClientID %>';
            v.Modal.General.CustomMenuStructure.Modal = '<%= mdlCustomMenuStructure.ClientID %>';
            v.Modal.General.CustomMenuStructure.MenuTreeData = '<%= menuTreeData.ClientID %>';

        }(SEL.CustomEntityAdministration.DomIDs.Views));

    </script>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            SEL.Common.SetTextAreaMaxLength();
            SEL.CustomEntityAdministration.Views.General.CustomMenuStructure.PageLoad();
            $('.buttonInner').focus(function () {
                $(this).css('background-position', 'left -155px');
                $(this).parent().css('background-position', 'right -155px');
            });
            $('.buttonInner').focusout(function () {
                $(this).css('background-position', 'left top');
                $(this).parent().css('background-position', 'right top');
            });
            $('.smallbuttonInner').focus(function () {
                $(this).css('background-position', 'left -34px');
                $(this).parent().css('background-position', 'right -34px');
            });
            $('.smallbuttonInner').focusout(function () {
                $(this).css('background-position', 'left top');
                $(this).parent().css('background-position', 'right top');
            });
            // Create the dialog area for Available form fields
            $("#dialog").dialog({ resizable: true, autoOpen: false, minWidth: 300, maxWidth: 300, height: 300, zIndex: SEL.CustomEntityAdministration.zIndices.Forms.AvailableFieldsModal(), stack: false, enableDock: true });
            // Set the validation for Default Currency
            $("#" + chkEnableCurrenciesID).click(function () { updateCurrenciesValidator(false); });
            // Update the display of the Default Currency label
            setMonetaryState();
            updateCurrenciesValidator(true);
            $(document).keydown(function (e) {
                if (e.keyCode === 27) // esc
                {
                    e.preventDefault();
                    closeOpenModal();
                }
            });
            $('#divMasterPopup').click(function (e) {
                $('#hrefMasterPopup').focus();
                return false;
            });

            $('#' + chkEnableAudiencesID).click(function() { SEL.CustomEntityAdministration.Base.SetAudienceListState(this); });

            $('#' + cmbtextformatlargeid).change(function () { ToggleMaxLengthDropDown(); });
        });

        function pageLoad() {
            addModalControlFocusOnShowOrHideEvent(modlistitemid, txtlistitemid, true);
            addModalControlFocusOnShowOrHideEvent(modlistitemid, listbox_tovalidate, false);
            addModalControlFocusOnShowOrHideEvent(modattributeid, txtattributenameid, true);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.FieldItems, true);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFieldSelector.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.MatchFields, false);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteDisplayFields, false);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.AutocompleteFields.AutocompleteFieldSelector.FieldItems, false);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.General.RelationshipName, true);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.OnetoN.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.OnetoN.Modal.General.RelationshipName, true);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Attributes.Summary.Modal.General.Name, true);
            addModalControlFocusOnShowOrHideEvent(modtabid, txttabid, true);
            addModalControlFocusOnShowOrHideEvent(modsectionid, txtsectionid, true);
            addModalControlFocusOnShowOrHideEvent(modformid, txtformnameid, true);
            addModalControlFocusOnShowOrHideEvent(SEL.CustomEntityAdministration.DomIDs.Views.Modal.Control, SEL.CustomEntityAdministration.DomIDs.Views.Modal.General.Name, true);


            $f(modmasterpopupid).add_shown(function () {
                $('#hrefMasterPopup').focus();
                $('#divMasterPopup').click(function (e) {
                    $('#hrefMasterPopup').focus();
                    return false;
                });
            });

            SEL.CustomEntityAdministration.Misc.SetupEnterKeyBindings();
        }

        function enablePopupDDL(obj) {
            var ddlPopupWindow = document.getElementById('<%=ddlPopupWindowView.ClientID%>');
            if (ddlPopupWindow.length > 0) {
                ddlPopupWindow.disabled = !obj.checked;
            }
        };
    </script>
   
     <!--[if lte IE 7]>
    <style type="text/css">
        .sm_tabcontainer { padding-top: 3px; }
        .sm_tabheader { padding-top: 0px; padding-bottom: 0px; }
    </style>
    <![endif]-->

    <span id="popupFormOptions" runat="server" clientidmode="Static"></span>
    <span id="popupFieldOptions" runat="server" clientidmode="Static"></span>
    <span id="relFilterHelpArea" class="infoHelpArea" runat="server" style="display:none;" clientidmode="Static">Using the Filter Editor allows you to control the information that is returned for the Relationship, to display only the information you are interested in. For example you may wish to only see results within a certain date range, or that have been created by a certain user.<br /><br />
        Choose from the fields displayed within the Available Fields section to use as the filter, and then select the operator and values to complete the filter.<br /><br />Some special key words have been set-up to allow greater functionality if the Relationship should contain user-specific information. These can be placed in the value section when creating a filter:<br /><div style="padding-left:32px;"><strong>@ME_ID</strong> – The user's identification number.<br /><strong>@ME</strong> – The user's username.<br /><strong>@MY_HIERARCHY</strong> – All of the users underneath the current user, for the chosen hierarchy.</div></span>      
    <span id="formDesignerHelpArea" class="infoHelpArea" runat="server" style="display:none; width:auto;" clientidmode="Static">Once you have become familiar with the Form builder you may wish to speed up the process of creating<br/>the form by using the Keyboard shortcuts.<br /><br />Keyboard shortcuts are a set of key combinations that perform a predefined function within the form <br />builder. <br /><br />These will help to make the process of adding Tabs, Sections and Attributes to a form quicker and easier.  <br /><br />Ctrl+ 1 = New Tab<br />Ctrl+ 2 = New Section<br />Ctrl+ 3 = Edit tab<br />Ctrl+ 4 =Open/Close available fields<br />Ctrl+ 5 = Dock/ undock available fields left<br />Ctrl+ 6 = Dock/ undock available fields right<br /><br />For further information on how to use the GreenLight form builder, please visit the product helptext.</span>
<span id="viewFilterHelpArea" class="infoHelpArea" runat="server" style="display:none;" clientidmode="Static">Using the Filter Editor allows you to control the information that is presented in the View, to display only the information you are interested in. For example for one View you may only require to see records within a certain date range, or that belong to a certain department.<br /><br />
Choose from the Attributes displayed within the Available Fields section to use as the filter, and then select the operator and values to complete the filter.<br /><br />Some special key words have been set-up to allow greater functionality if the View should contain information that is specific to the user presented with the View, to avoid having to create a view for each individual.  These can be placed in the value section when creating a filter:<br /><div style="padding-left:32px;"><strong>@ME_ID</strong> – The user's identification number.
<strong>@ME</strong> – The user's username.</div></span>
    <span id="viewColumnHelpArea" class="infoHelpArea" runat="server" style="display:none;" clientidmode="Static">This option allows you to customise the columns of information to be displayed and the order they should be seen on the screen.<br /><br />Choose from the Attributes on the left and drag to the Chosen Columns box on the right.<br /><br />Use the up <img src="<% = GetStaticLibPath() %>/icons/16/plain/navigate_open.png" alt="Up" style="zoom: 0.7"/> and down <img src="<% = GetStaticLibPath() %>/icons/16/plain/navigate_close.png" alt="Down" style="zoom: 0.7"/> arrows to arrange the fields in the appropriate order. Use the left <img src="<% = GetStaticLibPath() %>/icons/16/plain/navigate_left.png" alt="Left" style="zoom: 0.7"/> button to remove a selection.</span>
<span id="viewFilterModalHelpArea" class="infoHelpArea" runat="server" style="display:none;" clientidmode="Static">To add items to your selected list, drag and drop the required item into the selected area. Alternatively you can click the plus icon <img src="<% = GetStaticLibPath() %>/icons/custom/greenplus.png" alt="Add" style="zoom: 0.7"/>, or double click the required item.<br /><br />To remove an item from your selected list, click on the remove icon <img src="/shared/images/icons/delete2.png" alt="Remove" style="zoom: 0.7"/>, or double click the required item.<br /><br />The add all <img src="<% = GetStaticLibPath() %>/icons/16/plain/navigate_right2.png" alt="Add All" style="zoom: 0.7"/> and remove all <img src="<% = GetStaticLibPath() %>/icons/16/plain/navigate_left2.png" alt="Remove All" style="zoom: 0.7"/> buttons will populate the selected or available lists with the entire list population.<br /><br />Use the search filter at the top left of the list editor to locate an item within a large list.</span>
<span id="displayFieldHelpArea" class="infoHelpArea" runat="server" style="display:none;" clientidmode="Static">This option allows you to build prepopulated fields into the GreenLight form which are associated with the n:1 Attribute you are editing. For example, if you had selected Employees as the 'Related to' Attribute, you could then pick Job Title as a Lookup Display Field.  This would be available for use in the Form Builder in the same way as any other Attribute.<br /><br />Choose from the Available Fields on the left and drag to the Selected Display Fields box on the right.<br /><br /> Use the left <img src="<% = GetStaticLibPath() %>/icons/16/plain/navigate_left.png" alt="Left" style="zoom: 0.7"/> button to remove a display field.</span>
    
    <div id="divPages">
        <div id="pgGeneral" class="primaryPage">
            <div class="formpanel formpanel_padding" style="padding-left:0px;">
                <div class="sectiontitle">
                    General Details
                </div>
               <div class="twocolumn"><asp:Label CssClass="mandatory" ID="lblentityname" runat="server" Text="GreenLight name*" AssociatedControlID="txtentityname"></asp:Label><span class="inputs"><asp:TextBox ID="txtentityname" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftbeEntityName" runat="server" TargetControlID="txtentityname" FilterMode="InvalidChars" InvalidChars="<>" /></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqentityname" runat="server" ErrorMessage="Please enter a GreenLight name." Text="*" ControlToValidate="txtentityname" ValidationGroup="vgMain" Display="Dynamic"></asp:RequiredFieldValidator></span><asp:Label CssClass="mandatory" ID="lblpluralname" runat="server" Text="Plural name*" AssociatedControlID="txtpluralname"></asp:Label><span class="inputs"><asp:TextBox ID="txtpluralname" runat="server" MaxLength="250"></asp:TextBox><cc1:FilteredTextBoxExtender ID="ftbePluralName" runat="server" TargetControlID="txtpluralname" FilterMode="InvalidChars" InvalidChars="<>" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="img4" onmouseover="SEL.Tooltip.Show('eb4686e6-3f3f-46bf-89d0-4437a5c60323', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqpluralname" runat="server" ErrorMessage="Please enter a Plural name." Text="*" ControlToValidate="txtpluralname" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span></div>
                <asp:Panel runat="server" ID="pnlOwnerSelectinator" CssClass="onecolumnsmall"></asp:Panel>
                <div class="onecolumn">
                    <asp:Label ID="lbldescription" runat="server" Text="" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label><span
                        class="inputs"><asp:TextBox ID="txtdescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblBuiltIn" runat="server" Text="System GreenLight" AssociatedControlID="chkBuiltIn"></asp:Label><span
                        class="inputs"><asp:CheckBox ID="chkBuiltIn" runat="server" Enabled="False" /></span><span
                            class="inputicon"></span><span class="inputtooltipfield"><img
                    onmouseover="SEL.Tooltip.Show('4db22863-eb3c-4461-a42a-c4c0e5130949','sm', this);" src="../images/icons/16/plain/tooltip.png"
                    alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                    <asp:Label ID="lblenableattachments" runat="server" Text="Enable attachments" AssociatedControlID="chkenableattachments"></asp:Label><span
                        class="inputs"><asp:CheckBox ID="chkenableattachments" runat="server" /></span><span
                            class="inputicon"></span><span class="inputtooltipfield"><img
                    id="img7" onmouseover="SEL.Tooltip.Show('401f8ac5-019c-4445-98dc-9fb1f2bda8f2','sm', this);" src="../images/icons/16/plain/tooltip.png"
                    alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                </div>
                <div id="divMoneyRow">
                    <div class="twocolumn">
                        <asp:Label ID="lblEnableCurrencies" runat="server" AssociatedControlID="chkEnableCurrencies">Enable monetary record</asp:Label><span
                            class="inputs"><asp:CheckBox ID="chkEnableCurrencies" runat="server" /></span><span
                                class="inputicon"></span><span class="inputtooltipfield"><img
                        id="img5" onmouseover="SEL.Tooltip.Show('e20a1a22-2530-4a49-b8f3-67e9be144421', 'sm', this);" src="../images/icons/16/plain/tooltip.png"
                        alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                        <asp:Label ID="lblDefaultCurrency" runat="server" AssociatedControlID="ddlDefaultCurrency">Default currency</asp:Label><span
                            class="inputs"><asp:DropDownList runat="server" ID="ddlDefaultCurrency" Enabled="false">
                                <asp:ListItem Value="0">[None]</asp:ListItem>
                            </asp:DropDownList>
                        </span><span class="inputicon"></span><span class="inputtooltipfield">
                            <img
                        id="img6" onmouseover="SEL.Tooltip.Show('85c2dbce-c862-4336-880e-c342f27836fb','sm', this);" src="../images/icons/16/plain/tooltip.png"
                        alt="" class="tooltipicon" /></span><span
                            class="inputvalidatorfield">
                            <asp:CompareValidator runat="server" ID="cvDefaultCurrency" ValidationGroup="vgMain"
                                ControlToValidate="ddlDefaultCurrency" Type="Integer" Operator="GreaterThan" Enabled="false"
                                ValueToCompare="0" ErrorMessage="A Default currency must be selected when creating a GreenLight involving monetary records.">*</asp:CompareValidator></span>
                    </div>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblEnableAudiences" runat="server" Text="Enable audiences" AssociatedControlID="chkEnableAudiences"></asp:Label><span
                        class="inputs"><asp:CheckBox ID="chkEnableAudiences" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img
                    id="img8" onmouseover="SEL.Tooltip.Show('6948c214-4953-4b99-89b4-6b78c05288b3','sm', this);" src="../images/icons/16/plain/tooltip.png"
                    alt="" class="tooltipicon" /></span><span
                        class="inputvalidatorfield"></span>
                    <asp:Label ID="lblAudienceBehaviour" runat="server" AssociatedControlID="ddlAudienceBehaviour" CssClass="audienceBehaviour">Default audience access</asp:Label><span class="inputs"><asp:DropDownList ID="ddlAudienceBehaviour" runat="server">
                        <asp:ListItem Value="0">[None]</asp:ListItem>
                        <asp:ListItem Value="1">Everybody</asp:ListItem>
                        <asp:ListItem Value="2">Creator only</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img
                    id="img8" onmouseover="SEL.Tooltip.Show('CA2D94BC-CFC3-4D26-B624-7CCC5315BDA7','sm', this);" src="../images/icons/16/plain/tooltip.png"
                    alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpAudiences" ValidationGroup="vgMain"
                                ControlToValidate="ddlAudienceBehaviour" Type="Integer" Operator="GreaterThan" Enabled="false"
                                ValueToCompare="0" ErrorMessage="A Default audience access must be selected when creating a GreenLight involving audiences.">*</asp:CompareValidator></span>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblallowmergeaccess" runat="server" Text="Enable Torch" AssociatedControlID="chkallowdocmerge"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkallowdocmerge" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="img9" onmouseover="SEL.Tooltip.Show('f9000007-b183-4fd1-b708-2da8972c5d84','sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                    <asp:Label ID="lblFormSelectionAttribute" runat="server" AssociatedControlID="ddlFormSelectionAttribute">Form selection attribute</asp:Label><span class="inputs"><asp:DropDownList ID="ddlFormSelectionAttribute" runat="server" onchange="javascript:SEL.CustomEntityAdministration.Base.FormSelectionAttributeChanged();">
                        <asp:ListItem Value="0">[None]</asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgFormSelectionAttributeTooltip" onmouseover="SEL.Tooltip.Show('596FFAD2-AB81-4DF2-9723-D37BFC97A2C3','sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                </div>
                  <div class="twocolumn">
                    <asp:Label ID="lblAllowPopup" runat="server" Text="Enable pop-up window" AssociatedControlID="chkEnablePopupWindow"></asp:Label><span
                        class="inputs"><asp:CheckBox runat="server" ID="chkEnablePopupWindow" onclick="enablePopupDDL(this);" /></span><span class="inputicon"></span><span class="inputtooltipfield"><img
                    id="img11" onmouseover="SEL.Tooltip.Show('A6477409-1C13-46F9-B467-33E48D3359C2','sm', this);" src="../images/icons/16/plain/tooltip.png"
                    alt="" class="tooltipicon" /></span><span
                        class="inputvalidatorfield"></span>
                    <asp:Label ID="lblPopupView" runat="server" Text="Pop-up window View" AssociatedControlID="ddlPopupWindowView"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlPopupWindowView" Enabled="False">
                        <asp:ListItem Text="[None]" Value="-1" />
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img
                    id="img13" onmouseover="SEL.Tooltip.Show('58B9B344-0055-4DD7-B71B-CB6B3A7693C8','sm', this);" src="../images/icons/16/plain/tooltip.png"
                    alt="" class="tooltipicon" /></span><span
                        class="inputvalidatorfield"></span>
                      <asp:Panel ID="pnlTest" runat="server"></asp:Panel>
                </div>
                <div class="twocolumn">
                    <asp:Label ID="lblenablelocking" runat="server" Text="Enable locking" AssociatedControlID="chkenablelocking"></asp:Label><span
                        class="inputs"><asp:CheckBox ID="chkenablelocking" runat="server" /></span><span
                            class="inputicon"></span><span class="inputtooltipfield"><img id="img7" onmouseover="SEL.Tooltip.Show('AD20B53A-11C4-40A8-AB26-CB1D25A8240E','sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                    
                </div>
                <span style="display: none;">
                    <asp:TextBox ID="hdnAuditAttributeID" runat="server" Style="display: none;" />
                    <asp:TextBox ID="hdnAuditAttributeDisplayName" runat="server" Style="display: none;" />
                </span>

                <div class="sectiontitle">Support Information</div>
                <div class="onecolumnsmall">
                    <asp:Label ID="lblSupportQuestion" runat="server" Text="Question / Statement" AssociatedControlID="txtSupportQuestion"></asp:Label><span class="inputs"><asp:TextBox ID="txtSupportQuestion" runat="server" MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img onmouseover="SEL.Tooltip.Show('41E4B22B-1486-40BC-B36A-74FD6D2F2EC3','sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                </div>
                <asp:Panel runat="server" ID="pnlSupportQuestionSelectinator" CssClass="onecolumnsmall"></asp:Panel>

            </div>
        </div>
        <div id="pgForms" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">            
                <div class="sectiontitle">Current Forms</div>
                <asp:Panel ID="pnlformgrid" runat="server">                
                    <asp:Literal ID="litformgrid" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgAttributes" class="subPage " style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Current Attributes</div>
                <asp:Panel ID="pnlattributegrid" runat="server">
                    <asp:Literal ID="litattributegrid" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div id="pgViews" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">Current Views</div>
                <asp:Panel ID="pnlviewgrid" runat="server">
                    <asp:Literal ID="litviewgrid" runat="server"></asp:Literal>
                </asp:Panel>
            </div>
        </div>
        <div class="formpanel formpanel_padding">
            <helpers:CSSButton id="btnSaveEntity" runat="server" text="save" onclientclick="SEL.CustomEntityAdministration.Base.Save('save entity button');return false;" UseSubmitBehavior="False"/>
            <helpers:CSSButton id="btnCancelEntity" runat="server" text="cancel" onclientclick="CancelEntity();return false;" UseSubmitBehavior="False"/>
        </div>
    </div>
    <div id="dragFieldHolder" class="formpanel" style="height: 0px;"></div>
    <asp:Panel ID="pnladdattribute" runat="server" CssClass="modalpanel formpanel" Style="display: none;">
        <div>
            <div class="sectiontitle" id="divAttributeSectionHeader">
                Add/Edit Attributes
            </div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblattributename" runat="server" Text="Display name*"
                    AssociatedControlID="txtattributename"></asp:Label><span class="inputs"><asp:TextBox
                        ID="txtattributename" runat="server" CssClass="fillspan"
                        MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span id="spReqDisplayName" class="inputmultiplevalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtattributename"
                            ID="reqdisplayname" runat="server" ErrorMessage="Please enter a Display name for this attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic"></asp:RequiredFieldValidator><asp:CustomValidator ID="custAttDisplayName" runat="server" ErrorMessage="GreenLight Currency is not allowed as a Display name for this attribute (reserved keyword).<br/>It is used as a predefined attribute by the system." ValidationGroup="vgAttribute" Text="*" Enabled="true" ClientValidationFunction="ValidateAttributeDisplayName" Display="Dynamic"></asp:CustomValidator></span>
            </div>
            <div class="onecolumn">
                <asp:Label ID="lblattributedescription" runat="server" Text="" AssociatedControlID="txtattributedescription"><p class="labeldescription">Description</p></asp:Label><span
                    class="inputs"><asp:TextBox ID="txtattributedescription" runat="server" TextMode="MultiLine"
                        textareamaxlength="4000"></asp:TextBox></span>
            </div>
            <div class="onecolumn" id="divtooltip" attributeelement="slideDown">
                <asp:Label ID="lblattributetooltip" runat="server" Text="" AssociatedControlID="txtattributetooltip"><p class="labeldescription">Tooltip</p></asp:Label><span class="inputs"><asp:TextBox ID="txtattributetooltip" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image5" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('0e22942a-2be5-49ee-b0d0-dcc2e02d5e7e', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
            </div>
            
            <div class="twocolumn">
                <asp:Label ID="lblattributebuiltin" runat="server" Text="System attribute" AssociatedControlID="chkAttributeBuiltIn"></asp:Label><span class="inputs"><asp:CheckBox ID="chkAttributeBuiltIn" runat="server" Enabled="False" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image30" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4EB2A288-5336-430E-82E6-39CC56892AF9', 'sm', this);" /></span><span class="inputvalidatorfield">&nbsp;</span>            
            </div>

            <div class="twocolumn" id="divmandatory">
                <asp:Label ID="lblattributemandatory" runat="server" Text="Mandatory" AssociatedControlID="chkattributemandatory"></asp:Label><span class="inputs"><asp:CheckBox ID="chkattributemandatory" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image4" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('29f4c70a-8755-4afa-90f4-38a7bfe68f58', 'sm', this);" /></span><span class="inputvalidatorfield">&nbsp;</span><asp:Label ID="lblattributetype" runat="server" Text="Type*" AssociatedControlID="cmbattributetype" CssClass="mandatory"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbattributetype" runat="server">
                    <asp:ListItem Value="" Text="[None]"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Text"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Integer"></asp:ListItem>
                        <asp:ListItem Value="7" Text="Decimal"></asp:ListItem>
                        <asp:ListItem Value="6" Text="Currency"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Yes/No"></asp:ListItem>
                        <asp:ListItem Value="4" Text="List"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Date"></asp:ListItem>
                        <asp:ListItem Value="10" Text="Large Text"></asp:ListItem>
                        <%--<asp:ListItem Value="11" Text="Run Workflow"></asp:ListItem>--%>
                        <asp:ListItem Value="19" Text="Comment"></asp:ListItem> 
                        <asp:ListItem Value="22" Text="Attachment"></asp:ListItem>
                        <asp:ListItem Value="23" Text="Contact"></asp:ListItem>
                </asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image9" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('ffd3888b-95d8-4651-b3a3-3c8957fbaa67', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="cmbattributetype" ID="reqAttributeType" runat="server" ErrorMessage="Please select a Type for the attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator></span>
            </div>
            <div id="audit_row" attributeelement="slideDown">
                <div class="twocolumn" id="divaudit">
                    <asp:Label ID="lblAuditIdentifier" runat="server" Text="Used for audit" AssociatedControlID="chkAuditIdentifier"></asp:Label><span class="inputs"><asp:CheckBox ID="chkAuditIdentifier" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image6" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('45992269-e555-4895-a5ed-46ff7c84b3a0', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label ID="lblUnique" runat="server" Text="Unique" AssociatedControlID="chkUnique"></asp:Label><span class="inputs"><asp:CheckBox ID="chkUnique" runat="server"></asp:CheckBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image8" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="javascript:SEL.Tooltip.Show('8325a791-7771-42c6-ad0c-de98c8f13207', 'sm', this);" /></span><span class="inputvalidatorfield">&nbsp;</span>
                </div>
            </div>
            
            <div class="twocolumn">
                <asp:Label ID="lblDisplayInMobile" runat="server" Text="Display in mobile app" AssociatedControlID="chkDisplayInMobile"></asp:Label><span class="inputs"><asp:CheckBox ID="chkDisplayInMobile" runat="server" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image25" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4e4b65a6-a5db-48a1-a7ac-27aa27602dd5', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label ID="lblEncrypt" runat="server" Text="Encrypt" AssociatedControlID="chkEncrypt" class="encrypt"></asp:Label><span class="inputs encrypt"><asp:CheckBox ID="chkEncrypt" runat="server" class="encrypt"/></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield encrypt"><asp:Image ID="imgEncrypt" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('31C5675E-CC91-4E9F-8B83-49BF680A6E41', 'sm', this);" /></span>
            </div>

            <div id="divTextOptions" class="twocolumn" attributeelement="slideUp">
                    <label id="lbltextformat" for="<%= cmbtextformat.ClientID%>" class="mandatory">
                        Format*</label><span class="inputs"><asp:DropDownList ID="cmbtextformat" runat="server">
                            <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                            <asp:ListItem Text="Single Line" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Multiple Line" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                        </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span
                        class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="cmbtextformat" ID="reqTextFormat" runat="server" ErrorMessage="Please select a Format for this attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span><label id="lblmaximumlength" for="<%=txtmaxlength.ClientID %>">Maximum length</label><span class="inputs"><asp:TextBox ID="txtmaxlength" runat="server" MaxLength="6" CssClass="fillspan"></asp:TextBox></span><span
                                        class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image7" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4ae663e2-105a-46f3-a4a0-5ee3fd56cd97', 'sm', this);" /></span><span class="inputmultiplevalidatorfield"><asp:CompareValidator runat="server" ID="cmpMaxlength2" ControlToValidate="txtmaxlength" Type="Integer" Operator="GreaterThanEqual" ValidationGroup="vgAttribute" Text="*" ErrorMessage="Please select a number greater than or equal to 0 for Maximum length." Display="Dynamic" ValueToCompare="0"></asp:CompareValidator><asp:CompareValidator runat="server" ID="cmpSingleLineMaxLength" ControlToValidate="txtmaxlength" Type="Integer" Operator="LessThanEqual" ValidationGroup="vgAttribute" Text="*" ErrorMessage="Please select a number less than or equal to 500 for Maximum length." Display="Dynamic" ValueToCompare="500" Enabled="false"></asp:CompareValidator></span>
            </div>
            <div class="twocolumn" style="display: none;" id="divLargeTextOptions" attributeelement="slideUp">
                <label id="lblformatlarge" for="<%=cmbtextformatlarge.ClientID %>" class="mandatory">
                    Format*</label><span class="inputs"><asp:DropDownList ID="cmbtextformatlarge" runat="server"
                        CssClass="fillspan">
                        <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                        <asp:ListItem Text="Multiple Line" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Formatted Text Box" Value="6"></asp:ListItem>
                    </asp:DropDownList>
                    </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"></span><span
                        class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="cmbtextformatlarge" ID="reqLargeTextFormat" runat="server" ErrorMessage="Please select a Format for this attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span><span clientidmode="Static" id="maxLengthArea"><label id="lblmaxlengthlarge" for="<%=txtmaxlengthlarge.ClientID %>">Maximum length</label><span class="inputs"><asp:TextBox ID="txtmaxlengthlarge" runat="server"
                                CssClass="fillspan" MaxLength="6"></asp:TextBox></span><span
                                class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image15" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4ae663e2-105a-46f3-a4a0-5ee3fd56cd97', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpMaxlengthLarge" ControlToValidate="txtmaxlengthlarge" Type="Integer" Operator="GreaterThanEqual" ValidationGroup="vgAttribute" Text="*" ErrorMessage="Please select a number greater than or equal to 0 for Maximum length." ValueToCompare="0" Display="Dynamic"></asp:CompareValidator></span></span><span class="stripFont" style="display: none"><asp:Label runat="server" AssociatedControlID="chkStripFont" ID="lblStripFont">Remove font formatting from toolbar in formatted text box</asp:Label><span class="inputs"><asp:CheckBox ID="chkStripFont" runat="server" CssClass="fillspan"></asp:CheckBox></span></span>
            </div>
                
            <div id="divDecimalOptions" style="display: none;" class="twocolumn" attributeelement="slideUp">
                <label id="lblprecision" for="<%=txtprecision.ClientID %>" class="mandatory">
                    Precision*</label><span class="inputs"><asp:TextBox ID="txtprecision" runat="server" 
                        MaxLength="1" CssClass="fillspan"></asp:TextBox></span><span
                            class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image10" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('88abb6b4-1bf1-4a2b-9bcb-a3d509a08f9c', 'sm', this);" /></span><span class="inputmultiplevalidatorfield"><asp:RequiredFieldValidator
                                ControlToValidate="txtprecision" ID="reqPrecision" runat="server" ErrorMessage="Please enter a number for Precision (decimal places 1-5)."
                                ValidationGroup="vgAttribute" Text="*" Enabled="false" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator
                                    runat="server" ID="cmpPrecision1" ControlToValidate="txtprecision" ValueToCompare="0"
                                    Operator="GreaterThan" Type="Integer" ValidationGroup="vgAttribute" Text="*"
                                    ErrorMessage="Please select a Precision value greater than 0." Enabled="false" Display="Dynamic"></asp:CompareValidator><asp:CompareValidator
                                        runat="server" ID="cmpPrecision2" ControlToValidate="txtprecision" ValueToCompare="6"
                                        Operator="LessThan" Type="Integer" ValidationGroup="vgAttribute" Text="*" ErrorMessage="Please select a Precision value less than 6."
                                        Enabled="false" Display="Dynamic"></asp:CompareValidator></span>
            </div>
            <div id="divDateOptions" style="display: none;" class="twocolumn" attributeelement="slideUp">
                <label id="lbldateformat" for="<%=cmbdateformat.ClientID %>" class="mandatory">
                    Format*</label><span class="inputs"><asp:DropDownList ID="cmbdateformat" runat="server">
                        <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                        <asp:ListItem Text="Date Only" Value="4"></asp:ListItem>
                        <asp:ListItem Text="Time Only" Value="5"></asp:ListItem>
                        <asp:ListItem Text="Date and Time" Value="3"></asp:ListItem>
                    </asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="cmbdateformat" ID="reqDateFormat" runat="server" ErrorMessage="Please select a Format for this attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span>
                    
            </div>
            <div id="divTickboxOptions" style="display: none;" class="twocolumn" attributeelement="slideUp">
                <label id="lbldefaultvalue" for="<%=cmbdefaultvalue.ClientID %>" class="mandatory">
                    Default value*</label><span class="inputs"><asp:DropDownList ID="cmbdefaultvalue" 
                        runat="server">
                        <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                        <asp:ListItem Text="No" Value="No"></asp:ListItem>
                        <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                    </asp:DropDownList>
                    </span><span class="inputicon"></span><span class="inputtooltipfield"></span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ControlToValidate="cmbdefaultvalue" ID="reqDefaultYesNo" runat="server" ErrorMessage="Please select a Default value for this attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span>
            </div>
            <div id="divListOptions" style="display: none;" class="onecolumn" attributeelement="slideUp">
                <label id="lbllstitems" for="<%=lstitems.ClientID %>" class="mandatory">List items*</label><span class="inputs"><asp:ListBox ID="lstitems" runat="server"></asp:ListBox></span><span class="inputicon"><a href="javascript:showListItemModal(false);"><img src="../images/icons/16/plain/add2.png" alt="New List Item" /></a><a href="javascript:editListItem();"><img src="../images/icons/edit.gif" alt="Edit List Item" /></a><a href="javascript:removeListItem();"><img src="../images/icons/delete2.gif" alt="Delete List Item" /></a></span><span class="inputtooltipfield"><asp:Image ID="Image12" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('7f82bac8-b7be-46c7-8820-dc888e0bb86a', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="custAttListItem" runat="server" ErrorMessage="Please add a List item." ValidationGroup="vgAttribute" Text="*" Enabled="false" ClientValidationFunction="ValidateListBox" Display="Dynamic"></asp:CustomValidator></span>
            <input type="hidden" id="txtlistitems" name="txtlistitems" />
            </div>
            <div id="divAdviceText" style="display: none;" class="onecolumn" attributeelement="slideUp">
                <label id="lblAdviceText" for="<%=txtAdviceText.ClientID %>" class="mandatory">
                    Comment advice text*</label><span class="inputs"><asp:TextBox ID="txtAdviceText" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image13" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('371acc9c-8e9b-430c-9861-1c6d7b139edd', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtAdviceText" ID="reqComment" runat="server" ErrorMessage="Please enter a value for Comment advice text." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span>
            </div>
            <div id="divImageLibrary" style="display: none;" class="twocolumn" attributeelement="slideUp">
                <label id="lblImageLibrary" for="<%=chkImageLibrary.ClientID %>">Enable image library</label><span class="inputs"><asp:CheckBox ClientIDMode="static" runat="server" ID="chkImageLibrary" Checked="False" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image11" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('842C32D3-559A-4CCB-97EC-E5FB924A53B7', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
            </div>
            <div id="divContactOptions" style="display: none;" class="twocolumn" attributeelement="slideUp">
                <label id="lblContactFormat" for="<%=cmbContactFormat.ClientID %>" class="mandatory">
                    Format*</label><span class="inputs"><asp:DropDownList ID="cmbContactFormat" 
                        runat="server">
                        <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                        <asp:ListItem Text="E-Mail" Value="11"></asp:ListItem>
                        <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                        <asp:ListItem Text="SMS" Value="13"></asp:ListItem>
                    </asp:DropDownList>
                    </span><span class="inputicon"></span><span class="inputtooltipfield">
                        <asp:Image runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('56F46173-0E95-4661-A1FF-D359C1D416EB', 'sm', this);" /></span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ControlToValidate="cmbContactFormat" ID="reqContactFormat" runat="server" ErrorMessage="Please select a Format for this attribute." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span>
            </div>
            <div id="divWorkflowOptions" style="display: none;" class="onecolumnsmall" attributeelement="slideUp">
                <label id="lblattributeworkflow" for="<%=cmbattributeworkflow.ClientID %>">
                    Workflow</label><span class="inputs"><asp:DropDownList ID="cmbattributeworkflow" 
                        runat="server">
                    </asp:DropDownList>
                    </span>
            </div>
            <div id="divDisplayWidthOptions" style="display: none;" class="twocolumn" attributeelement="slideUp">
                <label id="lblDisplayWidth" for="<%=cmbDisplayWidth.ClientID %>" class="mandatory">
                    Display width*</label><span class="inputs"><asp:DropDownList ID="cmbDisplayWidth" 
                        runat="server">
                        <asp:ListItem Text="[None]" Value=""></asp:ListItem>
                        <asp:ListItem Text="Standard" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Wide" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                    </span><span class="inputicon"></span><span class="inputtooltipfield">
                        <asp:Image ID="Image14" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('7274a05a-da18-4875-b868-5f8f206b62ea', 'sm', this);" /></span>
                <span class="inputvalidatorfield">
                    <asp:RequiredFieldValidator ControlToValidate="cmbDisplayWidth" ID="reqDisplayWidth" runat="server" ErrorMessage="Please select a Display width." ValidationGroup="vgAttribute" Text="*" Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator></span>
            </div>
            <asp:Panel ID="pnllistitem" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
                <div class="sectiontitle" id="divListItem">
                    Add/Edit List Item
                </div>
                <div class="twocolumn">
                    <asp:Label CssClass="mandatory" ID="lbllistitem" runat="server" Text="List item*" AssociatedControlID="txtlistitem"></asp:Label><span class="inputs"><asp:TextBox ID="txtlistitem" runat="server" MaxLength="150" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtlistitem" ID="reqListItem" runat="server" ErrorMessage="Please add a List item." ValidationGroup="vgAttributeListItem" Text="*" Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator></span>
                </div>
                <div class="twocolumn">
                    <asp:Label runat="server" ID="lblArchiveListItem" AssociatedControlID="chkArchiveListItem" Text="Archived"></asp:Label>
                    <span class="inputs">
                        <asp:CheckBox runat="server" ID="chkArchiveListItem" /></span></div>
                <div class="formbuttons">
                    <helpers:CSSButton ID="btnSaveListItem" runat="server" Text="save" OnClientClick="addListItem();return false;" UseSubmitBehavior="False" />
                    <helpers:CSSButton ID="btnCancelListItem" runat="server" Text="cancel" OnClientClick="hideListItemModal();return false;" UseSubmitBehavior="False" />
                </div>
            </asp:Panel>
            <cc1:ModalPopupExtender ID="modlistitem" runat="server" TargetControlID="lnklistitem"
                PopupControlID="pnllistitem" BackgroundCssClass="modalBackground" CancelControlID="btnCancelListItem">
                </cc1:ModalPopupExtender>
            <asp:LinkButton ID="lnklistitem" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveAttribute" runat="server" Text="save" OnClientClick="saveAttribute();return false;" />
            <helpers:CSSButton ID="btnCancelAttribute" runat="server" Text="cancel" OnClientClick="closeAttributeModal();return false;" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modattribute" runat="server" TargetControlID="lnkattribute" PopupControlID="pnladdattribute" BackgroundCssClass="modalBackground" CancelControlID="btnCancelAttribute"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkattribute" runat="server" Style="display: none;">LinkButton</asp:LinkButton>


    <asp:Panel ID="pnlntoOnerelationship" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 900px">
        <div id="RelModContainer" clientidmode="Static">
            <div class="sectiontitle" id="divManyToOneRelationshipHeading">
                New n:1 Relationship Attribute
            </div>
            <asp:Image runat="server" ID="imgManyToOneFilterHelp" ClientIDMode="Static" ImageUrl="/icons/24/plain/information.png" Style="display: none; position: absolute;" AlternateText="Show Filter Editor Information" />
            <asp:Image runat="server" ID="imgDisplayFieldHelp" ClientIDMode="Static" ImageUrl="/icons/24/plain/information.png" Style="display: none; position: absolute;" AlternateText="Show Lookup Display Field Editor Information" />
            <cc1:TabContainer ID="tabConRelFields" runat="server" OnClientActiveTabChanged="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Modal.TabChange" Style="height: auto">
                <cc1:TabPanel runat="server" ID="tabRelDefinition">
                    <HeaderTemplate>General Details</HeaderTemplate>
                    <ContentTemplate>
                        <div class="sectiontitle">General Details</div>
                        <div class="onecolumnsmall">
                            <asp:Label CssClass="mandatory" ID="lblntoOnerelationshipdisplayname" runat="server" Text="Display name*" AssociatedControlID="txtntoOnerelationshipname"></asp:Label><span class="inputs"><asp:TextBox ID="txtntoOnerelationshipname" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtntoOnerelationshipname" ID="reqntoOnerelationshipname" runat="server" ErrorMessage="Please enter a Display name for this attribute." ValidationGroup="vgManyToOneRelationship" Text="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
                        </div>
                        <div class="onecolumn">
                            <asp:Label ID="lblntoOnerelationshipdescription" runat="server" Text="" AssociatedControlID="txtntoOnerelationshipdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtntoOnerelationshipdescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span>
                        </div>
                        <div class="onecolumn">
                            <asp:Label ID="lblntoOnerelationshiptooltip" runat="server" Text="" AssociatedControlID="txtntoOnerelationshiptooltip"><p class="labeldescription">Tooltip</p></asp:Label><span class="inputs"><asp:TextBox ID="txtntoOnerelationshiptooltip" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span>
                        </div>
                        <div class="twocolumn" id="divntoOnerelationshipMain">
                            <asp:Label ID="lblNToOneRelationshipBuiltIn" runat="server" Text="System attribute" AssociatedControlID="chkNToOneRelationshipBuiltIn"></asp:Label><span class="inputs" id="spanRelMainInputs"><asp:CheckBox ID="chkNToOneRelationshipBuiltIn" runat="server" Enabled="False" /></span><span class="inputicon" id="spanMainIcons"></span><span class="inputtooltipfield" id="spanMainTooltip"><img id="img2" onmouseover="SEL.Tooltip.Show('4EB2A288-5336-430E-82E6-39CC56892AF9', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield" id="spanMainValidator"></span><asp:Label ID="lblntoOnerelationshipmandatory" runat="server" Text="Mandatory" AssociatedControlID="chkntoOnerelationshipmandatory"></asp:Label><span class="inputs" id="spanRelMainInputs"><asp:CheckBox ID="chkntoOnerelationshipmandatory" runat="server" /></span><span class="inputicon" id="spanMainIcons"></span><span class="inputtooltipfield" id="spanMainTooltip"><img id="img2" onmouseover="SEL.Tooltip.Show('29f4c70a-8755-4afa-90f4-38a7bfe68f58', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield" id="spanMainValidator"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label CssClass="mandatory" ID="lblntoOnerelationshipentity" runat="server" Text="Related to*" AssociatedControlID="cmbntoOnerelationshipentity"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbntoOnerelationshipentity" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltipntoOnerelationshipTable" onmouseover="SEL.Tooltip.Show('6763c620-de9b-4813-9571-bb1d5fbda86d', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpntoOnerelationshipentity" ControlToValidate="cmbntoOnerelationshipentity" ValueToCompare="0" Operator="GreaterThan" ValidationGroup="vgManyToOneRelationship" Text="*" ErrorMessage="Please select a Related to from the list."></asp:CompareValidator></span>
                        </div>
                    </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel runat="server" ID="tabRelFields" ClientIDMode="Static">
                <HeaderTemplate>Fields</HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">Fields</div>
                    <div class="onecolumnsmall">
                        <asp:Label runat="server" ID="lblmtodisplayfield" Text="Display field*" CssClass="mandatory" AssociatedControlID="cmbmtodisplayfield"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="cmbmtodisplayfield" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="img1" onmouseover="SEL.Tooltip.Show('c799cc9a-902a-4f24-a6c4-aea9ab23d033', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpmtodisplayfield" Display="Dynamic" ControlToValidate="cmbmtodisplayfield" Operator="GreaterThan" ValidationGroup="vgManyToOneRelationship" ValueToCompare="0" ErrorMessage="Please select a Display field." Text="*"></asp:CompareValidator></span>
                    </div>
                    <div class="onecolumn">
                        <asp:Label runat="server" ID="lblmtomatchfields" CssClass="mandatory" AssociatedControlID="cmbmtomatchfields" Text="Lookup fields to match*"></asp:Label><span class="inputs"><asp:ListBox SelectionMode="Multiple" runat="server" ID="cmbmtomatchfields" CssClass="fillspan"></asp:ListBox></span><span class="inputicon"><a href="javascript:SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Modal.Show(false);"> <img src="../images/icons/16/plain/add2.png" alt="New Match Field" /></a><a href="javascript:SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Remove();"><img src="../images/icons/delete2.gif" alt="Delete Match Field" /></a></span><span class="inputtooltipfield">
                        <img id="img3" onmouseover="SEL.Tooltip.Show('84ac7ea6-7059-4ca5-aa06-422fd97f03a8', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="custmtomatchfields" runat="server" ErrorMessage="Please add a Lookup field to match." ValidationGroup="vgManyToOneRelationship" Text="*" ClientValidationFunction="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.ValidateFieldMatchList" Display="Dynamic"></asp:CustomValidator></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblmtomaxrows" AssociatedControlID="txtmtomaxrows" Text="Suggestions limit"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtmtomaxrows" CssClass="fillspan" MaxLength="3"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="img10" onmouseover="SEL.Tooltip.Show('b4c361f4-fb33-4266-921e-5f41e389dc1e', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield">
                        <asp:CompareValidator runat="server" ID="cmpmtomaxrows" ControlToValidate="txtmtomaxrows" Operator="GreaterThanEqual" ValueToCompare="0" Type="Integer" Text="*" ErrorMessage="Suggestions limit must be a numeric value between 0 and 999." Display="Dynamic" ValidationGroup="vgManyToOneRelationship"></asp:CompareValidator></span>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabLDF" ClientIDMode="Static">
                    <HeaderTemplate>Lookup Display Fields</HeaderTemplate>
                    <ContentTemplate>
                        <div class="sectiontitle">Lookup Display Fields</div>
                        <div class="modalcontentssmall">
                            <div class="onecolumnpanel">Use this editor to create lookup display fields that will create read-only fields linked to the Many to One.</div>
                            <helpers:TreeCombo ID="tcLDF" runat="server" ComboType="TreeAndLookUpDisplayFields" ShowButtonMenu="true" Width="882" Height="310" LeftPanelWidth="270" LeftTitle="Available Fields" RightTitle="Selected Display Fields" WebServicePath="~/shared/webservices/svcCustomEntities.asmx" FilterValidationGroup="vgFilter" WebServiceInitialTreeNodesMethod="GetInitialTreeNodesForManyToOne" WebServiceSelectedNodesMethod="GetSelectedNodesForLookupDisplayFields" ThemesPath="/static/js/jstree/themes/" RenderFilterModal="False" />
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
            <cc1:TabPanel runat="server" ID="tabRelFilters" ClientIDMode="Static">
                <HeaderTemplate>Filters</HeaderTemplate>
                    <ContentTemplate>
                        <div class="sectiontitle">Filters</div>
                        <div class="modalcontentssmall">
                            <div class="onecolumnpanel">Use this editor to create filters that will restrict the number of records returned for the lookup options.</div>
                            <helpers:TreeCombo ID="tcRelFilters" runat="server" ComboType="TreeAndFilters" ShowButtonMenu="true" AllowDuplicatesInDrop="True" Width="882" Height="310" LeftPanelWidth="270" LeftTitle="Available Fields" RightTitle="Selected Filters" WebServicePath="~/shared/webservices/svcCustomEntities.asmx" WebServiceInitialTreeNodesMethod="GetInitialTreeNodesForManyToOne" WebServiceSelectedNodesMethod="GetSelectedNodesForManyToOne" ThemesPath="/static/js/jstree/themes/" FilterValidationGroup="vgFieldFilter" RenderFilterModal="False" />
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="tabAutocompleteSearchResultsFields" ClientIDMode="Static">
                <HeaderTemplate>Autocomplete Display Fields</HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">Fields</div>
                    <div class="onecolumn">
                        <asp:Label runat="server" ID="lblAutoLookupFields" AssociatedControlID="txtAutoLookupDisplayField" Text="Autocomplete display fields"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtAutoLookupDisplayField" onclick="SEL.CustomEntityAdministration.Forms.SelectText(this);" CssClass="AutoCompleteAttribute" TextMode="MultiLine"></asp:TextBox></span><span class="inputicon"><a href="javascript:SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.AutocompleteDisplayFieldItem.Modal.Show(false);"> <img src="../images/icons/16/plain/add2.png" alt="New Field" /></a><a href="javascript:SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.AutocompleteDisplayFieldItem.Remove();"><img src="../images/icons/delete2.gif" alt="Delete Field" /></a></span><span class="inputtooltipfield">
                        <img id="imgToolTipAutocomplete" onmouseover="SEL.Tooltip.Show('305B9139-2AA9-488E-9B08-A1B6265237AE', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"></span>
                    </div>
               </ContentTemplate>
            </cc1:TabPanel>
            </cc1:TabContainer>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaventoOnerelationship" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Save();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelntoOnerelationship" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.Modal.Close();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlOnetonrelationship" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 900px">
            <div class="sectiontitle" id="divOneToManyRelationshipHeading">
                New 1:n Relationship Attribute
            </div>
            <asp:Image runat="server" ID="imgOneToManyFilterHelp" ClientIDMode="Static" ImageUrl="/icons/24/plain/information.png" Style="display: none; position: absolute;" AlternateText="Show Filter Editor Information" />
            <div class="twocolumn" id="divOnetonrelationshipMain">
                <asp:Label CssClass="mandatory" ID="lblOnetonrelationshipdisplayname" runat="server" Text="Display name*" AssociatedControlID="txtOnetonrelationshipname"></asp:Label><span class="inputs"><asp:TextBox ID="txtOnetonrelationshipname" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ControlToValidate="txtOnetonrelationshipname" ID="reqOnetonrelationshipname" runat="server" ErrorMessage="Please enter a Display name for this attribute." ValidationGroup="vgOneToManyRelationship" Text="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
            <div class="onecolumn">
                <asp:Label ID="lblOnetonrelationshipdescription" runat="server" Text="" AssociatedControlID="txtOnetonrelationshipdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtOnetonrelationshipdescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblOneToNRelationshipBuiltIn" runat="server" Text="System attribute" AssociatedControlID="chkOneToNRelationshipBuiltIn"></asp:Label><span class="inputs" id="spanRelMainInputs"><asp:CheckBox ID="chkOneToNRelationshipBuiltIn" runat="server" Enabled="False" /></span><span class="inputicon" id="spanMainIcons"></span><span class="inputtooltipfield" id="spanMainTooltip"><img id="img2" onmouseover="SEL.Tooltip.Show('4EB2A288-5336-430E-82E6-39CC56892AF9', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield" id="spanMainValidator"></span>
            </div>
            <div class="onecolumnsmall">
                <asp:Label CssClass="mandatory" ID="lblOnetonrelationshipentity" runat="server" Text="Related to*" AssociatedControlID="cmbOnetonrelationshipentity"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbOnetonrelationshipentity" runat="server" CssClass="fillspan"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"><img id="imgtooltipOnetonrelationshipTable" onmouseover="SEL.Tooltip.Show('7d0a0dcc-873f-4a78-b7aa-da7a53a1c304', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpOnetonrelationshipentity" ControlToValidate="cmbOnetonrelationshipentity" ValueToCompare="0" Operator="GreaterThan" ValidationGroup="vgOneToManyRelationship" Text="*" ErrorMessage="Please select a Related to from the list."></asp:CompareValidator></span>
            </div>
            <div class="onecolumnsmall">
                <asp:Label ID="lblOnetonrelationshipview" runat="server" Text="View*" CssClass="mandatory" AssociatedControlID="cmbOnetonrelationshipview"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbOnetonrelationshipview" runat="server"></asp:DropDownList></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="img12" onmouseover="SEL.Tooltip.Show('1bae0e0c-d69c-4454-add0-c48efa2fd003', 'sm', this);" src="../images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpOnetonrelationshipview" Display="Dynamic" ControlToValidate="cmbOnetonrelationshipview" ValidationGroup="vgOneToManyRelationship" Text="*" Operator="GreaterThan" ValueToCompare="0" ErrorMessage="Please select a View to be used."></asp:CompareValidator></span>
            </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveOnetonrelationship" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.OneToMany.Save();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelOnetonrelationship" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.OneToMany.Modal.Close();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modOnetonrelationship" runat="server" TargetControlID="lnkOnetonrelationship" PopupControlID="pnlOnetonrelationship" BackgroundCssClass="modalBackground" CancelControlID="btnCancelOnetonrelationship"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkOnetonrelationship" runat="server" Style="display: none;">LinkButton</asp:LinkButton>


    <cc1:ModalPopupExtender ID="modntoOnerelationship" runat="server" TargetControlID="lnkntoOnerelationship" PopupControlID="pnlntoOnerelationship" BackgroundCssClass="modalBackground" CancelControlID="btnCancelntoOnerelationship"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkntoOnerelationship" runat="server" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel ID="pnlFieldItems" runat="server" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle">New Lookup Field To Match</div>
        <div class="onecolumnsmall">
            <asp:Label runat="server" ID="lblFieldItemList" CssClass="mandatory" Text="Lookup field to match*" AssociatedControlID="lstFieldItemList"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="lstFieldItemList"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="reqFieldItemList" ControlToValidate="lstFieldItemList" ValidationGroup="vgFieldItemList" Text="*" ErrorMessage="Please select a Lookup field to match." Operator="GreaterThan" ValueToCompare="0" Display="Dynamic"></asp:CompareValidator></span>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveFieldListItem" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Save();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelFieldItem" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.FieldItem.Modal.Close();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modfielditemlist" runat="server" TargetControlID="lnkfielditem" PopupControlID="pnlFieldItems" BackgroundCssClass="modalBackground" CancelControlID="btnCancelFieldItem"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkfielditem" Style="display: none;">LinkButton</asp:LinkButton>
    
    <asp:Panel ID="pnlAutocompleteFieldItems" runat="server" CssClass="modalpanel formpanel" Style="display: none;">
        <div class="sectiontitle">Autocomplete field to display</div>
        <div class="onecolumnsmall">
            <asp:Label runat="server" ID="Label1" Text="Field to display in autocomplete result" AssociatedControlID="ddlAutocompleteFieldToDisplay"></asp:Label><span class="inputs"><asp:DropDownList runat="server" ID="ddlAutocompleteFieldToDisplay"></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveAutoCompleteField" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.AutocompleteDisplayFieldItem.Save();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelAutocompleteFieldItem" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.AutocompleteDisplayFieldItem.Modal.Close();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modAutocompleteFields" runat="server" TargetControlID="lnkAutocompleteField" PopupControlID="pnlAutocompleteFieldItems" BackgroundCssClass="modalBackground" CancelControlID="btnCancelFieldItem"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkAutocompleteField" Style="display: none;">LinkButton</asp:LinkButton>
    
    

    <asp:Panel runat="server" ID="pnlsummary" CssClass="modalpopup formpanel" Style="display: none; width: 990px;">
        <div class="sectiontitle" id="divSummaryHeading">
            Add/Edit Summary
        </div>
        <cc1:TabContainer ID="tabConSummary" runat="server" ActiveTabIndex="0">
            <cc1:TabPanel ID="tabSummaryGenDet" runat="server">
                <HeaderTemplate>
                    General Details
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">
                        General Details
                    </div>
                    <div id="divsummarygendetails" style="overflow: auto; height: 250px;">
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblsummaryname" AssociatedControlID="txtsummaryname"
                                CssClass="mandatory" Text="Display name*"></asp:Label>
                            <span class="inputs">
                                <asp:TextBox runat="server" ID="txtsummaryname" CssClass="fillspan" MaxLength="250"></asp:TextBox>
                            </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                class="inputvalidatorfield">
                                <asp:RequiredFieldValidator ID="reqsummaryname" runat="server" ControlToValidate="txtsummaryname"
                                    Display="Dynamic" ErrorMessage="Please enter a Display name for the attribute."
                                    Text="*" ValidationGroup="vgSummary"></asp:RequiredFieldValidator>
                            </span>
                        </div>
                        <div class="onecolumn">
                            <asp:Label runat="server" ID="lblsummarydescription" AssociatedControlID="txtsummarydescription"
                                Text=""><p class="labeldescription">Description</p></asp:Label>
                            <span class="inputs">
                                <asp:TextBox runat="server" ID="txtsummarydescription" CssClass="fillspan" TextMode="MultiLine"
                                    textareamaxlength="4000"></asp:TextBox>
                            </span><span class="inputicon"></span><span class="inputtooltipfield"></span><span
                                class="inputvalidatorfield"></span>
                        </div>
                        <div class="onecolumnsmall">
                            <asp:Label runat="server" ID="lblsourceentity" AssociatedControlID="cmbsourceentity"
                                CssClass="mandatory" Text="Summary source*"></asp:Label>
                            <span class="inputs">
                                <asp:DropDownList runat="server" ID="cmbsourceentity" CssClass="fillspan">
                                </asp:DropDownList>
                            </span><span class="inputicon">&nbsp;</span> <span class="inputtooltipfield">
                                <img id="imgtooltipnumber1" onmouseover="SEL.Tooltip.Show('9dd75b9f-ceea-4d68-a70f-423b52be5a17', 'sm', this);" src="../images/icons/16/plain/tooltip.png"
                                    alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator
                                        runat="server" ID="reqsourceentity" ControlToValidate="cmbsourceentity" Operator="NotEqual"
                                        ValueToCompare="0" ValidationGroup="vgSummary" Text="*" ErrorMessage="Please select a Summary source."
                                        Display="Dynamic"></asp:CompareValidator>
                                    </span>
                        </div>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabSummaryRels" runat="server">
                <HeaderTemplate>
                    Summary Relationship Selection
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">
                        Summary Relationship Selection
                    </div>
                    <div id="divavailablerelationships" style="overflow: auto; height: 250px;">
                        There are no selections available
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabSummaryCols" runat="server">
                <HeaderTemplate>
                    Summary Column Selection
                </HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">
                        Summary Column Selection
                    </div>
                    <div id="divrelationshipcolumns" style="overflow: auto; height: 250px;">
                        There are no columns available
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveSummary" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Attributes.Summary.saveSummary();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelSummary" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Attributes.Summary.Modal.Hide();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <div id="availableFieldDoms"></div>
    <asp:Panel ID="pnlcopyform" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div>
            <div class="sectiontitle">Copy Form</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblnewformname" runat="server" Text="Form name*" AssociatedControlID="txtcopyformname"></asp:Label><span class="inputs"><asp:TextBox ID="txtcopyformname" runat="server" CssClass="fillspan" MaxLength="100"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgCopyFormTooltip" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('1201a20e-8e04-4329-b97d-0dac8fe36e9d', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqNewFormName" Text="*" ValidationGroup="vgCopyForm" ControlToValidate="txtcopyformname" ErrorMessage="Please enter a Form name." Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveCopyForm" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Forms.Copy();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelCopyForm" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Forms.HideTabModal();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modcopyform" runat="server" TargetControlID="lnkcopyform" PopupControlID="pnlcopyform" BackgroundCssClass="modalBackground" CancelControlID="btnCancelCopyForm" DynamicServicePath="" Enabled="True"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkcopyform" runat="server" Style="display: none"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="modsummary" runat="server" TargetControlID="lnksummary" PopupControlID="pnlsummary" BackgroundCssClass="modalBackground" CancelControlID="btnCancelSummary"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnksummary" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel ID="pnlform" runat="server" CssClass="modalpopup formpanel" style="display: none; width: 890px; padding:20px;">
            <div class="sectiontitle" id="divFormSectionHeader">Add/Edit Form</div>
            <asp:Image runat="server" ID="imgFormDesignerHelp" ClientIDMode="Static" ImageUrl="/shared/images/icons/24/plain/information.png" style="display: none; position: absolute;" AlternateText="Show Hot Key Information" />
        <cc1:TabContainer ID="tabConForms" runat="server" ActiveTabIndex="1" OnClientActiveTabChanged="SEL.CustomEntityAdministration.Forms.FormModalTabChange">
            <cc1:TabPanel ID="tabGenDet" runat="server">
                <HeaderTemplate>
                General Details
                </HeaderTemplate>

                <ContentTemplate>
                    <div class="sectiontitle">General Details</div>
                        <div class="modalcontents">
                        <div class="twocolumn">
                            <asp:Label CssClass="mandatory" ID="lblformname" runat="server" Text="Form name*" AssociatedControlID="txtformname"></asp:Label><span class="inputs"><asp:TextBox ID="txtformname" runat="server" MaxLength="100"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqFormName" runat="server" ControlToValidate="txtformname" ErrorMessage="Please enter a Form name." ValidationGroup="vgFormEdit" Text="*" Display="Dynamic"></asp:RequiredFieldValidator></span>
                        </div>
                        <div class="onecolumn">
                            <asp:Label ID="lblformdescription" runat="server" Text="" AssociatedControlID="txtformdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtformdescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblFormBuiltIn" AssociatedControlID="chkFormBuiltIn" Text="System form"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkFormBuiltIn" CssClass="fillspan" Enabled="False" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image29" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('B953804A-1FDC-4A13-A8F3-EAE2E9DC717D', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblshowsubmenu" AssociatedControlID="chkshowsubmenu" Text="Show sub-menu"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkshowsubmenu" CssClass="fillspan" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image2" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('a01325ef-3309-47a6-88e4-7830c0502dd8', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblshowbreadcrumbs" AssociatedControlID="chkshowbreadcrumbs" Text="Show breadcrumbs"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkshowbreadcrumbs" CssClass="fillspan" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('735a3565-652d-4342-be99-3cce479e1613', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblHideTorch" AssociatedControlID="chkHideTorch" Text="Hide Torch"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkHideTorch" CssClass="fillspan" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgHideTorch" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('335D2F7C-2FC1-444B-AD4F-CABA752A6FB6', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblHideAttachments" AssociatedControlID="chkHideAttachments" Text="Hide attachments"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkHideAttachments" CssClass="fillspan" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgHideAttachments" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('D0198AF3-2D94-41AE-913A-54EC2ECAE185', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblHideAudiences" AssociatedControlID="chkHideAudiences" Text="Hide audiences"></asp:Label><span class="inputs"><asp:CheckBox runat="server" ID="chkHideAudiences" CssClass="fillspan" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgHideAudiences" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('D88B2B35-6B3A-4446-8BA6-6B72FCA43255', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                        </div>
                        <div class="sectiontitle">Form Buttons</div>
                        <div class="onecolumnpanel">
                            The following combination of five different buttons can be placed on the form. Although not mandatory to have all five, it is mandatory to have at least one.
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblsavebuttontext" AssociatedControlID="txtsavebuttontext" Text="Text for 'save' button*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtsavebuttontext" CssClass="fillspan" MaxLength="20"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image16" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('1355a41f-12f6-4897-8e4c-9c4f919f8d59', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvTextSave" runat="server" ErrorMessage="Please enter text for at least one of the Form buttons." ClientValidationFunction="ValidateButtonText" Display="Dynamic" Text="*" ControlToValidate="txtsavebuttontext" ValidationGroup="vgFormEdit" ValidateEmptyText="true"></asp:CustomValidator></span><asp:Label runat="server" ID="lblsaveandduplicatebuttontext" AssociatedControlID="txtsaveandduplicatebuttontext" Text="Text for 'save and duplicate' button*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtsaveandduplicatebuttontext" CssClass="fillspan" MaxLength="20"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image17" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('67d09f8f-b4a4-4644-acda-f5c008fd1350', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvTextsaveandduplicate" runat="server" Display="Dynamic" Text="*" ValidationGroup="vgFormEdit" ControlToValidate="txtsaveandduplicatebuttontext" ErrorMessage="" ClientValidationFunction="ValidateButtonText" ValidateEmptyText="true"></asp:CustomValidator></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblsaveandstaybuttontext" AssociatedControlID="txtsaveandstaybuttontext" Text="Text for 'save and stay' button*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtsaveandstaybuttontext" CssClass="fillspan" MaxLength="20"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image18" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('f1c826d6-2cff-4c3a-af73-640faada64d2', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvTextSaveAndStay" runat="server" ControlToValidate="txtsaveandstaybuttontext" Display="Dynamic" Text="*" ValidationGroup="vgFormEdit" ErrorMessage="" ClientValidationFunction="ValidateButtonText" ValidateEmptyText="true"></asp:CustomValidator></span><asp:Label runat="server" ID="lblcancelbuttontext" AssociatedControlID="txtcancelbuttontext" Text="Text for 'cancel' button*" CssClass="mandatory="></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtcancelbuttontext" CssClass="fillspan" MaxLength="20"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image19" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('877cafe8-e459-44ff-b584-2c738b63c2a7', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvTextCancel" runat="server" ErrorMessage="" ClientValidationFunction="ValidateButtonText" ControlToValidate="txtcancelbuttontext" Display="Dynamic" Text="*" ValidationGroup="vgFormEdit" ValidateEmptyText="true"></asp:CustomValidator></span>
                        </div>
                        <div class="twocolumn">
                            <asp:Label runat="server" ID="lblsaveandnewbuttontext" AssociatedControlID="txtsaveandnewbuttontext" Text="Text for 'save and new' button*" CssClass="mandatory"></asp:Label><span class="inputs"><asp:TextBox runat="server" ID="txtsaveandnewbuttontext" CssClass="fillspan" MaxLength="20"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image3" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('D5232E7B-D617-4807-841D-715F9651BE83', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CustomValidator ID="cvTextSaveAndNew" runat="server" ControlToValidate="txtsaveandnewbuttontext" Display="Dynamic" Text="*" ValidationGroup="vgFormEdit" ErrorMessage="" ClientValidationFunction="ValidateButtonText" ValidateEmptyText="true"></asp:CustomValidator></span>
                        </div>
                    </div>        
                </ContentTemplate>
            </cc1:TabPanel>

            <cc1:TabPanel ID="tabFormDes" runat="server">
                <HeaderTemplate>
                Form Design
                </HeaderTemplate>
                <ContentTemplate>            
                    <div id="dialog" title="Available Fields">
                        <div id="spacerArea"></div>
                        <div id="availableFields"></div>
                    </div>
                    <div id="formdesigntitle" class="sectiontitle">
                        Form Design
                    </div>
                    <div id="formDesignContents" class="modalcontents">
                        <asp:Panel runat="server" ID="tabContainer" CssClass="sm_tabcontainer">
                            <span id="tabBar"></span>
                            <span id="addTabHolder"></span>
                        </asp:Panel>
                        <div id="formMsgs"></div>
                        <div id="formTabs">
                            <div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>
        <div class="formbuttons" style="margin-top:20px;">
            <helpers:CSSButton id="btnSaveForm" runat="server" text="save" onclientclick="SEL.CustomEntityAdministration.Forms.SaveForm();return false;" UseSubmitBehavior="False"/>
            <helpers:CSSButton id="btnCancelForm" runat="server" text="cancel" onclientclick="SEL.CustomEntityAdministration.Forms.HideFormModal();return false;" UseSubmitBehavior="False"/>            
        </div>        
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modform" runat="server" TargetControlID="lnkform" PopupControlID="pnlform" BackgroundCssClass="modalBackground" CancelControlID="btnCancelForm"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkform" runat="server" Style="display: none;">LinkButton</asp:LinkButton>

    <asp:Panel ID="pnlview" runat="server" CssClass="modalpopup formpanel" style="display: none; width: 900px; padding:20px;">
        <div id="divViewHeader" class="sectiontitle">GreenLight Views</div>
        <asp:Image runat="server" ID="imgViewColumnHelp" ClientIDMode="Static" ImageUrl="/icons/24/plain/information.png" Style="display: none; position: absolute;" AlternateText="Show Column Editor Information" />
        <asp:Image runat="server" ID="imgViewFilterHelp" ClientIDMode="Static" ImageUrl="/icons/24/plain/information.png" Style="display: none; position: absolute;" AlternateText="Show Filter Editor Information" />
        <cc1:TabContainer ID="tabConViews" runat="server" OnClientActiveTabChanged="SEL.CustomEntityAdministration.Views.Tabs.TabChange">
            <cc1:TabPanel ID="tabViewGeneralDetails" runat="server">
                <HeaderTemplate>General Details</HeaderTemplate>
                <ContentTemplate>
            <div class="sectiontitle">
                        General Details
                    </div>
            <div class="modalcontentssmall">
            <div class="twocolumn">
                        <asp:Label runat="server" ID="lblViewName" AssociatedControlID="txtviewname" CssClass="mandatory">View name*</asp:Label><span class="inputs"><asp:TextBox ID="txtviewname" runat="server" MaxLength="100"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image28" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('294f7f53-90af-485a-8dea-28e1991c1386', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator Text="*" ID="reqviewname" runat="server" ErrorMessage="Please enter a View name." ControlToValidate="txtviewname" ValidationGroup="vgView" Display="Dynamic"></asp:RequiredFieldValidator></span>
                    </div>
                    <div class="onecolumn">
                        <asp:Label runat="server" ID="lblViewDescription" AssociatedControlID="txtviewdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtviewdescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label ID="lblViewBuiltIn" runat="server" Text="System view" AssociatedControlID="chkViewBuiltIn"></asp:Label><span class="inputs"><asp:CheckBox ID="chkViewBuiltIn" runat="server" Enabled="False" /></span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield"><asp:Image ID="Image31" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('8E3E4450-CFF8-4B97-B7AA-D0D86241138E', 'sm', this);" /></span><span class="inputvalidatorfield">&nbsp;</span>            
                    </div>
                    <div class="sectiontitle">Menu Display Options</div>
                    <div class="onecolumnsmall"><label>Menu</label><span class="inputs"><div id="divMenu" class="divMenu"><span></span><a href="#" onclick="SEL.CustomEntityAdministration.Views.General.CustomMenuStructure.ClearMenu();" class="clearMenu">Clear Menu</a></div></span><span class="inputicon"><asp:Image runat="server" ImageUrl="/shared/images/icons/16/Plain/edit.png" onclick="SEL.CustomEntityAdministration.Views.General.CustomMenuStructure.Show();" data-isadd="true" /></span><span class="inputtooltipfield"><asp:Image ID="Image20" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('3c9987a0-5d1d-4da8-8b75-06a854e4e0e1', 'sm', this);" /></span><span class="inputvalidatorfield"></span></div>
                    <div class="onecolumn"><asp:Label runat="server" ID="lblViewMenuDescription" AssociatedControlID="txtViewMenuDescription"><p class="labeldescription">Menu description</p></asp:Label><span class="inputs"><asp:TextBox ID="txtViewMenuDescription" runat="server" TextMode="MultiLine" textareamaxlength="4000"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgViewMenuDescriptionToolTip" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('a41b60df-a981-4287-b612-e419e10a30a3', 'sm', this);" /></span><span class="inputvalidatorfield"></span></div>
                    <div class="twocolumn"><asp:Label runat="server" ID="lblViewRecordCount" AssociatedControlID="chkViewRecordCount" Text="Show record count"></asp:Label><span class="inputs"><asp:CheckBox ID="chkViewRecordCount" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image34" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4D256449-D07F-43B6-BA69-BBA0514A9F6A', 'sm', this);" /></span></div>

                    <asp:Repeater runat="server" ID="MenuDisabledModulesRepeater">
                    <HeaderTemplate><div class="twocolumn"></HeaderTemplate>
                    <ItemTemplate>
                        <%# (Container.ItemIndex != 0 && Container.ItemIndex % 2 == 0) ? "</div><div class='twocolumn'>" : string.Empty %>
                        <label for="chkMenuDisabledModule<%#Eval("ModuleID")%>">Show in <%#Eval("BrandNamePlainText")%></label><span class="inputs"><input type="checkbox" name="chkMenuDisabledModule" checked="checked" id="chkMenuDisabledModule<%#Eval("ModuleID")%>" value="<%#Eval("ModuleID")%>" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('B7261471-2105-4874-ACD0-F15A6D095471', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                    <div class="sectiontitle">Options</div>
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblViewAddForm" AssociatedControlID="cmbviewaddform" Text="Default add form"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbviewaddform" runat="server"></asp:DropDownList></span><span class="inputicon"><asp:Image ID="imgViewAddFormMappings" runat="server" ImageUrl="/static/icons/16/new-icons/branch.png" onclick="SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.Show(this);" data-isadd="true" /></span><span class="inputtooltipfield"><asp:Image ID="Image21" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('6421f756-ed7e-437f-8cd3-2017e6695500', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblViewEditForm" AssociatedControlID="cmbvieweditform" Text="Default edit form"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbvieweditform" runat="server"></asp:DropDownList></span><span class="inputicon"><asp:Image ID="imgViewEditFormMappings" runat="server" ImageUrl="/static/icons/16/new-icons/branch.png" onclick="SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.Show(this);" data-isadd="false" /></span><span class="inputtooltipfield"><asp:Image ID="Image22" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('1b08aa1f-f29e-4746-ac53-67cea52ce238', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                    </div>
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblViewAllowDelete" AssociatedControlID="chkviewallowdelete" Text="Allow delete"></asp:Label><span class="inputs"><asp:CheckBox ID="chkviewallowdelete" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image23" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('e47cbfbb-5a4b-483a-afef-4d4dccb4ffd4', 'sm', this);" /></span><span class="inputvalidatorfield"></span><asp:Label runat="server" ID="lblViewAllowApproval" AssociatedControlID="chkviewallowapproval" Text="Allow approval"></asp:Label><span class="inputs"><asp:CheckBox ID="chkviewallowapproval" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image24" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('6fc1128b-c1d2-4322-b14a-e96b0da2ec1b', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                    </div>
                  <div class="twocolumn">
                        <asp:Label runat="server" ID="lblViewAllowArchive" AssociatedControlID="chkviewallowarchive" Text="Allow archive"></asp:Label><span class="inputs"><asp:CheckBox ID="chkviewallowarchive" runat="server" /></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="Image32" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('5902FFBB-9D81-4487-AF34-89509A9237E9', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                    </div>
                </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabViewFields" runat="server">
                <HeaderTemplate>Columns</HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">Column Layout</div>
                    <div class="modalcontentssmall">
                        <div class="onecolumnpanel">
                            Use the editor below to choose the columns for this summary.                        
                        </div>

                        <helpers:TreeCombo ID="tcFields" runat="server" ComboType="TreeAndColumns" ShowButtonMenu="true" Width="882" Height="310" LeftPanelWidth="450" LeftTitle="Available Fields" RightTitle="Selected Fields" WebServicePath="~/shared/webservices/svcCustomEntities.asmx" FilterValidationGroup="vgFilter" />

                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabViewFilters" runat="server">
                <HeaderTemplate>Filters</HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">Filters</div>
                    <div class="modalcontentssmall">
                        <div class="onecolumnpanel">Use this editor to create filters that will restrict the number of records returned for the view.</div>
                            
                        <helpers:TreeCombo ID="tcFilters" runat="server" ComboType="TreeAndFilters" ShowButtonMenu="true" AllowDuplicatesInDrop="True" Width="882" Height="310" LeftPanelWidth="270" LeftTitle="Available Fields" RightTitle="Selected Filters" WebServicePath="~/shared/webservices/svcCustomEntities.asmx" ThemesPath="/static/js/jstree/themes/" FilterValidationGroup="vgViewFilter" RenderFilterModal="True" />

                        <script type="text/javascript">
                            //<![CDATA[
                            $(document).ready(function () {
                                var dom = SEL.CustomEntityAdministration.DomIDs.Views.Modal.Fields,
                                    tc = dom.TreeContainer,
                                    cd = dom.Drop,
                                    st = SEL.CustomEntityAdministration.Views.Sort.Tab;
                                $('#' + tc + ' .treemenuleft .btn').click(function (e) { st.Refresh(); });
                                $('#' + tc + ' .treemenu .btn').click(function (e) { st.Refresh(); });
                                $('#' + cd).bind("move_node.jstree", function (e, d) { st.Refresh(); });
                                $('#' + cd).bind("delete_node.jstree", function (e, d) { st.Refresh(); });
                            });
                            //]]>
                        </script>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabViewIcon" runat="server">
                <HeaderTemplate>Icon</HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">Icon</div>
                    <div class="modalcontentssmall">
                        <div class="onecolumnpanel">Use this editor to pick an icon for the view. This is what will be displayed on the menu.</div>
                        <div class="twocolumn">
                        <div id="iconSearchArea">
                            <div id="iconResultsHeader">
                                <span id="selectedIconSpan"></span>
                                <span id="selectedIconInfo"><span style="font-weight: bold">Selected icon</span><span id="selectedIconName">window_dialog.png</span></span>
                                    <span id="iconSearchBox"><span id="iconSearchRemoveButton" class="ui-icon-close searchButton" title="Clear search options"></span><span id="iconSearchButton" class="ui-icon-search searchButton" title="Search"></span>
                                        <asp:TextBox ID="txtViewCustomIconSearch" runat="server" MaxLength="21" CssClass="searchBox"></asp:TextBox></span>
                            </div>
                            <div id="viewCustomIconContainer">
                                <span id="iconResultsLeft">&lt</span>
                                <span id="viewIconResults"></span>
                                <span id="iconResultsRight">&gt</span>
                            </div>                            
                            <div id="selectedIconContainer"></div>
                        </div>
                        </div>
                    </div>
                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabViewSort" runat="server">
                <HeaderTemplate>Sorting</HeaderTemplate>
                <ContentTemplate>
                    <div class="sectiontitle">Sorting</div>
                    <div class="modalcontentssmall">
                    <div class="onecolumnsmall">
                        <asp:Label runat="server" ID="lblSortColumn" AssociatedControlID="ddlSortColumn" Text="Sort column"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlSortColumn" runat="server" onchange="SEL.CustomEntityAdministration.Views.Sort.ChangeColumnSortOrderState();">
                        </asp:DropDownList>
                            </span><span class="inputicon"></span><span class="inputtooltipfield">
                                <asp:Image ID="Image26" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('e5b734ec-8382-4a20-b98c-9ea671dc7e29', 'sm', this);" /></span><span class="inputvalidatorfield"></span>
                    </div>
                    <div class="twocolumn">
                        <asp:Label runat="server" ID="lblSortOrder" AssociatedControlID="ddlSortOrder" Text="Sort direction"></asp:Label><span class="inputs"><asp:DropDownList ID="ddlSortOrder" runat="server" Enabled="false" onchange="SEL.CustomEntityAdministration.Views.Sort.ChangeDirectionState();">
                            <asp:ListItem Text="[None]" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Ascending" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Descending" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                            </span><span class="inputicon"></span><span class="inputtooltipfield">
                                <asp:Image ID="Image27" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('4949b7e1-22ae-4099-8eda-b27659abdc3f', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:CompareValidator runat="server" ID="cmpSortOrder" ValidationGroup="vgView"
                                ControlToValidate="ddlSortOrder" Type="Integer" Operator="GreaterThan" Enabled="False"
                                ValueToCompare="0" ErrorMessage="Please select a Sort direction.">*</asp:CompareValidator></span>
                    </div>
                    </div>                 
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:tabcontainer>
        <div class="formbuttons" style="margin-top:20px;">
            <helpers:CSSButton id="btnSaveView" runat="server" text="save" onclientclick="SEL.CustomEntityAdministration.Views.Save();return false;" UseSubmitBehavior="False"/>
            <helpers:CSSButton id="btnCloseView" runat="server" text="cancel" onclientclick="SEL.CustomEntityAdministration.Views.Modal.Hide();return false;" UseSubmitBehavior="False"/>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modview" runat="server" TargetControlID="lnkview" PopupControlID="pnlview" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkview" Style="display: none;">Link Button</asp:LinkButton>
    
    <asp:Panel ID="pnlViewFormSelectionMappings" runat="server" CssClass="modalpopup formpanel" Style="display: none;">
        <div id="pnlViewFormSelectionMappingsHeader" runat="server" class="sectiontitle mapping-title">Choose your forms</div>
        <div>
            <table class="cGrid" width="100%">
                <thead>
                    <tr>
                        <th>Form Selection Attribute Value</th>
                        <th>Form</th>
                        <th class="cgridnew-icon">&nbsp;</th>
                    </tr>
                </thead>
                <tbody class="mapping-templates">
                    <tr class="mapping-textrow" style="display: none;">
                        <td class="row1">
                            <input class="mapping-text" type="text" maxlength="4000" /></td>
                        <td class="row1">
                            <select class="mapping-forms"></select></td>
                        <td class="row1 cgridnew-icon">
                            <img class="btn mapping-addbutton" src="/static/icons/16/plain/add.png" alt="add form mapping" onclick="SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.New(this, 'text');" /></td>
                    </tr>
                    <tr class="mapping-listrow" style="display: none;">
                        <td class="row1">
                            <select class="mapping-list"></select></td>
                        <td class="row1">
                            <select class="mapping-forms"></select></td>
                        <td class="row1 cgridnew-icon">
                            <img class="btn mapping-addbutton" src="/static/icons/16/plain/add.png" alt="add form mapping" onclick="SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.New(this, 'list');" /></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="sectiontitle">Form Selection Mappings</div>
        <div id="pnlViewFormSelectionMappingsBody" runat="server">
            <table class="cGrid" width="100%">
                <thead>
                    <tr>
                        <th>Form Selection Attribute Value</th>
                        <th>Form</th>
                        <th>&nbsp;</th>
                    </tr>
                </thead>
                <tbody class="mapping-tbody">
                </tbody>
                <tbody class="mapping-empty">
                    <tr>
                        <td colspan="3" class="row1" style="text-align: center;">There are no form selection mappings to display.</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnSaveFormSelectionMappings" Text="save" OnClientClick="SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.Save();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton runat="server" ID="btnCloseFormSelectionMappings" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Views.General.FormSelectionMappings.Cancel();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    
    <cc1:ModalPopupExtender ID="mdlViewFormSelectionMappings" runat="server" TargetControlID="lnkViewFormSelectionMappings" PopupControlID="pnlViewFormSelectionMappings" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
    <asp:LinkButton runat="server" ID="lnkViewFormSelectionMappings" Style="display: none;">Link Button</asp:LinkButton>
    
    <asp:Panel ID="pnlCustomMenuStructure" runat="server" CssClass="modalpopup formpanel" Style="display: none; padding: 20px;">
        <div runat="server" class="sectiontitle mapping-title">Select the menu</div>      
        <div id="baseTree">
            <div id="menuTree" style="height: 240px;"></div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnSaveCustomMenu" Text="save" OnClientClick="SEL.CustomEntityAdministration.Views.General.CustomMenuStructure.Save();return false;" UseSubmitBehavior="False" />
            <helpers:CSSButton runat="server" ID="btnCloseCustomMenu" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Views.General.CustomMenuStructure.Cancel();return false;" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="menuTreeData"/>

   <cc1:ModalPopupExtender ID="mdlCustomMenuStructure" runat="server" TargetControlID="lnkCustomMenuStructure" PopupControlID="pnlCustomMenuStructure" BackgroundCssClass="modalBackground"></cc1:ModalPopupExtender>
     <asp:LinkButton runat="server" ID="lnkCustomMenuStructure" Style="display: none;">Link Button</asp:LinkButton>


    <!-- Start Form designer modals -->
    <asp:Panel ID="pnltab" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div>
            <div class="sectiontitle" id="lblTabModalTitle">Tab Details</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lbltabheader" runat="server" Text="Tab name*" AssociatedControlID="txttabheader"></asp:Label><span class="inputs"><asp:TextBox ID="txttabheader" runat="server" CssClass="fillspan" MaxLength="100"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqTabHeader" Text="*" ValidationGroup="vgTabHeader" ControlToValidate="txttabheader" ErrorMessage="Please enter a Tab name." Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveTab" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Forms.SaveTabWithModalSave();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelTab" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Forms.HideTabModal();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modtab" runat="server" TargetControlID="lnkaddtab" PopupControlID="pnltab" BackgroundCssClass="modalBackground" CancelControlID="btnCancelTab" DynamicServicePath="" Enabled="True"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkaddtab" runat="server" Style="display: none"></asp:LinkButton>

    <asp:Panel ID="pnlsection" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div class="sectiontitle" id="lblSectionModalTitle">Section Details</div>
        <div class="twocolumn">
            <asp:Label CssClass="mandatory" ID="lblsectionheader" runat="server" Text="Section name*" AssociatedControlID="txtsectionheader"></asp:Label><span class="inputs"><asp:TextBox ID="txtsectionheader" runat="server" CssClass="fillspan" MaxLength="100"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator runat="server" ID="reqSectionHeader" Text="*" ValidationGroup="vgSectionHeader" ControlToValidate="txtsectionheader" ErrorMessage="Please enter a Section name." Display="Dynamic"></asp:RequiredFieldValidator></span>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveSection" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Forms.AddSectionFromFormButton();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
            <helpers:CSSButton ID="btnCancelSection" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Forms.HideSectionModal();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modsection" runat="server" TargetControlID="lnkaddsection" PopupControlID="pnlsection" BackgroundCssClass="modalBackground" CancelControlID="btnCancelSection" DynamicServicePath="" Enabled="True"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkaddsection" runat="server" Style="display: none"></asp:LinkButton>
    
    <asp:Panel ID="pnlfield" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;">
        <div>
            <div class="sectiontitle" id="lblFieldLabelModalTitle">Field label</div>
            <div class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblfieldtext" runat="server" Text="Field label text*" AssociatedControlID="txtfieldlabel"></asp:Label><span class="inputs"><asp:TextBox ID="txtfieldlabel" runat="server" CssClass="fillspan" MaxLength="250"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"><asp:Image ID="imgLabelTooltip" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('b3b5aeca-7823-4969-8a24-6d8f1652c4d7', 'sm', this);" /></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator Text="*" ID="reqfieldlabel" runat="server" ErrorMessage="Please enter text for the Field label." ControlToValidate="txtfieldlabel" ValidationGroup="vgLabel" Display="Dynamic"></asp:RequiredFieldValidator></span>
            </div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveField" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Forms.UpdateFieldLabel(document.getElementById(txtfieldlabelID).value, true);return false;" ButtonSize="Standard" UseSubmitBehavior="False" /><helpers:CSSButton ID="btnCancelField" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Forms.HideFieldLabelModal();return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modfield" runat="server" TargetControlID="lnkaddfieldlabel" PopupControlID="pnlfield" BackgroundCssClass="modalBackground" CancelControlID="btnCancelField" DynamicServicePath="" Enabled="True"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkaddfieldlabel" runat="server" Style="display: none"></asp:LinkButton>
    
    <asp:Panel ID="pnlDefaultText" runat="server" CssClass="modalpanel formpanel formpanelsmall" Style="display: none;" ClientIDMode="Static">
        <div>
            <div class="sectiontitle" id="defaultTextModalTitle">Default text</div>
            <div id="defaultTextFieldContainer" class="twocolumn">
                <asp:Label CssClass="mandatory" ID="lblDefaultText" runat="server" Text="Default text" AssociatedControlID="txtDefaultText"></asp:Label><span class="inputs"><asp:TextBox ID="txtDefaultText" runat="server" CssClass="fillspan" ClientIDMode="Static"></asp:TextBox><asp:TextBox ID="txtDefaultTextLarge" Width="600px" Height="62px" runat="server" ClientIDMode="Static" TextMode="MultiLine" Wrap="True" Rows="4"></asp:TextBox><span id="HtmlEditorContainer" style="display: none"><HTMLEditor:HtmlEditorExtender ClientIDMode="Static" ID="EditorExtender1" runat="server" TargetControlID="txtHTMLEditor" EnableSanitization="False"></HTMLEditor:HtmlEditorExtender>
                    <asp:TextBox runat="server" Height="293px" ClientIDMode="Static" Width="600px" ID="txtHTMLEditor"></asp:TextBox></span></span><span class="inputicon"></span><span class="inputtooltipfield" style="padding-left: 10px"><asp:Image ID="imgDefaultTextTooltip" runat="server" ImageUrl="~/shared/images/icons/16/plain/tooltip.png" AlternateText="" CssClass="tooltipicon" onmouseover="SEL.Tooltip.Show('DAC160CA-B291-489D-A36B-D8D6D4D58128', 'sm', this);" /></span>
            </div>
        </div>
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSaveDefaultText" runat="server" Text="save" OnClientClick="SEL.CustomEntityAdministration.Forms.UpdateDefaultText(); return false;" ButtonSize="Standard" UseSubmitBehavior="False" /><helpers:CSSButton ID="btnCancelDefaultText" runat="server" Text="cancel" OnClientClick="SEL.CustomEntityAdministration.Forms.HideDefaultTextModal(); return false;" ButtonSize="Standard" UseSubmitBehavior="False" />
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modDefaultText" runat="server" TargetControlID="lnkDefaultText" PopupControlID="pnlDefaultText" BackgroundCssClass="modalBackground" CancelControlID="btnCancelDefaultText" DynamicServicePath="" Enabled="True" ClientIDMode="Static"></cc1:ModalPopupExtender>
    <asp:LinkButton ID="lnkDefaultText" runat="server" Style="display: none"></asp:LinkButton>
    <!-- Tree for summary mto display field selection -->
    <asp:Panel ID="pnlSummaryMTOFieldSelect" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 365px;">
        <div>
            <div class="sectiontitle">Select Field To Display</div>
            <div class="modalcontentssmall" style="height: 365px; overflow: auto;">
                <helpers:Tree runat="server" ID="tcMTODisplayField" WebServicePath="~/shared/webservices/svcCustomEntities.asmx" ShowButtonMenu="True" Width="300" Height="365" />
            </div>
            <div class="formbuttons">
                <helpers:CSSButton runat="server" ID="btnSaveMTOFieldSelect" Text="select" ButtonSize="Standard" OnClientClick="SEL.CustomEntityAdministration.Attributes.Summary.selectDisplayField();return false;" />&nbsp;<helpers:CSSButton runat="server" ID="btnCancelMTOFieldSelect" Text="cancel" ButtonSize="Standard" />
            </div>
        </div>
    </asp:Panel>
    <cc1:ModalPopupExtender runat="server" ID="modmtodisplayfield" TargetControlID="lnkmtodisplayfield" PopupControlID="pnlSummaryMTOFieldSelect" BackgroundCssClass="modalBackground" CancelControlID="btnCancelMTOFieldSelect" />
    <asp:LinkButton ID="lnkmtodisplayfield" runat="server" Style="display: none"></asp:LinkButton>
    
    <!-- End Form designer modals -->

    <tooltip:tooltip id="usrTooltip" runat="server"></tooltip:tooltip>
</asp:Content>

