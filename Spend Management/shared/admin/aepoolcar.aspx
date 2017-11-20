<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aepoolcar.aspx.cs" MasterPageFile="~/masters/smPagedForm.master" Inherits="Spend_Management.aepoolcar" %>
<%@ MasterType VirtualPath="~/masters/smPagedForm.master" %>

<%@ Register TagPrefix="aeCars" TagName="aeCar" Src="~/shared/usercontrols/aeCars.ascx" %>

<asp:Content runat="server" ID="customStyles" ContentPlaceHolderID="styles">
        <style type="text/css">
            
             .twocolumn p {
                 font-size: inherit !important;
             }

             .formbuttons {
                 margin-top: 15px;
             }

             #ctl00_contentmain_btnSaveAddUsers, #ctl00_contentmain_btnCancelAddUsers {
                 margin-top: 20px;
             }

            .formpanel .twocolumn label {
                vertical-align: unset;
            }

            #ctl00_contentmain_aeCar_TabContainer1_tabOdomter_chkfuelcard {
                margin-top: 1px;
            }

            #imgtooltip392{
                margin-top:6px;
            }
         </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <a href="javascript:changePage('General');" id="lnkGeneral" class="selectedPage">Pool Vehicle Details</a>
    <a href="javascript:changePage('Employees');" id="lnkEmployees">Pool Vehicle Users</a>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="contentoptions" runat="server">
    <div id="pgOptEmployees" style="display:none;">
        <a href="javascript:AddUserToPoolCar();" title="Add Pool Vehicle User" class="submenuitem">Add Pool Vehicle User</a>
    </div>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/poolCars.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcPoolCars.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        //<![CDATA[
        var mdlAddUserID = '<%= mdlAddUser.ClientID %>';
        var pnlPoolCarUsersGridID = '<% = pnlPoolCarUsersGrid.ClientID %>';
        var nEmployeeid = 0;
        var carID = <% = CarID %>;
        function showCarModal() { return; };
	    //]]>
    </script>
    
    <div id="divPages">
        <div id="pgGeneral" class="primaryPage">
            <div class="formpanel formpanel_padding" >
                <aeCars:aeCar runat="server" ID="aeCar" />
            </div>
        </div>
        
        <div id="pgEmployees" class="subPage" style="display: none;">
            <div class="formpanel formpanel_padding">
                <div class="sectiontitle">
                    <asp:Label id="lblPoolCarUsers" runat="server" meta:resourcekey="lblPoolCarUsersResource1">Pool Vehicle Users</asp:Label>
                </div>
                <asp:Panel ID="pnlPoolCarUsersGrid" runat="server">
                    <asp:Literal ID="litPoolCarUsersGrid" runat="server"></asp:Literal>
                </asp:Panel>
                
                <asp:LinkButton ID="lnkAddUser" runat="server" CausesValidation="False" style="display:none;">Add User</asp:LinkButton>

                <cc1:ModalPopupExtender ID="mdlAddUser" runat="server" TargetControlID="lnkAddUser" PopupControlID="pnlAddUser" BackgroundCssClass="modalBackground" OkControlID="btnSaveAddUsers" CancelControlID="btnCancelAddUsers"></cc1:ModalPopupExtender>
                <asp:Panel ID="pnlAddUser" runat="server" CssClass="formpanel modalpanel" style="display:none;">
                    <div class="sectiontitle"><asp:Label ID="lblAddUser" runat="server" Text="Add User" meta:resourcekey="lblAddUserResource1"></asp:Label></div>
                    <asp:Panel ID="pnlUsersGrid" runat="server">
                        <asp:Literal ID="litUsersGrid" runat="server"></asp:Literal>
                    </asp:Panel>
                    <div class="formbuttons">
                        <asp:Image ID="btnSaveAddUsers" runat="server" onclick="javascript:SaveAddUsers();" ImageUrl="~/shared/images/buttons/btn_save.png" />&nbsp;&nbsp;<asp:Image ID="btnCancelAddUsers" ImageUrl="~/shared/images/buttons/cancel_up.gif" runat="server" onclick="javascript:CancelAddUsers();" />
                    </div>
                </asp:Panel>
                
            </div>
        </div>
        
        <div class="formpanel formpanel_padding">
            <div class="formbuttons">
                <asp:Image ID="btnSave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" onclick="javascript:saveCar();" />&nbsp;<asp:Image ID="btnCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" onclick="javascript:navigateTo(false, true);" />
            </div>
        </div>
    </div>


</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            $("#lnkEmployees").click(function() {
                $('.formbuttons').css('margin-top', '-20px');
            });

            $("#lnkGeneral").click(function() {
                $('.formbuttons').css('margin-top', '10px');
            });

        });
    </script>

</asp:Content>