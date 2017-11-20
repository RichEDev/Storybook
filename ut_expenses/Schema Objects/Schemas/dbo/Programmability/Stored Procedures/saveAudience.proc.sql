CREATE PROCEDURE [dbo].[saveAudience]
@audienceID int,
@audienceName nvarchar(250),
@description nvarchar(MAX),
@employeeID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	IF (@audienceID = 0)
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM audiences WHERE audienceName = @audienceName)
			BEGIN
				INSERT INTO audiences (audienceName, description, createdOn, createdBy) VALUES (@audienceName, @description, getdate(), @employeeID);
				SET @audienceID = scope_identity();

				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceID, @audienceName, null;
			END
			ELSE
				SET @audienceID = -1;
		END
	ELSE
		BEGIN
			declare @oldaudienceName nvarchar(250);
			declare @olddescription nvarchar(MAX);
			select @oldaudienceName = audienceName, @olddescription = description from audiences WHERE audienceID=@audienceID;

			UPDATE audiences SET audienceName=@audienceName, description=@description, modifiedOn=getdate(), modifiedBy=@employeeID WHERE audienceID=@audienceID;

			if @oldaudienceName <> @audienceName
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceID, '22d141e3-dca2-4e2a-9cd8-503f95725648', @oldaudienceName, @audienceName, @audienceName, null;
			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceID, '0b6ddebd-d7af-423a-9b9c-5832eb20e2f8', @olddescription, @description, @audienceName, null;
		END
RETURN @audienceID;
