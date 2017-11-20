var currentContact;

function deleteContact(contactid) {
    var supplierid = $get(hiddenSupplierId);
    if (contactid != null && supplierid != null) {
        Spend_Management.svcSuppliers.deleteSupplierContact(contactid, supplierid.value, deleteContactComplete, deleteContactError);
    }
}

function deleteContactComplete(contactid) {
    clearContact();
    var message = $get(statusMsg);
    if (message != null) {
        message.value = "Contact Deleted Successfully"; 

        var table = $get('tbl_' + contactGridId);
        for (var i = 0; i < table.rows.length; i++) {
            if (table.rows[i].id == 'tbl_' + contactGridId + '_' + contactid) {
                table.deleteRow(i);
                break;
            }
        }
    }
    return;
}

function deleteContactError(error)
{
    SEL.MasterPopup.ShowMasterPopup('A problem occurred trying to delete the contact.', 'Page Message');
}

function getSupplierRecordComplete(data) {
    window.location.href = 'supplier_details.aspx?sid=' + data.SupplierId;
}

function editContact(contactid) {
    var hiddenId = $get(hiddenSupplierId);
    var supplierId = hiddenId.value;
    var hcId = $get(hiddenContactId);
    hcId.value = contactid;
    clearContact();
    
    Spend_Management.svcSuppliers.retrieveContact(contactid, supplierId, getContactComplete, getContactError);
}

function getContactError(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to retrieve the contact data.', 'Page Message');
}

