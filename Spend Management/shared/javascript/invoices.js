
function calculateTotalProductPrice(evt, rowIndex, aspElement)
{
    var quantity;
    var unitPrice;
    var salesTax;

    if (aspElement == true)
    {
        unitPrice = document.getElementById('ctl00_contentmain_product_unit_price_' + rowIndex).value;
        quantity = document.getElementById('ctl00_contentmain_product_quantity_' + rowIndex).value;
        salesTax = document.getElementById('ctl00_contentmain_product_sales_tax_' + rowIndex).value;
    }
    else
    {
        unitPrice = document.getElementById('product_unit_price_' + rowIndex).value;
        quantity = document.getElementById('product_quantity_' + rowIndex).value;
        salesTax = document.getElementById('product_sales_tax_' + rowIndex).value;
    }

    if (isNaN(unitPrice) == true || isNaN(quantity) == true || isNaN(salesTax) == true || quantity.length == 0 || unitPrice.length == 0 || salesTax.length == 0)
    {

        var net;
        var tax;
        var total;
        if (aspElement == true)
        {
            net = document.getElementById('ctl00_contentmain_product_net_td_' + rowIndex);
            tax = document.getElementById('ctl00_contentmain_product_tax_td_' + rowIndex);
            total = document.getElementById('ctl00_contentmain_product_total_td_' + rowIndex);
        }
        else
        {
            net = document.getElementById('product_net_td_' + rowIndex);
            tax = document.getElementById('product_tax_td_' + rowIndex);
            total = document.getElementById('product_total_td_' + rowIndex);
        }

        net.innerHTML = "0.00";
        tax.innerHTML = "0.00";
        total.innerHTML = "0.00";
    }
    else
    {
        Spend_Management.svcPurchaseOrders.GetProductTotalWithTaxByUnitCostTimesQuantity(rowIndex, quantity, unitPrice, salesTax, aspElement, calculateTotalProductPriceComplete, errorMsg);
    }
    return true;
}

function calculateTotalProductPriceComplete(data)
{
    var rowIndex = data[0];
    var rowNet = data[1];
    var rowTax = data[2];
    var rowTotal = data[3];
    var net;
    var tax;
    var total;

    if (data[4] == "True")
    {
        net = document.getElementById('ctl00_contentmain_product_net_td_' + rowIndex);
        tax = document.getElementById('ctl00_contentmain_product_tax_td_' + rowIndex);
        total = document.getElementById('ctl00_contentmain_product_total_td_' + rowIndex);
    }
    else
    {
        net = document.getElementById('product_net_td_' + rowIndex);
        tax = document.getElementById('product_tax_td_' + rowIndex);
        total = document.getElementById('product_total_td_' + rowIndex);
    }

    net.innerHTML = FormatCurrency(rowNet);
    tax.innerHTML = FormatCurrency(rowTax);
    total.innerHTML = FormatCurrency(rowTotal);



    var inputs = document.getElementsByTagName("input");
    var totalInput = document.getElementById(totalID);
    var currentTotals = document.getElementById(lineItemsTableID).tBodies[0];

    var newTotal = 0.00;
    var i;
    for (i = 1; i < currentTotals.rows.length; i++)
    {
        var currentRowTotal = currentTotals.rows[i].cells[9].innerHTML;

        currentRowTotal = currentRowTotal.replace(",", "");

        if (isNaN(currentRowTotal) == false)
        {
            newTotal = new Number(new Number(newTotal) + new Number(currentRowTotal)).toFixed(2);
        }
    }

    totalInput.value = FormatCurrency(newTotal);

    // calc total for this invoice

    return;
}

function calculateOverallTotal()
{
    var inputs = document.getElementsByTagName("input");
    var totalInput = document.getElementById(totalID);
    var currentTotals = document.getElementById(lineItemsTableID).tBodies[0];

    var newTotal = 0.00;
    var i;
    for (i = 1; i < currentTotals.rows.length; i++)
    {
        var currentRowTotal = currentTotals.rows[i].cells[9].innerHTML;

        currentRowTotal = currentRowTotal.replace(",", "");

        if (isNaN(currentRowTotal) == false)
        {
            newTotal = new Number(new Number(newTotal) + new Number(currentRowTotal)).toFixed(2);
        }
    }

    totalInput.value = FormatCurrency(newTotal);
    return;
}


