




CREATE PROCEDURE [dbo].[saveEmailTemplate] 
@emailtemplateid int,
@templatename nvarchar(100), 
@subject nvarchar(500),
@bodyhtml ntext,
@priority tinyint,
@basetableid uniqueidentifier,
@systemtemplate bit,
@date DateTime,
@userid int,
@CUemployeeID INT,
@CUdelegateID INT

AS
BEGIN
if @emailtemplateid = 0
begin
	insert into emailTemplates (templatename, [subject], bodyhtml, priority, basetableid, systemtemplate, createdon, createdby)
	values (@templatename, @subject, @bodyhtml, @priority, @basetableid, @systemtemplate, @date, @userid);
	set @emailtemplateid = scope_identity();

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, @templatename, null;
end
else
begin
	declare @oldtemplatename nvarchar(100);
	declare @oldsubject nvarchar(500);
	--declare @oldbodyhtml ntext; @oldbodyhtml = bodyhtml,
	declare @oldpriority tinyint;
	declare @oldbasetableid uniqueidentifier;
	declare @oldsystemtemplate bit;
	select @oldtemplatename = templatename, @oldsubject = [subject], @oldpriority = priority, @oldbasetableid = basetableid, @oldsystemtemplate = systemtemplate from emailTemplates where emailtemplateid = @emailtemplateid;

	update emailTemplates 
	set 
	templatename = @templatename,
	[subject] = @subject,
	bodyhtml = @bodyhtml,
	priority = @priority,
	basetableid = @basetableid,
	systemtemplate = @systemtemplate,
	modifiedon = @date,
	modifiedby = @userid
	where emailtemplateid = @emailtemplateid;

	if @oldtemplatename <> @templatename
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, '6552c033-8375-43c7-bb4c-a912338a3f21', @oldtemplatename, @templatename, @templatename, null;
	if @oldsubject <> @subject
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, '36b5dc4b-5921-4e75-978f-24b9379b992a', @oldsubject, @subject, @templatename, null;
	--if @oldbodyhtml <> @bodyhtml
		--exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, '5dc3fb05-7a79-400d-8a7e-78e96ae6f811', @oldbodyhtml, @bodyhtml, @templatename, null;
	if @oldpriority <> @priority
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, null, @oldpriority, @priority, @templatename, null;
	if @oldbasetableid <> @basetableid
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, '3bd34be8-59af-4b1c-bb00-cd5abf9382ee', @oldbasetableid, @basetableid, @templatename, null;
	if @oldsystemtemplate <> @systemtemplate
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 22, @emailtemplateid, 'cc9e6a54-b92b-42ba-80c0-46057ad71d3f', @oldsystemtemplate, @systemtemplate, @templatename, null;

end

return @emailtemplateid
END






 
