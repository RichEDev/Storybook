<%@ Page language="c#" Inherits="expenses.calendar" StylesheetTheme="ExpensesTheme" Codebehind="calendar.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat=server>
		<title>calendar</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK type="text/css" rel="stylesheet">
		<script language="javascript">
		var selDate;

		var calendar = "";
		var startday = 0;
		var endDay = 0;
		var padding = 0;
		
		var rowclass = "row1";
		
		function changeCalendar()
		{
			var month = new Number();
			var year = new Number();
			

			month = document.getElementById('month').options[document.getElementById('month').selectedIndex].value;
			year = document.getElementById('year').options[document.getElementById('year').selectedIndex].value;
			
			
			calendar = "";
			drawCalendar(month,year);
				
			var div = document.getElementById("caldiv");
			div.innerHTML = calendar;
		}
		
		function setDay(day)
		{
			//calendar.document.getElementById('dtdate');
			
			
			
		}
		function getYears()
		{
			var today = new Date();
			var output;
			var year = today.getFullYear();
			var i;
			var option;
			
			for (i = year; i >= (year-2); i--)
			{
				option = document.createElement('OPTION');
				option.text = i;
				option.value = i;
				document.getElementById('year').options.add(option);
			}
			
			
			//document.getElementById('yearcell').innerHTML = output;
		}
			function window_onload()
			{
				getYears();
				var today = new Date();
				var month = new Number();
				month = today.getMonth();
				document.getElementById('month').options[month].selected=true;
				changeCalendar();
				
			}
			
			function drawCalendar (month,year)
			{
				
				var output = new String();
				var firstDay = new Date();
				
				endDay = getDaysInMonth(month, year);
				
				createHeader();
				
				firstDay.setDate(01);
				firstDay.setMonth(month-1);
				firstDay.setFullYear(year);
				
				padStart(firstDay);
				createDays();
				
				
			}

	
			function createHeader()
			{
				var output;
				
				output = "<table class=calendar width=70>";
				output += "<tr><th>M</th><th>T</th><th>W</th><th>T</th><th>F</th><th>S</th><th>S</th></tr>";
				calendar += output;
				
			}
			
			function createFooter()
			{
				var output;
				output = "</table>";
				calendar += output;
			}
			function getDaysInMonth (month,year)
			{
				
				
				
				var day;
				var daylen = new Number();
				
				var isLeapyear = new Boolean(false);
				
				
				
				switch (month)
				{
					case "01":
					case "03":
					case "05":
					case "07":
					case "08":
					case "10":
					case "12":
					
						daylen = 31;
						break;
					case "02":
						
						
						if ((year % 4) == 0 || (year % 400) == 0)
						{
							isLeapyear = true;
						}
						else
						{
							isLeapyear = false;
						}
						if (isLeapyear == false)
						{
							daylen = 28;
						}
						else
						{
							daylen = 29;
						}
						break;
					
					case "04":
					case "06":
					case "09":
					case "11":
						daylen = 30;
						break;
					default:
						daylen = -1;
					
						
					
				}
				
				return daylen;
			}

			function padStart(startDate)
			{
				var day = startDate.getDay();
				
				var paddays = 0;
				var i;
				var output = "";
				switch (day)
				{
					case 0:
						paddays = 6;
						break;
					case 1:
						paddays = 0;
						break;
					case 2:
						paddays = 1;
						break;
					case 3:
						paddays = 2;
						break;
					case 4:
						paddays = 3;
						break;
					case 5:
						paddays = 4;
						break;
					case 6:
						paddays = 5;
						break;
				}
				
				padding = paddays;
				output = "<tr>";
				for (i = 0; i < paddays; i++)
				{
					output += "<td class='row1'>&nbsp;</td>";
				}
				
				calendar += output;
					
				
			}
			
			function createDays()
			{
				
				var i = 1;
				var output = "";
				
				
				while (i <= endDay)
				{
				
					output += "<td class='" + rowclass + "'><a target=main href='javascript:getcalendar(" + i + ");'>" + i + "</a></td>";
					if (((i+padding) % 7) == 0)
					{
						output += "</tr>";
						if (rowclass == "row1")
						{
							rowclass = "row2";
						}
						else
						{
							rowclass = "row1";
						}
						if ((i+padding) != endDay)
						{
							output += "<tr>";
						}
					}
					i++;
				}
				
				while (((i+padding-1) % 7) != 0)
				{
					output += "<td class='" + rowclass + "'></td>";
					i++;
				}
				output += "</tr>";
				calendar += output;
			}
		
		</script>
</HEAD>
	<body onload="window_onload();">
		<form id="Form1" method="post" runat="server">
			<asp:Literal id="litstyles" runat="server"></asp:Literal>
			<TABLE id="Table1">
				<TR>
					<TD>Month:</TD>
					<td>
						<select id="month" name="month" onchange="return changeCalendar();">
							<option value="01" selected>January</option>
							<option value="02">February</option>
							<option value="03">March</option>
							<option value="04">April</option>
							<option value="05">May</option>
							<option value="06">June</option>
							<option value="07">July</option>
							<option value="08">August</option>
							<option value="09">September</option>
							<option value="10">October</option>
							<option value="11">November</option>
							<option value="12">December</option>
						</select>
					</td>
					<td>Year:</td>
					<td id="yearcell" name="yearcell"><select name="year" id="year" onchange='return changeCalendar();'></select></td>
				</TR>
				<TR>
					<TD colspan="4"><div id="caldiv"></div>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
