


CREATE PROCEDURE [dbo].[savePurchaseOrderRecurringScheduleMonth]

@purchaseOrderID int,
@month tinyint
AS 
	BEGIN
		INSERT INTO purchaseOrderRecurringScheduleMonths (purchaseOrderID, [month]) VALUES (@purchaseOrderID, @month);
	END
RETURN @purchaseOrderID;




