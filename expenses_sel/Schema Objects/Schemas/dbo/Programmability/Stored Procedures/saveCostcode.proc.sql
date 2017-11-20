CREATE PROCEDURE [dbo].[saveCostcode]
@costcodeid int,
@costcode nvarchar(50),
@description nvarchar(4000),
@date DateTime,
@userid INT,
@employeeID INT,
@delegateID INT

AS

DECLARE @count INT;

declare @usingdescription nvarchar(10)
select @usingdescription = stringValue from accountProperties where stringKey = 'useCostCodeDescription' and subAccountID in (select top 1 subAccountID from accountsSubAccounts)
if @costcodeid = 0
BEGIN
	SET @count = (SELECT COUNT(*) FROM costcodes WHERE costcode = @costcode);
	IF @count > 0
		RETURN -1;
		
	if @usingdescription = '1'
		begin
			set @count = (select count(*) from costcodes where description = @description)
			if @count > 0
				return -2
				
		end
		INSERT INTO costcodes (costcode, [description], createdon, createdby) VALUES (@costcode, @description, @date, @userid);
		set @costcodeid = scope_identity()
		
		if @employeeID > 0
		BEGIN
			EXEC addInsertEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, @costcode, null
		END
end
else
BEGIN

	SET @count = (SELECT COUNT(*) FROM costcodes WHERE costcode = @costcode AND costcodeid <> @costcodeid);
	IF @count > 0
		RETURN -1;
	if @usingdescription = '1'
		begin
			set @count = (select count(*) from costcodes where description = @description and costcodeid <> @costcodeid)
			if @count > 0
				return -2
				
		end

	declare @oldCostcode nvarchar(50)
	declare @oldDescription nvarchar(4000)
	
	select @oldCostcode = costcode, @oldDescription = description from costcodes where [costcodeid] = @costcodeid;
	

	UPDATE costcodes SET costcode = @costcode, [description] = @description, [ModifiedOn] = @date, modifiedby = @userid WHERE [costcodeid] = @costcodeid
	
	if @employeeID > 0
	BEGIN
		if @costcode <> @oldCostcode
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, '359DFAC9-74E6-4BE5-949F-3FB224B1CBFC', @oldCostcode, @costcode, @costcode, null;
		if @description <> @oldDescription
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, 'AF80D035-6093-4721-8AFC-061424D2AB72', @oldDescription, @description, @costcode, null;
	END
end

return @costcodeid
