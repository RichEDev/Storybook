var RelationshipTextBoxOnLoadArray = new Array();

function stopPropagation(evt) {
    var e = (evt) ? evt : window.event;

    if (window.event) {
        e.cancelBubble = true;
    } else {
        e.stopPropagation();
    }

    return true;
}

function SetDropDownListValue(dropDownList, Value) {

    if (dropDownList != undefined) {
        for (var i = 0; i < dropDownList.options.length; i++) {
            if (dropDownList.options[i].value == Value) {
                dropDownList.options[i].selected = true;
            }
        }
    }
    return;
}

function browserName() {
    var agt = navigator.userAgent.toLowerCase();
    if (agt.indexOf("opera") != -1) return 'Opera';
    if (agt.indexOf("firefox") != -1) return 'Firefox';
    if (agt.indexOf("safari") != -1) return 'Safari';
    if (agt.indexOf("msie") != -1) return 'IE';
    if (agt.indexOf("netscape") != -1) return 'Netscape';
    if (agt.indexOf("mozilla/5.0") != -1) return 'Mozilla';
    if (agt.indexOf('\/') != -1) {
        if (agt.substr(0, agt.indexOf('\/')) != 'mozilla') {
            return navigator.userAgent.substr(0, agt.indexOf('\/'));
        }
        else return 'Netscape';
    } else if (agt.indexOf(' ') != -1)
        return navigator.userAgent.substr(0, agt.indexOf(' '));
    else return navigator.userAgent;
}

function validateform(validationGroup) {
    if (this.Page_Validators == undefined)
        return true;

    var validationErrorMessage = "";

    if (validationGroup == null) {
        Page_ClientValidate();
    }
    else {
        Page_ClientValidate(validationGroup);
    }

    if (Page_IsValid == false) {
        validationErrorMessage = "<ul style=\"margin:0; padding: 0;list-style-type: none;\">";
        for (i = 0; i < Page_Validators.length; i++) {
            if (Page_Validators[i].isvalid == false && typeof (Page_Validators[i].errormessage) == "string" && (validationGroup == null || Page_Validators[i].validationGroup == validationGroup)) {
                validationErrorMessage += "<li>" + Page_Validators[i].errormessage + "</li>";
            }
        }

        validationErrorMessage += "</ul>";
        if (typeof showMasterPopup == 'function')
        {
            // changed to be more friendly showMasterPopup(validationErrorMessage, "Page Validation Failed");
            showMasterPopup(validationErrorMessage, "Message from <strong><i>expenses</i>2010</strong>");
        }
        else {
            validationErrorMessage = "";

            for (i = 0; i < Page_Validators.length; i++) {
                if (Page_Validators[i].isvalid == false && typeof (Page_Validators[i].errormessage) == "string" && (validationGroup == null || Page_Validators[i].validationGroup == validationGroup)) {
                    validationErrorMessage += "-" + Page_Validators[i].errormessage + "\n";
                }
            }
            alert(validationErrorMessage);
        }


        return false;
    } else {
        return true;
    }
}

function changePage(id) {


    var pages = document.getElementById('divPages').getElementsByTagName('div');
    var pgOption;
    var pgLnk;
    for (var i = 0; i < pages.length; i++) {
        if (pages[i].id.length >= 2 && pages[i].id.substring(0, 2) == 'pg') {
            pages[i].style.display = 'none';
        }
    }

    var options = document.getElementById('divPageOptions').getElementsByTagName('div');
    for (var i = 0; i < options.length; i++) {
        pgOption = options[i];
        pgOption.style.display = 'none';
        pgOption.setAttribute("class", "");
        pgOption.setAttribute("className", "");
    }
    pgOption = document.getElementById('pg' + id);

    if (pgOption != undefined) {
        pgOption.style.display = '';
    }

    if (document.getElementById('pgOpt' + id) != null) {
        document.getElementById('pgOpt' + id).style.display = '';
    }


    var pagelinks = document.getElementById('divPageMenu').getElementsByTagName('a'); ;

    var link;
    for (var i = 0; i < pagelinks.length; i++) {

        link = pagelinks[i];



        link.setAttribute("class", "");
        link.setAttribute("className", "");


    }

    link = document.getElementById('lnk' + id);
    link.setAttribute("class", "selectedPage");
    link.setAttribute("className", "selectedPage");
    document.getElementById('subpagetitle').innerHTML = document.getElementById('lnk' + id).innerHTML;
    showOrHidePageOptions();
}

function showOrHidePageOptions()
{
    if ($('#divPageOptions').children(':visible').text() === '') {
        $('#Div1>.panel>.paneltitle').hide();
    } else {
        $('#Div1>.panel>.paneltitle').show();
    }
}

