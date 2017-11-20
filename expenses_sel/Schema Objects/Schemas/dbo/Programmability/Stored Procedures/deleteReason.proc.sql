

CREATE PROCEDURE [dbo].[deleteReason]
@reasonid int,
@employeeID INT,
@delegateID INT
 AS

	declare @recordTitle nvarchar(2000);
select @recordTitle = reason from reasons where reasonid = @reasonid;

update savedexpenses_current set reasonid = null where reasonid = @reasonid;
update savedexpenses_previous set reasonid = null where reasonid = @reasonid;
delete from reasons where reasonid = @reasonid;

exec addDeleteEntryToAuditLog @employeeID, @delegateID, 46, @reasonid, @recordTitle, null;
