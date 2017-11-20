CREATE PROCEDURE [dbo].[saveDepartment] 
	@departmentid INT,
	@department NVARCHAR(50),
	@description NVARCHAR(4000),
	@userid INT,
	@date DateTime,
@CUemployeeID INT,
@CUdelegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @usingdescription nvarchar(10)
select @usingdescription = stringValue from accountProperties where stringKey = 'useDepartmentCodeDescription' and subAccountID in (select top 1 subAccountID from accountsSubAccounts)

DECLARE @count int
    IF @departmentid > 0
		BEGIN
			SET @count = (SELECT COUNT(*) FROM [departments] WHERE departmentid <> @departmentid AND department = @department);
			IF @count > 0
				RETURN -1;
		if @usingdescription = '1'
		begin
			set @count = (select count(*) from departments where description = @description)
			if @count > 0
				return -2
				
		end
			declare @olddepartment NVARCHAR(50);
			declare @olddescription NVARCHAR(4000);
			select @olddepartment = department, @olddescription = [description] from departments where departmentid = @departmentid;

			update departments set department = @department, [description] = @description, modifiedon = @date, modifiedby = @userid where departmentid = @departmentid;
			
			if @CUemployeeID > 0
			BEGIN
				if @olddepartment <> @department
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 2, @departmentid, '9617a83e-6621-4b73-b787-193110511c17', @olddepartment, @department, @department, null;
				if @olddescription <> @description
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 2, @departmentid, '990fd383-14f8-4f50-a2e2-13a9d1f847b7', @olddescription, @description, @department, null;
			END
		END
	ELSE
		BEGIN
			SET @count = (SELECT COUNT(*) FROM [departments] WHERE department = @department);
			IF @count > 0
				RETURN -1;
				if @usingdescription = '1'
		begin
			set @count = (select count(*) from departments where description = @description and departmentid <> @departmentid)
			if @count > 0
				return -2
				
		end
			insert into departments (department, [description], createdby, createdon) values (@department, @description, @userid, @date);
			SET @departmentid = SCOPE_IDENTITY();
			
			if @CUemployeeID > 0
			BEGIN
				EXEC addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 2, @departmentid, @department, null;
			END
		END
	
	
	RETURN @departmentid
END
