<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="treeView.ascx.cs" Inherits="Spend_Management.treeView" %>
<asp:ScriptManagerProxy ID="smpTreeView" runat="server">
<Services>
<asp:ServiceReference Path="~/shared/webServices/svcTreeView.asmx" InlineScript="false" />
</Services>
</asp:ScriptManagerProxy>

<asp:Panel ID="pnlTreeView" runat="server" CssClass="formpanel" style="width: 200px">



</asp:Panel>

<script language="javascript" type="text/javascript">
    var TreeViewHolder = document.getElementById('<% = pnlTreeView.ClientID %>');
    var AddSubNode = false;
    var LoadedNodes = new Array();
    var CurrentNode = null;
    var UseOnClickMethod = '<% = OnClickMethod %>';

    function HideChildNodes(tableID) {
        var TreeChildNodes = document.getElementById(tableID).getElementsByTagName("DIV");

        for (var i = 0; i < TreeChildNodes.length; i++) {
            TreeChildNodes[i].style.display = "none";
            //TreeChildNodes[i].parentNode.removeChild(TreeChildNodes[i])
        }
        
        var ParentNode = GetLoadedNode(tableID);
        document.getElementById(tableID).innerHTML = "+ V <a href=\"javascript:GetNodes('" + ParentNode.ViewGroupID + "', true);\" id=\"lnkNodes_" + ParentNode.ViewGroupID + "\">" + ParentNode.GroupName + "</a>";
    }

    function GetLoadedNode(tableID) {
        for (var i = 0; i < LoadedNodes.length; i++) {
            if (LoadedNodes[i].ViewGroupID == tableID) {
                return LoadedNodes[i];
            }
        }
    }

    function CalculationTreeError() {
        AddSubNode = false;
    }

    function GetNodes(tableID, isSubNode) {
        CurrentNode = tableID;
        AddSubNode = isSubNode;
        if (isSubNode == true) {
            var ParentNode = GetLoadedNode(tableID);
            document.getElementById(tableID).innerHTML = "- V <a href=\"javascript:HideChildNodes('" + ParentNode.ViewGroupID + "', true);\" id=\"lnkNodes_" + ParentNode.ViewGroupID + "\">" + ParentNode.GroupName;
        }

            Spend_Management.svcTreeView.GetNodes(tableID, isSubNode, GetNodesCallBack, CalculationTreeError);
     
    }

    function GetNodesCallBack(data) {
        var newDiv;
        var newField;

        if (AddSubNode == false && data.length == 0) {
            alert("No top level nodes found");
        }
        
        for (var i = 0; i < data.length; i++) {
            newDiv = document.createElement("div");

            if (AddSubNode == true) {
                newDiv.style.paddingLeft = "20px";
            }
            newDiv.id = data[i].ViewGroupID

            if (data[i].Children > 0) {
                newDiv.innerHTML = "+ V <a href=\"javascript:GetNodes('" + data[i].ViewGroupID + "', true);\" id=\"lnkNodes_" + data[i].ViewGroupID + "\">" + data[i].GroupName + "</a>";
            }
            
            for (var x = 0; x < data[i].Fields.length; x++) {
                newField = document.createElement("DIV");
                newField.id = "lnkNodeField_" + data[i].Fields[x].FieldID;
                if (UseOnClickMethod != "") {
                    newField.innerHTML = " - F  <a href=\"javascript:<% = OnClickMethod %>('" + data[i].Fields[x].FieldID + "', '" + data[i].Fields[x].Description + "', '" + data[i].Fields[x].TableID + "', '" + data[i].Fields[x].FieldType + "');\">" + data[i].Fields[x].Description + "<a/>";
                } else {
                    newField.innerHTML = " - F " + data[i].Fields[x].Description;
                }
                newDiv.style.paddingLeft = "20px";

                if (CurrentNode != data[i].Fields[x].TableID && CurrentNode != data[i].ParentID) {
                    newField.style.display = "none";
                }
                newDiv.appendChild(newField);
            }


            if (newDiv.hasChildNodes() == true) {
                if (AddSubNode == true) {
                    document.getElementById(data[i].ParentID).appendChild(newDiv);
                } else {
                    TreeViewHolder.appendChild(newDiv);
                }
                LoadedNodes.push(data[i]);
            }
        }
        AddSubNode = false;
        CurrentNode = null;
    }

    GetNodes("618db425-f430-4660-9525-ebab444ed754", false);
</script>