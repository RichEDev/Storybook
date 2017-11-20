var calendar;
var target;

function displayCalendar()
{
	window.name = 'main';
	calendar = window.open('../calendar.aspx','caldoc','height=300, width=250, menubar=no;statusbar=yes');
}

function getcalendar(day)
{

	var year;
	var month;
	var day

	var strday = new String();
	strday = day.toString();
	if (strday.length == 1)
	{
		day = "0" + day;
	}
	
	year = calendar.document.getElementById('year').options[calendar.document.getElementById('year').selectedIndex].value;
	
	month = calendar.document.getElementById('month').options[calendar.document.getElementById('month').selectedIndex].value;
	
	var datebox = document.getElementById(target);
	datebox.value = day + "/" + month + "/" + year;
	

	calendar.close();
}

	   
	   
			
			
			