<%@ Page language="c#" Inherits="expenses.admin.qepage" MasterPageFile="~/expform.master" Codebehind="qepage.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .inputpanel a{
            padding:0px 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script type="text/javascript">
	var fields = new Array();
	var seloption = 0;
	function createTable(pos)
	{
		var i;
		var tbl = new String();
		var rowclass = 'row1';
		var title
		
		
		title = getTitle(pos);
		tbl = '<table class="datatbl">\n';
		tbl += '<tr><th colspan="3">' + title + '</th></tr>\n';
		for (i = 0; i < fields.length; i++)
		{
			if (fields[i][1] == pos)
			{
			    tbl += '<tr><td class="' + rowclass + '"><input type="radio" onclick="selectOption(' + i + ');" value="' + i + '" name="move" id="move' + i + '"><input type="hidden" name="pos" id="pos' + i + '" value="' + pos + '"><input type="hidden" name="fieldid" id="fieldid' + i + '" value="' + fields[i][2] + '"><input type="hidden" name="fieldtext" id="fieldtext' + i + '" value="' + fields[i][3] + '"></td><td class="' + rowclass + '">' + (i + 1) + '</td><td class="' + rowclass + '">' + fields[i][3] + '</td></tr>\n';
				if (rowclass == 'row1')
				{
					rowclass = 'row2';
				}
				else
				{
					rowclass = 'row1';
				}
			}
		}
		tbl += '</table>';
		
		document.getElementById(title).innerHTML = tbl;
	}
	
	function addField(pos)
	{
	    if (document.getElementById('availfields').selectedIndex > 0)
	    {
	        var seloption = document.getElementById('availfields').options[document.getElementById('availfields').selectedIndex];

	        fields[fields.length] = new Array(fields.length, pos, seloption.value, seloption.text);
	        document.getElementById('availfields').options[document.getElementById('availfields').selectedIndex] = null;
	        createTable(pos);
	    }
	    return;
	}
	
	function addText(pos) {
	    if (document.getElementById('txtfree').value != '') {
	        fields[fields.length] = new Array(fields.length, pos, 0, document.getElementById('txtfree').value);
	        document.getElementById('txtfree').value = '';
	        createTable(pos);
	    }
	}
	
	function getTitle(pos)
	{
		switch (pos)
		{
			case 1:
				title = "Top Left";
				break;
			case 2:
				title = "Top Centre";
				break;
			case 3:
				title = "Top Right";
				break;
			case 4:
				title = "Bottom Left";
				break;
			case 5:
				title = "Bottom Centre";
				break;
			case 6:
				title = "Bottom Right";
				break;
		}
		return title;
	}
	
	function selectOption(index)
	{
		seloption = index;
	}
	function moveUp()
	{
		var index = seloption;
		var current, previous;
		var i;
		if (index == 0)
		{
			return;
		}
		
		var pos = fields[index][1];
		
		
		i = index - 1;
		while (fields[i][1] != pos && i != 0)
		{
		
			i--;
		}
		
		if (i == -1)
		{
			return;
		}
		
		current = fields[index];
		
		previous = fields[i];
		fields[index] = previous;
		fields[i] = current;
		seloption = i;
	
		createTable(1);
		createTable(2);
		createTable(3);
		createTable(4);
		createTable(5);
		createTable(6);
		document.getElementById('move' + seloption).checked = true;
	}
	
	function moveDown()
	{
		var index = seloption;
		var current, next;
		if (index == (fields.length-1))
		{
			return;
		}
		var pos = fields[index][1];
		
		i = index + 1;
		while (fields[i][1] != pos && i != (fields.length+1))
		{
		
			i++;
		}
		
		if (i == (fields.length-1))
		{
			return;
		}
		
		current = fields[index];
		next = fields[i];
		fields[index] = next;
		fields[i] = current;
		seloption = i;
		createTable(1);
		createTable(2);
		createTable(3);
		createTable(4);
		createTable(5);
		createTable(6);
		document.getElementById('move' + seloption).checked = true;
	}
	
	function deleteItem()
	{
		var index = seloption;
		var i;
		
		var pos = fields[index][1];
		
		var opt = document.createElement("OPTION");
		opt.text = fields[index][3]
		opt.value = fields[index][2];
		
		if (fields[index][2] != 0)
		{
			document.getElementById('availfields').options.add(opt);
		}
		
		
		fields[index] = null;
		for (i = (index+1); i < fields.length; i++)
		{
			fields[i-1] = fields[i];
		}
		
		fields.pop();
		createTable(1);
		createTable(2);
		createTable(3);
		createTable(4);
		createTable(5);
		createTable(6);
	}
