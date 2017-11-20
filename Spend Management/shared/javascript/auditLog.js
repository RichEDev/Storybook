function generateReportRequest() {
    var element = null;
    if (document.getElementById(ddlstElement).options[document.getElementById(ddlstElement).selectedIndex].value != '0') {
        element = new Number(document.getElementById(ddlstElement).options[document.getElementById(ddlstElement).selectedIndex].value);
    }
    var action = null;
    if (document.getElementById(ddlstAction).options[document.getElementById(ddlstAction).selectedIndex].value != '0') {
        action = new Number(document.getElementById(ddlstAction).options[document.getElementById(ddlstAction).selectedIndex].value);
    }
    var startdate = document.getElementById(txtstartdate).value.substring(6, 10) + "/" + document.getElementById(txtstartdate).value.substring(3, 5) + "/" + document.getElementById(txtstartdate).value.substring(0, 2);
    var enddate = document.getElementById(txtenddate).value.substring(6, 10) + "/" + document.getElementById(txtenddate).value.substring(3, 5) + "/" + document.getElementById(txtenddate).value.substring(0, 2);
    var username = document.getElementById(txtusername).value;
    
    PageMethods.generateReportRequest(element, action, startdate, enddate, username, generateReportRequestComplete)
}

function generateReportRequestComplete(data) {
    window.open("../reports/exportreport.aspx?exporttype=2&requestnum=" + data, 'export', 'width=300,height=150,status=no,menubar=no');
}

function clearAuditLog() {
    if (confirm('Are you sure you wish to clear the audit log. ALL audit history will be deleted permanently.')) {
        PageMethods.clearAuditLog(clearAuditLogComplete);
    }
}

function clearAuditLogComplete(data) {
    if (data) {
        alert('The audit log has been cleared.');
        window.location.reload();
    }
    else {
        alert('Sorry, you do not have permission to clear the audit log.');
    }
}