



CREATE  PROCEDURE [dbo].[saveInvoiceLineItem]
            @invoiceID int,
            @lineItemID int,
            @productID int,
            @quantity decimal(18,2),
            @salesTaxID int,
            @unitID int,
            @unitPrice money,
			@employeeID int
AS 
	IF (@lineItemID = 0)
		BEGIN
			INSERT INTO invoiceLineItems (invoiceID, productID, unitOfMeasureID, salesTaxID, unitPrice, quantity) VALUES (@invoiceID, @productID, @unitID, @salesTaxID, @unitPrice, @quantity);
			UPDATE invoices SET modifiedOn=getdate(), modifiedby=@employeeID WHERE invoiceID=@invoiceID;
			SET @lineItemID = scope_identity();
		END
	ELSE
		BEGIN
			UPDATE invoiceLineItems SET invoiceID=@invoiceID, productID=@productID, unitOfMeasureID=@unitID, salesTaxID=@salesTaxID, unitPrice=@unitPrice, quantity=@quantity WHERE invoiceLineItemID=@lineItemID
			UPDATE invoices SET modifiedOn=getdate(), modifiedby=@employeeID WHERE invoiceID=@invoiceID;
		END
RETURN @lineItemID;