function editCostCodes(itemId)
{
    ccbShowCostCentreBreakdown(itemId);
    openCostCodesModal();
}

function openCostCodesModal()
{
    $find(productCostCodePanel).show();
    return;
}

function closeCostCodesModal()
{
    $find(productCostCodePanel).hide();
    return;
}

function updateCostCodes()
{
    var i = ccbSaveBreakdown();
    if (i == true) { closeCostCodesModal(); }
}

function checkForPurchaseOrderMatch()
{
    var poMatch = document.getElementById("purchaseOrderNumberSpan").getElementsByTagName('input')[0].value;
    if (poMatch != "")
    {
        Spend_Management.svcPurchaseOrders.GetPurchaseOrderForInvoiceByPurchaseOrderNumber(poMatch, checkForPurchaseOrderMatchComplete, errorMsg);
    }
    return;
}
function checkForPurchaseOrderMatchComplete(po)
{
    document.getElementById('supplierNameSpan').getElementsByTagName('input')[1].value = po.SupplierName;
    setTimeout("document.getElementById('supplierNameSpan').getElementsByTagName('input')[1].onkeyup();", 500);
    
    var line;
    var costCentres;
    var department;
    var projectCode;
    var lineItemsTable = document.getElementById(lineItemsTableID).tBodies[0];

    for (var i = 0; i < po.Products.length; i++)
    {
        line = po.Products[i];

        addLineItem(line.Product.ProductName, line.Product.ProductCode, line.Product.ProductId, line.UnitOfMeasure.UnitId, line.ProductUnitPrice, line.ProductQuantity, costCentres, department, projectCode, true);
    }
    
    return;
}

function populateUoms(selectBox)
{
    var option;

    for (var i = 0; i < uomValues.length; i++)
    {
        option = document.createElement("option");
        option.text = uomValues[i][1];
        option.value = uomValues[i][0];
        selectBox.options[i] = option;
    }

    return selectBox;
}

function populateSelectTaxes(selectBox)
{
    var option;

    for (var i = 0; i < salesTaxValues.length; i++)
    {
        option = document.createElement("option");
        option.text = salesTaxValues[i][1];
        option.value = salesTaxValues[i][0];
        selectBox.options[i] = option;
    }

    return selectBox;
}

function deleteAllRows(tblID)
{
    var tbl = document.getElementById(tblID).tBodies[0];
    for (var i = 0; i < tbl.rows.length; i++)
    {
        tbl.deleteRow(i);
    }
    return true;
}

function deleteRow(ctrl)
{
    var tbl_products = document.getElementById(lineItemsTableID).tBodies[0];
    if (tbl_products.rows.length > 1)
    {
        for (var i = 1; i < tbl_products.rows.length; i++)
        {
            if (tbl_products.rows[i].childNodes[0].childNodes[0].id == ctrl.id)
            {
                tbl_products.deleteRow(i);
                calculateOverallTotal();
                break;
            }
        }
    } else
    {
        alert("Cannot delete the last row.");
    }
}




