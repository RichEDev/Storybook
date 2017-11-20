<%@ Page language="c#" Inherits="expenses.admin.qeformdesign" MasterPageFile="~/expform.master" Codebehind="qeformdesign.aspx.cs" %>
<%@ MasterType VirtualPath="~/expform.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .inputpanel a{
            padding-left:-20px;
            padding-right: 20px;
        }

        .qeformdesign_table table input {
            margin-top: 10px;
            margin-bottom: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script type="text/javascript">
	var columns = new Array();
	var seloption = 0;
	function createTable()
	{
		var i;
		var tbl = new String();
		var rowclass = 'row1';
		tbl = '<table class="datatbl">\n';
		tbl += '<tr><td colspan="3"><a href="javascript:moveUp();"><img src="../buttons/b_ctrl_up.gif"></a>&nbsp;&nbsp;<a href="javascript:moveDown();"><img src="../buttons/b_ctrl_down.gif"></a>&nbsp;&nbsp;<a href="javascript:deleteItem();"><img src="../icons/delete2.gif"></a>';
		tbl += '<tr><th></th><th>Column No</th><th>Column Name</th></tr>\n';
		for (i = 0; i < columns.length; i++)
		{
		    tbl += '<tr><td class=' + rowclass + '><input type="radio" value="' + i + '" name="move" id="move' + i + '" onclick="selectOption(' + i + ');"><input type="hidden" name="coltype" id="coltype' + i + '" value="' + columns[i][1] + '"><input type="hidden" name="colid" id="colid' + i + '" value="' + columns[i][2] + '"></td><td class="' + rowclass + '">' + (i + 1) + '</td><td class="' + rowclass + '">' + columns[i][3] + '</td></tr>\n';
			if (rowclass == 'row1')
			{
				rowclass = 'row2';
			}
			else
			{
				rowclass = 'row1';
			}
		}
		tbl += '</table>';
		
		document.getElementById('columnstbl').innerHTML = tbl;
	}
	
	function assignField()
	{
		var i = new Number();
		var selopt;
		i = columns.length;
		if (document.getElementById('availfields').selectedIndex == -1)
		{
			return;
		}
		selopt = document.getElementById('availfields').options[document.getElementById('availfields').selectedIndex];
		
		columns[i] = new Array(i,1,selopt.value,selopt.text);
		createTable();
		
		document.getElementById('availfields').options[document.getElementById('availfields').selectedIndex] = null;
	}
	
	function assignSubcat()
	{
		var i = new Number();
		var selopt;
		i = columns.length;
		if (document.getElementById('availsubcats').selectedIndex == -1)
		{
			return;
		}
		selopt = document.getElementById('availsubcats').options[document.getElementById('availsubcats').selectedIndex];
		
		columns[i] = new Array(i,2,selopt.value,selopt.text);
		createTable();
		
		document.getElementById('availsubcats').options[document.getElementById('availsubcats').selectedIndex] = null;
		
		
	}
	
	function selectOption(index)
	{
		seloption = index;
	}
	function moveUp()
	{
		var index = seloption;
		var current, previous;
		if (index == 0)
		{
			return;
		}
		
		current = columns[index];
		previous = columns[index-1];
		columns[index] = previous;
		columns[index-1] = current;
		seloption = index-1;

		createTable();

		document.getElementById('move' + seloption).checked = true;		
		//Form1.move[seloption].checked = true;
	}
	
	function moveDown()
	{
		var index = seloption;
		var current, next;
		if (index == (columns.length-1))
		{
			return;
		}
		current = columns[index];
		next = columns[index+1];
		columns[index] = next;
		columns[index+1] = current;
		seloption = index+1;
		createTable();
		
		document.getElementById('move' + seloption).checked = true;	
		//Form1.move[seloption].checked = true;
	}
	
	function deleteItem()
	{
		var index = seloption;
		var i;
		
		if (columns[index][2] == 14)
		{
			alert('Date is a mandatory column and cannot be removed.');
			return;
		}
		var opt = document.createElement("OPTION");
		opt.text = columns[index][3]
		opt.value = columns[index][2];
		if (columns[index][1] == 1)
		{
			document.getElementById('availfields').options.add(opt);
		}
		else
		{
			document.getElementById('availsubcats').options.add(opt);
		}
		
		columns[index] = null;
		for (i = (index+1); i < columns.length; i++)
		{
			columns[i-1] = columns[i];
		}
		
		columns.pop();
		createTable();
	}
</script>

	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Available Columns</asp:Label>
            </div>
		<table>
			<tr>
				<td>
					<asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">Available Fields</asp:Label></td>
				<td></td>
				<td>
					<asp:Label id="Label3" runat="server" meta:resourcekey="Label3Resource1">Available Expense Items</asp:Label></td>
				<td></td>
			</tr>
			<tr>
				<td><div class="qeformdesign_select"><asp:literal id="litfields" runat="server" meta:resourcekey="litfieldsResource1"></asp:literal></div></td>
				<td><a href="javascript:assignField();">Add to Form</a></td>
				<td>
					<div class="qeformdesign_select"><asp:Literal id="litsubcats" runat="server" meta:resourcekey="litsubcatsResource1"></asp:Literal></div></td>
				<td><a href="javascript:assignSubcat();">Add to Form</a></td>
			</tr>
		</table>
	
	<div class="inputpanel">
		<div class="inputpaneltitle">
			<asp:Label id="Label4" runat="server" meta:resourcekey="Label4Resource1">Selected Columns</asp:Label></div>
		<div class="qeformdesign_table"><div id="columnstbl"></div></div>
	</div>
	<div class="inputpanel">
		<asp:ImageButton id="cmdok" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" meta:resourcekey="cmdokResource1"></asp:ImageButton>&nbsp;&nbsp;
		<asp:ImageButton id="cmdcancel" runat="server" ImageUrl="../buttons/cancel_up.gif" meta:resourcekey="cmdcancelResource1"></asp:ImageButton></div>


    </asp:Content>