function getContactComplete(data) {
    $find(contactPanel).show();

    if (data != null) {
        currentContact = data;

        var contactid = $get(hiddenContactId);
        if (contactid != null) {
            contactid.value = data[0].ContactId;
        }
        var path = getSContactsPath();
        var name = $get(path + "txtcontactname");
        if (name != null) {
            name.value = data[0].Name;
        }

        var position = $get(path + "txtposition");
        if (position != null) {
            position.value = data[0].Position;
        }
        var email = $get(path + "txtemail");
        if (email != null) {
            email.value = data[0].Email;
        }
        var mobile = $get(path + "txtmobile");
        if (mobile != null) {
            mobile.value = data[0].Mobile;
        }
        var comments = $get(path + "txtcomments");
        if (comments != null) {
            comments.value = data[0].Comments;
        }
        var maincontact = $get(path + "chkmaincontact");
        if (maincontact != null) {
            maincontact.checked = data[0].MainContact;
        }

        if (data[0].BusinessAddress != null) {
            var baddresstitle = $get(path + "txtbaddresstitle");
            if (baddresstitle != null) {
                baddresstitle.value = data[0].BusinessAddress.AddressTitle;
            }
            var baddr1 = $get(path + "txtbaddress1");
            if (baddr1 != null) {
                baddr1.value = data[0].BusinessAddress.AddressLine1;
            }
            var baddr2 = $get(path + "txtbaddress2");
            if (baddr2 != null) {
                baddr2.value = data[0].BusinessAddress.AddressLine2;
            }
            var btown = $get(path + "txtbtown");
            if (btown != null) {
                btown.value = data[0].BusinessAddress.Town;
            }
            var bcounty = $get(path + "txtbcounty");
            if (bcounty != null) {
                bcounty.value = data[0].BusinessAddress.County;
            }
            var bpcode = $get(path + "txtbpcode");
            if (bpcode != null) {
                bpcode.value = data[0].BusinessAddress.PostCode;
            }
            var bcountry = $get(path + "lstbcountry");
            if (bcountry != null) {
                if (data[0].BusinessAddress.CountryId != null) {
                    // if editing and country is archived, it will not be in the list. Repopulate ddlist and select
                    var bcMatch = false;
                    for (i = 0; i < bcountry.options.length; i++) {
                        if (bcountry.options[i].value == data[0].BusinessAddress.CountryId) {
                            bcMatch = true;
                            break;
                        }
                    }

                    if (!bcMatch)
                    {
                        $.ajax({
                            url: 'webServices/svcCountries.asmx/getCountryItems',
                            async: false,
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: '{"countryID": ' + data[0].BusinessAddress.CountryId + '}',
                            success: function (r)
                            {
                                bcountry.options.length = 0;
                                for (var idx = 0; idx < r.d.length; idx++)
                                {
                                    var option = document.createElement("option");
                                    option.text = r.d[idx].Text;
                                    option.value = r.d[idx].Value;
                                    try
                                    {
                                        bcountry.options.add(option, bcountry.options[null]);
                                    }
                                    catch (e)
                                    {
                                        bcountry.options.add(option, null);
                                    }
                                }
                            },
                            error: function (r)
                            {
                            }
                        });
                    }

                    for (i = 0; i < bcountry.options.length; i++) {
                        if (bcountry.options[i].value == data[0].BusinessAddress.CountryId) {
                            bcountry.selectedIndex = i;
                            break;
                        }
                    }
                }
            }

            var bphone = $get(path + "txtbswitchboard");
            if (bphone != null) {
                bphone.value = data[0].BusinessAddress.Switchboard;
            }
            var bfax = $get(path + "txtbfax");
            if (bfax != null) {
                bfax.value = data[0].BusinessAddress.Fax;
            }
        }

        if (data[0].HomeAddress != null) {
            var paddresstitle = $get(path + "txtpaddresstitle");
            if (paddresstitle != null) {
                paddresstitle.value = data[0].HomeAddress.AddressTitle;
            }
            var paddr1 = $get(path + "txtpaddress1");
            if (paddr1 != null) {
                paddr1.value = data[0].HomeAddress.AddressLine1;
            }
            var paddr2 = $get(path + "txtpaddress2");
            if (paddr2 != null) {
                paddr2.value = data[0].HomeAddress.AddressLine2;
            }
            var ptown = $get(path + "txtptown");
            if (ptown != null) {
                ptown.value = data[0].HomeAddress.Town;
            }
            var pcounty = $get(path + "txtpcounty");
            if (pcounty != null) {
                pcounty.value = data[0].HomeAddress.County;
            }
            var ppcode = $get(path + "txtppcode");
            if (ppcode != null) {
                ppcode.value = data[0].HomeAddress.PostCode;
            }
            var pcountry = $get(path + "lstpcountry");
            if (pcountry != null) {
                if (data[0].HomeAddress.CountryId != null) {
                    // if editing and country is archived, it will not be in the list. Repopulate ddlist and select
                    var pcMatch = false;
                    for (i = 0; i < pcountry.options.length; i++) {
                        if (pcountry.options[i].value == data[0].HomeAddress.CountryId) {
                            pcMatch = true;
                            break;
                        }
                    }

                    if (!pcMatch)
                    {
                        $.ajax({
                            url: 'webServices/svcCountries.asmx/getCountryItems',
                            async: false,
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: '{"countryID": ' + data[0].BusinessAddress.CountryId + '}',
                            success: function (r)
                            {
                                pcountry.options.length = 0;
                                for (var idx = 0; idx < r.d.length; idx++)
                                {
                                    var option = document.createElement("option");
                                    option.text = r.d[idx].Text;
                                    option.value = r.d[idx].Value;
                                    try
                                    {
                                        pcountry.options.add(option, pcountry.options[null]);
                                    }
                                    catch (e)
                                    {
                                        pcountry.options.add(option, null);
                                    }
                                }
                            },
                            error: function (r)
                            {
                            }
                        });
                    }
                }
                
                if (data[0].HomeAddress.CountryId != null) {
                    for (i = 0; i < pcountry.options.length; i++) {
                        if (pcountry.options[i].value == data[0].HomeAddress.CountryId) {
                            pcountry.selectedIndex = i;
                            break;
                        }
                    }
                }
            }
            var pphone = $get(path + "txtpswitchboard");
            if (pphone != null) {
                pphone.value = data[0].HomeAddress.Switchboard;
            }
            var pfax = $get(path + "txtpfax");
            if (pfax != null) {
                pfax.value = data[0].HomeAddress.Fax;
            }
        }

        var i;
        var j;
        var control;

        //Populate user defined fields from the web method
        for (i = 0; i < lstUserdefined.length; i++) {
            for (j = 0; j < data[1].length; j++) {
                control = undefined;

                if (lstUserdefined[i][0] == data[1][j][0]) {
                    control = document.getElementById(lstUserdefined[i][2]);
                }

                if (control != undefined) {
                    switch (lstUserdefined[i][1]) {
                        case 'Text':
                        case 'Currency':
                        case 'Number':
                        case 'Integer':
                            if (data[1][j][1] == null) {
                                control.value = '';
                            }
                            else {
                                control.value = data[1][j][1];

                            }
                            break;
                        case 'Relationship':
                            var idControl = document.getElementById(lstUserdefined[i][2] + '_ID');

                            if (data[1][j][1] == null)
                            {
                                if (idControl !== undefined)
                                {
                                    idControl.value = '';
                                }
                                control.value = '';
                            }
                            else
                            {
                                if (idControl !== undefined)
                                {
                                    idControl.value = data[1][j][1];
                                }
                                control.value = data[1][j][2];
                            }
                            break;
                        case 'RelationshipTextbox':
                            if (data[1][j][1] == null)
                            {
                                control.value = '';
                            }
                            else
                            {
                                control.value = data[1][j][1];
                                showEditLink(data[1][j][1], lstUserdefined[i][2]);

                            }
                            break;
                        case 'LargeText':
                            if (data[1][j][1] == null) {
                                control.value = '';
                            }
                            else {
                                control.value = data[1][j][1];

                                var contentCtl = $get(lstUserdefined[i][2] + '_ctl02_ctl00');

                                if (contentCtl != null || contentCtl != undefined) {
                                    contentCtl.contentWindow.document.body.innerHTML = data[1][j][1];
                                }
                            }
                            break;
                        case 'TickBox':
                            //control.selectedIndex = 0; -- THIS OVERRIDES THE DEFAULT
                            if (data[1][j][1] != null) {
                                // only set selected item if record held, otherwise leave as default
                                var selectVal = (data[1][j][1] == true ? '1' : '0');

                                for (var x = 0; x < control.options.length; x++) {
                                    if (control.options[x].value == selectVal) {
                                        control.selectedIndex = x;
                                        break;
                                    }
                                }
                            }
                            break;
                        case 'List':
                            control.selectedIndex = 0;
                            for (var x = 0; x < control.options.length; x++) {
                                if (control.options[x].value == data[1][j][1]) {
                                    control.selectedIndex = x;
                                    break;
                                }
                            }
                            break;
                        case 'DateTime':
                            if (data[1][j][1] == null) {
                                control.value = '';
                            }
                            else {
                                var tmpDate = data[1][j][1];
                                if (tmpDate.substring(0, 10) != '01/01/1900' && tmpDate != '00:00') {
                                    control.value = data[1][j][1];
                                }
                            }
                    }
                }
            }
        }
    }
}