function addLineItem(product, productCode, productID, unitOfMeasure, unitPrice, quantity, costCentres, departmentID, invoiceLineItemId, checkForPrevious)
{
    var tbl_products = document.getElementById(lineItemsTableID).tBodies[0];
    
    if (checkForPrevious == true)
    {
        for (var i = 0; i < tbl_products.rows.length; i++)
        {
            if (tbl_products.rows[i].cells[5].childNodes[1] != undefined && tbl_products.rows[i].cells[5].childNodes[1].value == productID)
            {
                return;
            }
        }
    }

    var newRow = tbl_products.insertRow(-1);
    var newCell;
    var textBox;
    var selectBox;
    var ctrl;
    var hiddenField;
    var option;
    var ddl;

    newRow.ID = lineItemIndexID;
    newRow.className = "bgtr";

    newCell = newRow.insertCell(0);
    newCell.innerHTML = "<img id=\"" + lineItemIndexID + "\" src=\"/shared/images/icons/delete2.png\" onclick=\"deleteRow(this);\" style=\"cursor: hand;\" />";

    // supplier box
    
    // product name
    newCell = newRow.insertCell(1);
    textBox = document.createElement("input");
    textBox.id = "product_textbox_" + lineItemIndexID;
    textBox.type = "text";
    if (product != null) { textBox.value = product; }
    newCell.appendChild(textBox);

    nameValidationSpan = document.createElement("span");
    nameValidationSpan.id = "product_name_validationspan_" + lineItemIndexID;
    nameValidationSpan.className = "inputvalidatorfield";
    nameValidationSpan.style.color = "red";
    nameValidationSpan.style.visibility = "hidden";
    nameValidationSpan.innerHTML = "*";
    newCell.appendChild(nameValidationSpan);

    inputValidatorSpanId = nameValidationSpan.id;
    controlToValidate = textBox.id;
    errorMessage = "Product Name is a required field";
    validationGroup = "";
    addMandatoryValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    // product code
    newCell = newRow.insertCell(2);
    textBox = document.createElement("input");
    textBox.type = "text";
    if (productCode != null) { textBox.value = productCode; }
    newCell.appendChild(textBox);

    // unit of measure
    newCell = newRow.insertCell(3);
    selectBox = document.createElement("select");

    populateUoms(selectBox);

    if (unitOfMeasure != null)
    {
        for (var i = 0; i < selectBox.options.length; i++)
        {
            if (selectBox.options[i].value == unitOfMeasure)
            {
                selectBox.options[i].selected = true;
                break;
            }
        }
    }

    newCell.appendChild(selectBox);
    
    // unit price
    newCell = newRow.insertCell(4);
    textBox = document.createElement("input");
    textBox.type = "text";
    textBox.size = "7";
    textBox.id = "product_unit_price_" + lineItemIndexID;
    if (unitPrice != null) { textBox.value = unitPrice; }
    newCell.appendChild(textBox);

    validatorSpan = document.createElement("span");
    validatorSpan.id = "product_unit_price_validationspan_" + lineItemIndexID;
    validatorSpan.className = "inputvalidatorfield";
    validatorSpan.style.color = "red";
    validatorSpan.style.visibility = "hidden";
    validatorSpan.innerHTML = "*";
    newCell.appendChild(validatorSpan);

    inputValidatorSpanId = validatorSpan.id;
    controlToValidate = textBox.id;
    errorMessage = "Product Unit Price must be a decimal value";
    validationGroup = "";
    addDecimalTypeValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    validatorSpanTwo = document.createElement("span");
    validatorSpanTwo.id = "product_unit_price_validationspan2_" + lineItemIndexID;
    validatorSpanTwo.className = "inputvalidatorfield";
    validatorSpanTwo.style.color = "red";
    validatorSpanTwo.style.visibility = "hidden";
    validatorSpanTwo.innerHTML = "*";
    newCell.appendChild(validatorSpanTwo);

    inputValidatorSpanId = validatorSpanTwo.id;
    errorMessage = "Product Unit Price is a required field";
    addMandatoryValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);
    
    // quanitity
    newCell = newRow.insertCell(5);
    ctrl = document.createElement("input");
    ctrl.type = "text";
    ctrl.size = "5";
    ctrl.id = "product_quantity_" + lineItemIndexID;
    if (quantity != null) { ctrl.value = quantity; }
    newCell.appendChild(ctrl);

    // SET UNIT PRICE AND UNIT QUANTITY TO TRIGGER AN ONCHANGE EVENT TO CALCULATE THE TOTAL
    if (browserName() == "IE")
    { // IE SPECIFIC
        textBox.onchange = new Function("calculateTotalProductPrice(event, '" + lineItemIndexID + "', false);");
        ctrl.onchange = new Function("calculateTotalProductPrice(event, '" + lineItemIndexID + "', false);");
    } else
    { // DOM COMPLIANT BROWSERS
        textBox.setAttribute("onChange", "calculateTotalProductPrice(event, '" + lineItemIndexID + "', false);");
        ctrl.setAttribute("onChange", "calculateTotalProductPrice(event, '" + lineItemIndexID + "', false);");
    }

    ctrl = document.createElement("input");
    ctrl.type = "hidden";
    ctrl.id = "product_purchase_order_product_id_" + lineItemIndexID;
    if (productID != null)
    {
        ctrl.value = productID;
    }
    else
    {
        ctrl.value = 0;
    }
    newCell.appendChild(ctrl);

    ctrl = document.createElement("input");
    ctrl.type = "hidden";
    //ctrl.id = "product_purchase_order_product_department_" + lineItemIndexID;
    ctrl.id = "product_purchase_order_product_rowid_" + lineItemIndexID;
    ctrl.value = lineItemIndexID;
    newCell.appendChild(ctrl);

    ctrl = document.createElement("input");
    ctrl.type = "hidden";
    //ctrl.id = "product_purchase_order_product_project_code_" + lineItemIndexID;
    ctrl.id = "product_purchase_order_product_invoicelineitemid_" + lineItemIndexID;
    if (invoiceLineItemId != null)
    {
        ctrl.value = invoiceLineItemId;
    }
    else
    {
        ctrl.value = 0;
    }
    newCell.appendChild(ctrl);

    ctrl = document.createElement("input");
    ctrl.type = "hidden";
    ctrl.id = "product_purchase_order_product_costcode_" + lineItemIndexID;
    ctrl.value = 0;
    newCell.appendChild(ctrl);

    quantityValidatorSpan = document.createElement("span");
    quantityValidatorSpan.id = "product_quantity_validationspan_" + lineItemIndexID;
    quantityValidatorSpan.className = "inputvalidatorfield";
    quantityValidatorSpan.style.color = "red";
    quantityValidatorSpan.style.visibility = "hidden";
    quantityValidatorSpan.innerHTML = "*";
    newCell.appendChild(quantityValidatorSpan);

    inputValidatorSpanId = quantityValidatorSpan.id;
    errorMessage = "Product Quantity must be a decimal value";
    validationGroup = "k";
    addDecimalTypeValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);

    quantityValidatorSpanTwo = document.createElement("span");
    quantityValidatorSpanTwo.id = "product_quantity_validationspan2_" + lineItemIndexID;
    quantityValidatorSpanTwo.className = "inputvalidatorfield";
    quantityValidatorSpanTwo.style.color = "red";
    quantityValidatorSpanTwo.style.visibility = "hidden";
    quantityValidatorSpanTwo.innerHTML = "*";
    newCell.appendChild(quantityValidatorSpanTwo);

    inputValidatorSpanId = quantityValidatorSpanTwo.id;
    errorMessage = "Product Quantity is a required field";
    addMandatoryValidator(inputValidatorSpanId, controlToValidate, errorMessage, validationGroup);
    
    // sales tax
    newCell = newRow.insertCell(6);
    ctrl = document.createElement("select");
    ctrl.id = "product_sales_tax_" + lineItemIndexID;
    populateSelectTaxes(ctrl);
    newCell.appendChild(ctrl);

    // SET SALES TAX TO TRIGGER AN ONCHANGE EVENT TO CALCULATE THE TOTAL
    if (browserName() == "IE")
    { // IE SPECIFIC
        ctrl.onchange = new Function("calculateTotalProductPrice(event, '" + lineItemIndexID + "', false);");
    } else
    { // DOM COMPLIANT BROWSERS
        ctrl.setAttribute("onChange", "calculateTotalProductPrice(event, '" + lineItemIndexID + "', false);");
    }

    newCell = newRow.insertCell(7);
    newCell.align = "center";
    var txtnode = document.createTextNode(" ");
    newCell.id = "product_net_td_" + lineItemIndexID;
    newCell.appendChild(txtnode);

    newCell = newRow.insertCell(8);
    newCell.align = "center";
    var txtnode = document.createTextNode(" ");
    newCell.id = "product_tax_td_" + lineItemIndexID;
    newCell.appendChild(txtnode);

    newCell = newRow.insertCell(9);
    newCell.align = "center";
    var txtnode = document.createTextNode(" ");
    newCell.id = "product_total_td_" + lineItemIndexID;
    newCell.appendChild(txtnode);

    if (quantity != null && unitPrice != null) { calculateTotalProductPrice(null, lineItemIndexID, false); }
    
    newCell = newRow.insertCell(10);
    newCell.align = "center";
    newCell.innerHTML = "<a href=\"javascript:ccbOpen(" + lineItemIndexID + ");\" alt=\"Show Split\">Split</a>";

    ccbAddItemArray(lineItemIndexID);
    lineItemIndexID++;
}

