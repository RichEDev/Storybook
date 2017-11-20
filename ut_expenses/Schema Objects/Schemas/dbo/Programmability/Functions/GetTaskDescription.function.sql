CREATE FUNCTION dbo.GetTaskDescription(@taskId int)
RETURNS nvarchar(500)
AS
BEGIN
DECLARE @description nvarchar(500);
DECLARE @regardingArea int;
DECLARE @regardingId int;
SET @description = '';

SELECT @regardingArea = regardingArea, @regardingId = regardingId from tasks where taskId = @taskId;

IF @regardingArea = 1 -- contract details
BEGIN
	SELECT @description = contractDescription FROM contract_details WHERE contractid = @regardingId;
END

IF @regardingArea = 4 -- supplier details
BEGIN
	SELECT @description = supplierName FROM supplier_details WHERE supplierid = @regardingId;
END

IF @regardingArea = 13 -- employees details
BEGIN
	SELECT @description = (firstname + ' ' + surname) FROM employees WHERE employeeid = @regardingId;
END

IF @regardingArea = 3 -- product details
BEGIN
	SELECT @description = productName from productDetails where productId = @regardingId;
END

IF @regardingArea = 9 -- invoice details
BEGIN
	SELECT @description = invoiceNumber from invoices where invoiceID = @regardingId;
END

IF @regardingArea = 2 -- contract product details
BEGIN
	SELECT @description = productName from contract_ProductDetails 
	inner join productDetails on productdetails.productId = contract_productDetails.productId
	where productdetails.productId = @regardingId;
END

IF @regardingArea = 10 -- invoice forecasts
BEGIN
	SELECT @description = Convert(nvarchar(25), [forecastAmount]) from contract_forecastdetails where [contractForecastId] = @regardingId;
END

RETURN @description;

END