function addFloat(a, b) {
    return a + b;
}

function addNumber(a, b) {
    return parseInt(a) + parseInt(b);
}

function FormatCurrency(amount)
{
	var delimiter = ",";
	var a = amount.toString().split('.',2);
	var d = a[1];
	var i = parseInt(a[0]);
	if(isNaN(i)) { return ''; }
	var minus = '';
	if(i < 0) { minus = '-'; }
	i = Math.abs(i);
	var n = new String(i);
	a = [];
	while(n.length > 3)
	{
		var nn = n.substr(n.length-3);
		a.unshift(nn);
		n = n.substr(0,n.length-3);
	}
	if(n.length > 0) { a.unshift(n); }
	n = a.join(delimiter);
	if(d.length < 1) { amount = n; }
	else { amount = n + '.' + d; }
	amount = minus + amount;
	return amount;
}

function addAspNetValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup) {
    if (document.getElementById(inputValidatorSpanId) && document.getElementById(controlToValidate) && errorMessage != "") {
        Page_Validators.push(document.getElementById(inputValidatorSpanId));
        var inputVarStr = inputValidatorSpanId;
        var inputValidatorSpanId = document.all ? document.all[inputValidatorSpanId] : document.getElementById(inputValidatorSpanId);
        inputValidatorSpanId.controltovalidate = controlToValidate;
        inputValidatorSpanId.errormessage = errorMessage;
        inputValidatorSpanId.validationGroup = validationGroup;
        inputValidatorSpanId.initialvalue = "";
        inputValidatorSpanId.text = "*";
        document.getElementById(inputVarStr).dispose = function() { Array.remove(Page_Validators, document.getElementById(inputVarStr)); }
        return true;
    }
    else {
        showMasterPopup("Incorrect Validator Parameters", "Validation Failed");
        return false;
    }

}


function addMandatoryValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup) {
    var response = addAspNetValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    if (response == true) {
        var inputValidatorSpanId = document.all ? document.all[inputValidatorSpanId] : document.getElementById(inputValidatorSpanId);
        inputValidatorSpanId.evaluationfunction = "RequiredFieldValidatorEvaluateIsValid";
        LoadValidator();
    }
}

function addMandatorySelectValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup) {
    var response = addAspNetValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    if (response == true) {
        var inputValidatorSpanId = document.all ? document.all[inputValidatorSpanId] : document.getElementById(inputValidatorSpanId);
        inputValidatorSpanId.evaluationfunction = "CompareValidatorEvaluateIsValid";
        inputValidatorSpanId.type = "Integer";
        inputValidatorSpanId.valuetocompare = 0;
        inputValidatorSpanId.operator = "GreaterThan";
        LoadValidator();
    }
}

function addGreaterThanOrEqualIntegerValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup, valueToCompare) {
    var response = addAspNetValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    if (response == true) {
        var inputValidatorSpanId = document.all ? document.all[inputValidatorSpanId] : document.getElementById(inputValidatorSpanId);
        inputValidatorSpanId.evaluationfunction = "CompareValidatorEvaluateIsValid";
        inputValidatorSpanId.type = "Integer";
        if (valueToCompare === undefined || valueToCompare == null) {
            showMasterPopup("ValueToCompare has not been set in a addGreaterThanOrEqualValidator in " + inputValidatorSpanId + ", " + controlToValidate, "Javascript Code Error");
        }
        else {
            inputValidatorSpanId.valuetocompare = valueToCompare;
        }
        inputValidatorSpanId.operator = "GreaterThanEqual";
        LoadValidator();
    }
}

function addGreaterThanDoubleValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup, valueToCompare) {
    var response = addAspNetValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    if (response == true) {
        var inputValidatorSpanId = document.all ? document.all[inputValidatorSpanId] : document.getElementById(inputValidatorSpanId);
        inputValidatorSpanId.evaluationfunction = "CompareValidatorEvaluateIsValid";
        inputValidatorSpanId.type = "Currency";
        inputValidatorSpanId.decimalChar = ".";
        if (valueToCompare === undefined || valueToCompare == null) {
            showMasterPopup("ValueToCompare has not been set in a addGreaterThanValidator in " + inputValidatorSpanId + ", " + controlToValidate, "Javascript Code Error");
        }
        else {
            inputValidatorSpanId.valuetocompare = valueToCompare;
        }
        inputValidatorSpanId.operator = "GreaterThan";
        LoadValidator();
    }
}

function addDecimalTypeValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup) {
    var response = addAspNetValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    if (response == true) {
        var inputValidatorSpanId = document.all ? document.all[inputValidatorSpanId] : document.getElementById(inputValidatorSpanId);
        inputValidatorSpanId.type = "Double";
        inputValidatorSpanId.decimalchar = ".";
        inputValidatorSpanId.evaluationfunction = "CompareValidatorEvaluateIsValid";
        inputValidatorSpanId.operator = "DataTypeCheck";
        LoadValidator();
    }

}

function LoadValidator() {
    if (typeof (ValidatorOnLoad) == "function") {
        ValidatorOnLoad();
    }

}

// generic redirect
function getTaskRegardingRecord(regardingArea, regardingId) {
    var url = '';

    switch (regardingArea) {
        case 1: //CONTRACT_DETAILS
            url = '/contractsummary.aspx?tab=0&id=' + regardingId;
            break;
        case 5: //CONTRACT_GROUPING
            url = '/contractsummary.aspx?tab=1&id=' + regardingId;
            break;
        case 2: //CONTRACT_PRODUCTS
        case 7: //CONPROD_GROUPING
            url = '/contractsummary.aspx?tab=2&id=' + regardingId;
            break;

        case 3: //PRODUCT_DETAILS
            url = '/shared/admin/productdetails.aspx?id=' + regardingId;
            break;
        case 4: // supplier details
        case 12: // supplier additional
            url = '/shared/supplier_details.aspx?sid=' + regardingId;
            break;

        case 6: // recharge

            break;
        case 9: // invoice details

            break;

        case 10: // invoice forecasts

            break;
        case 11: //supplier contacts

            break;
        case 13: // employees
            url = '/shared/admin/adminemployees.aspx?employeeid=' + regardingId;
            break;
        default:
            break;

    }

    window.location.href = url;
}

function DontSubmitForm(event) {
    if (event.keyCode == 13) {
        return false;
    }
}

function HTMLUdf_ClientValidate(source, args)
{
    var ctl = document.getElementById(source.controltovalidate);
    var content = $get(source.controltovalidate + '_ctl02_ctl00').contentWindow.document.body.innerHTML;

    if (content != '')
    {
        args.IsValid = true;
    }
    else
    {
        args.IsValid = false;
    }
}


function MasterMenuItemShowPolicy(path)
{
    window.open(path, null, 'width=500, height=500, resizable=yes, scrollbars=yes');
}
function MasterMenuItemOver(menuItem, src)
{
    menuItem.getElementsByTagName('img')[0].src = src;
    menuItem.getElementsByTagName('span')[0].style.color = "#003768";
    menuItem.getElementsByTagName('hr')[0].style.color = "#003768";
}
function MasterMenuItemOut(menuItem, src)
{
    menuItem.getElementsByTagName('img')[0].src = src;
    menuItem.getElementsByTagName('span')[0].style.color = "#6280a7";
    menuItem.getElementsByTagName('hr')[0].style.color = "#6280a7";
}



function NetscapeEventHandler_KeyDown(e) {
    if (e.which === 13 && e.target.type != 'textarea' && e.target.type != 'submit') { return true; }
    return false;
}

function MicrosoftEventHandler_KeyDown() {
    if (event.keyCode === 13 && event.srcElement.type != 'textarea' && event.srcElement.type != 'submit') { return true; }
    return false;
}

function RunOnEnter(e, functionName) 
{
    var nav = window.event ? true : false;
    if (nav) {
        if (MicrosoftEventHandler_KeyDown() === true)
        {
            eval(functionName);
            return false;
        }
    } else {
        if(NetscapeEventHandler_KeyDown(e) === true)
        {
            eval(functionName);
            e.preventDefault();
            return false;
        }
    }
    return true;
}



function GetHighestZIndex() 
{
    var documentDIVS = new Array();
    documentDIVS = document.getElementsByTagName("DIV");
    var highestZIndex = 0;
    for (var i = 0; i < documentDIVS.length; i++) {
        var zIndex = documentDIVS[i].style.zIndex;
        if(documentDIVS[i].style.display == "")
        {
            if (zIndex > highestZIndex) {
                highestZIndex = zIndex;
            }
        }
        }
    return highestZIndex;
}

function parseScript(_source) {
    var source = _source;
    var scripts = new Array();

    // Strip out tags
    while (source.indexOf("<script") > -1 || source.indexOf("</script") > -1) {
        var s = source.indexOf("<script");
        var s_e = source.indexOf(">", s);
        var e = source.indexOf("</script", s);
        var e_e = source.indexOf(">", e);

        // Add to scripts array
        scripts.push(source.substring(s_e + 1, e));
        // Strip from source
        source = source.substring(0, s) + source.substring(e_e + 1);
    }

    // Loop through every script collected and eval it
    for (var i = 0; i < scripts.length; i++) {
        try {
            eval(scripts[i]);
        }
        catch (ex) {
            // do what you want here when a script fails
        }
    }

    // Return the cleaned source
    return source;
}