function GetInvoiceLineItems()
{
    var tbl_products = document.getElementById(lineItemsTableID).tBodies[0];
    var products = new Array();

    // i = 1 to skip the header row.
    var tmpArr = ccbItemArray;
    
    for (var i = 1; i < tbl_products.rows.length; i++)
    {
        if (
            tbl_products.rows[i].cells[1].childNodes[0] != undefined && tbl_products.rows[i].cells[1].childNodes[0].value != "" &&
            tbl_products.rows[i].cells[2].childNodes[0] != undefined &&
            tbl_products.rows[i].cells[3].childNodes[0] != undefined && tbl_products.rows[i].cells[3].childNodes[0].value != "" &&
            tbl_products.rows[i].cells[4].childNodes[0] != undefined && tbl_products.rows[i].cells[4].childNodes[0].value != "" &&
            tbl_products.rows[i].cells[5].childNodes[0] != undefined && tbl_products.rows[i].cells[5].childNodes[0].value != "" &&
            tbl_products.rows[i].cells[5].childNodes[1] != undefined && tbl_products.rows[i].cells[5].childNodes[1].value != ""
        )
        {
            products.push(new Array());
            products[products.length - 1].push(tbl_products.rows[i].cells[1].childNodes[0].value);  // product
            products[products.length - 1].push(tbl_products.rows[i].cells[2].childNodes[0].value);  // prod code
            products[products.length - 1].push(tbl_products.rows[i].cells[3].childNodes[0].value);  // uom // dd
            products[products.length - 1].push(tbl_products.rows[i].cells[4].childNodes[0].value);  // price
            products[products.length - 1].push(tbl_products.rows[i].cells[5].childNodes[0].value);  // quantity
            products[products.length - 1].push(tbl_products.rows[i].cells[5].childNodes[1].value);  // productID
            products[products.length - 1].push(tbl_products.rows[i].cells[5].childNodes[3].value);  // invoiceLineItemID
            products[products.length - 1].push(tbl_products.rows[i].cells[6].childNodes[0].value);  // salesTax

            // if the invoicelineid isnt set take the rowid
            if (tbl_products.rows[i].cells[5].childNodes[3].value == 0)
            {
                products[products.length - 1].push(ccbGetBreakdownRows(tbl_products.rows[i].cells[5].childNodes[2].value)); // ccb array 6, rowid
            }
            else
            {
                products[products.length - 1].push(ccbGetBreakdownRows(tbl_products.rows[i].cells[5].childNodes[3].value)); // ccb array 6, invoicelineid
            }
        }
    }

    return products;
}

