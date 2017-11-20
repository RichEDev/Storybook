CREATE PROCEDURE [dbo].[saveApprovalMatrix]
 @approvalmatrixid int,
 @name nvarchar(250),
 @description nvarchar(max),
 @defaultApproverBudgetHolderId int,
 @defaultApproverEmployeeId int,
 @defaultApproverTeamId int,
 @employeeid int,
 @delegateid int
AS
 
 IF @approvalmatrixid = 0
  BEGIN
   IF EXISTS (SELECT [approvalMatrixId] FROM [approvalMatrices] WHERE [name] = @name)
    BEGIN
     RETURN -1;
    END

   INSERT INTO [approvalMatrices] ([name], [description], [defaultApproverBudgetHolderId], [defaultApproverEmployeeId], [defaultApproverTeamId], [createdBy], [createdOn], [cacheExpiry]) VALUES (@name, @description, @defaultApproverBudgetHolderId, @defaultApproverEmployeeId, @defaultApproverTeamId, @employeeid, GETUTCDATE(), GETUTCDATE())
   SET @approvalmatrixid = SCOPE_IDENTITY();
   
   EXEC addInsertEntryToAuditLog @employeeid, @delegateid, 176, @approvalmatrixid, @name, null;
   
   RETURN @approvalmatrixid;
  END
 ELSE
  BEGIN
   IF EXISTS (SELECT [approvalMatrixId] FROM [approvalMatrices] WHERE [name] = @name AND [approvalMatrixId] <> @approvalmatrixid)
    BEGIN
     RETURN -1;
    END

   DECLARE @oldName NVARCHAR(250);
   DECLARE @oldDescription NVARCHAR(MAX);
   DECLARE @oldDefaultApproverBudgetHolderId INT;
   DECLARE @oldDefaultApproverEmployeeId INT;
   DECLARE @oldDefaultApproverTeamId INT;
   
   SELECT @oldName = [name], @oldDescription = [description], @oldDefaultApproverBudgetHolderId = [defaultApproverBudgetHolderId], @oldDefaultApproverEmployeeId = [defaultApproverEmployeeId], @oldDefaultApproverTeamId = [defaultApproverTeamId] FROM [approvalMatrices] WHERE [approvalMatrixId] = @approvalmatrixid;
  
   UPDATE [approvalMatrices]
    SET [name] = @name, [description] = @description, [defaultApproverBudgetHolderId] = @defaultApproverBudgetHolderId, [defaultApproverEmployeeId] = @defaultApproverEmployeeId, [defaultApproverTeamId] = @defaultApproverTeamId, [modifiedBy] = @employeeid, [modifiedOn] = GETUTCDATE(), [cacheExpiry] = GETUTCDATE() 
    WHERE [approvalMatrixId] = @approvalmatrixid;
   
   IF @oldName <> @name
    EXEC addUpdateEntryToAuditLog @employeeid, @delegateid, 176, @approvalmatrixid, '10FAA46E-DFD5-4165-8B48-B87FE4DE489D', @oldName, @name, @name, null;
   IF @oldDescription <> @description
    EXEC addUpdateEntryToAuditLog @employeeid, @delegateid, 176, @approvalmatrixid, 'D5D3784A-46DE-413F-AD44-E4B14A42072C', @oldDescription, @description, @name, null;
   IF ISNULL(@oldDefaultApproverBudgetHolderId, -1)  <> ISNULL(@defaultApproverBudgetHolderId , -1)
   		EXEC addUpdateEntryToAuditLog @employeeid, @delegateid, 176, @approvalmatrixid, '9FAFFC0C-27E1-4910-A51D-AACC02FE3A5A',  @oldDefaultApproverBudgetHolderId, @defaultApproverBudgetHolderId, @name, null;
	IF ISNULL(@oldDefaultApproverEmployeeId, -1) <> ISNULL(@defaultApproverEmployeeId, -1)
    	EXEC addUpdateEntryToAuditLog @employeeid, @delegateid, 176, @approvalmatrixid, 'AA538138-BD92-4922-9946-3ACDD0E737F5', @oldDefaultApproverEmployeeId, @defaultApproverEmployeeId, @name, null;
   IF ISNULL(@oldDefaultApproverTeamId, -1) <> ISNULL(@defaultApproverTeamId, -1)
		EXEC addUpdateEntryToAuditLog @employeeid, @delegateid, 176, @approvalmatrixid, 'BA7F712D-8DC2-4279-ADDD-A978F9C1203C', @oldDefaultApproverTeamId, @defaultApproverTeamId, @name, null;
   RETURN @approvalmatrixid;
  END

RETURN 0