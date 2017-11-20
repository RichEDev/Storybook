CREATE PROCEDURE [dbo].[saveElementDocAttach] 

@id INT,
@attachmentID INT,
@docType TINYINT,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	      
	DECLARE @oldattachmentID INT;
	DECLARE @recordtitle NVARCHAR(2000);
	DECLARE @attachmentFileName NVARCHAR(255);
	DECLARE @attachmentOldFileName NVARCHAR(255);

	SELECT @attachmentFileName = [Filename] FROM [cars_attachments] WHERE attachmentID = @attachmentID
		           
    IF @docType = 1
		 BEGIN		      
			SELECT @recordtitle = username, @oldattachmentID = licenceAttachID FROM employees WHERE employeeid = @id;
			IF @oldattachmentID > 0
				BEGIN
					SELECT @attachmentOldFileName = [fileName] FROM  [cars_attachments] WHERE attachmentID = @oldattachmentID  
					EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, 'B1E54296-A73F-4535-A8D8-88E97E8A8F6F',@attachmentOldFileName, '', @recordtitle, null;
			    END
			ELSE
				BEGIN
					EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, 'B1E54296-A73F-4535-A8D8-88E97E8A8F6F','', @attachmentFileName, @recordtitle, null;
				END
	     UPDATE employees SET licenceAttachID = @attachmentID WHERE employeeid = @id;		
	END	   
	      
	IF @docType = 2
		 BEGIN
			SELECT @recordtitle = registration, @oldattachmentID = taxAttachID FROM cars WHERE carid = @id;	
			IF @oldattachmentID > 0
				Begin
					SELECT @attachmentOldFileName = fileName FROM [cars_attachments] WHERE attachmentID = @oldattachmentID      
					EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97,@id, '3DF4E0D3-C57A-4B0F-B644-59FFE31E066B', @attachmentOldFileName, '', @recordtitle, null;         
				END			
			ELSE
				BEGIN		
					EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97,@id, '3DF4E0D3-C57A-4B0F-B644-59FFE31E066B', '', @attachmentFileName, @recordtitle, null;         
				END
		  UPDATE cars SET taxAttachID = @attachmentID WHERE carid = @id; 	  
	 END

	IF @doctype = 3
		  BEGIN
			 SELECT @recordtitle = registration, @oldattachmentID = MOTAttachID FROM cars WHERE carid = @id;
			 IF @oldattachmentID > 0
				BEGIN 
					SELECT @attachmentOldFileName = fileName FROM [cars_attachments] WHERE attachmentID = @oldattachmentID   
    				EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, '4C1A0253-0175-4698-BA7A-68FDFA1C47AF', @attachmentOldFileName, '', @recordtitle, null;     
				END
		    ELSE
				BEGIN
					EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, '4C1A0253-0175-4698-BA7A-68FDFA1C47AF', '', @attachmentFileName, @recordtitle, null;     
				END
		  UPDATE cars SET MOTAttachID = @attachmentID WHERE carid = @id;        		
	END
	      
	IF @doctype = 4
		  BEGIN
			  select @recordtitle = registration, @oldattachmentID = insuranceAttachID FROM cars WHERE carid = @id;
			  if @oldattachmentID > 0
				BEGIN 
					select @attachmentOldFileName = fileName FROM  [cars_attachments] where attachmentID = @oldattachmentID   
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, 'DA8DE77E-48AE-4EF3-A18F-C862EDBF6837', @attachmentOldFileName, '', @recordtitle, null;
				END
			  ELSE
				BEGIN
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, 'DA8DE77E-48AE-4EF3-A18F-C862EDBF6837', '', @attachmentFileName, @recordtitle, null;    
				END
				UPDATE cars SET insuranceAttachID = @attachmentID WHERE carid = @id;
	END

	IF @doctype = 5
		  BEGIN
			 select @recordtitle = registration, @oldattachmentID = serviceAttachID FROM cars WHERE carid = @id;
			 IF @oldattachmentID > 0
				 BEGIN 
					 SELECT @attachmentOldFileName = fileName FROM  [cars_attachments] WHERE attachmentID = @oldattachmentID   
        			 EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, '0CC036AB-1B8D-4881-89FC-EAFBDA9AAAD6', @attachmentOldFileName, '', @recordtitle, null;
				 END
			 ELSE
				 BEGIN
         			 EXEC addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 97, @id, '0CC036AB-1B8D-4881-89FC-EAFBDA9AAAD6', '', @attachmentFileName, @recordtitle, null;
				 END
			UPDATE cars SET serviceAttachID = @attachmentID WHERE carid = @id;         
		  END    
	END
GO