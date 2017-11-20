var ufName;
var ufRecId;
var ufArea;
var ufTitle;
var ufHiddenText;

function doTextEntry(UF_name, UF_recId, UF_area, UF_hiddentext, UF_title) {
    ufName = UF_name;
    ufRecId = UF_recId;
    ufArea = UF_area;
    ufTitle = UF_title;
    ufHiddenText = UF_hiddentext;

    var cntl = document.getElementById('ctl00_contentmain_' + ufName);
    if (cntl != null) {
        if (browserName == 'IE') {
            document.getElementById('txtRTE').innerText = cntl.value;
            document.getElementById('lblRTE').innerText = ufTitle;
            document.getElementById('lblTextEditorHeading').innerHTML = 'Text edit window for ' + ufTitle;
        }
        else {
            var rte = document.getElementById('txtRTE');
            var lblrte = document.getElementById('lblRTE');
            var heading = document.getElementById('lblTextEditorHeading');

            rte.value = cntl.value;
            lblrte.innerHTML = ufTitle;
            heading.innerHTML = 'Text edit window for ' + ufTitle;
        }
    }

    LaunchTEModal();
}

function saveRTEdit() {
    var cntl = document.getElementById('ctl00_contentmain_' + ufName);
    var modalTxtBox = document.getElementById('txtRTE');
    if (cntl != null) {
        cntl.value = modalTxtBox.value;
    }

    cntl = document.getElementById('ctl00_contentmain_' + ufHiddenText);
    if (cntl != null) {
        cntl.value = modalTxtBox.value;
    }
    HideModal();
}

function LaunchTEModal() {
    $find(rteModal).show();

    var rte = document.getElementById('txtRTE');
    if (rte != null) {
        rte.focus();
    }
}

function HideModal() {
    $find(rteModal).hide();
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