function getSDetailsPath() {
	var path = "ctl00_contentmain_supplierTabs_SDAdditionalTab";
    
    return path;
}

function getSDAdditionalPath() {
	var path = "ctl00_contentmain_supplierTabs_SDetailsTab";
	if ($get(path + "_suppliername") != null) {
		return path;
	}

	return path;
}

function getSContactsPath() {
	var path = "ctl00_contentmain_";

    return path;
}

function clearContact() {
    currentContact = null;

    var path = getSContactsPath();

    var contactname = $get(path + "txtcontactname");
    if (contactname != null) {
        contactname.value = '';
    }
    var position = $get(path + "txtposition");
    if (position != null) {
        position.value = '';
    }
    var email = $get(path + "txtemail");
    if (email != null) {
        email.value = '';
    }
    var mobile = $get(path + "txtmobile");
    if (mobile != null) {
        mobile.value = '';
    }
    var comments = $get(path + "txtcomments");
    if (comments != null) {
        comments.value = '';
    }
    var maincontact = $get(path + "chkmaincontact");
    if (maincontact != null) {
        maincontact.checked = false;
    }

    var baddresstitle = $get(path + "txtbaddresstitle");
    if (baddresstitle != null) {
        baddresstitle.value = '';
    }
    var baddr1 = $get(path + "txtbaddress1");
    if (baddr1 != null) {
        baddr1.value = '';
    }
    var baddr2 = $get(path + "txtbaddress2");
    if (baddr2 != null) {
        baddr2.value = '';
    }
    var btown = $get(path + "txtbtown");
    if (btown != null) {
        btown.value = '';
    }
    var bcounty = $get(path + "txtbcounty");
    if (bcounty != null) {
        bcounty.value = '';
    }
    var bpcode = $get(path + "txtbpcode");
    if (bpcode != null) {
        bpcode.value = '';
    }
    var bcountry = $get(path + "lstbcountry");
    if (bcountry != null)
    {   
        $.ajax({
            url: 'webServices/svcCountries.asmx/getCountryItems',
            async: false,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: '{ "countryID": 0 }',
            success: function (r)
            {
                bcountry.options.length = 0;

                for (var idx = 0; idx < r.d.length; idx++)
                {
                    var option = document.createElement("option");
                    option.text = r.d[idx].Text;
                    option.value = r.d[idx].Value;
                    try
                    {
                        bcountry.options.add(option, bcountry.options[null]);
                    }
                    catch (e)
                    {
                        bcountry.options.add(option, null);
                    }
                }
            },
            error: function (r)
            {
            }
        });

        bcountry.selectedIndex = 0;
    }
    var bphone = $get(path + "txtbswitchboard");
    if (bphone != null) {
        bphone.value = '';
    }
    var bfax = $get(path + "txtbfax");
    if (bfax != null) {
        bfax.value = '';
    }

    var paddresstitle = $get(path + "txtpaddresstitle");
    if (paddresstitle != null) {
        paddresstitle.value = '';
    }
    var paddr1 = $get(path + "txtpaddress1");
    if (paddr1 != null) {
        paddr1.value = '';
    }
    var paddr2 = $get(path + "txtpaddress2");
    if (paddr2 != null) {
        paddr2.value = '';
    }
    var ptown = $get(path + "txtptown");
    if (ptown != null) {
        ptown.value = '';
    }
    var pcounty = $get(path + "txtpcounty");
    if (pcounty != null) {
        pcounty.value = '';
    }
    var ppcode = $get(path + "txtppcode");
    if (ppcode != null) {
        ppcode.value = '';
    }
    var pcountry = $get(path + "lstpcountry");
    if (pcountry != null)
    {
        $.ajax({
            url: 'webServices/svcCountries.asmx/getCountryItems',
            async: false,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: '{ "countryID": 0 }',
            success: function (r)
            {
                pcountry.options.length = 0;

                for (var idx = 0; idx < r.d.length; idx++)
                {
                    var option = document.createElement("option");
                    option.text = r.d[idx].Text;
                    option.value = r.d[idx].Value;
                    try
                    {
                        pcountry.options.add(option, pcountry.options[null]);
                    }
                    catch (e)
                    {
                        pcountry.options.add(option, null);
                    }
                }
            },
            error: function (r)
            {
            }
        });
        
        pcountry.selectedIndex = 0;
    }
    var pphone = $get(path + "txtpswitchboard");
    if (pphone != null) {
        pphone.value = '';
    }
    var pfax = $get(path + "txtpfax");
    if (pfax != null) {
        pfax.value = '';
    }

    var i;
    var control;

    //Populate user defined fields from the web method
    for (i = 0; i < lstUserdefined.length; i++) {
        control = document.getElementById(lstUserdefined[i][2]);

        if (control != undefined && lstUserdefined[i][3] == "scontacts") {
            switch (lstUserdefined[i][1]) {
                case 'Text':
                case 'Currency':
                case 'Number':
                case 'Integer':
                case 'DateTime':
                case 'LargeText':
                    control.value = "";
                    break;
                case 'Relationship':
                    control.value = '';
                    var idControl = document.getElementById(lstUserdefined[i][2] + '_ID');
                    if (idControl !== undefined)
                        idControl.value = '';
                break;
                case 'TickBox':
                    if (lstUserdefined[i][5] != "") {
                        for (var idx = 0; idx < control.options.length; idx++) {
                            if (lstUserdefined[i][5] == control.options[idx].text) {
                                control.options[idx].selected = true;
                                break;
                            }
                        }
                    }
                    else {
                        control.selectedIndex = 0;
                    }
                    break;
                case 'List':
                    control.selectedIndex = 0;
                    break;
            }
        }
    } 
}

