CREATE PROCEDURE [dbo].[GetInterfaceLogItemsToEmail] 

AS
BEGIN
      SELECT LogItemID, LogItemType, TransferType, NHSTrustID, [Filename], EmailSent, LogItemBody, CreatedOn, ModifiedOn FROM InterfaceLog WHERE EmailSent = 0
END
