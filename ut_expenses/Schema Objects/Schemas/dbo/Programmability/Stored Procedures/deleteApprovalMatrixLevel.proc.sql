CREATE PROCEDURE [dbo].[deleteApprovalMatrixLevel]
	@approvalmatrixId int,
	@approvalmatrixlevelId int,
	@auditEmployeeId int,
	@auditDelegateId int
AS
	DECLARE @matrixName nvarchar(2000) = (SELECT [name] + ' level' FROM [approvalMatrices] WHERE [approvalMatrixId] = @approvalmatrixId);
	
	DELETE FROM [approvalMatrixLevels] WHERE [approvalMatrixId] = @approvalmatrixId AND [approvalMatrixLevelId] = @approvalmatrixlevelId;
	
	exec addDeleteEntryToAuditLog @auditEmployeeId, @auditDelegateId, 176, @approvalmatrixlevelId, @matrixName, null;
	
RETURN 0;