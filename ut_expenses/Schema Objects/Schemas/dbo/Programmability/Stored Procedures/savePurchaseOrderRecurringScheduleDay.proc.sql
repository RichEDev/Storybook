


CREATE PROCEDURE [dbo].[savePurchaseOrderRecurringScheduleDay]

@purchaseOrderID int,
@day tinyint
AS 
	BEGIN
		INSERT INTO purchaseOrderRecurringScheduleDays (purchaseOrderID, [day]) VALUES (@purchaseOrderId, @day);
	END
RETURN @purchaseOrderID;




