<%@ Page Language="VB" MasterPageFile="~/FWMenu.master" AutoEventWireup="false"
    Inherits="frameworkWebApp.Framework2006.Home" Title="Framework Home" Codebehind="Home.aspx.vb" %>

<%@ MasterType VirtualPath="~/FWMenu.master" %>

<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<%-- Add content controls here --%>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="RHcontent">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <!--<link rel="stylesheet" type="text/css" media="screen" href="../css/layout.css" />
    <link rel="stylesheet" type="text/css"  media="screen" href="../css/styles.aspx?style=logon" />-->
    <script language="javascript" type="text/javascript">
    var url = 'Home.aspx?action=callback';
    
    function doCKSearch()
    {
        var cntl = document.getElementById('CKSearch');
        if(cntl != null)
        {
            if(cntl.value == '')
            {
                window.alert('Search value for Contract Key cannot be blank');
                return;
            }
            
            var cc_cntl = document.getElementById('CCSearch');
            if(cc_cntl != null)
            {
                cc_cntl.style.visibility = 'hidden';
            }
            
            var dts = 'searchVal=' + cntl.value;
            displayBroadcastMessage(url + '&type=ck', dts);
        }    
    }
    
    function doCDSearch()
    {
        var searchstatus = getStatus();

        var cntl = document.getElementById('CDSearch');
        if(cntl != null)
        {
            if(cntl.value == '')
            {
                window.alert('Search value criteria cannot be blank');
                return;
            }
            
            var incVariations = document.getElementById('includevariations');
            var ivValue;
            
            if(incVariations != null)
            {
                if(incVariations.checked == true)
                {
                    ivValue = 1;
                }
                else
                {
                    ivValue = 0;
                }
            }
            else
            {
                ivValue = '0';
            }
            
            var cc_cntl = document.getElementById('CCSearch');
            if(cc_cntl != null)
            {
                cc_cntl.style.visibility = 'hidden';
            }

            var dts = 'searchVal=' + cntl.value + '&status=' + searchstatus + '&iv=' + ivValue;
            displayBroadcastMessage(url + '&type=cd', dts);
        }
    }
    
    function doCNSearch()
    {
        var searchstatus = getStatus();

        var cntl = document.getElementById('CNSearch');
        if(cntl != null)
        {
            if(cntl.value == '')
            {
                window.alert('Search value for Contract Number cannot be blank');
                return;
            }
            
            var incVariations = document.getElementById('includevariations');
            var ivValue;
            
            if(incVariations != null)
            {
                if(incVariations.checked == true)
                {
                    ivValue = 1;
                }
                else
                {
                    ivValue = 0;
                }
            }
            else
            {
                ivValue = '0';
            }
            
            var cc_cntl = document.getElementById('CCSearch');
            if(cc_cntl != null)
            {
                cc_cntl.style.visibility = 'hidden';
            }
            
            var dts = 'searchVal=' + cntl.value + '&status=' + searchstatus + '&iv=' + ivValue;
            displayBroadcastMessage(url + '&type=cn', dts);
        }
    }
    
    function doCCSearch()
    {
        var searchstatus = getStatus();

        var cntl = document.getElementById('CCSearch');
        if(cntl != null)
        {
            if(cntl.value == '0')
            {
                window.alert('Search value selection cannot be blank');
                return;
            }

            var incVariations = document.getElementById('includevariations');
            var ivValue;

            if (incVariations != null) {
                if (incVariations.checked == true) {
                    ivValue = 1;
                }
                else {
                    ivValue = 0;
                }
            }
            else {
                ivValue = '0';
            }

            var dts = 'searchVal=' + cntl.value + '&status=' + searchstatus + '&iv=' + ivValue;
            
            cntl.style.visibility = 'hidden';
            
            displayBroadcastMessage(url + '&type=cc', dts);
        }
    }
    
    
    function doVNSearch()
    {
        var cntl = document.getElementById('VNSearch');
        if(cntl != null)
        {
            if(cntl.value == '')
            {
                window.alert('Search value criteria cannot be blank');
                return;
            }

            var cc_cntl = document.getElementById('CCSearch');
            if(cc_cntl != null)
            {
                cc_cntl.style.visibility = 'hidden';
            }

            var dts = 'searchVal=' + cntl.value;
            displayBroadcastMessage(url + '&type=vn', dts);
        }
    }
    
    function doPNSearch()
    {
        var searchstatus = getStatus();
        var cntl = document.getElementById('PNSearch');
        
        if(cntl != null)
        {
            if(cntl.value == '')
            {
                window.alert('Search value for Product cannot be blank');
                return;
            }
            
            var incVariations = document.getElementById('includevariations');
            var ivValue;
            
            if(incVariations != null)
            {
                if(incVariations.checked == true)
                {
                    ivValue = 1;
                }
                else
                {
                    ivValue = 0;
                }
            }
            else
            {
                ivValue = '0';
            }
            
            var cc_cntl = document.getElementById('CCSearch');
            if(cc_cntl != null)
            {
                cc_cntl.style.visibility = 'hidden';
            }
            
            var dts = 'searchVal=' + cntl.value + '&status=' + searchstatus + '&iv=' + ivValue;
            displayBroadcastMessage(url + '&type=pn', dts);
        }
    }
    
    function getStatus()
    {
        var retVal='N';
        var x;
        var cntl = document.getElementsByName('rdoStatus');

        for(x=0; x<3; x++)
        {
            if(cntl[x].checked == true)
            {
                switch(x)
                {
                case 0:
                    retVal = 'N';
                    break;
                case 1:
                    retVal = 'Y';
                    break;
                case 2:
                    retVal = 'B';
                    break;
                default:
                    retVal = 'N';
                }
            }
        }

        return retVal;
    }

    function toggle(divId)
    {
        var imgId = 'img' + divId;
        if(document.getElementById(divId).style.display != 'block')
        {
            document.getElementById(divId).style.display = 'block';
            document.getElementById(imgId).src = './buttons/close.gif';
        }
        else
        {
            document.getElementById(divId).style.display = 'none';
            document.getElementById(imgId).src = './buttons/open.gif';
        }
    }
    $(window).bind("load", function () {
        var afterLoadWidth = $(window).width();
        if (afterLoadWidth <= 968) {
            $('#righthandcontent').css('width', $('.well').width() - 33 + 'px');
            $('.main-content-area').children().first().css('margin-top', $('.homeSearchPanel').height() + 70 + 'px');
        }
    });

    $(document).ready(function () {
        var width = $(window).width();
        $(".col-md-6").css("width", "50%");
        if (width > 1366) {
            $('.homeSearchPanel').css("width", "104%");
        }
        if (width <= 968) {
            loadCSS("mediaQueryMobileStylesheet",
            "/static/styles/expense/css/mobileMediaQuery.css");
            $('.homeSearchPanel').css('height', 'initial');
        }
       
        if (width <= 480) {
           $('.homeSearchPanel').css('overflow-x', 'scroll');
        }

        $(window).resize(function () {
            clearTimeout(window.homePageResizedFinished);
            window.homePageResizedFinished = setTimeout(function () {
                var resizeWidth = $(window).width();
                if (resizeWidth <= 968) {
                    
                    $('.homeSearchPanel').css('height', 'auto');
                    $('#righthandcontent').css('width', $('.well').width() - 33 + 'px');
                    $('.main-content-area').children().first().css('margin-top', $('.homeSearchPanel').height() + 70 + 'px');
                   
                }
                else {
                    $('.main-content-area').children().first().removeAttr('style');
                    $('#righthandcontent').removeAttr('style');
                    $('.homeSearchPanel').removeAttr('style');
                }
                if (resizeWidth <= 480) {
                    $('.homeSearchPanel').css('overflow-x', 'scroll');
                }
                else {
                    $('.homeSearchPanel').css('overflow-x', 'auto');
                }
                if (!$('#wrapper').hasClass("toggled")) {
                    $('.r_nav').click();
                }
            }, 200);
        });
        
    });

    </script>

    <asp:Panel runat="server" ID="SearchFieldsPanel" CssClass="homeSearchPanel">
    <div class="inputpanel" align="center"><asp:Label runat="server" ID="lblErrorStatus" ForeColor="red"></asp:Label></div>
        <div class="inputpanel">
              <tooltip:tooltip ID="usrTooltip" runat="server"></tooltip:tooltip>
            <div class="inputpaneltitle">Quick Reference Search</div>
            <table>
                <tr><td class="labeltd">Contract Key</td><td class="inputtd"><asp:Literal ID="litCKSearch" runat="server"></asp:Literal></td><td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip430" onclick="SEL.Tooltip.Show('fa9ad3cc-fff9-42fb-a42c-76684bd6b649', 'fw', this);" class="tooltipicon" /></td></tr>
                <tr><td class="labeltd">Contract Number</td><td class="inputtd"><asp:Literal runat="server" ID="litCNSearch"></asp:Literal></td><td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip431" onclick="SEL.Tooltip.Show('6e739b4b-9f95-4223-8908-326012e5b65a', 'fw', this);" class="tooltipicon" /></td></tr>
                <tr><td class="labeltd"><asp:Label runat="server" ID="lblContractDesc">Contract Description</asp:Label></td><td class="inputtd"><asp:Literal ID="litCDSearch" runat="server"></asp:Literal></td><td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip432" onclick="SEL.Tooltip.Show('d9f067da-5a03-41da-ba54-27aeb4faeee0', 'fw', this);" class="tooltipicon" /></td></tr>
                <tr>
                    <td class="labeltd">
                        Product
                    </td>
                    <td class="inputtd">
                        <asp:Literal ID="litPNSearch" runat="server"></asp:Literal>
                    </td>
                    <td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip433" onclick="SEL.Tooltip.Show('ddf298a2-c9db-4dbb-b62a-5972a73c27ce', 'fw', this);" class="tooltipicon" /></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label runat="server" ID="lblSupplierName">Supplier Name</asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:Literal ID="litVNSearch" runat="server"></asp:Literal>
                    </td>
                    <td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip434" onclick="SEL.Tooltip.Show('49bef8c6-9908-4f6e-aad5-6296b9c03413', 'fw', this);" class="tooltipicon" /></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        <asp:Label runat="server" ID="lblCCSearch"></asp:Label>
                    </td>
                    <td class="inputtd">
                        <asp:Literal runat="server" ID="litCCSearch"></asp:Literal>
                    </td>
                    <td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip435" onclick="SEL.Tooltip.Show('6bee20d8-3fa3-49f8-817c-300610d75568', 'fw', this);" class="tooltipicon" /></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Contract Status
                    </td>
                    <td class="inputtd">
                        <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                    </td>
                    <td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip436" onclick="SEL.Tooltip.Show('d38c7660-df25-4afe-b6ef-6836d89213a7', 'fw', this);" class="tooltipicon" /></td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Include Contract Variations in Results
                    </td>
                    <td class="inputtd">
                        <asp:Literal ID="litChkIncludeVariations" runat="server"></asp:Literal>
                    </td>
                    <td><img src="./icons/16/plain/tooltip.png" alt="" id="imgtooltip437" onclick="SEL.Tooltip.Show('f6ecc40e-e7b7-4a11-9010-89b4d6a892eb', 'fw', this);" class="tooltipicon" /></td>
                </tr>
            </table>
        </div>
        <div class="inputpanel">
            <div class="inputpaneltitle" style="margin-top:15px;">
                Recent Access Links
            </div>
            <table>
                <tr>
                    <td class="labeltd">
                        Last Contract
                    </td>
                    <td class="inputtd">
                        <asp:HyperLink ID="hypLastContract" runat="server" Style="width: 200px;">last contract desc</asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        Last Report
                    </td>
                    <td class="inputtd">
                        <asp:HyperLink ID="hypLastReport" runat="server" Style="width: 200px;">last report ran</asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td class="labeltd">
                        My Tasks
                    </td>
                    <td class="inputtd">
                        <asp:HyperLink runat="server" ID="hypTasks" NavigateUrl="~/shared/tasks/MyTasks.aspx">tasks</asp:HyperLink>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="AccountInfoPanel">
        <div class="inputpanel">
            <div class="inputpaneltitle">Account Information</div>
            <div class="comment" style="width: 94%; margin-top: 5px;">
                <asp:Literal runat="server" ID="litAccountInfo"></asp:Literal>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