function blankContactSuccess(newcontact) {
    currentContact = newcontact;

    saveContact();
}

function blankContactError(error)
{
    SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to define a new contact', 'Page Message');
}

function saveContact() {
    if (validateform('scontacts') == false) { return false; }

    if (currentContact == null) {
        var supplierid = $get(hiddenSupplierId).value;
        Spend_Management.svcSuppliers.getBlankContact(supplierid, blankContactSuccess, blankContactError);
        return;
    }

    var tmpContact = currentContact[0];
    var path = getSContactsPath();
    var name = $get(path + "txtcontactname");
    if (name != null) {
        tmpContact.Name = name.value;
    }
    var position = $get(path + "txtposition");
    if (position != null) {
        tmpContact.Position = position.value;
    }
    var email = $get(path + "txtemail");
    if (email != null) {
        tmpContact.Email = email.value;
    }
    var mobile = $get(path + "txtmobile");
    if (mobile != null) {
        tmpContact.Mobile = mobile.value;
    }
    var comments = $get(path + "txtcomments");
    if (comments != null) {
        tmpContact.Comments = comments.value;
    }
    var maincontact = $get(path + "chkmaincontact");
    if (maincontact != null) {
        tmpContact.MainContact = maincontact.checked;
    }
    var baddresstitle = $get(path + "txtbaddresstitle");
    if (baddresstitle != null) {
        tmpContact.BusinessAddress.AddressTitle = baddresstitle.value;
    }
    var baddr1 = $get(path + "txtbaddress1");
    if (baddr1 != null) {
        tmpContact.BusinessAddress.AddressLine1 = baddr1.value;
    }
    var baddr2 = $get(path + "txtbaddress2");
    if (baddr2 != null) {
        tmpContact.BusinessAddress.AddressLine2 = baddr2.value;
    }
    var btown = $get(path + "txtbtown");
    if (btown != null) {
        tmpContact.BusinessAddress.Town = btown.value;
    }
    var bcounty = $get(path + "txtbcounty");
    if (bcounty != null) {
        tmpContact.BusinessAddress.County = bcounty.value;
    }
    var bpcode = $get(path + "txtbpcode");
    if (bpcode != null) {
        tmpContact.BusinessAddress.PostCode = bpcode.value;
    }
    var bcountry = $get(path + "lstbcountry");
    if (bcountry != null) {
        tmpContact.BusinessAddress.CountryId = bcountry.options[bcountry.selectedIndex].value;
    }
    var bphone = $get(path + "txtbswitchboard");
    if (bphone != null) {
        tmpContact.BusinessAddress.Switchboard = bphone.value;
    }
    var bfax = $get(path + "txtbfax");
    if (bfax != null) {
        tmpContact.BusinessAddress.Fax = bfax.value;
    }

    var paddresstitle = $get(path + "txtpaddresstitle");
    if (paddresstitle != null) {
        tmpContact.HomeAddress.AddressTitle = paddresstitle.value;
    }
    var paddr1 = $get(path + "txtpaddress1");
    if (paddr1 != null) {
        tmpContact.HomeAddress.AddressLine1 = paddr1.value;
    }
    var paddr2 = $get(path + "txtpaddress2");
    if (paddr2 != null) {
        tmpContact.HomeAddress.AddressLine2 = paddr2.value;
    }
    var ptown = $get(path + "txtptown");
    if (ptown != null) {
        tmpContact.HomeAddress.Town = ptown.value;
    }
    var pcounty = $get(path + "txtpcounty");
    if (pcounty != null) {
        tmpContact.HomeAddress.County = pcounty.value;
    }
    var ppcode = $get(path + "txtppcode");
    if (ppcode != null) {
        tmpContact.HomeAddress.PostCode = ppcode.value;
    }
    var pcountry = $get(path + "lstpcountry");
    if (pcountry != null) {
        tmpContact.HomeAddress.CountryId = pcountry.options[pcountry.selectedIndex].value;
    }
    var pphone = $get(path + "txtpswitchboard");
    if (pphone != null) {
        tmpContact.HomeAddress.Switchboard = pphone.value;
    }
    var pfax = $get(path + "txtpfax");
    if (pfax != null) {
        tmpContact.HomeAddress.Fax = pfax.value;
    }
    var userdefined = getItemsFromPanel('scontacts');

    Spend_Management.svcSuppliers.updateSupplierContact(tmpContact, userdefined, updateContactSuccess, updateContactError);
}