function DeleteInvoice(invoiceID)
{
    if(confirm("Are you certain you wish to delete this invoice?"))
    {
        Spend_Management.svcInvoices.ArchiveInvoice(invoiceID, DeleteInvoiceComplete, errorMsg);
    }
    return;
}
function DeleteInvoiceComplete(data)
{
    return;
}



function ajaxError(data)
{
    if (data["_message"] != undefined)
    {
        SEL.MasterPopup.ShowMasterPopup(data["_message"],"Javascript Error");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(data, "Javascript Error");
    }
    return;
}






/*
** from aeInvoice.aspx
*/
function SaveInvoice()
{
    if (validateform(null) == false) { return; }

    var purchaseOrderNumber = document.getElementById('purchaseOrderNumberSpan').getElementsByTagName('input')[0].value; // get the PO number, starting at the supplier
    var supplier = document.getElementById('supplierNameSpan').getElementsByTagName('input')[0].value;
    var invoiceNumber = document.getElementById(invoiceNumberID).value;
    var receivedDate = document.getElementById(receivedDateID).value;
    var dueDate = document.getElementById(dueDateID).value;
    var invoiceStatus = document.getElementById(invoiceStatusID).value;
    var paidDate = document.getElementById(paidDateID).value;
    var coverPeriodEnd = document.getElementById(coverPeriodEndID).value;
    var comments = document.getElementById(commentsID).value;
    var paymentReference = null;

    if (coverPeriodEnd != "") { coverPeriodEnd = coverPeriodEnd.substring(6, 10) + "/" + coverPeriodEnd.substring(3, 5) + "/" + coverPeriodEnd.substring(0, 2); } else { coverPeriodEnd = null; }
    if (paidDate != "") { paidDate = paidDate.substring(6, 10) + "/" + paidDate.substring(3, 5) + "/" + paidDate.substring(0, 2); } else { paidDate = null; }
    if (dueDate != "") { dueDate = dueDate.substring(6, 10) + "/" + dueDate.substring(3, 5) + "/" + dueDate.substring(0, 2); } else { dueDate = null; }
    if (receivedDate != "") { receivedDate = receivedDate.substring(6, 10) + "/" + receivedDate.substring(3, 5) + "/" + receivedDate.substring(0, 2); } else { receivedDate = null; }

    var lineItems = GetInvoiceLineItems();

    if (lineItems.length > 0)
    {
        Spend_Management.svcInvoices.SaveInvoice(invoiceID, purchaseOrderNumber, null, supplier, invoiceNumber, receivedDate, dueDate, invoiceStatus, comments, paidDate, coverPeriodEnd, paymentReference, lineItems, invoiceStateID, SaveInvoiceComplete, errorMsg);
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup("The invoice can not be saved without line items.", "Page Validation Failed");
    }
    return;
}

