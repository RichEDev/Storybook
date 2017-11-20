<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_listselector" Codebehind="listselector.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register TagPrefix="igcmbo" Namespace="Infragistics.WebUI.WebCombo" Assembly="Infragistics4.WebUI.WebCombo.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register TagPrefix="ignav" Namespace="Infragistics.WebUI.UltraWebNavigator" Assembly="Infragistics4.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Item Selector</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="litstyles" runat="server" meta:resourcekey="litstylesResource1"></asp:Literal>
    		<div class="inputpanel">
			<div class="inputpaneltitle">
                <asp:Label ID="lblfields" runat="server" Text="Fields" meta:resourcekey="lblfieldsResource1"></asp:Label>
			</div>
                <ignav:UltraWebTree ID="treeitems" runat="server" BorderStyle="Solid" BorderWidth="1px" CheckBoxes="True" DefaultImage="ig_treeOfficeFolder.gif" DefaultSelectedImage="ig_treeOfficeFolder.gif" Height="250px" HiliteClass="" HoverClass="" Indentation="20" meta:resourcekey="treeitemsResource1">
                    <images>
<DefaultImage Url="ig_treeOfficeFolder.gif"></DefaultImage>

<SelectedImage Url="ig_treeOfficeFolder.gif"></SelectedImage>

<CollapseImage Url="ig_treeXPMinus.gif"></CollapseImage>

<ExpandImage Url="ig_treeXPPlus.gif"></ExpandImage>
</images>
                    <NodeStyle>
<Padding Top="2px" Left="2px" Bottom="2px" Right="2px"></Padding>
</NodeStyle>
                    <SelectedNodeStyle BackColor="#316AC5" ForeColor="White">
<Padding Top="2px" Left="2px" Bottom="2px" Right="2px"></Padding>
</SelectedNodeStyle>
                </ignav:UltraWebTree>
		</div>
		<div class="inputpanel">
		<script language="javascript">
		    
		    
		    function getCheckedNodes()
		    {
		        var tree = igtree_getTreeById('treeitems');
		        var nodes = tree.getNodes();
		        var count = 0;
		        
		        var checkedNodes = new Array();
		        for (i = 0; i < nodes.length; i++)
		        {
		            if (nodes[i].getChecked() == true)
		            {
		                checkedNodes[count] = nodes[i];
		                count++;
		            }
		        }
		        
		        return checkedNodes;
		    }
		</script>
		<a href="javascript:getSelectedListOptions()" target="main"><img border=0 src="/shared/images/buttons/btn_save.png" /></a> &nbsp;&nbsp;
			<a href="javascript:window.close();"><img border=0 src="../images/buttons/cancel_up.gif" /></a></div>
    </form>
</body>
</html>
