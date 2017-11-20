CREATE PROCEDURE [dbo].[saveTeam]
@teamid int,
@teamname nvarchar(50),
@description nvarchar(4000),
@teamleaderid int,
@employeeid int,
@subAccountID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @alreadyExists BIT;

	IF (@teamid = 0)
		BEGIN
			SET @alreadyExists = (SELECT COUNT(*) FROM teams WHERE teamname=@teamname and (subAccountId = @subAccountID OR subAccountId is null))
			IF @alreadyExists > 0
				RETURN -1;


			INSERT INTO teams (teamname, description, teamleaderid, subAccountId, CreatedOn, CreatedBy) VALUES (@teamname, @description, @teamleaderid, @subAccountID, getdate(), @employeeID);
			SET @teamid = scope_identity();
			
			if @CUemployeeID > 0
			BEGIN
				exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamid, @teamname, null;
			END
		END
	ELSE
		BEGIN
			SET @alreadyExists = (SELECT COUNT(*) FROM teams WHERE teamname=@teamname AND teamid <> @teamid and (subAccountId = @subAccountID OR subAccountId is null))
			IF @alreadyExists > 0
				RETURN -1;

			declare @oldteamname nvarchar(50);
			declare @olddescription nvarchar(4000);
			declare @oldteamleaderid int;
			select @oldteamname = teamname, @olddescription = description, @oldteamleaderid = teamleaderid from teams WHERE teamid=@teamid;

			UPDATE teams SET teamname=@teamname, description=@description, teamleaderid=@teamleaderid, ModifiedOn=getdate(), ModifiedBy=@employeeID WHERE teamid=@teamid;
		
			if @CUemployeeID > 0
			BEGIN
				if @oldteamname <> @teamname
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamid, '1422263f-882e-4b8d-bd10-b6a4e9267e61', @oldteamname, @teamname, @teamname, null;
				if @olddescription <> @description
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamid, 'cde72fb1-e31a-412e-87d7-83d841209a92', @olddescription, @description, @teamname, null;
				if @oldteamleaderid <> @teamleaderid
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 49, @teamid, '992a7faf-fe98-4a0a-82fd-7c3cf10dd0dd', @oldteamleaderid, @teamleaderid, @teamname, null;
			END
		END
RETURN @teamid;
