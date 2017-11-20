function validateAndSubmit() {
	if(validateForm('pdetail'))
	{
		$find('<%=cmdExecSave.ClientID %>').click();
	}
	return;
}

function closeModal() {
	$find(modalpanelID).hide();
	return;
}

function editProduct(productid) {
	window.location.href = 'aeproductdetail.aspx?pid=' + productid;
	return;
}



function deleteProduct(productid) {

}