</script>

	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblprintout" runat="server" Text="Select data to add to the print out" meta:resourcekey="lblprintoutResource1"></asp:Label></div>
		<table>
			<tr>
				<td rowspan="6"><asp:literal id="litavailfields" runat="server" meta:resourcekey="litavailfieldsResource1"></asp:literal></td>
				<td><a href="javascript:addField(1)">
                    <asp:Label ID="lbltopleft" runat="server" Text="Add to Top Left" meta:resourcekey="lbltopleftResource1"></asp:Label></a></td>
				<td>Free Text</td>
				<td><a href="javascript:addText(1)">
                    <asp:Label ID="lbltxttopleft" runat="server" Text="Add to Top Left" meta:resourcekey="lbltxttopleftResource1"></asp:Label></a></td>
			</tr>
			<tr>
				<td><a href="javascript:addField(2)">
                    <asp:Label ID="lbltopcentre" runat="server" Text="Add to Top Centre" meta:resourcekey="lbltopcentreResource1"></asp:Label></a></td>
				<td rowspan="6"><textarea id="txtfree" rows="6" cols="20"></textarea></td>
				<td><a href="javascript:addText(2)">
                    <asp:Label ID="lbltxttopcentre" runat="server" Text="Add to Top Centre" meta:resourcekey="lbltxttopcentreResource1"></asp:Label></a></td>
			</tr>
			<tr>
				<td><a href="javascript:addField(3)">
                    <asp:Label ID="lbltopright" runat="server" Text="Add to Top Right" meta:resourcekey="lbltoprightResource1"></asp:Label></a></td>
				<td><a href="javascript:addText(3)">
                    <asp:Label ID="lbltxttopright" runat="server" Text="Add to Top Right" meta:resourcekey="lbltxttoprightResource1"></asp:Label></a></td>
			</tr>
			<tr>
				<td><a href="javascript:addField(4)">
                    <asp:Label ID="lblbottomleft" runat="server" Text="Add to Bottom Left" meta:resourcekey="lblbottomleftResource1"></asp:Label></a></td>
				<td><a href="javascript:addText(4)">
                    <asp:Label ID="lbltxtbottomleft" runat="server" Text="Add to Bottom Left" meta:resourcekey="lbltxtbottomleftResource1"></asp:Label></a></td>
			</tr>
			<tr>
				<td><a href="javascript:addField(5)">
                    <asp:Label ID="lblbottomcentre" runat="server" Text="Add to Bottom Centre" meta:resourcekey="lblbottomcentreResource1"></asp:Label></a></td>
				<td><a href="javascript:addTet(5)">
                    <asp:Label ID="lbltxtbottomcentre" runat="server" Text="Add to Bottom Centre" meta:resourcekey="lbltxtbottomcentreResource1"></asp:Label></a></td>
			</tr>
			<tr>
				<td><a href="javascript:addField(6)">
                    <asp:Label ID="lblbottomright" runat="server" Text="Add to Bottom Right" meta:resourcekey="lblbottomrightResource1"></asp:Label></a></td>
				<td><a href="javascript:addText(6)">
                    <asp:Label ID="lbltxtbottomright" runat="server" Text="Add to Bottom Right" meta:resourcekey="lbltxtbottomrightResource1"></asp:Label></a></td>
			</tr>
		</table>
	</div>
	<div class="inputpanel">
		<div class="inputpaneltitle">
            <asp:Label ID="lblpreview" runat="server" Text="Quick Entry Print Preview" meta:resourcekey="lblpreviewResource1"></asp:Label></div>
		<table height="300" width="700" border="0">
			<tr>
				<td colspan="2"><a href="javascript:moveUp();"><img src="../buttons/b_ctrl_up.gif"></a>&nbsp;&nbsp;<a href="javascript:moveDown();"><img src="../buttons/b_ctrl_down.gif"></a>&nbsp;&nbsp;<a href="javascript:deleteItem();"><img src="../icons/delete2.gif"></a></td>
				</tr>
			<tr>
				<td align="center">
					<div id="Top Left">
						<table class="datatbl">
							<tr>
								<th align="center">
                                    <asp:Label ID="lblpreviewtopleft" runat="server" Text="Top Left" meta:resourcekey="lblpreviewtopleftResource1"></asp:Label></th></tr>
						</table>
					</div>
				</td>
				<td align="center">
					<div id="Top Centre">
						<table class="datatbl" id="topcentre">
							<tr>
								<th align="center">
                                    <asp:Label ID="lblpreviewtopcentre" runat="server" Text="Top Centre" meta:resourcekey="lblpreviewtopcentreResource1"></asp:Label></th></tr>
						</table>
					</div>
				</td>
				<td align="center">
					<div id="Top Right">
						<table class="datatbl" id="topright">
							<tr>
								<th align="center">
                                    <asp:Label ID="lblpreviewtopright" runat="server" Text="Top Right" meta:resourcekey="lblpreviewtoprightResource1"></asp:Label></th></tr>
						</table>
					</div>
				</td>
			</tr>
			<tr>
				<td align="center" colSpan="3" height="60">
                    <asp:Label ID="lblquickentryform" runat="server" Text="Quick Entry Form" meta:resourcekey="lblquickentryformResource1"></asp:Label></td>
			</tr>
			<tr>
				<td align="center">
					<div id="Bottom Left">
						<table id="bottomleft" class="datatbl">
							<tr>
								<th align="center">
                                    <asp:Label ID="lblpreviewbottomleft" runat="server" Text="Bottom Left" meta:resourcekey="lblpreviewbottomleftResource1"></asp:Label></th></tr>
						</table>
					</div>
				</td>
				<td align="center">
					<div id="Bottom Centre">
						<table id="bottomcentre" class="datatbl">
							<tr>
								<th align="center">
                                    <asp:Label ID="lblpreviewbottomcentre" runat="server" Text="Bottom Centre" meta:resourcekey="lblpreviewbottomcentreResource1"></asp:Label></th></tr>
						</table>
					</div>
				</td>
				<td align="center">
					<div id="Bottom Right">
						<table id="bottomright" class="datatbl">
							<tr>
								<th align="center">
                                    <asp:Label ID="lblpreviewbottomright" runat="server" Text="Bottom Right" meta:resourcekey="lblpreviewbottomrightResource1"></asp:Label></th></tr>
						</table>
					</div>
				</td>
			</tr>
		</table>
	</div>
	<div class="inputpanel"><asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" CausesValidation="False" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>

    </asp:Content>