function SaveInvoiceComplete(data)
{
    if (data > 0)
    {
        invoiceID = data;
    }
    CloseInvoice();
    return;
}

function CloseInvoice()
{
    if (invoiceID == 0)
    {
        window.location = '/shared/invoiceMenu.aspx';
    }
    else
    {
        window.location = '/shared/invoices.aspx?state=' + invoiceStateID;
    }
    return;
}






/*
** Invoice Approvals Page
*/
function SelectInvoice(invoiceID)
{
    activeInvoiceID = invoiceID;
    Spend_Management.svcInvoices.GetInvoiceApprovalDetails(invoiceID, SelectInvoiceComplete, ajaxError);
    return;
}
function SelectInvoiceComplete(cInvoice)
{
    ZeroReadOnlyModalPanel(invoiceModalPanel);

    var historyTable = document.getElementById(invoiceHistory);
    var histRow;
    var histTh;
    var histCell;
    var histCellStyle = "row1";

    // create product table headers
    histRow = historyTable.insertRow(-1);
    histTh = document.createElement('th');
    histTh.innerHTML = 'Date';
    histRow.appendChild(histTh);
    histTh = document.createElement('th');
    histTh.innerHTML = 'Activity';
    histRow.appendChild(histTh);
    histTh = document.createElement('th');
    histTh.innerHTML = 'By User';
    histRow.appendChild(histTh);

    //create history rows
    for (i = 0; i < cInvoice.HistoryItems.length; i++)
    {
        histRow = historyTable.insertRow(-1);
        histCell = histRow.insertCell(0);
        histCell.className = histCellStyle;
        histCell.innerHTML = cInvoice.HistoryItems[i].CreatedOn.format("dd/MM/yyyy HH:mm");
        histCell = histRow.insertCell(1);
        histCell.className = histCellStyle;
        histCell.innerHTML = cInvoice.HistoryItems[i].Comment;
        histCell = histRow.insertCell(2);
        histCell.className = histCellStyle;
        histCell.innerHTML = cInvoice.HistoryItems[i].CreatedByString;

        histCellStyle = (histCellStyle == "row1") ? "row2" : "row1";
    }
    if (cInvoice.HistoryItems.length > 0) { document.getElementById(invoiceHistoryPanel).style.display = "block"; }
    if (cInvoice.HistoryItems.length > 4)
    {
        document.getElementById('historycontainer').style.height = "155px";
        document.getElementById('historycontainer').style.overflowY = "scroll";
        document.getElementById('historycontainer').childNodes[0].style.width = "642px";
    }
    else
    {
        document.getElementById('historycontainer').style.height = ((cInvoice.HistoryItems.length * 30) + 35) + "px";
        document.getElementById('historycontainer').style.overflowY = "";
        document.getElementById('historycontainer').childNodes[0].style.width = "100%";
    }

    var pnlInvoice = document.getElementById(invoiceModalPanel).getElementsByTagName("span");

    pnlInvoice[0].innerHTML = cInvoice.PONumber;
    pnlInvoice[4].innerHTML = cInvoice.Supplier.SupplierName;
    pnlInvoice[8].innerHTML = cInvoice.InvoiceNumber;
    pnlInvoice[12].innerHTML = cInvoice.InvoiceReceivedDate.format("dd/MM/yyyy");
    pnlInvoice[16].innerHTML = Number(cInvoice.TotalInvoiceAmount).toFixed(2);
    pnlInvoice[20].innerHTML = cInvoice.InvoiceDueDate.format("dd/MM/yyyy");
    pnlInvoice[24].innerHTML = cInvoice.InvoiceStatus.StatusDescription;
    pnlInvoice[28].innerHTML = (cInvoice.InvoicePaidDate != null) ? cInvoice.InvoicePaidDate.format("dd/MM/yyyy") : '&nbsp;';
    pnlInvoice[32].innerHTML = (cInvoice.InvoiceCoverPeriod != null) ? cInvoice.InvoiceCoverPeriod.format("dd/MM/yyyy") : '&nbsp;';
    document.getElementById(invoiceModalPanel).getElementsByTagName('textarea')[0].value = cInvoice.Comment;

    var tbl = document.getElementById(invoiceLineItems);
    var newRow;
    var th;
    var newCell;
    var cellStyle = "row1";
    var total = 0;
    var rowNet = 0;
    var rowTax = 0;

    // create product table headers
    newRow = tbl.insertRow(-1);
    th = document.createElement('th');
    th.innerHTML = 'Product Name';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Product Code';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Unit Measure';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Unit Price';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Quantity';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Sales Tax';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'NET';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Tax Amount';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Total';
    newRow.appendChild(th);
    th = document.createElement('th');
    th.innerHTML = 'Cost Centre';
    newRow.appendChild(th);

    //create product rows
    for (i = 0; i < cInvoice.LineItems.length; i++)
    {
        newRow = tbl.insertRow(-1);
        newCell = newRow.insertCell(0);
        newCell.className = cellStyle;
        newCell.innerHTML = cInvoice.LineItems[i].Product.ProductName;
        newCell = newRow.insertCell(1);
        newCell.className = cellStyle;
        newCell.innerHTML = cInvoice.LineItems[i].Product.ProductCode;
        newCell = newRow.insertCell(2);
        newCell.className = cellStyle;
        newCell.innerHTML = cInvoice.LineItems[i].UnitOfMeasure.UnitDescription;
        newCell = newRow.insertCell(3);
        newCell.className = cellStyle;
        newCell.innerHTML = Number(cInvoice.LineItems[i].UnitPrice).toFixed(2);
        newCell = newRow.insertCell(4);
        newCell.className = cellStyle;
        newCell.innerHTML = cInvoice.LineItems[i].Quantity;
        newCell = newRow.insertCell(5);
        newCell.className = cellStyle;
        newCell.innerHTML = cInvoice.LineItems[i].SalesTax.Description;
        newCell = newRow.insertCell(6);
        newCell.className = cellStyle;
        rowNet = cInvoice.LineItems[i].UnitPrice * cInvoice.LineItems[i].Quantity;
        newCell.innerHTML = Number(rowNet).toFixed(2);
        newCell = newRow.insertCell(7);
        newCell.className = cellStyle;
        rowTax = cInvoice.LineItems[i].UnitPrice * cInvoice.LineItems[i].Quantity * cInvoice.LineItems[i].SalesTax.SalesTax / 100;
        newCell.innerHTML = Number(rowTax).toFixed(2);
        newCell = newRow.insertCell(8);
        newCell.className = cellStyle;
        newCell.innerHTML = Number(rowNet + rowTax).toFixed(2);
        total = total + Math.round(rowNet + rowTax * 100) / 100;
        newCell = newRow.insertCell(9);
        newCell.className = cellStyle;
        newCell.style.textAlign = "center";
        newCell.innerHTML = "<a href=\"javascript:ccbOpen(" + cInvoice.LineItems[i].InvoiceLineItemID + ");\" alt=\"Show Split\">Split</a>";

        ccbAddItemArray(cInvoice.LineItems[i].InvoiceLineItemID, cInvoice.LineItems[i].CostCentreBreakdown);

        cellStyle = (cellStyle == "row1") ? "row2" : "row1";
    }

    OpenInvoiceApprovalModal();
    return;
}
function OpenInvoiceApprovalModal()
{
    $find(invoiceModalID).show();
    return;
}
function CloseInvoiceApprovalModal()
{
    $find(invoiceModalID).hide();
    return;
}
function ApproveInvoiceApprovalModal()
{
    Spend_Management.svcInvoices.ApproveInvoice(activeInvoiceID, CloseInvoiceApprovalModal, ajaxError);
    return;
}
function RejectInvoiceApprovalModal()
{
    $find(invoiceModalID).hide();
    $find(rejectedInvoiceModalID).show();
    return;
}
function RejectedInvoice()
{
    var reasonForRejection = document.getElementById(reasonForRejectionArea).value;
    Spend_Management.svcInvoices.RejectInvoice(activeInvoiceID, reasonForRejection, CloseInvoiceApprovalModal, ajaxError);
    return;
}
function BackToInvoiceApprovalModal()
{
    $find(rejectedInvoiceModalID).hide();
    document.getElementById(reasonForRejectionArea).value = "";
    $find(invoiceModalID).show();
    return;
}
function CloseInvoiceApprovalModal()
{
    refreshGrid(approvalsGrid, 1);
    $find(rejectedInvoiceModalID).hide();
    $find(invoiceModalID).hide();
    return;
}



