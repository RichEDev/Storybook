<%@ Page MasterPageFile="~/FWMaster.master" Language="vb" AutoEventWireup="true"
    Inherits="frameworkWebApp.Framework2006.ProductDetails" Codebehind="ProductDetails.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMaster.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
      <style type="text/css">
     .formpanel .onecolumn textarea 
        {
	        width: 562px;	
        }
    </style> <asp:ScriptManagerProxy ID="ScriptManagerProxy2" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/callback.js" />
            <asp:ScriptReference Path="~/javascript/userdefined.js?date=20180417" />             
        </Scripts>
    </asp:ScriptManagerProxy>
    <script language="javascript" type="text/javascript">
        var searchwnd;
        var UF_FieldName;
        var UF_Value;

        function getTextEntry() {
            var UF_Field;
            var src_Field;
            //Form1.
            src_Field = searchwnd.document.getElementById(UF_FieldName); //eval('searchwnd.' + UF_FieldName);
            // 'Form1.' + 
            UF_Field = document.getElementById(UF_FieldName); //eval(UF_FieldName);
            UF_Field.value = src_Field.value;
            searchwnd.close();
        }

        function getSearchResult() {
            var UF_Field_txt = document.getElementById(UF_FieldName + '_TXT');
            if (UF_Field_txt != null) {
                UF_Field_txt.value = searchwnd.document.getElementById('searchResultTxt').value;
            }

            var UF_Field_val = document.getElementById(UF_FieldName);
            if (UF_Field_val != null) {
                UF_Field_val.value = searchwnd.document.getElementById('searchResultId').value;
            }
            searchwnd.close();
        }

        function doSearch(i, UF_name) {
            window.name = 'main';
            UF_FieldName = UF_name;
            searchwnd = window.open('UFSearch.aspx?searchtype=' + i + '&ufid=' + UF_name, 'search', 'width=600, height=600, scrollbars=yes');
        }

        $(document).ready(function() {
            $('.formpanel .twocolumn .inputtooltipfield').addClass('reduceHiddenElementWidth');
        });
    </script>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    </asp:ScriptManagerProxy>
    <div class="formpanel formpanel_padding">
        <asp:Label ID="lblErrorString" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <asp:Panel runat="server" ID="PD_SearchPanel">
        <div class="formpanel formpanel_padding">
        <div class="sectiontitle">Product Filter</div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblProductFilter" AssociatedControlID="txtFilter">Product Filter</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtFilter" runat="server" CssClass="fillspan"></asp:TextBox></span> <span class="inputicon">
                        <asp:ImageButton runat="server" ID="cmdOK" ImageUrl="~/icons/16/plain/find.png" CausesValidation="False" /></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblCategoryFilter" runat="server" AssociatedControlID="lstCategoryFilter">Product Category Filter</asp:Label>
                <span class="inputs">
                    <asp:DropDownList ID="lstCategoryFilter" runat="server" AutoPostBack="True" CssClass="fillspan">
                    </asp:DropDownList>
                </span>
            </div>
        </div>
        <div class="formpanel formpanel_padding">
            <asp:Literal runat="server" ID="litSearchResults"></asp:Literal>
            <div class="formbuttons">
                <asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png"
                    CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="PD_EditFieldsPanel" Visible="false">
        <div class="formpanel formpanel_padding">
            <div class="sectiontitle">
                Product Details
                <asp:Label ID="lblTitle" runat="server"></asp:Label></div>
            <div class="twocolumn">
                <asp:Label ID="lblProductName" runat="server" CssClass="mandatory" AssociatedControlID="txtProductName">product name</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtProductName" TabIndex="1" runat="server" MaxLength="50" Wrap="False"
                        CssClass="fillspan"></asp:TextBox></span> <span class="inputicon">&nbsp;</span>
                <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                    ID="reqProductName" runat="server" ControlToValidate="txtProductName" ErrorMessage="This is a required field"
                    SetFocusOnError="True" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
                </span>
                <asp:Label ID="lblProductCode" runat="server" AssociatedControlID="txtProductCode">product code</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtProductCode" TabIndex="2" runat="server" MaxLength="15" CssClass="fillspan"></asp:TextBox></span><span
                        class="inputicon">&nbsp;</span> <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <div class="onecolumn">
                <asp:Label ID="lblProductDescription" runat="server" AssociatedControlID="txtProductDescription">product description</asp:Label><span class="inputs">
                    <asp:TextBox ID="txtProductDescription" TabIndex="3" runat="server" TextMode="MultiLine"
                        CssClass="fillspan" Rows="6"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
                <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblUserCode" runat="server" AssociatedControlID="txtUserCode">user code</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtUserCode" TabIndex="4" runat="server" MaxLength="15" Wrap="False"></asp:TextBox></span><span
                        class="inputicon">&nbsp;</span> <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblProductCategory" runat="server" AssociatedControlID="lstProductCategory">product category</asp:Label>
                <span class="inputs">
                    <asp:DropDownList ID="lstProductCategory" TabIndex="5" runat="server" CssClass="fillspan">
                    </asp:DropDownList>
                </span><span class="inputicon"></span><span class="inputtooltip">&nbsp;</span><span
                    class="inputvalidatorfield"></span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblInstalledVersion" runat="server" AssociatedControlID="txtInstalledVersion">installed version</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtInstalledVersion" TabIndex="6" runat="server" Wrap="False"></asp:TextBox></span><span
                        class="inputicon"></span> <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblDateInstalled" runat="server" AssociatedControlID="txtDateInstalled">date installed</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtDateInstalled" runat="server" TabIndex="7" CssClass="fillspan"></asp:TextBox></span><span
                        class="inputicon"><asp:ImageButton ID="calDateInstalled" runat="server" ImageUrl="~/icons/16/plain/calendar.gif" />
                        <cc1:CalendarExtender ID="calexDateInstalled" runat="server" Format="dd/MM/yyyy"
                            TargetControlID="txtDateInstalled" PopupButtonID="calDateInstalled">
                        </cc1:CalendarExtender>
                    </span><span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"><asp:CompareValidator
                        ID="cmpDateInstalled" runat="server" ControlToValidate="txtDateInstalled" ErrorMessage="Invalid date format entered"
                        Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" Display="Dynamic"
                        Text="*"></asp:CompareValidator>
                    </span>
            </div>
            <div class="twocolumn">
                <asp:Label ID="lblAvailableVersion" runat="server" AssociatedControlID="txtAvailableVersion">available version</asp:Label>
                <span class="inputs"><asp:TextBox ID="txtAvailableVersion" runat="server" TabIndex="8"
                    Wrap="False" CssClass="fillspan"></asp:TextBox></span><span class="inputicon">&nbsp;</span>
                    <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield"></span>
                    <asp:Label ID="lblNotes" runat="server" AssociatedControlID="litNotes">number of notes</asp:Label>
                    <span class="inputs"><asp:Literal ID="litNotes" runat="server"></asp:Literal></span><span class="inputicon"></span>
            </div>
            <div class="twocolumn">
                <asp:Label runat="server" ID="lblPVAssoc" AssociatedControlID="lstProductVendorAssoc"><asp:HyperLink ID="lnkProductVendorAssoc" TabIndex="9" runat="server" NavigateUrl="Associations.aspx?frompage=productdetails"
                    Enabled="False">product-vendor association</asp:HyperLink></asp:Label>
                <span class="inputs">
                    <asp:ListBox ID="lstProductVendorAssoc" runat="server" Rows="2">
                    </asp:ListBox>
                </span><span class="inputicon"></span><span class="inputtooltip">
                    <img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip451" onclick="SEL.Tooltip.Show('a0d48fef-fd13-40be-b9cf-057b181b22dd', 'fw', this);"
                        class="tooltipicon" /></span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
        </div>
        <div class="formpanel formpanel_padding">
            <div class="sectiontitle">
                Licence Information</div>
            <div class="twocolumn">
            <asp:Label runat="server" ID="lblLicences" AssociatedControlID="txtNumLicences">
                <asp:Literal ID="litLicences" runat="server"></asp:Literal></asp:Label>
                <span class="inputs">
                    <asp:TextBox runat="server" ID="txtNumLicences" ReadOnly="true"></asp:TextBox></span><span
                        class="inputicon"></span> <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
                <asp:Label ID="lblNoLicensedCopies" runat="server" AssociatedControlID="txtNumLicensedCopies">number of licensed copies</asp:Label>
                <span class="inputs">
                    <asp:TextBox ID="txtNumLicensedCopies" runat="server" TabIndex="10" ToolTip="Readonly field - updated automatically from Product Licences"
                        Wrap="False" ReadOnly="True" CssClass="fillspan"></asp:TextBox></span><span class="inputicon"></span>
                <span class="inputtooltip">&nbsp;</span><span class="inputvalidatorfield">&nbsp;</span>
            </div>
            <asp:PlaceHolder runat="server" ID="phPUFields"></asp:PlaceHolder>
        </div>
        <div class="formpanel formpanel_padding">
            <asp:ImageButton runat="server" ID="cmdUpdate" ImageUrl="./buttons/update.gif" TabIndex="10" />&nbsp;
            <asp:ImageButton runat="server" ID="cmdCancel" ImageUrl="./buttons/cancel.gif" CausesValidation="false"
                TabIndex="11" />
        </div>
        <script language="javascript" type="text/javascript">
            var rteModal = '<%=mdlTextEditor.ClientID %>';        
        </script>
        <asp:Panel runat="server" Style="display: none;" CssClass="modalpanel" ID="pnlRTEdit">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <span id="lblTextEditorHeading"></span>
                </div>
                <div class="onecolumnlarge">
                    <label id="lblRTE" for="txtRTE">
                    </label>
                    <span class="inputs">
                        <textarea id="txtRTE" style="width: 100%;" rows="14"></textarea></span>
                </div>
                <div class="formbuttons">
                    <asp:ImageButton runat="server" ID="cmdTESave" AlternateText="Save" ImageUrl="~/Buttons/update.gif"
                        OnClientClick="javascript:saveRTEdit();" CausesValidation="false" />&nbsp;&nbsp;<asp:ImageButton
                            runat="server" ID="cmdTECancel" AlternateText="Cancel" ImageUrl="~/Buttons/cancel.gif"
                            CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
        <cc1:ModalPopupExtender runat="server" ID="mdlTextEditor" TargetControlID="lnkLaunchModal"
            BackgroundCssClass="modalBackground" CancelControlID="cmdTECancel" OkControlID="cmdTESave"
            PopupControlID="pnlRTEdit" OnOkScript="saveRTEdit">
        </cc1:ModalPopupExtender>
        <asp:LinkButton runat="server" ID="lnkLaunchModal" Style="display: none;">&nbsp;</asp:LinkButton>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton runat="server" ID="lnkNew" CssClass="submenuitem" CausesValidation="false">New</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkDelete" CssClass="submenuitem" CausesValidation="false"
        Visible="false">Delete</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkNotes" CssClass="submenuitem" CausesValidation="false"
        Visible="False">Notes</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkLicences" CssClass="submenuitem" CausesValidation="false"
        Visible="false">Licences</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkTaskSummary" CssClass="submenuitem" CausesValidation="false"
        Visible="false">Task Summary</asp:LinkButton>
    <asp:LinkButton runat="server" ID="lnkAddTask" CssClass="submenuitem" CausesValidation="false"
        Visible="false">Add Task</asp:LinkButton>
</asp:Content>
