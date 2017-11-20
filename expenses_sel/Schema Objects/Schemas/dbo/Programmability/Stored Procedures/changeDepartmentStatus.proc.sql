



CREATE PROCEDURE [dbo].[changeDepartmentStatus]
@departmentid INT,
@archive bit,
@CUemployeeID INT,
@CUdelegateID INT

AS

declare @recordTitle nvarchar(2000);
declare @oldarchive bit;
select @recordtitle = department, @oldarchive = archived from departments where departmentid = @departmentid;

update departments set archived = @archive where departmentid = @departmentid;

if @oldarchive <> @archive
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 2, @departmentid, '03bb1843-a231-4be7-b564-1b813d6a5988', @oldarchive, @archive, @recordtitle, null;
