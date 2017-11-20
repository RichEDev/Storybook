


CREATE PROCEDURE [dbo].[saveElementDocAttach] 

@id int,
@attachmentID int,
@docType tinyint,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	
	declare @oldattachmentID int;
	declare @recordtitle nvarchar(2000);

	if @docType = 1
	BEGIN
		select @recordtitle = username, @oldattachmentID = licenceAttachID from employees WHERE employeeid = @id;
		UPDATE employees SET licenceAttachID = @attachmentID WHERE employeeid = @id;
		if @oldattachmentID <> @attachmentID
			begin
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, 'eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf', @oldattachmentID, @attachmentID, @recordtitle, null;
			end
	END	
	
	if @docType = 2
	BEGIN
		select @recordtitle = registration, @oldattachmentID = taxAttachID from cars WHERE carid = @id;
		UPDATE cars SET taxAttachID = @attachmentID WHERE carid = @id;
		if @oldattachmentID <> @attachmentID
			begin
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, null, @oldattachmentID, @attachmentID, @recordtitle, null;
			end
	END

	if @doctype = 3
	BEGIN
		select @recordtitle = registration, @oldattachmentID = MOTAttachID from cars WHERE carid = @id;
		UPDATE cars SET MOTAttachID = @attachmentID WHERE carid = @id;
		if @oldattachmentID <> @attachmentID
			begin
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, null, @oldattachmentID, @attachmentID, @recordtitle, null;
			end
	END
	
	if @doctype = 4
	BEGIN
		select @recordtitle = registration, @oldattachmentID = insuranceAttachID from cars WHERE carid = @id;
		UPDATE cars SET insuranceAttachID = @attachmentID WHERE carid = @id;
		if @oldattachmentID <> @attachmentID
			begin
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, null, @oldattachmentID, @attachmentID, @recordtitle, null;
			end
	END

	if @doctype = 5
	BEGIN
		select @recordtitle = registration, @oldattachmentID = serviceAttachID from cars WHERE carid = @id;
		UPDATE cars SET serviceAttachID = @attachmentID WHERE carid = @id;
		if @oldattachmentID <> @attachmentID
			begin
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, null, @oldattachmentID, @attachmentID, @recordtitle, null;
			end
	END
	
--	if @doctype = 6
--	BEGIN
--		UPDATE savedexpenses_current SET receiptAttachID = @attachmentID WHERE expenseid = @id
--	END
END






 
