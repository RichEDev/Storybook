<%@ Page Language="vb" AutoEventWireup="false" Inherits="Framework2006.UFSearch"
    CodeFile="UFSearch.aspx.vb" %>

<html>
<head runat="server">
    <title>Redirected Reference Screen</title>

    <script language="javascript" type="text/javascript">
			function populateSearchDetails(searchId, searchText)
			{
			    //alert('searchId = ' + searchId);
			    //alert('searchText = ' + searchText);
			    
			    var cntl_id = document.getElementById('searchResultId');
			    if(cntl_id != null)
			    {
			        cntl_id.value = searchId;
			        //alert('cntl_id.value = ' + cntl_id.value);
			    }
    			
			    var cntl_txt = document.getElementById('searchResultTxt');
                if(cntl_txt != null)
                {
                    cntl_txt.value = searchText;
                    //alert('cntl_txt.value = ' + cntl_txt.value);
                }
			}
			
			function PageSetup()
			{
				var cntl;
				cntl = document.getElementById('txtSearchString');
				if(cntl != null)
				{
					cntl.focus();
				}			
			}
			
			function SetFocus(UF_FieldName)
			{
				cntl = document.getElementById(UF_FieldName);
				if(cntl != null)
				{
					var srcWin;
					var srcCntl;
					srcWin = window.opener;
					// 'srcWin.Form1.' +
					srcCntl = document.getElementById(UF_FieldName);
					
					cntl.value = srcCntl.value;
					
					cntl.focus();					
				}
			}
			
    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
    <asp:Literal runat="server" ID="litStyles"></asp:Literal>
    <div align="center">
        <img alt="Framework Logo" src="./images/fwlogo.jpg" /></div>
    <div align="center">
        <asp:Label ID="lblTitle" runat="server">Search</asp:Label></div>
    <div>
    </div>
    <div align="center">
        <span>
            <asp:Label ID="lblSearchField" runat="server"></asp:Label></span><span>
                <asp:TextBox ID="txtSearchString" runat="server"></asp:TextBox></span><span>
                    <asp:ImageButton ID="cmdFind" runat="server" ImageUrl="./icons/16/plain/find.gif" /></span></div>
    <div>
    </div>
    <asp:Panel runat="server" ID="txtPanel">
    <div align="center">
        <table>
            <tr>
                <td>
                    <asp:TextBox runat="server" ID="txtTextEditor" Rows="25" TextMode="MultiLine" Wrap="true"
                        Width="400"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="panelSearch">
    <div style="overflow: auto; height: 300px" align="center">
        <asp:Literal ID="litSearchResults" runat="server"></asp:Literal>
    </div>
    <div>
    </div></asp:Panel>
    <div align="center">
        <span>
            <asp:Literal runat="server" ID="litSelect"></asp:Literal>
        </span>
    </div>
    <div align="center">
        <span>
            <input id="searchResultId" type="hidden" /></span><span><input type="hidden" id="searchResultTxt" /></span></div>
    <div>
    </div>
    <div>
        <span>
            <asp:Literal ID="litClose" runat="server"></asp:Literal></span></div>
            <div><asp:HiddenField runat="server" ID="hiddenEditorText" /></div>
    </form>
</body>
</html>
