CREATE  PROCEDURE [dbo].[saveAudienceBudgetHolder]
@audienceID int,
@budgetHolderID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
	BEGIN
		declare @title1 nvarchar(500);
		select @title1 = audienceName from audiences where audienceID = @audienceID;

		DECLARE @audienceBudgetHolderID int = -1;
		
		declare @recordTitle nvarchar(2000);
		set @recordTitle = (select 'Audience Budget Holder for ' + @title1);
		IF NOT EXISTS (SELECT budgetHolderID FROM audienceBudgetHolders WHERE audienceID = @audienceID AND budgetHolderID = @budgetHolderID)
		BEGIN
			INSERT INTO audienceBudgetHolders (audienceID, budgetHolderID) VALUES (@audienceID, @budgetHolderID);		
			SET @audienceBudgetHolderID = scope_identity();
			
			UPDATE audiences SET modifiedOn = getdate() WHERE audienceID = @audienceID;

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceBudgetHolderID, @recordTitle, null;
		END
		ELSE
		BEGIN
			SELECT @audienceBudgetHolderID = audienceBudgetHolderID FROM audienceBudgetHolders WHERE audienceID = @audienceID AND budgetHolderID = @budgetHolderID
		END
	END
RETURN @audienceBudgetHolderID;