// Generic function
// Clear all fields in a read-only presentation panel (not for forms containing <input>s)
function ZeroReadOnlyModalPanel(panelID)
{
    // clear any <span> tags with "inputs" cells avoiding textarea fields
    var pnlFields = document.getElementById(panelID).getElementsByTagName("span");
    for (i = 0; i < pnlFields.length; i++)
    {
        if (pnlFields[i].className == 'inputs' && pnlFields[i].firstChild != null && pnlFields[i].firstChild.nodeName.toLowerCase() != 'textarea')
        {
            pnlFields[i].innerHTML = '&nbsp;';
        }
    }
    // clear any textareas
    var pnlFields = document.getElementById(panelID).getElementsByTagName('textarea');
    for (i = 0; i < pnlFields.length; i++)
    {
        pnlFields[i].value = '&nbsp;';
    }
    // clear any table rows
    var pnlTable = document.getElementById(panelID).getElementsByTagName('table');
    for (i = 0; i < pnlTable.length; i++)
    {
        while (pnlTable[i].firstChild)
        {
            pnlTable[i].removeChild(pnlTable[i].firstChild);
        }
    }
    // clear the history
    tbl = document.getElementById(invoiceHistory);
    while (tbl.firstChild)
    {
        tbl.removeChild(tbl.firstChild);
    }

    return;
}




function errorMsg(data)
{
    if (data["_message"] != undefined)
    {
        alert(data["_message"]);
    }
    else
    {
        alert(data);
    }
    return;
}
    
