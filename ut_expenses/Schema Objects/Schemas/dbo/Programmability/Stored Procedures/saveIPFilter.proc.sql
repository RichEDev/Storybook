CREATE PROCEDURE [dbo].[saveIPFilter] 
	@ipfilterid INT,
	@ipaddress NVARCHAR(50),
	@description NVARCHAR(4000),
	@active BIT,
	@userid INT,
	@date DateTime,
	@CUemployeeID INT,
	@CUdelegateID INT
AS
BEGIN
DECLARE @count int
	IF @ipfilterid > 0
		BEGIN												
			IF EXISTS (SELECT ipFilterID FROM ipfilters WHERE ipAddress = @ipaddress AND ipFilterID <> @ipfilterid)
				BEGIN
				
					RETURN -1;
				END
			ELSE
				BEGIN
					
					DECLARE @olddescription NVARCHAR(4000);
					DECLARE @oldipaddress NVARCHAR(50);
					DECLARE @oldactive BIT;
												
					SELECT @oldipaddress = ipAddress, @olddescription = [description], @oldactive = active FROM ipfilters WHERE ipFilterID = @ipfilterid;

					UPDATE ipfilters SET ipAddress = @ipaddress, [description] = @description, active = @active, modifiedon = @date, modifiedby = @userid WHERE ipFilterID = @ipfilterid;
					
					IF @CUemployeeID > 0
					BEGIN
						IF @oldipaddress <> @ipaddress
							exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 162, @ipfilterid, 'b7fa9156-eace-4576-a24b-8ff33ed2c63d', @oldipaddress, @ipaddress, @ipaddress, null;
						IF @olddescription <> @description
							exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 162, @ipfilterid, 'b9024e7d-5ef5-4667-b7c8-68c1912fe5ef', @olddescription, @description, @ipaddress, null;
						IF @oldactive <> @active
							exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 162, @ipfilterid, '01958ed5-cda1-4b39-b631-1174c833fa37', @oldactive, @active, @ipaddress, null;
					END
					
				END			
		END		
	ELSE
		BEGIN
		
			IF EXISTS (SELECT ipFilterID FROM ipfilters WHERE ipAddress = @ipaddress)
				BEGIN
					RETURN -1;
				END
			ELSE
				BEGIN															
					INSERT INTO ipfilters (ipAddress, [description], active, createdby, createdon) VALUES (@ipaddress, @description, @active, @userid, @date);
					SET @ipfilterid = SCOPE_IDENTITY();
					
					IF @CUemployeeID > 0
					BEGIN
						EXEC addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 162, @ipfilterid, @ipaddress, null;
					END
				END
		END
		
	RETURN @ipfilterid
END
