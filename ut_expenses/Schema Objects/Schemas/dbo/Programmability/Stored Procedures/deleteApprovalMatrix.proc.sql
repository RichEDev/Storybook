CREATE PROCEDURE [dbo].[deleteApprovalMatrix]
	@approvalMatrixId int,
	@auditEmployeeId int,
	@auditDelegateId int
AS
	DECLARE @matrixName nvarchar(2000) = (SELECT [name] FROM [approvalMatrices] WHERE [approvalMatrixId] = @approvalmatrixId);
	
	-- check for use in signoff groups
	IF EXISTS (select [signoffid] from [signoffs] where ([signofftype] = 6 OR [signofftype] = 7) and [relid] = @approvalMatrixId)
	BEGIN
		RETURN -1;
	END
	
	DELETE FROM [approvalMatrices] WHERE [approvalMatrixId] = @approvalMatrixId;
	
	exec addDeleteEntryToAuditLog @auditEmployeeId, @auditDelegateId, 176, @approvalMatrixId, @matrixName, null;
	
RETURN 0;