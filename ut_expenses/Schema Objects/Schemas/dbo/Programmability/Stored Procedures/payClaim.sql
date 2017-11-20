CREATE PROCEDURE [dbo].[payClaim] 
 @claimid int,
 @delegateID INT,
 @employeeID INT

AS
BEGIN
 SET NOCOUNT ON;

 declare @oldpaid bit;
 declare @olddatepaid datetime;
 declare @newdatepaid datetime;
 declare @oldPayBeforeValidate bit;
 set @newdatepaid = convert(DateTime, convert(varchar, GETDATE(), 101));
 declare @recordtitle nvarchar(2000);
 select @recordtitle = [name], @oldpaid = paid, @olddatepaid = datepaid, @oldPayBeforeValidate = PayBeforeValidate from claims_base where claimid = @claimid;
 
 if	(@oldPayBeforeValidate = 1)
 BEGIN
 	 update claims_base set approved = 0 where claimid = @claimid;
	 update savedexpenses set paid = 1, DatePaid = convert(DateTime, convert(varchar, GETUTCDATE(), 101)) where claimid = @claimid;
 END
 ELSE
 BEGIN
	 update savedexpenses set paid = 1, DatePaid = convert(DateTime, convert(varchar, GETUTCDATE(), 101)) where claimid = @claimid;
	 update claims_base set approved = 1, paid = 1, datepaid = convert(DateTime, convert(varchar, GETUTCDATE(), 101)) where claimid = @claimid;
	 exec addUpdateEntryToAuditLog @employeeID, @delegateID, 100, @claimid, '382d575a-ce76-45ae-847a-7d374383e383', @oldpaid, 1, @recordtitle, null;
	 exec addUpdateEntryToAuditLog @employeeID, @delegateID, 100, @claimid, 'a7ede2fb-27c1-4e76-bae4-bf6e30661e65', @olddatepaid, @newdatepaid, @recordtitle, null;
 END

 
 
END
