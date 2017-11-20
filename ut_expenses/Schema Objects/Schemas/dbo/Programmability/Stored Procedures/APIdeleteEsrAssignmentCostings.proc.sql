CREATE PROCEDURE [dbo].[APIdeleteEsrAssignmentCostings]
	@ESRCostingAllocationId bigint 
	
AS
BEGIN
	DECLARE @costcodeid int = 0;
	DECLARE @costcode nvarchar(50);
	SELECT @costcode = CostCentre FROM ESRAssignmentCostings WHERE ESRCostingAllocationId = @ESRCostingAllocationId;
	SELECT @costcodeid = COSTCODEID FROM COSTCODES WHERE COSTCODE = @costcode;
	DELETE FROM ESRAssignmentCostings WHERE [ESRCostingAllocationId] = @ESRCostingAllocationId
	IF NOT EXISTS (SELECT ESRAssignmentId FROM ESRAssignmentCostings WHERE CostCentre = @costcode)
	BEGIN
		UPDATE costcodes SET archived = 1 WHERE costcodeid = @costcodeid;
	END
END