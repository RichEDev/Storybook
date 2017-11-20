<%@ Control Language="C#" AutoEventWireup="true" Inherits="reports_runtimecriteria" Codebehind="runtimecriteria.ascx.cs" %>
    <%@ Register TagPrefix="igtxt" Namespace="Infragistics.WebUI.WebDataInput" Assembly="Infragistics4.WebUI.WebDataInput.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register TagPrefix="igsch" Namespace="Infragistics.WebUI.WebSchedule" Assembly="Infragistics4.WebUI.WebDateChooser.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
    <script type="text/javascript">
        var selectList;
    var curI;
function selectListOption (fieldid, i)
    {
   
   curI = i;
   var selectedItems = new String();
   selectedItems = igedit_getById('ctl00_contentmain_criteria_txtbox' + i + '_values').getValue();
   
   var arritems;
   var queryString = new String();
   queryString = '';
   if (selectedItems != null)
   {
        if (selectedItems != '')
        {
            arritems = selectedItems.split(',');
            for (i = 0; i < arritems.length; i++)
            {
                queryString += 'id=' + arritems[i] + '&';
            }
            queryString = queryString.substring(0,queryString.length-1);
        }
    }
    window.name = 'main';
        selectList = window.open("listselector.aspx?fieldid=" + fieldid + "&" + queryString, null, 'height=500,width=500');
    }
    
function getSelectedListOptions()
{
    var checkedNodes = selectList.getCheckedNodes();
    var txt = new String();
    var ids = new String();
    txt = '';
    ids = '';
    for (i = 0; i < checkedNodes.length; i++)
    
    {
        txt += checkedNodes[i].getText() + ', ';
        ids += checkedNodes[i].getTag() + ',';
    }
    
    if (txt != '')
    {
        txt = txt.substring(0,txt.length-2);
        ids = ids.substring(0,ids.length-1);
        
    }

   var txtbox = igedit_getById('ctl00_contentmain_criteria_txtbox' + curI);
   txtbox.setValue(txt);
   
   txtbox = igedit_getById('ctl00_contentmain_criteria_txtbox' + curI + '_values');
   txtbox.setValue(ids);
   selectList.close();
    
}
    </script>
     <div>
        <asp:ValidationSummary ID="valcriteria" runat="server" ShowMessageBox="True" ShowSummary="False" meta:resourcekey="valcriteriaResource1" />
    </div>
<div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblstatic" runat="server" Text="Static Fields" meta:resourcekey="lblstaticResource1"></asp:Label></div>
    <asp:PlaceHolder ID="holderStatic" runat="server"></asp:PlaceHolder>
</div>

<div class="inputpanel">
    <div class="inputpaneltitle">
        <asp:Label ID="lblruntime" runat="server" Text="Filter Details" meta:resourcekey="lblruntimeResource1"></asp:Label></div>
   
    <asp:PlaceHolder ID="holderCriteria" runat="server"></asp:PlaceHolder>
</div>
