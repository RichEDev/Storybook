function okayScript() {
    document.getElementById(rdbYesID).checked = true;
    document.getElementById(rdbNoID).checked = false;
    var txtDln = document.getElementById(txtDLNID);
    txtDln.readOnly = false;
    //if (ValidateRegx(txtDln) == false) { txtDln.readOnly = false;}else {txtDln.readOnly = true;}   
}
function cancelScript() {
    document.getElementById(rdbNoID).checked = true;
    document.getElementById(rdbYesID).checked = false;
    document.getElementById(txtDLNID).readOnly = true;
}
function onCancelClick() {
    document.getElementById(rdbNoID).checked = true;
    document.getElementById(rdbYesID).checked = false;
    document.getElementById(chkAcceptID).checked = true;
    document.getElementById(btnAcceptID).disabled = false;
}
function handleClick(myRadio) {
    if (myRadio.checked) {
        document.getElementById(btnAcceptID).disabled = false;
    } else { document.getElementById(btnAcceptID).disabled = true; }
}

function validateLength(oSrc, args) {
    if (isSaveClicked) {
        if (document.getElementById(rdbYesID).checked && document.getElementById(txtDLNID).value == "") {
            args.IsValid = false;
            SEL.MasterPopup.ShowMasterPopup("Please enter the Driving Licence Number before saving details.");
        } else if (document.getElementById(txtDLNID).value != "") {
            document.getElementById(txtDLNID).value = document.getElementById(txtDLNID).value.toString().trim();
            var regPatt = new RegExp("^(?=.{16}$)[A-Za-z]{1,5}9{0,4}[0-9](?:[05][1-9]|[16][0-2])(?:[0][1-9]|[12][0-9]|3[01])[0-9](?:99|[A-Za-z][A-Za-z9])(?![IOQYZioqyz01_])\\w[A-Za-z]{2}", "g");
            var txtDln = document.getElementById(txtDLNID);
            if (ValidateRegx(txtDln) == false) {
                args.IsValid = false;
                if (document.getElementById(rdbYesID).checked == false && loadFirst == "1") {
                    txtDln.value = "";
                    txtDln.readOnly = true;
                } else {
                    SEL.MasterPopup.ShowMasterPopup("Invalid Driving Licence Number, Please enter valid Driving Licence Number.");
                    txtDln.readOnly = false;
                }
            } else {
                txtDln.readOnly = false;
            }
        }
    } isSaveClicked = false;
   
}

function ValidateRegx(txtDln) {
    var regPatt = new RegExp("^(?=.{16}$)[A-Za-z]{1,5}9{0,4}[0-9](?:[05][1-9]|[16][0-2])(?:[0][1-9]|[12][0-9]|3[01])[0-9](?:99|[A-Za-z][A-Za-z9])(?![IOQYZioqyz01_])\\w[A-Za-z]{2}", "g");    
    if (!regPatt.test(txtDln.value)) {
            return false;
        } return true;
}
function rdbtnNoClick(rdbtn) {
    if (loadFirst == "1") {
        var txtDln = document.getElementById(txtDLNID);
        txtDln.value = "";
        txtDln.readOnly = true;
    } else {
        var txtDln = document.getElementById(txtDLNID);
        if (txtDln.value != "" && !ValidateRegx(txtDln)) {
            txtDln.value = "";            
        }
        txtDln.readOnly = true;
    }
}

var loadFirst;
function pageLoad() {   
    loadFirst = document.getElementById(hdnloadfirst).value;
    if (document.getElementById(hdnerrormessageforduplicateLNC).value == "1") {
        SEL.MasterPopup.ShowMasterPopup("Driving Licence Number is already exist.");
        document.getElementById(txtDLNID).removeAttribute("readonly");
    }    
}
var isSaveClicked = false;
function saveButtonClick()
{
    isSaveClicked = true;
}
