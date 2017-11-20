<%@ Page Language="c#" Inherits="Spend_Management.aeallowance" MasterPageFile="~/masters/smForm.master"
    CodeBehind="aeallowance.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
     .sectiontitle {
         margin-top: 30px;
     }
  </style>
    <!--[if IE 7]>
<style>
         .formbuttons {
         display: inline-block;
     }

         #ieCustomStyle img{
         margin-top: -15px;
     }

    </style>
<![endif]-->
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript" type="text/javascript">
        var txtrate = '<%=txtrate.ClientID %>';
        var txthours = '<%=txthours.ClientID %>';
        var modRate = '<%=modRate.ClientID %>';
        var txtname = '<%=txtname.ClientID %>';
        var txtdescription = '<%=txtdescription.ClientID %>';
        var txtnighthours = '<%=txtnighthours.ClientID %>';
        var txtnightrate = '<%=txtnightrate.ClientID %>';
        var cmbcurrencies = '<%=cmbcurrencies.ClientID %>';
        var ratesGrid = '<%=pnlRates.ClientID %>';
    </script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/allowances.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">
            <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1">General Details</asp:Label></div>
        <div class="twocolumn">
            <asp:Label ID="lblname" runat="server" meta:resourcekey="lblnameResource1" Text="Allowance Name"
                AssociatedControlID="txtname"></asp:Label><span class="inputs"><asp:TextBox CssClass="fillspan"
                    ID="txtname" runat="server" meta:resourcekey="txtnameResource1" MaxLength="50"></asp:TextBox></span><span
                        class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span
                            class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqname" runat="server"
                                ControlToValidate="txtname" ErrorMessage="Please enter a name for this allowance"
                                meta:resourcekey="reqnameResource1" Text="*" ValidationGroup="vgMain"></asp:RequiredFieldValidator></span></div>
        <div class="onecolumnsmall">
            <asp:Label ID="Label4" runat="server" meta:resourcekey="Label4Resource1" AssociatedControlID="cmbcurrencies"
                        Text="Currency"></asp:Label><span class="inputs"><asp:DropDownList CssClass="fillspan"
                            ID="cmbcurrencies" runat="server" meta:resourcekey="cmbcurrenciesResource1">
                        </asp:DropDownList>
                        </span><span class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span
                            class="inputvalidatorfield">&nbsp;</span></div>
        <div class="onecolumn">
            <asp:Label ID="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1"
                Text="" AssociatedControlID="txtdescription"><p class="labeldescription">Description</p></asp:Label><span class="inputs"><asp:TextBox
                    ID="txtdescription" runat="server" MaxLength="4000" TextMode="MultiLine" CssClass="fillspan"></asp:TextBox></span><span
                        class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="sectiontitle">
            <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1">Night Rate Details</asp:Label></div>
        <div class="twocolumn">
            <asp:Label ID="lblnighthours" runat="server" meta:resourcekey="lblnighthoursResource1"
                Text="Number of Hours" AssociatedControlID="txtnighthours"></asp:Label><span class="inputs"><asp:TextBox
                    CssClass="fillspan" ID="txtnighthours" runat="server"></asp:TextBox></span><span
                        class="inputicon">&nbsp;</span><span class="inputtooltipfield"><img id="imgtooltip341"
                            onclick="SEL.Tooltip.Show('0d2373ae-ce6d-4381-84c1-470dede1bd65', 'sm', this);" src="../../shared/images/icons/16/plain/tooltip.png"
                            alt="" class="tooltipicon" /></span><span class="inputvalidatorfield"><asp:CompareValidator
                                ID="compnighthours" runat="server" ErrorMessage="Please enter a valid value for Number of hours"
                                ControlToValidate="txtnighthours" Operator="DataTypeCheck" Type="Integer" ValidationGroup="vgMain"
                                meta:resourcekey="comphoursResource1" Text="*"></asp:CompareValidator></span><asp:Label
                                    ID="lblnightrate" runat="server" meta:resourcekey="lblnightrateResource1" Text="Rate"
                                    AssociatedControlID="txtnightrate"></asp:Label><span class="inputs"><asp:TextBox
                                        CssClass="fillspan" ID="txtnightrate" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span
                                            class="inputtooltipfield"><img id="imgtooltip342" onclick="SEL.Tooltip.Show('3c8eca27-ca0a-4d0c-b3ac-5c8f37acd143', 'sm', this);"
                                                src="../../shared/images/icons/16/plain/tooltip.png" alt="" class="tooltipicon" /></span><span
                                                    class="inputvalidatorfield"><asp:CompareValidator ID="compnightrate" runat="server"
                                                        ErrorMessage="Please enter a valid value for Rate" Operator="DataTypeCheck" Type="Currency"
                                                        ValidationGroup="vgMain" ControlToValidate="txtnightrate" meta:resourcekey="comprateResource1"
                                                        Text="*"></asp:CompareValidator></span></div>
        <div class="sectiontitle">
            <asp:Label ID="Label3" runat="server" meta:resourcekey="Label3Resource1">Allowance Rates</asp:Label></div>
        <a href="javascript:currentAction='addRate';rateID=0;popupRateModal(true);">Add Rate</a>
        <asp:Panel ID="pnlRates" runat="server">
            <asp:Literal ID="litrates" runat="server" meta:resourcekey="litratesResource1"></asp:Literal>
        </asp:Panel>
        <div class="formbuttons"  id="ieCustomStyle">
            <a href="javascript:currentAction='OK',saveAllowance();">
                <img src="../../shared/images/buttons/btn_save.png"  /></a>&nbsp;&nbsp;<a href="adminallowances.aspx"><img
                    border="0" src="../../shared/images/buttons/cancel_up.gif"></a></div>
    </div>
    <asp:Panel ID="pnlRate" runat="server" CssClass="modalpanel" Style="display: none;">
        <div class="formpanel">
            <div class="sectiontitle">
                <asp:Label ID="Label5" runat="server" Text="General Details"></asp:Label></div>
            <div class="twocolumn">
                <asp:Label ID="Label6" runat="server" Text="Number of hours" AssociatedControlID="txthours"></asp:Label><span
                    class="inputs"><asp:TextBox ID="txthours" runat="server"></asp:TextBox></span><span
                        class="inputicon">&nbsp;</span><span class="inputtooltipfield">&nbsp;</span><span
                            class="inputvalidatorfield"><asp:RequiredFieldValidator ID="reqhours" runat="server"
                                ErrorMessage="Please enter a value for Number of hours in the box provided."
                                Text="*" ControlToValidate="txthours" ValidationGroup="vgRate"></asp:RequiredFieldValidator><asp:CompareValidator
                                    ID="comphours" Type="Integer" runat="server" ErrorMessage="The value entered for Number of hours is invalid."
                                    Text="*" ControlToValidate="txthours" Operator="GreaterThan" ValueToCompare="0"
                                    ValidationGroup="vgRate"></asp:CompareValidator></span><asp:Label ID="Label7" runat="server"
                                        Text="Rate" AssociatedControlID="txtrate"></asp:Label><span class="inputs"><asp:TextBox
                                            ID="txtrate" runat="server"></asp:TextBox></span><span class="inputicon">&nbsp;</span><span
                                                class="inputtooltipfield"></span><span class="inputvalidatorfield"><asp:RequiredFieldValidator
                                                    ID="reqrate" runat="server" ErrorMessage="Please enter a value for Rate in the box provided."
                                                    Text="*" ControlToValidate="txtrate" ValidationGroup="vgRate"></asp:RequiredFieldValidator><asp:CompareValidator
                                                        ID="comprate" runat="server" Type="Currency" ErrorMessage="The value entered for Rate is invalid."
                                                        Text="*" ControlToValidate="txtrate" Operator="GreaterThan" ValueToCompare="0"
                                                        ValidationGroup="vgRate"></asp:CompareValidator></span></div>
            <div class="formbuttons">
                <a href="javascript:saveRate();">
                    <img src="../../shared/images/buttons/btn_save.png" border="0"/></a>&nbsp;&nbsp;<asp:ImageButton
                        ID="cmdratecancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" /></div>
        </div>
    </asp:Panel>
    <asp:LinkButton ID="lnkRate" runat="server" Style="display: none;">LinkButton</asp:LinkButton>
    <cc1:ModalPopupExtender ID="modRate" runat="server" TargetControlID="lnkRate" PopupControlID="pnlRate"
        BackgroundCssClass="modalBackground" CancelControlID="cmdratecancel">
    </cc1:ModalPopupExtender>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".formpanel .sectiontitle:first").css("margin-top", 0);
            $(".formpanel .formbuttons img:first").css("margin-top", 0);
        });
    </script>
    
</asp:Content>