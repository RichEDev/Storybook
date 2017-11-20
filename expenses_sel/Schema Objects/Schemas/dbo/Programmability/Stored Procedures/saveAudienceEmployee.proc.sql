CREATE PROCEDURE [dbo].[saveAudienceEmployee]
@audienceID int,
@employeeID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
	BEGIN
		declare @title1 nvarchar(500);
		select @title1 = audienceName from audiences where audienceID = @audienceID;

		DECLARE @audienceEmployeeID int = -1;	
		
		declare @recordTitle nvarchar(2000);
		set @recordTitle = (select 'Audience Employee for ' + @title1);
		
		IF NOT EXISTS (SELECT employeeID FROM audienceEmployees WHERE audienceID = @audienceID AND employeeID = @employeeID)
		BEGIN
			INSERT INTO audienceEmployees (audienceID, employeeID) VALUES (@audienceID, @employeeID);		
			SET @audienceEmployeeID = scope_identity();

			UPDATE audiences SET modifiedOn = getdate() WHERE audienceID = @audienceID;

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceEmployeeID, @recordTitle, null;
		END
		ELSE
		BEGIN
			SELECT @audienceEmployeeID = audienceEmployeeID FROM audienceEmployees WHERE audienceID = @audienceID AND employeeID = @employeeID
		END
	END
RETURN @audienceEmployeeID;
