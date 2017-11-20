



CREATE PROCEDURE [dbo].[changeProjectcodeStatus]
@projectcodeid INT,
@archive bit,
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @count INT;

declare @oldarchive bit;
declare @recordtitle nvarchar(2000);
select @oldarchive = archived, @recordtitle = projectcode from project_codes where projectcodeid = @projectcodeid;

update project_codes set archived = @archive where projectcodeid = @projectcodeid;

if @oldarchive <> @archive
	exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 3, @projectcodeid, '7b406750-adbd-461f-9d36-97dbdbd8f451', @oldarchive, @archive, @recordtitle, null;