Array.prototype.contains = function(element)
{
    var i = 0;
    while (i < this.length)
    {
        if (this[i] === element)
        {
            return true;
        }
        else
        {
            i++;
        }
    }

    return false;
}

Array.prototype.remove = function(element)
{
    var i = 0;
    while (i < this.length)
    {
        if (this[i] === element)
        {
            this.splice(i, 1);
        } 
        else
        {
            i++;
        }
    }
    return;
}

function WebServiceError(data)
{
    if (data['_message'])
    {
        showMasterPopup("An error was returned as a result of something you've tried to do, it may be useful to send this to an administrator of the system.<br /><div class=\"masterPopupErrorMsg\" onclick=\"this.getElementsByTagName('div')[0].style.display=='none' ? this.getElementsByTagName('div')[0].style.display='' : this.getElementsByTagName('div')[0].style.display='none';\">Click here to see the message<div style=\"display: none;\"><br />" + data['_message'] + '</div></div>', 'Sorry, An Error Occurred');
    }
    else
    {
        showMasterPopup("An error was returned as a result of something you've tried to do, it may be useful to send this to an administrator of the system.<div class=\"masterPopupErrorMsg\" onclick=\"this.getElementsByTagName('div')[0].style.display=='none' ? this.getElementsByTagName('div')[0].style.display='' : this.getElementsByTagName('div')[0].style.display='none';\">" + data + '</div></div>', 'Sorry, An Error Occurred');
    }
    return;
}

function EscapeHTMLInString(text)
{
    text.replace('<', '&lt;');

    return text;
}

//Cast the return codes to a readable context when debugging or viewing the javascript. The return code values should match those of the
//'ReturnValues' enum in the SpendManagementLibrary
var ReturnCodeEnum = { 
    Success: { intVal:0, messageText:"" },
    AlreadyExists: { intVal:-1, messageText:"This item already exists" },
    Error: { intVal:-2, messageText:"An error has occurred processing this request" },
    EmployeeHomeLocationAssigned: { intVal:-3, messageText:"This address is assigned to an employees Home address" },
    EmployeeWorkLocationAssigned: { intVal: -4, messageText: "This address is assigned to an employees Work address" },
    ExpenseItemLocationAssigned: { intVal: -5, messageText: "This address is assigned to an expense item" },
    CustomFieldAssignedToWorkflowConditon: { intVal: -6, messageText: "Cannot delete as this custom field is assigned to a workflow condition" },
    CustomFieldAssignedToCustEntView: { intVal: -7, messageText: "Cannot delete as this custom field is assigned to a GreenLight view" }
};

//This is a generic method that displays the message text for a specific return value to the user. 
function DisplayReturnMessage(returnCode)
{
    switch (returnCode)
    {
        case ReturnCodeEnum.Success.intVal:
            break;
        case ReturnCodeEnum.AlreadyExists.intVal:
            showMasterPopup(ReturnCodeEnum.AlreadyExists.messageText, "Warning");
            break;
        case ReturnCodeEnum.Error.intVal:
            showMasterPopup(ReturnCodeEnum.Error.messageText, "Warning");
            break;
        case ReturnCodeEnum.EmployeeHomeLocationAssigned.intVal:
            showMasterPopup(ReturnCodeEnum.EmployeeHomeLocationAssigned.messageText, "Warning");
            break;
        case ReturnCodeEnum.EmployeeWorkLocationAssigned.intVal:
            showMasterPopup(ReturnCodeEnum.EmployeeWorkLocationAssigned.messageText, "Warning");
            break;
        case ReturnCodeEnum.ExpenseItemLocationAssigned.intVal:
            showMasterPopup(ReturnCodeEnum.ExpenseItemLocationAssigned.messageText, "Warning");
            break;
        case ReturnCodeEnum.CustomFieldAssignedToWorkflowConditon.intVal:
            showMasterPopup(ReturnCodeEnum.CustomFieldAssignedToWorkflowConditon.messageText, "Warning");
            break;
        case ReturnCodeEnum.CustomFieldAssignedToCustEntView.intVal:
            showMasterPopup(ReturnCodeEnum.CustomFieldAssignedToCustEntView.messageText, "Warning");
            break;
        default:
            break;
    }
}