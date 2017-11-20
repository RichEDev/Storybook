<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/exptemplate.master" CodeBehind="qeform.aspx.cs" Inherits="expenses.qeform" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<%@ Register Src="~/shared/usercontrols/addressDetailsPopup.ascx" TagName="Popup" TagPrefix="AddressDetails" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAutoComplete.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <script type="text/javascript" language="javascript" src="scripts/addingexpenses.js"></script>
    <script type="text/javascript" language="javascript">
        var index;
        var TodaysDate = '<% = todaysDate %>';
    
    function checkMandatory(sender, args)
    {
    
        var index = sender.id.substring(contentID.length + 6,contentID.length + sender.id.length-6);
        var needToValidate = false;
        var txttotal;
        for (var i = 0; i < arrTotalboxes.length; i++)
        {
            txttotal = document.getElementById(contentID + arrTotalboxes[i] + index);

            if (txttotal != null)
            {
                if (txttotal.value != '')
                {
                    needToValidate = true;
                    break;
                }
            }
        }
        
        if (needToValidate == true)
        {
        
            
            if (args.Value == '')
            {
            
                args.IsValid = false;
            
            }
                
                
                
            
        }
    }


    function popCats(i)
    {
        index = i;
        var subcatid = document.getElementById(contentID + 'txtsubcatid' + index).value;
        var cmbcars = document.getElementById(contentID + 'subcat' + subcatid + 'cars' + index);
        
        if (cmbcars != null)
        {
            carid = cmbcars.options[cmbcars.selectedIndex].value;
        }
        else
        {
            return;
        }

        PageMethods.getMileageCats(carid, accountid, carEmployeeID, popCatsComplete);
        
    }

    function popCatsComplete(data)
    {
        var subcatid = document.getElementById(contentID + 'txtsubcatid' + index).value;
        var cmbmileage = document.getElementById(contentID + 'subcat' + subcatid + 'mileage' + index);
        

        if (cmbmileage == null)
        {
            return;
        }
        else
        {
            cmbmileage.options.length = 0;
           
            for (var j = 0; j < data.length; j++)
            {
                cmbmileage.options[j] = new Option(data[j][0],data[j][1]);
            }
        }
    }

    function errorMessage(data)
    {
        if (data["_message"] != null)
        {
            SEL.MasterPopup.ShowMasterPopup(data["_message"], 'WebService Message');
        }
        else
        {
            SEL.MasterPopup.ShowMasterPopup(data, 'WebService Message');
        }
        return;
    }
    
    </script>
   
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="ValidationSummary1Resource1" />
   
   <asp:MultiView ID="mvQEForm" runat="server">
    <asp:View ID="viewUpload" runat="server">

	<div class="valdiv"><asp:Label id="lblmsg" runat="server" Visible="False" ForeColor="Red" meta:resourcekey="lblmsgResource1">Label</asp:Label></div>
	<div class="inputpanel">
        <table>
        <tr>
        <td class="labeltd" style="width:160px;"><asp:Label ID="lbllocation" runat="server" Text="Spreadsheet Location:" meta:resourcekey="lbllocationResource1"></asp:Label></td>
        <td class="inputtd"><asp:FileUpload ID="qeFileUpload" runat="server" Width="250" /></td>
        </tr>
	    </table>
	</div>
	<div class="inputpanel">
        <asp:ImageButton ID="cmdUpload" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdUpload_Click"  />&nbsp;&nbsp;
        <a href="qelst.aspx"><asp:Image ID="imgCancelQEF" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" /></a>

	</div>
    
        
    </asp:View>
    <asp:View ID="viewForm" runat="server">
            <div class="inputpanel"><table><tr><td><asp:Label id="lblmonth" runat="server" meta:resourcekey="lblmonthResource1">Month:</asp:Label></td><td><asp:DropDownList id="cmbmonth" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbmonth_SelectedIndexChanged" meta:resourcekey="cmbmonthResource1"></asp:DropDownList></td></tr></table></div>
    	    <div class="inputpanel">Please Note: R = Do you have a receipt, VR = Does it include VAT details</div>
            <div class="inputpanel table-border2"><asp:Table ID="tbl" runat="server" CssClass="datatbl" meta:resourcekey="tblResource1"></asp:Table></div>
    	    <div class="inputpanel table-border2"><asp:ImageButton ID="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" OnClick="cmdok_Click" meta:resourcekey="cmdokResource1" />&nbsp;&nbsp;
            <a href="qelst.aspx"><asp:Image ID="Image1" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" AlternateText="Cancel" /></a></div>
            
    	    
    	    
    	    <AddressDetails:Popup ID="addressDetailsPopup" runat="server" />
    </asp:View>
    </asp:MultiView>	    
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:Panel ID="pnlLinks" runat="server">
    <a class="submenuitem" href="qeprintout.aspx?quickentryid=<%=quickentryid%>" target="_blank" >Print</a>
	<asp:LinkButton id="cmdexport" runat="server" cssclass="submenuitem" onclick="cmdexport_Click" CausesValidation="False" meta:resourcekey="cmdexportResource1">Export to Excel</asp:LinkButton>
	</asp:Panel>
</asp:Content>


    