function updateContactError(error)
{
    SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to update the contact.', 'Page Message');
}

function updateContactSuccess(contact)
{
    SEL.Grid.refreshGrid('contactgrid', SEL.Grid.getCurrentPageNum('contactgrid'));
    $f(contactPanel).hide();
}

function cancelContact() {
    clearContact();
    $find(contactPanel).hide();
}

function AddContact() {
    var hiddenId = $get(hiddenSupplierId);
    var supplierId = hiddenId.value;
    var contactid = $get(hiddenContactId);
    contactid.value = "0";
    clearContact();

    $find(contactPanel).show();
}


function deleteSupplier(supplierid) {
    if (confirm('Click OK to confirm deletion of record and associated data')) {
        Spend_Management.svcSuppliers.deleteSupplier(supplierid, supplierDeleteComplete, supplierDeleteError);
    }
    return;
}

function supplierDeleteComplete(data) {
    var supplierid = data[0];
    var retCode = data[1];

    switch (retCode) {
        case 1:
            SEL.MasterPopup.ShowMasterPopup('Cannot delete supplier as it is currently assigned to one or more contracts', 'Message from ' + moduleNameHTML);
            break;
        case 2:
            SEL.MasterPopup.ShowMasterPopup('Cannot delete supplier as it is currently assigned to one or more not started, in progress or postponed tasks', 'Message from ' + moduleNameHTML);
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup('Cannot delete supplier as it is currently assigned to one or more GreenLights or user defined fields', 'Message from ' + moduleNameHTML);
            break;
        default:
            var supplierGridId = 'suppliergrid';

            var table = $get('tbl_' + supplierGridId);
            for (var i = 0; i < table.rows.length; i++) {
                if (table.rows[i].id == 'tbl_' + supplierGridId + '_' + supplierid) {
                    table.deleteRow(i);
                    break;
                }
            }
    }
    return;
}

