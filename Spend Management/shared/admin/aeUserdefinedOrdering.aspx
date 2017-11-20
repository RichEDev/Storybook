<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="aeUserdefinedOrdering.aspx.cs" Inherits="Spend_Management.aeUserdefinedOrdering" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">

<style type="text/css">

</style>

    <asp:ScriptManagerProxy ID="smproxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/prototype.js" />
            <asp:ScriptReference Path="~/shared/javaScript/scriptaculous.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcUserdefinedOrdering.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">General Details</div>
        <div ID="divComments" runat="server"><asp:Literal ID="litHelpGuide" runat="server"></asp:Literal></div>
        <asp:Panel ID="udfFieldGroupings" runat="server" CssClass="userdefinedGroupings"></asp:Panel>
        <asp:Panel ID="udfNonGroupedFields" runat="server" CssClass="userdefinedGroupings"></asp:Panel>
    </div>

    <div class="formpanel formpanel_left"><div class="formbuttons"><asp:Image ID="cmdsave" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" AlternateText="Save" onclick="SaveOrder()" /><asp:ImageButton ID="cmbcancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" OnClick="cmbcancel_Click" CausesValidation="False" /></div></div>


    <script type="text/javascript" language="javascript">

        var appliesTo = '<% = sAppliesToID %>';

    function SaveOrder() 
    {
        var itemOrder = UserdefinedFieldOrder();
        Spend_Management.svcUserdefinedOrdering.SaveOrder(itemOrder, appliesTo);
        window.location = "userdefinedOrdering.aspx";
    }

    // sUnorderedListNonGroupedID
    
    function UserdefinedFieldOrder() 
    {
        var udfGoupingsContainer = document.getElementById("<% = sUnorderedListID %>");
        var tempArray = [];
        var orderingObject = [];
        var tmpObj;
        var lis;

        /// Grouped userdefined fields
        for (var i = 0; i < udfGoupingsContainer.children.length; i++) 
        {
            tmpObj = { Group: udfGoupingsContainer.childNodes[i].id.replace(contentID, "") };
            for (var j = 0; j < udfGoupingsContainer.childNodes[i].childNodes[j].childNodes.length; j++)
            {
                lis = udfGoupingsContainer.childNodes[i].getElementsByTagName("li");
                tempArray = [];
                for (var k = 0; k < lis.length; k++) {
                    tempArray.push(lis[k].id.replace(contentID, ""));
                }
            }

            tmpObj.UserdefinedFields = tempArray;
            orderingObject.push(tmpObj);
        }
        
        /// Ungrouped userdefined fields
        var udfGoupingsContainer = document.getElementById("<% = sUnorderedListNonGroupedID %>");
        for (var i = 0; i < udfGoupingsContainer.children.length; i++) {
            tmpObj = { Group: udfGoupingsContainer.childNodes[i].id.replace(contentID, "") };
            for (var j = 0; j < udfGoupingsContainer.childNodes[i].childNodes[j].childNodes.length; j++) {
                lis = udfGoupingsContainer.childNodes[i].getElementsByTagName("li");
                tempArray = [];
                for (var k = 0; k < lis.length; k++) {
                    tempArray.push(lis[k].id.replace(contentID, ""));
                }
            }

            tmpObj.UserdefinedFields = tempArray;
            orderingObject.push(tmpObj);
        }



        return orderingObject;
    }
    </script>
</asp:Content>
