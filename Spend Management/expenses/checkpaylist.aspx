<%@ Page language="c#" Inherits="Spend_Management.checkpaylist" MasterPageFile="~/masters/smTemplate.master" Codebehind="checkpaylist.aspx.cs" EnableViewState="false" EnableSessionState="ReadOnly" %>
<%@ Import Namespace="SpendManagementLibrary" %>


<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="styles" runat="server">
    <style type="text/css">
        .formpanel .formbuttons {
            padding-bottom: 20px;
        }

    </style>

</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
   <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                        <Scripts>
                            <asp:ScriptReference Path="~/shared/javaScript/sel.claims.js?date=20171214" />
                        </Scripts>
                        <Services>
                            <asp:ServiceReference Path="~/expenses/webServices/claims.asmx" />
                        </Services>
                    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle"><asp:Label ID="lblsearch" runat="server" Text="Search for a Claim" meta:resourcekey="lblsearchResource1"></asp:Label></div>
        <div class="twocolumn"><asp:Label ID="lblsurname" runat="server" Text="Employee Surname" meta:resourcekey="lblsurnameResource1" AssociatedControlID="txtsurname"></asp:Label><span class="inputs"><asp:TextBox ID="txtsurname" runat="server" meta:resourcekey="txtsurnameResource1"></asp:TextBox></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span><asp:Label ID="lblfilter" runat="server" Text="Item Filter" meta:resourcekey="lblfilterResource1" AssociatedControlID="cmbfilter"></asp:Label><span class="inputs"><asp:DropDownList ID="cmbfilter" runat="server" meta:resourcekey="cmbfilterResource1"><asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="All Claims"></asp:ListItem><asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Cash Claims"></asp:ListItem><asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Credit Card Claims"></asp:ListItem><asp:ListItem Value="3" Text="Purchase Card Claims"></asp:ListItem></asp:DropDownList></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="formbuttons"><asp:ImageButton ID="cmdsearch" runat="server" ImageUrl="~/buttons/pagebutton_search.gif" OnClientClick="SEL.Claims.CheckAndPay.FilterCheckAndPayGrids();return false;"  meta:resourcekey="cmdsearchResource1" /></div>
        <div class="sectiontitle"><asp:Label ID="lblcurrent" runat="server" Text="Current Claims" meta:resourcekey="lblcurrentResource1"></asp:Label></div>
        <div id="divGridClaims"><asp:Literal ID="litClaimsGrid" runat="server"></asp:Literal></div>
        <div class="sectiontitle"><asp:Label ID="lblunallocated" runat="server" Text="Un-allocated Claims" meta:resourcekey="lblunallocatedResource1"></asp:Label></div>
        <div class="formbuttons"><a href="javaScript:SEL.Claims.CheckAndPay.AllocateToMe();">Allocate To Me</a></div>
        <div id="divGridUnallocated"><asp:Literal ID="litUnallocatedGrid" runat="server"></asp:Literal></div>
        <div class="formbuttons" style="padding:0px;"><asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton></div>
    </div>
    </asp:Content>


<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
     <script type="text/javascript">
         $(document).ready(function () {
             $('body').css('display','inline-block');
         });
    </script>
</asp:Content>