function supplierDeleteError(error) {
    SEL.MasterPopup.ShowMasterPopup('A problem occurred trying to delete the supplier.', 'Page Message');
}

function ShowAttachments(supplierId) {
    window.location.href = '../Attachments.aspx?attarea=1&ref=' + supplierId;
}

function openAttachment() {
    var lst = document.getElementById(attlst);
    if (lst != null) {
        var attId = lst.options[lst.selectedIndex].value;

        window.open('../ViewAttachment.aspx?id=' + attId);
    }
}

function showDuplicateErrorMessage() {
    SEL.MasterPopup.ShowMasterPopup('A record already exists with the name specified.', 'Save Error');
}

function setView() {
    switch (activeView) {
        case 0:
            changePage('SupplierDetails');
            break;
        case 1:
            changePage('ContactDetails');
            break;
        case 2:
            changePage('SupplierContracts');
        default:
            break;
    }
}

function checkUDF(udfCount, udfTabId) {
    if (udfCount == 0) {
        hideAjaxTab(udfTabId);
    }
    else {
        showAjaxTab(udfTabId);
    }
}

function refreshAdditionalDetailsUDFs(ddlSupCat) {
    var ddlSupCat = document.getElementById(ddlSupCat);

    document.getElementById(hdnSupCatID).value = ddlSupCat[ddlSupCat.selectedIndex].value;

    __doPostBack(updatePanelID, '');
}

