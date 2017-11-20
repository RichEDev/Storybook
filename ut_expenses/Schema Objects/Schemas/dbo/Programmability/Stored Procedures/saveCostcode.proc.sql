CREATE PROCEDURE [dbo].[saveCostcode]
@costcodeid int,
@costcode nvarchar(50),
@description nvarchar(4000),
@ownerEmployeeId int,
@ownerTeamId int,
@ownerBudgetHolderId int,
@date DateTime,
@userid INT,
@employeeID INT,
@delegateID INT

AS

-- check that only one of the owner variables is populated
if (isnull(@ownerEmployeeId,0)+isnull(@ownerTeamId,0)+isnull(@ownerBudgetHolderId,0) <> isnull(@ownerEmployeeId,0)) 
and (isnull(@ownerEmployeeId,0)+isnull(@ownerTeamId,0)+isnull(@ownerBudgetHolderId,0) <> isnull(@ownerTeamId,0)) 
and (isnull(@ownerEmployeeId,0)+isnull(@ownerTeamId,0)+isnull(@ownerBudgetHolderId,0) <> isnull(@ownerBudgetHolderId,0))
begin
	return -3;
end

DECLARE @count INT;

declare @usingdescription nvarchar(10)
select @usingdescription = stringValue from accountProperties where stringKey = 'useCostCodeDescription' and subAccountID in (select top 1 subAccountID from accountsSubAccounts)
if @costcodeid = 0
BEGIN
	SET @count = (SELECT COUNT(costcodeid) FROM costcodes WHERE costcode = @costcode);
	IF @count > 0
		RETURN -1;
		
	if @usingdescription = '1'
		begin
			set @count = (select count(costcodeid) from costcodes where description = @description)
			if @count > 0
				return -2
				
		end
		INSERT INTO costcodes (costcode, [description], OwnerEmployeeId, OwnerTeamId, OwnerBudgetHolderId, createdon, createdby) VALUES (@costcode, @description, @ownerEmployeeId, @ownerTeamId, @ownerBudgetHolderId, @date, @userid);
		set @costcodeid = scope_identity()
		
		if @employeeID > 0
		BEGIN
			EXEC addInsertEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, @costcode, null
		END
end
else
BEGIN
	SET @count = (SELECT COUNT(costcodeid) FROM costcodes WHERE costcode = @costcode AND costcodeid <> @costcodeid);
	IF @count > 0
		RETURN -1;

	if @usingdescription = '1'
		begin
			set @count = (select count(costcodeid) from costcodes where description = @description and costcodeid <> @costcodeid)
			if @count > 0
				return -2				
		end

	declare @oldCostcode nvarchar(50)
	declare @oldDescription nvarchar(4000)
	declare @oldOwnerEmployeeId int
	declare @oldOwnerTeamId int
	declare @oldOwnerBudgetHolderId int;
	declare @oldDesc nvarchar(100);
	declare @newDesc nvarchar(100);

	select @oldCostcode = costcode, @oldDescription = description, @oldOwnerEmployeeId = OwnerEmployeeId, @oldOwnerTeamId = OwnerTeamId, @oldOwnerBudgetHolderId = OwnerBudgetHolderId from costcodes where [costcodeid] = @costcodeid;	

	UPDATE costcodes SET costcode = @costcode, [description] = @description, OwnerEmployeeId = @ownerEmployeeId, OwnerTeamId = @ownerTeamId, OwnerBudgetHolderId = @ownerBudgetHolderId, [ModifiedOn] = @date, modifiedby = @userid WHERE [costcodeid] = @costcodeid
	
	if @employeeID > 0
	BEGIN
		if @costcode <> @oldCostcode
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, '359DFAC9-74E6-4BE5-949F-3FB224B1CBFC', @oldCostcode, @costcode, @costcode, null;
		if @description <> @oldDescription
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, 'AF80D035-6093-4721-8AFC-061424D2AB72', @oldDescription, @description, @costcode, null;
		if @ownerEmployeeId <> @oldOwnerEmployeeId
		begin
			select @newDesc = dbo.getEmployeeFullName(@ownerEmployeeId), @oldDesc = dbo.getEmployeeFullName(@oldOwnerEmployeeId);

			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, 'F6E81F6C-C186-4132-A080-A441E056B2C0', @oldDesc, @newDesc, @costcode, null;
		end
		if @ownerTeamId <> @oldOwnerTeamId
		begin
			select @newDesc = teamname from teams where teamid = @ownerTeamId;
			select @oldDesc = teamname from teams where teamid = @oldOwnerTeamId;

			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, '881F1629-7A29-4166-AFC8-E8E8D4B2FF1A', @oldDesc, @newDesc, @costcode, null;
		end
		if @ownerBudgetHolderId <> @oldOwnerBudgetHolderId
		begin
			select @newDesc = dbo.getEmployeeFullName(employees.employeeid) from budgetholders inner join employees on budgetholders.employeeid = employees.employeeid where budgetholderid = @ownerBudgetHolderId;
			select @oldDesc = dbo.getEmployeeFullName(employees.employeeid) from budgetholders inner join employees on budgetholders.employeeid = employees.employeeid where budgetholderid = @oldOwnerBudgetHolderId;

			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, '024944BD-45D3-42B0-B946-55C1A524F47A', @oldDesc, @newDesc, @costcode, null;
		end
	END
end

return @costcodeid
