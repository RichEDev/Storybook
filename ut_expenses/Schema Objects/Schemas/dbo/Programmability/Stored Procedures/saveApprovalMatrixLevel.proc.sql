CREATE PROCEDURE [dbo].[saveApprovalMatrixLevel]
 @approvalmatrixid int,
 @approvalmatrixlevelid int,
 @approvalLimit money,
 @approverBudgetHolderId int,
 @approverEmployeeId int,
 @approverTeamId int,
 @employeeID int,
 @delegateID int
AS
DECLARE @matrixName nvarchar(2000) = (SELECT [name] + ' level' FROM [approvalMatrices] WHERE [approvalMatrixId] = @approvalmatrixId);

IF @approvalmatrixlevelid = 0
 BEGIN
  IF EXISTS (SELECT [approvalMatrixLevelId] FROM [approvalMatrixLevels] WHERE [approvalLimit] = @approvalLimit AND [approvalMatrixId] = @approvalmatrixid)
  BEGIN
   RETURN -1;
  END

  INSERT INTO [approvalMatrixLevels] ([approvalMatrixId], [approvalLimit], [approverEmployeeId], [approverTeamId], [approverBudgetHolderId], [createdOn], [createdBy])
   VALUES (@approvalmatrixid, @approvalLimit, @approverEmployeeId, @approverTeamId, @approverBudgetHolderId, GETUTCDATE(), @employeeID);
  SET @approvalmatrixlevelid = SCOPE_IDENTITY();
  
  exec addInsertEntryToAuditLog @employeeID, @delegateID, 176, @approvalmatrixlevelid, @matrixName, null;
 END
ELSE
 BEGIN
  IF EXISTS (SELECT [approvalMatrixLevelId] FROM [approvalMatrixLevels] WHERE [approvalLimit] = @approvalLimit AND [approvalMatrixId] = @approvalmatrixid AND [approvalMatrixLevelId] <> @approvalmatrixlevelid)
  BEGIN
   RETURN -1;
  END

  declare @oldApprovalLimit money;
  declare @oldApproverEmployeeId int;
  declare @oldApproverTeamId int;
  declare @oldApproverBudgetHolderId int;

  SELECT @oldApprovalLimit = approvalLimit,
    @oldApproverEmployeeId = approverEmployeeId,
    @oldApproverTeamId = approverTeamId,
    @oldApproverBudgetHolderId = approverBudgetHolderId
   FROM [approvalMatrixLevels] WHERE [approvalmatrixid] = @approvalmatrixid AND [approvalmatrixlevelid] = @approvalmatrixlevelid

  UPDATE [approvalMatrixLevels] SET [approvalLimit] = @approvalLimit,
          [approverEmployeeId] = @approverEmployeeId,
          [approverTeamId] = @approverTeamId,
          [approverBudgetHolderId] = @approverBudgetHolderId,
          [modifiedOn] = GETUTCDATE(),
          [modifiedBy] = @employeeID
         WHERE [approvalmatrixid] = @approvalmatrixid AND [approvalmatrixlevelid] = @approvalmatrixlevelid
         
  if @oldApprovalLimit <> @approvalLimit
   exec addUpdateEntryToAuditLog @employeeID, @delegateID, 176, @approvalmatrixlevelid, '9C49A645-FF12-4AEE-83B3-6994C370FA16', @oldApprovalLimit, @approvalLimit, @matrixName, null;
  if ISNULL(@oldApproverBudgetHolderId, -1) <> ISNULL(@approverBudgetHolderId, -1)
   exec addUpdateEntryToAuditLog @employeeID, @delegateID, 176, @approvalmatrixlevelid, 'A78B1C6F-F093-4D31-8E1B-54235ECCD54F', @oldApproverBudgetHolderId, @approverBudgetHolderId, @matrixName, null;
  if ISNULL(@oldApproverEmployeeId, -1) <> ISNULL(@approverEmployeeId, -1)
   exec addUpdateEntryToAuditLog @employeeID, @delegateID, 176, @approvalmatrixlevelid, '62B315F8-773E-4D83-82E5-1CDC305F2A9A', @oldApproverEmployeeId, @approverEmployeeId, @matrixName, null;
  if ISNULL(@oldApproverTeamId, -1) <> ISNULL(@approverTeamId, -1)
   exec addUpdateEntryToAuditLog @employeeID, @delegateID, 176, @approvalmatrixlevelid, '1FC1252C-B1E1-4F5E-BDDC-5C13AC353F6E', @oldApproverTeamId, @approverTeamId, @matrixName, null;
 END
  
UPDATE [approvalMatrices] SET [CacheExpiry] = GETUTCDATE() WHERE [approvalMatrixId] = @approvalmatrixid;

return @approvalmatrixlevelid;