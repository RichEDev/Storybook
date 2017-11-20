// JScript File

function getIndex(tblid, id)
{
	var i;
	var tbl = document.getElementById(tblid);
	
	for (i = 0; i < tbl.rows.length; i++)
	{
		if (tbl.rows[i].id == id)
		{
			
			return i;
		}
	}
}
		
function doCallBack(url, dataToSend)
{
	var xmlRequest;
	try
	{
		xmlRequest = new XMLHttpRequest();
	}
	catch (e)
	{	
		try
		{
			xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (f)
		{
			xmlRequest = null;
		}
	}
	
	xmlRequest.open("POST",url,false);
	
	//xmlRequest.setRequestHeader("Content-Type","application/x-wais-source");
	xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	xmlRequest.send(dataToSend);
	return xmlRequest.responseText;
}

 // User field get and fetch for UFSearch.aspx when using dynamic ASP controls.
var searchwnd;
var UF_FieldNum;
var UF_FieldName;
var UF_FieldName_URLText;
var UF_Value;
 
function doASPSearch(i,UF_name)
{
	window.name = 'ufaspsearch';
	UF_FieldName = UF_name;
	searchwnd = window.open('UFSearch.aspx?action=ufaspsearch&searchtype=' + i + '&ufid=' + UF_name,'search','width=600, height=600, scrollbars=yes');
}

function getASPSearchResult()
{
    //alert('UF_FieldName = ' + UF_FieldName);
    
	var UF_Field_txt = $get('ctl00_contentmain_' + UF_FieldName + '_TXT');
	if(UF_Field_txt != null)
	{
	    //alert('got ASP Search result text field');
	    UF_Field_txt.value = searchwnd.document.getElementById('searchResultTxt').value;
	}
	
	var UF_Field_val = $get('ctl00_contentmain_' + UF_FieldName);
	if(UF_Field_val != null)
	{
	    //alert('got ASP Search Result Id field');
	    UF_Field_val.value = searchwnd.document.getElementById('searchResultId').value;
	}
	else
	{
	    alert('id field not found');
	}
	searchwnd.close();
}

function doASPTextEntry(UF_name, UF_recId, UF_area, UF_urltext)
{
	window.name = 'ufasptxt';
	UF_FieldName = UF_name;
    
    var hiddenURLData = $get('ctl00_contentmain_' + UF_urltext);
    if(hiddenURLData != null)
    {
        UF_Value = hiddenURLData.value;
    }
    else
    {
        UF_Value = '';
    }
	
	searchwnd = window.open('UFSearch.aspx?action=ufasptxt&ufn=' + UF_name + '&ufi=' + UF_recId + '&ufa=' + UF_area + '&ufv=' + UF_Value,'search','width=600, height=600, scrollbars=yes');
	if(searchwnd != null)
	{
	    // wait 2 secs to allow window to render
	    var t = setTimeout('populateTextEditor(UF_Value);',1000);
	}
}

    function populateTextEditor(UF_Value)
    {
        var txt = searchwnd.Form1.document.getElementById('hiddenEditorText'); //searchwnd.Form1.
        if(txt != null)
        {    
            var cntl = searchwnd.Form1.document.getElementById('txtTextEditor'); // searchwnd.Form1.
            if(cntl != null)
            {
                cntl.innerHTML = txt.value;
            }
        }
        else
        {
            alert('could not find hiddenEditorText');
        }
    }

function getASPTextEntry()
{
	var UF_Field;
	var src_Field;
	//Form1.
	src_Field = searchwnd.Form1.document.getElementById('txtTextEditor'); // searchwnd.document.getElementById(UF_FieldName);
	// 'Form1.' + 
	UF_Field =  $get('ctl00_contentmain_' + UF_FieldName);
	UF_FieldName_URLText = $get('ctl00_contentmain_' + UF_FieldName + '_URLTEXT');

	if (UF_Field != null && src_Field != null) {
	    //var replexp = new RegExp("\n","g");
	    //var txtData = new String(src_Field.value);
	    UF_Field.innerHTML = src_Field.value;  //txtData.replace(replexp,'<br>');
	}

	if (UF_FieldName_URLText != null && src_Field != null) {
	    UF_FieldName_URLText.value = src_Field.value;
	}
	searchwnd.close();
}
