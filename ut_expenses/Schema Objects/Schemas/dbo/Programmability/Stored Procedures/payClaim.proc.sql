CREATE PROCEDURE [dbo].[payClaim] 
 -- Add the parameters for the stored procedure here
 @claimid int,
 @delegateID INT,
 @employeeID INT

AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 declare @oldpaid bit;
 declare @olddatepaid datetime;
 declare @newdatepaid datetime;
 set @newdatepaid = convert(DateTime, convert(varchar, GETDATE(), 101));
 declare @recordtitle nvarchar(2000);
 select @recordtitle = [name], @oldpaid = paid, @olddatepaid = datepaid from claims_base where claimid = @claimid;
 
 update claims_base set paid = 1, datepaid = convert(DateTime, convert(varchar, GETDATE(), 101)) where claimid = @claimid;
 exec addUpdateEntryToAuditLog @employeeID, @delegateID, 100, @claimid, '382d575a-ce76-45ae-847a-7d374383e383', @oldpaid, 1, @recordtitle, null;
 exec addUpdateEntryToAuditLog @employeeID, @delegateID, 100, @claimid, 'a7ede2fb-27c1-4e76-bae4-bf6e30661e65', @olddatepaid, @newdatepaid, @recordtitle, null;

 
END