function launchSupplierGridURL(webUrl) {
    if (webUrl !== '') {
        // check format of link and make sure not linking to our domains
        var localRegex = /sel\-expenses\.com|sel\-framework\.com/i;
        var linkFormatRegex = /^(https?:\/\/)?(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?[\w\-\.\/\?\%\&amp\;\=]*/i;
        var linkProtocol = /^(https?:\/\/)/i;

        // the link should not contain our domain - currently set via account.hostname in udf constructor
        // the link should match one of the standard http/https/ftp/ftps/mailto formats to be valid
        if (localRegex.test(webUrl) || linkFormatRegex.test(webUrl) === false) {
            SEL.MasterPopup.ShowMasterPopup("The link you attempted to visit is either not valid, or disallowed", "Invalid Link");
            return;
        }

        // check they are aware of risk of launching link
        if (confirm("This will open a new browser window for the web address listed in the Web Address column.\n\nWould you like to continue to the linked resource?")) {
            if (linkProtocol.test(webUrl)) {
                // launch in new window
                window.open(webUrl, "_blank");
            }
            else {
                window.open('http://' + webUrl, "_blank");
            }
        }
    }
    else {
        SEL.MasterPopup.ShowMasterPopup("There is no website link for this supplier", "Invalid Link");
    }
}

function launchSupplierContactGridEmail(oLink) {
    if (oLink !== null && oLink.parentNode !== null && oLink.parentNode.parentNode !== null && oLink.parentNode.parentNode.getElementsByTagName('td')[6] !== null) {
        var emailURL = oLink.parentNode.parentNode.getElementsByTagName('td')[6].innerHTML;

        if (emailURL !== '') {
            // check format of link and make sure not linking to our domains
            var localRegex = /sel\-expenses\.com|sel\-framework\.com/i;
            var linkFormatRegex = /^(mailto:)?[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/i;
            var linkProtocol = /^(mailto:)/i;

            // the link should not contain our domain - currently set via account.hostname in udf constructor
            // the link should match one of the standard http/https/ftp/ftps/mailto formats to be valid
            if (localRegex.test(emailURL) || linkFormatRegex.test(emailURL) === false) {
                SEL.MasterPopup.ShowMasterPopup("The email link you attempted to use is either not valid, or disallowed", "Invalid Email");
                return false;
            }

            // check they are aware of risk of launching link
            if (confirm("This will attempt to open your default email client so that you may construct an email to this address.\n\nWould you like to continue?")) {
                if (linkProtocol.test(emailURL)) {
                    // launch in new window
                    window.open(emailURL, "_blank");
                }
                else {
                    window.open('mailto:' + emailURL, "_blank");
                }
            }
        }
    }
}

function launchSupplierURL(textBoxID) {
    if (textBoxID !== undefined && textBoxID !== null) {
        // check format of link and make sure not linking to our domains
        var localRegex = /sel\-expenses\.com|sel\-framework\.com/i;
        var linkFormatRegex = /^(https?:\/\/)?(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?[\w\-\.\/\?\%\&amp\;\=]*/i;
        var linkProtocol = /^(https?:\/\/)/i;

        var oTextBox = document.getElementById(textBoxID);
        if (oTextBox !== null && oTextBox.value !== '') {
            // the link should not contain our domain - currently set via account.hostname in udf constructor
            // the link should match one of the standard http/https/ftp/ftps/mailto formats to be valid
            if (localRegex.test(oTextBox.value) || linkFormatRegex.test(oTextBox.value) === false) {
                SEL.MasterPopup.ShowMasterPopup("The link you attempted to visit is either not valid, or disallowed", "Invalid Link");
                return false;
            }

            // check they are aware of risk of launching link
            if (confirm("The link you are about to open is not part of this application and can not be controlled by this software. Please be sure the link is genuine before opening it.\n\nWould you like to continue to the linked resource?")) {
                if (linkProtocol.test(oTextBox.value)) {
                    // launch in new window
                    window.open(oTextBox.value, "_blank");
                }
                else {
                    window.open('http://' + oTextBox.value, "_blank");
                }

            }
        }
    }
}

function launchSupplierEmail(textBoxID) {
    if (textBoxID !== undefined && textBoxID !== null) {
        // check format of link and make sure not linking to our domains
        var localRegex = /sel\-expenses\.com|sel\-framework\.com/i;
        var linkFormatRegex = /^(mailto:)?[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/i;
        var linkProtocol = /^(mailto:)/i;

        var oTextBox = document.getElementById(textBoxID);
        if (oTextBox !== null && oTextBox.value !== '') {
            // the link should not contain our domain - currently set via account.hostname in udf constructor
            // the link should match one of the standard http/https/ftp/ftps/mailto formats to be valid
            if (localRegex.test(oTextBox.value) || linkFormatRegex.test(oTextBox.value) === false) {
                SEL.MasterPopup.ShowMasterPopup("The email link you attempted to use is either not valid, or disallowed", "Invalid Email");
                return false;
            }

            // check they are aware of risk of launching link
            if (confirm("This will attempt to open your default email client so that you may construct an email to this address.\n\nWould you like to continue?")) {
                if (linkProtocol.test(oTextBox.value)) {
                    // launch in new window
                    window.open(oTextBox.value, "_blank");
                }
                else {
                    window.open('mailto:' + oTextBox.value, "_blank");
                }

            }
        }
    }
}   