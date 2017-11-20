



CREATE PROCEDURE dbo.deletePurchaseOrderRecurringScheduleDaysMonths

@purchaseOrderID int
AS 
	BEGIN
		DELETE FROM purchaseOrderRecurringScheduleDays WHERE purchaseOrderID = @purchaseOrderID;
		DELETE FROM purchaseOrderRecurringScheduleMonths WHERE purchaseOrderID = @purchaseOrderID;
	END
RETURN @purchaseOrderID;





